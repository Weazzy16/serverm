<script>
  import { onMount, onDestroy, createEventDispatcher } from 'svelte';
  import AccountComponent from './components/AccountComponent.svelte';

  // === Входные параметры ===
  export let data = {};
  export let cdn = 'https://cdn.majestic-files.com/public/master/static';
  export let accountId = null;

  const dispatch = createEventDispatcher();

  // === Локальный state ===
  let placeBetAmount = 0;
  let placeBetFocus = false;
  let multiplier = 1;
  let multiplierFocus = false;
  let multiplierValue = 'X1.00';
  const multipliers = [1.5, 2, 3, 4, 5];
  let selectedMultiplier = null;
  let alreadyClickToTookWinnings = false;

  // --- История (RAM) ---
  const HISTORY_BAR_LIMIT = 28;
  let volatileHistory = [];
  function extractCrashVal(h){
    if (typeof h === 'number' && isFinite(h)) return h;
    if (!h || typeof h !== 'object') return null;
    const keys = ['crashedAt','crashed','crashAt','crash','exitX','x','X','value','val','multiplier','m'];
    for (const k of keys){
      const v = Number(h[k]); if (isFinite(v) && v > 0) return v;
    }
    return null;
  }
  function normalizeHistory(list){
    if (!Array.isArray(list)) return [];
    const out=[]; for (const item of list){
      const v = extractCrashVal(item);
      if (v!=null) out.push({ id:(item?.id ?? item?.gameId ?? item?.gid ?? null), val:v, raw:item });
    }
    return out;
  }
  function pushVolatile(x){
    const v = Number(x); if (!isFinite(v) || v<=0) return;
    volatileHistory.unshift({ id:null, val:v, raw:v });
    if (volatileHistory.length > HISTORY_BAR_LIMIT) volatileHistory.length = HISTORY_BAR_LIMIT;
  }

  // === Canvas ===
  let gameInfo; let canvas; let ctx; let raf=null;
  let planeImg; let planeReady=false;
  let path=[]; let crashValue=1; let airplaneRoll=0;
  let lastGameId=null; let lastStartAt=0; let gameStartMs=0;

  function getServerTimeWithOffset(){ return Date.now() + (data?.main?.serverTimeOffset ?? 0); }
  const vh = (px)=> (px/1080) * (canvas ? (canvas.height/(window.devicePixelRatio||1)) : 1080);

  function changeSizeGameInfo(){ if (gameInfo?.style) gameInfo.style.height = (0.64044944 * gameInfo.offsetWidth) + 'px'; }
  function setupCanvas(){
    if (!canvas) return;
    const dpr = window.devicePixelRatio || 1;
    const rect = (gameInfo?.getBoundingClientRect?.() || canvas.getBoundingClientRect());
    const w = Math.max(1, rect.width), h = Math.max(1, rect.height);
    canvas.style.width = w+'px'; canvas.style.height = h+'px';
    canvas.width = Math.floor(w*dpr); canvas.height = Math.floor(h*dpr);
    ctx = canvas.getContext('2d'); ctx.setTransform(dpr,0,0,dpr,0,0);
  }

  // ====== Реактивная струя / видео ======
  const jetOrder = ['Start','Purple','Pink','Green','Yellow'];
  const jetSrc = {
    Start:  `${cdn}/img/panelMenu/miniGames/crash/jets/start.webm`,
    Purple: `${cdn}/img/panelMenu/miniGames/crash/jets/purple.webm`,
    Pink:   `${cdn}/img/panelMenu/miniGames/crash/jets/pink.webm`,
    Green:  `${cdn}/img/panelMenu/miniGames/crash/jets/green.webm`,
    Yellow: `${cdn}/img/panelMenu/miniGames/crash/jets/yellow.webm`,
    StartTransPurple: `${cdn}/img/panelMenu/miniGames/crash/jets/start_trans_purple.webm`,
    PurpleTransPink:  `${cdn}/img/panelMenu/miniGames/crash/jets/purple_trans_pink.webm`,
    PinkTransGreen:   `${cdn}/img/panelMenu/miniGames/crash/jets/pink_trans_green.webm`,
    GreenTransYellow: `${cdn}/img/panelMenu/miniGames/crash/jets/green_trans_yellow.webm`,
    Crash: `${cdn}/img/panelMenu/miniGames/crash/crash.webm`,
  };
  const jets={};
  function preloadJet(key, loop){
    if (jets[key]) return jets[key];
    const v=document.createElement('video');
    v.src=jetSrc[key]; v.loop=!!loop; v.muted=true; v.playsInline=true; v.preload='auto';
    v.addEventListener('canplay', ()=>{ try{ if(loop) v.play(); }catch{} });
    jets[key]=v; return v;
  }
  function ensureJets(){
    preloadJet('Start',true); preloadJet('Purple',true); preloadJet('Pink',true); preloadJet('Green',true); preloadJet('Yellow',true);
    preloadJet('StartTransPurple',false); preloadJet('PurpleTransPink',false); preloadJet('PinkTransGreen',false); preloadJet('GreenTransYellow',false);
    preloadJet('Crash',false);
  }
  function transKey(fromIdx,toIdx){
    const from=jetOrder[fromIdx], to=jetOrder[toIdx];
    const map={
      'Start->Purple':'StartTransPurple',
      'Purple->Pink':'PurpleTransPink',
      'Pink->Green':'PinkTransGreen',
      'Green->Yellow':'GreenTransYellow',
    };
    return map[`${from}->${to}`];
  }
  let currentJetIdx=0, currentVideo=null, playingTransition=false, boomVideo=null;
  function advanceJet(desiredIdx){
    if (playingTransition) return;
    if (desiredIdx===currentJetIdx){ currentVideo = jets[jetOrder[currentJetIdx]]; return; }
    if (desiredIdx===currentJetIdx+1){
      const key=transKey(currentJetIdx, desiredIdx), trans=jets[key];
      if (!trans){ currentJetIdx=desiredIdx; currentVideo=jets[jetOrder[currentJetIdx]]; return; }
      playingTransition=true; currentVideo=trans; trans.currentTime=0; trans.loop=false;
      try{ trans.play(); }catch{}
      trans.onended=()=>{
        playingTransition=false; currentJetIdx=desiredIdx;
        const loopVid = jets[jetOrder[currentJetIdx]];
        currentVideo=loopVid; try{ loopVid.currentTime=0; loopVid.play(); }catch{}
      };
    } else {
      currentJetIdx=desiredIdx; currentVideo=jets[jetOrder[currentJetIdx]];
    }
  }
  function colorId(x,hist=false){
    if (!hist && x<2) return 0;
    if ((!hist && x<3) || (hist && x<3)) return 1;
    if (x<5) return 2; if (x<10) return 3; return 4;
  }
  const colors = [
    { text:'#616161', background:'linear-gradient(180deg,#171717 0%,#A3A3A3 676.39%)' },
    { text:'#A359FF', background:'linear-gradient(180deg,#171717 0%,#A359FF 676.39%)' },
    { text:'#E81C5A', background:'linear-gradient(180deg,#171717 0%,#E81C5A 676.39%)' },
    { text:'#3DE7A5', background:'linear-gradient(180deg,#171717 0%,#3DE7A5 676.39%)' },
    { text:'#5F4613', background:'linear-gradient(180deg,#F2BF3B 0%,#F29E3B 118.29%)' }
  ];

  function draw(){
    if (!ctx || !canvas) return;
    ensureJets();

    const dpr=window.devicePixelRatio||1;
    const w=canvas.width/dpr, h=canvas.height/dpr;
    ctx.clearRect(0,0,w,h);

    const startX=vh(50), startY=h - vh(94);
    const tWidth=w - vh(149), tHeight=h - vh(289);

    // --- СПРАВА: шкала X ---
    const drawRightScale = () => {
      const eHeight = tHeight;
      const t = vh(2);
      const o = vh(15);
      const c = vh(10);
      const a = w - vh(58);

      const step = crashValue < 3 ? 0.5 : (crashValue < 6 ? 1 : 2);
      const fracStr = v => String(v).split('.')[1] || '0';

      let ticks = [1];
      let points = [];
      let miniCount = 3;
      let offset = 0;

      const upTo = Math.ceil(crashValue / step) + 4;
      for (let i = 0; i < upTo; i++) ticks.push(ticks[ticks.length - 1] + step);

      for (let i = 0; i < ticks.length; i++) {
        const val = ticks[i];
        const y = startY - eHeight/100 * ((val/crashValue)*100 - (1/crashValue)*100);
        points.push({ crashValue: val, coordY: y });

        if (i > 0 && i === ticks.length - 1) {
          const dist = Math.abs(y - points[i - 1].coordY) - t - c;
          miniCount = Math.floor((dist + 1) / (t + c));
          if (miniCount <= 0) miniCount = 1;
          if (miniCount >= 3) offset = (dist - miniCount * (t + c)) / 2;
        }
      }

      ctx.save();
      ctx.textBaseline = 'alphabetic';
      ctx.textAlign = 'left';
      ctx.font = `500 ${vh(12)}px ProximaNova-Condensed`;

      for (let i = 0; i < points.length; i++) {
        const p = points[i];
        const isHalf = fracStr(p.crashValue) === '5';
        const mainLen = isHalf ? (o - o/3) : o;

        ctx.beginPath();
        ctx.lineWidth = t;
        ctx.strokeStyle = isHalf ? 'rgba(255,255,255,0.4)' : 'rgba(255,255,255,0.5)';
        ctx.moveTo(a, p.coordY);
        ctx.lineTo(a - mainLen, p.coordY);
        ctx.stroke();

        ctx.fillStyle = isHalf ? 'rgba(251,251,251,0.30)' : 'rgba(251,251,251,0.60)';
        const label = Number(p.crashValue).toFixed(2) + 'x';
        const textH = vh(12);
        ctx.fillText(label, a + vh(6), p.coordY + textH / 2 - t);

        if (i > 0) {
          for (let j = 0; j < miniCount; j++) {
            const yy = p.coordY + j * (t + c) + (t + c) + (offset || 0);
            ctx.beginPath();
            ctx.lineWidth = t;
            ctx.strokeStyle = 'rgba(255,255,255,0.2)';
            ctx.moveTo(a, yy);
            ctx.lineTo(a - o/3, yy);
            ctx.stroke();
          }
        }
      }
      ctx.restore();
    };

    // --- СНИЗУ: шкала времени ---
    const drawBottomTimeScale = () => {
      const duration = path.length ? path[path.length - 1].x : 0;
      const denom = Math.max(1000, duration);
      const totalSecs = Math.floor(denom / 1000);
      if (totalSecs < 0) return;

      const baseY = h - vh(18);
      const tickH = vh(12);
      const lineW = vh(2);
      const color = 'rgba(255,255,255,0.35)';

      ctx.save();
      ctx.lineWidth = lineW;
      ctx.strokeStyle = color;
      ctx.fillStyle = color;
      ctx.textAlign = 'center';
      ctx.textBaseline = 'top';
      ctx.font = `500 ${vh(12)}px ProximaNova-Condensed`;

      for (let s = 0; s <= totalSecs; s++){
        const x = startX + tWidth * ((s * 1000) / denom);
        ctx.beginPath();
        ctx.moveTo(x, baseY - tickH);
        ctx.lineTo(x, baseY);
        ctx.stroke();
        ctx.fillText(`${s}s`, x, baseY + vh(4));
      }
      ctx.restore();
    };

    // ---- линия
    ctx.beginPath(); ctx.lineWidth = vh(2); ctx.strokeStyle='#E81C5A';
    ctx.moveTo(startX,startY);
    const duration = path.length ? path[path.length-1].x : 0;
    let endX=startX, endY=startY; const acc=[];
    for (const p of path){
      const x=(duration>0?(tWidth*(p.x/Math.max(1000,duration))):0)+startX;
      const y=startY - tHeight/100 * ((parseFloat(p.y)/parseFloat(crashValue))*100 - (path[0]?.y ?? 1)/parseFloat(crashValue)*100);
      ctx.lineTo(x,y); acc.push({x,y}); endX=x; endY=y;
    }
    ctx.stroke();
    if (acc.length){
      const last=acc[acc.length-1];
      ctx.beginPath(); ctx.moveTo(startX,startY);
      for(const a of acc) ctx.lineTo(a.x,a.y);
      ctx.lineTo(last.x,startY); ctx.lineTo(startX,startY);
      const g=ctx.createLinearGradient(last.x,last.y,last.x,startY);
      g.addColorStop(0,'rgba(232,28,90,0.40)'); g.addColorStop(1,'rgba(232,28,90,0.00)');
      ctx.fillStyle=g; ctx.fill();
    }

    // шкалы
    drawRightScale();
    drawBottomTimeScale();

    // крен
    const targetRoll = 100 - (1/parseFloat(crashValue))*100;
    airplaneRoll = Math.max(airplaneRoll, targetRoll);

    // самолёт и струя — только в полёте
    if (!holdCrashed){
      const planeScale=1.6, planeW=vh(140*planeScale), planeH=vh(45*planeScale);
      const px=acc.length?endX:startX, py=acc.length?endY:startY;
      if (!planeReady && planeImg?.complete) planeReady=true;

      ctx.save(); ctx.translate(px,py); ctx.rotate((0.45*airplaneRoll)*Math.PI/180*-1); ctx.translate(-px,-py);
      if (planeReady) ctx.drawImage(planeImg, px-planeW, py-planeH/1.2, planeW, planeH);
      ctx.restore();

      const desiredIdx=Math.min(4, Math.max(0, colorId(crashValue)));
      advanceJet(desiredIdx);
      if (currentVideo && currentVideo.readyState>=2){
        const n=vh(30), ih=vh(80);
        ctx.save();
        ctx.translate(px,py); ctx.rotate((0.45*airplaneRoll+94)*Math.PI/180*-1); ctx.translate(-px,-py);
        ctx.drawImage(currentVideo, px - n/2 + vh(22), py - ih - planeW + vh(17), n, ih);
        ctx.restore();
      }
    }
    // оверлей взрыва
    if (boomVideo && boomVideo.readyState>=2){
      const px=(acc.length?endX:startX), py=(acc.length?endY:startY);
      const bw=vh(220), bh=vh(220);
      ctx.save(); ctx.translate(px,py); ctx.rotate((0.45*airplaneRoll)*Math.PI/180*-1); ctx.translate(-px,-py);
      ctx.drawImage(boomVideo, px-bw/2, py-bh/2 - vh(20), bw, bh); ctx.restore();
    }
  }
  function startRender(){ if (raf) return; const loop=()=>{ try{ draw(); }catch(e){ console.error('[CRASH] draw',e);} raf=requestAnimationFrame(loop); }; raf=requestAnimationFrame(loop); }
  function stopRender(){ if (raf) cancelAnimationFrame(raf); raf=null; }

  // === Данные из снапшота ===
  $: getActiveTimer   = data?.timer?.active ?? false;
  $: getInitTimeTimer = data?.timer?.initTime ?? 0;
  $: getTimeStartTimer= data?.timer?.timeStart ?? "";

  $: getActiveGame    = data?.game?.active ?? false;
  $: getGameCrashedAt = data?.game?.crashedAt ?? null;
  $: getStartGameTime = data?.game?.gameStartTime ?? "";

  // История полоска
  $: historySourceRaw =
    data?.history ?? data?.historyBar ?? data?.main?.history ?? data?.lastHistory ??
    data?.recent ?? data?.recentGames ?? data?.latest ?? [];
  $: historyItems = (historySourceRaw||[])
    .map(h=>{
      const val = Number(typeof h==='number' ? h : (h?.crashedAt ?? h?.x ?? h?.value ?? h?.multiplier ?? 0));
      const id  = typeof h==='object' ? (h?.id ?? h?.gameId ?? h?.gid ?? h?.game?.id ?? null) : null;
      return { id, val, raw:h };
    })
    .filter(it=>it.val>0).slice(-28);

  const PAGES = ['main','historyList','historyItem'];
  let pageIndex = 0;            // 0 — main, 1 — historyList, 2 — historyItem
  let historyTargetId = null;   // id игры для страницы historyItem
  $: getOpenedPage = PAGES[pageIndex];

  $: getCurrentGame    = data?.currentGame ?? { id:null, players:[] };
  $: getCurrentGameID  = getCurrentGame?.id ?? null;
  $: getCurrentGamePlayers = data?.currentGame?.players ?? [];
  $: getCurrentGamePlayersSorted = [...getCurrentGamePlayers].sort((a,b)=>(b?.betAmount??0)-(a?.betAmount??0));
  $: getTotalBetsSum = getCurrentGamePlayers.reduce((s,p)=> s + Number(p?.betAmount??0), 0);

  // мои данные
  $: myId    = String(accountId ?? data?.main?.accountId ?? '');
  $: myLogin = String(data?.main?.login ?? '');
  $: betAlreadyExist = getCurrentGamePlayers.find(p => String(p?.accountId) === myId || (myLogin && String(p?.login) === myLogin));
  $: alreadyTookWinnings = alreadyClickToTookWinnings || !!betAlreadyExist?.exit;

  // --- HOLD + ожидание ---
  const CRASH_HOLD_MS = 5000;
  const CLIENT_WAIT_MS = 16000;

  let crashHoldUntil=0, lastCrashAtSeen=null, prevActiveGame=false;
  let clientWaitStartAt=0, clientWaitUntil=0;

  // --- СНАПШОТ завершившегося раунда для правой колонки ---
  let snapshotPlayers = [];
  let snapshotCrashAt = null;
  let snapshotGameId  = null;

  // --- Буферизируем «последнее активное» состояние, пока игра идёт ---
  let lastActiveGameId  = null;
  let roundPlayersBuffer = [];
  $: if (getActiveGame) {
    // Всегда держим последний актуальный список игроков текущего раунда
    roundPlayersBuffer = (getCurrentGamePlayersSorted || []).map(p => ({ ...p }));
    if (getCurrentGameID) lastActiveGameId = getCurrentGameID;
  }

  $: if (getActiveGame && getCurrentGameID && getCurrentGameID !== lastActiveGameId) {
    lastActivePlayers = [];
    lastActiveGameId  = getCurrentGameID;
  }

  function captureRoundSnapshot(){
    // если сервер уже обнулил currentGame.players — берём из буфера
    const source =
      (roundPlayersBuffer?.length ? roundPlayersBuffer
                                  : (getCurrentGamePlayersSorted || []));

    snapshotPlayers = source.map(p => ({ ...p }));
    snapshotCrashAt = getGameCrashedAt ?? null;

    // ID фиксируем как ИД завершившегося раунда (предпочтительно lastActiveGameId)
    const endedGameId = lastActiveGameId ?? getCurrentGameID ?? null;
    snapshotGameId = endedGameId;
  }

  function beginCrashHold(){
    if (Date.now() < crashHoldUntil) return;

    // СНАПШОТ фиксируем ДО запуска hold (чтобы не потерять игроков)
    captureRoundSnapshot();

    crashHoldUntil = Date.now() + CRASH_HOLD_MS;
    if (lastCrashAtSeen == null) lastCrashAtSeen = (getGameCrashedAt ?? -1);

    const boom = jets['Crash'];
    if (boom){
      boomVideo = boom; boom.currentTime=0; boom.loop=false;
      try{ boom.play(); }catch(e){}
      boom.onended = () => { if (boomVideo===boom) boomVideo=null; };
    }
    clientWaitStartAt = crashHoldUntil;
    clientWaitUntil   = crashHoldUntil + CLIENT_WAIT_MS;

    localStarted=false;
    if (gameTickHandle){ clearInterval(gameTickHandle); gameTickHandle=null; }
  }

  // триггерим холд
  $: if (getGameCrashedAt != null && getGameCrashedAt !== lastCrashAtSeen){
    lastCrashAtSeen = getGameCrashedAt;
    pushVolatile(getGameCrashedAt);
    beginCrashHold();
  }
  $: { if (prevActiveGame && !getActiveGame) beginCrashHold(); prevActiveGame = getActiveGame; }

  // при старте нового раунда сбрасываем hold и снимки
  $: if (getActiveGame){
    crashHoldUntil=0; clientWaitStartAt=0; clientWaitUntil=0; boomVideo=null;
    snapshotPlayers = []; snapshotCrashAt=null; snapshotGameId=null;
    roundPlayersBuffer = []; // сброс буфера на новый раунд
  }

  $: holdCrashed   = Date.now() < crashHoldUntil && !getActiveGame;
  $: clientWaiting = !getActiveGame && !getActiveTimer && !!lastCrashAtSeen && Date.now() >= crashHoldUntil && Date.now() < clientWaitUntil;
  $: if (getActiveTimer){ clientWaitStartAt=0; clientWaitUntil=0; }
  $: if (!holdCrashed && boomVideo) boomVideo=null;

