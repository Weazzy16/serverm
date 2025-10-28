using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.MarketPlace.Auction;
using NeptuneEvo.EternalDev.MarketPlace.Auction.Classes;
using NeptuneEvo.EternalDev.MarketPlace.Classes;
using NeptuneEvo.Fractions.LSNews.LiveStream;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.EternalDev.MarketPlace.Extensions
{
    public static class Businesses
    {
        public static bool CanBuyProperty(this Business business, ExtPlayer player = null)
        {
            return !IsOnMarketplace(business, player) && !IsInProfileStorage(business, player);
        }

        public static bool IsInProfileStorage(this Business business, ExtPlayer player = null)
        {
            var userProfileWithProperty = Manager.UserProfiles.Values.FirstOrDefault(x => x.Storage.Find(x => x.Type == Enums.LotType.Business && x.Data == business.ID.ToString()) != null);
            if (userProfileWithProperty != null)
            {
                if (player != null)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Этот бизнес хранится на складе маркетплейса у игрока #{userProfileWithProperty.UUID}", 3000);
                return true;
            }

            return false;
        }

        public static bool IsOnMarketplace(this Business business, ExtPlayer player = null)
        {
            bool Reject(string message)
            {
                if (player != null)
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, message, 3000);
                return true;
            }

            var marketItem = Manager.MarketItems.Values.ToList().Find(x => x.Type == Enums.LotType.Business && x.Data == business.ID.ToString());
            if (marketItem != null)
                return Reject($"Этот бизнес выставлен на маркетплейсе, взаимодействие с ним невозможно");

            var auctionItem = AuctionManager.AuctionItems.Values.ToList().Find(x => x.Type == Enums.LotType.Business && x.Data == business.ID.ToString());
            if (auctionItem != null)
                return Reject($"Этот бизнес выставлен на аукционе, взаимодействие с ним невозможно");

            return false;
        }
    }
}
