const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";

phoneData.gallery = [];

gm.events.add(clientName + "initGallery", (galleryJson) => {
    phoneData.gallery = JSON.parse(galleryJson);
});

rpc.register(rpcName + "getGallery", () => {
    return JSON.stringify(phoneData.gallery.reverse());
});

gm.events.add(clientName + "addGallery", (link) => {
    callRemote(serverName + "addGallery", link);
})

gm.events.add(clientName + "dellGallery", (link) => {

    const index = phoneData.gallery.findIndex(a => a[0] === link);
    if (phoneData.gallery [index])
        phoneData.gallery.splice(index, 1);

    callRemote(serverName + "dellGallery", link);
});

gm.events.add(clientName + "pushGallery", (json) => {

    json = JSON.parse(json);

    phoneData.gallery.push(json);

    //mp.gui.emmit(`window.events.callEvent("cef.phone.confirm", ${value})`);
});

