using NeptuneEvo.AleSystems.Fishing.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.Chars.Models;
using Newtonsoft.Json;
using NeptuneEvo.AleSystems.Fishing.Store;
using NeptuneEvo.AleSystems.Fishing.Store.Classes;
using System.Linq;

namespace NeptuneEvo.AleSystems.Fishing.Data
{
    public class Config
    {
        public static void Initialize()
        {
            STORES.ForEach(b => { b.GTAElements(); b.UpdatePrices(); });
            SPOTS_DATA.ForEach(s => s.GTAElements());

            FISH_ITEMS_DATA = FISH_ITEMS_DATA.OrderBy(x => x.Chance).Reverse().ToList();
        }

        // Данные о удочках
        public static readonly Dictionary<ItemId, RodData> RODS_DATA = new Dictionary<ItemId, RodData>()
        {
            { ItemId.Rod, new RodData(wear: 1, minTime: 15, maxTime: 60) },
            { ItemId.RodUpgraded, new RodData(wear: .5, minTime: 10, maxTime: 45) },
            { ItemId.RodMk2, new RodData(wear: .25, minTime: 5, maxTime: 30) },
        };

        // Список всех рыб
        public static List<FishItemData> FISH_ITEMS_DATA = new List<FishItemData>()
        {
             new FishItemData(ItemId.Sterlad, minLvl: 0, chance: 50, minPrice: 700000, maxPrice: 1850000),
             new FishItemData(ItemId.Losos, minLvl: 0, chance: 50, minPrice: 850000, maxPrice: 1950000),
             new FishItemData(ItemId.Osetr, minLvl: 0, chance: 50, minPrice: 900000, maxPrice: 2000000),
             new FishItemData(ItemId.BlackAmur, minLvl: 0, chance: 50, minPrice: 950000, maxPrice: 2270000),
             new FishItemData(ItemId.Skat, minLvl: 0, chance: 40, minPrice: 1000000, maxPrice: 2500000),
             new FishItemData(ItemId.Tunec, minLvl: 0, chance: 38, minPrice: 1100000, maxPrice: 2700000),
             new FishItemData(ItemId.Malma, minLvl: 0, chance: 35, minPrice: 1200000, maxPrice: 3000000),
             new FishItemData(ItemId.Fugu, minLvl: 0, chance: 30, minPrice: 100000, maxPrice: 1500000, rods: new List<ItemId>() { ItemId.RodMk2 }, isDonate: true),
        };

