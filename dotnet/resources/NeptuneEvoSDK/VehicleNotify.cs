using GTANetworkAPI;

namespace Redage.SDK
{
    /// <summary>
    /// Уведомления для транспортных средств (показываются под картой)
    /// </summary>
    public static class VehicleNotify
    {
        /// <summary>
        /// Отправить уведомление о транспорте (показывается под картой)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="time"></param>
        public static void Send(Player player, NotifyType type, string msg, int time = 3000)
        {
            Trigger.ClientEvent(player, "vehicleNotify", type, msg, time);
        }
    }
}