using EternalCore;
using GTANetworkAPI;
using NeptuneEvo.EternalDev.VehiclePetrol.Configs;
using NeptuneEvo.EternalDev.VehiclePetrol.Enums;
using Newtonsoft.Json;
using System;

namespace NeptuneEvo.EternalDev.VehiclePetrol
{
    public class Controller : Script
    {
        #region Author @merumond
        // ----------------------------------------------------------------------------------- //
        // |                                                                                 | //
        // |     ███╗░░░███╗███████╗██████╗░██╗░░░██╗███╗░░░███╗░█████╗░███╗░░██╗██████╗░    | //
        // |     ████╗░████║██╔════╝██╔══██╗██║░░░██║████╗░████║██╔══██╗████╗░██║██╔══██╗    | //
        // |     ██╔████╔██║█████╗░░██████╔╝██║░░░██║██╔████╔██║██║░░██║██╔██╗██║██║░░██║    | //
        // |     ██║╚██╔╝██║██╔══╝░░██╔══██╗██║░░░██║██║╚██╔╝██║██║░░██║██║╚████║██║░░██║    | //
        // |     ██║░╚═╝░██║███████╗██║░░██║╚██████╔╝██║░╚═╝░██║╚█████╔╝██║░╚███║██████╔╝    | //
        // |     ╚═╝░░░░░╚═╝╚══════╝╚═╝░░╚═╝░╚═════╝░╚═╝░░░░░╚═╝░╚════╝░╚═╝░░╚══╝╚═════╝░    | //
        // |                                                                                 | //
        // ----------------------------------------------------------------------------------- //
        #endregion
        
        public static VehiclesPetrolConfig Config = ELib.JsonReader.Read<VehiclesPetrolConfig>(VehiclesPetrolConfig.Path);

        /// <summary>
        /// Проверяет возможно ли заправить транспорт по названию модели
        /// </summary>
        /// <param name="modelName">Название модели транспорта</param>
        /// <param name="petrolType">Препологаемый тип топлива</param>
        /// <returns></returns>
        public static bool CanRefuel(string modelName, PetrolType petrolType)
        {
            if (!Config.IsEnabled) 
                return true;

            return GetValidPetrol(modelName) == petrolType;
        }

        /// <summary>
        /// Проверяет возможно ли заправить транспорт по хэшу модели
        /// </summary>
        /// <param name="modelName">Хэш модели транспорта</param>
        /// <param name="petrolType">Препологаемый тип топлива</param>
        /// <returns></returns>
        public static bool CanRefuel(uint model, PetrolType petrolType)
        {
            if (!Config.IsEnabled)
                return true;

            return GetValidPetrol(model) == petrolType;
        }

        /// <summary>
        /// Получить тип топлива, который подходит названии модели транспорта
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns>Подходящий тип топлива</returns>
        public static PetrolType GetValidPetrol(string modelName)
        {
            if (!Config.Data.TryGetValue(modelName, out var petrolType))
                return Config.DefaultType;

            return petrolType;
        }

        /// <summary>
        /// Получить тип топлива, который подходит хэшу модели транспорта
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns>Подходящий тип топлива</returns>
        public static PetrolType GetValidPetrol(uint model)
        {
            foreach (var vehicleSettings in Config.Data)
            {
                if (NAPI.Util.GetHashKey(vehicleSettings.Key) != model)
                    continue;

                return vehicleSettings.Value;
            }

            return Config.DefaultType;
        }
    }
}
