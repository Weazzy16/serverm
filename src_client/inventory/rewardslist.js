const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.events.add('client.rewardslist.day.load', () => {
    callRemote("server.rl.day.load");
});

gm.events.add('client.rewardslist.day.init', (json) => {

    json = JSON.parse(json);

    let awards = [];

    json.forEach((item) => {
        awards.push({
            day: item[0],
            //title: item[1],
            desc: item[1],
            type: item[2],
            itemId: item[3],
            data: item[4],
            image: item[5],
            status: item[6],
            time: item[7],
        })
    })

    mp.gui.emmit(`window.listernEvent ('rewardList.day.init', '${JSON.stringify(awards)}');`);
});

gm.events.add('client.rewardslist.day.take', (slotId) => {
    if (!global.antiFlood("rl_day", 250))
        return true;

    callRemote("server.rl.day.take", slotId);
});

//Донат

gm.events.add('client.rewardslist.donate.load', () => {
    callRemote("server.rl.donate.load");
});

gm.events.add('client.rewardslist.donate.init', (json) => {

    json = JSON.parse(json);

    let awards = [];

    json.forEach((item) => {
        awards.push({
            index: item[0],
            //title: item[1],
            desc: item[1],
            type: item[2],
            itemId: item[3],
            data: item[4],
            image: item[5],
            status: item[6],
            donate: item[7],
            maxDonate: item[8],
        })
    })

    mp.gui.emmit(`window.listernEvent ('rewardList.donate.init', '${JSON.stringify(awards)}');`);
});

gm.events.add('client.rewardslist.donate.take', (slotId) => {
    if (!global.antiFlood("rl_donate", 250))
        return true;

    callRemote("server.rl.donate.take", slotId);
});