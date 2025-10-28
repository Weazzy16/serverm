using MySqlConnector;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using System.Linq;
using NeptuneEvo.Core;
using NeptuneEvo.Accounts;
using GTANetworkMethods;
using Localization;
using NeptuneEvo.Functions;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.Fractions;

namespace NeptuneEvo.SystemByAle
{
    class LSCsystem : Script
    {
        #region args
        private static readonly nLog Log = new nLog("LSCbyAle");
        private const int moneyforfix = 500000;
        private static Vector3 VehicleRepairPosition = new Vector3(-360.20795, -123.28825, 38.69);
        private static Vector3 VehicleRepairPosition2 = new Vector3(-196.0042, -1305.5405, 31.364605);
        private static Vector3 VehicleRepairPosition3 = new Vector3(-1132.6757, -1999.3463, 13.171342);
        private static Vector3 VehicleRepairPosition4 = new Vector3(1175.0941, 2649.1772, 37.80895);
        private static Vector3 VehicleRepairPosition5 = new Vector3(120.39673, 6617.917, 31.847197);
        private static Vector3 VehicleRepairPosition6 = new Vector3(656.176, 680.2027, 128.91093);
        #endregion

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {

                CustomColShape.CreateCylinderColShape(VehicleRepairPosition, 3, 5, 0, ColShapeEnums.FIXCARLSC, 9); 
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition.X, VehicleRepairPosition.Y, VehicleRepairPosition.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт транспорта"), VehicleRepairPosition, 5F, 0.3F, 0, new Color(255, 255, 255));
                //2
                CustomColShape.CreateCylinderColShape(VehicleRepairPosition2, 3, 5, 0, ColShapeEnums.FIXCARLSC, 9);
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition2.X, VehicleRepairPosition2.Y, VehicleRepairPosition2.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт транспорта"), VehicleRepairPosition2, 5F, 0.3F, 0, new Color(255, 255, 255));
                //3
                CustomColShape.CreateCylinderColShape(VehicleRepairPosition3, 3, 5, 0, ColShapeEnums.FIXCARLSC, 9);
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition3.X, VehicleRepairPosition3.Y, VehicleRepairPosition3.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт транспорта"), VehicleRepairPosition3, 5F, 0.3F, 0, new Color(255, 255, 255));
                //4
                CustomColShape.CreateCylinderColShape(VehicleRepairPosition4, 3, 5, 0, ColShapeEnums.FIXCARLSC, 9);
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition4.X, VehicleRepairPosition4.Y, VehicleRepairPosition4.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт транспорта"), VehicleRepairPosition4, 5F, 0.3F, 0, new Color(255, 255, 255));
                //5
                CustomColShape.CreateCylinderColShape(VehicleRepairPosition5, 3, 5, 0, ColShapeEnums.FIXCARLSC, 9);
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition5.X, VehicleRepairPosition5.Y, VehicleRepairPosition5.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт транспорта"), VehicleRepairPosition5, 5F, 0.3F, 0, new Color(255, 255, 255));
                //6
                CustomColShape.CreateCylinderColShape(VehicleRepairPosition6, 3, 5, 0, ColShapeEnums.FIXCARLSC, 9);
                NAPI.Marker.CreateMarker(1, new Vector3(VehicleRepairPosition6.X, VehicleRepairPosition6.Y, VehicleRepairPosition6.Z - 1.75), new Vector3(), new Vector3(), 3f, new Color(255, 255, 255, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("~w~Ремонт транспорта"), VehicleRepairPosition6, 5F, 0.3F, 0, new Color(255, 255, 255));

            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }





        [Interaction(ColShapeEnums.FIXCARLSC)]
        public static void FixCarPay(ExtPlayer player, int interact)
        {
            try
            {
                var character = player.GetCharacterData();
                if (character == null) return;
                if (!player.IsInVehicle)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }
                if (character.Money < moneyforfix)
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.PlayerNotEnoughMoney), 3000);
                    return;
                }
                var vehicle = (ExtVehicle)player.Vehicle;
                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData != null)
                {
                    MoneySystem.Wallet.Change(player, -moneyforfix);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"- {moneyforfix}$", 3000);
                    Trigger.ClientEvent(player, "freeze", true);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Начат процесс починки транспорта, ожидайте", 5000);
                    NAPI.Task.Run(() =>
                    {
                        Trigger.ClientEvent(player, "freeze", false);
                        VehicleManager.RepairCar(vehicle);
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно починили машину", 3000);
                        var fractionData = Manager.GetFractionData((int)Fractions.Models.Fractions.CITY);
                        fractionData.Money += moneyforfix;
                    }, 5500);
                }
                else
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustInCar), 3000);
                    return;
                }
                return;
            }
            catch (Exception e)
            {
                Log.Write($"FixCarPay Exception: {e.ToString()}");
            }
        }
    }
}


