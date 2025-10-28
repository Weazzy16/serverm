// serverside/CrashGame.cs
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
    /// Мини-игра CRASH.
    /// Потокобезопасность: любой вызов GTA-API (NAPI, Trigger.ClientEvent и т.п.) — только из главного потока через NAPI.Task.Run.
    /// Работа с БД — через Trigger.SetTask (внутри можно синхронно дергать MySQL.QueryRead/QueryAsync).
    /// </summary>
    public class CrashGame : Script
    {
        // ===== внутренняя модель ставки =====
        private class CrashStake
        {
            public int UUID;
            public string Login;
            public int Bet;
            public float? ExitX;    // null = ещё в игре
            public bool Paid;
        }

        // Кеш последней истории, чтобы не гонять БД лишний раз
        private class LastGame
        {
            public int Id;
            public float CrashedAt;
            public int Bank;
            public int PlayersCount;
        }

        private const int HISTORY_PREVIEW_LIMIT = 28;    // сколько показываем в верхней мини-ленте
        private const int HISTORY_CACHE_LIMIT = 200;   // общий размер кеша

        private static readonly List<LastGame> LastGames = new List<LastGame>(); // наполняется на EndGame/Init

        // ===== состояние цикла =====
        private static readonly object Sync = new object();
        private static readonly Dictionary<int, CrashStake> Current = new Dictionary<int, CrashStake>(); // key = UUID

        private static bool TimerActive;           // идёт ожидание
        private static DateTime TimerStartAt;      // когда стартует раунд (UTC/серверное)
        private static DateTime TimerCreatedAt;    // когда был создан таймер
        private static int TimerInitTimeMs = 0;    // "initTime" для клиента

        private static bool GameActive;            // идёт раунд
        private static DateTime GameStartAt;       // старт текущей игры
        private static double? CrashedAt;          // X, где упало (после конца игры)

        private static int CurrentId = 1;          // id текущей игры

        private static System.Timers.Timer BroadcastLoop; // периодическая рассылка живых снапшотов во время игры
        private static readonly Random Rnd = new Random();
        private const int WAIT_BET_MS = 16000;

        // ===== жизненный цикл ресурса =====
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            NAPI.Util.ConsoleOutput("[Crash] resource start");
            Init();
        }

        [ServerEvent(Event.ResourceStop)]
        public void OnResourceStop()
        {
            StopLoop();
        }

        public static void Init()
        {
         //   NAPI.Util.ConsoleOutput("[Crash] Init started");

            lock (Sync)
            {
                Current.Clear();
                GameActive = false;
                TimerActive = false;
                CrashedAt = null;
            }

            // Читаем данные из БД в фоне
            Trigger.SetTask(() =>
            {
                int lastId = 0;

                try
                {
                    using (var cmd = new MySqlCommand("SELECT COALESCE(MAX(gameId), 0) AS lastId FROM crash_game;"))
                    {
                        var table = MySQL.QueryRead(cmd);
                        if (table != null && table.Rows.Count > 0)
                            lastId = Convert.ToInt32(table.Rows[0]["lastId"]);
                    }
               //     NAPI.Util.ConsoleOutput($"[Crash] Last game ID from DB: {lastId}");
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Crash] Init DB error: {e}");
                }

                // Загружаем историю
                List<LastGame> cache = new List<LastGame>();
                try
                {
                    using (var cmd = new MySqlCommand(@"
SELECT gameId, crashAt, bank, players
FROM crash_game
WHERE crashAt IS NOT NULL
ORDER BY gameId DESC
LIMIT 40;"))
                    {
                        var table = MySQL.QueryRead(cmd);
                        if (table != null)
                        {
                            foreach (DataRow r in table.Rows)
                            {
                                cache.Add(new LastGame
                                {
                                    Id = Convert.ToInt32(r["gameId"]),
                                    CrashedAt = Convert.ToSingle(r["crashAt"]),
                                    Bank = Convert.ToInt32(r["bank"]),
                                    PlayersCount = Convert.ToInt32(r["players"])
                                });
                            }
                        }
                    }
                 //   NAPI.Util.ConsoleOutput($"[Crash] Loaded {cache.Count} history games");
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Crash] History load error: {e}");
                }

                // Возвращаемся в главный поток
                NAPI.Task.Run(() =>
                {
                    lock (Sync)
                    {
                        CurrentId = lastId + 1;
                    }

                    lock (LastGames)
                    {
                        LastGames.Clear();
                        LastGames.AddRange(cache);
                    }

                //    NAPI.Util.ConsoleOutput($"[Crash] Starting game {CurrentId}");

                    Db_SaveRoundStart(CurrentId, 0);
                    StartTimer(WAIT_BET_MS);
                });
            });
        }

        // ===== математика множителя =====
        // формула роста: X = 1.00006 ^ ms
        private static double Grow(double ms) => Math.Pow(1.00006, ms);
        private static double MsToHit(double x) => Math.Log(x) / Math.Log(1.00006);

        // ===== режимы =====
        private static void StartTimer(int waitMs)
        {
          //  NAPI.Util.ConsoleOutput($"[Crash] StartTimer: {waitMs}ms");

            lock (Sync)
            {
                TimerActive = true;
                GameActive = false;
                CrashedAt = null;
                TimerInitTimeMs = waitMs;
                TimerCreatedAt = DateTime.UtcNow;
                TimerStartAt = DateTime.UtcNow.AddMilliseconds(waitMs);
            }

            // ДОБАВЬТЕ ЭТУ СТРОКУ - запускаем рассылку во время таймера
            StartLoop(1000); // обновляем каждую секунду во время ожидания

            BroadcastAll();

            // Используем System.Timer вместо NAPI.Task.Run
            var gameTimer = new System.Timers.Timer(waitMs);
            gameTimer.Elapsed += (s, e) => {
                gameTimer.Dispose();
                NAPI.Task.Run(() => {
                    StopLoop(); // останавливаем рассылку таймера
                    StartGame();
                });
            };
            gameTimer.AutoReset = false;
            gameTimer.Start();
        }
        private static void StartGame()
        {
           // NAPI.Util.ConsoleOutput("[Crash] StartGame called");

            lock (Sync)
            {
                TimerActive = false;
                GameActive = true;
                GameStartAt = DateTime.UtcNow;
                CrashedAt = null;
            }

            // Генерируем точку краша
            var targetX = (Rnd.NextDouble() < 0.60) ? (1.0 + Rnd.NextDouble() * 2.0) : (2.5 + Rnd.NextDouble() * 7.5);
            var msToCrash = (int)Math.Ceiling(MsToHit(targetX));

           // NAPI.Util.ConsoleOutput($"[Crash] Game will crash at {targetX:F2}x in {msToCrash}ms");

            StartLoop(200);
            BroadcastAll();

            // Используем System.Timer для краша
            var crashTimer = new System.Timers.Timer(msToCrash);
            crashTimer.Elapsed += (s, e) => {
                crashTimer.Dispose();
                NAPI.Task.Run(() => EndGame(targetX));
            };
            crashTimer.AutoReset = false;
            crashTimer.Start();
        }

        private static void EndGame(double crashX)
        {
        //    NAPI.Util.ConsoleOutput($"[Crash] EndGame at {crashX:F2}x");

            int gameId;
            double crash;
            int bankFinal;
            int playersCnt;

            lock (Sync)
            {
                GameActive = false;
                CrashedAt = Math.Round(crashX, 2);

                gameId = CurrentId;
                crash = CrashedAt.Value;
                bankFinal = Current.Values.Sum(v => v.Bet);
                playersCnt = Current.Count;
            }

            StopLoop();
            BroadcastAll();

        //    NAPI.Util.ConsoleOutput($"[Crash] Game {gameId} ended. Bank: {bankFinal}, Players: {playersCnt}");

            // Сохранение в БД
            Db_SaveRoundEnd(gameId, (float)crash, bankFinal, playersCnt);

            // Обновление кеша истории
            lock (LastGames)
            {
                LastGames.Insert(0, new LastGame
                {
                    Id = gameId,
                    CrashedAt = (float)crash,
                    Bank = bankFinal,
                    PlayersCount = playersCnt
                });
                if (LastGames.Count > HISTORY_CACHE_LIMIT)
                    LastGames.RemoveAt(LastGames.Count - 1);
            }

            // Подготовка следующего раунда
            lock (Sync)
            {
                Current.Clear();
                CurrentId++;
            }

            Db_SaveRoundStart(CurrentId, 0);

            // Пауза 5 секунд перед следующим раундом
            var nextRoundTimer = new System.Timers.Timer(5000);
            nextRoundTimer.Elapsed += (s, e) => {
                nextRoundTimer.Dispose();
                NAPI.Task.Run(() => StartTimer(WAIT_BET_MS));
            };
            nextRoundTimer.AutoReset = false;
            nextRoundTimer.Start();
        }

        // ===== периодическая рассылка живых снапшотов =====
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
            // персонально каждому — так проще держать main thread
            foreach (var pl in NAPI.Pools.GetAllPlayers())
            {
                var ep = pl as ExtPlayer;
                if (ep == null || !ep.IsCharacterData()) continue;
                BroadcastTo(ep);
            }
        }

        private static void BroadcastTo(ExtPlayer p)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    var ch = p.GetCharacterData();
                    if (ch == null)
                    {
                 //       NAPI.Util.ConsoleOutput($"[Crash] BroadcastTo: character data is null for {p.Name}");
                        return;
                    }

                    // Собираем все данные
                    List<object> players;
                    int gameId;
                    bool timerActive, gameActive;
                    double? crashedAt;
                    DateTime timerStartAt, gameStartTime;
                    int remainingMs = 0;

                    List<object> historyPreview;
                    lock (LastGames)
                    {
                        historyPreview = LastGames
                            .Take(HISTORY_PREVIEW_LIMIT)
                            .Select(h => new
                            {
                                id = h.Id,
                                crashedAt = h.CrashedAt
                            })
                            .Cast<object>()
                            .ToList();
                    }

                    lock (Sync)
                    {
                        gameId = CurrentId;
                        players = Current.Values.Select(v => new
                        {
                            login = v.Login,
                            accountId = v.UUID,
                            betAmount = v.Bet,
                            exit = v.ExitX
                        }).Cast<object>().ToList();

                        timerActive = TimerActive;
                        gameActive = GameActive;
                        crashedAt = CrashedAt;
                        timerStartAt = TimerStartAt;
                        gameStartTime = GameStartAt;

                        // Вычисляем оставшееся время для таймера
                        if (timerActive)
                        {
                            var elapsed = (DateTime.UtcNow - TimerCreatedAt).TotalMilliseconds;
                            remainingMs = Math.Max(0, (int)(TimerInitTimeMs - elapsed));
                        }
                    }

                    var dto = new
                    {
                        demo = false,
                        active = true,
                        timer = new
                        {
                            active = timerActive,
                            initTime = TimerInitTimeMs,
                            timeStart = timerStartAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            remainingMs = remainingMs  // Добавляем оставшееся время в миллисекундах
                        },
                        game = new
                        {
                            active = gameActive,
                            crashedAt = crashedAt,
                            gameStartTime = gameStartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            endGame = 0
                        },
                        currentGame = new
                        {
                            id = gameId,
                            players = players
                        },
                        history = historyPreview,
                        historyItem = new { game = (object)null, bets = new object[0] },
                        main = new
                        {
                            accountId = ch.UUID,
                            login = p.Name,
                            serverTimeOffset = 0
                        }
                    };

                    string json = JsonConvert.SerializeObject(dto);

                   // NAPI.Util.ConsoleOutput($"[Crash] Sending to {p.Name}: timer={timerActive}, game={gameActive}, remaining={remainingMs}ms");

                    Trigger.ClientEvent(p, "crashgames_update", json);
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Crash] BroadcastTo Exception for {p?.Name}: {e}");
                }
            });
        }

        // ===== Remote Events (клиент ↔ сервер) =====
        [RemoteEvent("server.crashgames.request")]
        public void CrashRequest(ExtPlayer player)
        {
            if (player.IsCharacterData()) BroadcastTo(player);
        }

        [RemoteEvent("server.crashgames.bet")]
        public void CrashBet(ExtPlayer player, int amount, float autoTakeX)
        {
            try
            {
                var acc = player.GetAccountData(); if (acc == null) return;
                var ch = player.GetCharacterData(); if (ch == null) return;

                lock (Sync)
                {
                    if (!TimerActive)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Ставки принимаются только во время ожидания раунда", 3000);
                        return;
                    }

                    // Проверяем, есть ли уже ставка у этого игрока
                    if (Current.ContainsKey(ch.UUID))
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

                // списываем коины и фиксируем ставку
                UpdateData.RedBucks(player, -amount, msg: "Мини-игра 'Краш': ставка");

                lock (Sync)
                {
                    Current[ch.UUID] = new CrashStake
                    {
                        UUID = ch.UUID,
                        Login = player.Name,
                        Bet = amount,
                        ExitX = null,
                        Paid = false
                    };
                }

                // в БД — новая ставка
                Db_SaveBet(CurrentId, ch.UUID, player.Name, amount, autoTakeX > 1 ? (float?)autoTakeX : null);

                BroadcastAll();

          //      NAPI.Util.ConsoleOutput($"[Crash] {player.Name} placed bet: {amount} coins, autoTake: {autoTakeX}x");
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Crash] Bet Exception: {e}");
            }
        }

        [RemoteEvent("server.crashgames.take")]
        public void CrashTake(ExtPlayer player)
        {
            try
            {
                var acc = player.GetAccountData();
                if (acc == null)
                {
                    NAPI.Util.ConsoleOutput($"[Crash] CrashTake: acc is null for {player?.Name}");
                    return;
                }

                var ch = player.GetCharacterData();
                if (ch == null)
                {
                    NAPI.Util.ConsoleOutput($"[Crash] CrashTake: ch is null for {player?.Name}");
                    return;
                }

                float exit;
                int payout;

                lock (Sync)
                {
                    if (!GameActive)
                    {
                        NAPI.Util.ConsoleOutput($"[Crash] CrashTake: game not active for {player.Name}");
                        return;
                    }

                    if (!Current.TryGetValue(ch.UUID, out var st))
                    {
                        NAPI.Util.ConsoleOutput($"[Crash] CrashTake: no stake found for {player.Name}");
                        return;
                    }

                    if (st.Paid || st.ExitX != null)
                    {
                        NAPI.Util.ConsoleOutput($"[Crash] CrashTake: already paid or exited for {player.Name}");
                        return;
                    }

                    // считаем X строго по серверному времени
                    var x = Grow((DateTime.UtcNow - GameStartAt).TotalMilliseconds);
                    exit = (float)Math.Round(Math.Max(1.0, x), 2);

                    st.ExitX = exit;
                    st.Paid = true;

                    payout = Convert.ToInt32(Math.Floor(st.Bet * exit));
                }

                UpdateData.RedBucks(player, payout, msg: "Мини-игра 'Краш': выигрыш");
                BattlePass.Repository.UpdateReward(player, 3);

                Db_UpdateCashout(CurrentId, ch.UUID, exit, payout);

                BroadcastAll();

                NAPI.Util.ConsoleOutput($"[Crash] {player.Name} took winnings: {exit}x = {payout} coins");
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Crash] Take Exception for {player?.Name}: {e}");
            }
        }

        // маршрутизатор с клиента (index.svelte вызывает client.crash.openPage)
        [RemoteEvent("server.crashgames.openPage")]
        public void CrashOpenPage(ExtPlayer player, int page, int gameId = 0)
        {
            if (page == 1) HistoryListInternal(player);
            else if (page == 2) HistoryItemInternal(player, gameId);
            else BroadcastTo(player);
        }

        [RemoteEvent("server.crashgames.history")]
        public void CrashHistory(ExtPlayer player, int page, int gameId = 0)
        {
            if (page == 1) HistoryListInternal(player);
            else if (page == 2) HistoryItemInternal(player, gameId);
            else BroadcastTo(player);
        }

        // (альтернативные явные события, если удобно дергать напрямую)
        [RemoteEvent("server.crash.history.list")]
        public void HistoryList(ExtPlayer player) => HistoryListInternal(player);

        [RemoteEvent("server.crash.history.item")]
        public void HistoryItem(ExtPlayer player, int gameId) => HistoryItemInternal(player, gameId);

        private static void HistoryListInternal(ExtPlayer p)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    var ch = p.GetCharacterData();
                    if (ch == null) return;

                    // сперва попробуем кеш
                    List<object> list = null;
                    lock (LastGames)
                    {
                        if (LastGames.Count > 0)
                        {
                            list = LastGames
                                .OrderByDescending(x => x.Id)
                                .Take(100)
                                .Select(h => new { id = h.Id, crashedAt = h.CrashedAt, bank = h.Bank, playersCount = h.PlayersCount })
                                .Cast<object>()
                                .ToList();
                        }
                    }

                    if (list == null || list.Count == 0)
                    {
                        // читаем из БД (синхронно, но в фоне)
                        Trigger.SetTask(() =>
                        {
                            var cmd = new MySqlCommand(@"
SELECT gameId AS id, crashAt AS crashedAt, bank, players AS playersCount
FROM crash_game
WHERE crashAt IS NOT NULL
ORDER BY gameId DESC
LIMIT 100;");
                            DataTable table = MySQL.QueryRead(cmd);
                            var l = new List<object>();
                            if (table != null)
                            {
                                foreach (DataRow r in table.Rows)
                                {
                                    l.Add(new
                                    {
                                        id = Convert.ToInt32(r["id"]),
                                        crashedAt = Convert.ToDouble(r["crashedAt"]),
                                        bank = Convert.ToInt32(r["bank"]),
                                        playersCount = Convert.ToInt32(r["playersCount"])
                                    });
                                }
                            }

                            NAPI.Task.Run(() =>
                            {
                                var dto = new
                                {
                                    active = true,
                                    pages = new[] { "main", "historyList", "historyItem" },
                                    openedPage = 1,
                                    history = l,
                                    historyItem = new { game = (object)null, bets = new object[0] },
                                    main = new
                                    {
                                        accountId = ch.UUID,
                                        login = p.Name,
                                        serverTimeOffset = 0
                                    }
                                };
                                Trigger.ClientEvent(p, "crashgames_update", JsonConvert.SerializeObject(dto));
                            });
                        });

                        return;
                    }

                    // отдаем кешированные
                    var dtoCached = new
                    {
                        active = true,
                        pages = new[] { "main", "historyList", "historyItem" },
                        openedPage = 1,
                        history = list,
                        historyItem = new { game = (object)null, bets = new object[0] },
                        main = new
                        {
                            accountId = ch.UUID,
                            login = p.Name,
                            serverTimeOffset = 0
                        }
                    };
                    Trigger.ClientEvent(p, "crashgames_update", JsonConvert.SerializeObject(dtoCached));
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Crash] HistoryList Exception: {e}");
                }
            });
        }

        private static void HistoryItemInternal(ExtPlayer p, int gameId)
        {
            // читаем БД в фоне
            Trigger.SetTask(() =>
            {
                try
                {
                    // карточка игры
                    var gameCmd = new MySqlCommand(@"
SELECT gameId AS id, crashAt AS crashedAt, bank, players AS playersCount
FROM crash_game
WHERE gameId = @gid
LIMIT 1;");
                    gameCmd.Parameters.AddWithValue("@gid", gameId);
                    var gameTable = MySQL.QueryRead(gameCmd);

                    // ставки этой игры
                    var betsCmd = new MySqlCommand(@"
SELECT uuid AS accountId, login, betAmount AS bet, exitX
FROM crash_bets
WHERE gameId = @gid
ORDER BY id ASC;");
                    betsCmd.Parameters.AddWithValue("@gid", gameId);
                    var betsTable = MySQL.QueryRead(betsCmd);

                    if (gameTable == null || gameTable.Rows.Count == 0)
                    {
                        // не нашли — вернём пусто
                        NAPI.Task.Run(() =>
                        {
                            var dtoEmpty = new
                            {
                                active = true,
                                pages = new[] { "main", "historyList", "historyItem" },
                                openedPage = 2,
                                history = new object[0],
                                historyItem = new { game = (object)null, bets = new object[0] },
                                main = new
                                {
                                    accountId = p.GetCharacterData()?.UUID ?? 0,
                                    login = p.Name,
                                    serverTimeOffset = 0
                                }
                            };
                            Trigger.ClientEvent(p, "crashgames_update", JsonConvert.SerializeObject(dtoEmpty));
                        });
                        return;
                    }

                    var gRow = gameTable.Rows[0];
                    var game = new
                    {
                        id = Convert.ToInt32(gRow["id"]),
                        crashedAt = Convert.ToDouble(gRow["crashedAt"]),
                        bank = Convert.ToInt32(gRow["bank"]),
                        playersCount = Convert.ToInt32(gRow["playersCount"])
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
                                bet = Convert.ToInt32(r["bet"]),
                                exitX = r["exitX"] == DBNull.Value ? (double?)null : Convert.ToDouble(r["exitX"])
                            });
                        }
                    }

                    NAPI.Task.Run(() =>
                    {
                        var dto = new
                        {
                            active = true,
                            pages = new[] { "main", "historyList", "historyItem" },
                            openedPage = 2,
                            history = new object[0],
                            historyItem = new { game, bets },
                            main = new
                            {
                                accountId = p.GetCharacterData()?.UUID ?? 0,
                                login = p.Name,
                                serverTimeOffset = 0
                            }
                        };
                        Trigger.ClientEvent(p, "crashgames_update", JsonConvert.SerializeObject(dto));
                    });
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Crash] HistoryItem Exception: {e}");
                }
            });
        }

        [Command("crashrestart", GreedyArg = true)]
        public void RestartCrash(ExtPlayer player)
        {
            NAPI.Util.ConsoleOutput("[Crash] Manual restart by admin");
            Init();
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Crash game restarted", 3000);
        }

        // (опционально) возврат ставки при выходе игрока в ожидании
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                var ch = player.GetCharacterData(); if (ch == null) return;

                lock (Sync)
                {
                    if (TimerActive && Current.TryGetValue(ch.UUID, out var st) && st.ExitX == null)
                    {
                        UpdateData.RedBucks(player, st.Bet, msg: "Краш — возврат ставки (выход)");
                        Current.Remove(ch.UUID);
                    }
                }
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Crash] OnPlayerDisconnected Exception: {e}");
            }
        }

        // ===== БД =====
        // предполагаемые таблицы:
        // crash_game(gameId PK, startAt DATETIME, endAt DATETIME, crashAt FLOAT, bank INT, players INT)
        // crash_bets(id AI PK, gameId INT, uuid INT, login VARCHAR(50), betAmount INT,
        //            autoTake FLOAT NULL, exitX FLOAT NULL, paid INT DEFAULT 0,
        //            createdAt DATETIME, paidAt DATETIME NULL)

        private static void Db_SaveRoundStart(int gameId, int bankNow)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
