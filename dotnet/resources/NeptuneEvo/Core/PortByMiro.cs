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
    class Port : Script
    {
        private static nLog Log = new nLog("Port");

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Main.CreateBlip(new Main.BlipData(371, "Работа Порт", new Vector3(15.441066, -2536.2468, 6.1464987), 77, true, 1.0f));
                CustomColShape.CreateCylinderColShape(new Vector3(15.441066, -2536.2468, 6.1464987), 1, 2, 0, ColShapeEnums.Port);
                PedSystem.Repository.CreateQuest("g_m_m_armgoon_01", new Vector3(15.441066, -2536.2468, 6.1464987), -132.9487f, title: "~y~NPC~w~ Анатолий Торетто ");

                int i = 0;
                foreach (var Check in Checkpoints)
                {
                    CustomColShape.CreateCylinderColShape(Check.Position, 1, 2, 0, ColShapeEnums.PortPoints, i);
                    i++;
                }

            }
            catch (Exception e) { Log.Write("ResourceStart: " + e.Message, nLog.Type.Error); }
        }

        [Interaction(ColShapeEnums.Port)]
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
                //Customization.ClearClothes(player, player.CharacterData.Gender);
                if (player.CharacterData.Gender)
                {
                    player.SetClothes(8, 59, 0);
                    player.SetClothes(11, 1, 0);
                    player.SetClothes(4, 0, 5);
                    player.SetClothes(6, 48, 0);
                }
                else
                {
                    player.SetClothes(8, 36, 0);
                    player.SetClothes(11, 0, 0);
                    player.SetClothes(4, 1, 5);
                    player.SetClothes(6, 49, 0);
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

        [Interaction(ColShapeEnums.PortPoints)]
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
                Main.OnAntiAnim(player);
                Trigger.PlayAnimation(player, "mp_intro_seq@", "mp_mech_fix", 39);
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

                            
                            MoneySystem.Wallet.Change(player, 500000);
                            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы успешно разложили груз и получили 500.000$!", 6000);

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

        private static List<Checkpoint> Checkpoints = new List<Checkpoint>()
        {
            new Checkpoint(new Vector3(6.575745, -2460.7227, 4.2753344), -108.74696),
            new Checkpoint(new Vector3(-164.61653, -2368.6968, 8.2753344), -108.74696),
            new Checkpoint(new Vector3(-101.38224, -2410.1575, 4.2753344), -108.74696),
            new Checkpoint(new Vector3(48.447826, -2484.17, 4.2753344), -108.74696),
            new Checkpoint(new Vector3(-157.10835, -2422.518, 4.2753344), -108.74696),
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
