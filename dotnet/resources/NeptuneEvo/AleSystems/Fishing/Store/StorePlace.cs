using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Character.Models;
using NeptuneEvo.Chars;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Database.Models;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.AleSystems.Fishing.Data;
using NeptuneEvo.AleSystems.Fishing.Store.Classes;
using NeptuneEvo.Players.Popup.List.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing.Store
{
    public class StorePlace
    {

        private static readonly nLog Log = new nLog(nameof(StorePlace));
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public float Heading { get; set; }
        public uint BlipSprite { get; set; }
        public byte BlipColor { get; set; }
        public uint PedHash { get; set; }
        public Dictionary<string, List<StoreItem>> Items { get; set; } = new Dictionary<string, List<StoreItem>>();

        public StorePlace(string name, Vector3 position, float heading, uint blipSprite, byte blipColor, uint pedHash, Dictionary<string, List<StoreItem>> items)
        {
            Name = name;
            Position = position;
            Heading = heading;
            BlipSprite = blipSprite;
            BlipColor = blipColor;
            Items = items;
            PedHash = pedHash;
        }

        private Ped _ped { get; set; }
        private ExtColShape _colShape { get; set; }
        private Blip _blip { get; set; }
        public void GTAElements()
        {
            _ped = NAPI.Ped.CreatePed(PedHash, Position, Heading, false, true, true, false, 0);
            _colShape = CustomColShape.CreateCylinderColShape(Position, 2, 2, 0, ColShapeEnums.FishMarket, Config.STORES.IndexOf(this));
            _blip = NAPI.Blip.CreateBlip(BlipSprite, Position, .8f, BlipColor, Name, 255, 0, true, 0, 0);
        }

        public void UpdatePrices()
        {
            foreach(var item in Items)
            {
                item.Value.ForEach(storeItem =>
                {
                    if (storeItem.IsUpdated || storeItem.Price == 0)
                        storeItem.Price = Main.rnd.Next(storeItem.MinPrice, storeItem.MaxPrice);
                });
            }
        }

        public void Interaction(ExtPlayer player)
        {
            try
            {
                Trigger.ClientEvent(player, "client.fishing.store.open", JsonConvert.SerializeObject(Items));
            }
            catch(Exception ex) { Log.Write("Interaction: " + ex.ToString()); }
        } 

        public void Action(ExtPlayer player, string category, int index, int value)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                if (index < 0 || value <= 0 || !Items.TryGetValue(category, out var storeItems)) return;

                var item = storeItems.ElementAt(index);
                if (item is null) return;

                if (item.IsSell)
                {
                    int count = Chars.Repository.getCountToLacationItem($"char_{characterData.UUID}", "inventory", item.ItemId);
                    if (count < value)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет столько предмета!", 3000);
                        return;
                    }
                    for (int i = 0; i < value; i++)
                        Chars.Repository.Remove(player, $"char_{characterData.UUID}", "inventory", item.ItemId, 1);

                    int totalPrice = item.Price * value;
                    if (!item.IsDonate)
                        MoneySystem.Wallet.Change(player, totalPrice);
                    else
                        UpdateData.RedBucks(player, totalPrice, "Продажа предмета");

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы продали {item.Name} за {totalPrice}$", 3000);
                }
                else
                {
                    int totalPrice = item.Price * value; 

                    if (player.CharacterData.Money < totalPrice)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                        return;
                    }

                    if (Chars.Repository.AddNewItem(player, $"char_{player.CharacterData.UUID}", "inventory", item.ItemId, value) == -1)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, LangFunc.GetText(LangType.Ru, DataName.NoSpaceInventory), 3000);
                        return;
                    }

                    MoneySystem.Wallet.Change(player, -totalPrice);

                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы купили {item.Name} поздравляем!", 3000);
                }
            }
            catch (Exception ex) { Log.Write("Buy: " + ex.ToString()); }
        }

        public static void OnAction(ExtPlayer player, string category, int index, int value)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                var colShape = player.ColShapesData.Find(x => x.ColShapeId == ColShapeEnums.FishMarket);
                if (colShape is null) return;

                var point = Config.STORES.ElementAt(colShape.Index);
                if (point is null) return;

                point.Action(player, category, index, value);
            }
            catch(Exception ex) { Log.Write("OnBuy: " + ex.ToString()); }
        }

        [Interaction(ColShapeEnums.FishMarket)]
        public static void OnInteraction(ExtPlayer player, int index)
        {
            try
            {
                var point = Config.STORES.ElementAt(index);
                if (point is null) return;

                point.Interaction(player);
            }
            catch(Exception ex) { Log.Write("OnInteraction: " + ex.ToString()); }
        }
    }
}
