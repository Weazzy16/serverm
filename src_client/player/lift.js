// ELEVATOR //
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
var liftcBack = "";

function openLift(type, cBack) {
    if (global.menuCheck()) return;
    let floors = [
        [translateText("Гараж"), translateText("1 этаж"), translateText("49 этаж"), translateText("Крыша")],
        [translateText("Гараж"), translateText("Бункер"), translateText("Крыша"), translateText("Склад")]
    ];
    mp.gui.emmit(`window.router.setView("PlayerLift", ${JSON.stringify (floors[type])})`);
    global.menuOpen();
    liftcBack = cBack;
}

function closeLift() {
    global.menuClose();
    mp.gui.emmit(`window.router.setHud();`);
    liftcBack = "";
}

gm.events.add('openlift', (type, cBack) => {
    openLift(type, cBack);
});

gm.events.add('lift', (act, data) => {
    switch (act) {
        case "stop":
            closeLift();
            break;
        case "start":
            callRemote(liftcBack, data);
            closeLift();
            break;
    }
});