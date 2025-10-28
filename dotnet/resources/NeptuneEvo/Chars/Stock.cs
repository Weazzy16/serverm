using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Character;
using NeptuneEvo.Quests;
using Redage.SDK;
using System;
using Localization;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;

namespace NeptuneEvo.Chars
{
    class Stock : Script
    {
        private static readonly nLog Log = new nLog("Chars.Stock");


        [ServerEvent(Event.ResourceStart)]
        public void Init()
        {
            try
            {
                Main.CreateBlip(new Main.BlipData(478, "Хранилище 'GoPostal'", new Vector3(-545.02136, -204.28348, 38.2), 32, true));
                Main.CreateBlip(new Main.BlipData(525, "Центр занятости", new Vector3(436.5074, -627.4617, 28.707539), 0, true));
                CustomColShape.CreateCylinderColShape(new Vector3(1048.2255, -3097.1624, -38.9999), 1f, 2, 5, ColShapeEnums.WarehouseExit);
                PedSystem.Repository.CreateQuest("s_m_m_postal_01",
                    new Vector3(-545.02136, -204.28348, 38.2), -151.96516f, 0,
                    title: "~y~NPC~w~ Почтальон Видюля\nСклад посылок", colShapeEnums: ColShapeEnums.Warehouse);
            }
            catch (Exception e)
            {
                Log.Write($"Init Exception: {e}");
            }
        }

        [Interaction(ColShapeEnums.WarehouseEnter)]
        public static void WarehouseEnter(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            player.Position = new Vector3(1048.2255, -3097.1624, -38.9999);
            Trigger.Dimension(player, 5);
            characterData.IsInPostalStock = true;
        }

        [Interaction(ColShapeEnums.WarehouseExit)]
        public static void WarehouseExit(ExtPlayer player)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;
            player.Position = new Vector3(132.9969, 96.3529, 83.5076);
            Trigger.Dimension(player, 0);
            characterData.IsInPostalStock = false;
        }

        [Interaction(ColShapeEnums.Warehouse)]
        public static void OpenWarehouse(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (sessionData.CuffedData.Cuffed) return;
            if (sessionData.DeathData.InDeath) return;
            if (Main.IHaveDemorgan(player, true)) return;

            player.SelectQuest(new PlayerQuestModel("npc_stock", 0, 0, false, DateTime.Now));
            Trigger.ClientEvent(player, "client.quest.open", index, "npc_stock", 0, 0, 0);
            BattlePass.Repository.UpdateReward(player, 151);
        }

        // Загрузка содержимого склада (index = 0)
        [RemoteEvent("server.gta5devmenustock")]
        public static void Perform(ExtPlayer player, int index)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (index == 0)
                {
                    if (!FunctionsAccess.IsWorking("warehouseopen"))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            LangFunc.GetText(LangType.Ru, DataName.FunctionOffByAdmins), 3000);
                        return;
                    }
                    Repository.LoadOtherItemsData(player, "warehouse", characterData.UUID.ToString(), 10);
                }
                else
                {
                    // Неправильное использование — игнорируем или логируем
                    Log.Write($"Perform: unexpected index {index}, expected 0");
                }
            }
            catch (Exception e)
            {
                Log.Write($"OnWarehouse Exception: {e}");
            }
        }

        // Забрать один предмет из склада
        [RemoteEvent("server.takeFromStock")]
        public static void TakeFromStock(ExtPlayer player, int slotIndex, int itemId)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                // Удаляем из склада (warehouse)
                Repository.RemoveIndex(player, "warehouse", slotIndex, 1);

                // Добавляем в инвентарь игрока
                Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", (ItemId)itemId, 1);

                // Повторно шлём обновлённый список склада
                Repository.LoadOtherItemsData(player, "warehouse", characterData.UUID.ToString(), 10);
            }
            catch (Exception e)
            {
                Log.Write($"TakeFromStock Exception: {e}");
            }
        }
    }


}

