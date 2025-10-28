using NeptuneEvo.Handles;
using NeptuneEvo.AleSystems.Fishing.Classes;
using NeptuneEvo;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing
{
    public static class FishingPlayer
    {
        private static readonly nLog Log = new nLog(nameof(FishingPlayer));
        private static Dictionary<int, FishingPlayerData> _players = new Dictionary<int, FishingPlayerData>();

        public static void Inititalize()
        {
            try
            {
                DataTable dataTable = MySQL.QueryRead("SELECT * FROM `fishing_job`");
                if (dataTable is null || dataTable.Rows.Count == 0) return;
                
                foreach (DataRow row in dataTable.Rows)
                {
                    int uuid = Convert.ToInt32(row["uuid"]);
                    int lvl = Convert.ToInt32(row["lvl"]);
                    int progress = Convert.ToInt32(row["progress"]);
                    int total = Convert.ToInt32(row["total"]);

                    FishingPlayerData playerData = new FishingPlayerData()
                    {
                        LVL = lvl,
                        Progress = progress,
                        Total = total
                    };

                    _players.TryAdd(uuid, playerData);
                }
            }
            catch(Exception ex) { Log.Write("Inititalize: " + ex.ToString()); }
        }

        public static FishingPlayerData GetFishingPlayer(this ExtPlayer player)
        {
            var characterData = player.CharacterData;
            if (characterData is null) return null;

            return GetFishingPlayerByUUID(characterData.UUID);
        }

        public static FishingPlayerData GetFishingPlayerByUUID(int uuid)
        {
            if (!_players.TryGetValue(uuid, out FishingPlayerData data))
            {
                data = new FishingPlayerData();
                _players.TryAdd(uuid, data);
                MySQL.Query($"INSERT INTO `fishing_job` (`uuid`,`lvl`,`progress`,`total`) VALUES({uuid}, {data.LVL}, {data.Progress}, {data.Total})");
                return data;
            }
            return data;
        }
    }
}
