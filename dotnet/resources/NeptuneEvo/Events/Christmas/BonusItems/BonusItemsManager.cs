using NeptuneEvo.Events.Christmas.BonusItems.Classes;
using NeptuneEvo.Events.Christmas.Data;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.Events.Christmas.BonusItems
{
    public class BonusItemsManager
    {
        private static readonly nLog Log = new nLog(nameof(BonusItemsManager));
        public static List<BonusItem> Pool = new List<BonusItem>();

        public static void OnEveryMinute()
        {
            try
            {
                var aliveCount = Pool.Where(x => x.IsAlive).Count();
                int count = Config.MAX_COUNT_OF_BONUS_ITEMS_IN_WORLD - aliveCount;
                if (count <= 0) return;
                var disabledItems = Pool.Where(x => !x.IsAlive && x.Cooldown < DateTime.Now).ToList();
                for (int i = 0; i < count; i++)
                {
                    if (disabledItems.Count == 0) break; 
                    var index = Main.rnd.Next(0, disabledItems.Count);
                    var bonusItem = disabledItems[index];
                    bonusItem.Spawn();

                    disabledItems.RemoveAt(index); 
                }
            }
            catch (Exception ex)
            {
                Log.Write("OnEveryMinute: " + ex.ToString());
            }
        }

        [Interaction(ColShapeEnums.ChristmasEvent_BonusItem)]
        public static void OnInteraction(ExtPlayer player, int index)
        {
            try
            {
                if (index < 0 || index >= Pool.Count) // Проверка валидности индекса
                {
                    Log.Write($"OnInteraction: Invalid index {index}");
                    return;
                }
                var bonusItem = Pool.ElementAt(index);
                if (!bonusItem.IsAlive) return;
                int countOfCandy = GetRandomCountCandy();
                ChristmasManager.GiveCandy(player, countOfCandy);
                bonusItem.Destroy();
            }
            catch (Exception ex)
            {
                Log.Write("OnInteraction: " + ex.ToString());
            }
        }
        public static int GetRandomCountCandy()
            => Main.rnd.Next(Config.MIN_AMOUNT_OF_CANDY_DISPENSED_IN_BONUSITEMS, Config.MAX_AMOUNT_OF_CANDY_DISPENSED_IN_BONUSITEMS);
    }
}
