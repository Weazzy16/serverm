const localPlayer = mp.players.local;

const requestAnimDict = animDict => new Promise(async (resolve, reject) => {
    if (mp.game.streaming.hasAnimDictLoaded(animDict))
        return resolve(true);

    mp.game.streaming.requestAnimDict(animDict);
    while(!mp.game.streaming.hasAnimDictLoaded(animDict)) {
        await mp.game.waitAsync(1);
    }

    return resolve(true);
});

mp.events.addDataHandler("im.followed", (entity, value, oldValue) => {
    if (value != -1 && mp.players.atRemoteId(value) != null) {
        let cop = mp.players.atRemoteId(value);
        dragTarget(cop, entity);
    }
    else if (entity.isSuspected) {
        if (entity.isAttached())
            entity.detach(true, true);

        entity.isSuspected = false;
        entity.clearTasks();
    }
});

mp.events.addDataHandler("follow.suspect", (entity, value, oldValue) => {
    if (value != -1 && mp.players.atRemoteId(value) != null) {
        let suspect = mp.players.atRemoteId(value);
        dragTarget(entity, suspect);
    }
    else if (entity.isDragging) {
        entity.isDragging = false;
        entity.clearTasks();

        if (entity.intervalChecker) 
            clearInterval(entity.intervalChecker);

        delete entity.intervalChecker;
        delete entity.suspect;
    }
});

mp.events.add("render", () => {
	if (localPlayer.getVariable("im.followed") != null && localPlayer.getVariable("im.followed") != -1) {
        mp.game.controls.disableAllControlActions(2);
        mp.game.controls.enableControlAction(2, 30, true);
        mp.game.controls.enableControlAction(2, 31, true);
        mp.game.controls.enableControlAction(2, 32, true);
        mp.game.controls.enableControlAction(2, 1, true);
        mp.game.controls.enableControlAction(2, 2, true);
    }
    
    if (localPlayer.getVariable("follow.suspect") != null && localPlayer.getVariable("follow.suspect") != -1) {
        if (!localPlayer.isPlayingAnim(dragAnimDict, "static", 3)) {
            localPlayer.taskPlayAnim(dragAnimDict, "static", 8.0, 1.0, -1, 49, 1.0, false, false, false);
        }
    }
});

const cuffAnimDict = "mp_arresting";
async function cuffTarget(cop, target) {
	if (!target || !cop || !target.handle || !cop.handle || !mp.players.exists(cop) || !mp.players.exists(target)) return;
    await requestAnimDict(cuffAnimDict);

    target.clearTasks();
    target.taskPlayAnim(cuffAnimDict, "idle", 8.0, 1.0, -1, 49, 1.0, false, false, false);
}

const dragAnimDict = "amb@code_human_wander_drinking_fat@female@base";
async function dragTarget(cop, target) {
    if (!target || !cop || !target.handle || !cop.handle || !mp.players.exists(cop) || !mp.players.exists(target) || target.isAttachedTo(cop.handle)) return;

    await cuffTarget(cop, target);
    await requestAnimDict(dragAnimDict);

    cop.taskPlayAnim(dragAnimDict, "static", 8.0, 1.0, -1, 49, 1.0, false, false, false);
    cop.isDragging = true;

    target.attachTo(cop.handle, 10000, .7, 0.4, 0, 0.0, 0.0, 0.0, false, false, false, false, 2, true);
    target.isSuspected = true;

    cop.suspect = target;

    cop.intervalChecker = setInterval(() => {
        if (cop && cop.suspect) {
            if (cop.isWalking() || cop.isRunning() || cop.isSprinting()) {
                const gotooPosition = cop.suspect.getOffsetFromInWorldCoords(0, 3, 1.32);
                const speed = cop.isSprinting() || cop.isRunning() ? 5 : 1;
                cop.suspect.taskGoStraightToCoord(gotooPosition.x, gotooPosition.y, gotooPosition.z, speed, 500, 130, 1.0);
                cop.suspect._walking = true;
            }
            else if (cop.suspect._walking) {
                var gotooPosition = cop.suspect.getOffsetFromInWorldCoords(0, 0, 1.32)
                cop.suspect.taskGoStraightToCoord(gotooPosition.x, gotooPosition.y, gotooPosition.z, 1, 2000, cop.suspect.getHeading(), 3.0);
                cop.suspect._walking = false;
            }
            mp.game.invoke("0x971D38760FBC02EF", cop.suspect.handle, true);
        }
        else {
            if (cop.intervalChecker != undefined) {
                clearInterval(cop.intervalChecker);
                delete cop.intervalChecker;
            }
            delete cop.suspect;
        }
		
		if (cop && cop.handle && cop.suspect && cop.suspect.handle)
            mp.game.invoke("0xC32779C16FCEECD9", cop.handle, 4, cop.suspect.handle, 45509, 0.18, 0.0, 0.1, 4, 50, 50);
    }, 1);
}