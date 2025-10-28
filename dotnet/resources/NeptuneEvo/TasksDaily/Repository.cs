using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;
using NeptuneEvo.Character;
using NeptuneEvo.Database; // MySQL.QueryRead / QueryAsync
using Redage.SDK;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars.Models;
using Ubiety.Dns.Core.Common;
using NeptuneEvo.Functions;
using NeptuneEvo.Accounts;
using NeptuneEvo.Chars;
using GTANetworkMethods;

namespace NeptuneEvo.CalendarTasks
{
    public class CalendarTaskData
    {
        public int TaskId { get; set; }
        public int CompletedCount { get; set; }

        public int RequirementCount { get; set; }
        public bool Completed { get; set; }
    }

    public static class Repository
    {
        // In‐memory cache: uuid → список прогресса
        private static readonly Dictionary<int, List<CalendarTaskData>> _cache
            = new Dictionary<int, List<CalendarTaskData>>();

        // Загружает из БД
        public static void LoadFromDb(int uuid)
        {
            var cmd = new MySqlCommand(@"
                SELECT `taskId`, `count` AS CompletedCount, `completed`
                FROM `calendar_tasks`
                WHERE `userId` = @uid;
            ");
            cmd.Parameters.AddWithValue("@uid", uuid);

            DataTable dt = MySQL.QueryRead(cmd);
            var list = new List<CalendarTaskData>();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new CalendarTaskData
                    {
                        TaskId = Convert.ToInt32(row["taskId"]),
                        CompletedCount = Convert.ToInt32(row["CompletedCount"]),
                        Completed = Convert.ToBoolean(row["completed"])
                    });
                }
            }

