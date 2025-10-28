const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.ipad.settings.",
    rpcName = "rpc.ipad.settings.",
    serverName = "server.ipad.settings.";

let settingData = {}

gm.events.add(clientName + "init", (json) => {
    settingData = JSON.parse(json);
});

rpc.register(rpcName + "isAir", () => {
    return settingData.IsAir;
});

gm.events.add(clientName + "air", () => {
    settingData.IsAir = !settingData.IsAir;
    callRemote(serverName + "air");
});

//

rpc.register(rpcName + "forbesVisible", () => {
    return settingData.ForbesVisible;
});

gm.events.add(clientName + "forbesVisible", () => {
    settingData.ForbesVisible = !settingData.ForbesVisible;
    callRemote(serverName + "forbesVisible");
});
//


gm.events.add(clientName + "removeSim", () => {
    callRemote(serverName + "removeSim");
});

//

rpc.register(rpcName + "bellId", () => {
    return settingData.BellId;
});

gm.events.add(clientName + "bellId", (id) => {
    settingData.BellId = id;
    callRemote(serverName + "bellId", id);
});

//

rpc.register(rpcName + "smsId", () => {
    return settingData.SmsId;
});

gm.events.add(clientName + "smsId", (id) => {
    settingData.SmsId = id;
    callRemote(serverName + "smsId", id);
});

//

//

rpc.register(rpcName + "wallpaper", () => {
    return settingData.Wallpaper;
});

gm.events.add(clientName + "wallpaper", (url) => {
    settingData.Wallpaper = url;
    callRemote(serverName + "wallpaper", url);
});

//

gm.events.add(clientName + "play", (url) => {
    call("sounds.stop", "ipadSound");

    call("sounds.playAmbient", url, {
        id: "ipadSound",
        volume: 0.05,
    });
});

