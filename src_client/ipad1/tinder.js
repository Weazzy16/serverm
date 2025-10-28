const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.ipad.tinder.",
    rpcName = "rpc.ipad.tinder.",
    serverName = "server.ipad.tinder.";

gm.events.add(clientName + "load", () => {
    callRemote(serverName + "load");
});

let profile = {}
let likes = []
let list = []

const addList = (json) => {
    json = JSON.parse(json);
    json.forEach((item) => {
        list.push({
            uuid: item[0],
            name: item[1],
            avatar: item[2],
            text: item[3],
        })
    })
}

gm.events.add(clientName + "init", (_profile, _likes, _list) => {
    _profile = JSON.parse(_profile);
    profile = {
        avatar: _profile[0],
        text: _profile[1],
        type: _profile[2],
        isVisible: _profile[3],
    }

    likes = []

    _likes = JSON.parse(_likes);
    _likes.forEach((item) => {
        likes.push({
            name: item[0],
            avatar: item[1],
            number: item[2],
        })
    })

    list = []
    addList (_list);

    mp.gui.emmit(`window.listernEvent ('ipad.tinder.load', true);`);
});

gm.events.add(clientName + "create", () => {

    mp.gui.emmit(`window.listernEvent ('ipad.tinder.load', false);`);
});

//

rpc.register(rpcName + "getList", () => {
    return JSON.stringify(list);
});

gm.events.add(clientName + "action", (uuid, isLove) => {
    list.splice(0, 1);
    callRemote(serverName + "action", uuid, isLove);
});

gm.events.add(clientName + "addList", (_list, isClear) => {
    if (isClear)
        list = [];

    addList (_list);
    mp.gui.emmit(`window.listernEvent ('ipad.tinder.getList');`);
});

gm.events.add(clientName + "addLikes", (name, avatar, number) => {
    likes.push({
        name: name,
        avatar: avatar,
        number: number,
    })

    mp.gui.emmit(`window.listernEvent ('ipad.tinder.getLikes');`);
});

//

rpc.register(rpcName + "getLikes", () => {
    return JSON.stringify(likes.reverse());
});

//

gm.events.add(clientName + "save", (avatar, text, type, isVisible) => {
    if (!global.antiFlood("tinder_save", 500))
        return true;

    profile = {
        avatar: avatar,
        text: text,
        type: type,
        isVisible: isVisible
    }

    mp.gui.emmit(`window.listernEvent ('ipad.tinder.getProfile');`);
    callRemote(serverName + "save", avatar, text, type, isVisible);
});

//

rpc.register(rpcName + "getProfile", () => {
    return JSON.stringify(profile);
});