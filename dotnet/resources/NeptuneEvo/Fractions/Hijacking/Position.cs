using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Fractions.Hijacking
{
    public class Position : Script
    {
        public class HijackingPosition
        {
            public Vector3 Position { get; set; }
            public Vector3 Rotation { get; set; }
            public bool IsActive { get; set; } = true;
            public HijackingPosition(Vector3 position, Vector3 rotation)
            {
                Position = position;
                Rotation = rotation;
            }
            public void SetActive(bool active)
            {
                IsActive = active;
            }
        }
    }
}
