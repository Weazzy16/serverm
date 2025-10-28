<script>
  import { onMount, onDestroy } from 'svelte';
  import { createEventDispatcher } from 'svelte';
  import AccountComponent from './AccountComponent.svelte'; // или путь к компоненту

  const dispatch = createEventDispatcher();
  
  export let data;
  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  export let characterId = null;
  export let accountId = null;
  export let serverTimeOffset = 0;
  
  // Data variables
  let placeBetFocus = false;
  let placeBetAmount = "0";
  let showWinner = false;
  let animItem = null;
  let lastGameID = null;
  let positionSwitchingTimeFunc = 75;
  let passedColors = 0;
  let tickSound = null;
  let socketInterval = null;
  let wheelElement;
  let isLoading = true;
  let timerDisplay = "00.00";
  let timerInterval = null;
  
  // Переменные для сохранения ставок
  let savedBetsData = null;
  let gameEnded = false;

  // Переменные для контроля звука и видимости
  let isPageVisible = true;
  let isWheelAnimating = false;

  // Переменные для контроля состояния страниц
  let currentPage = 0;
  let isPageChanging = false;
  let pendingPageChange = null;

  // Локальное управление состоянием страниц
  let localPageState = {
    currentPage: 0,
    historyList: [],
    historyItem: null
  };

  const easing = {
    easeOutCubic: function(t) {
      return --t * t * t + 1;
    },
    easeInOutCubic: function(t) {
      return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
    }
  };
  
  // Computed properties
  $: maxPlayers = 999999;
  $: getAccountId = accountId !== null ? +accountId : null;
  $: getActiveTimer = data?.timer?.active ?? false;
  $: getInitTimeTimer = data?.timer?.initTime ?? 0;
  $: getTimeStartTimer = data?.timer?.timeStart ?? "";
  $: getActiveWheel = data?.wheel?.active ?? false;
  $: getActiveWindow = data?.active ?? false;
  $: getWheelStartRotation = data?.wheel?.rotation ?? 0;
  $: getSpinTime = data?.wheel?.spinTime ?? 5000;
  $: getTimeStartSpin = data?.wheel?.timeStart ?? "";
  $: getWheelToRotation = data?.wheel?.toRotation ?? 0;
  $: getOpenedPage = data?.pages?.[data?.openedPage ?? 0] ?? 'main';
  $: getHistory = data?.history ?? [];
  $: getCurrentGame = data?.currentGame ?? { 
    id: null, 
    betsTypes: { 
      white: {players: [], coefficient: 2}, 
      red: {players: [], coefficient: 3}, 
      green: {players: [], coefficient: 5}, 
      yellow: {players: [], coefficient: 14} 
    } 
  };
  $: getHistoryList = data?.historyList ?? [];
  $: getHistoryItem = data?.historyItem;
  $: getHistoryItemData = getHistoryItem?.game;
  $: getHistoryItemPlayers = getHistoryItem?.bets ?? {};
  $: getWinner = data?.winner;
  
  // Получаем текущую страницу (приоритет локальному состоянию)
  $: getCurrentPage = () => {
    if (isPageChanging || pendingPageChange) {
      return currentPage;
    }
    return localPageState.currentPage;
  };

  // Reactive блок для отслеживания изменений страницы
  $: {
    const serverPage = data?.openedPage ?? 0;
    
    // Обновляем currentPage только если это не наше собственное изменение
    if (!isPageChanging && serverPage !== currentPage) {
      currentPage = serverPage;
    }
  }
  
  // Логика сохранения ставок
  $: if (getCurrentGame?.id && getCurrentGame.id !== lastGameID) {
    if (lastGameID !== null) {
      savedBetsData = null;
      gameEnded = false;
    }
    lastGameID = getCurrentGame.id;
  }

  $: if (getActiveWheel && !savedBetsData) {
    savedBetsData = JSON.parse(JSON.stringify(getCurrentGame?.betsTypes || {}));
  }

  $: if (getWinner && showWinner && !gameEnded) {
    gameEnded = true;
  }

  // Reactive блок для остановки звука при изменении состояний
  $: if (!getActiveWheel) {
    isWheelAnimating = false;
    stopAllSounds();
  }

  // Функция для получения данных о ставках
  function getBetsData() {
    if (gameEnded && savedBetsData) {
      return savedBetsData;
    }
    return getCurrentGame?.betsTypes || {};
  }
  
  $: getWheelSpinDuration = (() => {
    let duration = Math.abs((getServerTimeWithOffset() - new Date(getTimeStartSpin) - getSpinTime) / 1000);
    if (duration <= 0) duration = 0;
    if (duration >= getSpinTime) duration = getSpinTime;
    return duration;
  })();
  
  $: getPercentageOfTimeRemainingSpin = (() => {
    let percentage = getWheelSpinDuration / (getSpinTime / 1000) * 100;
    if (percentage <= 0) percentage = 0;
    if (percentage >= 100) percentage = 100;
    return percentage;
  })();
  
  $: getWheelRotation = (() => {
    const startRotation = getWheelStartRotation;
    const rotationDiff = getWheelToRotation - startRotation;
    let currentRotation = rotationDiff / 100 * (getPercentageOfTimeRemainingSpin > positionSwitchingTimeFunc ? 100 : getPercentageOfTimeRemainingSpin);
    if (currentRotation <= 0) currentRotation = 0;
    if (currentRotation >= rotationDiff) currentRotation = rotationDiff;
    return getWheelToRotation - currentRotation;
  })();
  
  $: overrideDeg = (() => {
    if (getActiveTimer) {
      return getWheelStartRotation;
    }
    if (!getActiveWheel && getWinner && showWinner) {
      return getWheelToRotation;
    }
    return null;
  })();
  
  $: getAllPlayers = [
    ...getBetsData()?.white?.players ?? [],
    ...getBetsData()?.red?.players ?? [],
    ...getBetsData()?.green?.players ?? [],
    ...getBetsData()?.yellow?.players ?? []
  ];
  
  $: getPlayersCount = getAllPlayers.reduce((acc, player) => {
    const exists = acc.find(p => p.accountId === player.accountId && p.serverId === player.serverId);
    if (!exists) acc.push(player);
    return acc;
  }, []).length;
  
  $: getTotalBetsSum = getAllPlayers.reduce((sum, player) => sum + Number(player.amount || 0), 0);
  
  // Обработчик видимости страницы
  function handleVisibilityChange() {
    isPageVisible = !document.hidden;
    
    if (!isPageVisible) {
      stopAllSounds();
    }
  }

  // Функция остановки всех звуков
  function stopAllSounds() {
    if (tickSound) {
      try {
        if (typeof tickSound.stop === 'function') {
          tickSound.stop();
        } else if (typeof tickSound.pause === 'function') {
          tickSound.pause();
          tickSound.currentTime = 0;
        }
      } catch (error) {
        console.warn('Sound stop error:', error);
      }
    }
  }

  // ИСПРАВЛЕННАЯ функция таймера - плавное отображение миллисекунд
  function updateTimer() {
    if (!getActiveTimer) {
      timerDisplay = "00.00";
      return;
    }
    
    try {
      const now = Date.now();
      const elapsed = now - new Date(getTimeStartTimer).getTime();
      const remaining = Math.max(0, getInitTimeTimer - elapsed);
      
      const seconds = Math.floor(remaining / 1000);
      const ms = Math.floor((remaining % 1000) / 10);
      
      timerDisplay = `${String(seconds).padStart(2, '0')}.${String(ms).padStart(2, '0')}`;
    } catch (e) {
      timerDisplay = "00.00";
    }
  }
  
  function startTimerInterval() {
    if (timerInterval) clearInterval(timerInterval);
    // Уменьшаем интервал до 16ms (~60fps) для плавности
    timerInterval = setInterval(updateTimer, 16);
  }
  
  function stopTimerInterval() {
    if (timerInterval) {
      clearInterval(timerInterval);
      timerInterval = null;
    }
  }
  
  // Methods
  function getServerTimeWithOffset() {
    return new Date(Date.now() + serverTimeOffset);
  }
  
  function onPageLoaded() {
    if (getActiveWheel) startWheel();
    passedColors = Math.trunc(getWheelRotation / 18);
    
    // Загрузка звука
    if (typeof Howl !== 'undefined') {
      tickSound = new Howl({
        src: [`${cdn}sounds/donate/tick.ogg`],
        volume: 0.3,
        loop: false,
        preload: true,
        onloaderror: function(id, error) {
          console.error('Sound load error:', error);
        }
      });
    } else {
      try {
        tickSound = new Audio(`${cdn}sounds/donate/tick.ogg`);
        tickSound.volume = 0.3;
        tickSound.preload = 'auto';
      } catch (error) {
        console.error('Audio creation error:', error);
      }
    }
  }
  
  // ИСПРАВЛЕННАЯ функция openPage
  function openPage(pageNum, id = null) {
    if (isPageChanging) return; // предотвращаем двойные клики
    
    isPageChanging = true;
    currentPage = pageNum;
    pendingPageChange = { page: pageNum, id };
    
    console.log('[Wheel] Opening page:', pageNum, id);
    
    dispatch('openPage', { page: pageNum, id });
    
    // Сбрасываем флаг через небольшую задержку
    setTimeout(() => {
      isPageChanging = false;
    }, 500);
  }

  // Функция для локального переключения страниц
  function switchToPage(pageNum, id = null) {
    localPageState.currentPage = pageNum;
    
    if (pageNum === 1) {
      // Запрашиваем список истории
      dispatch('openPage', { page: 1, id: null });
    } else if (pageNum === 2 && id) {
      // Запрашиваем конкретную игру
      dispatch('openPage', { page: 2, id });
    } else {
      // Возвращаемся на главную
      localPageState.currentPage = 0;
    }
  }

  // Функция для кнопки истории
  function handleHistoryClick() {
    console.log('[Wheel] History button clicked');
    switchToPage(1);
  }

  // Функция возврата на главную страницу
  function goBackToMain() {
    console.log('[Wheel] Going back to main');
    switchToPage(0);
  }

  // Функция для просмотра конкретной игры
  function viewHistoryItem(gameId) {
    console.log('[Wheel] Viewing history item:', gameId);
    switchToPage(2, gameId);
  }
  
  function makeBet(color) {
    const amount = Number(placeBetAmount);
    if (amount > 0) {
      dispatch('bet', {
        amount: amount,
        color: color
      });
      placeBetAmount = "0";
    }
  }

  // Функция очистки поля ввода
  function clearBetAmount() {
    placeBetAmount = "0";
  }

  // Функция получения текущей ставки игрока на определенный цвет
  function getPlayerCurrentBet(color) {
    if (!getAccountId) return 0;
    
    const betsData = getBetsData();
    const colorPlayers = betsData?.[color]?.players || [];
    
    return colorPlayers
      .filter(player => player.accountId === getAccountId)
      .reduce((sum, bet) => sum + Number(bet.amount || 0), 0);
  }
  
  function placeBetInputFocus() {
    placeBetFocus = true;
    if (!placeBetAmount || Number(placeBetAmount) <= 0) {
      placeBetAmount = "";
    }
  }
  
  function placeBetInputUnFocus() {
    placeBetFocus = false;
    if (!placeBetAmount || Number(placeBetAmount) <= 0) {
      placeBetAmount = "0";
    }
  }
  
  function getBankOnColor(players) {
    let total = 0;
    players.forEach(player => {
      if (player.amount) total += Number(player.amount);
      else if (player.bet) total += Number(player.bet);
    });
    return total;
  }
  
  function getHistoryListColors(game) {
    return {
      white: game?.white ?? 0,
      red: game?.red ?? 0,
      green: game?.green ?? 0,
      yellow: game?.yellow ?? 0
    };
  }
  
  function playersSorting(players) {
    return [...players].sort((a, b) => Number(b.amount) - Number(a.amount));
  }
  
  function startWheel() {
    showWinner = false;
    animTo(
      getWheelRotation,
      getWheelToRotation,
      getWheelSpinDuration,
      getPercentageOfTimeRemainingSpin < 50 ? easing.easeOutCubic : easing.easeInOutCubic,
      updateAnimateWheel
    );
  }
  
  function updateAnimateWheel() {
    showWinner = true;
    if (getOpenedPage === 'historyList') {
      dispatch('getWheelHistory');
    }
  }
  
  // ИСПРАВЛЕННАЯ анимация колеса с контролем звука
  function animTo(from, to, duration, easingFunc, callback) {
    const startTime = Date.now();
    isWheelAnimating = true;
    
    if (from === to) {
      isWheelAnimating = false;
      if (callback) callback();
      return;
    }
    
    const min = (a, b) => a < b ? a : b;
    let lastSoundAngle = Math.floor(from / 18) * 18;
    
    const animate = () => {
      try {
        if (!getActiveWheel || !isWheelAnimating) {
          isWheelAnimating = false;
          return;
        }
        
        const now = Date.now();
        const progress = min(1, (now - startTime) / (duration * 1000));
        const easedProgress = easingFunc(progress);
        const currentValue = easedProgress * (to - from) + from;
        
        if (wheelElement) {
          wheelElement.style.transform = `rotate(${currentValue}deg)`;
        }
        
        // Звук только если страница видима И анимация активна
        const currentSoundAngle = Math.floor(currentValue / 18) * 18;
        if (currentSoundAngle > lastSoundAngle && tickSound && isPageVisible && isWheelAnimating) {
          lastSoundAngle = currentSoundAngle;
          try {
            if (typeof tickSound.stop === 'function') {
              tickSound.stop();
              tickSound.play();
            } else {
              tickSound.currentTime = 0;
              const playPromise = tickSound.play();
              if (playPromise) {
                playPromise.catch(e => console.warn('Sound play error:', e));
              }
            }
          } catch (error) {
            console.warn('Sound play error:', error);
          }
        }
        
        if (progress < 1) {
          requestAnimationFrame(animate);
        } else {
          isWheelAnimating = false;
          if (callback) callback();
        }
      } catch (error) {
        console.error('Animation error:', error);
        isWheelAnimating = false;
      }
    };
    
    requestAnimationFrame(animate);
  }
  
  function preventForNumber(event) {
    const char = String.fromCharCode(event.which);
    if (!/[0-9]/.test(char)) {
      event.preventDefault();
    }
  }
  
