import { currentScenario, mainCamera, majesticCef } from "../intro";
import CamerasManager from "../intro/modules/camerasManager";

const add = mp.events.add;
const call = mp.events.call;

export default new class SpawnSelector {
    opened = true

    hovered = ""
    positions = {}

    /**
     * Длительность перехода
     */
    TRANSITION_DURATION = 1000

    constructor() {
        add("render", this.onTick.bind(this));

        const EVENTS = {
            "open": this.open.bind(this),
            "hover": this.onHover.bind(this),
            "submit": (type) => call("client:OnSelectCharacterv2", this.uuid, type)
        }

        for (const [eventName, callback] of Object.entries(EVENTS)) {
            gm.events.add(`client.spawnSelector.${eventName}`, callback);
        }
    }

    get scenario() {
        return currentScenario.scenes.spawn
    }

    get camera() {
        return mainCamera
    }

    uuid = 0
    open(uuid, data) {
        try {
            this.uuid = uuid;
            this.positions = typeof(data) == 'string' ? JSON.parse(data) : data;
            this.hovered = "";

            call("e-dev.intro.switch_scene", "spawn", 700);
            majesticCef.execute(`
                characterSelector.reset();
                SpawnSelector.open(${JSON.stringify(this.positions)}); 
            `);

            this.opened = true;
        }
        catch(e) { mp.console.logError(e); }
    }

    clearHovers() {
        if (this.camera == null)
            return;

        CamerasManager.setActiveCameraWithInterpRotation(this.camera, 
            this.scenario.camPos, 
            this.scenario.camLookAt, 
            this.TRANSITION_DURATION, this.TRANSITION_DURATION * 5, this.TRANSITION_DURATION * 5, 
            this.scenario.fov);
        CamerasManager.pointAtCoord(this.camera, this.scenario.camLookAt);
    }

    onHover(type, unHovered = false) {
        if (this.camera == null)
            return;

        if (mp.game.streaming.isNewLoadSceneActive()) 
            mp.game.streaming.newLoadSceneStop();
        
        if (this.hovered != "" && type == "" && unHovered) {
            if (type == "")
                this.hovered = "";

            this.clearHovers();
        }

        mp.console.logInfo(JSON.stringify(this.places));

        const data = this.positions[type];
        if (data == null)
            return;

        this.hovered = type;
        const position = new mp.Vector3(data.Position.x, data.Position.y, 300);

        const heading = 40;
        if (position.y > 5000)
            heading = 220;

        const camPos = this.getObjectOffset(position, heading, new mp.Vector3(0, -1500, 300));

        CamerasManager.setActiveCameraWithInterpRotation(this.camera, 
            camPos, 
            position, 
            this.TRANSITION_DURATION, this.TRANSITION_DURATION * 5, this.TRANSITION_DURATION * 5, 
            this.scenario.fov, 
            {
                farDof: 3000
            });
        CamerasManager.pointAtCoord(this.camera, position);

        mp.game.streaming.newLoadSceneStartSphere(position.x, position.y, data.Position.z, 500, 0);
    }

    get places() {
        if (this.camera == null)
            return [];

        const positions = Object.entries(this.positions).sort((a, b) => this.distanceToPos(b[1].Position, this.camera.getCoord()) - this.distanceToPos(a[1].Position, this.camera.getCoord()));
        const list = positions.map(([key, data]) => {
            return {
                id: key,
                position: new mp.Vector3(data.Position.x, data.Position.y, data.Position.z),
                upped: false
            }
        });

        for (let place of list) {
            const nearestMarker = list.find(findPlace => {
                if (findPlace.id == place.id)
                    return false;

                return global.vdist2(place.position, findPlace.position, true) < 100 && !findPlace.upped;
            });

            if (nearestMarker != null)
                place.upped = true;
        };

        return list;
    }

    onTick() {
        if (!this.opened || this.camera == null)
            return;

        const places = [ ...this.places ];

        for (const place of places) {
            const textureName = this.hovered == place.id ? `spawn_${place.id}_hovered` : `spawn_${place.id}`;

            const position = new mp.Vector3(place.position.x, place.position.y, 300.33453 + Number(place.upped ? 125 : 0));
            const angle = this.getHeadingFromCoords(this.camera.getCoord(), position);

            mp.game.graphics.drawLine(position.x, position.y, position.z, position.x, position.y, 0, 255, 255, 255, 150);
            
            this.drawTexture3d(position, "majestic_textures_001", textureName, {
                scaleX: 120,
                scaleY: 120,
                heading: angle,
                alpha: 255,
            });
        }
    }

    getHeadingFromCoords(camPos, secondPos) {
        return 180 * Math.atan2(secondPos.y - camPos.y, secondPos.x - camPos.x) / Math.PI + 270;;
    }

    distanceToPos(v1, v2) {
        return Math.abs(Math.sqrt(Math.pow(v2.x - v1.x, 2) + Math.pow(v2.y - v1.y, 2) ));
    }

    getCameraOffset(pos, angle, dist) {
        const heading = angle * 0.0174533;
        const position = new mp.Vector3(pos.x, pos.y, pos.z);

        position.y = position.y + dist * Math.sin(heading);
        position.x = position.x + dist * Math.cos(heading);
        return position;
    }

    getObjectOffset = (position, heading, offset) => mp.game.object.getObjectOffsetFromCoords(
        position.x,
        position.y,
        position.z,
        heading,
        offset.x,
        offset.y,
        offset.z
    );
    
    drawTexture3d = (position, textureDict, textureName, {scaleX = 1, scaleY = 1, heading = 0, color = [255, 255, 255, 255]}) => {
        if (!mp.game.graphics.hasStreamedTextureDictLoaded(textureDict)) {
            mp.game.graphics.requestStreamedTextureDict(textureDict, true);
            return;
        }
    
        const pos1 = new mp.Vector3(-0.5 * scaleX, 0, 0.5 * scaleY);
        const pos2 = new mp.Vector3(0.5 * scaleX, 0, 0.5 * scaleY);
        const pos3 = new mp.Vector3(-0.5 * scaleX, 0, -0.5 * scaleY);
        const pos4 = new mp.Vector3(0.5 * scaleX, 0, -0.5 * scaleY);
    
        const finalPos1 = this.getObjectOffset(position, heading, pos1);
        const finalPos2 = this.getObjectOffset(position, heading, pos2);
        const finalPos3 = this.getObjectOffset(position, heading, pos3);
        const finalPos4 = this.getObjectOffset(position, heading, pos4);
    
        mp.game.invoke('0x29280002282F1928', finalPos1.x, finalPos1.y, finalPos1.z, finalPos3.x, finalPos3.y, finalPos3.z, finalPos2.x, finalPos2.y, finalPos2.z, color[0], color[1], color[2], color[3], textureDict, textureName, 1e-9, 1e-9, 1e-9, 1e-9, .999999999, 1e-9, .999999999, 1e-9, 1e-9);
        mp.game.invoke('0x29280002282F1928', finalPos3.x, finalPos3.y, finalPos3.z, finalPos4.x, finalPos4.y, finalPos4.z, finalPos2.x, finalPos2.y, finalPos2.z, color[0], color[1], color[2], color[3], textureDict, textureName, 1e-9, .999999999, 1e-9, .999999999, .999999999, 1e-9, .999999999, 1e-9, 1e-9);
    };
}
