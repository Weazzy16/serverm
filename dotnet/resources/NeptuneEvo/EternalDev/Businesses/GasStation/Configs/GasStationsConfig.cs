using EternalCore.Json.Interfaces;
using NeptuneEvo.EternalDev.Businesses.GasStation.Enums;
using NeptuneEvo.EternalDev.VehiclePetrol.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NeptuneEvo.EternalDev.Businesses.GasStation.Configs
{
    public class GasStationsConfig : IConfigJson
    {
        public static string Path => "businesses/gas-stations.json";

        /// <summary>
        /// Тип запровчных станций по стандарту, если он не указан в StationTypes
        /// </summary>
        [JsonProperty("default_type")]
        public GasStationType DefaultType { get; set; } = GasStationType.LimitedLtd;

        /// <summary>
        /// Список типов для бизнесов по Id
        /// </summary>
        [JsonProperty("station_types")]
        public Dictionary<int, GasStationType> StationTypes { get; set; } = new Dictionary<int, GasStationType>();

        /// <summary>
        /// Список всех типов топлива с их настройками
        /// </summary>
        [JsonProperty("petrol_types")]
        public Dictionary<PetrolType, PetrolTypeData> PetrolTypes { get; set; } = new Dictionary<PetrolType, PetrolTypeData>();

        /// <summary>
        /// Список всех продуктов
        /// </summary>
        [JsonProperty("products")]
        public List<ProductData> Products { get; set; } = new List<ProductData>();
    }
}
