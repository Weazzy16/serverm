import { currentScenario, majesticCef } from "../EternalDev/intro";

const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
global.showhud = false;
global.Petrol = 0;
global.dimension = 0;
mp.game.gameplay.setFakeWantedLevel(0);

var cruiseSpeed = -1;
var cruiseLastPressed = 0;
global.showHint = true;

global.hudstatus =
{
    online: 0, // Last online int
    
    street: "",
	area: "",
	direction: "",

	speed: 0,
    invehicle: false,
    engine: false,
    doors: true,
    fuel: 0,
    health: 0
}

gm.events.add('showHUD', (show) => {
	try
	{
		global.showhud = show;
		if (!show) mp.gui.emmit(`window.hudStore.isHelp (${global.showhud})`);
		else if (show && global.showHint) mp.gui.emmit(`window.hudStore.isHelp (${global.showhud})`);

		mp.gui.emmit(`window.hudStore.isHudVisible (${global.showhud})`);

		mp.game.ui.displayAreaName(global.showhud);
		mp.game.ui.displayRadar(global.showhud);
		mp.game.ui.displayHud(global.showhud);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "showHUD", e.toString());
	}
});

let isInitCharData = false;
const InitCharDataToName = [
	{ name: "UUID", noInit: true },
	{ name: "Name", isString: true, noInit: true },
	{ name: "Gender" },
	{ name: "isAdmin" },
	{ name: "Money" },
	{ name: "BankMoney" },
	{ name: "WorkID" },
	{ name: "IsLeader" },
	{ name: "FractionID" },
	{ name: "FractionLVL", isString: true },
	{ name: "OrganizationID" },
	{ name: "EXP" },
	{ name: "LVL" },
	{ name: "Wanted" },
	{ name: "CreateDate", isString: true, noInit: true },
	{ name: "EarnedMoneyDay" },
	{ name: "EarnedMoneyMonth" },
	{ name: "EarnedMoney" },
	{ name: "SpentMoneyDay" },
	{ name: "SpentMoneyMonth" },
	{ name: "SpentMoney" },
	{ name: "Sim" },
];

const getJobSkillsInfo = (skillsData, nextLevelsData, currentLevelsInfo) => {
	if (!skillsData || !nextLevelsData || !currentLevelsInfo)
		return;

	if (skillsData.length < 1) return;

	let jobSkillsInfo = [
		{
			name: translateText("Электрик"),
			max: 15000,
			nextLevel: 700,
			currentLevel: 0,
			current: 0
		},
		{
			name: translateText("Газонокосильщик"),
			max: 40000,
			nextLevel: 2000,
			currentLevel: 0,
			current: 0
		},
		{
			name: translateText("Почтальон"),
			max: 4000,
			nextLevel: 200,
			currentLevel: 0,
			current: 0
		},
		{
			name: translateText("Таксист"),
			max: 1000,
			nextLevel: 25,
			currentLevel: 0,
			current: 0
		},
		{
			name: translateText("Водитель автобуса"),
			max: 70000,
			nextLevel: 3000,
			currentLevel: 0,
			current: 0
		},
		{
			name: translateText("Автомеханик"),
			max: 250,
			nextLevel: 10,
			currentLevel: 0,
			current: 0
		},
		{
			name: translateText("Дальнобойщик"),
			max: 700,
			nextLevel: 30,
			currentLevel: 0,
			current: 0
		},
		{
			name: translateText("Инкассатор"),
			max: 3000,
			nextLevel: 150,
			currentLevel: 0,
			current: 0
		}
	];

	for (let i = 0; i < jobSkillsInfo.length; i++) {
		if (skillsData[i] !== undefined) {
			jobSkillsInfo[i].current = skillsData[i];
		}

		if (nextLevelsData[i] !== undefined) {
			jobSkillsInfo[i].nextLevel = nextLevelsData[i];
		}

		if (currentLevelsInfo[i] !== undefined) {
			jobSkillsInfo[i].currentLevel = currentLevelsInfo[i];
		}
	}

	return jobSkillsInfo;
}

const getStatsData = (json) => {
	json = JSON.parse (json);

	return {
		Login: json[0],
		VipLvl: json[1],
		VipDate: json[2],
		Warns: json[3],
		Unwarn: json[4],
		TodayTime: json[5],
		MonthTime: json[6],
		YearTime: json[7],
		TotalTime: json[8],
		//
		jobSkillsInfo: getJobSkillsInfo (json[9], json[10], json[11]),
		//
		Name: json[12],
		isAdmin: json[13],
		WeddingName: json[14],
		Gender: json[15],
		LVL: json[16],
		EXP: json[17],
		Sim: json[18],
		WorkID: json[19],
		FractionID: json[20],
		FractionLVL: json[21],
		OrganizationID: json[22],
		OrganizationLVL: json[23],
		UUID: json[24],
		Bank: json[25],
		BankMoney: json[26],
		Money: json[27],
		CreateDate: json[28],
		//
		houseId: json[29],
		houseType: json[30],
		houseCash: json[31],
		houseCopiesHour: json[32],
		housePaid: json[33],
		maxcars: json[34],
		housePrice: json[35],
		housePosition: json[36],
		//
		BizId: json[37],
		BizCash: json[38],
		BizCopiesHour: json[39],
		BizPaid: json[40],
		BizSellPrice: json[41],
		BizEnterPoint: json[42],
		BizType: json[43],
		//
		Licenses: json[44],
		Wanted: json[45],
		EarnedMoneyDay: json[46],
		EarnedMoneyMonth: json[47],
		EarnedMoney: json[48],
		SpentMoneyDay: json[49],
		SpentMoneyMonth: json[50],
		SpentMoney: json[51],
		
		ReportsCount: json[52],
    Achievements: json[53],
  
	}
}




