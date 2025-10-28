export default new class ArrestMenu {
    POPUP_NAME = "MajesticArrest"
    
    constructor() {
        this.register("open", this.open.bind(this));
        this.register("close", this.close.bind(this));
        
        this.register("cancel", this.cancel.bind(this));
        this.register("submit", this.submit.bind(this));

        mp.events.add("global:escape", this.escape.bind(this));
    }

    register(eventName, callback) {
        mp.events.add(`e-dev.arrestMenu.${eventName}`, callback);
    }

    callServer(eventName, ...args) {
        mp.events.callRemote(`e-dev.arrestMenu.${eventName}`, ...args);
    }

    opened = false;
    open(data) {
        mp.gui.emmit(`window.router.setPopUp('${this.POPUP_NAME}', '${data}')`);
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

    submit(time, canPledge, pldege, reason) {
        this.callServer("submit", time, canPledge, pldege, reason);
    }

    escape() {
        if (!this.opened)
            return;

        this.cancel();
    }
}