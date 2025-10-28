const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const customScenarioMap = new Map();
class CustomScenario {
    constructor(data) {
        this.name = data;
        customScenarioMap.set(data, this);
    }
    isActive(player) {
        return player.cSen === this.name;
    }
    onStart(player) {}
    onStartForNew(player) {}
    onEnd(player) {}
}

global.CustomScenario = CustomScenario;

gm.events.add("playerStreamIn", (entity) => {
    const data = entity.cSen;

    if (data) {
        const customScenario = customScenarioMap.get(data);
        if (customScenario)
            customScenario.onStartForNew(entity);
    }
});

mp.events.addDataHandler("cSen", function (entity, data) {
    if (entity) {
        if (entity.cSen) {
            if (entity.handle !== 0) {
                const customScenario = customScenarioMap.get(entity.cSen);
                if (customScenario)
                    customScenario.onEnd(entity);
            }
            delete entity.cSen;
        }

        if (null !== data) {
            if (entity.handle !== 0) {
                const customScenario = customScenarioMap.get(data);
                if (customScenario)
                    customScenario.onStart(entity);
            }

            entity.cSen = data;
        }
    }
});