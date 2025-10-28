using EternalCore;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;
using System;

namespace NeptuneEvo.EternalDev.Businesses.GasStation.Methods
{
    public class BuyProduct
    {
        public static void On(ExtPlayer player, string productName, string paymentType)
        {
            try
            {
                var characterData = player.GetCharacterData();
                var sessionData = player.GetSessionData();
                if (characterData is null || sessionData is null)
                    return;

                if (!BusinessManager.BizList.TryGetValue(sessionData.BizID, out var business))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы должны находится на заправочной станции", 3000);
                    return;
                }

                var productData = Controller.Config.Products.Find(x => x.Name == productName);
                if (productData is null)
                    return;

                var businessProductData = business.Products.Find(x => x.Name == productName);
                if (businessProductData is null)
                    return;

                int totalCost = businessProductData.Price;
                if ((paymentType == "Wallet" && characterData.Money < totalCost)
                    || (paymentType == "Bank" && MoneySystem.Bank.GetBalance(characterData.Bank) < totalCost))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Недостаточно средств!", 3000);
                    return;
                }

                if (Chars.Repository.isFreeSlots(player, productData.ItemId) != 0)
                    return;

                if (!BusinessManager.takeProd(business.ID, 1, productName, totalCost))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"На запровочной станции нет {productData.Name}", 3000);
                    return;
                }

                business.BuyItemBusiness(player.CharacterData.UUID, productName, totalCost);
                GameLog.Money($"player({player.CharacterData.UUID})", $"biz({business.ID})", totalCost, $"buyProduct({productData.ItemId})");

                if (paymentType == "Wallet")
                    MoneySystem.Wallet.Change(player, -totalCost);
                else
                    MoneySystem.Bank.Change(characterData.Bank, -totalCost);

                Chars.Repository.AddNewItem(player, $"char_{characterData.UUID}", "inventory", productData.ItemId, 1, "");

                EventSys.SendCoolMsg(player, "Покупка", "Покупка предмета", $"{LangFunc.GetText(LangType.Ru, DataName.YouBuy, productData.Name, businessProductData.Price)}", "", 6000);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"{LangFunc.GetText(LangType.Ru, DataName.YouBuy, productData.Name, businessProductData.Price)}", 1000);

                Controller.Close(player);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(On), ex, nameof(Controller)); }
        }
    }
}
