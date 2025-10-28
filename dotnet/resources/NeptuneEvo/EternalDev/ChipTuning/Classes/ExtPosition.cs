using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Classes
{
    public class ExtPosition
    {
        [JsonProperty("x")]
        public double X { get; set; }
        
        [JsonProperty("y")]
        public double Y { get; set; }
        
        [JsonProperty("z")]
        public double Z { get; set; }

        [JsonProperty("heading")]
        public double Heading { get; set; }

        public ExtPosition(float x, float y, float z, float heading)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = heading;
        }

        [JsonConstructor]
        public ExtPosition(double x, double y, double z, double heading)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = heading;
        }

        public Vector3 GetVector3() 
            => new Vector3(X, Y, Z);

        public void Set(Entity entity)
        {
            NAPI.Entity.SetEntityPosition(entity, GetVector3());
            NAPI.Entity.SetEntityRotation(entity, new Vector3(0, 0, Heading));
        }
    }
}
