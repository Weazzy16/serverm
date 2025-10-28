using NeptuneEvo.Chars.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing.Classes
{
    public class FishItemData
    {
        public ItemId ItemId { get; set; }
        public int MinLvl { get; set; }
        public int Chance { get; set; }
        public int MinPrice { get; set; }   
        public int MaxPrice { get; set; }
        public bool IsDonate { get; set; }
        public List<ItemId> Rods { get; set; }

        public FishItemData(ItemId itemId, int minLvl, int chance, int minPrice, int maxPrice, List<ItemId> rods = null, bool isDonate = false)
        {
            ItemId = itemId;
            MinLvl = minLvl;
            Chance = chance;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Rods = rods is null ? new List<ItemId>() { ItemId.Rod, ItemId.RodUpgraded, ItemId.RodMk2 } : rods;
            IsDonate = isDonate;
        }
    }
}
