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

ipadData.coming = {};

const maxCallTime = 45;

let callTime = null;

const stopSound = () => call("sounds.stop", "call");

const closeCallView = () => mp.gui.emmit(`window.listernEvent ('downIpad');`);
const isIpadCall = () => mp.gui.emmit(`window.listernEvent ('isIpadCall');`);

const clearTimeOut = (isEndTimeOut = false) => {
    if (!isEndTimeOut && callTime !== null)
        clearTimeout (callTime);

    if (isEndTimeOut)
        call(clientName + "put");

    callTime = null;
}

const clearData = () => {
    ipadData.coming = {};
}

gm.events.add(clientName + "take", () => {//Поднять телефон
    callRemote(serverName + "take");
});

gm.events.add(clientName + "put", () => {//Отклонить

    callRemote(serverName + "put");

    clearTimeOut ();
    clearData ();
    closeCallView ();
    stopSound ();
});

gm.events.add(clientName + "callError", () => {
    //if (ipadData.coming.callStart)
    //    callRemote(serverName + "take");

    clearTimeOut ();
    clearData ();
    closeCallView ();
    stopSound ();
    global.ipadSound ("msgReceived", "abonentdaun.ogg");
});


gm.events.add(clientName + "call", (number) => {
    const contact = global.getContact(number);

    if (contact && contact.IsSystem) {
        mp.gui.emmit(`window.listernEvent ('ipad.call.onMessage', ${number});`);
        return;
    }

    callRemote(serverName + "call", number);

    //

    ipadData.coming = {
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
    ipadData.coming.callStart = true; // Сбросить телефон

    //call("sounds.stop", "call");

    global.ipadSound("call", "call.ogg", 0.25, true)
});

gm.events.add(clientName + "callAccept", () => {//Начали говорить
    ipadData.coming.isComing = true;

    if (ipadData.coming.callStart)
        delete ipadData.coming.callStart;

    clearTimeOut ();
    stopSound ();

    mp.gui.emmit(`window.listernEvent ('callAccept');`);
});

gm.events.add(clientName + "bell", (number) => {
    //
    ipadData.coming = {
        Number: number,
        isCall: false,
        isComing: false,
    };

    isIpadCall ();

    call('ipad.notify', number, translateText("Звонит вам.."), 5);
});

rpc.register(rpcName + "getComingIpad", () => {
    if (ipadData.coming && ipadData.coming.Number) {
        const comingData = {
            ...ipadData.coming,
            ...getContact(ipadData.coming.Number),

        }
        return JSON.stringify(comingData);
    }

    return false;
});

rpc.register(rpcName + "isCall", () => {
    if (ipadData.coming && typeof ipadData.coming.Number !== "undefined")
        return true;
    else
        return false;
});

//

global.ipadMute = false;
gm.events.add(clientName + "mute", (toggled) => {
    global.ipadMute = toggled;
});
