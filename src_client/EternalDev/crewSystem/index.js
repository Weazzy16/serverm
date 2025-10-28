import "./alertMarker";
import "./playerBlips";
const callRemote = mp.events.callRemote;

export default new class CrewSystem {
    constructor() {
        this.register("update-group", this.updateGroupData.bind(this));
        this.register("callServer", this.callServer.bind(this));
    }

    register(eventName, callback) { mp.events.add(`e-dev.crew-system.${eventName}`, callback); }
    callServer(eventName, ...args) { callRemote(`e-dev.crew-system.${eventName}`, ...args); }

    updateGroupData(data) {
        mp.gui.emmit(`window.crewStore.setCrewData(${data})`);
    }
}