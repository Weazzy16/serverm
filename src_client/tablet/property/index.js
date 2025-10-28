
require("./business");
require("./house");


const
    clientName = "client.tablet.",
    rpcName = "rpc.tablet.",
    serverName = "server.tablet.";


gm.events.add(clientName + "loadProperty", () => {
    mp.events.callRemote(serverName + "loadProperty");
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

    mp.gui.emmit(`window.listernEvent ('tabletMainPropertyLoad');`);
});


rpc.register(rpcName + "getProperty", () => {
    return JSON.stringify(propertyList);
});
