using LinqToDB.Common;
using MySqlX.XDevAPI.Common;
using NeptuneEvo.Chars;
using NeptuneEvo.Core.Armor.Classes;
using NeptuneEvo.Core.Armor.Data;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NeptuneEvo.Chars.Models;
using Localization;

namespace NeptuneEvo.Core.Armor
{
    public class ArmorManager
    {
        private static readonly nLog Log = new nLog(nameof(ArmorManager));
        public static readonly string DatabaseName = "player_armors";
        public static Dictionary<int, Dictionary<string, bool>> PlayerArmorsList = new Dictionary<int, Dictionary<string, bool>>();

        public static void OnStart()
        {
            try
            {
                DataTable data = MySQL.QueryRead($"SELECT * FROM `{DatabaseName}`");
                if (data != null && data.Rows.Count != 0)
                {
                    foreach(DataRow row in data.Rows)
                    {
                        int uuid = Convert.ToInt32(row["uuid"]);
                        Dictionary<string, bool> list = JsonConvert.DeserializeObject<Dictionary<string, bool>>(row["list"].ToString());
                        

                        PlayerArmorsList.Add(uuid, list);
                    }
                }

                Log.Write($"Armor System Loaded: players with skins: ({PlayerArmorsList.Count})");
            }
            catch(Exception ex) { Log.Write("OnStart: " + ex.ToString()); }
        }

        #region Действия с хранилищем игрока
        public static bool AddArmor(int uuid, string type)
        {
            try
            {
                if (Config.GetArmor(type) is null || !GetPlayerArmors(uuid, out Dictionary<string, bool> list)) return false;

                if (list.ContainsKey(type))
                    return false;

                list.Add(type, false);
                Save(uuid);

                return true;
            }
            catch(Exception ex) { Log.Write("AddArmor: " + ex.ToString()); return false; }
        }

        public static bool GetPlayerArmors(int uuid, out Dictionary<string, bool> list)
        {
            if (!PlayerArmorsList.TryGetValue(uuid, out var tempList))
            {
                list = new Dictionary<string, bool>();
                PlayerArmorsList.TryAdd(uuid, list);
                MySQL.Query($"INSERT INTO `{DatabaseName}` (`uuid`, `list`) VALUES({uuid}, '{JsonConvert.SerializeObject(list)}')");
                return false;
            }
            else
            {
                list = tempList;
                return true;
            }
        }

        public static void Save(int uuid)
        {
            if (GetPlayerArmors(uuid, out Dictionary<string, bool> list))
                MySQL.Query($"UPDATE `{DatabaseName}` SET `list`='{JsonConvert.SerializeObject(list)}' WHERE `uuid`={uuid}");
        }

        public static bool RemoveArmor(int uuid, string type)
        {
            try
            {
                if (!GetPlayerArmors(uuid, out Dictionary<string, bool> list))
                    return false;

                bool state = list[type];
                list.Remove(type);
                Save(uuid);
                return true;
            }
            catch (Exception ex) { Log.Write("RemoveArmor: " + ex.ToString()); return false; }
        }
        #endregion

        #region Remote events
        public static void ArmorAction(ExtPlayer player, string type)
        {
            try
            {
                if (!GetPlayerArmors(player.GetUUID(), out Dictionary<string, bool> list) || !list.TryGetValue(type, out bool state)) return;

                ArmorData armorData = Config.GetArmor(type);

                // Отключен бронежилет
                if (!state)
                {
                    if (player.CharacterData.Gender != armorData.Gender)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете активировать этот бронежилет!", 3000);
                        return;
                    }

                    List<string> activeArmors = list.Where(x => x.Value).Select(x => x.Key).ToList();
                    foreach (string activeArmor in activeArmors)
                    {
                        ArmorData arData = Config.GetArmor(activeArmor);
                        if (arData is null) continue;
                        if (arData.IsHeavy == armorData.IsHeavy)
                            list[activeArmor] = false;
                    }

                    list[type] = true;
                }
                // Включен бронежилет
                else
                {
                    list[type] = false;
                }

                OpenManager(player);
                Save(player.GetUUID());

                // 7 - armor slot
                var armorItem = Chars.Repository.GetItemData(player, "accessories", 7);
                if (armorItem is null || armorItem.ItemId == ItemId.Debug || Convert.ToBoolean(armorItem.Data.Split("_")[1]) != armorData.IsHeavy) return;

                TryWearArmor(player);
            }
            catch(Exception ex) { Log.Write("OnArmorAction: " + ex.ToString()); }
        }

