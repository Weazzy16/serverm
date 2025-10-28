using NeptuneEvo.Chars.Models;
//using NeptuneEvo.Core.Crafting.Classes;
using NeptuneEvo.Events.Christmas.Data;
using NeptuneEvo.Events.Christmas.Gifts.Classes;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Events.Christmas.Gifts
{
    public class GiftsManager
    {
        private static readonly nLog Log = new nLog(nameof(GiftsManager));

        public static void Open(ExtPlayer player, InventoryItemData item)
        {
            try
            {
                if (!Config.GIFTS_DATA.TryGetValue(item.ItemId, out var giftData)) return;

                var giftItem = GetRandomItem(giftData);
                if (giftItem is null) return;

                giftItem.Give(player);
                Chars.Repository.RemoveIndex(player, "inventory", item.Index, 1);
            }
            catch(Exception ex) { Log.Write("Open: " + ex.ToString()); }
        }

        private static GiftItemData GetRandomItem(List<GiftItemData> list)
        {
            try
            {
                GiftItemData giftItem = null;
                int pool = 0;

                list.ForEach(item => pool += item.Chance);

                int random = new Random().Next(0, pool) + 1;
                int accumulatedProbability = 0;

                foreach(var item in list)
                {
                    accumulatedProbability += item.Chance;
                    if (random <= accumulatedProbability)
                        giftItem = item;
                }

                return giftItem;
            }
            catch (Exception ex) { Log.Write("GetRandomItem: " + ex.ToString()); return null; }
        }
    }
}
