using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using Newtonsoft.Json;
using NeptuneEvo.EternalDev.Containers.Data;
using NeptuneEvo.Houses;
using Redage.SDK;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Character;
using NeptuneEvo.Chars;
using NeptuneEvo.Functions;
using NeptuneEvo.EternalDev.Containers.Methods;
using NeptuneEvo.Core;
using EternalCore;

namespace NeptuneEvo.EternalDev.Containers.Classes
{
    public class Container
    {
        public int Id { get; set; }
        public ContainerType ContainerType { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public int Price { get; set; }
        public int Step { get; set; }
        public int MaxBet { get; set; }
        public int NonRefDep { get; set; }

        public DateTime StartAuctionTime { get; set; }
        public DateTime FinishAuctionTime { get; set; }

        public List<BetData> Bets { get; set; } = new List<BetData>();
        public List<ExtPlayer> Subscribers { get; set; } = new List<ExtPlayer>();

        public bool IsDoorOpened { get; set; } = false;
        public bool IsAuctionStarted { get; set; } = false;

        public ContainerSettingsData Settings { get; set; }
        public PrizeData WinPrize { get; set; }

        [JsonIgnore]
        private ExtVehicle _vehicle { get; set; }

        [JsonIgnore]
        private GTANetworkAPI.Object _container { get; set; }
        [JsonIgnore]
        private GTANetworkAPI.Object _leftDoor { get; set; }
        [JsonIgnore]
        private GTANetworkAPI.Object _rightDoor { get; set; }
        [JsonIgnore]
        private TextLabel _textLabel { get; set; }

        private List<object> _cachedPrizes { get; set; } = new List<object>();

        public Container(int id, ContainerType containerType)
        {
            Id = id;
            ContainerType = containerType;

            if (Config.CONTAINER_SETTINGS.TryGetValue(containerType, out var containerSettings))
                Settings = containerSettings;

            Settings.Prizes.ForEach(x =>
                _cachedPrizes.Add(new
                {
                    x.Price,
                    x.Model,
                    x.Name,
                    x.IsDonate,
                    x.Rarity
                })
            );
        }

        public void GTAElements()
        {
            _container = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(Settings.ContainerModel), Position, Rotation, 255, 0);
            _leftDoor = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(Settings.LeftDoorModel), Position.GetPositionOffset(Rotation.Z - 90, 6.08) + new Vector3(-1.3, 0, 1.4), Rotation, 255, 0);
            _rightDoor = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(Settings.RightDoorModel), Position.GetPositionOffset(Rotation.Z - 90, 6.08) + new Vector3(1.3, 0, 1.4), Rotation, 255, 0);

