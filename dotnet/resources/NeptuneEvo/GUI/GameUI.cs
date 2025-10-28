using GTANetworkAPI;
using System;
using System.Collections.Generic;
using NeptuneEvo.MoneySystem;
using System.Data;
using NeptuneEvo;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using NeptuneEvo.Accounts;
using NeptuneEvo.Fractions.Player;
using GTANetworkMethods;
using Newtonsoft.Json;
using System.Reflection.Emit;
using Localization;
using NeptuneEvo.Chars;
using NeptuneEvo.Players.Phone.Messages.Models;
using Database;
using NeptuneEvo.Accounts.Models;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Quests;
using System.Collections;
using System.Numerics;
using System.Xml.Linq;
using NeptuneEvo.Players;
using static LinqToDB.SqlQuery.SqlPredicate;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Asn1.X509;
using NeptuneEvo.Houses;
using NeptuneEvo.Fractions;
using NeptuneEvo.VehicleData.LocalData.Models;
using System.Reflection;
using NeptuneEvo.Chars.Models;

namespace NeptuneEvo.GUI
{
    class GameUI : Script
    {
        private static nLog Log = new nLog("GameUI");

        [RemoteEvent("server::playermenu:open")]
        public static void OpenPlayerMenu(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();

            if (sessionData == null) return;
            var characterData = player.GetCharacterData();

            if (characterData == null) return;

            var accountData = player.GetAccountData();
            var house = HouseManager.GetHouse(player);
            var garage = house?.GetGarageData();

            var inPark = false;
            var inGarage = false;
            var ownerName = String.Empty;
            var vehiclesNumber = new List<string>();
            List<object> listCars = new List<object>();
            if (house != null)
            {
                vehiclesNumber = house.GetVehiclesCarAndAirNumber();
                ownerName = house.Owner;

                inPark = garage != null && (garage.Type == -1 || garage.Type == 6);

                inGarage = garage != null && garage.InGarage(player);
            }
            else
            {
                vehiclesNumber = VehicleManager.GetVehiclesCarAndAirNumberToPlayer(sessionData.Name);
            }
            foreach (var num in vehiclesNumber)
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(num);
                if (vehicleData == null)
                    continue;

                var carData = new List<object>
                    {
                        vehicleData.Model, //model
                        0,
                        num,
                        vehicleData.Model
                    };
                listCars.Add(carData);
            }
            if (accountData == null) return;
            int bankid = characterData.Bank;
            string work = (characterData.WorkID > 0) ? Jobs.WorkManager.JobStats[characterData.WorkID - 1] : "Отсутствует";
            Houses.House h = Houses.HouseManager.GetHouse(player);
            string gethouse = h == null ? "" : $"Дом #{h.ID}";
            string house1 = h == null ? "Нет прописки" : gethouse;
            string fraction = Fractions.Manager.FractionNames[player.GetFractionId()];
            string fractionRank = player.GetFractionRankName();
            // Console.WriteLine($"{allMoney} - {allMoneyMinused}");
            Chars.Repository.PlayerStats(player);
            Trigger.ClientEvent(player, "client::playermenu:open", characterData.Money, Bank.Accounts[bankid].Balance, characterData.CreateDate.ToString(), characterData.LVL, characterData.EXP, characterData.Time.ToString(), house1, fraction, fractionRank, work, Group.GroupNames[accountData.VipLvl], accountData.VipDate.ToString(), accountData.RedBucks, accountData.Login, 0, 0, characterData.AdminLVL, characterData.UUID, 0, characterData.Warns, characterData.EarnedMoney, 0, JsonConvert.SerializeObject(listCars));
        }
        

        

