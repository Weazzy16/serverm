// logs.js
import { writable } from 'svelte/store';

// сторы
export const banHistory      = writable([]);
export const criminalRecords = writable([]);

mp.events.add('client.logs.setBanHistory', (json) => {
  console.log('→ client.logs.setBanHistory:', json);
  let data;
  try {
    data = JSON.parse(json);
  } catch (e) {
    console.error('Ошибка парсинга JSON:', e, json);
    return;
  }
  banHistory.set(data);
});


mp.events.add('client.logs.setCriminalRecords', (json) => {
  criminalRecords.set(JSON.parse(json));
});
