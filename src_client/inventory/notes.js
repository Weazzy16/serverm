const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let NotesVisible = false;
global.OpenNotes = (text) => {
    if (global.menuCheck()) return;
    else if (NotesVisible) return;
    if (!text) mp.gui.emmit(`window.router.setView("EventsValentine");`);
    else mp.gui.emmit(`window.router.setView("EventsValentine", '${text}');`);
    NotesVisible = true;
    global.menuOpen();
}

gm.events.add("client.note.close", () => {
    try
    {
        if (!NotesVisible) return;
        mp.gui.emmit(`window.router.setHud();`);
        NotesVisible = false;
        global.menuClose();
    }
    catch (e) 
    {
        callRemote("client_trycatch", "inventory/notes", "client.note.close", e.toString());
    }
});

gm.events.add("client.note.create", (type, ItemId, nameValue, textValue) => {
    try
    {
        if (!NotesVisible) return;
        call('client.note.close');
        callRemote('server.note.create', type, ItemId, nameValue, textValue);
        gm.discord(translateText("Пишет записку"));
    }
    catch (e) 
    {
        callRemote("client_trycatch", "inventory/notes", "client.note.create", e.toString());
    }
});

gm.events.add("client.note.open", (json) => {
    global.OpenNotes (json);
    gm.discord(translateText("Читает записку"));
});