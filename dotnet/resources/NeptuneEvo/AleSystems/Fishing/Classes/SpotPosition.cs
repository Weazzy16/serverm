using GTANetworkAPI;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing.Classes
{
    public class SpotPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Heading { get; set; }

        public float Radius { get; set; } = 2;

        public SpotPosition(float x, float y, float z, float heading, float radius = 2)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = heading;
            Radius = radius;

            GTAElements();
        }

        private ExtColShape _colShape { get; set; }
        public void GTAElements()
        {
            _colShape = CustomColShape.CreateSphereColShape(Get(), Radius, 0, ColShapeEnums.FishSpot);
        }

        public Vector3 Get() => new Vector3(X, Y, Z);
    }
}
