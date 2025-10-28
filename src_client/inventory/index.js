const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
require('./rewardslist')


global.gamemenu = false;
global.myStats = false;

let openOtherToggled = false;
let DropIntervalId = 0;

global.pInt = (value) => {
    value = Math.round(value);
    return !value ? 0 : value;
}


global.binderFunctions.GameMenuOpen = () => {
	if (global.tableInFocus)
		return;

    if (!global.gamemenu) OpenGameMenu ();
    else global.binderFunctions.GameMenuClose ();
};

const OpenGameMenu = () => {
	try
	{
		if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || global.menuCheck() || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0)) return;
		if (!global.myStats) callRemote('server.gamemenu.updatestats');    
		mp.gui.emmit(`window.router.updateStatic("PlayerGameMenu");`);
		global.gamemenu = true;
		global.menuOpen(true);
		mp.game.graphics.startScreenEffect("MenuMGIn", 1, true);
		call("sounds.playInterface", "inventory/open_inv", 0.005);
		gm.discord(translateText("Исследует инвентарь"));
		mp.gui.emmit(`window.events.callEvent("cef.inventory.UpdateSpecialVars", ${global.localplayer.vehicle ? true : false})`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "inventory/index", "OpenGameMenu", e.toString());
	}
}


gm.events.add(global.renderName ["500ms"], () => {
	getDrops ();
});

const getDrops = () => {
	try
	{
		if (openOtherToggled) return;
        else if (global.localplayer.vehicle) return;
		else if (!global.gamemenu) return;
		let DropsObject = [];

		const playerPosition = global.localplayer.position;
		
		let
			distance,
			jsonData,
			Index = 0;

		mp.objects.forEachInStreamRangeItems(object => {
            if (object && mp.objects.exists(object) && object['dropData']) {
				distance = mp.game.gameplay.getDistanceBetweenCoords(playerPosition.x, playerPosition.y, playerPosition.z, object.position.x, object.position.y, object.position.z, true);
				if (distance < 3) {
					jsonData = object['dropData'];
					if (jsonData && jsonData.ItemId) {						
						jsonData.Index = Index;
						jsonData.remoteId = object.remoteId;

						DropsObject = [
							...DropsObject,
							jsonData
						]
						Index++;
					}
				}
            }
        });

        if (DropsObject.length > 0) {
            mp.gui.emmit(`window.events.callEvent("cef.inventory.InitOtherData", 8, '${translateText("На земле")}', '${JSON.stringify(DropsObject)}', ${DropsObject.length})`);
        } else {
            mp.gui.emmit(`window.events.callEvent("cef.inventory.InitOtherData", 0)`);
        }
	}
	catch (e) 
	{
		callRemote("client_trycatch", "inventory/index", "getDrops", e.toString());
	}
}

gm.events.add("client.inventory.InitBackpack", (maxSlot, json, use) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.InitMyData", ${maxSlot}, '${json}', ${use})`);
});

gm.events.add("client.inventory.InitData", (json, use) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.InitData", '${json}', ${use})`);
});

gm.events.add("client.inventory.InitOtherData", (otherId, otherName, json, maxSlot, itemId, isArmyCar, isMyTent) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.InitOtherData", ${otherId}, '${otherName}', '${json}', ${maxSlot}, '${itemId}', ${isArmyCar}, ${isMyTent})`);
    openOtherToggled = true;
    if (!global.gamemenu) global.binderFunctions.GameMenuOpen ();
});

gm.events.add("client.inventory.SlotToPrice", (json) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.SlotToPrice", '${json}')`);
});

gm.events.add("client.inventory.OtherClose", () => {
    callRemote('server.gamemenu.inventory.otherclose');
    openOtherToggled = false;
    getDrops ();
});

gm.events.add("client.inventory.InitTradeData", (Name) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.InitTradeData", '${Name}')`);
    
    openOtherToggled = true;
    if (!global.gamemenu) global.binderFunctions.GameMenuOpen ();
});

gm.events.add("client.inventory.UpdateSlot", (Location, SlotId, json, isInfo) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.UpdateSlot", '${Location}', ${SlotId}, '${json}', ${isInfo})`);
});

gm.events.add("start:HPAR::client", (hp, ar) => {
	mp.gui.emmit(`window.events.callEvent("cef.hp.Open")`);
	mp.gui.emmit(`window.events.callEvent("cef.hp.UpdateHealth", ${hp})`);
});

gm.events.add("update:HP::client", (hp) => {
	mp.gui.emmit(`window.events.callEvent("cef.hp.UpdateHealth", ${hp})`);
});



gm.events.add("client:close::HPAR", () => {
	mp.gui.emmit(`window.events.callEvent("cef.hp.Close")`);
});

let cefEatWaterReady = false;

gm.events.add('cefEatWaterReady', () => {
	cefEatWaterReady = true;
	mp.events.callRemote("cefEatWaterReady");
	
});

// Начальная инициализация
gm.events.add("start:EatSystem::client", (eat) => {
	mp.gui.emmit(`window.events.callEvent("cef.eatwater.Open")`);
	mp.gui.emmit(`window.events.callEvent("cef.eatwater.UpdateEat", "${eat.replace('%','')}")`);
});

gm.events.add("start:WaterSystem::client", (water) => {
	mp.gui.emmit(`window.events.callEvent("cef.eatwater.UpdateWater", "${water.replace('%','')}")`);
});

// Обновления еды и воды
gm.events.add("update:EatSystem::client", (eat) => {
	mp.gui.emmit(`window.events.callEvent("cef.eatwater.UpdateEat", "${eat.replace('%','')}")`);
});

gm.events.add("update:WaterSystem::client", (water) => {
	mp.gui.emmit(`window.events.callEvent("cef.eatwater.UpdateWater", "${water.replace('%','')}")`);
});

