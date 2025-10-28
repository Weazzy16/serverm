const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
mp.nametags.enabled = false;

global.binderFunctions.showGamertags = () => {
    if (!global.loggedin || global.chatActive || global.editing || global.menuCheck()) return;
    global.Name = !global.Name;
};

const eventsData = [
	{name: "ALVL"},
	{name: "AGM"},
	{name: "DMGDisable"},
	{name: "VoiceZone"},
	{name: "leader"},
	{name: "INVISIBLE", updateNameTag: true},
	{name: "HideNick", updateNameTag: true},
	{name: "InDeath"},
	{name: "AFK_STATUS"},
	{name: "vmuted"},
	{name: "isWhisper"},
	{name: "isDeaf"},
	{name: "PlayerAirsoftTeam"},
	{name: "REDNAME", updateNameTag: true},
	{name: "fraction", updateNameTag: true, updateName: true},
	{name: "organization", updateNameTag: true, updateName: true},
	{name: "IS_MASK", updateNameTag: true, updateName: true},
	{name: "NewUser"},
	{name: "SZ"},
	{name: "warType"},
	{name: "Old1"},
	{name: "Old2"},
	{name: "Old3"},
	{name: "UUID"},
];

eventsData.forEach(data => {
	mp.events.addDataHandler(data.name, (entity, value, oldValue) => {
		if (entity && mp.players.exists(entity) && entity.type === 'player') {
			SetSharedData (entity, data.name, value);

			if (data.updateName) 
				SetName (entity);
	
			if (data.updateNameTag && entity.handle !== 0)
				SetNameTag (entity);
		}
	});
});

global.SetSharedData = (player, name, value) => {
	if (player && mp.players.exists(player) && player.type === 'player') {
		player [name] = value;
	}
}

gm.events.add('newPassport', async (entity, pass) => {
	if (entity && mp.players.exists(entity) && entity.type === 'player') {
		await global.wait (10);
		SetName (entity);
		if (entity.handle !== 0)
			SetNameTag (entity);
	}
});

gm.events.add('setFriendList', async () => {
	try {
		await global.wait (10);
		mp.players.forEach(player => {
			if (player && mp.players.exists(player)) {
				SetName (player);
				if (player.handle !== 0)
					SetNameTag (player);
			}
		});
	}
    catch (e) {
		callRemote("client_trycatch", "player/gametag", "setFriendList", e.toString());
	}
});

gm.events.add('setFriend', async () => {
	try {
		await global.wait (10);
		mp.players.forEach(player => {
			if (player && mp.players.exists(player)) {
				SetName (player);
				if (player.handle !== 0)
					SetNameTag (player);
			}
		});
	}
    catch (e) {
		callRemote("client_trycatch", "player/gametag", "setFriend", e.toString());
	}
});

gm.events.add("pPlayerStreamIn", (entity) => {
	eventsData.forEach(data => {
		SetSharedData (entity, data.name, entity.getVariable(data.name));
	});
	SetName (entity);
	SetNameTag (entity);
});

const SetName = (player) => {
	if (!player || !mp.players.exists(player))
		return;
	player.currentName = GetCurrentName (player, player.remoteId);
}

global.getName = (player) => {
	if (!player || !mp.players.exists(player))
		return;
	if (!player.currentName) 
		player.currentName = GetCurrentName (player, player.remoteId);
	return player.currentName;
}

const nameSettings = {
	scale: 0.275,
	font: 0
}

const SetNameTag = (player) => {
    try {
        if (!player || !mp.players.exists(player))
            return;

        if (player['INVISIBLE'] || player['HideNick']) {
            player.nameTag = false;
        } else {
            // Генерация имени как в nametag.js
            player.nameTag = GeneratePlayerDisplayName(player);

            player.nameWidth = ((text, font, scale) => (
                mp.game.ui.setTextEntryForWidth("STRING"),
                mp.game.ui.addTextComponentSubstringPlayerName(text),
                mp.game.ui.setTextFont(font),
                mp.game.ui.setTextScale(scale, scale),
                mp.game.ui.getTextScreenWidth(true)
            ))(player.nameTag, nameSettings.font, nameSettings.scale);

            player.nameHeight = mp.game.ui.getTextScaleHeight(nameSettings.scale, nameSettings.font);
        }
    }
    catch (e) {
        callRemote("client_trycatch", "player/gamertag", "SetNameTag", e.toString());
    }
}

