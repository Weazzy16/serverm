const
    clientName = "client.tablet.",
    rpcName = "rpc.tablet.",
    serverName = "server.tablet.";

/***
 * Recents - [{}...]
 *      Number - int
 *      Time - DateTime
 *      IsCall - Bolean,
 *      Duration - int
 */

tabletData.recents = [];

gm.events.add(clientName + "initRecents", (json) => {
    json = JSON.parse(json);

    json.forEach((item) => {
        const recentData = {
            number: item[0],
            time: item[1],
            isCall: item[2],
            duration: item[3]
        }

        tabletData.recents.push(recentData);
    })
});

const maxRecent = 35;

const addRecents = (number, isCall, time, duration = -1) => {

    const recentData = {
        number: number,
        time: JSON.parse(time),
        isCall: isCall,
        duration: duration
    }

    const index = tabletData.recents.findIndex(r => r.number === number);

    if (typeof tabletData.recents [index] === "object")
        tabletData.recents.splice(index, 1);

    tabletData.recents.push(recentData);

    if (tabletData.recents.length >= maxRecent)
        tabletData.recents.splice(tabletData.recents.length - 1, 1);
}

gm.events.add(clientName + "addRecent", addRecents);

const updateRecent = (duration) => {
    const index = tabletData.recents.length - 1;

    if (typeof tabletData.recents [index] === "object")
        tabletData.recents [index].duration = duration;
}
gm.events.add(clientName + "updateRecent", updateRecent);

const getRecents = () => {
    let contactsData = [];

    tabletData.recents.forEach((recent) => {
        contactsData.unshift({
            ...recent,
            ...global.getContact (recent.number),
        });
    });

    return contactsData;
}

rpc.register(rpcName + "getRecents", () => {
    return JSON.stringify (getRecents ());
});

rpc.register(rpcName + "recentsClear", () => {
    tabletData.recents = [];
    mp.events.callRemote(serverName + "recentsClear");
    return true;
});