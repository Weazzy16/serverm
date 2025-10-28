using EternalCore.Json.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.Dice.Configs
{
    public class DiceConfig : IConfigJson
    {
        public static string Path => "dice.json";

        /// <summary>
        /// Кд между отправкой предложения игроку (в секундах)
        /// </summary>
        [JsonProperty("cooldown")]
        public int Cooldown { get; set; } = 5;

        /// <summary>
        /// Максимальное значение ставки
        /// </summary>
        [JsonProperty("max_bet")]
        public int MaxBet { get; set; } = 5_000_000;

        /// <summary>
        /// Минимальное значение ставки
        /// </summary>
        [JsonProperty("min_bet")]
        public int MinBet { get; set; } = 1;
    }
}
