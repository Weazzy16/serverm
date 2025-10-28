<script>
  import { onMount } from 'svelte';
  import { TimeFormat } from 'api/moment';
  import { serverDateTime } from 'store/server';
  import { format } from 'api/formatter';
  import { fly } from 'svelte/transition';
  import { charMoney, charBankMoney } from 'store/chars';
  import { tweened } from 'svelte/motion';
  import { cubicOut } from 'svelte/easing';
    import { mapWidth } from 'store/map'; // ✅ ДОБАВЛЕНО

  export let SafeSone = { x: 0, y: 0 };
  
  let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  
  // Погода (будет обновляться из старого placetime)
  let weather = { temperature: 20, dayType: 'day', weatherType: 'CLEAR' };
  let weatherTypeComputed = 'CLEAR';
  
  // Время (берём из serverDateTime)
  $: time = TimeFormat($serverDateTime, "H:mm");
  $: formattedDate = TimeFormat($serverDateTime, "DD.MM.YYYY");
  $: weekday = TimeFormat($serverDateTime, 'dddd')[0].toUpperCase() + TimeFormat($serverDateTime, 'dddd').slice(1);
  
  // Голос (будет обновляться)
  let talking = false;
  let talkingImg = 'microphone';
  let muteStatus = false;
  let speakingLanguage = null;
  
  // Деньги (анимация как в старом placetime)
  const money = tweened(0, {
    duration: 700,
    easing: cubicOut
  });
  
  const bank = tweened(0, {
    duration: 700,
    easing: cubicOut
  });
  
  let cash = 0;
  let bankData = { id: null, amount: 0 };
  let showCasinoChips = false;
  let casinoChipsBalance = 0;
  
  let moneyChangeDirection = null;
  let bankChangeDirection = null;
  let previousMoney = 0;
  let previousBank = 0;
  
  // Ошибки
  let errors = [];
  
  // Переводы погоды
  const weatherTranslations = {
    extrasunny: "Солнечно",
    clear: "Ясно",
    clouds: "Облачно",
    smog: "Смог",
    foggy: "Туманно",
    overcast: "Пасмурно",
    rain: "Дождь",
    thunder: "Гроза",
    clearing: "Легкий дождь",
    neutral: "Дождь с туманом",
    snow: "Снег",
    blizzard: "Метель",
    snowlight: "Легкий снегопад",
    xmas: "Снегопад",
    halloween: "Пасмурно"
  };
  
  // Маппинг погоды
  const weatherMap = {
    EXTRASUNNY: { day: '002-sunny', night: '020-moon' },
    CLEAR: { day: '002-sunny', night: '049-moon' },
    CLOUDS: { day: '016-cloudy', night: '004-cloudy' },
    SMOG: { day: '016-cloudy', night: '004-cloudy' },
    FOGGY: { day: '021-fog', night: '004-cloudy' },
    OVERCAST: { day: '032-rainfall', night: '036-night' },
    RAIN: { day: '032-rainfall', night: '028-rainfall' },
    THUNDER: { day: '035-thunderstorm', night: '031-thunderstorm' },
    CLEARING: { day: '032-rainfall', night: '036-night' },
    NEUTRAL: { day: '032-rainfall', night: '036-night' },
    SNOW: { day: '027-snow', night: '027-snow' },
    BLIZZARD: { day: '014-blizzard', night: '040-blizzard' },
    SNOWLIGHT: { day: '027-snow', night: '027-snow' },
    XMAS: { day: '023-snowing', night: '023-snowing' },
    HALLOWEEN: { day: '035-thunderstorm', night: '031-thunderstorm' }
  };
  
  // Подключаем обновления погоды из старого placetime
  if (typeof window !== 'undefined') {
    window.hudStore = window.hudStore || {};
    
    // Погода (из старого кода)
    window.hudStore.weatherTemp = (temp) => {
      weather.temperature = parseFloat(temp);
    };
    
    window.hudStore.currentWeather = (weatherId, temp) => {
      if (typeof weatherId === 'object') {
        const data = weatherId;
        weatherId = data.weatherId;
        temp = data.temp;
      }
      
      // Преобразуем weatherId в название типа
      const weatherTypes = ['EXTRASUNNY', 'CLEAR', 'CLOUDS', 'SMOG', 'FOGGY', 
                           'OVERCAST', 'RAIN', 'THUNDER', 'CLEARING', 'NEUTRAL',
                           'SNOWLIGHT', 'SNOW', 'BLIZZARD', 'XMAS'];
      weatherTypeComputed = weatherTypes[weatherId] || 'CLEAR';
      weather.temperature = parseFloat(temp);
      
      // Определяем день/ночь
      const hour = new Date($serverDateTime).getHours();
      weather.dayType = (hour >= 6 && hour < 20) ? 'day' : 'night';
    };
    
    // Голос
    window.hudStore.microphone = (value) => {
      talking = value;
    };
    
    window.hudStore.isMute = (value) => {
      muteStatus = value;
    };
  }
  
  // Анимация денег (как в старом placetime)
  onMount(() => {
    charMoney.subscribe(value => {
      const numericValue = Number(value);
      if (isNaN(numericValue)) return;
      
      if (numericValue > previousMoney) moneyChangeDirection = 'increase';
      else if (numericValue < previousMoney) moneyChangeDirection = 'decrease';
      else moneyChangeDirection = null;
      
      money.set(numericValue);
      cash = numericValue;
      previousMoney = numericValue;
      
      if (moneyChangeDirection) {
        setTimeout(() => {
          moneyChangeDirection = null;
        }, 700);
      }
    });
    
    charBankMoney.subscribe(value => {
      const numericValue = Number(value);
      if (isNaN(numericValue)) return;
      
      if (numericValue > previousBank) bankChangeDirection = 'increase';
      else if (numericValue < previousBank) bankChangeDirection = 'decrease';
      else bankChangeDirection = null;
      
      bank.set(numericValue);
      bankData = { id: 1, amount: numericValue };
      previousBank = numericValue;
      
      if (bankChangeDirection) {
        setTimeout(() => {
          bankChangeDirection = null;
        }, 700);
      }
    });
  });
  
  $: weatherIcon = weatherMap[weatherTypeComputed]?.[weather?.dayType] || '002-sunny';
  $: temperatureDisplay = weather.temperature > 0 ? '+' : '';
  $: weatherText = weatherTranslations[weatherTypeComputed.toLowerCase()] || weatherTypeComputed;

  import Statuses from "./statuses.svelte"; // или путь к компоненту статусов
  $: leftMargin = $mapWidth > 0 ? $mapWidth + 50 : 280; // 20px gap от карты

