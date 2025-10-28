using GTANetworkAPI;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NeptuneEvo.MiniGames
{
    public class JackpotGame : Script
    {
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
            // Необязательно, но можно хранить:
            public int WinnerAbsoluteIndex;
            public int OriginalCardsLength;
        }

        private const int HISTORY_CACHE_LIMIT = 100;
        private const int WAIT_BET_MS = 10000;
        private const int TIME_GAME_MS = 8000;

        private static readonly Dictionary<int, int> PlayerPages = new Dictionary<int, int>();
        private static readonly List<HistoryGame> HistoryList = new List<HistoryGame>();
        private static readonly object Sync = new object();
        private static readonly List<JackpotBet> CurrentBets = new List<JackpotBet>();
        private static readonly Random Rnd = new Random();

        private static bool TimerActive;
        private static DateTime TimerStartAt;
        private static DateTime TimerCreatedAt;
        private static int TimerInitTimeMs = 0;
        private static bool GameActive;
        private static DateTime GameStartAt;
        private static DateTime GameStartTime = DateTime.UtcNow;
        private static WinnerData Winner;
        private static int WinnerCardIndex = 0;
        private static double WinnerPosition = 0.5;
        private static int CurrentId = 1;
        private static System.Timers.Timer BroadcastLoop;

        private static SpecialAchievement LuckOfDay = null;
        private static SpecialAchievement BigJackpot = null;
        private static HistoryGame LastGame = null;

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
                TimerActive = false;
                Winner = null;
                WinnerCardIndex = 0;
                WinnerPosition = 0.5;
            }

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
                    BroadcastAll();

                    NAPI.Util.ConsoleOutput("[Jackpot] Initialized, waiting for players...");
                });
            });
        }

        private static void LoadSpecialAchievements()
        {
            try
            {
                using (var cmd = new MySqlCommand(@"
SELECT winnerLogin, winnerGender, winnerAccountId, winnerServerName, winnerServerId, winnerChance, bank
FROM jackpot_game
WHERE winner IS NOT NULL
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

        private static void StartTimer(int waitMs)
        {
            lock (Sync)
            {
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

            NAPI.Util.ConsoleOutput($"[Jackpot] Timer started for {waitMs / 1000} seconds");

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
            JackpotBet winnerBet = null;
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

                // Выбираем победителя
                winnerBet = SelectWinner(CurrentBets);

                if (winnerBet == null)
                {
                    NAPI.Util.ConsoleOutput("[Jackpot] ERROR: Winner is null!");
                    EndGame(null);
                    return;
                }

                totalBank = CurrentBets.Sum(b => b.Amount);
                double winnerChance = totalBank > 0 ? (double)winnerBet.Amount / totalBank * 100.0 : 0.0;

                NAPI.Util.ConsoleOutput($"[Jackpot] Winner: {winnerBet.Login} (UUID: {winnerBet.UUID})");
                NAPI.Util.ConsoleOutput($"[Jackpot] Winner chance: {winnerChance:F2}%");

                // Вычисляем количество карточек как на клиенте (Math.round -> AwayFromZero)
                int winnerCardCount = Math.Max(1, (int)Math.Round(winnerChance / 10.0, MidpointRounding.AwayFromZero));
                NAPI.Util.ConsoleOutput($"[Jackpot] Winner cards count: {winnerCardCount}");

                cardIndex = Rnd.Next(0, winnerCardCount);
                position = Rnd.NextDouble();

                WinnerCardIndex = cardIndex;
                WinnerPosition = position;

                NAPI.Util.ConsoleOutput($"[Jackpot] Selected card index: {cardIndex} of {winnerCardCount}");
                NAPI.Util.ConsoleOutput($"[Jackpot] Position within card: {position:F3}");

                // Пересчёт структуры карточек (cardsPerCycle, cycles, originalLength)
                var sortedBets = CurrentBets.OrderByDescending(b => b.Amount).ToList();

                int cardsPerCycle = 0;
                var cardsCountPerBet = new List<int>();
                foreach (var b in sortedBets)
                {
                    double ch = totalBank > 0 ? (double)b.Amount / totalBank * 100.0 : 0.0;
                    int cnt = Math.Max(1, (int)Math.Round(ch / 10.0, MidpointRounding.AwayFromZero));
                    cardsCountPerBet.Add(cnt);
                    cardsPerCycle += cnt;
                }

                const int MIN_TOTAL_CARDS = 180;
                int cycles = Math.Max(1, (int)Math.Ceiling((double)MIN_TOTAL_CARDS / Math.Max(1, cardsPerCycle)));
                int originalLength = cardsPerCycle * cycles;
                int totalCycles = Math.Max(1, originalLength / Math.Max(1, cardsPerCycle));
                int lastCycleStart = (totalCycles - 1) * cardsPerCycle;

                NAPI.Util.ConsoleOutput($"[Jackpot] Server cardsPerCycle: {cardsPerCycle}, cycles: {cycles}, originalLength: {originalLength}, lastCycleStart: {lastCycleStart}");

                // Находим позиции победителя в последнем цикле
                int running = 0;
                var winnerPositionsInCycle = new List<int>();
                for (int i = 0; i < sortedBets.Count; i++)
                {
                    var b = sortedBets[i];
                    int cnt = cardsCountPerBet[i];
                    if (b.UUID == winnerBet.UUID)
                    {
                        for (int p = 0; p < cnt; p++)
                            winnerPositionsInCycle.Add(running + p);
                    }
                    running += cnt;
                }

                if (winnerPositionsInCycle.Count == 0)
                {
                    NAPI.Util.ConsoleOutput("[Jackpot] ERROR: winner has no positions in last cycle (server calculation)");
                    winnerPositionsInCycle.Add(0);
                }

                int posInLastCycle = winnerPositionsInCycle[Math.Min(cardIndex, winnerPositionsInCycle.Count - 1)];
                int winnerAbsoluteIndex = lastCycleStart + posInLastCycle;

                NAPI.Util.ConsoleOutput($"[Jackpot] Winner posInLastCycle: {posInLastCycle}, winnerAbsoluteIndex: {winnerAbsoluteIndex}");

                // Назначаем билеты
                AssignTickets(CurrentBets);

                Winner = new WinnerData
                {
                    UUID = winnerBet.UUID,
                    Login = winnerBet.Login,
                    Gender = winnerBet.Gender,
                    AccountId = winnerBet.UUID,
                    ServerId = winnerBet.ServerId,
                    ServerName = winnerBet.ServerName,
                    BetAmount = winnerBet.Amount,
                    WinCoins = totalBank,
                    WinChance = winnerChance,
                    Ticket = winnerBet.Ticket,
                    WinnerCardIndex = cardIndex,
                    WinnerPosition = position,
                    WinnerAbsoluteIndex = winnerAbsoluteIndex,
                    OriginalCardsLength = originalLength
                };
            }

            StartLoop(200);
            BroadcastAll();

            NAPI.Util.ConsoleOutput($"[Jackpot] Game will run for {TIME_GAME_MS / 1000} seconds");

            var gameTimer = new System.Timers.Timer(TIME_GAME_MS);
            gameTimer.Elapsed += (s, e) => {
                gameTimer.Dispose();
                NAPI.Task.Run(() => {
                    NAPI.Util.ConsoleOutput("[Jackpot] Game time elapsed, ending game");
                    EndGame(CurrentBets.FirstOrDefault(b => b.UUID == Winner?.UUID));
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
                Db_SaveTickets(gameId, allBets);

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

                var winChance = (double)winner.Amount / bankFinal * 100;

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

            StartLoop(500);
            BroadcastAll();

            Db_SaveRoundEnd(gameId, winner, bankFinal, playersCnt);

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

            var showResultTimer = new System.Timers.Timer(3000);
            showResultTimer.Elapsed += (s, e) => {
                showResultTimer.Dispose();
                NAPI.Task.Run(() => {
                    lock (Sync)
                    {
                        CurrentBets.Clear();
                        CurrentId++;
                    }

                    Db_SaveRoundStart(CurrentId);

                    var nextRoundTimer = new System.Timers.Timer(500);
                    nextRoundTimer.Elapsed += (s2, e2) => {
                        nextRoundTimer.Dispose();
                        NAPI.Task.Run(() => {
                            NAPI.Util.ConsoleOutput("[Jackpot] Ready for next round");
                            BroadcastAll();
                        });
                    };
                    nextRoundTimer.AutoReset = false;
                    nextRoundTimer.Start();
                });
            };
            showResultTimer.AutoReset = false;
            showResultTimer.Start();
        }

        private static void AssignTickets(List<JackpotBet> bets)
        {
            int totalAmount = bets.Sum(b => b.Amount);
            int ticketCounter = 1;

            var sortedBets = bets.OrderByDescending(b => b.Amount).ToList();

            foreach (var bet in sortedBets)
            {
                bet.Ticket = ticketCounter;
                double chance = (double)bet.Amount / totalAmount * 100;

                int ticketsCount = Math.Max(1, (int)Math.Round(chance / 10.0, MidpointRounding.AwayFromZero));

                ticketCounter += ticketsCount;

                NAPI.Util.ConsoleOutput($"[Jackpot] Player {bet.Login}: chance={chance:F2}%, tickets={ticketsCount}, firstTicket={bet.Ticket}");
            }

            NAPI.Util.ConsoleOutput($"[Jackpot] Assigned tickets, total: {ticketCounter - 1}");
        }

        private static JackpotBet SelectWinner(List<JackpotBet> bets)
        {
            int totalAmount = bets.Sum(b => b.Amount);
            int winningNumber = Rnd.Next(0, totalAmount);

            NAPI.Util.ConsoleOutput($"[Jackpot] Total bank: {totalAmount}");
            NAPI.Util.ConsoleOutput($"[Jackpot] Winning number: {winningNumber}");

            int cumulativeSum = 0;
            foreach (var bet in bets)
            {
                cumulativeSum += bet.Amount;
                NAPI.Util.ConsoleOutput($"[Jackpot] Checking {bet.Login}: range 0-{cumulativeSum}");

                if (winningNumber < cumulativeSum)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Winner selected: {bet.Login} (UUID: {bet.UUID})");
                    return bet;
                }
            }

            NAPI.Util.ConsoleOutput("[Jackpot] WARNING: No winner found in loop, returning last bet");
            return bets.LastOrDefault();
        }

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

                    List<object> currentBets;
                    int gameId;
                    bool timerActive, gameActive;
                    DateTime timerStartAt, gameStartTime;
                    int remainingMs = 0;
                    object winnerDto = null;

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

                            // Пересчитаем структуру карточек и найдем winnerAbsoluteIndex и originalLength
                            var sortedBets2 = CurrentBets.OrderByDescending(b => b.Amount).ToList();
                            int cardsPerCycle2 = 0;
                            var counts2 = new List<int>();
                            foreach (var b in sortedBets2)
                            {
                                double ch2 = totalBank > 0 ? (double)b.Amount / totalBank * 100.0 : 0.0;
                                int cnt2 = Math.Max(1, (int)Math.Round(ch2 / 10.0, MidpointRounding.AwayFromZero));
                                counts2.Add(cnt2);
                                cardsPerCycle2 += cnt2;
                            }

                            int cycles2 = Math.Max(1, (int)Math.Ceiling(180.0 / Math.Max(1, cardsPerCycle2)));
                            int originalLength2 = cardsPerCycle2 * cycles2;
                            int totalCycles2 = Math.Max(1, originalLength2 / Math.Max(1, cardsPerCycle2));
                            int lastCycleStart2 = (totalCycles2 - 1) * cardsPerCycle2;

                            int running2 = 0;
                            var winnerPositionsInCycle2 = new List<int>();
                            for (int i = 0; i < sortedBets2.Count; i++)
                            {
                                var b = sortedBets2[i];
                                int cnt = counts2[i];
                                if (b.UUID == Winner.UUID)
                                {
                                    for (int p = 0; p < cnt; p++)
                                        winnerPositionsInCycle2.Add(running2 + p);
                                }
                                running2 += cnt;
                            }

                            int posInCycleUsed = winnerPositionsInCycle2.Count > 0 ? winnerPositionsInCycle2[Math.Min(WinnerCardIndex, winnerPositionsInCycle2.Count - 1)] : 0;
                            int winnerAbsoluteIndexDto = lastCycleStart2 + posInCycleUsed;

                            // Формируем cardsMap — порядок и количество карточек для каждого игрока в одном цикле
                            var cardsMap = new List<object>();
                            for (int i = 0; i < sortedBets2.Count; i++)
                            {
                                var b = sortedBets2[i];
                                cardsMap.Add(new
                                {
                                    accountId = b.UUID,
                                    login = b.Login,
                                    serverId = b.ServerId,
                                    count = counts2[i],
                                    amount = b.Amount
                                });
                            }

                            winnerDto = new
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
                                winnerCardIndex = WinnerCardIndex,
                                winnerPosition = WinnerPosition,
                                winnerAbsoluteIndex = winnerAbsoluteIndexDto,
                                originalCardsLength = originalLength2,
                                cardsPerCycle = cardsPerCycle2,
                                cardsMap = cardsMap
                            };
                        }
                    }

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
                            timeGame = TIME_GAME_MS,
                            timeStart = gameStartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            winner = winnerDto,
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
                        pages = new[] { "main", "historyList", "historyItem" },
                        openedPage = currentPage,
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

        private static void StartLoop(int intervalMs)
        {
            StopLoop();
            BroadcastLoop = new System.Timers.Timer(intervalMs);
            BroadcastLoop.Elapsed += (s, e) => {
                NAPI.Task.Run(() => BroadcastAll());
            };
            BroadcastLoop.AutoReset = true;
            BroadcastLoop.Start();
        }

        private static void StopLoop()
        {
            if (BroadcastLoop != null)
            {
                BroadcastLoop.Stop();
                BroadcastLoop.Dispose();
                BroadcastLoop = null;
            }
        }

        private static void Db_SaveRoundStart(int gameId)
        {
            Trigger.SetTask(() =>
            {
                try
                {
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO jackpot_game (gameId, startedAt) VALUES (@gid, NOW()) " +
                        "ON DUPLICATE KEY UPDATE startedAt=NOW();"))
                    {
                        cmd.Parameters.AddWithValue("@gid", gameId);
                        MySQL.Query(cmd);
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveRoundStart error: {e}");
                }
            });
        }

        private static void Db_SaveBet(int gameId, int uuid, string login, int amount)
        {
            Trigger.SetTask(() =>
            {
                try
                {
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO jackpot_bets (gameId, uuid, login, amount) VALUES (@gid, @uuid, @login, @amt);"))
                    {
                        cmd.Parameters.AddWithValue("@gid", gameId);
                        cmd.Parameters.AddWithValue("@uuid", uuid);
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@amt", amount);
                        MySQL.Query(cmd);
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveBet error: {e}");
                }
            });
        }

        private static void Db_SaveTickets(int gameId, List<JackpotBet> bets)
        {
            Trigger.SetTask(() =>
            {
                try
                {
                    foreach (var bet in bets)
                    {
                        using (var cmd = new MySqlCommand(
                            "UPDATE jackpot_bets SET ticket=@ticket WHERE gameId=@gid AND uuid=@uuid;"))
                        {
                            cmd.Parameters.AddWithValue("@ticket", bet.Ticket);
                            cmd.Parameters.AddWithValue("@gid", gameId);
                            cmd.Parameters.AddWithValue("@uuid", bet.UUID);
                            MySQL.Query(cmd);
                        }
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveTickets error: {e}");
                }
            });
        }

        private static void Db_SaveRoundEnd(int gameId, JackpotBet winner, int bank, int players)
        {
            Trigger.SetTask(() =>
            {
                try
                {
                    if (winner != null)
                    {
                        double winChance = (double)winner.Amount / bank * 100;

                        using (var cmd = new MySqlCommand(@"
UPDATE jackpot_game 
SET winner=@uuid, 
    winnerLogin=@login, 
    winnerGender=@gender,
    winnerAccountId=@accId,
    winnerServerName=@srvName,
    winnerServerId=@srvId,
    winnerAmount=@amt, 
    bank=@bank, 
    winnerChance=@chance,
    winnerTicket=@ticket,
    winnerCardIndex=@cardIdx,
    winnerPosition=@cardPos,
    playersCount=@pCnt,
    endedAt=NOW()
WHERE gameId=@gid;"))
                        {
                            cmd.Parameters.AddWithValue("@uuid", winner.UUID);
                            cmd.Parameters.AddWithValue("@login", winner.Login);
                            cmd.Parameters.AddWithValue("@gender", winner.Gender);
                            cmd.Parameters.AddWithValue("@accId", winner.UUID);
                            cmd.Parameters.AddWithValue("@srvName", winner.ServerName);
                            cmd.Parameters.AddWithValue("@srvId", winner.ServerId);
                            cmd.Parameters.AddWithValue("@amt", winner.Amount);
                            cmd.Parameters.AddWithValue("@bank", bank);
                            cmd.Parameters.AddWithValue("@chance", winChance);
                            cmd.Parameters.AddWithValue("@ticket", winner.Ticket);
                            cmd.Parameters.AddWithValue("@cardIdx", WinnerCardIndex);
                            cmd.Parameters.AddWithValue("@cardPos", WinnerPosition);
                            cmd.Parameters.AddWithValue("@pCnt", players);
                            cmd.Parameters.AddWithValue("@gid", gameId);
                            MySQL.Query(cmd);
                        }
                    }
                    else
                    {
                        using (var cmd = new MySqlCommand(
                            "UPDATE jackpot_game SET bank=@bank, playersCount=@pCnt, endedAt=NOW() WHERE gameId=@gid;"))
                        {
                            cmd.Parameters.AddWithValue("@bank", bank);
                            cmd.Parameters.AddWithValue("@pCnt", players);
                            cmd.Parameters.AddWithValue("@gid", gameId);
                            MySQL.Query(cmd);
                        }
                    }
                }
                catch (Exception e)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Db_SaveRoundEnd error: {e}");
                }
            });
        }

        [RemoteEvent("server.jackpot.request")]
        public void JackpotRequest(ExtPlayer player)
        {
            if (!player.IsCharacterData()) return;

            var ch = player.GetCharacterData();
            if (ch == null) return;

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
                    if (GameActive)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Игра уже идет, дождитесь следующего раунда", 3000);
                        return;
                    }

                    if (CurrentBets.Count >= 50)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Достигнуто максимальное количество игроков (50)", 3000);
                        return;
                    }

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

                UpdateData.RedBucks(player, -amount, msg: "Мини-игра 'Джекпот': ставка");

                bool shouldStartTimer = false;
                bool shouldStartGame = false;

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

                    if (CurrentBets.Count == 2 && !TimerActive)
                    {
                        shouldStartTimer = true;
                    }

                    if (CurrentBets.Count >= 50)
                    {
                        shouldStartGame = true;
                    }
                }

                Db_SaveBet(CurrentId, ch.UUID, player.Name, amount);

                if (shouldStartGame)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Max players reached, starting game immediately");
                    StopLoop();
                    StartGame();
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                        $"Ставка {amount} монет принята. Игра начинается!", 3000);
                }
                else if (shouldStartTimer)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Starting timer (2 players reached)");
                    StartTimer(WAIT_BET_MS);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                        $"Ставка {amount} монет принята. Таймер запущен!", 3000);
                }
                else
                {
                    BroadcastAll();
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                        $"Ставка {amount} монет принята", 3000);
                }
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"[Jackpot] Bet Exception: {e}");
            }
        }

        [RemoteEvent("server.jackpot.openPage")]
        public void JackpotOpenPage(ExtPlayer player, int page, int gameId = 0)
        {
            NAPI.Util.ConsoleOutput($"[Jackpot] OpenPage called: page={page}, gameId={gameId}");

            var ch = player.GetCharacterData();
            if (ch == null) return;

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
                    var gameCmd = new MySqlCommand(@"
SELECT gameId AS id, winnerLogin, winnerGender, winnerAccountId, winnerServerName, winnerServerId,
       winnerAmount, bank, winnerChance, winnerTicket, winnerCardIndex, winnerPosition
FROM jackpot_game
WHERE gameId = @gid
LIMIT 1;");
                    gameCmd.Parameters.AddWithValue("@gid", gameId);
                    var gameTable = MySQL.QueryRead(gameCmd);

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
                if (GameActive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Игра уже идет, дождитесь следующего раунда", 3000);
                    return;
                }

                if (CurrentBets.Count >= 50)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Достигнуто максимальное количество игроков (50)", 3000);
                    return;
                }

                int botId = 9000 + Rnd.Next(1000, 9999);

                while (CurrentBets.Any(b => b.UUID == botId))
                {
                    botId = 9000 + Rnd.Next(1000, 9999);
                }

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

                if (CurrentBets.Count == 2 && !TimerActive)
                {
                    NAPI.Util.ConsoleOutput($"[Jackpot] Starting timer (2 players reached via bot)");
                    StartTimer(WAIT_BET_MS);
                }

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
                $"Добавлен бот '{CurrentBets[CurrentBets.Count - 1].Login}' (ID: {CurrentBets[CurrentBets.Count - 1].UUID}) с ставкой {amount} монет", 3000);
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
                if (GameActive)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                        "Игра уже идет, дождитесь следующего раунда", 3000);
                    return;
                }

                string[] botNames = new[] { "TestBot", "PlayerTest", "DemoUser", "BotPlayer", "TestUser", "Gambler", "LuckyOne", "ProPlayer", "Winner", "Casino" };

                for (int i = 0; i < count; i++)
                {
                    if (CurrentBets.Count >= 50)
                    {
                        NAPI.Util.ConsoleOutput($"[Jackpot] Reached max players while adding bots ({addedCount} bots added)");
                        break;
                    }

                    int botId = 9000 + Rnd.Next(1000, 9999);

                    while (CurrentBets.Any(b => b.UUID == botId))
                    {
                        botId = 9000 + Rnd.Next(1000, 9999);
                    }

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

                    if (CurrentBets.Count == 2 && !TimerActive)
                    {
                        shouldStartTimer = true;
                    }

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

            StopLoop();
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
    }
}