gm.events.add("client.inventory.stats", (json) => {
	const characterData = getStatsData (json);
	if (characterData.housePosition) {
    const pos = characterData.housePosition;
    // rpc.getAreaName синхронно возвращает название зоны
    characterData.area = global.getAreaName(pos.x, pos.y, pos.z);
	characterData.street = global.getStreetName(pos.x, pos.y, pos.z);
  }
  if (characterData.BizEnterPoint) {
    const pos = characterData.BizEnterPoint;
    // Сохраняем в новые поля, чтобы не перезаписывать house*
    characterData.bizArea   = global.getAreaName(pos.x, pos.y, pos.z);
    characterData.bizStreet = global.getStreetName(pos.x, pos.y, pos.z);
  }

    mp.gui.emmit(`window.charStore.charData ('${JSON.stringify(characterData)}')`);
	if (characterData) {
		if (characterData.Name) {
			mp.gui.emmit(`window.charStore.charName ('${characterData.Name}')`);
		}		
		if (!isInitCharData) {
			isInitCharData = true;
		
			InitCharDataToName.forEach(data => {
				if (characterData [data.name] != undefined) {
					if (!data.noInit) {						
						call('client.charStore.' + data.name, characterData [data.name], data.isString);
					} else if (!data.isString) {
						mp.gui.emmit(`window.charStore.char${data.name} (${characterData [data.name]})`);
					} else {			
						mp.gui.emmit(`window.charStore.char${data.name} ('${characterData [data.name]}')`);
					}
				}
			});
		}
	}
});

gm.events.add('client.accountStore.otherStatsData', (json) => {
	const characterData = getStatsData (json);
	mp.gui.emmit(`window.accountStore.otherStatsData('${JSON.stringify(characterData)}')`);
});





InitCharDataToName.forEach(data => {
	if (!data.noInit) {
		gm.events.add('client.charStore.' + data.name, function (value, isString = false) {
			if (!isString)
				mp.gui.emmit(`window.charStore.char${data.name} (${value})`);
			else
				mp.gui.emmit(`window.charStore.char${data.name} ('${value}')`);
		});
	}
});



//


const InitAccountDataToName = [
	{ name: "Login", isString: true, noInit: true },
	{ name: "SocialClub", isString: true, noInit: true },
	{ name: "Redbucks" },
	{ name: "Unique", isString: true },
	{ name: "Vip" },
	{ name: "VipDate", isString: true },
	{ name: "Subscribe", isString: true },
	{ name: "Email", isString: true },
];

let spawnDatas = {};
gm.events.add("client:spawnSelector:tryOpen", (uuid) => {
	var spawnData = spawnDatas[Number(uuid)];
	
	if (spawnData == null) {
		call('notify', 4, 9, translateText("Ошибка при получении данных об спавне!"), 3000);
		return;
	}

	call("client.spawnSelector.open", uuid, JSON.stringify(spawnData))
});


gm.events.add('toslots', async function (data, customizationsData, clothes, accessory) {
	while(!currentScenario.isPlaying) {
        await global.wait(1);
    }

	mp.gui.emmit(`window.listernEvent ('queueText', false);`);
	majesticCef.execute(`auth.reset()`);

	global.authorizationOpened = false;
	
    const accountData = JSON.parse (data);

	spawnDatas = {};

	const charactersData = accountData.chars;
	for(const [uuid, data] of Object.entries(charactersData)) {
		spawnDatas[Number(uuid)] = data.Spawn;
	}

	majesticCef.execute(`characterSelector.initData(${data})`);

	let charsSlot = [-1,-1,-2];

	if (accountData) {
		mp.gui.emmit(`window.accountStore.accountLogin('${accountData.Login}')`);
		charsSlot = accountData.charsSlot;
	}

	mp.gui.emmit(`window.accountStore.accountData('${data}')`);

	call('initCustomizationCharsData', customizationsData, clothes, accessory, JSON.stringify (charsSlot));

    if (accountData) {	
		InitAccountDataToName.forEach(data => {
			if (accountData [data.name] != undefined) {
				if (!data.noInit) {						
					call('client.accountStore.' + data.name, accountData [data.name], data.isString);
				} else if (!data.isString) {
					mp.gui.emmit(`window.accountStore.account${data.name} (${accountData [data.name]})`);
				} else {			
					mp.gui.emmit(`window.accountStore.account${data.name} ('${accountData [data.name]}')`);
				}

				if (data.name == 'Redbucks')
					majesticCef.execute(`characterSelector.redBucks = ${accountData [data.name]}`);
			}
		});
	}
});

InitAccountDataToName.forEach(data => {
	mp.gui.emmit(`window.hudStore.isAdmin(${global.isAdmin})`);
	if (!data.noInit) {
		gm.events.add('client.accountStore.' + data.name, function (value, isString = false) {
			if (!isString)
				mp.gui.emmit(`window.accountStore.account${data.name} (${value})`);
			else
				mp.gui.emmit(`window.accountStore.account${data.name} ('${value}')`);

			if (data.name == "Redbucks")
				majesticCef.execute(`characterSelector.redBucks = ${value}`);
		});
	}
});

//

global.binderFunctions.o_hud = () => {// F5 key
	try
	{
		if (global.menuCheck () || global.chatActive ) return;

		if (global.showhud && global.showHint) {
			global.showHint = false;
			mp.gui.emmit(`window.hudStore.isHelp (false)`);
		}
		else if (global.showhud) {
			global.showhud = !global.showhud;
			call('showHUD', global.showhud);
		}
		else {
			global.showHint = true;
			mp.gui.emmit(`window.hudStore.isHelp (true)`);
			global.showhud = !global.showhud;
			call('showHUD', global.showhud);
		}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "global.binderFunctions.o_hud", e.toString());
	}
};

