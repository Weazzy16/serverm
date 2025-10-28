<script>
  import { onMount, onDestroy } from 'svelte';
  import Wheel from './Wheel.svelte';
  import './wheel.css';

  let data = { 
    active: false,
    timer: { active: false },
    wheel: { active: false },
    currentGame: { betsTypes: {} },
    history: [],
    historyList: [],
    historyItem: null,
    pages: ['main', 'historyList', 'historyItem'],
    openedPage: 0
  };
  let accountId = null;

  // Переменные для контроля смены страниц
  let pendingPageChange = null;

  // ИСПРАВЛЕННЫЙ обработчик обновления данных
  function handleUpdate(e) {
    try {
      console.log('[WheelGames] Update event received:', e.detail);
      const newData = JSON.parse(e.detail);
      
      // Если мы ожидаем смену страницы, не перезаписываем openedPage
      if (pendingPageChange && newData.openedPage !== pendingPageChange.page) {
        // Игнорируем это обновление для openedPage
        const { openedPage, ...dataWithoutPage } = newData;
        data = { ...data, ...dataWithoutPage };
      } else {
        data = { ...newData };
        if (pendingPageChange && newData.openedPage === pendingPageChange.page) {
          pendingPageChange = null; // сброс после успешной смены
        }
      }
      
      if (newData?.main?.accountId) {
        accountId = Number(newData.main.accountId);
      }
      
      console.log('[WheelGames] Data updated successfully. Timer:', data?.timer?.active, 'Wheel:', data?.wheel?.active, 'Page:', data?.openedPage);
    } catch (e) {
      console.error('[WheelGames] Update error:', e);
    }
  }

  // Обработчик событий истории
  function handleHistory(e) {
    try {
      console.log('[WheelGames] History event received:', e.detail);
      const historyData = JSON.parse(e.detail);
      
      if (historyData.type === 'historyList') {
        data = { 
          ...data, 
          historyList: historyData.historyList,
          openedPage: historyData.openedPage 
        };
      } else if (historyData.type === 'historyItem') {
        data = { 
          ...data, 
          historyItem: historyData.historyItem,
          openedPage: historyData.openedPage 
        };
      }
      
      console.log('[WheelGames] History data updated successfully');
    } catch (e) {
      console.error('[WheelGames] History update error:', e);
    }
  }

  onMount(() => {
    console.log('[WheelGames] Component mounted');
    
    // Подписываемся на события обновления
    window.addEventListener('wheelgames_update', handleUpdate);
    window.addEventListener('wheelgames_history', handleHistory);
    
    // Запрос данных через mp.trigger
    setTimeout(() => {
      console.log('[WheelGames] Requesting initial data');
      mp.trigger('client.wheelgames.request');
    }, 100);
  });

  onDestroy(() => {
    console.log('[WheelGames] Component destroying');
    window.removeEventListener('wheelgames_update', handleUpdate);
  window.removeEventListener('wheelgames_history', handleHistory);

  });

  // Игровые события (отправляем через mp.trigger)
  function handleBet(e) {
    const { amount, color } = e.detail;
    console.log('[WheelGames] Making bet:', amount, 'on', color);
    mp.trigger('client.wheelgames.bet', amount, color);
  }

  function handleOpenPage(e) {
    const { page, id } = e.detail;
    console.log('[WheelGames] Opening page:', page, id);
    
    // Устанавливаем ожидание смены страницы
    pendingPageChange = { page, id };
    
    mp.trigger('client.wheelgames.openPage', page, id);
  }
</script>

<Wheel
  {data}
  {accountId}
  on:bet={handleBet}
  on:openPage={handleOpenPage}
/>