<script>
  import { onMount, onDestroy } from 'svelte';
  import SVGComponent from './SVGComponent.svelte';
  import BindValue from './BindValue.svelte';
  import { fade } from 'svelte/transition';
  import '../css/hud.css';
  
  // Импорты из старого файла
  import { itemsInfo } from 'json/itemsInfo.js';
  import { otherStatsData } from 'store/account';
  import { charData } from 'store/chars';
  import moment from 'moment';
  import fraction from 'json/fraction.js';
  import jobs from 'json/jobs.js';
  import vipinfo from 'json/vipinfo.js';
  import { serverDonatMultiplier } from 'store/server';
  import { charWanted, charMoney, charBankMoney, charCoinsTime } from 'store/chars';
  import { isWaterMark, isPlayer } from 'store/hud';
  import { format } from 'api/formatter';
  import { charIsPet } from 'store/chars';
  import CountUp from 'api/countup';
  import { translateText } from 'lang';
  import { getQuest } from 'json/quests/quests.js';
  import { storeQuests, selectQuest } from 'store/quest';
  import keys from 'store/keys';
  import keysName from 'json/keys.js';
  import { isInputToggled, isHelp } from 'store/hud';
  import router from 'router';
  import { executeClient } from 'api/rage';
  
  // Импорт coins store
  import { coins, time, received, opened, open, close, startCountdown, stopCountdown } from './assets/coinsStore';

  // Props
  export let SafeSone;
  export let visible;
  export let showLogo = true;
  export let showOnline = true;
  export let showPlayerid = true;
  export let admin = 0;
  export let countDefaultReports = 0;
  export let countModerationReports = 0;
  export let onlineCount = 0;
  export let serverName = '';
  export let flagName = '';
  export let playerId = 0;
  export let accountId = 0;
  export let ammo = null;
  export let clip = null;
  export let wantedLevel = 0;
  export let isPlayingMinigame = false;
  export let hotkeysValue = 0;
  export let capture = null;
  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  
  // Kill List Props
  export let killList = [];
  export let login = '';
  
  // Keys
  export let showHotkeys = true;
  export let keysData = [
    { keyId: 20, title: 'Меню игрока', active: false },
    { keyId: 8, title: 'Навигатор', active: false },
    { keyId: 33, title: 'Голосовой чат', active: false },
    { keyId: 12, title: 'Инвентарь', active: false },
    { keyId: 25, title: 'Взаимодействие', active: false },
    { keyId: 22, title: 'Телефон', active: false },
    { keyId: 26, title: 'Скрыть подсказки', active: false },
    { keyId: 35, title: 'Зажигание ТС', active: false }
  ];
  
  // Demorgan
  export let demorgan = null;
  
  // Coins config - НОВЫЙ ДИЗАЙН
  export let coinsConfig = {
    shown: false,
    type: 'coins',
    quest: {
      amount: 200,
      finished: false,
      title: '5 часов'
    },
    model: null,
    name: null,
    limit: null,
    case: null,
    imageHash: null,
    data: null
  };
  
  // Mission config - НОВЫЙ ДИЗАЙН
  export let mission = {
    shown: false,
    personName: '',
    personJob: '',
    description: '',
    quests: []
  };
  
  // Внутренние переменные
  let selectCharData = $charData;
  let useVisible = -1;
  let isHint = false;
  let HideHelp = false;
  let QuestsList = [];
  let OldQuest = [];
  let quest = false;
  let weaponItemId = 0;
  let clipSize = 0;
  let serverOnline = 0;
  let serverPlayerId = 0;
  let serverPlayerUUID = 0;
  
  // Деньги
  let userData = {
    targetMoney: 0,
    changeMoney: 0,
    timerIdMoney: 0,
    Money: 0,
    targetBank: 0,
    changeBank: 0,
    timerIdBank: 0,
    Bank: 0,
  };
  
  // Computed
  $: showHotkeysBlock = showHotkeys && hotkeysValue === 0 && !$charIsPet && $isHelp;
  $: showCoins = ($opened || coinsConfig.shown) && !capture;
  $: showMission = (mission?.shown || (quest && quest.Title)) && !capture && $isHelp;
  $: demorganTime = calculateDemorganTime(demorgan?.time);
  $: showAmmo = ammo !== null && ammo > 0;
  $: weaponName = weaponItemId > 0 && itemsInfo[weaponItemId] ? itemsInfo[weaponItemId].Name : '';
  
  // Функции из старого файла
  const onSelectQuest = (actorName) => {
    const listIndex = QuestsList.findIndex(q => q.ActorName == actorName);
    if (QuestsList[listIndex]) {
      quest = QuestsList[listIndex];
      return;
    }
    quest = false;
  };
  
  const CounterUpdate = (args, value) => {
    if (userData["timerId" + args])
      clearTimeout(userData["timerId" + args]);
    userData["change" + args] = userData[args] > value ? (0 - (userData[args] - value)) : (value - userData[args]);
    userData[args] = value;
    userData["timerId" + args] = setTimeout(() => {
      userData["timerId" + args] = 0;
      userData["change" + args] = 0;
      if (!userData["target" + args]) {
        userData["target" + args] = new CountUp("target" + args, value);
      }
      else
        userData["target" + args].update(value);
    }, !userData["target" + args] ? 0 : 5000)
  };
  
  function calculateDemorganTime(time) {
    if (!time) return { days: 0, hours: 0, minutes: 0, seconds: 0 };
    
    let seconds = Math.floor(time / 1000);
    let minutes = Math.floor(seconds / 60);
    seconds %= 60;
    let hours = Math.floor(minutes / 60);
    minutes %= 60;
    let days = Math.floor(hours / 24);
    hours %= 24;
    
    return { days, hours, minutes, seconds };
  }
  
  // Watchers из старого файла
  $: {
    if (useVisible != visible) {
      if (visible && $otherStatsData.Name) {
        selectCharData = $otherStatsData;
      } else if (visible && !$otherStatsData.Name && selectCharData !== $charData) {
        selectCharData = $charData;
      } else if (!visible && $otherStatsData.Name) {
        selectCharData = $charData;
        window.accountStore.otherStatsData('{}');
      }
      useVisible = visible;
    }
  }
  
  $: {
    const uuid = Number(selectCharData?.UUID ?? $charData?.UUID ?? 0);
    if (uuid) {
      window.__hud = window.__hud || {};
      window.__hud.charUUID = uuid;
    }
  }
  
  // Подписки на stores
  storeQuests.subscribe((value) => {
    if (value && value.length && OldQuest != value) {
      executeClient("client.quest.selectQuest.Clear");
      
      OldQuest = value;
      QuestsList = [];
      
      value.forEach(questData => {
        const questInfo = getQuest(questData.ActorName, questData.Line);
        
        if (questInfo && questInfo.Title && questInfo.Tasks && questInfo.Tasks[questData.Stage]) {
          executeClient("client.quest.selectQuest.Add", questData.ActorName, (questInfo.Tasks.length - 1) === questData.Stage);
          
          QuestsList.push({
            ActorName: questData.ActorName,
            Title: questInfo.Title,
            Text: questInfo.Tasks[questData.Stage]
          });
        }
      });
      QuestsList = QuestsList;
      
      if (!quest && QuestsList.length && typeof QuestsList[0] === "object" && typeof $selectQuest !== "string") {
        quest = QuestsList[0];
        selectQuest.set(quest.ActorName);
        return;
      }
      
      onSelectQuest($selectQuest);
    }
  });
  
  selectQuest.subscribe((value) => {
    onSelectQuest(value);
  });
  
  // onMount
  onMount(() => {
    // События для coins
    window.events.addEvent("cef.coins.start", (newCoins, newTime, newReceived) => {
      open();
      coins.set(newCoins);
      time.set(newTime);
      received.set(parseInt(newReceived));
      
      if (parseInt(newReceived) === 0) {
        startCountdown(newTime);
      }
    });
    
    window.events.addEvent("cef.coins.stop", (newTime, newReceived) => {
      time.set(newTime);
      received.set(parseInt(newReceived));
      stopCountdown();
    });
    
    // Подписки на изменение денег
    charMoney.subscribe(value => {
      if (userData.Money !== value) {
        CounterUpdate("Money", value);
      }
    });
    
    charBankMoney.subscribe(value => {
      if (userData.Bank !== value) {
        CounterUpdate("Bank", value);
      }
    });
    
    // Window API
    window.hudStore.isHint = (value) => isHint = value;
    window.hudStore.HideHelp = (value) => HideHelp = value;
    window.setServerName = (name) => serverName = name;
    window.serverStore.serverOnline = (value) => serverOnline = value || onlineCount;
    window.serverStore.serverPlayerId = (value) => serverPlayerId = value || playerId;
    window.charStore.charUUID = (value) => serverPlayerUUID = value || accountId;
    window.hudStore.weaponItemId = (value) => weaponItemId = value;
    window.hudStore.clipSize = (value) => clipSize = value || clip;
    window.hudStore.ammo = (value) => ammo = value;
    
    window.app = {
      setTime: (val) => {
        time.set(val);
      }
    };
  });
  
  onDestroy(() => {
    stopCountdown();
  });
