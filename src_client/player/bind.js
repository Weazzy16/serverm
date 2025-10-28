import marketPlace from "../EternalDev/marketPlace";
import containers from "../EternalDev/containers";
import chipTuning from "../EternalDev/chipTuning";

const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
global.BinderStatus = false;
global.indexUpdate = -1;
const binderAvailable = [27,113,114,115,116,117,118,119,120,121,122,123,145,19,192,49,50,51,52,53,54,55,56,57,48,189,187,8,45,36,33,144,111,106,109,144,9,81,69,82,84,89,85,73,79,219,221,220,46,35,34,36,38,33,107,20,71,72,74,75,76,186,222,13,37,12,39,16,90,88,67,86,66,78,77,188,190,191,16,35,40,34,13,17,18,18,93,17,37,40,39,45,46,38];

// ← КАРТА СООТВЕТСТВИЯ НАЗВАНИЙ КЛАВИШ К ИНДЕКСАМ В global.userBinder
const chatDataToBinderIndex = {
    // Main
    "phone": 30,
    "table": 43,
    "pressI": 12,
    
    // Social
    "voice": 36,
    "radio": 50,
    "partyCreateMarker": 45,
    "fingerPointing": 45,
    
    // Menu
    "mainMenu": 27,
    "tasksMenu": 33,
    "reportMenu": 59,
    "openGarageInfo": 32,
    "circleAnimMenu": 31,
    "langWheel": 24,
    
    // Interactions
    "acceptRequest": 6,
    "denyRequest": 7,
    "pickupItem": 9,
    "takeItemFromHands": 9,
    "interaction": 9,
    "pressE": 9,
    
    // Vehicle
    "driverEnter": 38,
    "passengerEnter": 38,
    "toggleEngine": 38,
    "carLock": 39,
    "belt": 53,
    "gps": 32,
    "cruise": 28,
    "autopilot": 54,
    
    // Gov
    "cuff": 10,
    "followPlayer": 11,
    "hookCuffedPlayer": 11,
    "recording": 60,
    "changeVisorState": 27,
    "megaphone": 33,
    
    // Other
    "crouch": 34,
    "crawl": 61,
    "OOCMode": 19,
    "pressF": 9,
    "clothesAction": 8,
    "cancelMacro": 56,
    "muteMicrophone": 47
};

// ← КАРТА ПРЕОБРАЗОВАНИЯ НАЗВАНИЯ КЛАВИШИ В KeyCode
const keyNameToCode = {
    "K": 75, "Tab": 9, "I": 73, "V": 86, "R": 82, "B": 66, "G": 71,
    "F5": 116, "F6": 117, "F7": 118, "F": 70, "M": 77, "L": 76, "C": 67,
    "Y": 89, "N": 78, "E": 69, "A": 65, "X": 88, "Z": 90, "U": 85, "O": 79,
    "T": 84, "H": 72, "J": 74, "Ctrl": 17, "Shift": 16, "Esc": 27, "F10": 121
};

// ← СИНХРОНИЗАЦИЯ ChatData с global.userBinder
global.syncChatDataToUserBinder = (chatData) => {
    if (!chatData) return;
    
    console.log("Syncing ChatData to userBinder:", chatData);
    
    Object.keys(chatDataToBinderIndex).forEach(chatKey => {
        const binderIndex = chatDataToBinderIndex[chatKey];
        const chatValue = chatData[chatKey + "Key"] || chatData[chatKey];
        
        if (chatValue && chatValue !== "0" && global.userBinder[binderIndex]) {
            const keyCode = keyNameToCode[chatValue];
            if (keyCode) {
                global.userBinder[binderIndex].keyCode = keyCode;
                console.log(`Updated ${chatKey}: index=${binderIndex}, keyCode=${keyCode}`);
            }
        }
    });
};

const binderType = {
    all: "all",
    onFoot: "onFoot",
    inVehicle: "inVehicle",
    exception: "exception"
}

const binderGroup = {
    all: "all",
    admin: "admin",
    user: "user",
    fraction: "fraction",
}

