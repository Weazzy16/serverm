const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
global.reportactive = false;

gm.events.add('addreport', (id_, author_, quest_) => {
    mp.gui.emmit(`window.reportsStore.addReport(${id_},'${author_}','${quest_}')`);
	if(global.adminLVL <= 8) 
        call('notify', 0, 2, translateText("Пришел новый репорт!"), 3000);
})

gm.events.add('setreport', (id, name) => {
    mp.gui.emmit(`window.reportsStore.setStatus(${id}, '${name}')`);
})

gm.events.add('delreport', (id) => {
    mp.gui.emmit(`window.reportsStore.deleteReport(${id})`);
})
gm.events.add('addreportmsg', (id, author, msg) => {
    mp.gui.emmit(`window.reportsStore.addMessage(${id},'${author}','${msg}')`);
})
gm.events.add('takereport', (id) => {
    callRemote('takereport', id);
})
gm.events.add('otheradminreport', (id) => {
    callRemote('otheradminreport', id);
})
gm.events.add('closeticket', (id) => {
    mp.gui.emmit(`window.reportsStore.closeReport(${id})`);
})
gm.events.add('sendreport', (id, a) => {
    callRemote('sendreport', id, a);
})
gm.events.add('reportmsg', (msg) => {
    callRemote('addreportmsg', msg);
})
gm.events.add('funcreport', (id, a) => {
    global.binderFunctions.c_reports ();
    callRemote('funcreport', id, a);
})

gm.events.add('exitreport', () => {
    global.binderFunctions.c_reports ();
})
gm.events.add('closereport', (id) => {
    callRemote('closereport', id);
})
global.binderFunctions.c_reports = () => {
    mp.gui.emmit(`window.router.setHud();`)
    global.menuClose();
    global.reportactive = false;
    mp.gui.cursor.visible = false;
}

global.binderFunctions.o_reports = () => {// F6 key report menu
    if (!global.loggedin || global.chatActive || global.editing || global.advertsactive || new Date().getTime() - global.lastCheck < 1000) return;
    if (global.isAdmin != true) return;
    //global.lastCheck = new Date().getTime();
    if (!global.menuCheck ()) 
    {
        global.menuOpen();
        mp.gui.cursor.visible = true;
        global.reportactive = true;
        mp.gui.emmit(`window.router.setView('PlayerReports');`);
    } 
    else if(global.reportactive) global.binderFunctions.c_reports ();
};

var f10rep = new Date().getTime();

gm.events.add('f10report', (report) => {
	if (!global.loggedin || new Date().getTime() - f10rep < 3000) return;
    f10rep = new Date().getTime();

    callRemote('f10helpreport', report);
    global.closeHelpMenu();
});