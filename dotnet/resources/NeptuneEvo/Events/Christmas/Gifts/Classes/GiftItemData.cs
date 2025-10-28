using Localization;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Events.Christmas.Gifts.Enums;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NeptuneEvo.Events.Christmas.Gifts.Classes
{
    public class GiftItemData
    {
        private static readonly nLog Log = new nLog(nameof(GiftItemData));

        public GiftItemType Type { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public int Chance { get; set; }

        public GiftItemData(GiftItemType type, string name, string data, int chance)
        {
            Type = type;
            Name = name;
            Data = data;
            Chance = chance;
        }

        public void Give(ExtPlayer player)
        {
            try
            {
                string[] split = Data.Split("@");

                Trigger.ClientEvent(player, "client.christmas.gift.open", Type.ToString(), Name, Data);

                switch(Type)
                {
                    case GiftItemType.Item:
                        if (split.Length < 2) return;

                        ItemId itemId = (ItemId)Enum.Parse(typeof(ItemId), split[0]);
                        int count = Convert.ToInt32(split[1]);
                        string data = "";

                        if (split.Length > 2)
                            data = split[2];

                        if (Chars.Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", itemId, count, data, addInWarehouse: true) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventoryThenSclad), 3000);
                            return;
                        }
                        return;
                    case GiftItemType.Weapon:
                        if (split.Length < 1) return;
                        itemId = (ItemId)Enum.Parse(typeof(ItemId), split[0]);

                        if (Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", itemId, 1, "christmas-event", addInWarehouse: true) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventoryThenSclad), 3000);
                            return;
                        }
                        return;
                    case GiftItemType.Money:
                        if (!Int32.TryParse(Data, out var money))
                            return;

                        MoneySystem.Wallet.Change(player, money);
                        return;
                    case GiftItemType.Donate:
                        if (!Int32.TryParse(Data, out var donate))
                            return;

                        UpdateData.RedBucks(player, donate, "Новогодний ивент!");
                        return;
                    case GiftItemType.Vehicle:
                        string modelName = split[0];

                        if (Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", ItemId.CarCoupon, 1, modelName, addInWarehouse: true) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventoryThenSclad), 3000);
                            return;
                        }
                        return;
                    case GiftItemType.Clothes:
                        if (split.Length < 1) return;
                        itemId = (ItemId)Enum.Parse(typeof(ItemId), split[0]);

                        if (Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", itemId, 1, split[1], addInWarehouse: true) == -1)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventoryThenSclad), 3000);
                            return;
                        }
                        return;
                }
            }
            catch(Exception ex) { Log.Write("Give: " + ex.ToString()); }
        }
    }
}
