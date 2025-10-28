using NeptuneEvo.EternalDev.ChipTuning.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Configs
{
    public class ControllerSettingsData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("price")]
        public PriceSettings Price { get; set; }

        public ControllerSettingsData(string name, string description, ChipControllers type, PriceSettings price)
        {
            Name = name;
            Description = description;
            Type = type.ToString();
            Price = price;
        }
    }
}