</script>

<div class="hud-shadow"></div>

<!-- Hotkeys с использованием BindValue -->
{#if showHotkeysBlock}
  <div class="keys-block-main" transition:fade>
    {#each keysData as keyData}
      <BindValue 
        bind={[keysName[$keys[keyData.keyId]] || keyData.keyId]} 
        isActive={keyData.active}
      >
        <svelte:fragment slot="title">
          <p>{keyData.title}</p>
        </svelte:fragment>
      </BindValue>
    {/each}
  </div>
{/if}

<div class="online column-block align-end">
  {#if showLogo}
    <div class="online__logo row-block">
      {#if admin > 1 && !isPlayingMinigame}
        <div class="report-info align-center justify-end row-block">
          <div class="report-info-container row-block">
            <div class="report-info__title">Репорты:</div>
            <div class="reports-amount">{countDefaultReports}</div>
          </div>
          <div class="report-info-container row-block">
            <div class="report-info__title">Модерации:</div>
            <div class="reports-amount">{countModerationReports}</div>
          </div>
        </div>
      {/if}
      
      <img 
        src="{cdn}img/main/hud/logotype.svg" 
        alt="" 
        class="online__logo__picture"
      />
    </div>
  {/if}
  
  {#if showOnline}
    <div class="server-info row-block align-center">
      <div class="online-text row-block align-center">
        <span class="online-text__title">Online:</span>
        <span class="online-text__text">{serverOnline || onlineCount}</span>
      </div>
      
      <div class="server-id align-center row-block {!flagName ? 'no-flag' : ''}">
        {#if flagName}
          <img 
            class="server-id__picture" 
            src="{cdn}img/global/flags/{flagName}.svg" 
            alt=""
          />
        {/if}
        <span class="server-id__text">{serverName}</span>
      </div>
    </div>
  {/if}
  
  {#if showPlayerid}
    <div class="player-id">
      <span class="player-id__text-1">ID: {serverPlayerId || playerId} |</span>
      <span class="player-id__text-2">#{serverPlayerUUID || accountId}</span>
    </div>
  {/if}
  
  <!-- Оружие - ОДИН универсальный блок -->
  {#if showAmmo}
    <div class="bullets-block row-block align-center" transition:fade>
      <div class="bullets-block__text">
        <span class="ammo-count">
          {#if clipSize > 0 && clipSize < 1000}
            {clip || clipSize}
          {/if}
          {#if clipSize > 0 && clipSize < 1000}
            <span class="separator">/</span>
          {/if}
          {ammo}
        </span>
      </div>
      <img 
        src="{cdn}img/main/hud/bullets.svg" 
        class="bullets-block__picture" 
        alt=""
      />
      {#if weaponName}
        <span class="weapon-name">{weaponName}</span>
      {/if}
    </div>
  {/if}
  
  {#if wantedLevel && wantedLevel > 0}
    <div class="wanted row-block" transition:fade>
      {#each [5, 4, 3, 2, 1] as level}
        <svg 
          class="wanted-star {wantedLevel >= level ? 'active' : ''}" 
          width="20" 
          height="20" 
          viewBox="0 0 20 20"
        >
          <defs>
            <clipPath id="clip-path-{level}">
              <rect width="20" height="20" transform="translate(1883 155)" fill="#fff"/>
            </clipPath>
          </defs>
          <g transform="translate(-1883 -155)" clip-path="url(#clip-path-{level})">
            <g transform="translate(1883 155)">
              <path d="M10,15.773,16.18,19.5l-1.635-7.029L20,7.744l-7.191-.617L10,.5,7.191,7.127,0,7.744l5.455,4.727L3.82,19.5Z" fill="currentColor"/>
            </g>
          </g>
        </svg>
      {/each}
    </div>
  {/if}
  
  <!-- Kill List -->
  {#if killList && killList.length > 0}
    <div class="kill-list column-block align-end" transition:fade>
      {#each killList as kill}
        <div class="kill-block row-block list-complete-item {login === kill.nicknameKilled ? 'meKilled' : ''} {login === kill.nicknameKiller ? 'playerDidKilled' : ''}">
          <div class="killer-info" style="--color: {kill.killerPostColor}">
            <SVGComponent 
              class="killer-info__icon" 
              path="img/main/hud/kill-list/{kill.killerIcon}.svg" 
            />
            <span>{kill.nicknameKiller}</span>
          </div>
          
          <div class="weapon-info">
            {#if kill.weapon}
              <SVGComponent 
                path="img/main/hud/kill-list/weapons/{kill.weapon}.svg" 
                class="weapon-info__picture-weapon" 
              />
            {/if}
            {#if kill.headshoot}
              <SVGComponent 
                path="img/main/hud/kill-list/target.svg" 
                class="weapon-info__picture-target" 
              />
            {/if}
          </div>
          
          <div class="killed-info" style="--color: {kill.victimPostColor}">
            <span>{kill.nicknameKilled}</span>
            <SVGComponent 
              class="killer-info__icon" 
              path="img/main/hud/kill-list/{kill.victimIcon}.svg" 
            />
          </div>
        </div>
      {/each}
    </div>
  {/if}
  
  <!-- Missions Container -->
  {#if !isPlayingMinigame}
    <div class="missions-container align-end column-block {hotkeysValue === 2 ? 'hidden' : ''}">
      <!-- Бонусные коины - НОВЫЙ ДИЗАЙН -->
      {#if showCoins}
        <div class="missions column-block align-end" transition:fade>
          <div class="coins column-block justify-center {coinsConfig.type === 'coins' ? 'special' : ''} {coinsConfig.type}">
            {#if coinsConfig.type === 'coins'}
              <div class="coins-amount row-block align-center">
                <span class="coins-amount__title">{$coins || coinsConfig.quest.amount}</span>
                <img 
                  class="coins-amount__picture" 
                  src="{cdn}img/main/hud/mcoin.svg" 
                  alt=""
                />
                <span class="coins-amount__text">Бесплатно</span>
              </div>
              
              <div class="time-left full-height full-width">
                {#if $received === 0 && !coinsConfig.quest.finished}
                  <span class="time-left__title">Осталось времени:</span>
                {/if}
                <span class="time-left__value">
                  {#if $received === 1 || coinsConfig.quest.finished}
                    Задание выполнено
                  {:else}
                    {$time || coinsConfig.quest.title}
                  {/if}
                </span>
              </div>
            {:else if coinsConfig.type === 'vehicle'}
              <div class="vehicle">
                <img 
                  src="{cdn}img/vehicles/{coinsConfig.model}.png" 
                  alt="{coinsConfig.model}.png" 
                  class="vehicle-image"
                />
                <div class="info-container">
                  <div class="text-containers">
                    <span class="title">
                      Играй {Math.floor(coinsConfig.limit / 60)} часов
                    </span>
                    <span class="subtitle">{coinsConfig.name || coinsConfig.model}</span>
                  </div>
                  <div class="combined-time-left">
                    {#if coinsConfig.quest?.finished}
                      <SVGComponent class="icon" path="icons/ipad/apps/organizations/contracts/tick.svg" />
                    {:else}
                      <span class="title">{coinsConfig.quest?.title || 'N/A'}</span>
                      <span class="subtitle">Осталось</span>
                    {/if}
                  </div>
                </div>
              </div>
            {:else if coinsConfig.type === 'case'}
              <div class="case">
                <img 
                  src="{cdn}img/panelMenu/donate/cases/{coinsConfig.case.type}.png" 
                  alt="{coinsConfig.case.type}.png" 
                  class="case-image"
                />
                <div class="info-container">
                  <div class="text-containers">
                    <span class="title">
                      Играй {Math.floor(coinsConfig.limit / 60)} часов
                    </span>
                    <span class="subtitle">Кейс</span>
                  </div>
                  <div class="combined-time-left">
                    {#if coinsConfig.quest?.finished}
                      <SVGComponent class="icon" path="icons/ipad/apps/organizations/contracts/tick.svg" />
                    {:else}
                      <span class="title">{coinsConfig.quest?.title || 'N/A'}</span>
                      <span class="subtitle">Осталось</span>
                    {/if}
                  </div>
                </div>
              </div>
            {:else if coinsConfig.type === 'clothes'}
              <div class="clothes">
                <img 
                  src="{cdn}clothesImages/{coinsConfig.imageHash}.webp" 
                  alt="cloth-alt" 
                  class="cloth-image"
                />
                <div class="info-container">
                  <div class="text-containers">
                    <span class="title">
                      Играй {Math.floor(coinsConfig.limit / 60)} часов
                    </span>
                    <span class="subtitle">{coinsConfig.name || 'Одежда'}</span>
                  </div>
                  <div class="combined-time-left">
                    {#if coinsConfig.quest?.finished}
                      <SVGComponent class="icon" path="icons/ipad/apps/organizations/contracts/tick.svg" />
                    {:else}
                      <span class="title">{coinsConfig.quest?.title || 'N/A'}</span>
                      <span class="subtitle">Осталось</span>
                    {/if}
                  </div>
                </div>
              </div>
            {:else if coinsConfig.type === 'combined'}
              <div class="combined {coinsConfig.name}">
                <div class="images">
                  {#if coinsConfig.data?.model}
                    <img 
                      src="{cdn}img/vehicles/{coinsConfig.data.model}.png" 
                      alt="{coinsConfig.data.model}.png" 
                      class="vehicle-image"
                    />
                  {/if}
                  {#if coinsConfig.data?.case}
                    <img 
                      src="{cdn}img/panelMenu/donate/cases/{coinsConfig.data.case.type}.png" 
                      alt="{coinsConfig.data.case.type}.png" 
                      class="case-image"
                    />
                  {/if}
                </div>
                <div class="info-container">
                  <div class="text-containers">
                    <span class="title">
                      Играй {Math.floor(coinsConfig.data?.limit / 60)} часов
                    </span>
                    <span class="subtitle">Комбинированный бонус</span>
                  </div>
                  <div class="combined-time-left">
                    {#if coinsConfig.quest?.finished}
                      <SVGComponent class="icon" path="icons/ipad/apps/organizations/contracts/tick.svg" />
                    {:else}
                      <span class="title">{coinsConfig.quest?.title || 'N/A'}</span>
                      <span class="subtitle">Осталось</span>
                    {/if}
                  </div>
                </div>
              </div>
            {/if}
          </div>
        </div>
      {/if}
      
      <!-- Миссии/Квесты - НОВЫЙ ДИЗАЙН -->
      {#if showMission}
        <div class="missions" transition:fade>
          <div class="active-missions row-block">
            <div class="main-missions full-width">
              <headers class="column-block">
                <div class="header-info column-block">
                  <div class="header-info__title">{mission.personName || quest?.Title || 'Квест'}</div>
                  <div class="header-info__text">{mission.personJob || ''}</div>
                </div>
                <div class="progress">
                  <div class="progress__line" style="width: 0%"></div>
                </div>
              </headers>
              
              <main class="full-width">
                <div class="main-content full-width column-block">
                  <div class="main-content__text">{mission.description || quest?.Text || ''}</div>
                  
                  <div class="quests column-block full-width">
                    {#if mission.quests && mission.quests.length > 0}
                      {#each mission.quests as questItem}
                        <div class="quest-block align-center row-block {questItem.finished ? 'finished' : ''}">
                          <div class="row-block align-center">
                            <div class="checkbox">
                              <input 
                                type="checkbox" 
                                bind:checked={questItem.finished}
                                on:click={(e) => e.preventDefault()}
                                class="checkbox__input"
                              />
                              {#if questItem.finished}
                                <svg class="checkbox__picture" xmlns="http://www.w3.org/2000/svg" width="11.797" height="9.667" viewBox="0 0 11.797 9.667">
                                  <path d="M1814.956,295.8l3.229,3.338,6.771-7" transform="translate(-1814.058 -291.268)" fill="none" stroke="#f8f8f8" stroke-width="2.5"/>
                                </svg>
                              {/if}
                            </div>
                            <div class="quest-block__title">{questItem.title}</div>
                          </div>
                        </div>
                      {/each}
                    {:else if quest}
                      <div class="quest-block align-center row-block">
                        <div class="row-block align-center">
                          <div class="checkbox">
                            <input 
                              type="checkbox" 
                              checked={false}
                              on:click={(e) => e.preventDefault()}
                              class="checkbox__input"
                            />
                          </div>
                          <div class="quest-block__title">{@html quest.Text}</div>
                        </div>
                      </div>
                    {/if}
                  </div>
                </div>
              </main>
            </div>
          </div>
        </div>
      {/if}
    </div>
  {/if}
</div>

<!-- Demorgan Window -->
{#if demorgan}
  <div class="contentD" transition:fade>
    <div class="contentD-main">
      <div class="contentD-main__block contentD-main__data">
        <div class="contentD-main__element">
          <p>Администратор:</p>
          <p>
            {demorgan.admin}
            {#if demorgan.adminId !== undefined}
              <span>#{demorgan.adminId}</span>
            {/if}
          </p>
        </div>
        
        <div class="contentD-main__element timerd">
          <p>Осталось:</p>
          <div class="timerd">
            <div class="timer-column">
              <p>{demorganTime.days}</p>
              <p>дней</p>
            </div>
            <div class="timer-column">
              <p>{demorganTime.hours}</p>
              <p>часов</p>
            </div>
            <div class="timer-column">
              <p>{demorganTime.minutes}</p>
              <p>минут</p>
            </div>
            <div class="timer-column">
              <p>{demorganTime.seconds}</p>
              <p>секунд</p>
            </div>
          </div>
        </div>
      </div>
      
      <div class="contentD-main__block contentD-main__element">
        <p>Причина:</p>
        <p>{demorgan.reason}</p>
      </div>
    </div>
  </div>
{/if}

<style>
  /* Стили уже должны быть в hud.css */
</style>