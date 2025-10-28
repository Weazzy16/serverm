// src_client/logs.js
console.log('logs.js загружен');


mp.events.add('client.logs.setBanHistory', json => {
  console.log('→ получил client.logs.setBanHistory:', json);
  try { banHistory.set(JSON.parse(json)); }
  catch(e){ console.error('banHistory parse error', e, json); }
});

mp.events.add('client.logs.setCriminalRecords', json => {
  console.log('→ получил client.logs.setCriminalRecords:', json);
  try { criminalRecords.set(JSON.parse(json)); }
  catch(e){ console.error('criminalRecords parse error', e, json); }
});
