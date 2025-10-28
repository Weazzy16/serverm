<script>
  import './main.css';
  import './fonts/inv/style.css';
  import './fonts/items/style.css';
  import './fonts/gamemenu/style.css';
  import './fonts/Gilroy/stylesheet.css';
  import './fonts/SFPro/stylesheet.css';
  import { serverDateTime, isEvent } from 'store/server'
  import { charData } from 'store/chars';
  import { accountRedbucks } from 'store/account'
  import { format } from 'api/formatter'
  import { executeClient } from 'api/rage'
  import { otherStatsData } from 'store/account'
  import { hasJsonStructure } from "api/functions";
  import { addListernEvent } from 'api/functions';
  import { storeSettings } from 'store/settings'; // ← ДОБАВЬ ИМПОРТ

  export let visible = false;

  function formatMoney(num) {
  return new Intl.NumberFormat('ru-RU', {
    maximumFractionDigits: 0
  }).format(num);
}

  const handleKeyDown = (event) => {
      const { keyCode } = event;

      if (keyCode === 27)
          onClose ();
  }

   const onClose = () => {
      // Отправляем настройки на сервер при закрытии меню
      if (typeof window.executeClient === 'function') {
          window.executeClient("chatconfig", JSON.stringify($storeSettings));
      }
      executeClient("client.stockitemsexit");
  }
  let currentCaseTitle = '';
function handleCaseTitle(e) {
  currentCaseTitle = e.detail;
}
  let replacements = [
      { word: 'Osnovnoe', replacement: 'Основное' },
      { word: 'Statiskic', replacement: 'Статистика' },
      { word: 'Carsuser', replacement: 'Транспорт' },
      { word: 'Housebiz', replacement: 'Имущество' },
      { word: 'Skills', replacement: 'Навыки' },
      { word: 'Anim1', replacement: 'Действия' },
      { word: 'Anim2', replacement: 'Позы' },
      { word: 'Anim3', replacement: 'Позитивные' },
      { word: 'Anim4', replacement: 'Негативные' },
      { word: 'Anim5', replacement: 'Танцы' },
      { word: 'Anim6', replacement: 'Прочие' },
      { word: 'Anim7', replacement: 'Эксклюзивные' },
      { word: 'Anim8', replacement: 'Избранное' },
      { word: 'Anim9', replacement: 'Круговое меню' },
      { word: 'Q1', replacement: 'Квесты' },
      { word: 'Q2', replacement: 'Ежедневные' },
      { word: 'Q3', replacement: 'Бонусы' },
      { word: 'Q4', replacement: 'Достижения' },
      { word: 'Report', replacement: 'Обращения' },
      { word: 'BP1', replacement: 'Главная' },
      { word: 'BP2', replacement: 'Купить опыт' },
      { word: 'BP3', replacement: 'Ежедневные задания' },
      { word: 'BP4', replacement: 'Квест' },
      { word: 'Pay', replacement: 'Пополнить' },
      { word: 'Shop', replacement: 'Персонаж' },
      { word: 'Shopcl', replacement: '' },
      { word: 'Shoptaty', replacement: '' },
      { word: 'Shopanim', replacement: 'Анимации' },
      { word: 'Shopskin', replacement: '' },
      { word: 'Shopcar', replacement: '' },
      { word: 'Money', replacement: 'Валюта' },
      { word: 'Transa', replacement: 'Детализация' },
      { word: 'Roulette', replacement: 'Кейс' },
      { word: 'Cases', replacement: 'Кейсы' },
      { word: 'Premium', replacement: 'помогу' },
      { word: 'WheelGames', replacement: 'Рулетка' },
      { word: 'JackpotGames', replacement: 'Джекпот' },
      { word: 'CrashGames', replacement: 'Краш' },
      { word: 'Gamestat', replacement: 'FORBES ТОП 100' },
      { word: 'General', replacement: 'Общие' },
      { word: 'State', replacement: 'Государственные орг.' },
      { word: 'Crime', replacement: 'Криминальные орг.' },
      { word: 'FAQ', replacement: 'FAQ' },
      { word: 'Settings', replacement: 'Настройки' },
      { word: 'Stock', replacement: 'Хранилище' },
      { word: 0, replacement: 'Все' },
      { word: 1, replacement: 'Автомобили' },
      { word: 2, replacement: 'Вертолеты' },
      { word: 3, replacement: 'Все' },
      { word: 4, replacement: 'Головные уборы' },
      { word: 5, replacement: 'Верх' },
      { word: 6, replacement: 'Майки' },
      { word: 7, replacement: 'Верхняя одежда' },
      { word: 8, replacement: 'Низ' },
      { word: 9, replacement: 'Обувь' },
      { word: 10, replacement: 'Очки' },
      { word: 11, replacement: 'Наручные часы/Браслет' },
      { word: 12, replacement: 'Браслет' },
      { word: 13, replacement: 'Серьги' },
      { word: 14, replacement: 'Маски' },
      { word: 15, replacement: 'Аксессуары' },
      { word: 16, replacement: 'Бронежилеты' },
      { word: 17, replacement: 'Нашивки' },
      { word: 18, replacement: 'Рюкзаки' },
      { word: 19, replacement: 'Перчатки' },
      { word: 20, replacement: 'Цена. По убыванию' },
      { word: 21, replacement: 'Цена. По возрастанию' },
      { word: 22, replacement: 'Редкость. По убыванию' },
      { word: 23, replacement: 'Редкость. По возрастанию' },
  ];


  executeClient ("client.phone.cars.load");
  executeClient ("client.gta5devmenucars");
  executeClient ("client.donate.load");



  window.gta5devmenucars = function (vehiclesJson) {
      if (hasJsonStructure(vehiclesJson))
          carsList = JSON.parse(vehiclesJson);
  };

  let itemliststock;

  let carsList = [{
      number: "222133218 2132",
      model: "adder",
      header: "Hausbesitzer"
      },
      {
      number: "229",
      model: "adder",
      },
  ];
  

  addListernEvent("phoneCarsLoad", true);

  let useVisible = -1;

  $: {
      if (useVisible != visible) {
          if (visible && $otherStatsData.Name/* && $otherStatsData.UUID !== selectCharData.UUID*/) {
              selectCharData = $otherStatsData;
          } else if (visible && !$otherStatsData.Name && selectCharData !== $charData) {
              selectCharData = $charData;
          } else if (!visible && $otherStatsData.Name) {
              selectCharData = $charData;
              window.accountStore.otherStatsData ('{}');
          }
          useVisible = visible;
      }
  }
  
  let selectCharData = $charData; 
  let selectView = "Menu";
  let pageload = 0;

  window.pageloadf2 = (wiew) => {
      pageload = wiew;
  }

  import User from "./elements/user/index.svelte";
  import Animation from "./elements/anim/index.svelte";
  import Quests from "./elements/quest/index.svelte";
  import Rewrad from "./elements/rewardslist/index.svelte";
  import Achiev from "./elements/achiev/index.svelte";
  import Avail from "./elements/rewardslist/avail/index.svelte";
  import Calendar from "./elements/rewardslist/calendar/index.svelte";
  import Pay from './elements/shop/popup/popuppayment.svelte';  

  import Report from "./elements/report/index.svelte";
  import Roulette from "./elements/roulette/index.svelte";
  import BattlePass from "./elements/battlepass/index.svelte";
  import Shop from "./elements/shop/index.svelte";
  import MiniGames from "./elements/minigames/index.svelte";
  import CrashGames from "./elements/crashgames/index.svelte";
    import WheelGames from "./elements/wheelgames/index.svelte";
  import JackpotGames from "./elements/jackpotgames/index.svelte";
  import State from "./elements/rules/state.svelte";
  import Crime from "./elements/rules/crime.svelte";
  import General from "./elements/rules/general.svelte";
  import FAQ from "./elements/faq/index.svelte";
  import Settings from "./elements/settings/index.svelte";
  import Stock from "./elements/stock/stock.svelte";
  import Transa from "./elements/transa/index.svelte";
  import Gamestat from "./elements/gamestat/index.svelte";

  const Views = {
      Osnovnoe: User,
      Statiskic: User,
      Carsuser: User,
      Housebiz: User,
      Skills: User,
      Anim1: Animation,
      Anim2: Animation,
      Anim3: Animation,
      Anim4: Animation,
      Anim5: Animation,
      Anim6: Animation,
      Anim7: Animation,
      Anim8: Animation,
      Anim9: Animation,
      Q1: Quests,
      Q2: Achiev,
      Q3: Avail,
      Q4: Calendar,
      Report,
      BP1: BattlePass,
      BP2: BattlePass,
      BP3: BattlePass,
      BP4: BattlePass,
      Shop: Shop,
      Shopcl: Shop,
      Shoptaty: Shop,
      Shopanim: Shop,
      Shopskin: Shop,
      Shopcar: Shop,
      Money: Shop,
      Transa: Shop,
      Roulette,
      Pay: Pay,
      Cases: Shop,
      Premium: Shop,
      MiniGames,
      CrashGames,
      WheelGames,
      JackpotGames,
      State,
      Crime,
      General,
      FAQ,
      Settings,
      Stock,
      Transa,
      Gamestat
  }

  
  window.gameMenuView = (wiew) => {
      executeClient ("client.stockitems");
      executeClient ("client.phone.cars.load");
      executeClient ("client.gta5devmenucars");
      selecttypecat = false;
      catlist = false;
      selectgender = true;
      genderlist = false;
      selectView = "";
      pageload = 0;
      pagefilter = 20;
      setTimeout(() => {
          selectView = wiew;
      }, 12);
  }

  let selecttypecat;
  let catlist;
  let selectgender;
  let genderlist;
  let pricefilter;
  let catFilter;
  let pagefilter;

  const onSelectTypeCat = (type) => {
      pageload = type;
      catlist = false;
  }

  const onSelectTypeFilter = (type) => {
      pagefilter = type;
      catFilter = false;
  }

  const onSelectGender = (type) => {
      selectgender = type;
      genderlist = false;
  }

  function opencatlisttype() {
      catlist = !catlist;
  }
  
  function togglePriceFilter() {
      catFilter = !catFilter;
  }

  const opengenerlist = () => {
      genderlist = true;
  }

  let searchText = "";

  const defaultSorted = ["User", "Animation", "Quests", "Report", "BattlePass", "Shop", "MiniGames", "Rules", "FAQ", "Settings"];
  let _pagesSorted = ["User", "Animation", "Quests", "MiniGames", "Rules", "FAQ", "Settings"];
  let PagesSorted = ["User", "Animation", "Quests", "MiniGames", "Rules", "FAQ", "Settings"];

  const updatePage = (name, value) => {

      const index = _pagesSorted.indexOf(name)

      if (index !== -1 && !value)
          _pagesSorted.splice(index, 1);
      else if (index === -1 && value)
          _pagesSorted.push(name)

      let sorted = [];

      defaultSorted.forEach((value) => {
          if (_pagesSorted.includes (value))
              sorted.push(value)
      })

      PagesSorted = sorted;
  }

  import { charFractionID, charOrganizationID } from 'store/chars'


  isEvent.subscribe(value => {
      updatePage ("Animation", value);
  });
  charFractionID.subscribe(value => {
      updatePage ("MiniGames", value > 0);
  });
  charOrganizationID.subscribe(value => {
      updatePage ("User", value > 0);
  });


  let UseVisible = visible;

  $: {
      if (UseVisible != visible) {
          UseVisible = visible;
          if (!visible) {
              selectView = "Menu";
              onInputBlur ();
          }
      }
  }

  let isFocusInput = false;

  window.onFocusInput = (value) => isFocusInput = value;

  let timerId = null;

         
        
          let openCharacter = false;
          let openAnimations = false;
          let openQuests = false;
       
          let openShop = false;
          let openGamestat = false;
          let openMinigames = false;
          let openRules = false;
          let openFAQ = false;
          let openSettings = false;
          let openSeasonPass = false;
        
          // При выборе пункта меню теперь только переключаем view, не закрываем дропдауны
         function handleSelect(view) {
            window.gameMenuView(view);
          }
       