// CRUISE CONTROL //
global.binderFunctions.cruise = () => {// 6 key - cruise mode on/off
	try
	{
		if (!global.loggedin || global.chatActive || global.editing || global.menuCheck ()) return;
		if (!global.localplayer.isInAnyVehicle(false) || global.localplayer.vehicle.getPedInSeat(-1) != global.localplayer.handle) return;
		let vclass = global.localplayer.vehicle.getClass();
		if(vclass == 14 || vclass == 15 || vclass == 16) return;
		if(global.localplayer.vehicle.isOnAllWheels() == false) return;
		var veh = global.localplayer.vehicle;
		if(veh.isTyreBurst(4, true) || veh.isTyreBurst(5, true)) return;
		if(veh.model == 1747439474 || veh.model == 2034235290) return;
		if (new Date().getTime() - cruiseLastPressed < 300) {
			call('openInput', translateText("Круиз-контроль"), translateText("Укажите скорость в км/ч"), 3, 'setCruise');
			mp.gui.emmit(`window.vehicleState.cruiseControl (false)`);
		} else {
			if (cruiseSpeed == -1) {
				var vspeed = veh.getSpeed();
				if (vspeed > 1) {
					veh.setMaxSpeed(vspeed);
					cruiseSpeed = vspeed;
					mp.gui.emmit(`window.vehicleState.cruiseControl (true)`);
				}
			}
			else {
				cruiseSpeed = -1;
				veh.setMaxSpeed(mp.game.vehicle.getVehicleModelMaxSpeed(veh.model));
				mp.gui.emmit(`window.vehicleState.cruiseControl (false)`);
			}
		}
		cruiseLastPressed = new Date().getTime();
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "global.binderFunctions.cruise", e.toString());
	}
};

gm.events.add('setCruiseSpeed', function (speed)
{
	try
	{
		if (!speed || speed.length < 1) return;
		speed = parseInt(speed);
		if (speed === NaN || speed < 1) return;
		if (!global.localplayer.isInAnyVehicle(false) || global.localplayer.vehicle.getPedInSeat(-1) != global.localplayer.handle) return;
		let vclass = global.localplayer.vehicle.getClass();
		if(vclass == 14 || vclass == 15 || vclass == 16) return;
		if(global.localplayer.vehicle.isOnAllWheels() == false) return;
		var veh = global.localplayer.vehicle;
		if(veh.isTyreBurst(4, true) || veh.isTyreBurst(5, true)) return;
		if(veh.model == 1747439474 || veh.model == 2034235290) return;
		var curSpeed = veh.getSpeed();
		speed = speed / 3.6; // convert from kph to mps
		if(speed < curSpeed) 
		{
			call('notify', 4, 9, translateText("Нельзя установить скорость меньше, чем она есть на данный момент, снизьте скорость и попробуйте еще раз."), 6000);
			return;
		}

		var maxSpeed = mp.game.vehicle.getVehicleModelMaxSpeed(veh.model);

		if (speed > maxSpeed) speed = maxSpeed;
		veh.setMaxSpeed(speed);
		cruiseSpeed = speed;
		mp.gui.emmit(`window.vehicleState.cruiseControl (true)`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "setCruiseSpeed", e.toString());
	}
});

gm.events.add('newPassport', function (player, pass) {
	try
	{
		if (player && mp.players.exists(player))
        	global.passports[player.name] = pass;
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "newPassport", e.toString());
	}
});

let optimizationupdate = 0;
let dimensionupdate = 0;

global.hudstatus.invehicle = false;

const Direction = (heading) => {
	heading %= 360;
	if (heading >= 22.5 && heading < 67.5) return "NE";
	else if (heading >= 67.5 && heading < 112.5) return "E";
	else if (heading >= 112.5 && heading < 157.5) return "SE";
	else if (heading >= 157.5 && heading < 202.5) return "S";
	else if (heading >= 202.5 && heading < 247.5) return "SW";
	else if (heading >= 247.5 && heading < 292.5) return "W";
	else if (heading >= 292.5 && heading < 337.5) return "NW";
	else return "N";
}

global.getStreetName = (posX, posY, posZ) => {
	const street = mp.game.pathfind.getStreetNameAtCoord(posX, posY, posZ, 0, 0);
	return mp.game.ui.getStreetNameFromHashKey(street.streetName);
}

rpc.register("rpc.getStreetName", (pos) => {
	return getStreetName (pos.x, pos.y, pos.z);
});

global.getAreaName = (posX, posY, posZ) => {
	const area  = mp.game.zone.getNameOfZone(posX, posY, posZ);
	return mp.game.ui.getLabelText(area);
}

rpc.register("rpc.getAreaName", (pos) => {
	return getAreaName (pos.x, pos.y, pos.z);
});

// ✅ src_client/player/hud.js — единая передача всех событий в CEF

gm.events.add("everyday.coins.start", (newCoins, newTime, newReceived) => {
    mp.gui.emmit(`window.events.callEvent("cef.coins.start", "${newCoins}", "${newTime}", ${newReceived})`);
});

gm.events.add("client::everyday.coins.update", (newTime) => {
    mp.gui.emmit(`window.events.callEvent("cef.coins.update", "${newTime}")`);
});

gm.events.add("everyday.coins.stop", (newTime, newReceived) => {
    mp.gui.emmit(`window.events.callEvent("cef.coins.stop", "${newTime}", ${newReceived})`);
});

gm.events.add(global.renderName ["1s"], () => {
	try {
		getSafeZona ();

		// Обновляем онлайн в логотипе
		if(global.hudstatus.online != mp.players.length)
		{
			global.hudstatus.online = mp.players.length;
			
			mp.gui.emmit(`window.serverStore.serverOnline (${global.hudstatus.online})`);
			
		}
		// Обновляем улицу и район
		const street = getStreetName (global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z);
		const area = getAreaName (global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z);
		
		if(global.hudstatus.street != street)
		{
			global.hudstatus.street = street;
			
			mp.gui.emmit(`window.hudStore.street ("${street}")`);
		}
		
		if(global.hudstatus.area != area)
		{
			global.hudstatus.area = area;   
			
			mp.gui.emmit(`window.hudStore.area ("${area}")`);
		}

		const direction = Direction (360 - global.localplayer.getHeading ());
		if(global.hudstatus.direction != direction)
		{
			global.hudstatus.direction = direction;   
			
			mp.gui.emmit(`window.hudStore.direction ("${direction}")`);
		}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", global.renderName ["1s"], e.toString());
	}
});

