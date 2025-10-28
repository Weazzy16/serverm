using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using Localization;
using NeptuneEvo.Character;
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using NeptuneEvo.Quests.Models;
using NeptuneEvo.VehicleData.LocalData.Models;
using Newtonsoft.Json;
using Redage.SDK;
using NeptuneEvo.Character.Models;

namespace NeptuneEvo.AleSystems
{
    public struct RentCarSpawn
    {
        public int ServiceId;
        public Vector3 Position;
        public Vector3 Rotation;

        public RentCarSpawn(int serviceId, Vector3 position, Vector3 rotation)
        {
            ServiceId = serviceId;
            Position = position;
            Rotation = rotation;
        }
    }

    public class VehicleSpawn : Script
    {
        public static readonly nLog Log = new nLog(nameof(VehicleSpawn));

        private static int SelectVehiclePosition = 0;

        public static Vector3[] AutoServicePositions = new Vector3[]
        {
            new Vector3(103.07955, -1074.3184, 29.192348),  // Автосервис 1
            new Vector3(-50.89787, -2104.142, 16.2709),    // Автосервис 2
            new Vector3(393.37677, -641.63324, 28.607044), // Автосервис 3
        };

        public static int GetNearestAutoService(Vector3 playerPos)
        {
            int nearestIndex = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < AutoServicePositions.Length; i++)
            {
                float distance = playerPos.DistanceTo(AutoServicePositions[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestIndex = i + 1; // Индексация с 1
                }
            }
            return nearestIndex;
        }

        private static RentCarSpawn[] RentCarsSpawn = new RentCarSpawn[]
        {
            // Автосервис 1
            new RentCarSpawn(1, new Vector3(105.65153, -1062.9272, 29.00295), new Vector3(-1.0429102, 0.14183156, -113.63608)),
            new RentCarSpawn(1, new Vector3(107.96851, -1059.732, 28.984015), new Vector3(-0.48314232, -0.046658315, -115.022934)),
            new RentCarSpawn(1, new Vector3(109.46743, -1056.453, 28.981308), new Vector3(-0.4545266, 0.03703493, -113.69676)),

            // Автосервис 2
            new RentCarSpawn(2, new Vector3(-45.99706, -2096.494, 16.27115), new Vector3(0.051618f, -0.05478308f, 19.47269f)),
            new RentCarSpawn(2, new Vector3(-57.41492, -2106.457, 16.27176), new Vector3(-0.05241027f, 0.06955543f, 199.8726f)),
            new RentCarSpawn(2, new Vector3(-55.92258, -2099.988, 16.27176), new Vector3(0.07695632f, -0.006316445f, 19.3454f)),

            // Автосервис 3
            new RentCarSpawn(3, new Vector3(392.59622, -638.68616, 28.500456), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(3, new Vector3(392.97382, -644.4817, 28.500397), new Vector3(0.029187638, 0.02156517, -90.404854)),
            new RentCarSpawn(3, new Vector3(392.7322, -646.966, 28.500397), new Vector3(0.029187638, 0.02156517, -90.404854)),
        };

        public static (Vector3, Vector3) GetSpawnPosition(int serviceIndex)
        {
            var spawnPoints = RentCarsSpawn.Where(spawn => spawn.ServiceId == serviceIndex).ToList();
            if (spawnPoints.Count == 0) return (new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            var positionData = spawnPoints[SelectVehiclePosition % spawnPoints.Count];
            SelectVehiclePosition++;
            return (positionData.Position, positionData.Rotation);
        }

        [ServerEvent(Event.ResourceStart)]
        public void Event_ResourceStart()
        {
            foreach (var position in AutoServicePositions)
            {
                Main.CreateBlip(new Main.BlipData(402, "Автосервис", position, 4, true));
                CustomColShape.CreateCylinderColShape(position, 2, 2, 0, ColShapeEnums.VehicleSpawn, 0);
                NAPI.Marker.CreateMarker(1, position - new Vector3(0, 0, 0.87), new Vector3(), new Vector3(), 1.0f, new Color(255, 255, 255, 150));
            }
        }

        [Interaction(ColShapeEnums.VehicleSpawn)]
        public static void OpenVehicleSpawn(ExtPlayer player, int index)
        {
            var sessionData = player.GetSessionData();
            if (sessionData == null) return;
            if (!player.IsCharacterData()) return;

            var carsData = new List<Dictionary<string, object>>();
            var vehiclesNumber = VehicleManager.GetVehiclesCarNumberToPlayer(player.Name);
            foreach (string number in vehiclesNumber)
            {
                var vehicleData = VehicleManager.GetVehicleToNumber(number);
                if (vehicleData == null) continue;
                var price = 0;

                var carData = new Dictionary<string, object>
                {
                    {"Model", vehicleData.Model},
                    {"Number", number},
                    {"IsSpawn", VehicleData.LocalData.Repository.IsVehicleToNumber(VehicleAccess.Personal, number)},
                    {"Price", price},
                };
                carsData.Add(carData);
            }

            Trigger.ClientEvent(player, "client.vehiclespawn.open", JsonConvert.SerializeObject(carsData));
        }
    }
}
