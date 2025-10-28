using NeptuneEvo.Chars;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Configs
{
    public class PriceSettings
    {
        [JsonProperty("cost")]
        public int Cost { get; set; }

        [JsonProperty("isDonate")]
        public bool IsDonate { get; set; }

        [JsonProperty("depends_vehicle")]
        public bool DependsVehicle { get; set; }

        public PriceSettings(int cost, bool isDonate, bool dependsVehicle)
        {
            Cost = cost;
            IsDonate = isDonate;
            DependsVehicle = dependsVehicle;
        }

        public bool CanPay(ExtPlayer player, int vehiclePrice, bool isRemove, string message)
        {
            var amount = Convert.ToInt32(Math.Truncate(Cost + (DependsVehicle ? vehiclePrice * Controller.Config.VehiclePriceMultiplayer : 1)) * (isRemove ? Controller.Config.RemoveHandlingPriceMultiplayer : 1));

            if (IsDonate ? player.AccountData.RedBucks < amount : player.CharacterData.Money < amount)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас недостаточно средств", 3000);
                return false;
            }

            if (IsDonate)
                UpdateData.RedBucks(player, -amount, message);
            else
                MoneySystem.Wallet.Change(player, -amount);
            return true;
        }
    }
}
