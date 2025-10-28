using EternalCore;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.EternalDev.CrewSystem.Classes;
using NeptuneEvo.EternalDev.CrewSystem.Configs;
using NeptuneEvo.EternalDev.CrewSystem.Extensions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Players.Models;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace NeptuneEvo.EternalDev.CrewSystem
{
    public class Manager
    {
        public static CrewConfig Config = ELib.JsonReader.Read<CrewConfig>(CrewConfig.Path);
        public static ConcurrentDictionary<int, Crew> Pool = new ConcurrentDictionary<int, Crew>();

        public static void LoadPlayer(ExtPlayer player)
        {
            var playerCrew = GetPlayerCrew(player);
            if (playerCrew is null) return;

            playerCrew.Subscribe(player);
        }

        public static void PlayerAcceptInvite(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData is null) return;

            if (player.GetPlayerCrew(out _))
            {
                Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Вы уже состоите в группе!", 3000);
                return;
            }

            ExtPlayer target = sessionData.RequestData.From;
            sessionData.RequestData = new RequestData();

            var targetSessionData = target.GetSessionData();
            if (targetSessionData is null) return;

            if (!target.GetPlayerCrew(out var targetCrew))
            {
                Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, $"Ошибка подключения к группе!", 3000);
                return;
            }

            if (targetCrew.Subscribe(player) != null)
            {
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Вы успешно вступили в группу", 3000);
            }
        }

        public static void PlayerDenyInvite(ExtPlayer player)
        {
            var sessionData = player.GetSessionData();
            if (sessionData is null) return;

            ExtPlayer target = sessionData.RequestData.From;
            sessionData.RequestData = new RequestData();

            if (target is null) return;
            Notify.Send(target, NotifyType.Info, NotifyPosition.BottomCenter, $"Игрок отказался от предложения вступить в группу", 3000);
        }

        public static bool GetCrew(int id, out Crew crew)
            => Pool.TryGetValue(id, out crew); 
        
        public static Crew GetPlayerCrew(ExtPlayer player)
        {
            if (!player.IsCharacterData())
                return null;

            return Pool.Values.ToList()
                .Find(x => x.Members.ContainsKey(player.GetUUID()));
        }

        public static string GenerateCrewInviteCode()
        {
            var alreadyExistsInvitedCodes = Pool.Values.Select(x => x.InviteCode);

            string Generate()
            {
                var builder = new StringBuilder(Config.InvitedCodeLength);

                for (int i = 0; i < Config.InvitedCodeLength; i++)
                {
                    int index = Main.rnd.Next(Config.InvitedCodePattern.Length);
                    builder.Append(Config.InvitedCodePattern[index]);
                }

                return builder.ToString();
            }

            var i = 0;
            var invitedCode = Generate();
            while (alreadyExistsInvitedCodes.Contains(invitedCode) && i <= 35)
            {
                invitedCode = Generate();
                i++;

                if (i <= 35)
                    invitedCode = string.Empty;
            }

            return invitedCode.ToString();
        }

        public static void DeleteCrew(int id)
        {
            if (Pool.TryRemove(id, out var crew))
                crew.Members.Clear();
        }

        public static int GenerateId()
            => Pool.Count == 0 ? 1 : Pool.Keys.Max() + 1;
    }
}
