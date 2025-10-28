export default new class WantedMenu {
    POPUP_NAME = "MajesticWanted"
    
    constructor() {
        this.register("open", this.open.bind(this));
        this.register("close", this.close.bind(this));
        
        this.register("cancel", this.cancel.bind(this));
        this.register("submit", this.submit.bind(this));

        mp.events.add("global:escape", this.escape.bind(this));
    }

    register(eventName, callback) {
        mp.events.add(`e-dev.wantedMenu.${eventName}`, callback);
    }

    callServer(eventName, ...args) {
        mp.events.callRemote(`e-dev.wantedMenu.${eventName}`, ...args);
    }

    opened = false;
    open() {
        mp.gui.emmit(`window.router.setPopUp('${this.POPUP_NAME}')`);
        global.menuOpen();

        this.opened = true;
    }

    close() {
        if (!this.opened)
            return;

        mp.gui.emmit(`window.router.setPopUp();`);
        global.menuClose();

        this.opened = false;
    }

    cancel() {
        this.callServer("cancel");
    }

    submit(stars, reason) {
        this.callServer("submit", stars, reason);
    }

    escape() {
        if (!this.opened)
            return;

        this.cancel();
    }
}