const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
require("./business");
require("./house");


const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";


gm.events.add(clientName + "loadProperty", () => {
    callRemote(serverName + "loadProperty");
});

let propertyList = [];

gm.events.add(clientName + "propertyInit", (json) => {
    json = JSON.parse(json);

    propertyList = [];
    json.forEach((item) => {
        propertyList.push({
            type: item[0],
            isOwner: item[1],
            id: item[2]
        })
    })

    mp.gui.emmit(`window.listernEvent ('phoneMainPropertyLoad');`);
});


rpc.register(rpcName + "getProperty", () => {
    return JSON.stringify(propertyList);
});
