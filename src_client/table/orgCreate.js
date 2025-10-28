const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.org.create.",
    rpcName = "rpc.org.create.",
    serverName = "server.org.create.";

let isOpenCreateOrg = false;

gm.events.add(clientName + "init", (price) => {
    if (global.menuCheck() || isOpenCreateOrg) return;

    global.menuOpen();
    isOpenCreateOrg = true;

    mp.gui.emmit(`window.router.setView("FractionsCreate", ${price})`);


});

gm.events.add(clientName + "close", () => {
    if (!isOpenCreateOrg) return;

    global.menuClose();
    mp.gui.emmit('window.router.setHud()');
    isOpenCreateOrg = false;
});

gm.events.add(clientName + "buy", (isCrime, orgName) => {
    if (!global.antiFlood("table.missionUse", 500))
        return;

    callRemote(serverName + "buy", isCrime, orgName);
});