function formatNumber(num) {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
}
  
  // Watch логика
  $: if (getActiveWheel) {
    passedColors = Math.trunc(getWheelRotation / 18);
    startWheel();
    showWinner = false;
  }
  
  $: if (getActiveTimer) {
    startTimerInterval();
    updateTimer();
  } else {
    stopTimerInterval();
    timerDisplay = "00.00";
  }
  
  $: if (!isLoading) {
    onPageLoaded();
  }
  
  onMount(() => {
    isLoading = false;
    
    // Подписываемся на изменение видимости страницы
    document.addEventListener('visibilitychange', handleVisibilityChange);
    
    // Также отслеживаем потерю фокуса окна
    window.addEventListener('blur', () => {
      isPageVisible = false;
      stopAllSounds();
    });
    
    window.addEventListener('focus', () => {
      isPageVisible = true;
    });
  });
  
  onDestroy(() => {
    stopTimerInterval();
    if (socketInterval) {
      clearInterval(socketInterval);
    }
    
    // Останавливаем анимацию
    isWheelAnimating = false;
    
    // Отписываемся от всех событий
    document.removeEventListener('visibilitychange', handleVisibilityChange);
    window.removeEventListener('blur', () => {
      isPageVisible = false;
      stopAllSounds();
    });
    window.removeEventListener('focus', () => {
      isPageVisible = true;
    });
    
    // Финальная остановка звуков
    stopAllSounds();
  });
