// PETROL //
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.events.add('openPetrol', () => {
	try
	{
		if (global.menuCheck()) return;
		global.menuOpen();
		mp.gui.emmit(`window.router.setView("PlayerGasStation");`);
		gm.discord(translateText("Заправляется"));
	}
	catch (e) 
	{
		callRemote("client_trycatch", "vehicle/petrol", "openPetrol", e.toString());
	}
});

gm.events.add('petrol', (data) => {
	try
	{
		callRemote('petrol', data);
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "vehicle/petrol", "petrol", e.toString());
	}
});

gm.events.add('petrol.full', () => {
	try
	{
		callRemote('petrol', 9999);
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "vehicle/petrol", "petrol.full", e.toString());
	}
});

gm.events.add('petrol.gov', () => {
	try
	{
		callRemote('petrol', 99999);
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "vehicle/petrol", "petrol.gov", e.toString());
	}
});

gm.events.add('closePetrol', () => {
	try
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "vehicle/petrol", "closePetrol", e.toString());
	}
});