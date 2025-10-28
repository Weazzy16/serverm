/*const colorHudIds = [
    9,
    18,
    143,
    144,
    145,
]

gm.events.add('playerReady', () => {
    colorHudIds.forEach((id) => {
        Natives.ReplaceHudColourWithRgba (id, 43, 182, 168, 255);
    })
});*/

import creator from "../EternalDev/creator";
import { majesticCef } from "../EternalDev/intro";
import { checkCamInAir } from "../EternalDev/intro/modules/skyCamera";

global.authorizationOpened = false;

const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let isMerger = false;

rpc.register("rpc.isMerger", () => isMerger);

gm.events.add('client.init', function (serverId, serverName, donatMultiplier, donateDoubleConvert, merger) {

    isMerger = merger;

    call("setTraffic", 0);
    call("cleartraffic");


    mp.game.gxt.set('PM_PAUSE_HDR', `ASTRARP.FUN | ${serverName}`);
    mp.gui.emmit(`window.setServerName ('${serverName}')`);
    mp.gui.emmit(`window.serverStore.serverId (${parseInt(serverId)})`);
    mp.gui.emmit(`window.serverStore.serverDonatMultiplier (${donatMultiplier})`);
    mp.gui.emmit(`window.serverStore.serverDonateDoubleConvert (${donateDoubleConvert})`);
    global.menuOpen();
});

gm.events.add('client.closeAll', () => {
    global.FadeScreen (false, 500);
	mp.gui.emmit(`window.router.close()`);
});

let cameraIndex = 0;

global.setStartCam = async () => {
    call("e-dev.intro.start");
};

setTimeout(() => {
    global.setStartCam ();
    gm.discord(translateText("Восхищается окном логина"));
}, 150)

gm.events.add('client.auth', async (login) => {
    // while(!global.cefRendered) {
    //     await global.wait(1);
    // }

    call("e-dev.intro.switch_scene", "auth", 400);
    majesticCef.execute(`auth.open('${login}')`);

    mp.gui.emmit(`
        window.listernEvent ('queueText', false);
        window.accountStore.accountLogin('${login}');
        window.router.close();
    `);

    global.authorizationOpened = true;
});

export let selectedSlot = -1;
gm.events.add("client.auth.creator_open", (slot) => {
    selectedSlot = slot;

    majesticCef.execute(`
        characterSelector.reset(); 
    `);
    creator.open(true);
});



var lastButAuth = 0;
var lastButSlots = 0;

// events from cef
gm.events.add('client:OnSignInv2', function (username, password) {
    try
    {
        if (new Date().getTime() - lastButAuth < 500) {
            call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();
        callRemote('signin', username, password)
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "client:OnSignInv2", e.toString());
    }
});

gm.events.add('restorepass', function (state, authData) {
    try
    {
        if (new Date().getTime() - lastButAuth < 1000) {
            call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();

        var nameorcode = authData;
        callRemote('restorepass', state, nameorcode);
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "restorepass", e.toString());
    }
});

gm.events.add('restorepassstep', function (state) {
    majesticCef.execute(`auth.setRestoreStep(${state})`)
	mp.gui.emmit(`window.events.callEvent("cef.authentication.restoreStep", ${state})`);
});


gm.events.add('client:OnSignUpv2', function (username, email, promo, pass1, pass2) {
    try
    {
        if (new Date().getTime() - lastButAuth < 500) {
            call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();

        if (global.isInvalidLogin(username)) {
            call('notify', 1, 9, translateText("Логин не соответствует формату или слишком длинный!"), 3000);
            return;
        }

        if (global.isInvalidEmail(email)) {
            call('notify', 1, 9, translateText("Электронная почта не соответствует формату!"), 3000);
            return;
        }

        if(pass1 != pass2) {
            call('notify', 1, 9, translateText("Пароли не совпадают!"), 3000);
            return;
        }
        
        if(pass1.length < 3) {
            call('notify', 1, 9, translateText("Слишком короткий пароль!"), 3000);
            return;
        }

        callRemote('signup', username, pass1, email, promo);
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "client:OnSignUpv2", e.toString());
    }
});

gm.events.add('client.registration.error', function (message) {
    call('notify', 1, 9, message, 3000);
    mp.gui.emmit(`window.listernEvent ('isSendEmailMessage', false);`);
});

gm.events.add('client.registration.sendEmail', function () {
    mp.gui.emmit(`window.listernEvent ('isSendEmailMessage', true);`);
});

gm.events.add('client:OnSelectCharacterv2', async function (uuid, spawnid) {
    try
    {
        if (new Date().getTime() - lastButSlots < 500) {
            call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }

        majesticCef.execute(`
            SpawnSelector.reset();
        `);

        lastButSlots = new Date().getTime();
        mp.gui.emmit(`window.router.close()`);

        call("moveSkyCamera", global.localplayer, "up", 1);
        call('client.charcreate.close');

        await global.wait(1000);
        callRemote('selectchar', uuid, spawnid);

        call("e-dev.intro.stop");
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "client:OnSelectCharacterv2", e.toString());
    }
});

