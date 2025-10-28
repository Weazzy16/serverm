<script>
  import { onMount } from 'svelte';
  import "./forbs.css";

  // Тестовые данные
  let select = 'players';
  const players = [
    { id: 1, username: 'Mansur Carbone', static: 95, level: 60, biz: 'Los Santos Customs', family: 'Carbone', wealth: 2921394594.56, change: [2.928, 3.213, 3.374, 2.998, 2.141, 2.188, 2.218, 2.303, 2.521, 2.678, 2.768, 2.832] },
    { id: 2, username: 'Oliver Balmain', static: 780, level: 73, biz: null,                family: 'BALMAIN', wealth: 1633059010.28, change: [1.5,1.6,1.7,1.4,1.3,1.2,1.1,1.05,1.0,0.95,0.9,0.85] }
  ];
  const families = [
    { id: 1, name: 'Carbone', tag: 'CARB', leader: 'Mansur Carbone', static: 95, people:100, wealth: 2921394594.56, change: [2.9,32.1,3.4,3.0,2.2,2.2,2.2,2.3,2.5,2.7,2.8,2.8] },
    { id: 2, name: 'BALMAIN', tag: 'BLM', leader: 'Oliver Balmain', static: 780, people:100, wealth: 1633059010.28, change: [1.5,1.6,1.7,1.4,1.3,1.2,1.1,1.05,1,0.95,0.9,0.85] }
  ];

  const monthTitles = {
    1: 'Янв',
  2: 'Фев',
  3: 'Март',
  4: 'Апр',
  5: 'Май',
  6: 'Июнь',
  7: 'Июль',
  8: 'Авг',
  9: 'Сен',
  10: 'Окт',
  11: 'Нояб',
  12: 'Дек'
  };

  // Вычисляем список последних 12 месяцев
  function monthsList() {
    const now = new Date().getMonth() + 1;
    let m = [];
    for(let i=0;i<12;i++) m.unshift(((now-i+11)%12)+1);
    return m;
  }

  $: list = select === 'players' ? players : families;
  $: yearData = select === 'players' ? players[0].change : families[0].change;
  $: maxChange = Math.max(...yearData);

  function formatCurrency(v) {
    return '$' + v.toLocaleString('en-US', { minimumFractionDigits:2, maximumFractionDigits:2 });
  }
</script>


  <!-- Header -->
  <div class="forbes-header row-block justify-between align-center">
   
    <div class="tabs row-block">
      <button class="tab row-block align-center justify-start" class:selected={select==='players'} on:click={() => select='players'}><svg class:selected={select==='players'} class="people" xmlns="http://www.w3.org/2000/svg" width="17" height="13" viewBox="0 0 17 13" fill="none">
<path fill-rule="evenodd" clip-rule="evenodd" d="M5.40577 4.70615C5.40577 5.66781 4.62222 6.44904 3.65855 6.44904C2.69487 6.44904 1.91508 5.66747 1.91508 4.70615C1.91508 3.74448 2.69487 2.96292 3.65855 2.96292C4.62222 2.96292 5.40577 3.74448 5.40577 4.70615Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M5.86999 3.29072C5.86999 1.83545 7.04972 0.655719 8.50499 0.655719C9.96026 0.655719 11.14 1.83545 11.14 3.29072C11.14 4.74599 9.96026 5.92572 8.50499 5.92572C7.04972 5.92572 5.86999 4.74599 5.86999 3.29072Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M13.3399 2.96292C14.3008 2.96292 15.0816 3.74448 15.0816 4.70615C15.0816 5.66781 14.3008 6.44904 13.3399 6.44904C12.3791 6.44904 11.6016 5.66747 11.6016 4.70615C11.6016 3.74448 12.3791 2.96292 13.3399 2.96292Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M17 8.85968V10.0093C17 10.1521 16.9116 10.2813 16.779 10.3282C16.0584 10.5904 15.3132 10.7668 14.5514 10.8593V10.8525V9.24118C14.5552 8.44894 14.2597 7.68764 13.7284 7.09873C14.0413 7.04841 14.3406 6.93587 14.6096 6.76892C15.6635 7.02019 16.9932 7.59856 17 8.85968Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M13.8741 9.24244V10.8527C13.8741 11.1384 13.6938 11.3967 13.4255 11.4915C10.2423 12.6331 6.75885 12.6331 3.57571 11.4915C3.30702 11.3967 3.12676 11.1384 3.12676 10.8527V9.24244C3.13016 7.3366 5.15045 6.46676 6.73879 6.09572C7.81696 6.77528 9.18762 6.77188 10.2658 6.09572C11.8535 6.47287 13.8669 7.33218 13.8741 9.24244Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M3.2716 7.09876C2.74107 7.68704 2.4486 8.44908 2.4486 9.24138V10.8529V10.86C1.69021 10.7678 0.94203 10.5917 0.224455 10.3292C0.0884216 10.2816 0 10.1524 0 10.0095V8.85985C0.00340083 7.59863 1.33993 7.02021 2.39078 6.76892C2.65945 6.9352 2.95872 7.04742 3.2716 7.09876Z"/>
</svg><span class:selected={select==='players'}>Люди</span></button>
      <button class="tab row-block align-center justify-start" class:selected={select==='families'} on:click={() => select='families'}>
        <svg class:selected={select==='families'} class="families" xmlns="http://www.w3.org/2000/svg" width="14" height="16" viewBox="0 0 14 16" fill="none">
