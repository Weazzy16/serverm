// serverside/WheelGame.cs
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
    /// Мини-игра WHEEL (Колесо фортуны).
    /// Потокобезопасность: любой вызов GTA-API (NAPI, Trigger.ClientEvent и т.п.) — только из главного потока через NAPI.Task.Run.
    /// Работа с БД — через Trigger.SetTask (внутри можно синхронно дергать MySQL.QueryRead/QueryAsync).
    /// </summary>
    public class WheelGame : Script
    {
        // ===== внутренняя модель ставки =====
        private class WheelStake
        {
            public int UUID;
            public string Login;
            public int Bet;
            public string Color;  // white, red, green, yellow
            public bool Paid;
        }

        // Кеш последней истории
        private class LastGame
        {
            public int Id;
            public string ColorWin;
            public int Bank;
            public int PlayersCount;
            public Dictionary<string, int> ColorBets; // white, red, green, yellow суммы
        }

        // Коэффициенты для цветов
        private static readonly Dictionary<string, float> ColorCoefficients = new Dictionary<string, float>
        {
            { "white", 2.0f },
            { "red", 3.0f },
            { "green", 5.0f },
            { "yellow", 14.0f }
        };

        // Шансы выпадения (в процентах)
        private static readonly Dictionary<string, int> ColorChances = new Dictionary<string, int>
        {
            { "white", 45 },  // 45%
            { "red", 45 },    // 45%
            { "green", 8 },   // 8%
            { "yellow", 2 }   // 2%
        };

        private const int HISTORY_PREVIEW_LIMIT = 15;
        private const int HISTORY_CACHE_LIMIT = 100;

        private static readonly List<LastGame> LastGames = new List<LastGame>();

        // ===== состояние цикла =====
        private static readonly object Sync = new object();

        // Ставки сгруппированы по цветам
        private static readonly Dictionary<string, List<WheelStake>> CurrentBets = new Dictionary<string, List<WheelStake>>
        {
            { "white", new List<WheelStake>() },
            { "red", new List<WheelStake>() },
            { "green", new List<WheelStake>() },
            { "yellow", new List<WheelStake>() }
        };

        private static bool TimerActive;           // идёт ожидание ставок
        private static DateTime TimerStartAt;      // когда стартует раунд (UTC/серверное)
        private static DateTime TimerCreatedAt;    // когда был создан таймер
        private static int TimerInitTimeMs = 0;    // "initTime" для клиента

        private static bool WheelActive;           // идёт вращение колеса
        private static DateTime WheelStartAt;      // старт вращения
        private static string WinnerColor;         // цвет победителя (после конца игры)
        private static int WheelSpinTime = 5000;   // время вращения колеса в мс
        private static float WheelStartRotation = 0; // начальная позиция колеса
        private static float WheelToRotation = 0;    // конечная позиция колеса

        private static int CurrentId = 1;          // id текущей игры

        private static System.Timers.Timer BroadcastLoop; // периодическая рассылка
        private static readonly Random Rnd = new Random();
        private const int WAIT_BET_MS = 16000;     // время приёма ставок

        // ===== жизненный цикл ресурса =====
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            NAPI.Util.ConsoleOutput("[Wheel] resource start");
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
                foreach (var color in CurrentBets.Keys.ToList())
                {
                    CurrentBets[color].Clear();
                }
                WheelActive = false;
                TimerActive = false;
                WinnerColor = null;
                WheelStartRotation = 0;
                WheelToRotation = 0;
            }

            // Читаем данные из БД в фоне
            Trigger.SetTask(() =>
            {
                int lastId = 0;

                try
                {
                    using (var cmd = new MySqlCommand("SELECT COALESCE(MAX(gameId), 0) AS lastId FROM wheel_game;"))
                    {
                        var table = MySQL.QueryRead(cmd);
                        if (table != null && table.Rows.Count > 0)
                            lastId = Convert.ToInt32(table.Rows[0]["lastId"]);
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Wheel] Init DB error: {e}");
                }

                // Загружаем историю
                List<LastGame> cache = new List<LastGame>();
                try
                {
                    using (var cmd = new MySqlCommand(@"
SELECT gameId, colorWin, bank, players, whiteBets, redBets, greenBets, yellowBets
FROM wheel_game
WHERE colorWin IS NOT NULL
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
                                    ColorWin = Convert.ToString(r["colorWin"]),
                                    Bank = Convert.ToInt32(r["bank"]),
                                    PlayersCount = Convert.ToInt32(r["players"]),
                                    ColorBets = new Dictionary<string, int>
                                    {
                                        { "white", Convert.ToInt32(r["whiteBets"]) },
                                        { "red", Convert.ToInt32(r["redBets"]) },
                                        { "green", Convert.ToInt32(r["greenBets"]) },
                                        { "yellow", Convert.ToInt32(r["yellowBets"]) }
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Wheel] History load error: {e}");
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

                    Db_SaveRoundStart(CurrentId);
                    StartTimer(WAIT_BET_MS);
                });
            });
        }

        // ===== Генерация результата колеса =====
        private static string GenerateWinnerColor()
        {
            var roll = Rnd.Next(1, 101); // 1-100
            var cumulative = 0;

            foreach (var kvp in ColorChances)
            {
                cumulative += kvp.Value;
                if (roll <= cumulative)
                    return kvp.Key;
            }

            return "white"; // fallback
        }

        private static float CalculateWheelRotation(string winnerColor)
        {
            // Реальные углы секторов с вашего SVG колеса
            var colorAngles = new Dictionary<string, float[]>
    {
        { "white", new[] { 20f, 30f, 55f, 60f, 95f, 105f, 135f, 165f, 200f, 205f, 255f, 260f, 295f, 325f, 330f } },
        { "red", new[] { 10f, 75f, 80f, 115f, 125f, 225f, 275f, 280f, 310f, 315f, 365f } },
        { "green", new[] { 40f, 45f, 145f, 150f, 160f, 240f, 245f, 345f, 355f } },
        { "yellow", new[] { 185f, 190f } }
    };

            if (colorAngles.ContainsKey(winnerColor))
            {
                var angles = colorAngles[winnerColor];
                var targetAngle = angles[Rnd.Next(angles.Length)];
                // Добавляем полные обороты для красивого вращения
                var fullRotations = Rnd.Next(5, 10) * 360;
                return fullRotations + targetAngle;
            }
            return 360f * Rnd.Next(5, 10); // fallback
        }

        // ===== режимы =====
        private static void StartTimer(int waitMs)
        {
            lock (Sync)
            {
                TimerActive = true;
                WheelActive = false;
                WinnerColor = null;
                TimerInitTimeMs = waitMs;
                TimerCreatedAt = DateTime.UtcNow;
                TimerStartAt = DateTime.UtcNow.AddMilliseconds(waitMs);
                WheelStartRotation = 0;
                WheelToRotation = 0;
            }

            StartLoop(1000); // обновляем каждую секунду во время ожидания
            BroadcastAll();

            var gameTimer = new System.Timers.Timer(waitMs);
            gameTimer.Elapsed += (s, e) => {
                gameTimer.Dispose();
                NAPI.Task.Run(() => {
                    StopLoop();
                    StartWheel();
                });
            };
            gameTimer.AutoReset = false;
            gameTimer.Start();
        }

        private static void StartWheel()
        {
            string winnerColor;
            float toRotation;

            lock (Sync)
            {
                TimerActive = false;
                WheelActive = true;
                WheelStartAt = DateTime.UtcNow;

                winnerColor = GenerateWinnerColor();
                WinnerColor = winnerColor;

                WheelStartRotation = WheelToRotation; // продолжаем с предыдущей позиции
                toRotation = WheelStartRotation + CalculateWheelRotation(winnerColor);
                WheelToRotation = toRotation;
            }

            StartLoop(200); // более частое обновление во время вращения
            BroadcastAll();

            var wheelTimer = new System.Timers.Timer(WheelSpinTime);
            wheelTimer.Elapsed += (s, e) => {
                wheelTimer.Dispose();
                NAPI.Task.Run(() => EndGame(winnerColor));
            };
            wheelTimer.AutoReset = false;
            wheelTimer.Start();
        }

        private static void EndGame(string winnerColor)
        {
            int gameId;
            int bankFinal;
            int playersCnt;
            var colorBets = new Dictionary<string, int>();
            var winners = new List<WheelStake>();

            lock (Sync)
            {
                WheelActive = false;
                WinnerColor = winnerColor;

                gameId = CurrentId;

                // Подсчитываем банк и игроков
                bankFinal = CurrentBets.Values.SelectMany(list => list).Sum(bet => bet.Bet);
                playersCnt = CurrentBets.Values.SelectMany(list => list)
                    .GroupBy(bet => bet.UUID)
                    .Count();

                // Подсчитываем ставки по цветам
                foreach (var colorKey in CurrentBets.Keys)
                {
                    colorBets[colorKey] = CurrentBets[colorKey].Sum(bet => bet.Bet);
                }

                // Находим победителей
                if (CurrentBets.ContainsKey(winnerColor))
                {
                    winners = CurrentBets[winnerColor].ToList();
                }
            }

            StopLoop();

            // Выплачиваем выигрыши
            foreach (var winner in winners)
            {
                var winAmount = Convert.ToInt32(Math.Floor(winner.Bet * ColorCoefficients[winnerColor]));

                var player = NAPI.Pools.GetAllPlayers()
                    .Cast<ExtPlayer>()
                    .FirstOrDefault(p => p.IsCharacterData() && p.GetCharacterData()?.UUID == winner.UUID);

                if (player != null)
                {
                    UpdateData.RedBucks(player, winAmount, msg: "Мини-игра 'Колесо': выигрыш");
                    BattlePass.Repository.UpdateReward(player, 3);

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                        $"Вы выиграли {winAmount} монет! Коэффициент: {ColorCoefficients[winnerColor]}x", 5000);
                }

                Db_UpdateWinner(gameId, winner.UUID, winAmount);
            }

            // ПОКАЗЫВАЕМ РЕЗУЛЬТАТ 2-3 СЕКУНДЫ ПЕРЕД ОЧИСТКОЙ
            StartLoop(500); // Частое обновление для показа результата
            BroadcastAll();

            // Сохранение в БД
            Db_SaveRoundEnd(gameId, winnerColor, bankFinal, playersCnt, colorBets);

            // Обновление кеша истории
            lock (LastGames)
            {
                LastGames.Insert(0, new LastGame
                {
                    Id = gameId,
                    ColorWin = winnerColor,
                    Bank = bankFinal,
                    PlayersCount = playersCnt,
                    ColorBets = colorBets
                });
                if (LastGames.Count > HISTORY_CACHE_LIMIT)
                    LastGames.RemoveAt(LastGames.Count - 1);
            }

            // ЗАДЕРЖКА 2-3 СЕКУНДЫ ПЕРЕД ОЧИСТКОЙ СТАВОК И НАЧАЛОМ НОВОЙ ИГРЫ
            var showResultTimer = new System.Timers.Timer(2500); // 2.5 секунды показываем результат
            showResultTimer.Elapsed += (s, e) => {
                showResultTimer.Dispose();
                NAPI.Task.Run(() => {
                    // ТЕПЕРЬ очищаем ставки и готовим следующий раунд
                    lock (Sync)
                    {
                        foreach (var color in CurrentBets.Keys.ToList())
                        {
                            CurrentBets[color].Clear();
                        }
                        CurrentId++;
                    }

                    Db_SaveRoundStart(CurrentId);

                    // Еще небольшая пауза перед следующим раундом
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

       private static void BroadcastTo(ExtPlayer p)
{
    NAPI.Task.Run(() =>
    {
        try
        {
            var ch = p.GetCharacterData();
            if (ch == null) return;

            // Собираем данные по цветам
            Dictionary<string, object> betsTypes;
            List<object> historyPreview;
            int gameId;
            bool timerActive, wheelActive;
            string winnerColor;
            DateTime timerStartAt, wheelStartTime;
            int remainingMs = 0;
            float wheelStartRotation, wheelToRotation;

            lock (LastGames)
            {
                historyPreview = LastGames
                    .Take(HISTORY_PREVIEW_LIMIT)
                    .Select(h => new
                    {
                        id = h.Id,
                        colorWin = h.ColorWin
                    })
                    .Cast<object>()
                    .ToList();
            }

            lock (Sync)
            {
                gameId = CurrentId;

                betsTypes = new Dictionary<string, object>();
                foreach (var colorKey in CurrentBets.Keys)
                {
                    var players = CurrentBets[colorKey].Select(bet => new
                    {
                        login = bet.Login,
                        accountId = bet.UUID,
                        amount = bet.Bet,
                        gender = "male",
                        serverName = "Server",
                        serverId = 1
                    }).ToList();

                    betsTypes[colorKey] = new
                    {
                        coefficient = ColorCoefficients[colorKey],
                        players = players
                    };
                }

                timerActive = TimerActive;
                wheelActive = WheelActive;
                winnerColor = WinnerColor;
                timerStartAt = TimerStartAt;
                wheelStartTime = WheelStartAt;
                wheelStartRotation = WheelStartRotation;
                wheelToRotation = WheelToRotation;

                if (timerActive)
                {
                    var elapsed = (DateTime.UtcNow - TimerCreatedAt).TotalMilliseconds;
                    remainingMs = Math.Max(0, (int)(TimerInitTimeMs - elapsed));
                }
            }

            // ИСПРАВЛЕНО: НЕ отправляем данные о страницах в основном broadcast
            // Это позволяет клиенту самостоятельно управлять навигацией
            var dto = new
            {
                demo = false,
                active = true,
                timer = new
                {
                    active = timerActive,
                    initTime = TimerInitTimeMs, // Отправляем оставшееся время
                    timeStart = TimerCreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    remainingMs = remainingMs
                },
                wheel = new
                {
                    active = wheelActive,
                    rotation = wheelStartRotation,
                    toRotation = wheelToRotation,
                    spinTime = WheelSpinTime,
                    timeStart = wheelStartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                },
                winner = winnerColor,
                currentGame = new
                {
                    id = gameId,
                    betsTypes = betsTypes
                },
                history = historyPreview,
                // УБИРАЕМ эти поля из основного broadcast:
                // historyList = new object[0],
                // historyItem = new { game = (object)null, bets = new object[0] },
                // pages = new[] { "main", "historyList", "historyItem" },
                // openedPage = 0,
                main = new
                {
                    accountId = ch.UUID,
                    login = p.Name,
                    serverName = "Server",
                    serverTimeOffset = 0
                }
            };

            string json = JsonConvert.SerializeObject(dto);
            Trigger.ClientEvent(p, "wheelgames_update", json);
        }
        catch (Exception e)
        {
            NAPI.Util.ConsoleOutput($"[Wheel] BroadcastTo Exception for {p?.Name}: {e}");
        }
    });
}

        // ===== Remote Events (клиент ↔ сервер) =====
        [RemoteEvent("server.wheelgames.request")]
        public void WheelRequest(ExtPlayer player)
        {
            if (player.IsCharacterData()) BroadcastTo(player);
        }

        [RemoteEvent("server.wheelgames.bet")]
        public void WheelBet(ExtPlayer player, int amount, string color)
        {
            try
            {
                var acc = player.GetAccountData(); if (acc == null) return;
                var ch = player.GetCharacterData(); if (ch == null) return;

                // Проверяем валидность цвета
                if (!CurrentBets.ContainsKey(color))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Неверный цвет ставки", 3000);
                    return;
                }

                lock (Sync)
                {
                    if (!TimerActive)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Ставки принимаются только во время ожидания раунда", 3000);
                        return;
                    }
                }

                if (amount <= 0 || acc.RedBucks < amount)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        Localization.LangFunc.GetText(Localization.LangType.Ru, Localization.DataName.NetRB), 3000);
                    return;
                }

                // списываем коины
                UpdateData.RedBucks(player, -amount, msg: "Мини-игра 'Колесо': ставка");

                lock (Sync)
                {
                    // ИСПРАВЛЕНО: ищем существующую ставку игрока на этот цвет
                    var existingBet = CurrentBets[color].FirstOrDefault(bet => bet.UUID == ch.UUID);

                    if (existingBet != null)
                    {
                        // Увеличиваем существующую ставку
                        existingBet.Bet += amount;
                    }
                    else
                    {
                        // Создаем новую ставку
                        CurrentBets[color].Add(new WheelStake
                        {
                            UUID = ch.UUID,
                            Login = player.Name,
                            Bet = amount,
                            Color = color,
                            Paid = false
                        });
                    }
                }

                // в БД — сохраняем/обновляем ставку
                Db_SaveOrUpdateBet(CurrentId, ch.UUID, player.Name, amount, color);

                BroadcastAll();
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Wheel] Bet Exception: {e}");
            }
        }

        // НОВАЯ функция для обновления ставки в БД
        private static void Db_SaveOrUpdateBet(int gameId, int uuid, string login, int betAmount, string color)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    // Сначала проверяем, есть ли уже ставка
                    var checkCmd = new MySqlCommand(@"
SELECT betAmount FROM wheel_bets 
WHERE gameId = @gid AND uuid = @uuid AND color = @color 
LIMIT 1;");
                    checkCmd.Parameters.AddWithValue("@gid", gameId);
                    checkCmd.Parameters.AddWithValue("@uuid", uuid);
                    checkCmd.Parameters.AddWithValue("@color", color);

                    var existingTable = MySQL.QueryRead(checkCmd);

                    if (existingTable != null && existingTable.Rows.Count > 0)
                    {
                        // Обновляем существующую ставку
                        var currentAmount = Convert.ToInt32(existingTable.Rows[0]["betAmount"]);
                        var newAmount = currentAmount + betAmount;

                        var updateCmd = new MySqlCommand(@"
UPDATE wheel_bets 
SET betAmount = @newAmount, createdAt = NOW() 
WHERE gameId = @gid AND uuid = @uuid AND color = @color;");
                        updateCmd.Parameters.AddWithValue("@gid", gameId);
                        updateCmd.Parameters.AddWithValue("@uuid", uuid);
                        updateCmd.Parameters.AddWithValue("@color", color);
                        updateCmd.Parameters.AddWithValue("@newAmount", newAmount);
                        await MySQL.QueryAsync(updateCmd);
                    }
                    else
                    {
                        // Создаем новую запись
                        var insertCmd = new MySqlCommand(@"
INSERT INTO wheel_bets
    (gameId, uuid, login, betAmount, color, createdAt)
VALUES
    (@gid, @uuid, @login, @bet, @color, NOW());");
                        insertCmd.Parameters.AddWithValue("@gid", gameId);
                        insertCmd.Parameters.AddWithValue("@uuid", uuid);
                        insertCmd.Parameters.AddWithValue("@login", login ?? "");
                        insertCmd.Parameters.AddWithValue("@bet", betAmount);
                        insertCmd.Parameters.AddWithValue("@color", color);
                        await MySQL.QueryAsync(insertCmd);
                    }
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Wheel] Db_SaveOrUpdateBet: {e}"); }
        }
        

        // Обработчики страниц
        [RemoteEvent("server.wheelgames.openPage")]
        public void WheelOpenPage(ExtPlayer player, int page, int gameId = 0)
        {
            if (page == 1) HistoryListInternal(player);
            else if (page == 2) HistoryItemInternal(player, gameId);
            else BroadcastTo(player);
        }

        [RemoteEvent("server.wheelgames.history")]
        public void WheelHistory(ExtPlayer player, int page, int gameId = 0)
        {
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
                    lock (LastGames)
                    {
                        if (LastGames.Count > 0)
                        {
                            list = LastGames
                                .OrderByDescending(x => x.Id)
                                .Take(50)
                                .Select(h => new
                                {
                                    id = h.Id,
                                    colorWin = h.ColorWin,
                                    bank = h.Bank,
                                    playersCount = h.PlayersCount,
                                    white = h.ColorBets["white"],
                                    red = h.ColorBets["red"],
                                    green = h.ColorBets["green"],
                                    yellow = h.ColorBets["yellow"]
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
SELECT gameId AS id, colorWin, bank, players AS playersCount, 
       whiteBets AS white, redBets AS red, greenBets AS green, yellowBets AS yellow
FROM wheel_game
WHERE colorWin IS NOT NULL
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
                                        colorWin = Convert.ToString(r["colorWin"]),
                                        bank = Convert.ToInt32(r["bank"]),
                                        playersCount = Convert.ToInt32(r["playersCount"]),
                                        white = Convert.ToInt32(r["white"]),
                                        red = Convert.ToInt32(r["red"]),
                                        green = Convert.ToInt32(r["green"]),
                                        yellow = Convert.ToInt32(r["yellow"])
                                    });
                                }
                            }

                            NAPI.Task.Run(() =>
                            {
                                // ИСПРАВЛЕНО: отправляем отдельное событие только для истории
                                var historyDto = new
                                {
                                    type = "historyList",
                                    historyList = l,
                                    openedPage = 1
                                };
                                Trigger.ClientEvent(p, "wheelgames_history", JsonConvert.SerializeObject(historyDto));
                            });
                        });
                        return;
                    }

                    // ИСПРАВЛЕНО: отправляем отдельное событие для истории
                    var dtoCached = new
                    {
                        type = "historyList",
                        historyList = list,
                        openedPage = 1
                    };
                    Trigger.ClientEvent(p, "wheelgames_history", JsonConvert.SerializeObject(dtoCached));
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Wheel] HistoryList Exception: {e}");
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
SELECT gameId AS id, colorWin, bank, players AS playersCount,
       whiteBets AS white, redBets AS red, greenBets AS green, yellowBets AS yellow
