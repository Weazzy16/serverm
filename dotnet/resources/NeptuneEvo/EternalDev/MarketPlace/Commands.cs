using GTANetworkAPI;
using NeptuneEvo.EternalDev.MarketPlace.Enums;
using NeptuneEvo.EternalDev.MarketPlace.Extensions;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.EternalDev.MarketPlace
{
    public class Commands : Script
    {
        /// <summary>
        /// Уровень администратора для использования команды
        /// </summary>
        public static int AdminLvl = 10;

        [Command("removelotfromauction", Alias = "Используйте: /removelotfromauction [house | biz] [id]")]
        public static void RemoveLotFromAuction(ExtPlayer player, string propertyType, int id)
        {
            if (player.CharacterData.AdminLVL < AdminLvl)
                return;

            var lotType = propertyType == "house" ? LotType.House : propertyType == "biz" ? LotType.Business : LotType.None;
            if (lotType == LotType.None)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такого типа имущества не существует на аукционе!", 3000);
                return;
            }

            var auctionItem = Auction.AuctionManager.AuctionItems.Values.FirstOrDefault(x => x.Type == lotType && x.Data == id.ToString());
            if (auctionItem is null)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Имущества типа {propertyType} #{id} не выставлен на аукционе", 3000);
                return;
            }

            Auction.AuctionManager.DeleteAuctionItem(auctionItem);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно удалили имущество с аукциона!", 3000);
        }

        [Command("setauctiontime", Alias = "Используйте: /setauctiontime [house | biz] [id] [minutes]")]
        public static void SetAuctionTime(ExtPlayer player, string propertyType, int id, int minutes)
        {
            if (player.CharacterData.AdminLVL < AdminLvl)
                return;

            if (minutes < 1)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Минимум 1 минута", 3000);
                return;
            }

            var lotType = propertyType == "house" ? LotType.House : propertyType == "biz" ? LotType.Business : LotType.None;
            if (lotType == LotType.None)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Такого типа имущества не существует на аукционе!", 3000);
                return;
            }

            var auctionItem = Auction.AuctionManager.AuctionItems.Values.FirstOrDefault(x => x.Type == lotType && x.Data == id.ToString());
            if (auctionItem is null)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Имущества типа {propertyType} #{id} не выставлен на аукционе", 3000);
                return;
            }

            auctionItem.EndDate = DateTime.Now.AddMinutes(minutes);
            Manager.UpdatePage("auction");

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно установили время на {minutes} мин.", 3000);
        }

        [Command("addpropretytoauction", Alias = "Используйте: /addpropretytoauction [house | biz] [id]")]
        public static void AddPropertyToAuction(ExtPlayer player, string propertyType, int id)
        {
            if (player.CharacterData.AdminLVL < AdminLvl)
                return;

            switch (propertyType)
            {
                case "house":
                    var house = HouseManager.Houses.Find(x => x.ID == id);
                    if (house is null || !house.CanBuyProperty(player) || house.ID == 7) return;

                    if (house.Owner != string.Empty)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Кто-то владеет домом!", 3000);
                        return;
                    }

                    Auction.AuctionManager.SetPropertyToAuction(house);
                    break;
                case "biz":
                    if (!Core.BusinessManager.BizList.TryGetValue(id, out var business))
                        return;

                    if (!business.CanBuyProperty(player)) return;

                    if (business.IsOwner())
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Кто-то владеет этим бизнесом!", 3000);
                        return;
                    }

                    Auction.AuctionManager.SetPropertyToAuction(business);
                    break;
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно добавили имущество типа {propertyType} #{id} на аукцион!", 3000);
        }

        [Command("addallfreepropertytoauction", Alias = "Используйте: /addpropretytoauction [house | biz]")]
        public static void AddAllFreePropertyToAuction(ExtPlayer player, string propertyType)
        {
            if (player.CharacterData.AdminLVL < AdminLvl)
                return;

            switch (propertyType)
            {
                case "house":
                    var houses = HouseManager.Houses.Where(x => x.Type != 7 && x.Owner == string.Empty && !x.IsOnMarketplace()).ToList();
                    if (!houses.Any())
                        return;

                    houses.ForEach(house => Auction.AuctionManager.SetPropertyToAuction(house));
                    break;
                case "biz":
                    var businesses = Core.BusinessManager.BizList.Values.Where(x => !x.IsOwner() && !x.IsOnMarketplace()).ToList();
                    if (!businesses.Any())
                        return;

                    businesses.ForEach(business => Auction.AuctionManager.SetPropertyToAuction(business));
                    break;
            }

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно добавили все свободное имущество типа {propertyType} на аукцион!", 3000);
        }
    }
}
