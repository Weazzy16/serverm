using NeptuneEvo.Events.Christmas.Shop.Classes;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Handles;
using System.Linq;
using NeptuneEvo.Functions;
using Microsoft.VisualBasic;

namespace NeptuneEvo.Events.Christmas.Shop
{
    public class ExChangerManager
    {
        private static readonly nLog Log = new nLog(nameof(ExChangerManager));
        public static List<ExChangerPlace> Places = new List<ExChangerPlace>();

        public static void OnStart()
        {
            try
            {
                new ExChangerPlace("Новогодний ивент", new Vector3(462.46387, -782.9903, 27.357916), 140, NAPI.Util.GetHashKey("g_m_m_chigoon_01"), new List<ExChangeItem>()
                {
                    new ExChangeItem(ItemId.ChristmasTreeDecoration, 1, new Dictionary<ItemId, int>()
                    {
                        { ItemId.Candy, 10 }
                    }),
                    new ExChangeItem(ItemId.ChristmasTreeStar, 1, new Dictionary<ItemId, int>()
                    {
                        { ItemId.ChristmasTreeDecoration, 5 },
                        { ItemId.Candy, 5 },
                    }),
                    new ExChangeItem(ItemId.SmallGift, 1, new Dictionary<ItemId, int>()
                    {
                        { ItemId.ChristmasTreeStar, 2 },
                    }),
                    new ExChangeItem(ItemId.MediumGift, 1, new Dictionary<ItemId, int>()
                    {
                        { ItemId.ChristmasTreeStar, 4 },
                    }),
                    new ExChangeItem(ItemId.BigGift, 1, new Dictionary<ItemId, int>()
                    {
                        { ItemId.ChristmasTreeStar, 6 },
                    }),
                });

                NAPI.Blip.CreateBlip(304, new Vector3(462.46387, -782.9903, 27.357916), 1f, 5, "Новогодний ивент", 255, 0, true, 0, 0);
            }
            catch(Exception ex) { Log.Write("OnStart: " + ex.ToString()); }
        }

        public static void OnAction(ExtPlayer player, int index)
        {
            try
            {
                var interaction = player.ColShapesData.Find(x => x.ColShapeId == ColShapeEnums.ChristmasEvent_ExChanger);
                if (interaction is null) return;

                var place = Places.ElementAt(interaction.Index);
                if (place is null) return;

                place.Action(player, index);
            }
            catch(Exception ex) { Log.Write("OnAction: " + ex.ToString()); }
        }

        public static void OnInteraction(ExtPlayer player, int index)
        {
            try
            {
                var place = Places.ElementAt(index);
                if (place is null) return;

                place.Interaction(player);
            }
            catch(Exception ex) { Log.Write("OnInteraction: " + ex.ToString()); }
        }
    }
}
