const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let helpMenuState = false;

let isOpenEveryDayAward = false;

global.closeHelpMenu = () => {
    if(helpMenuState) {
        mp.gui.emmit(`window.router.setHud();`);
        helpMenuState = false;
        global.menuClose();
        mp.gui.cursor.visible = false;
    }

    if (isOpenEveryDayAward) {
        isOpenEveryDayAward = false;
        call('client.battlepass.open');
        //callRemote('server.everydayaward.open', false);
    }
}

global.isNewChar = false;

gm.events.add('client:OnOpenHelpMenu', () => {
    global.isNewChar = true;
});


/*
gm.events.add('client:OnOpenHelpMenu', () => {
    try
    {
        call('notify', 3, 9, translateText("Не забудь подойти к NPC Виталий ЗДебич, чтобы начать стартовую линейку квестов, которая познакомит тебя с нашим чудесным штатом!"), 3000);
        if (questNameToPeds [startActorName] && mp.peds.exists (questNameToPeds [startActorName])) {
            global.createCamera ("peds", questNameToPeds [startActorName]);
            global.localplayer.freezePosition(true);

            setTimeout(function() {
                mp.gui.chat.push(translateText("Не забудь подойти к !{#ff3333}NPC Виталий ЗДебич!{#FFF}, чтобы начать стартовую линейку квестов, которая познакомит тебя с нашим чудесным штатом!"));

                global.cameraManager.deleteCamera ('peds', true, 500);
                global.localplayer.freezePosition(false);

                global.binderFunctions.o_help ();
            }, 10000);
        } else
            global.binderFunctions.o_help ();
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/helpmenu", "client:OnOpenHelpMenu", e.toString());
    }
});*/


gm.events.add('client:OnCloseHelpMenu', () => {
    closeHelpMenu();
});

global.binderFunctions.o_help = () => {
	if(global.chatActive) return;
    if(!global.menuCheck() && !helpMenuState)
    {
        global.menuOpen();
        mp.gui.emmit(`window.router.setView("PlayerHelp")`);
        gm.discord(translateText("Изучает меню помощи"));
        helpMenuState = true;
        mp.gui.cursor.visible = true;
    }
    else closeHelpMenu();
};

global.binderFunctions.c_help = () => {
    closeHelpMenu()
};

gm.events.add('client.help.waypoint', (waypoint) => {
    callRemote('server.help.waypoint', waypoint);
});