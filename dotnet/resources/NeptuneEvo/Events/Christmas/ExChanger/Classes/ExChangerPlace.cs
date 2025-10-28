using GTANetworkAPI;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeptuneEvo.Events.Christmas.Shop.Classes
{
    public class ExChangerPlace
    {
        private static readonly nLog Log = new nLog(nameof(ExChangerPlace));

        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public float Heading { get; set; }
        public uint PedHash { get; set; }

        public List<ExChangeItem> Items { get; set; } = new List<ExChangeItem>();

        public ExChangerPlace(string name, Vector3 position, float heading, uint pedHash, List<ExChangeItem> items)
        {
            Name = name;
            Position = position;
            Heading = heading;
            PedHash = pedHash;
            Items = items;

            ExChangerManager.Places.Add(this);
            GTAElements();
        }

        private ExtColShape _colShape { get; set; }
        private Ped _ped { get; set; }

        public void GTAElements()
        {
            _colShape = CustomColShape.CreateCylinderColShape(Position, 2, 2, 0, ColShapeEnums.ChristmasEvent_ExChanger, ExChangerManager.Places.IndexOf(this));
            _ped = NAPI.Ped.CreatePed(PedHash, Position, Heading, false, true, true, true, 0);
        }

        public void Interaction(ExtPlayer player)
        {
            try
            {
                Trigger.ClientEvent(player, "client.christmas.exChanger.open", 
                    Name, 
                    JsonConvert.SerializeObject(Items));
            }
            catch(Exception ex) { Log.Write("Interaction: " + ex.ToString()); }
        }

        public void Action(ExtPlayer player, int index)
        {
            try
            {
                var item = Items.ElementAt(index);
                if (item is null) return;

                item.Buy(player);
            }
            catch(Exception ex) { Log.Write("Action: " + ex.ToString()); }
        }
    }
}