FROM wheel_game
WHERE gameId = @gid
LIMIT 1;");
                    gameCmd.Parameters.AddWithValue("@gid", gameId);
                    var gameTable = MySQL.QueryRead(gameCmd);

                    // ставки этой игры
                    var betsCmd = new MySqlCommand(@"
SELECT uuid AS accountId, login, betAmount AS amount, color, winAmount
FROM wheel_bets
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
                            Trigger.ClientEvent(p, "wheelgames_history", JsonConvert.SerializeObject(dtoEmpty));
                        });
                        return;
                    }

                    var gRow = gameTable.Rows[0];
                    var game = new
                    {
                        id = Convert.ToInt32(gRow["id"]),
                        colorWin = Convert.ToString(gRow["colorWin"]),
                        bank = Convert.ToInt32(gRow["bank"]),
                        playersCount = Convert.ToInt32(gRow["playersCount"]),
                        white = Convert.ToInt32(gRow["white"]),
                        red = Convert.ToInt32(gRow["red"]),
                        green = Convert.ToInt32(gRow["green"]),
                        yellow = Convert.ToInt32(gRow["yellow"])
                    };

                    // Группируем ставки по цветам
                    var betsByColor = new Dictionary<string, List<object>>
            {
                { "white", new List<object>() },
                { "red", new List<object>() },
                { "green", new List<object>() },
                { "yellow", new List<object>() }
            };

                    if (betsTable != null)
                    {
                        foreach (DataRow r in betsTable.Rows)
                        {
                            var color = Convert.ToString(r["color"]);
                            var bet = new
                            {
                                accountId = Convert.ToInt32(r["accountId"]),
                                login = Convert.ToString(r["login"] ?? ""),
                                amount = Convert.ToInt32(r["amount"]),
                                winAmount = r["winAmount"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["winAmount"]),
                                gender = "male", // можно добавить из профиля
                                serverName = "Server",
                                serverId = 1
                            };

                            if (betsByColor.ContainsKey(color))
                            {
                                betsByColor[color].Add(bet);
                            }
                        }
                    }

                    NAPI.Task.Run(() =>
                    {
                        var historyItemDto = new
                        {
                            type = "historyItem",
                            historyItem = new { game, bets = betsByColor },
                            openedPage = 2
                        };
                        Trigger.ClientEvent(p, "wheelgames_history", JsonConvert.SerializeObject(historyItemDto));
                    });
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Wheel] HistoryItem Exception: {e}");
                }
            });
        }

        [Command("wheelrestart", GreedyArg = true)]
        public void RestartWheel(ExtPlayer player)
        {
            NAPI.Util.ConsoleOutput("[Wheel] Manual restart by admin");
            Init();
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Wheel game restarted", 3000);
        }

        // Возврат ставки при выходе игрока во время ожидания
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                var ch = player.GetCharacterData(); if (ch == null) return;

                lock (Sync)
                {
                    if (TimerActive)
                    {
                        // Ищем ставку игрока во всех цветах
                        foreach (var colorBets in CurrentBets.Values)
                        {
                            var playerBet = colorBets.FirstOrDefault(bet => bet.UUID == ch.UUID);
                            if (playerBet != null)
                            {
                                UpdateData.RedBucks(player, playerBet.Bet, msg: "Колесо — возврат ставки (выход)");
                                colorBets.Remove(playerBet);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Wheel] OnPlayerDisconnected Exception: {e}");
            }
        }

        // ===== БД =====
        // предполагаемые таблицы:
        // wheel_game(gameId PK, startAt DATETIME, endAt DATETIME, colorWin VARCHAR(10), 
        //           bank INT, players INT, whiteBets INT, redBets INT, greenBets INT, yellowBets INT)
        // wheel_bets(id AI PK, gameId INT, uuid INT, login VARCHAR(50), betAmount INT,
        //           color VARCHAR(10), winAmount INT NULL, createdAt DATETIME, paidAt DATETIME NULL)

        private static void Db_SaveRoundStart(int gameId)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
INSERT INTO `wheel_game`
    (`gameId`,`startAt`,`bank`,`players`)
VALUES
    (@gid, NOW(), 0, 0)
ON DUPLICATE KEY UPDATE
    `startAt` = VALUES(`startAt`);");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Wheel] Db_SaveRoundStart: {e}"); }
        }

        private static void Db_SaveRoundEnd(int gameId, string colorWin, int bankFinal, int playersCnt, Dictionary<string, int> colorBets)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
