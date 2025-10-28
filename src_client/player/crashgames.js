const callRemote = mp.events.callRemote;

mp.console.logInfo('[CrashGames] client script LOADED');

// Переменная для хранения данных краш-игры
let crashGameData = {};

// Открытие краш игры
// Добавьте в crashgames.js:
gm.events.add('client.crashgames.open', () => {
    if (!global.loggedin || global.cuffed || global.isDeath == true) return;
    
    global.menuOpen();
    mp.gui.emmit(`window.router.setView('CrashGames')`);
    callRemote('server.crashgames.request');
    gm.discord("Играет в Краш");
});

// Закрытие
gm.events.add('client.crashgames.close', () => {
    if (global.menuCheck()) {
        global.menuClose();
        mp.gui.emmit(`window.router.setHud();`);
    }
});

// Обновление данных от сервера - основное событие
mp.events.add('crashgames_update', (jsonString) => {
    console.log('[CrashGames] Received update:', jsonString);
    
    try {
        crashGameData = JSON.parse(jsonString);
        
        // Отправляем безопасно экранированный JSON в CEF
        const safeJson = jsonString
            .replace(/\\/g, '\\\\')
            .replace(/'/g, "\\'")
            .replace(/\r?\n/g, '\\n');
            
        mp.gui.emmit(`
            window.dispatchEvent(new CustomEvent('crashgames_update', {
                detail: '${safeJson}'
            }));
        `);
        
    } catch (e) {
        mp.console.logInfo('[CrashGames] JSON parse error: ' + e.message);
    }
});

// События от CEF к серверу
mp.events.add('client.crashgames.bet', (amount, autoX) => {
    callRemote('server.crashgames.bet', Number(amount) || 0, Number(autoX) || 1.0);
});
mp.events.add('client.crashgames.take', () => {
    callRemote('server.crashgames.take'); // убрать все параметры
});

mp.events.add('client.crashgames.openPage', (page, gameId) => {
    callRemote('server.crashgames.openPage', Number(page) || 0, Number(gameId) || 0);
});

// RPC для получения данных из CEF
rpc.register("rpc.crashgames.getData", () => {
    return JSON.stringify(crashGameData);
});
mp.events.add('client.crashgames.request', () => {
    callRemote('server.crashgames.request');
});