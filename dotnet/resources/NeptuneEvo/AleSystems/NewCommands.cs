using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions;
using Redage.SDK;
using System;
using NeptuneEvo.Core;
using GTANetworkMethods;
using Database;
using LinqToDB;
using NeptuneEvo.Character.Models;

namespace NeptuneEvo.AleSystems
{
    class NewCommands : Script
    {
        private static readonly nLog Log = new nLog(nameof(NewCommands));

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Log.Write("Команды загружены " + "\u001B[33m" + "[by Rasn56]" + "\u001B[0m");
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static byte chide = 0;

        [Command("logger")]
        public static void CMD_logger(ExtPlayer player, bool state)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.AdminLVL != 9) return;

                Trigger.ClientEvent(player, "toggleShowCallRemote", state);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_logger Exception: {e.ToString()}");
            }
        }
        [Command("coinsreset")]
        public static void CMD_coinsReset(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            if (characterData.AdminLVL != 9) return;

            Trigger.SetTask(async () =>
            {
                try
                {
                    await using var db = new ServerBD("MainDB");

                    await db.Characters
                    .Set(v => v.ReceivedCoins, 0)
                    .Set(v => v.CoinsTime, 18000)
                    .UpdateAsync();

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно сбросили таймер коинов для всех.", 3000);
                }
                catch (Exception e)
                {
                    Log.Write($"CMD_coinsReset Exception: {e.ToString()}");
                }
            });
        }
        [Command("createped")]
        public static void CMD_createped(ExtPlayer player, string model, bool inv, bool freeze, bool locked, uint dim)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                if (characterData.AdminLVL != 9) return;

                var ped = NAPI.Ped.CreatePed(NAPI.Util.GetHashKey(model), player.Position, player.Heading, invincible: inv, frozen: freeze, controlLocked: locked, dimension: dim);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_createped Exception: {e.ToString()}");
            }
        }
        

      

        [Command("chide")]
        public static void CMD_chide(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.AdminLVL < 9)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно прав.", 3000);
                    return;
                }

                if (chide == 0)
                {
                    player.SetSharedData("INVISIBLE", true);
                    player.SetSharedData("HideNick", true);
                    player.SetSharedData("REDNAME", false);
                    player.SetSharedData("UUID", -1);
                    player.SetSharedData("isDrone", true);
                    player.SetSharedData("VoiceDist", -1);
                    Trigger.ClientEvent(player, "chideOn");
                    var poigdr = new Vector3(99999f, 9999f, 9999f);
                    player.Position = poigdr;
                    NAPI.Entity.SetEntityPosition(player, poigdr);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Теперь игроки и читеры вас не видят.", 3000);
                    chide = 1;
                }
                else
                {
                    player.SetSharedData("INVISIBLE", false);
                    player.SetSharedData("HideNick", false);
                    player.SetSharedData("REDNAME", true);
                    player.SetSharedData("InDeath", false);
                    player.SetSharedData("UUID", characterData.UUID);
                    player.SetSharedData("isDrone", false);
                    player.ResetSharedData("VoiceDist");
                    Trigger.ClientEvent(player, "chideOff");
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Теперь вас все видят.", 3000);
                    chide = 0;
                }

                NAPI.Util.ConsoleOutput($"[CHIDE] Status toggled: {chide}");
            }
            catch (Exception e)
            {
                NAPI.Util.ConsoleOutput($"CHIDE Exception: {e}");
            }
        

    }
    [RemoteEvent("client:dobiv")]
        public static void ClientDobiv(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (sessionData.DeathData.IsDying) Ems.ReviveFunc(player);
            }
            catch (Exception e)
            {
                Log.Write($"ClientDobiv Exception: {e.ToString()}");
            }
        }
        [Command("skinmenu")]
        public static void CMD_skinmenu(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                Trigger.ClientEvent(player, "client.skins.open");
            }
            catch (Exception e)
            {
                Log.Write($"CMD_skinmenu Exception: {e.ToString()}");
            }
        }
        
        
    }
}