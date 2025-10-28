using GTANetworkAPI;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.MarketPlace.Enums;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.MarketPlace.Classes
{
    public class MarketItem
    {
        public int Id { get; set; }
        public LotType Type { get; set; }
        public long Cost { get; set; }

        public string Data { get; set; }
        public int Owner { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<int> Views { get; set; } = new List<int>();
        public List<int> Favourites { get; set; } = new List<int>();

        public List<string> Photos { get; set; }
        public string Comment { get; set; }

        public bool IsSave { get; set; } = false;


        public void Save()
        {
            IsSave = true;
        }

        public void Delete()
        {
            if (!Manager.GetUserProfile(Owner, out var profile))
                return;

            var addToStorage = false;
            switch (Type)
            {
                case LotType.Item:
                case LotType.Clothes:
                    var parsedData = Data.Split("@@");
                    Chars.Repository.AddNewItem(null, $"marketStorage_{Owner}", "marketStorage", (ItemId)Convert.ToInt32(parsedData[0]), Convert.ToInt32(parsedData[1]), parsedData[2], MaxSlots: Manager.Config.MaxSlotsInStorage);
                    break;

                case LotType.Vehicle:
                    var vehicleData = VehicleManager.GetVehicleToAutoId(Convert.ToInt32(Data));
                    if (vehicleData is null) break;

                    if (vehicleData.Holder == Main.PlayerNames[Owner])
                        break;

                    addToStorage = true;
                    break;
                case LotType.House:
                    var house = HouseManager.Houses.Find(x => x.ID == Convert.ToInt32(Data));
                    if (house is null) break;

                    if (house.Owner == Main.PlayerNames[Owner])
                        break;

                    addToStorage = true;
                    break;
                case LotType.Business:
                    if (!BusinessManager.BizList.TryGetValue(Convert.ToInt32(Data), out var business))
                        break;

                    if (business.Owner == Main.PlayerNames[Owner])
                        break;

                    addToStorage = true;
                    break;
            }

            if (addToStorage)
            {
                profile.AddItem(new StorageItem()
                {
                    Type = Type,
                    Data = Data,
                });
            }

            var player = Main.GetPlayerByUUID(Owner);
            NAPI.Task.Run(() =>
            {   
                if (player is null)
                    return;

                Manager.UpdateStorage(player);
            }, 1000);

            Manager.DeleteLot(this);
            Manager.UpdatePage(Type.ToString().ToLower());
        }
    }
}
