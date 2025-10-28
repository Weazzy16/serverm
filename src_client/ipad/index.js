/***
 * Blacklist - [Number - int...]
 *
 */

/***
 * Messages - {}
 *      [Number - int] - {}
 *          Time - DateTime
 *          Type - Tynyint
 *          Text - Text
 *          Data - Text
 */

global.ipadData = { }


require("./property/index");
require("./recents");
require("./cars");
require("./settings");

require("./auction");

require("./phoneAnim");

const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.ipad.",
    rpcName = "rpc.ipad.",
    serverName = "server.ipad.";


global.ipadSound = (id, name, volume = 0.25, loop = false) => {
    call("sounds.playAmbient", "cloud/sound/iphone/" + name, {
        id: id,
        volume: volume,
        loop: loop
    });
}

global.isipadOpen = false;
gm.events.add(clientName + "finger", (direction) => {
    mp.game.mobile.moveFinger(direction);
})


global.binderFunctions.openPlayerMenu1 = () => {// M key
    call(clientName + "open")
};

let inFocus = false;
gm.events.add(clientName + "inputFocus", (toggled) => {
    if (!global.isipaddOpen)
        return;

    inFocus = toggled;
    global.menuOpen(!inFocus);
});

let ipadOpenAntiFlood = 0;

gm.events.add(clientName + "open", () => {
    if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || inFocus || ipadOpenAntiFlood > Date.now() || global.ipadCameraOpen) return;

    if (!global.isipadOpen) {
        if (global.menuCheck()) return;
        mp.gui.emmit(`window.hudStore.isHudNewipad (true)`);
        callRemote(serverName + "open");
        global.menuOpen(true);
        global.isipadOpen = true;
        if (global.localplayer.cSen !== "cphone_call") {
            mp.game.mobile.createMobileipad(0);
            mp.game.mobile.setMobileipadScale(0);
        }
        ipadOpenAntiFlood = Date.now() + 500;

    } else if (global.menuCheck())
        call(clientName + "close");
});

gm.events.add(clientName + "close", () => {
    if (!global.isipadOpen)
        return;

    ipadOpenAntiFlood = Date.now() + 500;

    inFocus = false;
    global.isipadOpen = false;
    mp.gui.emmit(`window.hudStore.isHudNewipad (false)`);
    callRemote(serverName + "close");
    global.menuClose();

    if (global.localplayer.cSen === "cphone_base")
        nativeInvoke ("DESTROY_MOBILE_PHONE");
});

/***
 * contacts - {}
 *      [Number - int]: Name - String (50)
 *
 */

