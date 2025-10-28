const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
const eventAdd = mp._events.add;

global.gm = {};

gm.events = {};
gm.events.add = (eventName, handler) => {
    eventAdd(eventName, function () {
        try {
            const argsResult = handler.apply(null, arguments);
            if (argsResult instanceof Promise)
                argsResult.catch(e => global.crushLog("eventAdd.1", eventName, e.stack));
        } catch (e) {
            global.crushLog("eventAdd.2", eventName, e.stack);
        }
    });
}


/*
const eventRemove = mp._events.remove;

mp.events.remove = (eventName, args) => {
    return eventRemove(eventName, function () {
        try {
            const argsResult = args.apply(null, arguments);
            if (argsResult instanceof Promise)
                argsResult.catch(e => crushLog("eventRemove.1", eventName, e.stack));
        } catch (e) {
            crushLog("eventRemove.2", eventName, e.stack);
        }
    });
}*/