            _cache[uuid] = list;
        }

        // Получить (и при необходимости загрузить)
        public static List<CalendarTaskData> Get(int uuid)
        {
            if (!_cache.ContainsKey(uuid))
                LoadFromDb(uuid);
            return _cache[uuid];
        }

        // Формируем и шлём список двух текущих задач из определения + прогресс игрока
        public static void SendToClient(ExtPlayer player)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            // 1) Определяем текущий месяц/день (UTC)
            int month = DateTime.UtcNow.Month;  // 1–12
            int day = DateTime.UtcNow.Day;    // 1–31

            // 2) По этому [user, month, day] читаем из БД заранее сгенерированный список taskIds
            List<int> requiredTaskIds;
            {
                var cmd = new MySqlCommand(@"
            SELECT `taskIds`
            FROM `calendar_user_tasks`
            WHERE `userId` = @uid AND `month` = @m AND `day` = @d;
        ");
                cmd.Parameters.AddWithValue("@uid", cd.UUID);
                cmd.Parameters.AddWithValue("@m", month);
                cmd.Parameters.AddWithValue("@d", day);

                var dt = MySQL.QueryRead(cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    // уже есть — десериализуем
                    requiredTaskIds = JsonConvert
                        .DeserializeObject<List<int>>(Convert.ToString(dt.Rows[0]["taskIds"]));
                }
                else
                {
                    // ещё нет — берём 2 случайных таска из определения и сохраняем
                    var rnd = new Random(cd.UUID ^ day ^ month);
                    requiredTaskIds = CalendarTasksDefinition.All
                        .OrderBy(_ => rnd.Next())
                        .Take(2)
                        .Select(t => t.TaskId)
                        .ToList();

                    var ins = new MySqlCommand(@"
                REPLACE INTO `calendar_user_tasks`
                  (`userId`,`month`,`day`,`taskIds`)
                VALUES
                  (@uid,@m,@d,@ids);
            ");
                    ins.Parameters.AddWithValue("@uid", cd.UUID);
                    ins.Parameters.AddWithValue("@m", month);
                    ins.Parameters.AddWithValue("@d", day);
                    ins.Parameters.AddWithValue("@ids", JsonConvert.SerializeObject(requiredTaskIds));
                    _ = MySQL.QueryAsync(ins);
                }
            }

            // 3) Достаём определения тасков из All
            var taskDefs = CalendarTasksDefinition.All
                .Where(t => requiredTaskIds.Contains(t.TaskId))
                .ToList();

            // 4) Прогресс игрока по таскам
            var progList = Get(cd.UUID); // List<CalendarTaskData>

            // 5) Собираем payload
            var payload = new List<object>(taskDefs.Count);
            foreach (var def in taskDefs)
            {
                var prog = progList.FirstOrDefault(p => p.TaskId == def.TaskId);
                int doneCount = prog?.CompletedCount ?? 0;

                // НИКОГДА не даём free-take, если у таска RequirementCount == 0
                bool canTake = def.RequirementCount > 0 && doneCount >= def.RequirementCount;
                // после того, как вы посчитали doneCount и canTake
                Console.WriteLine($"[CalendarTasks] Task {def.TaskId}: done={doneCount}/{def.RequirementCount}, canTake={canTake}");

                payload.Add(new
                {
                    taskId = def.TaskId,
                    botId = def.BotId,
                    botName = def.BotName,
                    title = def.Title,
                    description = def.Description,
                    questType = def.QuestType,
                    level = def.Level,
                    requirement = new
                    {
                        title = def.RequirementTitle,
                        count = def.RequirementCount
                    },
                    progress = doneCount,
                    remaining = Math.Max(0, def.RequirementCount - doneCount),
                    canTake
                });
            }

            // 6) Сериализуем и отправляем во вьюху
            string json = JsonConvert.SerializeObject(payload);
            Trigger.ClientEvent(player, "calendar_tasks_update", json);
        }


        /// <summary>
        /// Формирует для календаря список дней месяца.
        /// </summary>
        public static void SendMonth(ExtPlayer player, int month)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            // 1) get the static list of rewards for this month
            if (!CalendarTasksDefinition.Monthly.TryGetValue(month, out var staticRewards))
                staticRewards = new List<CalendarRewardInfo>();

            int today = DateTime.UtcNow.Day;
            var progList = Get(cd.UUID); // all CalendarTaskData for this player this session

            // 2) load bought days
            var bought = new HashSet<int>();
            {
                var cmd = new MySqlCommand(@"
            SELECT `day`
            FROM `calendar_days_bought`
            WHERE `userId` = @uid AND `month` = @m;
        ");
                cmd.Parameters.AddWithValue("@uid", cd.UUID);
                cmd.Parameters.AddWithValue("@m", month);
                var dt = MySQL.QueryRead(cmd);
                if (dt != null)
                    foreach (DataRow r in dt.Rows)
                        bought.Add(Convert.ToInt32(r["day"]));
            }

            // 3) load already claimed rewards
            var claimed = new HashSet<int>();
            {
                var cmd = new MySqlCommand(@"
            SELECT `day`
            FROM `calendar_rewards_claimed`
            WHERE `userId` = @uid AND `month` = @m;
        ");
                cmd.Parameters.AddWithValue("@uid", cd.UUID);
                cmd.Parameters.AddWithValue("@m", month);
                var dt = MySQL.QueryRead(cmd);
                if (dt != null)
                    foreach (DataRow r in dt.Rows)
                        claimed.Add(Convert.ToInt32(r["day"]));
            }

            // 4) prepare per-user required tasks for every day
            //    cache them in a dictionary day->List<int>
            var userTasks = new Dictionary<int, List<int>>();
            {
                var cmd = new MySqlCommand(@"
            SELECT `day`,`taskIds`
            FROM `calendar_user_tasks`
            WHERE `userId`=@uid AND `month`=@m;
        ");
                cmd.Parameters.AddWithValue("@uid", cd.UUID);
                cmd.Parameters.AddWithValue("@m", month);
                var dt = MySQL.QueryRead(cmd);

                // deserialize existing
                if (dt != null)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        int d = Convert.ToInt32(r["day"]);
                        var ids = JsonConvert
                          .DeserializeObject<List<int>>(Convert.ToString(r["taskIds"]));
                        userTasks[d] = ids;
                    }
                }

                // for any staticReward day that isn't in userTasks, generate + save
                var rnd = new Random();
                foreach (var info in staticRewards)
                {
                    int d = info.DayInMonth;
                    if (!userTasks.ContainsKey(d))
                    {
                        // pick 2 random from All
                        var picks = CalendarTasksDefinition.All
                            .OrderBy(_ => rnd.Next())
                            .Take(2)
                            .Select(t => t.TaskId)
                            .ToList();
                        userTasks[d] = picks;

                        // persist
                        var ins = new MySqlCommand(@"
                    REPLACE INTO `calendar_user_tasks`
                      (`userId`,`month`,`day`,`taskIds`)
                    VALUES (@uid,@m,@d,@ids);
                ");
                        ins.Parameters.AddWithValue("@uid", cd.UUID);
                        ins.Parameters.AddWithValue("@m", month);
                        ins.Parameters.AddWithValue("@d", d);
                        ins.Parameters.AddWithValue("@ids", JsonConvert.SerializeObject(picks));
                        _ = MySQL.QueryAsync(ins);
                    }
                }
            }

            // 5) assemble payload
            var payload = new List<object>(staticRewards.Count);
            foreach (var info in staticRewards)
            {
                int d = info.DayInMonth;

                // use per-user tasks, not the static info.RequiredTasks
                var required = userTasks.TryGetValue(d, out var list) ? list : new List<int>();
                int doneCount = progList.Count(p => required.Contains(p.TaskId) && p.Completed);
                bool allDone = required.Count > 0 && doneCount >= required.Count;
                bool isBought = bought.Contains(d);
                bool isClaimed = claimed.Contains(d);

                string status;
                bool canTake;

                if (isClaimed)
                {
                    status = "received";
                    canTake = false;
                }
                else if ((allDone || isBought) && d <= today)
                {
                    status = "available";
                    canTake = true;
                }
                else if (d < today && !allDone && !isBought)
                {
                    status = "skipped";
                    canTake = false;
                }
                else
                {
                    status = "locked";
                    canTake = false;
                }

                payload.Add(new
                {
                    month = info.Month,
                    day = d,
                    title = info.Title,
                    rewardType = info.RewardType.ToString(),
                    rewardCount = info.RewardCount,
                    rewardItemId = info.RewardItemId,
                    picturePath = info.RewardItemId > 0
                                  ? $"https://cdn.majestic-files.com/.../items/{info.RewardItemId}.png"
                                  : "",
                    progress = doneCount,
                    remaining = Math.Max(0, required.Count - doneCount),
                    canTake = canTake,
                    status = status,
                    price = DayBuyPrice
                });
            }

            // 6) fire it off
            Trigger.ClientEvent(player, "item_calendar_update", JsonConvert.SerializeObject(payload));
        }



        // Прибавляем к прогрессу
        public static void AddProgress(ExtPlayer player, int taskId, int amount = 1)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            var list = Get(cd.UUID);
            var entry = list.FirstOrDefault(x => x.TaskId == taskId);
            if (entry == null)
            {
                entry = new CalendarTaskData { TaskId = taskId };
                list.Add(entry);
            }
            if (entry.Completed) return;

            entry.CompletedCount += amount;
            if (entry.CompletedCount >=
                CalendarTasksDefinition
                    .All.First(d => d.TaskId == taskId)
                    .RequirementCount)
            {
                entry.Completed = true;
            }

            // Сохранение в БД
            Trigger.SetTask(async () =>
            {
                // Проверяем наличие
                var check = new MySqlCommand(@"
                    SELECT COUNT(*) AS cnt
                    FROM `calendar_tasks`
                    WHERE `userId` = @uid AND `taskId` = @tid;
                ");
                check.Parameters.AddWithValue("@uid", cd.UUID);
                check.Parameters.AddWithValue("@tid", taskId);
                var dtc = MySQL.QueryRead(check);
                bool exists = dtc != null
                    && dtc.Rows.Count > 0
                    && Convert.ToInt32(dtc.Rows[0]["cnt"]) > 0;

                if (exists)
                {
                    var upd = new MySqlCommand(@"
                        UPDATE `calendar_tasks`
                        SET `count` = @cnt, `completed` = @cmp
                        WHERE `userId` = @uid AND `taskId` = @tid;
                    ");
                    upd.Parameters.AddWithValue("@cnt", entry.CompletedCount);
                    upd.Parameters.AddWithValue("@cmp", entry.Completed);
                    upd.Parameters.AddWithValue("@uid", cd.UUID);
                    upd.Parameters.AddWithValue("@tid", taskId);
                    await MySQL.QueryAsync(upd);
                }
                else
                {
                    var ins = new MySqlCommand(@"
                        INSERT INTO `calendar_tasks`
                          (`userId`,`taskId`,`count`,`completed`)
                        VALUES
                          (@uid,@tid,@cnt,@cmp);
                    ");
                    ins.Parameters.AddWithValue("@uid", cd.UUID);
                    ins.Parameters.AddWithValue("@tid", taskId);
                    ins.Parameters.AddWithValue("@cnt", entry.CompletedCount);
                    ins.Parameters.AddWithValue("@cmp", entry.Completed);
                    await MySQL.QueryAsync(ins);
                }
            });

            // Обновляем UI
            SendToClient(player);
        }

        // Взять награду
        
        private const int DayBuyPrice = 300;
        public static void BuyDay(ExtPlayer player, int month, int day)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            // проверяем, что месяц — текущий
            if (DateTime.UtcNow.Month != month)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Нельзя покупать дни за прошлые/будущие месяцы", 3000);
                return;
            }

            // проверяем наличие записи в расписании
            if (!CalendarTasksDefinition.Monthly.TryGetValue(month, out var list)) return;
            var info = list.FirstOrDefault(r => r.DayInMonth == day);
            if (info == null) return;

            // проверяем, что пользователь ещё не купил этот день
            // (таблица calendar_day_purchased(userId, month, day))
            var check = new MySqlCommand(@"
            SELECT COUNT(*) 
            FROM `calendar_day_purchased`
            WHERE `userId` = @uid AND `month` = @m AND `day` = @d;
        ");
            check.Parameters.AddWithValue("@uid", cd.UUID);
            check.Parameters.AddWithValue("@m", month);
            check.Parameters.AddWithValue("@d", day);
            var dt = MySQL.QueryRead(check);
            var already = dt != null && Convert.ToInt32(dt.Rows[0][0]) > 0;
            if (already)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Вы уже разблокировали этот день", 3000);
                return;
            }

            // проверяем баланс
            var acc = player.GetAccountData();
            if (acc.RedBucks < DayBuyPrice)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Недостаточно койнов", 3000);
                return;
            }

            // списываем
            UpdateData.RedBucks(player, -DayBuyPrice, msg: $"CalendarBuyDay({month},{day})");
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                        $"День {day} разблокирован за {DayBuyPrice} койнов", 3000);

            // помечаем в БД как купленный
            Trigger.SetTask(async () =>
            {
                var ins = new MySqlCommand(@"
                INSERT INTO `calendar_day_purchased`
                  (`userId`,`month`,`day`,`purchasedAt`)
                VALUES
                  (@uid,@m,@d,UTC_TIMESTAMP());
            ");
                ins.Parameters.AddWithValue("@uid", cd.UUID);
                ins.Parameters.AddWithValue("@m", month);
                ins.Parameters.AddWithValue("@d", day);
                await MySQL.QueryAsync(ins);
            });

            // и пересылаем клиенту обновлённый список
            SendMonth(player, month);
        }
        public static void TakeMonthReward(ExtPlayer player, int month, int day)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            // 1) Find today's calendar reward
            if (!CalendarTasksDefinition.Monthly.TryGetValue(month, out var list)) return;
            var info = list.FirstOrDefault(r => r.DayInMonth == day);
            if (info == null) return;

            // 2) Verify all required tasks are completed
            var prog = Get(cd.UUID);
            bool ok = info.RequiredTasks.All(tid => prog.Any(d => d.TaskId == tid && d.Completed));
            if (!ok) return;

            // 3) Reset in-memory progress
            foreach (var tid in info.RequiredTasks)
            {
                var e = prog.FirstOrDefault(d => d.TaskId == tid);
                if (e != null)
                {
                    e.Completed = false;
                    e.CompletedCount = 0;
                }
            }

            // 4) Reset progress in the database
            Trigger.SetTask(async () =>
            {
                foreach (var tid in info.RequiredTasks)
                {
                    var reset = new MySqlCommand(@"
                UPDATE `calendar_tasks`
                SET `count` = 0, `completed` = 0
                WHERE `userId` = @uid AND `taskId` = @tid;
            ");
                    reset.Parameters.AddWithValue("@uid", cd.UUID);
                    reset.Parameters.AddWithValue("@tid", tid);
                    await MySQL.QueryAsync(reset);
                    // reset is eligible for GC after this line
                }
            });

            // 5) Give the reward
            switch (info.RewardType)
            {
                case CalendarRewardType.Money:
                    MoneySystem.Wallet.Change(player, info.RewardCount);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                                $"Вы получили {info.RewardCount}$ за календарь!", 3000);
                    break;
                case CalendarRewardType.Item:
                    Chars.Repository.AddNewItemWarehouse(player, (ItemId)info.RewardItemId, info.RewardCount);
                    Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                                "Вы получили предмет за календарь!", 3000);
                    break;
                case CalendarRewardType.Vehicle:
                    // vehicle reward logic…
                    break;
            }

            // 6) Mark this reward as claimed (NO using here!)
            var ins = new MySqlCommand(@"
        INSERT IGNORE INTO `calendar_rewards_claimed`
            (`userId`,`month`,`day`,`claimedAt`)
        VALUES
            (@uid,@m,@d,UTC_TIMESTAMP());
    ");
            ins.Parameters.AddWithValue("@uid", cd.UUID);
            ins.Parameters.AddWithValue("@m", month);
            ins.Parameters.AddWithValue("@d", day);
            _ = MySQL.QueryAsync(ins);
            // ins will be cleaned up by GC after use

            // 7) Refresh CEF
            SendMonth(player, month);
        }


    }

    public class CalendarTasksHandler : Script
    {

        [RemoteEvent("server:calendar:request")]
        public void OnClientRequest(ExtPlayer player)
        {
            Repository.SendToClient(player);
        }

        [RemoteEvent("server.calendar.takeMonthReward")]
        public void OnClientTakeMonthReward(ExtPlayer player, int month, int day)
        {
            Repository.TakeMonthReward(player, month, day);
        }


        [RemoteEvent("server.calendar.buyDay")]
        public void OnClientBuyDay(ExtPlayer player, int month, int day)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            // разрешаем только на текущий месяц
            if (DateTime.UtcNow.Month != month)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Нельзя покупать дни не текущего месяца", 3000);
                return;
            }

            const int PRICE = 250;
            // проверяем и списываем койны
            var acc = player.GetAccountData();
            if (acc == null || acc.RedBucks < PRICE)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Недостаточно койнов", 3000);
                return;
            }
            // здесь гарантированно снимутся койны и появится запись в логе
            UpdateData.RedBucks(player, -PRICE, msg: $"CalendarBuyDay({month}-{day})");

            // находим описание календарного дня
            if (!CalendarTasksDefinition.Monthly.TryGetValue(month, out var list)) return;
            var info = list.FirstOrDefault(r => r.DayInMonth == day);
            if (info == null) return;

            // помечаем таски как выполненные в памяти и БД
            var progList = Repository.Get(cd.UUID);
            foreach (var tid in info.RequiredTasks)
            {
                var e = progList.FirstOrDefault(x => x.TaskId == tid);
                if (e == null)
                {
                    e = new CalendarTaskData
                    {
                        TaskId = tid,
                        Completed = true,
                        CompletedCount = info.RequiredTasks.Count
                    };
                    progList.Add(e);
                }
                else
                {
                    e.Completed = true;
                    e.CompletedCount = info.RequiredTasks.Count;
                }

                Trigger.SetTask(async () =>
                {
                    var cmd = new MySqlCommand(@"
INSERT INTO `calendar_tasks` (`userId`,`taskId`,`count`,`completed`)
VALUES (@uid,@tid,@cnt,1)
ON DUPLICATE KEY UPDATE `count`=@cnt, `completed`=1;
");
                    cmd.Parameters.AddWithValue("@uid", cd.UUID);
                    cmd.Parameters.AddWithValue("@tid", tid);
                    cmd.Parameters.AddWithValue("@cnt", info.RequiredTasks.Count);
                    await MySQL.QueryAsync(cmd);
                });
            }

            // сохраняем факт покупки дня
            Trigger.SetTask(async () =>
            {
                var ins = new MySqlCommand(@"
INSERT IGNORE INTO `calendar_days_bought`
    (`userId`,`month`,`day`,`boughtAt`)
VALUES
    (@uid,@m,@d,UTC_TIMESTAMP());
");
                ins.Parameters.AddWithValue("@uid", cd.UUID);
                ins.Parameters.AddWithValue("@m", month);
                ins.Parameters.AddWithValue("@d", day);
                await MySQL.QueryAsync(ins);
            });

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter,
                        $"День {day} разблокирован за {PRICE} койнов", 3000);

            // обновляем календарь на клиенте
            Repository.SendMonth(player, month);
        }




        // CalendarTasksHandler.cs
        [RemoteEvent("server:calendar:requestMonth")]
        public void OnClientRequestMonth(ExtPlayer player, int month)
        {
            Repository.SendMonth(player, month);
        }
        [Command("comptask", GreedyArg = false)]
        public void CMD_CompTask(ExtPlayer admin, int targetId, int which, int amount)
        {
            try
            {
                // 1) Проверка прав
                if (!CommandsAccess.CanUseCmd(admin, AdminCommands.Setadmin))
                {
                    Notify.Send(admin, NotifyType.Error, NotifyPosition.BottomCenter,
                                "У вас нет доступа к этой команде", 3000);
                    return;
                }

                // 2) Находим цель
                var target = Main.GetPlayerByID(targetId);
                if (target == null)
                {
                    Notify.Send(admin, NotifyType.Error, NotifyPosition.BottomCenter,
                                "Игрок не найден", 3000);
                    return;
                }

                // 3) Текущие UTC-месяц и -день
                int month = DateTime.UtcNow.Month; // 1–12
                int day = DateTime.UtcNow.Day;   // 1–31

                // 4) Читаем из calendar_user_tasks
                List<int> requiredTaskIds;
                {
                    var cmd = new MySqlCommand(@"
                SELECT `taskIds`
                FROM `calendar_user_tasks`
                WHERE `userId` = @uid
                  AND `month`  = @m
                  AND `day`    = @d;
            ");
                    cmd.Parameters.AddWithValue("@uid", target.GetCharacterData().UUID);
                    cmd.Parameters.AddWithValue("@m", month);
                    cmd.Parameters.AddWithValue("@d", day);

                    var dt = MySQL.QueryRead(cmd);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        Notify.Send(admin, NotifyType.Warning, NotifyPosition.BottomCenter,
                                    "На сегодня нет назначенных тасков для этого игрока", 3000);
                        return;
                    }

                    requiredTaskIds = JsonConvert
                        .DeserializeObject<List<int>>(dt.Rows[0]["taskIds"].ToString());
                }

                // 5) Проверяем which
                if (which < 1 || which > requiredTaskIds.Count)
                {
                    Notify.Send(admin, NotifyType.Error, NotifyPosition.BottomCenter,
                                $"Параметр which должен быть от 1 до {requiredTaskIds.Count}", 3000);
                    return;
                }

                int taskId = requiredTaskIds[which - 1];

                // 6) Добавляем прогресс
                Repository.AddProgress(target, taskId, amount);

                Notify.Send(admin, NotifyType.Success, NotifyPosition.BottomCenter,
                            $"Игроку {target.Name} (id={targetId}) добавлено {amount} к таску {taskId}", 3000);
            }
            catch (Exception e)
            {
                
                Notify.Send(admin, NotifyType.Error, NotifyPosition.BottomCenter,
                            "Произошла ошибка при выполнении команды", 3000);
            }
        }



    }

}