        // Список всех точек ловли рыбы
        public static readonly List<FishSpot> SPOTS_DATA = new List<FishSpot>()
        {
            new FishSpot("Зона рыбалки", 50, new Vector3(-1858.7178, -1241.8573, 8.615783), 
                new List<SpotPosition>()
                {
                    new SpotPosition(-1849.4424f, -1251.1088f, 7.495778f, 140, radius: 10),
                    new SpotPosition(-1830.3875f, -1267.0994f, 7.4982624f, 140, radius: 10),
                    new SpotPosition(-1840.2538f, -1258.8212f, 7.4957886f, 140, radius: 10),
                    new SpotPosition(-1856.7246f, -1244.9999f, 7.4957905f, 140, radius: 10),
                    new SpotPosition(-1862.9568f, -1239.7794f, 7.4964943f, 140, radius: 10),
                    new SpotPosition(-1861.9534f, -1233.2709f, 7.4957886f, 140, radius: 10),
                    new SpotPosition(-1823.2106f, -1266.721f, 7.4982834f, 140, radius: 10),
                },
                new List<ItemId>() { ItemId.Sterlad, ItemId.Osetr }
            ),

             new FishSpot("Зона рыбалки", 600, new Vector3(1164.3871, 3955.7603, 30.678583),
                new List<SpotPosition>()
                {
                    new SpotPosition(1164.3871f, 3955.7603f, 30.678583f, 140, radius: 100),
                    new SpotPosition(1644.0227f, 4163.5444f, 30.800299f, 140, radius: 100),
                    new SpotPosition(2030.1279f, 4291.861f, 31.193203f, 140, radius: 100),
                    new SpotPosition(579.75287f, 3918.1309f, 31.47704f, 140, radius: 100),
                    new SpotPosition(153.17334f, 4059.195f, 31.211226f, 140, radius: 100),
                },
                new List<ItemId>() { ItemId.Sterlad, ItemId.Losos, ItemId.Tunec }
            ),

             new FishSpot("Зона рыбалки", 50, new Vector3(-3428.3438, 968.1135, 8.346685),
                new List<SpotPosition>()
                {
                    new SpotPosition(-3428.3728f, 977.5574f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3174f, 975.5813f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.4053f, 973.5774f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3894f, 971.5653f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.4026f, 969.5649f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.4324f, 967.6427f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.4429f, 965.59174f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3037f, 963.61127f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3926f, 961.71027f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3042f, 959.6657f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3079f, 957.70685f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3054f, 955.59375f, 8.346692f, 93, radius: 10),
                    new SpotPosition(-3428.3435f, 953.67706f, 8.346692f, 93, radius: 10),
                },
                new List<ItemId>() { ItemId.Losos, ItemId.BlackAmur, ItemId.Tunec, ItemId.Malma, ItemId.Fugu }
            ),
             new FishSpot("Зона рыбалки", 20, new Vector3(-192.40752, 789.36206, 198.10754),
                new List<SpotPosition>()
                {
                    new SpotPosition(-192.77943f, 790.0845f, 198.10757f, 93, radius: 20),
                },
                new List<ItemId>() { ItemId.Sterlad, ItemId.Osetr, ItemId.Malma }
            ),
            new FishSpot("Зона рыбалки", 10, new Vector3(-1548.5968, 4343.311, 1.4836367),
                new List<SpotPosition>()
                {
                    new SpotPosition(-1548.5968f, 4343.311f, 1.4836367f, 93, radius: 10),
                },
                new List<ItemId>() { ItemId.Sterlad, ItemId.Losos, ItemId.Tunec }
            ),
            new FishSpot("Зона рыбалки", 10, new Vector3(-1431.2411, 4318.6855, 1.1660756),
                new List<SpotPosition>()
                {
                    new SpotPosition(-1431.2411f, 4318.6855f, 1.1660756f, 93, radius: 10),
                },
                new List<ItemId>() { ItemId.Sterlad, ItemId.Losos, ItemId.Tunec }
            ),
            new FishSpot("Зона рыбалки", 50, new Vector3(-277.9763, 6638.35, 7.551846),
                new List<SpotPosition>()
                {
                    new SpotPosition(-277.9763f, 6638.35f, 7.551846f, 93, radius: 5),
                    new SpotPosition(-275.5613f, 6640.785f, 7.5526886f, 93, radius: 5),
                    new SpotPosition(-273.6339f, 6642.592f, 7.3942165f, 93, radius: 5),
                    new SpotPosition(-271.0382f, 6645.1035f, 7.3919325f, 93, radius: 5),
                    new SpotPosition(-281.81104f, 6634.5938f, 7.5199566f, 93, radius: 5),
                    new SpotPosition(-283.76434f, 6632.701f, 7.405982f, 93, radius: 5),
                    new SpotPosition(-285.76755f, 6630.83f, 7.2895026f, 93, radius: 5),
                    new SpotPosition(-287.3584f, 6629.454f, 7.197286f, 93, radius: 5),
                    new SpotPosition(-283.57883f, 6625.2266f, 7.198342f, 93, radius: 5),
                    new SpotPosition(-285.73218f, 6627.3105f, 7.136251f, 93, radius: 5),
                    new SpotPosition(-279.963f, 6621.5073f, 7.449249f, 93, radius: 5),
                },
                new List<ItemId>() { ItemId.Sterlad, ItemId.Losos, ItemId.Tunec }
            ),
            new FishSpot("Зона рыбалки", 20, new Vector3(3866.4597, 4463.7676, 2.7274485),
                new List<SpotPosition>()
                {
                    new SpotPosition(3867.3103f, 4463.664f, 2.7243242f, 93, radius: 5),
                    new SpotPosition(3866.4934f, 4462.3936f, 2.7273374f, 93, radius: 5),
                    new SpotPosition(3864.476f, 4462.354f, 2.7224035f, 93, radius: 5),
                    new SpotPosition(3862.04f, 4462.714f, 2.724004f, 93, radius: 5),
                    new SpotPosition(3866.3647f, 4464.8525f, 2.7278147f, 93, radius: 5),
                    new SpotPosition(3864.5083f, 4464.98f, 2.7239177f, 93, radius: 5),
                    new SpotPosition(3862.4265f, 4464.9873f, 2.7239783f, 93, radius: 5),
                    new SpotPosition(3860.6152f, 4465.097f, 2.7162397f, 93, radius: 5),
                    new SpotPosition(3859.4207f, 4465.198f, 2.716363f, 93, radius: 5),
                },
                new List<ItemId>() { ItemId.Sterlad, ItemId.Osetr, ItemId.Skat, ItemId.Malma }
            ),
            new FishSpot("Зона рыбалки", 10, new Vector3(4876.61, -4800.22, 1.0420382),
                new List<SpotPosition>()
                {
                    new SpotPosition(4876.61f, -4800.22f, 1.0420382f, 93, radius: 10),
                },
                new List<ItemId>() { ItemId.BlackAmur, ItemId.Skat, ItemId.Fugu }
            ),
        };

