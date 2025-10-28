<script>
  import { onMount, onDestroy } from 'svelte';
  import Game from './game.svelte';
  import './crash.css';

  let data = { 
    active: false,
    timer: { active: false },
    game: { active: false },
    currentGame: { players: [] },
    history: []
  };
  let accountId = null;

  // Обработчик обновления данных (аналогично календарю)
  function handleUpdate(e) {
    try {
      console.log('[CrashGames] Update event received:', e.detail);
      const newData = JSON.parse(e.detail);
      data = { ...newData };
      
      if (newData?.main?.accountId) {
        accountId = Number(newData.main.accountId);
      }
      
      console.log('[CrashGames] Data updated successfully. Timer:', data?.timer?.active, 'Game:', data?.game?.active);
    } catch (e) {
      console.error('[CrashGames] Update error:', e);
    }
  }

  onMount(() => {
    console.log('[CrashGames] Component mounted');
    
    // Подписываемся на событие обновления (как в календаре)
    window.addEventListener('crashgames_update', handleUpdate);
    
    // Запрос данных через mp.trigger (как в календаре)
    setTimeout(() => {
      console.log('[CrashGames] Requesting initial data');
      mp.trigger('client.crashgames.request');
    }, 100);
  });

  onDestroy(() => {
    console.log('[CrashGames] Component destroying');
    window.removeEventListener('crashgames_update', handleUpdate);
  });

  // Игровые события (отправляем через mp.trigger)
  function handleBet(e) {
    const { betAmount, autoTakeX } = e.detail;
    console.log('[CrashGames] Making bet:', betAmount, autoTakeX);
    mp.trigger('client.crashgames.bet', betAmount, autoTakeX);
  }

  function handleTake() {
    console.log('[CrashGames] Taking winnings');
    mp.trigger('client.crashgames.take');
  }

  function handleOpenPage(e) {
    const { page, id } = e.detail;
    console.log('[CrashGames] Opening page:', page, id);
    mp.trigger('client.crashgames.openPage', page, id);
  }
</script>

<Game
  {data}
  {accountId}
  on:bet={handleBet}
  on:take={handleTake}
  on:openPage={handleOpenPage}
/>