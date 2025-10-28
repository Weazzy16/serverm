using GTANetworkAPI;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Anticheat
{
    public class HitBox : Script
    {
        private static readonly nLog Log = new nLog("HitBox");
        
        /// <summary>
        /// Минимальный уровень админки для получения уведомлений
        /// </summary>
        private static readonly int _minLvl = 5;

        /// <summary>
        /// Кикать ли игрока, или нет?
        /// </summary>
        private static readonly bool _instantlyKick = false;

        [RemoteEvent("server.anticheat.hitbox")]
        public static void Detected(ExtPlayer player)
        {
            try
            {
                if (player.HasData("hitbox.notifyed")) 
                    return;

                player.SetData("hitbox.notifyed", true);
                Trigger.SendToAdmins(_minLvl, $"Игрок ({player.Value}) зашел на сервер с измененным хитбоксом");

                if (_instantlyKick)
                    player.Kick();
            }
            catch(Exception ex) { Log.Write("Detected: " + ex.ToString()); } 
        }
    }
}
