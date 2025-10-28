using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Fractions;
using Redage.SDK;
using System;
using System.Collections.Generic;
using Localization;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Core;

namespace NeptuneEvo.AleSystems
{
    class Emergency : Script
    {
        private static readonly nLog Log = new nLog(nameof(Emergency));

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Log.Write("Система кодов загружена " + "\u001B[33m" + "[by Ale]" + "\u001B[0m");
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }

        public static Dictionary<ExtPlayer, Blip> MarkCodes = new Dictionary<ExtPlayer, Blip>();//Todo

        private static readonly Random Random = new Random();
        private static readonly byte[] Colors = { 1, 40, 38, 25 };
        private static byte GetRandomColor()
        {
            return Colors[Random.Next(Colors.Length)];
        }

        public static DateTime NextCode = DateTime.MinValue;
        public static bool IsGoska(int fracid)
        {
            switch (fracid)
            {
                case (int)Fractions.Models.Fractions.POLICE:
                case (int)Fractions.Models.Fractions.ARMY:
                case (int)Fractions.Models.Fractions.CITY:
                case (int)Fractions.Models.Fractions.EMS:
                case (int)Fractions.Models.Fractions.FIB:
                case (int)Fractions.Models.Fractions.SHERIFF:
                    return true;
                default:
                    return false;
            }
        }
        public static void MarkCode(ExtPlayer player, string text)
        {
            try
            {

            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var MemberFractionData = player.GetFractionMemberData();
            if (MemberFractionData == null) return;

            var fractionData = Manager.GetFractionData(MemberFractionData.Id);
            if (fractionData == null) return;

            if (!IsGoska(MemberFractionData.Id)) return;

            if (sessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                return;
            }
            if (sessionData.DeathData.InDeath)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                return;
            }
             if (text.Length > 30) return;

            var id = MemberFractionData.Rank;
            var rank = fractionData.Ranks[id].Name;
            var Fname = characterData.FirstName;
            var Lname = characterData.LastName;

            Manager.sendFractionMessage((int)Fractions.Models.Fractions.POLICE, "!{#286187}[F] " + $"[{rank}] {Fname} {Lname} сообщил о происшествии: {text}", true);
            Manager.sendFractionMessage((int)Fractions.Models.Fractions.SHERIFF, "!{#286187}[F] " + $"[{rank}] {Fname} {Lname} сообщил о происшествии: {text}", true);
            Manager.sendFractionMessage((int)Fractions.Models.Fractions.ARMY, "!{#286187}[F] " + $"[{rank}] {Fname} {Lname} сообщил о происшествии: {text}", true);
            Manager.sendFractionMessage((int)Fractions.Models.Fractions.FIB, "!{#286187}[F] " + $"[{rank}] {Fname} {Lname} сообщил о происшествии: {text}", true);
            Manager.sendFractionMessage((int)Fractions.Models.Fractions.EMS, "!{#286187}[F] " + $"[{rank}] {Fname} {Lname} сообщил о происшествии: {text}", true);
            Manager.sendFractionMessage((int)Fractions.Models.Fractions.CITY, "!{#286187}[F] " + $"[{rank}] {Fname} {Lname} сообщил о происшествии: {text}", true);

            byte color = GetRandomColor();
            Blip CodeBlip = NAPI.Blip.CreateBlip(229, player.Position, 0.84f, color, $"Подкрепление - {text}", 255, 0, true, 0, 0);
            MarkCodes.Add(player, CodeBlip);

                sessionData.TimingsData.NextCode = DateTime.Now.AddMinutes(4);

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
            {
                var foreachMemberFractionData = foreachPlayer.GetFractionMemberData();
                if (foreachMemberFractionData == null) continue;

                Sounds.Play2d(foreachPlayer, "https://cdn.majestic-files.com/public/master/static/sounds/alert3.ogg", 0.7f);
            }
                
                NAPI.Task.Run(() => {
                    var blip = MarkCodes[player];
                    blip.Delete();
                    MarkCodes.Remove(player);
                }, 240000);
        }
            catch (Exception e)
            {
                Log.Write($"MarkCode Exception: {e.ToString()}");
            }

}

        [Command("mark", GreedyArg = true)]
        public static void CMD_mark(ExtPlayer player, string text)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (sessionData.CuffedData.Cuffed)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsCuffed), 3000);
                    return;
                }
                if (sessionData.DeathData.InDeath)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.IsDying), 3000);
                    return;
                }
                if (text.Length > 30) return;

                DateTime NextCode = sessionData.TimingsData.NextCode;
                if (DateTime.Now < NextCode)
                {
                    long ticks = sessionData.TimingsData.NextCode.Ticks - DateTime.Now.Ticks;
                    if (ticks >= 1)
                    {
                        DateTime g = new DateTime(ticks);
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы уже недавно отправляли код, подождите некоторое время.", 3000);
                        return;
                    }
                }
                MarkCode(player, text);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_mark Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("MarkAdded")]
        public static void Server_MarkAdded(ExtPlayer player, string text)
        {
            try
            {
                MarkCode(player, text);
            }
            catch (Exception e)
            {
                Log.Write($"Server_MarkAdded Exception: {e.ToString()}");
            }
        }
    }
}