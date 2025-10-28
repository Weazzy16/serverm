using GTANetworkAPI;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.MarketPlace.Configs
{
    public class AuctionPositionConfig
    {
        /// <summary>
        /// Позиция
        /// </summary>
        [JsonProperty("position")]
        public Vector3 Position { get; set; }

        /// <summary>
        /// Радиус
        /// </summary>
        [JsonProperty("range")]
        public float Range { get; set; }

        /// <summary>
        /// Измерение
        /// </summary>
        [JsonProperty("dimension")]
        public uint Dimension { get; set; }

        public AuctionPositionConfig(Vector3 position, float range, uint dimension)
        {
            Position = position;
            Range = range;
            Dimension = dimension;
        }

        public void GTAElements()
        {
            CustomColShape.CreateSphereColShape(Position, Range, Dimension, ColShapeEnums.MarketPlaceAuction);
        }

        [Interaction(ColShapeEnums.MarketPlaceAuction)]
        public static void OnInteraction(ExtPlayer player)
        {
            Manager.Subscribe(player);
        }
    }
}
