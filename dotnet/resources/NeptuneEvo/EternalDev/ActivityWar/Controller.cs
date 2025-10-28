using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.EternalDev.ActivityWar.Classes;
using NeptuneEvo.EternalDev.ActivityWar.Data;
using NeptuneEvo.Organizations.Player;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;

namespace NeptuneEvo.EternalDev.ActivityWar
{
    public class Controller : Script
    {
        private static readonly nLog _log = new nLog(nameof(Controller));

        [ServerEvent(Event.ResourceStart)]
        public static void OnStart() 
            => Manager.Initialize(); 

        [RemoteEvent("server.activityWar.open")]
        public static void OpenMenu(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                var memberData = player.GetOrganizationData();
                if (memberData is null || memberData.Id == -1) return;

                var data = new List<object>();
                int count = 0;
                foreach (var activity in Manager.Activities)
                {
                    var organizationData = Organizations.Manager.GetOrganizationData(activity.Value.Owner);

                    var item = new
                    {
                        ID = activity.Key,
                        Name = activity.Value.Name,
                        Owner = organizationData is null ? "" : organizationData.Name,
                        IsMy = memberData.Id == activity.Value.Owner,
                        Date = activity.Value.LastCapture.ToString("HH:mm, dd:MM"),
                        TimeLastCaptured = $"{Convert.ToInt32((DateTime.Now - activity.Value.LastCapture).TotalMinutes)} минут",
                        PlayersInZone = activity.Value.GetPlayers(memberData.Id).Count,
                    };

                    if (item.IsMy)
                        count++;

                    data.Add(item);
                }

                Trigger.ClientEvent(player, "client.activityWar.open", Config.PRICE_FOR_ANY_ACTIVIY, count, JsonConvert.SerializeObject(data));
            }
            catch (Exception e) { _log.Write("OpenMenu: " + e.ToString()); }
        }

        [RemoteEvent("server.activityWar.shootEvent")]
        public static void OnShootEvent(ExtPlayer player, int targetId)
        {
            try
            {
                var target = Main.GetPlayerByID(targetId);
                if (target is null) return;

                var characterData = player.GetCharacterData();
                if (characterData is null || !target.HasData("AW_DATA_ACTIVE")) return;

                ActivityPoint activity = target.GetData<ActivityPoint>("AW_DATA_ACTIVE");

                if (activity is null) return;
                activity.StopCapture(target);
            }
            catch (Exception e) { _log.Write("OnShootEvent: " + e.ToString()); }
        }

        [ServerEvent(Event.PlayerDeath)]
        public static void OnDeath(ExtPlayer player, ExtPlayer entityKiller, uint weapon)
        {
            try
            {
                if (!player.HasData("AW_DATA_ACTIVE")) return;
                ActivityPoint activity = player.GetData<ActivityPoint>("AW_DATA_ACTIVE");

                if (activity is null) return;
                activity.StopCapture(player);
            }
            catch(Exception ex) { _log.Write("OnDeath: " + ex.ToString()); }
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public static void OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                if (!player.HasData("AW_DATA_ACTIVE")) return;
                ActivityPoint activity = player.GetData<ActivityPoint>("AW_DATA_ACTIVE");

                if (activity is null) return;
                activity.StopCapture();
            }
            catch (Exception e) { _log.Write("OnPlayerDisconnected: " + e.ToString()); }
        }

        [Command("createactivitywar", GreedyArg = true)]
        public static void CMD_CreateActivityPoint(ExtPlayer player, string name)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null || characterData.AdminLVL < 8) return;

                var activity = new ActivityPoint
                {
                    Name = name,
                    Owner = -1,
                    Radius = 60,
                    Heading = Convert.ToInt32(player.Rotation.Z),
                    Point = player.Position,
                    LastCapture = DateTime.Now
                };

                Manager.CreateActivityPoint(activity);
            }
            catch (Exception e) { _log.Write("CMD_CreateActivityPoint: " + e.ToString()); }
        }
    }
}
