using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing.Classes
{
    public class RodData
    {
        public double Wear { get; set; }
        public int MinTime { get; set; }
        public int MaxTime { get; set; }   

        public RodData(double wear, int minTime, int maxTime)
        {
            Wear = wear;
            MinTime = minTime;
            MaxTime = maxTime;
        }
    }
}
