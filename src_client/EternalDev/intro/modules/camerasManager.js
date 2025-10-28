const gameplayCam = mp.cameras.new("gameplay");

const CamerasManagerInfo = {
    gameplayCamera: null,
    activeCamera: null,
    interpCamera: null,
    interpActive: false,
    _events: new Map(),
    cameras: new Map([
        ['testCamera', mp.cameras.new('default', new mp.Vector3(), new mp.Vector3(), 50.0)],
    ])
};

mp.events.add('render', () => {
    if (CamerasManagerInfo.interpCamera && CamerasManager.doesExist(CamerasManagerInfo.interpCamera) && !CamerasManagerInfo.activeCamera.isInterpolating()) {

        CamerasManager.fireEvent('stopInterp', CamerasManagerInfo.activeCamera);

        CamerasManagerInfo.interpCamera.setActive(false);
        CamerasManagerInfo.interpCamera.destroy();
        CamerasManagerInfo.interpCamera = null;
    }
});

const cameraSerialize = (camera) => {
    camera.setActiveCamera = (toggle) => {
        CamerasManager.setActiveCamera(camera, toggle);
    };

    camera.setActiveCameraWithInterp = (position, rotation, duration, easeLocation, easeRotation) => {
        CamerasManager.setActiveCameraWithInterp(camera, position, rotation, duration, easeLocation, easeRotation);
    };
};

class CamerasManager {

    static on(eventName, eventFunction) {
        if (CamerasManagerInfo._events.has(eventName)) {
            const event = CamerasManagerInfo._events.get(eventName);

            if (!event.has(eventFunction)) {
                event.add(eventFunction);
            }
        } else {
            CamerasManagerInfo._events.set(eventName, new Set([eventFunction]));
        }
    }

    static fireEvent(eventName, ...args) {
        if (CamerasManagerInfo._events.has(eventName)) {
            const event = CamerasManagerInfo._events.get(eventName);

            event.forEach(eventFunction => {
                eventFunction(...args);
            });
        }
    }

    static getCamera(name) {

        const camera = CamerasManagerInfo.cameras.get(name);

        if (typeof camera.setActiveCamera !== 'function') {
            cameraSerialize(camera);
        }

        return camera;
    }

    static setCamera(name, camera) {
        CamerasManagerInfo.cameras.set(name, camera);
    }

    static hasCamera(name) {
        return CamerasManagerInfo.cameras.has(name);
    }

    static destroyCamera(camera) {
        if (this.doesExist(camera)) {
            if (camera === this.activeCamera) {
                this.activeCamera.setActive(false);
            }
            camera.destroy();
        }
    }

    static createCamera(name, type, position, rotation, fov) {
        const cam = mp.cameras.new(type, position, rotation, fov);
        cameraSerialize(cam);
        CamerasManagerInfo.cameras.set(name, cam);
        return cam;
    }

    static setActiveCamera(activeCamera, toggle) {
        if (!toggle) {
            if (this.doesExist(CamerasManagerInfo.activeCamera)) {
                CamerasManagerInfo.activeCamera = null;
                activeCamera.setActive(false);
                mp.game.cam.renderScriptCams(false, false, 0, false, false);
            }

            if (this.doesExist(CamerasManagerInfo.interpCamera)) {
                CamerasManagerInfo.interpCamera.setActive(false);
                CamerasManagerInfo.interpCamera.destroy();
                CamerasManagerInfo.interpCamera = null;
            }

        } else {
            if (this.doesExist(CamerasManagerInfo.activeCamera)) {
                CamerasManagerInfo.activeCamera.setActive(false);
            }
            CamerasManagerInfo.activeCamera = activeCamera;
            activeCamera.setActive(true);
            mp.game.cam.renderScriptCams(true, false, 0, false, false);
        }
    }

    static setDof(camera, dofSettings) {
        if (dofSettings != null) {
			if (dofSettings.shallowMode != null) 
                camera.setUseShallowDofMode(dofSettings.shallowMode);
			if (dofSettings.nearDof != null) 
                camera.setNearDof(dofSettings.nearDof);
			if (dofSettings.farDof != null) 
                camera.setFarDof(dofSettings.farDof);
			if (dofSettings.strength != null) 
                camera.setDofStrength(dofSettings.strength);
		}
    }