</script>

<svelte:window on:keyup={handleKeyDown} />

<!-- svelte-ignore a11y-no-noninteractive-tabindex -->
<div class="gta5devmenuf2">
  <div class="leftmenu">
      <div class="headplayr">
          <img src="https://cdn.majestic-files.com/public/master/static/img/avatars/male.svg" alt=""/>
          <div class="infoplayer">
              <p>Логин</p>
              <span>{selectCharData.Login}</span>
          </div>
      </div>
      
        
        <ul class="menulist">
          <!-- Персонаж -->
          <li
            class="namenav"
            tabindex="0"
            class:active={selectView === 'Osnovnoe' || selectView === 'Statiskic' || selectView === 'Carsuser' || selectView === 'Housebiz' || selectView === 'Skills'}
          >
            <div
            
              class="navblock"
              on:click={() => openCharacter = !openCharacter}
              on:keypress={(e) => e.key === 'Enter' && (openCharacter = !openCharacter)}
            >
            <svg class="icon" width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M13.818,13.273H9.455a4.914,4.914,0,0,0-4.909,4.909v2.909a.545.545,0,0,0,.545.545H18.182a.545.545,0,0,0,.545-.545V18.182A4.915,4.915,0,0,0,13.818,13.273Z"/>
              <circle xmlns="http://www.w3.org/2000/svg" cx="4.909" cy="4.909" r="4.909" transform="translate(6.727 1.636)"/>
          </svg>   
              <p>Персонаж</p>
              
              <span class="arrow" class:open={openCharacter}></span>
              <svg class="arrow1" class:open={openCharacter} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
              </svg>
              
            </div>
            <ul class="dropdown" style="max-height: {openCharacter ? '650px' : '0'};">
             
              <li
                tabindex="0"
                on:click|stopPropagation={() => handleSelect('Osnovnoe')}
                class:active={selectView === 'Osnovnoe'}
              > <div class="line vertical"></div>
              <div class="line horizontal"></div><p>Основное</p></li>
              <li
                tabindex="0"
                on:click|stopPropagation={() => handleSelect('Statiskic')}
                class:active={selectView === 'Statiskic'}
              ><div class="line vertical"></div>
              <div class="line horizontal"></div><p>Статистика</p></li>
              <li
                tabindex="0"
                on:click|stopPropagation={() => handleSelect('Carsuser')}
                class:active={selectView === 'Carsuser'}
              ><div class="line vertical"></div>
              <div class="line horizontal"></div><p>Транспорт</p></li>
              <li
                tabindex="0"
                on:click|stopPropagation={() => handleSelect('Housebiz')}
                class:active={selectView === 'Housebiz'}
              ><div class="line vertical"></div>
              <div class="line horizontal"></div><p>Имущество</p></li>
              <li
                tabindex="0"
                on:click|stopPropagation={() => handleSelect('Skills')}
                class:active={selectView === 'Skills'}
              ><div class="line vertical"></div>
              <div class="line horizontal"></div><p>Навыки</p></li>
            </ul>
          </li>
        
          <!-- Анимации -->
          <li
            class="namenav"
            tabindex="0"
            class:active={selectView === 'Anim1' || selectView === 'Anim2'|| selectView === 'Anim3'|| selectView === 'Anim4'|| selectView === 'Anim5'|| selectView === 'Anim6'|| selectView === 'Anim7'|| selectView === 'Anim8' || selectView === 'Anim9'}
          >
            <div
              class="navblock"
              on:click={() => openAnimations = !openAnimations}
              on:keypress={(e) => e.key === 'Enter' && (openAnimations = !openAnimations)}
            >
            <svg class="icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
              <g xmlns="http://www.w3.org/2000/svg" transform="translate(70 1222)">
                <g transform="translate(-71.429 -1223.429)">
                  <path d="M11.429,11.429a5,5,0,1,1-5-5A5,5,0,0,1,11.429,11.429Z"/>
                  <path d="M16.429,11.429a5.01,5.01,0,0,1-5.87,4.919,6.414,6.414,0,0,0,0-9.838,5.01,5.01,0,0,1,5.87,4.919Z"/>
                  <path d="M21.429,11.429a5.01,5.01,0,0,1-5.87,4.919,6.414,6.414,0,0,0,0-9.838,5.01,5.01,0,0,1,5.87,4.919Z"/>
                </g>
              </g>
              
          </svg> 
              <p>Анимации</p>
              <span class="arrow" class:open={openAnimations}></span>
              <svg class="arrow1" class:open={openAnimations} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
              </svg>
            </div>
            <ul class="dropdown" style="max-height: {openAnimations ? '650px' : '0'};">
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim1')} class:active={selectView === 'Anim1'}><div class="line vertical"></div><div class="line horizontal"></div><p>Действия</p></li>
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim2')} class:active={selectView === 'Anim2'}><div class="line vertical"></div><div class="line horizontal"></div><p>Позы</p></li>
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim3')} class:active={selectView === 'Anim3'}><div class="line vertical"></div><div class="line horizontal"></div><p>Позитивные</p></li>
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim4')} class:active={selectView === 'Anim4'}><div class="line vertical"></div><div class="line horizontal"></div><p>Негативные</p></li>
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim5')} class:active={selectView === 'Anim5'}><div class="line vertical"></div><div class="line horizontal"></div><p>Танцы</p></li>
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim6')} class:active={selectView === 'Anim6'}><div class="line vertical"></div><div class="line horizontal"></div><p>Прочие</p></li>
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim7')} class:active={selectView === 'Anim7'}><div class="line vertical"></div><div class="line horizontal"></div><p>Эксклюзивные</p></li>
            <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim8')} class:active={selectView === 'Anim8'}><div class="line vertical"></div><div class="line horizontal"></div><p>Избранное</p></li>
             <li tabindex="0" on:click|stopPropagation={() => handleSelect('Anim9')} class:active={selectView === 'Anim9'}><div class="line vertical"></div><div class="line horizontal"></div><p>Круговое меню</p></li>
            
          </ul>
          </li>
        
          <!-- Задания -->
          <li
            class="namenav"
            tabindex="0"
            class:active={selectView === 'Q1' || selectView === 'Q2' || selectView === 'Q3'|| selectView === 'Q4'}
          >
            <div
              class="navblock"
              on:click={() => openQuests = !openQuests}
              on:keypress={(e) => e.key === 'Enter' && (openQuests = !openQuests)}
            >
            <svg class="icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
              <g xmlns="http://www.w3.org/2000/svg" id="Группа_масок_348" data-name="Группа масок 348" transform="translate(-100 -566)" clip-path="url(#clip-path)">
                <g id="star" transform="translate(98.743 564.727)">
                  <path id="Контур_16272" data-name="Контур 16272" d="M21.045,9.674l-4.383,4.272L17.7,19.98a.7.7,0,0,1-1.021.742l-5.419-2.849L5.839,20.721a.7.7,0,0,1-1.021-.742l1.035-6.034L1.47,9.674a.7.7,0,0,1,.39-1.2l6.057-.879,2.709-5.489a.733.733,0,0,1,1.262,0L14.6,7.594l6.057.879a.7.7,0,0,1,.39,1.2Z"/>
                </g>
              </g>
              
          </svg> 
              <p>Задания</p>
              <span class="arrow" class:open={openQuests}></span>
              <svg class="arrow1" class:open={openQuests} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
              </svg>
            </div>
            <ul class="dropdown" style="max-height: {openQuests ? '650px' : '0'};">
              <li tabindex="0" on:click|stopPropagation={() => handleSelect('Q1')} class:active={selectView === 'Q1'}><div class="line vertical"></div><div class="line horizontal"></div><p>Квесты</p></li>
              <li tabindex="0" on:click|stopPropagation={() => handleSelect('Q2')} class:active={selectView === 'Q2'}><div class="line vertical"></div><div class="line horizontal"></div><p>Ежедневные</p></li>
              <li tabindex="0" on:click|stopPropagation={() => handleSelect('Q3')} class:active={selectView === 'Q3'}><div class="line vertical"></div><div class="line horizontal"></div><p>Доступные</p></li>
              <li tabindex="0" on:click|stopPropagation={() => handleSelect('Q4')} class:active={selectView === 'Q4'}><div class="line vertical"></div><div class="line horizontal"></div><p>Календарь</p></li>
            </ul>
          </li>
        
          <!-- Обращения -->
