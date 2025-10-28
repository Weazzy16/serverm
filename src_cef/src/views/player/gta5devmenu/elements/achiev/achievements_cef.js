// achievements_cef.js

if (!window.events) {
    window.events = {
        _events: {},
        addEvent(name, handler) {
            this._events[name] = handler;
        },
        callEvent(name, ...args) {
            if (typeof this._events[name] === 'function')
                this._events[name](...args);
        }
    };
}

// Приходит с clientside через mp.gui.execute(...)
window.events.addEvent('cef.achievements.update', (json) => {
    let data;
    try {
        data = JSON.parse(json);
    } catch (e) {
        console.error('Achievement JSON parse error:', json);
        return;
    }
    if (typeof window.updateAchievements === 'function') {
        window.updateAchievements(data);
    } else {
        console.log('Achievements data:', data);
    }
});

// Для награды — CEF вызывает clientside!
if (!window.achievements) window.achievements = {};
window.achievements.getAward = function(id) {
    if (typeof mp !== 'undefined' && mp.events && mp.events.call) {
        mp.events.call('client.achievements.getAward', id);
    }
};
