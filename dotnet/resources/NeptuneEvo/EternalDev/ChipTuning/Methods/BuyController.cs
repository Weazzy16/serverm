using EternalCore;
using Google.Protobuf.WellKnownTypes;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Methods
{
    public class BuyController : Script
    {
        [RemoteEvent("e-dev.chipTuning.buy_controller")]
        public static void On(ExtPlayer player, string controllerName, bool isRemove)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                var vehicle = player.Vehicle as ExtVehicle;
                if (vehicle is null) return;

                var vehicleData = VehicleManager.GetVehicleToNumber(vehicle.NumberPlate);
                if (vehicleData is null) return;

                var vehiclePrice = VehiclePrice.Get(vehicleData.Model);

                var chipData = Controller.GetChipVehicleData(vehicleData.SqlId);
                if (chipData is null)
                    chipData = new Classes.ChipVehicleData()
                    {
                        VehicleId = vehicleData.SqlId,
                    };

                var product = Controller.Config.ControllerSettings.FirstOrDefault(x => x.Type == controllerName);
                if (product is null)
                    return;

                if (isRemove && !chipData.Controllers.ContainsKey(controllerName))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас не установлена данная модификация", 3000);
                    return;
                }

                if (!product.Price.CanPay(player, vehiclePrice, isRemove, "Покупка чип-тюнинга"))
                    return;

                if (chipData.Controllers.ContainsKey(controllerName))
                    chipData.Controllers[controllerName] = !isRemove;
                else
                    chipData.Controllers.TryAdd(controllerName, !isRemove);

                Controller.SaveOrCreate(chipData);
                Controller.ApplyVehicleData(vehicle, vehicleData.SqlId);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно установили {product.Name} на свое авто!", 3000);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(On), ex, nameof(BuyController)); }
        }
    }
}
