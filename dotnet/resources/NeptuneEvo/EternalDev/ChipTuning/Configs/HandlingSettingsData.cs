using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Configs
{
    public class HandlingSettingsData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("handlingParam")]
        public string HandlingParam { get; set; }

        [JsonProperty("min")]
        public decimal Min { get; set; }

        [JsonProperty("max")]
        public decimal Max { get; set; }

        [JsonProperty("step")]
        public decimal Step { get; set; }

        [JsonProperty("price")]
        public PriceSettings Price { get; set; }

        [JsonProperty("depends_default")]
        public bool DependsOnDefault { get; set; }

        [JsonProperty("default")]
        public decimal Default { get; set; }

        public HandlingSettingsData(string name, string handlingParam, decimal min, decimal max, decimal step, PriceSettings price, bool dependsOnDefault = false, decimal defaultValue = -1)
        {
            Name = name;
            HandlingParam = handlingParam;
            Min = min;
            Max = max;
            Step = step;
            Price = price;
            DependsOnDefault = dependsOnDefault;
            Default = defaultValue;
        }
    }
}
