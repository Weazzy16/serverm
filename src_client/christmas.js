const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;

const browser = browsers.new(`http://package/interface/modules/ChristmasEvent/index.html`);

mp.events.add("client.christmas.gift.open", (type, name, data) => {
    browser.execute(`GiftUI.open('${type}', '${name}', '${data}')`); 
    global.menuOpen();
});

mp.events.add("client.christmas.gift.close", () => {
    browser.execute(`GiftUI.reset()`);
    global.menuClose();
});


mp.events.add("client.christmas.exChanger.action", (inedx) => callRemote("server.christmas.exChanger.action", inedx));

mp.events.add("client.christmas.exChanger.open", (name, items) => {
    browser.execute(`ExChangerUI.open('${name}', ${items})`);
    global.menuOpen();
});

mp.events.add("client.christmas.exChanger.close", () => {
    browser.execute(`ExChangerUI.reset()`);
    global.menuClose();
});