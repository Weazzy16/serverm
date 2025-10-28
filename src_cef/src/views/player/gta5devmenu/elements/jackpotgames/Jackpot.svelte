<script>
  import { onMount, onDestroy, tick } from 'svelte';
  import { createEventDispatcher } from 'svelte';
  import AccountComponent from './AccountComponent.svelte';

  const dispatch = createEventDispatcher();
  
  export let data;
  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  export let characterId = null;
  export let accountId = null;
  export let serverTimeOffset = 0;

  let placeBetAmount = "0";
  let placeBetFocus = false;
  let showWinner = false;
  let showArrow = false;
  let winItem = null;
  let winItemHistory = null;
  let playerItemsGame = null;
  let playerItemsHistory = null;
  let isLoading = true;
  let timerInterval = null;
  let passedCards = null;
  let tickSound = null;
  let animStopped = false;
  let remainingScrolling = 0;
  let winnerScrollPosition = 0;
  let lastGameID = null;
  let gameStarted = false;
  let isAnimating = false;
  let animationFrameId = null;

  let playerBetWrapper = null;
  let playerBetWrapperHistory = null;
  let betsRef = null;
  let betsHistoryRef = null;

  // history animation flag — prevents rebuilding while viewing history
  let isHistoryAnimating = false;

  const maxPlayers = 50;
  const colors = ["#D33FB3", "#3F4ED3", "#D14B00", "#E81C5A", "#00B55E", "#F3B73B", "#CECECE", "#59B9FF", "#144E00", "#CCF600", "#A28168", "#8EA0A7", "#008A00", "#A359FF", "#00FFFF", "#FFFFFF", "#5F3910", "#1B1BB3", "#9F1D44", "#530FAD", "#834E4F", "#A600A6", "#3A475B", "#FFFF9B", "#A31010", "#9C5A40", "#7FCC5B", "#FFAC40", "#7B7F9B", "#8C7C4D", "#FFAB99", "#0066FF", "#FFE600", "#670540", "#33FF00", "#6100FF", "#93000B", "#FFC5B8", "#0E8585", "#756551", "#5F0812", "#4F4FD9", "#3A3939", "#FC0000", "#81720D", "#C62471", "#87B6FF", "#FF93B4", "#CBFFC2", "#FC8252"];

  const easing = {
    easeOutCubic: function(t) {
      return --t * t * t + 1;
    },
    easeInOutCubic: function(t) {
      return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
    }
  };

  $: getAccountId = accountId !== null ? +accountId : null;
  $: getActiveTimer = data?.timer?.active ?? false;
  $: getInitTimeTimer = data?.timer?.initTime ?? 0;
  $: getTimeStartTimer = data?.timer?.timeStart ?? "";
  $: getActiveGame = data?.game?.active ?? false;
  $: getActiveWindow = data?.active ?? false;
  $: getOpenedPage = data?.pages?.[data?.openedPage ?? 0] ?? 'main';
  $: getOpenedPageIndex = data?.openedPage ?? 0;
  $: getCurrentGame = data?.currentGame ?? { id: null, bets: [] };
  $: getCurrentGameBets = getCurrentGame?.bets ?? [];
  $: getCurrentGameID = getCurrentGame?.id;
  $: getWinner = data?.game?.winner;
  $: getWinnerIsVisible = !!(getWinner && showWinner && !getActiveTimer && !getActiveGame);
  $: getHistoryList = data?.historyList ?? [];
  $: getHistoryItem = data?.historyItem;
  $: getHistoryItemData = getHistoryItem?.game;
  $: getHistoryItemBets = getHistoryItem?.bets ?? [];
  $: getLuck = data?.luck;
  $: getJackpot = data?.jackpot;
  $: getLastGame = data?.lastGame;
  $: getGameTime = data?.game?.timeGame ?? 0;
  $: getStartGameTime = data?.game?.timeStart ?? "";
  $: getWinnerPosition = data?.game?.winnerPosition ?? 0;

  $: getCurrentGameBetsSorted = getCurrentGameBets
    .slice()
    .sort((a, b) => (b.amount || 0) - (a.amount || 0));

  $: getHistoryItemBetsSorted = getHistoryItemBets
    .slice()
    .sort((a, b) => (b.amount || 0) - (a.amount || 0));

  $: getTotalBetsSum = getCurrentGameBets.reduce((sum, bet) => sum + (bet.amount || 0), 0);

  $: playerColorMap = (() => {
    const map = new Map();
    const sortedBets = getCurrentGameBets
      .slice()
      .sort((a, b) => (b.amount || 0) - (a.amount || 0));
    
    sortedBets.forEach((bet, index) => {
      const key = `${bet.accountId}-${bet.serverId}`;
      map.set(key, index);
    });
    
    return map;
  })();

  $: playerColorMapHistory = (() => {
    if (!getHistoryItemBets || getHistoryItemBets.length === 0) {
      return new Map();
    }
    
    const map = new Map();
    const sortedBets = getHistoryItemBets
      .slice()
      .sort((a, b) => (b.amount || 0) - (a.amount || 0));
    
    sortedBets.forEach((bet, index) => {
      const key = `${bet.accountId}-${bet.serverId}`;
      map.set(key, index);
    });
    
    return map;
  })();

  $: getStartTime = (() => {
    let totalSeconds = Math.floor(getInitTimeTimer / 1000);
    let minutes = Math.floor(totalSeconds / 60);
    let seconds = totalSeconds % 60;
    
    let minutesStr = String(minutes).padStart(2, '0');
    let secondsStr = String(seconds).padStart(2, '0');
    
    return {
      minutes: [minutesStr[0], minutesStr[1]],
      seconds: [secondsStr[0], secondsStr[1]]
    };
  })();

  $: getGameDuration = (() => {
    if (getOpenedPage === 'main') {
      let elapsed = (getServerTimeWithOffset() - new Date(getStartGameTime) - getGameTime) / 1000 * -1;
      if (elapsed <= 0) elapsed = 0;
      if (elapsed >= getGameTime / 1000) elapsed = getGameTime / 1000;
      return elapsed;
    }
    return null;
  })();

  $: getPercentageOfTimeRemainingGame = (() => {
    let percentage = getGameDuration / (getGameTime / 1000) * 100;
    if (percentage <= 0) percentage = 0;
    if (percentage >= 100) percentage = 100;
    return percentage;
  })();

  function getPlayerColor(accountId, serverId = 1) {
    const key = `${accountId}-${serverId}`;
    const colorIndex = playerColorMap.get(key) ?? 0;
    return colors[colorIndex % colors.length];
  }

  function getPlayerColorHistory(accountId, serverId = 1) {
    const key = `${accountId}-${serverId}`;
    const colorIndex = playerColorMapHistory.get(key) ?? 0;
    return colors[colorIndex % colors.length];
  }

  function getServerTimeWithOffset() {
    return new Date(Date.now() + serverTimeOffset);
  }

  function updateTimer() {
    if (!getActiveTimer) return;
    
    try {
      const now = Date.now();
      const elapsed = now - new Date(getTimeStartTimer).getTime();
      const remaining = Math.max(0, getInitTimeTimer - elapsed);
    } catch (e) {
      console.error('[Jackpot] Timer error:', e);
    }
  }

  function startTimerInterval() {
    if (timerInterval) clearInterval(timerInterval);
    timerInterval = setInterval(updateTimer, 100);
  }
  
  function stopTimerInterval() {
    if (timerInterval) {
      clearInterval(timerInterval);
      timerInterval = null;
    }
  }

  function getChance(totalSum, amount) {
    if (totalSum === 0) return "0.00";
    return ((amount / totalSum) * 100).toFixed(2);
  }

