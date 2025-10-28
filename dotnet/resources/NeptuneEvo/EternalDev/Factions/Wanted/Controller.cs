using GTANetworkAPI;
using Localization;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Fractions.LSNews.LiveStream;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using Redage.SDK;
using System;

namespace NeptuneEvo.EternalDev.Factions.Wanted
{
    public class Controller : Script
    {
        public static void Open(ExtPlayer player, ExtPlayer target)
        {
            if (!player.IsFractionAccess(RankToAccess.Su) || target is null || target.CharacterData is null) 
                return;
            
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null)
                return;

            if (!sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                return;
            }

            if (!sessionData.WorkData.OnDuty)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.MustWorkDay), 3000);
                return;
            }
            if (memberFractionData.Id == (int)Fractions.Models.Fractions.CITY && !SafeZones.IsSafeZone(sessionData.InsideSafeZone, SafeZones.ZoneName.Court))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны находиться в здании суда", 3000);
                return;
            }
            if (target.CharacterData.ArrestTime != 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок в тюрьме", 3000);
                return;
            }
            if (!target.SessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCuffed), 3000);
                return;
            }

            Trigger.ClientEvent(player, "e-dev.wantedMenu.open");
        }

        [RemoteEvent("e-dev.wantedMenu.cancel")]
        public static void Cancel(ExtPlayer player)
        {
            player.SessionData.SelectData.SelectedPlayer = null;
            Trigger.ClientEvent(player, "e-dev.wantedMenu.close");
        }

        [RemoteEvent("e-dev.wantedMenu.submit")]
        public static void Submit(ExtPlayer player, int stars, string reason)
        {
            var target = player.SessionData.SelectData.SelectedPlayer;
            if (target is null)
                return;

            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null)
                return;

            if (!player.SessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                return;
            }

            if (stars < 0 || stars > 5)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете выдать такое количество звёзд", 3000);
                return;
            }

            if (player == target)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Невозможно применить на себе", 3000);
                return;
            }

            if (player.Position.DistanceTo(target.Position) > 2)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Человек слишком далеко", 3000);
                return;
            }
            if (!player.SessionData.WorkData.OnDuty)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны начать рабочий день", 3000);
                return;
            }
            if (!target.SessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCuffed), 3000);
                return;
            }

            if (stars == 0)
            {
                if (target.CharacterData.WantedLVL is null)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Игрок не находится в розыске", 3000);
                    return;
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы сняли розыск с игрока {target.Name.Replace('_', ' ')}", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name.Replace('_', ' ')} Снял с вас розыск", 3000);

                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Su, $"Снял розыск с игрока {target.GetName().Replace('_', ' ')} #{target.GetUUID()}");
                Police.setPlayerWantedLevel(target, null);

                target.Eval($"mp.game.audio.playSoundFrontend(-1, \"LOOSE_MATCH\", \"HUD_MINI_GAME_SOUNDSET\", true);");
                GameLog.FracLog(memberFractionData.Id, player.GetUUID(), target.CharacterData.UUID, player.Name, target.Name, $"su[0]");

                var message = "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} #{player.CharacterData.UUID} Cнял розыск с игрока {target.Name.Replace('_', ' ')} #{target.CharacterData.UUID}. Причина: {reason}";
                Manager.sendFractionMessage(7, message, true);
                Manager.sendFractionMessage(9, message, true);
            }
            else
            {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы объявили игрока " + target.Name.Replace('_', ' ') + " в розыск", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"{player.Name.Replace('_', ' ')} Объявил Вас в розыск. Причина: {reason}", 3000);
                Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Su, $"Объявил в розыск {target.GetName().Replace('_', ' ')} #{target.GetUUID()}");

                WantedLevel wantedLevel = new WantedLevel(stars, player.Name, DateTime.Now, reason);
                Police.setPlayerWantedLevel(target, wantedLevel);

                target.Eval($"mp.game.audio.playSoundFrontend(-1, \"LOOSE_MATCH\", \"HUD_MINI_GAME_SOUNDSET\", true);");
                GameLog.FracLog(memberFractionData.Id, player.GetUUID(), target.CharacterData.UUID, player.Name, target.Name, $"su[{wantedLevel.Level}]");

                var message = "!{#FF8C00}[F] " + $"{player.Name.Replace('_', ' ')} #{player.CharacterData.UUID} Объявил в розыск {target.Name.Replace('_', ' ')} #{target.CharacterData.UUID}. Причина: {reason}";

                Manager.sendFractionMessage(7, message, true);
                Manager.sendFractionMessage(9, message, true);
            }

            Cancel(player);
        }
    }
}
