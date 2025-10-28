<script>
  import { onMount } from 'svelte';

  // Пропсы из родительского компонента
  export let search;
  export let togglebackbutton;
import "./tran.css"
  // Данные пополнений и расходов
  let payments = [];
  let purchases = [];

  // Опции фильтра для пополнений и расходов
  let payFilters = ['Дата и время', 'Сумма оплаты', 'Зачислено', 'Комментарий'];
  let payFilter = 'Зачислено';
  let payDropdown = false;
  const selectPayFilter = (f) => {
    payFilter = f;
    payDropdown = false;
  };

  let expenseFilters = ['Дата и время', 'Сумма', 'Комментарий'];
  let expenseFilter = 'Выберите фильтр';
  let expenseDropdown = false;
  const selectExpenseFilter = (f) => {
    expenseFilter = f;
    expenseDropdown = false;
  };

  onMount(() => {
    payments = [
      { id: 1, date: '04.08.2025 09:15:30', payAmount: 50, credited: 50, currency: 'MC', rub: 'р.', comment: 'Пополнение через карту' },
      { id: 3, date: '02.08.2025 14:22:05', payAmount: 75, credited: 75, currency: 'MC', rub: 'р.', comment: 'Пополнение через WebMoney' }
    ];

    purchases = [
      { id: 1, date: '04.08.2025 11:00:00', amount: 30, currency: 'MC', rub: 'р.', comment: 'Покупка скин-паки' },
      { id: 2, date: '03.08.2025 16:30:20', amount: 45, currency: 'MC', rub: 'р.', comment: 'Покупка боевого пропуска' }
    ];
  });
</script>

<div class="donate full-width full-height">
  <div
    class="detailing full-width full-height row-block justify-between"
    {search}
    on:click={togglebackbutton}
  >
    <!-- Пополнения -->
    <div class="replenishments full-height column-block">
      <header class="full-width row-block align-center justify-between">
        <span class="detailing__title">Пополнения</span>

        <div class="filter-container row-block full-width justify-end">
          <div class="search row-block align-center">
            <svg class="search__picture " xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="16" height="16" viewBox="0 0 16 16">
  <path d="M8.818,932.149a7.177,7.177,0,1,0,4.469,12.779l2.988,2.987a.8.8,0,0,0,1.131-1.129L14.417,943.8a7.165,7.165,0,0,0-5.6-11.649Zm0,1.594a5.582,5.582,0,1,1-5.579,5.579,5.566,5.566,0,0,1,5.579-5.579Z" transform="translate(-1.645 -932.149)"/>
</svg>
            <input type="text" class="search__input" placeholder="Поиск" bind:value={search} />
          </div>

          <div class="main-select detailing-select" style="position: relative;" on:click|stopPropagation={() => payDropdown = !payDropdown}>
            <div class="main-option">
              <span>{payFilter}</span>
              <svg xmlns="http://www.w3.org/2000/svg" class="drop-arrow" width="15" height="7.5" viewBox="0 0 15 7.5"  style="transform: rotate({payDropdown ? 0 : 180}deg); transition: transform 0.2s;">
    <path d="M7.5,0,15,7.5H0Z"/>
</svg>
            </div>
            
            {#if payDropdown}
              <div class="list-select-main">
                {#each payFilters as f}
                <div class="list-select-main-wrapper">
                  <div class="list-item" on:click|stopPropagation={() => selectPayFilter(f)}>{f}</div></div>
                {/each}
              </div>
            {/if}
          </div>
        </div>
      </header>

      <div class="table replenishments-table column-block full-width full-height">
        <div class="table-header row-block align-center">
          <div class="header-item">Дата и время</div>
          <div class="header-item">Сумма оплаты</div>
          <div class="header-item">Зачислено</div>
          <div class="header-item">Комментарий</div>
        </div>
<div class="vue-recycle-scroller ready direction-vertical table-body full-height">
        {#each payments as p}
        
          <div class="row-block align-center table-row full-width">
            <div class="row-item">{p.date}</div>
            <div class="row-item">{p.payAmount} {p.rub}</div>
            <div class="row-item highlighted">{p.credited} {p.currency}</div>
            <div class="row-item comment">{p.comment}</div>
          </div>
        
          {/each}
            </div>
      </div>
    </div>

    <!-- Расходы -->
    <div class="expenses full-height column-block">
      <header class="full-width row-block align-center justify-between">
        <span class="detailing__title">Расходы</span>

        <div class="main-select detailing-select" style="position: relative;" on:click|stopPropagation={() => expenseDropdown = !expenseDropdown}>
          <div class="main-option">
            <span>{expenseFilter}</span>
            <svg xmlns="http://www.w3.org/2000/svg" class="drop-arrow" width="15" height="7.5" viewBox="0 0 15 7.5"  style="transform: rotate({expenseDropdown ? 0 : 180}deg); transition: transform 0.2s;">
    <path d="M7.5,0,15,7.5H0Z"/>
</svg>
          </div>
          {#if expenseDropdown}
            <div class="list-select-main">
              {#each expenseFilters as f}
              <div class="list-select-main-wrapper">
                <div class="list-item" on:click|stopPropagation={() => selectExpenseFilter(f)}>{f}</div></div>
              {/each}
            </div>
          {/if}
        </div>
      </header>

      <div class="table expenses-table column-block full-width full-height">
        <div class="table-header row-block align-center">
          <div class="header-item">Дата и время</div>
          <div class="header-item">Сумма</div>
          <div class="header-item">Комментарий</div>
        </div>
<div class="vue-recycle-scroller ready direction-vertical table-body full-height">
        {#each purchases as p}
          <div class="row-block align-center table-row full-width">
            <div class="row-item">{p.date}</div>
            <div class="row-item highlighted amount">{p.amount} {p.currency}</div>
            <div class="row-item comment">{p.comment}</div>
          </div>
        {/each}
        </div>
      </div>
    </div>
  </div>
</div>
