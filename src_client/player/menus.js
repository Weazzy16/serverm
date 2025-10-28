const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
global.menuOpened = true;
let timeMenuPause = 0;

global.menuCheck = () => {
    if (global.menuOpened || mp.game.ui.isPauseMenuActive() || global.isBind || global.isSartMetro/* || timeMenuPause > Date.now()*/) return true;
    return false;
};

global.awaitMenuCheck = () => new Promise(async (resolve, reject) => {
    try {
        if (!global.menuCheck())
            return resolve();
        let d = 0;
        while (global.menuCheck()) {
            if (d > 5000) return resolve(translateText("Ошибка awaitMenuCheck."));
            d++;
            await global.wait (0);
        }
        return resolve();
    } 
    catch (e) 
    {
		callRemote("client_trycatch", "player/menus", "render", e.toString());
        resolve();
    }
});

let isControls = false;
global.menuOpen = function (_isControls = false) {
    mp.gui.cursor.visible = true;

    if (_isControls)
        mp.gui.cursor.show(false, true);

    isControls = _isControls;
    global.menuOpened = true;

    //call('notify', 4, 9, translateText("Слишком быстро"), 3000);
}

global.menuClose = function () {
    mp.gui.cursor.show(true, true);

    mp.gui.cursor.visible = false;
    global.discordDefault ()
    global.menuOpened = false;
    isControls = false;
    global.isPopup = false;
}
global.menuOpenCustom = function () {
  
}

global.menuCloseCustom = function () {
    
}
/*
gm.events.add(global.renderName ["1s"], () => {
    if (mp.gui.cursor.visible && mp.game.ui.isPauseMenuActive()) {
        mp.gui.cursor.show(true, true);
        mp.gui.cursor.visible = false;
        mp.game.ui.setPauseMenuActive(false);

        mp.gui.emmit(`console.log ('renderName - ${mp.gui.cursor.visible} - ${mp.game.ui.isPauseMenuActive()}')`);
    }
});
*/

gm.events.add("render", () => {
    if (global.menuOpened && mp.game.ui.isPauseMenuActive())
        mp.game.ui.setPauseMenuActive(false);
    else if (global.ANTIANIM)
        mp.game.controls.disableControlAction(0, global.Inputs.LOOK_BEHIND, true);

    if (isControls) {
        mp.game.controls.disableAllControlActions(0);

        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_LR, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_UD, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_UP_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_DOWN_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_LEFT_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_RIGHT_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.JUMP, true);

        mp.game.controls.enableControlAction(0, global.Inputs.VEH_MOVE_LR, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_MOVE_UD, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_ACCELERATE, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_BRAKE, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_HANDBRAKE, true);

        if (mp.keys.isDown(global.Keys.VK_RBUTTON)) {
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LR, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UD, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UP_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_DOWN_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LEFT_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_RIGHT_ONLY, true);
        }
    }
});

/*gm.events.add("playerQuit", (player, exitType, reason) => {
    if (player.name === global.localplayer.name) {
        global.menuClose();
    }
});*/
// var UserMenu = browsers.new('http://package/interface/modules/UserMenu/index.html');

// var openedUserMenu = false;

// //===========================================================
// mp.keys.bind(Keys.VK_F2, false, () => {
// 	if (global.menuCheck() || !global.loggedin || global.menuOpened || global.cuffed || global.chatActive || global.editing) return;
// 	if(!openedUserMenu) callRemote("server::playermenu:open");
//   else call("client::playermenu:close")
// });

// gm.events.add('client::playermenu:open', (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w) => {
//   if (global.menuCheck() || global.menuOpened || global.cuffed || global.chatActive || global.editing) return;
//   mp.console.logInfo(`${JSON.parse(w)}`);
// 	UserMenu.execute(`UserMenu.Opened = true; UserMenu.PlayerData.CashMoney = ${a}; UserMenu.PlayerData.BankMoney = ${b}; UserMenu.PlayerData.RegisterDate = '${c}'; UserMenu.PlayerData.Level.Current = ${d}; UserMenu.PlayerData.Level.Exp = ${e}; UserMenu.PlayerData.TodayTime = '${f}'; UserMenu.PlayerData.House.Name = '${g}'; UserMenu.PlayerData.Organization.Name = '${h}'; UserMenu.PlayerData.Organization.NameRank = '${i}'; UserMenu.PlayerData.Work.Name = '${j}'; UserMenu.PlayerData.Work.NameRank = '${j}'; UserMenu.PlayerData.Premium.Name = '${k}'; UserMenu.PlayerData.Premium.UntilDate = '${l}'; UserMenu.OrleanPoints = '${m}'; UserMenu.Login = '${n}';UserMenu.AdminLVL = '${q}';UserMenu.StaticID = '${r}';UserMenu.PlayerData.MoneyLog.Earned = '${u}'; UserMenu.Vehicles=${w};`); 
//   mp.gui.emmit(`window.router.close()`);  
//   openedUserMenu = true;
// 	global.menuOpen(false);
//     call("showHUD", false);
// });