gm.events.add('client:OnCreateCharacterv2', function (slot, name, lastname) {
    try
    {
        if (global.checkName(name) || !global.checkName2(name) || name.length > 25 || name.length <= 2) {
            call('notify', 1, 9, translateText("Правильный формат имени: 3-25 символов и первая буква имени заглавная"), 3000);
            return;
        }

        if (global.checkName(lastname) || !global.checkName2(lastname) || lastname.length > 25 || lastname.length <= 2) {
            call('notify', 1, 9, translateText("Правильный формат фамилии: 3-25 символов и первая буква фамилии заглавная"), 3000);
            return;
        }

        if (new Date().getTime() - lastButSlots < 500) {
            call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButSlots = new Date().getTime();

        callRemote('newchar', slot, name, lastname);
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "client:OnCreateCharacterv2", e.toString());
    }
});

gm.events.add('buyNewSlot', function (slotId) {
    try
    {
        if (new Date().getTime() - lastButSlots < 500) {
            call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButSlots = new Date().getTime();
        callRemote('server.buySlots', Number (slotId));
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "buyNewSlot", e.toString());
    }
});

//***********************************Удаление */

gm.events.add('client.char.delete', function (slot) {
    callRemote('server.character.delete', slot);
});

gm.events.add('client.character.canceldelete', function (slot) {
    mp.gui.emmit(`window.accountStore.deleteCharacter(${slot}, "-")`);
});

gm.events.add('client.character.delete', function (slot, data) {
    mp.gui.emmit(`window.accountStore.deleteCharacter(${slot}, ${data})`);
});

gm.events.add('client.character.deleteSuccess', function (slot) {
    mp.gui.emmit(`window.accountStore.deleteSuccessCharacter(${slot})`);
});

gm.events.add('client.character.accountIsSession', function (toggled) {
    mp.gui.emmit(`window.accountStore.accountIsSession(${toggled})`);
});
//****************************************** */


gm.events.add('queue.text', (withkick, data) => {
    mp.gui.emmit(`window.listernEvent ('queueText', '${data}');`);

	if(withkick == true)
        callRemote('kickclient');
});

gm.events.add('unlockSlot', function (slotId) {
    mp.gui.emmit(`window.accountStore.unlockSlots(${slotId})`);
    majesticCef.execute(`characterSelector.unlockSlot(${slotId})`);
});

let useModel = -1;

gm.events.add(global.renderName ["2.5ms"], () => {
    if (global.loggedin && useModel !== global.localplayer.model) {
        useModel = global.localplayer.model;
        global.localplayer.setConfigFlag (429, true);
    }
});

gm.events.add('ready', async function (isSpawn = true) {
    try
    {
        call("e-dev.intro.stop");

        if (mp.game.streaming.isNewLoadSceneActive()) 
            mp.game.streaming.newLoadSceneStop();
        
        await global.wait(1000);
        call("freeze", true);
        call("moveSkyCamera", global.localplayer, "down");
        
        await checkCamInAir();

        global.loggedin = true;
        global.authorizationOpened = false;
        global.menuClose();
        call('showHUD', true);
        
        mp.gui.emmit(`window.serverStore.serverPlayerId (${global.localplayer.remoteId})`);
        global.localplayer.setInvincible(false);
        global.localplayer.setVisible(true, false);
        global.SetWalkStyle (global.localplayer, null);
        global.SetFacialClipset (global.localplayer, null);
        
        // global.setPlayerToGround ();
        // await global.wait(500);
        // global.FadeScreen (false, 2500);
        mp.gui.emmit(`window.router.setHud()`);
        global.setPlayerToGround ();
        // await global.wait(500);
        //global.localplayer.freezePosition(false);

        call("freeze", false);
        if (!global.isNewChar && isSpawn) {
            callRemote('IsCompensation');
        }
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "ready", e.toString());
    }
});

gm.events.add("initAwards", async (json) => {

    if (json) {
        json = JSON.parse(json);
        const award = {
            desc: json[0],
            type: json[1],
            itemId: json[2],
            data: json[3],
            image: json[4],
            time: json[5],
        }
        mp.gui.emmit(`window.listernEvent ('hud.award', true, '${JSON.stringify(award)}');`);

        await global.wait(1000 * 15);

        mp.gui.emmit(`window.listernEvent ('hud.award', false);`);

        await global.wait(500);
    }

    await global.wait(1000 * 30);

    mp.gui.emmit(`window.listernEvent ('hud.bp.info', true);`);

    await global.wait(1000 * 15);

    mp.gui.emmit(`window.listernEvent ('hud.bp.info', false);`);
    await global.wait(500);
    call('initWarZone', null, true);
});

// events from cef
gm.events.add('client.merger.auntification', function (password, serverId) {
    try
    {
        if (new Date().getTime() - lastButAuth < 3000) {
            mp.gui.emmit(`window.events.callEvent("cef.merger.progress", -2)`);
            call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();
        callRemote('server.merger.auntification', password, serverId)
    }
    catch (e) 
    {
        callRemote("client_trycatch", "player/auth", "client.merger.auntification", e.toString());
    }
});

gm.events.add('client.merger.progress', function (value) {
    if (value === 999) {
        global.FadeScreen (true, 0);
        call('client.charcreate.close', true);
        mp.gui.emmit(`events.callEvent("cef.authentication.setView", "Start")`);
    } else mp.gui.emmit(`window.events.callEvent("cef.merger.progress", ${value})`);
});

//

gm.events.add('client.session.update', function () {
    callRemote('server.session.update');
});

//

gm.events.add('client.email.confirm', function (email) {
    if (global.isInvalidEmail(email)) {
        call('notify', 1, 9, translateText("Электронная почта не соответствует формату!"), 3000);
        return;
    }

    callRemote('server.email.confirm', email);
});