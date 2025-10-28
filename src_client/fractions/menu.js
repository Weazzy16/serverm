const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let isFractionMenu = false;

global.binderFunctions.open_Table = () => {
    if (global.menuCheck()) return;

    if (global.fractionId !== 0 && global.organizationId !== 0)
        global.OpenCircle(translateText("Открыть планшет"), 0);
    else if (global.fractionId === 15 || (global.fractionId === 6 && global.isLeader))
        global.OpenCircle(translateText("Открыть планшет"), 0);
    else if (global.fractionId !== 0) {
        mp.gui.emmit(`window.gameMenuView ("Fractions");`);
        if (!global.gamemenu)
            global.binderFunctions.GameMenuOpen ();
    }
    else if (global.organizationId !== 0) {
        mp.gui.emmit(`window.gameMenuView ("Organization");`);
        if (!global.gamemenu)
            global.binderFunctions.GameMenuOpen ();
    }
}

gm.events.add('client.table.open', async (usersList, vehiclesList, boardList, settings, defaultAccess, access, updateInfo, clothesList, isOrgTable = false) => {
    try 
    {
        //if (global.menuCheck()) return;
        await global.awaitMenuCheck ();
        global.menuOpen();
        mp.gui.emmit(
            `window.router.setView("FractionsMenu", {usersList: '${usersList}', vehiclesList: '${vehiclesList}', boardList: '${boardList}', settings: '${settings}', defaultAccess: '${defaultAccess}', access: '${access}', updateInfo: '${updateInfo}', clothesList: '${clothesList}', isOrgTable: '${isOrgTable}'})`
        );
        isFractionMenu = true;
        gm.discord(translateText("Изучает фракционный планшет"));
    }
    catch (e) 
    {
        callRemote("client_trycatch", "fractions/mats", "matsOpen", e.toString());
    }
});

global.closeFractionTableMenu = () => {
    if (!isFractionMenu)
        return;
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
    isFractionMenu = false;
}

gm.events.add('client.table.rank', (isUp, name) => {//+
    callRemote('server.table.rank', isUp, name);
});

gm.events.add('client.table.irank', (rank, name) => {//+
    callRemote('server.table.irank', rank, name);
});

gm.events.add('client.table.call', (text, name) => {//+
    callRemote('server.table.call', text, name);
});

gm.events.add('client.table.uninvite', (name) => {//+
    callRemote('server.table.uninvite', name);
});

gm.events.add('client.table.invite', (text) => {//+
    callRemote('server.table.invite', text);
});

gm.events.add('client.table.fracad', (text) => {//+
    callRemote('server.table.fracad', text);
});

gm.events.add('client.table.ufracad', (index, text) => {//+
    callRemote('server.table.ufracad', index, text);
});

gm.events.add('client.table.dfracad', (index) => {//+
    callRemote('server.table.dfracad', index);
});

gm.events.add('client.table.gps', (number) => {//+
    callRemote('server.table.gps', number);
});

gm.events.add('client.table.evacuation', (number) => {//+
    callRemote('server.table.evacuation', number);
});

gm.events.add('client.table.gethistory', (name, pageId) => {
    callRemote('server.table.gethistory', name, pageId);
});

gm.events.add('client.table.logs', (json) => {
    mp.gui.emmit(`window.events.callEvent("cef.table.hget", '${json}')`);
});

gm.events.add('client.table.vrank', (isUp, number) => {//+
    callRemote('server.table.vrank', isUp, number);
});

gm.events.add('client.table.clothingSetRank', (isUp, name, gender) => {
    callRemote('server.table.clothingSetRank', isUp, name, gender);
});

gm.events.add('client.table.startEditClothingSet', (oldName, newName, gender) => {
    global.closeFractionTableMenu ();
    callRemote('server.table.startEditClothingSet', oldName, newName, gender);
});

gm.events.add('client.table.editClothingSet', (dictionary, id, texture) => {
    callRemote('server.table.editClothingSet', dictionary, id, texture);
});





gm.events.add('client.table.createrank', (rankName) => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.createrank', rankName);
});

gm.events.add('client.table.editrank', (index, rankName) => {//+
    callRemote('server.table.editrank', index, rankName);
});

gm.events.add('client.table.accessdelete', (index, accessIndex) => {//+
    callRemote('server.table.accessdelete', index, accessIndex);
});

gm.events.add('client.table.accessadd', (index, accessIndex) => {//+
    callRemote('server.table.accessadd', index, accessIndex);
});

gm.events.add('client.table.event', (index) => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.event', index);
});




gm.events.add('client.table.dellorg', () => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.dellorg');
});



gm.events.add('client.table.dellrank', (index) => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.dellrank', index);
});

gm.events.add('client.table.sellcar', (number) => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.sellcar', number);
});

gm.events.add('client.table.leave', () => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.leave');
});

gm.events.add('client.table.defaultrank', () => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.defaultrank');
});

gm.events.add('client.table.defaultvrank', () => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.defaultvrank');
});

gm.events.add('client.table.upgrade', (type) => {//+
    callRemote('server.table.upgrade', type);
});

gm.events.add('client.table.tuning', () => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.tuning');
});

gm.events.add('client.table.dron', () => {//+
    global.closeFractionTableMenu ();
    callRemote('server.table.dron');
});

gm.events.add('client.table.reprimand', (uuid, name, text) => {
    callRemote('server.table.reprimand', uuid, name, text);
});