using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Database;
using GTANetworkAPI;
using LinqToDB;
using Localization;
using NeptuneEvo.Handles;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.Functions;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Players.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Table.Models;
using NeptuneEvo.World.War.Models;
using Newtonsoft.Json;
using System.Drawing;
using Org.BouncyCastle.Asn1.Pkcs;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.X509;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Fractions.Models;

namespace NeptuneEvo.Fractions
{
    public class GangsCapture : Script
    {
        private static readonly nLog _nLog = new nLog("Fractions.GangCapture");

        public static Dictionary<int, GangPoint> GangPoints = new Dictionary<int, GangPoint>();

        public static Dictionary<int, int> GangPointsColor { get; } = new Dictionary<int, int>
        {
            { (int) Models.Fractions.FAMILY, 52 },
            { (int) Models.Fractions.BALLAS, 58 },
            { (int) Models.Fractions.VAGOS, 70 },
            { (int) Models.Fractions.MARABUNTA, 77 },
            { (int) Models.Fractions.BLOOD, 59 },
        };

        private static Dictionary<int, DateTime> _nextCaptDate = new Dictionary<int, DateTime>
        {
            { (int) Models.Fractions.FAMILY, DateTime.Now },
            { (int) Models.Fractions.BALLAS, DateTime.Now },
            { (int) Models.Fractions.VAGOS, DateTime.Now },
            { (int) Models.Fractions.MARABUNTA, DateTime.Now },
            { (int) Models.Fractions.BLOOD, DateTime.Now },
        };

        private static Dictionary<int, DateTime> _protectDate = new Dictionary<int, DateTime>
        {
            { (int) Models.Fractions.FAMILY, DateTime.Now },
            { (int) Models.Fractions.BALLAS, DateTime.Now },
            { (int) Models.Fractions.VAGOS, DateTime.Now },
            { (int) Models.Fractions.MARABUNTA, DateTime.Now },
            { (int) Models.Fractions.BLOOD, DateTime.Now },
        };

        private static Vector3[] _gangZones = new Vector3[90]
        {
            // left side
            new Vector3(-200.8397, -1431.556, 30.18104),
            new Vector3(-100.8397, -1431.556, 30.18104),
            new Vector3(-0.8397, -1431.556, 30.18104),
            new Vector3(99.1603, -1431.556, 30.18104),
            new Vector3(-200.8397, -1531.556, 30.18104),
            new Vector3(-100.8397, -1531.556, 30.18104),
            new Vector3(-0.8397, -1531.556, 30.18104),
            new Vector3(99.1603, -1531.556, 30.18104),
            new Vector3(199.1603, -1531.556, 30.18104),
            new Vector3(299.1603, -1531.556, 30.18104),
            new Vector3(399.1603, -1531.556, 30.18104),
            new Vector3(499.1603, -1531.556, 30.18104),
            new Vector3(-200.8397, -1631.556, 30.18104),
            new Vector3(-100.8397, -1631.556, 30.18104),
            new Vector3(-0.8397, -1631.556, 30.18104),
            new Vector3(99.1603, -1631.556, 30.18104),
            new Vector3(199.1603, -1631.556, 30.18104),
            new Vector3(299.1603, -1631.556, 30.18104),
            new Vector3(399.1603, -1631.556, 30.18104),
            new Vector3(499.1603, -1631.556, 30.18104),
            new Vector3(599.1603, -1631.556, 30.18104),
            new Vector3(-100.8397, -1731.556, 30.18104),
            new Vector3(-0.8397, -1731.556, 30.18104),
            new Vector3(99.1603, -1731.556, 30.18104),
            new Vector3(199.1603, -1731.556, 30.18104),
            new Vector3(299.1603, -1731.556, 30.18104),
            new Vector3(399.1603, -1731.556, 30.18104),
            new Vector3(499.1603, -1731.556, 30.18104),
            new Vector3(599.1603, -1731.556, 30.18104),
            new Vector3(-0.8397, -1831.556, 30.18104),
            new Vector3(99.1603, -1831.556, 30.18104),
            new Vector3(199.1603, -1831.556, 30.18104),
            new Vector3(299.1603, -1831.556, 30.18104),
            new Vector3(399.1603, -1831.556, 30.18104),
            new Vector3(499.1603, -1831.556, 30.18104),
            new Vector3(599.1603, -1831.556, 30.18104),
            new Vector3(99.1603, -1931.556, 30.18104),
            new Vector3(199.1603, -1931.556, 30.18104),
            new Vector3(299.1603, -1931.556, 30.18104),
            new Vector3(399.1603, -1931.556, 30.18104),
            new Vector3(499.1603, -1931.556, 30.18104),
            new Vector3(599.1603, -1931.556, 30.18104),
            new Vector3(199.1603, -2031.556, 30.18104),
            new Vector3(299.1603, -2031.556, 30.18104),
            new Vector3(399.1603, -2031.556, 30.18104),
            new Vector3(499.1603, -2031.556, 30.18104),
            new Vector3(299.1603, -2131.556, 30.18104),
            new Vector3(399.1603, -2131.556, 30.18104),
            //right side
            new Vector3(1368.8984, -1501.556, 28.17772),
            new Vector3(1268.8984, -1501.556, 28.17772),
            new Vector3(1368.8984, -1601.556, 28.17772),
            new Vector3(1268.8984, -1601.556, 28.17772),
            new Vector3(1168.8984, -1601.556, 28.17772),
            new Vector3(868.8984, -1601.556, 28.17772),
            new Vector3(768.8984, -1601.556, 28.17772),
            new Vector3(1368.8984, -1701.556, 28.17772),
            new Vector3(1268.8984, -1701.556, 28.17772),
            new Vector3(1168.8984, -1701.556, 28.17772),
            new Vector3(968.8984, -1701.556, 28.17772),
            new Vector3(868.8984, -1701.556, 28.17772),
            new Vector3(768.8984, -1701.556, 28.17772),
            new Vector3(1268.8984, -1801.556, 28.17772),
            new Vector3(1168.8984, -1801.556, 28.17772),
            new Vector3(968.8984, -1801.556, 28.17772),
            new Vector3(868.8984, -1801.556, 28.17772),
            new Vector3(768.8984, -1801.556, 28.17772),
            new Vector3(1068.8984, -1901.556, 28.17772),
            new Vector3(968.8984, -1901.556, 28.17772),
            new Vector3(868.8984, -1901.556, 28.17772),
            new Vector3(768.8984, -1901.556, 28.17772),
            new Vector3(1068.8984, -2001.556, 28.17772),
            new Vector3(968.8984, -2001.556, 28.17772),
            new Vector3(868.8984, -2001.556, 28.17772),
            new Vector3(768.8984, -2001.556, 28.17772),
            new Vector3(1068.8984, -2101.556, 28.17772),
            new Vector3(968.8984, -2101.556, 28.17772),
            new Vector3(868.8984, -2101.556, 28.17772),
            new Vector3(768.8984, -2101.556, 28.17772),
            new Vector3(1068.8984, -2201.556, 28.17772),
            new Vector3(968.8984, -2201.556, 28.17772),
            new Vector3(868.8984, -2201.556, 28.17772),
            new Vector3(768.8984, -2201.556, 28.17772),
            new Vector3(1068.8984, -2301.556, 28.17772),
            new Vector3(968.8984, -2301.556, 28.17772),
            new Vector3(868.8984, -2301.556, 28.17772),
            new Vector3(768.8984, -2301.556, 28.17772),
            new Vector3(1068.8984, -2401.556, 28.17772),
            new Vector3(968.8984, -2401.556, 28.17772),
            new Vector3(868.8984, -2401.556, 28.17772),
            new Vector3(768.8984, -2401.556, 28.17772),
        };

