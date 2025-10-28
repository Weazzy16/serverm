using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.Containers.Enums;
using NeptuneEvo.VehicleData.Models;
using NeptuneEvo.VehicleModel;

namespace NeptuneEvo.EternalDev.Containers.Classes
{
    public class PrizeData
    {
        public string Model { get; set; }
        public int Price { get; set; }
        public string Rarity { get; set; } 
        public bool IsDonate { get; set; }
        public int Chance { get; set; }

        public string Name
        {
            get
            {
                var vName = vMain.GetName(NAPI.Util.GetHashKey(Model));
                if (vName is null) 
                    return Model;   
                return vName;
            }
        }

        public PrizeData(string model, int chance, Rarity rarity = Enums.Rarity.Gray, int price = -1, bool isDonate = false)
        {
            Model = model;
            Chance = chance;
            Rarity = rarity.ToString().ToLower();
            Price = price;
            IsDonate = isDonate;

            if (Price == -1)
            {
                if (BusinessManager.BusProductsData.TryGetValue(Model, out var busProductData))
                    Price = busProductData.Price;
                else
                    Price = 0;
            }
        }
    }
}
