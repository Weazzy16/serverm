using GTANetworkAPI;
using System;
using Redage.SDK;
using NeptuneEvo.Core;
using NeptuneEvo.Character;
using Database;
using NeptuneEvo;
using NeptuneEvo.Handles;
using static NeptuneEvo.Core.BusinessManager;
using System.Collections.Generic;
using NeptuneEvo.VehicleData.Models;
using MySql.Data.MySqlClient;
using NeptuneEvo.Players;
using NeptuneEvo.Functions;
using Localization;
using NeptuneEvo.Accounts;
using GTANetworkMethods;
using System.Linq.Expressions;
using Org.BouncyCastle.Bcpg;
using NeptuneEvo.MoneySystem;
using System.Linq;

namespace NeptuneEvo.Houses
{
    class Svalka : Script
    {
        //private static Vector3 Svalki[i] = new Vector3(-363.91403, -92.45042, 39.015663);
        private static nLog Log = new nLog("Свалка");

        public static List<Vector3> Svalki = new List<Vector3>
        {
        //new Vector3(-363.91403, -92.45042, 39.015663),
        new Vector3(-462.2022, -1715.0991, 18.643421),
        //new Vector3(-363.91403, -92.45042, 39.015663),
        //new Vector3(105.3244, 3697.4966, 17.569134),
        };

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                for (int i = 0; i < Svalki.Count; i++)
                {
                    NAPI.Marker.CreateMarker(1, Svalki[i] - new Vector3(0, 0, 0.7), new Vector3(), new Vector3(), 3, new Color(255, 0, 0));
                    NAPI.Blip.CreateBlip(163, Svalki[i], 0.9f, 1, "Свалка", 255, 0, true, 0, 0);
                    CustomColShape.CreateCylinderColShape(Svalki[i], 3, 3, 0, ColShapeEnums.Svalka);
                    NAPI.TextLabel.CreateTextLabel("~r~Сдача транспорта на свалку", Svalki[i] + new Vector3(0, 0, 1.5), 5F, 0.3F, 0, new Color(255, 255, 255));
                }
            }
            catch (Exception e)
            {
                Log.Write("ResourceStart: " + e.Message, nLog.Type.Error);
            }
        }

        [Interaction(ColShapeEnums.Svalka)]
        private void SellCar(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null)
                return;

            var accountData = player.GetAccountData();
            if (accountData == null)
                return;

            var characterData = player.GetCharacterData();
            if (characterData == null)
                return;

            if (!player.IsInVehicle)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не в транспорте.", 3000);
                return;
            }

            var vehicle = (ExtVehicle)player.Vehicle;
            var vehicleData = VehicleManager.GetVehicleToNumber(vehicle.NumberPlate);
            var pl = characterData.FirstName + "_" + characterData.LastName;
            var holder = VehicleManager.Vehicles[player.Vehicle.NumberPlate].Holder;
            if (pl != holder)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Это не ваш автомобиль", 3000);
                return;
            }
            if (characterData.FirstName == null || characterData.LastName == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот транспорт нельзя сдать на свалку.", 3000);
                return;
            }
            if (vehicleData == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот транспорт нельзя сдать на свалку.", 3000);
                return;
            }
            if (holder == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот транспорт нельзя сдать на свалку.", 3000);
                return;
            }
            if (pl == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот транспорт нельзя сдать на свалку.", 3000);
                return;
            }

            if (vehicleData == null) return;

            if (vehicleData.Holder != sessionData.Name) return;

            sessionData.CarSellGov = vehicle.NumberPlate;

            int price = 0;
            if (BusinessManager.BusProductsData.ContainsKey(vehicleData.Model))
            {
                switch (accountData.VipLvl)
                {
                    case 0: // None
                    case 1: // Bronze
                    case 2: // Silver
                    case 3: // Gold
                        price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.6);
                        break;
                    case 4: // Platinum
                    case 5: // Media Platinum
                        price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.7);
                        break;
                    default:
                        price = Convert.ToInt32(BusinessManager.BusProductsData[vehicleData.Model].Price * 0.5);
                        break;
                }
            }
            Trigger.ClientEvent(player, "openDialog", "SvalkaSell", $"Вы хотите сдать ваш транспорт на свалку? Вы получите ${price}");
            return;
        }
    }
}