        private static Vector3[] _tpCaptureCoords = new Vector3[90]
        {
            new Vector3(-200.8397, -1431.556, 31.58104),
            new Vector3(-92.435104, -1420.9907, 29.644262),
            new Vector3(-5.3002477, -1412.8955, 29.287712),
            new Vector3(99.1603, -1431.556, 30.18104),
            new Vector3(-200.8397, -1531.556, 33.78104),
            new Vector3(-100.8397, -1531.556, 33.98104),
            new Vector3(-0.8397, -1531.556, 30.18104),
            new Vector3(99.1603, -1531.556, 30.18104),
            new Vector3(223.01283, -1515.8212, 29.291681),
            new Vector3(299.1603, -1531.556, 30.18104),
            new Vector3(399.1603, -1531.556, 30.18104),
            new Vector3(502.19296, -1536.3683, 29.248728),
            new Vector3(-200.77438, -1633.441, 33.6612),
            new Vector3(-86.67318, -1655.2671, 29.33629),
            new Vector3(-0.8397, -1631.556, 30.18104),
            new Vector3(99.1603, -1631.556, 30.18104),
            new Vector3(199.1603, -1631.556, 30.18104),
            new Vector3(300.8472, -1632.7241, 32.516785),
            new Vector3(399.1603, -1631.556, 30.18104),
            new Vector3(497.1187, -1630.1417, 29.375402),
            new Vector3(599.1603, -1631.556, 25.18104),
            new Vector3(-100.8397, -1731.556, 30.18104),
            new Vector3(-0.8397, -1731.556, 30.18104),
            new Vector3(99.1603, -1731.556, 30.18104),
            new Vector3(199.76115, -1733.3491, 29.16118),
            new Vector3(299.1603, -1731.556, 30.18104),
            new Vector3(399.1603, -1731.556, 30.18104),
            new Vector3(499.1603, -1731.556, 30.18104),
            new Vector3(599.1603, -1731.556, 30.18104),
            new Vector3(-0.8397, -1831.556, 25.18104),
            new Vector3(99.1603, -1831.556, 26.18104),
            new Vector3(193.4253, -1845.384, 27.146263),
            new Vector3(299.1603, -1831.556, 27.18104),
            new Vector3(395.85574, -1829.1324, 28.229649),
            new Vector3(492.4637, -1838.8887, 27.49535),
            new Vector3(599.1603, -1831.556, 25.18104),
            new Vector3(99.1603, -1931.556, 21.18104),
            new Vector3(199.1603, -1931.556, 22.18104),
            new Vector3(299.1603, -1931.556, 26.18104),
            new Vector3(393.26226, -1936.4803, 24.56807),
            new Vector3(488.4064, -1923.7424, 25.242794),
            new Vector3(599.1603, -1931.556, 21.18104),
            new Vector3(199.1603, -2031.556, 19.18104),
            new Vector3(291.71225, -2021.907, 19.655933),
            new Vector3(405.5024, -2037.1438, 22.370207),
            new Vector3(499.1603, -2031.556, 26.18104),
            new Vector3(299.1603, -2131.556, 16.18104),
            new Vector3(399.1603, -2131.556, 19.18104),
            new Vector3(768.8984, -2401.556, 21.17772),
            new Vector3(894.2473, -2408.1538, 29.385946),
            new Vector3(976.2924, -2405.001, 30.509533),
            new Vector3(1048.0906, -2394.8113, 30.183367),
            new Vector3(768.8984, -2301.556, 28.17772),
            new Vector3(855.897, -2306.1804, 30.345802),
            new Vector3(926.09955, -2301.0046, 30.516901),
            new Vector3(1053.4723, -2306.1409, 30.590824),
            new Vector3(770.6354, -2201.574, 29.140207),
            new Vector3(868.8984, -2201.556, 28.17772),
            new Vector3(968.8984, -2201.556, 31.17772),
            new Vector3(1068.8984, -2201.556, 31.17772),
            new Vector3(768.8984, -2101.556, 30.17772),
            new Vector3(868.8984, -2101.556, 31.17772),
            new Vector3(967.56067, -2099.8726, 30.833656),
            new Vector3(1068.8984, -2101.556, 33.17772),
            new Vector3(768.8984, -2001.556, 30.17772),
            new Vector3(868.8984, -2001.556, 31.17772),
            new Vector3(949.22186, -2000.7246, 30.115568),
            new Vector3(1060.9913, -2002.5533, 31.016212),
            new Vector3(768.8984, -1901.556, 30.17772),
            new Vector3(881.3785, -1906.5698, 30.655567),
            new Vector3(969.41516, -1894.0066, 31.144627),
            new Vector3(1068.8984, -1901.556, 32.17772),
            new Vector3(793.1398, -1807.5277, 29.203436),
            new Vector3(868.8984, -1801.556, 30.17772),
            new Vector3(960.30194, -1801.2676, 31.06433),
            new Vector3(1168.8984, -1801.556, 37.17772),
            new Vector3(1268.8984, -1801.556, 43.17772),
            new Vector3(770.3192, -1726.1405, 29.48708),
            new Vector3(853.3811, -1704.9241, 29.25864),
            new Vector3(968.8984, -1701.556, 30.17772),
            new Vector3(1168.8984, -1701.556, 36.17772),
            new Vector3(1268.8984, -1701.556, 56.17772),
            new Vector3(1368.8984, -1701.556, 62.17772),
            new Vector3(770.2724, -1603.4686, 31.10974),
            new Vector3(868.8984, -1601.556, 31.17772),
            new Vector3(1168.8984, -1601.556, 35.17772),
            new Vector3(1268.8984, -1601.556, 54.17772),
            new Vector3(1368.8984, -1601.556, 54.17772),
            new Vector3(1268.8984, -1501.556, 40.17772),
            new Vector3(1368.8984, -1501.556, 58.17772),
        };

