using Newtonsoft.Json;

namespace NeptuneEvo.EternalDev.Businesses.GasStation.Configs
{
    public class PetrolTypeData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }
    }
}
