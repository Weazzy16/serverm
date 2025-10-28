using NeptuneEvo.EternalDev.ChipTuning.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Classes
{
    public class ChipVehicleData
    {
        public int VehicleId { get; set; }

        public Dictionary<string, decimal> Handlings = new Dictionary<string, decimal>();

        public Dictionary<string, bool> Controllers = new Dictionary<string, bool>();
    }
}
