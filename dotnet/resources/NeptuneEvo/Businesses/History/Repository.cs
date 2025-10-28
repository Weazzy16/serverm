using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;

namespace NeptuneEvo.Businesses.History
{
    public class Repository
    {
        private static List<Businesshistories> InsertHistoryList = new List<Businesshistories>();
        private static readonly nLog Log = new nLog("Businesses.History.Repository");

        public static void AddHistory(int uuid, int bizId, string itemName, int cost)
        {
            InsertHistoryList.Add(new Businesshistories
            {
                Uuid = uuid,
                Bizid = bizId,
                Item = itemName,
                Price = cost,
                Date = DateTime.Now
            });
        }
        public class BusinessStatsDTO
        {
            [JsonProperty("income")]
            public decimal Income { get; set; }

            [JsonProperty("netProfit")]
            public decimal NetProfit { get; set; }

            [JsonProperty("averageCheck")]
            public decimal AverageCheck { get; set; }

            [JsonProperty("governmentPrice")]
            public decimal GovernmentPrice { get; set; }

            [JsonProperty("salesCount")]
            public decimal SalesCount { get; set; }

            [JsonProperty("profitability")]
            public decimal Profitability { get; set; }
        }

        public static async Task<Dictionary<string, BusinessStatsDTO>> GetBusinessStats(int bizId)
        {
            if (!BusinessManager.BizList.TryGetValue(bizId, out var business))
                return null;

            await using var db = new ServerBD("MainDB");

            var historyList = await db.Businesshistory
                .Where(bh => bh.Bizid == bizId)
                .OrderByDescending(bh => bh.Autoid)
                .Select(bh => new
                {
                    bh.Item,
                    bh.Uuid,
                    bh.Date,
                    bh.Price
                })
                .ToListAsync();

            BusinessStatsDTO GetData(int days)
            {
                var historyPerTime = historyList.Where(x => x.Date >= DateTime.Now.Subtract(new TimeSpan(days, 0, 0, 0)));

                var count = historyPerTime.Count();
                return new BusinessStatsDTO()
                {
                    Income = business.Pribil,
                    NetProfit = Math.Max(business.Pribil - business.Zatratq, 0),
                    AverageCheck = count == 0 ? 0 : Math.Round(historyPerTime.Select(x => (decimal)x.Price).Sum() / count),
                    GovernmentPrice = business.SellPrice,
                    SalesCount = historyPerTime.Count(),
                    Profitability = business.Zatratq == 0 ? 0 : Math.Max(Math.Round((decimal)business.Pribil / business.Zatratq * 100), 0),
                };
            }

            var result = new Dictionary<string, BusinessStatsDTO>();

            result.Add("yesterday", GetData(1));
            result.Add("week", GetData(7));
            result.Add("month", GetData(30));
            result.Add("quarter", GetData(GetDaysPassedOfCurrentQuarter()));
            result.Add("year", GetData(365));

            return result;
        }

        public static int GetDaysPassedOfCurrentQuarter()
        {
            DateTime today = DateTime.Today;
            DateTime startOfNextQuarter = GetStartOfNextQuarter(today);
            return (today - startOfNextQuarter).Days;
        }

        public static DateTime GetStartOfNextQuarter(DateTime date)
        {
            int year = date.Year;
            int month = (date.Month - 1) / 3 * 3 + 4;

            if (month > 12)
            {
                month = 1;
                year++;
            }

            return new DateTime(year, month, 1);
        }

        public static bool IsCountInsertHistory() => InsertHistoryList.Count > 0;

        public static async Task InsertHistory(ServerBD db)
        {
            try
            {
                var messages = InsertHistoryList.ToList();
                InsertHistoryList.Clear();

                foreach (var message in messages)
                {
                    await db.InsertAsync(message);
                }
            }
            catch (Exception e)
            {
                Log.Write($"InsertMessages Exception: {e.ToString()}");
            }
        }

        private static int MaxHistoryTime = 30;

        public static async void GetHistory(ExtPlayer player, int bizId)
        {

            try
            {
                await using var db = new ServerBD("MainDB");//В отдельном потоке

                var historyList = await db.Businesshistory
                    .Where(bh => bh.Bizid == bizId && bh.Date >= DateTime.Now.Subtract(new TimeSpan(MaxHistoryTime, 0, 0, 0)))
                    .OrderByDescending(bh => bh.Autoid)
                    .Select(bh => new
                    {
                        bh.Item,
                        bh.Uuid,
                        bh.Date,
                        bh.Price
                    })
                    .ToListAsync();

                var historyChart = new List<List<object>>();
                var historyProductsList = new List<List<List<object>>>();
                var historyUsersList = new List<List<List<object>>>();

                for (int i = 0; i < MaxHistoryTime; i++)
                {
                    var time = DateTime.Now;
                    time = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0).Subtract(new TimeSpan(i, 0, 0, 0));

                    var historyTime = $"{time.Day}.{time.Month}";

                    var historyProductCount = historyList
                        .Where(hl => hl.Date.Day == time.Day && hl.Date.Month == time.Month && hl.Date.Year == time.Year)
                        .Sum(hl => hl.Price);

                    historyChart.Add(new List<object>
                    {
                        historyTime,
                        historyProductCount
                    });

                    if (i == 0/* || i == 6 || i == 29*/)
                    {
                        var historyProducts = historyList
                            .Where(hl => hl.Date >= time)
                            .GroupBy(hl => hl.Item)
                            .Select(hl => new List<object>
                            {
                                hl.Key,
                                hl.Sum(v => v.Price)
                            })
                            .OrderByDescending(h => h[1])
                            .ToList();

                        historyProductsList.Add(historyProducts);

                        var historyUsers = historyList
                            .Where(hl => hl.Date >= time)
                            .GroupBy(hl => hl.Uuid)
                            .Select(hl => new List<object>
                            {
                                hl.Key,
                                hl.Sum(v => v.Price)
                            })
                            .OrderByDescending(h => h[1])
                            .ToList();

                        historyUsersList.Add(historyUsers);
                    }
                }

                Trigger.ClientEvent(player, "client.phone.business.initStats", JsonConvert.SerializeObject(historyChart), JsonConvert.SerializeObject(historyProductsList), JsonConvert.SerializeObject(historyUsersList));
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }
    }
}