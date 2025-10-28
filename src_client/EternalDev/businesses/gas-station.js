const localPlayer = mp.players.local;

const callRemote = mp.events.callRemote; // ETO DOBAVIL MOLODECH

export default new class GasStation {
    VIEW_NAME = "MajesticGasStation"

    THEMES = {
        Dark: "dark",
        Light: "light"
    }

    constructor() {
        mp.events.add({
            "client.gas-station.open": this.open.bind(this),
            "client.gas-station.close": this.close.bind(this),
            "client.gas-station.buy_product": this.buyProduct.bind(this),
            "client.gas-station.buy_petrol": this.buyPetrol.bind(this),

            "client.gas-station.fill_petrol": this.fillPetrol.bind(this),
            "client.gas-station.end_filling": this.endFilling.bind(this)
        })
    }

    opened = false

    open(data) {
        data = JSON.parse(data);

        data.Station.Street = global.getAreaName(localPlayer.position.x, localPlayer.position.y, localPlayer.position.z);
        
        if (data.CarData != null) {
            data.CarData.Model = mp.game.vehicle.getDisplayNameFromVehicleModel(data.CarData.Model);
            if (data.CarData.Name == null)
                data.CarData.Name = data.CarData.Model;
        }

        const viewData = {
            theme: this.getTheme(),
            ...data
        };

        mp.gui.emmit(`
            window.router.setView('${this.VIEW_NAME}', '${JSON.stringify(viewData)}');
        `);
        
        global.menuOpen();
        this.opened = true;
    }

    getTheme() {
        const currentHour = mp.game.clock.getHours();
        return currentHour > 5 && currentHour <= 22 ? this.THEMES.Light : this.THEMES.Dark;
    }

    close() {
        if (this.isFilling || !this.opened) 
            return;
        
        mp.gui.emmit(`
            window.router.setHud() 
        `);

        global.menuClose();
        this.opened = false;
    }

    buyPetrol(productName, count, paymentType) {
        callRemote("server.gas-station.buy_petrol", productName, count, paymentType); // ZAMENIT TUTA
    }

    buyProduct(productName, paymentType) {
        callRemote("server.gas-station.buy_product", productName, paymentType); // ZAMENI TUTA, KAK YA SDELAL VISHE
    }
    
    isFilling = false;
    fillPetrol(value) {
        mp.gui.emmit(`window.gasStation_startAnimation(${value})`);
        this.isFilling = true;
    }

    endFilling() {
        this.isFilling = false;
        this.close();
    }
}