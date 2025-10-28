const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const localplayer = mp.players.local;
gm.events.add(global.renderName ["5s"], () => {
    const vehicle = localplayer.vehicle;

    if (localplayer.mileageClass) {
        if (!(vehicle && vehicle.getPedInSeat(-1) === localplayer.handle && localplayer.mileageClass.vehicle.handle === vehicle.handle))
            localplayer.mileageClass = null
    } else if (vehicle && vehicle.getPedInSeat(-1) === localplayer.handle) {
        localplayer.mileageClass = new MileageClass(vehicle);
    }
});

gm.events.add(global.renderName ["200ms"], () => {
    if (localplayer.mileageClass && localplayer.vehicle)
        localplayer.mileageClass.calc();
});

gm.events.add(global.renderName ["1s"], () => {
    if (localplayer.mileageClass && localplayer.vehicle)
        localplayer.mileageClass.update();
});

gm.events.add("vehicles.mileage.onUpdate", vehicle => {
    if (localplayer.mileageClass && localplayer.vehicle)
        localplayer.mileageClass.reset(vehicle);
});

class MileageClass {
    constructor(vehicle) {
        this.vehicle     = vehicle;
        // сразу инициализируем базовую позицию
        const pos = vehicle.position;
        this.lastPosition = { x: pos.x, y: pos.y, z: pos.z };

        // здесь можно прочитать из SharedData или из сервера:
        // this.centimeters = mp.getLocalData('MILEAGE_CENTI') || 0;
        this.centimeters = 0;
    }

    calc() {
        const pos = this.vehicle.position;
        const dx = pos.x - this.lastPosition.x;
        const dy = pos.y - this.lastPosition.y;
        const dz = pos.z - this.lastPosition.z;
        const dist = Math.sqrt(dx*dx + dy*dy + dz*dz);

        this.lastPosition = { x: pos.x, y: pos.y, z: pos.z };

        if (dist < 0.5) return;  // игнорим мелкие «буксы»

        this.centimeters += Math.round(dist * 100);
    }

    /**
     * Отправляет накопленные сантиметры на сервер (раз в секунду)
     * и сбрасывает счётчик.
     */
    update() {
        if (
            this.centimeters > 0 &&
            mp.vehicles.exists(this.vehicle) &&
            this.vehicle.remoteId
        ) {
            callRemote(
                'server.vehicle.updateMileage',
                this.vehicle.remoteId,
                this.centimeters
            );
            this.centimeters = 0;
        }
    }

    /**
     * Сбрасывает локальный счётчик и сохранённую позицию,
     * если машина ресется (при стриме/уходе).
     */
    reset(vehicle) {
        if (
            mp.vehicles.exists(vehicle) &&
            vehicle.handle === this.vehicle.handle
        ) {
            this.centimeters  = 0;
            this.lastPosition = null;
        }
    }
}
