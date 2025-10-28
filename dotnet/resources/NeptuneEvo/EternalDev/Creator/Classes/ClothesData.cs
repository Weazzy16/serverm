using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.Creator.Classes
{
    public class ClothesData
    {
        [JsonProperty("component")]
        public int Component {  get; set; }

        [JsonProperty("drawable")]
        public int Drawable { get; set; }

        [JsonProperty("texture")]
        public int Texture { get; set; }
    }
}
