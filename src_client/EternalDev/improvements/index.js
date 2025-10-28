const TimerManager = new class {
    constructor() {
        this.list = {};
    }

    create(name, time, updateCallback, stopCallback) {
        if (this.list.hasOwnProperty(name))
            this.clear(name);

        const instance = setInterval(() => {
            const data = this.list[name];
            if (data == null || data.time <= 0) 
                return this.clear(name);

            data.time--;
            updateCallback(data.time);
        }, 1000);

        this.list[name] = {
            instance,
            time,

            stopCallback,
            updateCallback
        }

        return name;
    }

    clear(name) {
        const data = this.list[name];
        if (data != null && data.instance !== undefined) {
            data.stopCallback();

            clearInterval(data.instance)
            delete this.list[name];
        }
    }
}

export default new class PlayerImprovements {
    constructor() {
        gm.events.add("client.improvements.timer", (time) => this.start(time));
        gm.events.add("client.improvements.timerStop", () => this.onStop());
    }

    start(time) {
        TimerManager.create("improvement", time, 
            (time) => this.update(time), 
            () => this.onStop()
        );

        this.update(time);
    }

    update(time) {
        mp.gui.emmit(`window.hudStore.setImprovementTime(${time})`);
    }

    onStop() {
        mp.gui.emmit(`window.hudStore.setImprovementTime(-1)`)
    }
}