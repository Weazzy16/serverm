const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;

export default new class Atm {
    VIEW_NAME = "PlayerAtm"

    THEMES = {
        Dark: "dark",
        Light: "light"
    }

    constructor() {
        mp.events.add({
            "client.atm.open": this.open.bind(this),
            "client.atm.close": this.close.bind(this),
            "client.atm.set_page": this.setPage.bind(this),
            "client.atm.set_transaction": this.setTransaction.bind(this),
            "client.atm.change_page": this.tryChangePage.bind(this),
            "client.atm.action": this.action.bind(this)
        })
    }

    getTheme() {
        const currentHour = mp.game.clock.getHours();
        return currentHour > 5 && currentHour <= 22 ? this.THEMES.Light : this.THEMES.Dark;
    }

    opened = false

    open(id) {
        const viewData = {
            theme: this.getTheme(),
            id
        };

        mp.gui.emmit(`
            window.router.setView('${this.VIEW_NAME}', '${JSON.stringify(viewData)}');
        `);

        global.menuOpen();
        this.opened = true;
    }

    close() {
        if (!this.opened) return;

        mp.gui.emmit(`
            window.router.setHud() 
        `);

        global.menuClose();
        this.opened = false;
    }

    setPage(pageName) {
        mp.gui.emmit(`window.majestic_atm.setPage('${pageName}')`);
    }

    setTransaction(type, subData) {
        mp.gui.emmit(`window.majestic_atm.setTransaction('${type}', ${subData})`);
    }

    tryChangePage(pageName) {
        callRemote("server.atm.change_page", pageName);
    }

    action(type, ...values) {
        callRemote("server.atm.action", type, ...values);
    }
}