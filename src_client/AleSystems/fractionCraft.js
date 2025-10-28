const callRemote = mp.events.callRemote;
const browsers = mp.browsers;
const CraftBrowser = browsers.new(`http://package/cef/Crafting/index.html`);

mp.events.add({
    "client.fractions.crafting.open": (name, items) => {
        CraftBrowser.execute(`app.open('${name}', ${items})`)
        global.menuOpen();
    },

    "client.fractions.crafting.close": () => {
        CraftBrowser.execute(`app.reset()`)
        global.menuClose();
    },

    "client.fractions.crafting.do": (index) => {
        callRemote("server.fractions.crafting.do", index);
    }
});