<li
  class="namenav"
  tabindex="0"
  class:active={selectView === 'Report'}
>
  <div class="navblock" on:click={() => handleSelect('Report')} on:keypress={(e) => e.key === 'Enter' && handleSelect('Report')}>
    <!-- icon placeholder -->
    <svg class="icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path xmlns="http://www.w3.org/2000/svg" d="M2.383.586V15.9a.586.586,0,0,0,.586.586H13.516A.586.586,0,0,0,14.1,15.9V.586A.586.586,0,0,0,13.516,0H2.969A.586.586,0,0,0,2.383.586Zm9.375,12.93H9.414a.586.586,0,0,1,0-1.172h2.344a.586.586,0,0,1,0,1.172ZM4.727,2.969H8.242a.586.586,0,0,1,0,1.172H4.727a.586.586,0,0,1,0-1.172Zm0,2.344h7.031a.586.586,0,0,1,0,1.172H4.727a.586.586,0,0,1,0-1.172Zm0,2.344h7.031a.586.586,0,0,1,0,1.172H4.727a.586.586,0,0,1,0-1.172Zm0,2.344h7.031a.586.586,0,0,1,0,1.172H4.727a.586.586,0,0,1,0-1.172Z"/>
      <path xmlns="http://www.w3.org/2000/svg" d="M6.484,20H17.031a.586.586,0,0,0,.586-.586V4.141a.586.586,0,0,0-.586-.586H15.273V15.9a1.76,1.76,0,0,1-1.758,1.758H5.9v1.758A.586.586,0,0,0,6.484,20Z"/>
  </svg>
    <p>Обращения</p>
    <svg xmlns="http://www.w3.org/2000/svg" width="1.2093518518518518vh" height="0.7028703703703704vh" viewBox="0 0 13.061 7.591" class="arrow1s">
      <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none"></path>
    </svg>
  </div>
