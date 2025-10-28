using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Core;
using NeptuneEvo.Fractions;
using NeptuneEvo.Fractions.Models;
using NeptuneEvo.Fractions.Player;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;
using System;
using System.Collections.Generic;
/*  
system by MIRO
*/
namespace NeptuneEvo.Jobs
{
    class Diver : Script
    {
        private static int checkpointPayment = 250000;
        private static nLog Log = new nLog("Diver");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                #region перс и блип
                 #endregion
                Main.CreateBlip(new Main.BlipData(780, "Работа Дайвер", new Vector3(1695.163, 42.85501, 160.6473), 77, true, 1.0f));
                CustomColShape.CreateCylinderColShape(new Vector3(1676.5415, 38.343056, 161.76869), 1, 2, 0, ColShapeEnums.Diver);
                PedSystem.Repository.CreateQuest("mp_m_boatstaff_01", new Vector3(1676.5415, 38.343056, 161.76869), -41.22963f, title: "~y~NPC~w~ Дайвер Миро");

                int i = 0;
                foreach (var Check in Checkpoints)
                {
                    CustomColShape.CreateCylinderColShape(Check.Position, 1, 2, 0, ColShapeEnums.DiverPoints, i);
                    i++;
                }

            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        [Interaction(ColShapeEnums.Diver)]
        public static void StartWorkDay(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            if (player.GetData<bool>("ON_WORK"))
            {
                Customization.ApplyCharacter(player);
                player.SetData("ON_WORK", false);
                Trigger.ClientEvent(player, "deleteCheckpoint", 15);
                Trigger.ClientEvent(player, "deleteWorkBlip");

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Уволил тебе с работы", 3000);
                return;
            }
            else
            {
               #region шмот
                #endregion
                //Customization.ClearClothes(player, player.CharacterData.Gender);
                if (player.CharacterData.Gender)
            {
                player.SetClothes(8, 151, 0); // Балон
                player.SetClothes(3, 17, 0); // Перчатки
                player.SetClothes(6, 67, 0); // Ласты
                player.SetClothes(11, 178, 0); // Куртка
                player.SetClothes(4, 77, 0); // Штаны
            }
            else
            {
                player.SetClothes(8, 187, 0); // Балон
                player.SetClothes(3, 18, 0); // Перчатки
                player.SetClothes(6, 70, 0); // Ласты
                player.SetClothes(11, 180, 0); // Куртка
                player.SetClothes(4, 79, 0); // Штаны
            }

                var check = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
                player.SetData("WORKCHECK", check);
                Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[check].Position, 1, 0, 255, 0, 0);
                Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[check].Position);

                player.SetData("ON_WORK", true);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Отметил тебе на карте точку иди туда и поработай там", 7000);

                return;
            }
        }

        [Interaction(ColShapeEnums.DiverPoints)]
        private static void PlayerEnterCheckpoint(ExtPlayer player, int value)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (!player.GetData<bool>("ON_WORK") || value != player.GetData<int>("WORKCHECK")) return;

                if (Checkpoints[value].Position.DistanceTo(player.Position) > 3) return;
              
                NAPI.Entity.SetEntityPosition(player, Checkpoints[value].Position + new Vector3(0, 0, 1.2));
                NAPI.Entity.SetEntityRotation(player, new Vector3(0, 0, Checkpoints[value].Heading));
               // Main.OnAntiAnim(player);
                player.SetData("WORKCHECK", -1);
                NAPI.Task.Run(() =>
                {
                    try
                    {
                        if (player != null)
                        {
                            player.StopAnimation();
                            Trigger.StopAnimation(player);
                            Main.OffAntiAnim(player);

                            var characterData = player.GetCharacterData();

                            
                            MoneySystem.Wallet.Change(player, checkpointPayment);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы подняли хабар и получи за него награду", 6000);

                            var nextCheck = WorkManager.rnd.Next(0, Checkpoints.Count - 1);

                            while (nextCheck == value) nextCheck = WorkManager.rnd.Next(0, Checkpoints.Count - 1);
                            player.SetData("WORKCHECK", nextCheck);
                            Trigger.ClientEvent(player, "createCheckpoint", 15, 1, Checkpoints[nextCheck].Position, 1, 0, 255, 0, 0);
                            Trigger.ClientEvent(player, "createWorkBlip", Checkpoints[nextCheck].Position);
                        }
                    }
                    catch { }
                }, 4000);

            }
            catch (Exception e) { Log.Write("PlayerEnterCheckpoint: " + e.Message, nLog.Type.Error); }
        }
        private static List<string> Objects = new List<string>()
        {
            "apa_mp_h_acc_bottle_01", // Собрать мусор 0
            "bkr_prop_clubhouse_laptop_01b", // Собрать мусор 1
            "bkr_prop_coke_boxeddoll", // Собрать мусор 2
            "prop_roadcone02b", // Собрать мусор 3
            "prop_mr_rasberryclean", // Собрать мусор 4
        };

        #region Чекпоинты
         #endregion
        private static List<Checkpoint> Checkpoints = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(1762.287, -19.40464, 154.4776), 206.6532),
            new Checkpoint(new Vector3(1857.945, 1.099715, 152.0033), 206.6532),
            new Checkpoint(new Vector3(1876.625, 104.593, 149.4533), 206.6532),
            new Checkpoint(new Vector3(1958.733, 112.7229, 152.9555), 206.6532),
            new Checkpoint(new Vector3(1971.705, 190.3279, 148.1627), 206.6532),
        };
        internal class Checkpoint
        {
            public Vector3 Position { get; }
            public double Heading { get; }

            public Checkpoint(Vector3 pos, double rot)
            {
                Position = pos;
                Heading = rot;
            }
        }
    }
}
