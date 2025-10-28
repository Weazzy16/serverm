// DIAL //
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
var vall, off;

gm.events.add('dial', (act, val, reset) => {
    try
    {
        switch (act) {
            case "open":
                if (reset == true) 
                {
                    mp.gui.emmit('window.router.setHud()');
                    global.menuClose();
                }
                var off = Math.random(2, 5);

                val = val;
                off = off;
                mp.gui.emmit(
                    `window.router.setView("PlayerBreakingLock", {value: ${val}, off: ${off}})`
                );
                global.menuOpen();
                break;
            case "close":
                mp.gui.emmit('window.router.setHud()');
                global.menuClose();
                break;
            case "call":
                callRemote('dialPress', val);
                global.menuClose();
                break;
        }
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/dial", "dial", e.toString());
    }
});