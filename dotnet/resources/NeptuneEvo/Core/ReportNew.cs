using System;
using System.Linq;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Handles;
using Redage.SDK;
using NeptuneEvo.Accounts;
using NeptuneEvo.Players;
using Database;
using LinqToDB;
using NeptuneEvo.Character;
using System.Data;

namespace NeptuneEvo.Core
{
    class ReportNew : Script
    {
        private static readonly nLog Log = new nLog("Core.ReportNew");

        public class TicketMessage
        {
            public string Author { get; set; }
            public string Text { get; set; }
        }

        public class Ticket
        {
            public int Id { get; set; }
            public string Author { get; set; }
            public string BlockedBy { get; set; } = "";
            public bool Closed { get; set; } = false;
            public List<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
        }

        public static Dictionary<int, Ticket> Tickets = new Dictionary<int, Ticket>();

        public static void Init()
        {
            try
            {
                Tickets = new Dictionary<int, Ticket>();
                using DataTable result = MySQL.QueryRead("SELECT * FROM questions");
                if (result != null)
                {
                    foreach (System.Data.DataRow row in result.Rows)
                    {
                        
                        int id = Convert.ToInt32(row[0]);
                        bool closed = row[6] != DBNull.Value;
                        Tickets[id] = new Ticket
                        {
                            Id = id,
                            Author = row[1].ToString(),
                            BlockedBy = row[3].ToString(),
                            Closed = closed,
                            Messages = new List<TicketMessage> { new TicketMessage { Author = row[1].ToString(), Text = row[2].ToString() } }
                        };
                    }
                }
                using DataTable msgResult = MySQL.QueryRead("SELECT * FROM question_messages ORDER BY id");
                if (msgResult != null)
                {
                    foreach (System.Data.DataRow row in msgResult.Rows)
                    {
                        int qid = Convert.ToInt32(row[1]);
                        if (!Tickets.ContainsKey(qid)) continue;
                        Tickets[qid].Messages.Add(new TicketMessage { Author = row[2].ToString(), Text = row[3].ToString() });
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write($"Init Exception: {e}");
            }
        }

        public static void onAdminLoad(ExtPlayer player)
        {
            try
            {
                var data = player.GetCharacterData();
                if (data == null || data.AdminLVL < Main.ServerSettings.MinAdminLvlReport) return;
                foreach (var t in Tickets.Values.Where(t => !t.Closed))
                {
                    Trigger.ClientEvent(player, "addreport", t.Id, t.Author, t.Messages.First().Text);
                    foreach (var msg in t.Messages.Skip(1))
                        Trigger.ClientEvent(player, "addreportmsg", t.Id, msg.Author, msg.Text);
                    Trigger.ClientEvent(player, "setreport", t.Id, t.BlockedBy);
                }
            }
            catch (Exception e)
            {
                Log.Write($"onAdminLoad Exception: {e}");
            }
        }
        [RemoteEvent("loadreports")]
        public static void LoadReports(ExtPlayer player)
        {
            try
            {
                var name = player.Name;
                foreach (var t in Tickets.Values.Where(r => r.Author == name))
                {
                    Trigger.ClientEvent(player, "addreport", t.Id, t.Author, t.Messages.First().Text);
                    foreach (var msg in t.Messages.Skip(1))
                        Trigger.ClientEvent(player, "addreportmsg", t.Id, msg.Author, msg.Text);
                    Trigger.ClientEvent(player, "setreport", t.Id, t.BlockedBy);
                    if (t.Closed)
                        Trigger.ClientEvent(player, "closeticket", t.Id);
                }
            }
            catch (Exception e)
            {
                Log.Write($"LoadReports Exception: {e}");
            }
        }
        [RemoteEvent("addreportmsg")]
        public static void ReportAddMessage(ExtPlayer player, string message)
        {
            try
            {
                if (!player.IsCharacterData()) return;
                var ticket = Tickets.Values.FirstOrDefault(t => t.Author == player.Name && !t.Closed);
                if (ticket == null) return;
                message = Main.BlockSymbols(message);
                string playerName = player.Name;
                ticket.Messages.Add(new TicketMessage { Author = playerName, Text = message });
                Trigger.SetTask(async () =>
                {
                    await using var db = new ServerBD("MainDB");
                    await db.InsertAsync(new QuestionMessages { QuestionId = ticket.Id, Author = playerName, Text = message, Time = DateTime.Now });
                });
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    var cData = foreachPlayer.GetCharacterData();
                    if (cData == null || cData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                    Trigger.ClientEvent(foreachPlayer, "addreportmsg", ticket.Id, player.Name, message);
                }
                Trigger.ClientEvent(player, "addreportmsg", ticket.Id, player.Name, message);
            }
            catch (Exception e)
            {
                Log.Write($"ReportAddMessage Exception: {e}");
            }
        }

        public static void AddReport(ExtPlayer player, string question, string author)
        {
            try
            {
                question = Main.BlockSymbols(question);
                var id = Tickets.Count == 0 ? 1 : Tickets.Keys.Max() + 1;
                var ticket = new Ticket
                {
                    Id = id,
                    Author = author,
                    Messages = new List<TicketMessage> { new TicketMessage { Author = author, Text = question } }
                };
                Tickets[id] = ticket;
                Trigger.SetTask(async () =>
                {
                    await using var db = new ServerBD("MainDB");
                    await db.InsertAsync(new Questions
                    {
                        ID = (uint)id,
                        Author = author,
                        Question = question,
                        Opened = DateTime.Now,
                        Status = 0
                    });
                    await db.InsertAsync(new QuestionMessages { QuestionId = id, Author = author, Text = question, Time = DateTime.Now });
                });
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    var cData = foreachPlayer.GetCharacterData();
                    if (cData == null || cData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                    Trigger.ClientEvent(foreachPlayer, "addreport", id, author, question);
                }
                Trigger.ClientEvent(player, "addreport", id, author, question);
            }
            catch (Exception e)
            {
                Log.Write($"AddReport Exception: {e}");
            }
        }

        [RemoteEvent("sendreport")]
        public static void SendAnswer(ExtPlayer player, int id, string text)
        {
            try
            {
                var cData = player.GetCharacterData();
                if (cData == null || cData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) return;
                if (!Tickets.ContainsKey(id)) return;
                text = Main.BlockSymbols(text);
                var ticket = Tickets[id];
                string adminName = player.Name;
                ticket.Messages.Add(new TicketMessage { Author = adminName, Text = text });
                ticket.BlockedBy = adminName;
                Trigger.SetTask(async () =>
                {
                    await using var db = new ServerBD("MainDB");
                    await db.Questions.Where(q => q.ID == id).Set(q => q.Respondent, adminName).Set(q => q.Response, text).UpdateAsync();
                    await db.InsertAsync(new QuestionMessages { QuestionId = id, Author = adminName, Text = text, Time = DateTime.Now });
                });
                ExtPlayer target = (ExtPlayer)NAPI.Player.GetPlayerFromName(ticket.Author);
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    var adData = foreachPlayer.GetCharacterData();
                    if (adData == null || adData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                    Trigger.ClientEvent(foreachPlayer, "addreportmsg", id, adminName, text);
                    Trigger.ClientEvent(foreachPlayer, "setreport", id, ticket.BlockedBy);
                }
                if (target.IsCharacterData())
                    Trigger.ClientEvent(target, "addreportmsg", id, adminName, text);
            }
            catch (Exception e)
            {
                Log.Write($"SendAnswer Exception: {e}");
            }
        }

        [RemoteEvent("takereport")]
        public static void TakeReport(ExtPlayer player, int id)
        {
            try
            {
                var cData = player.GetCharacterData();
                if (cData == null || cData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) return;
                if (!Tickets.ContainsKey(id)) return;
                var ticket = Tickets[id];
                ticket.BlockedBy = ticket.BlockedBy == player.Name ? "" : player.Name;
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    var adData = foreachPlayer.GetCharacterData();
                    if (adData == null || adData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                    Trigger.ClientEvent(foreachPlayer, "setreport", id, ticket.BlockedBy);
                }
            }
            catch (Exception e)
            {
                Log.Write($"TakeReport Exception: {e}");
            }
        }

        [RemoteEvent("closereport")]
        public static void CloseReport(ExtPlayer player, int id)
        {
            try
            {
                var ticket = Tickets.GetValueOrDefault(id);
                if (ticket == null) return;
                if (!ticket.Author.Equals(player.Name)) return;
                ticket.Closed = true;
                Trigger.SetTask(async () =>
                {
                    await using var db = new ServerBD("MainDB");
                    await db.Questions.Where(q => q.ID == id).Set(q => q.Closed, DateTime.Now).UpdateAsync();
                });
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    var adData = foreachPlayer.GetCharacterData();
                    if (adData == null || adData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                    Trigger.ClientEvent(foreachPlayer, "delreport", id);
                }
                Trigger.ClientEvent(player, "closeticket", id);
            }
            catch (Exception e)
            {
                Log.Write($"CloseReport Exception: {e}");
            }
        }

        [RemoteEvent("otheradminreport")]
        public static void OtherAdmin(ExtPlayer player, int id)
        {
            try
            {
                var ticket = Tickets.GetValueOrDefault(id);
                if (ticket == null) return;
                if (!ticket.Author.Equals(player.Name)) return;
                ticket.BlockedBy = "";
                string requesterName = player.Name;
                ticket.Messages.Add(new TicketMessage { Author = requesterName, Text = "Помощь другого администратора" });
                Trigger.SetTask(async () =>
                {
                    await using var db = new ServerBD("MainDB");
                    await db.InsertAsync(new QuestionMessages { QuestionId = id, Author = requesterName, Text = "Помощь другого администратора", Time = DateTime.Now });
                });
                foreach (ExtPlayer foreachPlayer in Main.AllAdminsOnline)
                {
                    var adData = foreachPlayer.GetCharacterData();
                    if (adData == null || adData.AdminLVL < Main.ServerSettings.MinAdminLvlReport) continue;
                    Trigger.ClientEvent(foreachPlayer, "setreport", id, "");
                    Trigger.ClientEvent(foreachPlayer, "addreportmsg", id, requesterName, "Помощь другого администратора");
                }
                Trigger.ClientEvent(player, "addreportmsg", id, requesterName, "Помощь другого администратора");
            }
            catch (Exception e)
            {
                Log.Write($"OtherAdmin Exception: {e}");
            }
        }
    }
}