const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;

const
    clientName = "client.ipad.cars.",
    rpcName = "rpc.ipad.cars.",
    serverName = "server.ipad.cars.";

let vehiclesList = [];
let carlist = []
let inGarage = false;
let isOwner = false;

gm.events.add(clientName + "load", () => {
    callRemote(serverName + "load");
});


const headerName = (isSell) => {
    if (typeof isSell !== "number" && !isSell) {
        return isOwner ? translateText("Подселённого") : translateText("Домовладельца");
    }

    return translateText("Личный");
}

let filterData = [];

mp.events.add("client.gta5devmenucars", () => {
    mp.gui.emmit(`window.gta5devmenucars(${carlist},${inGarage},${isOwner});`);
});

gm.events.add(clientName + "init", (vehiclesJson, _inGarage, _isOwner) => {
    filterData = [];

    inGarage = _inGarage;
    isOwner = _isOwner;

    carlist = JSON.stringify(vehiclesJson);

    vehiclesJson = JSON.parse(vehiclesJson);

    vehiclesList = [];
    vehiclesJson.forEach((item) => {
        if (item[0] === "rent") {
            vehiclesList.push({
                isRent: true,
                model: item[1],
                number: item[2],
                date: item[3],
                rentPrice: item[4],
                isJob: item[5],
                header: translateText("Аренда"),
            })

            if (!filterData.includes(translateText("Аренда")))
                filterData.push(translateText("Аренда"))
        } else {
            vehiclesList.push({
                sqlId: item[0],
                model: item[1],
                number: item[2],
                isCarGarage: item[3],
                place: item[4],
                ticket: item[5],
                isAir: item[6],
                isCreate: item[7],
                color: item[8],
                sell: item[9],
                header: headerName (item[9])
            })
            if (!filterData.includes(headerName (item[9])))
                filterData.push(headerName (item[9]))
        }
    });
    mp.gui.emmit(`window.listernEvent ('ipadCarsLoad')`)
});

gm.events.add(clientName + "error", () => {
    mp.gui.emmit(`window.listernEvent ('ipadCarsLoad')`)
});

rpc.register(rpcName + "filterData", () => {
    return JSON.stringify(filterData);
});

rpc.register(rpcName + "getCarsList", () => {
    return JSON.stringify(vehiclesList);
});

rpc.register(rpcName + "inGarage", () => {
    return !!inGarage;
});

rpc.register(rpcName + "isOwner", () => {
    return !!isOwner;
});