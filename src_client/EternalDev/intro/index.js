import CamerasManager from './modules/camerasManager';

import SCENARIO_CLUB from './data/club';
import SCENARIO_XMAS from './data/xmas';
import SCENARIO_XMAS2 from './data/xmas2';
import SCENARIO_STREETRACE from './data/streetRace';
import SCENARIO_BEACH from './data/beach';

const call = mp.events.call;
const callRemote = mp.events.callRemote;
const browsers = mp.browsers;
export const currentScenario = SCENARIO_XMAS;
export const majesticCef = browsers.new("http://package/e-dev/majestic/index.html");

export let mainCamera;
let timeout;

let pedsPool = [];
let vehiclesPool = [];

async function startScenario() {
    mp.game.cam.doScreenFadeOut(0);
    mp.game.streaming.clearFocus();

    await global.wait(1500);
    
    global.localplayer.position = currentScenario.playerPosition;
    global.localplayer.freezePosition(true);

    mp.game.streaming.requestIpl(currentScenario.iplName);
    if (currentScenario.deleteObjects)
        currentScenario.deleteObjects.forEach(data => {
            mp.game.entity.createModelHide(data.x, data.y, data.z, data.radius, data.model, true);
        });

    createPeds();
    createVehicles();

    while (pedsPool.filter(x => x.isScenReady).length != currentScenario.peds.length 
        || vehiclesPool.filter(x => x.isScenReady).length != currentScenario.vehicles.length 
        ) {
        await global.wait(100);
    }

    currentScenario.isPlaying = true;

    global.FadeScreen(false, 500);
    mp.game.cam.doScreenFadeIn(500);

    mp.gui.cursor.show(false, false);

    createCamera(0);
    majesticCef.execute(`
        intro.opened = true; 
        playMusic();
    `);
}

let isIntroSkipped = false;
function onTick() {
    mp.game.cam.setUseHiDof();

    if (!currentScenario.isPlaying || isIntroSkipped)
        return;

    if (mp.game.controls.isDisabledControlJustReleased(0, 24) && !mp.gui.cursor.visible) {
        majesticCef.execute(`
            intro.opened = false; 
        `);

        callRemote("server.startGame");
        isIntroSkipped = true;
    }
}

function stopScenario() {
    currentScenario.isPlaying = false;

    destroyCamera();
    destroyPeds();
    destroyVehicles();
	
	majesticCef.execute(`stopMusic()`);

    if (timeout != null)
        clearTimeout(timeout);

    timeout = null;
}

function createCamera(index) {
    let intro = currentScenario;
    if (!intro.isPlaying) return;

    var cameraSettings = intro.cameras[index];
    if (typeof(cameraSettings) == 'undefined') {
        createCamera(0);
        return;
    }

    let camType = typeof(cameraSettings.to) == 'undefined' ? "default" : "interpolate";
    destroyCamera();

    mainCamera = CamerasManager.createCamera("intro", "default", cameraSettings.from.pos, new mp.Vector3(0, 0, 0), cameraSettings.from.fov);
    CamerasManager.setDof(mainCamera, cameraSettings.from.dof);
    CamerasManager.pointAtCoord(mainCamera, cameraSettings.from.pointAt);

    CamerasManager.setActiveCamera(mainCamera, true);

    if (camType == "interpolate") {
        CamerasManager.setActiveCameraWithInterpPoint(mainCamera, cameraSettings.to.pos, cameraSettings.to.pointAt, cameraSettings.duration, 0, 0, 
            cameraSettings.to.fov, cameraSettings.to.dof);
    }

    timeout = setTimeout(() => {
        createCamera(index + 1);
    }, camType == "interpolate" ? cameraSettings.duration : intro.defaultSceneDuration);
}

function destroyCamera() {
    if (mainCamera != null) {
        mainCamera.destroy();
        mainCamera = null;
        mp.game.cam.renderScriptCams(false, true, 0, true, true);
    }
}

function createPeds() {
    currentScenario.peds.forEach(data => {
        const entity = mp.peds.new(mp.game.joaat(data.model), data.position, data.heading || 0, global.localplayer.dimension);
        if (data.rotation)
            entity.setRotation(data.rotation.x, data.rotation.y, data.rotation.z, 2, true);

        entity.scenData = data;
        entity.isScenReady = false;

        pedsPool.push(entity);
        onStream(entity);
    });
}

