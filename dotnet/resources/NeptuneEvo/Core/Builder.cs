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
namespace NeptuneEvo.Jobs
{
    public class Builder : Script
    {
        private static readonly nLog Log = new nLog(nameof(Builder));
        private static readonly int BuildColdayn = 2;
        private static readonly List<Vector3> BuilderPoints = new List<Vector3>()
        {
            new Vector3(x: 26.474945, y: -441.65454, z: 45.557747),
            new Vector3(x: 29.189466, y: -383.7328, z: 45.55739),
        };

        [ServerEvent(Event.ResourceStart)]
        public static void OnStart()
        {
            try
            {
                NAPI.Blip.CreateBlip(478, new Vector3(50.21503, -414.5201, 45.500652), 0.9f, 3, "Работа строителя", 255, 0, true, dimension: 0);
                CustomColShape.CreateCylinderColShape(new Vector3(51.299084, -418.8979, 39.921528), 1, 2, 0, ColShapeEnums.ProrabBuild);
                PedSystem.Repository.CreateQuest("g_m_m_armgoon_01", new Vector3(51.299084, -418.8979, 39.921528), -116.88297f, title: "~y~NPC~w~ Гермес Шнырёв\nРабота на стройке");
                CustomColShape.CreateCylinderColShape(new Vector3(52.668865, -414.37277, 39.921627), 1, 2, 0, ColShapeEnums.ProrabDress);
                PedSystem.Repository.CreateQuest("g_m_m_armgoon_01", new Vector3(52.668865, -414.37277, 39.921627), -110.649185f, title: "~y~NPC~w~ Mortal\nВыдает одежду");
                BuilderPoints.ForEach(position =>
                {
                    CustomColShape.CreateCylinderColShape(position, 5, 2, 0, ColShapeEnums.BuildPoint, BuilderPoints.IndexOf(position));
                    NAPI.Marker.CreateMarker(MarkerType.VerticalCylinder, position - new Vector3(0, 0, 3), new Vector3(), new Vector3(), 4f, new Color(255, 0, 0, 220), false, 0);
                });
            }
            catch (Exception ex) { Log.Write("OnStart: " + ex.ToString(), nLog.Type.Error); }
        }

        [Interaction(ColShapeEnums.ProrabBuild)]
        public static void StartWorkDay(ExtPlayer player)
        {

            if (player.GetData<bool>("ON_WORK"))
            {
                player.SetData("ON_WORK", false);
                Customization.ApplyCharacter(player);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Уволил тебя с работы.", 3000);
                return;
            }
            else
            {
                player.SetData("ON_WORK", true);
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Что-бы начать работу - подойди к Mortal он даст тебе одежду", 5000);

                return;
            }
        }

        [Interaction(ColShapeEnums.ProrabDress)]
        public static void ProabDress(ExtPlayer player)
        {

            if (player.GetData<bool>("ON_WORK"))
            {
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
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы одели форму. Что-бы начать работать идите по меткам", 5000);
            }
            else
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы не работаете строителем. Устроится можно у прораба", 5000);
                return;
            }
        }

        [Interaction(ColShapeEnums.BuildPoint)]
        public static void InteractionBuilder(ExtPlayer player, int index)
        {
            try
            {
                if (!player.GetData<bool>("ON_WORK")) return;
                
                if (player.HasData("Builder.lastPoint") && player.GetData<int>("Builder.lastPoint") == index)
                {
                    Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, $"Вы уже работали в этой точке!", 4000);
                    return;
                }

                player.SetData("Builder.timeout", DateTime.Now.AddSeconds(BuildColdayn));
                player.SetData("Builder.lastPoint", index);

                Trigger.ClientEvent(player, "client.buildermenu.start", "Builder.EndMinigame");
                player.SetData("Builder.isMinigame", true);

                Trigger.StopAnimation(player);

                Trigger.PlayAnimation(player, "amb@code_human_wander_eating_donut_fat@female@base", "car_side_attack_a", 1);
            }
            catch (Exception ex) { Log.Write("InteractionBuilder: " + ex.ToString()); }
        }

        [RemoteEvent("Builder.EndMinigame")]
        public static void BuilderEnd(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (!player.HasData("Builder.isMinigame") || !player.GetData<bool>("Builder.isMinigame")) return;
                player.SetData("Builder.isMinigame", false);

                Trigger.StopAnimation(player);
                MoneySystem.Wallet.Change(player, 500000);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы отработали эту точку и получили 500.000$. Идите на следующую", 5000);
            }
            catch (Exception ex) { Log.Write("BuilderEnd: " + ex.ToString(), nLog.Type.Error); }
        }
    }
}
