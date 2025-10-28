using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players.Popup.List.Models;
using NeptuneEvo.Players;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static NeptuneEvo.Fractions.Hijacking.Position;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Fractions.Models;

namespace NeptuneEvo.Fractions.Hijacking
{
    public class Main : Script
    {
        private static readonly nLog Log = new nLog("Hijacking");

        public static ExtVehicle _veh;
        public static ExtColShape _cols;
        public static HijackingPosition activePositions;
        public static Random rnd = new Random();
        public static Vector3 FinishHijacking = new Vector3(1540.4772, 6336.3423, 22.955172);
        public static string NumberVeh = null;
        public static Color _colormarker = new Color(163, 131, 188);
        public static Blip _blip;
        public static VehecleHij SelectedVeh;
        public static string TimerTask;

        public static List<HijackingPosition> PositionsVeh = new List<HijackingPosition>()
        {
            new HijackingPosition(new Vector3(286.05154, 150.3633, 104.24346), new Vector3(-0.7785859, 0.010205129, -110.00212)),
            new HijackingPosition(new Vector3(330.34653, 163.18925, 103.52409), new Vector3(1.3986064, 0.05890426, 70.33218)),
            new HijackingPosition(new Vector3(232.6546, 116.872345, 102.778946), new Vector3(0.28896704, -0.08269749, -20.097967)),
            new HijackingPosition(new Vector3(220.87799, 174.18686, 105.44652), new Vector3(-0.670204, -0.24243982, -114.55817)),
            new HijackingPosition(new Vector3(91.448, 183.54625, 104.74335), new Vector3(2.581168, -0.81041247, 160.24596)),
        };

        public static List<VehecleHij> vehecleHijs = new List<VehecleHij>()
        {
            new VehecleHij("Panto", 0, "Черный"),
            new VehecleHij("Ingot", 31, "Красный"),
            new VehecleHij("Regina", 112, "Белая"),
            new VehecleHij("Schafter", 31, "Красный"),
            new VehecleHij("focusrs", 158, "Золотой"),
            new VehecleHij("Tailgater", 0, "Черный"),
            new VehecleHij("Glendale", 112, "Белая"),
            new VehecleHij("Stafford", 0, "Черный"),
            new VehecleHij("Akuma", 31, "Красный"),
            new VehecleHij("Sanchez", 0, "Черный"),
            new VehecleHij("Bati", 0, "Черный"),
            new VehecleHij("Bati2", 0, "Черный"),
            new VehecleHij("Cavalcade", 0, "Черный"),
            new VehecleHij("Faggio", 112, "Белая"),
            new VehecleHij("Dubsta", 0, "Черный"),
            new VehecleHij("Serrano", 112, "Белая"),
            new VehecleHij("Granger", 0, "Черный"),
            new VehecleHij("Rocoto", 112, "Белая"),
            //new VehecleHij("Tailgater", 0, "Черный"),
            new VehecleHij("t20", 44, "Красный"),
            new VehecleHij("weevil", 55, "Зеленый"),
        };


        public static void StartWorkDay(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            var fracId = player.GetFractionId();
            if (Manager.FractionTypes[fracId] != FractionsType.Gangs && Manager.FractionTypes[fracId] != FractionsType.Mafia)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Задания доступны только для банд и мафий!", 3000);
                return;
            }
        }