// const getJobSkillsInfo = (skillsData, nextLevelsData, currentLevelsInfo) => {
// 	if (!skillsData || !nextLevelsData || !currentLevelsInfo)
// 		return;

// 	if (skillsData.length < 1) return;

// 	let jobSkillsInfo = [
// 		{
// 			name: translateText("Электрик"),
// 			max: 15000,
// 			nextLevel: 700,
// 			currentLevel: 0,
// 			current: 0
// 		},
// 		{
// 			name: translateText("Газонокосильщик"),
// 			max: 40000,
// 			nextLevel: 2000,
// 			currentLevel: 0,
// 			current: 0
// 		},
// 		{
// 			name: translateText("Почтальон"),
// 			max: 4000,
// 			nextLevel: 200,
// 			currentLevel: 0,
// 			current: 0
// 		},
// 		{
// 			name: translateText("Таксист"),
// 			max: 1000,
// 			nextLevel: 25,
// 			currentLevel: 0,
// 			current: 0
// 		},
// 		{
// 			name: translateText("Водитель автобуса"),
// 			max: 70000,
// 			nextLevel: 3000,
// 			currentLevel: 0,
// 			current: 0
// 		},
// 		{
// 			name: translateText("Автомеханик"),
// 			max: 250,
// 			nextLevel: 10,
// 			currentLevel: 0,
// 			current: 0
// 		},
// 		{
// 			name: translateText("Дальнобойщик"),
// 			max: 700,
// 			nextLevel: 30,
// 			currentLevel: 0,
// 			current: 0
// 		},
// 		{
// 			name: translateText("Инкассатор"),
// 			max: 3000,
// 			nextLevel: 150,
// 			currentLevel: 0,
// 			current: 0
// 		}
// 	];

// 	for (let i = 0; i < jobSkillsInfo.length; i++) {
// 		if (skillsData[i] !== undefined) {
// 			jobSkillsInfo[i].current = skillsData[i];
// 		}

// 		if (nextLevelsData[i] !== undefined) {
// 			jobSkillsInfo[i].nextLevel = nextLevelsData[i];
// 		}

// 		if (currentLevelsInfo[i] !== undefined) {
// 			jobSkillsInfo[i].currentLevel = currentLevelsInfo[i];
// 		}
// 	}

// 	return jobSkillsInfo;
// }

// const getStatsData = (json) => {
// 	json = JSON.parse (json);

// 	return {
// 		Login: json[0],
// 		VipLvl: json[1],
// 		VipDate: json[2],
// 		Warns: json[3],
// 		Unwarn: json[4],
// 		TodayTime: json[5],
// 		MonthTime: json[6],
// 		YearTime: json[7],
// 		TotalTime: json[8],
// 		//
// 		jobSkillsInfo: getJobSkillsInfo (json[9], json[10], json[11]),
// 		//
// 		Name: json[12],
// 		isAdmin: json[13],
// 		WeddingName: json[14],
// 		Gender: json[15],
// 		LVL: json[16],
// 		EXP: json[17],
// 		Sim: json[18],
// 		WorkID: json[19],
// 		FractionID: json[20],
// 		FractionLVL: json[21],
// 		OrganizationID: json[22],
// 		OrganizationLVL: json[23],
// 		UUID: json[24],
// 		Bank: json[25],
// 		BankMoney: json[26],
// 		Money: json[27],
// 		CreateDate: json[28],
// 		//
// 		houseId: json[29],
// 		houseType: json[30],
// 		houseCash: json[31],
// 		houseCopiesHour: json[32],
// 		housePaid: json[33],
// 		maxcars: json[34],
// 		//
// 		BizId: json[35],
// 		BizCash: json[36],
// 		BizCopiesHour: json[37],
// 		BizPaid: json[38],
// 		//
// 		Licenses: json[39],
// 		Wanted: json[40],
// 	}
// }

