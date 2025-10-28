const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const
    clientName = "client.businessmanage.",
    rpcName = "rpc.businessmanage.",
    serverName = "server.businessmanage.";

let businessStats = {}
let businessStock = []
let businessOrders = []

let getTopProd = []
let getTopPlayers = []

gm.events.add(clientName + "open", async (jsonStats, jsonStock, jsonOrders) => {
    businessStats = JSON.parse(jsonStats);
    businessStock = JSON.parse(jsonStock);
    businessOrders = JSON.parse(jsonOrders);
    await global.awaitMenuCheck ();
    global.menuOpen();
    mp.gui.emmit(
        `window.router.setView("BusinessManage")`
    );
    gm.discord(translateText("Управляет своим бизнесом"));
});

rpc.register(rpcName + "getStats", () => {
	return JSON.stringify (businessStats);
});

rpc.register(rpcName + "getOrders", () => {
	return JSON.stringify (businessOrders);
});

rpc.register(rpcName + "getStocks", () => {
	return JSON.stringify (businessStock);
});

gm.events.add(clientName + "getHistory", () => {
    callRemote(serverName + 'gethistory');
})
gm.events.add('client:setBizWaypoint', (x, y) => {
  // Ставим новую метку на карту
  mp.game.ui.setNewWaypoint(x, y);
});
gm.events.add(clientName + "setHistory", (jsonProd, jsonUser) => {
    json = JSON.parse(json);

    json.forEach((data) => {
        getTopProd.push({
            name: data[4],
            price: data[5]
        })
        getTopPlayers.push({
            uuid: (data[3]).toString(36),
            name: data[4],
            price: data[5]
        })
    })

    mp.gui.emmit(`window.listernEvent.updateBusinessHistory ();`);

})

rpc.register(rpcName + "getTopProd", () => {
    return JSON.stringify (getTopProd);
});




























gm.events.add('client.businessmanage.cancelOrder', (id) => {
	try
	{
        if(id) callRemote('server.businessmanage.cancelOrder', id);
	}
	catch (e) 
    {
        callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.cancelOrder", e.toString());
    }
})

gm.events.add('client.businessmanage.makeOrder', (id) => {
	try
	{
        if(id) callRemote('server.businessmanage.makeOrder', id);
	}
	catch (e) 
    {
        callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.makeOrder", e.toString());
    }
})

gm.events.add('client.businessmanage.changePrice', (id, price) => {
	try
	{
        if(id) callRemote('server.businessmanage.changePrice', id);
	}
	catch (e) 
    {
        callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.changePrice", e.toString());
    }
})

gm.events.add('client.businessmanage.fillStocks', (id) => {
	try
	{
        if(id) callRemote('server.businessmanage.fillStocks', id);
	}
	catch (e) 
    {
        callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.fillStocks", e.toString());
    }
})

gm.events.add('client.businessmanage.sellBiz', () => {
	try
	{
        if(id) callRemote('server.businessmanage.sellBiz');
	}
	catch (e) 
    {
        callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.sellBiz", e.toString());
    }
})