INSERT INTO `crash_game`
    (`gameId`,`startAt`,`bank`,`players`)
VALUES
    (@gid, NOW(), @bank, 0)
ON DUPLICATE KEY UPDATE
    `startAt` = VALUES(`startAt`),
    `bank`    = VALUES(`bank`);");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@bank", bankNow);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Crash] Db_SaveRoundStart: {e}"); }
        }

        private static void Db_SaveRoundEnd(int gameId, float crashAt, int bankFinal, int playersCnt)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
UPDATE `crash_game`
SET
    `endAt`   = NOW(),
    `crashAt` = @crash,
    `bank`    = @bank,
    `players` = @pcnt
WHERE `gameId` = @gid;");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@crash", crashAt);
                    cmd.Parameters.AddWithValue("@bank", bankFinal);
                    cmd.Parameters.AddWithValue("@pcnt", playersCnt);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Crash] Db_SaveRoundEnd: {e}"); }
        }

        private static void Db_SaveBet(int gameId, int uuid, string login, int betAmount, float? autoTake)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
INSERT INTO `crash_bets`
    (`gameId`,`uuid`,`login`,`betAmount`,`autoTake`,`createdAt`)
VALUES
    (@gid, @uuid, @login, @bet, @auto, NOW());");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@login", login ?? "");
                    cmd.Parameters.AddWithValue("@bet", betAmount);
                    cmd.Parameters.AddWithValue("@auto", (object?)autoTake ?? DBNull.Value);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Crash] Db_SaveBet: {e}"); }
        }

        private static void Db_UpdateCashout(int gameId, int uuid, float exitX, int paid)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
UPDATE `crash_bets`
SET
    `exitX`  = @exitx,
    `paid`   = @paid,
    `paidAt` = NOW()
WHERE `gameId` = @gid AND `uuid` = @uuid;");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@exitx", exitX);
                    cmd.Parameters.AddWithValue("@paid", paid);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Crash] Db_UpdateCashout: {e}"); }
        }
    }
}