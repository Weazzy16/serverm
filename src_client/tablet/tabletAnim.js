let smarttabletScenarioList = [];
class TabletScenarioBase extends global.CustomScenario {
    constructor() {
        super("cphone_base");
    }
    async onStart(player) {
        if (player === global.localplayer)
            return;

        global.requestAnimDict("celltablet@str").then(async () => {
            if (mp.players.exists(player) && 0 !== player.handle) {
                mp.attachments.addFor (player, mp.game.joaat("tabletcall"));
                player.taskPlayAnim("celltablet@str", "celltablet_text_press_a", 8, 0, -1, 49, 0, false, false, false)
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
            mp.attachments.removeFor(player, mp.game.joaat("tabletcall"));
            player.stopAnimTask("celltablet@str", "celltablet_text_press_a", 3)
            //player.vehicle ? player.stopAnimTask("celltablet@str", "celltablet_text_press_a", 3) : player.clearTasksImmediately()
        }
    }
}
smarttabletScenarioList.push(new TabletScenarioBase());
class TabletScenarioCall extends global.CustomScenario {
    constructor() {
        super("cphone_call");
    }
    async onStart(player) {
        if (player === global.localplayer) {
            player.taskUseMobileTablet(1);
            return;
        }
        global.requestAnimDict("anim@celltablet@in_car@ds").then(async () => {
            if (mp.players.exists(player) && 0 !== player.handle) {
                mp.attachments.addFor (player, mp.game.joaat("tabletcall"));
                player.taskPlayAnim("anim@celltablet@in_car@ds", "celltablet_call_listen_base", 8, 0, -1, 49, 0, false, false, false)
            }
        });
    }
    onStartForNew(player) {
        this.onStart(player);
    }
    onEnd(player) {
        if (player === global.localplayer) {
            player.taskUseMobileTablet(0);
            return;
        }
        if (mp.players.exists(player) && 0 !== player.handle) {
            mp.attachments.removeFor(player, mp.game.joaat("tabletcall"));
            player.stopAnimTask("anim@celltablet@in_car@ds", "celltablet_call_listen_base", 3)
            //player.vehicle ? player.stopAnimTask("celltablet@str", "celltablet_text_press_a", 3) : player.clearTasksImmediately()
        }
    }
}
smarttabletScenarioList.push(new TabletScenarioCall());
const isPlayerHasAnyTabletScenario = (player) => {
    const name = player.cSen;
    return "cphone_base" === name || "cphone_call" === name;
};

gm.events.add("client.tablet.anim", async (playerId, type) => {
    const player = mp.players.atRemoteId(playerId);
    if (mp.players.exists(player) && 0 !== player.handle && player !== global.localplayer)
        if (0 === type) {
            for (const scenario of smarttabletScenarioList)
                if (scenario.isActive(player)) {
                    scenario.onStart(player);
                    break;
                }
        } else if (1 === type) {
            global.requestAnimDict("celltablet@self").then(async () => {
                if (mp.players.exists(player) && 0 !== player.handle) {
                    player.taskPlayAnim("celltablet@self", "selfie", 4, 4, -1, 49, 0, false, false, false);
                }
            });
        }
})