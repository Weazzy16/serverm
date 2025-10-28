using GTANetworkAPI;
using NeptuneEvo.Fractions.Crafting.Data;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Player;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.Fractions.Crafting.Classes
{
    public class CraftingTable
    {
        private static readonly nLog Log = new nLog(nameof(CraftingTable));

        public Vector3 Position { get; set; }
        public Models.Fractions FractionId { get; set; }
        public List<CraftItem> Items { get; set; }

        public CraftingTable(Vector3 position, Models.Fractions fractionId, List<CraftItem> items)
        {
            Position = position;
            FractionId = fractionId;
            Items = items;
        }

        private ExtColShape _colShape { get; set; }
        private Marker _marker { get; set; }
        private TextLabel _table {  get; set; }

        public void GTAElements()
        {
            _colShape = CustomColShape.CreateCylinderColShape(Position, 1, 1, 0, ColShapeEnums.FractionCrafting, (int)FractionId);
            _marker = NAPI.Marker.CreateMarker(1, Position - new Vector3(0, 0, 1.12), new Vector3(), new Vector3(), 0.9f, new Color(255, 255, 255, 150), false, 0);
            _table = NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Крафт"), Position + new Vector3(0, 0, 0.2), 5F, 0.5F, 0, new Color(255, 255, 255));
        }

        public static void Interaction(ExtPlayer player, int organizationId)
        {
            try
            {
                var organizationData = Organizations.Manager.GetOrganizationData(organizationId);
                if (organizationData is null || player.GetOrganizationData() != organizationData) return;

                var items = new List<CraftItem>();
                foreach(var item in Config.ORGANIZATION_CRAFTING_ITEMS)
                {
                    if (!organizationData.Schemes.TryGetValue(item.Key, out var isHave) || !isHave) continue;
                    items.Add(item.Value);
                }

                Trigger.ClientEvent(player, "client.fractions.crafting.open", organizationData.Name, JsonConvert.SerializeObject(items));
            }
            catch(Exception ex) { Log.Write("Interaction: " + ex.ToString()); }
        }

        public void Interaction(ExtPlayer player)
        {
            try
            {
                var fractionData = player.GetFractionData();
                if (fractionData is null || fractionData.Id != (int)FractionId) return;

                Trigger.ClientEvent(player, "client.fractions.crafting.open", fractionData.Name, JsonConvert.SerializeObject(Items));
            }
            catch(Exception ex) { Log.Write("Interaction: " + ex.ToString()); }
        }

        public static void Craft(ExtPlayer player, int index, int organizationId)
        {
            try
            {
                var organizationData = Organizations.Manager.GetOrganizationData(organizationId);
                if (organizationData is null || player.GetOrganizationData() != organizationData) return;

                var items = new List<CraftItem>();
                foreach (var item in Config.ORGANIZATION_CRAFTING_ITEMS)
                {
                    if (!organizationData.Schemes.TryGetValue(item.Key, out var isHave) || !isHave) continue;
                    items.Add(item.Value);
                }

                var craftItem = items.ElementAt(index);
                if (craftItem is null) return;

                craftItem.Craft(player, organizationId);
            }
            catch(Exception ex) { Log.Write("Craft: " + ex.ToString()); }
        }

        public void Craft(ExtPlayer player, int index)
        {
            try
            {
                var fractionData = player.GetFractionData();
                if (fractionData is null || fractionData.Id != (int)FractionId) return;

                var crafItem = Items.ElementAt(index);
                if (crafItem is null) return;

                crafItem.Craft(player);
            }
            catch(Exception ex) { Log.Write("Craft: " + ex.ToString()); }
        }
    }
}
