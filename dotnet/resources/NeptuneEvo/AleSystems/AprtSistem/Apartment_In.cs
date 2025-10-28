using System;
using Localization;
using GTANetworkAPI;
using Redage.SDK;
using NeptuneEvo.Functions;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;

namespace NeptuneEvo.AleSystems
{
    class ApartmentOut : Script
    {
        private static readonly nLog Log = new nLog(nameof(ApartmentOut));

        public static Vector3[] ApartmentOuts = new Vector3[1]
        {
            new Vector3(-598.79114, 147.48155, 61.67285)
        };

        public static Vector3[] ApartmentOutPoints = new Vector3[1]
        {
            new Vector3(502.96225, 1298.2014, -186.62741)
        };

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
                Log.Write("Система апартаментов загружена " + "\u001B[33m" + "[by Miro Dev]" + "\u001B[0m");

                Main.CreateBlip(new Main.BlipData(475, "Апартаменты", ApartmentOuts[0], 47, true));
                CustomColShape.CreateCylinderColShape(ApartmentOuts[0], 1, 1, 0, ColShapeEnums.ApartmentOut, 1);
                NAPI.Marker.CreateMarker(1, ApartmentOuts[0] - new Vector3(0, 0, 1.05), new Vector3(), new Vector3(), 0.80f, new Color(237, 194, 21, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Апартаменты №1"), new Vector3(ApartmentOuts[0].X, ApartmentOuts[0].Y, ApartmentOuts[0].Z + 0.2), 5F, 0.5F, 0, new Color(255, 255, 255));
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }

        }
        [Interaction(ColShapeEnums.ApartmentOut)]
        public static void OpenApartments(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;

            var characterData = player.GetCharacterData();
            if (characterData == null) return;

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
           // if (Main.IHaveDemorgan(player, true)) return;

            //var house = Houses.HouseManager.GetHouse(player, true);
           // if (house != null)
            //{
            //    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже имеется дом.", 3000);
            //    return;
           // }

            switch (index)
            {
                    case 1:
                        NAPI.Entity.SetEntityPosition(player, ApartmentOutPoints[0]);
                        NAPI.Entity.SetEntityDimension(player, 0);
                        player.Dimension = 0;
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Система ещё на стадии разработки, могут быть баги!", 2500);
                        return;

                    default:
                        // Not supposed to end up here. 
                         Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Система ещё на стадии разработки, могут быть баги!", 2500);
                        break;
            }
        }
    }
}