// Новая функция генерации имени как в nametag.js
const GeneratePlayerDisplayName = (player) => {
    try {
        const playerName = global.getName(player);
        const staticId = player['UUID'];
        const remoteId = player.remoteId;
        const isAdmin = global.isAdmin;
        const playerFraction = player['fraction'] || 0;
        const playerOrganization = player['organization'] || 0;
        const localFraction = global.fractionId || 0;
        const localOrganization = global.organizationId || 0;
        const isFriend = global.friends[player.name] !== undefined;
        const isInMask = player['IS_MASK'];
        const playerAdmin = player['ALVL'] || 0;

        // Админ видит все
        if (isAdmin) {
            return `${playerName} [${remoteId}]\n#${staticId}`;
        }
        
        // Админ игрока и без временного имени
        if (playerAdmin && !player.playerTempName) {
            return `${playerName} [${remoteId}]\n#${staticId}`;
        }
        
        // В маске и разные фракция/организация
        if (isInMask && 
            playerFraction > 0 && 
            playerFraction != localFraction && 
            playerOrganization > 0 && 
            playerOrganization != localOrganization) {
            return `[${remoteId}]\n#${staticId}`;
        }
        
        // Та же фракция или организация, или друг без маски
        if ((localFraction && localFraction === playerFraction) || 
            (localOrganization && localOrganization === playerOrganization) || 
            (isFriend && !isInMask)) {
            return `${playerName} [${remoteId}]\n#${staticId}`;
        }
        
        // Все остальные случаи - только ID
        return `[${remoteId}]\n#${staticId}`;
    }
    catch (e) {
        callRemote("client_trycatch", "player/gamertag", "GeneratePlayerDisplayName", e.toString());
        return `[${player.remoteId}]\n#${player['UUID']}`;
    }
}

global.loadTextureDict = dict => new Promise(async (resolve, reject) => {
	try {
		if (mp.game.graphics.hasStreamedTextureDictLoaded(dict))
			return resolve(true);
        mp.game.graphics.requestStreamedTextureDict(dict, false)
        let d = 0;
		while (!mp.game.graphics.hasStreamedTextureDictLoaded(dict)) {
            if (d > 5000) return resolve("Ошибка loadTextureDict. Model: " + dict);
            d++;
            await global.wait (0);
        }
        return resolve(true);
    } 
    catch (e) {
		callRemote("client_trycatch", "player/gametag", "loadTextureDict", e.toString());
		resolve();
	}
});

const TextureDictLoadedData = [
	"redage_textures_001",
	"mpinventory",
	"majestic_textures_001"
]

global.wait(3000).then(() => {
	TextureDictLoadedData.forEach((textureDict) => {
		if (!mp.game.graphics.hasStreamedTextureDictLoaded(textureDict))
			global.loadTextureDict (textureDict);
	});
});

const MaxStreeamDist = 225;

gm.events.add(global.renderName ["1s"], () => {
	try {
		mp.players.forEachInStreamRange(player => {
			if (global.localplayer.hasClearLosTo(player.handle, 17))
				player.hasLosTo = true;
			else 
				player.hasLosTo = false;
		});
	}
    catch (e) {
		callRemote("client_trycatch", "player/gametag", global.renderName ["1s"], e.toString());
	}
});

const defaultColor = [255, 255, 255, 200]; // Изменен альфа канал как в nametag.js

gm.events.add('render', (nametags) => {
	try {
		if (!global.loggedin) return;

		if (global.isTagsHead) {
			nametags.forEach(nametag => {
				try {
					let [player, x, y, distance] = nametag;
					if (distance <= MaxStreeamDist) {
						if (!player.hasLosTo) return;
						if (!player.isOnScreen()) return;
						if (!player.nameTag) return;
						if (player['INVISIBLE'] || player['HideNick']) return; // Дополнительная проверка

						// Вычисление масштаба на основе расстояния как в nametag.js
						const distanceScale = 1 - distance / MaxStreeamDist;
						const textScale = Math.max(0.65 * nameSettings.scale, nameSettings.scale) * distanceScale;
						const spriteScale = Math.max(0.65 * 0.7, 0.7) * distanceScale;

						DrawPlayerName(player, x, y + 0.015, textScale);
						
						y += 0.03015;
						DrawPlayerIcon(player, x, y, defaultColor, spriteScale);
						DrawHealthAndArmor(player, x, y + 0.005, 0.035, 0.003);						
					}
				}
				catch (e) {
					if(new Date().getTime() - global.trycatchtime["player/gamertag1"] < 5000) return;
					global.trycatchtime["player/gamertag1"] = new Date().getTime();
					callRemote("client_trycatch", "player/gamertag", "render 1", e.toString());
				}
			})
		}
	}
	catch (e) {
		if(new Date().getTime() - global.trycatchtime["player/gamertag"] < 60000) return;
		global.trycatchtime["player/gamertag"] = new Date().getTime();
		callRemote("client_trycatch", "player/gamertag", "render", e.toString());
	}
});