gm.events.add("client:close::EatWater", () => {
	mp.gui.emmit(`window.events.callEvent("cef.eatwater.Close")`);
});


gm.events.add("client.inventory.TradeUpdate", (status) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.TradeUpdate", ${status})`);
});

gm.events.add("client.inventory.tradeMoney", (name, value) => {
    mp.gui.emmit(`window.events.callEvent("cef.inventory.tradeMoney", "${name}", "${value}")`);
});

gm.events.add("client.inventory.Close", () => {
    global.binderFunctions.GameMenuClose ();
});

gm.events.add("client.inventory.Open", async () => {
    global.myStats = true;
      
    await global.wait(50);
    global.binderFunctions.GameMenuOpen ();
});

gm.events.add("client.gamemenu.inventory.move", (selectArrayName, selectIndex, hoverArrayName, hoverIndex) => {
    callRemote('server.gamemenu.inventory.move', String(selectArrayName), global.pInt (selectIndex), String(hoverArrayName), global.pInt (hoverIndex));
});

gm.events.add("client.gamemenu.inventory.move.stack", (selectArrayName, selectIndex, hoverArrayName, hoverIndex, count) => {
    callRemote('server.gamemenu.inventory.move.stack', String(selectArrayName), global.pInt (selectIndex), String(hoverArrayName), global.pInt (hoverIndex), global.pInt (count));
});

gm.events.add("client.gamemenu.inventory.use", (ArrayName, Index) => {
    callRemote('server.gamemenu.inventory.use', String(ArrayName), global.pInt (Index));
});

gm.events.add("client.gamemenu.inventory.drop", (ArrayName, Index) => {
	let position = global.localplayer.position;
	position.z = mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z, 0.0, false);
    callRemote('server.gamemenu.inventory.drop', String(ArrayName), global.pInt (Index), position.z);
});

gm.events.add("client.gamemenu.inventory.stack", (ArrayName, Index, id, value) => {
    callRemote('server.gamemenu.inventory.stack', String(ArrayName), global.pInt (Index), global.pInt (id), global.pInt (value));
});

gm.events.add("client.gamemenu.inventory.buy", (ArrayName, Index, value) => {
    callRemote('server.gamemenu.inventory.buy', String(ArrayName), global.pInt (Index), global.pInt (value));
});

gm.events.add("client.gamemenu.inventory.trade", (status) => {
    callRemote('server.gamemenu.inventory.trade', global.pInt (status));
});

gm.events.add("client.gamemenu.inventory.tradeMoney", (value) => {
    callRemote('server.gamemenu.inventory.tradeMoney', global.pInt (value));
});

gm.events.add("client.gamemenu.inventory.toput", (ArrayName, Index) => {
    callRemote('server.gamemenu.inventory.toput', String(ArrayName), global.pInt (Index));
});

gm.events.add("client.gamemenu.inventory.nearby", (remoteId) => {
	const object = mp.objects.atRemoteId(remoteId);
	if (object && object.doesExist() && object.handle) {
    	callRemote('server.raise', object);
	}
});

gm.events.add("checkClientSpecialVars", () => {
	if (!global.menuCheck()) return;
	mp.gui.emmit(`window.events.callEvent("cef.inventory.UpdateSpecialVars", ${global.localplayer.vehicle ? true : false})`);
});


global.binderFunctions.GameMenuClose = (toggled = true) => {
	try
	{
		if (!global.gamemenu) return; 
    
		DropIntervalId = 0;
		if (toggled) {
			callRemote('server.gamemenu.inventory.close');
			mp.gui.emmit(`window.accountStore.otherStatsData ('{}')`);
		}
		global.myStats = false;
		mp.gui.emmit(`window.router.setHud();`);
		mp.gui.emmit(`window.events.callEvent("cef.inventory.Close")`);
		global.gamemenu = false;
		global.menuClose();
		openOtherToggled = false;
		call("sounds.playInterface", "inventory/open_inv", 0.005);
		call('client.everydayawards.close');
		mp.game.graphics.stopScreenEffect("MenuMGIn");
	}
	catch (e) 
	{
		callRemote("client_trycatch", "inventory/index", "global.binderFunctions.GameMenuClose", e.toString());
	}
}

global.GetItemData = (entity) => {
	try
	{
		if (entity == null || entity.type != "object" || !mp.objects.exists(entity)) return;
		if (entity['dropData'] && entity['dropData'].ItemId != undefined) {				
			mp.gui.emmit(`window.hudItem.drop (${entity['dropData'].ItemId}, ${entity['dropData'].Count}, '${entity['dropData'].Data}')`);
		} //else if () {
		//	ObjectName
		//}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "inventory/index", "global.GetItemData", e.toString());
	}
}

global.GetItem = (ItemId) => {
	mp.gui.emmit(`window.isItem([${ItemId}])`);
}

global.GetItems = (ItemsId) => {
	mp.gui.emmit(`window.isItem('${JSON.stringify(ItemsId)}')`);
}

mp.events.add('render', () => {
    if (global.gamemenu) {
        // Отключаем клавиши движения
        mp.game.controls.disableControlAction(0, 32, true); // W — вперёд
        mp.game.controls.disableControlAction(0, 33, true); // S — назад
        mp.game.controls.disableControlAction(0, 34, true); // A — влево
        mp.game.controls.disableControlAction(0, 35, true); // D — вправо

        // Отключаем другие необходимые действия (коды могут варьироваться)
        mp.game.controls.disableControlAction(0, 44, true); // Q (например, смена камеры или др.)
        mp.game.controls.disableControlAction(0, 74, true); // F (вход/выход из транспорта)
        mp.game.controls.disableControlAction(0, 47, true); // G (бросок предметов и т.п.)
    }
});