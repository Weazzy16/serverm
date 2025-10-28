using GTANetworkAPI;
using NeptuneEvo.Chars.Models;
using System.Collections.Generic;
using System.Linq;

public enum CalendarRewardType
{
    Money,
    Item,
    Vehicle,
    Donate,
    None
}

public class CalendarTaskInfo
{
    public int TaskId { get; set; }
    public int Month { get; set; }
    public int DayInMonth { get; set; }
    public List<int> RequiredTasks { get; set; }   // вот оно

    public int BotId { get; set; }
    public string BotName { get; set; }
    public string Title { get; set; }
    public int Level { get; set; }
    public string Description { get; set; }
    public string QuestType { get; set; }
    public string RequirementTitle { get; set; }
    public int RequirementCount { get; set; }
    public CalendarRewardType RewardType { get; set; }
    public int RewardCount { get; set; }
    public int RewardItemId { get; set; }
    public VehicleHash RewardVehicle { get; set; }
}

public class CalendarRewardInfo
{

    public int Month { get; set; }            // 1–12
    // День в этом месяце
    public int DayInMonth { get; set; }       // 1–31

    // Инфо для отображения
    public string Title { get; set; }
    public string Description { get; set; }
    public string QuestType { get; set; }

    // Список задач, которые нужно выполнить, чтобы снять награду
    public List<int> RequiredTasks { get; set; }

    // Что даётся в награду
    public CalendarRewardType RewardType { get; set; }
    public int RewardCount { get; set; }
    public int RewardItemId { get; set; }
    // (если нужен транспорт)
    public VehicleHash RewardVehicle { get; set; }
    public int RequirementCount { get; internal set; }

    public CalendarRewardInfo()
    {
        RequiredTasks = new List<int>();
    }
}

public static class CalendarTasksDefinition
{
    /// <summary>
    /// Плоский список всех записей (сохранили для обратной совместимости).
    /// </summary>

