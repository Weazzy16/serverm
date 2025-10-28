// NeptuneEvo/Achievements/AchievementsHandler.cs

using System;
using System.Collections.Generic;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySqlConnector;
using Newtonsoft.Json;
using NeptuneEvo.Character;
using NeptuneEvo.Database;
using Database;
using LinqToDB;
using NeptuneEvo.Handles;
using NeptuneEvo.Chars.Models;
using Redage.SDK;
using GTANetworkMethods;
using NeptuneEvo.Events;
using NeptuneEvo.Core;
using NeptuneEvo.MoneySystem; // для MoneySystem.Wallet.Change



namespace NeptuneEvo.Achievements
{
    // DTO для клиентского UI
    public class AchievementData
    {
        public int Id { get; set; }
        public int Current { get; set; }
        public int Target { get; set; }
        public bool Completed { get; set; }
        public bool Rewarded { get; set; }
    }
    public enum AchievementRewardType
    {
        Money,
        Item,
        Vehicle
    }

    // Описание награды
    public class AchievementReward
    {
        public int AchievementId { get; }
        public AchievementRewardType Type { get; }
        public int Count { get; }
        public int ItemId { get; }
        public VehicleHash Vehicle { get; }

        public AchievementReward(int achievementId,
                                 AchievementRewardType type,
                                 int count = 0,
                                 int itemId = 0,
                                 VehicleHash vehicle = VehicleHash.T20)
        {
            AchievementId = achievementId;
            Type = type;
            Count = count;
            ItemId = itemId;
            Vehicle = vehicle;
        }
    }

    // Список наград за каждое достижение
    public static class AchievementRewards
    {
        public static readonly List<AchievementReward> List = new List<AchievementReward>()
{
  new AchievementReward(1, AchievementRewardType.Money, count:300000),
  new AchievementReward(2, AchievementRewardType.Money, count:300000),
  // …
  new AchievementReward(21, AchievementRewardType.Item,  itemId:(int)ItemId.Debug, count:1),
  // … и так далее для всех 
};

    }

    public static class Repository
    {
        // Цели для ваших ачивок
        private static readonly Dictionary<int, int> _achievementTargets = new Dictionary<int, int>
        {
            { 1, 20 },  { 2, 20 },  { 3, 25 }, { 4, 50 },  { 5, 100 },
            { 6, 500000 }, { 7, 1 }, { 8, 500 }, { 9, 100 }, { 10, 3 },
            { 11,100 }, { 12,25 }, { 13,500000 }, { 14,100000 }, { 15,1 },
            { 16,10 }, { 17,500000 }, { 18,100 }, { 19,500 }, { 20,15 },
            { 21,1 }, { 22,100 }, { 23,100 }, { 24,100 }, { 25,650 },
            { 26,375 }, { 27,300 }, { 28,1250 }, { 29,750 }
            // добавьте остальные по аналогии…
        };

        // In‐memory кэш: UUID → список прогресса
        private static readonly Dictionary<int, List<AchievementData>> _cache =
    new Dictionary<int, List<AchievementData>>();


        public static void LoadFromDb(int uuid)
        {
            using var db = new ServerBD("MainDB");
            var rows = db.Achievement
                         .Where(a => a.UserId == uuid)
                         .ToList();

            var list = new List<AchievementData>();
            foreach (var kv in _achievementTargets)
            {
                var rec = rows.FirstOrDefault(r => r.AchievementId == kv.Key);
                if (rec != null)
                {
                    list.Add(new AchievementData
                    {
                        Id = rec.AchievementId,
                        Current = rec.CompletedValue,
                        Target = kv.Value,
                        Completed = rec.Completed,
                        Rewarded = rec.Rewarded
                    });
                }
                else
                {
                    list.Add(new AchievementData
                    {
                        Id = kv.Key,
                        Current = 0,
                        Target = kv.Value,
                        Completed = false,
                        Rewarded = false
                    });
                }
            }

            _cache[uuid] = list;
        }

        public static List<AchievementData> Get(int uuid)
        {
            if (!_cache.ContainsKey(uuid))
                LoadFromDb(uuid);
            return _cache[uuid];
        }

        public static void SendToClient(ExtPlayer player)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            var list = Get(cd.UUID);
            var payload = list.Select(a => new
            {
                id = $"a{a.Id}",
                current = a.Current,
                target = a.Target,
                completed = a.Completed,
                rewarded = a.Rewarded
            }).ToList();

            var json = JsonConvert.SerializeObject(payload);
            Trigger.ClientEvent(player, "achievements_update", json);
        }

        public static void AddProgress(ExtPlayer player, int achId, int amount = 1)
        {
            var cd = player.GetCharacterData();
            if (cd == null) return;

            var list = Get(cd.UUID);
            var ach = list.FirstOrDefault(x => x.Id == achId);
            if (ach == null || ach.Completed) return;  // если уже достигнуто — ничего не делаем

            ach.Current += amount;
            if (ach.Current >= ach.Target)
            {
                ach.Current = ach.Target;
                ach.Completed = true;     // достигли цели, отмечаем Completed=true
                ach.Rewarded = false;    // награда ещё не выдана
            }

            // сохраняем в БД…
            Trigger.SetTask(async () =>
            {
                await using var db = new ServerBD("MainDB");
                var row = await db.Achievement
                                  .Where(a => a.UserId == cd.UUID && a.AchievementId == achId)
                                  .FirstOrDefaultAsync();

                if (row != null)
                {
                    row.CompletedValue = ach.Current;
                    row.Completed = ach.Completed;
                    row.Rewarded = ach.Rewarded;
                    await db.UpdateAsync(row);
                }
                else
                {
                    await db.InsertAsync(new Achievement
                    {
                        UserId = cd.UUID,
                        AchievementId = achId,
                        CompletedValue = ach.Current,
                        Completed = ach.Completed,
                        Rewarded = ach.Rewarded
                    });
                }
            });

            SendToClient(player);
        }

