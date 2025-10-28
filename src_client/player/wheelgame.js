const callRemote = mp.events.callRemote;

mp.console.logInfo('[WheelGames] client script LOADED');

// Переменная для хранения данных wheel игры
let wheelGameData = {};

// Открытие wheel игры
gm.events.add('client.wheelgames.open', () => {
    if (!global.loggedin || global.cuffed || global.isDeath == true) return;
    
    global.menuOpen();
    mp.gui.emmit(`window.router.setView('WheelGames')`);
    callRemote('server.wheelgames.request');
    gm.discord("Играет в колесо фортуны");
});

// Закрытие
gm.events.add('client.wheelgames.close', () => {
    if (global.menuCheck()) {
        global.menuClose();
        mp.gui.emmit(`window.router.setHud();`);
    }
});

// Обновление данных от сервера - основное событие
mp.events.add('wheelgames_update', (jsonString) => {
    console.log('[WheelGames] Received update:', jsonString);
    
    try {
        wheelGameData = JSON.parse(jsonString);
        
        // Отправляем безопасно экранированный JSON в CEF
        const safeJson = jsonString
            .replace(/\\/g, '\\\\')
            .replace(/'/g, "\\'")
            .replace(/\r?\n/g, '\\n');
            
        mp.gui.emmit(`
            window.dispatchEvent(new CustomEvent('wheelgames_update', {
                detail: '${safeJson}'
            }));
        `);
        
    } catch (e) {
        mp.console.logInfo('[WheelGames] JSON parse error: ' + e.message);
    }
});
// Добавить после события wheelgames_update:
mp.events.add('wheelgames_history', (jsonString) => {
    console.log('[WheelGames] Received history:', jsonString);
    
    try {
        // Отправляем безопасно экранированный JSON в CEF
        const safeJson = jsonString
            .replace(/\\/g, '\\\\')
            .replace(/'/g, "\\'")
            .replace(/\r?\n/g, '\\n');
            
        mp.gui.emmit(`
            window.dispatchEvent(new CustomEvent('wheelgames_history', {
                detail: '${safeJson}'
            }));
        `);
        
    } catch (e) {
        mp.console.logInfo('[WheelGames] History JSON parse error: ' + e.message);
    }
});
// События от CEF к серверу
mp.events.add('client.wheelgames.bet', (amount, color) => {
    callRemote('server.wheelgames.bet', Number(amount) || 0, String(color) || 'white');
});

mp.events.add('client.wheelgames.openPage', (page, gameId) => {
    callRemote('server.wheelgames.openPage', Number(page) || 0, Number(gameId) || 0);
});

mp.events.add('client.wheelgames.history', (page, gameId) => {
    callRemote('server.wheelgames.history', Number(page) || 0, Number(gameId) || 0);
});

// RPC для получения данных из CEF
rpc.register("rpc.wheelgames.getData", () => {
    return JSON.stringify(wheelGameData);
});

mp.events.add('client.wheelgames.request', () => {
    callRemote('server.wheelgames.request');
});