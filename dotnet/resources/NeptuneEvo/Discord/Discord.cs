using Discord;
using Discord.WebSocket;
using GTANetworkAPI;
using NeptuneEvo.Core;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using NeptuneEvo.Players.Phone.Messages.Models;
using Localization;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using Color = Discord.Color;
using NeptuneEvo.Chars;
using NeptuneEvo.Handles;

namespace NeptuneEvo.Discord
{
    class Bot
    {
        public static DiscordSocketClient Client { get; private set; }
        private static nLog LogWrite = new nLog("BOT");

        public static string Token = "MTM2OTM4NzE3MjU5NTEwNTgwNA.Gh4526.E3zBzXg6SdubYwLNEHQ8s53lZY_q8dpuCLDG-w";
        public static ulong DiscordServerId = 720676827429077004;
        public static ulong FullLogsChannelId = 1304210751124279326;
        public static ulong NotificationsChannelId = 1369387852043124787;

        private static int CurrentOnline = 0;
        private static int PeakOnline = 0;

        private static Timer UpdateTimer;
        private static Timer DailyRewardTimer;

        public static List<ulong> OwnerIds { get; private set; } = new List<ulong>
        {
           305383445759000576,
           305383445759000576
        };

        // Список времени для уведомлений и выдачи
        private static readonly List<(TimeSpan NotificationTime, TimeSpan RewardTime)> RewardSchedule = new List<(TimeSpan, TimeSpan)>
        {
            (new TimeSpan(7, 32, 0), new TimeSpan(7, 33, 0)), // Уведомление за 30 минут до выдачи
            (new TimeSpan(18, 30, 0), new TimeSpan(19, 0, 0))  // Уведомление за 30 минут до выдачи
        };

        public static void Main()
        {
            new Bot().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            InitializeTimers();
            Client = new DiscordSocketClient();

            await Client.LoginAsync(TokenType.Bot, Token);  // Ошибка может быть здесь
            await Client.StartAsync();
            ScheduleDonationReward(); // Вызываем функцию, которая запускает таймер для уведомлений и выдачи
            LogWrite.Write("Bot started successfully.", nLog.Type.Success);
        }


        private void InitializeTimers()
        {
            // Таймер для обновления статистики
            UpdateTimer = new Timer(6000); // Интервал 1 минута
            UpdateTimer.Elapsed += UpdateOnlineStatistics;
            UpdateTimer.Start();

            // Таймер для выдачи доната
          
        }
        private async void UpdateOnlineStatistics(object sender, ElapsedEventArgs e)
        {
            // Обновляем информацию о текущем онлайн
            CurrentOnline = NAPI.Pools.GetAllPlayers().Count;
            if (CurrentOnline > PeakOnline)
                PeakOnline = CurrentOnline;

            // Обновляем статус бота в Discord
            string statusText = $"Majestics RolePlay | Онлайн: {CurrentOnline} | Пик: {PeakOnline}";
            await Client.SetGameAsync(statusText);  // Устанавливаем новый игровой статус бота
        }
        // Метод для запуска таймера
        private void ScheduleDonationReward()
        {
            DailyRewardTimer = new Timer(60000); // Проверка каждые 60 секунд
            DailyRewardTimer.Elapsed += (sender, e) =>
            {
                DateTime now = DateTime.Now;

                foreach (var (notificationTime, rewardTime) in RewardSchedule)
                {
                    // Уведомление за 30 минут до выдачи
                    if (now.TimeOfDay.Hours == notificationTime.Hours && now.TimeOfDay.Minutes == notificationTime.Minutes)
                    {
                        SendDiscordNotification(
                            "Напоминание о выдаче донатной валюты!",
                            "<@&1369387519715573854> Через 30 минут всем игрокам будет выдано **30 000 Majestic Coins**. Не пропустите!",
                            Color.Gold
                        );
                    }

                    // Выдача доната
                    if (now.TimeOfDay.Hours == rewardTime.Hours && now.TimeOfDay.Minutes == rewardTime.Minutes)
                    {
                        GiveRewardToAllPlayers();
                        SendDiscordNotification(
                            "Выдача донатной валюты!",
                            "<@&1369387519715573854> Всем игрокам успешно выдано **30 000 Majestic Coins**. Приятной игры!",
                            Color.Green
                        );
                    }
                }
            };
            DailyRewardTimer.Start();
        }



        private void SendDiscordNotification(string title, string description, Color color)
        {
            var guild = Client.GetGuild(DiscordServerId);
            if (guild != null)
            {
                var channel = guild.GetTextChannel(NotificationsChannelId);
                if (channel != null)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle(title)
                        .WithDescription(description)
                        .WithColor(color)
                        .WithFooter($"Сервер {guild.Name}")
                        .WithTimestamp(DateTimeOffset.Now);

                    channel.SendMessageAsync(embed: embed.Build());
                }
            }
        }

        private void GiveRewardToAllPlayers()
        {
            foreach (ExtPlayer foreachPlayer in NeptuneEvo.Character.Repository.GetPlayers())
            {
                if (!foreachPlayer.IsCharacterData()) continue;

                var foreachAccountData = foreachPlayer.GetAccountData();
                if (foreachAccountData == null) continue;

                UpdateData.RedBucks(foreachPlayer, 30000, msg: "Ежедневная выдача AC");
                Players.Phone.Messages.Repository.AddSystemMessage(
                    foreachPlayer,
                    (int)DefaultNumber.RedAge,
                    LangFunc.GetText(LangType.Ru, DataName.RbIncome, 30000, "Система"),
                    DateTime.Now
                );
            }
        }
    }
}
