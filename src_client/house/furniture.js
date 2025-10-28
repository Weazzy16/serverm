const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.events.add('startEditing', (model) => {
	try
	{
		global.binderFunctions.GameMenuClose ();
		gm.discord(translateText("Расставляет мебель в доме"));
		global.OnObjectEditor (mp.game.joaat (model), null, (pos, rot, _) => {				
			callRemote('acceptEdit', pos.x, pos.y, pos.z, rot.x, rot.y, rot.z);
		},
		() => {			
			callRemote('cancelEdit');
			global.discordDefault ()
		}, true);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "house/furniture", "startEditing", e.toString());
	}
});