gm.events.add(global.renderName ["125ms"], () => {
	try {
		if (!global.loggedin) return;

		const vehiclestatus = global.localplayer.isInAnyVehicle(false);
		const vehicle = global.localplayer.vehicle;

		if(vehiclestatus && global.hudstatus.invehicle) {
			if(vehicle != null) 
			{
				// Скорость
				if (global.hudstatus.speed !== (vehicle.getSpeed() * 3.6).toFixed()) {
					global.hudstatus.speed = (vehicle.getSpeed() * 3.6).toFixed();
					mp.gui.emmit(`window.vehicleState.speed(${global.hudstatus.speed})`);
				}
				
				// Передача
				const currentGear = vehicle.gear || 0;
				mp.gui.emmit(`window.vehicleState.gear(${currentGear})`);
				
				// Топливо
				if (global.Petrol === 'undefined' || typeof global.Petrol !== "number" || isNaN(global.Petrol)) {
					global.Petrol = Number(vehicle.getVariable('PETROL'));
					mp.gui.emmit(`window.vehicleState.fuel(${global.Petrol}, ${vehicle.getClass()})`);
				}
				
				// Пробег
				const sharedMileage = vehicle.getVariable('MILEAGE');
				if (sharedMileage !== undefined && sharedMileage !== null) {
					mp.gui.emmit(`window.vehicleState.mileage(${sharedMileage})`);
				}
				
				// ✅ RPM из игры со смещением (1 строка!)
				const rpm = Math.max(0, (vehicle.rpm || 0) - 0.20);
				mp.gui.emmit(`window.vehicleState.rpm(${rpm})`);
				
				// Круиз-контроль
				let vehmodel = vehicle.model;
				if (cruiseSpeed != -1) {
					if(vehmodel == 1747439474 || vehmodel == 2034235290) {
						cruiseSpeed = -1;
						mp.gui.emmit(`window.vehicleState.cruiseControl (false)`);
					}
					else vehicle.setMaxSpeed(cruiseSpeed);
				} else {
					if(vehmodel == 1747439474) vehicle.setMaxSpeed(30);	
					else if(vehmodel == 2034235290) vehicle.setMaxSpeed(40);
				}
			const isHandbrakePressed = mp.game.controls.isControlPressed(0, 76); // 76 = VEH_HANDBRAKE (пробел)
				mp.gui.emmit(`window.vehicleState.handbrake(${isHandbrakePressed})`);
				
				// ✅ ДОБАВЬ: Фары (lights)
				const lightsState = vehicle.getVariable('LIGHTS_STATE') || 0;
				mp.gui.emmit(`window.vehicleState.lights(${lightsState})`);
			}
		} else if (vehiclestatus && !global.hudstatus.invehicle) {
			ShowSpeed (vehicle);
		} else if (!vehiclestatus && global.hudstatus.invehicle) {
			CloseVehicleSpeed ();
		}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", global.renderName ["125ms"], e.toString());
	}
});


// ✅ ==================== ФАРЫ (С БЕЗОПАСНОЙ НАТИВКОЙ) ====================

// ✅ Функция получения состояния фар из игры (с защитой от краша)
function getVehicleLightsStateSafe(vehicle) {
    try {
        // Используем безопасную версию нативки
        const result = Natives.GET_VEHICLE_LIGHTS_STATE_SAFE(vehicle.handle);
        
        if (!result.success) {
            console.log('🔦 [ERROR] Native failed:', result.error);
            return -1; // Нативка не работает
        }
        
        if (result.highBeamsOn) return 2; // Дальний свет
        if (result.lightsOn) return 1; // Ближний свет
        return 0; // Выключены
    } catch (e) {
        console.log('🔦 [ERROR] getVehicleLightsStateSafe:', e);
        return -1;
    }
}

let lastLightsState = -1;
let lightsCheckFailed = false;
let nativeCheckAttempts = 0;

// ✅ Проверка каждые 500мс
setInterval(() => {
    try {
        // Если нативка не работает после 3 попыток — не пытаемся больше
        if (lightsCheckFailed && nativeCheckAttempts >= 3) return;
        
        if (!global.loggedin || !global.localplayer.isInAnyVehicle(false)) {
            if (lastLightsState !== -1) {
                lastLightsState = -1;
            }
            return;
        }
        
        const vehicle = global.localplayer.vehicle;
        if (!vehicle || vehicle.getPedInSeat(-1) != global.localplayer.handle) return;
        
        // ✅ Получаем состояние фар из игры
        const currentState = getVehicleLightsStateSafe(vehicle);
        
        // Если нативка не работает — считаем попытки
        if (currentState === -1) {
            nativeCheckAttempts++;
            if (nativeCheckAttempts >= 3) {
                lightsCheckFailed = true;
                console.log('🔦 [WARNING] Native не работает после 3 попыток. Используй биндер на H!');
            }
            return;
        }
        
        // Нативка работает! Сбрасываем счётчик
        nativeCheckAttempts = 0;
        lightsCheckFailed = false;
        
        // Если состояние изменилось — отправляем на сервер
        if (currentState !== lastLightsState) {
            lastLightsState = currentState;
            callRemote('server.vehicle.syncLights', currentState);
            console.log('🔦 [AUTO] Lights state changed:', currentState);
        }
    } catch (e) {
        console.log('🔦 [AUTO ERROR]:', e);
        lightsCheckFailed = true;
    }
}, 500);



// ✅ ==================== КОНЕЦ ФАРЫ ====================

