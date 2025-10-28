using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Fractions.Hijacking
{
    public class VehecleHij
    {
        public string Name { get; set; }
        public int Color { get; set; }
        public string ColorName { get; set; }
        public VehecleHij(string name, int color, string colorname)
        {
            Name = name;
            Color = color;
            ColorName = colorname;
        }
    }
}
