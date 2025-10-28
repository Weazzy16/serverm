using GTANetworkAPI;
using NeptuneEvo.Players;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;

namespace NeptuneEvo.EternalDev.Containers
{
    public class Controller : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnStart()
            => Manager.OnStart();

        [ServerEvent(Event.PlayerConnected)]
        public void OnConnected(Player player)
            => Manager.OnConnected(player as ExtPlayer);

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnDisconnected(Player player, DisconnectionType disconnectionType, string reason)
          => Manager.OnDisconnected(player as ExtPlayer);

        [Interaction(ColShapeEnums.Container)]
        public void OnInteraction(ExtPlayer player, int containerId)
        {
            if (!Manager.Containers.TryGetValue(containerId, out var container))
                return;

            container.OnInteraction(player);
        }

        [RemoteEvent("server.containers.menu.close")]
        public void OnClose(ExtPlayer player)
        {
            var session = player.GetSessionData();
            if (session is null || session.CurrentContainer is null) return;

            session.CurrentContainer.UnSubscribe(player);
        }

        [RemoteEvent("server.containers.menu.place_bet")]
        public void OnPlaceBet(ExtPlayer player, int bet, string paymentType)
        {
            var session = player.GetSessionData();
            if (session is null || session.CurrentContainer is null) return;

            session.CurrentContainer.PlaceBet(player, bet, paymentType);
        }

        [RemoteEvent("server.containers.menu.prize_action")]
        public void OnPrizeAction(ExtPlayer player, string action)
        {
            var session = player.GetSessionData();
            if (session is null || session.CurrentContainer is null) return;

            switch (action)
            {
                case "sell":
                    session.CurrentContainer.SellPrize(player);
                    break;
                case "take":
                    session.CurrentContainer.TakePrize(player);
                    break;
            }
        }
    }
}
