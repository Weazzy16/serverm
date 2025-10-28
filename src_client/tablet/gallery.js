const
    clientName = "client.tablet.",
    rpcName = "rpc.tablet.",
    serverName = "server.tablet.";

tabletData.gallery = [];

gm.events.add(clientName + "initGallery", (galleryJson) => {
    tabletData.gallery = JSON.parse(galleryJson);
});

rpc.register(rpcName + "getGallery", () => {
    return JSON.stringify(tabletData.gallery.reverse());
});

gm.events.add(clientName + "addGallery", (link) => {
    mp.events.callRemote(serverName + "addGallery", link);
})

gm.events.add(clientName + "dellGallery", (link) => {

    const index = tabletData.gallery.findIndex(a => a[0] === link);
    if (tabletData.gallery [index])
        tabletData.gallery.splice(index, 1);

    mp.events.callRemote(serverName + "dellGallery", link);
});

gm.events.add(clientName + "pushGallery", (json) => {

    json = JSON.parse(json);

    tabletData.gallery.push(json);

    //mp.gui.emmit(`window.events.callEvent("cef.tablet.confirm", ${value})`);
});

