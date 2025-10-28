const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
global.binderFunctions.o_chat = () => {
    if (global.loggedin === true && !global.menuCheck () && global.chatActive === false && global.showhud)
    {
        mp.gui.emmit(`window.chat.toggleInput(true)`);
        global.chatActive = true;
        global.menuOpen();
    }
};

global.binderFunctions.c_chat = () => {
    if(global.loggedin === true/* && global.menuCheck ()*/ && global.chatActive === true)
    {
        mp.gui.emmit(`window.chat.toggleInput(false, false, false, false)`);
        global.menuClose();
        global.chatActive = false;
    }
};

gm.events.add('client:OnChatInputChanged', (toggled) => {
    
    global.chatActive = toggled;
    if(toggled) 
        global.menuOpen();
    else 
        global.menuClose();
});

/*gm.events.add('client:toggleInput', (toggled) => {
    callRemote("toggleInput", toggled);
});*/


gm.events.add('loadChatConfig', (params) => {
    UpdateSettingsData (params);
});

gm.events.add('chatconfig', function (params) {
    UpdateSettingsData (params);
	callRemote("chatConfigSave", params);
});

gm.events.add('updateRadioVolume', function (volume = 15) {
    mp.gui.emmit(`window.events.callEvent("cef.walkietalkie.updatePhoneRadioVolume", ${volume})`);
});

let isMap = true;
global.isTagsHead = true;
global.FirstLVLMute = false;
global.HudCompass = true;
global.MaxVoiceVolume = 100;
global.MaxVolumeRadio = 70;
global.MaxVolumeBoombox = 70;
global.DistancePlayer = 100;
global.DistanceVehicle = 100;

const UpdateSettingsData = (params) => {
    try
    {
        mp.gui.emmit(`window.chat.updateConfig('${params}')`)
        mp.gui.emmit(`window.settingsStore.init ('${params}')`);
        params = JSON.parse (params);
        if (params.HudToggled !== undefined) {
            mp.gui.emmit(`window.hudStore.isHudVisible (${params.HudToggled})`);
            global.showhud = params.HudToggled;
        }
         if (params.schat !== undefined) {
            // Показать/скрыть чат
            mp.gui.emmit(`window.hudStore.isChatVisible(${params.schat})`);
        }
        
        if (params.hud !== undefined) {
            mp.gui.emmit(`window.hudStore.isHudVisible(${params.hud})`);
            global.showhud = params.hud;
        }
        
        if (params.radar !== undefined) {
            mp.gui.emmit(`window.hudStore.isRadarVisible(${params.radar})`);
            global.bigMapStatus = !params.radar ? 3 : 0;
        }
        
        if (params.ids !== undefined) {
            global.isTagsHead = params.ids;
        }
        
        if (params.compass !== undefined) {
            mp.gui.emmit(`client.compass(${params.compass})`);
        }
        
        if (params.displayChatMsg !== undefined) {
            // Логика для сообщений над головой
        }
        
        if (params.schatPunish !== undefined) {
            global.showPunishments = params.schatPunish;
        }
        if (params.HudStats !== undefined)
            mp.gui.emmit(`window.hudStore.isPlayer (${params.HudStats})`);
    
        if (params.TagsHead !== undefined)
            global.isTagsHead = params.TagsHead;
    
        if (params.HudSpeed !== undefined)
            mp.gui.emmit(`window.vehicleState.isToggledVehicleHud (${params.HudSpeed})`);
    
        if (params.HudOnline !== undefined)
            mp.gui.emmit(`window.hudStore.isWaterMark (${params.HudOnline})`);
    
        if (params.HudLocation !== undefined)
            mp.gui.emmit(`window.hudStore.isWorld (${params.HudLocation})`);
    
        if (params.HudKey !== undefined) {
            mp.gui.emmit(`window.hudStore.isHelp (${params.HudKey})`);
            global.showHint = params.HudKey;
        }
    
        if (params.HudMap !== undefined && isMap !== params.HudMap) {
            isMap = params.HudMap;
            global.bigMapStatus = !isMap ? 3 : 0;
        }
       if (params.HudCompass !== undefined) {// Compass
            mp.gui.emmit(`client.compass (${params.HudCompass})`); 
           }
    
        if (params.VolumeInterface !== undefined)
            global.VolumeInterface = params.VolumeInterface;

        if (params.VolumeQuest !== undefined)
            global.VolumeQuest = params.VolumeQuest;

        if (params.VolumeAmbient !== undefined)
            global.VolumeAmbient = params.VolumeAmbient;

        if (params.VolumePhoneRadio !== undefined)
            global.VolumePhoneRadio = params.VolumePhoneRadio;
    
        if (params.VolumeVoice !== undefined) {
            global.MaxVoiceVolume = params.VolumeVoice;
        }
    
        if (params.VolumeRadio !== undefined) {
            global.MaxVolumeRadio = params.VolumeRadio;
            call('UpdateVoiceRadio');
        }
    
        if (params.VolumeBoombox !== undefined) {
            global.MaxVolumeBoombox = params.VolumeBoombox;
        }
        
        if (params.FirstMute !== undefined)
            global.FirstLVLMute = params.FirstMute;
        
        if (params.DistancePlayer !== undefined && params.DistancePlayer !== global.DistancePlayer) {
            global.DistancePlayer = params.DistancePlayer;
            const getDist = global.getLodDist (global.DistancePlayer);
            mp.players.forEachInStreamRange(player => {
                if (player && player.handle !== 0) {
                    player.setLodDist(getDist);
                }
            });
        }
    
        if (params.DistanceVehicle !== undefined && params.DistanceVehicle !== global.DistanceVehicle) {
            global.DistanceVehicle = params.DistanceVehicle;
            const getDist = global.getLodDist (global.DistanceVehicle);
            mp.vehicles.forEachInStreamRange(vehicle => {
                if (vehicle && vehicle.handle !== 0) {
                    vehicle.setLodDist(getDist);
                }
            })
        }
    
        if (params.cPToggled !== undefined)
            global.crosshairParameters.toggled = params.cPToggled;
    
        if (params.cPWidth !== undefined)
            global.crosshairParameters.width = params.cPWidth;
    
        if (params.cPGap !== undefined)
            global.crosshairParameters.gap = params.cPGap;
    
        if (params.cPDot !== undefined)
            global.crosshairParameters.dot = params.cPDot;
    
        if (params.cPThickness !== undefined)
            global.crosshairParameters.thickness = params.cPThickness;
    
        if (params.cPColorR !== undefined)
            global.crosshairParameters.color[0] = params.cPColorR;
    
        if (params.cPColorG !== undefined)
            global.crosshairParameters.color[1] = params.cPColorG;
    
        if (params.cPColorB !== undefined)
            global.crosshairParameters.color[2] = params.cPColorB;
    
        if (params.cPOpacity !== undefined)
            global.crosshairParameters.opacity = params.cPOpacity;
    
        if (params.cPCheck !== undefined)
            global.crosshairParameters.checkp = params.cPCheck;
            
        if (params.cEfValue !== undefined)
            mp.game.graphics.bloodVfxMode = params.cEfValue;

        if (params.notifCount !== undefined)
            global.notifyCount = params.notifCount;

        if (params.hitPoint !== undefined) {
            global.hitPoint = params.hitPoint;
        }
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/chat", "UpdateSettingsData", e.toString());
    }    
}

global.getLodDist = (dist) => {
    return Math.round (getLerp (65, 400, dist / 100));
}

global.getLerp = (min, max, coef) => {
    return min * (1 - coef) + max * coef;
}