const drawSprites = [
	["redage_textures_001", "newUser"],
	["redage_textures_001", "telefon"],
	["redage_textures_001", "racia"],
	["redage_textures_001", "micro_on"],
	["redage_textures_001", "micro_off"],
	["redage_textures_001", "microphone"],
	["redage_textures_001", "quest_perform", -45],
	["redage_textures_001", "quest_take", -45],
	["redage_textures_001", "door_open", -45],
	["redage_textures_001", "door_close", -45],	
	["redage_textures_001", "death"],
	["redage_textures_001", "afk"],
	["redage_textures_001", "shepot"],
	["redage_textures_001", "mutedinamik"],
	["redage_textures_001", "chat"],
	["redage_textures_001", "friends"],
	["redage_textures_001", "admin"],
	["redage_textures_001", "ticket", -45],
	["redage_textures_001", "attack"],
	["redage_textures_001", "shield"],
	["redage_textures_001", "verify_1"],
	["redage_textures_001", "verify_2"],
	["redage_textures_001", "verify_3"],
]

const defaultSpriteSize = 0.55;
const defaultSpritesSize = {
	"door_open": 1.0,
	"door_close": 1.0,
	"ticket": 1.0,
	"quest_perform": 1.0,
	"quest_take": 1.0,
};

let drawSpritesSize = {};

gm.events.add(global.renderName ["2s"], () => {
	const activeResolution = mp.game.graphics.getScreenActiveResolution(0, 0);

	drawSprites.forEach((drawSprite) => {
		const size = defaultSpritesSize [ drawSprite [1] ] ? defaultSpritesSize [ drawSprite [1] ] : defaultSpriteSize;
		const drawSpritesResolution = mp.game.graphics.getTextureResolution(drawSprite [0], drawSprite [1]);
		drawSpritesSize [ drawSprite [1] ] = { 
			width: (size * drawSpritesResolution.x) / activeResolution.x,
			height: (size * drawSpritesResolution.y) / activeResolution.y,
			heading: drawSprite [2] ? drawSprite [2] : 0
		}
	});
});

const DrawPlayerName = (player, x, y, scale = nameSettings.scale) => {
	try {
		const textData = {};
		textData.font = 0; 
		textData.color = defaultColor;
		textData.scale = [scale, scale]; // Используем переданный масштаб
		textData.outline = true;
		textData.centre = true; // Центрирование как в nametag.js

		let nameTag = player.nameTag;

		if (global.isAdmin && player.getVariable("IS_MEDIA") == true)
			nameTag += '\n~g~MEDIA'

		mp.game.graphics.drawText(nameTag, [x, y], textData);

		// Иконка голоса перенесена в DrawPlayerIcon для лучшей организации
	}
    catch (e) {
		callRemote("client_trycatch", "player/gamertag", "DrawPlayerName", e.toString());
	}
}

const adminColors = [
	defaultColor,
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[211, 126, 15, 255],
	[255, 221, 0, 255],
	[211, 126, 15, 255],
	[255, 0, 0, 255],
	[255, 0, 0, 255],
]