let localStarted=false, startTime=0;
let countdownHandle=null, gameTickHandle=null;
let timerStartedAt = 0; // добавьте эту переменную
let serverRemainingMs = 0; // добавьте эту переменную

  $: phase = (()=>{
    if (getActiveGame) return 'game';
    if (Date.now() < crashHoldUntil) return 'hold';
    if (getActiveTimer) return 'waitServer';
    if (lastCrashAtSeen != null && Date.now() < clientWaitUntil) return 'waitClient';
    return 'idle';
  })();

  $: waitingVisible = (phase==='waitServer' || phase==='waitClient' || (!getActiveGame && (!getActiveTimer && lastCrashAtSeen!=null && Date.now()>=crashHoldUntil && Date.now()<clientWaitUntil)));
  $: gameWrapperVisible = !waitingVisible;
  $: canTake = !!betAlreadyExist && (getActiveGame || localStarted) && betAlreadyExist?.exit == null;

  // ИСПРАВЛЕННАЯ функция updateCountdown
  function updateCountdown(){
  if (getActiveTimer){
    const now = Date.now();
    
    // Если получили новые данные от сервера
    if (data?.timer?.remainingMs !== undefined && data.timer.remainingMs !== serverRemainingMs) {
      serverRemainingMs = data.timer.remainingMs;
      timerStartedAt = now;
      console.log('Server timer update:', serverRemainingMs + 'ms');
    }
    
    // Вычисляем текущее время на основе серверных данных + локальный отсчет
    if (timerStartedAt > 0) {
      const elapsed = now - timerStartedAt;
      const remaining = Math.max(0, serverRemainingMs - elapsed);
      startTime = remaining / 1000;
    } else {
      // Fallback к старому методу если нет данных от сервера
      const startAt = new Date(getTimeStartTimer).getTime();
      const serverNow = getServerTimeWithOffset();
      const diff = Math.max(0, (startAt - serverNow) / 1000);
      startTime = diff;
    }
    
    console.log('Timer update:', {
      remainingMs: serverRemainingMs,
      elapsed: timerStartedAt > 0 ? (now - timerStartedAt) : 0,
      startTime: startTime.toFixed(2) + 's',
      getActiveTimer
    });
    
    if (getActiveTimer && startTime <= 0) {
      localStarted = true;
    }
  } else {
    // Сброс при окончании таймера
    timerStartedAt = 0;
    serverRemainingMs = 0;
    
    if (!getActiveGame && lastCrashAtSeen != null){
      const now = Date.now();
      if (now < crashHoldUntil) {
        startTime = Math.max(0, (crashHoldUntil - now) / 1000);
      } else if (now < clientWaitUntil) {
        startTime = Math.max(0, (clientWaitUntil - now) / 1000);
      } else {
        startTime = 0;
      }
    } else {
      startTime = 0;
    }
  }
}

