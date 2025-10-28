using GTANetworkAPI;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Fractions.Crafting
{
    public class CraftingController : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnStart() 
            => CraftingManager.OnStart();

        [Interaction(ColShapeEnums.FractionCrafting)]
        public void OnInteraction(ExtPlayer player, int fractionId)
            => CraftingManager.OnInteraction(player, fractionId);

        [RemoteEvent("server.fractions.crafting.do")]
        public void Event_Craft(ExtPlayer player, int index)
            => CraftingManager.Craft(player, index);
    }
}
