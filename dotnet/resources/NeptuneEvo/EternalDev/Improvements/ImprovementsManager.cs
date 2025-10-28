using GTANetworkAPI;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.EternalDev.Improvements.Classes;
using NeptuneEvo.EternalDev.Improvements.Data;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.Improvements
{
    public class ImprovementsManager
    {
        private static readonly nLog Log = new nLog(nameof(ImprovementsManager));

        public static void OnUse(ExtPlayer player, InventoryItemData inventoryItem, string arrayName, int index)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null) return;

                if (!GetImprovementItemData(inventoryItem.ItemId, out var improvementsData)) return;

                if (session.TimersData.ImprovementData != null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На вас уже действует какой-то эффект!", 3000);
                    return;
                }

                if (Config.COOLDOWN_TO_USE_AFTER_GET_DAMAGE != 0 && session.LastDamage.AddSeconds(Config.COOLDOWN_TO_USE_AFTER_GET_DAMAGE) > DateTime.Now)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"В вас недавно стреляли!", 3000);
                    return;
                }

                StartImprovement(player, improvementsData);

                player.PlayAnimation(improvementsData.Animation.Dictionary, improvementsData.Animation.Name, improvementsData.Animation.Flag);
                Chars.Attachments.AddAttachment(player, Chars.Repository.ItemsInfo[inventoryItem.ItemId].Model);

                NAPI.Task.Run(() => 
                { 
                    try
                    {
                        if (player is null) 
                            return;

                        player.PlayAnimation("rcmcollect_paperleadinout@", "kneeling_arrest_get_up", 33);
                        Chars.Attachments.RemoveAttachment(player, Chars.Repository.ItemsInfo[inventoryItem.ItemId].Model);
                    }
                    catch(Exception ex) { Log.Write("OnUse.Task: " + ex.ToString()); }
                }, improvementsData.Animation.Duration);

                Chars.Repository.RemoveIndex(player, arrayName, index, 1);
            }
            catch (Exception ex) { Log.Write("OnUse: " + ex.ToString()); }
        }

        private static void StartImprovement(ExtPlayer player, ImprovementsData improvementsData)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null) return;

                if (improvementsData.ExtraHealing > 0)
                    Heal(player, improvementsData.ExtraHealing);

                session.TimersData.ImprovementData 
                    = Timers.StartOnce(1000 * improvementsData.TimeEffect, () => StopImprovement(player));

                if (improvementsData.HealingCooldown > 0 && improvementsData.Healing > 0)
                    session.TimersData.ImprovementHealing
                        = Timers.Start(1000 * improvementsData.HealingCooldown, () => Heal(player, improvementsData.Healing));

                Trigger.SetSharedData(player, "improvementData.damageMultiplayer", improvementsData.DamageMultiplayer);

                Trigger.ClientEvent(player, "client.improvements.timer", improvementsData.TimeEffect);
            }
            catch(Exception ex) { Log.Write("StartImprovement: " + ex.ToString()); }
        }

        private static void StopImprovement(ExtPlayer player)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null) return;

                if (session.TimersData.ImprovementData != null)
                    Timers.Stop(session.TimersData.ImprovementData);
                session.TimersData.ImprovementData = null;

                if (session.TimersData.ImprovementHealing != null)
                    Timers.Stop(session.TimersData.ImprovementHealing);
                session.TimersData.ImprovementHealing = null;

                Trigger.SetSharedData(player, "improvementData.damageMultiplayer", 0);
            }
            catch(Exception ex) { Log.Write("StopImprovement: " + ex.ToString()); }
        }

        private static void Heal(ExtPlayer player, int value)
        {
            try
            {
                NAPI.Task.Run(() =>
                {
                    player.Health = Math.Min(player.Health + value, 100);
                });
            }
            catch(Exception ex) { Log.Write("Heal: " + ex.ToString()); }
        }

        public static bool GetImprovementItemData(ItemId itemId, out ImprovementsData improvementsData)
        {
            Config.IMPROVEMENTS_DATA.TryGetValue(itemId, out improvementsData);
            return improvementsData != null;
        }
    }
}
