import { ALERT_MARKER_COLORS, ALERT_MARKER_DISTANCE, ALERT_MARKER_DURATION } from "../config";
import { AlertMarker } from "./marker";
const callRemote = mp.events.callRemote;

const localPlayer = mp.players.local;
export default new class AlertMarkerManager {
    constructor() {
        mp.events.add({
            "e-dev.crew-system.alert-marker": this.createMarker.bind(this),
            "render": this.onTick.bind(this)
        });

        global.binderFunctions.AlertCrewMarker = () => this.onBind();
    }

    markers = [];
    createMarker(position, type = null) {
        if (global.vdist2(position, localPlayer.position) > ALERT_MARKER_DISTANCE)
            return;

        const marker = new AlertMarker();

        if (type != null)
            marker.settings.colorSprite = this.getColor(type);

        marker.settings.worldPosition = position;
        marker.toggle(true);

        this.markers.push(marker);
        global.wait(ALERT_MARKER_DURATION).then(() => { this.markers.splice(this.markers.indexOf(marker), 1) });
    }

    getColor(type) {
        return ALERT_MARKER_COLORS[type];
    }

    timeout = 0;
    onBind() {
        if (!global.loggedin || global.menuCheck() || Date.now() - this.timeout < 1000) 
            return;

        const gameplayCam = mp.cameras.gameplay.getCoord(); 
        const cameraDirection = mp.cameras.gameplay.getDirection(); 
        const endPosition = new mp.Vector3(
            ALERT_MARKER_DISTANCE * cameraDirection.x + gameplayCam.x, 
            ALERT_MARKER_DISTANCE * cameraDirection.y + gameplayCam.y, 
            ALERT_MARKER_DISTANCE * cameraDirection.z + gameplayCam.z
        );

        const result = mp.raycasting.testPointToPoint(gameplayCam, endPosition, mp.players.local, 17);
        if (result) {
            callRemote("e-dev.crew-system.alert-marker", JSON.stringify(result.position));
        }

        this.timeout = Date.now();
    }

    onTick() {
        // if (mp.game.controls.getDisabledControlNormal(0, 348)) {
        //     if (Date.now() - this.timeout > 1000) {
        //         this.timeout = Date.now();
        //         this.onBind();
        //     }
        // }

        this.markers.forEach(marker => marker && marker.onTick());
    }
}