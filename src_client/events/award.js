const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let IsCompensation = false;

gm.events.add("client.everydayawards", async (isInit, RealDay, json, WeekTime, donate) => {
    try
    {
		if (global.menuCheck() && !global.gamemenu) return;
        IsCompensation = isInit;
        mp.gui.emmit(`window.gameMenuView ("EverydayReward");`);
        gm.discord(translateText("Изучает ежедневные награды"));
        await global.wait(50);       
		mp.gui.emmit(`window.events.callEvent("cef.everydayreward.init", ${RealDay}, '${json}', ${WeekTime}, ${donate})`);
        if (!global.gamemenu) global.binderFunctions.GameMenuOpen ();
    }
    catch (e) 
    {
        callRemote("client_trycatch", "events/award", "client.everydayawards", e.toString());
    }
});

gm.events.add("client.everydayawards.close", () => {
    try
    {
        if (IsCompensation)
            callRemote('IsCompensation');

        IsCompensation = false;
    }
    catch (e) 
    {
        callRemote("client_trycatch", "events/award", "client.everydayawards.close", e.toString());
    }
});

gm.events.add("client.everydayawards.take", () => {
    callRemote('server.everydayaward.take')
    global.binderFunctions.GameMenuClose ()
});

gm.events.add("client.everydayawards.checkbox", () => {
    callRemote('server.everydayaward.checkbox')
});

gm.events.add("client.everydayawards.open", () => {
    callRemote('server.everydayaward.open', false)
});

