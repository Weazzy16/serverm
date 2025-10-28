// serverside/JackpotGame.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;

using NeptuneEvo.Handles;        // ExtPlayer, IsCharacterData(), GetCharacterData()
using NeptuneEvo.Accounts;       // UpdateData.RedBucks(...)
using NeptuneEvo.Chars;          // BattlePass.Repository.UpdateReward(...)
using NeptuneEvo.Functions;      // Notify, LangFunc
using NeptuneEvo.Database;       // MySQL.QueryAsync / QueryRead
using NeptuneEvo.Character;      // Trigger.SetTask
using Redage.SDK;

namespace NeptuneEvo.MiniGames
{
    /// <summary>
    /// Мини-игра JACKPOT.
    /// Потокобезопасность: любой вызов GTA-API (NAPI, Trigger.ClientEvent и т.п.) — только из главного потока через NAPI.Task.Run.
    /// Работа с БД — через Trigger.SetTask (внутри можно синхронно дергать MySQL.QueryRead/QueryAsync).
    /// </summary>
    public class JackpotGame : Script
    {
        // ===== внутренняя модель ставки =====
        private class JackpotBet
        {
            public int UUID;
            public string Login;
            public int Amount;
            public string Gender;
            public string ServerName;
            public int ServerId;
            public int Ticket;
            public bool Paid;
        }

        // Кеш последней истории
        private class HistoryGame
        {
            public int Id;
            public string WinnerLogin;
            public string WinnerGender;
            public int WinnerAccountId;
            public string WinnerServerName;
            public int WinnerServerId;
            public int WinnerAmount;
            public int Bank;
            public double WinnerChance;
            public int WinnerTicket;
            public int WinnerCardIndex;
            public double WinnerPosition;
        }

        // Специальные достижения
        private class SpecialAchievement
        {
            public string WinnerLogin;
            public string WinnerGender;
            public int WinnerAccountId;
            public string ServerName;
            public int ServerId;
            public double WinnerChance;
            public int Bank;
        }

        // ДОБАВЬТЕ ЭТОТ КЛАСС:
        private class WinnerData
        {
            public int UUID;
            public string Login;
            public string Gender;
            public int AccountId;
            public int ServerId;
            public string ServerName;
            public int BetAmount;
            public int WinCoins;
            public double WinChance;
            public int Ticket;
            public int WinnerCardIndex;
            public double WinnerPosition;
        }

        private const int HISTORY_CACHE_LIMIT = 100;
        private static readonly Dictionary<int, int> PlayerPages = new Dictionary<int, int>();
        private static readonly List<HistoryGame> HistoryList = new List<HistoryGame>();

        // ===== состояние цикла =====
        private static readonly object Sync = new object();
        private static readonly List<JackpotBet> CurrentBets = new List<JackpotBet>();
        private static bool TimerActive;
        private static DateTime TimerStartAt;
        private static DateTime TimerCreatedAt;
        private static int TimerInitTimeMs = 0;
        private static bool GameActive;
        private static DateTime GameStartAt;

        // ИЗМЕНИТЕ ТИП Winner на WinnerData:
        private static WinnerData Winner;          // победитель (был JackpotBet)

        private static int GameTime = 8000;
        private static int WinnerCardIndex = 0;
        private static double WinnerPosition = 0.5;
        private static int CurrentId = 1;
        private static System.Timers.Timer BroadcastLoop;
        private static readonly Random Rnd = new Random();
        private const int WAIT_BET_MS = 15000;

        // ДОБАВЬТЕ эту переменную если её нет:
        private static DateTime GameStartTime = DateTime.UtcNow;

        // ИЗМЕНИТЕ константу:
        private const int TIME_GAME_MS = 8000;  // вместо GameTime

        // Специальные достижения
        private static SpecialAchievement LuckOfDay = null;
        private static SpecialAchievement BigJackpot = null;
        private static HistoryGame LastGame = null;

        // ===== жизненный цикл ресурса =====
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            NAPI.Util.ConsoleOutput("[Jackpot] resource start");
            Init();
        }

        [ServerEvent(Event.ResourceStop)]
        public void OnResourceStop()
        {
            StopLoop();
        }

