using NeptuneEvo.Core.Armor.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Core.Armor.Data
{
    public class Config
    {
        // ****************************************************
        // ИНСТРУКЦИЯ ПО ЗАПОЛНЕНИЮ
        // 
        // !!! ЗНАЧЕНИЕ КЛЮЧА (ПЕРВОЕ В СТРЧОКЕ) ДОЛЖНО БЫТЬ УНИКАЛЬНЫМ! ЕГО НУЖНО УСТАНОВИТЬ 1 РАЗ, ИНАЧЕ БУДУТ ПРОБЛЕМЫ!!!
        // 
        // параметр "Name" - название бронежилета
        // параметр "FractionId" - номер фракции, для которой этот бронежилет. Если не важно какая фракция, то ставим '0'
        // параметр "IsHeavy" - тяжелый бронежилет или нет. true - тяжелый, false - легкий
        // параметр "Drawable" - айди отображения в игре
        // параметр "Texture" - цвет бронежилета
        // параметр "Gender" - гендер. true - мужское, false - женский
        //
        // ****************************************************

        public static readonly Dictionary<string, ArmorData> ARMOR_TYPES = new Dictionary<string, ArmorData>()
        {
            { "standart.green.armor", new ArmorData(name: "[GTA] Стандартный бронежилет (Зеленый)", isHeavy: false, drawable: 1, texture: 2, gender: true) },
            { "super.grey.armor", new ArmorData(name: "[GTA] Серый Бронежилет", isHeavy: false, drawable: 60, texture: 0, gender: true) },
            { "savage.armor", new ArmorData(name: "[CUSTOM] Бронежилет Okey", isHeavy: false, drawable: 90, texture: 0, gender: true) },
            { "barbey.armor", new ArmorData(name: "[CUSTOM] Бронежилет Barbey", isHeavy: false, drawable: 91, texture: 0, gender: true) },
            { "offwhite.armor", new ArmorData(name: "[CUSTOM] Бронежилет OFF-White", isHeavy: false, drawable: 92, texture: 0, gender: true) },
            { "gift.armor", new ArmorData(name: "[CUSTOM] Бронежилет GIFT", isHeavy: false, drawable: 93, texture: 0, gender: true) },
            { "ghetto.armor", new ArmorData(name: "[CUSTOM] Бронежилет Ghetto Club", isHeavy: false, drawable: 94, texture: 0, gender: true) },
            { "dragon.armor", new ArmorData(name: "[CUSTOM] Бронежилет Dragon", isHeavy: false, drawable: 118, texture: 0, gender: true) },
            { "losangeles.armor", new ArmorData(name: "[CUSTOM] Бронежилет Los-Angeles", isHeavy: false, drawable: 72, texture: 0, gender: true) },

            { "1standart.green.armor", new ArmorData(name: "[GTA] [T] Стандартный бронежилет (Зеленый)", isHeavy: true, drawable: 1, texture: 2, gender: true) },
            { "1super.grey.armor", new ArmorData(name: "[GTA]  [T] Серый Бронежилет", isHeavy: true, drawable: 60, texture: 0, gender: true) },
            { "1savage.armor", new ArmorData(name: "[CUSTOM]  [T] Бронежилет Okey", isHeavy: true, drawable: 90, texture: 0, gender: true) },
            { "1barbey.armor", new ArmorData(name: "[CUSTOM] [T] Бронежилет Barbey", isHeavy: true, drawable: 91, texture: 0, gender: true) },
            { "1offwhite.armor", new ArmorData(name: "[CUSTOM] [T] Бронежилет OFF-White", isHeavy: true, drawable: 92, texture: 0, gender: true) },
            { "1gift.armor", new ArmorData(name: "[CUSTOM] [T] Бронежилет GIFT", isHeavy: true, drawable: 93, texture: 0, gender: true) },
            { "1ghetto.armor", new ArmorData(name: "[CUSTOM] [T] Бронежилет Ghetto Club", isHeavy: true, drawable: 94, texture: 0, gender: true) },
            { "1dragon.armor", new ArmorData(name: "[CUSTOM] [T] Бронежилет Dragon", isHeavy: true, drawable: 118, texture: 0, gender: true) },
            { "1losangeles.armor", new ArmorData(name: "[CUSTOM] [T] Бронежилет Los-Angeles", isHeavy: true, drawable: 72, texture: 0, gender: true) },
        };
                                       
        // Минимальный уровень администратора, чтобы пользоваться командой /givearmor
        public static readonly int MIN_ADMIN_LVL_TO_GIVE_ARMOR = 8;

        // Бронижилеты для фракций
        public static readonly List<ArmorData> ARMOR_FRACTION_TYPES = new List<ArmorData>()
        {
            // none frac (male)
            new ArmorData(name: "", fractionId: 9, isHeavy: true, drawable: 64, texture: 2, gender: true),
            new ArmorData(name: "", fractionId: 9, isHeavy: false, drawable: 65, texture: 2, gender: true),

            new ArmorData(name: "", fractionId: 7, isHeavy: true, drawable: 64, texture: 0, gender: true),
            new ArmorData(name: "", fractionId: 7, isHeavy: false, drawable: 65, texture: 0, gender: true),

            new ArmorData(name: "", fractionId: 6, isHeavy: true, drawable: 68, texture: 3, gender: true),
            new ArmorData(name: "", fractionId: 6, isHeavy: false, drawable: 65, texture: 0, gender: true),

            new ArmorData(name: "", fractionId: 14, isHeavy: true, drawable: 70, texture: 2, gender: true),
            new ArmorData(name: "", fractionId: 14, isHeavy: false, drawable: 71, texture: 2, gender: true),
            // none frac (female)
            new ArmorData(name: "", fractionId: 0, isHeavy: false, drawable: 1, texture: 0, gender: true),
            new ArmorData(name: "", fractionId: 0, isHeavy: true, drawable: 18, texture: 0, gender: true),
        };

        public static ArmorData GetArmor(string type)
        {
            ARMOR_TYPES.TryGetValue(type, out ArmorData armor);
            return armor;
        }
    }
}
