using EternalCore.Json.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NeptuneEvo.EternalDev.VehicleAudio.Configs
{
    public class VehicleAudioConfig : IConfigJson
    {
        public static string Path => "vehicle_audio.json";

        /// <summary>
        /// Кд на проигрывание треков
        /// </summary>
        [JsonProperty("cooldown")]
        public int Cooldown { get; set; } = 5;

        /// <summary>
        /// Список треков
        /// </summary>
        [JsonProperty("tracks")]
        public List<TrackItem> Tracks { get; set; } = new List<TrackItem>();

        /// <summary>
        /// Заблокированные классы авто, в которых не будет проигрывателя
        /// </summary>
        [JsonProperty("blocked_vehicle_classes")]
        public List<int> BlockedVehicleClasses = new List<int>()
        {
            8, 13, 14, 15, 16, 21
        };
    }
}
