import { HOOK, registerHook } from "./e-sound/data/hook";
import SoundManager from "./e-sound/controller";

const localPlayer = mp.players.local;

const callRemote = mp.events.callRemote;
const call = mp.events.call;

export default new class VehicleAudio {
    isOpened = false

    constructor() {
        mp.game.audio.setUserRadioControlEnabled(false);
        mp.game.audio.setFrontendRadioActive(false);

        mp.keys.bind(0x51, false, this.onKeyBind.bind(this));

        gm.events.add("client.vehicleAudio.open", this.open.bind(this));
        gm.events.add("client.vehicleAudio.close", this.close.bind(this));

        gm.events.add("e-dev.vehicleAudio.server", (eventName, ...args) => callRemote(eventName, ...args))

        registerHook(HOOK.UPDATE, this.updateRadio.bind(this));
        registerHook(HOOK.CREATED, this.updateRadio.bind(this));
    }

    updateRadio(soundEntity) {
        if (localPlayer.vehicle == null || localPlayer.vehicle.handle != soundEntity.entity.handle || !this.isOpened)
            return;

        this.open(soundEntity.id);
    }

    onKeyBind() {
        if (!global.loggedin 
            || mp.gui.chat.isTypingInTextChat || (global.menuOpened && !this.isOpened) || !localPlayer.vehicle 
            || localPlayer.vehicle.getPedInSeat(-1) != localPlayer.handle) 
            return;
        
        if (!this.isOpened)
            callRemote("server.vehicleAudio.open");
        else
            call("client.vehicleAudio.close");
    }

    open(id) {
        if (localPlayer.vehicle == null || localPlayer.vehicle.getPedInSeat(-1) != localPlayer.handle) 
            return;
    
        const soundData = SoundManager.getSoundByHandle(localPlayer.vehicle.handle);
    
        const volume = soundData ? soundData._volume : 100;
        const pausing = soundData ? soundData.paused : true;
        const loop = soundData ? soundData.loop : false;
        const url = soundData ? soundData.url : "";
    
        SoundManager.browser.execute(`AudioPlayerUI.Open('${id}', '${url}', ${volume}, ${pausing}, ${loop})`);
        this.isOpened = true;
    
        if (!global.menuOpened)
            global.menuOpen()
    }

    close() {
        if (!this.isOpened) 
            return;

        SoundManager.browser.execute(`AudioPlayerUI.Reset()`);
        global.menuClose();

        this.isOpened = false;
    }
}