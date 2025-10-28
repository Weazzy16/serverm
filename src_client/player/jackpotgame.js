const callRemote = mp.events.callRemote;

mp.console.logInfo('[Jackpot] client script LOADED');

// Переменная для хранения данных джекпот-игры
let jackpotGameData = {};

// Открытие джекпот игры
gm.events.add('client.jackpot.open', () => {
    if (!global.loggedin || global.cuffed || global.isDeath == true) return;
    
    global.menuOpen();
    mp.gui.emmit(`window.router.setView('JackpotGames')`);
    callRemote('server.jackpot.request');
    gm.discord("Играет в Джекпот");
});

// Закрытие
gm.events.add('client.jackpot.close', () => {
    if (global.menuCheck()) {
        global.menuClose();
        mp.gui.emmit(`window.router.setHud();`);
    }
});

// Обновление данных от сервера - основное событие
mp.events.add('jackpot_update', (jsonString) => {
    console.log('[Jackpot] Received update:', jsonString);
    
    try {
        jackpotGameData = JSON.parse(jsonString);
        
        // Отправляем безопасно экранированный JSON в CEF
        const safeJson = jsonString
            .replace(/\\/g, '\\\\')
            .replace(/'/g, "\\'")
            .replace(/\r?\n/g, '\\n');
            
        mp.gui.emmit(`
            window.dispatchEvent(new CustomEvent('jackpot_update', {
                detail: '${safeJson}'
            }));
        `);
        
    } catch (e) {
        mp.console.logInfo('[Jackpot] JSON parse error: ' + e.message);
    }
});

// Обновление истории от сервера
mp.events.add('jackpot_history', (jsonString) => {
    console.log('[Jackpot] Received history:', jsonString);
    
    try {
        const historyData = JSON.parse(jsonString);
        
        // Отправляем безопасно экранированный JSON в CEF
        const safeJson = jsonString
            .replace(/\\/g, '\\\\')
            .replace(/'/g, "\\'")
            .replace(/\r?\n/g, '\\n');
            
        mp.gui.emmit(`
            window.dispatchEvent(new CustomEvent('jackpot_history', {
                detail: '${safeJson}'
            }));
        `);
        
    } catch (e) {
        mp.console.logInfo('[Jackpot] History JSON parse error: ' + e.message);
    }
});

// События от CEF к серверу
mp.events.add('client.jackpot.bet', (amount) => {
    callRemote('server.jackpot.bet', Number(amount) || 0);
});

mp.events.add('client.jackpot.openPage', (page, gameId) => {
    callRemote('server.jackpot.openPage', Number(page) || 0, Number(gameId) || 0);
});

mp.events.add('client.jackpot.history', (page, gameId) => {
    callRemote('server.jackpot.history', Number(page) || 0, Number(gameId) || 0);
});

// RPC для получения данных из CEF
rpc.register("rpc.jackpot.getData", () => {
    return JSON.stringify(jackpotGameData);
});

mp.events.add('client.jackpot.request', () => {
    callRemote('server.jackpot.request');
});