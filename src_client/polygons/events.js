const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
setInterval(() => {
    try {
        if (!global.loggedin) return;
        const { position, dimension } = global.localplayer;

        mp.polygons.pool.map((polygon) => {
            if (polygon.colliding) {
                if (!mp.polygons.isPositionWithinPolygon(position, polygon, dimension)) {
                    polygon.colliding = false;
                    call("playerLeavePolygon", polygon);
                }
            } else {
                if (mp.polygons.isPositionWithinPolygon(position, polygon, dimension)) {
                    polygon.colliding = true;
                    call("playerEnterPolygon", polygon);
                }
            }
        });
    } catch (e) {
        if (new Date().getTime() - global.trycatchtime["polygons/events"] < 5000) return;
        global.trycatchtime["polygons/events"] = new Date().getTime();
        callRemote("client_trycatch", "polygons/events", "setInterval", e.toString());
    }
}, 500);