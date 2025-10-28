using NeptuneEvo.EternalDev.CrewSystem.Classes;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.CrewSystem.Extensions
{
    public static class Players
    {
        public static bool GetPlayerCrew(this ExtPlayer player, out Crew crew)
        {
            var sessionData = player.GetSessionData();
            if (sessionData is null || sessionData.CrewSession is null || sessionData.CrewSession.CrewId == -1)
            {
                crew = null;
                return false;
            }

            return Manager.GetCrew(sessionData.CrewSession.CrewId, out crew);
        }
    }
}
