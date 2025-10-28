using GTANetworkAPI;
using System;

namespace NeptuneEvo.EternalDev.Containers.Methods
{
    public static class Vector3Extensions
    {
        public static Vector3 GetPositionOffset(this Vector3 pos, double angle, double dist)
        {
            angle = angle * 0.0174533;
            double y = pos.Y + dist * Math.Sin(angle);
            double x = pos.X + dist * Math.Cos(angle);

            Vector3 result = new Vector3(x, y, pos.Z);
            return result;
        }
    }
}
