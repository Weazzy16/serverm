const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;

global.gamemenu = false;
global.myStats = false;


global.binderFunctions.GTA5DEVMENU = () => {
	if (global.tableInFocus)
		return;

    if (!global.gamemenu) GTA5DEVMENU ();
};


const GTA5DEVMENU = () => {
	try
	{
		if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || global.menuCheck() || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0)) return;
		if (!global.myStats) callRemote('server.gamemenu.updatestats');   
		mp.gui.emmit(`window.router.setView("Gta5devMenu");`);
		call("client.battlepass.open");
		call("client.phone.cars.load");
		call("client.gta5devmenucars"); 
		global.gamemenu = true;
		global.menuOpen(true);
	}
	catch (e) 
	{
		callRemote("client_trycatch", "inventory/index", "GTA5DEVMENU", e.toString());
	}
}

mp.events.add("client.stockitems", () => {
	callRemote('server.gta5devmenustock', 0)
})



gm.events.add("client.inventory.InitOtherDataStock", (otherId, otherName, jsonString, maxSlots) => {
	// JSON.stringify гарантированно экранирует всё, что угодно
	const jsJson = JSON.stringify(jsonString);
	// передаём уже валидный JS-литерал
	mp.gui.emmit(`window['client.inventory.InitOtherDataStock'](
	  ${otherId},
	  ${JSON.stringify(otherName)},
	  ${jsJson},
	  ${maxSlots}
	)`);
  });



  
  // При получении от C# списка — зовём Svelte-хендлер:
// вместо вашего current-кода

  
  
  
mp.events.add("client.takeFromStock", (slotIndex, itemId) => {
    // slotIndex и itemId пришли из Svelte (executeClient)
    callRemote('server.takeFromStock', slotIndex, itemId);
});

// 1) CEF → RAGE-MP: пересылаем запрос на сервер
mp.events.add("requestBanHistory", (uuid) => {
    callRemote("requestBanHistory", uuid);
});

// 2) RAGE-MP → CEF: получаем JSON-строку и шлём в браузер как CustomEvent
mp.events.add("showBanHistory", (jsonString) => {
    // Здесь используем именно mp.gui.emmit, чтобы код попал в CEF-контекст
    mp.gui.emmit(`
        window.dispatchEvent(new CustomEvent(
            'showBanHistory',
            { detail: JSON.parse('${jsonString.replace(/\\/g, "\\\\").replace(/'/g, "\\'")}') }
        ));
    `);
});

mp.events.add("requestBanCount", (uuid) => {
    callRemote("requestBanCount", uuid);
});

// когда сервер шлёт ответ count
mp.events.add("showBanCount", (count) => {
    mp.gui.emmit(`
      window.dispatchEvent(new CustomEvent(
        'showBanCount',
        { detail: ${count} }
      ));
    `);
});

// **CEF → JS**: ловим сигнал из Svelte



// Сервер → клиент: пришёл таймер в секундах
// 1) CEF → RAGE-MP: клиент попросил текущее значение таймера
mp.events.add("requestPaydayTimer", () => {
  callRemote("requestPaydayTimer");
});

// 2) RAGE-MP → CEF: сервер прислал уже отыгранные секунды
mp.events.add("updatePaydayTimer", (seconds) => {
  console.log("[Client] updatePaydayTimer:", seconds);
  // форвардим в CEF как CustomEvent
  mp.gui.emmit(`
    window.dispatchEvent(new CustomEvent(
      'updatePaydayTimer',
      { detail: ${seconds} }
    ));
  `);
});