</script>

<style>
  .money-block__value {
    transition: color 0.5s ease, transform 0.3s ease;
  }
  
  .increase {
    color: #6fec74 !important;
    transform: scale(1.05);
  }
  
  .decrease {
    color: #f44336 !important;
    transform: scale(1.05);
  }
</style>
<div class="user-container column-block" style="margin-left: {leftMargin}px;">
  <!-- Status Container -->
  <div class="status-container">
    <Statuses />
  </div>
  
  <!-- Errors -->
  <div class="errors row-block">
    {#each errors as error}
      <div class="error column-block align-center">
        <img 
          class="error__picture" 
          src="{cdn}img/main/hud/errors/{error.img}.svg" 
          alt=""
        />
        <span class="error__title">{error.title}</span>
      </div>
    {/each}
  </div>
  
  <!-- Info Container -->
  <div class="info-container column-block">
    <!-- Weather -->
    <div class="weather row-block full-width">
      <span class="weather__text-temperature">
        {temperatureDisplay} {weather.temperature.toFixed(1)}
      </span>
      <img 
        class="weather__picture" 
        src="{cdn}img/main/hud/weather/{weatherIcon}.svg" 
        alt=""
      />
      <span class="weather__text-name">
        {weatherText}
      </span>
    </div>
    
    <!-- Time + Voice -->
    <div class="row-block align-center">
      <!-- Time Container -->
      <div class="time-container">
        <div class="main-time">
          <div class="main-time__text-hours">{time}</div>
          <div class="main-time__text-date">{formattedDate}</div>
        </div>
        <div class="time-container__text-weekday">{weekday}</div>
      </div>
      
      <!-- Voice -->
      <div class="voice">
        <div class="voice-icon-wrapper">
          {#if talking}
            <img src="{cdn}img/main/hud/{talkingImg}.svg" alt="" />
          {:else if muteStatus}
            <img src="{cdn}img/main/hud/microphone-mute.svg" alt="" />
          {/if}
        </div>
        {#if speakingLanguage && speakingLanguage !== 'en'}
          <img 
            class="voice-icon" 
            src="{cdn}icons/languages/{speakingLanguage}.svg" 
            alt=""
          />
        {/if}
      </div>
    </div>
  </div>
  
  <!-- Money -->
<div class="money">
  <div class="money-container">
    <!-- Cash -->
    <div class="money-block cash_hud">
      <span class="money-block__logo cash_hud">
        <img src="{cdn}icons/main/hud/cash.svg" alt="" />
      </span>
      <span class="money-block__value cash_hud {moneyChangeDirection}">
        ${Math.round($money).toLocaleString('ru-RU')}
      </span>
    </div>
    
    <!-- Bank -->
    <div class="money-block bank_hud">
      {#if bankData && bankData.id}
        <span class="money-block__logo bank_hud">
          <img src="{cdn}icons/main/hud/bank.svg" alt="" />
        </span>
        <span class="money-block__value cash_hud {bankChangeDirection}">
          ${Math.round($bank).toLocaleString('ru-RU')}
        </span>
      {/if}
      
      {#if showCasinoChips}
        <span class="chip_hud">
          <img src="{cdn}icons/main/hud/chip.svg" alt="" />
          {parseInt(casinoChipsBalance)?.toLocaleString('ru-RU')}
        </span>
      {/if}
    </div>
  </div>
</div>
</div>