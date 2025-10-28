using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using NeptuneEvo.Jobs.Models;
using NeptuneEvo.Players;
using NeptuneEvo.Quests.Models;
using Redage.SDK;

namespace NeptuneEvo.Quests
{    
    public enum zdobich_quests
    {
        Error = -99,
        NoMission = -1,
        Start = 0,
        Stage2 = 2,
        Stage3 = 3,
        Stage7 = 7,
        Stage8 = 8,
        Stage9 = 9,
        Stage10 = 10,
        Stage11 = 11,
        Stage12 = 12,//
        Stage13 = 13,//
        Stage14 = 14,//
        Stage15 = 15,//
        Stage16 = 16,//
        Stage17 = 17,//
        Stage18 = 18,//
        Stage19 = 19,//
        Stage20 = 20,
        Stage21 = 21,
        Stage23 = 23,
        Stage24 = 24,
        Stage25 = 25,
        Stage26 = 26,
        Stage28 = 28,
        Stage29 = 29,
        Stage30 = 30,
        Stage31 = 31,
        Stage33 = 33,
        Stage34 = 34,
        End = 35,
    };
    
    public class Zdobich : Script
    {
        private static readonly nLog Log = new nLog("Quests.Zdobich");
        public static string QuestName = "npc_zdobich";

        public static void Perform(ExtPlayer player, PlayerQuestModel playerQuestData)
        {
            try
            {
                if (!player.IsCharacterData()) return;

                var returnLine = Get(player, playerQuestData.Line);
                
                if (returnLine != zdobich_quests.Error)
                {
                    var questData = player.GetQuest();
                    if (questData != null)
                        questData.Line = (int)returnLine;
                    
                    qMain.UpdatePerform(player, QuestName, (short)returnLine);
                }
            }
            catch (Exception e)
            {
                Log.Write($"Perform Task.Run Exception: {e.ToString()}");
            }
        }
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            PedSystem.Repository.CreateQuest("csb_isldj_00", new Vector3(-1039.7443, -2731.1567, 20.214405), -157.1046f, questName: QuestName, title: "~y~NPC~w~ Виталий Дебич\nКвесты новичков", colShapeEnums: ColShapeEnums.QuestZdobich);
            
               
        }

        public static zdobich_quests Get(ExtPlayer player, int line, bool reward = false)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return zdobich_quests.Error;

