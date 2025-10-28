// wheel.cef.js
// Подключает Svelte-компонент "index.svelte" к вашему роутеру и gm.events.
// Требования: в client JS должны приходить события:
//  - client.wheel.set -> window.events.callEvent("cef.wheel.set", json)
//  - client.wheel.history -> window.events.callEvent("cef.wheel.history", json)
//  - client.wheel.historyItem -> window.events.callEvent("cef.wheel.historyItem", json)
// И уходить события:
//  - gm.events.call("client.wheel.bet", color, amount)
//  - gm.events.call("client.wheel.getHistory")
//  - gm.events.call("client.wheel.getHistoryID", id)
//  - gm.events.call("client.wheel.removePlayer")

import Wheel from './index.svelte';

// ————— Глобальные зависимости (мягкие проверки)
const gmSafe = (fn, ...args) => {
  try { return window?.gm?.events?.call?.(fn, ...args); } catch { /* noop */ }
};
const on = (name, cb) => window?.events?.addEvent?.(name, cb);

// Текущее состояние (будет передаваться в пропсы)
const state = {
  data: undefined,
  cdn: window?.cdn || 'https://cdn.majestic-files.com/public/master/static',
  serverTimeOffset: 0,
  t: window?.t || ((k)=>k),
  n: window?.n || ((v)=> (v ?? 0).toLocaleString('ru-RU')),
  testMode: false
};

let app = null;

// аккуратно обновляем пропсы svelte
function setProps(patch) {
  Object.assign(state, patch);
  if (app) {
    app.$set({
      data: state.data,
      cdn: state.cdn,
      serverTimeOffset: state.serverTimeOffset,
      t: state.t,
      n: state.n,
      testMode: state.testMode
    });
  }
}

// Монтирование/размонтирование экрана
function mountWheel(target) {
  if (app) return app;

  app = new Wheel({
    target,
    props: {
      data: state.data,
      cdn: state.cdn,
      serverTimeOffset: state.serverTimeOffset,
      t: state.t,
      n: state.n,
      testMode: state.testMode
    }
  });

  // === Svelte -> клиент JS
  app.$on('wheelBet', (e) => {
    const { amount, color } = e?.detail || {};
    if (!color || !amount) return;
    gmSafe('client.wheel.bet', color, Number(amount));
  });

  app.$on('getWheelHistory', () => gmSafe('client.wheel.getHistory'));
  app.$on('getWheelHistoryID', (e) => gmSafe('client.wheel.getHistoryID', Number(e?.detail?.id || 0)));
  app.$on('removePlayerWheel', () => gmSafe('client.wheel.removePlayer'));

  // на всякий случай сразу попросим историю (не обязательно)
  // gmSafe('client.wheel.getHistory');

  return app;
}

function unmountWheel() {
  try { app?.$destroy?.(); } finally { app = null; }
}

// === Сервер -> Svelte (через клиент JS)
// Приходит ПОЛНЫЙ снапшот состояния (таймер/колесо/ставки/история/победитель)
on('cef.wheel.set', (json) => {
  try {
    const data = typeof json === 'string' ? JSON.parse(json) : json;
    setProps({ data });
  } catch (e) { console.error('[wheel.cef] bad json for set:', e); }
});

// Список истории
on('cef.wheel.history', (json) => {
  try {
    const list = typeof json === 'string' ? JSON.parse(json) : json;
    const data = state.data || {};
    data.historyList = list;
    setProps({ data });
  } catch (e) { console.error('[wheel.cef] bad json for history:', e); }
});

// Деталка истории
on('cef.wheel.historyItem', (json) => {
  try {
    const item = typeof json === 'string' ? JSON.parse(json) : json;
    const data = state.data || {};
    data.historyItem = item;
    setProps({ data });
  } catch (e) { console.error('[wheel.cef] bad json for historyItem:', e); }
});

// === Регистрация в вашем роутере
// Открытие экрана должен вызывать client.wheel.open в клиентском JS,
// который, помимо router.setView('Wheel'), отправит на сервер server.wheel.open.
window.router?.register?.('Wheel', {
  mount: (target) => {
    const inst = mountWheel(target);
    // Если экран открыт напрямую из меню — попросим сервер прислать снапшот
    gmSafe('client.wheel.getHistory'); // опционально сразу подтянем историю
    return inst;
  },
  unmount: () => unmountWheel()
});

// На всякий случай оставим ручной API для отладки в консоли
window.WheelCEF = {
  mount: mountWheel,
  unmount: unmountWheel,
  set(data) { setProps({ data }); },
  state
};
