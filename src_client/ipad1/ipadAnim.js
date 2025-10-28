const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
let smartipadScenarioList = [];
class IpadScenarioBase extends global.CustomScenario {
    constructor() {
        super("cipad_base");
    }
    async onStart(player) {
        if (player === global.localplayer)
            return;

        global.requestAnimDict("cellipad@str").then(async () => {
            if (mp.players.exists(player) && 0 !== player.handle) {
                mp.attachments.addFor (player, mp.game.joaat("ipadcall"));
                player.taskPlayAnim("cellipad@str", "cellipad_text_press_a", 8, 0, -1, 49, 0, false, false, false)
            }
        });
    }
    onStartForNew(player) {
        this.onStart(player);
    }
    onEnd(player) {
        if (player === global.localplayer)
            return;

        if (mp.players.exists(player) && 0 !== player.handle) {
            mp.attachments.removeFor(player, mp.game.joaat("ipadcall"));
            player.stopAnimTask("cellipad@str", "cellipad_text_press_a", 3)
            //player.vehicle ? player.stopAnimTask("cellipad@str", "cellipad_text_press_a", 3) : player.clearTasksImmediately()
        }
    }
}
smartipadScenarioList.push(new IpadScenarioBase());
class IpadScenarioCall extends global.CustomScenario {
    constructor() {
        super("cipad_call");
    }
    async onStart(player) {
        if (player === global.localplayer) {
            player.taskUseMobileIpad(1);
            return;
        }
        global.requestAnimDict("anim@cellipad@in_car@ds").then(async () => {
            if (mp.players.exists(player) && 0 !== player.handle) {
                mp.attachments.addFor (player, mp.game.joaat("ipadcall"));
                player.taskPlayAnim("anim@cellipad@in_car@ds", "cellipad_call_listen_base", 8, 0, -1, 49, 0, false, false, false)
            }
        });
    }
    onStartForNew(player) {
        this.onStart(player);
    }
    onEnd(player) {
        if (player === global.localplayer) {
            player.taskUseMobileIpad(0);
            return;
        }
        if (mp.players.exists(player) && 0 !== player.handle) {
            mp.attachments.removeFor(player, mp.game.joaat("ipadcall"));
            player.stopAnimTask("anim@cellipad@in_car@ds", "cellipad_call_listen_base", 3)
            //player.vehicle ? player.stopAnimTask("cellipad@str", "cellipad_text_press_a", 3) : player.clearTasksImmediately()
        }
    }
}
smartipadScenarioList.push(new IpadScenarioCall());
const isPlayerHasAnyIpadScenario = (player) => {
    const name = player.cSen;
    return "cipad_base" === name || "cipad_call" === name;
};

gm.events.add("client.ipad.anim", async (playerId, type) => {
    const player = mp.players.atRemoteId(playerId);
    if (mp.players.exists(player) && 0 !== player.handle && player !== global.localplayer)
        if (0 === type) {
            for (const scenario of smartipadScenarioList)
                if (scenario.isActive(player)) {
                    scenario.onStart(player);
                    break;
                }
        } else if (1 === type) {
            global.requestAnimDict("cellipad@self").then(async () => {
                if (mp.players.exists(player) && 0 !== player.handle) {
                    player.taskPlayAnim("cellipad@self", "selfie", 4, 4, -1, 49, 0, false, false, false);
                }
            });
        }
})