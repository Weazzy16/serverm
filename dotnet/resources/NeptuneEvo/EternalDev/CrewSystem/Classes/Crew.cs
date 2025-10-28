using Localization;
using NeptuneEvo.Core;
using NeptuneEvo.EternalDev.CrewSystem.Enums;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.EternalDev.CrewSystem.Classes
{
    public class Crew
    {
        public int Id { get; set; }
        public ConcurrentDictionary<int, CrewMember> Members { get; set; } = new ConcurrentDictionary<int, CrewMember>();
        public string InviteCode { get; set; }

        public Crew(int id, string inviteCode)
        {
            Id = id;
            InviteCode = inviteCode;
        }

        public CrewMember Subscribe(ExtPlayer player, bool isLeader = false)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return null;

            if (!Members.TryGetValue(player.GetUUID(), out var crewMember))
            {
                crewMember = new CrewMember(player.GetUUID(), isLeader ? CrewAccess.Leader : CrewAccess.Member);
                ActionToPlayers((p) => Notify.Send(p, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок {player.GetName()} вступил в группу", 3000));

                Members.TryAdd(player.GetUUID(), crewMember);
            }

            sessionData.CrewSession.CrewId = Id;
            crewMember.UpdateVariables(player);

            Update();
            return crewMember;
        }

        public bool UnSubscribe(int uuid, bool isKicked = false)
        {
            if (!Members.TryGetValue(uuid, out var crewMember))
                return false;

            var targetPlayer = crewMember.Player;
            if (targetPlayer != null)
            {
                Trigger.ClientEvent(targetPlayer, "e-dev.crew-system.update-group", null);
                Notify.Send(targetPlayer, NotifyType.Info, NotifyPosition.BottomCenter, isKicked ? $"Лидер группы выгнал вас" : "Вы вышли из группы", 3000);

                targetPlayer.SessionData.CrewSession.CrewId = -1;
                crewMember.UpdateVariables(targetPlayer, isRemove: true);
            }

            Members.TryRemove(uuid, out _);
            Update();

            if (Main.PlayerNames.TryGetValue(uuid, out var playerNames))
                ActionToPlayers((p) => Notify.Send(p, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок {playerNames} покинул группу", 3000));

            return true;
        }

        public bool GetCrewMember(int uuid, out CrewMember crewMember)
            => Members.TryGetValue(uuid, out crewMember);

        public bool TrySetMemberCommander(ExtPlayer player, int targetUuid)
        {
            if (!GetCrewMember(player.GetUUID(), out var playerCrewMember) || !GetCrewMember(targetUuid, out var targetCrewMember))
                return false;

            if (targetCrewMember.Access >= playerCrewMember.Access)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нет доступа", 3000);
                return false;
            }

            targetCrewMember.Access = CrewAccess.Commander;
            Update();

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы назначили нового помощника!", 3000);

            var targetPlayer = targetCrewMember.Player;
            if (targetPlayer != null)
            {
                Notify.Send(targetPlayer, NotifyType.Info, NotifyPosition.BottomCenter, $"Вас назначили помощником лидера группы!", 3000);
                targetCrewMember.UpdateVariables(targetPlayer);
            }

            return true;
        }

        public bool TrySetMemberLeader(ExtPlayer player, int targetUuid)
        {
            if (!GetCrewMember(player.GetUUID(), out var playerCrewMember) || !GetCrewMember(targetUuid, out var targetCrewMember))
                return false;

            if (playerCrewMember.Access != CrewAccess.Leader)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не лидер группы", 3000);
                return false;
            }

            targetCrewMember.Access = CrewAccess.Leader;
            playerCrewMember.Access = CrewAccess.Commander;

            var targetPlayer = targetCrewMember.Player;
            if (targetPlayer != null)
            {
                Notify.Send(targetPlayer, NotifyType.Info, NotifyPosition.BottomCenter, $"Вам передали лидерство группы", 3000);
                targetCrewMember.UpdateVariables(targetPlayer);
            }

            playerCrewMember.UpdateVariables(player);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно назначили нового лидера группы", 3000);
            Update();

            return true;
        }

        public bool TryChangeInviteCode(ExtPlayer player)
        {
            if (!GetCrewMember(player.GetUUID(), out var crewMember)) 
                return false;

            if (crewMember.Access < CrewAccess.Commander)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"У вас нет доступа для изменения кода", 3000);
                return false;
            }

            var invitedCode = Manager.GenerateCrewInviteCode();
            if (invitedCode == string.Empty)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка изменения кода!", 3000);
                return false;
            }

            InviteCode = invitedCode;
            Update();

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы изменили код!", 3000);
            return true;
        }

        public bool TryKickMember(ExtPlayer player, int uuid)
        {
            if (!GetCrewMember(player.GetUUID(), out var playerCrewMember) || playerCrewMember.Access < CrewAccess.Commander) 
                return false;

            if (!Members.TryGetValue(uuid, out var targetCrewMember))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Такого участника не существует!", 3000);
                return false;
            }

            if (targetCrewMember.Access >= playerCrewMember.Access)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не можете выгнать из группы этого игрока!", 3000);
                return false;
            }

            UnSubscribe(targetCrewMember.UUID, isKicked: true);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно выгнали игрока из группы", 3000);

            return true;
        }

        public bool TryInvitePlayer(ExtPlayer player, ExtPlayer target)
        {
            if (!GetCrewMember(player.GetUUID(), out var playerCrewMember) || playerCrewMember.Access < CrewAccess.Commander)
                return false;

            var targetSessionData = target.GetSessionData();
            if (target is null || targetSessionData is null)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Данного игрока не существует!", 3000);
                return false;
            }

            if (player.GetUUID() == target.GetUUID())
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Нельзя указывать себя", 3000);
                return false;
            }

            if (target.Position.DistanceTo(player.Position) > 5)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Игрок слишком далеко", 3000);
                return false;
            }

            if (target.SessionData.CrewSession.CrewId != -1)
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок уже состоит в группе", 3000);
                return false;
            }

            if (Members.Count >= Manager.Config.MaxPlayersInCrew)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Слишком много участников в группе (максимум {Manager.Config.MaxPlayersInCrew} участников)", 3000);
                return false;
            }

            if (targetSessionData.RequestData.IsRequested && targetSessionData.RequestData.Time > DateTime.Now)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У игрока уже есть приглашение", 3000);
                return false;
            }

            targetSessionData.RequestData.IsRequested = true;
            targetSessionData.RequestData.Request = "CREW:INVITE";
            targetSessionData.RequestData.From = player;
            targetSessionData.RequestData.Time = DateTime.Now.AddSeconds(10);

            EventSys.SendCoolMsg(target, "Предложение", "Вступить в группу!", $"Игрок #{player.GetUUID()} предлагает вступить вам в группу!", "", 10000);
            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно отправили приглашение в группу", 3000);
            return true;
        }

        public bool TryDisbandment(ExtPlayer player)
        {           
            if (!GetCrewMember(player.GetUUID(), out var crewMember))
                return false;

            if (crewMember.Access != CrewAccess.Leader)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Вы не лидер группы", 3000);
                return false;
            }

            ActionToPlayers((target) =>
            {
                var pSessionData = target.GetSessionData();
                if (pSessionData is null || !GetCrewMember(target.GetUUID(), out var targetCrewMember)) return;

                target.SessionData.CrewSession.CrewId = -1;

                if (target != player)
                    Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Лидер расформировал группу", 3000);
                
                Trigger.ClientEvent(target, "e-dev.crew-system.update-group", null);
                targetCrewMember.UpdateVariables(target, isRemove: true);
            });

            Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно расформировали группу", 3000);
            Manager.DeleteCrew(Id);

            return true;
        }

        public void Update()
        {
            var players = GetPlayers();
            if (players.Length == 0) return;

            Trigger.ClientEventToPlayers(players, "e-dev.crew-system.update-group", JsonConvert.SerializeObject(this));
        }

        public ExtPlayer[] GetPlayers()
            => Members.Values.Select(x => x.Player)
                    .Where(x => x != null).ToArray();

        public void ActionToPlayers(Action<ExtPlayer> action, ExtPlayer ignoreEntity = null)
        {
            var players = GetPlayers();
            if (players.Length == 0) return;

            foreach (var player in players)
            {
                if (player == ignoreEntity) 
                    continue;
                
                action.Invoke(player);
            }
        }
    }
}
