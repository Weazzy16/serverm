import { majesticCef } from "../intro";

const callRemote = mp.events.callRemote;
export default new class ExChangeProps {
    constructor() {
        this.register("open", this.open.bind(this));
        this.register("close", this.close.bind(this));
        this.register("cancel", this.cancel.bind(this));
        this.register("success", this.success.bind(this));
        this.register("changeData", this.tryUpdate.bind(this));
        this.register("update", this.update.bind(this));
    }

    register(eventName, callback) {
        gm.events.add(`client.exChange.${eventName}`, callback);
    }

    open(name, isOwner, data) {
        global.menuOpen();
        majesticCef.execute(`exChangeProps.Open('${name}', ${isOwner}, ${data})`);
    }

    close() {
        global.menuClose();
        majesticCef.execute(`exChangeProps.Reset()`);
    }

    update(data) {
        majesticCef.execute(`exChangeProps.SetTradeData(${data})`);
    }

    tryUpdate(type, index, isReady, surcharge) {
        callRemote("server.exChange.changeData", Number(type), Number(index), isReady, Number(surcharge));
    }

    success() {
        callRemote("server.exChange.success");
    }

    cancel() {
        callRemote("server.exChange.cancel");
    }
}