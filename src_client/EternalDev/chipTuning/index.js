const call = mp.events.call;
const callRemote = mp.events.callRemote;

const localPlayer = mp.players.local;

export default new class ChipTuning {
    VIEW_NAME = "ChipTuning"

    constructor() {
        this.register("open", this.open.bind(this));
        this.register("close", this.close.bind(this));
        this.register("update", this.update.bind(this));
        this.register("buy", this.buy.bind(this));

        this.register("set_handling", (handlingName, value) => {
            this.setHandling(this.vehicle, handlingName, value);
        });

        mp.events.add("entityStreamIn", this.syncVehicle.bind(this));
        mp.events.add("playerEnterVehicle", this.onEnterVehicle.bind(this));
        mp.events.add("playerLeaveVehicle", this.onLeaveVehicle.bind(this));

        mp.events.addDataHandler("e-dev.chipTuning.handlings", (entity, data) => this.syncVehicle(entity));

        global.binderFunctions.SwitchStabilization = () => this.onBind_SwitchStabilization();
    }

    register(eventName, callBack) {
        mp.events.add(`e-dev.chipTuning.${eventName}`, callBack);
    } 

    get vehicle() {
        return localPlayer.vehicle;
    }

    isStabilizationEnabled = false
    onBind_SwitchStabilization() {
        if (!this.vehicle)
            return;
        
        if (!this.hasController("StabilizationSwitching")) {
            call("notify", 1, 9, "У Вас не установлен переключатель стабилизации Т/С", 3000);
            return;
        }

        this.isStabilizationEnabled = !this.isStabilizationEnabled;
        this.vehicle.setReduceGrip(this.isStabilizationEnabled);  

        call("notify", 2, 9, this.isStabilizationEnabled ? "Вы отключили стабилизацию" : "Вы включили стабилизацию", 3000);
    }

    onLeaveVehicle(vehicle) {
        this.isStabilizationEnabled = false;
         
        if (vehicle)
            vehicle.setReduceGrip(false);
    }

    onEnterVehicle(vehicle, seat) {
        vehicle.setReduceGrip(false);
        this.syncVehicle(vehicle);
    }

    opened = false
    async open(handlingList, vehicleHandlings, vehicleControllers, vehiclePrice) {    
        this.vehicleHandlings = JSON.parse(vehicleHandlings);
        this.vehicleControllers = JSON.parse(vehicleControllers);

        this.vehiclePrice = vehiclePrice;

        this.currentHandlings = await this.getVehicleHandlings(JSON.parse(handlingList), this.vehicleHandlings);
        const viewData = {
            vehicleHandlings: this.currentHandlings,
            vehicleControllers: this.vehicleControllers,
            vehiclePrice: this.vehiclePrice
        };

        mp.gui.emmit(`window.router.setPopUp('${this.VIEW_NAME}', '${JSON.stringify(viewData)}')`);
        global.menuOpen(false);

        call("client:OnBrowserInit");
        this.opened = true;
    }

    async tryClose() {
        if (!this.opened)
            return;

        this.opened = false;

        var vehicle = this.vehicle;

        global.FadeScreen(true, 600);
        await global.wait(600);

        for (const [handlingName, { value: val }] of Object.entries(this.currentHandlings)) {
            if (vehicle != null)
                this.setHandling(vehicle, handlingName, val);
        }

        callRemote("e-dev.chipTuning.close");
    }

    close() {
        global.FadeScreen(false, 600);

        mp.gui.emmit(`window.router.setHud()`);
        global.menuClose();

        this.opened = false;
        this.syncVehicle(this.vehicle);
    }

    update(handlingName, value) {
        this.setHandling(this.vehicle, handlingName, value);
    }

    setHandling(vehicle, handlingName, value) {
        if (vehicle == null || !vehicle.handle || !mp.vehicles.exists(vehicle))
            return;

        value = parseFloat(value);
        switch(handlingName) {
            // Развал колес
            case "wheelCamber":
                vehicle.setWheelCamber(255, value);
                break;
            // Высота подвески
            case "wheelHeight":
                vehicle.setWheelHeight(255, value)
                break;
            // Ширина колесной базы
            case "wheelTrackWidth":
                vehicle.setWheelTrackWidth(255, value);
                break;
            // Диаметр колес
            case "wheelRadius":
                vehicle.setWheelRadius(value);
                break;
            // Ширина колес
            case "wheelWidth":
                vehicle.setWheelWidth(value)
                break;
            default: 
                vehicle.setHandling(handlingName, value);
                break;
        }
    }

    buy(type, ...args) {
        callRemote(`e-dev.chipTuning.buy_${type}`, ...args);
    }

    hasController(controller) {
        const data = this.vehicle.getVariable("e-dev.chipTuning.controllers");
        if (data == null)
            return false;

        const controllers = JSON.parse(data);
        return controllers[controller] == true;
    }

    async getVehicleHandlings(handlings, vehicleHandlings) {
        if (this.vehicle == null)
            return;

        var vehicle = mp.vehicles.new(this.vehicle.model, new mp.Vector3(this.vehicle.position.x, this.vehicle.position.y, this.vehicle.position.z - 15), {
			color: [[0, 0, 0], [0, 0, 0]],
            numberPlate: "ETERNAL",
            dimension: this.vehicle.dimension
        });

        while (!vehicle || !vehicle.handle || !mp.vehicles.exists(vehicle)) {
            await global.wait(1)
        }

        // vehicle.setWheelType(this.vehicle.wheelType);
        vehicle.setMod(23, this.vehicle.getMod(23));

        await global.wait(100);

        mp.console.logInfo(`v: ${vehicle.getWheelRadius()}; v2: ${this.vehicle.getWheelRadius()}`)

        const result = {};

        for (const handlingName of handlings) {
            let defaultValue = vehicle.getHandling(handlingName);
            switch(handlingName) {
                case "wheelCamber": defaultValue = vehicle.getWheelCamber(0); break;
                case "wheelHeight": defaultValue = vehicle.getWheelHeight(0); break;
                case "wheelTrackWidth": defaultValue = vehicle.getWheelTrackWidth(0); break;
                case "wheelRadius": defaultValue = vehicle.getWheelRadius(); break;
                case "wheelWidth": defaultValue = vehicle.getWheelWidth(); break;
            }

            const value = vehicleHandlings[handlingName] || defaultValue;
            
            result[handlingName] = {
                value,
                values: [value],
                default: defaultValue
            };
        }

        vehicle.destroy();
        return result;
    }

    async getDefaultHandling(vehicleModel, handlingName) {
        const position = localPlayer.position;
        var vehicle = mp.vehicles.new(vehicleModel, new mp.Vector3(position.x, position.y, position.z - 15), {
            dimension: localPlayer.dimension
        });

        while (!vehicle || !vehicle.handle || !mp.vehicles.exists(vehicle)) {
            await global.wait(1)
        }

        const handlingValue = vehicle.getHandling(handlingName);
        vehicle.destroy();

        return handlingValue;
    }

    syncVehicle(vehicle) {
        if (vehicle == null || !vehicle.handle || vehicle.type != "vehicle" || !mp.vehicles.exists(vehicle))
            return;

        const data = vehicle.getVariable("e-dev.chipTuning.handlings");
        if (data != null) {
            const handlings = JSON.parse(data);

            for (const [handlingName, value] of Object.entries(handlings)) {
                this.setHandling(vehicle, handlingName, Number(value));
            }
        }
    }
}