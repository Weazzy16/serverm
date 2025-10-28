using NeptuneEvo.Fractions.Crafting.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;
using NeptuneEvo.Chars.Models;

namespace NeptuneEvo.Fractions.Crafting.Data
{
    public class Config
    {
        // Список всех точек
        public static readonly List<CraftingTable> CRAFTING_TABLES = new List<CraftingTable>()
        {
            new CraftingTable(position: new Vector3(102.71416, -1966.6221, 20.889582), fractionId: Models.Fractions.BALLAS, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),
                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(-202.57375, -1576.572, 35.046745), fractionId: Models.Fractions.FAMILY, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),
                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(494.9449, -1536.8561, 25.872599), fractionId: Models.Fractions.BLOOD, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),
                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(1444.2715, -1488.0685, 63.62896), fractionId: Models.Fractions.VAGOS, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),
                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(961.07513, -2152.877, 31.269325), fractionId: Models.Fractions.MARABUNTA, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),
                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),

            //GOS CRAFT
            new CraftingTable(position: new Vector3(325.0452, -591.69977, 43.26127), fractionId: Models.Fractions.EMS, items: new List<CraftItem>() {
                new CraftItem(ItemId.StunGun, 150, 1),

                new CraftItem(ItemId.HealthKit, 25, 1),
                new CraftItem(ItemId.Epinephrine, 75, 1),
                new CraftItem(ItemId.Defibrillator, 1500, 1),
            }),
            // 2525.7095, -341.90482, 101.89329
            new CraftingTable(position: new Vector3(118.9617, -729.1614, 241.152), fractionId: Models.Fractions.FIB, items: new List<CraftItem>() {
                new CraftItem(ItemId.Nightstick, 200, 1),
                new CraftItem(ItemId.StunGun, 200, 1),
                new CraftItem(ItemId.HeavyPistol, 300, 1),
                new CraftItem(ItemId.Pistol50, 300, 1),
                new CraftItem(ItemId.PumpShotgun, 500, 1),
                new CraftItem(ItemId.HeavyShotgun, 600, 1),
                new CraftItem(ItemId.AssaultRifleMk2, 700, 1),
                new CraftItem(ItemId.CarbineRifleMk2, 800, 1),
                new CraftItem(ItemId.BullpupRifleMk2, 800, 1),
                new CraftItem(ItemId.MarksmanRifleMk2, 1500, 1),
                new CraftItem(ItemId.HeavySniperMk2, 1500, 1),

                new CraftItem(ItemId.BodyArmor, 200, 1, "100"),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),
                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(1402.876, 1153.0103, 114.333694), fractionId: Models.Fractions.CITY, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),

                new CraftItem(ItemId.BodyArmor, 200, 1, "100"),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),
                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            //1402.876, 1153.0103, 114.333694 POLICE
            new CraftingTable(position: new Vector3(479.0797, -996.76697, 30.69196), fractionId: Models.Fractions.POLICE, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),

                new CraftItem(ItemId.BodyArmor, 200, 1, "100"),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),
                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(1402.876, 1153.0103, 114.333694), fractionId: Models.Fractions.ARMY, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),

                new CraftItem(ItemId.BodyArmor, 200, 1, "100"),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),
                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(1402.876, 1153.0103, 114.333694), fractionId: Models.Fractions.MERRYWEATHER, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),

                new CraftItem(ItemId.BodyArmor, 200, 1, "100"),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),
                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),

            //MAFIA CRAFT
            new CraftingTable(position: new Vector3(1402.876, 1153.0103, 114.333694), fractionId: Models.Fractions.LCN, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),

                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(-101.76794, 1013.9823, 235.75327), fractionId: Models.Fractions.RUSSIAN, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),

                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(-1794.324, 428.4012, 125.68696), fractionId: Models.Fractions.ARMENIAN, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),
                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
            new CraftingTable(position: new Vector3(-1479.6038, -22.038597, 54.65404), fractionId: Models.Fractions.YAKUZA, items: new List<CraftItem>() {
                new CraftItem(ItemId.Pistol, 100, 1),
                new CraftItem(ItemId.Revolver, 300, 1),
                new CraftItem(ItemId.MiniSMG, 300, 1),
                new CraftItem(ItemId.CombatPDW, 400, 1),
                new CraftItem(ItemId.PumpShotgun, 300, 1),
                new CraftItem(ItemId.AssaultRifle, 500, 1),
                new CraftItem(ItemId.BodyArmor, 200, 1, "50"),
                new CraftItem(ItemId.Spank, 200, 1),
                new CraftItem(ItemId.HealthKit, 100, 1),
                ///ammo
                new CraftItem(ItemId.PistolAmmo, 50, 96),
                new CraftItem(ItemId.SMGAmmo, 100, 240),
                new CraftItem(ItemId.RiflesAmmo, 100, 240),
                new CraftItem(ItemId.ShotgunsAmmo, 100, 48),

                new CraftItem(ItemId.SniperAmmo, 100, 48),
            }),
        };

        public static readonly Dictionary<string, CraftItem> ORGANIZATION_CRAFTING_ITEMS = new Dictionary<string, CraftItem>()
        {
            { "Pistol", new CraftItem(ItemId.Pistol, 100, 1) },
            { "HeavyPistol", new CraftItem(ItemId.HeavyPistol, 300, 1) },
            { "MiniSMG", new CraftItem(ItemId.MiniSMG, 300, 1) },
            { "MachinePistol", new CraftItem(ItemId.MachinePistol, 300, 1) },           
            { "SMGMk2", new CraftItem(ItemId.SMGMk2, 400, 1) },
            { "CombatPDW", new CraftItem(ItemId.CombatPDW, 400, 1) },
            { "CompactRifle", new CraftItem(ItemId.CompactRifle, 400, 1) },
            { "AssaultRifle", new CraftItem(ItemId.AssaultRifle, 500, 1) },
            { "PumpShotgun", new CraftItem(ItemId.PumpShotgun, 300, 1) },
            { "Armor", new CraftItem(ItemId.BodyArmor, 200, 1, "50") },
            { "SPANK", new CraftItem(ItemId.Spank, 200, 1) },
            { "HealthKit", new CraftItem(ItemId.HealthKit, 100, 1) },

            { "PistolAmmo", new CraftItem(ItemId.PistolAmmo, 50, 96) },
            { "SMGAmmo", new CraftItem(ItemId.SMGAmmo, 100, 240) },
            { "RiflesAmmo", new CraftItem(ItemId.RiflesAmmo, 100, 240) },
            { "ShotgunsAmmo", new CraftItem(ItemId.ShotgunsAmmo, 100, 48) },
            { "SniperAmmo", new CraftItem(ItemId.SniperAmmo, 100, 48) }
        };
    }
}
