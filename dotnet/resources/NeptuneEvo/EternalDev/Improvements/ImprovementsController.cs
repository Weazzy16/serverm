using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;
using System;

namespace NeptuneEvo.EternalDev.Improvements
{
    public class ImprovementsController : Script
    {
        private static readonly nLog Log = new nLog(nameof(ImprovementsController));

        [ServerEvent(Event.PlayerDamage)]
        public static void OnPlayerDamage(ExtPlayer player, float healthLoss, float armorLoss)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null || !player.IsCharacterData()) return;

                session.LastDamage = DateTime.Now;
            }
            catch (Exception ex) { Log.Write("OnPlayerDamage: " + ex.ToString()); }
        }
    }
}
