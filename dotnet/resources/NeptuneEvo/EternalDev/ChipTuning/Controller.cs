using EternalCore;
using GTANetworkAPI;
using NeptuneEvo.EternalDev.ChipTuning.Classes;
using NeptuneEvo.EternalDev.ChipTuning.Configs;
using NeptuneEvo.EternalDev.ChipTuning.Enums;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning
{
    public class Controller : Script
    {
        public static readonly ChipTunConfig Config = ELib.JsonReader.Read<ChipTunConfig>(ChipTunConfig.Path);

        public static ConcurrentDictionary<int, ChipVehicleData> ChipVehicles = new ConcurrentDictionary<int, ChipVehicleData>();
        public static ConcurrentDictionary<int, ChipPlace> ChipPlaces = new ConcurrentDictionary<int, ChipPlace>();

        [ServerEvent(Event.ResourceStart)]
        public void OnStart()
        {
            var sqlData = MySQL.QueryRead("SELECT * FROM `e-dev_chiptuning`");
            if (sqlData is null || sqlData.Rows.Count == 0)
            {
                ELib.Logger.Info("ChipTuning database is empty");
            }
            else
            {
                foreach (DataRow row in sqlData.Rows)
                {
                    var data = new ChipVehicleData()
                    {
                        VehicleId = Convert.ToInt32(row["vehicleId"]),
                        Handlings = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(row["handlings"].ToString()),
                        Controllers = JsonConvert.DeserializeObject<Dictionary<string, bool>>(row["controllers"].ToString()),
                    };

                    ChipVehicles.TryAdd(data.VehicleId, data);
                }
            }

            for (int index = 0; index < Config.Positions.Length; index++)
            {
                var place = new ChipPlace(index, Config.Positions[index]);
                place.GTAElements();

                ChipPlaces.TryAdd(index, place);
            }

            ELib.Logger.Done($"ChipTuning successfuly loaded ({ChipVehicles.Count})", nameof(Controller));
        }

        [Interaction(ColShapeEnums.ChipTuning)]
        public void OnInteraction(ExtPlayer player, int id)
        {
            if (!ChipPlaces.TryGetValue(id, out var place))
                return;

            place.Interaction(player);
        }

        public static void SaveOrCreate(ChipVehicleData chipVehicleData)
        {
            if (!ChipVehicles.ContainsKey(chipVehicleData.VehicleId))
            {
                MySQL.Query($"INSERT INTO `e-dev_chiptuning` (`vehicleId`, `handlings`, `controllers`) " +
                    $"VALUES({chipVehicleData.VehicleId}, '{JsonConvert.SerializeObject(chipVehicleData.Handlings)}', '{JsonConvert.SerializeObject(chipVehicleData.Controllers)}')");
                ChipVehicles.TryAdd(chipVehicleData.VehicleId, chipVehicleData);
            }
            else
            {
                MySQL.Query($"UPDATE `e-dev_chiptuning` SET `handlings`='{JsonConvert.SerializeObject(chipVehicleData.Handlings)}', `controllers`='{JsonConvert.SerializeObject(chipVehicleData.Controllers)}' " +
                    $"WHERE `vehicleId`={chipVehicleData.VehicleId}");
            }
        }

        public static ChipVehicleData GetChipVehicleData(int vehicleId)
        {
            ChipVehicles.TryGetValue(vehicleId, out var data);
            return data;
        }

        public static void ApplyVehicleData(ExtVehicle vehicle, int vehicleId)
        {
            var chipData = GetChipVehicleData(vehicleId);
            if (chipData is null) return;

            vehicle.SetSharedData("e-dev.chipTuning.handlings", JsonConvert.SerializeObject(chipData.Handlings));
            vehicle.SetSharedData("e-dev.chipTuning.controllers", JsonConvert.SerializeObject(chipData.Controllers));
        }
    }
}
