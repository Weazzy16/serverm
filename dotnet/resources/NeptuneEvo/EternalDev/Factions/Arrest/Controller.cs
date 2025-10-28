using EternalCore;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.Factions.Arrest.Configs;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Table.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;

namespace NeptuneEvo.EternalDev.Factions.Arrest
{
    public class Controller : Script
    {
        public static ArrestMenuConfig Config { get; set; } = ELib.JsonReader.Read<ArrestMenuConfig>(ArrestMenuConfig.Path);

        public static void Open(ExtPlayer player, ExtPlayer target)
        {
            if (!player.IsFractionAccess(RankToAccess.Arrest) || target is null)
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
            if (sessionData.InArrestArea == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны быть возле камеры", 3000);
                return;
            }
            if (target.CharacterData.ArrestTime != 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок уже в тюрьме", 3000);
                return;
            }
            if (target.CharacterData.WantedLVL == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в розыске", 3000);
                return;
            }
            if (!target.SessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCuffed), 3000);
                return;
            }

            Trigger.ClientEvent(player, "e-dev.arrestMenu.open", JsonConvert.SerializeObject(new
            {
                TimeSettings = new int[] { Config.TimeSettings[0], Config.TimeSettings[1] * target.CharacterData.WantedLVL.Level },
                Config.PledgeSettings,

                UserData = new
                {
                    Avatar = $"https://a.rsg.sc/n/{target.SessionData.SocialClubName}",
                    Name = target.GetName().Replace("_", " "),
                    UUID = target.GetUUID(),
                    Phone = target.CharacterData.Sim != -1 ? $"{target.CharacterData.Sim}" : "Нет",
                    target.Customization.Gender,
                    CriminalRecord = 0
                }
            }));
        }

        [RemoteEvent("e-dev.arrestMenu.cancel")]
        public static void Cancel(ExtPlayer player)
        {
            player.SessionData.SelectData.SelectedPlayer = null;
            Trigger.ClientEvent(player, "e-dev.arrestMenu.close");
        }

        [RemoteEvent("e-dev.arrestMenu.submit")]
        public static void Submit(ExtPlayer player, int time, bool canPledge, int pledge, string reason)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            var target = sessionData.SelectData.SelectedPlayer;
            if (target is null)
                return;

            var memberFractionData = player.GetFractionMemberData();
            if (memberFractionData == null)
                return;

            if (!sessionData.WorkData.OnDuty && Fractions.Manager.FractionTypes[memberFractionData.Id] == FractionsType.Gov)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.WorkDayNotStarted), 3000);
                return;
            }

            var targetSessionData = target.GetSessionData();
            if (targetSessionData == null) return;

            var targetCharacterData = target.GetCharacterData();
            if (targetCharacterData == null) return;

            if (!player.IsFractionAccess(RankToAccess.Arrest)) return;

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
            if (!sessionData.WorkData.OnDuty)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны начать рабочий день", 3000);
                return;
            }
            if (sessionData.InArrestArea == -1)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы должны быть возле камеры", 3000);
                return;
            }
            if (targetCharacterData.ArrestTime != 0)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок уже в тюрьме", 3000);
                return;
            }
            if (targetCharacterData.WantedLVL == null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не в розыске", 3000);
                return;
            }
            if (!targetSessionData.CuffedData.Cuffed)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoCuffed), 3000);
                return;
            }
            if (targetSessionData.Following != null)
                Fractions.FractionCommands.unFollow(targetSessionData.Following, target);

            Fractions.FractionCommands.unCuffPlayer(target);
            targetSessionData.CuffedData.CuffedByCop = false;
            targetSessionData.CuffedData.CuffedByMafia = false;

            targetCharacterData.WantedLVL.Reason = reason;
            targetCharacterData.ArrestTime = time * 60;
            targetCharacterData.ArrestType = (sbyte)sessionData.InArrestArea;

            Commands.RPChat("sme", player, " поместил {name} в КПЗ", target);
            Fractions.Manager.sendFractionMessage((int)Fractions.Models.Fractions.POLICE, "!{#1e90ff}[F] " + $"{player.Name.Replace('_', ' ')} #{player.CharacterData.UUID} Посадил в КПЗ {target.Name.Replace('_', ' ')} ({targetCharacterData.WantedLVL.Reason})", true);
            Fractions.Manager.sendFractionMessage((int)Fractions.Models.Fractions.FIB, "!{#1e90ff}[F] " + $"{player.Name.Replace('_', ' ')} #{player.CharacterData.UUID} Посадил в КПЗ {target.Name.Replace('_', ' ')} ({targetCharacterData.WantedLVL.Reason})", true);
            Fractions.Table.Logs.Repository.AddLogs(player, FractionLogsType.Arrest, $"Посадил в КПЗ {target.Name.Replace('_', ' ')} ({targetCharacterData.UUID})");

            target.Eval($"mp.game.audio.playSoundFrontend(-1, \"Mission_Pass_Notify\", \"DLC_HEISTS_GENERAL_FRONTEND_SOUNDS\", true);");

            int minutes = Convert.ToInt32(targetCharacterData.ArrestTime / 60);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы посадили игрока #{target.CharacterData.UUID} на {minutes} минут", 3000);
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок #{player.CharacterData.UUID} посадил Вас на {minutes} минут", 3000);

            GameLog.Arrest(player.GetUUID(), targetCharacterData.UUID, targetCharacterData.WantedLVL.Reason, targetCharacterData.WantedLVL.Level, player.Name, target.Name);
            GameLog.FracLog(memberFractionData.Id, player.GetUUID(), targetCharacterData.UUID, player.Name, target.Name, $"arrest({targetCharacterData.WantedLVL.Reason})");
            Fractions.FractionCommands.arrestPlayer(target);

            if (canPledge)
            {
                Trigger.ClientEvent(target, "openDialog", "ETERNAL::ARRESTMENU:PLEDGE", $"Вам доступен выход под залог.Сумма залога ${pledge}. Желаете оплатить выход под залог ?");
                target.SessionData.Pledge = pledge;
            }

            Cancel(player);
        }

        public static void PledgeConfirm(ExtPlayer player)
        {
            if (player.CharacterData.ArrestTime <= 0)
                return;

            if (!MoneySystem.Wallet.Change(player, -player.SessionData.Pledge))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас недостаточно средств", 3000);
                return;
            }

            player.SessionData.Pledge = 0;
            Fractions.FractionCommands.freePlayer(player, false);
        }
    }
}
