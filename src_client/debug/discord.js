const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
gm.discord = (detailedStatus) => {
    let state = "на Majestic Role Play";

    if (global.localplayer && typeof global.localplayer.remoteId !== "undefined")
        state = translateText('на Majestic Role Play под ID {0}', global.localplayer.remoteId);

    mp.discord.update(detailedStatus, state);
}

global.discordDefault = () => {
    gm.discord(translateText('Наслаждается жизнью'))
};

discordDefault ();