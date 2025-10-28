using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Methods
{
    public class Leave : Script
    {
        [RemoteEvent("e-dev.chipTuning.close")]
        public void OnEvent(ExtPlayer player)
            => On(player, player.Vehicle as ExtVehicle);

        [ServerEvent(Event.PlayerExitVehicle)]
        public void PlayerExitVehicle(ExtPlayer player, ExtVehicle vehicle)
            => On(player, vehicle);

        [ServerEvent(Event.PlayerDeath)]
        public void PlayerDeath(ExtPlayer player, ExtPlayer killer, uint reason)
            => On(player, player.Vehicle as ExtVehicle);

        public void On(ExtPlayer player, ExtVehicle vehicle)
        {
            try
            {
                if (!player.HasData("chipTuning"))
                    return;

                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                if (vehicle == null)
                    vehicle = player.Vehicle as ExtVehicle;

                if (vehicle != null)
                    NAPI.Entity.SetEntityPosition(vehicle, characterData.ExteriorPos);
                player.Position = characterData.ExteriorPos;

                if (vehicle != null)
                    Trigger.Dimension(vehicle, 0);
                player.Dimension = 0;

                if (vehicle != null)
                {
                    player.SetIntoVehicle(vehicle, 0);
                    VehicleManager.ApplyCustomization(vehicle as ExtVehicle);
                }

                characterData.ExteriorPos = new Vector3();
                player.ResetData("chipTuning");
                Trigger.ClientEvent(player, "e-dev.chipTuning.close");
            }
            catch(Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
