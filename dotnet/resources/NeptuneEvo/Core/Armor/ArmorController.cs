using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Core.Armor.Data;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Core.Armor
{
    public class ArmorController : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public static void OnStart()
            => ArmorManager.OnStart();

        [RemoteEvent("server.armorManager.action")]
        public static void On_ArmorAction(ExtPlayer player, string type)
            => ArmorManager.ArmorAction(player, type);

        [RemoteEvent("server.armorManager.createGift")]
        public static void On_CreateGiftArmor(ExtPlayer player, string type)
            => ArmorManager.CreateGiftArmor(player, type);

        [RemoteEvent("server.armorMenu.open")]
        public static void On_OpenManager(ExtPlayer player) 
            => ArmorManager.OpenManager(player);

        [Command("givearmor")]
        public static void Command_GiveArmor(ExtPlayer player, int playerId, string type)
        {
            var characterData = player.GetCharacterData();
            if (characterData is null || characterData.AdminLVL < Config.MIN_ADMIN_LVL_TO_GIVE_ARMOR) return;

            var target = Main.GetPlayerByID(playerId);
            if (target is null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок не найден!", 3000);
                return;
            }

            if (ArmorManager.AddArmor(target.GetUUID(), type))
            {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы выдали броню {type}", 3000);
            }
            else
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Не удалось выдать броню", 3000);
            }
        }
    }
}
