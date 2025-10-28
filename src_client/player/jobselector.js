const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
var jobselectorOpened = false;

gm.events.add("showJobMenu", (lvlJson) => {
    try
    {
        if(!global.menuCheck() || jobselectorOpened == true)
        {
            global.menuOpen();
            mp.gui.cursor.visible = true;
            jobselectorOpened = true;
            gm.discord(translateText("Выбирает работу по душе"));

            mp.gui.emmit(
                `window.router.setView("PlayerJobSelector", '${lvlJson}')`
            );
        }
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/jobselector", "showJobMenu", e.toString());
    }
});

gm.events.add("closeJobMenu", () => 
{
    global.menuClose();
    mp.gui.cursor.visible = false;
    jobselectorOpened = false;
    mp.gui.emmit(`window.router.setHud()`);
});

global.binderFunctions.jobselectorOpened = () => {
    if(jobselectorOpened)
    {
        mp.gui.emmit(`window.router.setHud()`);
        jobselectorOpened = false;
        global.menuClose();
    }
};

gm.events.add("selectJob", (jobid) => {
    try
    {
        if (new Date().getTime() - global.lastCheck < 1000) return;
        global.lastCheck = new Date().getTime();
        callRemote("jobjoin", jobid);
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/jobselector", "selectJob", e.toString());
    }
});