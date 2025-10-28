const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
mp.events.add('entityStreamIn', (entity) => {
    if (entity) {
        if (entity.type === 'player' && mp.players.exists(entity)) {
            entity.setLodDist(global.getLodDist (global.DistancePlayer));
            call('pPlayerStreamIn', entity);
            call('playerStreamIn', entity);
        } else if (entity.type === 'vehicle' && mp.vehicles.exists(entity)) {
            entity.setLodDist(global.getLodDist (global.DistanceVehicle));
            call('vehicleStreamIn', entity);
        } else if (entity.type === 'ped' && mp.peds.exists(entity)) {
            entity.setLodDist(global.getLodDist (global.DistancePlayer));
            call('pedStreamIn', entity);
        } else if (entity.type === 'object' && mp.objects.exists(entity)) {
            call('objectStreamIn', entity);
        }
    }
});

mp.events.add('entityStreamOut', (entity) => {
    if (entity) {
        if (entity.type === 'player' && mp.players.exists(entity)) {
            call('playerStreamOut', entity);
        } else if (entity.type === 'vehicle' && mp.vehicles.exists(entity)) {
            call('vehicleStreamOut', entity);
        } else if (entity.type === 'ped' && mp.peds.exists(entity)) {
            call('pedStreamOut', entity);
        } else if (entity.type === 'object' && mp.objects.exists(entity)) {
            call('objectStreamOut', entity);
        }
    }
});


mp.peds.newLegacy = (hash, position, heading, streamIn, dimension) => {
    let ped = mp.peds.new(hash, position, heading, dimension);
    ped.streamInHandler = streamIn;
    return ped;
};

mp.events.add("pedStreamIn", entity => {
    if (entity.streamInHandler) {
        entity.streamInHandler(entity);
    }
});

mp.vehicles.newLegacy = (hash, position, parameters, streamIn) => {
    let vehicle = mp.vehicles.new(hash, position, parameters);
    vehicle.streamInHandler = streamIn;
    return vehicle;
};

mp.events.add("vehicleStreamIn", entity => {
    if (entity.streamInHandler) {
        entity.streamInHandler(entity);
    }
});

const _lastEngineState = new Map();

// на каждый кадр проверяем: сел ли игрок в машину и поменяло ли двигатель состояние
mp.events.add('render', () => {
    const player = mp.players.local;
    const veh = player.vehicle;
    if (veh && veh.handle !== 0) {
        const id = veh.remoteId;
        const running = veh.engineOn;  // true = двигатель включён

        // если впервые встречаем эту машину — запомним состояние
        if (!_lastEngineState.has(id)) {
            _lastEngineState.set(id, running);
        }

        // если состояние изменилось — шлём на сервер
        if (_lastEngineState.get(id) !== running) {
            _lastEngineState.set(id, running);
            mp.events.callRemote('server.vehicle.engineToggle', veh, running);
        }
    }
});