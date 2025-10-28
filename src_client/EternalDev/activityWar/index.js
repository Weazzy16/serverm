const localPlayer = mp.players.local;

const eventsAdd = mp.events.add;
const callRemote = mp.events.callRemote;

export default new class ActivityWar {
    isCaptured = false
    blips = []

    TIME_TO_WAR = 240

    constructor() {
        gm.events.add("client.activityWar.open", this.openUi.bind(this));
        gm.events.add("client.activityWar.close", this.closeUi.bind(this));
        
        gm.events.add("client.activityWar.start", this.startActivity.bind(this));
        gm.events.add("client.activityWar.stop", this.stopActivity.bind(this));

        gm.events.add("client.activityWar.setCaptured", this.setCaptured.bind(this));

        eventsAdd("playerWeaponShot", this.onShoot.bind(this));
        eventsAdd("playerWeaponShot", this.onQuit.bind(this));
        eventsAdd("render", this.render.bind(this));

        global.binderFunctions.ActivityWar = () => this.keyBind();
        setInterval(() => this.interval(), 1000);
    }

    keyBind() {
        if (global.menuCheck() || global.chatActive || mp.players.local.getVariable('InDeath') == true) 
            return;
        callRemote("server.activityWar.open");
    }

    onShoot(targetPosition, targetEntity) {
        if (targetEntity != null && targetEntity.type == 'player' && targetEntity.getVariable("IS_PLAYER_AW_CAPTURED") != null) {
            callRemote("server.activityWar.shootEvent", targetEntity.remoteId);
        }
    }

    openUi(price, count, data) {
        global.menuOpen();

        mp.gui.emmit(`window.router.setView('ActivityWar');`);
        mp.gui.emmit(`window.setActivityWarData(${count}, ${price}, ${data})`);
    }

    closeUi() {
        global.menuClose();
        mp.gui.emmit(`window.router.setHud();`);
    }

    setCaptured(toggle) {
        this.isCaptured = toggle;
        localPlayer.freezePosition(toggle);
    }

    startActivity(id, x, y, z, radius, time, name, isDeff) {
        let prevBlip = this.getActivityBlip(id);

        if (prevBlip == null) {
            let position = new mp.Vector3(x, y, z);
    
            let blip = mp.game.ui.addBlipForRadius(parseFloat(position.x), parseFloat(position.y), parseFloat(position.z), parseFloat(radius));
            mp.game.invoke("0xDF735600A4696DAF", blip, 9);
            mp.game.invoke("0x45FF974EEE1C8734", blip, 100);
            mp.game.invoke("0x03D7FB09E75D6B7E", blip, 1);
    
            if (prevBlip == null) {
                this.blips.push({ID: id, Blip: blip, Time: time });
            }
            else {
                mp.game.ui.removeBlip(prevBlip.Blip);
                prevBlip.Blip = blip;    
            }
        }
    
        mp.gui.emmit(`window.hudStore.addActivityFamily(${id}, '${name}', ${!isDeff});`);
    }

    stopActivity(id) {
        let blip = this.getActivityBlip(id);
        if (blip != null) {
            mp.game.ui.removeBlip(blip.Blip);
        }

        this.blips.splice(this.blips.indexOf(blip), 1);
        mp.gui.emmit(`window.hudStore.deleteActivity(${id})`);
    }

    interval() {
        this.blips.forEach(element => {
            if (element.Time >= 0) {
                var minutes = Math.floor(element.Time / 60);
                var seconds = (element.Time % 60).toFixed();

                element.Time--;
        
                let text = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
                mp.gui.emmit(`window.hudStore.updateActivityTime(${element.ID}, '${text}');`);
            }
        });
    }
    
    onQuit(player) {
        if (player == localPlayer) {
            this.blips.forEach(element => {
                if (element.Blip != null)
                    mp.game.ui.removeBlip(element.Blip);
            });
        }
    }

    getActivityBlip(id) {
        return this.blips.find(x => x.ID == id);
    }

    render() {
        if (this.isCaptured) {
            localPlayer.freezePosition(true);

            mp.game.controls.disableAllControlActions(2);

            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LR, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UD, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UP_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_DOWN_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LEFT_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_RIGHT_ONLY, true);
        }
    }
};