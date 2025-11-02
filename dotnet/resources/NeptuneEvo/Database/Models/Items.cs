using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using LinqToDB;
using MySqlConnector;
using Redage.SDK;

namespace NeptuneEvo.Database.Models
{
    public class Items
    {
        private static readonly nLog Log = new nLog("Database.Items");
                
        public static void Start()
        {
            var thread = new Thread(Worker);
            thread.IsBackground = true;
            thread.Name = "ItemsSave";
            thread.Start();
        }
        private static async void Worker()
        {
            while (true)
            {
                try
                {

                    var updateListData = ItemsUpdate.Values.ToList();
                    ItemsUpdate.Clear();
                    
                    var dellListData = ItemsDelete.ToList();
                    ItemsDelete.Clear();

                    if (updateListData.Count > 0 || dellListData.Count > 0)
                    {
                        await using var db = new ServerBD("MainDB");//В отдельном потоке 

                        await ItemUpdate(db, updateListData);
                        
                        await ItemDelete(db, dellListData);
                    }
                    
                }
                catch (Exception e)
                {
                    Log.Write($"LogsWorker Exception: {e.ToString()}");
                }
                Thread.Sleep(1000 * 30);
            }
        }
        
        //
        
        private static Dictionary<int, List<object>> ItemsUpdate = new Dictionary<int, List<object>>();
        public static bool IsItemUpdate(int sqlId) => ItemsUpdate.ContainsKey(sqlId);

        public static void AddItemUpdate(int sqlId, string locationName, int count, string data, string location, int slotId, bool isTurn = false)
        {
            // ✅ SQL с новым полем
            using MySqlCommand cmd = new MySqlCommand
            {
                CommandText = @"
            UPDATE `items_data` 
            SET `data_id` = @data_id, 
                `item_count` = @item_count, 
                `item_data` = @item_data, 
                `location` = @location, 
                `slotId` = @slotId, 
                `is_turn` = @is_turn
            WHERE `auto_id` = @auto_id"
            };
            cmd.Parameters.AddWithValue("@data_id", locationName);
            cmd.Parameters.AddWithValue("@item_count", count);
            cmd.Parameters.AddWithValue("@item_data", data);
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@slotId", slotId);
            cmd.Parameters.AddWithValue("@is_turn", isTurn ? 1 : 0); // ✅ НОВЫЙ ПАРАМЕТР
            cmd.Parameters.AddWithValue("@auto_id", sqlId);
            MySQL.Query(cmd);
        }

        private static async Task ItemUpdate(ServerBD db, List<List<object>> listData)
        {
            try
            {
                foreach (var saveData in listData)
                {
                    await db.ItemsData
                        .Where(v => v.AutoId == Convert.ToInt32(saveData[0]))
                        .Set(v => v.DataId, Convert.ToString(saveData[1]))
                        .Set(v => v.ItemCount, Convert.ToInt32(saveData[2]))
                        .Set(v => v.ItemData, Convert.ToString(saveData[3]))
                        .Set(v => v.Location, Convert.ToString(saveData[4]))
                        .Set(v => v.SlotId, Convert.ToInt16(saveData[5]))
                        .UpdateAsync();
                }
            }
            catch (Exception e)
            {
                Log.Write($"ItemUpdate Exception: {e.ToString()}");
            }
        }
        
        //
        
        private static List<int> ItemsDelete = new List<int>();
        public static void AddItemDelete(int sqlId) => ItemsDelete.Add(sqlId);
        private static async Task ItemDelete(ServerBD db, List<int> listData)
        {
            try
            {
                foreach (var sqlId in listData)
                {
                    await db.ItemsData
                        .Where(v => v.AutoId == sqlId)
                        .DeleteAsync();

                    await db.Notes
                        .Where(v => v.ItemId == sqlId)
                        .DeleteAsync();
                }
            }
            catch (Exception e)
            {
                Log.Write($"ItemDelete Exception: {e.ToString()}");
            }
        }
    }
}