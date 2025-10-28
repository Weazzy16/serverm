using Localization;
using MySqlX.XDevAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Database.Models;
using NeptuneEvo.Handles;
using NeptuneEvo.AleSystems.Fishing.Classes;
using NeptuneEvo.AleSystems.Fishing.Data;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing
{
    public class FishingManager
    {
        private static readonly nLog Log = new nLog(nameof(FishingManager));

        public static void Initialize()
        {
            try
            {
                FishingPlayer.Inititalize();
                Config.Initialize();
            }
            catch(Exception ex) { Log.Write("Initialize: " + ex.ToString()); }
        }

        public static void OnPayday() => Config.STORES.ForEach(buyer => buyer.UpdatePrices());

        public static FishItemData GetRandomFish(int currentLvl, List<ItemId> fish, ItemId rodId)
        {
            try
            {
                int pool = 0;

                var fishItems = Config.FISH_ITEMS_DATA.Where(x => fish.Contains(x.ItemId) && x.MinLvl <= currentLvl && x.Rods.Contains(rodId)).ToList();
                fishItems.ForEach(item => pool += item.Chance);

                int random = Main.rnd.Next(0, pool);
                int accumulatedProbability = 0;

                foreach(var item in fishItems)
                {
                    accumulatedProbability += item.Chance;
                    if (random <= accumulatedProbability)
                        return item;
                }

                return fishItems[0];
            }
            catch (Exception ex) { Log.Write("GetRandomFish: " + ex.ToString()); return null; }
        }

        public static void UseRod(ExtPlayer player, InventoryItemData item)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null) return;

                var characterData = player.GetCharacterData();
                if (characterData is null || !Config.RODS_DATA.TryGetValue(item.ItemId, out var rodData)) return;

                var colShape = player.ColShapesData.Find(x => x.ColShapeId == Functions.ColShapeEnums.FishSpot);
                if (colShape is null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не в зоне рыболовства", 3000);
                    return;
                }

                if (player.HasData("fishing.timer"))
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Вы уже рыбачите!", 3000);
                    return;
                }

                int countOfFishBait = Chars.Repository.getCountToLacationItem($"char_{player.CharacterData.UUID}", "inventory", ItemId.Bait);
                if (countOfFishBait < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет наживки!", 3000);
                    return;
                }
                Chars.Repository.Remove(player, $"char_{player.CharacterData.UUID}", "inventory", ItemId.Bait, 1);

                int time = Main.rnd.Next(rodData.MinTime, rodData.MaxTime);

                string timerName = Timers.StartOnce(time * 1000, () => StartMinigame(player));
                player.SetData("fishing.timer", timerName);

                Trigger.PlayAnimation(player, "amb@world_human_stand_fishing@base", "base", 1);

                player.SetData("fishing.currentRod", item);

                Attachments.AddAttachment(player, Attachments.AttachmentsName.Rod);
            }
            catch(Exception ex) { Log.Write("UseRod: " + ex.ToString()); }
        }

        private static void StartMinigame(ExtPlayer player)
        {
            try
            {
                if (player is null || !player.HasData("fishing.timer")) return;
                Timers.Stop(player.GetData<string>("fishing.timer"));
                player.ResetData("fishing.timer");

                Trigger.StopAnimation(player);
                Trigger.PlayAnimation(player, "amb@world_human_stand_fishing@idle_a", "idle_c", 1);
                Trigger.ClientEvent(player, "client.fishing.minigame.open");

                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Что-то клюнуло...", 3000);
                player.SetData("fishing.minigame.started", true);
            }
            catch(Exception ex) { Log.Write("StartMinigame: " + ex.ToString()); }
        }

        public static void EndMinigame(ExtPlayer player, bool state)
        {
            try
            {
                if (!player.HasData("fish.spot") || !player.HasData("fishing.minigame.started") || !player.HasData("fishing.currentRod")) return;

                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                var session = player.GetSessionData();
                if (session is null) return;

                Reset(player, !state);
                if (!state) return;

                var fishSpot = player.GetData<FishSpot>("fish.spot");

                var activeItem = player.GetData<InventoryItemData>("fishing.currentRod");
                if (activeItem is null || !Config.RODS_DATA.TryGetValue(activeItem.ItemId, out var rodData))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас в руках должна быть удочка", 3000);
                    BasicSync.DetachObject(player);
                    return;
                }

                var fishingPlayerData = player.GetFishingPlayer();
                if (fishingPlayerData is null) return;

                FishItemData fishItem = GetRandomFish(fishingPlayerData.LVL, fishSpot.Fish, activeItem.ItemId);
                if (fishItem is null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вам не удалось поймать рыбу!", 3000);
                    BasicSync.DetachObject(player);
                    return;
                }

                if (string.IsNullOrEmpty(activeItem.Data))
                    activeItem.Data = "100";
                activeItem.Data = Convert.ToString(Convert.ToDouble(activeItem.Data) - rodData.Wear);

                if (Convert.ToDouble(activeItem.Data) < 1)
                {
                    Chars.Repository.Remove(player, $"char_{player.CharacterData.UUID}", "inventory", activeItem.ItemId, 1);
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Удочка сломалась!", 3000);
                    BasicSync.DetachObject(player);
                }

                if (Chars.Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", fishItem.ItemId, 1) == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    BasicSync.DetachObject(player);
                    return;
                }

                fishingPlayerData.AddProgress(player);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы поймали {Chars.Repository.ItemsInfo.FirstOrDefault(x => x.Key == fishItem.ItemId).Value.Name}!", 3000);
                BasicSync.DetachObject(player);
            }

            catch(Exception ex) { Log.Write("EndMinigame: " + ex.ToString()); }
        }

        public static void Reset(ExtPlayer player, bool isLoose = false)
        {
            try
            {
                if (!player.HasData("fishing.minigame.started")) return;

                if (player.HasData("fishing.timer"))
                {
                    Timers.Stop(player.GetData<string>("fishing.timer"));
                    player.ResetData("fishing.timer");
                }

                Trigger.StopAnimation(player);
                Trigger.ClientEvent(player, "client.fishing.minigame.close");

                Attachments.RemoveAttachment(player, Attachments.AttachmentsName.Rod);

                if (isLoose)
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Наживка слетела", 3000);
                    BasicSync.DetachObject(player);
            }
            catch (Exception ex) { Log.Write("OnDead: " + ex.ToString()); }
        }
    }
}