            public static void StartTask(ExtPlayer player)
        {
            if (player.HasData("HijTask"))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже взяли задание на угон тс", 3000);
                return;
            }
            NumberVeh = VehicleManager.GenerateNumber(VehicleAccess.Hijacking, "");
            SelectedVeh = vehecleHijs[rnd.Next(0, vehecleHijs.Count)];
            activePositions = PositionsVeh.Where(position => position.IsActive).ToList()[0];
            activePositions.SetActive(false);
            _veh = VehicleStreaming.CreateVehicle(NAPI.Util.GetHashKey(SelectedVeh.Name), activePositions.Position, activePositions.Rotation.Z, SelectedVeh.Color, SelectedVeh.Color, NumberVeh, acc: VehicleAccess.Hijacking, petrol: 9999, locked: false, engine: true);
            Trigger.ClientEvent(player, "BlipsHijacking", true, activePositions.Position + new Vector3(rnd.Next(150, 200), rnd.Next(150, 200), activePositions.Position.Z));
            _veh.SetData("Hijacking", true);
            player.SetData("HijTask", true);
            NAPI.Data.SetEntityData(player, "VEHICLE_ONEMSTIMER", 0);
            NAPI.Data.SetEntityData(player, "VEHICLE_ONEMS", Timers.StartTask(1000, () => timer_playerStartHikacking(player)));
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы взяли задание на угон тс", 3000);
            player.SendChatMessage("Сейчас тебе придется угнать авто марки " + $"{SelectedVeh.Name}" + " ~w~с номером - !{#9898e6}" + $"{NumberVeh}.~w~ Цвет - {SelectedVeh.ColorName}.");
        }

        private static void timer_playerStartHikacking(ExtPlayer player)
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    if (!player.HasData("VEHICLE_ONEMS")) return;
                    if (NAPI.Data.GetEntityData(player, "VEHICLE_ONEMSTIMER") > 1800)
                    {
                        if (player.HasData("HijTask"))
                        {
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы не успели сдать авто вовремя.", 3000);
                            if (_veh != null) _veh.Delete();
                            if (activePositions != null)
                            {
                                activePositions.SetActive(true);
                                activePositions = null;
                            }
                            if (_blip != null) _blip.Delete();
                            NumberVeh = null;
                            Trigger.ClientEvent(player, "BlipsHijacking", false, new Vector3());
                            player.ResetData("HijTask");
                            player.ResetData("VEHICLE_ONEMSTIMER");
                            player.ResetData("VEHICLE_ONEMS");
                            return;
                        }
                    }
                    NAPI.Data.SetEntityData(player, "VEHICLE_ONEMSTIMER", NAPI.Data.GetEntityData(player, "VEHICLE_ONEMSTIMER") + 1);
                }
                catch (Exception e) { Log.Write("exitVehicleTimer: " + e.ToString()); }
            });
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void Event_OnPlayerDisconnected(ExtPlayer player, DisconnectionType type, string reason)
        {
            try
            {
                if (player.HasData("HijTask"))
                {
                    NAPI.Task.Run(() =>
                    {
                        if (_veh != null) _veh.Delete();
                        if (activePositions != null)
                        {
                            activePositions.SetActive(true);
                            activePositions = null;
                        }
                        if(_blip != null) _blip.Delete();
                        player.ResetData("HijTask");
                        Trigger.ClientEvent(player, "BlipsHijacking", false, new Vector3());
                    });
                }
            }
            catch (Exception e) { Log.Write("PlayerDisconnected: " + e.Message, nLog.Type.Error); }
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public static void OnPlayerEnterVehicleHandler(ExtPlayer player, ExtVehicle vehicle, sbyte seatid)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null)
                    return;

                if (vehicle.HasData("Hijacking"))
                {
                    _cols = CustomColShape.CreateCylinderColShape(FinishHijacking, 7f, 3f, 0, ColShapeEnums.HijackingFinish);
                    Trigger.ClientEvent(player, "createCheckpoint", NumberVeh, 1, FinishHijacking, 5, _colormarker.Red, _colormarker.Green, _colormarker.Blue);
                    Trigger.ClientEvent(player, "createWaypoint", FinishHijacking.X, FinishHijacking.Y);
                    Trigger.ClientEvent(player, "BlipsHijacking", false, new Vector3());
                    foreach (var p in NAPI.Pools.GetAllPlayers().Cast<ExtPlayer>())
                    {
                        var charactersData = p.GetCharacterData();
                        if (charactersData == null)
                            return;
                        var memberFractionData = player.GetFractionMemberData();
                        if (memberFractionData == null)
                            return;

                        var fractionData = Fractions.Manager.GetFractionData(memberFractionData.Id);
                        if (fractionData == null)
                            return;


                        var fracId = fractionData.Id;
                        if (fracId == 7)
                        {
                            p.SendChatMessage($"~g~[Крыса] Только что было угнано авто - {_veh.DisplayName}. С регистрационным номером - {NumberVeh}. Цвет: {SelectedVeh.ColorName}");
                            _blip = NAPI.Blip.CreateBlip(468, player.Vehicle.Position, 1, 38, "Викрадення авто", 255, 0, true, 0, 0);
                        }

                    }
                }
                else return;
            }
            catch (Exception e) { Log.Write("PlayerEnterVehicle: " + e.Message, nLog.Type.Error); }
        }

        /*private static void timer_setHijBlipPolice()
        {
            if(_blip != null)
            {
                NAPI.Task.Run(() => {
                    TimerTask = Timers.StartTask(1000, () =>
                    {
                        if (_veh.Position != _blip.Position)
                        {
                            _blip.Position = _veh.Position;
                            Trigger.ClientEvent(player, "createWorkBlip", NumberVeh, 1, FinishHijacking, 5, _colormarker.Red, _colormarker.Green, _colormarker.Blue);
                        }
                        else
                        {
                            DestroyTimer();
                        }
                    });
                    TreeObject.SetSharedData("SAWMILL_OBJECT", true);
                    Destroy = false;
                });
            }
        }*/

        [Interaction(ColShapeEnums.HijackingFinish, In: true)]
        public static void OnHijackingFinish(ExtPlayer player)
        {
            try
            {
                if (player.HasData("HijTask"))
                {
                    var sessionData = player.GetSessionData();
                    if (sessionData == null) return;

                    var characterData = player.GetCharacterData();
                    if (characterData == null)
                        return;

                    if (!player.IsInVehicle) return;
                    player.Vehicle.Delete();
                    var count = 100000 * rnd.Next(0, 12) + rnd.Next(100, 400); 
                    MoneySystem.Wallet.Change(player, +count);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно отвезли авто на точку и почили {count}$.", 3000);
                    VehicleManager.WarpPlayerOutOfVehicle(player);
                    player.ResetData("HijTask");
                    NumberVeh = null;
                    if (activePositions != null)
                    {
                        activePositions.SetActive(true);
                        activePositions = null;
                    }
                    if (_cols != null) _cols.Delete();
                    if (_blip != null) _blip.Delete();
                }
            }
            catch (Exception e)
            {
                Log.Write($"openGoPostalStart Exception: {e.ToString()}");
            }
        }
    }
}