const GetDrawPlayerIcon = (player) => {
	try {
		let returnIcon = [];

		// Админ иконка
		if (player['REDNAME']) {
			let playerALVL = Number(player['ALVL']) || 0;
			let adminSprite = "admin";
			let adminDict = "redage_textures_001";
			
			// Разные иконки для разных уровней админки как в nametag.js
			switch (playerALVL) {
				case 1:
				case 2:
					adminSprite = "admin_helper";
					adminDict = "majestic_textures_001";
					break;
				case 7:
				case 8:
				case 9:
				case 10:
					adminSprite = "admin_7";
					adminDict = "majestic_textures_001";
					break;
				default:
					adminSprite = "admin";
					adminDict = "redage_textures_001";
			}
			returnIcon.push({textureDict: adminDict, textureName: adminSprite, color: adminColors[playerALVL]});
		}

		// Иконки статусов в определенном порядке
		if (!global.localplayer["NewUser"] && player["NewUser"])
			returnIcon.push({textureDict: "redage_textures_001", textureName: "newUser", color: [255, 255, 255, 255]});

		// Приоритет иконок голоса и статусов
		if (player['vmuted']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "mutedinamik", color: [255, 0, 0, 255]});
		} else if (player.isVoiceActive && !player["InDeath"]) {
			if (player.isListening == 2) {
				returnIcon.push({textureDict: "redage_textures_001", textureName: "telefon"});
			} else if (player.isListening == 3) {
				returnIcon.push({textureDict: "redage_textures_001", textureName: "racia"});
			} else {
				let voiceName = "micro_on";
				let voiceColor = defaultColor;
				
				if (global.pplMuted[player.name] === true) {
					voiceColor = [255, 0, 0, 255];
					voiceName = "mutedinamik";
				} else if (global.pplMutedMe[player.name] === true) {			
					voiceColor = [255, 0, 0, 255];
					voiceName = "micro_off";
				} else if (player.voiceDist) {
					voiceName = "microphone";
				}
				returnIcon.push({textureDict: "redage_textures_001", textureName: voiceName, color: voiceColor});
			}
		}

		if (player["InDeath"]) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "death", color: [231, 76, 60, 200]});
		} 

		if (player['AFK_STATUS']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "afk", color: [231, 76, 60, 200]});
		}

		if (player['isWhisper']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "shepot"});
		} else if (player['isDeaf']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "mutedinamik"});
		}

		// Иконки чата
		if (player.isTypingInTextChat) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "chat"});
		}

		// Верификация
		if (player['Old1']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "verify_1"});
		} else if (player['Old2']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "verify_2"});
		} else if (player['Old3']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "verify_3"});
		}

		// Боевые статусы
		if (player["warType"] === 2)
			returnIcon.push({textureDict: "redage_textures_001", textureName: "attack", color: [255, 0, 0, 255]});
		else if (player["warType"] === 1)
			returnIcon.push({textureDict: "redage_textures_001", textureName: "shield", color: [0, 204, 102, 255]});

		// Команда
		if (global.localplayer['PlayerAirsoftTeam'] && 
			global.localplayer['PlayerAirsoftTeam'] >= 0 && 
			global.localplayer['PlayerAirsoftTeam'] == player['PlayerAirsoftTeam']) {
			returnIcon.push({textureDict: "redage_textures_001", textureName: "friends"});
		}

		return returnIcon;
	}
	catch (e) {
		if(new Date().getTime() - global.trycatchtime["player/gamertag5"] < 5000) return [];
		global.trycatchtime["player/gamertag5"] = new Date().getTime();
		callRemote("client_trycatch", "player/gamertag", "GetDrawPlayerIcon", e.toString());
		return [];
	}
}

const DrawPlayerIcon = (player, x, y, color, spriteScale = 0.7) => {
	try {
		const returnIcon = GetDrawPlayerIcon(player);
		let iconX = x;
		
		if (returnIcon && returnIcon.length > 0) {
			// Центрирование иконок как в nametag.js
			const iconSpacing = 0.01;
			const totalWidth = returnIcon.length * iconSpacing;
			iconX = x - totalWidth / 2 + iconSpacing / 2;

			returnIcon.forEach((data) => {
				DrawScaledSprite(
					data.textureDict, 
					data.textureName, 
					data.color || color, 
					iconX, 
					y - 0.02, 
					spriteScale
				);
				iconX += iconSpacing;
			});
		}
	}
	catch (e) {
		if(new Date().getTime() - global.trycatchtime["player/gamertag5"] < 5000) return;
		global.trycatchtime["player/gamertag5"] = new Date().getTime();
		callRemote("client_trycatch", "player/gamertag", "DrawPlayerIcon", e.toString());
	}
}

// Новая функция для масштабированных спрайтов
const DrawScaledSprite = (textureDict, textureName, colour, x, y, scale = 1.0) => {
	const drawData = drawSpritesSize[textureName];
	if (drawData) {
		const scaledWidth = drawData.width * scale;
		const scaledHeight = drawData.height * scale;
		mp.game.graphics.drawSprite(
			textureDict, 
			textureName, 
			x, 
			y, 
			scaledWidth, 
			scaledHeight, 
			drawData.heading, 
			colour[0], 
			colour[1], 
			colour[2], 
			colour[3]
		);
	}
}

global.DrawSprite = (textureDict, textureName, colour, x, y) => {
	DrawScaledSprite(textureDict, textureName, colour, x, y, 1.0);
}

