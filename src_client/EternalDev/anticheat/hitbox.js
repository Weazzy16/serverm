const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
export default new class HitBox {
	DEFAULT_DIMENSIONS = {
		min: { x: -0.61, y: -0.25, z: -1.29 },
		max: { x: 0.61, y: 0.25, z: 0.94 }
	}

	constructor() {
		this.playerDimensions = mp.game.gameplay.getModelDimensions(mp.game.joaat("mp_m_freemode_01"));

		setTimeout(() => {
			if (this.check("min") || this.check("max")) {
				callRemote("server.anticheat.hitbox");
			}
		}, 500);
	}

	check(type) {
		if (this.playerDimensions[type].x.toFixed(2) > this.DEFAULT_DIMENSIONS[type].x ||
			this.playerDimensions[type].y.toFixed(2) > this.DEFAULT_DIMENSIONS[type].y ||
			this.playerDimensions[type].z.toFixed(2) > this.DEFAULT_DIMENSIONS[type].z) return true;
		return false;
	}
}