</li>

<!-- Сезонный пропуск -->
<li class="namenav" tabindex="0" class:active={selectView === 'BP1' || selectView === 'BP2' || selectView === 'BP3'|| selectView === 'BP4'}>
  <div class="navblock" on:click={() => openSeasonPass = !openSeasonPass} on:keypress={(e) => e.key === 'Enter' && (openSeasonPass = !openSeasonPass)}>
    <!-- icon placeholder -->
    <svg class="icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path xmlns="http://www.w3.org/2000/svg" fill-rule="evenodd" clip-rule="evenodd" d="M10 0C10.6136 0 11.1111 0.497461 11.1111 1.11111V3.61384L12.918 1.80692C13.3519 1.373 14.0555 1.373 14.4894 1.80692C14.9233 2.24083 14.9233 2.94435 14.4894 3.37827L11.1111 6.75653V8.88889H13.2435L16.6217 5.51062C17.0556 5.07671 17.7592 5.07671 18.1931 5.51062C18.627 5.94454 18.627 6.64805 18.1931 7.08197L16.3862 8.88889H18.8889C19.5025 8.88889 20 9.38635 20 10C20 10.6136 19.5025 11.1111 18.8889 11.1111H16.3862L18.1931 12.918C18.627 13.3519 18.627 14.0555 18.1931 14.4894C17.7592 14.9233 17.0556 14.9233 16.6217 14.4894L13.2435 11.1111H11.1111V13.2435L14.4894 16.6217C14.9233 17.0556 14.9233 17.7592 14.4894 18.1931C14.0555 18.627 13.3519 18.627 12.918 18.1931L11.1111 16.3862V18.8889C11.1111 19.5025 10.6136 20 10 20C9.38635 20 8.88889 19.5025 8.88889 18.8889V16.3862L7.08197 18.1931C6.64805 18.627 5.94454 18.627 5.51062 18.1931C5.07671 17.7592 5.07671 17.0556 5.51062 16.6217L8.88889 13.2435V11.1111H6.75653L3.37827 14.4894C2.94435 14.9233 2.24083 14.9233 1.80692 14.4894C1.373 14.0555 1.373 13.3519 1.80692 12.918L3.61384 11.1111H1.11111C0.497461 11.1111 0 10.6136 0 10C0 9.38635 0.497461 8.88889 1.11111 8.88889H3.61384L1.80692 7.08197C1.373 6.64805 1.373 5.94454 1.80692 5.51062C2.24083 5.07671 2.94435 5.07671 3.37827 5.51062L6.75653 8.88889H8.88889V6.75653L5.51062 3.37827C5.07671 2.94435 5.07671 2.24083 5.51062 1.80692C5.94454 1.373 6.64805 1.373 7.08197 1.80692L8.88889 3.61384V1.11111C8.88889 0.497461 9.38635 0 10 0Z" fill="#31953B"/>  
    </svg>
    <p>Сезонный пропуск</p>
    <span class="arrow" class:open={openSeasonPass}></span>
    <svg class="arrow1" class:open={openSeasonPass} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
    </svg>
  </div>
  <ul class="dropdown" style="max-height: {openSeasonPass ? '650px' : '0'};">
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('BP1')} class:active={selectView === 'BP1'}><div class="line vertical"></div><div class="line horizontal"></div><p>Главная</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('BP2')} class:active={selectView === 'BP2'}><div class="line vertical"></div><div class="line horizontal"></div><p>Купить опыт</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('BP3')} class:active={selectView === 'BP3'}><div class="line vertical"></div><div class="line horizontal"></div><p>Ежедневные задания</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('BP4')} class:active={selectView === 'BP4'}><div class="line vertical"></div><div class="line horizontal"></div><p>Квест</p></li>
  </ul>
</li>

<!-- Магазин -->
<li class="namenav" tabindex="0" class:active={openShop || ['Pay','Cases','Shopcl','Shoptaty','Shopcar','Money','Stock','Transa'].includes(selectView)}>
  <div class="navblock" on:click={() => openShop = !openShop} on:keypress={(e) => e.key === 'Enter' && (openShop = !openShop)}>
    <!-- icon placeholder -->
    <svg class="icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path xmlns="http://www.w3.org/2000/svg" d="M18.889,1.111H1.111V3.333H18.889ZM20,12.222V10L18.889,4.444H1.111L0,10v2.222H1.111v6.667H12.222V12.222h4.444v6.667h2.222V12.222ZM10,16.667H3.333V12.222H10Z"/>
      
  </svg> 
    <p>Магазин</p>
    <span class="arrow" class:open={openShop}></span>
    <svg class="arrow1" class:open={openShop} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
    </svg>
  </div>
  <ul class="dropdown" style="max-height: {openShop ? '730px' : '0'};">
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Pay')} class:active={selectView === 'Pay'}><div class="line vertical"></div><div class="line horizontal"></div><p>Пополнить <span class="x2-donate">x2</span></p> </li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Cases')} class:active={selectView === "Cases" || selectView === "Roulette"}><div class="line vertical"></div><div class="line horizontal"></div><p>Кейсы</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Premium')} class:active={selectView === 'Premium'}><div class="line vertical"></div><div class="line horizontal"></div><p>Majestic Premium</p></li>
        <li tabindex="0" on:click|stopPropagation={() => handleSelect('Money')} class:active={selectView === 'Money'}><div class="line vertical"></div><div class="line horizontal"></div><p>Валюта</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Shop')} class:active={selectView === 'Shop'}><div class="line vertical"></div><div class="line horizontal"></div><p>Персонаж</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Shopcar')} class:active={selectView === 'Shopcar'}><div class="line vertical"></div><div class="line horizontal"></div><p>Транспорт</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Shopcl')} class:active={selectView === 'Shopcl'}><div class="line vertical"></div><div class="line horizontal"></div><p>Одежда</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Shopanim')} class:active={selectView === 'Shopanim'}><div class="line vertical"></div><div class="line horizontal"></div><p>Анимации</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Shopskin')} class:active={selectView === 'Shopskin'}><div class="line vertical"></div><div class="line horizontal"></div><p>Скины</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Stock')} class:active={selectView === 'Stock'}><div class="line vertical"></div><div class="line horizontal"></div><p>Предметы</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Transa')} class:active={selectView === 'Transa'}><div class="line vertical"></div><div class="line horizontal"></div><p>Детализация</p></li>
  </ul>
</li>


<!--Игровая стат -->
<li class="namenav" tabindex="0" class:active={openGamestat || ['Gamestat'].includes(selectView)}>
  <div class="navblock" on:click={() => openGamestat = !openGamestat} on:keypress={(e) => e.key === 'Enter' && (openGamestat = !openGamestat)}>
    <!-- icon placeholder -->
    <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 20 20" fill="none">
