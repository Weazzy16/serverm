using Localization;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Models;
using NeptuneEvo.Organizations.Player;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Fractions.Crafting.Classes
{
    public class CraftItem
    {
        private static readonly nLog Log = new nLog(nameof(CraftItem));

        public ItemId ItemId { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Count { get; set; }
        public string Data { get; set; }

        public CraftItem(ItemId itemId, int cost, int count = 1, string data = "")
        {
            ItemId = itemId;
            Name = Chars.Repository.ItemsInfo[itemId].Name;
            Cost = cost;
            Count = count;
            Data = data;
        }

        public void Craft(ExtPlayer player, int organizationId = -1)
        {
            try
            {
                FractionData fractionData = null;
                OrganizationData organizationData = null;
                
                if (organizationId == -1)
                {
                    fractionData = player.GetFractionData();
                    if (fractionData is null) return;

                    if (fractionData.Materials < Cost)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно материалов на складе фракции!", 3000);
                        return;
                    }
                }
                else
                {
                    organizationData = player.GetOrganizationData();
                    if (organizationData is null) return;

                    if (organizationData.Materials < Cost)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно материалов на складе организации!", 3000);
                        return;
                    }
                }

                string data = Data;
                if (Chars.Repository.ItemsInfo[ItemId].functionType == newItemType.Weapons)
                {                                                                                                  
                    data = WeaponRepository.GetSerial(true, organizationId == -1 ? fractionData.Id : organizationData.Id);
                }

                if (Chars.Repository.AddNewItem(player, $"char_{player.GetUUID()}", "inventory", ItemId, Count, data) == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    return;
                }

                if (organizationId == -1)
                {
                    fractionData.Materials -= Cost;
                    GameLog.Stock(fractionData.Id, player.GetUUID(), player.Name, $"{ItemId}({data})", Count, "out");
                }
                else
                {
                    organizationData.Materials -= Cost;
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы скрафтили {Chars.Repository.ItemsInfo[ItemId].Name}", 3000);
            }
            catch(Exception ex) { Log.Write("Craft: " + ex.ToString()); }
        }
    }
}
