using GTANetworkAPI;
using NeptuneEvo.Chars.Models;
using NeptuneEvo.Fractions;
using NeptuneEvo.Functions;
using NeptuneEvo;
using Redage.SDK;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkMethods;

namespace NeptuneEvo.AleSystems.Fishing.Classes
{
    public class FishSpot
    {
        public string Name { get; set; }
        public float Radius { get; set; }
        public List<ItemId> Fish { get; set; }
        public Vector3 Center { get; set; }
        public List<SpotPosition> Spots { get; set; } = new List<SpotPosition>();

        public FishSpot(string name, float radius, Vector3 center, List<SpotPosition> spots, List<ItemId> fish)
        {
            Name = name;
            Radius = radius;
            Center = center;
            Spots = spots;
            Fish = fish;
        }

        public void GTAElements()
        {
            NAPI.Blip.CreateBlip(68, Center, 0.77f, 4, Name, 255, 0, true, 0, 0);
            
            var colShape = CustomColShape.CreateSphereColShape(Center, Radius, 0);
            colShape.OnEntityEnterColShape += (s, e) =>
            {
                e.SetData("fish.spot", this);
                if (e.HasData("fish.spot")) {
                NAPI.ClientEvent.TriggerClientEvent(e, "client:show::FishingStatus");
                }
            };
            colShape.OnEntityExitColShape += (s, e) =>
            {
                e.ResetData("fish.spot");
                if (!e.HasData("fish.spot")) NAPI.ClientEvent.TriggerClientEvent(e, "client:close::FishingStatus");

            };
        }
    }
}