        public static void Init()
        {
            lock (Sync)
            {
                CurrentBets.Clear();
                GameActive = false;
                TimerActive = false; // НЕ запускаем таймер
                Winner = null;
                WinnerCardIndex = 0;
                WinnerPosition = 0.5;
            }

            // Читаем данные из БД в фоне
            Trigger.SetTask(() =>
            {
                int lastId = 0;

                try
                {
                    using (var cmd = new MySqlCommand("SELECT COALESCE(MAX(gameId), 0) AS lastId FROM jackpot_game;"))
                    {
                        var table = MySQL.QueryRead(cmd);
                        if (table != null && table.Rows.Count > 0)
                            lastId = Convert.ToInt32(table.Rows[0]["lastId"]);
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Init DB error: {e}");
                }

                // Загружаем историю
                List<HistoryGame> cache = new List<HistoryGame>();
                try
                {
                    using (var cmd = new MySqlCommand(@"
SELECT gameId, winnerLogin, winnerGender, winnerAccountId, winnerServerName, winnerServerId,
       winnerAmount, bank, winnerChance, winnerTicket, winnerCardIndex, winnerPosition
FROM jackpot_game
WHERE winner IS NOT NULL
ORDER BY gameId DESC
LIMIT 40;"))
                    {
                        var table = MySQL.QueryRead(cmd);
                        if (table != null)
                        {
                            foreach (DataRow r in table.Rows)
                            {
                                cache.Add(new HistoryGame
                                {
                                    Id = Convert.ToInt32(r["gameId"]),
                                    WinnerLogin = Convert.ToString(r["winnerLogin"]),
                                    WinnerGender = Convert.ToString(r["winnerGender"]),
                                    WinnerAccountId = Convert.ToInt32(r["winnerAccountId"]),
                                    WinnerServerName = Convert.ToString(r["winnerServerName"]),
                                    WinnerServerId = Convert.ToInt32(r["winnerServerId"]),
                                    WinnerAmount = Convert.ToInt32(r["winnerAmount"]),
                                    Bank = Convert.ToInt32(r["bank"]),
                                    WinnerChance = Convert.ToDouble(r["winnerChance"]),
                                    WinnerTicket = Convert.ToInt32(r["winnerTicket"]),
                                    WinnerCardIndex = Convert.ToInt32(r["winnerCardIndex"]),
                                    WinnerPosition = Convert.ToDouble(r["winnerPosition"])
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] History load error: {e}");
                }

                LoadSpecialAchievements();

                NAPI.Task.Run(() =>
                {
                    lock (Sync)
                    {
                        CurrentId = lastId + 1;
                    }

                    lock (HistoryList)
                    {
                        HistoryList.Clear();
                        HistoryList.AddRange(cache);
                        if (cache.Count > 0)
                        {
                            LastGame = cache[0];
                        }
                    }

                    Db_SaveRoundStart(CurrentId);

                    // НЕ запускаем таймер автоматически
                    // Таймер запустится, когда будет минимум 2 игрока
                    BroadcastAll();
                });
            });
        }

        private static void LoadSpecialAchievements()
        {
            try
            {
                // Удачливый игрок дня (самый низкий шанс победы за сегодня)
                using (var cmd = new MySqlCommand(@"
SELECT winnerLogin, winnerGender, winnerAccountId, winnerServerName, winnerServerId, winnerChance, bank
FROM jackpot_game
WHERE DATE(endAt) = CURDATE() AND winner IS NOT NULL
ORDER BY winnerChance ASC
LIMIT 1;"))
                {
                    var table = MySQL.QueryRead(cmd);
                    if (table != null && table.Rows.Count > 0)
                    {
                        var r = table.Rows[0];
                        LuckOfDay = new SpecialAchievement
                        {
                            WinnerLogin = Convert.ToString(r["winnerLogin"]),
                            WinnerGender = Convert.ToString(r["winnerGender"]),
                            WinnerAccountId = Convert.ToInt32(r["winnerAccountId"]),
                            ServerName = Convert.ToString(r["winnerServerName"]),
                            ServerId = Convert.ToInt32(r["winnerServerId"]),
                            WinnerChance = Convert.ToDouble(r["winnerChance"]),
                            Bank = Convert.ToInt32(r["bank"])
                        };
                    }
                }

                // Самый большой джекпот за всё время
                using (var cmd = new MySqlCommand(@"
SELECT winnerLogin, winnerGender, winnerAccountId, winnerServerName, winnerServerId, winnerChance, bank
FROM jackpot_game
WHERE winner IS NOT NULL
ORDER BY bank DESC
LIMIT 1;"))
                {
                    var table = MySQL.QueryRead(cmd);
                    if (table != null && table.Rows.Count > 0)
                    {
                        var r = table.Rows[0];
                        BigJackpot = new SpecialAchievement
                        {
                            WinnerLogin = Convert.ToString(r["winnerLogin"]),
                            WinnerGender = Convert.ToString(r["winnerGender"]),
                            WinnerAccountId = Convert.ToInt32(r["winnerAccountId"]),
                            ServerName = Convert.ToString(r["winnerServerName"]),
                            ServerId = Convert.ToInt32(r["winnerServerId"]),
                            WinnerChance = Convert.ToDouble(r["winnerChance"]),
                            Bank = Convert.ToInt32(r["bank"])
                        };
                    }
                }
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Jackpot] LoadSpecialAchievements error: {e}");
            }
        }

        // ===== Генерация результата джекпота =====
        private static JackpotBet SelectWinner(List<JackpotBet> bets)
        {
            if (bets.Count == 0) return null;

            var totalAmount = bets.Sum(b => b.Amount);
            var random = Rnd.NextDouble() * totalAmount;

            double currentSum = 0;
            foreach (var bet in bets)
            {
                currentSum += bet.Amount;
                if (random <= currentSum)
                {
                    return bet;
                }
            }

            return bets.Last(); // fallback
        }

        private static void AssignTickets(List<JackpotBet> bets)
        {
            int ticketNumber = 1;
            foreach (var bet in bets)
            {
                bet.Ticket = ticketNumber++;
            }
        }

        // ===== режимы =====
        private static void StartTimer(int waitMs)
        {
            lock (Sync)
            {
                // НЕ запускаем таймер, если меньше 2 игроков
                if (CurrentBets.Count < 2)
                {
                    NAPI.Util.ConsoleOutput("[Jackpot] Waiting for at least 2 players before starting timer");
                    TimerActive = false;
                    GameActive = false;
                    Winner = null;
                    BroadcastAll();
                    return;
                }

                TimerActive = true;
                GameActive = false;
                Winner = null;
                TimerInitTimeMs = waitMs;
                TimerCreatedAt = DateTime.UtcNow;
                TimerStartAt = DateTime.UtcNow.AddMilliseconds(waitMs);
                WinnerCardIndex = 0;
                WinnerPosition = 0.5;
            }

            StartLoop(1000);
            BroadcastAll();

            var gameTimer = new System.Timers.Timer(waitMs);
            gameTimer.Elapsed += (s, e) => {
                gameTimer.Dispose();
                NAPI.Task.Run(() => {
                    StopLoop();
                    StartGame();
                });
            };
            gameTimer.AutoReset = false;
            gameTimer.Start();
        }

        private static void StartGame()
        {
            JackpotBet winner;
            int cardIndex;
            double position;
            int totalBank;

            lock (Sync)
            {
                TimerActive = false;
                GameActive = true;
                GameStartAt = DateTime.UtcNow;
                GameStartTime = DateTime.UtcNow;

                if (CurrentBets.Count == 0)
                {
                    NAPI.Util.ConsoleOutput("[Jackpot] No bets, ending game");
                    EndGame(null);
                    return;
                }

                NAPI.Util.ConsoleOutput($"[Jackpot] Starting game with {CurrentBets.Count} players");

                // Назначаем билеты
                AssignTickets(CurrentBets);

                // Выбираем победителя
                winner = SelectWinner(CurrentBets);

                if (winner == null)
                {
                    NAPI.Util.ConsoleOutput("[Jackpot] ERROR: Winner is null!");
                    EndGame(null);
                    return;
                }

                // Вычисляем общий банк и шанс победителя
                totalBank = CurrentBets.Sum(b => b.Amount);
                double winnerChance = (double)winner.Amount / totalBank * 100;
                int winnerCardCount = Math.Max(1, (int)Math.Round(winnerChance / 10));

                NAPI.Util.ConsoleOutput($"[Jackpot] Winner: {winner.Login} (UUID: {winner.UUID})");
                NAPI.Util.ConsoleOutput($"[Jackpot] Winner chance: {winnerChance:F2}%");
                NAPI.Util.ConsoleOutput($"[Jackpot] Winner cards count: {winnerCardCount}");

                // Выбираем случайную карточку из карточек победителя
                cardIndex = Rnd.Next(0, winnerCardCount);

                // Позиция внутри карточки (0.0 - 1.0)
                position = Rnd.NextDouble();

                WinnerCardIndex = cardIndex;
                WinnerPosition = position;

                NAPI.Util.ConsoleOutput($"[Jackpot] Selected card index: {cardIndex} of {winnerCardCount}");
                NAPI.Util.ConsoleOutput($"[Jackpot] Position within card: {position:F3}");

                // ИСПРАВЛЕНИЕ: Создаем WinnerData из JackpotBet
                Winner = new WinnerData
                {
                    UUID = winner.UUID,
                    Login = winner.Login,
                    Gender = winner.Gender,
                    AccountId = winner.UUID,
                    ServerId = winner.ServerId,
                    ServerName = winner.ServerName,
                    BetAmount = winner.Amount,
                    WinCoins = totalBank,
                    WinChance = winnerChance,
                    Ticket = winner.Ticket,
                    WinnerCardIndex = cardIndex,
                    WinnerPosition = position
                };
            }

            // Запускаем частое обновление во время игры
            StartLoop(200);
            BroadcastAll();

            NAPI.Util.ConsoleOutput($"[Jackpot] Game will run for {TIME_GAME_MS / 1000} seconds");

            // Таймер окончания игры
            var gameTimer = new System.Timers.Timer(TIME_GAME_MS);
            gameTimer.Elapsed += (s, e) => {
                gameTimer.Dispose();
                NAPI.Task.Run(() => {
                    NAPI.Util.ConsoleOutput("[Jackpot] Game time elapsed, ending game");
                    EndGame(winner);
                });
            };
            gameTimer.AutoReset = false;
            gameTimer.Start();
        }

        private static void EndGame(JackpotBet winner)
        {
            int gameId;
            int bankFinal;
            int playersCnt;
            List<JackpotBet> allBets;

            lock (Sync)
            {
                GameActive = false;

                gameId = CurrentId;
                allBets = CurrentBets.ToList();
                bankFinal = allBets.Sum(bet => bet.Amount);
                playersCnt = allBets.GroupBy(bet => bet.UUID).Count();
            }

            StopLoop();

            if (winner != null)
            {
                // Сначала сохраняем билеты в БД
                Db_SaveTickets(gameId, allBets);

                // Выплачиваем выигрыш
                var player = NAPI.Pools.GetAllPlayers()
                    .Cast<ExtPlayer>()
                    .FirstOrDefault(p => p.IsCharacterData() && p.GetCharacterData()?.UUID == winner.UUID);

                if (player != null)
                {
                    UpdateData.RedBucks(player, bankFinal, msg: "Мини-игра 'Джекпот': выигрыш");
                    BattlePass.Repository.UpdateReward(player, 3);

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                        $"Вы выиграли джекпот {bankFinal} монет!", 5000);
                }

                // Обновляем специальные достижения
                var winChance = (double)winner.Amount / bankFinal * 100;

                // Удачливый игрок дня
                if (LuckOfDay == null || winChance < LuckOfDay.WinnerChance)
                {
                    LuckOfDay = new SpecialAchievement
                    {
                        WinnerLogin = winner.Login,
                        WinnerGender = winner.Gender,
                        WinnerAccountId = winner.UUID,
                        ServerName = winner.ServerName,
                        ServerId = winner.ServerId,
                        WinnerChance = winChance,
                        Bank = bankFinal
                    };
                }

                // Самый большой джекпот
                if (BigJackpot == null || bankFinal > BigJackpot.Bank)
                {
                    BigJackpot = new SpecialAchievement
                    {
                        WinnerLogin = winner.Login,
                        WinnerGender = winner.Gender,
                        WinnerAccountId = winner.UUID,
                        ServerName = winner.ServerName,
                        ServerId = winner.ServerId,
                        WinnerChance = winChance,
                        Bank = bankFinal
                    };
                }
            }

            // ПОКАЗЫВАЕМ РЕЗУЛЬТАТ 3 СЕКУНДЫ
            StartLoop(500);
            BroadcastAll();

            // Сохранение в БД
            Db_SaveRoundEnd(gameId, winner, bankFinal, playersCnt);

            // Обновление кеша истории
            if (winner != null)
            {
                var historyGame = new HistoryGame
                {
                    Id = gameId,
                    WinnerLogin = winner.Login,
                    WinnerGender = winner.Gender,
                    WinnerAccountId = winner.UUID,
                    WinnerServerName = winner.ServerName,
                    WinnerServerId = winner.ServerId,
                    WinnerAmount = winner.Amount,
                    Bank = bankFinal,
                    WinnerChance = (double)winner.Amount / bankFinal * 100,
                    WinnerTicket = winner.Ticket,
                    WinnerCardIndex = WinnerCardIndex,
                    WinnerPosition = WinnerPosition
                };

                lock (HistoryList)
                {
                    HistoryList.Insert(0, historyGame);
                    if (HistoryList.Count > HISTORY_CACHE_LIMIT)
                        HistoryList.RemoveAt(HistoryList.Count - 1);

                    LastGame = historyGame;
                }
            }

            // ЗАДЕРЖКА 3 СЕКУНДЫ ПЕРЕД НАЧАЛОМ НОВОЙ ИГРЫ
            var showResultTimer = new System.Timers.Timer(3000);
            showResultTimer.Elapsed += (s, e) => {
                showResultTimer.Dispose();
                NAPI.Task.Run(() => {
                    // Очищаем ставки и готовим следующий раунд
                    lock (Sync)
                    {
                        CurrentBets.Clear();
                        CurrentId++;
                    }

                    Db_SaveRoundStart(CurrentId);

                    // Небольшая пауза перед следующим раундом
                    var nextRoundTimer = new System.Timers.Timer(500);
                    nextRoundTimer.Elapsed += (s2, e2) => {
                        nextRoundTimer.Dispose();
                        NAPI.Task.Run(() => StartTimer(WAIT_BET_MS));
                    };
                    nextRoundTimer.AutoReset = false;
                    nextRoundTimer.Start();
                });
            };
            showResultTimer.AutoReset = false;
            showResultTimer.Start();
        }

        // ===== периодическая рассылка =====
        private static void StartLoop(int intervalMs)
        {
            StopLoop();
            BroadcastLoop = new System.Timers.Timer(intervalMs) { AutoReset = true };
            BroadcastLoop.Elapsed += (s, e) => BroadcastAll();
            BroadcastLoop.Start();
        }

        private static void StopLoop()
        {
            try
            {
                BroadcastLoop?.Stop();
                BroadcastLoop?.Dispose();
            }
            catch { }
            BroadcastLoop = null;
        }

        // ===== отправка снапшотов =====
        private static void BroadcastAll()
        {
            foreach (var pl in NAPI.Pools.GetAllPlayers())
            {
                var ep = pl as ExtPlayer;
                if (ep == null || !ep.IsCharacterData()) continue;
                BroadcastTo(ep);
            }
        }

        private static void BroadcastTo(ExtPlayer p, int? forcePageIndex = null)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    var ch = p.GetCharacterData();
                    if (ch == null) return;

                    int currentPage = 0;
                    if (forcePageIndex.HasValue)
                    {
                        currentPage = forcePageIndex.Value;
                        lock (PlayerPages)
                        {
                            PlayerPages[ch.UUID] = currentPage;
                        }
                    }
                    else
                    {
                        lock (PlayerPages)
                        {
                            if (PlayerPages.ContainsKey(ch.UUID))
                                currentPage = PlayerPages[ch.UUID];
                        }
                    }

                    if (currentPage != 0) return;

                    // Собираем данные текущей игры
                    List<object> currentBets;
                    int gameId;
                    bool timerActive, gameActive;
                    DateTime timerStartAt, gameStartTime;
                    int remainingMs = 0;
                    object winner = null;

                    lock (Sync)
                    {
                        gameId = CurrentId;

                        currentBets = CurrentBets.Select(bet => new
                        {
                            accountId = bet.UUID,
                            login = bet.Login,
                            amount = bet.Amount,
                            gender = bet.Gender ?? "male",
                            serverName = bet.ServerName ?? "Server",
                            serverId = bet.ServerId,
                            ticket = bet.Ticket
                        }).Cast<object>().ToList();

                        timerActive = TimerActive;
                        gameActive = GameActive;
                        timerStartAt = TimerStartAt;
                        gameStartTime = GameStartAt;

                        if (timerActive)
                        {
                            var elapsed = (DateTime.UtcNow - TimerCreatedAt).TotalMilliseconds;
                            remainingMs = Math.Max(0, (int)(TimerInitTimeMs - elapsed));
                        }

                        if (Winner != null)
                        {
                            var totalBank = CurrentBets.Sum(b => b.Amount);
                            var winChance = totalBank > 0 ? (double)Winner.BetAmount / totalBank * 100 : 0;

                            winner = new 
                            {
                                login = Winner.Login,
                                gender = Winner.Gender,
                                accountId = Winner.UUID,
                                serverName = Winner.ServerName,
                                serverId = Winner.ServerId,
                                betAmount = Winner.BetAmount,
                                winCoins = totalBank,
                                winChance = winChance,
                                ticket = Winner.Ticket,
                                winnerCardIndex = WinnerCardIndex
                            };
                        }
                    }

                    // Специальные достижения
                    object luck = null, jackpot = null, lastGame = null;

                    if (LuckOfDay != null)
                    {
                        luck = new
                        {
                            winnerLogin = LuckOfDay.WinnerLogin,
                            winnerGender = LuckOfDay.WinnerGender,
                            winnerAccountId = LuckOfDay.WinnerAccountId,
                            serverName = LuckOfDay.ServerName,
                            serverId = LuckOfDay.ServerId,
                            winnerChance = LuckOfDay.WinnerChance
                        };
                    }

                    if (BigJackpot != null)
                    {
                        jackpot = new
                        {
                            winnerLogin = BigJackpot.WinnerLogin,
                            winnerGender = BigJackpot.WinnerGender,
                            winnerAccountId = BigJackpot.WinnerAccountId,
                            serverName = BigJackpot.ServerName,
                            serverId = BigJackpot.ServerId,
                            bank = BigJackpot.Bank
                        };
                    }

                    if (LastGame != null)
                    {
                        lastGame = new
                        {
                            winnerLogin = LastGame.WinnerLogin,
                            winnerGender = LastGame.WinnerGender,
                            winnerAccountId = LastGame.WinnerAccountId,
                            serverName = LastGame.WinnerServerName,
                            serverId = LastGame.WinnerServerId,
                            winnerChance = LastGame.WinnerChance
                        };
                    }

                    var dto = new
                    {
                        demo = false,
                        active = true,
                        timer = new
                        {
                            active = timerActive,
                            initTime = remainingMs,
                            timeStart = TimerCreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                        },
                        game = new
                        {
                            active = gameActive,
                            timeGame = GameTime,
                            timeStart = gameStartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            winner = winner,
                            winnerPosition = WinnerPosition
                        },
                        currentGame = new
                        {
                            id = gameId,
                            bets = currentBets
                        },
                        luck = luck,
                        jackpot = jackpot,
                        lastGame = lastGame,
                        pages = new[] { "main", "historyList", "historyItem" },  // ДОБАВЬТЕ ЭТУ СТРОКУ
                        openedPage = currentPage,  // используем currentPage вместо 0
                        main = new
                        {
                            accountId = ch.UUID,
                            login = p.Name,
                            serverName = "Server",
                            serverTimeOffset = 0
                        }
                    };

                    string json = JsonConvert.SerializeObject(dto);
                    Trigger.ClientEvent(p, "jackpot_update", json);
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] BroadcastTo Exception for {p?.Name}: {e}");
                }
            });
        }

        // ===== Remote Events (клиент ↔ сервер) =====
        [RemoteEvent("server.jackpot.request")]
        public void JackpotRequest(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;

            var ch = player.GetCharacterData();
            if (ch == null) return;

            // Сбрасываем страницу на главную при запросе данных
            lock (PlayerPages)
            {
                PlayerPages[ch.UUID] = 0;
            }

            BroadcastTo(player, 0);
        }

        [RemoteEvent("server.jackpot.bet")]
        public void JackpotBets(ExtPlayer player, int amount)
        {
            try
            {
                var acc = player.GetAccountData(); if (acc == null) return;
                var ch = player.GetCharacterData(); if (ch == null) return;

                lock (Sync)
                {
                    // ИЗМЕНЕНО: Разрешаем ставки если НЕ идет игра (вместо проверки TimerActive)
                    if (GameActive)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Игра уже идет, дождитесь следующего раунда", 3000);
                        return;
                    }

                    // Проверка максимального количества игроков
                    if (CurrentBets.Count >= 50)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Достигнуто максимальное количество игроков (50)", 3000);
                        return;
                    }

                    // Проверяем, есть ли уже ставка от этого игрока
                    var existingBet = CurrentBets.FirstOrDefault(bet => bet.UUID == ch.UUID);
                    if (existingBet != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Вы уже сделали ставку в этом раунде", 3000);
                        return;
                    }
                }

                if (amount <= 0 || acc.RedBucks < amount)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        Localization.LangFunc.GetText(Localization.LangType.Ru, Localization.DataName.NetRB), 3000);
                    return;
                }

                // Списываем коины
                UpdateData.RedBucks(player, -amount, msg: "Мини-игра 'Джекпот': ставка");

                bool shouldStartTimer = false;

                lock (Sync)
                {
                    CurrentBets.Add(new JackpotBet
                    {
                        UUID = ch.UUID,
                        Login = player.Name,
                        Amount = amount,
                        Gender = "male",
                        ServerName = "Server",
                        ServerId = 1,
                        Ticket = 0,
                        Paid = false
                    });

                    // Если это вторая ставка и таймер не запущен - запускаем
                    if (CurrentBets.Count == 2 && !TimerActive)
                    {
                        shouldStartTimer = true;
                    }

                    // Если 50 игроков - запускаем игру немедленно
                    if (CurrentBets.Count >= 50)
                    {
                        NAPI.Util.ConsoleOutput($"[Jackpot] Max players reached, starting game");
                        StopLoop();
                        StartGame();
                        return;
                    }
                }

                // Сохраняем в БД
                Db_SaveBet(CurrentId, ch.UUID, player.Name, amount);

                if (shouldStartTimer)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Starting timer (2 players reached)");
                    StartTimer(WAIT_BET_MS);
                }
                else
                {
                    BroadcastAll();
                }
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Jackpot] Bet Exception: {e}");
            }
        }

        // Обработчики страниц
        [RemoteEvent("server.jackpot.openPage")]
        public void JackpotOpenPage(ExtPlayer player, int page, int gameId = 0)
        {
            NAPI.Util.ConsoleOutput($"[Jackpot] OpenPage called: page={page}, gameId={gameId}");

            var ch = player.GetCharacterData();
            if (ch == null) return;

            // Сохраняем текущую страницу игрока
            lock (PlayerPages)
            {
                PlayerPages[ch.UUID] = page;
            }

            if (page == 1)
            {
                HistoryListInternal(player);
            }
            else if (page == 2)
            {
                HistoryItemInternal(player, gameId);
            }
            else
            {
                // Возврат на главную
                lock (PlayerPages)
                {
                    PlayerPages[ch.UUID] = 0;
                }
                BroadcastTo(player, 0);
            }
        }

        [RemoteEvent("server.jackpot.history")]
        public void JackpotHistory(ExtPlayer player, int page, int gameId = 0)
        {
            NAPI.Util.ConsoleOutput($"[Jackpot] History called: page={page}, gameId={gameId}");
            if (page == 1) HistoryListInternal(player);
            else if (page == 2) HistoryItemInternal(player, gameId);
            else BroadcastTo(player);
        }

        private static void HistoryListInternal(ExtPlayer p)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    var ch = p.GetCharacterData();
                    if (ch == null) return;

                    List<object> list = null;
                    lock (HistoryList)
                    {
                        if (HistoryList.Count > 0)
                        {
                            list = HistoryList
                                .OrderByDescending(x => x.Id)
                                .Take(50)
                                .Select(h => new
                                {
                                    id = h.Id,
                                    winnerLogin = h.WinnerLogin,
                                    winnerGender = h.WinnerGender,
                                    winnerAccountId = h.WinnerAccountId,
                                    winnerServerName = h.WinnerServerName,
                                    winnerServerId = h.WinnerServerId,
                                    winnerAmount = h.WinnerAmount,
                                    bank = h.Bank,
                                    winnerChance = h.WinnerChance,
                                    winnerTicket = h.WinnerTicket
                                })
                                .Cast<object>()
                                .ToList();
                        }
                    }

                    if (list == null || list.Count == 0)
                    {
                        Trigger.SetTask(() =>
                        {
                            var cmd = new MySqlCommand(@"
SELECT gameId AS id, winnerLogin, winnerGender, winnerAccountId, winnerServerName, winnerServerId,
       winnerAmount, bank, winnerChance, winnerTicket
FROM jackpot_game
WHERE winner IS NOT NULL
ORDER BY gameId DESC
LIMIT 50;");

                            DataTable table = MySQL.QueryRead(cmd);
                            var l = new List<object>();
                            if (table != null)
                            {
                                foreach (DataRow r in table.Rows)
                                {
                                    l.Add(new
                                    {
                                        id = Convert.ToInt32(r["id"]),
                                        winnerLogin = Convert.ToString(r["winnerLogin"]),
                                        winnerGender = Convert.ToString(r["winnerGender"]),
                                        winnerAccountId = Convert.ToInt32(r["winnerAccountId"]),
                                        winnerServerName = Convert.ToString(r["winnerServerName"]),
                                        winnerServerId = Convert.ToInt32(r["winnerServerId"]),
                                        winnerAmount = Convert.ToInt32(r["winnerAmount"]),
                                        bank = Convert.ToInt32(r["bank"]),
                                        winnerChance = Convert.ToDouble(r["winnerChance"]),
                                        winnerTicket = Convert.ToInt32(r["winnerTicket"])
                                    });
                                }
                            }

                            NAPI.Task.Run(() =>
                            {
                                var historyDto = new
                                {
                                    type = "historyList",
                                    historyList = l,
                                    openedPage = 1
                                };
                                Trigger.ClientEvent(p, "jackpot_history", JsonConvert.SerializeObject(historyDto));
                            });
                        });
                        return;
                    }

                    var dtoCached = new
                    {
                        type = "historyList",
                        historyList = list,
                        openedPage = 1
                    };
                    Trigger.ClientEvent(p, "jackpot_history", JsonConvert.SerializeObject(dtoCached));
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] HistoryList Exception: {e}");
                }
            });
        }

        private static void HistoryItemInternal(ExtPlayer p, int gameId)
        {
            Trigger.SetTask(() =>
            {
                try
                {
                    // карточка игры
                    var gameCmd = new MySqlCommand(@"
SELECT gameId AS id, winnerLogin, winnerGender, winnerAccountId, winnerServerName, winnerServerId,
       winnerAmount, bank, winnerChance, winnerTicket, winnerCardIndex, winnerPosition
FROM jackpot_game
WHERE gameId = @gid
LIMIT 1;");
                    gameCmd.Parameters.AddWithValue("@gid", gameId);
                    var gameTable = MySQL.QueryRead(gameCmd);

                    // ставки этой игры
                    var betsCmd = new MySqlCommand(@"
SELECT uuid AS accountId, login, amount, gender, serverName, serverId, ticket
FROM jackpot_bets
WHERE gameId = @gid
ORDER BY id ASC;");
                    betsCmd.Parameters.AddWithValue("@gid", gameId);
                    var betsTable = MySQL.QueryRead(betsCmd);

                    if (gameTable == null || gameTable.Rows.Count == 0)
                    {
                        NAPI.Task.Run(() =>
                        {
                            var dtoEmpty = new
                            {
                                type = "historyItem",
                                historyItem = new { game = (object)null, bets = new object[0] },
                                openedPage = 2
                            };
                            Trigger.ClientEvent(p, "jackpot_history", JsonConvert.SerializeObject(dtoEmpty));
                        });
                        return;
                    }

                    var gRow = gameTable.Rows[0];
                    var game = new
                    {
                        id = Convert.ToInt32(gRow["id"]),
                        winnerLogin = Convert.ToString(gRow["winnerLogin"]),
                        winnerGender = Convert.ToString(gRow["winnerGender"]),
                        winnerAccountId = Convert.ToInt32(gRow["winnerAccountId"]),
                        winnerServerName = Convert.ToString(gRow["winnerServerName"]),
                        winnerServerId = Convert.ToInt32(gRow["winnerServerId"]),
                        winnerAmount = Convert.ToInt32(gRow["winnerAmount"]),
                        bank = Convert.ToInt32(gRow["bank"]),
                        winnerChance = Convert.ToDouble(gRow["winnerChance"]),
                        winnerTicket = Convert.ToInt32(gRow["winnerTicket"]),
                        winnerCardIndex = Convert.ToInt32(gRow["winnerCardIndex"]),
                        winnerPosition = Convert.ToDouble(gRow["winnerPosition"])
                    };

                    var bets = new List<object>();
                    if (betsTable != null)
                    {
                        foreach (DataRow r in betsTable.Rows)
                        {
                            bets.Add(new
                            {
                                accountId = Convert.ToInt32(r["accountId"]),
                                login = Convert.ToString(r["login"] ?? ""),
                                amount = Convert.ToInt32(r["amount"]),
                                gender = Convert.ToString(r["gender"] ?? "male"),
                                serverName = Convert.ToString(r["serverName"] ?? "Server"),
                                serverId = Convert.ToInt32(r["serverId"]),
                                ticket = Convert.ToInt32(r["ticket"])
                            });
                        }
                    }

                    NAPI.Task.Run(() =>
                    {
                        var historyItemDto = new
                        {
                            type = "historyItem",
                            historyItem = new { game, bets },
                            openedPage = 2
                        };
                        Trigger.ClientEvent(p, "jackpot_history", JsonConvert.SerializeObject(historyItemDto));
                    });
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] HistoryItem Exception: {e}");
                }
            });
        }
        [Command("jackpotaddbot")]
        public void AddTestPlayer(ExtPlayer player, int amount = 100)
        {
            if (!player.IsCharacterData()) return;
            var acc = player.GetAccountData();
            if (acc == null) return;

            lock (Sync)
            {
                // ИСПРАВЛЕНО: проверяем GameActive вместо TimerActive
                if (GameActive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Игра уже идет, дождитесь следующего раунда", 3000);
                    return;
                }

                // Проверка на максимальное количество игроков
                if (CurrentBets.Count >= 50)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Достигнуто максимальное количество игроков (50)", 3000);
                    return;
                }

                int botId = 9000 + CurrentBets.Count;
                string[] botNames = new[] { "TestBot", "PlayerTest", "DemoUser", "BotPlayer", "TestUser", "Gambler", "LuckyOne", "ProPlayer" };
                string randomName = botNames[Rnd.Next(botNames.Length)] + Rnd.Next(100, 999);

                CurrentBets.Add(new JackpotBet
                {
                    UUID = botId,
                    Login = randomName,
                    Amount = amount,
                    Gender = Rnd.Next(2) == 0 ? "male" : "female",
                    ServerName = "Server",
                    ServerId = 1,
                    Ticket = 0,
                    Paid = false
                });

                Db_SaveBet(CurrentId, botId, randomName, amount);

                // Если это второй игрок и таймер не запущен - запускаем
                if (CurrentBets.Count == 2 && !TimerActive)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Starting timer (2 players reached via bot)");
                    StartTimer(WAIT_BET_MS);
                }

                // Если достигли 50 игроков - сразу запускаем игру
                if (CurrentBets.Count >= 50)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Max players reached via bots, starting game");
                    StopLoop();
                    StartGame();
                    return;
                }
            }

            BroadcastAll();
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                $"Добавлен бот '{CurrentBets[CurrentBets.Count - 1].Login}' с ставкой {amount} монет", 3000);
        }

        [Command("jackpotaddbots")]
        public void AddMultipleTestPlayers(ExtPlayer player, int count = 5, int minAmount = 50, int maxAmount = 500)
        {
            if (!player.IsCharacterData()) return;
            var acc = player.GetAccountData();
            if (acc == null) return;

            if (count <= 0 || count > 50)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                    "Количество ботов должно быть от 1 до 50", 3000);
                return;
            }

            if (minAmount <= 0 || maxAmount <= 0 || minAmount > maxAmount)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                    "Неверные параметры суммы ставки", 3000);
                return;
            }

            bool shouldStartTimer = false;
            bool shouldStartGame = false;
            int addedCount = 0;

            lock (Sync)
            {
                // ИСПРАВЛЕНО: проверяем GameActive вместо TimerActive
                if (GameActive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Игра уже идет, дождитесь следующего раунда", 3000);
                    return;
                }

                string[] botNames = new[] { "TestBot", "PlayerTest", "DemoUser", "BotPlayer", "TestUser", "Gambler", "LuckyOne", "ProPlayer", "Winner", "Casino" };

                for (int i = 0; i < count; i++)
                {
                    // Проверяем лимит на каждой итерации
                    if (CurrentBets.Count >= 50)
                    {
                        NAPI.Util.ConsoleOutput($"[Jackpot] Reached max players while adding bots ({addedCount} bots added)");
                        break;
                    }

                    int botId = 9000 + CurrentBets.Count;
                    string randomName = botNames[Rnd.Next(botNames.Length)] + Rnd.Next(100, 999);
                    int amount = Rnd.Next(minAmount, maxAmount + 1);

                    CurrentBets.Add(new JackpotBet
                    {
                        UUID = botId,
                        Login = randomName,
                        Amount = amount,
                        Gender = Rnd.Next(2) == 0 ? "male" : "female",
                        ServerName = "Server",
                        ServerId = 1,
                        Ticket = 0,
                        Paid = false
                    });

                    Db_SaveBet(CurrentId, botId, randomName, amount);
                    addedCount++;

                    // Проверяем, нужно ли запустить таймер (при достижении 2 игроков)
                    if (CurrentBets.Count == 2 && !TimerActive)
                    {
                        shouldStartTimer = true;
                    }

                    // Проверяем, достигли ли мы 50 игроков
                    if (CurrentBets.Count >= 50)
                    {
                        shouldStartGame = true;
                        break;
                    }
                }
            }

            BroadcastAll();

            if (shouldStartGame)
            {
                NAPI.Util.ConsoleOutput($"[Jackpot] Max players reached via bots, starting game immediately");
                StopLoop();
                NAPI.Task.Run(() => StartGame());
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                    $"Добавлено {addedCount} ботов. Максимум достигнут, игра запущена!", 3000);
            }
            else if (shouldStartTimer)
            {
                NAPI.Util.ConsoleOutput($"[Jackpot] Starting timer (2 players reached via bots)");
                StartTimer(WAIT_BET_MS);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                    $"Добавлено {addedCount} ботов. Таймер запущен!", 3000);
            }
            else
            {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                    $"Добавлено {addedCount} тестовых игроков", 3000);
            }
        }

        [Command("jackpotclear")]
        public void ClearTestPlayers(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;

            int removedCount = 0;

            lock (Sync)
            {
                removedCount = CurrentBets.RemoveAll(bet => bet.UUID >= 9000);

                // Если после удаления осталось меньше 2 игроков и таймер активен - останавливаем
                if (CurrentBets.Count < 2 && TimerActive)
                {
                    NAPI.Util.ConsoleOutput("[Jackpot] Less than 2 players after clearing bots, stopping timer");
                    StopLoop();
                    TimerActive = false;
                }
            }

            BroadcastAll();
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                $"Удалено {removedCount} тестовых игроков", 3000);
        }

        [Command("jackpotrestart")]
        public void RestartJackpot(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;

            NAPI.Util.ConsoleOutput("[Jackpot] Manual restart by admin: " + player.Name);

            // Останавливаем текущую игру
            StopLoop();


            // Перезапускаем
            Init();

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                "Джекпот перезапущен", 3000);
        }

        [Command("jackpotinfo")]
        public void JackpotInfo(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;

            lock (Sync)
            {
                string info = $"=== JACKPOT INFO ===\n" +
                             $"Game ID: {CurrentId}\n" +
                             $"Players: {CurrentBets.Count}/50\n" +
                             $"Timer Active: {TimerActive}\n" +
                             $"Game Active: {GameActive}\n" +
                             $"Total Bank: {CurrentBets.Sum(b => b.Amount)} coins\n" +
                             $"Winner: {(Winner != null ? Winner.Login : "None")}";

                NAPI.Chat.SendChatMessageToPlayer(player, info);
            }
        }

        [Command("jackpotforcestart")]
        public void ForceStartGame(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;

            lock (Sync)
            {
                if (CurrentBets.Count < 2)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Нужно минимум 2 игрока для старта", 3000);
                    return;
                }

                if (GameActive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Игра уже идет", 3000);
                    return;
                }
            }

            NAPI.Util.ConsoleOutput($"[Jackpot] Force start by admin: {player.Name}");
            StopLoop();
            NAPI.Task.Run(() => StartGame());

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                "Игра принудительно запущена", 3000);
        }

        // Возврат ставки при выходе игрока во время ожидания


        // ===== БД =====
        // Предполагаемые таблицы:
        // jackpot_game(gameId PK, startAt DATETIME, endAt DATETIME, winner INT NULL,
        //              winnerLogin VARCHAR(50), winnerGender VARCHAR(10), winnerAccountId INT,
        //              winnerServerName VARCHAR(50), winnerServerId INT, winnerAmount INT,
        //              bank INT, players INT, winnerChance DOUBLE, winnerTicket INT,
        //              winnerCardIndex INT, winnerPosition DOUBLE)
        // jackpot_bets(id AI PK, gameId INT, uuid INT, login VARCHAR(50), amount INT,
        //              gender VARCHAR(10), serverName VARCHAR(50), serverId INT,
        //              ticket INT, createdAt DATETIME)

        private static void Db_SaveRoundStart(int gameId)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
