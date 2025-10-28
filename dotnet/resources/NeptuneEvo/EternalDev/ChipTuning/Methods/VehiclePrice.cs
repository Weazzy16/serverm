using NeptuneEvo.Core;
using NeptuneEvo.VehicleData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Methods
{
    public class VehiclePrice
    {
        public static int Get(string model)
        {
            if (!BusinessManager.BusProductsData.TryGetValue(model, out var product))
            {
                return 0;
            }

            return product.Price == 0 ? product.OtherPrice : product.Price;
        }
    }
}
