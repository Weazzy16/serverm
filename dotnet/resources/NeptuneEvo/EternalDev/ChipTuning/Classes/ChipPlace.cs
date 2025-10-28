using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.VehicleData.LocalData;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Classes
{
    public class ChipPlace
    {
        public int Id { get; set; }
        public ExtPosition Position { get; set; }
        
        public ChipPlace(int id, ExtPosition position)
        {
            Id = id;
            Position = position;
        }

        public void GTAElements()
        {
            NAPI.Blip.CreateBlip(Controller.Config.BlipSprite, Position.GetVector3(), 1f, Controller.Config.BlipColor, "Чип-тюнинг", 255, 0, true, 0, 0);

            CustomColShape.CreateCylinderColShape(Position.GetVector3(), 4f, 4f, 0, ColShapeEnums.ChipTuning, Id);
            NAPI.Marker.CreateMarker(MarkerType.VerticalCylinder, Position.GetVector3(), new Vector3(), new Vector3(), 3f, new Color(77, 255, 212), false, 0);
        }

        public void Interaction(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData is null) return;

            var vehicle = player.Vehicle as ExtVehicle;
            if (vehicle is null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находится в авто!", 3000);
                return;
            }

            var vehicleData = VehicleManager.GetVehicleToNumber(vehicle.NumberPlate);
            if (vehicleData is null || !VehicleManager.canAccessByNumber(player, vehicleData.Number))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Это не ваше авто!", 3000);
                return;
            }

            var chipData = Controller.GetChipVehicleData(vehicleData.SqlId);
            if (chipData is null)
                chipData = new ChipVehicleData();

            uint dimension = Dimensions.RequestPrivateDimension(player.Value);
            Trigger.Dimension(player, dimension);
            Trigger.Dimension(vehicle, dimension);

            player.SetIntoVehicle(vehicle, 0);
            player.SetData("chipTuning", this);

            characterData.ExteriorPos = Position.GetVector3() + new Vector3(0, 0, 1.12);

            int vehiclePrice = Methods.VehiclePrice.Get(vehicleData.Model);
            Trigger.ClientEvent(player, "e-dev.chipTuning.open", 
                JsonConvert.SerializeObject(Controller.Config.HandlingSettings.Select(x => x.HandlingParam)), 
                JsonConvert.SerializeObject(chipData.Handlings), 
                JsonConvert.SerializeObject(chipData.Controllers), 
                vehiclePrice
            );
        }
    }
}
