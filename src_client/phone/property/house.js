const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.phone.house.",
    rpcName = "rpc.phone.house.",
    serverName = "server.phone.house.";

gm.events.add(clientName + "load", () => {
    callRemote(serverName + "load");
});

let menuAccess = {};
let residentsData = {};
let houseData = {};
let garagesData = {};
let houseFurnitures = [];

let isCloseRemote = false;

gm.events.add(clientName + "init", (_menuAccess, carsData, _residentsData, _houseData, housePos, _garagesData, _houseFurnitures) => {
    menuAccess = JSON.parse(_menuAccess)
    residentsData = JSON.parse(_residentsData);
    houseData = JSON.parse(_houseData);
    console.log('→ [house.js] houseData =', houseData);
    garagesData = JSON.parse(_garagesData);
    houseFurnitures = JSON.parse(_houseFurnitures);
    


    houseData = JSON.parse (_houseData);
    if (houseData && Object.values (houseData) && Object.values (houseData).length) {
        housePos = JSON.parse (housePos);

        houseData.area = global.getAreaName (housePos.x, housePos.y, housePos.z);
    }

    mp.gui.emmit(`window.listernEvent ('phoneHouseInit');`);


    //
    call("client.phone.cars.init", carsData, menuAccess.includes("inGarage"), menuAccess.includes("sell"));

    isCloseRemote = menuAccess.includes("inPark")
});


gm.events.add(clientName + "close", () => {//+
    if (isCloseRemote)
        callRemote('server.house.close');

    isCloseRemote = false;
});

rpc.register(rpcName + "getStats", () => {
    return JSON.stringify(menuAccess);
});

rpc.register(rpcName + "houseData", () => {
    return JSON.stringify(houseData);
});

rpc.register(rpcName + "houseFurnitures", () => {
    return JSON.stringify(houseFurnitures);
});

rpc.register(rpcName + "residentsData", () => {
    return JSON.stringify(residentsData);
});

rpc.register(rpcName + "garagesData", () => {
    return JSON.stringify(garagesData);
});

//


gm.events.add(clientName + "fUse", (id, type) => {//+

    if (type)
        call('client.phone.close');

    callRemote('server.house.furniture.use', id, type);
    gm.discord(translateText("Пользуется мебелью"));
});

gm.events.add(clientName + "action", (action) => {//+
    call('client.phone.close');

    callRemote('server.house.action', action);
});

gm.events.add(clientName + "openPark", () => {//+
    call('client.phone.close');

    call('client.parking.open');
});


gm.events.add(clientName + "fBuy", (name) => {//+
    call('client.phone.close');
    callRemote('server.house.furniture.buy', name);
    gm.discord(translateText("Покупает мебель в дом"));
});

gm.events.add(clientName + "rAccess", (name, action) => {//+
    callRemote('server.house.resident.access', name, action);
});

gm.events.add(clientName + "rDell", (name) => {//+
    call('client.phone.close');
    callRemote('server.house.resident.dell', name);
});