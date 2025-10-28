const
    clientName = "client.tablet.",
    rpcName = "rpc.tablet.",
    serverName = "server.tablet.";

tabletData.coming = {};

const maxCallTime = 45;

let callTime = null;

const stopSound = () => mp.events.call("sounds.stop", "call");

const closeCallView = () => mp.gui.emmit(`window.listernEvent ('downTablet');`);
const isTabletCall = () => mp.gui.emmit(`window.listernEvent ('isTabletCall');`);

const clearTimeOut = (isEndTimeOut = false) => {
    if (!isEndTimeOut && callTime !== null)
        clearTimeout (callTime);

    if (isEndTimeOut)
        mp.events.call(clientName + "put");

    callTime = null;
}

const clearData = () => {
    tabletData.coming = {};
}

gm.events.add(clientName + "take", () => {//Поднять телефон
    mp.events.callRemote(serverName + "take");
});

gm.events.add(clientName + "put", () => {//Отклонить

    mp.events.callRemote(serverName + "put");

    clearTimeOut ();
    clearData ();
    closeCallView ();
    stopSound ();
});

gm.events.add(clientName + "callError", () => {
    //if (tabletData.coming.callStart)
    //    mp.events.callRemote(serverName + "take");

    clearTimeOut ();
    clearData ();
    closeCallView ();
    stopSound ();
    global.tabletSound ("msgReceived", "abonentdaun.ogg");
});


gm.events.add(clientName + "call", (number) => {
    const contact = global.getContact(number);

    if (contact && contact.IsSystem) {
        mp.gui.emmit(`window.listernEvent ('tablet.call.onMessage', ${number});`);
        return;
    }

    mp.events.callRemote(serverName + "call", number);

    //

    tabletData.coming = {
        Number: number,
        isCall: true,
        isComing: false,
    };

    //

    callTime = setTimeout(() => {
        clearTimeOut (true);
        clearData ();
        closeCallView ();
        stopSound ();
    }, maxCallTime * 1000);
});

gm.events.add(clientName + "callStart", () => {
    tabletData.coming.callStart = true; // Сбросить телефон

    //mp.events.call("sounds.stop", "call");

    global.tabletSound("call", "call.ogg", 0.25, true)
});

gm.events.add(clientName + "callAccept", () => {//Начали говорить
    tabletData.coming.isComing = true;

    if (tabletData.coming.callStart)
        delete tabletData.coming.callStart;

    clearTimeOut ();
    stopSound ();

    mp.gui.emmit(`window.listernEvent ('callAccept');`);
});

gm.events.add(clientName + "bell", (number) => {
    //
    tabletData.coming = {
        Number: number,
        isCall: false,
        isComing: false,
    };

    isTabletCall ();

    mp.events.call('tablet.notify', number, translateText("Звонит вам.."), 5);
});

rpc.register(rpcName + "getComingTablet", () => {
    if (tabletData.coming && tabletData.coming.Number) {
        const comingData = {
            ...tabletData.coming,
            ...getContact(tabletData.coming.Number),

        }
        return JSON.stringify(comingData);
    }

    return false;
});

rpc.register(rpcName + "isCall", () => {
    if (tabletData.coming && typeof tabletData.coming.Number !== "undefined")
        return true;
    else
        return false;
});

//

global.tabletMute = false;
gm.events.add(clientName + "mute", (toggled) => {
    global.tabletMute = toggled;
});