gm.events.add(global.renderName ["5s"], () => {
	try {
		if (!global.loggedin) return;

		// Отключаем IDLE камеру когда курсор активен или игрок мёртв
		if(mp.gui.cursor.visible || global.deathTimerOn) {
			mp.game.invoke("0xF4F2C0D4EE209E20");
		}
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", global.renderName ["5s"], e.toString());
	}
});
gm.events.add("showpersonid", (personid) =>{
	mp.gui.emmit(`window.serverStore.UUID(${parseInt(personid)})`);
})
gm.events.add("render", () => {
	if ((!global.chatActive && global.menuOpened && !global.phoneStatus && !global.isPopup && !global.deathTimerOn && !global.startedMining && !global.isPhoneOpen) || global.bigMapStatus === 3)
		Natives.HIDE_HUD_AND_RADAR_THIS_FRAME ();

	if (!global.loggedin) return;

	// Устанавливаем GTA интерфейсам цвет проекта
	mp.game.ui.setHudColour(143, 228,66,66,255);
	mp.game.ui.setHudColour(144, 228,66,66,255);
	mp.game.ui.setHudColour(145, 228,66,66,255);

	// Отключаем не нужные HUD компоненты
	mp.game.ui.hideHudComponentThisFrame(2); // HUD_WEAPON_ICON
	mp.game.ui.hideHudComponentThisFrame(3); // HUD_CASH
	mp.game.ui.hideHudComponentThisFrame(6); // HUD_VEHICLE_NAME
	mp.game.ui.hideHudComponentThisFrame(7); // HUD_AREA_NAME
	mp.game.ui.hideHudComponentThisFrame(8); // HUD_VEHICLE_CLASS
	mp.game.ui.hideHudComponentThisFrame(9); // HUD_STREET_NAME

	mp.game.ui.hideHudComponentThisFrame(19); // HUD_WEAPON_WHEEL
	mp.game.ui.hideHudComponentThisFrame(20); // HUD_WEAPON_WHEEL_STATS
	mp.game.ui.hideHudComponentThisFrame(22); // MAX_HUD_WEAPONS
});

let FixTrailer = null;
const AttachTrailerToVehicle = (vehicle) => {
	if (!vehicle)
		return;
	else if (!mp.vehicles.exists(vehicle))
		return;
	if (!FixTrailer && Natives.IS_VEHICLE_ATTACHED_TO_TRAILER (vehicle.handle)) {
		let trailer = vehicle.getTrailer(0);
		if (!trailer)
			return;
		trailer = mp.vehicles.atHandle(trailer);
		if (trailer && mp.vehicles.exists(trailer)) {
			FixTrailer = trailer;
			//mp.gui.chat.push(`AttachTrailerToVehicle - ${trailer.remoteId}`);
			Natives.DETACH_VEHICLE_FROM_TRAILER(vehicle.handle);
			Natives.ATTACH_VEHICLE_TO_TRAILER(vehicle.handle, trailer.handle, 1000);
			//vehicle.attachToTrailer(trailer.handle, 1000);
		}
	} else if (FixTrailer && !Natives.IS_VEHICLE_ATTACHED_TO_TRAILER (vehicle.handle) && mp.vehicles.exists(FixTrailer)) {
		let trailer = vehicle.getTrailer(0);
		let trailerEntity = null;
		if (trailer)
			trailerEntity = mp.vehicles.atHandle(trailer);
		if (!trailer || !trailerEntity || trailerEntity != FixTrailer) {
			//mp.gui.chat.push(`AttachTrailerToVehicle - ${FixTrailer.remoteId}`);
			//vehicle.attachToTrailer(FixTrailer.handle, 1000);
			Natives.ATTACH_VEHICLE_TO_TRAILER(vehicle.handle, FixTrailer.handle, 1000);
		}
	}
}






let isAttach = -1;

let isUpdateTrailerData = new Date().getTime();

const AttachTrailer = (vehicle) => {
	if (isUpdateTrailerData > new Date().getTime())
		return;
	else if (isAttach)
		return;
	
	mp.gui.chat.push(`AttachTrailer `);
	isUpdateTrailerData = new Date().getTime() + (1000 * 20);

	let trailer = vehicle.getTrailer(0);
	if (!trailer)
		return;
	trailer = mp.vehicles.atHandle(trailer);
	mp.gui.chat.push(`AttachTrailer2 = ` + trailer);
	if (trailer && mp.vehicles.exists(trailer)) {
		isAttach = true;
		//Natives.DETACH_VEHICLE_FROM_TRAILER(vehicle.handle);
		//Natives.ATTACH_VEHICLE_TO_TRAILER(vehicle.handle, trailer.handle, 1000);
		callRemote("VehicleTrailerAttach", vehicle, trailer);
	}
	/*mp.vehicles.forEachInStreamRange(async (trailer) => {
		if (!isAttach && vehicle && mp.vehicles.exists(vehicle) && mp.vehicles.exists(trailer) && trailer.handle !== 0 && vehicle.handle !== trailer.handle && Natives.GET_VEHICLE_TRAILER_VEHICLE (vehicle.handle, trailer.handle)) {
			//isAttach = true;
			mp.gui.chat.push(`AttachTrailer3 - ${trailer.remoteId} - ${vehicle.getTrailer(0)}`);
			//callRemote("VehicleTrailerAttach", vehicle, trailer);
		}
	});*/
}

const DeattachTrailer = (vehicle) => {
	if (isUpdateTrailerData > new Date().getTime())
		return;
	else if (!isAttach)
		return;
	isAttach = false;
	callRemote("VehicleTrailerDeattach", vehicle);
}
/*
const SetTrailer = (entity, data) => {
	try 
	{		
		if (entity && mp.players.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0) {
			if (!data) data = entity.getVariable("Trailer");
			
			mp.gui.chat.push("SetTrailer1 - " + data);
		}
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["synchronization/clothes2"] < 5000) return;
		global.trycatchtime["synchronization/clothes2"] = new Date().getTime();
		callRemote("client_trycatch", "synchronization/clothes", "setProps", e.toString());
	}
}

mp.events.addDataHandler("Trailer", (entity, value, oldValue) => {
	mp.gui.chat.push("Trailer - " + value);
    SetTrailer (entity, value);
});

gm.events.add('entityStreamIn', (entity) => {
	SetTrailer (entity, null);
});*/
mp.events.add('updateSpeedFrames', (frames, delay) => {
  if (typeof window.vehicleState.speedFrames === 'function') {
    window.vehicleState.speedFrames(frames);
  }
  if (typeof window.vehicleState.delay === 'function') {
    window.vehicleState.delay(delay);
  }
});
const onEnterVehicle = (vehicle) => new Promise(async (resolve, reject) => {
    try {
        if (mp.vehicles.exists(vehicle) && global.localplayer.isInAnyVehicle(false) && vehicle.getPedInSeat(-1) == global.localplayer.handle)
            return resolve(true);
        let d = 0;
        while (!(mp.vehicles.exists(vehicle) && global.localplayer.isInAnyVehicle(false) && vehicle.getPedInSeat(-1) == global.localplayer.handle)) {
            if (d > 35) return resolve(translateText("Ошибка onEnterVehicle."));
            d++;
            await global.wait (150);
        }
        return resolve(true);
    } 
    catch (e) 
    {
        callRemote("client_trycatch", "player/hud", "onEnterVehicle", e.toString());
        resolve();
    }
});

