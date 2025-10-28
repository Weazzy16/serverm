// BODY SEARCH //
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
global.isBSearchActive = false;

gm.events.add('bsearch', (act) => {

    switch (act) {
        case 1:
            callRemote('pSelected', global.entity, translateText("Посмотреть лицензии"));
            break;
        case 2:
            callRemote('pSelected', global.entity, translateText("Посмотреть паспорт"));
            break;
        default:
            global.isBSearchActive = false;
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            break;
    }
});

gm.events.add('bsearchOpen', (data) => {
    if (global.menuCheck()) return;
    global.menuOpen();
    global.isBSearchActive = data;
    mp.gui.emmit(`window.router.setView("FractionsBSearch", ${data});`);
})