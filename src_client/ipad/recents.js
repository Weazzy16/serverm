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

/***
 * Recents - [{}...]
 *      Number - int
 *      Time - DateTime
 *      IsCall - Bolean,
 *      Duration - int
 */

ipadData.recents = [];

gm.events.add(clientName + "initRecents", (json) => {
    json = JSON.parse(json);

    json.forEach((item) => {
        const recentData = {
            number: item[0],
            time: item[1],
            isCall: item[2],
            duration: item[3]
        }

        ipadData.recents.push(recentData);
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

    const index = ipadData.recents.findIndex(r => r.number === number);

    if (typeof ipadData.recents [index] === "object")
        ipadData.recents.splice(index, 1);

    ipadData.recents.push(recentData);

    if (ipadData.recents.length >= maxRecent)
        ipadData.recents.splice(ipadData.recents.length - 1, 1);
}

gm.events.add(clientName + "addRecent", addRecents);

const updateRecent = (duration) => {
    const index = ipadData.recents.length - 1;

    if (typeof ipadData.recents [index] === "object")
        ipadData.recents [index].duration = duration;
}
gm.events.add(clientName + "updateRecent", updateRecent);

const getRecents = () => {
    let contactsData = [];

    ipadData.recents.forEach((recent) => {
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
    ipadData.recents = [];
    callRemote(serverName + "recentsClear");
    return true;
});