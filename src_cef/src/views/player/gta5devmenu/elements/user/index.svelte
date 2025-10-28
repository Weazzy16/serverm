<script>
    import { otherStatsData } from 'store/account';
    import { charData } from 'store/chars';
    import { format } from 'api/formatter';
    import { onMount, onDestroy } from 'svelte';
    import moment from 'moment';
    import fraction from 'json/fraction.js';
    import jobs from 'json/jobs.js';
    import vipinfo from 'json/vipinfo.js';
    export let visible;
    import './42112.20ba0f25.css'
   import './Skills.css'
    import { authInfo } from './authInfo.js';

    let onSelectedView2 = "Page1";
    let onSelectedView3 = "Page1";
    let onSelectedView4 = "Page1";
    let onSelectedView5 = "Page1";
    let onSelectedView6 = "Page1";
    let selectCharData = $charData;

    let useVisible = -1;

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
const fuelColors = {
    Regular: '#464646',
    Plus:    '#1a6fc4',
    Premium: '#C14242',
    Diesel:  '#849d63',
    Electro: '#008c8c'
  };
  export let carsList = [];       // массив, где item[10] — это fuelType
  export let document = { cloud: '' };
  export let selectView;
  
  // нормализация в удобный объект
  $: normalized = carsList.map(item => ({
    model:        item[1],
    plate:        item[2],
    price:        Number(item[9]  ?? 0),
    fuel:         String(item[10] ?? 'Regular'),
    engine:       Number(item[11] ?? 0),
    brakes:       Number(item[12] ?? 0),
    transmission: Number(item[13] ?? 0),
    turbo:        Number(item[14] ?? 0),
    km:           Number(item[4]  ?? 0)
  }));
    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }

    let targetDate = new Date(selectCharData.CreateDate);
    let now = new Date();
    let diffInMilliseconds = now - targetDate;

    let diff = {
        years: 0,
        months: 0,
        days: 0,
        hours: 0,
        minutes: 0,
    };

    onMount(() => {
        diff.years = Math.floor(diffInMilliseconds / 31536000000);
        diffInMilliseconds = diffInMilliseconds % 31536000000;

        diff.months = Math.floor(diffInMilliseconds / 2628000000);
        diffInMilliseconds = diffInMilliseconds % 2628000000;

        diff.days = Math.floor(diffInMilliseconds / 86400000);
        diffInMilliseconds = diffInMilliseconds % 86400000;

        diff.hours = Math.floor(diffInMilliseconds / 3600000);
        diffInMilliseconds = diffInMilliseconds % 3600000;

        diff.minutes = Math.floor(diffInMilliseconds / 60000);
    });
    let installedMeta  = [
    { key: 'engine',      icon: 'engine.svg',      label: 'ур.' },
    { key: 'brakes',      icon: 'brakes.svg',      label: 'ур.' },
    { key: 'transmission',icon: 'transmission.svg',label: 'ур.' },
    { key: 'turbo',       icon: 'turbo.svg',       label: 'ур.' }
];
$: sortedCars = carsList
    ? [...carsList].sort((a, b) => (a[0] ?? 0) - (b[0] ?? 0))
    : [];

import './BanHistory.css';

  let isCrimOpen    = false;

  // пример ваших данных

  export let criminalRecords = [
    // { date, officer, reason, duration, end }
  ];
 let banCount = 0;
let isBanOpen  = false;
  let banHistory = [];