INSERT INTO `jackpot_game`
    (`gameId`,`startAt`,`bank`,`players`)
VALUES
    (@gid, NOW(), 0, 0)
ON DUPLICATE KEY UPDATE
    `startAt` = VALUES(`startAt`);");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveRoundStart: {e}"); }
        }

        private static void Db_SaveRoundEnd(int gameId, JackpotBet winner, int bankFinal, int playersCnt)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    if (winner == null)
                    {
                        var cmd = new MySqlCommand(@"
UPDATE `jackpot_game`
SET
    `endAt` = NOW(),
    `bank` = @bank,
    `players` = @pcnt
WHERE `gameId` = @gid;");
                        cmd.Parameters.AddWithValue("@gid", gameId);
                        cmd.Parameters.AddWithValue("@bank", bankFinal);
                        cmd.Parameters.AddWithValue("@pcnt", playersCnt);
                        await MySQL.QueryAsync(cmd);
                    }
                    else
                    {
                        var winChance = bankFinal > 0 ? (double)winner.Amount / bankFinal * 100 : 0;

                        var cmd = new MySqlCommand(@"
UPDATE `jackpot_game`
SET
    `endAt` = NOW(),
    `winner` = @winner,
    `winnerLogin` = @winnerLogin,
    `winnerGender` = @winnerGender,
    `winnerAccountId` = @winnerAccountId,
    `winnerServerName` = @winnerServerName,
    `winnerServerId` = @winnerServerId,
    `winnerAmount` = @winnerAmount,
    `bank` = @bank,
    `players` = @pcnt,
    `winnerChance` = @winnerChance,
    `winnerTicket` = @winnerTicket,
    `winnerCardIndex` = @winnerCardIndex,
    `winnerPosition` = @winnerPosition
WHERE `gameId` = @gid;");
                        cmd.Parameters.AddWithValue("@gid", gameId);
                        cmd.Parameters.AddWithValue("@winner", winner.UUID);
                        cmd.Parameters.AddWithValue("@winnerLogin", winner.Login);
                        cmd.Parameters.AddWithValue("@winnerGender", winner.Gender);
                        cmd.Parameters.AddWithValue("@winnerAccountId", winner.UUID);
                        cmd.Parameters.AddWithValue("@winnerServerName", winner.ServerName);
                        cmd.Parameters.AddWithValue("@winnerServerId", winner.ServerId);
                        cmd.Parameters.AddWithValue("@winnerAmount", winner.Amount);
                        cmd.Parameters.AddWithValue("@bank", bankFinal);
                        cmd.Parameters.AddWithValue("@pcnt", playersCnt);
                        cmd.Parameters.AddWithValue("@winnerChance", winChance);
                        cmd.Parameters.AddWithValue("@winnerTicket", winner.Ticket);
                        cmd.Parameters.AddWithValue("@winnerCardIndex", WinnerCardIndex);
                        cmd.Parameters.AddWithValue("@winnerPosition", WinnerPosition);
                        await MySQL.QueryAsync(cmd);
                    }
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveRoundEnd: {e}"); }
        }

        private static void Db_SaveTickets(int gameId, List<JackpotBet> bets)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    foreach (var bet in bets)
                    {
                        var cmd = new MySqlCommand(@"
UPDATE `jackpot_bets`
SET `ticket` = @ticket
WHERE `gameId` = @gid AND `uuid` = @uuid;");
                        cmd.Parameters.AddWithValue("@gid", gameId);
                        cmd.Parameters.AddWithValue("@uuid", bet.UUID);
                        cmd.Parameters.AddWithValue("@ticket", bet.Ticket);
                        await MySQL.QueryAsync(cmd);
                    }
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveTickets: {e}"); }
        }

        private static void Db_SaveBet(int gameId, int uuid, string login, int amount)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
INSERT INTO `jackpot_bets`
    (`gameId`,`uuid`,`login`,`amount`,`gender`,`serverName`,`serverId`,`ticket`,`createdAt`)
VALUES
    (@gid, @uuid, @login, @amount, @gender, @serverName, @serverId, 0, NOW());");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@login", login ?? "");
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@gender", "male");
                    cmd.Parameters.AddWithValue("@serverName", "Server");
                    cmd.Parameters.AddWithValue("@serverId", 1);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveBet: {e}"); }
        }

        private static void Db_UpdateWinner(int gameId, JackpotBet winner, int winAmount)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    // Обновляем все ставки в этой игре, устанавливая ticket
                    var cmd = new MySqlCommand(@"
UPDATE `jackpot_bets` b
INNER JOIN (
    SELECT uuid, ticket
    FROM `jackpot_bets`
    WHERE gameId = @gid
) t ON b.uuid = t.uuid AND b.gameId = @gid
SET b.ticket = t.ticket;");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Jackpot] Db_UpdateWinner: {e}"); }
        }
    }
}