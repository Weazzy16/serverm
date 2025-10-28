<script>
  import { ItemType, ItemId, itemsInfo } from 'json/itemsInfo.js';

  export let item;
  export let currentMonth;   // 0–11
  export let selectedMonth;  // 0–11
  export let todayDay;       // 1–31

  let buyModal = false;
$: itemColor = itemsInfo[item.rewardItemId]?.Color?.toLowerCase() || '';
  // 1) Заголовок: для вещей берём из словаря, иначе просто Title
   $: displayTitle = (
    item.rewardType === 'Item'
      // для предметов — берём имя из словаря
      ? (itemsInfo[item.rewardItemId]?.Name ?? item.title)
      : item.rewardType === 'Money'
        // для денег — форматируем с разделителем тысяч и добавляем знак $
        ? `$${item.rewardCount.toLocaleString('ru-RU')}`
        : item.rewardType === 'Donate'
          // для доната — точно так же, но суффикс MC
          ? `${item.rewardCount.toLocaleString('ru-RU')} MC`
          // fallback
          : item.title
  );

  // 2) Иконка награды
  const ICON_DONATE  = 'https://cdn.majestic-files.com/public/master/static/icons/global/mc.svg';
  const ICON_MONEY = 'https://cdn.majestic-files.com/public/master/static/img/donate/products/10k.png';
  $: rewardIcon = (
    item.rewardType === 'Money'  ? ICON_MONEY
    : item.rewardType === 'Donate' ? ICON_DONATE
    : `http://cdn.piecerp.ru/cloud/inventoryItems/items/${item.rewardItemId}.png`
  );

  // 3) Статус для отображения
$: displayStatus = (() => {
    if (item.status === 'received') return 'received';

    // прошлые месяцы — всё пропущено
    if (selectedMonth < currentMonth) return 'skipped';
    // будущие месяцы — ещё не наступило
    if (selectedMonth > currentMonth) return 'notReceived';

    // для текущего месяца
    if (item.day < todayDay) {
      // дни до сегодня — либо available (если выполнено/куплено), либо skipped
      return item.canTake ? 'available' : 'skipped';
    }
    if (item.day === todayDay) {
      // сегодня — как обычно
      return item.canTake ? 'available' : 'locked';
    }
    // дни после сегодня внутри текущего месяца — notReceived
    return 'notReceived';
  })();

  // 4) Прогресс фона
  $: progressPct = (item.progress + item.remaining) > 0
    ? item.progress / (item.progress + item.remaining) * 100
    : 0;

  // 5) Покупка пропущенного дня
  function openBuy() {
    if (displayStatus === 'skipped') buyModal = true;
  }
  function closeBuy() {
    buyModal = false;
  }
  function buyDay() {
    mp.trigger('client.calendar.buyDay', item.month, item.day);
    buyModal = false;
  }

  // 6) Взятие награды
  function takeReward() {
    if (displayStatus === 'available') {
      mp.trigger('client.calendar.takeMonthReward', item.month, item.day);
    }
  }
</script>

{#if buyModal}
  <div class="buy-day-modal shown">
    <div class="buy-day-modal__container">
        <div class="buy-day-modal__title">Разблокировать день</div>
  
      <div class="buy-day-modal__text">Покупка разблокирует день и даёт забрать подарок.</div>
      <div class="buy-day-modal__buttons">
        <button class="buy-day-modal__buttons__item" on:click={buyDay}>
          Купить за 250
        </button>
        <button class="buy-day-modal__buttons__item" on:click={closeBuy}>Отменить</button>
      </div>
      <div class="buy-day-modal__info">
        <div class="buy-day-modal__info__title">Информация:</div>
        <div class="buy-day-modal__info__text">
          Разблокировка еженедельных и ежемесячных наград доступна при соблюдении их условий.
        </div>
      </div>
    </div>
  </div>
{/if}

<div class="item {itemColor} {displayStatus}">
  <div class="item-main">
    <div class="item-main__data">
      <div class="item-main__data-line"></div>
      <p>{item.day}</p>
    </div>
    <div class="item-main__image">
      {#if ['available','active'].includes(displayStatus )}
        <div class="item-main__image-background" style="height: {progressPct}%"></div>
      {/if}
      <div class="item-main__image-container">
        <img class="item-main__image-container" src={rewardIcon}  alt="reward" />
      </div>
    </div>
    <div class="item-main__title">{displayTitle}</div>
  </div>

  <div class="item-additional">
   {#if displayStatus  === 'received'}
   <div class="item-additional__circle">
  <svg xmlns="http://www.w3.org/2000/svg" width="21" height="16" viewBox="0 0 21 16" fill="none">
<path d="M18.3103 0.399994L7.99321 10.5447L2.93093 5.56703L0.360352 8.09413L7.99321 15.6L20.8804 2.92709L18.3103 0.399994Z" fill="#FBFBFB"/>
</svg>
  </div>
  
{:else if displayStatus  === 'skipped' && selectedMonth === currentMonth}
  <button  class="item-additional__button" on:click={openBuy}><p>Купить день</p></button>
    {:else if displayStatus  === 'available' && selectedMonth === currentMonth}
  <button  class="item-additional__button" on:click={takeReward}><p>Забрать</p></button>
{:else if displayStatus === 'notReceived'}
      <!-- просто серый значок или пусто, можно подсказать когда станет доступно -->
      
    {/if}

  </div>
</div>