        // Список для продажи в магазине рыболовства
        public static readonly Dictionary<ItemId, int> FISHING_STORE_ITEMS = new Dictionary<ItemId, int>()
        {
            { ItemId.Rod, 35000 },
            { ItemId.RodUpgraded, 50000 },
            { ItemId.RodMk2, 100000 },
            { ItemId.Bait, 10 },
        };

        // Список о лвлах уровень
        public static readonly Dictionary<int, int> EXP_LVL_DATA = new Dictionary<int, int>()
        {
            { 1, 50 },     // 250 - количетсво рыб, чтобы подняться с 1 уровная до 2
            { 2, 75 },     // 250 - количетсво рыб, чтобы подняться с 2 уровная до 3
            { 3, 125 },     // 250 - количетсво рыб, чтобы подняться с 3 уровная до 4
            { 4, 175 },     // 250 - количетсво рыб, чтобы подняться с 4 уровная до 5
            { 5, 250 },     // 250 - количетсво рыб, чтобы подняться с 5 уровная до 6
        };

        public static List<StorePlace> STORES = new List<StorePlace>()
        {
            new StorePlace("Рыболовный магазин", position: new Vector3(1533.448, 3779.9336, 34.51497), heading: -151, blipSprite: 356, blipColor: 38, (uint)PedHash.OldMan2,
                new Dictionary<string, List<StoreItem>>()
                {
                    { "fishingCategoryRod", new List<StoreItem>() {
                        new StoreItem("Удочка", ItemId.Rod, minPrice: 110000, maxPrice: 200000, value: 1),
                        new StoreItem("Удочка улучш.", ItemId.RodUpgraded, minPrice: 270000, maxPrice: 410000, value: 1),
                        new StoreItem("Удочка Mk2", ItemId.RodMk2, minPrice: 500000, maxPrice: 780000, value: 1),
                    }},
                    { "fishingCategoryBait", new List<StoreItem>() {
                        new StoreItem("Наживка", ItemId.Bait, 89, 281, 1),
                    }},
                }
            ),
            new StorePlace("Рыболовный магазин", position: new Vector3(5378.2783, -5388.3364, 43.50213), heading: 89, blipSprite: 356, blipColor: 38, (uint)PedHash.Bevhills01AMM,
                new Dictionary<string, List<StoreItem>>()
                {
                    { "fishingCategoryRod", new List<StoreItem>() {
                        new StoreItem("Удочка", ItemId.Rod, minPrice: 110000, maxPrice: 200000, value: 1),
                        new StoreItem("Удочка улучш.", ItemId.RodUpgraded, minPrice: 270000, maxPrice: 410000, value: 1),
                        new StoreItem("Удочка Mk2", ItemId.RodMk2, minPrice: 500000, maxPrice: 780000, value: 1),
                    }},
                    { "fishingCategoryBait", new List<StoreItem>() {
                        new StoreItem("Наживка", ItemId.Bait, 89, 281, 1),
                    }},
                }
            ),
            new StorePlace("сикрет шоп", position: new Vector3(1070.8549, -780.4616, 58.34833),heading: 89, blipSprite: 374, blipColor: 42, (uint)PedHash.Bevhills01AMM,
                new Dictionary<string, List<StoreItem>>()
                {
                    { "fishingCategoryShop", new List<StoreItem>() {
                        new StoreItem("Spank", ItemId.Sprunk, minPrice: 790000, maxPrice: 800000, value: 1),
                        new StoreItem("Бронижилет", ItemId.BodyArmor, minPrice: 79000, maxPrice: 80000, value: 1),
                    }},
                }
            ),
            new StorePlace("Рыболовный магазин", position: new Vector3(-1043.8572, 5327.3936, 44.571556), heading: 29, blipSprite: 356, blipColor: 38, (uint)PedHash.Beachvesp02AMY,
                new Dictionary<string, List<StoreItem>>()
                {
                    { "fishingCategoryRod", new List<StoreItem>() {
                        new StoreItem("Удочка", ItemId.Rod, minPrice: 110000, maxPrice: 200000, value: 1),
                        new StoreItem("Удочка улучш.", ItemId.RodUpgraded, minPrice: 270000, maxPrice: 410000, value: 1),
                        new StoreItem("Удочка Mk2", ItemId.RodMk2, minPrice: 500000, maxPrice: 780000, value: 1),
                    }},
                    { "fishingCategoryBait", new List<StoreItem>() {
                        new StoreItem("Наживка", ItemId.Bait, 89, 281, 1),
                    }},
                }
            ),

            new StorePlace("Скупщик рыбы", position: new Vector3(-2079.9229, 2610.1213, 3.0839717), heading: 87, blipSprite: 780, blipColor: 38, (uint)PedHash.ExArmy01, new Dictionary<string, List<StoreItem>>()
            {
                { "fishingCategoryFish", new List<StoreItem>() {
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[0].ItemId].Name, FISH_ITEMS_DATA[0].ItemId, FISH_ITEMS_DATA[0].MinPrice, FISH_ITEMS_DATA[0].MaxPrice, 1, true, true, FISH_ITEMS_DATA[0].IsDonate),
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[1].ItemId].Name, FISH_ITEMS_DATA[1].ItemId, FISH_ITEMS_DATA[1].MinPrice, FISH_ITEMS_DATA[1].MaxPrice, 1, true, true, FISH_ITEMS_DATA[1].IsDonate),
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[2].ItemId].Name, FISH_ITEMS_DATA[2].ItemId, FISH_ITEMS_DATA[2].MinPrice, FISH_ITEMS_DATA[2].MaxPrice, 1, true, true, FISH_ITEMS_DATA[2].IsDonate),
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[3].ItemId].Name, FISH_ITEMS_DATA[3].ItemId, FISH_ITEMS_DATA[3].MinPrice, FISH_ITEMS_DATA[3].MaxPrice, 1, true, true, FISH_ITEMS_DATA[3].IsDonate),
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[4].ItemId].Name, FISH_ITEMS_DATA[4].ItemId, FISH_ITEMS_DATA[4].MinPrice, FISH_ITEMS_DATA[4].MaxPrice, 1, true, true, FISH_ITEMS_DATA[4].IsDonate),
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[5].ItemId].Name, FISH_ITEMS_DATA[5].ItemId, FISH_ITEMS_DATA[5].MinPrice, FISH_ITEMS_DATA[5].MaxPrice, 1, true, true, FISH_ITEMS_DATA[5].IsDonate),
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[6].ItemId].Name, FISH_ITEMS_DATA[6].ItemId, FISH_ITEMS_DATA[6].MinPrice, FISH_ITEMS_DATA[6].MaxPrice, 1, true, true, FISH_ITEMS_DATA[6].IsDonate),
                    new StoreItem(Chars.Repository.ItemsInfo[FISH_ITEMS_DATA[7].ItemId].Name, FISH_ITEMS_DATA[7].ItemId, FISH_ITEMS_DATA[7].MinPrice, FISH_ITEMS_DATA[7].MaxPrice, 1, true, true, FISH_ITEMS_DATA[7].IsDonate),
                }},
            })
        };
    }
}
