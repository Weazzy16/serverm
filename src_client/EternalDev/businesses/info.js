const localPlayer = mp.players.local;
//tut netu CALLREMOTE ili CALL


export default new class BusinessInfo {
    VIEW_NAME = "MajesticBusinessInfo"

    opened = false
    constructor() {
        mp.events.add("client.businessInfo.open", this.open.bind(this));
        mp.events.add("client.businessInfo.close", this.close.bind(this));
    }

    open(data) {
        const viewData = {
            ...JSON.parse(data),
            Address: global.getAreaName(localPlayer.position.x, localPlayer.position.y, localPlayer.position.z)
        };

        mp.gui.emmit(`
            window.router.setView('${this.VIEW_NAME}', '${JSON.stringify(viewData)}')
        `);

        global.menuOpen();
        this.opened = true;
    }

    close() {
        if (!this.opened) 
            return;

        mp.gui.emmit(`window.router.setHud()`);
        global.menuClose();
        this.opened = false;
    }
}