        public static void Reward(ExtPlayer player, int achId)
        {

            var cd = player.GetCharacterData();
            if (cd == null) return;

            var list = Get(cd.UUID);
            var ach = list.FirstOrDefault(x => x.Id == achId);

            // проверяем: достижение достигнуто (Completed=true) и при этом награда ещё НЕ выдана (Rewarded=false)
            if (ach == null || !ach.Completed || ach.Rewarded) return;

            ach.Rewarded = true;  // отмечаем, что награду выдали

            // сохраняем в БД
            Trigger.SetTask(async () =>
            {
                await using var db = new ServerBD("MainDB");
                var row = await db.Achievement
                                  .Where(a => a.UserId == cd.UUID && a.AchievementId == achId)
                                  .FirstOrDefaultAsync();
                if (row != null)
                {
                    row.Rewarded = true;
                    await db.UpdateAsync(row);
                }
            });
            NAPI.Util.ConsoleOutput($"[ACH] Reward() pre-check: Completed={ach.Completed}, Rewarded={ach.Rewarded}");

            SendToClient(player);

            // выдаём сам бонус
            var reward = AchievementRewards.List.FirstOrDefault(r => r.AchievementId == achId);
            NAPI.Util.ConsoleOutput($"[ACH] found reward = {(reward != null ? reward.Type.ToString() : "null")} for ID={achId}");
            if (reward != null)
            {
                NAPI.Util.ConsoleOutput($"[ACH] Calling GiveAchievementBonus for ID={achId}");
                GiveAchievementBonus(player, reward);
            }
        }




        private static string GetSerial()
        {
            try
            {
                var rand = new Random();
                int serial = rand.Next(100_000, 999_999);
                return $"ACH{serial}";
            }
            catch
            {
                // Логирование ошибки убрано
                return "ACH000000";
            }
        }


        private static void GiveAchievementBonus(ExtPlayer player, AchievementReward award)
        {

            try
            {
                var cd = player.GetCharacterData();
                if (cd == null) return;
                NAPI.Util.ConsoleOutput($"[ACH] Inside GiveAchievementBonus: ID={award.AchievementId}, Type={award.Type}");

                switch (award.Type)
                {
                    case AchievementRewardType.Money:
                        // из BattlePass: даём деньги + лог + уведомление
                        MoneySystem.Wallet.Change(player, award.Count);
                        GameLog.Money(
                            "system",
                            $"player({cd.UUID})",
                            award.Count,
                            $"AchGiveBonus({award.AchievementId})"
                        );
                        Notify.Send(
                            player,
                            NotifyType.Success,
                            NotifyPosition.BottomCenter,
                            $"Вы получили {MoneySystem.Wallet.Format(award.Count)} за достижение!",
                            6000
                        );
                        break;

                    case AchievementRewardType.Item:
                        // из BattlePass: даём каждый предмет в цикле, серийник убрал
                        for (int i = 0; i < award.Count; i++)
                        {
                            var item = (ItemId)award.ItemId;
                            // если нужно добавить уникальные data, используйте GetSerial как в BP
                            Chars.Repository.AddNewItemWarehouse(
                                player,
                                item,
                                1,
                                "" // или какой-то data, например GetSerial()
                            );
                            GameLog.Money(
                                "system",
                                $"player({cd.UUID})",
                                1,
                                $"AchGiveBonus(Item,{award.ItemId})"
                            );
                        }
                        Notify.Send(
                            player,
                            NotifyType.Success,
                            NotifyPosition.BottomCenter,
                            "Вы получили предмет за достижение!",
                            6000
                        );
                        break;

                    case AchievementRewardType.Vehicle:
                        // из BattlePass: спавним и подсаживаем
                        var pos = player.Position;
                        var veh = NAPI.Vehicle.CreateVehicle(
                            award.Vehicle,
                            new Vector3(pos.X + 2, pos.Y + 2, pos.Z),
                            player.Rotation.Z,
                            0, 0,
                            "REWARD"
                        );
                        NAPI.Task.Run(() => NAPI.Player.SetPlayerIntoVehicle(player, veh, 0));
                        Notify.Send(
                            player,
                            NotifyType.Success,
                            NotifyPosition.BottomCenter,
                            "Вам выдан транспорт за достижение!",
                            6000
                        );
                        break;
                }
            }
            catch (Exception ex)
            {
                NAPI.Util.ConsoleOutput($"[ACH] GiveAchievementBonus Exception: {ex}");
            }
        }

    }


            
        
    

        public class AchievementsHandler : Script
    {
        // Загрузка ачивок при входе в игру / спавне персонажа
        

        // Клиент запросил список
        [RemoteEvent("server:achievement:request")]
        public void OnClientRequest(ExtPlayer player)
        {
            var uuid = player.GetCharacterData()?.UUID ?? 0;
            NAPI.Util.ConsoleOutput($"[ACH] OnClientRequest for UUID={uuid}");
            Repository.SendToClient(player);
            NAPI.Util.ConsoleOutput($"[ACH] Sent achievements_update to UUID={uuid}");
        }

        // Клиент запросил награду
        [RemoteEvent("server:achievement:reward")]
        public void OnClientReward(ExtPlayer player, int id)
        {
            var uuid = player.GetCharacterData()?.UUID ?? 0;
            NAPI.Util.ConsoleOutput($"[ACH] OnClientReward for UUID={uuid}, id={id}");
            Repository.Reward(player, id);
            NAPI.Util.ConsoleOutput($"[ACH] Reward processed for UUID={uuid}, id={id}");
        }
    }
}
