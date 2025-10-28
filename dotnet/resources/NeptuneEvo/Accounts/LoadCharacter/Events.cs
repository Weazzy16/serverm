using GTANetworkAPI;
using NeptuneEvo.Handles;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Accounts.LoadCharacter
{
    public class Events : Script
    {
        [RemoteEvent("server.accounts.load_character")]
        public void TryLoad(ExtPlayer player)
        {
            Repository.Load(player, DateTime.Now);
        }
    }
}
