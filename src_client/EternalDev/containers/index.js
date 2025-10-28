import Scaleform from "./scaleform";
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
export default new class Containers {
    VIEW_NAME = "MajesticContainers"
    
    opened = false

    constructor() {
        mp.events.add({
            "client.containers.menu.open": this.open.bind(this),
            "client.containers.menu.try_close": this.tryClose.bind(this),
            "client.containers.menu.close": this.close.bind(this),
            "client.containers.menu.update": this.updateMenu.bind(this),
            "client.containers.menu.place_bet": (bet, paymentType) => callRemote("server.containers.menu.place_bet", bet, paymentType),
            "client.containers.menu.prize_action": (type) => callRemote("server.containers.menu.prize_action", type),
            "client.containers.create": this.createContainer.bind(this),
            "client.containers.state": this.changeContainerData.bind(this),
            "render": this.onTick.bind(this)
        });

        this.scaleformNames = ["player_name_01","player_name_02","player_name_03","player_name_04","player_name_05","player_name_06"];
        this.containers = {};
    }

    createContainer(data) {
        const containers = JSON.parse(data);

        containers.forEach(containerData => {
            const scaleFormName = this.scaleformNames[containerData.Id - 1];
            const containerScaleform = new Scaleform(scaleFormName);

            if (containerData.IsAuctionStarted) containerScaleform.callFunction("SET_PLAYER_NAME", `Активный лот #${containerData.Id}: \n${containerData.Name}\n Текущая ставка\n${containerData.Price}${this.getVault(containerData.isDonate)}`);
            else containerScaleform.callFunction("SET_PLAYER_NAME", "\nНе активный лот\n");

            this.containers[containerData.Id] = {
                isDonate: containerData.IsDonate,
                name: containerData.Name,
                position: containerData.Position,
                rotation: containerData.Rotation,
                scaleform: containerScaleform
            };
        });
    }

    getVault(isDonate) {
        return isDonate ? "MJ" : "$";
    }

    changeContainerData(id, state, price) {
        if (!this.containers.hasOwnProperty(id)) 
            return;

        const containerData = this.containers[id];
        if (!containerData) return;
        
        if (state) containerData.scaleform.callFunction("SET_PLAYER_NAME", `Активный лот #${id}: \n${containerData.name}\n Текущая ставка\n${price}${this.getVault(containerData.isDonate)}`);
        else containerData.scaleform.callFunction("SET_PLAYER_NAME", "\nНе активный лот\n");
    }

    onTick() {
        const playerPosition = mp.players.local.position;

        for (const containerId in this.containers) {
            const containerData = this.containers[containerId];
            const position = containerData.position;

            if (mp.game.system.vdist(position.x, position.y, position.z, playerPosition.x, playerPosition.y, playerPosition.z) > 150) 
                continue;

            const fl = .6;

            const labelPosition = this.getPositionOffset(containerData.position, containerData.rotation.z, 1.35);
            containerData.scaleform.render3D(new mp.Vector3(labelPosition.x, labelPosition.y, labelPosition.z + 2.5), new mp.Vector3(0, 0, containerData.rotation.z - 90), new mp.Vector3(15 * fl, 8 * fl, 10 * fl))
        }
    }

    open(containerData, currentBet, waitSeconds, bets, winPrize) {
        global.menuOpen();

        const viewData = {
            containerData: JSON.parse(containerData),
            bets: JSON.parse(bets),
            winPrize: typeof winPrize == 'string' ? JSON.parse(winPrize) : winPrize,
            currentBet,
            waitSeconds
        };

        mp.gui.emmit(`
            window.router.setView('${this.VIEW_NAME}', '${JSON.stringify(viewData)}');
        `);
        
        this.opened = true;
    }

    tryClose() {
        if (!this.opened)
            return;

        callRemote("server.containers.menu.close")
    }

    close() {
        if (!this.opened)
            return;

        global.menuClose();
		mp.gui.emmit(`
            window.router.setHud();
        `);
            
        this.opened = false;
    }

    getPositionOffset(pos, angle, dist) {
        angle = angle * 0.0174533;
    
        const vector3 = new mp.Vector3(pos.x, pos.y, pos.z);
        vector3.y = vector3.y + dist * Math.sin(angle);
        vector3.x = vector3.x + dist * Math.cos(angle);
        return vector3;
    }

    updateMenu(type, data) {
        mp.gui.emmit(`
            window.containers.update('${type}', ${data}) 
        `);
    } 
}