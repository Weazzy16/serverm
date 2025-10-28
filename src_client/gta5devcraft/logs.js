// logs.js
import { writable } from 'svelte/store';

// сторы
export const banHistory      = writable([]);
export const criminalRecords = writable([]);

mp.events.add('client.logs.setBanHistory', (json) => {
  banHistory.set(JSON.parse(json));
});

mp.events.add('client.logs.setCriminalRecords', (json) => {
  criminalRecords.set(JSON.parse(json));
});