// -------------------------
  // makeBet: ensure local UI updates immediately
  // -------------------------
  async function makeBet() {
    const amount = Number(placeBetAmount);
    if (amount > 0) {
      // send bet to server
      dispatch('bet', { amount });
      placeBetAmount = "0";

      // Try to immediately refresh local UI so user sees their cards without needing another player
      try {
        // If bets are already present, createPlayerCards will use them; call it to be sure
        createPlayerCards();
        await tick();

        // If still nothing rendered, insert a lightweight local placeholder card (UI-only)
        const rendered = playerBetWrapper ? playerBetWrapper.querySelectorAll('[data-scrollid]').length : 0;
        if ((playerItemsGame?.length || 0) === 0 || rendered === 0) {
          const placeholder = {
            accountId: getAccountId || 0,
            login: 'YOU',
            serverId: 1,
            amount,
            scrollId: `local-${getAccountId || 'me'}-${Date.now()}`,
            cycle: 0,
            cardLocalIndex: 0,
            _isLocalStub: true
          };
          if (!Array.isArray(playerItemsGame)) playerItemsGame = [];
          playerItemsGame = [placeholder, ...playerItemsGame];
          await tick();
          console.log('[Jackpot] Inserted local placeholder card for immediate UX', placeholder);
        } else {
          console.log('[Jackpot] createPlayerCards produced DOM nodes after bet, no placeholder needed');
        }
      } catch (e) {
        console.warn('[Jackpot] makeBet local UI update failed', e);
      }
    }
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

  function openPage(pageNum, id = null) {
    console.log('[Jackpot] Opening page:', pageNum, id);
    dispatch('openPage', { page: pageNum, id });
  }

  // -------------------------
  // createPlayerCards: added logs and safe DOM-check after tick
  // -------------------------
  async function createPlayerCards() {
    console.log('[Jackpot] createPlayerCards() called — isAnimating=', isAnimating, 'getCurrentGameBets.length=', getCurrentGameBets?.length);
    if (isAnimating) return;
    const serverMap = data?.game?.winner?.cardsMap ?? null;
    const serverOriginalLength = data?.game?.winner?.originalCardsLength ?? null;
    const serverCardsPerCycle = data?.game?.winner?.cardsPerCycle ?? null;

    if (serverMap && Array.isArray(serverMap) && serverMap.length > 0) {
      const cycles = serverOriginalLength && serverCardsPerCycle ? Math.max(1, Math.floor(serverOriginalLength / serverCardsPerCycle)) : 1;
      playerItemsGame = buildOriginalCardsFromServerMap(serverMap, cycles);
      console.log('[Jackpot] Created originalCards from server cardsMap:', playerItemsGame.length);
      console.log('[Jackpot] First 20 cards:', playerItemsGame.slice(0,20).map(c => `${c.login}(${c.cardLocalIndex})`).join(', '));
    } else {
      playerItemsGame = [];
      
      if (!getCurrentGameBets || getCurrentGameBets.length === 0) {
        console.log('[Jackpot] No bets to create cards');
        return;
      }

      const sortedBets = getCurrentGameBets
        .slice()
        .sort((a, b) => (b.amount || 0) - (a.amount || 0));

      console.log('[Jackpot] Creating cards for bets:', sortedBets.map(b => `${b.login}: ${b.amount}`));

      let cardsPerCycle = 0;
      sortedBets.forEach(bet => {
        const chanceNumber = (getTotalBetsSum > 0) ? (bet.amount / getTotalBetsSum) * 100 : 0;
        const cardCount = Math.max(1, Math.round(chanceNumber / 10));
        cardsPerCycle += cardCount;
      });

      const minTotalCards = 180;
      const cycles = Math.max(1, Math.ceil(minTotalCards / (cardsPerCycle || 1)));

      console.log('[Jackpot] Cards per cycle:', cardsPerCycle, 'Cycles:', cycles);

      for (let cycle = 0; cycle < cycles; cycle++) {
        sortedBets.forEach((bet, betIndex) => {
          const chanceNumber = (getTotalBetsSum > 0) ? (bet.amount / getTotalBetsSum) * 100 : 0;
          const cardCount = Math.max(1, Math.round(chanceNumber / 10));
          
          for (let i = 0; i < cardCount; i++) {
            playerItemsGame.push({
              ...bet,
              scrollId: `${cycle}-${betIndex}-${i}-${bet.accountId}`,
              cycle: cycle,
              cardLocalIndex: i
            });
          }
        });
      }

      console.log('[Jackpot] Total cards created:', playerItemsGame.length);
      console.log('[Jackpot] First 20 cards:', playerItemsGame.slice(0, 20).map(c => `${c.login}(${c.cardLocalIndex})`).join(', '));
    }

    // after building array, wait render and log DOM result — helps ensure template produced nodes
    await tick();
    try {
      const rendered = playerBetWrapper ? playerBetWrapper.querySelectorAll('[data-scrollid]').length : 0;
      console.log('[Jackpot] createPlayerCards: DOM elements with data-scrollid found =', rendered);
      // if none rendered but array not empty, it likely means template missing data-scrollid or key issue
      if ((playerItemsGame?.length || 0) > 0 && rendered === 0) {
        console.warn('[Jackpot] createPlayerCards: array non-empty but no DOM nodes found! Check template each (item.scrollId) and data-scrollid usage.');
      }
    } catch (e) {
      console.warn('[Jackpot] createPlayerCards DOM check failed', e);
    }
  }

  function buildOriginalCardsFromServerMap(cardsMap, cycles = 1) {
    const arr = [];
    for (let cycle = 0; cycle < cycles; cycle++) {
      for (let i = 0; i < cardsMap.length; i++) {
        const item = cardsMap[i];
        const cnt = Number(item.count) || 1;
        for (let j = 0; j < cnt; j++) {
          arr.push({
            accountId: Number(item.accountId),
            login: item.login,
            serverId: item.serverId,
            amount: item.amount ?? 0,
            scrollId: `${cycle}-${i}-${j}-${item.accountId}`,
            cycle,
            cardLocalIndex: j
          });
        }
      }
    }
    return arr;
  }

  /* =========================
     HISTORY: create cards & show winner (no wheel)
     ========================= */

  function createHistoryPlayerCards() {
    // do not rebuild while history view is animating/showing
    if (isHistoryAnimating) return;

    if (!getHistoryItemBets || getHistoryItemBets.length === 0) {
      playerItemsHistory = [];
      return;
    }

    // Build a deterministic duplicated array with unique scrollIds for history.
    // We prefer enough total cards (~180) but keep uniqueness by including cycle and an index.
    const totalBank = getHistoryItemData?.bank || 0;

    // compute rough per-bet card counts (same logic as main)
    let totalCards = 0;
    const betCardCounts = getHistoryItemBets.map(bet => {
      const chance = getChance(totalBank, bet.amount);
      const cnt = Math.max(1, Math.round(parseFloat(chance) / 10));
      totalCards += cnt;
      return cnt;
    });

    if (totalCards === 0) {
      playerItemsHistory = [];
      return;
    }

    const cycles = Math.max(1, Math.ceil(180 / totalCards));
    const cards = [];

    for (let cycle = 0; cycle < cycles; cycle++) {
      for (let i = 0; i < getHistoryItemBets.length; i++) {
        const bet = getHistoryItemBets[i];
        const cardCount = betCardCounts[i] || 1;
        for (let k = 0; k < cardCount; k++) {
          cards.push({
            ...bet,
            // unique id for history: includes cycle and k index
            scrollId: `hist-${cycle}-${i}-${k}-${bet.accountId}`,
            cycle,
            cardLocalIndex: k
          });
        }
      }
    }

    // add a few unique extras for smoothness
    for (let e = 0; e < 4; e++) {
      if (cards[e]) {
        cards.push({ ...cards[e], scrollId: `${cards[e].scrollId}-extra${e}` });
      }
    }

    playerItemsHistory = cards;
    console.log('[Jackpot] Created history cards count:', playerItemsHistory.length);

    // Immediately position to winner WITHOUT animating the wheel
    // Use tick() then call startHistoryScrollToWinner to set static position
    tick().then(() => {
      startHistoryScrollToWinner();
    });
  }

  // Jump immediately to winner position in history (no spin / no ticks)
  async function startHistoryScrollToWinner() {
    if (!playerItemsHistory || playerItemsHistory.length === 0) return;
    if (!getHistoryItemData) return;

    isHistoryAnimating = true;

    const winnerAccountId = getHistoryItemData.winnerAccountId;
    const winnerServerId = getHistoryItemData.winnerServerId;
    const winnerCardIndex = getHistoryItemData.winnerCardIndex || 0;

    // find all occurrences of the winner in the duplicated history array
    const winnerCards = playerItemsHistory.filter(c => c.accountId === winnerAccountId && c.serverId === winnerServerId);

    if (!winnerCards || winnerCards.length === 0) {
      console.warn('[Jackpot][History] Winner cards not found in playerItemsHistory');
      isHistoryAnimating = false;
      return;
    }

    const chosenCard = winnerCards[Math.min(winnerCardIndex, winnerCards.length - 1)];
    let desiredIndex = playerItemsHistory.indexOf(chosenCard);
    if (desiredIndex === -1) {
      desiredIndex = playerItemsHistory.findIndex(c => c.accountId === winnerAccountId && c.serverId === winnerServerId);
      if (desiredIndex === -1) {
        console.warn('[Jackpot][History] Fallback search failed');
        isHistoryAnimating = false;
        return;
      }
    }

    // ensure DOM rendered
    await tick();

    const container = playerBetWrapperHistory;
    const viewRef = betsHistoryRef || container;
    if (!container) {
      console.warn('[Jackpot][History] History container not found');
      isHistoryAnimating = false;
      return;
    }

    function getElementOffsetLeftRelativeToContainer(el, containerEl) {
      let left = 0;
      let current = el;
      while (current && current !== containerEl) {
        left += current.offsetLeft || 0;
        current = current.offsetParent;
      }
      return left;
    }

    const targetEl = container.querySelector(`[data-scrollid="${playerItemsHistory[desiredIndex].scrollId}"]`);
    if (!targetEl) {
      console.warn('[Jackpot][History] Target element not found for scrollId', playerItemsHistory[desiredIndex].scrollId);
      isHistoryAnimating = false;
      return;
    }

    const absoluteLeft = getElementOffsetLeftRelativeToContainer(targetEl, container);
    const elWidth = targetEl.offsetWidth || 144;
    const centerOffset = (viewRef.clientWidth || container.clientWidth) / 2;
    const rawTarget = absoluteLeft - centerOffset + (elWidth / 2);
    const maxScroll = Math.max(0, container.scrollWidth - container.clientWidth);
    const exactFinalScroll = Math.max(0, Math.min(Math.round(rawTarget), Math.round(maxScroll)));

    // set instantly (no animation)
    try {
      container.scrollLeft = exactFinalScroll;
      void container.offsetHeight; // force reflow
    } catch (e) {
      console.warn('[Jackpot][History] Failed to set scrollLeft', e);
    }

    // wait a couple frames to ensure visual stable
    await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));

    // clear previous and set highlight
    try {
      const prev = container.querySelector('.final-winner');
      if (prev) prev.classList.remove('final-winner');
      targetEl.classList.add('final-winner');
    } catch (e) {
      console.warn('[Jackpot][History] Failed to toggle final-winner class', e);
    }

    winItemHistory = playerItemsHistory[desiredIndex];
    isHistoryAnimating = false;
  }

  /* =========================
     End history section
     ========================= */

  function scrollTo(startPos, endPos, duration, easingFunc, callback) {
    const startTime = Date.now();

    if (!playerBetWrapper) {
        console.error('[Jackpot] playerBetWrapper not found');
        if (callback) callback();
        return;
    }

    // Clamp endPos to available scroll range
    const maxScroll = Math.max(0, playerBetWrapper.scrollWidth - playerBetWrapper.clientWidth);
    const endPosClamped = Math.max(0, Math.min(endPos, maxScroll));

    playerBetWrapper.scrollLeft = startPos;

    if (startPos === endPosClamped) {
        // Прямо устанавливаем и вызываем callback
        playerBetWrapper.scrollLeft = endPosClamped;
        if (callback) callback();
        return;
    }

    const animate = () => {
        if (animStopped) {
            if (callback) callback();
            return;
        }

        const now = Date.now();
        const elapsed = Math.min(1, (now - startTime) / (duration * 1000));
        const eased = easingFunc(elapsed);
        const currentPos = eased * (endPosClamped - startPos) + startPos;

        if (playerBetWrapper) {
            playerBetWrapper.scrollLeft = currentPos;
        }

        const currentCard = Math.floor((currentPos - 72) / 144) + 1;
        if (passedCards !== currentCard && currentCard !== 0) {
            passedCards = currentCard;
            if (tickSound) {
                try {
                    tickSound.play();
                } catch (e) {}
            }
        }

        if (elapsed < 1 && !animStopped) {
            animationFrameId = requestAnimationFrame(animate);
        } else {
            // Финальная жёсткая установка для устранения дробной погрешности
            try {
                if (playerBetWrapper) {
                    playerBetWrapper.scrollLeft = endPosClamped;
                }
            } catch (e) { console.warn('[Jackpot] final scroll correction failed', e); }

            if (callback) callback();
        }
    };

    animationFrameId = requestAnimationFrame(animate);
  }

 async function startGame() {
  console.log('========== 🎯 NEW JACKPOT GAME START 🎯 ==========');

  isAnimating = true;
  destroyAnim();
  createPlayerCards();
  showArrow = true;

  setTimeout(async () => {
    try {
      if (!getWinner || !getWinner.accountId) {
        console.warn('[Jackpot] No winner data');
        isAnimating = false;
        return;
      }

      console.log('[Jackpot] Winner from server:', getWinner);

      const originalCards = Array.isArray(playerItemsGame) ? [...playerItemsGame] : [];
      console.log('[Jackpot] Using created cards:', originalCards.length);

      // Diagnostics: cardsPerCycle (same as server)
      const sortedBets = getCurrentGameBets.slice().sort((a, b) => (b.amount || 0) - (a.amount || 0));
      let cardsPerCycle = 0;
      sortedBets.forEach(bet => {
        const chanceNumber = (getTotalBetsSum > 0) ? (bet.amount / getTotalBetsSum) * 100 : 0;
        const cardCount = Math.max(1, Math.round(chanceNumber / 10));
        cardsPerCycle += cardCount;
      });
      const totalCycles = Math.floor(originalCards.length / Math.max(1, cardsPerCycle));
      const lastCycleStart = (totalCycles - 1) * cardsPerCycle;
      const lastCycleCards = originalCards.slice(lastCycleStart, lastCycleStart + cardsPerCycle);

      // winner pos
      const serverWinnerAbsoluteIndex = getWinner?.winnerAbsoluteIndex ?? null;
      const serverOriginalLength = getWinner?.originalCardsLength ?? null;
      let winnerPosInArray = null;
      if (serverWinnerAbsoluteIndex !== null && !isNaN(Number(serverWinnerAbsoluteIndex))) {
        winnerPosInArray = Number(serverWinnerAbsoluteIndex);
        if (serverOriginalLength && serverOriginalLength !== originalCards.length) {
          console.warn('[Jackpot] serverOriginalLength differs from client originalCards length', serverOriginalLength, originalCards.length);
        }
      } else {
        const winnerCardsInLastCycle = lastCycleCards.filter(c =>
          c.accountId === getWinner.accountId && c.serverId === getWinner.serverId
        );
        if (winnerCardsInLastCycle.length === 0) {
          console.error('[Jackpot] ERROR: No winner cards in last cycle!');
          isAnimating = false;
          return;
        }
        let cardIndexLocal = getWinner.winnerCardIndex || 0;
        if (cardIndexLocal >= winnerCardsInLastCycle.length) cardIndexLocal = winnerCardsInLastCycle.length - 1;
        const selectedCard = winnerCardsInLastCycle[cardIndexLocal];
        const posInLastCycle = lastCycleCards.indexOf(selectedCard);
        winnerPosInArray = lastCycleStart + posInLastCycle;
      }
      if (winnerPosInArray === null) {
        console.error('[Jackpot] Failed to determine winner absolute position');
        isAnimating = false;
        return;
      }

      // --- dynamic copies selection (robust) ---
      const defaultCardWidth = 144;
      let cardWidth = defaultCardWidth;
      try {
        const sampleEl = playerBetWrapper?.querySelector('[data-scrollid]');
        if (sampleEl && sampleEl.offsetWidth) cardWidth = sampleEl.offsetWidth;
      } catch (e) {}

      const visibleSlots = betsRef ? Math.ceil(betsRef.clientWidth / cardWidth) : Math.ceil(1000 / cardWidth);
      const screensBuffer = 3;
      const desiredBufferSlots = visibleSlots * screensBuffer;
      const originalLength = originalCards.length || 1;

      let minCopies = Math.max(3, 2 * Math.ceil(desiredBufferSlots / originalLength) + 1);
      minCopies = Math.max(minCopies, 5);
      const maxCopiesLimit = 60;
      const marginPx = Math.round((betsRef ? betsRef.clientWidth : 600) / 2);

      let chosenCopies = null, chosenTargetCopy = null, chosenFinalIndex = null;
      for (let c = minCopies; c <= maxCopiesLimit; c++) {
        const targetCandidate = Math.floor(c / 2);
        const totalSlots = c * originalLength;
        const finalIndexCandidate = (targetCandidate * originalLength) + winnerPosInArray;
        const rawTarget = (finalIndexCandidate * cardWidth) - (betsRef.clientWidth / 2) + (cardWidth / 2) + (cardWidth * (getWinnerPosition || 0));
        const maxScrollApprox = Math.max(0, totalSlots * cardWidth - (playerBetWrapper ? playerBetWrapper.clientWidth : 0));
        const endPosClampedApprox = Math.max(0, Math.min(Math.round(rawTarget), Math.round(maxScrollApprox)));
        if (endPosClampedApprox > marginPx && endPosClampedApprox < (maxScrollApprox - marginPx)) {
          chosenCopies = c; chosenTargetCopy = targetCandidate; chosenFinalIndex = finalIndexCandidate; break;
        }
        for (let offset = -1; offset <= 1; offset++) {
          const t = targetCandidate + offset;
          if (t < 0 || t >= c) continue;
          const finalIdx = (t * originalLength) + winnerPosInArray;
          const rawT = (finalIdx * cardWidth) - (betsRef.clientWidth / 2) + (cardWidth / 2) + (cardWidth * (getWinnerPosition || 0));
          const endClamped = Math.max(0, Math.min(Math.round(rawT), Math.round(maxScrollApprox)));
          if (endClamped > marginPx && endClamped < (maxScrollApprox - marginPx)) {
            chosenCopies = c; chosenTargetCopy = t; chosenFinalIndex = finalIdx; break;
          }
        }
        if (chosenCopies !== null) break;
      }
      if (chosenCopies === null) {
        chosenCopies = Math.min(maxCopiesLimit, Math.max(minCopies, 12));
        chosenTargetCopy = Math.floor(chosenCopies / 2);
        chosenFinalIndex = (chosenTargetCopy * originalLength) + winnerPosInArray;
      }
      console.log('[Jackpot] Copies selection:', { cardWidth, visibleSlots, desiredBufferSlots, originalLength, chosenCopies, chosenTargetCopy });

      // duplicate
      playerItemsGame = [];
      for (let copy = 0; copy < chosenCopies; copy++) {
        originalCards.forEach((card, idx) => {
          playerItemsGame.push({ ...card, scrollId: `${card.scrollId}-copy${copy}`, _origIndex: idx, _copyIndex: copy });
        });
      }
      const finalIndex = chosenFinalIndex;
      console.log('[Jackpot] Duplicated length:', playerItemsGame.length, 'finalIndex:', finalIndex, 'cardAtFinal:', playerItemsGame[finalIndex]?.login, playerItemsGame[finalIndex]?.scrollId);

      // wait render
      await tick();

      // helper to get accumulated offsetLeft
      function getElementOffsetLeftRelativeToContainer(el, container) {
        let left = 0;
        let current = el;
        while (current && current !== container) {
          left += current.offsetLeft || 0;
          current = current.offsetParent;
        }
        return left;
      }

      // remove previous highlight (do not add early)
      try { const prev = playerBetWrapper && playerBetWrapper.querySelector('.final-winner'); if (prev) prev.classList.remove('final-winner'); } catch (e) {}

      // compute exactFinalScroll right before animating
      const desiredScrollId = playerItemsGame[finalIndex]?.scrollId;
      let desiredScrollIndex = finalIndex;
      let exactFinalScroll = 0;
      if (desiredScrollId && playerBetWrapper && betsRef) {
        const targetEl = playerBetWrapper.querySelector(`[data-scrollid="${desiredScrollId}"]`);
        if (targetEl) {
          const absoluteElLeft = getElementOffsetLeftRelativeToContainer(targetEl, playerBetWrapper);
          const elWidth = targetEl.offsetWidth || cardWidth;
          const centerOffset = betsRef.clientWidth / 2;
          const rawTarget = absoluteElLeft - centerOffset + (elWidth / 2) + (elWidth * (getWinnerPosition || 0));
          const maxScroll = Math.max(0, playerBetWrapper.scrollWidth - playerBetWrapper.clientWidth);
          exactFinalScroll = Math.max(0, Math.min(Math.round(rawTarget), Math.round(maxScroll)));
          const foundIndex = playerItemsGame.findIndex(x => x.scrollId === desiredScrollId);
          if (foundIndex !== -1) desiredScrollIndex = foundIndex;
        } else {
          const cw = cardWidth;
          const centerOffset = betsRef.clientWidth / 2;
          const rawTarget = (finalIndex * cw) - centerOffset + (cw / 2) + (cw * (getWinnerPosition || 0));
          const maxScroll = Math.max(0, playerBetWrapper.scrollWidth - playerBetWrapper.clientWidth);
          exactFinalScroll = Math.max(0, Math.min(Math.round(rawTarget), Math.round(maxScroll)));
        }
      } else {
        const cw = cardWidth;
        const centerOffset = (betsRef ? betsRef.clientWidth : (playerBetWrapper ? playerBetWrapper.clientWidth : 600)) / 2;
        const rawTarget = (finalIndex * cw) - centerOffset + (cw / 2) + (cw * (getWinnerPosition || 0));
        const maxScroll = playerBetWrapper ? Math.max(0, playerBetWrapper.scrollWidth - playerBetWrapper.clientWidth) : 0;
        exactFinalScroll = Math.max(0, Math.min(Math.round(rawTarget), Math.round(maxScroll)));
      }
      remainingScrolling = exactFinalScroll;
      console.log('[Jackpot] exactFinalScroll:', exactFinalScroll, 'desiredIndex:', desiredScrollIndex, 'finalIndex:', finalIndex);

      // temporarily disable pointer events to avoid interference
      try { if (playerBetWrapper) playerBetWrapper.style.pointerEvents = 'none'; } catch (e) {}

      // main animation
      animStopped = false;
      const duration = (getGameTime / 1000) || 8;
      scrollTo(0, exactFinalScroll, duration, easing.easeInOutCubic, () => {
        (async () => {
          // wait couple frames
          await new Promise(r => requestAnimationFrame(() => requestAnimationFrame(r)));

          if (!playerBetWrapper || !betsRef) {
            try { if (playerBetWrapper) playerBetWrapper.style.pointerEvents = ''; } catch (e) {}
            finishAnim();
            return;
          }

          // REFERENCE: center coordinates for elementFromPoint
          const centerRect = betsRef.getBoundingClientRect();
          const centerX = centerRect.left + centerRect.width / 2;
          const centerY = centerRect.top + centerRect.height / 2;

          // element under arrow
          let nodeUnder = document.elementFromPoint(centerX, centerY);
          // climb up to find data-scrollid
          while (nodeUnder && !nodeUnder.dataset?.scrollid) nodeUnder = nodeUnder.parentElement;

          const idUnder = nodeUnder ? nodeUnder.dataset.scrollid : null;
          const expectedId = playerItemsGame[desiredScrollIndex]?.scrollId;

          if (idUnder !== expectedId) {
            console.warn('[Jackpot] Verification mismatch: element under arrow != expected. idUnder=', idUnder, 'expected=', expectedId);
            // try to find the expected element and force snap (one attempt)
            const expectedEl = playerBetWrapper.querySelector(`[data-scrollid="${expectedId}"]`);
            if (expectedEl) {
              const absoluteElLeft3 = getElementOffsetLeftRelativeToContainer(expectedEl, playerBetWrapper);
              const elWidth3 = expectedEl.offsetWidth || cardWidth;
              const centerOffset3 = betsRef.clientWidth / 2;
              const rawTarget3 = absoluteElLeft3 - centerOffset3 + (elWidth3 / 2) + (elWidth3 * (getWinnerPosition || 0));
              const maxScroll3 = Math.max(0, playerBetWrapper.scrollWidth - playerBetWrapper.clientWidth);
              const finalSnap = Math.max(0, Math.min(Math.round(rawTarget3), Math.round(maxScroll3)));

              // short correction animation (fast)
              const snapDuration = 150;
              const start = playerBetWrapper.scrollLeft;
              const diff = finalSnap - start;
              const startTime = performance.now();
              await new Promise(resolveSnap => {
                const snapAnimate = (now) => {
                  const tt = Math.min(1, (now - startTime) / snapDuration);
                  const ttt = tt - 1;
                  const eased = ttt * ttt * ttt + 1;
                  playerBetWrapper.scrollLeft = start + diff * eased;
                  if (Math.abs(playerBetWrapper.scrollLeft - finalSnap) > 0.5 && performance.now() - startTime < snapDuration + 50) {
                    requestAnimationFrame(snapAnimate);
                  } else {
                    playerBetWrapper.scrollLeft = finalSnap;
                    resolveSnap();
                  }
                };
                requestAnimationFrame(snapAnimate);
              });

              // small pause and re-evaluate under-arrow
              await new Promise(r => requestAnimationFrame(r));
              nodeUnder = document.elementFromPoint(centerX, centerY);
              while (nodeUnder && !nodeUnder.dataset?.scrollid) nodeUnder = nodeUnder.parentElement;
              const idUnder2 = nodeUnder ? nodeUnder.dataset.scrollid : null;
              console.log('[Jackpot] After force-snap idUnder2=', idUnder2, 'expected=', expectedId);
            }
          }

          // final small correction already done; add final-winner class
          try {
            const elFinal = playerBetWrapper.querySelector(`[data-scrollid="${playerItemsGame[desiredScrollIndex]?.scrollId}"]`);
            if (elFinal) elFinal.classList.add('final-winner');
          } catch (e) {}

          // restore pointer events
          try { if (playerBetWrapper) playerBetWrapper.style.pointerEvents = ''; } catch (e) {}

          if (playerItemsGame && playerItemsGame[desiredScrollIndex]) {
            winItem = playerItemsGame[desiredScrollIndex];
            console.log('[Jackpot] Animation finished, final winItem:', winItem.login, winItem.scrollId);
          }

          finishAnim();
        })();
      });
    } catch (err) {
      console.error('[Jackpot] startGame Exception:', err);
      isAnimating = false;
      try { finishAnim(); } catch (e) {}
    }
  }, 100);
}

  function finishAnim() {
    isAnimating = false;
    showWinner = true;
    if (getOpenedPage === 'historyList') {
      dispatch('getJackpotHistory');
    }
  }

  function destroyAnim() {
    animStopped = true;
    if (animationFrameId) {
      cancelAnimationFrame(animationFrameId);
      animationFrameId = null;
    }
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

  async function onPageLoaded() {
  if (getActiveGame || getWinner) {
    if (!gameStarted) {
      gameStarted = true;
      startGame();
    }
  } else if (getCurrentGameBets.length > 0) {
    await tick();
    createPlayerCards();
  }

  // Robust tickSound initialization with format probing and fallbacks.
  // Tries to pick a source that exists on the server and that the runtime can likely play.
  try {
    const candidateSources = [
      { url: `${cdn}sounds/donate/tick.ogg`, mime: 'audio/ogg; codecs="vorbis"' },
      { url: `${cdn}sounds/donate/tick.mp3`, mime: 'audio/mpeg' },
      { url: `${cdn}sounds/donate/tick.wav`, mime: 'audio/wav' }
    ];

    // Utility: check if fetch of small range or HEAD succeeds
    async function probeUrl(url) {
      try {
        // Try HEAD first (cheap). Some CDNs might not allow HEAD; fall back to ranged GET.
        const headResp = await fetch(url, { method: 'HEAD', cache: 'no-cache' });
        if (headResp && headResp.ok) return true;
      } catch (e) {
        // ignore and try ranged GET
      }

      try {
        const rangeResp = await fetch(url, {
          method: 'GET',
          headers: { Range: 'bytes=0-1023' },
          cache: 'no-cache'
        });
        if (rangeResp && (rangeResp.status === 206 || rangeResp.status === 200)) return true;
      } catch (e) {
        // not available or CORS blocked
      }

      return false;
    }

    // Utility: test canPlayType for a mime
    function canPlay(mime) {
      try {
        const a = document.createElement('audio');
        if (!a || typeof a.canPlayType !== 'function') return false;
        const res = a.canPlayType(mime);
        return res === 'probably' || res === 'maybe';
      } catch (e) {
        return false;
      }
    }

    // Find all sources that both exist (probe) and/or canPlayType indicates support.
    const availableSources = [];
    for (const s of candidateSources) {
      let exists = false;
      try {
        exists = await probeUrl(s.url);
      } catch (e) {
        exists = false;
      }

      const playable = canPlay(s.mime);
      console.log('[Jackpot] tick probe:', s.url, 'exists=', exists, 'canPlay=', playable);

      // Prefer sources that both exist and are playable. If nothing both, allow exists-only or playable-only.
      if (exists && playable) {
        availableSources.push(s.url);
      } else if (exists && availableSources.length === 0) {
        // keep as fallback if no better found yet
        availableSources.push(s.url);
      } else if (!exists && playable && availableSources.length === 0) {
        // playable but server may block HEAD—still try it if nothing else
        availableSources.push(s.url);
      }
    }

    if (!availableSources || availableSources.length === 0) {
      console.warn('[Jackpot] No playable tick sound source found (probe failed or codecs missing). Tick sound disabled.');
      tickSound = null;
      return;
    }

    // Try to initialize Howler with the found sources (if Howl available)
    if (typeof Howl !== 'undefined') {
      tickSound = new Howl({
        src: availableSources,
        volume: 0.12,
        loop: false,
        html5: true,
        preload: true,
        onloaderror: (id, err) => {
          console.warn('[Jackpot] tickSound load error', id, err);
        },
        onplayerror: (id, err) => {
          console.warn('[Jackpot] tickSound play error', id, err);
        }
      });

      // One-time unlock on first user gesture to satisfy autoplay policies
      const unlockAudio = () => {
        try {
          const playId = tickSound.play();
          if (typeof playId !== 'undefined') {
            tickSound.stop(playId);
          }
        } catch (e) {
          console.warn('[Jackpot] unlockAudio error (Howl)', e);
        } finally {
          document.removeEventListener('pointerdown', unlockAudio);
          document.removeEventListener('keydown', unlockAudio);
        }
      };
      document.addEventListener('pointerdown', unlockAudio, { once: true });
      document.addEventListener('keydown', unlockAudio, { once: true });

      console.log('[Jackpot] tickSound created (Howl) with sources:', availableSources);
      return;
    }

    // If Howler not available, create HTMLAudio fallback using first available source
    try {
      const audio = new Audio(availableSources[0]);
      audio.preload = 'auto';
      audio.volume = 0.12;

      // Try to load and listen for errors
      audio.addEventListener('error', (ev) => {
        console.warn('[Jackpot] HTMLAudio error', ev, audio.error);
      }, { once: true });

      tickSound = {
        play: () => {
          try {
            const p = audio.play();
            if (p && typeof p.then === 'function') {
              p.catch(err => console.warn('[Jackpot] HTMLAudio play failed', err));
            }
            return p;
          } catch (err) {
            console.warn('[Jackpot] HTMLAudio play exception', err);
            return null;
          }
        },
        stop: () => {
          try { audio.pause(); audio.currentTime = 0; } catch (e) {}
        }
      };

      // Unlock on first gesture
      const unlockAudioHtml = () => {
        try {
          const p = audio.play();
          if (p && typeof p.then === 'function') {
            p.then(() => audio.pause()).catch(() => {});
          } else {
            audio.pause();
          }
        } catch (e) {
          console.warn('[Jackpot] unlockAudio error (HTMLAudio)', e);
        } finally {
          document.removeEventListener('pointerdown', unlockAudioHtml);
          document.removeEventListener('keydown', unlockAudioHtml);
        }
      };
      document.addEventListener('pointerdown', unlockAudioHtml, { once: true });
      document.addEventListener('keydown', unlockAudioHtml, { once: true });

      console.log('[Jackpot] tickSound created (HTMLAudio) with source:', availableSources[0]);
    } catch (e) {
      console.warn('[Jackpot] Failed to create HTMLAudio fallback for tickSound', e);
      tickSound = null;
    }
  } catch (e) {
    console.warn('[Jackpot] tickSound initialization error', e);
    tickSound = null;
  }
}

  $: if (getActiveTimer) {
    startTimerInterval();
    updateTimer();
  } else {
    stopTimerInterval();
  }

  $: if (getActiveGame && getWinner && getWinner.accountId && !gameStarted) {
    gameStarted = true;
    startGame();
  }

  $: if (!getActiveGame && gameStarted) {
    gameStarted = false;
  }

  $: if (getCurrentGameID && getCurrentGameID !== lastGameID) {
    if (lastGameID !== null) {
      playerItemsGame = null;
      showWinner = false;
      showArrow = false;
      winItem = null;
      gameStarted = false;
    }
    lastGameID = getCurrentGameID;
  }

  $: if (getCurrentGameBets && getCurrentGameBets.length > 0) {
    if (!isAnimating) {
      // schedule createPlayerCards after microtask so other state updates settle
      tick().then(() => {
        try {
          createPlayerCards();
        } catch (e) {
          console.warn('[Jackpot] createPlayerCards() scheduled call failed', e);
        }
      });
    }
  } else {
    if (!isAnimating && !getActiveGame) {
      showArrow = false;
      playerItemsGame = null;
    }
  }


  $: if (getHistoryItem && getHistoryItemData && getHistoryItemBets.length > 0) {
    // build history cards and then immediately snap to winner (no wheel)
    createHistoryPlayerCards();
    // createHistoryPlayerCards triggers tick().then(startHistoryScrollToWinner());
  }

  // Keep existing lifecycle & other reactive blocks...
  $: if (!isLoading) {
    onPageLoaded();
  }

  onMount(() => {
    isLoading = false;
  });

  onDestroy(() => {
    stopTimerInterval();
    destroyAnim();
  });
</script>
<style>
/* Добавьте стиль для финальной подсветки (если нужно) */
.final-winner {
  box-shadow: 0 0 0 3px rgba(255, 192, 203, 0.25) inset;
  /* или любой другой визуальный эффект, который вы используете для подсветки победителя */
}
</style>

<!-- Игровая разметка: ключи и data-scrollid добавлены -->
<div class="jackpot full-width full-height row-block align-start justify-between">
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
      {#if getOpenedPageIndex === 0 || getOpenedPage === 'main'}
        <div class="main full-width full-height column-block align-start justify-between">
          <!-- ГЛАВНАЯ СТРАНИЦА -->
          <!-- Верхняя панель -->
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
              
              {#if getCurrentGameID}
                <div class="gameID"><span>#{getCurrentGameID}</span></div>
              {/if}
            </div>

            <div class="row-block align-center justify-start">
              <button class="history row-block align-center justify-center" on:click={() => openPage(1)}>
                <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
	<path d="M9 18.5C4.03725 18.5 0 14.4628 0 9.5C0 4.53725 4.03725 0.5 9 0.5C9.414 0.5 9.75 0.836 9.75 1.25C9.75 1.664 9.414 2 9 2C4.8645 2 1.5 5.3645 1.5 9.5C1.5 13.6355 4.8645 17 9 17C13.1355 17 16.5 13.6355 16.5 9.5C16.5 9.086 16.8353 8.75 17.25 8.75C17.6647 8.75 18 9.086 18 9.5C18 14.4628 13.9628 18.5 9 18.5ZM9 2C8.805 2 8.61 1.9175 8.4675 1.7825C8.3325 1.64 8.25 1.445 8.25 1.25C8.25 1.055 8.3325 0.86 8.4675 0.7175C8.7525 0.44 9.255 0.44 9.5325 0.7175C9.6675 0.86 9.75 1.055 9.75 1.25C9.75 1.445 9.6675 1.64 9.5325 1.7825C9.39 1.9175 9.195 2 9 2ZM15.93 6.6275C15.7725 6.245 15.9518 5.8025 16.335 5.645C16.7175 5.4875 17.16 5.6675 17.3175 6.05C17.475 6.4325 17.295 6.875 16.9125 7.0325C16.815 7.07 16.7175 7.0925 16.6275 7.0925C16.3275 7.0925 16.0492 6.92 15.93 6.6275ZM14.3018 4.1975C14.0093 3.905 14.0093 3.425 14.3018 3.1325C14.595 2.84 15.0675 2.84 15.3593 3.1325C15.6525 3.425 15.6525 3.905 15.3593 4.1975C15.2175 4.34 15.0225 4.415 14.835 4.415C14.64 4.415 14.4525 4.34 14.3018 4.1975ZM11.8725 2.57C11.4893 2.4125 11.31 1.97 11.4675 1.5875C11.625 1.205 12.06 1.025 12.45 1.1825C12.8325 1.34 13.0125 1.7825 12.8475 2.165C12.735 2.45 12.45 2.6225 12.1575 2.6225C12.06 2.6225 11.9625 2.6075 11.8725 2.57ZM17.25 10.25C17.055 10.25 16.8593 10.1675 16.7175 10.0325C16.5818 9.89 16.5 9.695 16.5 9.5C16.5 9.305 16.5818 9.11 16.7175 8.9675C17.0025 8.69 17.505 8.69 17.7825 8.9675C17.9175 9.11 18 9.305 18 9.5C18 9.695 17.9175 9.89 17.7825 10.0325C17.64 10.1675 17.445 10.25 17.25 10.25ZM11.9992 11.75C11.8867 11.75 11.772 11.7245 11.6648 11.6713L8.66475 10.1713C8.4105 10.0438 8.25 9.78425 8.25 9.5V4.25C8.25 3.836 8.586 3.5 9 3.5C9.414 3.5 9.75 3.836 9.75 4.25V9.0365L12.3353 10.3295C12.7058 10.5147 12.8557 10.9655 12.6705 11.336C12.5392 11.5985 12.2745 11.75 11.9992 11.75Z"/>
</svg>
                <span>История</span>
              </button>

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
                  <span class="count">{getCurrentGameBets.length}</span>
                   
                  <span class="text">Игроки</span>
                 
                </div>
              </div>

              <div class="bank row-block align-center justify-start">
                <div class="bank-info column-block align-start justify-between">
                  <span class="count row-block align-center justify-start">
                    {formatNumber(getTotalBetsSum)}
                    <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                  </span>
                  <span class="text">Банк</span>
                </div>
              </div>
            </div>
          </div>
          <!-- Верхняя панель и прочее — без изменений -->
          <div class="gameWrapper row-block align-center justify-center full-width">
            <div class="game column-block align-start justify-between">
              <div class="gameInfo row-block align-center justify-between full-width">
                <div class="bank row-block align-center justify-start">
                  Банк {formatNumber(getTotalBetsSum)}
                  <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                </div>
                <div class="players row-block align-center justify-start">
                  <div class="back full-height" style="width: {Math.min(100, (getCurrentGameBets.length / maxPlayers) * 100)}%"></div>
                  <span>{getCurrentGameBets.length}/{maxPlayers} игроков</span>
                </div>
              </div>

              <div bind:this={betsRef} class="bets row-block align-center justify-start full-width" class:empty={getCurrentGameBets.length <= 0}>
                {#if getCurrentGameBets.length <= 0}
                <div class="emptyBets column-block align-center justify-center">
                    <span class="text">Ставок нет</span>
                    <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 36 36">
    <path d="M8.98633 4.49756C8.15833 4.49756 7.48633 5.16956 7.48633 5.99756C7.48633 6.82556 8.15833 7.49756 8.98633 7.49756H26.9863C27.8143 7.49756 28.4863 6.82556 28.4863 5.99756C28.4863 5.16956 27.8143 4.49756 26.9863 4.49756H8.98633ZM7.48633 10.4976C6.65833 10.4976 5.98633 11.1696 5.98633 11.9976C5.98633 12.8256 6.65833 13.4976 7.48633 13.4976H28.4863C29.3143 13.4976 29.9863 12.8256 29.9863 11.9976C29.9863 11.1696 29.3143 10.4976 28.4863 10.4976H7.48633ZM8.98633 16.4976C6.50083 16.4976 4.48633 18.5121 4.48633 20.9976V26.9976C4.48633 29.4831 6.50083 31.4976 8.98633 31.4976H26.9863C29.4718 31.4976 31.4863 29.4831 31.4863 26.9976V20.9976C31.4863 18.5121 29.4718 16.4976 26.9863 16.4976H8.98633Z"/>

    <defs>
        <linearGradient id="paint0_linear_55_3401" x1="17.9648" y1="7.49609" x2="17.9648" y2="28.4961" gradientUnits="userSpaceOnUse">
            <stop stop-color="white"/>
            <stop offset="1" stop-color="#9A9A9A"/>
        </linearGradient>
    </defs>
</svg>
                  </div>
                {/if}

                {#if showArrow && getCurrentGameBets.length > 0}
                  <div class="winArrowWrapper row-block align-center justify-center full-width">
                    <svg class="winArrow" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 26 23">
                      <path d="M15.6053 21.5161C14.4474 23.4946 11.5526 23.4946 10.3947 21.5161L0.407536 4.45161C-0.750391 2.47312 0.697019 3.04708e-07 3.01287 1.0225e-07L22.9871 -1.64396e-06C25.303 -1.84641e-06 26.7504 2.47312 25.5925 4.45161L15.6053 21.5161Z" fill="url(#paint0_linear_55_3401)"/>
                      <defs>
                        <linearGradient id="paint0_linear_55_3401" x1="13" y1="25.9677" x2="13" y2="-8.65591" gradientUnits="userSpaceOnUse">
                          <stop stop-color="white"/>
                          <stop offset="1" stop-color="#999999"/>
                        </linearGradient>
                      </defs>
                    </svg>
                  </div>
                {/if}

                {#if playerItemsGame && playerItemsGame.length > 0}
                  <div bind:this={playerBetWrapper} class="playerBetWrapper scrollDisabled row-block align-center justify-start full-width">
                    {#each playerItemsGame as item (item.scrollId)}
                      <div class="playerBet column-block align-center justify-start" 
                           data-scrollid={item.scrollId}
                           class:lose={!!(getWinner && getWinnerIsVisible && winItem) && winItem.scrollId !== item.scrollId}
                           style="border-color: {getPlayerColor(item.accountId, item.serverId)}">
                        <AccountComponent
                          displayedComponents={["avatar", "you"]}
                          login={item.login}
                          accountId={item.accountId}
                          currentUserId={getAccountId}
                        />
                        <span class="amount row-block align-center justify-start">
                          {formatNumber(item.amount)}
                          <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                        </span>
                        <span class="chance">{getChance(getTotalBetsSum, item.amount)}%</span>
                        <div class="color" style="background: {getPlayerColor(item.accountId, item.serverId)}"></div>
                      </div>
                    {/each}
                  </div>
                {/if}
              </div>
              <div class="colors row-block align-center justify-start full-width">
                {#each getCurrentGameBetsSorted as bet}
                  <div class="colorItem" style="background: {getPlayerColor(bet.accountId, bet.serverId)}; width: {getChance(getTotalBetsSum, bet.amount)}%"></div>
                {/each}
              </div>
{#if !getWinnerIsVisible}
                <div class="placeBet row-block align-center justify-start full-width">
                  <div class="inputWrapper full-height" class:disabled={getActiveGame || showArrow}>
                    <input
                      bind:value={placeBetAmount}
                      type="number"
                      class="full-width full-height"
                      class:focus={placeBetFocus}
                      disabled={getActiveGame || showArrow}
                      on:focus={placeBetInputFocus}
                      on:blur={placeBetInputUnFocus}
                      on:keypress={preventForNumber}
                      on:cut|preventDefault
                      on:copy|preventDefault
                      on:paste|preventDefault
                    />
                    <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                  </div>

                  <button class="startTime full-height row-block align-center justify-between"
                          class:disabled={getActiveGame || Number(placeBetAmount) <= 0 || showArrow}
                          disabled={getActiveGame || Number(placeBetAmount) <= 0 || showArrow}
                          on:click={makeBet}>
                    <span class="text">{getActiveTimer ? 'Начало через' : 'Играть'}</span>
                    {#if getActiveTimer}
                      <div class="time row-block align-center justify-start">
                        <span class="item row-block align-center justify-center">{getStartTime.minutes[0]}</span>
                        <span class="item row-block align-center justify-center">{getStartTime.minutes[1]}</span>
                        <span class="colon">:</span>
                        <span class="item row-block align-center justify-center">{getStartTime.seconds[0]}</span>
                        <span class="item row-block align-center justify-center">{getStartTime.seconds[1]}</span>
                      </div>
                    {/if}
                  </button>
                </div>
              {/if}
              <!-- Остальная разметка (colors, placeBet, winner block и пр.) без изменений -->
              {#if getWinnerIsVisible}
                <div class="gameItem row-block align-center justify-between full-width">
                  <div class="row-block align-center justify-start">
                    <AccountComponent
                      displayedComponents={["avatar", "serverName", "you", "login", "accountId"]}
                      login={getWinner.login}
                      serverName={getWinner.serverName}
                      serverId={getWinner.serverId}
                      accountId={getWinner.accountId}
                      currentUserId={getAccountId}
                    >
                      <p>Победитель</p>
                    </AccountComponent>
                  </div>

                  <div class="row-block align-center justify-start">
                    <div class="amountWrapper">
                      <span class="amount row-block align-center justify-start">
                        {formatNumber(getWinner.betAmount)}
                        <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                      </span>
                    </div>
                    <svg class="arrowIcon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 18 18">
	<path d="M15.273 9.53026C15.4136 9.38962 15.4926 9.19889 15.4926 9.00001C15.4926 8.80114 15.4136 8.61041 15.273 8.46976L11.0302 4.22701C10.961 4.15538 10.8783 4.09824 10.7868 4.05894C10.6953 4.01963 10.5968 3.99894 10.4973 3.99807C10.3977 3.99721 10.2989 4.01619 10.2067 4.0539C10.1146 4.09161 10.0308 4.1473 9.96042 4.21772C9.89 4.28814 9.83431 4.37187 9.7966 4.46405C9.75889 4.55622 9.73991 4.65498 9.74078 4.75456C9.74164 4.85415 9.76233 4.95256 9.80164 5.04407C9.84094 5.13557 9.89808 5.21833 9.96971 5.28751L12.9322 8.25001L2.99996 8.25001C2.80105 8.25001 2.61029 8.32903 2.46963 8.46968C2.32898 8.61033 2.24996 8.8011 2.24996 9.00001C2.24996 9.19892 2.32898 9.38969 2.46963 9.53034C2.61029 9.67099 2.80105 9.75001 2.99996 9.75001L12.9322 9.75001L9.96971 12.7125C9.8331 12.854 9.7575 13.0434 9.75921 13.2401C9.76092 13.4367 9.83979 13.6248 9.97885 13.7639C10.1179 13.9029 10.306 13.9818 10.5027 13.9835C10.6993 13.9852 10.8888 13.9096 11.0302 13.773L15.273 9.53026Z"/>
</svg>
                    <div class="amountWrapper">
                      <span class="amount row-block align-center justify-start">
                        {formatNumber(getWinner.winCoins)}
                        <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                      </span>
                    </div>
                  </div>

                  <div class="row-block align-center justify-start">
                    <div class="chance row-block align-center justify-start">
                      <span class="desc">Шанс</span>
                      <span class="value">{getWinner.winChance.toFixed(2)}%</span>
                    </div>
                    <div class="ticket row-block align-center justify-start">
                      <span class="desc">Билет</span>
                      
                      <div class="ticketNumber row-block align-center justify-start">
                        <span>#{getWinner.ticket}</span>
                        <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 22">
	<path d="M21.4256 9.47104C21.7422 9.47213 22 9.13271 22 8.75071V6.51223C21.9973 4.85001 20.8819 3.50307 19.5033 3.5L17.1007 3.50109C16.6475 3.50109 16.2793 3.94419 16.2793 4.4921V5.32159C16.2793 5.68504 16.0604 6.01573 15.76 6.04957C15.4154 6.08777 15.1277 5.76145 15.1277 5.35651V4.4921C15.1277 3.94531 14.7605 3.50109 14.3063 3.50109H2.49667C1.11895 3.50327 0.00255174 4.85007 4.36183e-06 6.51332V8.77583C4.36183e-06 9.15892 0.256919 9.47107 0.576245 9.47107C1.50528 9.46889 2.25791 10.3769 2.26062 11.5C2.26062 12.6209 1.50798 13.5289 0.578955 13.5289H0.576241C0.257818 13.5289 0 13.84 0 14.2242V16.4878C0.00271389 18.15 1.11809 19.4969 2.49667 19.5H14.3082C14.7614 19.5 15.1296 19.0569 15.1296 18.509V18.0702C15.1296 17.7068 15.3458 17.375 15.6461 17.3379C15.9908 17.2953 16.2811 17.6206 16.2811 18.0299V18.509C16.2811 19.0558 16.6484 19.5 17.1025 19.5H19.5052C20.882 19.4967 21.9974 18.1499 22 16.4878V14.2493C22 13.8684 21.7422 13.5279 21.4256 13.529C20.4956 13.5301 19.7421 12.6209 19.7421 11.5C19.7421 10.3792 20.4956 9.46995 21.4256 9.47104ZM16.2801 14.9979C16.2801 15.3832 16.0232 15.6931 15.7057 15.6931C15.3863 15.6931 15.1294 15.3832 15.1294 14.9979V13.2102C15.1294 12.8271 15.3863 12.5138 15.7057 12.5138C16.0232 12.5138 16.2801 12.8271 16.2801 13.2102V14.9979ZM16.2801 10.1782C16.2801 10.5635 16.0232 10.8734 15.7057 10.8734C15.3863 10.8734 15.1294 10.5635 15.1294 10.1782V8.39048C15.1294 8.00739 15.3863 7.69416 15.7057 7.69416C16.0232 7.69416 16.2801 8.00739 16.2801 8.39048V10.1782Z"/>
</svg>
                      </div>
                    </div>
                  </div>
                </div>
              {/if}
            </div>

            <!-- ПРАВАЯ ПАНЕЛЬ - Удача дня -->
            <!-- ПРАВАЯ ПАНЕЛЬ - Удача дня -->
            <div class="luckOfDay column-block align-start justify-between">
              <div class="title row-block align-center justify-start full-width">
                <span class="text">Удача дня</span>
              </div>

              <div class="item lucky row-block align-center justify-between full-width" class:empty={!getLuck}>
                <div class="row-block align-center justify-start">
                  {#if getLuck}
                    <AccountComponent
                      displayedComponents={["avatar", "serverName", "you", "login", "accountId"]}
                      login={getLuck.winnerLogin}
                      serverName={getLuck.serverName}
                      serverId={getLuck.serverId}
                      accountId={getLuck.winnerAccountId}
                      currentUserId={getAccountId}
                    >
                      <p>Счастливчик</p>
                    </AccountComponent>
                  {:else}
                    <div class="avatar empty"></div>
                    <div class="nameType column-block align-start justify-start">
                      <div class="name empty"></div>
                      <div class="type empty"></div>
                    </div>
                  {/if}
                </div>
                {#if getLuck}
                  <div class="chance row-block align-center justify-start">
                    <span class="text">Шанс</span>
                    <span class="value">{getLuck.winnerChance.toFixed(2)}%</span>
                  </div>
                {:else}
                  <div class="chance empty row-block align-center justify-start"></div>
                {/if}
              </div>

              <div class="item bigJackpot row-block align-center justify-between full-width" class:empty={!getJackpot}>
                <div class="row-block align-center justify-start">
                  {#if getJackpot}
                    <AccountComponent
                      displayedComponents={["avatar","serverName", "you", "login", "accountId"]}
                      login={getJackpot.winnerLogin}
                      serverName={getJackpot.serverName}
                      serverId={getJackpot.serverId}
                      accountId={getJackpot.winnerAccountId}
                      currentUserId={getAccountId}
                    >
                      <p>Большой куш</p>
                    </AccountComponent>
                  {:else}
                    <div class="avatar empty"></div>
                    <div class="nameType column-block align-start justify-start">
                      <div class="name empty"></div>
                      <div class="type empty"></div>
                    </div>
                  {/if}
                </div>
                {#if getJackpot}
                  <div class="chance row-block align-center justify-start">
                    <span class="text">{formatNumber(getJackpot.bank)}</span>
                    <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                  </div>
                {:else}
                  <div class="chance empty row-block align-center justify-start"></div>
                {/if}
              </div>

              <div class="item prevGame row-block align-center justify-between full-width" class:empty={!getLastGame}>
                <div class="row-block align-center justify-start">
                  {#if getLastGame}
                    <AccountComponent
                      displayedComponents={["avatar","serverName", "you", "login", "accountId"]}
                      login={getLastGame.winnerLogin}
                      serverName={getLastGame.serverName}
                      serverId={getLastGame.serverId}
                      accountId={getLastGame.winnerAccountId}
                      currentUserId={getAccountId}
                    >
                      <p>Пред. игра</p>
                    </AccountComponent>
                  {:else}
                    <div class="avatar empty"></div>
                    <div class="nameType column-block align-start justify-start">
                      <div class="name empty"></div>
                      <div class="type empty"></div>
                    </div>
                  {/if}
                </div>
                {#if getLastGame}
                  <div class="chance row-block align-center justify-start">
                    <span class="text">Шанс</span>
                    <span class="value">{getLastGame.winnerChance?.toFixed(2)}%</span>
                  </div>
                {:else}
                  <div class="chance empty row-block align-center justify-start"></div>
                {/if}
              </div>
            </div>
          </div>

          <!-- Список ставок внизу и пр. -->
          <div class="betsWrapper column-block align-center justify-start full-width" class:empty={getCurrentGameBets.length <= 0}>
            {#if getCurrentGameBets.length <= 0}
              <div class="emptyBets column-block align-center justify-center">
                <span class="text">Ставок нет</span>
                <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 36 36">
    <path d="M8.98633 4.49756C8.15833 4.49756 7.48633 5.16956 7.48633 5.99756C7.48633 6.82556 8.15833 7.49756 8.98633 7.49756H26.9863C27.8143 7.49756 28.4863 6.82556 28.4863 5.99756C28.4863 5.16956 27.8143 4.49756 26.9863 4.49756H8.98633ZM7.48633 10.4976C6.65833 10.4976 5.98633 11.1696 5.98633 11.9976C5.98633 12.8256 6.65833 13.4976 7.48633 13.4976H28.4863C29.3143 13.4976 29.9863 12.8256 29.9863 11.9976C29.9863 11.1696 29.3143 10.4976 28.4863 10.4976H7.48633ZM8.98633 16.4976C6.50083 16.4976 4.48633 18.5121 4.48633 20.9976V26.9976C4.48633 29.4831 6.50083 31.4976 8.98633 31.4976H26.9863C29.4718 31.4976 31.4863 29.4831 31.4863 26.9976V20.9976C31.4863 18.5121 29.4718 16.4976 26.9863 16.4976H8.98633Z"/>

    <defs>
        <linearGradient id="paint0_linear_55_3401" x1="17.9648" y1="7.49609" x2="17.9648" y2="28.4961" gradientUnits="userSpaceOnUse">
            <stop stop-color="white"/>
            <stop offset="1" stop-color="#9A9A9A"/>
        </linearGradient>
    </defs>
</svg>
              </div>
            {/if}

            {#each getCurrentGameBetsSorted as bet}
              <div class="betItem row-block align-center justify-between full-width"
                   class:owned={bet.accountId === getAccountId}
                   style="border-color: {getPlayerColor(bet.accountId, bet.serverId)}">
                <AccountComponent
                  displayedComponents={["avatar","serverName", "you", "login", "accountId"]}
                  login={bet.login}
                  serverName={bet.serverName}
                  serverId={bet.serverId}
                  accountId={bet.accountId}
                  currentUserId={getAccountId}
                />
                
                <div class="amountWrapper">
                  <span class="amount row-block align-center justify-start">
                    {formatNumber(bet.amount)}
                    <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                  </span>
                </div>

                <div class="chance row-block align-center justify-start">
                  <span class="title">Шанс</span>
                  <span class="value">{getChance(getTotalBetsSum, bet.amount)}%</span>
                </div>

                <div class="ticket row-block align-center justify-start">
                  <span class="title">Билет</span>
                  
                  <div class="ticketNumber row-block align-center justify-start">
                    <span>#{bet.ticket}</span>
                    <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 22">
	<path d="M21.4256 9.47104C21.7422 9.47213 22 9.13271 22 8.75071V6.51223C21.9973 4.85001 20.8819 3.50307 19.5033 3.5L17.1007 3.50109C16.6475 3.50109 16.2793 3.94419 16.2793 4.4921V5.32159C16.2793 5.68504 16.0604 6.01573 15.76 6.04957C15.4154 6.08777 15.1277 5.76145 15.1277 5.35651V4.4921C15.1277 3.94531 14.7605 3.50109 14.3063 3.50109H2.49667C1.11895 3.50327 0.00255174 4.85007 4.36183e-06 6.51332V8.77583C4.36183e-06 9.15892 0.256919 9.47107 0.576245 9.47107C1.50528 9.46889 2.25791 10.3769 2.26062 11.5C2.26062 12.6209 1.50798 13.5289 0.578955 13.5289H0.576241C0.257818 13.5289 0 13.84 0 14.2242V16.4878C0.00271389 18.15 1.11809 19.4969 2.49667 19.5H14.3082C14.7614 19.5 15.1296 19.0569 15.1296 18.509V18.0702C15.1296 17.7068 15.3458 17.375 15.6461 17.3379C15.9908 17.2953 16.2811 17.6206 16.2811 18.0299V18.509C16.2811 19.0558 16.6484 19.5 17.1025 19.5H19.5052C20.882 19.4967 21.9974 18.1499 22 16.4878V14.2493C22 13.8684 21.7422 13.5279 21.4256 13.529C20.4956 13.5301 19.7421 12.6209 19.7421 11.5C19.7421 10.3792 20.4956 9.46995 21.4256 9.47104ZM16.2801 14.9979C16.2801 15.3832 16.0232 15.6931 15.7057 15.6931C15.3863 15.6931 15.1294 15.3832 15.1294 14.9979V13.2102C15.1294 12.8271 15.3863 12.5138 15.7057 12.5138C16.0232 12.5138 16.2801 12.8271 16.2801 13.2102V14.9979ZM16.2801 10.1782C16.2801 10.5635 16.0232 10.8734 15.7057 10.8734C15.3863 10.8734 15.1294 10.5635 15.1294 10.1782V8.39048C15.1294 8.00739 15.3863 7.69416 15.7057 7.69416C16.0232 7.69416 16.2801 8.00739 16.2801 8.39048V10.1782Z"/>
</svg>
                  </div>
                </div>
              </div>
            {/each}
          </div>
        </div>
       {:else if getOpenedPageIndex === 1 || getOpenedPage === 'historyList'}
        <!-- СТРАНИЦА ИСТОРИИ -->
        <div class="historyList full-width full-height column-block align-center justify-start">
          {#if getHistoryList && getHistoryList.length > 0}
            <div class="title row-block align-center justify-start full-width">
              <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
	<path d="M9 18.5C4.03725 18.5 0 14.4628 0 9.5C0 4.53725 4.03725 0.5 9 0.5C9.414 0.5 9.75 0.836 9.75 1.25C9.75 1.664 9.414 2 9 2C4.8645 2 1.5 5.3645 1.5 9.5C1.5 13.6355 4.8645 17 9 17C13.1355 17 16.5 13.6355 16.5 9.5C16.5 9.086 16.8353 8.75 17.25 8.75C17.6647 8.75 18 9.086 18 9.5C18 14.4628 13.9628 18.5 9 18.5ZM9 2C8.805 2 8.61 1.9175 8.4675 1.7825C8.3325 1.64 8.25 1.445 8.25 1.25C8.25 1.055 8.3325 0.86 8.4675 0.7175C8.7525 0.44 9.255 0.44 9.5325 0.7175C9.6675 0.86 9.75 1.055 9.75 1.25C9.75 1.445 9.6675 1.64 9.5325 1.7825C9.39 1.9175 9.195 2 9 2ZM15.93 6.6275C15.7725 6.245 15.9518 5.8025 16.335 5.645C16.7175 5.4875 17.16 5.6675 17.3175 6.05C17.475 6.4325 17.295 6.875 16.9125 7.0325C16.815 7.07 16.7175 7.0925 16.6275 7.0925C16.3275 7.0925 16.0492 6.92 15.93 6.6275ZM14.3018 4.1975C14.0093 3.905 14.0093 3.425 14.3018 3.1325C14.595 2.84 15.0675 2.84 15.3593 3.1325C15.6525 3.425 15.6525 3.905 15.3593 4.1975C15.2175 4.34 15.0225 4.415 14.835 4.415C14.64 4.415 14.4525 4.34 14.3018 4.1975ZM11.8725 2.57C11.4893 2.4125 11.31 1.97 11.4675 1.5875C11.625 1.205 12.06 1.025 12.45 1.1825C12.8325 1.34 13.0125 1.7825 12.8475 2.165C12.735 2.45 12.45 2.6225 12.1575 2.6225C12.06 2.6225 11.9625 2.6075 11.8725 2.57ZM17.25 10.25C17.055 10.25 16.8593 10.1675 16.7175 10.0325C16.5818 9.89 16.5 9.695 16.5 9.5C16.5 9.305 16.5818 9.11 16.7175 8.9675C17.0025 8.69 17.505 8.69 17.7825 8.9675C17.9175 9.11 18 9.305 18 9.5C18 9.695 17.9175 9.89 17.7825 10.0325C17.64 10.1675 17.445 10.25 17.25 10.25ZM11.9992 11.75C11.8867 11.75 11.772 11.7245 11.6648 11.6713L8.66475 10.1713C8.4105 10.0438 8.25 9.78425 8.25 9.5V4.25C8.25 3.836 8.586 3.5 9 3.5C9.414 3.5 9.75 3.836 9.75 4.25V9.0365L12.3353 10.3295C12.7058 10.5147 12.8557 10.9655 12.6705 11.336C12.5392 11.5985 12.2745 11.75 11.9992 11.75Z"/>
</svg>
              <span class="text">
                <div class="back__icon active" on:click={() => openPage(0)}>
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none">
                    <path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"></path>
                  </svg>
                </div>
                 
                История игры - Джекпот
              </span>
            </div>

            <div class="list full-width full-height column-block align-center justify-start">
              {#each getHistoryList as game}
                <button class="gameItem historyGameItem row-block align-center justify-between full-width" on:click={() => openPage(2, game.id)}>
                  <div class="historyListItem row-block align-center justify-start">
                    <div class="gameID">
                      <span>#{game.id}</span>
                    </div>
                    <AccountComponent
                      displayedComponents={["avatar","serverName", "you", "login", "accountId"]}
                      login={game.winnerLogin}
                      serverName={game.winnerServerName}
                      serverId={game.winnerServerId}
                      accountId={game.winnerAccountId}
                      currentUserId={getAccountId}
                    >
                      <p>Победитель</p>
                    </AccountComponent>
                  </div>

                  <div class="historyListItem">
                    <div class="row-block align-center justify-center">
                      <div class="amountWrapperItems">
                        <div class="amountWrapper">
                          <span class="amount row-block align-center justify-start">
                            {formatNumber(game.winnerAmount)}
                            <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                          </span>
                        </div>
                      </div>
                      <svg class="arrowIcon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 18 18">
	<path d="M15.273 9.53026C15.4136 9.38962 15.4926 9.19889 15.4926 9.00001C15.4926 8.80114 15.4136 8.61041 15.273 8.46976L11.0302 4.22701C10.961 4.15538 10.8783 4.09824 10.7868 4.05894C10.6953 4.01963 10.5968 3.99894 10.4973 3.99807C10.3977 3.99721 10.2989 4.01619 10.2067 4.0539C10.1146 4.09161 10.0308 4.1473 9.96042 4.21772C9.89 4.28814 9.83431 4.37187 9.7966 4.46405C9.75889 4.55622 9.73991 4.65498 9.74078 4.75456C9.74164 4.85415 9.76233 4.95256 9.80164 5.04407C9.84094 5.13557 9.89808 5.21833 9.96971 5.28751L12.9322 8.25001L2.99996 8.25001C2.80105 8.25001 2.61029 8.32903 2.46963 8.46968C2.32898 8.61033 2.24996 8.8011 2.24996 9.00001C2.24996 9.19892 2.32898 9.38969 2.46963 9.53034C2.61029 9.67099 2.80105 9.75001 2.99996 9.75001L12.9322 9.75001L9.96971 12.7125C9.8331 12.854 9.7575 13.0434 9.75921 13.2401C9.76092 13.4367 9.83979 13.6248 9.97885 13.7639C10.1179 13.9029 10.306 13.9818 10.5027 13.9835C10.6993 13.9852 10.8888 13.9096 11.0302 13.773L15.273 9.53026Z"/>
</svg>
                      <div class="amountWrapperItems">
                        <div class="amountWrapper">
                          <span class="amount row-block align-center justify-start">
                            {formatNumber(game.bank)}
                            <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <div class="historyListItem row-block align-center justify-start">
                    <div class="chance row-block align-center justify-start">
                      <span class="desc">Шанс</span>
                      <span class="value">{game.winnerChance?.toFixed(2)}%</span>
                    </div>
                    <div class="ticket row-block align-center justify-start">
                      <span class="desc">Билет</span>
                      
                      <div class="ticketNumber row-block align-center justify-start">
                        <span>#{game.winnerTicket}</span>
                        <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 22">
	<path d="M21.4256 9.47104C21.7422 9.47213 22 9.13271 22 8.75071V6.51223C21.9973 4.85001 20.8819 3.50307 19.5033 3.5L17.1007 3.50109C16.6475 3.50109 16.2793 3.94419 16.2793 4.4921V5.32159C16.2793 5.68504 16.0604 6.01573 15.76 6.04957C15.4154 6.08777 15.1277 5.76145 15.1277 5.35651V4.4921C15.1277 3.94531 14.7605 3.50109 14.3063 3.50109H2.49667C1.11895 3.50327 0.00255174 4.85007 4.36183e-06 6.51332V8.77583C4.36183e-06 9.15892 0.256919 9.47107 0.576245 9.47107C1.50528 9.46889 2.25791 10.3769 2.26062 11.5C2.26062 12.6209 1.50798 13.5289 0.578955 13.5289H0.576241C0.257818 13.5289 0 13.84 0 14.2242V16.4878C0.00271389 18.15 1.11809 19.4969 2.49667 19.5H14.3082C14.7614 19.5 15.1296 19.0569 15.1296 18.509V18.0702C15.1296 17.7068 15.3458 17.375 15.6461 17.3379C15.9908 17.2953 16.2811 17.6206 16.2811 18.0299V18.509C16.2811 19.0558 16.6484 19.5 17.1025 19.5H19.5052C20.882 19.4967 21.9974 18.1499 22 16.4878V14.2493C22 13.8684 21.7422 13.5279 21.4256 13.529C20.4956 13.5301 19.7421 12.6209 19.7421 11.5C19.7421 10.3792 20.4956 9.46995 21.4256 9.47104ZM16.2801 14.9979C16.2801 15.3832 16.0232 15.6931 15.7057 15.6931C15.3863 15.6931 15.1294 15.3832 15.1294 14.9979V13.2102C15.1294 12.8271 15.3863 12.5138 15.7057 12.5138C16.0232 12.5138 16.2801 12.8271 16.2801 13.2102V14.9979ZM16.2801 10.1782C16.2801 10.5635 16.0232 10.8734 15.7057 10.8734C15.3863 10.8734 15.1294 10.5635 15.1294 10.1782V8.39048C15.1294 8.00739 15.3863 7.69416 15.7057 7.69416C16.0232 7.69416 16.2801 8.00739 16.2801 8.39048V10.1782Z"/>
</svg>
                      </div>
                    </div>
                  </div>
                </button>
              {/each}
            </div>
          {:else}
            <span class="empty-list">История пуста</span>
          {/if}
        </div>

      {:else if (getOpenedPageIndex === 2 || getOpenedPage === 'historyItem') && getHistoryItem && getHistoryItemData}
        <!-- ДЕТАЛИ ИГРЫ -->
        {#if getHistoryItemData && getHistoryItemBets}
          <div class="historyItem full-width full-height column-block align-center justify-start">
            <div class="info row-block align-center justify-between full-width">
              <div class="row-block align-center justify-start">
                <span class="title">Игра
                  <div class="back__icon active" on:click={() => openPage(1)}>
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none">
                      <path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"></path>
                    </svg>
                  </div>
                </span>
                <div class="gameID">
                  <span>#{getHistoryItemData.id}</span>
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
                    <span class="count">{getHistoryItemBets.length}</span>
                    <span class="text">Игроки</span>
                    
                  </div>
                </div>

                <div class="bank row-block align-center justify-start">
                  <div class="bank-info column-block align-start justify-between">
                    <span class="count row-block align-center justify-start">
                      {formatNumber(getHistoryItemData.bank)}
                      <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                    </span>
                    <span class="text">Банк</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="gameWrapper row-block align-center justify-center full-width">
              <div class="game column-block align-start justify-between">
                <div bind:this={betsHistoryRef} class="bets row-block align-center justify-start full-width" class:empty={getHistoryItemBets.length <= 0}>
                  <div class="winArrowWrapper row-block align-center justify-center full-width">
                    <svg class="winArrow" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 26 23">
                      <path d="M15.6053 21.5161C14.4474 23.4946 11.5526 23.4946 10.3947 21.5161L0.407536 4.45161C-0.750391 2.47312 0.697019 3.04708e-07 3.01287 1.0225e-07L22.9871 -1.64396e-06C25.303 -1.84641e-06 26.7504 2.47312 25.5925 4.45161L15.6053 21.5161Z" fill="url(#paint0_linear_55_3401)"/>
                      <defs>
                        <linearGradient id="paint0_linear_55_3401" x1="13" y1="25.9677" x2="13" y2="-8.65591" gradientUnits="userSpaceOnUse">
                          <stop stop-color="white"/>
                          <stop offset="1" stop-color="#999999"/>
                        </linearGradient>
                      </defs>
                    </svg>
                  </div>

                  {#if playerItemsHistory && playerItemsHistory.length > 0}
                    <div bind:this={playerBetWrapperHistory} class="playerBetWrapper scrollDisabled row-block align-center justify-start full-width">
                     {#each playerItemsHistory as item (item.scrollId)}
  <div class="playerBetHistory column-block align-center justify-start"
       data-scrollid={item.scrollId}
       class:lose={!!(winItemHistory && winItemHistory.scrollId !== item.scrollId)}
       style="border-color: {getPlayerColorHistory(item.accountId, item.serverId)}">
    <AccountComponent
      displayedComponents={["avatar", "you"]}
      login={item.login}
      accountId={item.accountId}
      currentUserId={getAccountId}
    />
    <span class="amount row-block align-center justify-start">
      {formatNumber(item.amount)}
      <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
    </span>
    <span class="chance">{getChance(getHistoryItemData.bank, item.amount)}%</span>
    <div class="color" style="background-color: {getPlayerColorHistory(item.accountId, item.serverId)}"></div>
  </div>
{/each}
                    </div>
                  {/if}
                </div>

                <div class="colors row-block align-center justify-start full-width">
                  {#each getHistoryItemBetsSorted as bet}
                    <div class="colorItem" style="background: {getPlayerColorHistory(bet.accountId, bet.serverId)}; width: {getChance(getHistoryItemData.bank, bet.amount)}%"></div>
                  {/each}
                </div>

                <div class="gameItem row-block align-center justify-between full-width">
                  <div class="row-block align-center justify-start">
                    <AccountComponent
                      displayedComponents={["avatar","serverName", "you", "login", "accountId"]}
                      login={getHistoryItemData.winnerLogin}
                      serverName={getHistoryItemData.winnerServerName}
                      serverId={getHistoryItemData.winnerServerId}
                      accountId={getHistoryItemData.winnerAccountId}
                      currentUserId={getAccountId}
                    >
                      <p>Победитель</p>
                    </AccountComponent>
                  </div>

                  <div class="row-block align-center justify-start">
                    <div class="amountWrapper">
                      <span class="amount row-block align-center justify-start">
                        {formatNumber(getHistoryItemData.winnerAmount)}
                        <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                      </span>
                    </div>
                    <svg class="arrowIcon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 18 18">
	<path d="M15.273 9.53026C15.4136 9.38962 15.4926 9.19889 15.4926 9.00001C15.4926 8.80114 15.4136 8.61041 15.273 8.46976L11.0302 4.22701C10.961 4.15538 10.8783 4.09824 10.7868 4.05894C10.6953 4.01963 10.5968 3.99894 10.4973 3.99807C10.3977 3.99721 10.2989 4.01619 10.2067 4.0539C10.1146 4.09161 10.0308 4.1473 9.96042 4.21772C9.89 4.28814 9.83431 4.37187 9.7966 4.46405C9.75889 4.55622 9.73991 4.65498 9.74078 4.75456C9.74164 4.85415 9.76233 4.95256 9.80164 5.04407C9.84094 5.13557 9.89808 5.21833 9.96971 5.28751L12.9322 8.25001L2.99996 8.25001C2.80105 8.25001 2.61029 8.32903 2.46963 8.46968C2.32898 8.61033 2.24996 8.8011 2.24996 9.00001C2.24996 9.19892 2.32898 9.38969 2.46963 9.53034C2.61029 9.67099 2.80105 9.75001 2.99996 9.75001L12.9322 9.75001L9.96971 12.7125C9.8331 12.854 9.7575 13.0434 9.75921 13.2401C9.76092 13.4367 9.83979 13.6248 9.97885 13.7639C10.1179 13.9029 10.306 13.9818 10.5027 13.9835C10.6993 13.9852 10.8888 13.9096 11.0302 13.773L15.273 9.53026Z"/>
</svg>
                    <div class="amountWrapper">
                      <span class="amount row-block align-center justify-start">
                        {formatNumber(getHistoryItemData.bank)}
                        <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                      </span>
                    </div>
                  </div>

                  <div class="row-block align-center justify-start">
                    <div class="chance row-block align-center justify-start">
                      <span class="desc">Шанс</span>
                      <span class="value">{getHistoryItemData.winnerChance?.toFixed(2)}%</span>
                    </div>
                    <div class="ticket row-block align-center justify-start">
                      <span class="desc">Билет</span>
                      
                      <div class="ticketNumber row-block align-center justify-start">
                        <span>#{getHistoryItemData.winnerTicket}</span>
                        <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 22">
	<path d="M21.4256 9.47104C21.7422 9.47213 22 9.13271 22 8.75071V6.51223C21.9973 4.85001 20.8819 3.50307 19.5033 3.5L17.1007 3.50109C16.6475 3.50109 16.2793 3.94419 16.2793 4.4921V5.32159C16.2793 5.68504 16.0604 6.01573 15.76 6.04957C15.4154 6.08777 15.1277 5.76145 15.1277 5.35651V4.4921C15.1277 3.94531 14.7605 3.50109 14.3063 3.50109H2.49667C1.11895 3.50327 0.00255174 4.85007 4.36183e-06 6.51332V8.77583C4.36183e-06 9.15892 0.256919 9.47107 0.576245 9.47107C1.50528 9.46889 2.25791 10.3769 2.26062 11.5C2.26062 12.6209 1.50798 13.5289 0.578955 13.5289H0.576241C0.257818 13.5289 0 13.84 0 14.2242V16.4878C0.00271389 18.15 1.11809 19.4969 2.49667 19.5H14.3082C14.7614 19.5 15.1296 19.0569 15.1296 18.509V18.0702C15.1296 17.7068 15.3458 17.375 15.6461 17.3379C15.9908 17.2953 16.2811 17.6206 16.2811 18.0299V18.509C16.2811 19.0558 16.6484 19.5 17.1025 19.5H19.5052C20.882 19.4967 21.9974 18.1499 22 16.4878V14.2493C22 13.8684 21.7422 13.5279 21.4256 13.529C20.4956 13.5301 19.7421 12.6209 19.7421 11.5C19.7421 10.3792 20.4956 9.46995 21.4256 9.47104ZM16.2801 14.9979C16.2801 15.3832 16.0232 15.6931 15.7057 15.6931C15.3863 15.6931 15.1294 15.3832 15.1294 14.9979V13.2102C15.1294 12.8271 15.3863 12.5138 15.7057 12.5138C16.0232 12.5138 16.2801 12.8271 16.2801 13.2102V14.9979ZM16.2801 10.1782C16.2801 10.5635 16.0232 10.8734 15.7057 10.8734C15.3863 10.8734 15.1294 10.5635 15.1294 10.1782V8.39048C15.1294 8.00739 15.3863 7.69416 15.7057 7.69416C16.0232 7.69416 16.2801 8.00739 16.2801 8.39048V10.1782Z"/>
</svg>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="betsWrapper column-block align-center justify-start full-width" class:empty={getHistoryItemBets.length <= 0}>
              {#if getHistoryItemBets.length <= 0}
                <div class="emptyBets column-block align-center justify-center">
                  <span class="text">Ставок нет</span>
                  <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 36 36">
    <path d="M8.98633 4.49756C8.15833 4.49756 7.48633 5.16956 7.48633 5.99756C7.48633 6.82556 8.15833 7.49756 8.98633 7.49756H26.9863C27.8143 7.49756 28.4863 6.82556 28.4863 5.99756C28.4863 5.16956 27.8143 4.49756 26.9863 4.49756H8.98633ZM7.48633 10.4976C6.65833 10.4976 5.98633 11.1696 5.98633 11.9976C5.98633 12.8256 6.65833 13.4976 7.48633 13.4976H28.4863C29.3143 13.4976 29.9863 12.8256 29.9863 11.9976C29.9863 11.1696 29.3143 10.4976 28.4863 10.4976H7.48633ZM8.98633 16.4976C6.50083 16.4976 4.48633 18.5121 4.48633 20.9976V26.9976C4.48633 29.4831 6.50083 31.4976 8.98633 31.4976H26.9863C29.4718 31.4976 31.4863 29.4831 31.4863 26.9976V20.9976C31.4863 18.5121 29.4718 16.4976 26.9863 16.4976H8.98633Z"/>

    <defs>
        <linearGradient id="paint0_linear_55_3401" x1="17.9648" y1="7.49609" x2="17.9648" y2="28.4961" gradientUnits="userSpaceOnUse">
            <stop stop-color="white"/>
            <stop offset="1" stop-color="#9A9A9A"/>
        </linearGradient>
    </defs>
</svg>
                </div>
              {/if}

              {#each getHistoryItemBetsSorted as bet}
                <div class="betItem row-block align-center justify-between full-width"
                     style="border-color: {getPlayerColorHistory(bet.accountId, bet.serverId)}">
                  <AccountComponent
                    displayedComponents={["avatar","serverName", "you", "login", "accountId"]}
                    login={bet.login}
                    serverName={bet.serverName}
                    serverId={bet.serverId}
                    accountId={bet.accountId}
                    currentUserId={getAccountId}
                  />
                  
                  <div class="amountWrapper">
                    <span class="amount row-block align-center justify-start">
                      {formatNumber(bet.amount)}
                      <img class="coins" src="{cdn}img/donate/mcoin.svg" alt="" />
                    </span>
                  </div>

                  <div class="chance row-block align-center justify-start">
                    <span class="title">Шанс</span>
                    <span class="value">{getChance(getHistoryItemData.bank, bet.amount)}%</span>
                  </div>

                  <div class="ticket row-block align-center justify-start">
                    <span class="title">Билет</span>
                    
                    <div class="ticketNumber row-block align-center justify-start">
                      <span>#{bet.ticket}</span>
                      <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22 22">
	<path d="M21.4256 9.47104C21.7422 9.47213 22 9.13271 22 8.75071V6.51223C21.9973 4.85001 20.8819 3.50307 19.5033 3.5L17.1007 3.50109C16.6475 3.50109 16.2793 3.94419 16.2793 4.4921V5.32159C16.2793 5.68504 16.0604 6.01573 15.76 6.04957C15.4154 6.08777 15.1277 5.76145 15.1277 5.35651V4.4921C15.1277 3.94531 14.7605 3.50109 14.3063 3.50109H2.49667C1.11895 3.50327 0.00255174 4.85007 4.36183e-06 6.51332V8.77583C4.36183e-06 9.15892 0.256919 9.47107 0.576245 9.47107C1.50528 9.46889 2.25791 10.3769 2.26062 11.5C2.26062 12.6209 1.50798 13.5289 0.578955 13.5289H0.576241C0.257818 13.5289 0 13.84 0 14.2242V16.4878C0.00271389 18.15 1.11809 19.4969 2.49667 19.5H14.3082C14.7614 19.5 15.1296 19.0569 15.1296 18.509V18.0702C15.1296 17.7068 15.3458 17.375 15.6461 17.3379C15.9908 17.2953 16.2811 17.6206 16.2811 18.0299V18.509C16.2811 19.0558 16.6484 19.5 17.1025 19.5H19.5052C20.882 19.4967 21.9974 18.1499 22 16.4878V14.2493C22 13.8684 21.7422 13.5279 21.4256 13.529C20.4956 13.5301 19.7421 12.6209 19.7421 11.5C19.7421 10.3792 20.4956 9.46995 21.4256 9.47104ZM16.2801 14.9979C16.2801 15.3832 16.0232 15.6931 15.7057 15.6931C15.3863 15.6931 15.1294 15.3832 15.1294 14.9979V13.2102C15.1294 12.8271 15.3863 12.5138 15.7057 12.5138C16.0232 12.5138 16.2801 12.8271 16.2801 13.2102V14.9979ZM16.2801 10.1782C16.2801 10.5635 16.0232 10.8734 15.7057 10.8734C15.3863 10.8734 15.1294 10.5635 15.1294 10.1782V8.39048C15.1294 8.00739 15.3863 7.69416 15.7057 7.69416C16.0232 7.69416 16.2801 8.00739 16.2801 8.39048V10.1782Z"/>
</svg>
                    </div>
                  </div>
                </div>
              {/each}
            </div>
          </div>
        {/if}
      {/if}
    </div>
  {/if}
</div>