const DrawHealthAndArmor = (player, x, y, width, height) => {
	try {
		const isAdmin = global.isAdmin;
		// Проверка прицеливания как в nametag.js
		const isAiming = mp.game.player.isFreeAimingAtEntity(player.handle) || 
						 mp.game.player.isTargettingEntity(player.handle) ||
						 global.localplayer.entityAimingAt === player;

		if (isAdmin || isAiming) {
			y += 0.0225;
			
			let health = Math.max(player.getHealth() - 100, 0);
			if (health > 100 || health < 0) health = 100;
			health = health <= 100 ? health / 100 : (health - 100) / 100;

			let armour = player.getArmour() / 100;
			if (armour > 100 || armour < 0) armour = 100;
			
			// Цвета здоровья как в nametag.js
			const healthColorBG = [61, 61, 61];
			const healthColor = [255, 255, 255];
			
			// Фон полоски здоровья
			mp.game.graphics.drawRect(x, y, width, height, healthColorBG[0], healthColorBG[1], healthColorBG[2], 200);
			
			// Полоска здоровья
			mp.game.graphics.drawRect(
				x - width / 2 * (1 - health), 
				y, 
				width * health, 
				height, 
				healthColor[0], 
				healthColor[1], 
				healthColor[2], 
				200
			);

			// Полоска брони
			if (armour > 0) {
				y += 0.007;
				mp.game.graphics.drawRect(x, y, width, height, 12, 52, 77, 200);
				mp.game.graphics.drawRect(
					x - width / 2 * (1 - armour), 
					y, 
					width * armour, 
					height, 
					80, 
					146, 
					187, 
					200
				);
			}
		}
	}
	catch (e) {
		if(new Date().getTime() - global.trycatchtime["player/gamertag3"] < 5000) return;
		global.trycatchtime["player/gamertag3"] = new Date().getTime();
		callRemote("client_trycatch", "player/gamertag", "DrawHealthAndArmor", e.toString());
	}
}

global.GetScale = (realDist, maxDist) => {
	return Math.max(0.1, 1 - realDist / maxDist);
};

gm.events.add('sendRPMessage', (type, msg, players) => {
	try {
		var chatcolor = ``;

		players.forEach((id) => {
			var player = mp.players.atRemoteId(id);
			if (mp.players.exists(player)) {
				if (type === "chat" || type === "s") {
					let localPos = global.localplayer.position;
					let position = player.position;
					let dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
					var color = (dist < 2) ? "FFFFFF" :
						(dist < 4) ? "F7F9F9" :
							(dist < 6) ? "DEE0E0" :
								(dist < 8) ? "C5C7C7" : "ACAEAE";
					chatcolor = color;
				}
				msg = msg.replace("{name}", global.getName(player));
			}
		});

		if (type === "chat" || type === "s") msg = `!{${chatcolor}}${msg}`;
		mp.gui.chat.push(msg);
	}
	catch (e) {
		callRemote("client_trycatch", "player/gamertag", "sendRPMessage", e.toString());
	}
});

const IsNameToPlayer = (player) => {
	try {
		if (global.isAdmin) return 1;
		else if (player === global.localplayer) return 1;
		const playerFraction = player['fraction'];
		if (global.fractionId != 0 && playerFraction != 0 && global.fractionId === playerFraction) return 1;
		const playerOrganization = player['organization'];
		if (global.organizationId != 0 && playerOrganization != 0 && global.organizationId === playerOrganization) return 1;
		else if (!player['IS_MASK'] && global.passports[player.name] != undefined) return 2;
		else if (!player['IS_MASK'] && global.friends[player.name] != undefined) return 3;
		return 0;
	}
	catch (e) {
		if(new Date().getTime() - global.trycatchtime["player/gamertag6"] < 5000) return 0;
		global.trycatchtime["player/gamertag6"] = new Date().getTime();
		callRemote("client_trycatch", "player/gamertag", "IsNameToPlayer", e.toString());
		return 0;
	}
}

const GetCurrentName = (player, id) => {
    try {
        if (!player || !mp.players.exists(player)) return "";

        const isSelf = (player === global.localplayer);
        const isFriend = global.friends[player.name] !== undefined;
        const isMasked = player['IS_MASK'];

        if (global.isAdmin) {
            return `${player.name.replace('_', ' ')} #${player['UUID']}`;
        }

        if (isSelf) {
            return player.name.replace('_', ' ');
        }

        if (isFriend && !isMasked) {
            return `${player.name.replace('_', ' ')} #${player['UUID']}`;
        }

        return `#${player['UUID']}`;
    }
    catch (e) {
        callRemote("client_trycatch", "player/gamertag", "GetCurrentName", e.toString());
        return "";
    }
}