function destroyPeds() {
    pedsPool.forEach(entity => entity.destroy());
    pedsPool = [];
}

function createVehicles() {
    currentScenario.vehicles.forEach(data => {
        var veh = mp.vehicles.new(mp.game.joaat(data.model), data.position, {
            numberPlate: data.numberPlate,
            heading: data.rotation,
            dimension: global.localplayer.dimension,
        });

        veh.scenData = data;
        veh.isScenReady = false;

        if (typeof(data.freezePosition) === 'boolean')
            veh.freezePosition(data.freezePosition);
        
        vehiclesPool.push(veh);
        onStream(veh);
    });
}

function destroyVehicles() {
    vehiclesPool.forEach(entity => entity.destroy());
    vehiclesPool = [];
}

function onStream(entity) {
    if (!entity || entity.handle == 0 || !entity.scenData) 
        return;
    
    let data = entity.scenData;
    switch(entity.type) {
        case "ped":
            if (typeof(data.animation) !== 'undefined') {
                global.requestAnimDict(data.animation.animDict).then(async () => {
                    if (!entity || !entity.handle || !mp.peds.exists(entity)) return;
                    
                    while (!entity.isPlayingAnim(data.animation.animDict, data.animation.animName, 3)) {
                        entity.taskPlayAnim(data.animation.animDict, data.animation.animName, 8, 1.0, -1, 1, 0, false, false, false); 
                        await global.wait(1);
                    }
                });
            }

            if (typeof(data.scenario) !== 'undefined')
                entity.taskStartScenarioInPlace(data.scenario, 0, false);

            if (typeof(data.freezePosition) === 'boolean')
                entity.freezePosition(data.freezePosition);
        break;
        case "vehicle":
            if (typeof(data.colors) !== 'undefined')
                entity.setColours(data.colors[0], data.colors[1]);
            
            if (typeof(data.freezePosition) === 'boolean')
                entity.freezePosition(data.freezePosition);

            if (typeof(data.engine) !== 'undefined')
                entity.setEngineOn(data.engine, true, true);

            if (typeof(data.doors) !== 'undefined') {
                for (const [id, state] of Object.entries(data.doors))
                    state ? entity.setDoorOpen(parseInt(id), false, false) : entity.setDoorShut(parseInt(id), false);
            }
			
			// if (typeof data.tuning !== 'undefined') {
			// 	for (const [id, value] of Object.entries(data.tuning))
			// 		entity.setMod(id, value);
			// }
        break;
    }

    entity.isScenReady = true;
}

function setPosition() {
    global.localplayer.position = currentScenario.playerPosition;
    global.localplayer.freezePosition(true);
}

function switchScene(scene, duration) {
    const data = currentScenario.scenes[scene];
    if (data == null)
        return;

    if (timeout != null) {
        clearTimeout(timeout);
        timeout = null;
    }

    CamerasManager.setActiveCameraWithInterpPoint(mainCamera, data.camPos, data.camLookAt, duration, 0, 0, data.fov, {
        nearDof: 0,
        farDof: 3000,
        strength: 1,
        shallowMode: true
    });
}

function selectCharacter(uuid, slot) {
    if (new Date().getTime() - global.lastCheck < 700 || slot == -1) 
        return;

    const charsData = currentScenario.characters;
    const scenario = uuid < 0 ? charsData : charsData[slot];

    if (scenario == null) 
        return;

    CamerasManager.setActiveCameraWithInterpPoint(mainCamera, scenario.camPos, scenario.camLookAt, 700, 0, 0, scenario.fov, scenario.dof);
}

gm.events.add("e-dev.intro.start", startScenario);
gm.events.add("e-dev.intro.stop", stopScenario);
gm.events.add("e-dev.intro.set_position", setPosition);
gm.events.add("e-dev.intro.switch_scene", switchScene);
gm.events.add("client.characterSelector.select", selectCharacter);
mp.events.add("entityStreamIn", onStream);
mp.events.add("render", onTick);