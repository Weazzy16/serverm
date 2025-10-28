const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.events.add('setWorldLights', function (toggle) {
	try 
	{
		mp.game.graphics.resetLightsState();
		for (let i = 0; i <= 16; i++) {
			if(i != 6 && i != 7) mp.game.graphics.setLightsState(i, toggle);
		}
	} 
	catch (e) 
	{
		callRemote("client_trycatch", "world/sync", "setWorldLights", e.toString());
	}
});

gm.events.add('setDoorLocked', function (model, x, y, z, locked, angle) {
	try 
	{
		mp.game.object.doorControl(model, x, y, z, locked, 0, 0, angle);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "world/sync", "setDoorLocked", e.toString());
	}
});