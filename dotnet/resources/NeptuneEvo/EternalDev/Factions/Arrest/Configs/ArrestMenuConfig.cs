using EternalCore.Json.Interfaces;
using Newtonsoft.Json;

namespace NeptuneEvo.EternalDev.Factions.Arrest.Configs
{
    public class ArrestMenuConfig : IConfigJson
    {
        public static string Path => "arrestMenu.json";

        [JsonProperty("time_settings")]
        public int[] TimeSettings { get; set; } = new int[2]
        {
            5, 20
        };

        [JsonProperty("pledge_settings")]
        public int[] PledgeSettings { get; set; } = new int[2]
        {
            15_000,
            125_000
        };
    }
}
