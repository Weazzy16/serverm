using NeptuneEvo.Chars.Models;
using Newtonsoft.Json;

namespace NeptuneEvo.EternalDev.Businesses.GasStation.Configs
{
    public class ProductData
    {
        [JsonProperty("itemId")]
        public ItemId ItemId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
