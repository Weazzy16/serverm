using NeptuneEvo.Fractions.Crafting.Classes;
using NeptuneEvo.Fractions.Crafting.Data;
using NeptuneEvo.Handles;
using NeptuneEvo.Organizations.Player;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Fractions.Crafting
{
    public class CraftingManager
    {
        private static readonly nLog Log = new nLog(nameof(CraftingManager));

        public static void OnStart()
        {
            Config.CRAFTING_TABLES.ForEach(x => x.GTAElements());
        }

        public static void OnInteraction(ExtPlayer player, int fractionId, bool isOrg = false)
        {
            try
            {
                if (isOrg)
                {
                    CraftingTable.Interaction(player, fractionId);
                }
                else
                {
                    var craftingTable = Config.CRAFTING_TABLES.Find(x => (int)x.FractionId == fractionId);
                    if (craftingTable is null) return;

                    craftingTable.Interaction(player);
                }
            }
            catch(Exception ex) { Log.Write("OnInteraction: " + ex.ToString()); }
        }

        public static void Craft(ExtPlayer player, int index)
        {
            try
            {
                var colShapeData = player.ColShapesData.Find(x => x.ColShapeId == Functions.ColShapeEnums.FractionCrafting);
                if (colShapeData is null)
                {
                    colShapeData = player.ColShapesData.Find(x => x.ColShapeId == Functions.ColShapeEnums.Organizations && x.Index == 1);
                    if (colShapeData is null) return;

                    var organizationData = player.GetOrganizationData();
                    if (organizationData is null) return;

                    CraftingTable.Craft(player, index, organizationData.Id);
                    return;
                }

                var craftingTable = Config.CRAFTING_TABLES.Find(x => (int)x.FractionId == colShapeData.Index);
                if (craftingTable is null) return;

                craftingTable.Craft(player, index);
            }
            catch(Exception ex) { Log.Write("Craft: " + ex.ToString()); }
        }
    }
}
