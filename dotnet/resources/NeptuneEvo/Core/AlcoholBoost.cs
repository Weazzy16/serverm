using NeptuneEvo.Chars.Models;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Core
{
    public class AlcoholBoost
    {
        /// <summary>
        /// Список со всеми значениями для каждого типа алкаголя
        /// 
        /// 0 - Нисколько
        /// 100 - Бессмертный
        /// </summary>
        public static readonly Dictionary<ItemId, int> DAMAGE_BOOSTERS = new Dictionary<ItemId, int>()
        {
            { ItemId.Spank, 55 },
            { ItemId.RusDrink1, 55 },
            { ItemId.RusDrink2, 55 },
            { ItemId.RusDrink3, 55 },
            { ItemId.YakDrink2, 55 },
            { ItemId.LcnDrink1, 55 },
            { ItemId.LcnDrink2, 55 },
            { ItemId.LcnDrink3, 55 },
            { ItemId.ArmDrink1, 55 },
            { ItemId.ArmDrink2, 55 },
            { ItemId.ArmDrink3, 55 }
        };

        public static void Set(ExtPlayer player, ItemId itemId)
        {
            if (!DAMAGE_BOOSTERS.TryGetValue(itemId, out var boosterValue)) return;
            player.SetSharedData("alcohol.boost", boosterValue);
        }

        public static void Reset(ExtPlayer player)
        {
            player.SetSharedData("alcohol.boost", 0);
        }
    }
}
