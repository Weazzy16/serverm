using GTANetworkAPI;
using NeptuneEvo.Chars.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.MarketPlace.Configs
{
    public class MarketPlaceConfig
    {
        public static string Path => "marketPlace/settings.json";

        /// <summary>
        /// Список позиций для склада
        /// </summary>
        [JsonProperty("storage_positions")]
        public StoragePlaceConfig[] StoragePostions = new StoragePlaceConfig[]
        {
            new StoragePlaceConfig(new Vector3(-826.19, -757.41, 22.31), 25f, new Vector3(-846.72, -750.43, 24.1))
        };

        /// <summary>
        /// Время длительности тестдрайва
        /// </summary>
        [JsonProperty("testdrive_time")]
        public int TestdriveTime { get; set; } = 300;

        /// <summary>
        /// Нужно ли находится рядом с автосалоном, чтобы начать тестдрайв
        /// </summary>
        [JsonProperty("need_nearest_autoshop_for_testdrive")]
        public bool NeedNearestAutoshopForTestdrive { get; set; } = true;

        /// <summary>
        /// Список позиций для тестдрайва
        /// </summary>
        [JsonProperty("testdrive_positions")]
        public TestdrivePosition[] TestdrivePositions = new TestdrivePosition[]
        {
            new TestdrivePosition(new Vector3(-816.0768, -743.8378, 23.710386), -170)
        };

        /// <summary>
        /// Список позициий для интерьера аукциона/маркетплейса
        /// </summary>
        [JsonProperty("interior_positions")]
        public InteriorPositionConfig[] InteriorPositions = new InteriorPositionConfig[]
        {
            new InteriorPositionConfig(name: "Аукцион", blipSprite: 525, blipColor: 4, blipScale: .65f, 
                exteriorPosition: new Vector3(-827.46, -699.8, 28.05), interiorPosition: new Vector3(-1159.13, -385.77, -162.37), 
                markerColor: new Color(52, 152, 219, 100), markerScale: 1f, dimension: 3000
            )
        };

        /// <summary>
        /// Список позиций для открытия интерфейса маркетплейса (например в интерьере аукциона)
        /// </summary>
        [JsonProperty("auction_positions")]
        public AuctionPositionConfig[] AuctionPositions = new AuctionPositionConfig[]
        {
            new AuctionPositionConfig(position: new Vector3(-1138.45, -391.86, -162), range: 5, dimension: 3000)
        };

        /// <summary>
        /// Максимальное количество слотов на складе
        /// </summary>
        [JsonProperty("storage_slots")]
        public int MaxSlotsInStorage { get; set; } = 300;

        /// <summary>
        /// Настройки самого маркетплейса
        /// </summary>
        [JsonProperty("app")]
        public AppConfig App = new AppConfig();

        /// <summary>
        /// Список предметов, которых нельзя выставлять на маркетплейсе
        /// </summary>
        [JsonProperty("blocked_items_for_sale")]
        public ItemId[] BlockedItemsForSale = new ItemId[]
        {
            ItemId.Material
        };
    }
}