const ShowSpeed = (vehicle) => {
	if (vehicle && mp.vehicles.exists(vehicle) && global.localplayer.isInAnyVehicle(false) && vehicle.getPedInSeat(-1) == global.localplayer.handle && !global.hudstatus.invehicle) {
		global.getVehicleIndicatorsInfo (vehicle);
		FixTrailer = null;
		isAttach = -1;

		if (vehicle.model == 1747439474) vehicle.setMaxSpeed(30);
		else if (vehicle.model == 2034235290) vehicle.setMaxSpeed(40);

		mp.gui.emmit(`window.hudStore.inVehicle (true)`);	

		mp.gui.emmit(`window.vehicleState.maxSpeed(${(mp.game.vehicle.getVehicleModelMaxSpeed(vehicle.model) * 3.6).toFixed()})`);

		global.hudstatus.invehicle = true;
		
		global.Petrol = Number(vehicle.getVariable('PETROL'));
		mp.gui.emmit(`window.vehicleState.fuel(${global.Petrol}, ${vehicle.getClass()})`);
	}
}

gm.events.add("playerLeaveVehicle", (entity) => {
	lastLightsState = -1;
    nativeCheckAttempts = 0;
	CloseVehicleSpeed ();
});

const CloseVehicleSpeed = () => {
	if (global.hudstatus.invehicle) {
		mp.gui.emmit(`window.hudStore.inVehicle (false)`);
		global.localplayer.setConfigFlag (32, true);
		global.Petrol = 0;
lastLightsState = -1;
		global.hudstatus.invehicle = false;
	}
}

let safeZone = null;
let aspectRatio = null;

const getSafeZona = () => {
	const size = mp.game.graphics.getSafeZoneSize();
	const ratio = mp.game.graphics.getScreenAspectRatio(false);
	if(size != safeZone || ratio != aspectRatio) {
		const resolution = mp.game.graphics.getScreenActiveResolution(0,0)
		safeZone = size;
		aspectRatio = ratio;
		mp.gui.emmit(`window.hud.updateSafeZone(${resolution.x}, ${resolution.y}, ${safeZone}, ${aspectRatio})`);
		getMinimapAnchor ();
	}
}

gm.events.add("client:OnBrowserInit", () => {
	getMinimapAnchor ();
	getSafeZona ();
});

global.bigMapStatus = 0;

mp.game.ui.setRadarZoom(1.0);
mp.game.ui.setRadarBigmapEnabled(false, false);

mp.keys.bind(global.Keys.VK_Z, false, function () {
	try 
	{
		if (!global.loggedin || mp.gui.cursor.visible || global.chatActive || global.menuCheck()) return;
		if (global.bigMapStatus === 0) {
			mp.game.ui.setRadarZoom(0.0);
			global.bigMapStatus = 1;			
		} else if (global.bigMapStatus === 1) {
			mp.game.ui.setRadarBigmapEnabled(true, false);
			mp.game.ui.setRadarZoom(0.0);
			global.bigMapStatus = 2;
		} else if (global.bigMapStatus === 2) {
			global.bigMapStatus = 3;
		} else {
			mp.game.ui.setRadarBigmapEnabled(false, false);
			mp.game.ui.setRadarZoom(1.0);
			global.bigMapStatus = 0;			
		}
				
		getMinimapAnchor ();
    } 
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "VK_Z", e.toString());
	}
});

let mapSize = null;

const getMinimapAnchor = () => {
	try {
		const sfX = 1.0 / 20.0;
		const sfY = 1.0 / 20.0;
		const size = 1;//mp.game.graphics.getSafeZoneSize();
		const aspectRatio = mp.game.graphics.getScreenAspectRatio(false);
		const resolution = mp.game.graphics.getScreenActiveResolution(0, 0);
		const scaleX = 1.0 / resolution.x;
		const scaleY = 1.0 / resolution.y;

		const minimap = {
			width: scaleX * (resolution.x / ((global.bigMapStatus == 2 ? 2.53 : 4) * aspectRatio)),
			height: scaleY * (resolution.y / 5.674),
			scaleX: scaleX,
			scaleY: scaleY,
			leftX: scaleX * (resolution.x * (sfX * (Math.abs(size - 1.0) * 10))),
			bottomY: 1.0 - scaleY * (resolution.y * (sfY * (Math.abs(size - 1.0) * 10))),
		};

		minimap.rightX = minimap.leftX + minimap.width;
		minimap.topY = minimap.bottomY - minimap.height;
		
		minimap.width = minimap.rightX * resolution.x;

		if (global.bigMapStatus == 3) minimap.width = 0;
		mp.gui.emmit(`window.hud.updateMapWidth(${minimap.width})`);
		//mp.gui.emmit(`window.hud.updateMapSize(${global.bigMapStatus})`);
		return;
	} 
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "getMinimapAnchor", e.toString());
	}
}
global.isEnter = null;
gm.events.add('hud.oEnter', function (text) {
	try 
	{
		global.isEnter = text;
		mp.gui.emmit(`window.hudEnter('${text}')`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "hud.oEnter", e.toString());
	}
});

gm.events.add('hud.cEnter', function () {
	try 
	{
		global.isEnter = null;
		mp.gui.emmit(`window.hudEnter(-1)`);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "player/hud", "hud.cEnter", e.toString());
	}
});

gm.events.add("hud.tip", (dict, name) => {
	mp.gui.emmit(`window.showTip ('${dict}', '${name}')`);
});

