using System;
using System.Globalization;
using System.Threading;         // Для Timer
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using MySqlConnector;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using Redage.SDK;
using GTANetworkMethods;

namespace NeptuneEvo.MoneySystem
{
    // Класс для работы с балансом, начислением и расходами денег
    class Wallet : Script
    {
        private static readonly nLog Log = new nLog("MoneySystem.Wallet");

        public static bool Change(ExtPlayer player, long amount, bool isDisconnect = false)
        {
            try
            {
                var cd = player.GetCharacterData();
                if (cd == null) return false;

                long newMoney = (long)cd.Money + amount;
                if (newMoney < 0) return false;
                if (amount < 0 && Admin.IsServerStoping) return false;
                cd.Money = newMoney;

                if (Math.Abs(amount) >= 10000)
                    Database.Models.Money.AddMoneyUpdate(cd.UUID, cd.Money);

                if (amount >= 1)
                {
                    cd.EarnedMoney += (ulong)amount;
                    cd.EarnedMoneyDay += (ulong)amount;
                    cd.EarnedMoneyMonth += (ulong)amount;
                    MySQL.Query(
                        "UPDATE characters SET EarnedMoney=" + cd.EarnedMoney
                        + ",EarnedMoneyDay=" + cd.EarnedMoneyDay
                        + ",EarnedMoneyMonth=" + cd.EarnedMoneyMonth
                        + " WHERE UUID=" + cd.UUID
                    );
                }
                else if (amount < 0)
                {
                    ulong spent = (ulong)Math.Abs(amount);
                    cd.SpentMoney += spent;
                    cd.SpentMoneyDay += spent;
                    cd.SpentMoneyMonth += spent;
                    MySQL.Query(
                        "UPDATE characters SET SpentMoney=" + cd.SpentMoney
                        + ",SpentMoneyDay=" + cd.SpentMoneyDay
                        + ",SpentMoneyMonth=" + cd.SpentMoneyMonth
                        + " WHERE UUID=" + cd.UUID
                    );
                }

                if (!isDisconnect)
                    Trigger.ClientEvent(player, "client.charStore.Money", cd.Money);

                return true;
            }
            catch (Exception e)
            {
                Log.Write("Change Exception: " + e);
                return false;
            }
        }

        // Команда ручного сброса статистики заработка/трат
        // /resetstatmoney [1=Earned|2=Spent] [0=Total|1=Day|2=Month]
        [Command("resetstatmoney")]
        public void CMD_ResetStatMoney(ExtPlayer player, int statType, int periodType)
        {
            if (!player.IsCharacterData()) return;
            var cd = player.GetCharacterData();

            bool resetTotal = periodType == 0;
            bool resetDay = periodType == 1;
            bool resetMonth = periodType == 2;
            string statName;

            if (statType == 1) statName = "Заработано";
            else if (statType == 2) statName = "Потрачено";
            else
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                    "Используйте: /resetstatmoney 1-Заработано|2-Потрачено 0-Всего|1-День|2-Месяц", 3000);
                return;
            }

            var setParts = new System.Collections.Generic.List<string>();
            if (statType == 1)
            {
                if (resetTotal) { cd.EarnedMoney = 0; setParts.Add("EarnedMoney=0"); }
                if (resetDay) { cd.EarnedMoneyDay = 0; setParts.Add("EarnedMoneyDay=0"); }
                if (resetMonth) { cd.EarnedMoneyMonth = 0; setParts.Add("EarnedMoneyMonth=0"); }
            }
            else
            {
                if (resetTotal) { cd.SpentMoney = 0; setParts.Add("SpentMoney=0"); }
                if (resetDay) { cd.SpentMoneyDay = 0; setParts.Add("SpentMoneyDay=0"); }
                if (resetMonth) { cd.SpentMoneyMonth = 0; setParts.Add("SpentMoneyMonth=0"); }
            }

            var setClause = string.Join(",", setParts);
            MySQL.Query("UPDATE characters SET " + setClause + " WHERE UUID=" + cd.UUID);