    static setActiveCameraWithInterpPoint(activeCamera, position, pointAt, duration, easeLocation, easeRotation, fov = null, dofSettings = null) {
        if (this.doesExist(CamerasManagerInfo.activeCamera)) {
            CamerasManagerInfo.activeCamera.setActive(false);
        }

        if (this.doesExist(CamerasManagerInfo.interpCamera)) {
            CamerasManager.fireEvent('stopInterp', CamerasManagerInfo.interpCamera);

            CamerasManagerInfo.interpCamera.setActive(false);
            CamerasManagerInfo.interpCamera.destroy();
            CamerasManagerInfo.interpCamera = null;
        }

        fov = fov == null ? activeCamera.getFov() : fov;
        const interpCamera = mp.cameras.new('default', activeCamera.getCoord(), activeCamera.getRot(2), activeCamera.getFov());

        let activeCameraPointAt = new mp.Vector3();
        if (activeCamera.__pointingPosition != null)
            activeCameraPointAt = activeCamera.__pointingPosition;

		interpCamera.pointAtCoord(activeCameraPointAt.x, activeCameraPointAt.y, activeCameraPointAt.z);

        activeCamera.setCoord(position.x, position.y, position.z);
        activeCamera.pointAtCoord(pointAt.x, pointAt.y, pointAt.z);
        activeCamera.__pointingPosition = pointAt;

        CamerasManager.setDof(activeCamera, dofSettings);

        activeCamera.setFov(fov);

        CamerasManagerInfo.activeCamera = activeCamera;
        CamerasManagerInfo.interpCamera = interpCamera;

        activeCamera.setActiveWithInterp(interpCamera.handle, duration, easeLocation, easeRotation);
        mp.game.cam.renderScriptCams(true, false, 0, false, false);

        CamerasManager.fireEvent('startInterp', CamerasManagerInfo.interpCamera);
    }

    static setActiveCameraWithInterpRotation(activeCamera, position, rotation, duration, easeLocation, easeRotation, fov = null, dofSettings = null) {
        if (this.doesExist(CamerasManagerInfo.activeCamera)) {
            CamerasManagerInfo.activeCamera.setActive(false);
        }

        if (this.doesExist(CamerasManagerInfo.interpCamera)) {
            CamerasManager.fireEvent('stopInterp', CamerasManagerInfo.interpCamera);

            CamerasManagerInfo.interpCamera.setActive(false);
            CamerasManagerInfo.interpCamera.destroy();
            CamerasManagerInfo.interpCamera = null;
        }

        const interpCamera = mp.cameras.new('default', activeCamera.getCoord(), activeCamera.getRot(2), activeCamera.getFov());
        activeCamera.setCoord(position.x, position.y, position.z);
        activeCamera.setRot(rotation.x, rotation.y, rotation.z, 2);
        activeCamera.stopPointing();

        CamerasManager.setDof(activeCamera, dofSettings);
        activeCamera.setFov(fov);

        CamerasManagerInfo.activeCamera = activeCamera;
        CamerasManagerInfo.interpCamera = interpCamera;
        activeCamera.setActiveWithInterp(interpCamera.handle, duration, easeLocation, easeRotation);
        mp.game.cam.renderScriptCams(true, false, 0, false, false);

        CamerasManager.fireEvent('startInterp', CamerasManagerInfo.interpCamera);
    }

    static pointAtCoord(camera, position) {
        if (this.doesExist(camera)) {
            camera.pointAtCoord(position.x, position.y, position.z);
            camera.__pointingPosition = position;
        }
    }

    static doesExist(camera) {
        return mp.cameras.exists(camera) && camera.doesExist();
    }

    static get activeCamera() {
        return CamerasManagerInfo.activeCamera;
    }

    static get gameplayCam() {
        if (!CamerasManagerInfo.gameplayCamera) {
            CamerasManagerInfo.gameplayCamera = gameplayCam;
        }
        return CamerasManagerInfo.gameplayCamera;
    }
}

export default CamerasManager;