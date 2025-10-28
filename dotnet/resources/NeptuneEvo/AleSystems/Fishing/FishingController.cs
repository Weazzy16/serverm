using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing
{
    public class FishingController : Script
    {
        [RemoteEvent("server.fishing.minigame.end")]
        public void Event_MinigameEnd(ExtPlayer player, bool state) 
            => FishingManager.EndMinigame(player, state);

        [RemoteEvent("server.fishing.store.action")]
        public void Event_StoreAction(ExtPlayer player, string category, int index, int value)
            => Store.StorePlace.OnAction(player, category, index, value);

        [ServerEvent(Event.PlayerDeath)]
        public void OnDeath(ExtPlayer player, ExtPlayer killer, uint weapon) 
            => FishingManager.Reset(player);
    }
}
