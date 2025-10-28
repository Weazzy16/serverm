using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.EternalDev.ActivityWar.Data;
using NeptuneEvo.Organizations.Player;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeptuneEvo.EternalDev.ActivityWar.Classes
{
    public class ActivityPoint
    {
        private static readonly nLog Log = new nLog(nameof(ActivityPoint));

        public int Owner { get; set; } = -1;
        public int Captured { get; set; } = -1;

        public Vector3 Point { get; set; }
        public string Name { get; set; }
        public int Radius { get; set; }
        public int Heading { get; set; }
        public string Timer { get; set; }
        public DateTime LastCapture { get; set; }
        public DateTime StartCapture { get; set; }

        public DateTime Cooldown { get; set; } = DateTime.MinValue;

        public List<ExtPlayer> Players { get; set; } = new List<ExtPlayer>();

        [JsonIgnore]
        private ColShape ColShape;

        [JsonIgnore]
        private ExtColShape PedColShape;

        [JsonIgnore]
        private TextLabel TextLabel;

        [JsonIgnore]
        private Blip Blip;

        [JsonIgnore]
        private Ped Ped;

        public void InteractionNpc(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null || !characterData.IsAlive) return;

                var organizationMemberData = player.GetOrganizationData();
                if (organizationMemberData is null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не член какой-то семьи...", 3000);
                    return;
                }

                if (organizationMemberData.Id == Owner)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Територия и так ваша!", 3000);
                    return;
                }
                
                if (Captured != -1)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Какая-то семья уже захватывает точку", 3000);
                    return;
                }

                if (Cooldown > DateTime.Now)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Недавно пытались захватить точку, подождите некоторое время!", 3000);
                    return;
                }
                
                if (GetPlayers(organizationMemberData.Id).Count < Config.MINIMAL_PLAYERS_FROM_ORGANIZATION_TO_START)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Для захвата точки необходимо минимум {Config.MINIMAL_PLAYERS_FROM_ORGANIZATION_TO_START} человека из вашей семьи", 3000);
                    return;
                }

                if (Owner == -1)
                {
                    Owner = organizationMemberData.Id;
                    Organizations.Manager.SendMessageToAllPlayers(Owner, $"~b~[СЕМЬЯ] Ваша семья захватила точку {Name}");
                    Organizations.Manager.SendMessageToAllPlayers(Owner, $"Ваша семья захватила точку {Name}. Ура!", true);

                    LastCapture = DateTime.Now;

                    Save();
                }
                else
                {
                    var ownerOrganization = Organizations.Manager.GetOrganizationData(Owner);
                    if (ownerOrganization is null)
                    {
                        Owner = -1;
                        InteractionNpc(player);

                        return;
                    }

                    Captured = organizationMemberData.Id;

                    Organizations.Manager.SendMessageToAllPlayers(Owner, $"~b~[СЕМЬЯ] Вашу точку {Name} решили захватить {organizationMemberData.Name}. Дайте им пизды!");
                    Organizations.Manager.SendMessageToAllPlayers(Owner, $"Вашу точку {Name} решили захватить {organizationMemberData.Name}", true);

                    Organizations.Manager.SendMessageToAllPlayers(Captured, $"~b~[СЕМЬЯ] Ваша семья напала на {ownerOrganization.Name} точка: {Name}");

                    player.SetData("AW_DATA_ACTIVE", this);
                    Trigger.SetSharedData(player, "IS_PLAYER_AW_CAPTURED", true);
                    Trigger.ClientEvent(player, "client.activityWar.setCaptured", true);

                    StartCapture = DateTime.Now;

                    foreach (var pDeff in Organizations.Manager.GetMembersInOrganization(Owner))
                         Manager.UpdateTime(pDeff);

                    foreach (var pDeff in Organizations.Manager.GetMembersInOrganization(Captured))
                        Manager.UpdateTime(pDeff);
                  

                    Timer = Timers.StartOnce(Config.TIME_TO_WAR * 1000, () =>
                    {
                        try
                        {
                            StopTimer();
                            if (player is null) return;

                            player.ResetData("AW_DATA_ACTIVE");
                            player.SetSharedData("IS_PLAYER_AW_CAPTURED", false);
                            Trigger.ClientEvent(player, "client.activityWar.setCaptured", false);

                            Organizations.Manager.SendMessageToAllPlayers(Owner, $"~b~[СЕМЬЯ] Ваша семья не смогла отстоять точку: {Name}");
                            Organizations.Manager.SendMessageToAllPlayers(Owner, $"Ваша семья не смогла отстоять точку: {Name}", true);

                            Organizations.Manager.SendMessageToAllPlayers(Captured, $"~b~[СЕМЬЯ] Ваша семья захватила точку: {Name}");
                            Organizations.Manager.SendMessageToAllPlayers(Captured, $"Ваша семья захватила точку: {Name}", true);

                            Owner = organizationMemberData.Id;
                            Captured = -1;

                            LastCapture = DateTime.Now;

                            Trigger.ClientEventForAll("client.activityWar.stop", Manager.Activities.FirstOrDefault(x => x.Value == this).Key);
                            Save();
                        }
                        catch (Exception e) { Log.Write("StartWar.Task: " + e.ToString()); }
                    }, true);
                }
            }
            catch (Exception e) { Log.Write("StartWar: " + e.ToString()); }
        }

        public void Save()
        {
            MySQL.Query($"UPDATE `family_activities` SET `owner`={Owner}, `lastcapture`='{MySQL.ConvertTime(LastCapture)}' " +
                $"WHERE `id`={Manager.Activities.FirstOrDefault(x => x.Value == this).Key}");
        }

        private void StopTimer()
        {
            if (Timer != string.Empty)
            {
                Timers.Stop(Timer);
                Timer = String.Empty;
            }
        }

        public void StopCapture(ExtPlayer player = null)
        {
            try
            {
                if (player != null)
                {
                    player.ResetData("AW_DATA_ACTIVE");
                    player.SetSharedData("IS_PLAYER_AW_CAPTURED", false);
                    Trigger.ClientEvent(player, "client.activityWar.setCaptured", false);
                }

                StopTimer();

                Organizations.Manager.SendMessageToAllPlayers(Owner, $"~b~[СЕМЬЯ] Ваша семья отстояла точку {Name}");
                Organizations.Manager.SendMessageToAllPlayers(Owner, $"Ваша семья отстояла точку {Name}. Ура!", true);

                Organizations.Manager.SendMessageToAllPlayers(Captured, $"~b~[СЕМЬЯ] Ваша семья не смогла захватить точку {Name}");
                Organizations.Manager.SendMessageToAllPlayers(Captured, $"Ваша семья проиграла в захвате точки {Name}", true);

                Captured = -1;
                LastCapture = DateTime.Now;

                Cooldown = DateTime.Now.AddSeconds(10);
                Trigger.ClientEventForAll("client.activityWar.stop", Manager.Activities.FirstOrDefault(x => x.Value == this).Key);
            }
            catch (Exception e) { Log.Write("StopCapture: " + e.ToString()); }
        }

        public List<ExtPlayer> GetPlayers(int organizationId)
        {
            return Players.Where(x => x.IsOrganizationMemberData() && x.GetOrganizationData().Id == organizationId).ToList();
        }

        public void GTAElements()
        {
            Blip = NAPI.Blip.CreateBlip(642, Point, 1f, 4, $"Корпорация: {Name}", 255, 0, true, 0, 0);
            Ped = NAPI.Ped.CreatePed(NAPI.Util.GetHashKey("cs_barry"), Point, Heading, false, true, true, true, 0);
            
            TextLabel = NAPI.TextLabel.CreateTextLabel($"Корпорация: \n~y~{Name}", Point + new Vector3(0, 0, .5), 10f, 1f, 0, new Color(255, 255, 255), false, 0);

            PedColShape = CustomColShape.CreateCylinderColShape(Point, 2, 2, 0, ColShapeEnums.OrganizationActivityWar);

            ColShape = NAPI.ColShape.CreateSphereColShape(Point, Radius, 0);
            ColShape.OnEntityEnterColShape += (s, e) =>
            {
                try
                {
                    var player = e as ExtPlayer;
                    player.SetData("AW_DATA", this);

                    var memberData = player.GetOrganizationData();
                    if (memberData is null) return;

                    Players.Add(player);
                    if (Owner != -1) return;

                    var ownerOrganization = Organizations.Manager.GetOrganizationData(Owner);

                    if (ownerOrganization is null)
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы попали на свободную територию {Name}!", 3000);
                    else if (Owner != memberData.Id)
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы попали на територию семьи {ownerOrganization.Name}!", 3000);
                    else
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы попали на територию своей семьи, добро пожаловать!", 3000);
                }
                catch (Exception ex) { Log.Write("OnEntityEnterColShape: " + ex.ToString()); }
            };

            ColShape.OnEntityExitColShape += (s, e) =>
            {
                try
                {
                    e.ResetData("AW_DATA");

                    if (Players.Contains((ExtPlayer)e))
                        Players.Remove((ExtPlayer)e);
                }
                catch (Exception ex) { Log.Write("OnEntityExitColShape: " + ex.ToString()); }
            };
        }
    }
}
