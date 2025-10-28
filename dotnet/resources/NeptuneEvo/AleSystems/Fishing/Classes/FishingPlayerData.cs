using NeptuneEvo.Character;
using NeptuneEvo.Handles;
using NeptuneEvo.AleSystems.Fishing.Data;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.AleSystems.Fishing.Classes
{
    public class FishingPlayerData
    {
        private static readonly nLog Log = new nLog(nameof(FishingPlayerData));

        public int LVL { get; set; } = 1;
        public int Progress { get; set; } = 0;
        public int Total { get; set; } = 0;

        public void AddProgress(ExtPlayer player)
        {
            try
            {
                var characterData = player.GetCharacterData();
                if (characterData is null) return;


                if (Config.EXP_LVL_DATA.TryGetValue(LVL, out int needExp))
                {
                    Progress++;
                    if (Progress >= needExp)
                    {
                        LVL++;
                        Progress = 0;
                        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, $"Поздравляем с повышением уровня рыболова - {LVL}!", 3000);
                    }
                    else
                    {
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"До достижения следующего уровня рыболова осталось выловить {needExp - Progress} рыб", 5555);
                    }
                }

                Total++;
                Save(player);
            }
            catch (Exception ex) { Log.Write("AddProgress: " + ex.ToString()); }
        }

        public void Save(ExtPlayer player)
        {
            try
            {
                MySQL.Query($"UPDATE `fishing_job` SET `lvl`={LVL}, `progress`={Progress}, `total`={Total} WHERE `uuid`={player.CharacterData.UUID}");
            }
            catch (Exception ex) { Log.Write("Save: " + ex.ToString()); }
        }
    }
}
