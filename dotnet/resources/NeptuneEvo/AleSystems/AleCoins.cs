    using System;
    using GTANetworkAPI;
    using Redage.SDK;
    using NeptuneEvo.Players;
    using NeptuneEvo.Character;
    using NeptuneEvo.Handles;
    using GTANetworkMethods;
    using NeptuneEvo.Chars;
    using NeptuneEvo.Accounts;
using System.Linq;

    namespace NeptuneEvo.AleSystems
    {
        class AleCoins : Script
        {
            private static readonly nLog Log = new nLog(nameof(AleCoins));

            [ServerEvent(Event.ResourceStart)]
            public void onResourceStart()
            {
                try
                {
                    Log.Write("Coin system loaded");
                }
                catch (Exception e)
                {
                    Log.Write($"onResourceStart Exception: {e.ToString()}");
                }
            }

            public static void AleCoinsStart(ExtPlayer player)
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;

                if (characterData.ReceivedCoins == 1)
                {
                    Trigger.ClientEvent(player, "everyday.coins.start", "200", "00:00", 1);
                    return;
                }

                if (characterData.CoinsTime < 0) characterData.CoinsTime = 18000;
                sessionData.TimersData.CoinsTimer = Timers.Start(1000, () => AleCoins.CoinsTimer(player));

            
                var timeHour = characterData.CoinsTime / 3600;
                var remainingSeconds = characterData.CoinsTime % 3600;
                var timeMinute = remainingSeconds / 60;
                var timeSecond = remainingSeconds % 60;

            var coins = "200";
            string time;

            if (characterData.CoinsTime <= 60)
                time = "00:01";
            else
                time = $"{timeHour:D2}:{timeMinute:D2}";

            var received = 0;
            Trigger.ClientEvent(player, "everyday.coins.start", coins, time, received);

           
            }
        public static void AleCoinsStop(ExtPlayer player)
        {
            

            var accountData = player.GetAccountData();
            if (accountData == null) return;

            var characterData = player.GetCharacterData();
            if (characterData == null) return;

            var amount = 200;
            var time = "00:00";
            var received = 1;

            characterData.ReceivedCoins = 1;

            // 🔁 Обновляем в БД
            MySQL.Query($"UPDATE `characters` SET `ReceivedCoins` = 1 WHERE `uuid` = {characterData.UUID}");

            Trigger.ClientEvent(player, "everyday.coins.stop", time, received);
            


            if (accountData.RedBucks + amount < 0) amount = 0;

            UpdateData.RedBucks(player, amount, msg: "Отыгрыш 5 часов.");
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы отыграли 5 часов за день и получаете бонус в размере 200 Majestic Coins!", 3000);
        }

        public static void CoinsTimer(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                var timeHour = characterData.CoinsTime / 3600;
                var remainingSeconds = characterData.CoinsTime % 3600;
                var timeMinute = remainingSeconds / 60;
                var timeSecond = remainingSeconds % 60;

                string time;
                if (characterData.CoinsTime < 600)
                    time = $"0{timeHour}:0{timeMinute}";
                else if (characterData.CoinsTime > 3600 && characterData.CoinsTime < 4200)
                    time = $"0{timeHour}:0{timeMinute}";
                else if (characterData.CoinsTime > 7200 && characterData.CoinsTime < 7800)
                    time = $"0{timeHour}:0{timeMinute}";
                else if (characterData.CoinsTime > 10800 && characterData.CoinsTime < 11400)
                    time = $"0{timeHour}:0{timeMinute}";
                else if (characterData.CoinsTime > 14400 && characterData.CoinsTime < 15000)
                    time = $"0{timeHour}:0{timeMinute}";
                else
                    time = $"0{timeHour}:{timeMinute}";

                // Обновление в БД каждую минуту
                if (characterData.CoinsTime % 1 == 0)
                {
                    NAPI.Task.Run(() =>
                    {
                        MySQL.Query($"UPDATE `characters` SET `CoinsTime` = {characterData.CoinsTime} WHERE `uuid` = {characterData.UUID}");
                    });
                }

                Trigger.ClientEvent(player, "client::everyday.coins.update", time);

                // Уменьшаем и проверяем окончание таймера
                characterData.CoinsTime--;
                if (characterData.CoinsTime <= 0)
                    CoinsTimerEnd(player);
            }
            catch (Exception e)
            {
                Log.Write($"CoinsTimer Exception: {e}");
            }
        }



        public static void CoinsTimerEnd(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                var characterData = player.GetCharacterData();
                if (sessionData == null || characterData == null) return;

                // Остановим таймер
                if (sessionData.TimersData.CoinsTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.CoinsTimer);
                    sessionData.TimersData.CoinsTimer = null;
                }

                characterData.CoinsTime = 0;
                characterData.ReceivedCoins = 1;

                // ✅ ОБНОВЛЕНИЕ В БД
                MySQL.Query($"UPDATE `characters` SET `CoinsTime` = 0, `ReceivedCoins` = 1 WHERE `uuid` = {characterData.UUID}");

                // ✅ Отдаём бонус
                UpdateData.RedBucks(player, 200, msg: "Отыгрыш 5 часов.");
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы получили 200 Majestic Coins за отыгрыш 5 часов!", 5000);

                // ✅ ОТПРАВКА СОБЫТИЯ В HUD
                Trigger.ClientEvent(player, "everyday.coins.stop", "00:00", 1);
                
            }
            catch (Exception e)
            {
                Log.Write($"CoinsTimerEnd Exception: {e}");
            }
        }



        [ServerEvent(Event.ResourceStart)]
        public void StartDailyTimer()
        {
            ScheduleNextMidnightReset();
        }

        private void ScheduleNextMidnightReset()
        {
            var now = DateTime.Now;
            var midnight = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
            var timeToMidnight = (midnight - now).TotalMilliseconds;

            // ⛑ Защита: если таймер меньше 1 секунды — выставляем минимум 1 секунду
            if (timeToMidnight < 1000)
                timeToMidnight = 1000;

            NAPI.Task.Run(() =>
            {
                ResetDailyCoins();
                ScheduleNextMidnightReset(); // Перезапускает таймер на следующие сутки
            }, (long)timeToMidnight);
        }


        private void ResetDailyCoins()
        {
            foreach (ExtPlayer player in Character.Repository.GetPlayers())
            {
                var characterData = player.GetCharacterData();
                var sessionData = player.GetSessionData();
                if (characterData == null || sessionData == null) continue;

                characterData.ReceivedCoins = 0;
                characterData.CoinsTime = 18000;

                if (sessionData.TimersData.CoinsTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.CoinsTimer);
                    sessionData.TimersData.CoinsTimer = null;
                }

                NAPI.Task.Run(() =>
                {
                    AleCoinsStart(player);
                }, 1000);
            }
            Log.Write("Ежедневный сброс таймера монет выполнен.");
        }

        [Command("cisr")]
        public static void CMD_cisr(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.AdminLVL < 9)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет доступа.", 3000);
                    return;
                }

                foreach (ExtPlayer foreachPlayer in Character.Repository.GetPlayers())
                {
                    if (!foreachPlayer.IsCharacterData()) continue;

                    var foreachCharacterData = foreachPlayer.GetCharacterData();
                    if (foreachCharacterData == null) continue;

                    foreachCharacterData.ReceivedCoins = 0;
                    foreachCharacterData.CoinsTime = 18000;

                    // 🔁 Сохраняем изменения в БД
                    MySQL.Query($"UPDATE `characters` SET `ReceivedCoins` = 0, `CoinsTime` = 18000 WHERE `uuid` = {foreachCharacterData.UUID}");

                    NAPI.Task.Run(() =>
                    {
                        AleCoinsStart(foreachPlayer);
                    }, 1100);
                }

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Сброс монет успешно выполнен.", 3000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_cisr Exception: {e.ToString()}");
            }
        }

        [Command("rescoin")]
        public static void CMD_rescoin(ExtPlayer player, int targetId, int time)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData == null) return;

                if (characterData.AdminLVL < 1)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У вас нет доступа.", 3000);
                    return;
                }

                var target = Character.Repository.GetPlayers().FirstOrDefault(p => p.Value == targetId);

                if (target == null || !target.IsCharacterData())
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Игрок не найден.", 3000);
                    return;
                }

                var targetCharacterData = target.GetCharacterData();
                var sessionData = target.GetSessionData();
                if (targetCharacterData == null || sessionData == null)
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Ошибка данных игрока.", 3000);
                    return;
                }

                targetCharacterData.ReceivedCoins = 0;
                targetCharacterData.CoinsTime = time;

                // 💾 Сохраняем в БД
                MySQL.Query($"UPDATE `characters` SET `ReceivedCoins` = 0, `CoinsTime` = {time} WHERE `uuid` = {targetCharacterData.UUID}");

                if (sessionData.TimersData.CoinsTimer != null)
                {
                    Timers.Stop(sessionData.TimersData.CoinsTimer);
                    sessionData.TimersData.CoinsTimer = null;
                }

                NAPI.Task.Run(() =>
                {
                    AleCoinsStart(target);
                });

                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Таймер монет игроку {target.Name} установлен: {time} сек.", 3000);
                Notify.Send(target, NotifyType.Warning, NotifyPosition.BottomCenter, $"Админ сбросил таймер монет. Новый: {time} сек.", 5000);
            }
            catch (Exception e)
            {
                Log.Write($"CMD_rescoin Exception: {e.ToString()}");
            }
        }



    }
}
