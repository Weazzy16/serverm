using GTANetworkAPI;
using System;
using System.Collections.Generic;
using NeptuneEvo.EternalDev.Containers.Classes;
using NeptuneEvo.EternalDev.Containers.Enums;
using Newtonsoft.Json;
using System.Security.Cryptography;
using NeptuneEvo.Fractions.Hijacking;
using Discord;

namespace NeptuneEvo.EternalDev.Containers.Data
{
    public class Config
    {
        /// <summary>
        /// Название команды для старта аукциона
        /// </summary>
        [JsonProperty("command_start")]
        public const string COMMAND_START = "startcont";

        /// <summary>
        /// Минимальный уровень администратора для использования команд связанных с системой контейнеров
        /// </summary>
        [JsonProperty("admin_lvl")]
        public static readonly int ADMIN_LVL = 7;

        /// <summary>
        /// Название блипа
        /// </summary>
        [JsonProperty("blip_name")]
        public static readonly string BLIP_NAME = "Аукцион контейнеров";

        /// <summary>
        /// Спрайт блипа на карте
        /// Все спрайты и цвета - https://wiki.rage.mp/index.php?title=Blips
        /// </summary>
        [JsonProperty("blip_sprite")]
        public static readonly short BLIP_SPRITE = 568;

        /// <summary>
        /// Цвет блипа на карте, когда аукцион не проводится
        /// Все спрайты и цвета - https://wiki.rage.mp/index.php?title=Blips
        /// </summary>
        [JsonProperty("blip_inactive_color")]
        public static readonly byte BLIP_INACTIVE_COLOR = 4;

        /// <summary>
        /// Цвет блипа на карте, когда аукцион проводится
        /// Все спрайты и цвета - https://wiki.rage.mp/index.php?title=Blips
        /// </summary>
        [JsonProperty("blip_active_color")]
        public static readonly byte BLIP_ACTIVE_COLOR = 2;

        /// <summary>
        /// Зона проведения аукциона
        /// </summary>
        [JsonProperty("location")]
        public static readonly Vector3 LOCATION = new Vector3(884.9928, -2938.086, 5.900781);

        /// <summary>
        /// Продолжительность аукциона в минутах
        /// </summary>
        [JsonProperty("container_auction_duration")]
        public static readonly int CONTAINER_AUCTION_DURATION = 15;

        /// <summary>
        /// Время в минутах, в течении которого, победитель может забрать приз
        /// </summary>
        [JsonProperty("time_to_claim_prize")]
        public static readonly int TIME_TO_CLAIM_PRIZE = 3;

        /// <summary>
        /// Время в секундах, когда при ставках время будет увеличиваться
        /// </summary>
        [JsonProperty("time_rule_to_add_by_bet")]
        public static readonly int TIME_RULE_TO_ADD_BY_BET = 30;

        /// <summary>
        /// Время в секундах, когда при ставке увлечивается конец на текущее значение
        /// </summary>
        [JsonProperty("time_add_auction_by_bet")]
        public static readonly int TIME_ADD_AUCTION_BY_BET = 30;

