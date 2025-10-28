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
    class ApartmentSystem : Script
    {
        private static readonly nLog Log = new nLog(nameof(ApartmentSystem));

        public static Vector3[] ApartmentsOut = new Vector3[1]
        {
            new Vector3(502.96225, 1298.2014, -186.62741)
        };

        public static Vector3[] ApartmentsPoints = new Vector3[1]
        {
            new Vector3(-598.79114, 147.48155, 61.67285)
        };

        [ServerEvent(Event.ResourceStart)]
        public void onResourceStart()
        {
            try
            {
           //     Log.Write("Система апартаментов загружена " + "\u001B[33m" + "[by Miro Dev]" + "\u001B[0m");

                CustomColShape.CreateCylinderColShape(ApartmentsOut[0], 1, 1, 0, ColShapeEnums.ApartmentsEnter, 1);
                NAPI.Marker.CreateMarker(1, ApartmentsOut[0] - new Vector3(0, 0, 1.05), new Vector3(), new Vector3(), 0.80f, new Color(237, 194, 21, 220));
                NAPI.TextLabel.CreateTextLabel(Main.StringToU16("Выход"), new Vector3(ApartmentsOut[0].X, ApartmentsOut[0].Y, ApartmentsOut[0].Z + 0.2), 5F, 0.5F, 0, new Color(255, 255, 255));
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }

        }
        [Interaction(ColShapeEnums.ApartmentsEnter)]
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
            if (Main.IHaveDemorgan(player, true)) return;

          //  var house = Houses.HouseManager.GetHouse(player, true);
          //  if (house != null)
          //  {
          //      Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У Вас уже имеется дом.", 3000);
          //      return;
          //  }

            switch (index)
            {
                    case 1:
                        NAPI.Entity.SetEntityPosition(player, ApartmentsPoints[0]);
                        NAPI.Entity.SetEntityDimension(player, 0);
                        player.Dimension = 0;
                        //Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Система ещё на стадии разработки", 2500);
                        return;
                    default:
                        // Not supposed to end up here. 
                        Notify.Send(player, NotifyType.Warning, NotifyPosition.BottomCenter, "Система ещё на стадии разработки", 2500);
                        break;
            }
        }
    }
}