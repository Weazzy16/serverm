using System;
using System.Data;
using GTANetworkAPI;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Linq;
using Redage.SDK;
using NeptuneEvo.Players;
using NeptuneEvo.Handles;
using NeptuneEvo.EternalDev.Containers.Classes;
using NeptuneEvo.EternalDev.Containers.Data;
using EternalCore;

namespace NeptuneEvo.EternalDev.Containers
{
    public class Manager
    {
        public static ConcurrentDictionary<int, Container> Containers = new ConcurrentDictionary<int, Container>();
        public static bool IsAuctionStarted = false;
        private static Blip _blip = null;

        public static void OnStart()
        {
            try
            {
                var result = MySQL.QueryRead($"SELECT * FROM `containers`");
                if (result is null || result.Rows.Count == 0)
                {
                    ELib.Logger.Info("Database `containers` is empty", nameof(Manager));
                    return;
                }

                foreach (DataRow row in result.Rows)
                {
                    int id = Convert.ToInt32(row["id"]);
                    ContainerType containerType = (ContainerType)Enum.Parse(typeof(ContainerType), row["type"].ToString());

                    var container = new Container(id, containerType)
                    {
                        Position = JsonConvert.DeserializeObject<Vector3>(row["position"].ToString()),
                        Rotation = JsonConvert.DeserializeObject<Vector3>(row["rotation"].ToString()),
                        Price = Convert.ToInt32(row["price"]),
                        MaxBet = Convert.ToInt32(row["max_bet"]),
                        Step = Convert.ToInt32(row["step"]),
                        NonRefDep = Convert.ToInt32(row["non_ref_dep"])
                    };

                    if (Containers.TryAdd(id, container))
                        container.GTAElements();
                }

                _blip = NAPI.Blip.CreateBlip(Config.BLIP_SPRITE, Config.LOCATION, 1f, Config.BLIP_INACTIVE_COLOR, Config.BLIP_NAME, 255, 0, true, 0, 0);
                Timers.Start(1000, () => OnEverySecond());

                ELib.Logger.Done("Containers system successfully loaded", nameof(Manager));
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnStart), ex, nameof(Manager)); }
        }

        public static void OnConnected(ExtPlayer player)
        {
            try
            {
                Trigger.ClientEvent(player, "client.containers.create", JsonConvert.SerializeObject(
                    Containers.Values.Select(cont => new
                    {
                        cont.Id,
                        cont.Position,
                        cont.Rotation,
                        Price = cont.GetCurrentPrice(),
                        cont.IsAuctionStarted,
                        cont.Settings.Name,
                        cont.Settings.IsDonate
                    })
                ));
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnConnected), ex, nameof(Manager)); }
        }

        public static void OnDisconnected(ExtPlayer player)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null || session.CurrentContainer is null)
                    return;

                session.CurrentContainer.UnSubscribe(player);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnDisconnected), ex, nameof(Manager)); }
        }

        public static void OnFinish()
        {
            try
            {
                if (!IsAuctionStarted || Containers.Values.Where(x => x.IsAuctionStarted).Count() != 0)
                    return;

                //!{{#ffcc00}}
                NAPI.Chat.SendChatMessageToAll("Завершился аукцион грузовых контейнеров. Поздравляем победителей!");
                NAPI.Task.Run(() => NAPI.Blip.SetBlipColor(_blip, Config.BLIP_INACTIVE_COLOR));
                IsAuctionStarted = false;
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnFinish), ex, nameof(Manager)); }
        }

        public static void OnEverySecond()
        {
            try
            {
                if (IsAuctionStarted)
                {
                    foreach (var container in Containers.Values)
                        container.OnEverySecond();
                }
                else
                {
                    DateTime now = DateTime.Now;
                    if (!Config.AUCTION_TIME.TryGetValue(now.DayOfWeek, out var hours) || !hours.Contains(now.Hour) || now.Minute != 0)
                        return;

                    TryStart();
                }
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnEverySecond), ex, nameof(Manager)); }
        }

        public static void TryStart()
        {
            try
            {
                if (IsAuctionStarted)
                    return;

                IsAuctionStarted = true;
                foreach (var container in Containers.Values)
                    container.StartAuction();

                //!{{#ffcc00}}
                NAPI.Chat.SendChatMessageToAll("Начался аукцион грузовых контейнеров! Удачных торгов!");
                NAPI.Task.Run(() => NAPI.Blip.SetBlipColor(_blip, Config.BLIP_ACTIVE_COLOR));
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(TryStart), ex, nameof(Manager)); }
        }
    }
}
