using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Core.Armor.Classes
{
    public class ArmorData
    {
        public string Name { get; set; }
        public int FractionId { get; set; }
        public bool IsHeavy { get; set; }
        public int Drawable { get; set; }
        public int Texture { get; set; }
        public bool Gender { get; set; }

        public ArmorData(string name, bool isHeavy, int drawable, int texture, bool gender, int fractionId = 0)
        {
            Name = name;;
            IsHeavy = isHeavy;
            Drawable = drawable;
            Texture = texture;
            Gender = gender;
            FractionId = fractionId;
        }
    }
}
