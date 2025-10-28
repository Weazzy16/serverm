using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Players.Models
{
    public class CrewSessionData
    {
        public int CrewId { get; set; } = -1;
        public List<DateTime> AlertMarkers { get; set; } = new List<DateTime>();
    }
}
