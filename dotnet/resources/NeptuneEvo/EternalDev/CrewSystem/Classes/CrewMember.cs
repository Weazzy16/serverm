using NeptuneEvo.EternalDev.CrewSystem.Enums;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.CrewSystem.Classes
{
    public class CrewMember
    {
        public int UUID { get; set; }
        public CrewAccess Access { get; set; }
        public string Name 
        { 
            get
            { 
                if (Main.PlayerNames.TryGetValue(UUID, out var name))
                    return name;

                return "Unknow_Unknow";
            } 
        }
        public bool IsOnline 
        { 
            get
            {
                return Player != null;
            } 
        }

        public CrewMember(int uuid, CrewAccess access)
        {
            UUID = uuid;
            Access = access;
        }

        [JsonIgnore]
        public ExtPlayer Player
        {
            get
            {
                return Main.GetPlayerByUUID(UUID);
            }
        }

        public void UpdateVariables(ExtPlayer player, bool isRemove = false)
        {
            player = player is null ? Player : player;
            if (player is null) return;

            player.SetSharedData("CREW:MEMBER", isRemove ? null : JsonConvert.SerializeObject(
                new
                {
                    Id = player.SessionData.CrewSession.CrewId,
                    Access = (int)Access
                })
            );
        }
    }
}
