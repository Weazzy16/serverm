const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
var lastScreenEffect = "";
gm.events.add('startScreenEffect', function (effectName, duration, looped) {
	try {
		lastScreenEffect = effectName;
		mp.game.graphics.startScreenEffect(effectName, duration, looped);
	} catch (e) 
	{
		callRemote("client_trycatch", "utils/screeneffects", "startScreenEffect", e.toString());
	}
});

gm.events.add('stopScreenEffect', function (effectName) {
	try {
		var effect = (effectName == undefined) ? lastScreenEffect : effectName;
		mp.game.graphics.stopScreenEffect(effect);
	} catch (e) 
	{
		callRemote("client_trycatch", "utils/screeneffects", "stopScreenEffect", e.toString());
	}
});

gm.events.add('stopAndStartScreenEffect', function (stopEffect, startEffect, duration, looped) {
	try {
		mp.game.graphics.stopScreenEffect(stopEffect);
		mp.game.graphics.startScreenEffect(startEffect, duration, looped);
	} catch (e) 
	{
		callRemote("client_trycatch", "utils/screeneffects", "stopAndStartScreenEffect", e.toString());
	}
});

gm.events.add('screenFadeOut', function (duration) {
	try
	{
		global.FadeScreen (true, duration);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "utils/screeneffects", "screenFadeOut", e.toString());
	}
});

gm.events.add('screenFadeIn', function (duration) {
	try
	{
		global.FadeScreen (false, duration);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "utils/screeneffects", "screenFadeIn", e.toString());
	}
});