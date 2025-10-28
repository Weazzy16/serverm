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
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
global.ipadData = { }

require("./gps/index.js");
require("./taxi/index.js");
require("./mechanic/index.js");
require("./message.js");
require("./call.js");
require("./gallery");
require("./weather");
require("./news");
require("./property/index");
require("./recents");
require("./cars");
require("./job/index");
require("./settings");
require("./forbes");
require("./notify");
require("./auction");

require('./camera/index');
require("./ipadAnim");
require("./tinder");

const
    clientName = "client.ipad.",
    rpcName = "rpc.ipad.",
    serverName = "server.ipad.";


global.ipadSound = (id, name, volume = 0.25, loop = false) => {
    call("sounds.playAmbient", "cloud/sound/iipad/" + name, {
        id: id,
        volume: volume,
        loop: loop
    });
}

global.isIpadOpen = false;
gm.events.add(clientName + "finger", (direction) => {
    mp.game.mobile.moveFinger(direction);
})


global.binderFunctions.openPlayerMenu1 = () => {// M key
    call(clientName + "open")
};

let inFocus = false;
gm.events.add(clientName + "inputFocus", (toggled) => {
    if (!global.isIpadOpen)
        return;

    inFocus = toggled;
    global.menuOpen(!inFocus);
});

let ipadOpenAntiFlood = 0;

gm.events.add(clientName + "open", () => {
    if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || inFocus || ipadOpenAntiFlood > Date.now() || global.ipadCameraOpen) return;

    if (!global.isIpadOpen) {
        if (global.menuCheck()) return;
        mp.gui.emmit(`window.hudStore.isHudNewIpad (true)`);
        callRemote(serverName + "open");
        global.menuOpen(true);
        global.isIpadOpen = true;
        if (global.localplayer.cSen !== "cipad_call") {
            mp.game.mobile.createMobileIpad(0);
            mp.game.mobile.setMobileIpadScale(0);
        }
        ipadOpenAntiFlood = Date.now() + 500;

    } else if (global.menuCheck())
        call(clientName + "close");
});

gm.events.add(clientName + "close", () => {
    if (!global.isIpadOpen)
        return;

    ipadOpenAntiFlood = Date.now() + 500;

    inFocus = false;
    global.isIpadOpen = false;
    mp.gui.emmit(`window.hudStore.isHudNewIpad (false)`);
    callRemote(serverName + "close");
    global.menuClose();

    if (global.localplayer.cSen === "cipad_base")
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
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/police.jpg",
        IsSystem: true
    },
    911: {
        Name: translateText("Больница"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/ems.jpg",
        IsSystem: true
    },
    333: {
        Name: translateText("Механик"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/mech.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },
    228: {
        Name: translateText("Такси"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/taxi.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    101: {
        Name: "Astra",
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/ra.jpg",
        IsSystem: true,
        DefaultMessage: translateText("Привет! :cowboy_hat_face: Сюда ты можешь отправить найденный бонус-код или промо-код и сразу получить свои бонусы. Просто пришли его в ответном сообщении! :gift: :gift: :gift:")
    },

    4386: {
        Name: translateText("Банк"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/bank.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999999: {
        Name: translateText("Информатор"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/inform.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999998: {
        Name: translateText("Склад"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/sklad.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999997: {
        Name: translateText("Аукцион"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/auc.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999996: {
        Name: "News",
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/news.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999995: {
        Name: translateText("Арендатор"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/rent.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999994: {
        Name: translateText("Палатка"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/tent.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999993: {
        Name: translateText("Подсказки"),
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/help.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999992: {
        Name: "Tinder",
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/tinder.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },
    99999991: {
        Name: "Прямой эфир",
        Avatar: "http://cdn.piecerp.ru/cloud/img/iipad/efir.png",
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

//




//Messages