    // Здесь теперь собираем в один список сразу все задачи.
    public static readonly List<CalendarTaskInfo> All = new List<CalendarTaskInfo>()
    {
        // Компактные инициализаторы CalendarTaskInfo
new CalendarTaskInfo { TaskId = 0, BotId = 9, BotName = "Beatrice Franks", Level = 3, Title = "Дрифтер", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Соберите очки в любой зоне дрифта", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 1, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Удочки и спокойствие", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Поймайте рыбу", RequirementCount = 20 },
new CalendarTaskInfo { TaskId = 2, BotId = 6, BotName = "Rudolph Burnett", Level = 2, Title = "Приветствие игроков", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Поздоровайтесь с игроками", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 3, BotId = 8, BotName = "Cameron Whitney", Level = 2, Title = "Пиротехник", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Установите заряды фейерверка", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 4, BotId = 18, BotName = "Mollie Malone", Level = 2, Title = "Любитель ставок в рулетке", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Выиграйте ставки в рулетке в казино", RequirementCount = 20 },
new CalendarTaskInfo { TaskId = 5, BotId = 18, BotName = "Mollie Malone", Level = 3, Title = "Любитель ставок в блекджеке", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Выиграйте ставки в блекджек в казино", RequirementCount = 15 },
new CalendarTaskInfo { TaskId = 6, BotId = 18, BotName = "Mollie Malone", Level = 2, Title = "Любитель ставок в скачках", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Выиграйте ставки в скачках в казино", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 7, BotId = 18, BotName = "Mollie Malone", Level = 3, Title = "Любитель ставок в игральных автоматах", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Выиграйте ставки в игральных автоматах в казино", RequirementCount = 15 },
new CalendarTaskInfo { TaskId = 8, BotId = 20, BotName = "Frederick Summers", Level = 3, Title = "Кладоискатель", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Найдите и выкопайте клад используя металлодетектор", RequirementCount = 3 },
new CalendarTaskInfo { TaskId = 9, BotId = 20, BotName = "Frederick Summers", Level = 3, Title = "Дальнобойщик", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Завершите рейсы на работе дальнобойщика", RequirementCount = 15 },
new CalendarTaskInfo { TaskId = 10, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Грибник", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Соберите грибы в лесу", RequirementCount = 60 },
new CalendarTaskInfo { TaskId = 11, BotId = 5, BotName = "Otis Knight", Level = 3, Title = "Дровосек", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Срубите брёвна в лесу", RequirementCount = 20 },
new CalendarTaskInfo { TaskId = 12, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Почтальон", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Завершите рейсы на работе почтальона", RequirementCount = 15 },
new CalendarTaskInfo { TaskId = 13, BotId = 20, BotName = "Frederick Summers", Level = 3, Title = "Мусоровоз", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Завершите рейсы на работе мусоровоза", RequirementCount = 90 },
new CalendarTaskInfo { TaskId = 14, BotId = 9, BotName = "Beatrice Franks", Level = 3, Title = "Игрок в миниигры", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Сыграйте миниигры в Arena War", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 15, BotId = 8, BotName = "Cameron Whitney", Level = 2, Title = "Сильные руки", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Воспользуйтесь качалкой", RequirementCount = 20 },
new CalendarTaskInfo { TaskId = 16, BotId = 8, BotName = "Cameron Whitney", Level = 2, Title = "Большое кино", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Включите любой видеоролик в Кинотеатре", RequirementCount = 3 },
new CalendarTaskInfo { TaskId = 17, BotId = 20, BotName = "Frederick Summers", Level = 2, Title = "Чистота и порядок", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Воспользоваться услугами автомойки", RequirementCount = 3 },
new CalendarTaskInfo { TaskId = 18, BotId = 8, BotName = "Cameron Whitney", Level = 2, Title = "Шашлындос", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Пожарьте что-то на гриле в дали от магазина 24/7. Например рыбу", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 19, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Фермер", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Соберите любой урожай на ферме", RequirementCount = 140 },
new CalendarTaskInfo { TaskId = 20, BotId = 8, BotName = "Cameron Whitney", Level = 2, Title = "Таксист", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Завершите рейсы на работе таксиста", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 21, BotId = 8, BotName = "Cameron Whitney", Level = 2, Title = "Водитель автобуса", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Завершите рейсы на работе водителя автобуса", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 23, BotId = 8, BotName = "Cameron Whitney", Level = 2, Title = "Обновление авто", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Установите любые детали тюнинга в авто", RequirementCount = 3 },
new CalendarTaskInfo { TaskId = 25, BotId = 20, BotName = "Frederick Summers", Level = 3, Title = "Парашютист", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Раскройте парашют паря в воздухе", RequirementCount = 10 },
new CalendarTaskInfo { TaskId = 26, BotId = 20, BotName = "Frederick Summers", Level = 2, Title = "Армрестлинг", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Сыграйте в платный армрестлинг", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 27, BotId = 20, BotName = "Frederick Summers", Level = 2, Title = "Я почти миллионер", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Купите лотерейный билет в магазине 24/7", RequirementCount = 1 },
new CalendarTaskInfo { TaskId = 28, BotId = 20, BotName = "Frederick Summers", Level = 2, Title = "Не протухло, можно есть", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Съешьте шаурму без отравления", RequirementCount = 3 },
new CalendarTaskInfo { TaskId = 29, BotId = 20, BotName = "Frederick Summers", Level = 3, Title = "Первая с хрустом, вторая с буксом", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Толкните несколько автомобилей", RequirementCount = 10 },
new CalendarTaskInfo { TaskId = 30, BotId = 5, BotName = "Otis Knight", Level = 2, Title = "Вы водитель BMW?", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Замените масло в автомобиле", RequirementCount = 1 },
new CalendarTaskInfo { TaskId = 31, BotId = 5, BotName = "Otis Knight", Level = 2, Title = "Вы водитель Tesla?", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Замените аккумулятор в автомобиле", RequirementCount = 1 },
new CalendarTaskInfo { TaskId = 32, BotId = 5, BotName = "Otis Knight", Level = 3, Title = "Бомж", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Посмотрите содержимое мусорок", RequirementCount = 20 },
new CalendarTaskInfo { TaskId = 33, BotId = 18, BotName = "Mollie Malone", Level = 3, Title = "Любитель Слёрма", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Выпейте напитки из торговых автоматов", RequirementCount = 10 },
new CalendarTaskInfo { TaskId = 34, BotId = 18, BotName = "Mollie Malone", Level = 2, Title = "Милота", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Погладьте животное", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 35, BotId = 18, BotName = "Mollie Malone", Level = 2, Title = "Вы теперь одеты, как черт", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Купите что-нибудь в магазине одежды", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 36, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Работяга", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Заработайте деньги на работе (где можно устроиться)", RequirementCount = 5000 },
new CalendarTaskInfo { TaskId = 37, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Водитель автобуса", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Работайте водителем автобуса", RequirementCount = 35 },
new CalendarTaskInfo { TaskId = 38, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Подход с умом", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Замерьте разгон авто используя предмет Dragy", RequirementCount = 1 },
new CalendarTaskInfo { TaskId = 39, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Просто так не сижу", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Получите пособие по безработице/зарплату в государственной фракции в PayDay", RequirementCount = 2 },
new CalendarTaskInfo { TaskId = 40, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Водила", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Соберите километраж на транспорте", RequirementCount = 5 },
new CalendarTaskInfo { TaskId = 41, BotId = 6, BotName = "Rudolph Burnett", Level = 3, Title = "Шахтёр", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Собрать сырьё на шахте", RequirementCount = 50 },
new CalendarTaskInfo { TaskId = 42, BotId = 9, BotName = "Beatrice Franks", Level = 2, Title = "Киборг - убийца", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание",  RequirementTitle = "Убейте игроков на арене Maze Bank Arena в режиме \"Бой насмерть\" или \"Гонка вооружений\"", RequirementCount = 20 },
new CalendarTaskInfo { TaskId = 43, BotId = 9, BotName = "Beatrice Franks", Level = 2, Title = "Самая быстрая рука", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание",  RequirementTitle = "Убейте игрока из револьвера на арене в режиме \"Бой насмерть\" или \"Гонка вооружений\" на арене Maze Bank Arena", RequirementCount = 20 },
new CalendarTaskInfo { TaskId = 44, BotId = 5, BotName = "Rudolph Franks", Level = 2, Title = "Будущее наступило", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Воспользуйтесь автопилотом", RequirementCount = 1 },
new CalendarTaskInfo { TaskId = 45, BotId = 4, BotName = "Rudolph Franks", Level = 3, Title = "Дикарь", Description = "Для разблокировки награды из раздела «Календарь», Вам необходимо выполнить все ежедневные задания.", QuestType = "Ежедневное задание", RequirementTitle = "Употребите сырое мясо дичи", RequirementCount = 3 },


        // …
    };


    // Расписание наград: 12 месяцев, RequiredTasks случайные
    public static readonly Dictionary<int, List<CalendarRewardInfo>> Monthly
         = new Dictionary<int, List<CalendarRewardInfo>>()
         {
        [1] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 0, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Biolink
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 0, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.HairStyling
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 27, 2 },
            RewardType   = CalendarRewardType.Donate,
            RewardCount  = 50,
            
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 4,
          
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case0
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 5,
   
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.HealthKit
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 21, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.ImmortalitixPill
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 44, 1 },
            RewardType   = CalendarRewardType.Donate,
            RewardCount  = 200,
           
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 38, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 4, 38 },
            RewardType   = CalendarRewardType.Money,
            RewardCount  = 7500,
           
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 29, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 7, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.HealthKit
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 21, 33 },
            RewardType   = CalendarRewardType.Money,
            RewardCount  = 7500,
            
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 11, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 3, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 28, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 41, 7 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 21, 8 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 2, 38 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 32, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 33, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 11, 9 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 14, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 23, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 24, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 15, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 32, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 7, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 34, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 7, 40 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 15, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 1,
            DayInMonth   = 31,
            RequiredTasks = new List<int> { 2, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [2] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 13, 18 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case10
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 11, 10 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 22, 20 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 3, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 6, 44 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 40, 10 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 42, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 22, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 24, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 20, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 38, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 17, 9 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 27, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 1, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 1, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 17, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 2, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 32, 31 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 10, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 26, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 15, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 10, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 8, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 36, 38 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 43, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 39, 9 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 33, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 2,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 16, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [3] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 28, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case13
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 17, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 21, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 44, 23 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 4, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 33, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 3, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 0, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 5, 20 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 40, 29 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 25, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 21, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 5, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 37, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 30, 44 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 40, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 14, 36 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 27, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 45, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 4, 29 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 38, 18 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 0, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 9, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 7, 13 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 13, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 23, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 32, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 14, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 31, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 38, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 3,
            DayInMonth   = 31,
            RequiredTasks = new List<int> { 43, 20 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [4] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 7, 15 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case15
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 38, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 12, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 6, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 29, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 29, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 8, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 40, 13 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 30, 8 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 22, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 22, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 22, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 12, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 38, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 19, 18 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 29, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 4, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 39, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 33, 13 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 1, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 26, 9 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 39, 1 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 13, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 2, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 32, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 25, 27 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 17, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 13, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 5, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 4,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 17, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [5] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 26, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case16
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 39, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 33, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 23, 1 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 12, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 42, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 44, 19 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 8, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 24, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 3, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 27, 20 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 32, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 32, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 13, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 32, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 13, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 8, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 31, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 31, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 43, 26 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 24, 36 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 12, 8 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 5, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 22, 25 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 42, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 16, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 16, 44 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 33, 7 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 36, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 2, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 5,
            DayInMonth   = 31,
            RequiredTasks = new List<int> { 43, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [6] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 29, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case16
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 43, 18 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 29, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 34, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 23, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 36, 25 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 0, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 8, 38 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 45, 8 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 23, 7 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 38, 40 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 41, 19 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 38, 20 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 5, 13 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 40, 19 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 22, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 27, 13 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 18, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 23, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 7, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 1, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 14, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 36, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 43, 25 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 45, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 29, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 28, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 12, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 7, 15 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 6,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 42, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [7] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 0, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 42, 23 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 1, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 4,
            
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 5,
            
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 15, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 28, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 4, 11 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 5, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 18, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 16, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 16, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 45, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 6, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 14, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 31, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 39, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 33, 15 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 19, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 1, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 25, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 9, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 21, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 30, 29 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 26, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 44, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 36, 23 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 38, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 17, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 18, 29 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 7,
            DayInMonth   = 31,
            RequiredTasks = new List<int> { 5, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [8] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 5, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 17, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 10, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 39, 9 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 32, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 45, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 11, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 35, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 33, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 16, 29 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 23, 38 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 12, 41 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 11, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 22, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 35, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 4, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 42, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 34, 44 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 8, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 14, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 14, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 16, 21 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 3, 19 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 9, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 23, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 36, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 11, 46 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 6, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 10, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 8, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 8,
            DayInMonth   = 31,
            RequiredTasks = new List<int> { 37, 20 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [9] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 1, 31 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 13, 8 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 19, 29 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 16, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 7, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 30, 23 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 2, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 37, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 6, 7 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 25, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 3, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 25, 40 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 22, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 31, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 28, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 40, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 0, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 38, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 6, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 38, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 12, 9 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 14, 26 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 26, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 29, 31 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 30, 10 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 9, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 16, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 19, 44 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 30, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 9,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 44, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [10] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 41, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 4, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 6, 26 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 36, 42 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 34, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 29, 11 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 13, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 16, 23 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 26, 36 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 6, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 44, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 38, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 40, 20 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 21, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 23, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 24, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 11, 6 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 12, 38 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 30, 7 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 13, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 34, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 30, 10 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 21, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 20, 7 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 20, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 7, 10 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 25, 4 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 22, 15 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 6, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 44, 12 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 10,
            DayInMonth   = 31,
            RequiredTasks = new List<int> { 14, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [11] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 16, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 23, 36 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 4, 18 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 44, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 12, 7 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 41, 28 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 20, 29 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 28, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 16, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 37, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 5, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 17, 30 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 33, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 3, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 12, 38 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 42, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 3, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 15, 19 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 24, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 38, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 8, 37 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 46, 23 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 10, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 40, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 22, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 7, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 40, 14 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 12, 22 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 13, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 11,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 5, 43 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
        [12] = new List<CalendarRewardInfo>()
    {
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 1,
            RequiredTasks = new List<int> { 16, 5 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 2,
            RequiredTasks = new List<int> { 32, 35 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 3,
            RequiredTasks = new List<int> { 24, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 4,
            RequiredTasks = new List<int> { 13, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 5,
            RequiredTasks = new List<int> { 1, 18 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 6,
            RequiredTasks = new List<int> { 17, 27 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 7,
            RequiredTasks = new List<int> { 7, 31 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 8,
            RequiredTasks = new List<int> { 33, 34 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 9,
            RequiredTasks = new List<int> { 44, 8 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 10,
            RequiredTasks = new List<int> { 20, 9 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 11,
            RequiredTasks = new List<int> { 31, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 12,
            RequiredTasks = new List<int> { 2, 24 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 13,
            RequiredTasks = new List<int> { 16, 40 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 14,
            RequiredTasks = new List<int> { 34, 16 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 15,
            RequiredTasks = new List<int> { 45, 10 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 16,
            RequiredTasks = new List<int> { 32, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 17,
            RequiredTasks = new List<int> { 22, 0 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 18,
            RequiredTasks = new List<int> { 16, 33 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 19,
            RequiredTasks = new List<int> { 26, 8 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 20,
            RequiredTasks = new List<int> { 5, 45 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 21,
            RequiredTasks = new List<int> { 13, 3 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 22,
            RequiredTasks = new List<int> { 31, 19 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 23,
            RequiredTasks = new List<int> { 0, 2 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 24,
            RequiredTasks = new List<int> { 9, 39 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 25,
            RequiredTasks = new List<int> { 45, 15 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 26,
            RequiredTasks = new List<int> { 45, 17 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 27,
            RequiredTasks = new List<int> { 45, 11 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 28,
            RequiredTasks = new List<int> { 40, 11 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 29,
            RequiredTasks = new List<int> { 20, 13 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 30,
            RequiredTasks = new List<int> { 44, 36 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
        new CalendarRewardInfo
        {
            Month        = 12,
            DayInMonth   = 31,
            RequiredTasks = new List<int> { 38, 32 },
            RewardType   = CalendarRewardType.Item,
            RewardCount  = 1,
            RewardItemId = (int)ItemId.Case12
        },
    },
    };

    // Информационная структура из определения
    public static readonly List<CalendarRewardInfo> AllRewards = new List<CalendarRewardInfo>();

    static CalendarTasksDefinition()
    {
        AllRewards = new List<CalendarRewardInfo>();
        foreach (var monthList in Monthly.Values)
            AllRewards.AddRange(monthList);
    }
    }