UPDATE `wheel_game`
SET
    `endAt` = NOW(),
    `colorWin` = @colorWin,
    `bank` = @bank,
    `players` = @pcnt,
    `whiteBets` = @whiteBets,
    `redBets` = @redBets,
    `greenBets` = @greenBets,
    `yellowBets` = @yellowBets
WHERE `gameId` = @gid;");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@colorWin", colorWin);
                    cmd.Parameters.AddWithValue("@bank", bankFinal);
                    cmd.Parameters.AddWithValue("@pcnt", playersCnt);
                    cmd.Parameters.AddWithValue("@whiteBets", colorBets["white"]);
                    cmd.Parameters.AddWithValue("@redBets", colorBets["red"]);
                    cmd.Parameters.AddWithValue("@greenBets", colorBets["green"]);
                    cmd.Parameters.AddWithValue("@yellowBets", colorBets["yellow"]);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Wheel] Db_SaveRoundEnd: {e}"); }
        }

        private static void Db_SaveBet(int gameId, int uuid, string login, int betAmount, string color)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
INSERT INTO `wheel_bets`
    (`gameId`,`uuid`,`login`,`betAmount`,`color`,`createdAt`)
VALUES
    (@gid, @uuid, @login, @bet, @color, NOW());");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@login", login ?? "");
                    cmd.Parameters.AddWithValue("@bet", betAmount);
                    cmd.Parameters.AddWithValue("@color", color);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Wheel] Db_SaveBet: {e}"); }
        }

        private static void Db_UpdateWinner(int gameId, int uuid, int winAmount)
        {
            try
            {
                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
UPDATE `wheel_bets`
SET
    `winAmount` = @winAmount,
    `paidAt` = NOW()
WHERE `gameId` = @gid AND `uuid` = @uuid;");
                    cmd.Parameters.AddWithValue("@gid", gameId);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@winAmount", winAmount);
                    await MySQL.QueryAsync(cmd);
                });
            }
            catch (Exception e) { NAPI.Util.ConsoleOutput($"[Wheel] Db_UpdateWinner: {e}"); }
        }
    }
}