// gm.events.add('client::playermenu:data', (json) => {
//   const characterData = getStatsData (json);
//   mp.console.logInfo(`${JSON.stringify(characterData)}`);
//   UserMenu.execute(`UserMenu.OtherData('${JSON.stringify(characterData)}')`);
// })

// gm.events.add("client::donate:buyClothes", (model) => {
//     callRemote("server::donate:buyClothes", model)
// });

// gm.events.add('client::playermenu:close', () => {
// 	UserMenu.execute(`UserMenu.Opened = false; UserMenu.Reset();`);
//   mp.gui.emmit(`window.router.setHud();`);
//     openedUserMenu = false;
// 	global.menuClose(false);
//     call("showHUD", true);
// });

// gm.events.add('cnslg', (id) => {
//   mp.console.logInfo(`${id}`);
// });

// gm.events.add('addreport', (id, author, authorId, messages, openeddate, status, dId) => {
//     UserMenu.execute(`UserMenu.ReportAdd(${id},'${author}',${authorId},${messages},'${openeddate}','${status}',${dId})`);
//     mp.console.logInfo("added");
//   });
  
//   gm.events.add("client::sounds:playSoundAdmin", () => {
    
//   })
  
//   gm.events.add("deletereportfromlist", (id) => {
//     UserMenu.execute(`UserMenu.ReportDelete(${id})`);
//   });
  
//   gm.events.add('updreport', (id, author, authorId, messages, openeddate, status, authordId) => {
//     UserMenu.execute(`UserMenu.UpdReport(${id},'${author}',${authorId},${messages},'${openeddate}','${status}',${authordId})`);
    
//     mp.console.logInfo("added");
//   })
  
//   gm.events.add('client::report:slice', (id) => {
//     UserMenu.execute(`UserMenu.SliceReport(${id})`);
//   })
  
//   gm.events.add("client::report:close", (reportId) => {
//     callRemote("server::report:close", reportId);
//   })
  
//   gm.events.add("takereportsuc", (id) => {
//     UserMenu.execute(`UserMenu.ReportTake(${id})`);
//   });
  
//   gm.events.add("sendmsgtorep", (msg, reportId) => {
//     callRemote("sendmsgtoreport", msg, reportId);
//   })
  
//   gm.events.add('setreport', (id, name) => {
//     report.execute(`setStatus(${id}, '${name}')`);
//   })
//   gm.events.add('delreport', (id) => {
//     report.execute(`deleteReport(${id})`);
//   })
//   gm.events.add('client::spectateOnReport', (id) => {
//   callRemote('sever::spectateOnReport', id);
//   });
  
//   gm.events.add("cl::report:take", (reportId) => {
//     callRemote("server::report:take", reportId);
//   });
  
//   gm.events.add('client::teleportOnReport', (id) => {
//   callRemote('sever::teleportOnReport', id);
//   });
//   gm.events.add('takereport', (id, r) => {
//     callRemote('takereport', id, r);
//   })
//   gm.events.add('sendreport', (id, a) => {  
//     callRemote('sendreport', id, a);
//   })
//   gm.events.add('exitreport', () => {
//   global.menuClose();
//   reportactive = false;
//     mp.gui.cursor.visible = false;
//   })

// gm.events.add('client::playermenu:update', (key, value) => {
//   UserMenu.execute(`UserMenu.${key} = ${value}`);
// });

// gm.events.add('client::promocode:logic', (a,b) => {
//   callRemote('server::promocode:logic', a,b);
// });

// gm.events.add("client::donate:buyShark", (coins) => {
//   callRemote("server::donate:buyShark", coins)
// });

// gm.events.add("client::donate:buyCar", (model) => {
//   callRemote("server::donate:buyCar", model)
// });

// gm.events.add("client::donate:buyPrem", (coins) => {
//   callRemote("server::donate:buyPrem", coins)
// });


// gm.events.add("client::donate:buyPacket", (coins) => {
//   callRemote("server::donate:buyPacket", coins)
// });