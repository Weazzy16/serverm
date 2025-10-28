<script>
  import { onMount, onDestroy } from 'svelte';
  import Jackpot from './Jackpot.svelte';
  import './jackpot.css';

  let data = { 
    active: false,
    timer: { active: false },
    game: { active: false },
    currentGame: { bets: [] },
    historyList: [],
    historyItem: null,
    luck: null,
    jackpot: null,
    lastGame: null,
    pages: ['main', 'historyList', 'historyItem'],
    openedPage: 0
  };
  let accountId = null;

  // Обработчик обновления данных - УПРОЩЕННЫЙ
  function handleUpdate(e) {
    try {
      const newData = JSON.parse(e.detail);
      
      // Просто обновляем все данные, включая openedPage
      data = { ...newData };
      
      if (newData?.main?.accountId) {
        accountId = Number(newData.main.accountId);
      }
      
    } catch (e) {
      console.error('[Jackpot] Update error:', e);
    }
  }

  // Обработчик событий истории
  function handleHistory(e) {
    try {
      const historyData = JSON.parse(e.detail);
      
      if (historyData.type === 'historyList') {
        console.log('[Jackpot] Updating historyList, items:', historyData.historyList?.length);
        data = { 
          ...data, 
          historyList: historyData.historyList || [],
          openedPage: historyData.openedPage || 1
        };
      } else if (historyData.type === 'historyItem') {
        console.log('[Jackpot] Updating historyItem');
        data = { 
          ...data, 
          historyItem: historyData.historyItem,
          openedPage: historyData.openedPage || 2
        };
      }
      
      console.log('[Jackpot] History updated, page:', data.openedPage);
    } catch (e) {
      console.error('[Jackpot] History update error:', e);
    }
  }

  onMount(() => {
    console.log('[Jackpot] Component mounted');
    
    window.addEventListener('jackpot_update', handleUpdate);
    window.addEventListener('jackpot_history', handleHistory);
    
    setTimeout(() => {
      console.log('[Jackpot] Requesting initial data');
      mp.trigger('client.jackpot.request');
    }, 100);
  });

  onDestroy(() => {
    console.log('[Jackpot] Component destroying');
    window.removeEventListener('jackpot_update', handleUpdate);
    window.removeEventListener('jackpot_history', handleHistory);
  });

  function handleBet(e) {
    const { amount } = e.detail;
    console.log('[Jackpot] Making bet:', amount);
    mp.trigger('client.jackpot.bet', amount);
  }

  function handleOpenPage(e) {
    const { page, id } = e.detail;
    console.log('[Jackpot] Opening page:', page, id);
    
    // Просто отправляем запрос без сохранения состояния
    mp.trigger('client.jackpot.openPage', page, id || 0);
  }
</script>

<Jackpot
  {data}
  {accountId}
  on:bet={handleBet}
  on:openPage={handleOpenPage}
/>