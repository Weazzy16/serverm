using GTANetworkAPI;
using NeptuneEvo.Handles;
using NeptuneEvo.Character;
using Redage.SDK;
using System;
using System.IO;
using System.Text;

namespace NeptuneEvo.AleSystems
{
    public static class AntiCheat
    {
        private static readonly nLog Log = new nLog(nameof(AntiCheat));

        [ServerEvent(Event.ResourceStart)]
        public static void onResourceStart()
        {
            try
            {
                Log.Write("IAC загружен " );
            }
            catch (Exception e)
            {
                Log.Write($"onResourceStart Exception: {e.ToString()}");
            }
        }
        #region Check Leet-1337
        public static void AntiCheatCheck(this ExtPlayer player)
        {
            try
            {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "1337_scripts");
                string path2 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Leet");
		        string path3 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "loader_64.exe");
		        string path4 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop", "loader_64.exe");
                if (Directory.Exists(path) || Directory.Exists(path2) || File.Exists(path3) || File.Exists(path4))
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) return;
                    NAPI.Task.Run(() =>
                    {
                        //AdminConsole.AntiCheatLog(player, "возможно использует чит = Leet 1337");

                        using (StreamWriter AntiCheatCheck = new StreamWriter("IAC-Logs/AntiCheatCheck.txt", true, Encoding.UTF8))
                        {
                            DateTime Time2 = DateTime.Now;
                            AntiCheatCheck.Write(Time2 + $" [IAC] {player.Name}[{characterData.UUID}] | ID: {player.Value} возможно использует чит = Leet 1337\r\n");
                            AntiCheatCheck.Close();
                        }
                    }, 2222);
                }
            }
            catch (Exception e)
            {
                Log.Write($"AntiCheatCheck Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server:AntiCheatCheck")]
        public static void serverAntiCheatCheck(this ExtPlayer player)
        {
            try
            {
                string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "1337_scripts");
                string path2 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Leet");
		        string path3 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "loader_64.exe");
		        string path4 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop", "loader_64.exe");
                if (Directory.Exists(path) || Directory.Exists(path2) || File.Exists(path3) || File.Exists(path4))
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) return;

                    NAPI.Task.Run(() =>
                    {
                        //AdminConsole.AntiCheatLog(player, "возможно использует чит = Leet 1337");

                        using (StreamWriter AntiCheatCheck = new StreamWriter("IAC-Logs/AntiCheatCheck.txt", true, Encoding.UTF8))
                        {
                            DateTime Time2 = DateTime.Now;
                            AntiCheatCheck.Write(Time2 + $" [IAC] {player.Name}[{characterData.UUID}] | ID: {player.Value} возможно использует чит = Leet 1337\r\n");
                            AntiCheatCheck.Close();
                        }
                    }, 2222);
                }
            }
            catch (Exception e)
            {
                Log.Write($"serverAntiCheatCheck Exception: {e.ToString()}");
            }
        }
        #endregion
        #region Check Unicore
        public static void AntiCheatCheck2(this ExtPlayer player)
        {
            try
            {
		        string path1 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "build (1).exe");
		        string path2 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop", "build (1).exe");
                if (File.Exists(path1) || File.Exists(path2))
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) return;

                    NAPI.Task.Run(() =>
                    {
                        //AdminConsole.AntiCheatLog(player, "возможно использует чит = Unicore");

                        using (StreamWriter AntiCheatCheck = new StreamWriter("IAC-Logs/AntiCheatCheck.txt", true, Encoding.UTF8))
                        {
                            DateTime Time2 = DateTime.Now;
                            AntiCheatCheck.Write(Time2 + $" [IAC] {player.Name}[{characterData.UUID}] | ID: {player.Value} возможно использует чит = Unicore\r\n");
                            AntiCheatCheck.Close();
                        }
                    }, 2222);
                }
            }
            catch (Exception e)
            {
                Log.Write($"AntiCheatCheck2 Exception: {e.ToString()}");
            }
        }
        [RemoteEvent("server:AntiCheatCheck2")]
        public static void serverAntiCheatCheck2(this ExtPlayer player)
        {
            try
            {
		        string path1 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "build (1).exe");
		        string path2 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop", "build (1).exe");
                if (File.Exists(path1) || File.Exists(path2))
                {
                    var characterData = player.GetCharacterData();
                    if (characterData == null) return;

                    NAPI.Task.Run(() =>
                    {
                        //AdminConsole.AntiCheatLog(player, "возможно использует чит = Unicore");

                        using (StreamWriter AntiCheatCheck = new StreamWriter("IAC-Logs/AntiCheatCheck.txt", true, Encoding.UTF8))
                        {
                            DateTime Time2 = DateTime.Now;
                            AntiCheatCheck.Write(Time2 + $" [IAC] {player.Name}[{characterData.UUID}] | ID: {player.Value} возможно использует чит = Unicore\r\n");
                            AntiCheatCheck.Close();
                        }
                    }, 2222);
                }
            }
            catch (Exception e)
            {
                Log.Write($"serverAntiCheatCheck2 Exception: {e.ToString()}");
            }
        }
        #endregion
    }
}