</script>
<div class="Wheel full-width full-height row-block align-start justify-between">
  {#if isLoading}
    <div class="no-content">
      <h1>Loading...</h1>
    </div>
  {:else if !getActiveWindow}
    <div class="no-content">
      <h1>Coming soon...</h1>
    </div>
  {:else}
    <div class="full-width full-height">
      {#if getCurrentPage() === 'main' || getCurrentPage() === 0}
        <div class="main full-width full-height row-block align-start justify-center">
          <div class="game align-start justify-start column-block full-height">
            <img class="back full-height full-width" src="{cdn}/img/panelMenu/miniGames/wheel/gameBack.png" alt="" />
            
            <div class="history row-block align-center justify-start">
              <button class="full-height" on:click={handleHistoryClick}>
                <svg width="24" height="24" viewBox="0 0 24 24" fill="white">
                  <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8zm.5-13H11v6l5.25 3.15.75-1.23-4.5-2.67V7z"/>
                </svg>
              </button>
              
              {#if getHistory && getHistory.length > 0}
                <div class="itemsw row-block align-center justify-start">
                  {#each getHistory as historyItem, index}
                    <div class="itemw {historyItem.colorWin}"></div>
                  {/each}
                </div>
              {:else}
                <div class="itemsw emptyHistoryItems row-block align-center justify-start">
                  {#each Array(15) as _, index}
                    <div class="itemw"></div>
                  {/each}
                </div>
              {/if}
            </div>
            
            <!-- Отображение времени/результата -->
            {#if getWinner && showWinner}
              <div class="timeStart column-block align-center justify-start">
                <span class="time {getWinner}">
                  {getCurrentGame?.betsTypes?.[getWinner]?.coefficient || 2}X
                </span>
                <span class="text">Выиграло</span>
              </div>
            {:else if getActiveTimer}
              <div class="timeStart column-block align-center justify-start">
                <span class="time">{timerDisplay}</span>
                <span class="text">Начало через</span>
              </div>
            {:else if getActiveWheel}
              <div class="timeStart column-block align-center justify-start">
                <span class="time spinning">...</span>
                <span class="text">Spinning</span>
              </div>
            {:else}
              <div class="timeStart column-block align-center justify-start">
                <span class="time">...</span>
                <span class="text">Игра началась</span>
              </div>
            {/if}
            
            <div class="wheelWrapper">
              <svg 
                bind:this={wheelElement}
                class="wheel full-width full-height"
                xmlns="http://www.w3.org/2000/svg"
                width="1000"
                height="1000"
                viewBox="0 0 1000 1000"
                fill="none"
                style={overrideDeg !== null ? `transform: rotate(${overrideDeg}deg)` : ''}
              >
                <mask id="mask0_39_7520" style="mask-type:alpha;" maskUnits="userSpaceOnUse" x="0" y="0" width="1000" height="1000">
                  <circle cx="500.277" cy="500.754" r="432.5" stroke="black" stroke-width="135"></circle>
                </mask>
                <g mask="url(#mask0_39_7520)">
                  <path d="M500.275 500.754L904.782 794.647C935.339 752.66 959.516 705.735 975.895 655.295L500.275 500.754Z" fill="url(#paint0_linear_39_7520)"></path>
                  <path d="M500.278 500.754L95.7715 794.647C126.632 837.049 163.988 874.406 206.385 905.261L500.278 500.754Z" fill="url(#paint1_linear_39_7520)"></path>
                  <path d="M500.276 500.754L206.383 905.261C248.369 935.817 295.294 959.994 345.734 976.374L500.276 500.754Z" fill="url(#paint2_linear_39_7520)"></path>
                  <path d="M500.276 500.753L95.7694 206.86C65.2077 248.847 41.0358 295.772 24.6562 346.212L500.276 500.753Z" fill="url(#paint3_linear_39_7520)"></path>
                  <path d="M500.277 500.754H0.277344C0.277344 554.693 8.8538 606.621 24.6573 655.29L500.277 500.754Z" fill="url(#paint4_linear_39_7520)"></path>
                  <path d="M24.6562 655.29C41.0358 705.735 65.2077 752.655 95.7694 794.641L500.276 500.748L24.6562 655.29Z" fill="url(#paint5_linear_39_7520)"></path>
                  <path d="M654.812 976.368C705.257 959.989 752.177 935.817 794.163 905.255L500.275 500.754L654.812 976.368Z" fill="url(#paint6_linear_39_7520)"></path>
                  <path d="M500.275 500.754L794.168 905.261C836.571 874.4 873.927 837.044 904.782 794.647L500.275 500.754Z" fill="url(#paint7_linear_39_7520)"></path>
                  <path d="M500.275 500.754V1000.75C554.214 1000.75 606.142 992.177 654.812 976.374L500.275 500.754Z" fill="url(#paint8_linear_39_7520)"></path>
                  <path d="M345.736 976.368C394.406 992.172 446.334 1000.75 500.273 1000.75V500.754L345.736 976.368Z" fill="url(#paint9_linear_39_7520)"></path>
                  <path d="M24.6573 346.212C8.8538 394.887 0.277344 446.815 0.277344 500.754H500.277L24.6573 346.212Z" fill="url(#paint10_linear_39_7520)"></path>
                  <path d="M1000.27 500.754C1000.27 446.815 991.694 394.887 975.89 346.218L500.275 500.754H1000.27Z" fill="url(#paint11_linear_39_7520)"></path>
                  <path d="M500.275 500.754L904.782 206.86C873.922 164.458 836.565 127.102 794.168 96.2466L500.275 500.754Z" fill="url(#paint12_linear_39_7520)"></path>
                  <path d="M500.275 499.939L975.895 345.403C959.516 294.958 935.344 248.038 904.782 206.052L500.275 499.939Z" fill="url(#paint13_linear_39_7520)"></path>
                  <path d="M500.275 500.754L975.895 655.29C991.699 606.621 1000.28 554.693 1000.28 500.754H500.275Z" fill="url(#paint14_linear_39_7520)"></path>
                  <path d="M500.275 500.754L654.812 25.1339C606.142 9.33036 554.214 0.753906 500.275 0.753906V500.754Z" fill="url(#paint15_linear_39_7520)"></path>
                  <path d="M500.278 0.753906C446.339 0.753906 394.411 9.33036 345.736 25.1339L500.273 500.754V0.753906H500.278Z" fill="url(#paint16_linear_39_7520)"></path>
                  <path d="M794.163 96.2474C752.177 65.6911 705.252 41.5138 654.812 25.1343L500.275 500.754L794.163 96.2474Z" fill="url(#paint17_linear_39_7520)"></path>
                  <path d="M500.276 500.754L345.734 25.1343C295.289 41.5138 248.369 65.6858 206.383 96.2474L500.276 500.754Z" fill="url(#paint18_linear_39_7520)"></path>
                  <path d="M206.383 96.2466C163.981 127.107 126.625 164.464 95.7695 206.86L500.277 500.754L206.383 96.2466Z" fill="url(#paint19_linear_39_7520)"></path>
                </g>
                <defs>
                  <linearGradient id="paint0_linear_39_7520" x1="946.467" y1="705.224" x2="530.905" y2="472.608" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint1_linear_39_7520" x1="189.848" y1="831.052" x2="347.133" y2="672.939" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#E81C5A"></stop>
                    <stop offset="0.34101" stop-color="#9F1D44"></stop>
                  </linearGradient>
                  <linearGradient id="paint2_linear_39_7520" x1="284.217" y1="947.774" x2="390.775" y2="476.02" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint3_linear_39_7520" x1="47.4632" y1="269.794" x2="160.046" y2="336.019" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#3DE7A5"></stop>
                    <stop offset="1" stop-color="#22A874"></stop>
                  </linearGradient>
                  <linearGradient id="paint4_linear_39_7520" x1="59.88" y1="603.403" x2="282.562" y2="547.111" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#E81C5A"></stop>
                    <stop offset="0.333031" stop-color="#9F1D44"></stop>
                  </linearGradient>
                  <linearGradient id="paint5_linear_39_7520" x1="62.3638" y1="739.992" x2="456.404" y2="538.833" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint6_linear_39_7520" x1="736.203" y1="939.495" x2="526.765" y2="522.277" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint7_linear_39_7520" x1="867.825" y1="869.959" x2="769.315" y2="766.482" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#3DE7A5"></stop>
                    <stop offset="1" stop-color="#22A874"></stop>
                  </linearGradient>
                  <linearGradient id="paint8_linear_39_7520" x1="578.09" y1="935.356" x2="544.977" y2="766.482" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#E81C5A"></stop>
                    <stop offset="0.34101" stop-color="#9F1D44"></stop>
                  </linearGradient>
                  <linearGradient id="paint9_linear_39_7520" x1="429.087" y1="970.125" x2="458.06" y2="783.038" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#3DE7A5"></stop>
                    <stop offset="0.417506" stop-color="#22A874"></stop>
                  </linearGradient>
                  <linearGradient id="paint10_linear_39_7520" x1="0.277341" y1="441.979" x2="383.556" y2="500.754" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint11_linear_39_7520" x1="1000.28" y1="422.939" x2="500.275" y2="496.66" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint12_linear_39_7520" x1="858.719" y1="140.654" x2="692.681" y2="565.274" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint13_linear_39_7520" x1="912.527" y1="288.005" x2="681.567" y2="391.482" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#E81C5A"></stop>
                    <stop offset="0.34101" stop-color="#9F1D44"></stop>
                  </linearGradient>
                  <linearGradient id="paint14_linear_39_7520" x1="948.123" y1="566.151" x2="656.732" y2="521.449" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#E81C5A"></stop>
                    <stop offset="0.34101" stop-color="#9F1D44"></stop>
                  </linearGradient>
                  <linearGradient id="paint15_linear_39_7520" x1="577.544" y1="0.753906" x2="500.275" y2="487.509" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint16_linear_39_7520" x1="423.007" y1="0.753907" x2="480.411" y2="332.708" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#F2BF3B"></stop>
                    <stop offset="0.304173" stop-color="#F29E3B"></stop>
                  </linearGradient>
                  <linearGradient id="paint17_linear_39_7520" x1="731.236" y1="46.284" x2="663.355" y2="168.801" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#3DE7A5"></stop>
                    <stop offset="1" stop-color="#22A874"></stop>
                  </linearGradient>
                  <linearGradient id="paint18_linear_39_7520" x1="271.8" y1="57.8734" x2="553.641" y2="394.771" gradientUnits="userSpaceOnUse">
                    <stop stop-color="white"></stop>
                    <stop offset="0.293038" stop-color="#9A9A9A"></stop>
                  </linearGradient>
                  <linearGradient id="paint19_linear_39_7520" x1="184.879" y1="182.873" x2="337.197" y2="325.257" gradientUnits="userSpaceOnUse">
                    <stop stop-color="#E81C5A"></stop>
                    <stop offset="0.34101" stop-color="#9F1D44"></stop>
                  </linearGradient>
                </defs>
              </svg>
              <div class="line"></div>
            </div>
            
            <img class="dealer" src="{cdn}/img/panelMenu/miniGames/wheel/dealer.png" alt="" />
            
            <div class="placeBet column-block align-center justify-start" class:lose={!getActiveTimer || getPlayersCount >= maxPlayers}>
              <div class="inputWrapper full-width">
                <input 
                  bind:value={placeBetAmount}
                  type="text"
                  maxlength="6"
                  class="full-width full-height"
                  class:focus={placeBetFocus}
                  disabled={!getActiveTimer}
                  on:focus={placeBetInputFocus}
                  on:blur={placeBetInputUnFocus}
                  on:keypress={preventForNumber}
                  on:cut|preventDefault
                  on:copy|preventDefault
                  on:paste|preventDefault
                />
                <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
              </div>
              
              <div class="colors row-block align-center justify-between full-width">
                {#each Object.entries(getCurrentGame?.betsTypes || {}) as [colorKey, colorData]}
                  {@const playerBet = getPlayerCurrentBet(colorKey)}
                  <button 
                    class="color align-center justify-center {colorKey}"
                    class:current-bet={playerBet > 0}
                    class:disabled={!getActiveTimer || Number(placeBetAmount) <= 0}
                    disabled={!getActiveTimer || Number(placeBetAmount) <= 0}
                    on:click={() => makeBet(colorKey)}
                  >
                    {colorData?.coefficient || 2}X
                    
                  </button>
                {/each}
              </div>
            </div>
          </div>
          
          <div class="bets align-start justify-start column-block full-height">
            <div class="info row-block align-center justify-between full-width">
              <div class="row-block align-center justify-start">
                <svg class="minigamesIcon" xmlns="http://www.w3.org/2000/svg" width="31" height="25" viewBox="0 0 31 25" fill="none">
<path d="M9.30349 0.996582C4.18849 0.996582 0.0214844 5.16356 0.0214844 10.2786V18.9966C0.0214844 22.3101 2.70798 24.9966 6.02148 24.9966C8.14248 24.9966 9.87199 23.7276 10.943 21.9966C12.206 21.9936 17.654 22.0026 19.1465 21.9966C20.2205 23.7141 21.9095 24.9966 24.0215 24.9966C27.335 24.9966 30.0215 22.3101 30.0215 18.9966V10.2786C30.0215 5.16356 25.8545 0.996582 20.7395 0.996582H9.30349ZM9.02148 6.99658C9.84948 6.99658 10.5215 7.66858 10.5215 8.49658V9.99658H12.0215C12.8495 9.99658 13.5215 10.6686 13.5215 11.4966C13.5215 12.3246 12.8495 12.9966 12.0215 12.9966H10.5215V14.4966C10.5215 15.3246 9.84948 15.9966 9.02148 15.9966C8.19348 15.9966 7.52148 15.3246 7.52148 14.4966V12.9966H6.02148C5.19348 12.9966 4.52148 12.3246 4.52148 11.4966C4.52148 10.6686 5.19348 9.99658 6.02148 9.99658H7.52148V8.49658C7.52148 7.66858 8.19348 6.99658 9.02148 6.99658ZM21.0215 6.99658C21.8495 6.99658 22.5215 7.66858 22.5215 8.49658C22.5215 9.32458 21.8495 9.99658 21.0215 9.99658C20.1935 9.99658 19.5215 9.32458 19.5215 8.49658C19.5215 7.66858 20.1935 6.99658 21.0215 6.99658ZM18.0215 9.99658C18.8495 9.99658 19.5215 10.6686 19.5215 11.4966C19.5215 12.3246 18.8495 12.9966 18.0215 12.9966C17.1935 12.9966 16.5215 12.3246 16.5215 11.4966C16.5215 10.6686 17.1935 9.99658 18.0215 9.99658ZM24.0215 9.99658C24.8495 9.99658 25.5215 10.6686 25.5215 11.4966C25.5215 12.3246 24.8495 12.9966 24.0215 12.9966C23.1935 12.9966 22.5215 12.3246 22.5215 11.4966C22.5215 10.6686 23.1935 9.99658 24.0215 9.99658ZM21.0215 12.9966C21.8495 12.9966 22.5215 13.6686 22.5215 14.4966C22.5215 15.3246 21.8495 15.9966 21.0215 15.9966C20.1935 15.9966 19.5215 15.3246 19.5215 14.4966C19.5215 13.6686 20.1935 12.9966 21.0215 12.9966Z"/>
<defs>
<linearGradient id="paint0_linear_55_3401" x1="15.0215" y1="0.996582" x2="15.0215" y2="24.9966" gradientUnits="userSpaceOnUse">
<stop stop-color="white"/>
<stop offset="1" stop-color="#9A9A9A"/>
</linearGradient>
</defs>
</svg>
                <span class="title">Игра</span>
                <div class="gameID">
                  {#if getCurrentGame.id}
                    <span>#{getCurrentGame.id}</span>
                  {/if}
                </div>
              </div>
              
              <div class="row-block align-center justify-start">
                <div class="players row-block align-center justify-start">
                  <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 30 30">
	<path d="M15 15.5C19.1179 15.5 22.4667 12.1363 22.4667 8C22.4667 3.86375 19.1179 0.5 15 0.5C10.8821 0.5 7.53333 3.86375 7.53333 8C7.53333 12.1363 10.8821 15.5 15 15.5ZM17.8 17.375H12.2C6.0232 17.375 1 22.4206 1 28.625V30.5H29V28.625C29 22.4206 23.9768 17.375 17.8 17.375Z"/>

	<defs>
		<linearGradient id="paint0_linear_55_3401" x1="15" y1="0.5" x2="15" y2="30.5" gradientUnits="userSpaceOnUse">
			<stop stop-color="white"/>
			<stop offset="1" stop-color="#999999"/>
		</linearGradient>
	</defs>
</svg>
                  <div class="players-info column-block align-start justify-between">
                    
                    <span class="count">{getPlayersCount}</span>
                    <span class="text">Игроки</span>
                  </div>
                </div>
                
                <div class="bank row-block align-center justify-start">
                  <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="35" height="35" viewBox="0 0 35 35">
	<path d="M26 6.5C30.4183 6.5 34 10.0817 34 14.5V19.5C34 23.9183 30.4183 27.5 26 27.5H9C4.58172 27.5 1 23.9183 1 19.5V14.5C1 10.0817 4.58172 6.5 9 6.5H26ZM6.93945 21.6504C5.75269 20.4636 4.3902 19.9019 3.89551 20.3955C3.40116 20.8899 3.96233 22.2533 5.14941 23.4404C6.3365 24.6275 7.69999 25.1887 8.19434 24.6943C8.68796 24.1996 8.12621 22.8371 6.93945 21.6504ZM30.9023 20.3955C30.4076 19.9019 29.0452 20.4636 27.8584 21.6504C26.6716 22.8371 26.1099 24.1996 26.6035 24.6943C27.0979 25.1887 28.4613 24.6275 29.6484 23.4404C30.8355 22.2533 31.3967 20.8899 30.9023 20.3955ZM17.5 12.5C15.0147 12.5 13 14.5147 13 17C13 19.4853 15.0147 21.5 17.5 21.5C19.9853 21.5 22 19.4853 22 17C22 14.5147 19.9853 12.5 17.5 12.5ZM17.5 15.5C18.3284 15.5 19 16.1716 19 17C19 17.8284 18.3284 18.5 17.5 18.5C16.6716 18.5 16 17.8284 16 17C16 16.1716 16.6716 15.5 17.5 15.5ZM8.19434 10.1045C7.69999 9.61014 6.3365 10.1713 5.14941 11.3584C3.96233 12.5455 3.40116 13.909 3.89551 14.4033C4.3902 14.8969 5.7527 14.3352 6.93945 13.1484C8.12621 11.9617 8.68796 10.5992 8.19434 10.1045ZM29.6484 11.3584C28.4613 10.1713 27.0979 9.61014 26.6035 10.1045C26.1099 10.5992 26.6716 11.9617 27.8584 13.1484C29.0452 14.3352 30.4076 14.8969 30.9023 14.4033C31.3967 13.909 30.8355 12.5455 29.6484 11.3584Z"/>

	<defs>
		<linearGradient id="paint0_linear_55_3401" x1="17.5" y1="6.5" x2="17.5" y2="27.5" gradientUnits="userSpaceOnUse">
			<stop stop-color="white"/>
			<stop offset="1" stop-color="#999999"/>
		</linearGradient>
	</defs>
</svg>
                  <div class="bank-info column-block align-start justify-between">
                    
                    <span class="count row-block align-center justify-start">
                      {formatNumber(getTotalBetsSum)}
                      <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                    </span>
                    <span class="text">Банк</span>
                  </div>
                </div>
              </div>
            </div>
            
            <div class="colorsInfo full-width row-block align-center justify-between">
              {#each Object.entries(getCurrentGame?.betsTypes || {}) as [colorKey, colorData]}
                <div class="color full-height column-block align-start justify-between" 
                     class:lose={getWinner && showWinner && getWinner !== colorKey}>
                  <div class="coefficient full-width row-block align-center justify-center {colorKey}">
                    {colorData?.coefficient || 2}X
                  </div>
                  <div class="colorInfo row-block align-center justify-between">
                    <div class="playersWrapper row-block align-center justify-start">
                      <span class="count">{colorData?.players?.length || 0}</span>
                    </div>
                    <div class="bankWrapper row-block align-center justify-start">
                      <span class="count">{formatNumber(getBankOnColor(colorData?.players || []))}</span>
                      <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                    </div>
                  </div>
                </div>
              {/each}
            </div>
            
            <div class="betsOnColor full-width full-height row-block align-start justify-between">
              {#each Object.entries(getCurrentGame?.betsTypes || {}) as [colorKey, colorData]}
                <div class="betOnColor full-height column-block align-center justify-start" 
                     class:empty={!colorData?.players?.length}
                     class:lose={getWinner && showWinner && getWinner !== colorKey}>
                  {#if !colorData?.players?.length}
                    <div class="emptyBets column-block align-center justify-center">
                      <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 36 36">
    <path d="M8.98633 4.49756C8.15833 4.49756 7.48633 5.16956 7.48633 5.99756C7.48633 6.82556 8.15833 7.49756 8.98633 7.49756H26.9863C27.8143 7.49756 28.4863 6.82556 28.4863 5.99756C28.4863 5.16956 27.8143 4.49756 26.9863 4.49756H8.98633ZM7.48633 10.4976C6.65833 10.4976 5.98633 11.1696 5.98633 11.9976C5.98633 12.8256 6.65833 13.4976 7.48633 13.4976H28.4863C29.3143 13.4976 29.9863 12.8256 29.9863 11.9976C29.9863 11.1696 29.3143 10.4976 28.4863 10.4976H7.48633ZM8.98633 16.4976C6.50083 16.4976 4.48633 18.5121 4.48633 20.9976V26.9976C4.48633 29.4831 6.50083 31.4976 8.98633 31.4976H26.9863C29.4718 31.4976 31.4863 29.4831 31.4863 26.9976V20.9976C31.4863 18.5121 29.4718 16.4976 26.9863 16.4976H8.98633Z"/>

    <defs>
        <linearGradient id="paint0_linear_55_3401" x1="17.9648" y1="7.49609" x2="17.9648" y2="28.4961" gradientUnits="userSpaceOnUse">
            <stop stop-color="white"/>
            <stop offset="1" stop-color="#9A9A9A"/>
        </linearGradient>
    </defs>
</svg>
                      <span class="text">Ставок нет</span>
                    </div>
                  {/if}
                  
                  {#each playersSorting(colorData?.players || []) as player, index}
                    <div class="player full-width row-block align-center justify-between">
                      <AccountComponent
                          displayedComponents={["accountId", "serverName", "you"]}
                        login={player.login}
                        accountId={player.accountId}
                        serverName={player.serverName}
                        serverId={player.serverId}
                          currentUserId={getAccountId}

                      />  
                      <div class="amount full-height row-block align-center justify-start">
                        <span class="text">{formatNumber(player.amount || 0)}</span>
                        <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                      </div>
                    </div>
                  {/each}
                </div>
              {/each}
            </div>
          </div>
        </div>
        
      {:else if getCurrentPage() === 'historyList' || getCurrentPage() === 1}
        <div class="historyList full-width full-height column-block align-center justify-start">
          <div class="title row-block align-center justify-between full-width">
            <span class="text"><div class="back__icon active" on:click={goBackToMain}><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none"><path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"></path></svg></div>           Game History - Wheel</span>

          </div>
          
          {#if getHistoryList && getHistoryList.length > 0}
            <div class="list full-width full-height column-block align-center justify-start">
              {#each getHistoryList as historyGame, index}
                <button class="game row-block align-center justify-between full-width"
                        on:click={() => viewHistoryItem(historyGame.id)}>
                  <div class="historyListItem">
                    <div class="gameID">
                      <span>#{historyGame.id}</span>
                    </div>
                  </div>
                  
                  <div class="historyListItem">
                    <div class="playersBank row-block align-center justify-start">
                      <div class="playersCount row-block align-center justify-start">
                        {historyGame.playersCount}
                      </div>
                      <div class="bank row-block align-center justify-start">
                        {formatNumber(historyGame.bank)}
                        <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                      </div>
                    </div>
                  </div>
                  
                  <div class="historyListItem">
                    <div class="betsWrapper row-block align-center justify-start">
                      {#each Object.entries(getHistoryListColors(historyGame)) as [colorKey, amount]}
                        <div class="bet row-block align-center justify-start" class:win={historyGame.colorWin === colorKey}>
                          <div class="color {colorKey}"></div>
                          <span>{formatNumber(amount)}</span>
                          <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                        </div>
                      {/each}
                    </div>
                  </div>
                </button>
              {/each}
            </div>
          {:else}
            <span class="empty-list">История пуста</span>
          {/if}
        </div>
        
      {:else if (getCurrentPage() === 'historyItem' || getCurrentPage() === 2) && getHistoryItem && getHistoryItemData}
        <div class="historyItem full-width full-height column-block align-center justify-start">
          <div class="title row-block align-center justify-between full-width">
            <div class="buttons row-block align-center justify-start">
          
              
            </div>
          </div>
          
          <div class="info row-block align-center justify-between full-width">
            <div class="row-block align-center justify-start">
               <div class="back-btn">
                <div class="back__icon active" on:click={() => switchToPage(1)}><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none"><path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"></path></svg></div>
               </div>
              <span class="title">Игра</span>
              <div class="gameID">
                <span>#{getHistoryItemData.id}</span>
              </div>
            </div>
            
            <div class="betsWrapper row-block align-center justify-start">
              {#each Object.entries(getHistoryListColors(getHistoryItemData)) as [colorKey, amount]}
                <div class="bet row-block align-center justify-start" class:win={getHistoryItemData.colorWin === colorKey}>
                  <div class="color {colorKey}"></div>
                  <span>{formatNumber(amount)}</span>
                  <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                </div>
              {/each}
            </div>
            
            <div class="row-block align-center justify-start">
              <div class="players row-block align-center justify-start">
                 <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 30 30">
	<path d="M15 15.5C19.1179 15.5 22.4667 12.1363 22.4667 8C22.4667 3.86375 19.1179 0.5 15 0.5C10.8821 0.5 7.53333 3.86375 7.53333 8C7.53333 12.1363 10.8821 15.5 15 15.5ZM17.8 17.375H12.2C6.0232 17.375 1 22.4206 1 28.625V30.5H29V28.625C29 22.4206 23.9768 17.375 17.8 17.375Z"/>

	<defs>
		<linearGradient id="paint0_linear_55_3401" x1="15" y1="0.5" x2="15" y2="30.5" gradientUnits="userSpaceOnUse">
			<stop stop-color="white"/>
			<stop offset="1" stop-color="#999999"/>
		</linearGradient>
	</defs>
</svg>
                <div class="players-info column-block align-start justify-between">
                  <span class="count">{getHistoryItemData.playersCount}</span>
                  <span class="text">Игроки</span>
                </div>
              </div>
              
              <div class="bank row-block align-center justify-start">
                <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="35" height="35" viewBox="0 0 35 35">
	<path d="M26 6.5C30.4183 6.5 34 10.0817 34 14.5V19.5C34 23.9183 30.4183 27.5 26 27.5H9C4.58172 27.5 1 23.9183 1 19.5V14.5C1 10.0817 4.58172 6.5 9 6.5H26ZM6.93945 21.6504C5.75269 20.4636 4.3902 19.9019 3.89551 20.3955C3.40116 20.8899 3.96233 22.2533 5.14941 23.4404C6.3365 24.6275 7.69999 25.1887 8.19434 24.6943C8.68796 24.1996 8.12621 22.8371 6.93945 21.6504ZM30.9023 20.3955C30.4076 19.9019 29.0452 20.4636 27.8584 21.6504C26.6716 22.8371 26.1099 24.1996 26.6035 24.6943C27.0979 25.1887 28.4613 24.6275 29.6484 23.4404C30.8355 22.2533 31.3967 20.8899 30.9023 20.3955ZM17.5 12.5C15.0147 12.5 13 14.5147 13 17C13 19.4853 15.0147 21.5 17.5 21.5C19.9853 21.5 22 19.4853 22 17C22 14.5147 19.9853 12.5 17.5 12.5ZM17.5 15.5C18.3284 15.5 19 16.1716 19 17C19 17.8284 18.3284 18.5 17.5 18.5C16.6716 18.5 16 17.8284 16 17C16 16.1716 16.6716 15.5 17.5 15.5ZM8.19434 10.1045C7.69999 9.61014 6.3365 10.1713 5.14941 11.3584C3.96233 12.5455 3.40116 13.909 3.89551 14.4033C4.3902 14.8969 5.7527 14.3352 6.93945 13.1484C8.12621 11.9617 8.68796 10.5992 8.19434 10.1045ZM29.6484 11.3584C28.4613 10.1713 27.0979 9.61014 26.6035 10.1045C26.1099 10.5992 26.6716 11.9617 27.8584 13.1484C29.0452 14.3352 30.4076 14.8969 30.9023 14.4033C31.3967 13.909 30.8355 12.5455 29.6484 11.3584Z"/>

	<defs>
		<linearGradient id="paint0_linear_55_3401" x1="17.5" y1="6.5" x2="17.5" y2="27.5" gradientUnits="userSpaceOnUse">
			<stop stop-color="white"/>
			<stop offset="1" stop-color="#999999"/>
		</linearGradient>
	</defs>
</svg>
                <div class="bank-info column-block align-start justify-between">
                  <span class="count row-block align-center justify-start">
                    {formatNumber(getHistoryItemData.bank)}
                    <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                  </span>
                  <span class="text">Банк</span>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Информация о коэффициентах по цветам -->
          <div class="colorsInfo full-width row-block align-center justify-between">
            {#each Object.entries(getHistoryListColors(getHistoryItemData)) as [colorKey, amount]}
              <div class="color full-height column-block align-start justify-between" 
                   class:win={getHistoryItemData.colorWin === colorKey}>
                <div class="coefficient full-width row-block align-center justify-center {colorKey}">
                  {getCurrentGame?.betsTypes?.[colorKey]?.coefficient || (colorKey === 'white' ? 2 : colorKey === 'red' ? 3 : colorKey === 'green' ? 5 : 14)}X
                </div>
                <div class="colorInfo row-block align-center justify-between">
                  <div class="playersWrapper row-block align-center justify-start">
                    <span class="count">{getHistoryItemPlayers?.[colorKey]?.length || 0}</span>
                  </div>
                  <div class="bankWrapper row-block align-center justify-start">
                    <span class="count">{formatNumber(getBankOnColor(getHistoryItemPlayers?.[colorKey] || []))}</span>
                    <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                  </div>
                </div>
              </div>
            {/each}
          </div>
          
          <!-- Список ставок игроков по цветам -->
          <div class="betsOnColor full-width full-height row-block align-start justify-between">
            {#each Object.entries(getHistoryListColors(getHistoryItemData)) as [colorKey, amount]}
              <div class="betOnColor full-height column-block align-center justify-start" 
                   class:empty={!getHistoryItemPlayers?.[colorKey]?.length}
                   class:win={getHistoryItemData.colorWin === colorKey}>
                {#if !getHistoryItemPlayers?.[colorKey]?.length}
                  <div class="emptyBets column-block align-center justify-center">
                    <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 36 36">
    <path d="M8.98633 4.49756C8.15833 4.49756 7.48633 5.16956 7.48633 5.99756C7.48633 6.82556 8.15833 7.49756 8.98633 7.49756H26.9863C27.8143 7.49756 28.4863 6.82556 28.4863 5.99756C28.4863 5.16956 27.8143 4.49756 26.9863 4.49756H8.98633ZM7.48633 10.4976C6.65833 10.4976 5.98633 11.1696 5.98633 11.9976C5.98633 12.8256 6.65833 13.4976 7.48633 13.4976H28.4863C29.3143 13.4976 29.9863 12.8256 29.9863 11.9976C29.9863 11.1696 29.3143 10.4976 28.4863 10.4976H7.48633ZM8.98633 16.4976C6.50083 16.4976 4.48633 18.5121 4.48633 20.9976V26.9976C4.48633 29.4831 6.50083 31.4976 8.98633 31.4976H26.9863C29.4718 31.4976 31.4863 29.4831 31.4863 26.9976V20.9976C31.4863 18.5121 29.4718 16.4976 26.9863 16.4976H8.98633Z"/>

    <defs>
        <linearGradient id="paint0_linear_55_3401" x1="17.9648" y1="7.49609" x2="17.9648" y2="28.4961" gradientUnits="userSpaceOnUse">
            <stop stop-color="white"/>
            <stop offset="1" stop-color="#9A9A9A"/>
        </linearGradient>
    </defs>
</svg>
                    <span class="text">Ставок нет</span>
                  </div>
                {:else}
                  {#each playersSorting(getHistoryItemPlayers[colorKey] || []) as player, index}
                    <div class="player full-width row-block align-center justify-between">
                      <AccountComponent
                          displayedComponents={["accountId", "serverName", "you"]}
                        login={player.login}
                        accountId={player.accountId}
                        serverName={player.serverName}
                        serverId={player.serverId}
                          currentUserId={getAccountId}

                      />
                      <div class="amount full-height column-block align-end justify-center">
                        <div class="bet-amount row-block align-center justify-end">
                          <span class="text">{formatNumber(player.amount || 0)}</span>
                          <img class="coins" src="{cdn}/img/donate/mcoin.svg" alt="" />
                        </div>
                        
                      </div>
                    </div>
                  {/each}
                {/if}
              </div>
            {/each}
          </div>
        </div>
      {/if}
    </div>
  {/if}
</div>