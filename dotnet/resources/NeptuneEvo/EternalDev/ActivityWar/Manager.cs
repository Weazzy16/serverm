using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using GTANetworkAPI;
using Redage.SDK;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using NeptuneEvo.Organizations.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.EternalDev.ActivityWar.Data;
using NeptuneEvo.EternalDev.ActivityWar.Classes;

namespace NeptuneEvo.EternalDev.ActivityWar
{
    public class Manager
    {
        private static readonly nLog _log = new nLog("ActivityWar");

        private static int _lastId = 0;
        public static Dictionary<int, ActivityPoint> Activities = new Dictionary<int, ActivityPoint>();
        
        public static void Initialize()
        {
            try
            {
                var result = MySQL.QueryRead("SELECT * from `family_activities`");
                if (result is null || result.Rows.Count == 0) return;

                foreach(DataRow row in result.Rows)
                {
                    ActivityPoint activity = new ActivityPoint();
                    activity.Owner = Convert.ToInt32(row["owner"]);
                    activity.Name = Convert.ToString(row["name"]);
                    activity.Point = JsonConvert.DeserializeObject<Vector3>(row["point"].ToString());
                    activity.Radius = Convert.ToInt32(row["radius"]);
                    activity.Heading = Convert.ToInt32(row["heading"]);
                    activity.LastCapture = (DateTime)(row["lastcapture"]);

                    int id = Convert.ToInt32(row["id"]);
                    
                    if (id > _lastId)
                        _lastId = id;

                    CreateActivityPoint(activity, id);
                    _log.Write($"Загружено семейный актив #{id} - {activity.Name}");
                }
            }
            catch(Exception e) { _log.Write("Initialize: " + e.ToString()); }
        }

        public static void CreateActivityPoint(ActivityPoint activity, int id = - 1)
        {
            int idkey = id;
            if (id == -1)
                idkey = _lastId++;

            activity.GTAElements();
            Activities.TryAdd(idkey, activity);

            if (id == -1)
            {
                MySQL.Query($"INSERT INTO `family_activities` (`id`,`owner`,`name`,`point`,`heading`,`radius`,`lastcapture`) " +
                    $"VALUES({idkey}, {activity.Owner}, '{activity.Name}', '{JsonConvert.SerializeObject(activity.Point)}', {activity.Heading}, {activity.Radius}, '{MySQL.ConvertTime(activity.LastCapture)}')");
            }
        }
     
        public static bool IsPlayerCaptured(ExtPlayer player)
        {
            var isCaptured = player.HasData("AW_DATA_ACTIVE");
            if (isCaptured)
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете выполнить это действие сейчас!", 3000);

            return isCaptured;
        }

        public static void EveryMinute()
        {
            try
            {
                foreach(var item in Activities)
                {                      
                    ActivityPoint activity = item.Value;

                    if (activity.Owner != -1)
                    {
                        var family = Organizations.Manager.GetOrganizationData(activity.Owner);
                        if (family is null) continue;

                        family.Money += Config.PRICE_FOR_ANY_ACTIVIY;
                    }
                }
            }
            catch(Exception e) { _log.Write("EveryMinute: " + e.ToString()); }
        }

        [Interaction(ColShapeEnums.OrganizationActivityWar)]
        public static void Interaction(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null || !player.HasData("AW_DATA")) return;

                ActivityPoint activity = player.GetData<ActivityPoint>("AW_DATA");
                if (activity is null) return;

                activity.InteractionNpc(player);
            }
            catch(Exception e) { _log.Write("Interaction: " + e.ToString()); }
        }

        public static void UpdateTime(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                var memberOrgData = player.GetOrganizationData();
                if (memberOrgData is null) return;

                foreach(var item in Activities)
                {
                    var capturedOrg = Organizations.Manager.GetOrganizationData(item.Value.Captured);
                    string capturedOrgName = capturedOrg != null ? capturedOrg.Name : "-";

                    var endCapture = item.Value.StartCapture.AddSeconds(Config.TIME_TO_WAR);

                    if (item.Value.Captured != -1 && (item.Value.Captured == memberOrgData.Id || item.Value.Owner == memberOrgData.Id))
                        Trigger.ClientEvent(player, "client.activityWar.start", item.Key, item.Value.Point.X, item.Value.Point.Y, item.Value.Point.Z, item.Value.Radius, (endCapture - DateTime.Now).TotalSeconds, $"{capturedOrgName} захватывает {item.Value.Name}", item.Value.Owner == memberOrgData.Id);
                }
            }
            catch(Exception e) { _log.Write("UpdateTime: "+ e.ToString()); }
        }
    }
}
