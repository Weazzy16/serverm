using System;
using Localization;
using GTANetworkAPI;
using NeptuneEvo.Core;
using Redage.SDK;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.Quests.Models;
using GTANetworkMethods;
using Google.Protobuf;
using System.Numerics;
using System.Collections.Generic;
using System.Data;
using NeptuneEvo.Chars;
using Org.BouncyCastle.Asn1.X509;
using Database;
using NeptuneEvo.Chars.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;
using NeptuneEvo.Players.Phone.Messages.Models;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Players.Models;
using System.Threading;
using Org.BouncyCastle.Asn1.Cms;

namespace NeptuneEvo.AleSystems
{
    class GolodSystem : Script
    {
        private static readonly nLog Log = new nLog(nameof(GolodSystem));

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Log.Write("Hungersystem is loaded");
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("cefEatWaterReady")]
        public void OnEatWaterReady(ExtPlayer player)
        {
            GolodSystem.GolodSystemStart(player);
        }

        public static void GolodSystemStart(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            sessionData.TimersData.GolodTimer = Timers.Start(145000, () => GolodTimer(player));
            sessionData.TimersData.WaterTimer = Timers.Start(115000, () => WaterTimer(player));

            var eat = $"{characterData.Golod}%";
            var water = $"{characterData.Water}%";
            Trigger.ClientEvent(player, "start:EatSystem::client", eat);
            Trigger.ClientEvent(player, "start:WaterSystem::client", water);
        }
public static void GolodTimer(ExtPlayer player)
{
    try
    {
        var characterData = player.GetCharacterData();
        if (characterData == null) return;

        if (characterData.Golod > 0)
        {
            characterData.Golod -= 1;
        }

        if (characterData.Golod > 100)
        {
            characterData.Golod = 100;
        }
        else if (characterData.Golod < 0)
        {
            characterData.Golod = 0;
        }

        var eat = $"{characterData.Golod}%";
        Trigger.ClientEvent(player, "update:EatSystem::client", eat);

        if (characterData.Golod <= 1)
        {
            HandleCriticalHungerLevel(player, characterData);
        }
    }
    catch (Exception e)
    {
        Log.Write($"GolodTimer Exception: {e.ToString()}");
    }
}
private static void HandleCriticalHungerLevel(ExtPlayer player, CharacterData characterData)
{
    NAPI.Task.Run(() =>
    {
        characterData.Golod = 0;
        Trigger.ClientEvent(player, "update:EatSystem::client", "0%");
        player.Health -= 2;
        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Вам срочно нужно поесть, Вы теряете силы!", 3000);
    }, 150);
}
public static void WaterTimer(ExtPlayer player)
{
    try
    {
        var characterData = player.GetCharacterData();
        if (characterData == null) return;

        if (characterData.Water > 0)
        {
            characterData.Water -= 1;
        }

        if (characterData.Water > 100)
        {
            characterData.Water = 100;
        }
        else if (characterData.Water < 0)
        {
            characterData.Water = 0;
        }

        var water = $"{characterData.Water}%";
        Trigger.ClientEvent(player, "update:WaterSystem::client", water);

        if (characterData.Water <= 1)
        {
            HandleCriticalWaterLevel(player, characterData);
        }
    }
    catch (Exception e)
    {
        Log.Write($"WaterTimer Exception: {e.ToString()}");
    }
}
private static void HandleCriticalWaterLevel(ExtPlayer player, CharacterData characterData)
{
    NAPI.Task.Run(() =>
    {
        characterData.Water = 0;
        Trigger.ClientEvent(player, "update:WaterSystem::client", "0%");
        player.Health -= 2;
        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Вам срочно нужно выпить воды, Вы теряете силы!", 3000);
    }, 150);
}
        public static void GolodChanger(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.Golod >= 100) characterData.Golod = 100;
                if (characterData.Golod <= 0)
                {
                    characterData.Golod = 0;
                    Trigger.ClientEvent(player, "update:EatSystem::client", "0%");
                }
                var eat = $"{characterData.Golod}%";

                Trigger.ClientEvent(player, "update:EatSystem::client", eat);
            }
            catch (Exception e)
            {
                Log.Write($"GolodChanger Exception: {e.ToString()}");
            }
        }
        public static void WaterChanger(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.Water >= 100) characterData.Water = 100;
                if (characterData.Water <= 0)
                {
                    characterData.Water = 0;
                    Trigger.ClientEvent(player, "update:WaterSystem::client", "0%");
                }
                    var water = $"{characterData.Water}%";

                Trigger.ClientEvent(player, "update:WaterSystem::client", water);
            }
            catch (Exception e)
            {
                Log.Write($"WaterChanger Exception: {e.ToString()}");
            }
        }
    }
}
