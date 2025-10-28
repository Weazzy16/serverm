const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let isInterface = false;

global.input = {
    head: "",
    desc: "",
    len: "",
    cBack: "",
    set: function (h, d, l, c) {
        this.head = h, this.desc = d;
        this.len = l, this.cBack = c;
        //if (global.menuCheck()) return;
        mp.gui.emmit(
            `window.router.setPopUp("PopupInput", {title: '${this.head}', plholder: '${this.desc}', length: ${this.len}});`
        );
        mp.gui.cursor.visible = true;
        isInterface = false;
        if (!global.menuCheck ()) {
            global.menuOpen();
            isInterface = true;
            global.isPopup = true;
        }
    },
    open: function () {
        if (global.menuCheck()) return;
        mp.gui.emmit(`window.router.setPopUp("PopupInput")`);
        global.menuOpen();
        //call('startScreenEffect', "MenuMGHeistIn", 1, true);
    },
    close: function () {
        mp.gui.emmit(`window.router.setPopUp()`);
        if (isInterface) 
            global.menuClose();
        isInterface = false;
        global.isPopup = false;
        //call('stopScreenEffect', "MenuMGHeistIn");
    }
};

gm.events.add('input', (text) => {
    try
    {
        if (input.cBack == "") return;

        if (input.cBack == "join_private_lobby")
            call('airsoft_joinPrivateLobby_client', 2, text);
        else if (input.cBack == "mafia_join_private_lobby")
            call('mafia_joinPrivateLobby_client', 2, text);
        else if (input.cBack == "tanks_join_private_lobby")
            call('tanks_joinPrivateLobby_client', 2, text);
        else if (input.cBack == "setCruise")
            call('setCruiseSpeed', text);
        else if (input.cBack == "boombox")
            callRemote('setFirstBoomboxURL', text);
        else if (input.cBack == "update_boombox_url")
            callRemote('updateBoomboxURL', text);
        else if (input.cBack == 'take_frac_ammo')
            callRemote('server.takeFractionAmmo', text);
        else if (input.cBack == "sendReportFromClientInput")
            callRemote('sendReportFromClient', text);
        else 
            callRemote('inputCallback', input.cBack, text);
        
            input.cBack = "";
        input.close();
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/input", "input", e.toString());
    }
});

gm.events.add('openInput', (h, d, l, c) => {
    //if (global.menuCheck()) return;
    input.set(h, d, l, c);
    //input.open();
})

gm.events.add('closeInput', () => {
    input.close();
})