const menuType = {
    all: "all",
    vehicle: "vehicle",
    fast: "fast",
    fraction: "fraction",
    other: "other",
    admin: "admin"
}

global.userBinder = [
    {//0
        title: "FlyCam",
        keyCode: global.Keys.VK_F8,
        keyCodeDefault: global.Keys.VK_F8,
        function: "noclip",
        type: binderType.all,
        group: binderGroup.admin,
        menu: menuType.admin
    },
    {//1
        title: "ESP",
        keyCode: global.Keys.VK_F12,
        keyCodeDefault: global.Keys.VK_F12,
        function: "esp",
        type: binderType.all,
        group: binderGroup.admin,
        menu: menuType.admin
    },
    {//2
        title: translateText("Телепорт по WayPoint"),
        keyCode: global.Keys.VK_F4,
        keyCodeDefault: global.Keys.VK_F4,
        function: "markerteleport",
        type: binderType.all,
        group: binderGroup.admin,
        menu: menuType.admin
    },
    {//3
        title: translateText("Рабочее пространство"),
        keyCode: global.Keys.VK_F7,
        keyCodeDefault: global.Keys.VK_F7,
        function: "open_Table",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.fraction
    },
    {//4
        title: translateText("Бортовой компьютер полиции"),
        keyCode: global.Keys.VK_U,
        keyCodeDefault: global.Keys.VK_U,
        function: "o_policepc",
        type: binderType.inVehicle,
        group: binderGroup.fraction,
        menu: menuType.fraction
    },
    {//5
        keyCode: global.Keys.VK_ESCAPE,
        keyCodeDefault: global.Keys.VK_ESCAPE,
        function: "c_policepc",
        trigges: false,
        type: binderType.all,
        group: binderGroup.all,
        menu: menuType.all
    },
    {//6
        title: translateText("Принятие предложений"),
        keyCode: global.Keys.VK_Y,
        keyCodeDefault: global.Keys.VK_Y,
        function: "acceptPressed",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//7
        title: translateText("Отклонение предложений"),
        keyCode: global.Keys.VK_N,
        keyCodeDefault: global.Keys.VK_N,
        function: "cancelPressed",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//8
        title: translateText("Анимации"),
        keyCode: global.Keys.VK_U,
        keyCodeDefault: global.Keys.VK_U,
        function: "o_animation",
        type: binderType.onFoot,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//9
        title: translateText("Взаимодействие"),
        keyCode: global.Keys.VK_E,
        keyCodeDefault: global.Keys.VK_E,
        function: "interactionPressed",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.other
    },
    {//10
        title: translateText("Надеть наручники/стяжки"),
        keyCode: global.Keys.VK_X,
        keyCodeDefault: global.Keys.VK_X,
        function: "playerPressCuffBut",
        type: binderType.all,
        group: binderGroup.fraction,
        menu: menuType.fraction
    },
    {//11
        title: translateText("Вести за собой"),
        keyCode: global.Keys.VK_Z,
        keyCodeDefault: global.Keys.VK_Z,
        function: "playerPressFollowBut",
        type: binderType.all,
        group: binderGroup.fraction,
        menu: menuType.fraction
    },
    {//12
        title: translateText("Инвентарь"),
        keyCode: global.Keys.VK_I,
        keyCodeDefault: global.Keys.VK_I,
        function: "GameMenuOpen",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//13
    },
    {//14
        title: translateText("Перезарядка оружия"),
        keyCode: global.Keys.VK_R,
        keyCodeDefault: global.Keys.VK_R,
        function: "playerReload",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//15
        title: translateText("Быстрый доступ (Слот 1)"),
        keyCode: global.Keys.VK_1,
        keyCodeDefault: global.Keys.VK_1,
        function: "changeweap_1",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.fast
    },
    {//16
        title: translateText("Быстрый доступ (Слот 2)"),
        keyCode: global.Keys.VK_2,
        keyCodeDefault: global.Keys.VK_2,
        function: "changeweap_2",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.fast
    },
    {//17
        title: translateText("Быстрый доступ (Слот 3)"),
        keyCode: global.Keys.VK_3,
        keyCodeDefault: global.Keys.VK_3,
        function: "changeweap_3",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.fast
    },
    {//18
        title: translateText("Быстрый доступ (Слот 4)"),
        keyCode: global.Keys.VK_4,
        keyCodeDefault: global.Keys.VK_4,
        function: "changeweap_4",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.fast
    },
    {//19
        title: translateText("Чат"),
        keyCode: global.Keys.VK_T,
        keyCodeDefault: global.Keys.VK_T,
        function: "o_chat",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//20
        keyCode: global.Keys.VK_ESCAPE,
        keyCodeDefault: global.Keys.VK_ESCAPE,
        function: "c_chat",
        trigges: false,
        type: binderType.all,
        group: binderGroup.all,
        menu: menuType.all
    },
    {//21
    },
    {//22
    },
    {//23
        keyCode: global.Keys.VK_ESCAPE,
        keyCodeDefault: global.Keys.VK_ESCAPE,
        function: "gamemenu",
        trigges: false,
        type: binderType.all,
        group: binderGroup.all,
        menu: menuType.all
    },
    {//24
        title: translateText("Отображание имён"),
        keyCode: global.Keys.VK_5,
        keyCodeDefault: global.Keys.VK_5,
        function: "showGamertags",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.other
    },
    {//25
        title: translateText("Меню помощи"),
        keyCode: global.Keys.VK_F10,
        keyCodeDefault: global.Keys.VK_F10,
        function: "o_help",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.other
    },
    {//26
        keyCode: global.Keys.VK_ESCAPE,
        keyCodeDefault: global.Keys.VK_ESCAPE,
        function: "c_help",
        trigges: false,
        type: binderType.all,
        group: binderGroup.all,
        menu: menuType.all
    },
    {//27
        title: translateText("Состояние HUD"),
        keyCode: global.Keys.VK_F5,
        keyCodeDefault: global.Keys.VK_F5,
        function: "o_hud",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.other
    },
    {//28
        title: translateText("Круиз контроль"),
        keyCode: global.Keys.VK_6,
        keyCodeDefault: global.Keys.VK_6,
        function: "cruise",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//29
        keyCode: global.Keys.VK_ESCAPE,
        keyCodeDefault: global.Keys.VK_ESCAPE,
        function: "jobselectorOpened",
        trigges: false,
        type: binderType.all,
        group: binderGroup.all,
        menu: menuType.all
    },
    {//30
        title: translateText("Телефон"),
        keyCode: global.Keys.VK_UP,
        keyCodeDefault: global.Keys.VK_UP,
        function: "openPlayerMenu",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//31
        title: translateText("Взаимодействие с игроками"),
        keyCode: global.Keys.VK_G,
        keyCodeDefault: global.Keys.VK_G,
        function: "openCircleMenu",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//32
        title: translateText("Взаимодействие с предметами"),
        keyCode: global.Keys.VK_H,
        keyCodeDefault: global.Keys.VK_H,
        function: "dropObject",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//33
        title: translateText("Меню репортов"),
        keyCode: global.Keys.VK_F6,
        keyCodeDefault: global.Keys.VK_F6,
        function: "o_reports",
        type: binderType.all,
        group: binderGroup.admin,
        menu: menuType.admin
    },
    {//34
        title: translateText("Присесть"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "SitWalkSyle",
        type: binderType.onFoot,
        group: binderGroup.user,
        menu: menuType.fast
    },
    {//35
        keyCode: global.Keys.VK_ESCAPE,
        keyCodeDefault: global.Keys.VK_ESCAPE,
        function: "c_weaponshop",
        trigges: false,
        type: binderType.all,
        group: binderGroup.all,
        menu: menuType.admin
    },
    {//36
        title: translateText("Микрофон"),
        keyCode: global.Keys.VK_N,
        keyCodeDefault: global.Keys.VK_N,
        function: "enableVoice",
        type: binderType.exception,
        group: binderGroup.user,
        binding: "voice",
        menu: menuType.all
    },
    {//37
        keyCode: global.Keys.VK_N,
        keyCodeDefault: global.Keys.VK_N,
        function: "disableVoice",
        type: binderType.exception,
        group: binderGroup.all,
        trigges: false,
        binding: "voice",
        menu: menuType.all
    },
    {//38
        title: translateText("Двигатель транспорта"),
        keyCode: global.Keys.VK_B,
        keyCodeDefault: global.Keys.VK_B,
        function: "engineCarPressed",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//39
        title: translateText("Закрыть/открыть транспорт"),
        keyCode: global.Keys.VK_L,
        keyCodeDefault: global.Keys.VK_L,
        function: "lockCarPressed",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//40
        title: translateText("Левый поворотник"),
        keyCode: global.Keys.VK_LEFT,
        keyCodeDefault: global.Keys.VK_LEFT,
        function: "lightleft",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//41
        title: translateText("Правый поворотник"),
        keyCode: global.Keys.VK_RIGHT,
        keyCodeDefault: global.Keys.VK_RIGHT,
        function: "lightright",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//42
        title: translateText("Аварийная сигнализация"),
        keyCode: global.Keys.VK_DOWN,
        keyCodeDefault: global.Keys.VK_DOWN,
        function: "signaling",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//43
        title: translateText("Меню биндера"),
        keyCode: global.Keys.VK_TAB,
        keyCodeDefault: global.Keys.VK_TAB,
        function: "openBinder",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.other
    },
    {//44
        keyCode: global.Keys.VK_ESCAPE,
        keyCodeDefault: global.Keys.VK_ESCAPE,
        function: "c_globalEscape",
        trigges: false,
        type: binderType.all,
        group: binderGroup.all,
        menu: menuType.all
    },
    {//45
        title: translateText("Указать пальцем"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "down_fingerpointing",
        type: binderType.onFoot,
        group: binderGroup.user,
        binding: "fingerpointing",
        menu: menuType.all
    },
    {//46
        keyCode: 0,
        keyCodeDefault: 0,
        function: "up_fingerpointing",
        type: binderType.onFoot,
        group: binderGroup.all,
        trigges: false,
        binding: "fingerpointing",
        menu: menuType.all
    },
    {//47
        title: translateText("Перезагрузить микрофон"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "voiceReload",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//48
        title: translateText("Быстрый доступ (Слот 5)"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "changeweap_5",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.fast
    },
    {//49
        title: translateText("Удаление уведомления"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "dellNotification",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//50
        title: translateText("Рация"),
        keyCode: global.Keys.VK_M,
        keyCodeDefault: global.Keys.VK_M,
        function: "enableRadioVoice",
        type: binderType.exception,
        group: binderGroup.user,
        binding: "radio",
        menu: menuType.all
    },
    {//51
        keyCode: 0,
        keyCodeDefault: 0,
        function: "disableRadioVoice",
        type: binderType.exception,
        group: binderGroup.all,
        trigges: false,
        binding: "radio",
        menu: menuType.all
    },
    {//52
        title: translateText("Достать/убрать рацию"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "takeWalkieTalkie",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//53
        title: translateText("Пристегнуть/отстегнуть ремень безопасности"),
        keyCode: global.Keys.VK_J,
        keyCodeDefault: global.Keys.VK_J,
        function: "onBelt",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//54
        title: translateText("Включить/отключить автопилот"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "onAutoPilot",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//55
        title: translateText("Открыть взаимодействие с животным"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "onAnimal",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//56
        title: translateText("Отменить анимацию"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "",
        type: binderType.onFoot,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//57
        title: translateText("Передать метку водителю"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "onSendWaypoint",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//58
        title: translateText("Включить/отключить сирену"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "onSirenSync",
        type: binderType.inVehicle,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//59
        title: translateText("Отправить репорт"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "openReportInput",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//60
        title: translateText("Запись экрана"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "onRecorder",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//61
        title: translateText("Ползти"),
        keyCode: 0,
        keyCodeDefault: 0,
        function: "Snake",
        type: binderType.onFoot,
        group: binderGroup.user,
        menu: menuType.fast
    },
    {//62
        title: "Стабилизация Т/С",
        keyCode: 0,
        keyCodeDefault: 0,
        function: "SwitchStabilization",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.vehicle
    },
    {//63
        title: "GTA5DEVMENU",
        keyCode: global.Keys.VK_F2,
        keyCodeDefault: global.Keys.VK_F2,
        function: "GTA5DEVMENU",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//64
        title: "Войны за предприятия",
        keyCode: 0,
        keyCodeDefault: 0,
        function: "ActivityWar",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.fraction
    },
    {//65
        title: translateText("Планшет"),
        keyCode: global.Keys.VK_K,
        keyCodeDefault: global.Keys.VK_K,
        function: "openPlayerMenu1",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//66
        title: "GTA5DEVCRAFT",
        keyCode: global.Keys.VK_H,
        keyCodeDefault: global.Keys.VK_H,
        function: "GTA5DEVCRAFT",
        type: binderType.all,
        group: binderGroup.user,
        menu: menuType.all
    },
    {//67
        title: "animgta5dev",
        keyCode: 0,
        keyCodeDefault: 0,
        function: "animgta5dev",
        type: binderType.onFoot,
        group: binderGroup.user,
        binding: "fingerpointing",
        menu: menuType.all
    },
    {//68
        keyCode: 0,
        keyCodeDefault: 0,
        function: "animgta5devdis",
        type: binderType.onFoot,
        group: binderGroup.all,
        trigges: false,
        binding: "fingerpointing",
        menu: menuType.all
    },
];

gm.events.add('escape', () => {
	global.binderFunctions.c_globalEscape ();
});

// ← ЗАГРУЗКА ПРИВЯЗОК ИЗ CHATDATA
gm.events.add("loadChatConfig", (chatDataJson) => {
    try {
        console.log("loadChatConfig event received");
        const chatData = JSON.parse(chatDataJson);
        console.log("Syncing ChatData to userBinder");
        
        global.syncChatDataToUserBinder(chatData);
        
        console.log("Chat bindings synced successfully");
    } catch (e) {
        console.log("Error in loadChatConfig:", e);
    }
});

global.binderFunctions.openReportInput = () => {
    call('openInput', translateText("Репорт"), translateText("Введите ваше сообщение"), 150, 'sendReportFromClientInput');
}

global.binderFunctions.c_globalEscape = (isDeath = false) => {
    if (containers.opened)
         return containers.tryClose();
    if (chipTuning.opened)
        return chipTuning.tryClose();
    if (marketPlace.opened)
        return marketPlace.closeApp();
    if (global.circleOpen) global.CloseCircle (true);
    if (global.reportactive) global.binderFunctions.c_reports ();
    if (global.BinderStatus) CloseBinder ();
    global.CloseDocs ();
    global.binderFunctions.blackjackExit ();
    global.binderFunctions.spinExit ();
    global.binderFunctions.horseExit ();
    global.binderFunctions.rouletteExit ();
    global.closeFractionTableMenu ();
    global.binderFunctions.c_animation ();
    global.binderFunctions.closeWedding ();
    call('closeWalkieTalkieMenu');    
    call('client.closedonatesite');
    call('client.petshop.close');
    call('client.advert.close');
    call('client.policecomputer.close');
    global.binderFunctions.GameMenuClose ();
    call('client.familyZoneClose');
    call('client.closeWar');

    if (isDeath) {
        call('client.battlepass.close');
    }
};

let binderListeners = {};

class BinderActions {
    open (type) {
        let returnList = []
        global.userBinder.forEach((item, index) => {
            if (type === item.menu) {
                if (item.group !== binderGroup.all) {
                    returnList = [
                        ...returnList, {
                            title: item.title,
                            name: global.Keys[item.keyCode],
                            index: index
                        }
                    ]
                }
            }
        });
        return returnList;
    }

    bind () {
        let returnList = {}
        global.userBinder.forEach((item, index) => {
            if (item.group !== binderGroup.all) {
                returnList[item.keyCode] = returnList[item.keyCode] ? `${returnList[item.keyCode]}, ${item.title}` : item.title;
            }
        });
        return returnList;
    }

    getIsBinder (bindType) {
        if (bindType === binderType.all || bindType === binderType.exception) return 1;
        else if (bindType === binderType.onFoot && !global.localplayer.isInAnyVehicle(false)) return 1;
        else if (bindType === binderType.inVehicle && global.localplayer.isInAnyVehicle(false)) return 1;
        return 0;
    }
    
    getControllBind (keyCode, trigges) {
        if (!global.loggedin) return;

        global.updateAfkStatus (false);

        if (global.indexUpdate !== -1) {
            const index = global.indexUpdate;
            mp.gui.emmit(`window.binder.index();`);
            global.indexUpdate = -1;
            let success = true;
            const selectBinder = global.userBinder [index];
            if (parseInt (keyCode) === parseInt (selectBinder.keyCode)) {
				if(index == 43) {
					call('notify', 4, 9, translateText("Невозможно снять привязку с меню биндера"), 3000);
                    return;
				}
                binder.destroy (keyCode, index);
                
                global.userBinder [index].keyCode = 0;
                if (global.BinderStatus && selectBinder.group !== binderGroup.all) {
                    mp.gui.emmit(`window.binder.updateData(${index}, '${global.Keys[0]}')`);
                    mp.gui.emmit(`window.binder.setBindData('${JSON.stringify(binderActions.bind())}');`);
                }
                InfoUpdate (index);
        
                if (binderListeners [index] && binderListeners [index].length && global.userBinder [index].binding) {
                    binderListeners [index].forEach((i) => {
                        if (i !== index && global.userBinder [index].binding === global.userBinder [i].binding) {
                            binder.destroy (index, i);
                            return;
                        }
                    });
                }
                callRemote('bindConfigSave', index, 0);
            } else { 
                if (selectBinder.type !== binderType.exception) {
                    global.userBinder.forEach((item) => {
                        if (success && parseInt (keyCode) === parseInt (item.keyCode) && (item.type === binderType.all || selectBinder.type === binderType.all)) {
                            success = false;
                            call('notify', 4, 9, translateText("Данная клавиша уже занята"), 3000);
                            return;
                        } else if (success && parseInt (keyCode) === parseInt (item.keyCode) && item.type === binderType.onFoot && selectBinder.type === binderType.onFoot) {
                            success = false;
                            call('notify', 4, 9, translateText("Данная клавиша уже занята"), 3000);            
                            return;
                        } else if (success && parseInt (keyCode) === parseInt (item.keyCode) && item.type === binderType.inVehicle && selectBinder.type === binderType.inVehicle) {
                            success = false;
                            call('notify', 4, 9, translateText("Данная клавиша уже занята"), 3000);            
                            return;
                        }
                    });
                }
                if (success) {
                    binder.update (keyCode, index);
                    callRemote('bindConfigSave', index, keyCode);
                }
            }
        } else {
            if (binderListeners [keyCode] && binderListeners [keyCode].length) {
                binderListeners [keyCode].forEach((index) => {
                    if (this.getIsBinder (global.userBinder [index].type) &&
                        ((global.userBinder [index].trigges === undefined && trigges === true) || (global.userBinder [index].trigges !== undefined && trigges === global.userBinder [index].trigges)) &&
                        global.binderFunctions [global.userBinder [index].function]) {
                        global.binderFunctions [global.userBinder [index].function]();
                    }
                });
            }
        }
    }
}

let binderActions = new BinderActions();

let bindStatus = {};
class Binder {

    update (keyCode, index, refresh = false) {
        const oldKeyCode = parseInt (global.userBinder [index].keyCode);
        this.destroy (oldKeyCode, index);
        this.create (keyCode, index, refresh);
        InfoUpdate (index);
 console.log("Calling window.binder.updateData:", index, global.Keys[keyCode]);
    mp.gui.emmit(`window.binder.updateData(${index}, '${global.Keys[keyCode]}')`);
        if (binderListeners [oldKeyCode] && binderListeners [oldKeyCode].length && global.userBinder [index].binding) {
            binderListeners [oldKeyCode].forEach((i) => {
                if (i !== index && global.userBinder [index].binding === global.userBinder [i].binding) {
                    this.destroy (oldKeyCode, i);
                    this.create (keyCode, i);
                    return;
                }
            });
        }
    }

    create (keyCode, index, refresh) {
        keyCode = parseInt (keyCode);
        global.userBinder [index].keyCode = keyCode;
        if (global.BinderStatus && global.userBinder [index].group !== binderGroup.all && !refresh) {
            mp.gui.emmit(`window.binder.updateData(${index}, '${global.Keys[keyCode]}')`);
            mp.gui.emmit(`window.binder.setBindData('${JSON.stringify(binderActions.bind())}');`);
        }
        if (!bindStatus [`${keyCode}_${global.userBinder [index].trigges === undefined ? true : global.userBinder [index].trigges}`] ||
            bindStatus [`${keyCode}_${global.userBinder [index].trigges === undefined ? true : global.userBinder [index].trigges}`] === undefined) {
            bindStatus[`${keyCode}_${global.userBinder [index].trigges === undefined ? true : global.userBinder [index].trigges}`] = true;
            mp.keys.bind(keyCode,
                global.userBinder [index].trigges === undefined ? true : global.userBinder [index].trigges,
                () => binderActions.getControllBind (keyCode, global.userBinder [index].trigges));
        }

        if (!binderListeners [keyCode] || binderListeners [keyCode].indexOf(index) === -1) {
            if (!binderListeners [keyCode])
                binderListeners [keyCode] = [];
            binderListeners [keyCode].push(index);
        }
    }

    destroy (keyCode, index) {
        keyCode = parseInt (keyCode);
        if (binderListeners [keyCode]) {
            const indexOf = binderListeners [keyCode].indexOf(index);
            if (indexOf !== -1) {
                binderListeners [keyCode].splice(indexOf, 1);
            }
        }
    }
}

const InfoUpdate = (index) => {    
    mp.gui.emmit(`window.keysStore.updateName (${index}, ${global.userBinder[index].keyCode})`);
}

gm.events.add("client:OnBrowserInit", () => {
    global.userBinder.forEach((item, index) => {
        mp.gui.emmit(`window.keysStore.updateName (${index}, ${item.keyCode})`);
    });
});

let binder = new Binder();

binderAvailable.forEach((item) => {
    if (!bindStatus[`${item}_true`] || bindStatus[`${item}_true`] === undefined) {
        bindStatus[`${item}_true`] = true;
        mp.keys.bind(item, true, () => binderActions.getControllBind (item, true));
        mp.keys.bind(item, false, () => binderActions.getControllBind (item, false));
    }
});

global.userBinder.forEach((item, index) => {
    binder.create (item.keyCode, index)
});

global.binderFunctions.openBinder = () => {
    if (!global.BinderStatus) OpenBinder ();
    else CloseBinder ();
}

const OpenBinder = () => {
    if (global.menuCheck() || global.BinderStatus) return;

    mp.gui.emmit(`window.router.setView("PlayerBinder");`);
    setTimeout(() => {
        mp.gui.emmit(`window.binder.setBindData('${JSON.stringify(binderActions.bind())}');`);
        mp.gui.emmit(`window.binder.setData('${JSON.stringify(binderActions.open(menuType.all))}');`);
    }, 50);
    global.BinderStatus = true;
    global.menuOpen();
};

const CloseBinder = () => {
    mp.gui.emmit(`window.router.setHud();`);
    global.BinderStatus = false;
    global.menuClose();
};

gm.events.add('loadBindConfig', (params) => {
	try 
	{
        params = JSON.parse(params);
        let key;
        for(key in params) {
            if (params[key] && global.userBinder [Number (key)])
                binder.update (Number (params[key]), Number (key));
        }	
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/bind", "loadBindConfig", e.toString());
	}
});

gm.events.add("client:binder", (type, index, keyCode) => {
    if (type === "update") {
        global.indexUpdate = Number (index);
    } else if (type === "get") {
        mp.gui.emmit(`window.binder.setData('${JSON.stringify(binderActions.open(index))}');`);
    } else if (type === "refresh") {
        global.userBinder.forEach((item, i) => {
            binder.update (item.keyCodeDefault, i, true);
        });
        mp.gui.emmit(`window.binder.setBindData('${JSON.stringify(binderActions.bind())}');`);
        mp.gui.emmit(`window.binder.setData('${JSON.stringify(binderActions.open(index))}');`);
		callRemote('bindConfigSave', 0, 0);
    } else if (type === "close") {
        CloseBinder ();
    }
});

global.isBind = false;

gm.events.add("setBindToKey", (key) => {
    if (key === -1) global.isBind = false;
    else global.isBind = true;
});

global.isKeyClamp = false;

const keyClampMap = new Map();
class KeyClamp {
    constructor(name, key) {
        this.name = mp.game.joaat(name);
        this.key = key;
        this.intervalId = null;
        this.value = 0;
        this.maxValue = 0;
        this.isConfirm = false;
        this.isKeyDown = false;
        keyClampMap.set(this.name, this);
    }

    bind () {
        global.isKeyClamp = true;
        mp.gui.emmit(`window.events.callEvent("cef.KeyClamp.open", ${this.key})`);
        mp.keys.bind ( this.key, true, () => this.keyDown ());
        mp.keys.bind ( this.key, false, () => this.keyUp ());
    }

    unbind () {
        this.keyUp ();
        mp.gui.emmit(`window.events.callEvent("cef.KeyClamp.close")`);
        mp.keys.unbind ( this.key, true, () => this.keyDown ());
        mp.keys.unbind ( this.key, false, () => this.keyUp ());
    }

    keyDown () {
        if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || global.menuCheck()) return;
        if (!global.antiFlood("keyClamp", 250))
            return;

        this.isKeyDown = true;
        callRemote('keyClamp.keyDown');
    }

    keyUp () {
        this.isKeyDown = false;

        if (!this.isConfirm)
            return;

        this.isConfirm = false;

        callRemote('keyClamp.end', this.value);

        if (this.intervalId !== null)
            clearInterval(this.intervalId);

        this.intervalId = null;
        this.value = 0;
        this.cancel ();
    }

    setValue (value) {
        this.value += value;
        mp.gui.emmit(`window.events.callEvent("cef.KeyClamp.updateProgress", ${100 - (this.value / this.maxValue * 100)})`);
        if (!this.isConfirm) {
            this.isConfirm = true;
            this.confirm();
        } else if (0 >= this.value) {
            this.keyUp ();
        }
    }

    confirm () {}
    cancel () {}

    delete () {
        if (keyClampMap.has(this.name))
            keyClampMap.delete(this.name);
    }
}

global.keyClamp = KeyClamp;

gm.events.add('keyClamp.bind', (name) => {
    if (keyClampMap.has(name)) {
        const keyClamp = keyClampMap.get(name);
        keyClamp.bind();
    }
});

gm.events.add('keyClamp.unbind', (name) => {
    if (keyClampMap.has(name)) {
        const keyClamp = keyClampMap.get(name);
        keyClamp.unbind();
    }
});

gm.events.add('keyClamp.value', (name, value, maxValue) => {
    if (keyClampMap.has(name)) {
        const keyClamp = keyClampMap.get(name);

        if (!keyClamp.isKeyDown) {
            callRemote('keyClamp.end', value);
            return;
        }

        keyClamp.maxValue = maxValue;
        keyClamp.setValue(value);
    }
});