<path fill-rule="evenodd" clip-rule="evenodd" d="M10.6401 8.75749C10.972 8.42549 11.4133 8.24263 11.8826 8.24263C12.3519 8.24263 12.7931 8.42549 13.125 8.75749L18.1144 13.7491V6.23468C18.1144 6.07928 18.0526 5.93026 17.9428 5.82036L12.2967 0.171648C12.1636 0.0384819 11.9746 -0.0226211 11.7888 0.00758037C11.6029 0.0377418 11.4429 0.155437 11.3587 0.323926L5.87455 11.2975H2.3475V13.8913H5.50868L10.6401 8.75749Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M18.1144 15.6466C18.1144 15.4913 18.0526 15.3425 17.9428 15.2327L12.2967 9.5896C12.1869 9.47982 12.0379 9.41812 11.8826 9.41812C11.7273 9.41812 11.5783 9.47982 11.4685 9.5896L5.9939 15.0612H2.34748V17.6525H18.1144L18.1144 15.6466Z"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M19.4141 18.8281H1.17186V0.585922C1.17186 0.262332 0.90952 -1.76294e-05 0.58592 -1.76294e-05C0.26233 -1.76294e-05 -1.95367e-05 0.262322 -1.95367e-05 0.585922V19.414C-1.95367e-05 19.7376 0.26232 20 0.58592 20H19.414C19.7376 20 20 19.7376 20 19.414C20 19.0905 19.7376 18.8281 19.414 18.8281L19.4141 18.8281Z"/>
</svg>
    <p>Игровая статистика</p>
    <span class="arrow" class:open={openGamestat}></span>
    <svg class="arrow1" class:open={openGamestat} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
    </svg>
  </div>
  <ul class="dropdown" style="max-height: {openGamestat ? '70px' : '0'};">
   
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Gamestat')} class:active={selectView === 'Gamestat'}><div class="line vertical"></div><div class="line horizontal"></div><p>Forbes Топ 100</p></li>
  </ul>
</li>

<!-- MiniGames -->
<li class="namenav" tabindex="0" class:active={openMinigames || ['MiniGames'].includes(selectView)}>
  <div class="navblock" on:click={() => openMinigames = !openMinigames} on:keypress={(e) => e.key === 'Enter' && (openMinigames = !openMinigames)}>
    <!-- icon placeholder -->
    <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="31" height="25" viewBox="0 0 31 25" fill="none">
<path d="M9.30349 0.996582C4.18849 0.996582 0.0214844 5.16356 0.0214844 10.2786V18.9966C0.0214844 22.3101 2.70798 24.9966 6.02148 24.9966C8.14248 24.9966 9.87199 23.7276 10.943 21.9966C12.206 21.9936 17.654 22.0026 19.1465 21.9966C20.2205 23.7141 21.9095 24.9966 24.0215 24.9966C27.335 24.9966 30.0215 22.3101 30.0215 18.9966V10.2786C30.0215 5.16356 25.8545 0.996582 20.7395 0.996582H9.30349ZM9.02148 6.99658C9.84948 6.99658 10.5215 7.66858 10.5215 8.49658V9.99658H12.0215C12.8495 9.99658 13.5215 10.6686 13.5215 11.4966C13.5215 12.3246 12.8495 12.9966 12.0215 12.9966H10.5215V14.4966C10.5215 15.3246 9.84948 15.9966 9.02148 15.9966C8.19348 15.9966 7.52148 15.3246 7.52148 14.4966V12.9966H6.02148C5.19348 12.9966 4.52148 12.3246 4.52148 11.4966C4.52148 10.6686 5.19348 9.99658 6.02148 9.99658H7.52148V8.49658C7.52148 7.66858 8.19348 6.99658 9.02148 6.99658ZM21.0215 6.99658C21.8495 6.99658 22.5215 7.66858 22.5215 8.49658C22.5215 9.32458 21.8495 9.99658 21.0215 9.99658C20.1935 9.99658 19.5215 9.32458 19.5215 8.49658C19.5215 7.66858 20.1935 6.99658 21.0215 6.99658ZM18.0215 9.99658C18.8495 9.99658 19.5215 10.6686 19.5215 11.4966C19.5215 12.3246 18.8495 12.9966 18.0215 12.9966C17.1935 12.9966 16.5215 12.3246 16.5215 11.4966C16.5215 10.6686 17.1935 9.99658 18.0215 9.99658ZM24.0215 9.99658C24.8495 9.99658 25.5215 10.6686 25.5215 11.4966C25.5215 12.3246 24.8495 12.9966 24.0215 12.9966C23.1935 12.9966 22.5215 12.3246 22.5215 11.4966C22.5215 10.6686 23.1935 9.99658 24.0215 9.99658ZM21.0215 12.9966C21.8495 12.9966 22.5215 13.6686 22.5215 14.4966C22.5215 15.3246 21.8495 15.9966 21.0215 15.9966C20.1935 15.9966 19.5215 15.3246 19.5215 14.4966C19.5215 13.6686 20.1935 12.9966 21.0215 12.9966Z"/>
<defs>
<linearGradient id="paint0_linear_55_3401" x1="15.0215" y1="0.996582" x2="15.0215" y2="24.9966" gradientUnits="userSpaceOnUse">
<stop stop-color="white"/>
<stop offset="1" stop-color="#9A9A9A"/>
</linearGradient>
</defs>
</svg>
    <p>Мини-игры</p>
    <span class="arrow" class:open={openMinigames}></span>
    <svg class="arrow1" class:open={openMinigames} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
    </svg>
  </div>
  <ul class="dropdown" style="max-height: {openMinigames ? '170px' : '0'};">
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('CrashGames')} class:active={selectView === 'CrashGames'}><div class="line vertical"></div><div class="line horizontal"></div><p>Краш </p> </li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('JackpotGames')} class:active={selectView === 'JackpotGames'}><div class="line vertical"></div><div class="line horizontal"></div><p>Джекпот</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('WheelGames')} class:active={selectView === 'WheelGames'}><div class="line vertical"></div><div class="line horizontal"></div><p>Рулетка</p></li>
  </ul>
</li>