            CustomColShape.CreateCylinderColShape(Position.GetPositionOffset(Rotation.Z - 90, 6.5), 3, 2, 0, ColShapeEnums.Container, Id);
            _textLabel = NAPI.TextLabel.CreateTextLabel(GetLabelText(), Position.GetPositionOffset(Rotation.Z - 90, 6.5) + new Vector3(0, 0, 1.7), 10f, 2f, 4, new Color(255, 255, 255, 150), false, 0);
        }

        #region Global functions
        public void StartAuction()
        {
            try
            {
                IsAuctionStarted = true;

                StartAuctionTime = DateTime.Now;
                FinishAuctionTime = DateTime.Now.AddMinutes(Config.CONTAINER_AUCTION_DURATION);
                Bets = new List<BetData>();

                Trigger.ClientEventForAll("client.containers.state", Id, true, GetCurrentPrice());
                UpdateContainerVariables();
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(StartAuction), ex, nameof(Container)); }
        }

        public void FinishAuction()
        {
            try
            {
                if (Bets.Count > 0)
                {
                    WinPrize = GeneratePrize();
                    CreateVehicle();
                    OpenContainerDoors();
                }

                Subscribers.ToList().ForEach(p 
                    => UnSubscribe(p));

                Trigger.ClientEventForAll("client.containers.state", Id, false, GetCurrentPrice());
                IsAuctionStarted = false;

                Manager.OnFinish();
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(FinishAuction), ex, nameof(Container)); }
        }
        #endregion

        #region Subscribers
        public void Subscribe(ExtPlayer player)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null || session.CurrentContainer != null)
                    return;

                if (!Subscribers.Contains(player))
                    Subscribers.Add(player);

                string jsonPrize = null;
                var winner = GetActiveBet();
                if (!IsAuctionStarted && winner != null)
                {
                    jsonPrize = JsonConvert.SerializeObject(new
                    {
                        WinPrize.Name,
                        WinPrize.Price,
                        WinPrize.Rarity,
                        WinPrize.Model,
                        WinPrize.IsDonate,
                        Time = (FinishAuctionTime.AddMinutes(Config.TIME_TO_CLAIM_PRIZE) - DateTime.Now).TotalSeconds
                    });
                }

                session.CurrentContainer = this;
                Trigger.ClientEvent(player, "client.containers.menu.open",
                    JsonConvert.SerializeObject(new
                    {
                        Id = Id,
                        Type = ContainerType.ToString(),
                        Name = Settings.Name,
                        Cost = Price,
                        NonRefDep = NonRefDep,
                        Step = Step,
                        MaxBet = MaxBet,
                        IsDonate = Settings.IsDonate,

                        Prizes = _cachedPrizes
                    }),

                    GetCurrentPrice(),
                    (FinishAuctionTime - DateTime.Now).TotalSeconds,
                    JsonConvert.SerializeObject(Bets),
                    jsonPrize
                );
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(Subscribe), ex, nameof(Container)); }
        } 

        public void UnSubscribe(ExtPlayer player)
        {
            try
            {
                var session = player.GetSessionData();
                if (session is null || session.CurrentContainer is null)
                    return;

                if (Subscribers.Contains(player))
                    Subscribers.Remove(player);

                session.CurrentContainer = null;
                Trigger.ClientEvent(player, "client.containers.menu.close");
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(UnSubscribe), ex, nameof(Container)); }
        }
        #endregion

        #region Winner actions
        public void PlaceBet(ExtPlayer player, int bet, string paymentType)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;

                if (!IsAuctionStarted)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Аукцион на контейнер завершён!", 3000);
                    return;
                }

                int minBet = GetMinBet();
                if (bet < minBet)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ставка не может быть меньше {minBet}{GetVault()}", 3000);
                    return;
                }

                if (bet > MaxBet)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ставка не может быть больше {MaxBet}{GetVault()}", 3000);
                    return;
                }

                int totalBet = bet + NonRefDep;

                var betData = new BetData(characterData.UUID, player.AccountData.Login, characterData.FirstName + "_" + characterData.LastName, bet);
                if (Settings.IsDonate)
                {
                    if (player.AccountData.RedBucks < totalBet)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно Majestic Coins", 3000);
                        return;
                    }
                }
                else
                {
                    if ((paymentType == "Wallet" && characterData.Money < totalBet) 
                        || (paymentType == "Card" && MoneySystem.Bank.GetBalance(characterData.Bank) < totalBet))
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Недостаточно средств", 3000);
                        return;
                    }
                }

                if (Bets.Count > 0)
                {
                    BetData lastBet = Bets.Last();
                    ExtPlayer target = betData.GetPlayer();
                    if (target != null)
                    {
                        if (Settings.IsDonate) UpdateData.RedBucks(player, lastBet.Price, $"Возврат ставки за контейнер #{Id}");
                        else MoneySystem.Wallet.Change(target, lastBet.Price);

                        if (lastBet.Uuid != betData.Uuid)
                            Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Ваша ставка на контейнер #{Id} была перебита", 3000);
                    }
                    else
                    {
                        if (Settings.IsDonate)
                            MySQL.Query($"UPDATE `accounts` SET `redbucks`+={lastBet.Price} WHERE `login` = '{lastBet.Login}'");
                        else
                            MySQL.Query($"UPDATE `characters` SET `money` = `money` + {lastBet.Price} WHERE `uuid`={lastBet.Uuid}");
                    }
                }

                if (Settings.IsDonate)
                {
                    UpdateData.RedBucks(player, -totalBet, $"Ставка за контейнер #{Id}");
                }
                else
                {
                    if (paymentType == "Wallet") MoneySystem.Wallet.Change(player, -totalBet);
                    else MoneySystem.Bank.Change(characterData.Bank, -totalBet);
                }

                if (DateTime.Now.AddSeconds(Config.TIME_RULE_TO_ADD_BY_BET) >= FinishAuctionTime)
                {
                    FinishAuctionTime = FinishAuctionTime.AddSeconds(Config.TIME_ADD_AUCTION_BY_BET);
                    UpdateMenuByType("waitSeconds", (FinishAuctionTime - DateTime.Now).TotalSeconds);

                    betData.UpTime = Config.TIME_ADD_AUCTION_BY_BET;
                }

                Bets.Add(betData);

                UpdateMenuByType("bets", JsonConvert.SerializeObject(Bets));
                UpdateMenuByType("currentBet", GetCurrentPrice());
                UpdateContainerVariables();

                Trigger.ClientEventForAll("client.containers.state", Id, true, GetCurrentPrice());
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(PlaceBet), ex, nameof(Container)); }
        }

        public void TakePrize(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null || IsAuctionStarted) return;

                var winner = GetActiveBet();
                if (winner is null || winner.Uuid != characterData.UUID || !IsDoorOpened) return;

                int vehiclesCount = VehicleManager.GetVehiclesCarCountToPlayer(player.Name);
                if (vehiclesCount >= GarageManager.MaxGarageCars)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас максимальное количество авто!", 6000);
                    return;
                }
                var house = HouseManager.GetHouse(player, true);
                if (house != null)
                {
                    var garage = house.GetGarageData();
                    if (garage == null || vehiclesCount >= GarageManager.GarageTypes[garage.Type].MaxCars)
                    {
                        Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас максимальное количество авто!", 6000);
                        return;
                    }
                }

                VehicleManager.Create(player, WinPrize.Model, new Color(225, 225, 225), new Color(225, 225, 225), Text: "Выигрыш в контейнере!", Logs: $"Container({WinPrize.Model}");

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Поздравляем, успешно забрали автомобиль модели {WinPrize.Name}. Автомобиль был доставлен к контейнеру.", 3000);
                UnSubscribe(player);
                CloseContainerDoors();
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(TakePrize), ex, nameof(Container)); }
        }

        public void SellPrize(ExtPlayer player, bool isTimer = false)
        {
            try
            {
                if (IsAuctionStarted) return;

                var winner = GetActiveBet();
                if (winner is null || !IsDoorOpened) return;

                if (player != null)
                {
                    var characterData = player.GetCharacterData();
                    if (characterData is null || winner.Uuid != characterData.UUID)
                        return;

                    if (WinPrize.IsDonate)
                    {
                        UpdateData.RedBucks(player, WinPrize.Price, $"Выигрыш в контейнере");
                    }
                    else
                    {
                        MoneySystem.Wallet.Change(player, WinPrize.Price);
                    }

                    if (isTimer)
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Время ожидания вышло, транспорт был продан государству.", 3000);
                    else
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно продали содержимое контейнера за: {string.Format("{0:N}", WinPrize.Price)}{GetVault()}", 3000);

                    UnSubscribe(player);
                }
                else
                {
                    if (WinPrize.IsDonate)
                        MySQL.Query($"UPDATE `accounts` SET `redbucks` += {WinPrize.Price} WHERE `login` = '{winner.Login}'");
                    else
                        MySQL.Query($"UPDATE `characters` SET `money` = `money` + {winner.Price} WHERE `uuid`={winner.Uuid}");
                }

                CloseContainerDoors();
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(SellPrize), ex, nameof(Container)); }
        }
        #endregion

        #region Event functions
        public void OnInteraction(ExtPlayer player)
        {
            try
            {
                var character = player.GetCharacterData();
                if (character is null) return;

                var winner = GetActiveBet();
                if ((!IsAuctionStarted && winner is null) || (!IsAuctionStarted && winner != null && character.UUID != winner.Uuid))
                {
                    Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Аукцион на контейнер завершён!", 3000);
                    return;
                }

                Subscribe(player);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnInteraction), ex, nameof(Container)); }
        }

        public void OnEverySecond()
        {
            try
            {
                if (FinishAuctionTime > DateTime.Now || !IsAuctionStarted)
                    return;

                FinishAuction();
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnEverySecond), ex, nameof(Container)); }
        }
        #endregion

        #region Win timer
        private string _winTimer { get; set; }
        public void OnEndWinTimer()
        {
            try
            {
                if (_winTimer != null)
                {
                    Timers.Stop(_winTimer);
                    _winTimer = null;
                }

                if (WinPrize != null)
                {
                    var winner = GetActiveBet();
                    if (winner != null) {
                        SellPrize(winner.GetPlayer(), true);
                    }
                }
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(OnEndWinTimer), ex, nameof(Container)); }
        }
        #endregion

        #region Doors
        private void OpenContainerDoors()
        {
            try
            {
                if (IsDoorOpened)
                    return;

                IsDoorOpened = true;
                int i = 0;

                NAPI.Task.Run(async () =>
                {
                    while (i < 120)
                    {
                        i++;
                        NAPI.Task.Run(() => _leftDoor.Rotation -= new Vector3(0, 0, 1));
                        NAPI.Task.Run(() => _rightDoor.Rotation -= new Vector3(0, 0, -1));

                        await Task.Delay(25);
                    }
                });

                var winner = GetActiveBet();
                if (winner != null)
                {
                    _winTimer = Timers.StartOnce(Config.TIME_TO_CLAIM_PRIZE * (1000 * 60), () => OnEndWinTimer());
                    var winPlayer = winner.GetPlayer();
                    if (winPlayer != null)
                        Notify.Send(winPlayer, NotifyType.Info, NotifyPosition.BottomCenter,
                            $"Вы победили в аукционе за контейнер #{Id}. У вас есть {Config.TIME_TO_CLAIM_PRIZE} минут, чтобы забрать приз, иначе он отправится будет продан", 7500);
                }
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OpenContainerDoors), ex, nameof(Container)); }
        }

        private void CloseContainerDoors()
        {
            try
            {
                if (!IsDoorOpened)
                    return;

                IsDoorOpened = false;

                NAPI.Task.Run(async () =>
                {
                    int count = 0;
                    while (count < 120)
                    {
                        count++;
                        NAPI.Task.Run(() => _leftDoor.Rotation += new Vector3(0, 0, 1));
                        NAPI.Task.Run(() => _rightDoor.Rotation += new Vector3(0, 0, -1));

                        await Task.Delay(25);
                    }

                    DestroyVehicle();
                });

                Bets.Clear();
                WinPrize = null;

                if (_winTimer != null)
                {
                    Timers.Stop(_winTimer);
                    _winTimer = null;
                }

                UpdateContainerVariables();
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(CloseContainerDoors), ex, nameof(Container)); }
        }
        #endregion

        #region Vehicle
        private void CreateVehicle()
        {
            NAPI.Task.Run(() =>
            {
                try
                {
                    _vehicle = NAPI.Vehicle.CreateVehicle((VehicleHash)NAPI.Util.GetHashKey(WinPrize.Model), Position, Rotation.Z + 180, 0, 0, "E-DEV", 255, true, false, 0) as ExtVehicle;
                    VehicleStreaming.SetLockStatus(_vehicle, true);
                    VehicleStreaming.SetEngineState(_vehicle, false);
                }
                catch (Exception ex) { ELib.Logger.Error(nameof(CreateVehicle), ex, nameof(Container)); }
            });
        }
        
        private void DestroyVehicle()
        {
            NAPI.Task.Run(() =>
            {
                if (_vehicle != null)
                    _vehicle.Delete();

                _vehicle = null;
            });
        }
        #endregion

        #region Updates
        private void UpdateContainerVariables()
            => NAPI.Task.Run(() => NAPI.TextLabel.SetTextLabelText(_textLabel, GetLabelText()));

        public void UpdateMenuByType(string type, dynamic data)
            => Trigger.ClientEventToPlayers(Subscribers.ToArray(), "client.containers.menu.update", type, data);
        #endregion

        #region Getters
        private string GetLabelText()
        {
            if (!IsAuctionStarted)
                return "Аукцион не проводится";
            else
                return $"Контейнер #{Id} \n Активная ставка: {string.Format("{0:N}", GetCurrentPrice())}{GetVault()}";
        }

        private PrizeData GeneratePrize()
        {
            int pool = 0;
            foreach (var prizeData in Settings.Prizes)
            {
                pool += prizeData.Chance;
            }

            int random = Main.rnd.Next(0, pool) + 1;
            int accumulatedProbability = 0;
            foreach (var prizeData in Settings.Prizes)
            {
                accumulatedProbability += prizeData.Chance;
                if (random <= accumulatedProbability)
                    return prizeData;
            }
            return null;
        }

        private BetData GetActiveBet()
            => Bets.Count > 0 ? Bets.Last() : null;

        public int GetCurrentPrice()
            => Bets.Count == 0 ? Price : GetActiveBet().Price;

        private int GetMinBet()
            => GetCurrentPrice() + Step;

        private string GetVault()
           => Settings.IsDonate ? "AS" : "$";
        #endregion
    }
}