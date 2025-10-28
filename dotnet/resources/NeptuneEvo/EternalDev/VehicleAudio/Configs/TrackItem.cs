using Newtonsoft.Json;

namespace NeptuneEvo.EternalDev.VehicleAudio.Configs
{
    public class TrackItem
    {
        /// <summary>
        /// Уникальный индификатор трека, он не может повторятся
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Имя трека
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Описание трека
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Ссылка на трек
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Длительность трека (для отображения)
        /// </summary>
        [JsonProperty("duration")]
        public string Duration { get; set; }
    }
}