            string periodName = resetTotal ? "за всё время" : resetDay ? "за день" : "за месяц";
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                statName + " " + periodName + " успешно сброшены.", 3000);
        }

        public static bool ChangeDonateBalance(ExtPlayer player, int Amount)
        {
            var cd = player.GetCharacterData();
            var ad = player.GetAccountData();
            if (cd == null) return false;
            int temp = Convert.ToInt32(ad.RedBucks + Amount);
            if (temp < 0) return false;
            ad.RedBucks = temp;
            MySQL.Query("UPDATE accounts SET redbucks=" + temp + " WHERE login='" + ad.Login + "'");
            return true;
        }

        public static int GetPriceToVip(ExtPlayer player, int price) => price;

        public static string Format<T>(T value) where T : IFormattable
        {
            long v = Convert.ToInt64(value);
            if (v >= -9 && v <= 9) return value.ToString();
            return value.ToString("0,0", CultureInfo.CreateSpecificCulture("el-GR"));
        }
    }

    // Класс для периодического сброса суточного и месячного заработка и расходов
    class EarningsReset : Script
    {
        private Timer _dailyTimer;
        private Timer _monthlyTimer;

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            // Ежедневный сброс в полночь
            _dailyTimer = new Timer(
                ResetDaily,
                null,
                (int)GetMillisecondsUntilMidnight(),
                (int)TimeSpan.FromDays(1).TotalMilliseconds
            );

            // Ежемесячный сброс: проверка каждый день, сброс при смене месяца
            _monthlyTimer = new Timer(
                ResetMonthly,
                null,
                (int)GetMillisecondsUntilMonthStart(),
                (int)TimeSpan.FromDays(1).TotalMilliseconds
            );
        }

        private void ResetDaily(object state)
        {
            NAPI.Util.ConsoleOutput("[EarningsReset] Daily reset");
            MySQL.Query("UPDATE characters SET EarnedMoneyDay=0,SpentMoneyDay=0");
            foreach (var p in NAPI.Pools.GetAllPlayers().Cast<ExtPlayer>())
            {
                var cd = p.GetCharacterData(); if (cd == null) continue;
                cd.EarnedMoneyDay = 0; cd.SpentMoneyDay = 0;
            }
        }

        private void ResetMonthly(object state)
        {
            NAPI.Util.ConsoleOutput("[EarningsReset] Monthly reset");
            MySQL.Query("UPDATE characters SET EarnedMoneyMonth=0,SpentMoneyMonth=0");
            foreach (var p in NAPI.Pools.GetAllPlayers().Cast<ExtPlayer>())
            {
                var cd = p.GetCharacterData(); if (cd == null) continue;
                cd.EarnedMoneyMonth = 0; cd.SpentMoneyMonth = 0;
            }
        }

        private double GetMillisecondsUntilMidnight()
        {
            var now = DateTime.Now;
            var midnight = now.Date.AddDays(1);
            var ms = (midnight - now).TotalMilliseconds;
            return ms < 0 ? 0 : ms > int.MaxValue ? int.MaxValue : ms;
        }

        private double GetMillisecondsUntilMonthStart()
        {
            var now = DateTime.Now;
            var next = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            var ms = (next - now).TotalMilliseconds;
            return ms < 0 ? 0 : ms > int.MaxValue ? int.MaxValue : ms;
        }
    }

    // Класс для обновления EarnedMoneyDay и SpentMoneyDay каждую минуту
    class EarnedSpentUpdater : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            new Timer(_ =>
            {
                foreach (var p in NAPI.Pools.GetAllPlayers().Cast<ExtPlayer>())
                {
                    var cd = p.GetCharacterData(); if (cd == null) continue;
                    Trigger.ClientEvent(p, "UpdateEarnedSpent", cd.EarnedMoneyDay, cd.SpentMoneyDay);
                }
            }, null, 0, 60000);
        }
    }
}