<path fill-rule="evenodd" clip-rule="evenodd" d="M5.96775 15.7419V13.0323C5.96775 12.8897 6.0833 12.7742 6.22581 12.7742H7.7742C7.91672 12.7742 8.03227 12.8897 8.03227 13.0323V15.7419C8.03227 15.8845 7.91672 16 7.7742 16H6.22581C6.08329 16 5.96775 15.8845 5.96775 15.7419ZM8.80646 4.87097H8.22581C7.94075 4.87097 7.70968 4.63987 7.70968 4.35484C7.70968 4.06981 7.94074 3.83871 8.22581 3.83871H8.80646C9.09152 3.83871 9.32259 4.06981 9.32259 4.35484C9.32259 4.63987 9.09153 4.87097 8.80646 4.87097ZM8.80646 6.80645H8.22581C7.94075 6.80645 7.70968 6.57535 7.70968 6.29032C7.70968 6.00529 7.94074 5.77419 8.22581 5.77419H8.80646C9.09152 5.77419 9.32259 6.00529 9.32259 6.29032C9.32259 6.57535 9.09153 6.80645 8.80646 6.80645ZM8.80646 8.74193H8.22581C7.94075 8.74193 7.70968 8.51083 7.70968 8.2258C7.70968 7.94077 7.94074 7.70967 8.22581 7.70967H8.80646C9.09152 7.70967 9.32259 7.94077 9.32259 8.2258C9.32259 8.51083 9.09153 8.74193 8.80646 8.74193ZM8.80646 10.6774H8.22581C7.94075 10.6774 7.70968 10.4463 7.70968 10.1613C7.70968 9.87626 7.94074 9.64516 8.22581 9.64516H8.80646C9.09152 9.64516 9.32259 9.87626 9.32259 10.1613C9.32259 10.4463 9.09153 10.6774 8.80646 10.6774ZM5.74195 4.87097H5.1613C4.87627 4.87097 4.64517 4.63987 4.64517 4.35484C4.64517 4.06981 4.87627 3.83871 5.1613 3.83871H5.74195C6.02698 3.83871 6.25808 4.06981 6.25808 4.35484C6.25808 4.63987 6.02698 4.87097 5.74195 4.87097ZM5.74195 6.80645H5.1613C4.87627 6.80645 4.64517 6.57535 4.64517 6.29032C4.64517 6.00529 4.87627 5.77419 5.1613 5.77419H5.74195C6.02698 5.77419 6.25808 6.00529 6.25808 6.29032C6.25808 6.57535 6.02698 6.80645 5.74195 6.80645ZM5.74195 8.74193H5.1613C4.87627 8.74193 4.64517 8.51083 4.64517 8.2258C4.64517 7.94077 4.87627 7.70967 5.1613 7.70967H5.74195C6.02698 7.70967 6.25808 7.94077 6.25808 8.2258C6.25808 8.51083 6.02698 8.74193 5.74195 8.74193ZM5.74195 10.6774H5.1613C4.87627 10.6774 4.64517 10.4463 4.64517 10.1613C4.64517 9.87626 4.87627 9.64516 5.1613 9.64516H5.74195C6.02698 9.64516 6.25808 9.87626 6.25808 10.1613C6.25808 10.4463 6.02698 10.6774 5.74195 10.6774ZM12.9855 14.9677H11.9678V2.32258C11.9678 2.03752 11.7367 1.80645 11.4516 1.80645H7.58066V0.51613C7.58066 0.23107 7.3496 0 7.06453 0H3.48388C3.19882 0 2.96775 0.23106 2.96775 0.51613V1.80645H2.5484C2.26334 1.80645 2.03227 2.03751 2.03227 2.32258V14.9677H1.0145C0.736497 14.9677 0.497397 15.1815 0.484467 15.4592C0.470657 15.7556 0.706657 16 1.00002 16H4.67744C4.81996 16 4.9355 15.8844 4.9355 15.7419V12.7742H4.72427C4.44637 12.7742 4.20724 12.5606 4.19414 12.283C4.18017 11.9866 4.41637 11.7419 4.70969 11.7419C4.86553 11.7419 9.40469 11.7419 9.27575 11.7419C9.55365 11.7419 9.79278 11.9555 9.80588 12.2331C9.81985 12.5295 9.58369 12.7742 9.29033 12.7742H9.06452V15.7419C9.06452 15.8844 9.18007 16 9.32258 16H13C13.2933 16 13.5293 15.7556 13.5155 15.4592C13.5026 15.1815 13.2635 14.9677 12.9855 14.9677H12.9855Z"/>
</svg>
        <span class:selected={select==='families'}>Организации</span></button>
    </div>
  </div>