const getKillListColorData = (player) => {
	try
	{
		let killerColor = "#DFDFDF";

		if (player['PlayerAirsoftTeam'] >= 0)
			killerColor = "#DFDFDF";

		const fractionId = player['fraction'];
		
		if (fractionId == 1) killerColor = "#228B22";
		else if (fractionId == 2) killerColor = "#BA55D3";
		else if (fractionId == 3) killerColor = "#F0E68C";
		else if (fractionId == 4) killerColor = "#4169E1";
		else if (fractionId == 5) killerColor = "#CC3333";
		else if (fractionId == 6) killerColor = "#ADFF2F";
		else if (fractionId == 7) killerColor = "#0000FF";
		else if (fractionId == 8) killerColor = "#FF3333";
		else if (fractionId == 9) killerColor = "#0000FF";
		else if (fractionId == 10) killerColor = "#CC9900";
		else if (fractionId == 11) killerColor = "#87CEEB";
		else if (fractionId == 12) killerColor = "#8B0000";
		else if (fractionId == 13) killerColor = "#A9A9A9";
		else if (fractionId == 14) killerColor = "#8B4513";
		else if (fractionId == 15) killerColor = "#FF9900";
		else if (fractionId == 18) killerColor = "#0000FF";
	
		return killerColor;
	}
	catch (e)
	{
		return "#DFDFDF";
	}
}

const weaponIcom = {
	[100416529]: 'inv-item-Sniper-Rifle',
	[125959754]: '',
	[126349499]: 'inv-item-marijuana',
	[137902532]: 'inv-item-Vintage-Pistol',
	[171789620]: 'inv-item-Combat-PDW',
	[177293209]: 'inv-item-Heavy-Sniper-Mk-II',
	[205991906]: 'inv-item-Heavy-Sniper',
	[317205821]: 'inv-item-Sweeper-Shotgun',
	[324215364]: 'inv-item-Micro-SMG',
	[419712736]: 'inv-item-Pipe-Wrench',
	[453432689]: 'inv-item-Pistol',
	[487013001]: 'inv-item-Pump-Shotgun',
	[584646201]: 'inv-item-AP-Pistol',
	[727643628]: 'inv-item-Combat-Pistol',
	[736523883]: 'inv-item-SMG',
	[883325847]: 'inv-item-gasoline',
	[911657153]: 'inv-item-Stun-Gun',
	[961495388]: 'inv-item-Assault-Rifle-Mk-II',
	[984333226]: 'inv-item-Heavy-Shotgun',
	[1119849093]: '',
	[1141786504]: 'inv-item-Golf-Club',
	[1198256469]: '',
	[1198879012]: 'inv-item-Flare-Gun',
	[1233104067]: 'inv-item-Flare-Gun',
	[1305664598]: '',
	[1317494643]: 'inv-item-Hammer',
	[1432025498]: 'inv-item-Pump-Shotgun-Mk-II',
	[1593441988]: 'inv-item-Pistol',
	[1627465347]: 'inv-item-Gusenberg-Sweeper',
	[1649403952]: 'inv-item-Compact-Rifle',
	[1672152130]: '',
	[1737195953]: 'inv-item-Nightstick',
	[1785463520]: 'inv-item-Marksman-Rifle-Mk-II',
	[1834241177]: '',
	[2017895192]: 'inv-item-Sawed-Off-Shotgun',
	[2024373456]: 'inv-item-SMG-Mk-II',
	[2132975508]: 'inv-item-Bullpup-Rifle',
	[2138347493]: '',
	[2144741730]: 'inv-item-Combat-MG',
	[-2084633992]: 'inv-item-Carbine-Rifle',
	[-2067956739]: 'inv-item-crowbar',
	[-2066285827]: 'inv-item-Bullpup-Rifle-Mk-II',
	[-2009644972]: 'inv-item-SNS-Pistol-Mk-II',
	[-1951375401]: 'inv-item-Flashlight',
	[-1853920116]: 'inv-item-Heavy-Revolver-Mk-II',
	[-1834847097]: 'inv-item-Antique-Cavalry-Dagger',
	[-1810795771]: 'inv-item-Pool-Cue',
	[-1786099057]: 'inv-item-Baseball-Bat',
	[-1768145561]: 'inv-item-Special-Carbine-Mk-II',
	[-1746263880]: 'inv-item-Double-Action-Revolver',
	[-1716589765]: 'inv-item-Pistol-50',
	[-1716189206]: 'inv-item-Knife',
	[-1660422300]: 'inv-item-MG',
	[-1654528753]: 'inv-item-Bullpup-Shotgun',
	[-1568386805]: '',
	[-1466123874]: 'inv-item-Musket',
	[-1357824103]: 'inv-item-Advanced-Rifle',
	[-1355376991]: 'inv-item-Vintage-Pistol',
	[-1312131151]: '',
	[-1121678507]: 'inv-item-Mini-SMG',
	[-1076751822]: 'inv-item-SNS-Pistol',
	[-1075685676]: 'inv-item-Pistol-Mk-II',
	[-1074790547]: 'inv-item-Assault-Rifle',
	[-1063057011]: 'inv-item-Special-Carbine',
	[-1045183535]: 'inv-item-Heavy-Revolver',
	[-952879014]: 'inv-item-Marksman-Rifle',
	[-879347409]: 'inv-item-Heavy-Revolver-Mk-II',
	[-853065399]: 'inv-item-Battle-Axe',
	[-771403250]: 'inv-item-Heavy-Pistol',
	[-656458692]: 'inv-item-Brass-Knuckles',
	[-619010992]: 'inv-item-Machine-Pistol',
	[-608341376]: 'inv-item-Combat-MG-Mk-II',
	[-598887786]: 'inv-item-Marksman-Pistol',
	[-581044007]: 'inv-item-Machete',
	[-538741184]: 'inv-item-Switchblade',
	[-494615257]: 'inv-item-Assault-Shotgun',
	[-275439685]: 'inv-item-Double-Barrel-Shotgun',
	[-270015777]: 'inv-item-Assault-SMG',
	[-102973651]: 'inv-item-Hatchet',
	[-102323637]: 'inv-item-Broken-Bottle',
	[-86904375]: 'inv-item-Carbine-Rifle-Mk-II',
}

