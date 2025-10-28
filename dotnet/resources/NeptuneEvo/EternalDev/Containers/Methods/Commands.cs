using System;
using GTANetworkAPI;
using NeptuneEvo.EternalDev.Containers.Data;
using Redage.SDK;
using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using EternalCore;

namespace NeptuneEvo.EternalDev.Containers.Methods
{
    public class Commands : Script
    {
        [Command(Config.COMMAND_START)]
        public void OnCommand_StartAuction(ExtPlayer player)
        {
            try
            {
                var character = player.GetCharacterData();
                if (character is null || character.AdminLVL < Config.ADMIN_LVL) 
                    return;

                Manager.TryStart();
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(OnCommand_StartAuction), ex, nameof(Commands)); }
        }
    }
}