        public static void CreateGiftArmor(ExtPlayer player, string type)
        {
            try
            {
                if (!GetPlayerArmors(player.GetUUID(), out var list) || !list.ContainsKey(type)) return;

                if (Chars.Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", ItemId.ArmorGift, 1, type) == -1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                    return;
                }

                RemoveArmor(player.GetUUID(), type);
                TryWearArmor(player);

                OpenManager(player);

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы упаковали бронежилет!", 3000);
            }
            catch(Exception ex) { Log.Write("CreateGiftArmor: " + ex.ToString()); }
        }
        #endregion

        #region Item
        public static void UseArmorBoxItem(ExtPlayer player, InventoryItemData inventoryItem, string arrayName, int index)
        {
            try
            {
                string type = inventoryItem.Data;
                if (Config.GetArmor(type) is null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Коробка с бронежилетом содержит ошибку!", 3000);
                    return;
                }

                if (!AddArmor(player.GetUUID(), type))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас уже есть этот бронежилет!", 3000);
                    return;
                }

                Chars.Repository.RemoveIndex(player, arrayName, index, 1);
            }
            catch(Exception ex) { Log.Write("UseArmorBoxItem: " + ex.ToString()); }
        }
        #endregion

        public static void OnConnected(ExtPlayer player)
            => Trigger.ClientEvent(player, "client.armorManager.init", JsonConvert.SerializeObject(Config.ARMOR_TYPES));

        public static void OpenManager(ExtPlayer player)
        {
            try
            {
                if (!GetPlayerArmors(player.GetUUID(), out Dictionary<string, bool> list)) return;
                Trigger.ClientEvent(player, "client.armorManager.open", JsonConvert.SerializeObject(list));
            }
            catch(Exception ex) { Log.Write("OpenManager: " + ex.ToString()); }
        }

        public static bool TryWearArmor(ExtPlayer player)
        {
            try
            {
                if (!GetPlayerArmors(player.GetUUID(), out Dictionary<string, bool> list)) return false;

                int playerFractionId = 0;
                var fractionMemberData = player.GetFractionMemberData();
                if (fractionMemberData != null)
                    playerFractionId = fractionMemberData.Id;

                ArmorData needArmorData = null;
                string[] activeArmors = list.Where(x => x.Value).Select(x => x.Key).ToArray();

                bool isHeavy = false;
                var armorItem = Chars.Repository.GetItemData(player, "accessories", 7);
                if (armorItem.ItemId != ItemId.Debug)
                    isHeavy = Convert.ToBoolean(armorItem.Data.Split("_")[1]);

                foreach (string activeArmor in activeArmors)
                {
                    ArmorData arData = Config.GetArmor(activeArmor);
                    if (arData is null || arData.IsHeavy != isHeavy) continue;

                    needArmorData = arData;
                }

                if (needArmorData is null)
                {
                    var fractionArmorData = GetFractionArmorData(playerFractionId, player.CharacterData.Gender, isHeavy);
                    if (fractionArmorData is null)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка использования бронежилета!", 3000);
                        return false;
                    }
                    ClothesComponents.SetClothes(player, 9, fractionArmorData.Drawable, fractionArmorData.Texture);

                    ClothesComponents.UpdateClothes(player);
                    return false;
                }

                ClothesComponents.SetClothes(player, 9, needArmorData.Drawable, needArmorData.Texture);
                ClothesComponents.UpdateClothes(player);
                return true;
            }
            catch(Exception ex) { Log.Write("WearArmor: " + ex.ToString()); return false; }
        }

        public static ArmorData GetFractionArmorData(int fraction, bool gender, bool isHeavy)
        {
            var armors = Config.ARMOR_FRACTION_TYPES.Where(x => x.FractionId == fraction && x.Gender == gender && x.IsHeavy == isHeavy);
            if (armors.Count() == 0) return null;
            return armors.First();
        } 
    }
}