const defaultContacts = {
    112: {
        Name: translateText("Полиция"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/police.jpg",
        IsSystem: true
    },
    911: {
        Name: translateText("Больница"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/ems.jpg",
        IsSystem: true
    },
    333: {
        Name: translateText("Механик"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/mech.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },
    228: {
        Name: translateText("Такси"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/taxi.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    101: {
        Name: "Stories",
        Avatar: "http://88.151.88.77/cloud/img/iphone/ra.jpg",
        IsSystem: true,
        DefaultMessage: translateText("Привет! :cowboy_hat_face: Сюда ты можешь отправить найденный бонус-код или промо-код и сразу получить свои бонусы. Просто пришли его в ответном сообщении! :gift: :gift: :gift:")
    },

    4386: {
        Name: translateText("Банк"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/bank.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999999: {
        Name: translateText("Информатор"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/inform.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999998: {
        Name: translateText("Склад"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/sklad.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999997: {
        Name: translateText("Аукцион"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/auc.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999996: {
        Name: "News",
        Avatar: "http://88.151.88.77/cloud/img/iphone/news.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999995: {
        Name: translateText("Арендатор"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/rent.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999994: {
        Name: translateText("Палатка"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/tent.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999993: {
        Name: translateText("Подсказки"),
        Avatar: "http://88.151.88.77/cloud/img/iphone/help.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999992: {
        Name: "Tinder",
        Avatar: "http://88.151.88.77/cloud/img/iphone/tinder.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },
    99999991: {
        Name: "Прямой эфир",
        Avatar: "http://88.151.88.77/cloud/img/iphone/efir.png",
        IsSystem: true,
        IsNotShow: false,
        NoSend: false
    },
}


ipadData.contacts = defaultContacts;

gm.events.add(clientName + "initContacts", (json) => {
    ipadData.contacts = defaultContacts;

    json = JSON.parse(json);
    for (let key in json) {
        ipadData.contacts [key] = {
            Name: json [key][0],
            Avatar: json [key][1]
        };
    }
});

ipadData.blackList = [];
gm.events.add(clientName + "initBalckList", (json) => {
    ipadData.blackList = JSON.parse(json);
});

//********************* Functions

global.getContact = (number) => {
    let rContactData = {
        Name: number.toString(),
        Number: number,
        IsBlackList: ipadData.blackList.includes (number),
        Avatar: null,
        IsAdded: false
    }

    const contactData = ipadData.contacts [number];
    if (typeof contactData === "object") {
        rContactData = {
            ...rContactData,
            ...contactData,
        }

        rContactData.IsAdded = true;
    }

    return rContactData;
}

const getContacts = () => {
    let contactsData = []
    Object.keys (ipadData.contacts).forEach((number) => {
        contactsData.push({
            ...getContact (number),
            Number: number
        });
    });

    let rContactsData = [];
    contactsData.forEach((data) => {
        const letterName = data.Name[0].toUpperCase();
        const index = rContactsData.findIndex(ld => ld.Name === letterName);

        if (index === -1) {
            const letterData = {
                Name: letterName,
                List: []
            };

            letterData.List.push(data)

            rContactsData.push(letterData);
        } else {

            rContactsData[index].List.push(data);
        }

    });

    return rContactsData;
}
//

gm.events.add(clientName + "addContact", (number, name, avatar) => {
    if (typeof ipadData.contacts [number] !== "object") {
        ipadData.contacts [number] = {
            Name: name,
            Avatar: avatar
        }
        callRemote(serverName + "addContact", number, name, avatar);
    }
});

gm.events.add(clientName + "updateContact", (number, name, avatar) => {
    if (typeof ipadData.contacts [number] === "object") {
        ipadData.contacts [number] = {
            Name: name,
            Avatar: avatar
        };
        callRemote(serverName + "updateContact", number, name, avatar);
    }
});

gm.events.add(clientName + "dellContact", (number) => {
    if (typeof ipadData.contacts [number] === "object") {
        delete ipadData.contacts [number];
        callRemote(serverName + "dellContact", number);
    }
});

rpc.register(rpcName + "getContacts", () => {
    return JSON.stringify (getContacts ());
});

rpc.register(rpcName + "getContact", (number) => {
    return JSON.stringify (getContact (number));
});

//

rpc.register(rpcName + "addBlackList", (number) => {;
    const index = ipadData.blackList.findIndex(n => n === number);
    if (index === -1) {
        ipadData.blackList.push(number);
        callRemote(serverName + "addBlackList", number);
        return true;
    }
    return false;
});

rpc.register(rpcName + "dellBlackList", (number) => {
    const index = ipadData.blackList.findIndex(n => n === number);
    if (index !== -1) {
        ipadData.blackList.splice(index, 1);
        callRemote(serverName + "dellBlackList", number);
        return true;
    }
    return false;
});

rpc.register(rpcName + "dellContact", (number) => {
    if (typeof ipadData.contacts [number] === "object") {
        delete ipadData.contacts [number];
        callRemote(serverName + "dellContact", number);
        return true;
    }
    return false;
});

gm.events.add("client.inventory.InitOtherDataStockMarket", (otherId, otherName, json, maxSlot, itemId, isArmyCar, isMyTent) => {
	itemstock = json
	mp.gui.emmit(`window.gta5devmenustockitemsmarket(${itemstock})`);
});

mp.events.add('clien.gta5devmarketload', () => {
    callRemote("server.gta5devmarketload");
});

mp.events.add("loadtovar",(text) => {
    mp.gui.emmit(`window.gta5devmenutovar(${JSON.stringify(text)})`);
});

mp.events.add("tovarselect",(text) => {
    mp.gui.emmit(`window.gta5devmenutovarselect(${JSON.stringify(text)})`);
});

mp.events.add('selectitems', (id) => {
    callRemote("server.selectitems", id);
});