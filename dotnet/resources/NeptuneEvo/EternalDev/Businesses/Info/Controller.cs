using EternalCore;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.EternalDev.Businesses.Info.Classes;
using NeptuneEvo.EternalDev.Businesses.Info.Configs;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace NeptuneEvo.EternalDev.Businesses.Info
{
    public class Controller : Script
    {
        public static readonly BusinessInfoConfig Config = ELib.JsonReader.Read<BusinessInfoConfig>(BusinessInfoConfig.Path);
        public static ConcurrentDictionary<int, InfoPoint> Pool = new ConcurrentDictionary<int, InfoPoint>();
        
        [ServerEvent(Event.ResourceStart)]
        public static void OnStart()
        {
            DataTable data = MySQL.QueryRead("SELECT * FROM `business_info_points`");

            if (data != null && data.Rows.Count != 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    try
                    {
                        int id = Convert.ToInt32(row["id"]);
                        Vector3 position = JsonConvert.DeserializeObject<Vector3>(row["position"].ToString());

                        InitPoint(id, new InfoPoint(id, position));
                    }
                    catch (Exception ex) { ELib.Logger.Error($"Error with business #{row["id"]}", ex, nameof(Controller)); }
                }

                ELib.Logger.Done("BusinessInfo system successfully loaded", nameof(Controller));
            }
            else
            {
                ELib.Logger.Warn("BusinessInfo system doesnt loaded", nameof(Controller));
            }

            NAPI.Command.Register(typeof(Controller).GetMethod(nameof(OnCommandCreate)), 
                new RuntimeCommandInfo(Config.COMMAND_CREATE, $"Используейте: /{Config.COMMAND_CREATE} [ID Бизнеса]"));
        }

        private static void InitPoint(int id, InfoPoint point)
        {
            point.GTAElements();
            Pool.TryAdd(id, point);
        }

        [Interaction(ColShapeEnums.BusinessInfo)]
        public static void OnInteraction(ExtPlayer player, int businessId)
        {
            if (!Pool.TryGetValue(businessId, out InfoPoint point)) 
                return;
            point.Interaction(player);
        }

        public static void OnCommandCreate(ExtPlayer player, int businessId)
        {
            var characterData = player.GetCharacterData();
            if (characterData.AdminLVL < Config.ADMIN_LVL) 
                return;

            bool reCreate = false;
            if (Pool.ContainsKey(businessId))
            {
                Pool.TryRemove(businessId, out var oldPoint);
                oldPoint.Destroy();

                reCreate = true;
            }

            var point = new InfoPoint(businessId, player.Position);
            InitPoint(businessId, point);

            if (reCreate)
                MySQL.Query($"UPDATE `business_info_points` SET `position`='{JsonConvert.SerializeObject(point.Position)}' WHERE `id`={point.BusinessId}");
            else
                MySQL.Query($"INSERT INTO `business_info_points` (`id`, `position`) VALUES({point.BusinessId}, '{JsonConvert.SerializeObject(point.Position)}')");
        }
    }
}
