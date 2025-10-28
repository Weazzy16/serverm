const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.ipad.truck.",
    rpcName = "rpc.ipad.truck.",
    serverName = "server.ipad.truck.";

gm.events.add(clientName + "load", () => {
    callRemote(serverName + "load");
});

let selectedOrders = {};
let orders = [];

gm.events.add(clientName + "init", (_selectedOrders, _orders) => {
    _selectedOrders = JSON.parse(_selectedOrders);

    selectedOrders = {};

    const playerPos = mp.players.local.position;

    if (_selectedOrders && _selectedOrders.length) {
        selectedOrders.uid = _selectedOrders[0];
        selectedOrders.name = _selectedOrders[1];

        selectedOrders.pos = new mp.Vector3(_selectedOrders[2], _selectedOrders[3], _selectedOrders[4]);
        selectedOrders.dist = Math.round(global.vdist2(selectedOrders.pos, playerPos, true));

        selectedOrders.aStreet = global.getStreetName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z);
        selectedOrders.aArea = global.getAreaName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z);
        selectedOrders.area = global.getStreetName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z) + " - " + global.getAreaName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z);
    }

    orders = [];
    _orders = JSON.parse(_orders);
    _orders.forEach((item) => {
        const order = {};

        order.uid = item[0];
        order.name = item[1];
        order.price = item[2];

        order.pos = new mp.Vector3(item[3], item[4], item[5]);
        order.dist = Math.round(global.vdist2(order.pos, playerPos, true));

        order.aStreet = global.getStreetName(order.pos.x, order.pos.y, order.pos.z);
        order.aArea = global.getAreaName(order.pos.x, order.pos.y, order.pos.z);
        order.area = global.getStreetName(order.pos.x, order.pos.y, order.pos.z) + " - " + global.getAreaName(order.pos.x, order.pos.y, order.pos.z);

        orders.push(order);
    });

    mp.gui.emmit(`window.listernEvent ('ipadTruckerLoad');`);

});

rpc.register(rpcName + "getSelect", () => {
    return JSON.stringify(selectedOrders);
});

rpc.register(rpcName + "getList", (count) => {
    let ordersList = [];

    for(let i = 0; i < count; i++) {
        if (orders [i])
            ordersList.push(orders [i]);
    }
    return JSON.stringify (ordersList);
});

gm.events.add(clientName + "take", (uid) => {
    callRemote(serverName + "take", uid);
});

gm.events.add(clientName + "cancel", () => {
    callRemote(serverName + "cancel");
});