<div class="forbes full-width full-height column-block">
  <!-- Main info -->
  <div class="info-wrapper align-start justify-start row-block full-width">
    <div class="selectedItem full-height">
      <div class="avatar align-start justify-start column-block">
        <img class="img" src="https://cdn.majestic-files.com/public/master/static/img/avatars/male.svg" alt="">
        <span class="top align-center justify-center">TOP 1</span>
      </div>
      <div class="info align-start justify-start column-block">
        {#if select==='players'}
          <div class="name-static-tag align-center justify-start row-block">
            <span class="name">{players[0].username}</span>
            <span class="static">#{players[0].static}</span>
          </div>
          <div class="level align-center justify-start row-block">
            <i class="icon-level"></i>
            <span class="value">{players[0].level}</span>
            <span class="title"> УРОВЕНЬ</span>
          </div>
          <div class="business align-center justify-start row-block">
            <i class="icon-business"></i><span class="title">{players[0].biz || '—'}</span>
          </div>
          <div class="family align-center justify-start row-block">
            <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="11" height="13" viewBox="0 0 11 13" fill="none">
<path fill-rule="evenodd" clip-rule="evenodd" d="M4.66129 12.7903V10.5887C4.66129 10.4729 4.75517 10.379 4.87097 10.379H6.12903C6.24482 10.379 6.33871 10.4729 6.33871 10.5887V12.7903C6.33871 12.9061 6.24483 13 6.12903 13H4.87097C4.75518 13 4.66129 12.9061 4.66129 12.7903ZM6.96774 3.95767H6.49597C6.26435 3.95767 6.07662 3.7699 6.07662 3.53832C6.07662 3.30673 6.26436 3.11897 6.49597 3.11897H6.96774C7.19936 3.11897 7.38709 3.30674 7.38709 3.53832C7.38709 3.76991 7.19935 3.95767 6.96774 3.95767ZM6.96774 5.53025H6.49597C6.26435 5.53025 6.07662 5.34248 6.07662 5.1109C6.07662 4.87931 6.26436 4.69155 6.49597 4.69155H6.96774C7.19936 4.69155 7.38709 4.87932 7.38709 5.1109C7.38709 5.34249 7.19935 5.53025 6.96774 5.53025ZM6.96774 7.10283H6.49597C6.26435 7.10283 6.07662 6.91506 6.07662 6.68348C6.07662 6.45189 6.26436 6.26413 6.49597 6.26413H6.96774C7.19936 6.26413 7.38709 6.4519 7.38709 6.68348C7.38709 6.91507 7.19935 7.10283 6.96774 7.10283ZM6.96774 8.67541H6.49597C6.26435 8.67541 6.07662 8.48764 6.07662 8.25606C6.07662 8.02447 6.26436 7.83671 6.49597 7.83671H6.96774C7.19936 7.83671 7.38709 8.02448 7.38709 8.25606C7.38709 8.48765 7.19935 8.67541 6.96774 8.67541ZM4.47782 3.95767H4.00605C3.77446 3.95767 3.5867 3.7699 3.5867 3.53832C3.5867 3.30673 3.77447 3.11897 4.00605 3.11897H4.47782C4.70941 3.11897 4.89717 3.30674 4.89717 3.53832C4.89717 3.76991 4.7094 3.95767 4.47782 3.95767ZM4.47782 5.53025H4.00605C3.77446 5.53025 3.5867 5.34248 3.5867 5.1109C3.5867 4.87931 3.77447 4.69155 4.00605 4.69155H4.47782C4.70941 4.69155 4.89717 4.87932 4.89717 5.1109C4.89717 5.34249 4.7094 5.53025 4.47782 5.53025ZM4.47782 7.10283H4.00605C3.77446 7.10283 3.5867 6.91506 3.5867 6.68348C3.5867 6.45189 3.77447 6.26413 4.00605 6.26413H4.47782C4.70941 6.26413 4.89717 6.4519 4.89717 6.68348C4.89717 6.91507 4.7094 7.10283 4.47782 7.10283ZM4.47782 8.67541H4.00605C3.77446 8.67541 3.5867 8.48764 3.5867 8.25606C3.5867 8.02447 3.77447 7.83671 4.00605 7.83671H4.47782C4.70941 7.83671 4.89717 8.02448 4.89717 8.25606C4.89717 8.48765 4.7094 8.67541 4.47782 8.67541ZM10.3632 12.1613H9.53629V1.88711C9.53629 1.6555 9.34855 1.46776 9.11693 1.46776H5.97177V0.419365C5.97177 0.187755 5.78403 1.52588e-05 5.55242 1.52588e-05H2.64315C2.41153 1.52588e-05 2.2238 0.187755 2.2238 0.419365V1.46776H1.88307C1.65146 1.46776 1.46372 1.6555 1.46372 1.88711V12.1613H0.636778C0.410908 12.1613 0.216638 12.335 0.206128 12.5606C0.194908 12.8014 0.386658 13 0.625008 13H3.61291C3.7287 13 3.82259 12.9061 3.82259 12.7903V10.379H3.65097C3.42517 10.379 3.23088 10.2055 3.22024 9.97997C3.20889 9.73916 3.4008 9.54033 3.63912 9.54033C3.76574 9.54033 7.45381 9.54033 7.34905 9.54033C7.57485 9.54033 7.76914 9.71386 7.77978 9.9394C7.79113 10.1802 7.59925 10.379 7.3609 10.379H7.17743V12.7903C7.17743 12.9061 7.27131 13 7.38711 13H10.375C10.6134 13 10.8051 12.8014 10.7939 12.5606C10.7834 12.335 10.5891 12.1613 10.3632 12.1613H10.3632Z"/>
</svg>
<span class="title">{players[0].family}</span>
          </div>
          <div class="money"><span class="amount">{formatCurrency(players[0].wealth)}</span><svg class="icon unchanged" xmlns="http://www.w3.org/2000/svg" width="10" height="6" viewBox="0 0 10 6" fill="none">
<g opacity="0.2">
<path fill-rule="evenodd" clip-rule="evenodd" d="M1 2C0.447715 2 0 1.55228 0 1C0 0.447715 0.447715 0 1 0H9C9.55228 0 10 0.447715 10 1C10 1.55228 9.55229 2 9 2H1Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M1 6C0.447715 6 0 5.55228 0 5C0 4.44772 0.447715 4 1 4H9C9.55228 4 10 4.44772 10 5C10 5.55228 9.55229 6 9 6H1Z"/>
</g>
</svg></div>
        {:else}
          <div class="name-static-tag align-center justify-start row-block">
            <span class="name">{families[0].leader}</span>
            <span class="static">#{families[0].static}</span> <div class="tag align-center justify-center">{families[0].tag}</div>
          </div>
         <div class="membersf align-center justify-start row-block">
            <span class="value">{families[0].people}</span><span class="title">Участников</span>
         </div>
          <div class="leader align-center justify-start row-block">
            <i class="icon-leader"></i><svg class="icon" xmlns="http://www.w3.org/2000/svg" width="13" height="11" viewBox="0 0 13 11" fill="none">
<path fill-rule="evenodd" clip-rule="evenodd" d="M11.8374 8.75113L11.5991 9.78623C11.5503 9.99744 11.3626 10.1468 11.1461 10.1468H1.85439C1.63789 10.1468 1.45019 9.99744 1.40141 9.78623L1.16308 8.75113H11.8374Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M12.9884 3.74737L12.0482 7.82402H0.952091L0.01194 3.74737C-0.02938 3.56759 0.03933 3.37989 0.18744 3.26979C0.33601 3.15969 0.53518 3.14807 0.694891 3.23959L3.56828 4.8828L6.11343 1.0626C6.19654 0.938085 6.33396 0.860975 6.483 0.855392C6.63342 0.848887 6.77502 0.916711 6.86649 1.03472L9.85131 4.87491L12.2781 3.25632C12.4364 3.15132 12.643 3.15226 12.7994 3.26051C12.9564 3.36876 13.0307 3.56201 12.9884 3.74738L12.9884 3.74737Z"/>
</svg><span class="name">{families[0].leader}</span><span class="static">#{families[0].static}</span>
          </div>
          <div class="money"><span class="amount">{formatCurrency(families[0].wealth)}</span><svg class="icon unchanged" xmlns="http://www.w3.org/2000/svg" width="10" height="6" viewBox="0 0 10 6" fill="none">
<g opacity="0.2">
<path fill-rule="evenodd" clip-rule="evenodd" d="M1 2C0.447715 2 0 1.55228 0 1C0 0.447715 0.447715 0 1 0H9C9.55228 0 10 0.447715 10 1C10 1.55228 9.55229 2 9 2H1Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M1 6C0.447715 6 0 5.55228 0 5C0 4.44772 0.447715 4 1 4H9C9.55228 4 10 4.44772 10 5C10 5.55228 9.55229 6 9 6H1Z"/>
</g>
</svg></div>
        {/if}
      </div>
    </div>

    <!-- Chart -->
    <div class="changeForYear full-height align-center justify-start column-block">
      <span class="title">Изменение состояния за год</span>
      <div class="monthWrapper align-start justify-beetwen row-block">
        {#each yearData as val, idx}
          <div class="month align-center justify-end column-block">
            <div class="column align-center justify-end column-block">
                
            <div class="value" style="height:{val/maxChange*100}%"><div class="amount">{val}B</div></div>
            <div class="monthTitle">{monthTitles[monthsList()[idx]]}</div>
          </div>
          </div>
        {/each}
      </div>
    </div>
  </div>

  <!-- Table -->
  <div class="table align-start justify-start column-block full-width full-height">
    <!-- Header row -->
    {#if select==='players'}
    <div class="header player align-center justify-start row-block full-width">
      <span class="header__title title1">#</span>
      <span class="header__title title2">Имя фамилия</span>
      <span class="header__title title3">Static</span>
      <span class="header__title title4">Ур.</span>
      <span class="header__title title5">Бизнес во владении</span>
      <span class="header__title title6">Название семьи</span>
      <span class="header__title title7">Состояние</span>
      <span class="header__title title8">Изменение</span>
    </div>
    {:else}
    <div class="header families row-block align-center full-width">
      <span class="header__title title1">#</span>
      <span class="header__title title2">Название организации</span>
      <span class="header__title title3">Тэг</span>
      <span class="header__title title4">Лидер организации</span>
      <span class="header__title title5">Static</span>
      <span class="header__title title6">Состояние</span>
      <span class="header__title title7">Изменение</span>
    </div>
    {/if}

    <!-- Items -->
    <div class="itemsWrapper align-start justify-start column-block full-width">
      {#each list as item, i}
        <div class="item {select} align-center justify-start row-block full-width" key={item.id}>
          <span class="item__title title1">{i+1}</span>
          {#if select==='players'}
            <span class="item__title title2">{item.username}</span>
            <span class="item__title title3 dimmer">{item.static}</span>
            <span class="item__title title4">{item.level}</span>
            <span class="item__title title5">{item.biz||'—'}</span>
            <span class="item__title title6">{item.family}</span>
            <span class="item__title title7">{formatCurrency(item.wealth)}</span>
          {:else}
            <span class="item__title title2">{item.name}</span>
            <span class="item__title title3 dimmer">{item.tag}</span>
            <span class="item__title title4">{item.leader}</span>
            <span class="item__title title5 dimmer">{item.static}</span>
            <span class="item__title title6">{formatCurrency(item.wealth)}</span>
          {/if}
          <span class="item__title title8 align-center justify-center">
            {#if item.prevId !== undefined}
              <i class="item__title__icon {item.id<item.prevId?'positive':item.id>item.prevId?'negative':'unchanged'}"></i>
              {Math.abs(item.prevId-item.id)}
            {/if}
          </span>
        </div>
      {/each}
    </div>
  </div>
</div>
