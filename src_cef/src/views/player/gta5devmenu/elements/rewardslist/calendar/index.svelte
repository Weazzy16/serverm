<script>
  import { onMount, onDestroy } from 'svelte';
  import CalendarItem from './CalendarItem.svelte';
  import './calendar.css';

  export let months = [
    'январь','февраль','март','апрель',
    'май','июнь','июль','август',
    'сентябрь','октябрь','ноябрь','декабрь'
  ];

  // системный «сегодня»
  const now = new Date();
  let currentMonth = now.getUTCMonth(); // 0–11
  let todayDay    = now.getUTCDate();   // 1–31

  // UI state
  let selectedMonth = currentMonth;
  let feedItems = [];

  function handleMonthUpdate(e) {
    try {
      feedItems = JSON.parse(e.detail);
    } catch {
      feedItems = [];
    }
  }

  function requestMonth(i) {
    mp.trigger('client.calendar.requestMonth', i + 1);
  }

  function selectMonth(i) {
    selectedMonth = i;
    requestMonth(i);
  }

  onMount(() => {
    window.addEventListener('item_calendar_update', handleMonthUpdate);
    requestMonth(currentMonth);
  });

  onDestroy(() => {
    window.removeEventListener('item_calendar_update', handleMonthUpdate);
  });
</script>

<div data-v-5929cd6e class="menuWrapper-main">
  <div class="content">
    <div class="content-bar">
      {#each months as name,i}
      <button
        class="content-bar__item
               {i < currentMonth  ? 'finished':''}
               {i === selectedMonth ? 'active':''}
               {i === currentMonth  ? 'current':''}"
        disabled={i > currentMonth}
        on:click={() => selectMonth(i)}
      >
          <p>{name}</p>
         <svg data-v-626370ae="" data-v-5929cd6e-s="" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="none"><mask data-v-626370ae="" data-v-5929cd6e-s="" id="mask0_601_352" maskUnits="userSpaceOnUse" x="0" y="0" width="16" height="16" style="mask-type: luminance;"><path data-v-626370ae="" data-v-5929cd6e-s="" d="M16 0H0V16H16V0Z" fill="white"></path></mask><g data-v-626370ae="" data-v-5929cd6e-s="" mask="url(#mask0_601_352)"><path data-v-626370ae="" data-v-5929cd6e-s="" d="M7.99965 16C6.51912 15.9944 5.10131 15.4015 4.05741 14.3516C3.01351 13.3017 2.42881 11.8805 2.43165 10.4C2.47322 9.22796 2.81379 8.08602 3.42098 7.08267C3.75372 6.44385 4.04672 5.78512 4.29832 5.11022L4.59076 4.30311L5.3081 4.48089L5.19698 5.33333C5.11111 6.07173 5.23076 6.81949 5.54276 7.49422C5.66608 7.76634 5.82478 8.02099 6.01476 8.25156C5.94888 7.1572 6.06574 6.05951 6.36054 5.00356C6.66223 3.93531 7.15888 2.93207 7.82543 2.04444C8.35892 1.2118 9.12949 0.557943 10.0379 0.167111L11.2841 0L10.4459 0.936889C10.0649 1.36188 9.85466 1.91281 9.85565 2.48356C9.85565 3.58844 10.5339 4.37333 11.3188 5.28089C11.8965 5.9058 12.4014 6.59437 12.8237 7.33333C13.3251 8.27721 13.5806 9.33219 13.5668 10.4009C13.5694 11.8811 12.9847 13.3019 11.941 14.3516C10.8973 15.4013 9.47988 15.9941 7.99965 16Z"></path></g></svg>
        </button>
      {/each}
    </div>

    <div class="content-main">
      <div class="content-main__list">
        <div class="content-main__list-scrollable">
        {#if feedItems.length}
      {#each feedItems as item (item.day)}
        <CalendarItem
          {item}
          currentMonth={currentMonth}
          selectedMonth={selectedMonth}
          todayDay={todayDay}
          on:refresh={() => requestMonth(selectedMonth)}
        />
      {/each}
    {:else}
      <p class="empty-list">Нет данных для этого месяца</p>
    {/if}
        </div>
      </div>
    </div>
  </div>
</div>

<style>
  .empty-list {
    color: #888;
    text-align: center;
    margin: 2rem;
  }
</style>
