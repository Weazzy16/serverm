using Google.Protobuf.WellKnownTypes;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Events.Christmas.BonusItems.Classes;
using NeptuneEvo.Events.Christmas.Gifts.Classes;
using NeptuneEvo.Events.Christmas.Gifts.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace NeptuneEvo.Events.Christmas.Data
{
    public class Config
    {
        // Максимальное возможное количество бонусных айтемов на карте за 1 раз
        public static readonly int MAX_COUNT_OF_BONUS_ITEMS_IN_WORLD = 5;

        // Минимальное кд для спавна бонусных предметов (в минутах)
        public static readonly int COOLDOWN_FOR_REPSAWN_BONUS_ITEMS = 5;
        
        // Минимальное количество конфет которых может выдать система
        public static readonly int MIN_AMOUNT_OF_CANDY_DISPENSED = 1;
                                                                     
        // Максимальное количество конфет которых может выдать система 
        public static readonly int MAX_AMOUNT_OF_CANDY_DISPENSED = 5;

        // Список подарков
        public static readonly Dictionary<ItemId, List<GiftItemData>> GIFTS_DATA = new Dictionary<ItemId, List<GiftItemData>>()
        {
            { ItemId.SmallGift, new List<GiftItemData>()
            {
                new GiftItemData(GiftItemType.Money, "Деньги 10.000.000$", "10000", 50),
                new GiftItemData(GiftItemType.Money, "Деньги 30.000.000$", "50000", 30),
                new GiftItemData(GiftItemType.Money, "Деньги 30.000.000$", "50000", 30),
                new GiftItemData(GiftItemType.Money, "Деньги 30.000.000$", "50000", 30),
                new GiftItemData(GiftItemType.Donate, "Деньги 30.000.000$", "50000", 30),
                new GiftItemData(GiftItemType.Vehicle, "Деньги 30.000.000$", "50000", 30),
                new GiftItemData(GiftItemType.Vehicle, "Деньги 30.000.000$", "50000", 30),
                new GiftItemData(GiftItemType.Vehicle, "Asea", "Asea", 20),
            }},
        };

        // Минимальное количество конфет которых может выдать система в бонусных предметах
        public static readonly int MIN_AMOUNT_OF_CANDY_DISPENSED_IN_BONUSITEMS = 5;

        // Максимальное количество конфет которых может выдать система в бонусных предметах 
        public static readonly int MAX_AMOUNT_OF_CANDY_DISPENSED_IN_BONUSITEMS = 20;

        public static readonly List<BonusItem> BONUS_ITEMS_IN_WORLD = new List<BonusItem>()
        {
            new BonusItem("mj_snowman_1_2", new Vector3(), new Vector3()),
        };
    }
}
