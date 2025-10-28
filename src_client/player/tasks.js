const callRemote = mp.events.callRemote;

// 1) Слушаем данные для вкладки «Доступные»
mp.events.add('calendar_tasks_update', (json) => {
    console.log('[RAGE] calendar_tasks_update →', json);
    const safe = json
      .replace(/\\/g, '\\\\')
      .replace(/'/g, "\\'")
      .replace(/\r?\n/g, '\\n');
    mp.gui.emmit(`
      window.dispatchEvent(new CustomEvent('calendar_tasks_update', {
        detail: '${safe}'
      }));
    `);
});

// 2) Слушаем данные для календаря
mp.events.add('item_calendar_update', (json) => {
    console.log('[RAGE] item_calendar_update →', json);
    const safe = json
      .replace(/\\/g, '\\\\')
      .replace(/'/g, "\\'")
      .replace(/\r?\n/g, '\\n');
    mp.gui.emmit(`
      window.dispatchEvent(new CustomEvent('item_calendar_update', {
        detail: '${safe}'
      }));
    `);
});

// 3) Когда CEF просит прогресс для вкладки «Доступные»
mp.events.add('client.calendar.request', () => {
    console.log('[RAGE] client.calendar.request');
    callRemote('server:calendar:request');
});

// 4) Когда CEF просит расписание месяца (для календаря)
mp.events.add('client.calendar.requestMonth', (monthIndex) => {
    console.log('[RAGE] client.calendar.requestMonth →', monthIndex);
    callRemote('server:calendar:requestMonth', monthIndex);
});

// 5) Когда CEF хочет забрать награду (и для вкладки «Доступные», и для календаря)
mp.events.add('client.calendar.takeMonthReward', (month, day) => {
  callRemote('server:calendar:takeMonthReward', month, day);
});

mp.events.add('client.calendar.takeMonthReward', (month, day) => {
  console.log('[RAGE] client.calendar.takeMonthReward →', month, day);
  callRemote('server.calendar.takeMonthReward', month, day);
});

// 5) When the user clicks “Купить день” on the calendar:
mp.events.add('client.calendar.buyDay', (month, day) => {
  console.log('[RAGE] client.calendar.buyDay →', month, day);
  callRemote('server.calendar.buyDay', month, day);
});