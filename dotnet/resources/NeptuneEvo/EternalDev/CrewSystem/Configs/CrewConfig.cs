using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.CrewSystem.Configs
{
    public class CrewConfig
    {
        public static string Path => "crew.json";

        /// <summary>
        /// Длина генерируемого кода для приглашения в группу
        /// </summary>
        [JsonProperty("invited_code_length")]
        public int InvitedCodeLength { get; set; } = 6;

        /// <summary>
        /// Паттерн на генерацию кода, можно добавить любые символы
        /// </summary>
        [JsonProperty("invited_code_pattern")]
        public string InvitedCodePattern { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// Максимальное количество участников в группе
        /// </summary>
        [JsonProperty("max_players_in_crew")]
        public int MaxPlayersInCrew { get; set; } = 100;

        /// <summary>
        /// Максимальное количество маркеров, которые может поставить игрок
        /// </summary>
        [JsonProperty("max_alert_markers")]
        public int MaxAlertMarkers { get; set; } = 3;

        /// <summary>
        /// Длительность показа 1 маркера
        /// </summary>
        [JsonProperty("alert_marker_duration")]
        public int AlertMarkerDuration { get; set; } = 7000;
    }
}