        private const float RANGE = 100f;

        private static readonly List<HistoryDTO> _historyCaptures = new List<HistoryDTO>();

        [ServerEvent(Event.ResourceStart)]
        public static void OnResourceStart()
        {
            try
            {
                using DataTable result = MySQL.QueryRead("SELECT * FROM `gangspoints`");

                if (result == null || result.Rows.Count == 0)
                    return;

                foreach (DataRow Row in result.Rows)
                {
                    GangPoint region = new GangPoint();

                    region.ID = Convert.ToInt32(Row["id"]);
                    region.GangOwner = Convert.ToInt32(Row["gangid"]);

                    if (region.ID >= 90)
                        break;

                    GangPoints.Add(region.ID, region);
                }

                foreach (var region in GangPoints.Values)
                {
                    CustomColShape.Create2DColShape(_gangZones[region.ID].X, _gangZones[region.ID].Y, 100, 100, 0, ColShapeEnums.GangZone, region.ID);
                    CustomColShape.Create2DColShape(_gangZones[region.ID].X, _gangZones[region.ID].Y, 100, 100, 0, ColShapeEnums.WarGangZone, region.ID);
                }
            }
            catch (Exception e)
            {
                _nLog.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static void UpdateZone(byte id, int fractionId, ushort attackingId, ushort protectingId)
        {
            try
            {
                _protectDate[protectingId] = DateTime.Now.AddMinutes(20);
                _protectDate[attackingId] = DateTime.Now.AddMinutes(20);

                var nextCapt = DateTime.Now.AddMinutes(Main.ServerSettings.CaptureNextTimeMinutes);

                _nextCaptDate[attackingId] = nextCapt;

                if (!GangPoints.TryGetValue(id, out var region))
                    return;

                if (region.GangOwner == fractionId)
                    return;

                region.CaptureWarData.InWar = false;
                region.GangOwner = fractionId;
                region.Save();

                Trigger.ClientEventForAll("setZoneFlash", region.ID, false);
                Trigger.ClientEventForAll("setZoneColor", region.ID, GangPointsColor[region.GangOwner]);
            }
            catch (Exception e)
            {
                Debugs.Repository.Exception(e);
            }
        }

        public static void LoadBlips(ExtPlayer player)
        {
            try
            {
                if (!player.IsCharacterData())
                    return;

                var colors = new List<int>();

                foreach (var g in GangPoints.Values)
                    colors.Add(GangPointsColor[g.GangOwner]);

                Trigger.ClientEvent(player, "client.capture.loadBlips", JsonConvert.SerializeObject(colors));

                var wars = World.War.Repository.Wars.Values.FirstOrDefault(w => w.Type == WarType.Gangs);

                if (wars != null)
                    Trigger.ClientEvent(player, "setZoneFlash", wars.ObjectId, true);
            }
            catch (Exception e)
            {
                _nLog.Write($"LoadBlips Exception: {e.ToString()}");
            }
        }

        [Interaction(ColShapeEnums.WarGangZone, In: true)]
        public void InWarGangZone(ExtPlayer player, int index)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData.TimersData.GangTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.GangTimer);
                    sessionData.TimersData.GangTimer = null;
                }
            }
            catch (Exception e)
            {
                _nLog.Write($"InWarGangZone Exception: {e.ToString()}");
            }
        }

        //[Interaction(ColShapeEnums.WarGangZone, Out: true)]
        //public void OutWarGangZone(ExtPlayer player, int index)
        //{
        //    var sessionData = player.GetSessionData();
        //    sessionData.TimersData.GangTimer = Timers.Start(10000, () =>
        //    {
        //        NAPI.Task.Run(() =>
        //        {
        //            player.Health -= 10;
        //            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вернитесь в зону капта", 5000);
        //        });
        //    });
        //}

        [RemoteEvent("server.capture.getMembers")]
        private static void _getMembers(ExtPlayer player, int squareId)
        {
            var square = GangPoints[squareId];

            List<object> result = new List<object>();

            foreach (var member in Manager.AllMembers[player.GetFractionId()])
            {
                int uuid = member.UUID;

                result.Add(new
                {
                    nickname = Main.PlayerNames[uuid],
                    uuid = uuid,
                    status = Main.GetPlayerByUUID(uuid) != null ? "Онлайн" : member.LastLoginDate.ToString("dd.MM HH:mm")
                });
            }

            Trigger.ClientEvent(player, "client.capture.getMembers", JsonConvert.SerializeObject(result));
        }
        [RemoteEvent("server.capture.cancel")]
        public static void CaptureCancel(ExtPlayer player, int squareId)
        {
            if (GangPoints.ContainsKey(squareId) == false) return;
            var square = GangPoints[squareId];
            int fractionId = player.GetFractionId();

            if (fractionId == (int)Models.Fractions.None)
                return;

            if (!FunctionsAccess.IsWorking("CMD_capture"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                return;
            }

            if (!player.IsFractionAccess(RankToAccess.Capture))
                return;

            var sessionData = player.GetSessionData();
            var characterData = player.GetCharacterData();

            if (sessionData == null || characterData == null)
                return;

            if (characterData.IsBannedCrime == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вам ограничили доступ к системам capture и bizwar. Причина: {characterData.BanCrimeReason}.", 5000);
                return;
            }

            if (square.CaptureWarData.InWar == false)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "За эту территорию не назначена война", 5000);
                return;
            }
            var war = World.War.Repository.Wars.Values.FirstOrDefault(el => el.ObjectId == square.ID);
            if(war != null)
            {
                World.War.Repository.EndWar(war.Id);
                war.Delete();
            }
            Trigger.SendToAdmins(1, $"{ChatColors.Red}[A] {player.Name} ({player.Value}) из {Manager.GetName(fractionId)} отменил capture за территорию #{square.ID}.");
            GameLog.FracLog(fractionId, player.GetUUID(), -1, player.Name, "-1", $"cancel capture({square.ID}, {Manager.FractionNames[square.GangOwner]})");
            square.CaptureWarData = new CaptureWarData();

            Notify.Send(player, NotifyType.Success, NotifyPosition.Center, $"Вы отменили капт", 5000);
            CaptureTabletLoad(player);
        }

        [RemoteEvent("server.capture.getInfoCapture")]
        private static void _getInfoCapture(ExtPlayer player, int squareId)
        {
            var square = GangPoints[squareId];

            List<object> selectedMembers = new List<object>();
            List<object> nonSelectedMembers = new List<object>();

            foreach (var uuid in square.CaptureWarData.SelectedMembers)
            {
                selectedMembers.Add(new
                {
                    nickname = Main.PlayerNames[uuid],
                    uuid = uuid,
                    status = Main.GetPlayerByUUID(uuid) != null ? "Онлайн" : Manager.AllMembers[player.GetFractionId()].Find(m => m.UUID == uuid).LastLoginDate.ToString("dd.MM HH:mm")
                });
            }

            foreach (var member in Manager.AllMembers[player.GetFractionId()].FindAll(m => !square.CaptureWarData.SelectedMembers.Contains(m.UUID)))
            {
                int uuid = member.UUID;

                nonSelectedMembers.Add(new
                {
                    nickname = Main.PlayerNames[uuid],
                    uuid = uuid,
                    status = Main.GetPlayerByUUID(uuid) != null ? "Онлайн" : member.LastLoginDate.ToString("dd.MM HH:mm")
                });
            }

            object info = new
            {
                index = squareId,
                fractionName = Manager.GetName(square.GangOwner),
                hour = square.CaptureWarData.StartDate.Hour,
                minute = square.CaptureWarData.StartDate.Minute,
                countMembers = square.CaptureWarData.CountMembers,
                armor = square.CaptureWarData.Armor,
                resist = square.CaptureWarData.Resist,
                calibreIndex = square.CaptureWarData.CalibreIndex,
                selectedMembers = selectedMembers,
                nonSelectedMembers = nonSelectedMembers,
            };

            Trigger.ClientEvent(player, "client.capture.getInfoCapture", JsonConvert.SerializeObject(info));
        }

        [RemoteEvent("server.capture.getNotifications")]
        private static void _getNotifications(ExtPlayer player)
        {
            Models.Fractions fractionPlayer = (Models.Fractions)player.GetFractionId();

            List<object> history = new List<object>();

            foreach (var h in _historyCaptures.FindAll(x => x.Defenders == fractionPlayer || x.Attacker == fractionPlayer))
            {
                if (h.Defenders == fractionPlayer)
                {
                    history.Add(new
                    {
                        text = $"На вас напала банда {h.Attacker}, назначил атаку {h.NicknameAttacker}",
                        receiver = Manager.FractionNames[Convert.ToInt32(h.Defenders)],
                        date = h.GetFormattedTime()
                    });
                }
                else
                {
                    history.Add(new
                    {
                        text = $"Вы атаковали банду {h.Defenders}, назначил атаку {h.NicknameAttacker}",
                        receiver = Manager.FractionNames[Convert.ToInt32(h.Defenders)],
                        date = h.GetFormattedTime()
                    });
                }
            }

            Trigger.ClientEvent(player, "client.capture.getNotifications", JsonConvert.SerializeObject(history));
        }

        [RemoteEvent("server.capture.beatCapture")]
        private static void _beatCapture(ExtPlayer player, int squareId, int countMembers, int startHour, int startMinute, string calibreName, string selectedMembersStr, bool armor, bool resist, ushort calibreIndex)
        {
            var selectedMembers = JsonConvert.DeserializeObject<List<int>>(selectedMembersStr);
            var square = GangPoints[squareId];
            int fractionId = player.GetFractionId();

            if (fractionId == (int)Models.Fractions.None)
                return;

            if (!player.IsFractionAccess(RankToAccess.Capture))
                return;

            var sessionData = player.GetSessionData();
            var characterData = player.GetCharacterData();

            if (sessionData == null || characterData == null)
                return;

            if (characterData.IsBannedCrime == true)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вам ограничили доступ к системам capture и bizwar. Причина: {characterData.BanCrimeReason}.", 5000);
                return;
            }

            //if (square.CaptureWarData.InWar == true)
            //{
            //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "За эту территорию уже забита война", 5000);
            //    return;
            //}

            //if (countMembers > selectedMembers.Count)
            //{
            //    Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"Недостаточно человек назначено на капт", 5000);
            //    return;
            //}



            var myFractionName = Manager.GetName(fractionId);
            var myAttacks = GangPoints.Values.Where(el=>
                el.CaptureWarData.InWar == true &&
                el.CaptureWarData.AttackGangName == myFractionName
            ).ToList();

            if (myAttacks.Count > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"У вас закончился лимит захватов", 5000);
                return;
            }

            var defends = GangPoints.Values.Where(el=>el.CaptureWarData.InWar == true && el.GangOwner == square.GangOwner).ToList();
            if (defends.Count > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"У банды закончился лимит защит", 5000);
                return;
            }

            DateTime now = DateTime.Now;

            DateTime startDate = new DateTime(now.Year, now.Month, now.Day, startHour, startMinute, 0);
            if (startDate < now)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"Нельзя создать капт в прошлом", 5000);
                return;
            }

            //есть ли капты в этот момент
            //ищем защиты своих территорий в диапазоне 60 минут
            var myDefendsByTime = GangPoints.Values.Where(el =>
                el.CaptureWarData.InWar == true &&
                el.GangOwner == fractionId &&
                Math.Abs(el.CaptureWarData.StartDate.Subtract(startDate).TotalMinutes) < 60
            ).ToList();
            if(myDefendsByTime.Count > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"В это время у вас будет капт", 5000);
                return;
            }
            //ищем атаки на своего врага в диапазоне 60 минут
            var myAttacksByTime = GangPoints.Values.Where(el=>
                el.CaptureWarData.InWar == true &&
                el.GangOwner == square.GangOwner &&
                Math.Abs(el.CaptureWarData.StartDate.Subtract(startDate).TotalMinutes) < 60
                ).ToList();
            if (myAttacksByTime.Count > 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.Center, $"В это время у вражеской банды будет капт", 5000);
                return;
            }
            //


            _historyCaptures.Add(new HistoryDTO((Models.Fractions)fractionId, (Models.Fractions)square.GangOwner, player.GetName()));

            Trigger.SendToAdmins(1, $"{ChatColors.Red}[A] {player.Name} ({player.Value}) из {myFractionName} спровоцировал capture за территорию {square.CaptureWarData.DefendGangName}.");
            GameLog.FracLog(fractionId, player.GetUUID(), -1, player.Name, "-1", $"capture({square.ID}, {Manager.FractionNames[square.GangOwner]})");






            square.CaptureWarData = new CaptureWarData(true, startDate, countMembers, myFractionName, fractionId,
                                                       Manager.GetName(square.GangOwner), selectedMembers, armor, resist, calibreIndex);

            Timers.StartOnce((int)Math.Round(startDate.Subtract(now).TotalSeconds) * 1000, () =>
            {
                StartCapture(square.ID);
            });

            string message = _getMessage(false, startHour, startMinute, countMembers, square.CaptureWarData.GetAmmoType().ToString(), armor, resist);

            foreach (var m in Manager.AllMembers[fractionId])
            {
                ExtPlayer p = Main.GetPlayerByUUID(m.UUID);

                if (p != null)
                    Notify.SendBanner(p, "Война за территорию", message);
            }

            string messageEnemy = _getMessage(true, startHour, startMinute, countMembers, square.CaptureWarData.GetAmmoType().ToString(), armor, resist);

            foreach (var m in Manager.AllMembers[square.GangOwner])
            {
                ExtPlayer p = Main.GetPlayerByUUID(m.UUID);

                if (p != null)
                    Notify.SendBanner(p, "Война за территорию", messageEnemy);
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.Center, $"Вы успешно забили капт на {startHour}:{startMinute}", 5000);
            CaptureTabletLoad(player);
        }

        private static string _getMessage(bool forDefenders, int startHour, int startMinute, int countMembers, string ammoType, bool armor, bool resist)
        {
            string result = string.Empty;

            result += $"{(forDefenders ? "Вам" : "Вы")} назначили войну на {startHour}:{startMinute}, {countMembers}x{countMembers} на {ammoType}. ";

            if (armor && resist)
                result += "Броня, резист разрешены";
            else if (armor)
                result += "Броня разрешена";
            else if (resist)
                result += "Резит разрешен";

            return result;
        }

        [Command("capture")]
        public static void OpenTablet(ExtPlayer player)
        {
            int countYourSquares = GangPoints.Values.Where(p => p.GangOwner == player.GetFractionId()).Count();


            var squares = JsonConvert.SerializeObject(_getSquares());
            var name = Manager.GetName(player.GetFractionId());

            //Console.WriteLine(countYourSquares);
            //Console.WriteLine(squares);
            //Console.WriteLine(name);


            Trigger.ClientEvent(player, "client.capture.open", countYourSquares, squares, name);
        }
        [RemoteEvent("server.capture.load")]
        public static void CaptureTabletLoad(ExtPlayer player)
        {
            int countYourSquares = GangPoints.Values.Where(p => p.GangOwner == player.GetFractionId()).Count();


            var squares = JsonConvert.SerializeObject(_getSquares());
            var name = Manager.GetName(player.GetFractionId());

            //Console.WriteLine(countYourSquares);
            //Console.WriteLine(squares);
            //Console.WriteLine(name);

            Trigger.ClientEvent(player, "client.capture.load", countYourSquares, squares, name);
        }



        private static List<object> _getSquares()
        {
            List<object> squares = new List<object>();

            foreach (var square in GangPoints.Values)
            {
                squares.Add(new
                {
                    index = square.ID,
                    color = _getFractionColor(square.GangOwner),
                    fractionName = Manager.GetName(square.GangOwner),
                    attackGangName = square.CaptureWarData.AttackGangName,
                    defendGangName = square.CaptureWarData.DefendGangName,
                    countMembers = square.CaptureWarData.CountMembers,
                    calibreIndex = square.CaptureWarData.CalibreIndex,
                    startDate = square.CaptureWarData.GetFormattedTime(),
                    inWar = square.CaptureWarData.InWar
                });
            }

            return squares;
        }

        private static string _getFractionColor(int fractionId)
        {
            return fractionId switch
            {
                (int)Models.Fractions.FAMILY => "families",
                (int)Models.Fractions.BALLAS => "ballas",
                (int)Models.Fractions.VAGOS => "vagos",
                (int)Models.Fractions.MARABUNTA => "marabunta",
                (int)Models.Fractions.BLOOD => "bloods",
                _ => "",
            };
        }

        public static void StartCapture(int squareId)
        {
            try
            {
                if (GangPoints.ContainsKey(squareId) == false) return;
                var square = GangPoints[squareId];
                var fracId = square.CaptureWarData.AttackingId;


                var warData = new Players.Models.WarData
                {
                    ObjectId = (ushort) square.ID,
                    Type = WarType.Gangs,
                    MapName = String.Empty,
                    MapId = (ushort) square.ID,
                    Position = _gangZones[square.ID],
                    Range = RANGE,
                    AttackingId = (ushort) fracId,
                    ProtectingId = (ushort) square.GangOwner
                };

                ushort warId = World.War.Repository.WarGang(warData, (sbyte) WarGripType.LastSurvivor, 0, 0);
                if (warId == 0) return;

                Trigger.ClientEventForAll("setZoneFlash", warData.ObjectId, true, GangPointsColor[warData.ProtectingId]);


                Manager.SendCoolFractionMsg(square.GangOwner, "Капт", "На вас напали!", LangFunc.GetText(LangType.Ru, DataName.CaptureAlertDefend, Manager.GetName(fracId)), "", 10000);
                Manager.SendCoolFractionMsg(fracId, "Капт", "Нападение", LangFunc.GetText(LangType.Ru, DataName.CaptureAlertAttack), "", 10000);
            }
            catch (Exception e)
            {
                _nLog.Write($"StartCapture Exception: {e}");
            }
        }

        [Command("tpcapture")]
        public static void CMD_tpcapture(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                var characterData = player.GetCharacterData();

                if (sessionData == null || characterData == null)
                    return;

                if (characterData.IsBannedCrime)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вам ограничили доступ к системам capture и bizwar. Причина: {characterData.BanCrimeReason}.", 5000);
                    return;
                }

                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }

                if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }

                if (Main.IHaveDemorgan(player, true) || !player.IsFractionAccess(RankToAccess.CaptureJoin))
                    return;

                var fracId = player.GetFractionId();
                //ищем именно свой капт (либо нападение, либо защита)
                var war = World.War.Repository.Wars.Values.FirstOrDefault(w => 
                    w.Type == WarType.Gangs && 
                    (
                    w.AttackingId == fracId || 
                    w.ProtectingId == fracId
                    )
                );

                if (war == null)
                    return;

                


                List<ItemId> CalibresNew = new List<ItemId> {
                    ItemId.PistolAmmo,
                    ItemId.RiflesAmmo,
                    ItemId.ShotgunsAmmo,
                    ItemId.SMGAmmo,
                    ItemId.SniperAmmo
                };

                ItemId calibre = Calibres[war.WeaponsCategory];
                CalibresNew.Remove(calibre);
                foreach (ItemId item in CalibresNew)
                {
                    if (Chars.Repository.isItem(player, "inventory", item) != null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Выбросите все оружие и патроны, которое не подходит", 3000);
                        return;
                    }
                }

                if (sessionData.UsedTPOnCaptureOrBizwar > 0)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, _getUsedTPErrorMessage(sessionData.UsedTPOnCaptureOrBizwar), 3000);
                    return;
                }

                if (sessionData.WarData.IsWarZone)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.YouAlreadyInClaimRegion), 3000);
                    return;
                }

                _teleportPlayerToWarZone(player, war, fracId, sessionData);
            }
            catch (Exception e)
            {
                _nLog.Write($"CMD_tpcapture Exception: {e}");
            }
        }

        public static List<ItemId> Calibres = new List<ItemId> {
            ItemId.PistolAmmo,
            ItemId.RiflesAmmo,
            ItemId.ShotgunsAmmo,
            ItemId.SMGAmmo,
            ItemId.SniperAmmo
        };

        private static string _getUsedTPErrorMessage(int usedTP)
        {
            return usedTP switch
            {
                1 => LangFunc.GetText(LangType.Ru, DataName.AlreadyTpToClaimRegion),
                2 => LangFunc.GetText(LangType.Ru, DataName.YouWasInClaimRegion),
                _ => string.Empty
            };
        }

        private static void _teleportPlayerToWarZone(ExtPlayer player, World.War.Models.WarData war, int fracId, SessionData sessionData)
        {
            if (fracId != war.ProtectingId)
            {
                player.Position = war.Position;
                sessionData.PositionCaptureOrBizwar = player.GetPosition();
                sessionData.UsedTPOnCaptureOrBizwar = 1;
                Trigger.ClientEvent(player, "freeze", true);
                Timers.StartOnce(60000, () =>
                {
                    Trigger.ClientEvent(player, "freeze", false);
                });
                EventSys.SendCoolMsg(player, "Капт", "Телепорт", LangFunc.GetText(LangType.Ru, DataName.YouWasTpCenterClaimRegion), "", 5000);

            }
            else
            {
                var nearestCaptureCoord = _getNearestCaptureCoord(war.Position);

                if (nearestCaptureCoord != null)
                {
                    player.Position = nearestCaptureCoord;
                    sessionData.PositionCaptureOrBizwar = player.GetPosition();
                    sessionData.UsedTPOnCaptureOrBizwar = 1;
                    World.War.Repository.EntryZone(player, war.Id);
                    Trigger.ClientEvent(player, "freeze", true);
                    Timers.StartOnce(60000, () =>
                    {
                        Trigger.ClientEvent(player, "freeze", false);
                    });
                    EventSys.SendCoolMsg(player, "Капт", "Телепорт", LangFunc.GetText(LangType.Ru, DataName.YouWasTpNearClaimRegion), "", 5000);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.CantFindNearClaimRegion), 3000);
                }
            }
        }

        private static Vector3 _getNearestCaptureCoord(Vector3 warPosition)
        {
            Vector3 nearestCaptureCoord = null;

            foreach (var captureCoord in _tpCaptureCoords)
            {
                if (warPosition.DistanceTo(captureCoord) <= 150)
                    continue;

                if (nearestCaptureCoord == null || warPosition.DistanceTo(captureCoord) < warPosition.DistanceTo(nearestCaptureCoord))
                {
                    nearestCaptureCoord = captureCoord;
                }
            }

            return nearestCaptureCoord;
        }

        public class GangPoint
        {
            public int ID { get; set; }
            public int GangOwner { get; set; }
            public CaptureWarData CaptureWarData { get; set; } = new CaptureWarData();

            public void Save()
            {
                Trigger.SetTask(async () =>
                {
                    try
                    {
                        await using var db = new ServerBD("MainDB");

                        await db.Gangspoints
                            .Where(g => g.Id == ID)
                            .Set(g => g.Gangid, (sbyte)GangOwner)
                            .UpdateAsync();
                    }
                    catch (Exception e)
                    {
                        _nLog.Write($"Gangspoints Save Exception: {e.ToString()}");
                    }

                });
            }
        }

        public class CaptureWarData
        {
            public bool InWar { get; set; } = false;
            public DateTime StartDate { get; set; } = DateTime.Now;
            public int CountMembers { get; set; } = 3;
            public string AttackGangName { get; set; } = string.Empty;
            public int AttackingId { get; set; }
            public string DefendGangName { get; set; } = string.Empty;
            public bool Armor { get; set; } = false;
            public bool Resist { get; set; } = false;
            public ushort CalibreIndex { get; set; } = 0;
            public List<int> SelectedMembers { get; set; } = new List<int>();

            private static List<string> _calibres = new List<string>() {
                "PistolAmmo",
                "RiflesAmmo",
                "ShotgunsAmmo",
                "SMGAmmo",
                "SniperAmmo",
            };

            public CaptureWarData(bool inWar, DateTime startDate, int countMembers, string attackGangName, int attackingId, string defendGangName,
                List<int> selectedMembers, bool armor = false, bool resist = false, ushort calibreIndex = 0)
            {
                InWar = inWar;
                StartDate = startDate;
                CountMembers = countMembers;
                AttackGangName = attackGangName;
                AttackingId = attackingId;
                DefendGangName = defendGangName;
                SelectedMembers = selectedMembers;
                Armor = armor;
                Resist = resist;
                CalibreIndex = calibreIndex;
            }

            public CaptureWarData()
            {

            }

            public ItemId GetAmmoType()
            {
                return Enum.Parse<ItemId>(_calibres[CalibreIndex]);
            }

            public string GetFormattedTime()
            {
                return $"{StartDate.Day}, {StartDate.Month} {StartDate.Hour}:{StartDate.Minute}";
            }
        }

        private class HistoryDTO
        {
            public DateTime DateTime { get; set; }
            public Models.Fractions Attacker { get; set; }
            public Models.Fractions Defenders { get; set; }
            public string NicknameAttacker { get; set; }

            public HistoryDTO(Models.Fractions attacker, Models.Fractions defenders, string nicknameAttacker)
            {
                Attacker = attacker;
                Defenders = defenders;
                NicknameAttacker = nicknameAttacker;

                DateTime = DateTime.Now;
            }

            public string GetFormattedTime()
            {
                return $"{DateTime.ToString("dd.MM HH:mm")}";
            }
        }
    }
}