$: {
  const need = getActiveTimer || clientWaiting;
  if (need && !countdownHandle){ 
    updateCountdown(); 
    countdownHandle=setInterval(updateCountdown, 100); // изменил с 50 на 100
  }
  if (!need && countdownHandle){ 
    clearInterval(countdownHandle); 
    countdownHandle=null; 
  }
}

  // Дополнительное отслеживание изменений данных таймера
$: if (data?.timer?.remainingMs !== undefined && getActiveTimer) {
  updateCountdown();
}

// Сброс при смене состояния таймера
$: if (!getActiveTimer) {
  timerStartedAt = 0;
  serverRemainingMs = 0;
}

  // игровой тик
  function initGame(){
    if (gameTickHandle){ clearInterval(gameTickHandle); gameTickHandle=null; }
    path=[{x:0,y:1}]; crashValue=1; airplaneRoll=0;
    currentJetIdx=0; playingTransition=false; currentVideo=jets['Start']||null;
    boomVideo=null; crashHoldUntil=0;

    gameStartMs = Date.parse(getStartGameTime) || 0;
    if (!gameStartMs) return;

    const firstDt = Math.max(0, getServerTimeWithOffset() - gameStartMs);
    crashValue = Math.pow(1.00006, firstDt);
    path[0] = { x:firstDt, y:crashValue };

    const tick = () => {
      if (!gameStartMs) return;
      if (!getActiveGame) return;
      const dt = getServerTimeWithOffset() - gameStartMs;
      if (dt < 0) return;
      crashValue = Math.pow(1.00006, dt);
      path.push({ x:dt, y:crashValue });
      if (path.length>2000) path.shift();
    };
    tick(); gameTickHandle=setInterval(tick,16);
  }

  // Реинициализация при смене gameId / serverStartTime
  $: if (getActiveGame && getCurrentGameID && getCurrentGameID !== lastGameId) {
    lastGameId = getCurrentGameID; lastCrashAtSeen = null; initGame();
  }
  $: if (getActiveTimer && !getActiveGame && (snapshotPlayers.length || snapshotGameId != null)) {
    snapshotPlayers = [];
    snapshotCrashAt = null;
    snapshotGameId  = null;
  }
  $: if (getActiveGame) {
    const st = Date.parse(getStartGameTime) || 0;
    if (st && st !== lastStartAt) { lastStartAt = st; lastCrashAtSeen = null; initGame(); }
  }

  $: if ((getActiveTimer && !getActiveGame) || clientWaiting){ if (gameTickHandle){ clearInterval(gameTickHandle); gameTickHandle=null; } }
  $: if (gameWrapperVisible && canvas){ changeSizeGameInfo(); setupCanvas(); }
  $: if (!canvas){ ctx=null; }

  // === множитель ===
  function changeMultiplier(){ multiplierValue='X'+Number(multiplier).toFixed(2); const i=multipliers.indexOf(Number(Number(multiplier).toFixed(2))); selectedMultiplier = i!==-1 ? i : null; }
  function selectMultiplier(i){ const val=multipliers[i]; multiplier=(i!==selectedMultiplier && val) ? val : 1; changeMultiplier(); }
  $: { const at=data?.autoTake ?? 1; multiplier= at>1 ? at : 1; changeMultiplier(); }

  // === действия ===
  function makeBet(){ if (placeBetAmount>0){ dispatch('bet',{ betAmount:Number(placeBetAmount)||0, autoTakeX:Number(multiplier)||1 }); placeBetAmount=0; } }
  function take(){ dispatch('take'); alreadyClickToTookWinnings = true; }
  function openPage(i, gameId=null){
    pageIndex = Math.max(0, Math.min(PAGES.length-1, i));
    historyTargetId = gameId ?? null;
    // если баку нужно — пусть слушает это событие для подгрузки данных
    dispatch('openPage', { page: i, id: gameId });
  }
  $: currentHistoryGame =
    (data?.historyItem?.game &&
     (!historyTargetId ||
      String(data.historyItem.game?.id ?? '') === String(historyTargetId))) ? data.historyItem.game : null;

  $: currentHistoryBets = currentHistoryGame ? (data?.historyItem?.bets || []) : [];
  
  // --- Видимые данные для правой колонки ---
  // Показываем результаты не только в hold, но и пока показывается ожидание следующего раунда.
  $: showingHold = (!getActiveGame && !getActiveTimer &&
                    (Date.now() < crashHoldUntil || clientWaiting) &&
                    snapshotPlayers.length > 0);

  $: if (getActiveTimer && !getActiveGame && (snapshotPlayers.length || snapshotGameId != null)) {
    snapshotPlayers = []; snapshotCrashAt = null; snapshotGameId = null;
  }

  $: visibleGameID = showingHold && snapshotGameId ? snapshotGameId : getCurrentGameID;
  $: visiblePlayers = (getCurrentGamePlayersSorted.length>0 && !showingHold) ? getCurrentGamePlayersSorted : (showingHold ? snapshotPlayers : []);
  $: visibleBank = showingHold
        ? snapshotPlayers.reduce((s,p)=>s + Number(p?.betAmount??0), 0)
        : getTotalBetsSum;

  onMount(()=>{
    planeImg = new Image(); planeImg.crossOrigin='anonymous';
    const setReady=()=>{ planeReady=true; draw(); };
    planeImg.onload=setReady;
    planeImg.onerror=()=>console.warn('[CRASH] plane image failed to load:', planeImg?.src);
    planeImg.src=`${cdn}/img/panelMenu/miniGames/crash/airplane.svg?v=2`;
    if (planeImg.decode){ planeImg.decode().then(setReady).catch(()=>{}); }

    changeSizeGameInfo(); setupCanvas(); startRender();
    window.__crash = {
      get pathLen(){ return path.length; },
      get x(){ return crashValue; },
      get flags(){ return { getActiveGame, getActiveTimer, localStarted, gameStartMs, holdCrashed, clientWaiting, phase }; }
    };
    window.addEventListener('resize', changeSizeGameInfo);
    window.addEventListener('resize', setupCanvas);
  });
  
  onDestroy(()=>{
    if (countdownHandle) clearInterval(countdownHandle);
    if (gameTickHandle) clearInterval(gameTickHandle);
    window.removeEventListener('resize', changeSizeGameInfo);
    window.removeEventListener('resize', setupCanvas);
    stopRender();
  });
