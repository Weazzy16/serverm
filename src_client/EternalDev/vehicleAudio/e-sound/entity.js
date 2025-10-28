import { HOOK, invokeHook } from "./data/hook";
import { SOUND_TYPE } from "./data/type";

const localPlayer = mp.players.local;
const gameplayCamera = mp.cameras.new("gameplay");

export class SoundEntity {
    browser = null

    constructor(browser, id, url, type, entity, {
        looped,
        startOffest,
        volume,
        maxDistance,
        isPausing
    }) {
        if (!this.isValidEntity(entity))
            return

        this.browser = browser;

        this.id = id;

        this.entity = entity;
        this.type = type;

        this.url = url;

        this.loop = looped == null ? false : looped;
        this.startOffest = startOffest == null ? 0 : startOffest;
        this.volume = volume == null ? 100 : volume;
        this.maxDistance = maxDistance == null ? 10 : maxDistance;

        this.muted = true;
        this.paused = isPausing == null ? false : isPausing;

        this.hasLos = true;
        this.lastHasLos = true;

        this.emit(`SoundManager.Play3D('${this.id}', '${this.url}', 0, ${this.loop}, ${this.startOffest})`);
        invokeHook(HOOK.CREATED, this);
    }

    emit(code) {
        if (this.browser == null)
            return;
         
        this.browser.execute(code);
    }

    isValidEntity(entity) {
        return mp.players.exists(entity) || mp.vehicles.exists(entity) || mp.objects.exists(entity) || mp.peds.exists(entity);
    }

    _loop = false
    set loop(value) {
        this._loop = value;
        // Not implemented
    }

    get loop() {
        return this._loop;
    }

    _volume = 0
    set volume(value) {
        this._volume = value;
    }

    get volume() {
        let multiplayer = 1;
        switch(this.type) {
            case SOUND_TYPE.VEHICLE:
                if (localPlayer.vehicle && localPlayer.vehicle.handle == this.entity.handle)
                    multiplayer = 1;
                else
                    multiplayer = 0.4;
                break;
        }

        return (this._volume / 100) * multiplayer;
    }

    worker() {
        if (!this.isValidEntity(this.entity)) 
            return;

        const soundPosition = this.entity.getCoords(true);
        const distance = mp.game.gameplay.getDistanceBetweenCoords(soundPosition.x, soundPosition.y, soundPosition.z, 
            localPlayer.position.x, localPlayer.position.y, localPlayer.position.z);
        
        let volume = 0;
        if (distance > this.maxDistance + 2) {
            this.muted = true;
            return;
        }

        this.muted = false;
        volume = this.volume * (1 - (distance / this.maxDistance));

        let useLowpassEffect = false;
        let lowpassStrength = 600;

        switch(this.type) {
            case SOUND_TYPE.VEHICLE:
                if (localPlayer.vehicle == null || this.entity.handle != localPlayer.vehicle.handle) {
                    useLowpassEffect = true;

                    const doorsCount = mp.game.vehicle.getVehicleModelMaxNumberOfPassengers(this.entity.model);
                    const maxStrengthDoor = (1200 / doorsCount) * 5;
                    const maxAddedVolume = .1 / doorsCount;

                    for(let i = 0; i < doorsCount; i++) {
                        const doorAngeRatio = this.entity.getDoorAngleRatio(i)
                        lowpassStrength += maxStrengthDoor * doorAngeRatio;
                        volume += maxAddedVolume * doorAngeRatio;
                    }
                }
                break;
            case SOUND_TYPE.PLAYER:
                // Player sounds
                break;
        }

        const pan = !useLowpassEffect ? 0 : this.calculatePan();
        this.emit(`SoundManager.UpdateSound3d('${this.id}', ${volume * volume}, ${pan}, ${useLowpassEffect}, ${lowpassStrength})`);
    }

    calculatePan() {
        const cam_pos = localPlayer.position;
        const cam_vector = gameplayCamera.getDirection();
        const entityPosition = this.entity.getCoords(true);

        const target_pos = new mp.Vector3(entityPosition.x, entityPosition.y, entityPosition.z);
        const target_vector = { x: target_pos.x-cam_pos.x, y: target_pos.y-cam_pos.y };
       
        let dx = target_vector.x * cam_vector.x + target_vector.y * cam_vector.y;
        let dy = Math.sqrt(cam_vector.x * cam_vector.x + cam_vector.y * cam_vector.y) * Math.sqrt(target_vector.x * target_vector.x + target_vector.y * target_vector.y);
        let s = cam_vector.x * (target_pos.y - cam_pos.y) - cam_vector.y * (target_pos.x - cam_pos.x);
       
        let a = .9;

        if (s > 0) 
            a = -.9
        else if (s < 0) 
            a = .9
        else 
            a = 0;
        
        let pan = Math.sqrt(1 - dx / dy * (dx / dy)) * a;
        return pan;
    }

    _muted = false
    get muted() {
        return this._muted;
    }

    set muted(value) {
        if (this._muted == value) 
            return;

        this._muted = value;

        this.emit(`SoundManager.SetMuted3d('${this.id}', ${this._muted})`);
        invokeHook([HOOK.SET_MUTED, HOOK.UPDATE], this);
    }

    _paused = false
    get paused() {
        return this._paused;
    }

    set paused(value) {
        if (this._paused == value) 
            return;
        
        this._paused = value;

        this.emit(`SoundManager.SetPaused3d('${this.id}', ${this._paused})`);
        invokeHook([HOOK.SET_PAUSED, HOOK.UPDATE], this);
    }

    destroy() {
        this.emit(`SoundManager.Stop3D('${this.id}')`);
        invokeHook(HOOK.DESTROY, this);
    }
}