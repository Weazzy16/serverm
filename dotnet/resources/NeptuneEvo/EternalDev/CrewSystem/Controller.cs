using GTANetworkAPI;
using NeptuneEvo.EternalDev.CrewSystem.Classes;
using NeptuneEvo.EternalDev.CrewSystem.Extensions;
using NeptuneEvo.Handles;
using Redage.SDK;
using System.Linq;

namespace NeptuneEvo.EternalDev.CrewSystem
{
    public class Controller : Script
    {
        [RemoteEvent("e-dev.crew-system.create")]
        public void CreateCrew(ExtPlayer player)
        {
            if (player.GetPlayerCrew(out _))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы уже состоите в группе!", 3000);
                return;
            }

            var invitedCode = Manager.GenerateCrewInviteCode();
            if (invitedCode == string.Empty)
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка создания группы!", 3000);
                return;
            }

            var crewId = Manager.GenerateId();
            var crew = new Crew(crewId, invitedCode);

            Manager.Pool.TryAdd(crewId, crew);
            crew.Subscribe(player, isLeader: true);
        }

        [RemoteEvent("e-dev.crew-system.enter-code")]
        public void EnterCode(ExtPlayer player, string inviteCode)
        {
            if (player.GetPlayerCrew(out var _))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже состоите в группе!", 3000);
                return;
            }

            var crewData = Manager.Pool.Values.ToList().Find(x => x.InviteCode == inviteCode);
            if (crewData is null) return;

            crewData.Subscribe(player);
        }

        [RemoteEvent("e-dev.crew-system.leave")]
        public void Leave(ExtPlayer player)
        {
            if (!player.GetPlayerCrew(out var crewData))
                return;

            crewData.UnSubscribe(player.GetUUID());
        }

        [RemoteEvent("e-dev.crew-system.disbandment")]
        public void Disbandment(ExtPlayer player)
        {
            if (!player.GetPlayerCrew(out var crewData))
                return;

            crewData.TryDisbandment(player);
        }

        [RemoteEvent("e-dev.crew-system.set-commander")]
        public void SetCommander(ExtPlayer player, int uuid)
        {
            if (!player.GetPlayerCrew(out var crewData))
                return;

            crewData.TrySetMemberCommander(player, uuid);
        }

        [RemoteEvent("e-dev.crew-system.set-leader")]
        public void SetLeader(ExtPlayer player, int uuid)
        {
            if (!player.GetPlayerCrew(out var crewData))
                return;

            crewData.TrySetMemberLeader(player, uuid);
        }

        [RemoteEvent("e-dev.crew-system.kick")]
        public void Kick(ExtPlayer player, int uuid)
        {
            if (!player.GetPlayerCrew(out var crewData))
                return;

            crewData.TryKickMember(player, uuid);
        }

        [RemoteEvent("e-dev.crew-system.invite")]
        public void Invite(ExtPlayer player, string strId)
        {
            if (!player.GetPlayerCrew(out var crewData))
                return;

            if (!int.TryParse(strId, out var uuid))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Введите корректный статический Id игрока", 3000);
                return;
            }

            crewData.TryInvitePlayer(player, Main.GetPlayerByUUID(uuid));
        }

        [RemoteEvent("e-dev.crew-system.change-invited-code")]
        public void ChangeInviteCode(ExtPlayer player)
        {
            if (!player.GetPlayerCrew(out var crewData))
                return;

            crewData.TryChangeInviteCode(player);
        }
    }
}
