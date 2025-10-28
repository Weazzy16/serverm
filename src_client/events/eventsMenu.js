const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.events.add('eventsMenuShow', () => {
    try
    {
        if (global.menuCheck() || global.eventsMenuActive) return;
        mp.gui.emmit(`window.router.setView("GamesOtherMain")`);
        global.menuOpen();
        global.eventsMenuActive = true;
    }
    catch (e)
    {
        callRemote("client_trycatch", "events/eventsMenu", "eventsMenuShow", e.toString());
    }
});

gm.events.add('eventsMenuHide', () => {
    try
    {
        if (!global.eventsMenuActive) return;
        mp.gui.emmit(`window.router.setHud();`);
        global.menuClose();
        global.eventsMenuActive = false;
    }
    catch (e)
    {
        callRemote("client_trycatch", "events/eventsMenu", "eventsMenuHide", e.toString());
    }
});

gm.events.add('selectEventClient', (index) => {
    call('eventsMenuHide');
    callRemote('selectEventServer', index);
});