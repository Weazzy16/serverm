using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.MarketPlace.Auction;
using NeptuneEvo.Handles;
using Redage.SDK;
using System.Linq;

namespace NeptuneEvo.EternalDev.MarketPlace.Extensions
{
    public static class Vehicles
    {
        public static bool CanBuyProperty(this VehicleData.Models.VehicleData vehicleData, ExtPlayer player = null)
        {
            return !IsOnMarketplace(vehicleData, player) && !IsInProfileStorage(vehicleData, player);
        }

        public static bool IsInProfileStorage(this VehicleData.Models.VehicleData vehicleData, ExtPlayer player = null)
        {
            var userProfileWithProperty = Manager.UserProfiles.Values.FirstOrDefault(x => x.Storage.Find(x => x.Type == Enums.LotType.House && x.Data == vehicleData.SqlId.ToString()) != null);
            if (userProfileWithProperty != null)
            {
                if (player != null)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот бизнес хранится на складе маркетплейса у игрока #{userProfileWithProperty.UUID}", 3000);
                return true;
            }

            return false;
        }

        public static bool IsOnMarketplace(this VehicleData.Models.VehicleData vehicleData, ExtPlayer player = null)
        {
            bool Reject(string message)
            {
                if (player != null)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, message, 3000);
                return true;
            }

            var marketItem = Manager.MarketItems.Values.ToList().Find(x => x.Type == Enums.LotType.Vehicle && x.Data == vehicleData.SqlId.ToString());
            if (marketItem != null)
                return Reject($"Этот транспорт выставлен на маркетплейсе, взаимодействие с ним невозможно");

            var auctionItem = AuctionManager.AuctionItems.Values.ToList().Find(x => x.Type == Enums.LotType.Vehicle && x.Data == vehicleData.SqlId.ToString());
            if (auctionItem != null)
                return Reject($"Этот транспорт выставлен на аукционе, взаимодействие с ним невозможно");

            return false;
        }

        public static bool IsOnMarketplace(this Vehicle entity, ExtPlayer player = null)
        {
            var vehicle = entity as ExtVehicle;
            if (vehicle.VehicleLocalData.Access != VehicleData.LocalData.Models.VehicleAccess.Personal)
                return false;

            var vehicleData = VehicleManager.GetVehicleToNumber(vehicle.NumberPlate);
            return vehicleData.IsOnMarketplace(player);
        }
    }
}
