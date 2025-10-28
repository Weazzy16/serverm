using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.Creator.Classes
{
    public class OverlayData
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("opacity")]
        public float Opacity { get; set; }

        [JsonProperty("color1")]
        public int Color1 { get; set; }

        [JsonProperty("color2")]
        public int Color2 { get; set; }
    }
}