            switch ((zdobich_quests)line)
            {
                case zdobich_quests.Stage2:
                    MoneySystem.Wallet.Change(player, 65000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage3;
                
                case zdobich_quests.Stage3:
                    return zdobich_quests.Stage7;
                
                case zdobich_quests.Stage7:
                    UpdateData.Exp(player, 100);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework2, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage8;
                
                case zdobich_quests.Stage8:
                    MoneySystem.Wallet.Change(player, 45000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case0, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage9;
                
                case zdobich_quests.Stage9:
                    UpdateData.Exp(player, 100);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bear, 1, addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Note, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage10;
                
                case zdobich_quests.Stage10:
                    MoneySystem.Wallet.Change(player, 48000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Rose, 1, addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework4, 1, addInWarehouse:true);
                    return zdobich_quests.Stage11;

                case zdobich_quests.Stage11:
                    MoneySystem.Wallet.Change(player, 15000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case2, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage20;
                
                case zdobich_quests.Stage20:
                    MoneySystem.Wallet.Change(player, 10000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bong, 1, "420", addInWarehouse:true); 
                    return zdobich_quests.Stage21;
                
                case zdobich_quests.Stage21:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.HealthKit, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage23;
                
                case zdobich_quests.Stage23:
                    MoneySystem.Wallet.Change(player, 10000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case1, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage24;
                
                case zdobich_quests.Stage24:
                    UpdateData.Exp(player, 1);
                    MoneySystem.Wallet.Change(player, 15000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework3, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage25;
                
                case zdobich_quests.Stage25:
                    MoneySystem.Wallet.Change(player, 5000000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Ball, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage26;
                
                case zdobich_quests.Stage26:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.BodyArmor, 1, "100", addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Bear, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage28;
                
                case zdobich_quests.Stage28:
                    MoneySystem.Wallet.Change(player, 3500000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Vape, 1, "100", addInWarehouse:true); 
                    return zdobich_quests.Stage29;
                
                case zdobich_quests.Stage29:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Guitar, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage30;
                
                case zdobich_quests.Stage30:
                    MoneySystem.Wallet.Change(player, 3500000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.LoveNote, 1, "1000", addInWarehouse:true); 
                    return zdobich_quests.Stage31;
                
                case zdobich_quests.Stage31:
                    UpdateData.Exp(player, 1);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Firework4, 3, addInWarehouse:true); 
                    return zdobich_quests.Stage33;
                
                case zdobich_quests.Stage33:
                    UpdateData.Exp(player, 1);
                    MoneySystem.Wallet.Change(player, 8500000);
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Case0, 1, addInWarehouse:true); 
                    Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Binoculars, 1, addInWarehouse:true); 
                    return zdobich_quests.Stage34;
                
                case zdobich_quests.Stage34:
                    return zdobich_quests.NoMission;
                /*case zdobich_quests.End:
                    //Награды
                    UpdateData.Exp(player, 10);
                    MoneySystem.Wallet.Change(player, 50000);
                    return zdobich_quests.NoMission;*/
            }
            return zdobich_quests.Error;
        }
        public static void Take(ExtPlayer player, int Line)
        {
            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            switch ((zdobich_quests)Line)
            {
                case zdobich_quests.Start:
                    if (characterData.Gender)
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Top, 1, "210_0_True"); 
                    else
                        Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", ItemId.Top, 1, "65_0_False"); 
    
                    qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Start, (int)zdobich_quests.Stage2);
                    break;
                case zdobich_quests.Stage3:
                    qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Stage3, (int)zdobich_quests.Stage7);
                    break;
                case zdobich_quests.Stage7:
                    if (characterData.Licenses[1])
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage7, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage9:
                    characterData.Handshaked++;
                    if (characterData.Handshaked >= 5)
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage9, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage9, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage24:
                    var house = HouseManager.GetHouse(player, checkOwner: true);
                    if (house != null)
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage24, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage24, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage25:
                    house = HouseManager.GetHouse(player, checkOwner: true);
                    if (house != null && (FurnitureManager.HouseFurnitures.ContainsKey(house.ID) || house.Type == 7))
                    {
                        if (house.Type != 7)
                        {
                            var houseFurnitureCount = FurnitureManager.HouseFurnitures[house.ID].Count;
                            if (houseFurnitureCount > 0)
                            {
                                Timers.StartOnce(50, () =>
                                {
                                    qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage25, 1,
                                        isUpdateHud: true);
                                    qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage25, true);
                                    OpenSuccess(player);
                                });
                            }
                        }
                        else
                        {
                            Timers.StartOnce(50, () =>
                            {
                                qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Stage25,
                                    (int)zdobich_quests.Stage26);
                                //qMain.UpdateQuestsStatus(player, QuestName, (int)zdobich_quests.Stage26, 1);
                                OpenSuccess(player);
                            });
                        }
                    }
                    break;
                case zdobich_quests.Stage26:
                    var vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                    if (vehiclesCount > 0)
                    {        
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsStage(player, QuestName, (int) zdobich_quests.Stage26, 1, isUpdateHud: true);
                            qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage26, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage31:
                    if (characterData.Licenses[7])
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage31, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
                case zdobich_quests.Stage33:
                    if (characterData.Licenses[6])
                    {
                        Timers.StartOnce(50, () =>
                        {
                            qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage33, true);
                            OpenSuccess(player); 
                        });
                    }
                    break;
            }
        }

        private static void OpenSuccess(ExtPlayer player)
        {
            var shapeData = CustomColShape.GetData(player, ColShapeEnums.QuestZdobich);

            if (shapeData != null)
                QuestZdobich(player, shapeData.Index, 1);
        }

        [Interaction(ColShapeEnums.QuestZdobich)]
        private static void Open(ExtPlayer player, int index)
        {
            QuestZdobich(player, index);
        }
        private static void QuestZdobich(ExtPlayer player, int index, int speed = 0)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) 
                return;
            
            var characterData = player.GetCharacterData();
            if (characterData == null) 
                return;
            
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
            
            if (Main.IHaveDemorgan(player, true)) return;
            bool isBool = qMain.SetQuests(player, QuestName, isInsert: true);
            if (!isBool) return;
            var questData = player.GetQuest();
            if (questData == null) 
                return;

            if (questData.Line == (int)zdobich_quests.Stage8 && !questData.Complete && Chars.Repository.isItem(player, "inventory", ItemId.GasCan) != null && Chars.Repository.isItem(player, "inventory", ItemId.Wrench) != null)
            {
                qMain.UpdateQuestsComplete(player, QuestName, (int)zdobich_quests.Stage8, true);
                questData.Complete = true;
            }
            
            if (questData.Line == (int)zdobich_quests.Stage26 && characterData.BizIDs.Count > 0)
            {
                questData.Line = (int)zdobich_quests.Stage28;
                
                qMain.UpdateQuestsLine(player, QuestName, (int)zdobich_quests.Stage26,
                    (int)zdobich_quests.Stage28);
                qMain.UpdateQuestsStatus(player, QuestName, (int)zdobich_quests.Stage28, 1);
            }

            if (questData.Line == (int) zdobich_quests.Stage33 && !questData.Complete && characterData.Licenses[6])
            {
                qMain.UpdateQuestsStage(player, QuestName, (int)zdobich_quests.Stage33, 2, isUpdateHud: true);
                qMain.UpdateQuestsComplete(player, QuestName, (int) zdobich_quests.Stage33, true);
                questData.Complete = true;
            }
            

            Trigger.ClientEvent(player, "client.quest.open", index, QuestName, questData.Line, questData.Status, questData.Complete, speed);
        }
    }
}