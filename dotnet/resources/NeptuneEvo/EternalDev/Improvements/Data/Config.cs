using NeptuneEvo.Chars.Models;
using NeptuneEvo.EternalDev.Improvements.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.Improvements.Data
{
    public class Config
    {
        public static readonly Dictionary<ItemId, ImprovementsData> IMPROVEMENTS_DATA = new Dictionary<ItemId, ImprovementsData>()
        {
            { ItemId.Medovuha, new ImprovementsData(damageMultiplayer: 20, timeEffect: 15, extraHealing: 30) },
        };

        /// <summary>
        /// Кулдаун на использование баффа после последнего попадания в игрока
        /// </summary>
        public static readonly int COOLDOWN_TO_USE_AFTER_GET_DAMAGE = 10;
    }
}