</script>

{#if data?.active || getActiveTimer || getActiveTimer === null}
  <div class="crash full-width full-height">
    {#if getOpenedPage === 'main'}
      <div class="main full-width full-height column-block align-start justify-between">
        <!-- История -->
        <div class="historyWrapper full-width row-block align-start justify-start">
          <button class="openHistoryBtn row-block align-start justify-start" on:click={() => openPage(1)}>
            <span>История</span>
          </button>

          {#if historyItems.length}
            <div class="historyItemScrollWrapper row-block align-start justify-start">
              {#each historyItems as h (h.id ?? h.val)}
                <button class="historyItem row-block align-center justify-center"
                        style="background:{colors[colorId(h.val, true)]?.background}"
                        on:click={() => openPage(2, h.id ?? null, true)}>
                  <span style="color:{colors[colorId(h.val, true)]?.text}">X{h.val.toFixed(2)}</span>
                </button>
              {/each}
            </div>
          {:else}
            <div class="historyItemScrollWrapper emptyHistoryItem row-block align-start justify-start">
              {#each Array(50) as _}
                <button class="historyItem row-block align-center justify-center" />
              {/each}
            </div>
          {/if}
        </div>

        <!-- Игра -->
        <div class="gameWrapper full-width full-height row-block align-start justify-center">
          <div class="game full-height column-block align-start justify-start">
            <div class="gameInfo row-block align-start justify-center" bind:this={gameInfo} style="position:relative;">
              <img class="back full-width full-height" alt="" src={`${cdn}/img/panelMenu/miniGames/crash/gameBack.png`} style="position:absolute; inset:0; z-index:1; object-fit:cover;" />

              {#if getActiveTimer || getActiveTimer === null || waitingVisible}
                <!-- Ожидание -->
                <div class="waitGame column-block align-center justify-center full-width full-height">
                  <span class="title">Ожидание раунда</span>
                  <span class="sub-title">Начнётся через</span>
                  <span class="time row-block align-center justify-center">
                    <span class="sec">{String(Math.floor(startTime)).padStart(2,'0')}</span>
                    <span class="dot-time">.</span>
                    <span class="ms">{String(Math.floor((startTime%1)*100)).padStart(2,'0')}</span>
                    <span class="symbol">s</span>
                  </span>
                  <img class="airplane full-width full-height" alt="" src={`${cdn}/img/panelMenu/miniGames/crash/airplane.svg?`} />
                </div>
              {:else}
                <!-- Раунд / 5-секундный hold -->
                <div class="game-wrapper column-block align-center justify-start full-width full-height">
                  <div class="top column-block align-center justify-start">
                    <div class="multiplierX column-block align-start justify-start {holdCrashed ? 'crashed' : ''}">
                      <span class="crash-value row-block align-center justify-center">
                        <span class="int">{Number(crashValue).toFixed(2).split('.')[0]}</span>
                        <span class="dot-crash">.</span>
                        <span class="fractional">{Number(crashValue).toFixed(2).split('.')[1]}</span>
                        <span class="symbol">x</span>
                      </span>
                    </div>
                    {#if holdCrashed}
                      <span class="text">Игра окончена</span>
                    {:else if betAlreadyExist}
                      <span class="text">Возможный выигрыш</span>
                    {/if}
                    {#if betAlreadyExist && !holdCrashed}
                      <div class="winnig row-block align-center justify-center">
                        <div class="plus"> + </div>
                        <div class="value">{Math.floor((betAlreadyExist?.betAmount ?? 0) * crashValue - (betAlreadyExist?.betAmount ?? 0))}</div>
                        <img class="coins" alt="" src={`${cdn}/img/donate/mcoin.svg`} />
                      </div>
                    {/if}
                  </div>

                  <canvas id="canvas" class="crashCanvas" bind:this={canvas} style="position:absolute; inset:0; z-index:5; pointer-events:none; width:100%; height:100%;"></canvas>
                  <div class="flashbang"/>
                </div>
              {/if}
            </div>

            <!-- Панель ставок -->
            <div class="placeBet column-block align-center justify-between">
              <div class="betWrapper full-width row-block align-start justify-between">
                <div class="bet">
                  <div class="title row-block align-center justify-start">Ставка</div>
                  <div class="betInput inputWrapper {(!getActiveTimer || getCurrentGamePlayers.length >= 999999) ? 'disabled' : ''}">
                    <input type="number" class="full-width full-height {placeBetFocus ? 'focus' : ''}"
                      bind:value={placeBetAmount}
                      disabled={!getActiveTimer || betAlreadyExist || getCurrentGamePlayers.length >= 999999}
                      on:focus={() => (placeBetFocus = true, placeBetAmount <= 0 && (placeBetAmount = null))}
                      on:blur={() => (placeBetFocus = false, !placeBetAmount && (placeBetAmount = 0))}/>
                    <img class="coins" alt="" src={`${cdn}/img/donate/mcoin.svg`} />
                  </div>
                </div>

                <div class="winnig">
                  <div class="title row-block align-center justify-start">Выйграешь</div>
                  <div class="winnigValue full-width row-block align-center justify-start {alreadyTookWinnings ? 'disabled' : ''}">
                    <span class="value">{Math.floor((betAlreadyExist ? betAlreadyExist.betAmount : 0) * (getActiveGame ? crashValue : 1))}</span>
                    <img class="coins" alt="" src={`${cdn}/img/donate/mcoin.svg`} />
                  </div>
                </div>
              </div>

              <div class="multiplier full-width column-block align-start justify-between">
                <div class="row-block align-center justify-start">
                  <div class="inputWrapper multiplierInput {(!getActiveTimer || placeBetAmount <= 0) ? 'disabled' : ''}">
                    <div class="value">{multiplierValue}</div>
                    <input type="number" class="full-width full-height {multiplierFocus ? 'focus' : ''}"
                      bind:value={multiplier}
                      disabled={!getActiveTimer || placeBetAmount <= 0}
                      on:input={changeMultiplier}
                      on:focus={() => (multiplierFocus = true)}
                      on:blur={() => (multiplierFocus = false, (!multiplier || multiplier < 1) && (multiplier = 1), changeMultiplier())}/>
                  </div>
                  <div class="row-block align-center justify-start">
                    {#each multipliers as m, idx}
                      <button class="multiplierItem row-block align-center justify-center {idx === selectedMultiplier ? 'selected' : ''} {(!getActiveTimer || placeBetAmount <= 0 || betAlreadyExist) ? 'disabled' : ''}"
                        disabled={!getActiveTimer || placeBetAmount <= 0 || betAlreadyExist}
                        on:click={() => selectMultiplier(idx)}>X{Number(m).toFixed(2)}</button>
                    {/each}
                  </div>
                </div>

                {#if getActiveTimer || getActiveTimer === null}
                  <button class="makeBet full-width {(placeBetAmount <= 0 || betAlreadyExist || getCurrentGamePlayers.length >= 999999) ? 'disabled' : ''}"
                    disabled={placeBetAmount <= 0 || betAlreadyExist || getCurrentGamePlayers.length >= 999999}
                    on:click={makeBet}>Сделать ставку</button>
                {:else}
                  <button class="makeBet take full-width{canTake ? '' : 'disabled'}" disabled={!canTake} on:click={take}>ЗАБРАТЬ</button>
                {/if}
              </div>
            </div>
          </div>

          <!-- Правый список ставок -->
          <div class="bets full-height column-block align-start justify-start">
            <div class="info row-block align-center justify-between full-width">
              <div class="row-block align-center justify-start">
                <span class="title">Игра</span>
                <div class="gameID">{#if visibleGameID}<span>#{visibleGameID}</span>{/if}</div>
              </div>
              <div class="row-block align-center justify-start">
                <div class="players row-block align-center justify-start">
                  <div class="players-info column-block align-start justify-between">
                    <span class="count">{visiblePlayers.length}</span>
                    <span class="text">Игроки</span>
                  </div>
                </div>
                <div class="bank row-block align-center justify-start">
                  <div class="bank-info column-block align-start justify-between">
                    <span class="count row-block align-center justify-start">{visibleBank} <img class="coins" alt="" src={`${cdn}/img/donate/mcoin.svg`} /></span>
                    <span class="text">Банк</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="betsWrapper full-height full-width column-block align-center justify-start {visiblePlayers.length <= 0 ? 'empty' : ''}">
              {#if visiblePlayers.length <= 0}
                <div class="emptyBets column-block align-center justify-center"><span class="text">Пока нет ставок</span></div>
              {/if}

              {#each visiblePlayers as p}
                <div class="betItem row-block align-center justify-between full-width
                            {showingHold ? (p?.exit != null ? 'winnerBack' : 'crashed')
                                          : (p?.exit != null && Number(p?.exit) <= crashValue ? 'winnerBack' : ((p?.exit == null && !getActiveGame && !getActiveTimer) ? 'crashed' : ''))}">
                  <AccountComponent
                    login={p?.login}
                    accountId={p?.accountId}
                    serverName={p?.serverName}
                    serverId={p?.serverId}
                    cdn={cdn}
                    displayedComponents={["avatar","you","login","accountId","serverName"]}
                  />

                  {#if (showingHold && p?.exit != null) || (!showingHold && p?.exit != null && Number(p?.exit) <= crashValue)}
                    <!-- Победитель -->
                    <div class="winner full-height row-block align-center justify-start">
                      <div class="amountWrapper"><span>{Math.floor((p?.betAmount ?? 0) * Number(p?.exit).toFixed(2))}</span><img alt="mcoin" src={`${cdn}/img/donate/mcoin.svg`} /></div>
                      <div class="multiplierExit"><span>X{Number(p?.exit).toFixed(2)}</span></div>

                      <!-- фон кубков (из макета) -->
                      <svg xmlns="http://www.w3.org/2000/svg" width="323" height="195" viewBox="0 0 323 195" fill="none">
                        <path d="M234.108 37.6583C231.352 36.0673 227.824 37.0125 226.233 39.7683L223.351 44.7606L219.138 42.3279C210.401 37.2839 199.332 40.2456 194.285 48.987L191.853 53.2005C183.987 66.8244 185.61 83.7633 199.468 97.4954C200.287 104.019 203.671 110.852 209.001 115.64L202.968 126.182L192.983 120.417C190.228 118.826 186.7 119.771 185.109 122.527C183.518 125.283 184.463 128.811 187.219 130.402L217.173 147.695C219.928 149.286 223.456 148.341 225.047 145.585C226.638 142.83 225.693 139.302 222.937 137.711L212.953 131.946L218.97 121.442C225.785 123.659 233.208 123.299 239.554 120.539C258.718 125.773 273.848 118.706 281.714 105.082L284.146 100.868C289.19 92.1318 286.229 81.063 277.487 76.0161L273.274 73.5835L276.156 68.5912C277.747 65.8355 276.802 62.3077 274.046 60.7166C269.054 57.8343 239.1 40.5406 234.108 37.6583Z" fill="white" fill-opacity="0.08"/>
                        <path d="M133.466 114.254C132.011 113.414 130.15 113.913 129.31 115.367L127.789 118.002L125.565 116.718C120.955 114.056 115.114 115.619 112.45 120.232L111.167 122.456C107.016 129.645 107.872 138.584 115.185 145.831C115.618 149.274 117.404 152.88 120.216 155.406L117.032 160.969L111.763 157.927C110.309 157.088 108.447 157.587 107.608 159.041C106.768 160.495 107.267 162.357 108.721 163.196L124.528 172.323C125.983 173.162 127.844 172.663 128.684 171.209C129.524 169.755 129.025 167.893 127.571 167.054L122.302 164.012L125.477 158.468C129.073 159.638 132.991 159.448 136.34 157.992C146.453 160.754 154.437 157.024 158.588 149.835L159.872 147.611C162.534 143.001 160.971 137.159 156.358 134.496Z" fill="white" fill-opacity="0.08"/>
                        <path d="M95.7728 41.7277C94.5658 41.0308 93.0207 41.4449 92.3238 42.6519L91.0614 44.8385L89.2159 43.773C85.3893 41.5637 80.5413 42.8609 78.3307 46.6896L77.2652 48.5351C73.8201 54.5023 74.5308 61.9215 80.6006 67.9361C80.9596 70.7935 82.4417 73.7862 84.7762 75.8833L82.1337 80.5006L77.7605 77.9757C76.5535 77.2788 75.0083 77.6929 74.3115 78.8999C73.6146 80.1069 74.0286 81.652 75.2356 82.3489L88.3552 89.9235C89.5622 90.6203 91.1074 90.2063 91.8043 88.9993C92.5011 87.7923 92.0871 86.2471 90.8801 85.5503Z" fill="white" fill-opacity="0.08"/>
                      </svg>
                    </div>
                  {:else}
                    <!-- В игре / Краш -->
                    <div class="inGameWrapper">
                      <div class="amountWrapper"><span>{p?.betAmount}</span><img alt="mcoin" src={`${cdn}/img/donate/mcoin.svg`} /></div>
                      <div class="inGame {(showingHold && p?.exit==null) ? 'crashed' : ((!getActiveGame && !getActiveTimer && p?.exit==null) ? 'crashed' : '')}">
                        {#if showingHold}
                          {#if p?.exit == null}
                            <span>Краш</span>
                          {:else}
                            <span>В игре</span>
                          {/if}
                        {:else}
                          {#if getActiveGame || getActiveTimer}
                            <span>В игре</span>
                          {:else}
                            <span>Краш</span>
                          {/if}
                        {/if}
                      </div>
                    </div>
                  {/if}
                </div>
              {/each}
            </div>
          </div>
        </div>
      </div>

  {:else if getOpenedPage === 'historyItem'}
  <div class="historyItem full-width full-height column-block align-center justify-start">
    <div class="info row-block align-center justify-between full-width">
      <div class="row-block align-center justify-start">
        <span class="title">Игра</span>
        <div class="gameID"><span>#{currentHistoryGame ? currentHistoryGame.id : (historyTargetId ?? '—')}</span></div>
      </div>
      <div class="row-block align-center justify-start">
        <div class="players row-block align-center justify-start">
          <div class="players-info column-block align-start justify-between">
            <span class="count">{currentHistoryGame ? currentHistoryGame.playersCount : '—'}</span>
            <span class="text">Игроки</span>
          </div>
        </div>
        <div class="bank row-block align-center justify-start">
          <div class="bank-info column-block align-start justify-between">
            <span class="count row-block align-center justify-start">
              {currentHistoryGame ? currentHistoryGame.bank : '—'} <img class="coins" alt="" src={`${cdn}/img/donate/mcoin.svg`} />
            </span>
            <span class="text">Банк</span>
          </div>
        </div>
      </div>
    </div>

    <div class="betsWrapper full-height full-width column-block align-center justify-start {( !currentHistoryGame || currentHistoryBets.length === 0 ) ? 'empty' : ''}">
      {#if !currentHistoryGame}
        <!-- данных ещё нет — но страница не закрывается -->
        <div class="emptyBets column-block align-center justify-center">
          <span class="text">Пока нет данных…</span>
        </div>
      {:else if currentHistoryBets.length === 0}
        <div class="emptyBets column-block align-center justify-center">
          <span class="text">Пока нет ставок</span>
        </div>
      {/if}

      {#if currentHistoryGame}
        {#each currentHistoryBets as b}
          <div class="betItem row-block align-center justify-between full-width {b?.exitX != null ? 'winnerBack' : ''} {b?.exitX == null ? 'crashed' : ''}">
            <div class="account"><div class="login">{b?.login}</div><div class="meta">#{b?.accountId}</div></div>
            <div class="amountWrapper"><span>{b?.bet}</span><img class="coins" alt="" src={`${cdn}/img/donate/mcoin.svg`} /></div>
            {#if b?.exitX == null}
              <div class="inGame crashed"><span>Краш</span></div>
              <div class="inGame crashed"><span>X{Number(currentHistoryGame?.crashedAt ?? 0).toFixed(2)}</span></div>
            {:else}
              <div class="winner full-height row-block align-center justify-start">
                <div class="amountWrapper"><span>{Math.floor(b?.bet * Number(b?.exitX).toFixed(2))}</span><img class="coins" alt="" src={`${cdn}/img/donate/mcoin.svg`} /></div>
                <div class="multiplierExit"><span>X{Number(b?.exitX).toFixed(2)}</span></div>
              </div>
            {/if}
          </div>
        {/each}
      {/if}
    </div>
  </div>
{/if}

  </div>
{:else}
  <div class="no-content"><h1>Coming soon...</h1></div>
{/if}

<style>
  /* Фикс слоя фона победителя, чтобы не закрывал контент */
  .betItem { position: relative; overflow: hidden; }
  .betItem > .bg { position:absolute; inset:0; z-index:0; pointer-events:none; opacity:0; transition:opacity .2s ease; }
  .betItem.winnerBack > .bg { opacity:1; }
  .betItem > *:not(.bg) { position:relative; z-index:1; }
</style>