function openBanHistory() {
    mp.trigger('requestBanHistory', selectCharData.UUID);
    // … остальное открытие модалки …
  }

  function openBanCount() {
    // отдельный запрос на число банов
    mp.trigger('requestBanCount', selectCharData.UUID);
  }

  function onCountEvent(e) {
    console.log('[Svelte] showBanCount event:', e.detail);
    banCount = e.detail;
  }

  onMount(() => {
    // подписка на приход count
    window.addEventListener('showBanCount', onCountEvent);
    // сразу запрашиваем, чтобы при рендере было не 0
    openBanCount();
  });

  onDestroy(() => {
    window.removeEventListener('showBanCount', onCountEvent);
  });
  function openBan() {
    isBanOpen = true;
    mp.trigger('requestBanHistory', selectCharData.UUID);
  }
  function closeBan() { isBanOpen = false; }
  function backBan(e) { if (e.target === e.currentTarget) closeBan(); }

  // вспомогательная функция для склонений
  function declOfNum(n, titles) {
    return titles[
      (n%10===1 && n%100!==11) ? 0
      : (n%10>=2 && n%10<=4 && (n%100<10||n%100>=20)) ? 1
      : 2
    ];
  }

  // форматируем дату в DD.MM.YYYY
  function formatDate(s) {
    const d = new Date(s);
    const dd = String(d.getDate()).padStart(2,'0');
    const mm = String(d.getMonth()+1).padStart(2,'0');
    const yy = d.getFullYear();
    return `${dd}.${mm}.${yy}`;
  }

  // форматируем длительность
  function formatDuration(start, end) {
    const t0 = new Date(start);
    const t1 = new Date(end);
    const diffMs = t1 - t0;

    const mins  = Math.floor(diffMs / 60000);
    if (mins < 60) {
      const word = declOfNum(mins, ['минута','минуты','минут']);
      return `${mins} ${word}`;
    }

    const hrs = Math.floor(diffMs / 3600000);
    if (hrs < 24) {
      const word = declOfNum(hrs, ['час','часа','часов']);
      return `${hrs} ${word}`;
    }

    const days = Math.floor(diffMs / 86400000);
    // считаем целые месяцы как 30-дневные отрезки
    const months = Math.floor(days / 30);
    if (months >= 2) {
      const word = declOfNum(months, ['месяц','месяца','месяцев']);
      return `${months} ${word}`;
    }

    // иначе показываем дни
    const word = declOfNum(days, ['день','дня','дней']);
    return `${days} ${word}`;
  }

  function onHistoryEvent(e) {
    banHistory = e.detail.map(item => ({
      date:     formatDate(item.Time),
      admin:    item.Admin,
      reason:   item.Reason,
      duration: formatDuration(item.Time, item.Until),
      end:      formatDate(item.Until)
    }));
  }

  onMount(() => {
    window.addEventListener('showBanHistory', onHistoryEvent);
  });
  onDestroy(() => {
    window.removeEventListener('showBanHistory', onHistoryEvent);
  });

  const openCrim   = () => isCrimOpen = true;
  const closeCrim  = () => isCrimOpen = false;
  const backCrim   = e => e.target === e.currentTarget && closeCrim();

   import { speedLimits } from './speedLimits.js';
   const getMaxSpeed = (modelName) =>
    speedLimits[modelName.toLowerCase()] ?? 0;

    import { getVehicleTitle } from '../../../../../json/vehicleTitles.js';
const calcWidth = job =>
    job.nextLevel > 0
      ? Math.min(100, Math.round((job.current / job.nextLevel) * 100))
      : 0;
  let selec1tCharData = {
    jobSkillsInfo: [
      {
        id: 1,
        name: 'Алхимия',
        currentLevel: 2,
        current: 40,
        nextLevel: 100,
        description: 'Дополнительный текст, если есть'
      },
      {
        id: 2,
        name: 'Кузнечное дело',
        currentLevel: 5,
        current: 5,
        nextLevel: 5,
        description: ''
      },
      {
        id: 3,
        name: 'Травничество',
        currentLevel: 1,
        current: 10,
        nextLevel: 50,
        description: 'Текст про травы'
      },
      {
        id: 3,
        name: 'Травничество',
        currentLevel: 1,
        current: 10,
        nextLevel: 50,
        description: 'Текст про травы'
      }
      ,
      {
        id: 3,
        name: 'Травничество',
        currentLevel: 1,
        current: 10,
        nextLevel: 50,
        description: 'Текст про травы'
      }
    ]
  };
  const categories = [
    [
      {
        name: 'Алхимия',
        description: 'Дополнительный текст, если есть',
        rank: 2,
        counter: 40,
        maxCounter: 100
      },
      {
        name: 'Кузнечное дело',
        description: '',
        rank: 5,
        counter: 5,
        maxCounter: 5
      },
      {
        name: 'Травничество',
        description: 'Текст про травы Текст про травы Текст про травы',
        rank: 1,
        counter: 10,
        maxCounter: 50
      },
      {
        name: 'Травничество',
        description: 'Текст про травы',
        rank: 1,
        counter: 10,
        maxCounter: 50
      },
      {
        name: 'Травничество',
        description: 'Текст про травы',
        rank: 1,
        counter: 10,
        maxCounter: 50
      },
      {
        name: 'Травничество',
        description: 'Текст про травы',
        rank: 1,
        counter: 10,
        maxCounter: 50
      }
    ]
  ];

  // Повторяем метод из Vue: проценты прогресса
  function getProgressPercent(job) {
    return Math.round((job.counter / job.maxCounter) * 100);
  }
</script>

