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

ipadData.gallery = [];

gm.events.add(clientName + "initGallery", (galleryJson) => {
    ipadData.gallery = JSON.parse(galleryJson);
});

rpc.register(rpcName + "getGallery", () => {
    return JSON.stringify(ipadData.gallery.reverse());
});

gm.events.add(clientName + "addGallery", (link) => {
    callRemote(serverName + "addGallery", link);
})

gm.events.add(clientName + "dellGallery", (link) => {

    const index = ipadData.gallery.findIndex(a => a[0] === link);
    if (ipadData.gallery [index])
        ipadData.gallery.splice(index, 1);

    callRemote(serverName + "dellGallery", link);
});

gm.events.add(clientName + "pushGallery", (json) => {

    json = JSON.parse(json);

    ipadData.gallery.push(json);

    //mp.gui.emmit(`window.events.callEvent("cef.ipad.confirm", ${value})`);
});

