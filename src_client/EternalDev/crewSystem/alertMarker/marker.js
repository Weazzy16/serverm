import { ALERT_MARKER_DURATION } from "../config";
import { drawSprite } from "./utils";

export class AlertMarker {
    settings;
    visible;
    animationRotationStateOne;
    animScaleStateTwo;
    spriteVisible;
    heading;
    baseScale;
    outlineAlpha;
    alphaDirectionUp;
    timetoDestroy;
    endTime;

    constructor(settings = {
        type: "sprite",
        text: "Marker",
        lib: "commonmenu",
        sprite: "mp_alerttriangle",
        colorSprite: [ 157, 28, 47, 130 ],
        scaleSprite: 1,
        spriteHeading: 0,
        worldPosition: new mp.Vector3(0, 0, 0),
        color: [ 157, 28, 47, 190 ],
        colorOutline: [ 0, 255, 255, 255 ],
        outlineScale: .505,
        maxScale: .5,
        minScale: .2,
        distToHide: 1
    }, time = ALERT_MARKER_DURATION) {
        this.settings = settings; 
        this.visible = true; 
        this.animationRotationStateOne = true; 
        this.animScaleStateTwo = false;
        this.spriteVisible = false; 
        this.heading = 0; 
        this.baseScale = 1; 
        this.outlineAlpha = 0; 
        this.alphaDirectionUp = true; 
        this.timetoDestroy = time; 

        mp.game.graphics.requestStreamedTextureDict("helicopterhud", true); 
        this.endTime = Date.now() + time; 
        
        if ("sprite" === settings.type) 
            mp.game.graphics.requestStreamedTextureDict(settings.lib, true);
    }

    toggle(state) {
        this.visible = state;
        mp.game.audio.playSoundFrontend(-1, "Start_Squelch", "CB_RADIO_SFX", true);
    }

    onTick() {
        if (this.visible) {
            const realDistance = Math.ceil(global.vdist2(mp.players.local.position, this.settings.worldPosition));
            let distance = 1;
            let time = (this.endTime - Date.now()) / this.timetoDestroy;
            if(time < 0) 
                time = 0;
            
            const alpha = global.getLerp(0, 255, time);
            distance = realDistance > 30 ? 1 : realDistance / 30;
            
            const size = global.getLerp(.5, 1, distance);

            if ("sprite" === this.settings.type) {
                const scaleSprite = this.settings.scaleSprite;
                drawSprite(
                    this.settings.lib, 
                    this.settings.sprite, 
                    { 
                        x: this.settings.worldPosition.x, 
                        y: this.settings.worldPosition.y, 
                        z: this.settings.worldPosition.z 
                    },
                    [ 
                        size * scaleSprite * .5, 
                        size * scaleSprite * .5
                    ],
                    0, 
                    { 
                        r: Number(this.settings.colorSprite[0]), 
                        g: Number(this.settings.colorSprite[1]), 
                        b: Number(this.settings.colorSprite[2]),
                        a: Number(Math.round(alpha))
                    }
                );
            }
        }
    }
}