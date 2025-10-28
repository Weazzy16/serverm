using Localization;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace NeptuneEvo.Events.Christmas.Shop.Classes
{
    public class ExChangeItem
    {
        private static readonly nLog Log = new nLog(nameof(ExChangeItem));
        public string Name { get; set; }
        public ItemId ItemId { get; set; }
        public int Count { get; set; }
          
        [JsonIgnore]

        public Dictionary<ItemId, int> NeedItems = new Dictionary<ItemId, int>();

        public Dictionary<string, int> NeedItemsUI = new Dictionary<string, int>();

        public ExChangeItem(ItemId itemId, int count, Dictionary<ItemId, int> needItems)
        {
            ItemId = itemId;
            Count = count;
            NeedItems = needItems;

            Name = Chars.Repository.ItemsInfo[ItemId].Name;
            NeedItemsUI = NeedItems.ToDictionary(x => Chars.Repository.ItemsInfo[x.Key].Name, x => x.Value);
        }

        public void Buy(ExtPlayer player)
        {
            try
            {
                if (!IsHaveNeedItems(player))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас недостаточно предметов для обмена!", 3000);
                    return;
                }

                if (Chars.Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", ItemId, Count) == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    return;
                }

                RemoveItems(player);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы получили {Chars.Repository.ItemsInfo[ItemId].Name} x{Count}!", 3000);
            }
            catch(Exception ex) { Log.Write("Buy: " + ex.ToString()); }
        }

        public bool IsHaveNeedItems(ExtPlayer player)
        {
            foreach(var needItem in NeedItems)
            {
                var count = Chars.Repository.getCountItem($"char_{player.CharacterData.UUID}", needItem.Key);
                if (count < needItem.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveItems(ExtPlayer player)
        {
            foreach (var needItem in NeedItems)
            {
                Chars.Repository.Remove(player, $"char_{player.CharacterData.UUID}", "inventory", needItem.Key, needItem.Value);
            }
        }
    }
}
