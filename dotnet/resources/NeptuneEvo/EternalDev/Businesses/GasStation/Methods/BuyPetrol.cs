using EternalCore;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Table.Tasks.Models;
using NeptuneEvo.Table.Tasks.Player;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleData.LocalData.Models;
using Redage.SDK;
using System;
using System.Linq;

namespace NeptuneEvo.EternalDev.Businesses.GasStation.Methods
{
    public class BuyPetrol
    {
        private class Events 
        {
            public const string FILL_PETROL = "client.gas-station.fill_petrol";
        }

        public static void On(ExtPlayer player, string productName, int count, string paymentType)
        {
            try
            {
                var characterData = player.GetCharacterData();
                var sessionData = player.GetSessionData();
                if (characterData is null || sessionData is null || count <= 0)
                    return;

                var vehicle = player.Vehicle as ExtVehicle;
                if (vehicle is null)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы должны находится в транспорте", 3000);
                    return;
                }

                var petrolData = Controller.Config.PetrolTypes.FirstOrDefault(x => x.Value.Name == productName);
                if (petrolData.Value is null)
                    return;

                if (!VehiclePetrol.Controller.CanRefuel(vehicle.Model, petrolData.Key))
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Данный тип топлива не подходит для вашего авто", 3000);
                    return;
                }

                if (!BusinessManager.BizList.TryGetValue(sessionData.BizID, out var business))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находится на заправочной станции", 3000);
                    return;
                }

                var productData = business.Products.Find(x => x.Name == productName);
                if (productData is null) return;

                var vehicleLocalData = vehicle.GetVehicleLocalData();
                if (vehicleLocalData is null || vehicleLocalData.Petrol <= -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете заправить этот транспорт", 3000);
                    return;
                }

                if (vehicleLocalData.Petrol + count > vehicleLocalData.MaxPetrol)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете заправить такое количество топлива", 3000);
                    return;
                }

                int totalCost = productData.Price * count;
                if ((paymentType == "Wallet" && characterData.Money < totalCost)
                    || (paymentType == "Bank" && MoneySystem.Bank.GetBalance(characterData.Bank) < totalCost))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств!", 3000);
                    return;
                }

                if (!BusinessManager.takeProd(business.ID, count, productName, totalCost))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На запровочной станции нет такого количества топлива!", 3000);
                    return;
                }

                business.BuyItemBusiness(player.CharacterData.UUID, productName, totalCost);

                GameLog.Money($"player({player.CharacterData.UUID})", $"biz({business.ID})", totalCost, "buyPetrol");

                if (paymentType == "Wallet")
                    MoneySystem.Wallet.Change(player, -totalCost);
                else
                    MoneySystem.Bank.Change(characterData.Bank, -totalCost);

                Players.Phone.Messages.Repository.AddSystemMessage(player, (int)DefaultNumber.Bank, LangFunc.GetText(LangType.Ru, DataName.ZapravkaSuccess, totalCost), DateTime.Now);

                if (vehicleLocalData.Access == VehicleAccess.Fraction)
                    player.AddTableScore(TableTaskId.Item1);

                BattlePass.Repository.UpdateReward(player, 40);
                if (count >= 20)
                    BattlePass.Repository.UpdateReward(player, 19);

                VehicleStreaming.SetEngineState(vehicle, false);

                vehicleLocalData.Petrol += count;
                vehicle.SetSharedData("PETROL", vehicleLocalData.Petrol);

                if (vehicleLocalData.Access == VehicleAccess.Personal)
                {
                    var vehicleData = VehicleManager.GetVehicleToNumber(vehicle.NumberPlate);
                    if (vehicleData != null)
                        vehicleData.Fuel = vehicleLocalData.Petrol;
                }

                Commands.RPChat("sme", player, $"заправил" + (player.CharacterData.Gender ? "" : "а") + LangFunc.GetText(LangType.Ru, DataName.veh));
                Trigger.ClientEvent(player, Events.FILL_PETROL, count);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(On), ex, nameof(Controller)); }
        }
    }
}
