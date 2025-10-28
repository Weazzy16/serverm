// STOCK //!
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.events.add('openStock', (data) => {
	try 
	{
		if (global.menuCheck()) return;
		mp.gui.emmit(`window.router.setView("FractionsStock", '${data}');`);
		gm.discord(translateText("На фракционном складе"));
		global.menuOpen();
	}
	catch (e) 
	{
		callRemote("client_trycatch", "fractions/stock", "openStock", e.toString());
	}
});

gm.events.add('closeStock', () => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "fractions/stock", "closeStock", e.toString());
	}
});

gm.events.add('stockTake', (index) => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);

		switch (index) {
			case 0: //cash
				callRemote('setStock', "money");
				global.input.set(translateText("Взять деньги"), translateText("Введите кол-во денег"), 10, "take_stock");
				break;
			case 1: //healkit
				callRemote('setStock', "medkits");
				global.input.set(translateText("Взять аптечки"), translateText("Введите кол-во аптечек"), 10, "take_stock");
				break;
			case 2: //weed
				callRemote('setStock', "drugs");
				global.input.set(translateText("Взять наркотики"), translateText("Введите кол-во наркоты"), 10, "take_stock");
				break;
			case 3: //mats
				callRemote('setStock', "mats");
				global.input.set(translateText("Взять маты"), translateText("Введите кол-во матов"), 10, "take_stock");
				break;
			case 4: //weapons stock
				callRemote('openWeaponStock');
				break;
		}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "fractions/stock", "stockTake", e.toString());
	}
});

gm.events.add('stockPut', (index) => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);

		switch (index) {
			case 3: //mats
				callRemote('setStock', "mats");
				global.input.set(translateText("Положить маты"), translateText("Введите кол-во матов"), 10, "put_stock");
				break;
			case 0: //cash
				callRemote('setStock', "money");
				global.input.set(translateText("Положить деньги"), translateText("Введите кол-во денег"), 10, "put_stock");
				break;
			case 1: //healkit
				callRemote('setStock', "medkits");
				global.input.set(translateText("Положить аптечки"), translateText("Введите кол-во аптечек"), 10, "put_stock");
				break;
			case 2: //weed
				callRemote('setStock', "drugs");
				global.input.set(translateText("Положить наркотики"), translateText("Введите кол-во наркоты"), 10, "put_stock");
				break;
			case 4: //weapons stock
				callRemote('openWeaponStock');
				break;
		}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "fractions/stock", "stockPut", e.toString());
	}
});

gm.events.add('stockExit', () => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "fractions/stock", "stockExit", e.toString());
	}
});