gm.events.add("hud.kill", (killerId, victimId, weapon) => {
	let killer = mp.players.atRemoteId(killerId);
	let killerName = ""
	let killerColor = ""
	if (killer && mp.players.exists(killer)) {
		killerName = `${killer.name}(${killerId})`;
		killerColor = getKillListColorData (killer);
	}

	let victim = mp.players.atRemoteId(victimId);
	let victimName = ""
	let victimColor = ""
	if (victim && mp.players.exists(victim)) {
		victimName = `${victim.name}(${victimId})`;
		victimColor = getKillListColorData (victim);
	}
	mp.gui.emmit(`window.kill.show ('${killerName}', '${weaponIcom [weapon] ? weaponIcom [weapon] : ""}', '${victimName}', '${killerColor}', '${victimColor}')`);
});

gm.events.add("hud.kill.clear", () => {
	mp.gui.emmit(`window.kill.clear ()`);
});

let frameCount = Natives.GET_FRAME_COUNT ();
let gameTimer = Natives.GET_GAME_TIMER ();
let _frameCount = frameCount;
let _gameTimer = gameTimer;
let FPS = 0;

global.GetFps = () => {
	_frameCount = Natives.GET_FRAME_COUNT ();
	_gameTimer = Natives.GET_GAME_TIMER ();
	if (_gameTimer - gameTimer > 1000) {
		FPS = _frameCount - frameCount - 1;
		frameCount = _frameCount;
		gameTimer = _gameTimer;
	}
	mp.game.graphics.drawText("FPS: " + FPS, [0.5, 0.9], {
		scale: [0.35, 0.35],
		outline: true,
		color: [255, 255, 255, 185],
		font: 0
	});
}











///

global.notifyCount = 2;

class Notification {
	constructor(type, title, text, timeout) {
		this.type = type;
		this.title = title;
		this.elements = text;
		this.timeout = timeout;
		this.endtime = -1;
		this.percent = 100;
	}
}

let notifications = [];

const AddNotification = (type, title, text, timeout) => {
	const index = notifications.findIndex(notify => notify.title === title && notify.text === text);
	if (index !== -1) return;
	notifications.push(new Notification(type, title, text, timeout));
}

gm.events.add('notifyClear', (index) => {
	SkipNotification (index, false)
});

const SkipNotification = (index, clearAll) => {
	if (clearAll && notifications.length > 0) {
		notifications = [];
		UpdateNotification ();
	} else if (typeof notifications[0] === "object") {
		notifications.splice(index, 1);
		UpdateNotification ();
	}
}

const UpdateNotification = () => {
	if (typeof notifications[0] === "object") {
		const count = global.notifyCount <= 1 ? 2 : global.notifyCount;
		const notifys = Array.from(notifications);
		mp.gui.emmit(`window.notification('${JSON.stringify(notifys.splice(0, count))}')`);
	} else
		mp.gui.emmit(`window.notification(false)`);
}

const Tick = () => {
	if (notifications.length > 0) {
		const count = global.notifyCount <= 1 ? 2 : global.notifyCount;

		for (let i = 0; i < count; i++) {
			if (typeof notifications[i] === "object") {
				const now = new Date().getTime();
				const notify = notifications[i];

				if(notify.endtime === -1) {
					notify.endtime = new Date().getTime() + notify.timeout;
					UpdateNotification ();
				} else if (notifications[i].percent > 0) {
					notifications[i].percent = 0;
					UpdateNotification ();
				}

				if(notify !== undefined && now > notify.endtime) {
					notifications.splice(i, 1);
					UpdateNotification ();
				}
			}
		}
	}
}

gm.events.add(global.renderName ["150ms"], () => {
	Tick ();
});

const types = ['information', 'error', 'success', 'information', 'error'];

gm.events.add('notify', (type, _, msg, time) => {
	AddNotification (types [type], translateText("Уведомление"), msg, time);
});

gm.events.add('notifyToKey', (type, layout, msg, time, index) => {
	AddNotification (types [type], translateText("Уведомление"), msg.replace(/!_!/g, global.Keys[global.userBinder[index].keyCode]), time);
});

gm.events.add('vehicleNotify', (type, msg, time) => {
	// Перенаправляем в check.svelte через window.listernEvent
	mp.gui.emmit(`window.listernEvent.vehicleNotify(${type}, "${msg}", ${time})`);
});

let dellNotificationTime = null;

global.binderFunctions.dellNotification = () => {
	if (notifications.length > 0) {
		if (dellNotificationTime !== null) {
			clearTimeout(dellNotificationTime);
			dellNotificationTime = null;
			SkipNotification(0, true);
		} else {
			SkipNotification(0, false);
			dellNotificationTime = setTimeout(() => {
				dellNotificationTime = null;
			}, 250);
		}
	}
};

gm.events.add("hud.event", async (_subTitle, _title, _desc, _image, timeWait) => {

	//nativeInvoke ("_START_SCREEN_EFFECT", 'MP_SmugglerCheckpoint', 2000, true);
	//mp.game.audio.playSoundFrontend(-1, "Zone_Team_Capture", "DLC_Apartments_Drop_Zone_Sounds", true);
	mp.gui.emmit(`window.listernEvent ('hud.event', true, '${_subTitle}', '${_title}', '${_desc}', '${_image}');`);
	await global.wait(timeWait);
	mp.gui.emmit(`window.listernEvent ('hud.event', false);`);
});

gm.events.add("hud.event.cool", async (_subTitle, _title, _desc, _image, timeWait) => {

	//nativeInvoke ("_START_SCREEN_EFFECT", 'MP_SmugglerCheckpoint', 2000, true);
	//mp.game.audio.playSoundFrontend(-1, "Zone_Team_Capture", "DLC_Apartments_Drop_Zone_Sounds", true);
	mp.gui.emmit(`window.listernEvent ('hud.event', true, '${_subTitle}', '${_title}', '${_desc}', '${_image}');`);
	await global.wait(timeWait);
	mp.gui.emmit(`window.listernEvent ('hud.event', false);`);
});

