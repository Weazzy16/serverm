using EternalCore.Json.Interfaces;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace NeptuneEvo.EternalDev.Businesses.Info.Configs
{
    public class BusinessInfoConfig : IConfigJson
    {
        public static string Path => "businesses/info.json";

        /// <summary>
        /// Цвета точки о информации
        /// </summary>
        [JsonProperty("marker_color")]
        public Color MARKER_COLOR = new Color(255, 255, 255, 120);

        /// <summary>
        /// Список маркеров https://wiki.rage.mp/index.php?title=Markers
        /// </summary>
        [JsonProperty("marker_type")]
        public int MARKER_TYPE = 29;

        /// <summary>
        /// Размер маркера
        /// </summary>
        [JsonProperty("marker_scale")]
        public float MARKER_SCALE = 1f;

        /// <summary>
        /// Название команды для создания точки с информацией
        /// </summary>
        [JsonProperty("command_create")]
        public string COMMAND_CREATE = "createbusinessinfo";

        /// <summary>
        /// Минимальный уровень админки для использования команды создания точек
        /// </summary>
        [JsonProperty("admin_lvl")]
        public int ADMIN_LVL = 10;
    }
}
