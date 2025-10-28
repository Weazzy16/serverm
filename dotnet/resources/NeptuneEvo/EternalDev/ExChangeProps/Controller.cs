using Database;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.ExChangeProps.Classes;
using NeptuneEvo.EternalDev.ExChangeProps.Enums;
using NeptuneEvo.Handles;
using NeptuneEvo.Houses;
using NeptuneEvo.VehicleModel;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeptuneEvo.EternalDev.ExChangeProps
{
    public class Controller : Script
    {
        #region Author
        // По поводу заказа
        // @merumond

        // Эти и другие системы можно купить в дискорде https://discord.gg/aCkVrm9TRe

        // Created by

        // ███╗░░░███╗███████╗██████╗░██╗░░░██╗███╗░░░███╗░█████╗░███╗░░██╗██████╗░
        // ████╗░████║██╔════╝██╔══██╗██║░░░██║████╗░████║██╔══██╗████╗░██║██╔══██╗
        // ██╔████╔██║█████╗░░██████╔╝██║░░░██║██╔████╔██║██║░░██║██╔██╗██║██║░░██║
        // ██║╚██╔╝██║██╔══╝░░██╔══██╗██║░░░██║██║╚██╔╝██║██║░░██║██║╚████║██║░░██║
        // ██║░╚═╝░██║███████╗██║░░██║╚██████╔╝██║░╚═╝░██║╚█████╔╝██║░╚███║██████╔╝
        // ╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚═╝░╚═════╝░╚═╝░░░░░╚═╝░╚════╝░╚═╝░░╚══╝╚═════╝░
        #endregion

        private static readonly nLog Log = new nLog("ExChange");
        private static Dictionary<string, ExChangeData> _exChanges = new Dictionary<string, ExChangeData>();

        public static void ConfirmRequest(ExtPlayer target)
        {
            try
            {
                if (!target.HasData("exchange.owner")) return;
                ExtPlayer owner = target.GetData<ExtPlayer>("exchange.owner");
                if (owner is null) return;

                var targetData = target.GetCharacterData();
                if (targetData is null) return;

                var ownerData = owner.GetCharacterData();
                if (ownerData is null) return;

                var data = new ExChangeData()
                {
                    Owner = ownerData.UUID,
                    Target = targetData.UUID
                };
                var exchangeKey = $"{ownerData.UUID}_exchange_{targetData.UUID}";

                owner.SetData("exchange.key", exchangeKey);
                target.SetData("exchange.key", exchangeKey);

                _exChanges.TryAdd(exchangeKey, data);

                Trigger.ClientEvent(owner, "client.exChange.open", target.Name, true, JsonConvert.SerializeObject(GetProps(owner)));
                Trigger.ClientEvent(target, "client.exChange.open", owner.Name, false, JsonConvert.SerializeObject(GetProps(target)));
            }
            catch (Exception ex) { Log.Write("ConfirmRequest: " + ex.ToString()); }
        }

        public static void RefusalRequest(ExtPlayer target)
        {
            try
            {
                ExtPlayer owner = target.GetData<ExtPlayer>("exchange.owner");

                owner.ResetData("exchange.target");
                target.ResetData("exchange.owner");

                Notify.Send(owner, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок отказался от вашего предложения обмена...", 3000);
                Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, "Вы отказались от обмена", 3000);
            }
            catch (Exception ex) { Log.Write("RefusalRequest: " + ex.ToString()); }
        }


        public static Action<ExtPlayer> ChangeProps(ExtPlayer first, ExtPlayer second, PropertyType type, int index, int money)
        {
            try
            {
                var characterData = first.GetCharacterData();
                if (characterData is null) return null;

                MoneySystem.Wallet.Change(first, -money);
                MoneySystem.Wallet.Change(second, money);

                switch (type)
                {
                    case PropertyType.Vehicle:
                        var changeVehicleData = GetVehicles(first).ElementAt(index) as VehicleProperty;
                        
                        var vehicleData = VehicleManager.GetVehicleToNumber(changeVehicleData.Number);
                        return (player) =>
                        {
                            vehicleData.Holder = player.GetName();
                            VehicleManager.SaveHolder(vehicleData.Number);
                        };

                    case PropertyType.House:
                        var houseData = GetHouses(first).ElementAt(index) as HouseProperty;
                        var house = HouseManager.Houses.Find(x => x.ID == houseData.Id);

                        house.ClearOwner(isClearUpgraded: false, isSave: false);

                        return (player) =>
                        {
                            house.PetName = player.CharacterData.PetName;
                            house.SetOwner(player.GetName());

                            using (var db = new ServerBD("MainDB"))
                                house.Save(db).Wait();
                        };

                    case PropertyType.Business:
                        var businessData = GetBusinesses(first).ElementAt(index) as BusinessProprety;
                        characterData.BizIDs.Remove(businessData.Id);

                        var business = BusinessManager.BizList[businessData.Id];
                        return (player) =>
                        {
                            business.SetOwner(player.GetName());
                            player.CharacterData.BizIDs.Add(businessData.Id);

                            using (var db = new ServerBD("MainDB"))
                                business.Save(db).Wait();
                        };
                }

                return null;
            }
            catch (Exception ex) { Log.Write("ChangeProps: " + ex.ToString()); CancelExChange(first); return null; }
        }

        public static void ResetData(ExtPlayer player, ExtPlayer target, bool isCancel)
        {
            try
            {
                var key = player.GetData<string>("exchange.key");

                player.ResetData("exchange.target");
                player.ResetData("exchange.owner");
                player.ResetData("exchange.key");

                target.ResetData("exchange.target");
                target.ResetData("exchange.owner");
                target.ResetData("exchange.key");

                if (isCancel)
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Обмен отменен!", 3000);
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, "Обмен отменен!", 3000);
                }
                else
                {
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Обмен прошел успешно!", 3000);
                    Notify.Send(target, NotifyType.Success, NotifyPosition.BottomCenter, "Обмен прошел успешно!", 3000);
                }

                Trigger.ClientEvent(player, "client.exChange.close");
                Trigger.ClientEvent(target, "client.exChange.close");

                _exChanges.Remove(key);
            }
            catch (Exception ex) { Log.Write("ResetData: " + ex.ToString()); }
        }


        [RemoteEvent("server.exChange.changeData")]
        public static void UpdateExChange(ExtPlayer player, int type, int indexData, bool isReady, int surcharge)
        {
            try
            {
                if (!GetChangeData(player, out var changeData)) return;

                var playerData = player.GetCharacterData();
                if (playerData is null) return;

                var propertyType = (PropertyType)type;

                ExtPlayer target = null;
                if (player.HasData("exchange.owner")) // Target
                {
                    if (changeData.Target != playerData.UUID)
                    {
                        Log.Write($"Игрок ({player.SocialClubName} | {player.Name} | #{playerData.UUID}) попытался абузить обмен имуществом!!!");
                        player.Kick("Абуз трейдов");
                        return;
                    }

                    changeData.TargetPropType = propertyType;
                    changeData.TargetPropIndex = indexData;
                    changeData.TargetReady = isReady;
                    changeData.TargetSurcharge = surcharge;

                    target = player.GetData<ExtPlayer>("exchange.owner");
                }
                if (player.HasData("exchange.target")) // Owner
                {
                    if (changeData.Owner != playerData.UUID)
                    {
                        Log.Write($"Игрок ({player.SocialClubName} | {player.Name} | #{playerData.UUID}) попытался абузить обмен имуществом!!!");
                        player.Kick("Абуз трейдов");
                        return;
                    }

                    changeData.OwnerPropType = propertyType;
                    changeData.OwnerPropIndex = indexData;
                    changeData.OwnerReady = isReady;
                    changeData.OwnerSurcharge = surcharge;

                    target = player.GetData<ExtPlayer>("exchange.target");
                }

                if (target is null) return;

                Trigger.ClientEvent(target, "client.exChange.update", JsonConvert.SerializeObject(new
                {
                    PlayerName = player.GetName().Replace("_", " "),
                    Ready = isReady,

                    Data = GetDataInExchange(player, propertyType, indexData),

                    Type = type,
                    Surcharge = surcharge
                }));
            }
            catch (Exception ex) { Log.Write("UpdateExChange: " + ex.ToString()); }
        }

        [RemoteEvent("server.exChange.success")]
        public static void SuccesExChange(ExtPlayer owner)
        {
            try
            {
                if (!owner.HasData("exchange.target") || !GetChangeData(owner, out var changeData)) return;

                var playerData = owner.GetCharacterData();
                if (playerData is null) return;

                if (changeData.Owner != playerData.UUID) return;

                if (!changeData.OwnerReady || !changeData.TargetReady || changeData.OwnerPropType == PropertyType.None || changeData.TargetPropType == PropertyType.None)
                {
                    Notify.Send(owner, NotifyType.Info, NotifyPosition.BottomCenter, "Вы не выполнили все условия обмена!", 3000);
                    return;
                }

                ExtPlayer target = owner.GetData<ExtPlayer>("exchange.target");

                bool canOwnerMakeTrade = CanMakeExChange(owner, changeData.TargetPropType, changeData.OwnerPropType, changeData.TargetSurcharge);
                bool canTargetMakeTrade = CanMakeExChange(target, changeData.OwnerPropType, changeData.TargetPropType, changeData.OwnerSurcharge);

                if (!canOwnerMakeTrade || !canTargetMakeTrade)
                {
                    CancelExChange(owner);
                    return;
                }

                var ownerAction = ChangeProps(owner, target, changeData.OwnerPropType, changeData.OwnerPropIndex, changeData.OwnerSurcharge);
                var targetAction = ChangeProps(target, owner, changeData.TargetPropType, changeData.TargetPropIndex, changeData.TargetSurcharge);

                ownerAction?.Invoke(target);
                targetAction?.Invoke(owner);

                ResetData(owner, target, false);
            }
            catch (Exception ex) { Log.Write("SuccesExChange: " + ex.ToString()); CancelExChange(owner); }
        }

        [RemoteEvent("server.exChange.cancel")]
        public static void CancelExChange(ExtPlayer player)
        {
            try
            {
                if (!GetChangeData(player, out var changeData)) return;

                var playerData = player.GetCharacterData();
                if (playerData is null) return;

                ExtPlayer target = null;
                if (player.HasData("exchange.owner")) // Target
                {
                    target = player.GetData<ExtPlayer>("exchange.owner");
                }
                if (player.HasData("exchange.target")) // Owner
                {
                    target = player.GetData<ExtPlayer>("exchange.target");
                }

                ResetData(player, target, true);
            }
            catch (Exception ex) { Log.Write("CancelExChange: " + ex.ToString()); }
        }

        #region Getters functions
        public static bool CanMakeExChange(ExtPlayer player, PropertyType type, PropertyType exChangeType, int surcharge)
        {
            try
            {
                var playerData = player.GetCharacterData();
                if (playerData is null) return false;

                if (playerData.Money < surcharge)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас недостаточно средств для совершения обмена!", 3000);
                    return false;
                }

                if (exChangeType == type) 
                    return true;

                var house = HouseManager.GetHouse(player, true);

                switch (type)
                {
                    case PropertyType.Vehicle:
                        int maxCars = 1;

                        if (house != null)
                            maxCars += GarageManager.Garages.ContainsKey(house.GarageID) ? GarageManager.GarageTypes[GarageManager.Garages[house.GarageID].Type].MaxCars : 0;

                        if (VehicleManager.GetVehiclesCarCountToPlayer(player.Name.ToString()) >= maxCars)
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас максимальное количество машин", 3000);
                            return false;
                        }
                        break;
                    case PropertyType.House:
                        if (house != null)
                        {
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "У вас уже есть дом!", 3000);
                            return false;
                        }
                        break;
                    case PropertyType.Business:
                        if (playerData.BizIDs.Count >= Group.GroupMaxBusinesses[player.AccountData.VipLvl])
                        {
                            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете иметь больше {playerData.BizIDs.Count} бизнеса", 3000);
                            return false;
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex) { Log.Write("CanMakeExChange: " + ex.ToString()); return false; }
        }

        public static bool GetChangeData(ExtPlayer player, out ExChangeData data)
        {
            data = null;
            if (!player.HasData("exchange.key")) return false;

            string exchangeKey = player.GetData<string>("exchange.key");
            if (!_exChanges.TryGetValue(exchangeKey, out data)) return false;

            return true;
        }

        public static object GetDataInExchange(ExtPlayer player, PropertyType type, int index)
        {
            try
            {
                if (index < 0) return null;
                switch (type)
                {
                    case PropertyType.Vehicle:
                        var vehicles = GetVehicles(player);
                        if (vehicles is null || index > vehicles.Count - 1)
                            return null;

                        return vehicles.ElementAt(index);
                    case PropertyType.House:
                        var houses = GetHouses(player);
                        if (houses is null || index > houses.Count - 1)
                            return null;

                        return houses.ElementAt(index);
                    case PropertyType.Business:
                        var businesses = GetBusinesses(player);
                        if (businesses is null || index > businesses.Count - 1)
                            return null;

                        return businesses.ElementAt(index);
                    default: return null;
                }
            }
            catch (Exception ex) { Log.Write("GetDataInExchange: " + ex.ToString()); return null; }
        }

        public static object GetProps(ExtPlayer player)
        {
            try
            {
                return new
                {
                    Vehicles = GetVehicles(player),
                    Businesses = GetBusinesses(player),
                    Houses = GetHouses(player)
                };
            }
            catch (Exception ex) { Log.Write($"GetProps ({player.Name}): " + ex.ToString()); return null; }
        }

        private static List<object> GetBusinesses(ExtPlayer player)
        {
            try
            {
                List<object> dataBusinesses = new List<object>();
                var business = BusinessManager.BizList.FirstOrDefault(x => x.Value.Owner == player.Name).Value;
                if (business is null) return dataBusinesses;

                var data = new BusinessProprety()
                {
                    Name = BusinessManager.BusinessTypeNames[business.Type],
                    Id = business.ID,
                    Type = business.Type,
                    Price = business.SellPrice,
                };

                dataBusinesses.Add(data);

                return dataBusinesses;
            }
            catch (Exception ex) { Log.Write("GetBusinesses: " + ex.ToString()); return new List<object>(); }
        }

        private static List<object> GetHouses(ExtPlayer player)
        {
            try
            {
                List<object> dataHouses = new List<object>();
                House house = HouseManager.GetHouse(player, true);
                if (house is null || house.Owner != player.Name) return dataHouses;

                var data = new HouseProperty()
                {
                    Name = "Дом",
                    Id = house.ID,
                    Garage = GarageManager.Garages.ContainsKey(house.GarageID) ? GarageManager.GarageTypes[GarageManager.Garages[house.GarageID].Type].MaxCars : 0,
                    Price = house.Price,
                    TypeId = house.Type,
                    Type = HouseManager.HouseTypeList[house.Type].Name,
                };

                dataHouses.Add(data);

                return dataHouses;
            }
            catch (Exception ex) { Log.Write("GetHouses: " + ex.ToString()); return new List<object>(); }
        }

        private static List<object> GetVehicles(ExtPlayer player)
        {
            try
            {
                List<object> dataVehicles = new List<object>();
                foreach (var number in VehicleManager.GetVehiclesCarNumberToPlayer(player.Name))
                {
                    if (VehicleManager.Vehicles.ContainsKey(number))
                    {
                        var vehData = VehicleManager.Vehicles[number];
                        var data = new VehicleProperty()
                        {
                            Model = vehData.Model,
                            Name = vMain.GetName(NAPI.Util.GetHashKey(vehData.Model)),
                            Number = number,
                            Mile = 0,
                            Tun = new VehicleProperty.TuningData()
                            {
                                Engine = vehData.Components.Engine + 1,
                                Turbo = vehData.Components.Turbo + 1,
                                Suspension = vehData.Components.Suspension + 1,
                                Transmission = vehData.Components.Transmission + 1
                            }
                        };

                        dataVehicles.Add(data);
                    }
                };

                return dataVehicles;
            }
            catch (Exception ex) { Log.Write("GetVehicles: " + ex.ToString()); return new List<object>(); }
        }
        #endregion
    }
}
