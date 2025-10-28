// client/player/achievements.js
const callRemote = mp.events.callRemote;

// Сервер → CEF
// client/player/achievements.js

// 1) Когда сервер присылает прогресс:
mp.events.add('achievements_update', (payload) => {
  console.log('[RAGE] achievements_update →', payload);
  const safe = payload
    .replace(/\\/g, '\\\\')
    .replace(/'/g, "\\'")
    .replace(/\r?\n/g, '\\n');

  // Форвардим в CEF:
  mp.gui.emmit(`
    window.dispatchEvent(new CustomEvent('showAchievements', {
      detail: JSON.parse('${safe}')
    }));
  `);
});

// 2) Когда CEF запросил список:
mp.events.add('client.achievements.request', () => {
  console.log('[RAGE] client.achievements.request');
  callRemote('server:achievement:request');
});

// 3) Когда CEF запросил награду:
mp.events.add('client.achievements.getAward', (id) => {
  console.log('[RAGE] client.achievements.getAward →', id);
  callRemote('server:achievement:reward', id);
});


