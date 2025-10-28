using GTANetworkAPI;
using LinqToDB.Tools;
using NeptuneEvo.EternalDev.CrewSystem.Extensions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;

namespace NeptuneEvo.EternalDev.CrewSystem.Alerts
{
    public class Synchronization : Script
    {
        [RemoteEvent("e-dev.crew-system.alert-marker")]
        public void SyncAlert(ExtPlayer player, string jsonPosition)
        {
            var sessionData = player.GetSessionData();
            if (sessionData is null || !player.GetPlayerCrew(out var crewData) || !crewData.GetCrewMember(player.GetUUID(), out var crewMember)) return;

            var position = JsonConvert.DeserializeObject<Vector3>(jsonPosition);

            if (sessionData.CrewSession.AlertMarkers.Count >= Manager.Config.MaxAlertMarkers)
            {
                if (DateTime.Now.Ticks - sessionData.CrewSession.AlertMarkers[0].Ticks < Manager.Config.AlertMarkerDuration)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Подождите, маркеры перезагружаются...", 3000);
                    return;
                }

                sessionData.CrewSession.AlertMarkers.RemoveAt(0);
            }

            sessionData.CrewSession.AlertMarkers.Add(DateTime.Now);
            Trigger.ClientEventToPlayers(crewData.GetPlayers(), "e-dev.crew-system.alert-marker", position, crewMember.Access);
        }
    }
}
