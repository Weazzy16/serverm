using NeptuneEvo.Chars.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing.Store.Classes
{
    public class StoreItem
    {
        public string Name { get; set; }
        public ItemId ItemId { get; set; }
        public int Price { get; set; }
        public int Value { get; set; } = 1;

        [JsonIgnore]
        public int MinPrice { get; set; }
        [JsonIgnore]
        public int MaxPrice { get; set; }
        [JsonIgnore]
        public bool IsUpdated { get; set; } = false;
        public bool IsSell { get; set; } = false;
        public bool IsDonate { get; set; } = false;

        public StoreItem(string name, ItemId itemId, int minPrice, int maxPrice, int value = 1, bool isUpdated = false, bool isSell = false, bool isDonate = false)
        {
            Name = name;
            ItemId = itemId;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Value = value;
            IsUpdated = isUpdated;
            IsSell = isSell;
            IsDonate = isDonate;
        }
    }
}
