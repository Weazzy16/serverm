using GTANetworkAPI;
using NeptuneEvo.Events.Christmas.Shop;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Events.Christmas.ExChanger
{
    public class ExChangerController : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnStart()
            => ExChangerManager.OnStart();

        [RemoteEvent("server.christmas.exChanger.action")]
        public void OnAction(ExtPlayer player, int index)
            => ExChangerManager.OnAction(player, index);

        [Interaction(ColShapeEnums.ChristmasEvent_ExChanger)]
        public void OnInteraction(ExtPlayer player, int index)
            => ExChangerManager.OnInteraction(player, index);
    }
}
