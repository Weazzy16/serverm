// Minimal local store used by SettingItem for demo/testing.
import { storeSettings } from 'store/settings';


const meta = {
  // meta for settings used by SettingItem
  language: { title: "Язык", type: "selector", list: [{ value: "ru", label: "Русский" }, { value: "en", label: "English" }] },
  schat: { title: "Чат", type: "checkbox" },
  hud: { title: "Игровой интерфейс", type: "checkbox" },
  radar: { title: "Карта", type: "checkbox" },
  ids: { title: "ID над игроками", type: "checkbox" },
  compass: { title: "Компас", type: "checkbox" },
  captureHud: { title: "Интерфейс сражения за территорию", type: "selector", list: [{ value: "dark", label: "Тёмный" }, { value: "light", label: "Светлый" }] },
  driftCounter: { title: "Дрифт очки", type: "checkbox" },
  showHotkeys: { title: "Отображение подсказок", type: "selector", list: [{ value: "no", label: "Нет" }, { value: "yes", label: "Да" }] },
  speedoType: { title: "Спидометр", type: "selector", list: [{value: "off", label: "Выключен"}, { value: "simple", label: "Расширенный (1)" }, { value: "extended", label: "Расширенный (2)" }, {value: "min2", label: "Минималистичный (1)"}, {value: "min2", label: "Минималистичный (2)"},{value: "min3", label: "Минималистичный (3)"}] },
  ultraWideMode: { title: "Ультра широкий режим", type: "checkbox" },
  speedoUpdate: { title: "Скорость обновления спидометра", type: "selector", list: [{ value: "mental", label: "Моментально" }, { value: "fast", label: "Быстро" }, { value: "normal", label: "Нормально" },{ value: "slow", label: "Медленно" }] },
  viewTheme: { title: "Тема интерфейса", type: "selector", list: [{ value: "dark", label: "Тёмная" }, { value: "light", label: "Светлая" }] },

  hitmarker: { title: "Хитмаркер", type: "checkbox" },
  streamerMode: { title: "Режим стримера", type: "checkbox" },
  displayChatMsg: { title: "Сообщения над игроками", type: "checkbox" },
  chatPunish: { title: "Наказания в чат", type: "checkbox" },
  walkStyle: { title: "Стиль походки", type: "selector", list: [ { value: "normal", label: "Стандартный" }, { value: "proud", label: "Гордый" }, { value: "confident", label: "Уверенный" }, { value: "drunk", label: "Пьяный" },  { value: "fast", label: "Быстрый" },  { value: "slow", label: "Медленный" },   { value: "fat", label: "Толстый" }, { value: "gangster", label: "Гангстер" }, { value: "hasty", label: "Торопливый" }, { value: "injured", label: "Раненый" }, { value: "timid", label: "Пугливый" }, { value: "sad", label: "Печальный" }, { value: "tough", label: "Жесткий" }, { value: "showoff", label: "Хвастун" }, { value: "tramp", label: "Бродяга" }, { value: "sexy", label: "Сексуальный" }, { value: "hiking", label: "Пешийтуризм" }] },
  vehicleEntering: { title: "Тип посадки", type: "selector", list: [{ value: "driver_priority", label: "Приоритет водителя" }, { value: "passenger_priority", label: "Ближайшая дверь" }, { value: "fast_priority", label: "Разные кнопки" }] },
  facialAnim: { title: "Эмоция лица", type: "selector", list: [{value:"standard",label:"Стандартный"},{value:"smug",label:"Самодовольный"},{value:"tense",label:"Напряженный"},{value:"driver",label:"Водила"},{value:"skydiver",label:"Скайдайвер"},{value:"injured",label:"Раненый"},{value:"angry",label:"Злой"},{value:"drunk",label:"Пьяный"},{value:"happy",label:"Счастливый"},{value:"pouty",label:"Дутый"},{value:"focused",label:"Сконцентрированный"},{value:"shocked1",label:"Шокированный 1"},{value:"shocked2",label:"Шокированный 2"},{value:"sleepy",label:"Сонный"},{value:"beaten",label:"Сбитый"},{value:"dead1",label:"Мертвый 1"},{value:"dead2",label:"Мертвый 2"}] },
  integratedFont: { title: "Общий шрифт", type: "selector", list: [{ value: "ProximaNova", label: "ProximaNova" }, { value: "Arial", label: "Arial" }] },
  cruiseType: { title: "Тип круиз-контроля", type: "selector", list: [{ value: "speed_limit", label: "Огран. скорости" }, { value: "adaptive", label: "Обычный" }] },


  generalSoundVol: { title: "Громкость звука", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
  voiceChatVol: { title: "Громкость голосового чата игроков", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
  ambientMusicVol: { title: "Громкость музыки окружения", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
  animMusicVol: { title: "Громкость музыки анимаций", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
  radioVol: { title: "Громкость радио", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
  walkieSoundVol: { title: "Громкость рации", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
  notificationVol: { title: "Уведомление о репорте", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
    voiceChatBlock: { title: "Блокировка голосового чата игроков", type: "selector", list: [{ value: "no", label: "Нет" }, { value: "yes", label: "Да" }] },
  voiceChatOffDelay: { title: "Задержка отключения голосового чата", type: "selector", list: [{ value: "0", label: "0 ms" }, { value: "200", label: "200 ms" }, { value: "500", label: "500 ms" }] },
  micSensitivity: { title: "Чувствительность микрофона", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },
  cinemaVolume: { title: "Громкость кинотеатра", type: "range", sliderOptions: { min: 0, max: 100, interval: 1 } },

  animMusicToggle: { title: "Музыка анимации", type: "checkbox" },
  ambientMusicToggle: { title: "Музыка окружения", type: "checkbox" },
  accessorySounds: { title: "Звуки аксессуаров", type: "checkbox" },
  navigatorVoice: { title: "Озвучка навигатора", type: "checkbox" },
  radarSound: { title: "Звук радара", type: "checkbox" },
  seatbeltWarn: { title: "Предупреждение о ремне безопасности", type: "checkbox" },
  hitmarkerSound: { title: "Звук хитмаркера", type: "checkbox" },
    listeningMode: { title: "Слух", type: "selector", list: [{ value: "standard", label: "Стандартный" }, { value: "enhanced", label: "Расширенный" }] },
  voiceActivationMode: { title: "Режим активации голосового чата", type: "selector", list: [{ value: "push", label: "По нажатию" }, { value: "voice", label: "Голосом" }] },
  noiseSuppression: { title: "Шумоподавление", type: "checkbox" },



  // --- game chat: admin flags (checkboxes) ---
  adminDeleteWeazelNews: { title: "Удалять Weazel News", type: "checkbox" },
  adminDeleteMarketplace: { title: "Удалять объявления", type: "checkbox" },
  adminBigChipsExchange: { title: "Обмен больших фишек", type: "checkbox" },
  adminLargeMoneyTransfer: { title: "Переводы крупных сумм", type: "checkbox" },
  adminFamilyBonus: { title: "Семейные бонусы", type: "checkbox" },
  adminMassBonus: { title: "Массовые бонусы", type: "checkbox" },
  adminCaptZoneColorChange: { title: "Смена цвета зоны захвата", type: "checkbox" },
  adminFractionBonus: { title: "Фракционные бонусы", type: "checkbox" },
  adminFractionFunding: { title: "Финансирование фракций", type: "checkbox" },
  adminCharacterAppearanceChange: { title: "Изменение внешности", type: "checkbox" },
  adminCharacterNameChange: { title: "Смена имени персонажа", type: "checkbox" },
  adminAntiCheatBan: { title: "Античит бан", type: "checkbox" },
  adminSystemShutdown: { title: "Выключение системы", type: "checkbox" },
  adminAdminLeakBan: { title: "Блокировка по утечке админов", type: "checkbox" },
  adminForbiddenAnimation: { title: "Запрет анимаций", type: "checkbox" },
  adminSuspiciousDonateActivity: { title: "Подозрительные донаты", type: "checkbox" },
  adminForbiddenSoftware: { title: "Запрет ПО", type: "checkbox" },
  adminStungunUsage: { title: "Использование тазера", type: "checkbox" },
  adminAimUsage: { title: "Использование прицела", type: "checkbox" },

  // --- chat settings meta ---

chatWidth: { title: "Ширина блока чата", type: "range", sliderOptions: { min: 200, max: 800, interval: 10 } },
chatHeight: { title: "Высота блока чата", type: "range", sliderOptions: { min: 100, max: 800, interval: 10 } },
chatSettings: { 
    title: "Настройки", 
    type: "selector", 
    list: [
      { value: "standard", label: "Стандартные" }, 
      { value: "custom", label: "Пользовательский" }
    ] 
  },
  chatTheme: { 
    title: "Тема", 
    type: "selector", 
    list: [
      { value: "Clean", label: "Clean" }, 
      { value: "Block", label: "Block" }
    ] 
  },
  chatFont: { 
    title: "Шрифт", 
    type: "selector", 
    list: [
      { value: "ProximaNova", label: "Proxima Nova" }, 
      { value: "ProximaNova-Condensed", label: "PNova Condensed" },
      { value: "ProximaNova-ExtraCondensed", label: "PNova ExtraCondensed" },
      { value: "San-Francisco", label: "San Francisco" }
    ] 
  },
  chatFontStyle: { 
    title: "Начертание", 
    type: "selector", 
    list: [
      { value: "Light", label: "Light" }, 
      { value: "Regular", label: "Regular" },
      { value: "Semibold", label: "Semibold" },
      { value: "Bold", label: "Bold" }
    ] 
  },
  chatBorderType: { 
    title: "Тип внешней границы", 
    type: "selector", 
    list: [
      { value: "Stroke", label: "Обводка" }, 
      { value: "Shadow", label: "Тень" }
    ] 
  },
  chatTimestamp: { 
    title: "Время в чате", 
    type: "selector", 
    list: [
      { value: "No", label: "Нет" }, 
      { value: "Time", label: "Время" },
      { value: "DateTime", label: "Дата и время" }
    ] 
  },
  chatFontSize: { 
    title: "Размер шрифта", 
    type: "range", 
    sliderOptions: { min: 12, max: 30, interval: 1 } 
  },
  chatFontShadow: { 
    title: "Размер тени", 
    type: "range", 
    sliderOptions: { min: 0, max: 10, interval: 1 } 
  },
  chatFontShadowAlpha: { 
    title: "Прозрачность тени", 
    type: "range", 
    sliderOptions: { min: 0, max: 100, interval: 1 } 
  },
  chatLinesMargin: { 
    title: "Отступ между сообщениями", 
    type: "range", 
    sliderOptions: { min: 0, max: 100, interval: 1 } 
  },
  chatLineHeight: { 
    title: "Мех строчное расстояние", 
    type: "range", 
    sliderOptions: { min: 0, max: 10, interval: 1 } 
  },
  
  chatCmdsSettings: { 
    title: "Настройки быстрых команд", 
    type: "selector", 
    list: [
      { value: "standard", label: "Стандартные" }, 
      { value: "custom", label: "Пользовательский" }
    ] 
  },
  chatCmdsFont: { 
    title: "Шрифт быстрых команд", 
    type: "selector", 
    list: [
      { value: "ProximaNova", label: "ProximaNova" }, 
      { value: "Arial", label: "Arial" }
    ] 
  },
  chatCmdsFontStyle: { 
    title: "Начертание команд", 
    type: "selector", 
    list: [
      { value: "Light", label: "Light" }, 
      { value: "Regular", label: "Regular" },
      { value: "Semibold", label: "Semibold" },
      { value: "Bold", label: "Bold" }
    ] 
  },
  chatCmdsFontSize: { 
    title: "Размер шрифта команд", 
    type: "range", 
    sliderOptions: { min: 10, max: 24, interval: 1 } 
  },

  };
// API - используем storeSettings из store/settings
export function getSettingItem(key) {
  return meta[key] ?? null;
}

export function getSettingValue(key) {
  let val;
  storeSettings.subscribe(v => val = v[key])();
  return val;
}

export function updateSettingValue(key, value) {
  storeSettings.update(v => ({
    ...v,
    [key]: value
  }));
}