        [RemoteEvent("server::donate:buyShark")]
        private static void Server_donate_buyshark(ExtPlayer player, int coins)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) return;

                var accountData = player.GetAccountData();

                if (accountData == null) return;
                switch (coins)
                {
                    case 100:
                        if (accountData.RedBucks <= 99)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 100;
                        MySQL.Query($"UPDATE accounts SET redbucks={accountData.RedBucks} WHERE login='{accountData.Login}'");
                        MoneySystem.Wallet.Change(player, 10000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Набор успешно куплен, $10 000 начислены вам наличными", 3000);
                        break;
                    case 500:
                        if (accountData.RedBucks <= 499)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 500;
                        MySQL.Query($"UPDATE accounts SET redbucks={accountData.RedBucks} WHERE login='{accountData.Login}'");
                        MoneySystem.Wallet.Change(player, 50000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Набор успешно куплен, $50 000 начислены вам наличными", 3000);
                        break;
                    case 2000:
                        if (accountData.RedBucks <= 1999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 2000;
                        MySQL.Query($"UPDATE accounts SET redbucks={accountData.RedBucks} WHERE login='{accountData.Login}'");
                        MoneySystem.Wallet.Change(player, 200000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Набор успешно куплен, $200 000 начислены вам наличными", 3000);
                        break;
                    case 5000:
                        if (accountData.RedBucks <= 4999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 5000;
                        MySQL.Query($"UPDATE accounts SET redbucks={accountData.RedBucks} WHERE login='{accountData.Login}'");
                        MoneySystem.Wallet.Change(player, 500000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Набор успешно куплен, $500 000 начислены вам наличными", 3000);
                        break;
                    case 20000:
                        if (accountData.RedBucks <= 19999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 20000;
                        MySQL.Query($"UPDATE accounts SET redbucks={accountData.RedBucks} WHERE login='{accountData.Login}'");
                        MoneySystem.Wallet.Change(player, 2000000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Набор успешно куплен, $2 000 000 начислены вам наличными", 3000);
                        break;
                    case 100000:
                        if (accountData.RedBucks <= 99999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 100000;
                        MySQL.Query($"UPDATE accounts SET redbucks={accountData.RedBucks} WHERE login='{accountData.Login}'");
                        MoneySystem.Wallet.Change(player, 10000000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Набор успешно куплен, $10 000 000 начислены вам наличными", 3000);
                        break;
                }
            }
            catch (Exception e) { Log.Write("Server_donate_buyshark: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("server::donate:buyCar")]
        private static void BuyCar(ExtPlayer player, string model)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var accountData = player.GetAccountData();

                if (accountData == null) return;
                ItemId nameWeapon = ItemId.CarCoupon;
                switch (model)

                {

                    case "bdivo":

                        var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                        if (vehiclesCount >= Houses.GarageManager.MaxGarageCars)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас максимальное кол-во машин", 3000);
                            return;
                        }
                        if (player.GetAccountData().RedBucks < 1500000 )
                        {
                            return;
                        }
                        accountData.RedBucks -= 1500000;
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Chars.Repository.AddNewItemWarehouse(player, nameWeapon, 1, model);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetItemSclad, model), 3000);
                        break;
                }
            }
            catch (Exception e)
            {

                Log.Write($"BuyPremium Exception: {e.ToString()}");
            }
        }

        [RemoteEvent("server::donate:buyClothes")]
        private static void BuyClothes(ExtPlayer player, int model)
        {
            try
            {

                var sessionData = player.GetSessionData();
                if (sessionData == null)
                    return;

                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                var accountData = player.GetAccountData();
                if (accountData == null)
                    return;
                string Data = $"{model}_0_{characterData.Gender}";
                switch (model)
                {
                    case 600:
                        if (accountData.RedBucks <= 100000)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Majestic Coins для покупки данного набора", 3000);
                            return;
                        }
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили свитшот Stone Island", 3000);
                        accountData.RedBucks -= 100000;
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Chars.Repository.AddNewItemWarehouse(player, ItemId.Top, 1, Data);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouGetItemSclad, "Руины [М]"), 3000);
                        break;
                }
            }
            catch (Exception e)
            {

                Log.Write($"BuyPremium Exception: {e.ToString()}");
            }
        }


   

        [RemoteEvent("server::donate:buyPrem")]
        private static void Server_donate_buyPrem(ExtPlayer player, int coins)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) return;

                var accountData = player.GetAccountData();

                if (accountData == null) return;
                switch (coins)
                {
                    case 250:
                        if (accountData.RedBucks <= 249)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 250;
                        accountData.VipLvl = 1;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Бронзовый аккаунт успешно куплен на 30 дней, он распространяется на весь аккаунт", 3000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                    case 500:
                        if (accountData.RedBucks <= 499)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 500;
                        accountData.VipLvl = 2;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Серебряный аккаунт успешно куплен на 30 дней, он распространяется на весь аккаунт", 3000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                    case 750:
                        if (accountData.RedBucks <= 749)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 750;
                        accountData.VipLvl = 3;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Золотой аккаунт успешно куплен на 30 дней, он распространяется на весь аккаунт", 3000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                    case 1000:
                        if (accountData.RedBucks <= 999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 1000;
                        accountData.VipLvl = 4;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Платинум аккаунт успешно куплен на 30 дней, он распространяется на весь аккаунт", 3000);
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                }
            }
            catch (Exception e) { Log.Write("Server_donate_buyPrem: " + e.Message, nLog.Type.Error); }
        }

        [RemoteEvent("server::donate:buyPacket")]
        private static void Server_donate_buyPacket(ExtPlayer player, int coins)
        {
            try
            {
                var characterData = player.GetCharacterData();

                if (characterData == null) return;

                var accountData = player.GetAccountData();

                if (accountData == null) return;
                if (characterData.LVL >= 5)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Купить стартовые наборы можно только до 5 уровня игрового персонажа", 3000);
                    return;
                }
                switch (coins)
                {
                    case 500:
                        if (accountData.RedBucks <= 499)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 500;
                        
                        MoneySystem.Wallet.Change(player, +25000);
                        accountData.VipLvl = 1;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        MySQL.Query($"UPDATE characters SET money={characterData.Money} WHERE uuid={characterData.UUID}");
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Бронзовый набор успешно куплен", 3000);
                        characterData.Licenses[1] = true;
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                    case 1000:
                        if (accountData.RedBucks <= 999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 1000;
                        
                        MoneySystem.Wallet.Change(player, +50000);
                        accountData.VipLvl = 2;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        MySQL.Query($"UPDATE characters SET money={characterData.Money} WHERE uuid={characterData.UUID}");
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Серебряный набор успешно куплен", 3000);
                        characterData.Licenses[0] = true; characterData.Licenses[1] = true;
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                    case 1500:
                        if (accountData.RedBucks <= 1499)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 1500;
                        
                        MoneySystem.Wallet.Change(player, +75000);

                        accountData.VipLvl = 3;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        MySQL.Query($"UPDATE characters SET money={characterData.Money} WHERE uuid={characterData.UUID}");
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Золотой набор успешно куплен", 3000);
                        characterData.Licenses[0] = true; characterData.Licenses[1] = true; characterData.Licenses[5] = true;
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                    case 2000:
                        if (accountData.RedBucks <= 1999)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Orlean Points для покупки данного набора", 3000);
                            return;
                        }
                        accountData.RedBucks -= 2000;
                        
                        MoneySystem.Wallet.Change(player, +100000);
                        accountData.VipLvl = 4;
                        accountData.VipDate = DateTime.Now.AddDays(30);
                        MySQL.Query($"UPDATE accounts SET viplvl={accountData.VipLvl},vipdate='{Convert.ToDateTime(accountData.VipDate)}' WHERE login='{accountData.Login}'");
                        MySQL.Query($"UPDATE characters SET money={characterData.Money} WHERE uuid={characterData.UUID}");
                        Trigger.ClientEvent(player, "client::playermenu:update", "OrleanPoints", accountData.RedBucks);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Платинум набор успешно куплен", 3000);
                        characterData.Licenses[0] = true; characterData.Licenses[1] = true; characterData.Licenses[5] = true; characterData.Licenses[2] = true;
                        Trigger.ClientEvent(player, "client::playermenu:update", "stats", JsonConvert.SerializeObject(GameUI.GetStats(player)));
                        break;
                }
            }
            catch (Exception e) { Log.Write("Server_donate_buyPacket: " + e.Message, nLog.Type.Error); }
        }

        public static List<object> GetStats(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            var accountData = player.GetAccountData();
            long bank = (characterData.Bank != 0) ? Bank.Accounts[characterData.Bank].Balance : 0;
            Houses.House h = Houses.HouseManager.GetHouse(player);
            string gethouse = h == null ? "" : $"Дом #{h.ID}";
            string house = h == null ? "Нет прописки" : gethouse;
            string work = (characterData.WorkID > 0) ? Jobs.WorkManager.JobStats[characterData.WorkID - 1] : "Безработный";
            string fraction = Fractions.Manager.FractionNames[player.GetFractionId()];
            string fractionRank = player.GetFractionRankName() != null ? player.GetFractionRankName() : "Отсутствует";

            string status = (accountData.VipLvl > 0) ? $"{Group.GroupNames[accountData.VipLvl]}" : $"{Group.GroupNames[accountData.VipLvl]}";

            List<object> data = new List<object>
            {
                characterData.LVL,
                characterData.EXP,
                Convert.ToInt32(3 + characterData.LVL *  3),
                characterData.CreateDate.ToString("dd.MM.yyyy"),
                characterData.Money,
                bank,
                24,//Age,
                house,
                work,
                "Не женат",
                fractionRank,
                fraction,
                status,
                accountData.VipLvl > 0 ? accountData.VipDate.ToString("dd.MM.yyyy") : null,
            };
            return data;
        }

        public class HouseInfo
        {
            public bool have { get; set; }
            public int ID { get; set; }
            public HouseInfo(bool h, int id)
            {
                have = h;
                ID = id;
            }
        }
        public class ApartamentInfo
        {
            public bool have { get; set; }
            public string name { get; set; }
            public ApartamentInfo(bool h, string n)
            {
                have = h;
                name = n;
            }
        }
        public class BusinessInfo
        {
            public bool have { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public BusinessInfo(bool h, int i, string n)
            {
                have = h;
                id = i;
                name = n;
            }
        }
        public class TransportInfo
        {
            public string Name { get; set; }
            public string Number { get; set; }
            public float Mile { get; set; }
            public TransportInfo(string name, string plate, float mile)
            {
                Name = name;
                Number = plate;
                Mile = mile;
            }
        }
    }
}