        /// <summary>
        /// Расписание проведения аукциона контейнеров
        /// </summary>
        [JsonProperty("auction_time")]
        public static readonly Dictionary<DayOfWeek, int[]> AUCTION_TIME = new Dictionary<DayOfWeek, int[]>()
        {
            //{ DayOfWeek.Monday, new int[] { 18 } },
            //{ DayOfWeek.Tuesday, new int[] { 18 } },
            //{ DayOfWeek.Wednesday, new int[] { 18 } },
            //{ DayOfWeek.Thursday, new int[] { 18 } },
            //{ DayOfWeek.Friday, new int[] { 18 } },
            //{ DayOfWeek.Saturday, new int[] { 18 } },
            //{ DayOfWeek.Sunday, new int[] { 18 } },
            { DayOfWeek.Monday, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 } },
            { DayOfWeek.Tuesday, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 } },
            { DayOfWeek.Wednesday, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 } },
            { DayOfWeek.Thursday, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 } },
            { DayOfWeek.Friday, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 } },
            { DayOfWeek.Saturday, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 } },
            { DayOfWeek.Sunday, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 } },
        };

        /// <summary>
        /// Настройка всех типов контейнеров
        /// </summary>
        [JsonProperty("container_settings")]
        public static readonly Dictionary<ContainerType, ContainerSettingsData> CONTAINER_SETTINGS = new Dictionary<ContainerType, ContainerSettingsData>()
        {
            //{ ContainerType.Low, new ContainerSettingsData(name: "Низкий класс", prizes: new List<PrizeData>()
            //    {
            //        new PrizeData("nsx", chance: 10, rarity: Rarity.Red, price: 50000000),
            //        new PrizeData("nsx2", chance: 15, rarity: Rarity.Red, price: 35000000),
            //        new PrizeData("aclass2", chance: 20, rarity: Rarity.Gold, price: 27000000),
            //        new PrizeData("g63", chance: 25, rarity: Rarity.Pink, price: 25000000),
            //        new PrizeData("g55", chance: 32, rarity: Rarity.Pink, price: 22000000),
            //        new PrizeData("eclass4", chance: 35, rarity: Rarity.Pink, price: 18000000),
            //        new PrizeData("m2g42", chance: 35, rarity: Rarity.Pink, price: 15000000),
            //        new PrizeData("q60s", chance: 35, rarity: Rarity.Pink, price: 15000000),

            //        new PrizeData("accord", chance: 42, rarity: Rarity.Gray, price: 7000000),
            //        new PrizeData("focusrs", chance: 45, rarity: Rarity.Gray, price: 6000000),
            //        new PrizeData("bmwe39", chance: 45, rarity: Rarity.Gray, price: 5000000),
            //    }, isDonate: false, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            //},
            //{ ContainerType.Donate, new ContainerSettingsData(name: "Donate класс", prizes: new List<PrizeData>()
            //    {
            //        //new PrizeData("evijia", chance: 10, rarity: Rarity.Red, price: 50000000),
            //        new PrizeData("maybach", chance: 10, rarity: Rarity.Red, price: 50000000),
            //        new PrizeData("cybertruck", chance: 15, rarity: Rarity.Red, price: 35000000),
            //        new PrizeData("amgone", chance: 20, rarity: Rarity.Gold, price: 27000000),
            //        //new PrizeData("xclass2", chance: 25, rarity: Rarity.Pink, price: 25000000),
            //        new PrizeData("supragr", chance: 32, rarity: Rarity.Pink, price: 22000000),
            //        new PrizeData("eclass4", chance: 35, rarity: Rarity.Pink, price: 18000000),
            //        new PrizeData("m2g42", chance: 35, rarity: Rarity.Pink, price: 15000000),
            //        new PrizeData("q60s", chance: 35, rarity: Rarity.Pink, price: 15000000),

            //        new PrizeData("accord", chance: 42, rarity: Rarity.Gray, price: 7000000),
            //        new PrizeData("focusrs", chance: 45, rarity: Rarity.Gray, price: 6000000),
            //        new PrizeData("bmwe39", chance: 45, rarity: Rarity.Gray, price: 5000000),
            //    }, isDonate: true, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            //},
            // { ContainerType.Medium, new ContainerSettingsData(name: "Medium класс", prizes: new List<PrizeData>()
            //    {
            //        new PrizeData("m5e60", chance: 10, rarity: Rarity.Red, price: 50000000),
            //        new PrizeData("gtr32", chance: 15, rarity: Rarity.Red, price: 35000000),
            //        new PrizeData("modelx", chance: 18, rarity: Rarity.Gold, price: 30000000),
            //        new PrizeData("models", chance: 20, rarity: Rarity.Gold, price: 27000000),
            //        new PrizeData("gle63", chance: 25, rarity: Rarity.Pink, price: 25000000),
            //        new PrizeData("tahoe2", chance: 32, rarity: Rarity.Pink, price: 22000000),
            //        new PrizeData("s15", chance: 35, rarity: Rarity.Pink, price: 18000000),
            //        new PrizeData("camry2", chance: 35, rarity: Rarity.Pink, price: 15000000),
            //        new PrizeData("charger2", chance: 35, rarity: Rarity.Pink, price: 15000000),

            //        new PrizeData("accord", chance: 42, rarity: Rarity.Gray, price: 7000000),
            //        new PrizeData("ram", chance: 45, rarity: Rarity.Gray, price: 6000000),
            //        new PrizeData("golf7r", chance: 45, rarity: Rarity.Gray, price: 5000000),
            //    }, isDonate: false, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            //},
            //  { ContainerType.Premium, new ContainerSettingsData(name: "Premium класс", prizes: new List<PrizeData>()
            //    {
            //        new PrizeData("escalade2", chance: 10, rarity: Rarity.Red, price: 50000000),
            //        new PrizeData("63gls2", chance: 15, rarity: Rarity.Red, price: 35000000),
            //        new PrizeData("s63cab", chance: 20, rarity: Rarity.Gold, price: 27000000),
            //        new PrizeData("s600", chance: 25, rarity: Rarity.Pink, price: 25000000),
            //        new PrizeData("gt63s", chance: 32, rarity: Rarity.Pink, price: 22000000),
            //        new PrizeData("rs6", chance: 35, rarity: Rarity.Pink, price: 18000000),
            //        new PrizeData("lc300", chance: 35, rarity: Rarity.Pink, price: 15000000),
            //        new PrizeData("m850", chance: 35, rarity: Rarity.Pink, price: 15000000),

            //        new PrizeData("m4f82", chance: 42, rarity: Rarity.Gray, price: 7000000),
            //        new PrizeData("focusrs", chance: 45, rarity: Rarity.Gray, price: 6000000),
            //        new PrizeData("bmwe39", chance: 45, rarity: Rarity.Gray, price: 5000000),
            //    }, isDonate: false, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            //},
            //   { ContainerType.PremiumPlus, new ContainerSettingsData(name: "PremiumPlus класс", prizes: new List<PrizeData>()
            //    {
            //        new PrizeData("xclass", chance: 10, rarity: Rarity.Red, price: 50000000),
            //        new PrizeData("rrphantom", chance: 15, rarity: Rarity.Red, price: 35000000),
            //        new PrizeData("wraithb", chance: 20, rarity: Rarity.Gold, price: 27000000),
            //        new PrizeData("macan", chance: 25, rarity: Rarity.Pink, price: 25000000),
            //        new PrizeData("imola", chance: 25, rarity: Rarity.Pink, price: 25000000),
            //        new PrizeData("m8gc", chance: 25, rarity: Rarity.Pink, price: 25000000),

            //    }, isDonate: false, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            //},



            { ContainerType.Low, new ContainerSettingsData(name: "Китайская Народная Республика", prizes: new List<PrizeData>()
                {
                    //new PrizeData("nsx", chance: 10, rarity: Rarity.Red, price: 50000000),

                }, isDonate: false, containerModel: "prop_container_01g", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            },
            { ContainerType.Medium, new ContainerSettingsData(name: "Соединенные Штаты", prizes: new List<PrizeData>()
                {

                    new PrizeData("fordgt", chance: 13, rarity: Rarity.Pink, price: 35000000),

                    
                    //new PrizeData("", chance: 15, rarity: Rarity.Blue, price: 25000000),
                    new PrizeData("16challenger", chance: 22, rarity: Rarity.Gray, price: 5000000),
                    

                }, isDonate: false, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            },
            { ContainerType.Premium, new ContainerSettingsData(name: "Европейский Союз", prizes: new List<PrizeData>()
                {

                    new PrizeData("g63", chance: 8, rarity: Rarity.Red, price: 65000000),
                    new PrizeData("63gls2", chance: 10, rarity: Rarity.Red, price: 50000000),
                    new PrizeData("gt63s", chance: 8, rarity: Rarity.Red, price: 50000000),
                    new PrizeData("rs6", chance: 13, rarity: Rarity.Pink, price: 25000000),

                    new PrizeData("g55", chance: 16, rarity: Rarity.Blue, price: 17000000),
                    new PrizeData("macan", chance: 18, rarity: Rarity.Blue, price: 9500000),
                    new PrizeData("I8", chance: 18, rarity: Rarity.Blue, price: 7000000),

                    new PrizeData("weevil", chance: 21, rarity: Rarity.Gray, price: 3000000),

                }, isDonate: false, containerModel: "prop_container_01d", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            },
            { ContainerType.PremiumPlus, new ContainerSettingsData(name: "Восточная коллекция", prizes: new List<PrizeData>()
                {
                    new PrizeData("supragr", chance: 10, rarity: Rarity.Red, price: 50000000),
                    new PrizeData("gtr32", chance: 10, rarity: Rarity.Red, price: 43000000),
                    new PrizeData("400z", chance: 15, rarity: Rarity.Pink, price: 10000000),
                    new PrizeData("370z", chance: 8, rarity: Rarity.Pink, price: 31000000),
                    new PrizeData("evo10", chance: 22, rarity: Rarity.Gray, price: 6000000),
                    new PrizeData("evo9", chance: 22, rarity: Rarity.Gray, price: 3200000),

                }, isDonate: false, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            },
            { ContainerType.Donate, new ContainerSettingsData(name: "Объединенные Арабские Эмираты", prizes: new List<PrizeData>()
            {
                    new PrizeData("countach", chance: 2, rarity: Rarity.Red, price: 100000000),
                    new PrizeData("laferrari17", chance: 7, rarity: Rarity.Red, price: 70000000),
                    new PrizeData("rrphantom", chance: 8, rarity: Rarity.Red, price: 55000000),
                    new PrizeData("gemera", chance: 7, rarity: Rarity.Red, price: 70000000),
                    new PrizeData("ghost", chance: 10, rarity: Rarity.Red, price: 50000000),

                    new PrizeData("urus", chance: 10, rarity: Rarity.Pink, price: 30000000),
                }, isDonate: true, containerModel: "prop_container_02a", leftDoorModel: "prop_cntrdoor_ld_l", rightDoorModel: "prop_cntrdoor_ld_r")
            },
        };
    }
}
