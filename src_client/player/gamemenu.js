const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let gamemenu_state = false;

global.binderFunctions.gamemenu = () => {
    if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk) return;

    if(!gamemenu_state && !global.menuCheck())
    {
        gamemenu_state = true;
        mp.gui.emmit(`window.router.updateStatic("PlayerGameMenu");`);
        global.menuOpen(true);
    }
    else if(gamemenu_state)
    {
        gamemenu_state = false;
        mp.gui.emmit(`window.router.setHud();`);
        global.menuClose();
    }
};