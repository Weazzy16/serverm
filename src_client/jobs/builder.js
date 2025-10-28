const callRemote = mp.events.callRemote;


const browser = mp.browsers.new(`http://package/cef/Builder/index.html`);

let callbackEvent = "";
mp.events.add("client.buildermenu.start", (callback) => {
    callbackEvent = callback;
    browser.execute(`app.init()`);
    global.menuOpen();
});

mp.events.add("client.buildermenu.end", () => {
    callRemote(callbackEvent);
    callbackEvent = "";
    global.menuClose();
});