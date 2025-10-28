using NeptuneEvo.Handles;
using System;

namespace NeptuneEvo.EternalDev.VehicleAudio.Extensions
{
    public static class Players
    {
        public static bool IsTimeouted(this ExtPlayer player, string nameTimeout, int time)
        {
            nameTimeout = nameTimeout + ".timeout";
            if (!player.HasData(nameTimeout))
            {
                player.SetData(nameTimeout, DateTime.Now);
                return true;
            }
            else
            {
                if (DateTime.Now > player.GetData<DateTime>(nameTimeout).AddSeconds(time))
                {
                    player.SetData(nameTimeout, DateTime.Now);
                    return true;
                }
            }
            return false;
        }
    }
}
