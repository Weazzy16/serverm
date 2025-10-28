using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.Database; // ServerBD, Linq2DB
using Redage.SDK;
using LinqToDB;
using Database;

namespace NeptuneEvo.CalendarTasks
{

    public static class StatsRepository
    {
        // Кэш <(user,month) → stats>
        private static readonly Dictionary<(int, byte), CalendarStats> _cache
            = new Dictionary<(int, byte), CalendarStats>();

        // Получить или создать запись
        public static CalendarStats GetStats(int userId, byte month)
        {
            var key = (userId, month);
            if (_cache.TryGetValue(key, out var st)) return st;

            using var db = new ServerBD("MainDB");
            st = db.GetTable<CalendarStats>()
                   .FirstOrDefault(x => x.UserId == userId && x.Month == month)
                 ?? new CalendarStats { UserId = userId, Month = month };
            _cache[key] = st;
            return st;
        }

        // +1 к completed
        public static void IncrementCompleted(int userId, byte month)
        {
            var st = GetStats(userId, month);
            st.Completed++;
            Persist(st);
        }

        // +1 к skipped
        public static void IncrementSkipped(int userId, byte month)
        {
            var st = GetStats(userId, month);
            st.Skipped++;
            Persist(st);
        }

        // Удалить из БД (при ресете месяца)
        public static void ResetMonth(int userId, byte month)
        {
            _cache.Remove((userId, month));
            Trigger.SetTask(async () =>
            {
                await using var db = new ServerBD("MainDB");
                await db.CalendarStats
                        .Where(x => x.UserId == userId && x.Month == month)
                        .DeleteAsync<CalendarStats>();
            });
        }

        // Общая асинхронная запись
        private static void Persist(CalendarStats st)
        {
            Trigger.SetTask(async () =>
            {
                await using var db = new ServerBD("MainDB");
                var exist = await db.CalendarStats
                                    .Where(x => x.UserId == st.UserId && x.Month == st.Month)
                                    .FirstOrDefaultAsync();
                if (exist != null)
                {
                    exist.Completed = st.Completed;
                    exist.Skipped = st.Skipped;
                    await db.UpdateAsync(exist);
                }
                else
                {
                    await db.InsertAsync(st);
                }
            });
        }
    }
}
