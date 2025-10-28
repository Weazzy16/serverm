using EternalCore;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.Businesses.GasStation.Configs;
using NeptuneEvo.Handles;
using NeptuneEvo.VehicleData.LocalData;
using NeptuneEvo.VehicleModel;
using Redage.SDK;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace NeptuneEvo.EternalDev.Businesses.GasStation
{
    public class Controller : Script
    {
        #region Author @merumond
        // ----------------------------------------------------------------------------------- //
        // |                                                                                 | //
        // |     ███╗░░░███╗███████╗██████╗░██╗░░░██╗███╗░░░███╗░█████╗░███╗░░██╗██████╗░    | //
        // |     ████╗░████║██╔════╝██╔══██╗██║░░░██║████╗░████║██╔══██╗████╗░██║██╔══██╗    | //
        // |     ██╔████╔██║█████╗░░██████╔╝██║░░░██║██╔████╔██║██║░░██║██╔██╗██║██║░░██║    | //
        // |     ██║╚██╔╝██║██╔══╝░░██╔══██╗██║░░░██║██║╚██╔╝██║██║░░██║██║╚████║██║░░██║    | //
        // |     ██║░╚═╝░██║███████╗██║░░██║╚██████╔╝██║░╚═╝░██║╚█████╔╝██║░╚███║██████╔╝    | //
        // |     ╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚═╝░╚═════╝░╚═╝░░░░░╚═╝░╚════╝░╚═╝░░╚══╝╚═════╝░    | //
        // |                                                                                 | //
        // ----------------------------------------------------------------------------------- //
        #endregion
        
        public class Events
        {
            public const string BUY_PETROL = "server.gas-station.buy_petrol";
            public const string BUY_PRODUCT = "server.gas-station.buy_product";

            public const string OPEN = "client.gas-station.open";
            public const string CLOSE = "client.gas-station.close";
        }

        public static GasStationsConfig Config = ELib.JsonReader.Read<GasStationsConfig>(GasStationsConfig.Path);

        public static void Interaction(ExtPlayer player, Business business)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                var vehicle = player.Vehicle as ExtVehicle;
                if (vehicle != null)
                {
                    var vehicleLocalData = vehicle.GetVehicleLocalData();
                    if (vehicleLocalData is null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете заправить этот транспорт", 3000);
                        return;
                    }
                }

                var data = new
                {
                    Station = new {
                        Id = business.ID,
                        Street = "",
                        Company = Config.StationTypes.ContainsKey(business.ID) ? Config.StationTypes[business.ID].ToString() : Config.DefaultType.ToString(),
                    },

                    Products = business.Products.Where(p => Config.Products.Find(x => x.Name == p.Name) != null)
                        .Select(p =>
                        {
                            var config = Config.Products.Find(x => x.Name == p.Name);
                            return new
                            {
                                ItemId = config.ItemId,
                                Name = config.Name,
                                Price = p.Price,
                                Weight = 1000,
                                Stock = p.Lefts
                            };
                        }
                    ),

                    FuelTypes = business.Products.Where(p => Config.PetrolTypes.Values.FirstOrDefault(x => x.Name == p.Name) != null)
                        .Select(p =>
                        {
                            var config = Config.PetrolTypes.Values.FirstOrDefault(x => x.Name == p.Name);
                            return new
                            {
                                Name = config.Name,
                                Price = p.Price,
                                Active = p.Lefts > 0
                            };
                        }
                    ),

                    CarData = vehicle is null ? null : new
                    {
                        CurrentFuel = vehicle.VehicleLocalData.Petrol,
                        MaxFuel = vehicle.VehicleLocalData.MaxPetrol,
                        Model = vehicle.Model,
                        Name = vMain.GetName(vehicle.Model),
                        FuelSystem = 100,
                        RecommendedFuel = VehiclePetrol.Controller.GetValidPetrol(vehicle.Model).ToString()
                    }
                };

                Trigger.ClientEvent(player, Events.OPEN, JsonConvert.SerializeObject(data));
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(Interaction), ex, nameof(Controller)); }
        }

        public static void Close(ExtPlayer player)
            => Trigger.ClientEvent(player, Events.CLOSE);

        [RemoteEvent(Events.BUY_PETROL)]
        public static void OnBuyPetrol(ExtPlayer player, string productName, int count, string paymentType)
            => Methods.BuyPetrol.On(player, productName, count, paymentType);

        [RemoteEvent(Events.BUY_PRODUCT)]
        public static void OnBuyProduct(ExtPlayer player, string productName, string paymentType)
            => Methods.BuyProduct.On(player, productName, paymentType);
    }
}
