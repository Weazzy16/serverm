/** Находит расстояние между двумя векторами */
const callRemote = mp.events.callRemote;
const call = mp.events.call;
const callRemoteUnreliable = mp.events.callRemoteUnreliable;
const browsers = mp.browsers;
const _callRemote = mp._events.callRemote ;
const _call = mp._events.call;
export const getDistanceBetweenPoints3D = function getDistanceBetweenPoints3D(v1, v2) {
    return Math.abs(Math.sqrt(Math.pow((v2.x - v1.x), 2) + Math.pow((v2.y - v1.y), 2) + Math.pow((v2.z - v1.z), 2)));
}

/** Прогрузка анимации */
export const requestAnim = function requestAnim(dict) {
    new Promise((resolve, _) => {
        const interval = setInterval(() => {
            if(mp.game.streaming.hasAnimDictLoaded(dict)) {
                clearInterval(interval);
                resolve();
            }
            else {
                mp.game.streaming.requestAnimDict(dict);
            }
        }, 300);
    });
}