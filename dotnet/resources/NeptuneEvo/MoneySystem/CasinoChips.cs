using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using Redage.SDK;
using System;
using NeptuneEvo.Character;

namespace NeptuneEvo.MoneySystem
{
    class CasinoChips : Script
    {
        private static readonly nLog Log = new nLog("MoneySystem.CasinoChips");

        public static bool AddChips(ExtPlayer player, int amount)
        {
            try
            {
                if (amount <= 0) return false;
                var characterData = player.GetCharacterData();
                if (characterData == null) return false;
                characterData.CasinoChips += amount;
                Trigger.ClientEvent(player, "client.charStore.CasinoChips", characterData.CasinoChips);
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"AddChips Exception: {e.ToString()}");
                return false;
            }
        }

        public static bool RemoveChips(ExtPlayer player, int amount)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return false;
                if (amount <= 0 || characterData.CasinoChips < amount) return false;
                characterData.CasinoChips -= amount;
                Trigger.ClientEvent(player, "client.charStore.CasinoChips", characterData.CasinoChips);
                return true;
            }
            catch (Exception e)
            {
                Log.Write($"RemoveChips Exception: {e.ToString()}");
                return false;
            }
        }

        public static bool ExchangeMoneyToChips(ExtPlayer player, int amount)
        {
            try
            {
                if (amount <= 0) return false;
                if (!Wallet.Change(player, -amount)) return false;
                return AddChips(player, amount);
            }
            catch (Exception e)
            {
                Log.Write($"ExchangeMoneyToChips Exception: {e.ToString()}");
                return false;
            }
        }

        public static bool ExchangeChipsToMoney(ExtPlayer player, int amount)
        {
            try
            {
                if (amount <= 0) return false;
                if (!RemoveChips(player, amount)) return false;
                return Wallet.Change(player, amount);
            }
            catch (Exception e)
            {
                Log.Write($"ExchangeChipsToMoney Exception: {e.ToString()}");
                return false;
            }
        }
    }
}