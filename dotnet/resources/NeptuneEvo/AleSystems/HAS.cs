using System;
using GTANetworkAPI;
using Redage.SDK;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;


namespace NeptuneEvo.AleSystems
{
    class HAS : Script
    {
        private static readonly nLog Log = new nLog(nameof(HAS));

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Log.Write("HPsystem is loaded ");
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }

        }
        [RemoteEvent("cefReady")]
public void OnCefReady(ExtPlayer player)
        {
            HAS.LoadHPARData(player);
        }

        public static void LoadHPARData(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            sessionData.TimersData.HealthTimer = Timers.Start(1, () => HealthTimer(player));
            sessionData.TimersData.ArmorTimer = Timers.Start(1, () => ArmorTimer(player));
            
            var hp = player.Health;
            var ar = player.Armor;
            Trigger.ClientEvent(player, "start:HPAR::client", hp, ar);
        }
        public static void HealthTimer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                NAPI.Task.Run(() =>
                {
                    var hp = player.Health;

                    Trigger.ClientEvent(player, "update:HP::client", hp);
                }, 1);
            }
            catch (Exception e)
            {
                Log.Write($"HealthTimer Exception: {e.ToString()}");
            }
        }
        public static void ArmorTimer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                NAPI.Task.Run(() => {
                    var ar = player.Armor;

                    Trigger.ClientEvent(player, "update:AR::client", ar);
                }, 1);
            }
            catch (Exception e)
            {
                Log.Write($"ArmorTimer Exception: {e.ToString()}");
            }
        }
    }
}
