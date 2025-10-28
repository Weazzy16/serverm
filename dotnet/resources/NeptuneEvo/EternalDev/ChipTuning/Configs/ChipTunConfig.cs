using GTANetworkAPI;
using NeptuneEvo.EternalDev.ChipTuning.Classes;
using NeptuneEvo.EternalDev.ChipTuning.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.EternalDev.ChipTuning.Configs
{
    public class ChipTunConfig
    {
        public static string Path => "edev-chipTuning.json";

        [JsonProperty("blip_sprite")]
        public uint BlipSprite = 446;

        [JsonProperty("blip_color")]
        public byte BlipColor = 4;

        [JsonProperty("remove-handling-price-mult")]
        public decimal RemoveHandlingPriceMultiplayer = .5m;

        [JsonProperty("vehicle-price-mult")]
        public decimal VehiclePriceMultiplayer = .1m;

        [JsonProperty("positions")]
        public ExtPosition[] Positions = new ExtPosition[] 
        {
            new ExtPosition(110.481544, 6627.144, 30.731223, -135)
        };

        [JsonProperty("handling-settings")]
        public HandlingSettingsData[] HandlingSettings { get; set; } = new HandlingSettingsData[]
        {
            new HandlingSettingsData("Привод", "fDriveBiasFront", min: 0, max: 1, step: .5m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),
            new HandlingSettingsData("Смещение тормозной силы", "fBrakeBiasFront", min: 0m, max: 1m, step: 0.05m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),
            new HandlingSettingsData("Сила ручного тормоза", "fHandBrakeForce", min: 0, max: 2, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),
            new HandlingSettingsData("Резкость входа в поворот", "fTractionCurveMax", min: 0, max: 4.95m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),
            new HandlingSettingsData("Сцепление в повороте", "fTractionCurveMin", min: 0, max: 4.95m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),
            new HandlingSettingsData("Пробуксовка на старте", "fLowSpeedTractionLossMult", min: 0.15m, max: 5, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),
            new HandlingSettingsData("Кривая потеря боковой тяги", "fTractionCurveLateral", min: 0.05m, max: 0.95m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),

            new HandlingSettingsData("Диаметр колес", "wheelRadius", min: 1, max: 1.1m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true), dependsOnDefault: true, defaultValue: 1),
            new HandlingSettingsData("Ширина колес", "wheelWidth", min: 1, max: 1.2m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true), dependsOnDefault: true, defaultValue: 1),
            new HandlingSettingsData("Развал колес", "wheelCamber", min: -0.3m, max: 0m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true), dependsOnDefault: true, defaultValue: 0),
            new HandlingSettingsData("Ширина колесной базы", "wheelTrackWidth", min: 1, max: 1.2m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true), dependsOnDefault: true, defaultValue: 0),
            new HandlingSettingsData("Высота подвески", "wheelHeight", min: -0.1m, max: 0.15m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true), dependsOnDefault: true, defaultValue: 0),
            new HandlingSettingsData("Максимальный угол поворота колес", "fSteeringLock", min: 0.3m, max: 0.9m, step: 0.01m, price: new PriceSettings(25000000, isDonate: false, dependsVehicle: true)),
        };

        [JsonProperty("controller-settings")]
        public ControllerSettingsData[] ControllerSettings { get; set; } = new ControllerSettingsData[]
        {
            new ControllerSettingsData("Переключение стабилизации Т/С", 
                "Переключатель стабилизации позволяет в ручном режиме управлять стабилизацией автомобиля на ходу, чтобы уходить в занос или восстанавливать сцепление с покрытием", 
                ChipControllers.StabilizationSwitching,
                price: new PriceSettings(200000000, isDonate: true, dependsVehicle: true)),
        };
    }
}
