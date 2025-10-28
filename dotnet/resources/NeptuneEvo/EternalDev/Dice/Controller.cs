using EternalCore;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.BattlePass.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.EternalDev.Dice.Configs;
using NeptuneEvo.EternalDev.Dice.Enums;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.Dice
{
    public class Controller
    {
        public static DiceConfig Config = ELib.JsonReader.Read<DiceConfig>(DiceConfig.Path);

        public static void SendRequest(ExtPlayer player, ExtPlayer target, int bet)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData is null) return;

                if (bet < Config.MinBet || bet > Config.MaxBet)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Значение не может быть меньше {Config.MinBet}$ и больше {Config.MaxBet}", 3000);
                    return;
                }

                var targetSessionData = target.GetSessionData();
                if (target is null || targetSessionData is null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден!", 3000);
                    return;
                }

                if (targetSessionData.DiceData.Target != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игроку уже предложили игру в кости!", 3000);
                    return;
                }

                targetSessionData.DiceData = new DiceData()
                {
                    Target = player,
                    Money = bet
                };

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы предложили {GetName(player, target)} сыграть в кости на {bet}$", 3000);
                Trigger.ClientEvent(target, "openDialog", "ETERNAL_DICE", $"{GetName(target, player)} предложил сыграть в кости на ${bet}");
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(SendRequest), ex, nameof(Controller)); }
        }

        public static void Accept(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData is null) return;

                var diceData = sessionData.DiceData;
                var target = diceData.Target;

                bool isPlayerReady = CheckToStart(diceData, player, target);
                bool isTargetReady = CheckToStart(diceData, target, player);

                if (!isPlayerReady || !isTargetReady)
                {
                    Cancel(player, false);
                    return;
                }

                int playerDice = GenerateNumber();
                int targetDice = GenerateNumber();

                PlayAnimation(player, playerDice);
                PlayAnimation(target, targetDice);

                ResultGame(player, target, playerDice, targetDice, diceData.Money);
                ResultGame(target, player, targetDice, playerDice, diceData.Money);

                sessionData.DiceData = new DiceData();
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(Accept), ex, nameof(Controller)); }
        }

        public static void Cancel(ExtPlayer player, bool cancel = true)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData is null) return;

                var target = sessionData.DiceData.Target;
                if (target != null)
                    Notify.Send(target, NotifyType.Error, NotifyPosition.BottomCenter, cancel ? $"{GetName(target, player)} отказался от игры!" : "Игра отменена", 3000);

                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, cancel ? "Вы отказались от игры" : "Игра отменена", 3000);
                sessionData.DiceData = new DiceData();
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(Cancel), ex, nameof(Controller)); }
        }

        private static void ResultGame(ExtPlayer player, ExtPlayer target, int cubeNumber, int targetCubeNumber, int bet)
        {
            if (player is null) 
                return;

            var result = cubeNumber == targetCubeNumber ? GameResultType.Draw : cubeNumber > targetCubeNumber ? GameResultType.Win : GameResultType.Lose;
            var resultName = result == GameResultType.Draw ? "Ничья" 
                : result == GameResultType.Lose ? "Вы проиграли" 
                : "Вы победили"; 

            Notify.Send(player, result == GameResultType.Draw ? NotifyType.Info : result == GameResultType.Lose ? NotifyType.Error : NotifyType.Success, NotifyPosition.BottomCenter, 
                $"{GetName(player, player)} выпало {cubeNumber}, {GetName(player, target)} выпало {targetCubeNumber}. {resultName}!", 3000);

            switch(result)
            {
                case GameResultType.Win:
                    MoneySystem.Wallet.Change(player, bet);
                    break;
                case GameResultType.Lose:
                    MoneySystem.Wallet.Change(player, -bet);
                    break;
            }
        }

        private static string GetName(ExtPlayer player, ExtPlayer target)
        {
            if (target is null)
                return "Игроку";

            var targetName = target.GetName();

            var characterData = player.GetCharacterData();
            if (characterData is null || !characterData.Friends.TryGetValue(targetName, out bool handshaked) || !handshaked)
                return $"#{target.GetUUID()}";

            return targetName.Replace("_", " ");
        }

        private static bool CheckToStart(DiceData diceData, ExtPlayer player, ExtPlayer target)
        {
            if (player is null) 
                return false;
            
            if (diceData.Target is null || target.Position.DistanceTo(player.Position) > 10)
                return false;

            if (player.CharacterData.Money < diceData.Money)
                return false;

            return true;
        }

        private static void PlayAnimation(ExtPlayer player, int cubeNumber)
        {
            var sessionData = player.GetSessionData();
            if (sessionData is null) return;

            if (sessionData.TimersData.DiceAnimation != null)
            {
                var timerData = Timers.Get(sessionData.TimersData.DiceAnimation);

                if (timerData != null)
                    timerData.action.Invoke();

                Timers.Stop(sessionData.TimersData.DiceAnimation);
            }

            Trigger.PlayAnimation(player, "majestic_animations_custom", "dice_big", 49);
            Attachments.AddAttachment(player, NAPI.Util.GetHashKey($"dice_cube_{cubeNumber}"));

            sessionData.TimersData.DiceAnimation = Timers.StartOnce(5000, () =>
            {
                if (player is null)
                    return;

                Trigger.StopAnimation(player);
                Attachments.RemoveAttachment(player, NAPI.Util.GetHashKey($"dice_cube_{cubeNumber}"));
            }, true);
        }

        private static int GenerateNumber()
            => Main.rnd.Next(1, 7);
    }
}
