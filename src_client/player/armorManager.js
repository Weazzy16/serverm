const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const browserar = browsers.new(`http://package/interface/modules/ArmorManager/index.html`);

mp.events.add({
    "client.armorManager.callServer": (eventName, ...args) => {
        callRemote(eventName, ...args);
    },
    "client.armorManager.init": (data) => {
        browserar.execute(`app.init(${data})`);
    },
    "client.armorManager.open": (data) => {
        global.menuOpen();
        browserar.execute(`app.open(${data})`);
    },
    "client.armorManager.close": () => {
        global.menuClose();
        browserar.execute(`app.reset()`);
    },
});

mp.keys.bind(global.Keys.VK_K, false, () => {
    if (!global.loggedin || global.chatActive || global.editing || global.menuCheck() || new Date().getTime() - global.lastCheck < 1000) return;
    callRemote("server.armorMenu.open")
});