{#if selectView === "Osnovnoe"}
    <div class="list1">
        <div class="block">
            <h1>Денежные средства</h1>
            <!-- 
                 Предположим, что вы хотите в <p> отобразить 
                 общую сумму или любую другую нужную информацию.
            -->
            <p>${(selectCharData.BankMoney + selectCharData.Money|| 0).toLocaleString('ru-RU')}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/money.svg" alt=""/>
            <!-- Блок, в котором выводим иконки и суммы -->
            <span class="money-infos">
                <b>
                    <svg class="black-image" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 12.981">
                        <g transform="translate(-1 -1.019)">
                          <path d="M13.5,5H3A1,1,0,0,1,3,3H5a.472.472,0,0,0,.5-.5A.472.472,0,0,0,5,2H3A2.006,2.006,0,0,0,1,4v8a2.006,2.006,0,0,0,2,2H13.5A1.473,1.473,0,0,0,15,12.5v-6A1.473,1.473,0,0,0,13.5,5ZM14,7.9v3.2a1.479,1.479,0,0,0-.5-.1h-2a1.5,1.5,0,0,1,0-3h2A1.479,1.479,0,0,0,14,7.9Z" fill="currentColor"/>
                          <path d="M3.5,3.5A.472.472,0,0,0,3,4a.472.472,0,0,0,.5.5H13a.617.617,0,0,0,.4-.2.486.486,0,0,0,.05-.45l-1-2.5a.52.52,0,0,0-.65-.3L5.4,3.5Z" fill="currentColor"/>
                          <path d="M12.5,9h-1a.5.5,0,0,0,0,1h1a.5.5,0,0,0,0-1Z" fill="currentColor"/>
                        </g>
                      </svg>
                  ${(selectCharData.Money || 0).toLocaleString('ru-RU')}
                </b>
                <b>
                  <!-- Вставляем инлайн SVG для банка -->
                  <svg class="black-image" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 12.6">
                    <path d="M15.417,7.025v7a.7.7,0,0,1-.7.7H2.117a.7.7,0,0,1-.7-.7v-7Zm0-1.4h-14v-2.8a.7.7,0,0,1,.7-.7h12.6a.7.7,0,0,1,.7.7Zm-4.9,5.6v1.4h2.8v-1.4Z" transform="translate(-1.417 -2.125)" fill="currentColor"/>
                  </svg>
                  ${(selectCharData.BankMoney || 0).toLocaleString('ru-RU')}
                </b>
              </span>
              
          </div>
          
        <div class="block">
            <h1>Возраст</h1>
            <p>21 год</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/age.svg" alt=""/>
            <span>Национальность:<b>Нет</b></span>
        </div>
        <div class="block">
            <h1>Место жительства</h1>
            
            {#if selectCharData.HouseId}
        <p>Есть прописка</p>
    {:else}
        <p>Нет прописки</p>
    {/if}
    <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/property.svg" alt=""/>
    <span class="address-info">
        Дом: <b>{selectCharData.HouseId ? '#' + selectCharData.HouseId : 'Нет'}</b>
        <div>Квартира: <b>Нет</b></div>
      </span>
        </div>
        <div class="block">
            <h1>Работа</h1>
            <p>{jobs[selectCharData.WorkID]}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/job.svg" alt=""/>
            <span></span>
        </div>
        
        
        <div class="block">
            <h1>Семейное положение</h1>
            <p>{selectCharData.WeddingName}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/married.svg" alt=""/>
            <span></span>
        </div>
       
        <div class="block">
            <h1>Организация</h1>
            
            <p>{fraction[selectCharData.FractionID]}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/fraction.svg" alt=""/>
            <span>Должность<b>{selectCharData.FractionLVL}</b></span>
        </div>
        
        
        <div class="block">
            <h1>Семья</h1>
            <p>{selectCharData.OrganizationID}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/family.svg" alt=""/>
            <span>Должность<b>{selectCharData.OrganizationLVL}</b></span>
        </div>
        
        <div class="block">
            <h1>Премиум статус</h1>
            <p>{selectCharData.VipLvl > 0 ? `${vipinfo[selectCharData.VipLvl]}` : vipinfo[selectCharData.VipLvl]}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/premSubscription.svg" alt=""/>
            <span>Истикает:<b>{selectCharData.VipLvl > 0 ? `${moment(selectCharData.VipDate).format('DD.MM.YYYY')}` : vipinfo[selectCharData.VipLvl]}</b></span>
        </div>
        
        <div class="block">
            <h1>Payday</h1>
            <p>{selectCharData.Sim == -1 ? "Нет Сим-Карты" : selectCharData.Sim}</p>
            <span></span>
        </div>
        <div class="block">
            <h1>Номер счета</h1>
            <p>XXXXXXXXXX{selectCharData.Bank}</p>
            <span></span>
        </div>
    </div>
{/if}

{#if selectView === "Statiskic"}
    <div class="list1">
        <div class="block">
            <h1>Дата регистрации</h1>
            <p>
                {#if diff.years > 0}
                    {diff.years} года назад
                    {:else if diff.months > 0}
                        {diff.months} месяц(а) назад
                    {:else if diff.days > 0}
                        {diff.days} д. назад
                    {:else if diff.hours > 0}
                        {diff.hours} ч. назад
                    {:else}
                        {diff.minutes} мин. назад
                {/if}
            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/registration.svg" alt=""/>
            <span>Дата регистрации:<b>{moment(selectCharData.CreateDate).format('DD.MM.YYYY HH:mm')}</b></span>
        </div>
        <div class="block">
            <h1>Уровень</h1>
            <p>{selectCharData.LVL}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/level.svg" alt=""/>
            <span>Респекты<b>{selectCharData.EXP} / {(3 + selectCharData.LVL * 3)}</b></span>
        </div>
        <div class="block">
            <h1>Время в игре</h1>
            <p class="timeb2">
                {#if onSelectedView2 === "Page1"}
                    {moment.duration(selectCharData.TodayTime, "minutes").format("h[ч.] m[м.]")}
                {/if}
                {#if onSelectedView2 === "Page2"}
                    {moment.duration(selectCharData.MonthTime, "minutes").format("w[нед.] d[д.] h[ч.] m[м.]")}
                {/if}
                {#if onSelectedView2 === "Page3"}
                    {moment.duration(selectCharData.TotalTime, "minutes").format("y[г.] M[мес.] w[nнед.] d[д.] h[ч.] m[м.]")}
                {/if}
            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/in_game_time.svg" alt=""/>
            <div class="timeblocks1">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView2 = "Page1"} class:active={onSelectedView2 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView2 = "Page2"} class:active={onSelectedView2 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView2 = "Page3"} class:active={onSelectedView2 == "Page3"}>Всего</div>
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Аресты</h1>
            <p>{selectCharData.arrest}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/arrests.svg" alt=""/>
            <span>Количество<b>{selectCharData.arrest > 0 ? `${selectCharData.arrest}` : 0}</b></span>
        </div>
        
        <div class="block">
            <h1>Судимости</h1>
            <p class="timeb2">{criminalRecords.length}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/criminal_records.svg" alt=""/>
            <div class="timeblocks1">
              <span class="timeblocks" on:click={openCrim}>Активные судимости</span>

            </div>
        </div>
        <div class="block">
            <h1>Убийства</h1>
            <p>{selectCharData.Kills > 0 ? `${selectCharData.Kills}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/kills.svg" alt=""/>
            <span>Смерти:<b>{selectCharData.Deaths} </b></span>
        </div>
       <!-- 1) Ваша карточка -->
<div class="block">
  <h1>Блокировки</h1>
  <p class="timeb2">{banCount}</p>
  <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/bans.svg" alt=""/>
   <span class="timeblocks" on:click={openBan}>История блокировок</span>

</div>

{#if isBanOpen}
  <div class="table-container" on:click={backBan}>
    <div class="table-wrapper">
      <div class="title">История блокировок</div>
      <div class="cross" on:click={closeBan}>
        <svg viewBox="0 0 24 24"><path d="M6 6l12 12M6 18L18 6" stroke-width="2" stroke-linecap="round"/></svg>
      </div>

      <div class="description">
        <div class="description__text">
          Информация о блокировках, полученных в оффлайн и онлайн
        </div>
        <div class="description__warns">
          <span class="description__warns-text">Активных предупреждений:</span>
          <span class="description__warns-value">{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0}</span>
          <span class="description__warns-value-add">/ 2</span>
        </div>
      </div>

      <div class="table">
        <div class="grid-item table-header">
          <div class="header-item">Дата</div>
          <div class="header-item">Администратор</div>
          <div class="header-item">Причина</div>
          <div class="header-item">Срок наказания</div>
          <div class="header-item">Дата окончания</div>
        </div>
        <div class="table-content">
          <div class="scroller">
             {#if banHistory.length}
            {#each banHistory as row}
              <div class="grid-item table-item">
                <div class="table-item-data">{row.date}</div>
                <div class="table-item-data">#{row.admin}</div>
                <div class="table-item-data">{row.reason}</div>
                <div class="table-item-data">{row.duration}</div>
                <div class="table-item-data">{row.end}</div>
              </div>
            {/each}
            {:else}
              <div class="not-found">
                <div class="icon">
                  <!-- ваш SVG-иконка -->
                </div>
                <div class="text"> не найдено</div>
              </div>
            {/if}
          </div>
        </div>
      </div>
    </div>
  </div>
{/if}

<!-- попап по судимостям -->
{#if isCrimOpen}
  <div class="table-container" on:click={backCrim}>
    <div class="table-wrapper">
      <div class="title">Активные судимости</div>
      <div class="cross" on:click={closeCrim}>
        <svg viewBox="0 0 24 24"><path d="M6 6l12 12M6 18L18 6" stroke-width="2" stroke-linecap="round"/></svg>
      </div>

      <div class="description">
        <div class="description__text">
          Информация об активных судимостях
        </div>
        <div class="description__warns">
          <span class="description__warns-text">Активных судимостей:</span>
          <span class="description__warns-value">{criminalRecords.length}</span>
        </div>
      </div>

      <div class="table">
        <div class="grid-item table-header">
          <div class="header-item">Дата</div>
          <div class="header-item">Сотрудник</div>
          <div class="header-item">Причина</div>
          <div class="header-item">Срок судимости</div>
          <div class="header-item">Дата окончания</div>
        </div>
        <div class="table-content">
          <div class="scroller">
            {#if criminalRecords.length}
              {#each criminalRecords as row}
                <div class="grid-item table-item">
                  <div class="table-item-data">{row.date}</div>
                  <div class="table-item-data">{row.officer}</div>
                  <div class="table-item-data">{row.reason}</div>
                  <div class="table-item-data">{row.duration}</div>
                  <div class="table-item-data">{row.end}</div>
                </div>
              {/each}
            {:else}
              <div class="not-found">
                <div class="icon">
                  <!-- ваш SVG-иконка -->
                </div>
                <div class="text">Судимостей не найдено</div>
              </div>
            {/if}
          </div>
        </div>
      </div>
    </div>
  </div>
{/if}

        <div class="block">
            <h1>Заработано</h1>
            <p class="timeb2">
                {#if onSelectedView3 == "Page1"}
                    ${(selectCharData.EarnedMoneyDay || 0).toLocaleString('ru-RU')}
                {/if}
                {#if onSelectedView3 == "Page2"}
                    ${(selectCharData.EarnedMoneyMonth || 0).toLocaleString('ru-RU')}
                {/if}
                {#if onSelectedView3 == "Page3"}
                    ${(selectCharData.EarnedMoney || 0).toLocaleString('ru-RU')}
                {/if}
            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/money_gained.svg" alt=""/>
            <div class="timeblocks1">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView3 = "Page1"} class:active={onSelectedView3 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView3 = "Page2"} class:active={onSelectedView3 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView3 = "Page3"} class:active={onSelectedView3 == "Page3"}>Всего</div>
            </span>
        </div>
        </div>
        
        <div class="block">
            <h1>Потрачено</h1>
            <p class="timeb2">
                {#if onSelectedView4 == "Page1"}
    ${(selectCharData.SpentMoneyDay|| 0).toLocaleString('ru-RU')}
{/if}
{#if onSelectedView4 == "Page2"}
    ${(selectCharData.SpentMoneyMonth|| 0).toLocaleString('ru-RU')}
{/if}
{#if onSelectedView4 == "Page3"}
    ${(selectCharData.SpentMoney|| 0).toLocaleString('ru-RU')}
{/if}

            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/money_spent.svg" alt=""/>
            <div class="timeblocks1">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView4 = "Page1"} class:active={onSelectedView4 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView4 = "Page2"} class:active={onSelectedView4 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView4 = "Page3"} class:active={onSelectedView4 == "Page3"}>Всего</div>
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Выигрыш в казино</h1>
            <p class="timeb2">
                {#if onSelectedView5 == "Page1"}
    ${"0"}
{/if}
{#if onSelectedView5 == "Page2"}
${"В разработке"}
{/if}
{#if onSelectedView5 == "Page3"}
${"Амер гей"}
{/if}

            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/money_casino_win.svg" alt=""/>
            <div class="timeblocks2">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView5 = "Page1"} class:active={onSelectedView5 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView5 = "Page2"} class:active={onSelectedView5 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView5 = "Page3"} class:active={onSelectedView5 == "Page3"}>Всего</div>
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Контракты</h1>
            <p class="timeb2">
                {#if onSelectedView6 == "Page1"}
        {"0"}
{/if}
{#if onSelectedView6 == "Page2"}
{"В разработке"}
{/if}


            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/family_contracts.svg" alt=""/>
            <div class="timeblocks2">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView6 = "Page1"} class:active={onSelectedView6 == "Page1"}>Общие</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView6 = "Page2"} class:active={onSelectedView6 == "Page2"}>Личные</div>
                
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Мировых заданий</h1>
            <p class="timeb2">{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/world_quests.svg" alt=""/>
            <span>Получено:<b>{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0} / 3</b></span>
        </div>
        <div class="block">
            <h1>Достижений</h1>
            <p class="timeb2">{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/achievements.svg" alt=""/>
            <span></span>
        </div>
        <div class="block">
            <h1>Введённый промокод</h1>
            <p class="timeb2">{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/promo.svg" alt=""/>
            <span>Личный промокод:<b>{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0} / 3</b></span>
        </div>
    </div>
{/if}

{#if selectView === 'Skills'}
<div class="skills">
  <div class="wrapperss column-block full-width">
    {#each categories as category, categoryIndex}
      <div class="categoryss column-block ">
        <!-- Заголовок категории -->
        <h3>Профессии</h3>

        <!-- Обёртка карточек -->
        <div class="card4-wrapperss column-block full-height">
          {#each category as job}
            <div class="card4">
              <!-- Заголовок карточки -->
              <div class="card4-heading">
                <div class="top">
                  <h6>{job.name}</h6>
                  <!-- Иконка "more" -->
                  <div class="more">
                    <!-- Здесь ваш SVGComponent: 
                    <svg width="16" height="16" viewBox="0 0 16 16">
                      <circle cx="8" cy="8" r="1.5" fill="currentColor"/>
                      <circle cx="4" cy="8" r="1.5" fill="currentColor"/>
                      <circle cx="12" cy="8" r="1.5" fill="currentColor"/>
                    </svg>-->
                  </div>
                </div>
                {#if job.description}
                  <p>{job.description}</p>
                {/if}
              </div>

              <!-- Футер с прогрессом -->
              <div class="card4-footer">
                <div class="progress4">
                  <div class="progress4-counter">
                    {#if job.rank}
                      <!-- Ранг -->
                      <span>{job.rank} ранг</span>
                    {:else}
                      <!-- Процент, если без ранга -->
                      <p>
                        Прогресс:
                        <span>{getProgressPercent(job)}%</span>
                      </p>
                    {/if}

                    <!-- Числовой счётчик -->
                    <div class="counter-progress4">
                      <p>
                        <span>{job.counter}</span>
                        / {job.maxCounter}
                      </p>
                    </div>
                  </div>

                  <!-- Линейный прогресс -->
                  <div class="progress4-bar">
                    <div
                      class="progress4-bar-inner"
                      style="width: {getProgressPercent(job)}%;"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          {/each}
        </div>
      </div>
    {/each}
  </div>
</div>
{/if}

{#if selectView === "Carsuser"}
    <div class="listcars">
        {#each sortedCars  as item}
        {@const price = Number(item[9] ?? 0)}
        {@const fuel = String(item[10] ?? 'Regular')}
      <!-- если в item[i] лежит –1, то (–1 + 1) = 0, а дальше не опустимся ниже 0 -->
{@const brakesLevel       = Math.max((item[12]  ?? -1) + 1, 0)}

  {@const engineLevel       = Math.max((item[11]  ?? -1) + 1, 0)}
  {@const transmissionLevel = Math.max((item[13]  ?? -1) + 1, 0)}
  {@const turboLevel        = Math.max((item[14]  ?? -1) + 1, 0)}

  {@const engineBonus       = engineLevel * 5}
  {@const gearboxBonus      = transmissionLevel * 4}
  {@const turboBonus        = turboLevel * 2}
  
      <!-- 2) считаем 70% от неё и округляем -->
      {@const scrapPrice = Math.round(price * 0.7)}
            <div class="blockcars">
                <div class="headblock">
                    <div class="km-block">
                        <img src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/road.svg" alt="road" class="icon" />
                        <span class="price-text">{item[15]} км</span>
                    </div>
                    
                     <div class="price-wrapper">
        <div class="price-row">
          <img src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/bank.svg" class="icon" alt="Цена"/>
          <span class="price-text">
            {price > 0 ? `$${price.toLocaleString('ru-RU')}` : '—'}
          </span>
        </div>
        <div class="price-row">
          <img
            src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/recycle-bin.svg" class="icon" alt="Свалка"/>
          <span class="price-text">
            {scrapPrice > 0 ? `$${scrapPrice.toLocaleString('ru-RU')}` : '—'}
          </span>
        </div>
        </div>
              <!--  <img src="http://cdn.piecerp.ru/cloud/inventoryItems/vehicle/bdivo.png" alt=""/> -->
                  <img src="http://cdn.piecerp.ru/cloud/inventoryItems/vehicle/{item[1].toLowerCase()}.png" alt=""/>
                </div>
                <h1>{item[1] && getVehicleTitle(item[1].toLowerCase())}</h1>
                <div class="infocar">
                    <p>Рег. номер: <b>{item[2]}</b></p>
                    <div class="fuel">
                        <img class="fuel" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/gas-pump.svg" alt="">
                       <span
    class="fuel"
    style="background-color: {fuelColors[item[10] || 'Premium']};"
  >
    {item[10] || 'Premium'}
  </span>                                    
                    </div>
                    
                </div>
                <div class="lines">
                </div>
                
    <div class="infodop">
  <p>Установленные предметы:</p>
  <div class="dop">
    <div class="dop-item">
      <img
          src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/gpsTracker.svg"
          style="width:20px; height:20px; margin-right:25px;"
          alt="GPS"
        />
      <div class="tooltip"><p>GPS-трекер</p></div>
    </div>
    <div class="dop-item">
       <img
          src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/doorTazer.svg"
          style="width:20px; height:20px; margin-right:25px;"
          alt="Tazer"
        />
      <div class="tooltip"><p>Электрошокер</p></div>
    </div>
    <div class="dop-item">
      <img
          src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/signaling.svg"
          style="width:20px; height:20px; margin-right:25px;"
          alt="Signaling"
        />
      <div class="tooltip"><p>Сигнализация</p></div>
    </div>
  </div>
</div>

    <!-- блок «Установленные предметы» с уровнями -->
    <div class="installed-items">
      <div class="row">
        <!-- Engine -->
        <div class="item2" >
          <img
            src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/engine.svg"
            class="icons"
            alt="Engine"
          />
          <span class="name">{engineLevel} ур.</span>
          <div class="tooltip"><p>Двигатель</p></div>
        </div>
        <!-- Brakes -->
        <div class="item2">
          <img
            src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/brakes.svg"
            class="icons"
            alt="Brakes"
          />
          <span class="name">{brakesLevel} ур.</span>
          <div class="tooltip"><p>Тормоза</p></div>
        </div>
        <!-- Transmission -->
        <div class="item2">
          <img
            src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/transmission.svg"
            class="icons"
            alt="Transmission"
          />
          <span class="name">{transmissionLevel} ур.</span>
          <div class="tooltip"><p>Коробка передач</p></div>
        </div>
      </div>
      <div class="row">
        <!-- Turbo -->
        <div class="item2">
          <img
            src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/turbo.svg"
            class="icons"
            alt="Turbo"
          />
          <span class="name">{turboLevel} ур.</span>
          <div class="tooltip"><p>Турбина</p></div>
        </div>
        <!-- Max Speed -->
        <div class="item2">
          <img
            src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/maxSpeed.svg"
            class="icons"
            alt="Max Speed"
          />
          <span class="name"> км/ч</span>
          <div class="tooltip"><p>Максимальная скорость</p></div>
        </div>
      </div>
    </div>
  </div>  <!-- /blockcars -->
{/each} 
    </div>
{/if}

{#if selectView === "Housebiz"}
  

<div class="property" >

  <div class="wrapper" >
{#if selectCharData.houseId}
    <!-- === Карточка Дома === -->
    <div class="card" >
      <img class="img" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/house/{selectCharData.houseId}.jpg" alt=""/>
      <div class="card-body" >
        <div class="header" >
          <div class="property-info" >
            <p >Собственность</p>
            <h3 >Дом #{selectCharData.houseId}</h3>
            <div class="property-price" >
              <!-- inline-SVG из cost.svg -->
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14">
	<path d="M13.9294 0.0681575C13.885 0.0245144 13.8247 0 13.7619 0C13.6991 0 13.6389 0.0245144 13.5944 0.0681575L12.7151 0.932708H7.1878L0.245199 7.81782C0.0881982 7.97223 0 8.18162 0 8.39995C0 8.61828 0.0881982 8.82767 0.245199 8.98208L5.10251 13.7577C5.18032 13.8345 5.27279 13.8954 5.37461 13.937C5.47643 13.9786 5.5856 14 5.69586 14C5.80611 14 5.91528 13.9786 6.0171 13.937C6.11892 13.8954 6.21139 13.8345 6.2892 13.7577L13.0518 6.92939V1.26288L13.9311 0.398333C13.9531 0.376576 13.9705 0.350762 13.9823 0.322373C13.9941 0.293985 14.0002 0.26358 14 0.232903C13.9998 0.202227 13.9935 0.171882 13.9814 0.143611C13.9693 0.11534 13.9516 0.0896982 13.9294 0.0681575ZM10.4397 4.66592C10.205 4.66592 9.97555 4.59749 9.7804 4.46929C9.58525 4.34109 9.43315 4.15887 9.34333 3.94567C9.25351 3.73248 9.23001 3.49789 9.2758 3.27157C9.32159 3.04524 9.43461 2.83735 9.60057 2.67418C9.76653 2.51101 9.97798 2.39989 10.2082 2.35487C10.4384 2.30985 10.677 2.33296 10.8938 2.42127C11.1107 2.50957 11.296 2.65912 11.4264 2.85098C11.5568 3.04285 11.6264 3.26843 11.6264 3.49919C11.6262 3.80856 11.5011 4.10519 11.2786 4.32395C11.0561 4.54271 10.7544 4.6657 10.4397 4.66592Z"/>
</svg>
              <span class="price" >${selectCharData.housePrice}</span>
            </div>
          </div>
          <div class="dop-info" >
            <div class="weight" >
              <span >{selectCharData.houseWeight}</span>
              <div class="icon-wrapper" > <span>KG</span></div>
            </div>
            <div class="item" >
              <span >{selectCharData.maxcars}</span>
              <img
                  
                  src="http://cdn.piecerp.ru/cloud/img/panelmenu/icon/house/garage.svg"
                  alt="Гараж"
                />
            </div>
            <div class="item" >
              <span >Мастерская</span>
              <img
                  
                  src="http://cdn.piecerp.ru/cloud/img/panelmenu/icon/house/cross-box.svg"
                  alt="Закрыть мастерскую"
                />
            </div>
          </div>
        </div>
        <div class="footer" >
          <button class="gps-icon" >
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="18" viewBox="0 0 14 18">
    <path d="M10.667,1.667a7,7,0,0,0-7,7c0,5.667,7,11,7,11s7-5.333,7-11a7,7,0,0,0-7-7Zm0,10.333A3.333,3.333,0,1,1,14,8.667,3.333,3.333,0,0,1,10.667,12Z" transform="translate(-3.667 -1.667)"/>
</svg>
          </button>
          <div >
            <h3 >{selectCharData.housePosition}</h3>
            <p >Джеймстаун-стрит</p>
          </div>
        </div>
      </div>
    </div>
   {/if}
    {#if selectCharData.BizId}
      <!-- === Карточка Бизнеса (точно такая же разметка, но свои данные) === -->
      <div class="card" >
        <img
          class="img"
          
          src="http://6577f74f-piece.s3.twcstorage.ru/cloud/img/panelmenu/main/biz/ammo_shops/1.jpg"
          alt="Бизнес #{selectCharData.BizId}"
        />
        <div class="card-body" >
          <div class="header" >
            <div class="property-info" >
              <p >Собственность</p>
              <h3 >#{selectCharData.BizId}</h3>
              <div class="property-price" >
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14">
	<path d="M13.9294 0.0681575C13.885 0.0245144 13.8247 0 13.7619 0C13.6991 0 13.6389 0.0245144 13.5944 0.0681575L12.7151 0.932708H7.1878L0.245199 7.81782C0.0881982 7.97223 0 8.18162 0 8.39995C0 8.61828 0.0881982 8.82767 0.245199 8.98208L5.10251 13.7577C5.18032 13.8345 5.27279 13.8954 5.37461 13.937C5.47643 13.9786 5.5856 14 5.69586 14C5.80611 14 5.91528 13.9786 6.0171 13.937C6.11892 13.8954 6.21139 13.8345 6.2892 13.7577L13.0518 6.92939V1.26288L13.9311 0.398333C13.9531 0.376576 13.9705 0.350762 13.9823 0.322373C13.9941 0.293985 14.0002 0.26358 14 0.232903C13.9998 0.202227 13.9935 0.171882 13.9814 0.143611C13.9693 0.11534 13.9516 0.0896982 13.9294 0.0681575ZM10.4397 4.66592C10.205 4.66592 9.97555 4.59749 9.7804 4.46929C9.58525 4.34109 9.43315 4.15887 9.34333 3.94567C9.25351 3.73248 9.23001 3.49789 9.2758 3.27157C9.32159 3.04524 9.43461 2.83735 9.60057 2.67418C9.76653 2.51101 9.97798 2.39989 10.2082 2.35487C10.4384 2.30985 10.677 2.33296 10.8938 2.42127C11.1107 2.50957 11.296 2.65912 11.4264 2.85098C11.5568 3.04285 11.6264 3.26843 11.6264 3.49919C11.6262 3.80856 11.5011 4.10519 11.2786 4.32395C11.0561 4.54271 10.7544 4.6657 10.4397 4.66592Z"/>
</svg>

                <span class="price" >${selectCharData.BizSellPrice}</span>
              </div>
            </div>
            
          </div>
          <div class="footer" >
          <button class="gps-icon" >
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="18" viewBox="0 0 14 18">
    <path d="M10.667,1.667a7,7,0,0,0-7,7c0,5.667,7,11,7,11s7-5.333,7-11a7,7,0,0,0-7-7Zm0,10.333A3.333,3.333,0,1,1,14,8.667,3.333,3.333,0,0,1,10.667,12Z" transform="translate(-3.667 -1.667)"/>
</svg>
          </button>
          <div >
            <h3 >{selectCharData.BizAddress}</h3>
            <p >Джеймстаун-стрит</p>
          </div>
        </div>
        </div>
      </div>
    {/if}

  </div>
</div>

{/if}