<!-- General -->
<li class="namenav" tabindex="0" class:active={openRules || ['General'].includes(selectView)}>
  <div class="navblock" on:click={() => openRules = !openRules} on:keypress={(e) => e.key === 'Enter' && (openRules = !openRules)}>
    <!-- icon placeholder -->
    <svg class="icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
      <g xmlns="http://www.w3.org/2000/svg" id="rules" transform="translate(-100 -566)" clip-path="url(#clip-path)">
        <g id="document" transform="translate(102.295 566)">
          <path id="Контур_15849" d="M3.654,3.79V.355L.22,3.79Zm0,0"/>
          <path id="Контур_15850" d="M14.939,0H4.866V4.4A.606.606,0,0,1,4.26,5h-4.4V19.394A.606.606,0,0,0,.47,20H14.939a.606.606,0,0,0,.606-.606V.606A.606.606,0,0,0,14.939,0ZM4.468,6.493h6.473a.606.606,0,0,1,0,1.212H4.468a.606.606,0,0,1,0-1.212Zm0,2.258h6.473a.606.606,0,0,1,0,1.212H4.468a.606.606,0,0,1,0-1.212Zm0,2.258h6.473a.606.606,0,1,1,0,1.211H4.468a.606.606,0,1,1,0-1.211Zm0,2.258H9.346a.606.606,0,1,1,0,1.211H4.468a.606.606,0,0,1,0-1.211Zm0,0"/>
        </g>
      </g>
  </svg>
    <p>Правила сервера</p>
    <span class="arrow" class:open={openRules}></span>
    <svg class="arrow1" class:open={openRules} width="1.21vh" height="0.70vh" viewBox="0 0 13.061 7.591" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none" />
    </svg>
  </div>
  <ul class="dropdown" style="max-height: {openRules ? '170px' : '0'};">
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('General')} class:active={selectView === 'General'}><div class="line vertical"></div><div class="line horizontal"></div><p>Общие </p> </li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('State')} class:active={selectView === 'State'}><div class="line vertical"></div><div class="line horizontal"></div><p>Государственные орг.</p></li>
    <li tabindex="0" on:click|stopPropagation={() => handleSelect('Crime')} class:active={selectView === 'Crime'}><div class="line vertical"></div><div class="line horizontal"></div><p>Криминальные орг.</p></li>
  </ul>
</li>
<!-- Остальные без выпадающих -->




<li class="namenav" tabindex="0" class:active={selectView === 'Settings'}>
  <div class="navblock" on:click={() => handleSelect('Settings')} on:keypress={(e) => e.key === 'Enter' && handleSelect('Settings')}>
    <!-- icon placeholder -->
    <svg class="icon" width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
      <path xmlns="http://www.w3.org/2000/svg" d="M19.473,7.893a2.613,2.613,0,0,1-2.06-3.533,1.227,1.227,0,0,0-.367-1.38,9.833,9.833,0,0,0-2.933-1.7,1.233,1.233,0,0,0-1.393.387,2.607,2.607,0,0,1-4.107,0A1.233,1.233,0,0,0,7.22,1.28,9.88,9.88,0,0,0,4.487,2.813,1.24,1.24,0,0,0,4.1,4.233,2.6,2.6,0,0,1,1.933,7.807,1.233,1.233,0,0,0,.853,8.8a9.427,9.427,0,0,0-.187,1.867,9.547,9.547,0,0,0,.127,1.567A1.233,1.233,0,0,0,1.88,13.267,2.6,2.6,0,0,1,4,16.94a1.213,1.213,0,0,0,.34,1.453,9.907,9.907,0,0,0,2.907,1.673,1.333,1.333,0,0,0,.42.073,1.227,1.227,0,0,0,1-.52A2.58,2.58,0,0,1,10.8,18.5a2.613,2.613,0,0,1,2.093,1.053,1.227,1.227,0,0,0,1.44.407A10,10,0,0,0,17,18.367a1.233,1.233,0,0,0,.36-1.407,2.6,2.6,0,0,1,2.087-3.593,1.233,1.233,0,0,0,1.047-1.013,9.667,9.667,0,0,0,.173-1.687A9.572,9.572,0,0,0,20.5,8.885a1.22,1.22,0,0,0-1.026-.991ZM14,10.667a3.333,3.333,0,1,1-3.333-3.333A3.333,3.333,0,0,1,14,10.667Z"/>
  </svg>
    <p>Настройки</p>
    <svg xmlns="http://www.w3.org/2000/svg" width="1.2093518518518518vh" height="0.7028703703703704vh" viewBox="0 0 13.061 7.591" class="arrow1s">
      <path d="M297.128,108l6,6,6-6" transform="translate(-296.598 -107.47)" fill="none"></path>
    </svg>
  </div>
