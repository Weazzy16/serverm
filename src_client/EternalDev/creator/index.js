import { selectedSlot } from "../../player/auth";
import { majesticCef } from "../intro";
import CREATOR_CONFIG from "../spawnSelector/data/config";

const call = mp.events.call;
const callRemote = mp.events.callRemote;

const localPlayer = mp.players.local;

export default new class CharacterCreator {
    camera = null
    customization = {}

    constructor() {
        const EVENTS = {
            "open": this.open.bind(this),
            "close": this.close.bind(this),
            "update": this.update.bind(this),
            "ready": this.ready.bind(this),
            "cancel": this.cancel.bind(this),
            "setStep": this.setStep.bind(this)
        }

        for (const [eventName, callback] of Object.entries(EVENTS)) {
            gm.events.add(`client.creator.${eventName}`, callback);
        }

        this.camera = null;

        this.spawnPosition = {
            x: -760.47,
            y: 314.92,
            z: 217.05,
            r: 48,
            offset: new mp.Vector3(0, 3, 0.25)
        };

        /**
         * 0 - male | 1 - female 
         * */ 
        this.currentGender = 0;

        this.animLib = {
            loopMale: 'mp_character_creation@customise@male_a',
            loopFemale: 'mp_character_creation@customise@female_a'
        };

        this.isFirst = true;
    }

    async open(isFirst, name = "", surname = "") {
        mp.game.cam.doScreenFadeOut(300);
        await global.wait(300);

        this.customization = { ...CREATOR_CONFIG.defaultCustomization };

        await Promise.all([
            global.requestAnimDict(this.animLib.loopMale),
            global.requestAnimDict(this.animLib.loopFemale),
        ]);

        localPlayer.model = mp.game.joaat('mp_m_freemode_01');
        localPlayer.position = new mp.Vector3(this.spawnPosition.x, this.spawnPosition.y, this.spawnPosition.z);
        localPlayer.setHeading(this.spawnPosition.r);
        localPlayer.freezePosition(true);
        localPlayer.resetAlpha();
        localPlayer.setVisible(true, false);
        localPlayer.clearTasksImmediately();
        localPlayer.setStealthMovement(false, '0');

        this.applyCustomization(localPlayer, this.customization);
        this.removeClothes(this.currentGender);

        this.createCamera();

        this.isFirst = isFirst;
        this.opened = true;

        global.menuOpen();
        mp.gui.emmit('window.router.close()');

        majesticCef.execute(`creator.open('${name}', '${surname}')`);
        mp.game.cam.doScreenFadeIn(300);
        
        await global.wait(700);
        this.setStep("first");
    }

    createCamera() {
        const camPosition = mp.game.object.getObjectOffsetFromCoords(this.spawnPosition.x, this.spawnPosition.y, this.spawnPosition.z, this.spawnPosition.r, this.spawnPosition.offset.x, this.spawnPosition.offset.y, this.spawnPosition.offset.z);
        this.camera = mp.cameras.new('default', new mp.Vector3(camPosition.x, camPosition.y, camPosition.z), new mp.Vector3(0, 0, 0), 40);
        
        this.camera._pointAtPos = new mp.Vector3(this.spawnPosition.x, this.spawnPosition.y, this.spawnPosition.z);
        this.camera.pointAtCoord(this.spawnPosition.x, this.spawnPosition.y, this.spawnPosition.z + 0.25);
        this.camera.setActive(true);
    
        mp.game.cam.renderScriptCams(true, false, 0, false, false)
    }

    close() {
        if (this.camera) {
            if (this.camera.hasOwnProperty("interpCamera")) {
                this.camera.interpCamera.setActive(false);
                this.camera.interpCamera.destroy();
                this.camera.interpCamera = null;
            }

            this.camera.destroy();
            this.camera = null;
        }

        mp.game.cam.renderScriptCams(false, true, 0, true, true);
        this.opened = false;

        majesticCef.execute(`creator.close()`);
    }

    async cancel() {
        if (!this.isFirst) {
            call("notify", 2, 9, "Вы не можете отменить создание персонажа", 3000);
            return;
        }

        mp.game.cam.doScreenFadeOut(300);
        await global.wait(300);
    
        this.close();
        
        call("e-dev.intro.set_position");
        callRemote("server.accounts.load_character");
    }

    setStep(step) {
        if (this.camera == null || !this.opened) return;
        let bone = 0, zUp = 0.25, y = 3, fov = 40;
        switch (step) {
            case 'first':
            case 'second':
            case 'third':
                bone = 31086, zUp = 0.6, y = 1.5, fov = 30;
                break;
            case 'fourth':
                bone = 31086, zUp = 0.5, y = 1.5, fov = 40;
                break;
            case 'fifth':
                bone = 0, zUp = -0.1, y = 3, fov = 40;
                break;
        }

        this.pointCamAtWithOffset(0, 0, zUp, 0, y, 1, 500, fov, bone);
    }

    pointCamAtWithOffset(x, y, z, ox, oy, oz, transition, fov, bone) {
        const offset = new mp.Vector3(ox, oy, oz);
        const camPosition = mp.game.object.getObjectOffsetFromCoords(this.spawnPosition.x, this.spawnPosition.y, this.spawnPosition.z, this.spawnPosition.r, offset.x, offset.y, offset.z); 
        
        if (this.camera.hasOwnProperty("interpCamera")) {
            this.camera.interpCamera.setActive(false);
            this.camera.interpCamera.destroy();
            this.camera.interpCamera = null;
        }
        
        const interpCamera = mp.cameras.new('default', this.camera.getCoord(), this.camera.getRot(2), this.camera.getFov());
        
        let activeCameraPointAt = new mp.Vector3();
        if (this.camera._pointAtPos != undefined)
            activeCameraPointAt = this.camera._pointAtPos;

        interpCamera.pointAtCoord(activeCameraPointAt.x, activeCameraPointAt.y, activeCameraPointAt.z);

        this.camera.setCoord(camPosition.x + x, camPosition.y + y, camPosition.z);
        this.camera.pointAtCoord(this.spawnPosition.x + x, this.spawnPosition.y + y, this.spawnPosition.z + z);
        this.camera.setFov(fov);

        this.camera.interpCamera = interpCamera;
        this.camera._pointAtPos = new mp.Vector3(this.spawnPosition.x + x, this.spawnPosition.y + y, this.spawnPosition.z + z);

        this.camera.setActiveWithInterp(interpCamera.handle, transition, transition * .5, transition * .5);
        mp.game.cam.renderScriptCams(true, false, 0, false, false);
    }

    applyCustomization(entity, data) {
        try {
            entity.setEyeColor(data.eyeColor);
            entity.setHeadBlendData(data.mother, data.father, 0, data.mother, data.father, 0, parseInt(data.similarity), parseInt(data.skinSimilarity), 0, false);
            data.faceFeatureParams.forEach((value, index) => entity.setFaceFeature(index, parseInt(value)));

            Object.keys(data.overlay).forEach(overlayId => {

                const overlayData = data.overlay[overlayId];
                entity.setHeadOverlay(parseInt(overlayId), 
                    parseInt(overlayData.index), 
                    parseInt(overlayData.opacity), 
                    parseInt(overlayData.color1), 
                    parseInt(overlayData.color2)
                );

                let overlayColor = 0;
                switch (parseInt(overlayId)) {
                    case 1:
                    case 2:
                    case 10:
                        overlayColor = 1;
                        break;
                    case 5:
                    case 8:
                        overlayColor = 2;
                }

                entity.setHeadOverlayColor(parseInt(overlayId), 
                    overlayColor, 
                    data.overlay[overlayId].color1 ? data.overlay[overlayId].color1 : 1, 
                    data.overlay[overlayId].color2 ? data.overlay[overlayId].color2 : 1);

            });

            entity.setComponentVariation(2, data.hair, 0, 2);
            entity.setHairColor(data.hairColor[0], data.hairColor[1]);
        }
        catch(e) { mp.console.logError('applyCustomization: ' + `${e}`); }
    }

    update(type, value) {
        if (typeof value === 'string')
            value = JSON.parse(value);
        
        switch (type) {
            case 'sex':
                localPlayer.model = value ? mp.game.joaat('mp_f_freemode_01') : mp.game.joaat('mp_m_freemode_01'); 
                this.currentGender = value; 

                this.removeClothes(this.currentGender);
                break;
            case 'mother':
            case 'father':
            case 'similarity':
            case 'skinSimilarity':
                this.customization[type] = value, 
                localPlayer.setHeadBlendData(this.customization.mother, this.customization.father, 0, this.customization.mother, this.customization.father, 0, this.customization.similarity, this.customization.skinSimilarity, 0, false);
                break;
            case 'hair':
                this.customization.hair = value - 1; 
                
                localPlayer.setComponentVariation(2, this.customization.hair, 0, 2);
                localPlayer.setHairColor(this.customization.hairColor[0], this.customization.hairColor[1]);
                break;
            case 'hairMainColor':
                this.customization.hairColor[0] = value; 
                localPlayer.setHairColor(this.customization.hairColor[0], this.customization.hairColor[1]);
                break;
            case 'hairAdditionalColor':
                this.customization.hairColor[1] = value;
                localPlayer.setHairColor(this.customization.hairColor[0], this.customization.hairColor[1]);
                break;
            case 'eyeColor':
                this.customization.eyeColor = value - 1; 
                localPlayer.setEyeColor(value);
                break;
            case 'blemishes': 
                {
                    const overlayData = this.customization.overlay[0];
                    overlayData.index = value;
                    localPlayer.setHeadOverlay(0, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'blemishesOpacity': 
                {
                    const overlayData = this.customization.overlay[0];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(0, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'beards': 
                {
                    const overlayData = this.customization.overlay[1];
                    overlayData.index = value - 1;

                    localPlayer.setHeadOverlay(1, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'beardOpacity': 
                {
                    const overlayData = this.customization.overlay[1];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(1, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'beardColor': 
                {
                    const overlayData = this.customization.overlay[1];
                    overlayData.color1 = value;
                    localPlayer.setHeadOverlay(1, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'beardColor2': 
                {
                    const overlayData = this.customization.overlay[1];
                    overlayData.color2 = value;
                    localPlayer.setHeadOverlay(1, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'eyebrows': 
                {
                    const overlayData = this.customization.overlay[2];
                    overlayData.index = value - 1;
                    localPlayer.setHeadOverlay(2, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'browOpacity': 
                {
                    const overlayData = this.customization.overlay[2];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(2, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'browColor': 
                {
                    const overlayData = this.customization.overlay[2];
                    overlayData.color1 = value;
                    localPlayer.setHeadOverlay(2, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'browColor2': {
                    const overlayData = this.customization.overlay[2];
                    overlayData.color2 = value;
                    localPlayer.setHeadOverlay(2, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'ageing': 
                {
                    const overlayData = this.customization.overlay[3];
                    overlayData.index = value;
                    localPlayer.setHeadOverlay(3, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'ageingOpacity': 
                {
                    const overlayData = this.customization.overlay[3];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(3, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'complexion': 
                {
                    const overlayData = this.customization.overlay[4];
                    overlayData.index = value;
                    localPlayer.setHeadOverlay(4, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'complexionOpacity': 
                {
                    const overlayData = this.customization.overlay[4];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(4, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'sunDamage': 
                {
                    const overlayData = this.customization.overlay[7];
                    overlayData.index = value;
                    localPlayer.setHeadOverlay(7, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'sunDamageOpacity': 
                {
                    const overlayData = this.customization.overlay[7];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(7, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'molesAndFreckles': 
                {
                    const overlayData = this.customization.overlay[9];
                    overlayData.index = value;
                    localPlayer.setHeadOverlay(9, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'molesAndFrecklesOpacity': 
                {
                    const overlayData = this.customization.overlay[9];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(9, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'chestHair': 
                {
                    const overlayData = this.customization.overlay[10];
                    overlayData.index = value - 1;
                    localPlayer.setHeadOverlay(10, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'chestHairOpacity': 
                {
                    const overlayData = this.customization.overlay[10];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(10, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'chestHairColor': 
                {
                    const overlayData = this.customization.overlay[10];
                    overlayData.color1 = value;
                    localPlayer.setHeadOverlay(10, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'chestHairColor2': 
                {
                    const overlayData = this.customization.overlay[10];
                    overlayData.color2 = value;
                    localPlayer.setHeadOverlay(10, overlayData.index, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'bodyBlemishes': 
                {
                    const overlayData = this.customization.overlay[11];
                    overlayData.index = value;
                    localPlayer.setHeadOverlay(11, value, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'bodyBlemishesOpacity': 
                {
                    const overlayData = this.customization.overlay[11];
                    overlayData.opacity = value;
                    localPlayer.setHeadOverlay(11, value, overlayData.opacity, overlayData.color1, overlayData.color2);
                    break;
                }
            case 'noseSize': 
                {
                    this.customization.faceFeatureParams[0] = value.x;
                    this.customization.faceFeatureParams[1] = value.y;
                    localPlayer.setFaceFeature(0, this.customization.faceFeatureParams[0]);
                    localPlayer.setFaceFeature(1, this.customization.faceFeatureParams[1]);
                    break;
                }
            case 'noseLength': 
                {
                    this.customization.faceFeatureParams[2] = value;
                    localPlayer.setFaceFeature(2, this.customization.faceFeatureParams[2]);
                    break;
                }
            case 'noseBridge': 
                {
                    this.customization.faceFeatureParams[3] = value;
                    localPlayer.setFaceFeature(3, this.customization.faceFeatureParams[3]);
                    break;
                }
            case 'noseTip': 
                {
                    this.customization.faceFeatureParams[5] = value.x;
                    this.customization.faceFeatureParams[4] = value.y;
                    localPlayer.setFaceFeature(5, this.customization.faceFeatureParams[5]);
                    localPlayer.setFaceFeature(4, this.customization.faceFeatureParams[4]);
                    break;
                }
            case 'browSize': 
                {
                    this.customization.faceFeatureParams[7] = value.x;
                    this.customization.faceFeatureParams[6] = value.y;
                    localPlayer.setFaceFeature(7, this.customization.faceFeatureParams[7]);
                    localPlayer.setFaceFeature(6, this.customization.faceFeatureParams[6]);
                    break;
                }
            case 'cheekbonesSize': 
                {
                    this.customization.faceFeatureParams[9] = value.x;
                    this.customization.faceFeatureParams[8] = value.y;
                    localPlayer.setFaceFeature(9, this.customization.faceFeatureParams[9]);
                    localPlayer.setFaceFeature(8, this.customization.faceFeatureParams[8]);
                    break;
                }
            case 'cheekWidth': 
                {
                    this.customization.faceFeatureParams[10] = value;
                    localPlayer.setFaceFeature(10, this.customization.faceFeatureParams[10]);
                    break;
                }
            case 'eyes': 
                {
                    this.customization.faceFeatureParams[11] = value - 1;
                    localPlayer.setFaceFeature(11, this.customization.faceFeatureParams[11]);
                    break;
                }
            case 'lips': 
                {
                    this.customization.faceFeatureParams[12] = value;
                    localPlayer.setFaceFeature(12, this.customization.faceFeatureParams[12]);
                    break;
                }
            case 'jawSize':     
                {
                    this.customization.faceFeatureParams[13] = value.x;
                    this.customization.faceFeatureParams[14] = value.y;
                    localPlayer.setFaceFeature(13, this.customization.faceFeatureParams[13]);
                    localPlayer.setFaceFeature(14, this.customization.faceFeatureParams[14]);
                    break;
                }
            case 'chinLength': 
                {
                    this.customization.faceFeatureParams[15] = value;
                    localPlayer.setFaceFeature(15, this.customization.faceFeatureParams[15]);
                    break;
                }
            case 'chinPosition': 
                {
                    this.customization.faceFeatureParams[16] = value;
                    localPlayer.setFaceFeature(16, this.customization.faceFeatureParams[16]);
                    break;
                }
            case 'chinWidth':   
                {
                    this.customization.faceFeatureParams[17] = value;
                    localPlayer.setFaceFeature(17, this.customization.faceFeatureParams[17]);
                    break;
                }
            case 'chinForm': 
                {
                    this.customization.faceFeatureParams[18] = value;
                    localPlayer.setFaceFeature(18, this.customization.faceFeatureParams[18]);
                    break;
                }
            case 'neckWidth': 
                {
                    this.customization.faceFeatureParams[19] = value;
                    localPlayer.setFaceFeature(19, this.customization.faceFeatureParams[19]);
                    break;
                }

            case 'tops':
            case 'legs':
            case 'shoes': {
                    const clothesData = CREATOR_CONFIG.clothesConfig[this.currentGender][type][value - 1];
                    if (!this.customization.clothes || !this.customization.clothes[type]) {
                        break;
                    }

                    this.customization.clothes[type] = {
                        component: clothesData.component,
                        drawable: clothesData.drawable,
                        texture: clothesData.texture
                    };

                    localPlayer.setComponentVariation(clothesData.component, clothesData.drawable, clothesData.texture, 0);
                    
                    let animationData = {
                        dict: this.currentGender ? "mp_character_creation@customise@female_a" : "mp_character_creation@customise@male_a",
                        name: CREATOR_CONFIG.clothesAnimations[global.getRandomInt(0, CREATOR_CONFIG.clothesAnimations.length - 1)]
                    }

                    mp.game.task.clearPedSecondaryTask(localPlayer.handle)
                    global.requestAnimDict(animationData.dict).then(() => {
                        localPlayer.taskPlayAnim(animationData.dict, animationData.name, 4, 4, -1, 0, 0, false, false, false);
                    });
                    break;
                }
            }
    }

    ready(name, surname) {
        if (global.checkName(name) || !global.checkName2(name) || name.length > 25 || name.length <= 2) {
            call('notify', 1, 9, translateText("Правильный формат имени: 3-25 символов и первая буква имени заглавная"), 3000);
            return;
        }

        if (global.checkName(surname) || !global.checkName2(surname) || surname.length > 25 || surname.length <= 2) {
            call('notify', 1, 9, translateText("Правильный формат фамилии: 3-25 символов и первая буква фамилии заглавная"), 3000);
            return;
        }

        callRemote("server.creator.ready", selectedSlot, name, surname, 
            this.currentGender, 
            this.customization.mother,
            this.customization.father,
            this.customization.similarity,
            this.customization.skinSimilarity,
            this.customization.hair,
            this.customization.hairColor[0],
            this.customization.hairColor[1],
            this.customization.eyeColor,
            JSON.stringify(this.customization.overlay),
            JSON.stringify(this.customization.faceFeatureParams),
            JSON.stringify(this.customization.clothes)
        );
    }

    removeClothes(gender) {
        const emptySlots = this.getEmptyClothesSlots(gender);
        emptySlots.forEach(item => {
            if (item.isProp) {
                localPlayer.clearProp(item.component);
            }
            else {
                localPlayer.setComponentVariation(item.component, item.drawable, item.texture, 0);
            }
        });
    }

    getEmptyClothesSlots(gender) {
        return [
            {
                'isProp': 0,
                'component': 1,
                'drawable': 0,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 3,
                'drawable': 15,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 4,
                'drawable': 0 === gender ? 21 : 15,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 5,
                'drawable': 0,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 6,
                'drawable': 0 === gender ? 34 : 35,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 7,
                'drawable': 0,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 8,
                'drawable': 0 === gender ? 15 : 10,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 9,
                'drawable': 0,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 10,
                'drawable': 0,
                'texture': 0
            },
            {
                'isProp': 0,
                'component': 11,
                'drawable': 15,
                'texture': 0
            },
            {
                'isProp': 1,
                'component': 0,
                'drawable': -1,
                'texture': -1
            },
            {
                'isProp': 1,
                'component': 1,
                'drawable': -1,
                'texture': -1
            },
            {
                'isProp': 1,
                'component': 2,
                'drawable': -1,
                'texture': -1
            },
            {
                'isProp': 1,
                'component': 6,
                'drawable': -1,
                'texture': -1
            },
            {
                'isProp': 1,
                'component': 7,
                'drawable': -1,
                'texture': -1
            }
        ];
    }
}