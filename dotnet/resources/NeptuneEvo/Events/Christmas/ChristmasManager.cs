using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Events.Christmas.Player;
using NeptuneEvo.Events.Christmas.Data;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Events.Christmas
{
    public class ChristmasManager
    {
        private static readonly nLog Log = new nLog(nameof(ChristmasManager));

        public static bool CanTakeCandy(ExtPlayer player, int houseId)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData is null) return false;

                if (sessionData.ChristmasEvent is null) 
                    sessionData.ChristmasEvent = new ChristmasEventPlayerData();

                if (sessionData.ChristmasEvent.PassedHouses.Contains(houseId))
                    return false;

                var randomCandy = GetRandomCountCandy();
                GiveCandy(player, randomCandy);

                sessionData.ChristmasEvent.PassedHouses.Add(houseId);
                return true;
            }
            catch(Exception ex) { Log.Write("CanTakeCandy: " + ex.ToString()); return false; }
        }

        public static int GetRandomCountCandy() 
            => Main.rnd.Next(Config.MIN_AMOUNT_OF_CANDY_DISPENSED, Config.MAX_AMOUNT_OF_CANDY_DISPENSED);

        public static bool GiveCandy(ExtPlayer player, int count)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return false;

                if (count <= 0)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Вам ничего не выпало!", 3000);
                    return false;
                }

                if (Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Candy, count) == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    return false;
                }

                return true;
            }
            catch(Exception ex) { Log.Write("GiveCandy: " + ex.ToString()); return false; }
        }
    }
}