</li>
</ul>
        </div>
        
        
 <div class="rightmenu" class:active={selectView != "Menu"}  class:shopcar-bg={selectView === 'Shopcar' || selectView === 'Osnovnoe' || selectView === 'Statiskic' || selectView === 'Carsuser' || selectView === 'Housebiz' || selectView === 'Skills' || selectView === 'Q1' || selectView === 'Q2' || selectView === 'Q3'|| selectView === 'Q4' || selectView === 'Pay' || selectView === 'Shopcl' || selectView === 'Shoptaty' || selectView === 'Shopskin' || selectView === 'Settings' }>
      
      <div class="topnav">
        
          {#if selectView === "Shopcl" || selectView === "Shopcar" || selectView === "Ani1m0"  }
          <div class="back__icon" on:click|stopPropagation={() => { if (selectView === 'Shopcl') { handleSelect('Shopcl');  } else if (selectView === 'Shopcar') { handleSelect('Shopcar'); } else if (selectView === 'Ani1m0') { handleSelect('Shop'); } }} class:active={selectView === 'Shopcar'} >
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none">
	<path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"/>
</svg>
</div>
          <div class="bginput">
              <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="16" height="16" viewBox="0 0 16 16">
  <path d="M8.818,932.149a7.177,7.177,0,1,0,4.469,12.779l2.988,2.987a.8.8,0,0,0,1.131-1.129L14.417,943.8a7.165,7.165,0,0,0-5.6-11.649Zm0,1.594a5.582,5.582,0,1,1-5.579,5.579,5.566,5.566,0,0,1,5.579-5.579Z" transform="translate(-1.645 -932.149)"/>
</svg>                       
              <input bind:value={searchText} placeholder="Поиск"> 
          </div>
          {:else if selectView === 'Roulette'}
          <h1>{currentCaseTitle || '...'}</h1>
          {:else if  selectView === "Anim1" || selectView === "Anim2" || selectView === "Anim3" || selectView === "Anim4" || selectView === "Anim5" || selectView === "Anim6" || selectView === "Anim7" || selectView === "Anim8" || selectView === "Shopanim"}
          
          <h1>
    {#if replacements.find(r => selectView === r.word)}
      {replacements.find(r => selectView === r.word).replacement}
    {:else}
      {selectView}
    {/if}
  </h1>
  <div class="bginput">
              <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="16" height="16" viewBox="0 0 16 16">
  <path d="M8.818,932.149a7.177,7.177,0,1,0,4.469,12.779l2.988,2.987a.8.8,0,0,0,1.131-1.129L14.417,943.8a7.165,7.165,0,0,0-5.6-11.649Zm0,1.594a5.582,5.582,0,1,1-5.579,5.579,5.566,5.566,0,0,1,5.579-5.579Z" transform="translate(-1.645 -932.149)"/>
</svg>                      
              <input bind:value={searchText} placeholder="Поиск"> 
          </div>
{:else}
  <h1>
    {#if replacements.find(r => selectView === r.word)}
      {replacements.find(r => selectView === r.word).replacement}
    {:else}
      {selectView}
    {/if}
  </h1>
{/if}
          {#if selectView ==="Shopcl" || selectView === "Shopcar" }
          
          
              <div class="cataloglisttype">
                  <div class="blocktype" class:active={catlist} on:keypress on:click={opencatlisttype}>
                      <svg width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                          <g clip-path="url(#clip0_122_2)">
                          <path d="M10.8525 10.3534L11.8966 11.3975C12.0345 11.5353 12.0345 11.7588 11.8966 11.8966C11.7588 12.0345 11.5353 12.0345 11.3975 11.8966L10.3534 10.8525C9.97402 11.1302 9.50616 11.2941 9 11.2941C7.73299 11.2941 6.70589 10.267 6.70589 9C6.70589 7.73299 7.73299 6.70589 9 6.70589C10.267 6.70589 11.2941 7.73299 11.2941 9C11.2941 9.50616 11.1302 9.97402 10.8525 10.3534ZM9 7.41176C8.12283 7.41176 7.41176 8.12283 7.41176 9C7.41176 9.87717 8.12283 10.5882 9 10.5882C9.87717 10.5882 10.5882 9.87717 10.5882 9C10.5882 8.12283 9.87717 7.41176 9 7.41176ZM0.705891 0H4.2353C4.62516 0 4.94119 0.316031 4.94119 0.705891V4.2353C4.94119 4.62516 4.62516 4.94119 4.2353 4.94119H0.705891C0.316031 4.94119 0 4.62516 0 4.2353V0.705891C0 0.316031 0.316031 0 0.705891 0ZM0.705891 6.70589H4.2353C4.62516 6.70589 4.94119 7.02192 4.94119 7.41178V10.9412C4.94119 11.331 4.62516 11.6471 4.2353 11.6471H0.705891C0.316031 11.6471 0 11.331 0 10.9412V7.41176C0 7.02192 0.316031 6.70589 0.705891 6.70589ZM7.41176 0H10.9412C11.331 0 11.6471 0.316031 11.6471 0.705891V4.2353C11.6471 4.62516 11.331 4.94119 10.9412 4.94119H7.41176C7.0219 4.94119 6.70587 4.62516 6.70587 4.2353V0.705891C6.70589 0.316031 7.02192 0 7.41176 0Z"/>
                          </g>
                          <defs>
                          <clipPath id="clip0_122_2">
                          <rect width="12" height="12" fill="white"/>
                          </clipPath>
                          </defs>
                      </svg>
                      {#if pageload}                            
                          <p>{replacements.find(r => pageload === r.word)?.replacement ?? pageload}</p>
                          {:else}
                          <p>Все</p>
                      {/if}
                      <svg width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                          <g clip-path="url(#clip0_122_6)">
                          <path d="M11.6894 2.56167C11.2753 2.1466 10.6035 2.1466 10.1889 2.56167L6.00017 7.18501L1.81141 2.56167C1.39683 2.1466 0.725501 2.1466 0.310938 2.56167C-0.103646 2.97675 -0.103646 3.64907 0.310938 4.06414L5.18351 9.44271C5.40778 9.66698 5.70648 9.76588 6.00019 9.74741C6.29338 9.76588 6.59259 9.66698 6.81684 9.44271L11.6894 4.06414C12.1035 3.64907 12.1035 2.97676 11.6894 2.56167Z"/>
                          </g>
                          <defs>
                          <clipPath id="clip0_122_6">
                          <rect width="12" height="12" fill="white"/>
                          </clipPath>
                          </defs>
                      </svg>                            
                  </div>

                  
                  <ul class:active={catlist}>
                  {#if selectView === 'Shopcar'}
                      <li on:keypress={() => {}} on:click={() => onSelectTypeCat(0)}>Все</li>
                      <li on:keypress={() => {}} on:click={() => onSelectTypeCat(1)}>Автомобили</li>
                      <li on:keypress={() => {}}  on:click={() => onSelectTypeCat(2)}>Вертолеты</li>
  {:else}
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (0)}>Все</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (4)}>Головные уборы</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (5)}>Верх</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (6)}>Майки</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (7)}>Верхняя одежда</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (8)}>Низ</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (9)}>Обувь</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (10)}>Очки</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (11)}>Наручные часы/Браслет</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (12)}>Браслет</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (13)}>Серьги</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (14)}>Маски</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (15)}>Аксессуары</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (16)}>Бронежилеты</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (17)}>Нашивки</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (18)}>Рюкзаки</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeCat (19)}>Перчатки</li>
                      
                      {/if}

                      
                  </ul>
              </div>
              <div class="cataloglisttype">
                  <div class="blocktype" class:active={catFilter} on:keypress on:click={togglePriceFilter}>
                      <svg width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                          <g clip-path="url(#clip0_130_15)">
                          <path d="M10.8621 2.338H1.13788C0.988418 2.33813 0.840397 2.30878 0.702288 2.25164C0.564179 2.1945 0.438691 2.11069 0.333005 2.00501C0.227319 1.89932 0.143509 1.77383 0.0863692 1.63572C0.0292296 1.49761 -0.000117729 1.34959 6.25548e-06 1.20013C-0.000117729 1.05067 0.0292296 0.902647 0.0863692 0.764538C0.143509 0.626429 0.227319 0.500941 0.333005 0.395255C0.438691 0.289569 0.564179 0.205759 0.702288 0.148619C0.840397 0.0914796 0.988418 0.0621323 1.13788 0.0622563H10.8621C11.0116 0.0621323 11.1596 0.0914796 11.2977 0.148619C11.4358 0.205759 11.5613 0.289569 11.667 0.395255C11.7727 0.500941 11.8565 0.626429 11.9136 0.764538C11.9708 0.902647 12.0001 1.05067 12 1.20013C12.0001 1.34959 11.9708 1.49761 11.9136 1.63572C11.8565 1.77383 11.7727 1.89932 11.667 2.00501C11.5613 2.11069 11.4358 2.1945 11.2977 2.25164C11.1596 2.30878 11.0116 2.33813 10.8621 2.338ZM8.30922 6.00145C8.30947 5.85186 8.28022 5.7037 8.22315 5.56543C8.16607 5.42715 8.0823 5.30149 7.97661 5.19563C7.87093 5.08977 7.7454 5.00579 7.60723 4.94849C7.46905 4.89118 7.32093 4.86169 7.17135 4.86169H1.13788C0.988294 4.86169 0.840175 4.89118 0.701999 4.94849C0.563823 5.00579 0.438301 5.08977 0.332614 5.19563C0.226928 5.30149 0.143153 5.42715 0.08608 5.56543C0.0290075 5.7037 -0.00024161 5.85186 6.25548e-06 6.00145C0.000256148 6.30307 0.120249 6.59225 0.333615 6.80544C0.546981 7.01863 0.836261 7.13838 1.13788 7.13838H7.17135C7.47297 7.13838 7.76225 7.01863 7.97561 6.80544C8.18898 6.59225 8.30897 6.30307 8.30922 6.00145ZM8.30922 10.8009C8.30934 10.6514 8.28 10.5034 8.22286 10.3653C8.16572 10.2272 8.08191 10.1017 7.97622 9.99601C7.87054 9.89032 7.74505 9.80651 7.60694 9.74937C7.46883 9.69223 7.32081 9.66289 7.17135 9.66301H1.13788C0.988497 9.66289 0.840554 9.6922 0.702506 9.74928C0.564458 9.80636 0.439013 9.89009 0.333339 9.99568C0.227665 10.1013 0.143835 10.2266 0.0866401 10.3646C0.029445 10.5026 6.20412e-06 10.6506 6.25548e-06 10.7999C-0.000490318 10.9496 0.0285821 11.098 0.0855548 11.2364C0.142528 11.3748 0.226279 11.5007 0.332002 11.6066C0.437725 11.7126 0.563339 11.7967 0.701633 11.854C0.839927 11.9113 0.988179 11.9408 1.13788 11.9406H7.17135C7.32105 11.9408 7.4693 11.9113 7.60759 11.854C7.74589 11.7967 7.8715 11.7126 7.97723 11.6066C8.08295 11.5007 8.1667 11.3748 8.22367 11.2364C8.28065 11.098 8.30972 10.9496 8.30922 10.7999V10.8009Z"/>
                          </g>
                          <defs>
                          <clipPath id="clip0_130_15">
                          <rect width="12" height="12" fill="white"/>
                          </clipPath>
                          </defs>
                      </svg>                                                          
                      <p>{replacements.find(r => pagefilter === r.word)?.replacement ?? pagefilter}</p>
                      <svg width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                          <g clip-path="url(#clip0_122_6)">
                          <path d="M11.6894 2.56167C11.2753 2.1466 10.6035 2.1466 10.1889 2.56167L6.00017 7.18501L1.81141 2.56167C1.39683 2.1466 0.725501 2.1466 0.310938 2.56167C-0.103646 2.97675 -0.103646 3.64907 0.310938 4.06414L5.18351 9.44271C5.40778 9.66698 5.70648 9.76588 6.00019 9.74741C6.29338 9.76588 6.59259 9.66698 6.81684 9.44271L11.6894 4.06414C12.1035 3.64907 12.1035 2.97676 11.6894 2.56167Z"/>
                          </g>
                          <defs>
                          <clipPath id="clip0_122_6">
                          <rect width="12" height="12" fill="white"/>
                          </clipPath>
                          </defs>
                      </svg>                            
                  </div>
                  <ul class:active={catFilter}>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeFilter (20)}>Цена. По убыванию</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeFilter (21)}>Цена. По возрастанию</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeFilter (22)}>Редкость. По убыванию</li>
                      <li on:keypress={() => {}} on:click={()=> onSelectTypeFilter (23)}>Редкость. По возрастанию</li>
                  </ul>
              </div>
          {/if}
          
          <div class="balance">
              <svg class="icon"id="Group_1326" data-name="Group 1326" xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 22 22">
                  <circle id="Ellipse_21" data-name="Ellipse 21" cx="11" cy="11" r="11" fill="#e81c5a"/>
                  <path id="Path_6385" data-name="Path 6385" d="M6.193-3.412a2,2,0,0,0-.522,1.031c0,.378.183,1.318.692,1.318.326,0,.953-.744,1.148-.992A27.023,27.023,0,0,1,12.5-7.119l.026.026c-.914,1.188-2.571,3.145-2.571,4.711,0,.757.287,1.579,1.148,1.579,1.122,0,3.928-2.219,4.241-3.119l-.052-.5c-1.762,1.71-2.78,2.3-3.145,2.3-.313,0-.457-.378-.457-.639a5.748,5.748,0,0,1,1.5-2.962l2.023-2.467c.157-.2.561-.653.561-.914s-.548-1.122-.9-1.122c-.235,0-.5.274-.666.418C12.5-8.45,11-7.027,9.391-5.644L9.364-5.67c.535-.822,1.945-2.754,1.945-3.719a.936.936,0,0,0-.953-.9c-.535.013-1.644.992-2.127,1.449L6.154-6.832l-.026-.026a16.464,16.464,0,0,0,.809-2.271.967.967,0,0,0-.157-.5l-.313-.548c-.131-.222-.183-.4-.5-.418A11.83,11.83,0,0,0,3.5-9.677L2.03-9,2-9.024c.509-.444.692-.679.692-.757s-.091-.131-.157-.131a.81.81,0,0,0-.326.183c-1.161.822-1.083.8-1.2.953a7.615,7.615,0,0,0-.339,1.54c-.065.313.1.483.313.483.666,0,.039-.026,4.059-1.905a1.3,1.3,0,0,1,.4-.157c.222,0,.457.261-.914,2.714L1.808-2.655c.144.4.4,1.436.953,1.436.418,0,.744-.5.966-.8L5.684-4.652,8.816-8.045c.091-.1.248-.274.4-.274s.2.144-.026.561A19.4,19.4,0,0,1,7.081-4.574Z" transform="translate(3.129 16.731)" fill="#fff"/>
                </svg>                      
              <p>{formatMoney( $accountRedbucks)}</p>                           
          </div>
          <div class="btnbuy" on:click|stopPropagation={() => handleSelect('Pay')} >
              <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <g clip-path="url(#clip0_472_639)">
                  <path d="M7.99935 0.666626C6.54896 0.666626 5.13113 1.09672 3.92517 1.90252C2.71921 2.70831 1.77928 3.85362 1.22424 5.19361C0.669193 6.53361 0.523969 8.00809 0.806927 9.43062C1.08989 10.8531 1.78832 12.1598 2.8139 13.1854C3.83949 14.211 5.14616 14.9094 6.56869 15.1924C7.99122 15.4753 9.46571 15.3301 10.8057 14.7751C12.1457 14.22 13.291 13.2801 14.0968 12.0741C14.9026 10.8682 15.3327 9.45036 15.3327 7.99996C15.3327 6.05504 14.5601 4.18978 13.1848 2.81451C11.8095 1.43924 9.94427 0.666626 7.99935 0.666626ZM11.3327 8.66663H8.66602V11.3333C8.66602 11.5101 8.59578 11.6797 8.47076 11.8047C8.34573 11.9297 8.17616 12 7.99935 12C7.82254 12 7.65297 11.9297 7.52795 11.8047C7.40292 11.6797 7.33269 11.5101 7.33269 11.3333V8.66663H4.66602C4.48921 8.66663 4.31964 8.59639 4.19461 8.47136C4.06959 8.34634 3.99935 8.17677 3.99935 7.99996C3.99935 7.82315 4.06959 7.65358 4.19461 7.52855C4.31964 7.40353 4.48921 7.33329 4.66602 7.33329H7.33269V4.66663C7.33269 4.48981 7.40292 4.32025 7.52795 4.19522C7.65297 4.0702 7.82254 3.99996 7.99935 3.99996C8.17616 3.99996 8.34573 4.0702 8.47076 4.19522C8.59578 4.32025 8.66602 4.48981 8.66602 4.66663V7.33329H11.3327C11.5095 7.33329 11.6791 7.40353 11.8041 7.52855C11.9291 7.65358 11.9994 7.82315 11.9994 7.99996C11.9994 8.17677 11.9291 8.34634 11.8041 8.47136C11.6791 8.59639 11.5095 8.66663 11.3327 8.66663Z"/>
                  </g>
                  <defs>
                  <clipPath id="clip0_472_639">
                  <rect width="16" height="16" fill="white"/>
                  </clipPath>
                  </defs>
              </svg>                            
              <p>Пополнить</p>
          </div>
          <div class="esc">
              <p>Выход</p>
              <span>ESC</span>
          </div>
      </div>
      <div class="menu">
          <svelte:component this={Views[selectView]} on:casetitle={(e) => currentCaseTitle = e.detail} {pagefilter} {pageload} {itemliststock} {carsList} {searchText} {visible} {selectView} />
      </div>
  </div>
</div>