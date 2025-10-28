const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.events.add('client.parking.open', async () => {
	try
	{
        await global.awaitMenuCheck ();
        //

        global.menuOpen();
        mp.gui.emmit(
            `window.router.setView("HouseMenu")`
        );

	}
	catch (e) 
	{
		callRemote("client_trycatch", "house/index", "client.house.open", e.toString());
	}
});

gm.events.add('client.parking.close', () => {//+
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
})

gm.events.add('client.garage.parking', (number, place) => {//+
    callRemote('server.garage.parking', number, place);
});

//gm.events.add('client.parking.confirm', (sqlId, place) => {//+
//    mp.gui.emmit(`window.events.callEvent("cef.parking.confirm", ${sqlId}, ${place})`);
//});

gm.events.add('client.parking.updateCar', (json) => {//+
    mp.gui.emmit(`window.events.callEvent("cef.parking.carsData", '${json}')`);
    mp.gui.emmit(`window.events.callEvent("cef.parking.confirm")`);
});

gm.events.add('client.vehicle.action', (number, action) => {//+
    if (action === "sell")
        call('client.house.close');

    callRemote('server.vehicle.action', number, action);
});

gm.events.add('client.garage.update', () => {//+
    call('client.house.close');
    callRemote('server.garage.update');
});

//

gm.events.add('client.houseinfo.open', (data) => {//+
    if (global.menuCheck()) return;

    global.menuOpen();
    gm.discord(translateText("Присматривает дом"));
    
    mp.gui.emmit(
        `window.router.setView("HouseBuy", '${data}')`
    );
});

gm.events.add('client.houseinfo.close', () => {//+
    global.menuClose();
    mp.gui.emmit(`window.router.setHud()`);
});

gm.events.add('client.houseinfo.action', (action) => {//+
    call('client.houseinfo.close');
    callRemote('server.houseinfo.action', action);
});
// Когда браузер вызовет 'client:setHouseWaypoint'
gm.events.add('client:setHouseWaypoint', (x, y) => {
  // Ставим новую метку на карту
  mp.game.ui.setNewWaypoint(x, y);
});


//////////////////////

gm.events.add('client.furniture.open', (json) => {//+
    if (global.menuCheck()) return;
    global.menuOpen();
    gm.discord(translateText("Присматривает мебель"));
    mp.gui.emmit(
        `window.router.setView("HouseFurniture", '${json}')`
    );
});

gm.events.add('client.furniture.buy', (name, type) => {//+
    callRemote('server.furniture.buy', name, type);
});

gm.events.add('client.furniture.close', () => {//+
    global.menuClose();
    mp.gui.emmit(`window.router.setHud()`);
});

//


gm.events.add('client.vehicleair.open', (json) => {//+
    if (global.menuCheck()) return;
    global.menuOpen();
    gm.discord(translateText("В магазине вертолётов"));
    mp.gui.emmit(
        `window.router.setView("VehicleAir", '${json}')`
    );
});

gm.events.add('client.vehicleair.exit', () => {//+
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
})