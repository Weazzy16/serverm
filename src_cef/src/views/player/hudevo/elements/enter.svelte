<script>
  import { fade } from 'svelte/transition';
  import SVGComponent from './SVGComponent.svelte';
  
  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  
  let visible = false;
  let hintText = '';
  let hintKeys = [];
  
  const iconsForKeys = ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Space', 'Shift', 'Ctrl', 'Alt', 'Tab', 'Enter', 'Escape'];
  
  const keyCodeMap = {
    'E': 69,
    'Tab': 9,
    'ArrowRight': 39,
    'ArrowLeft': 37,
    'Space': 32,
    'Enter': 13,
    'Escape': 27
  };
  
  // СТАРЫЙ формат данных (из игры)
  const InfoData = {
    Warehouse: { text: 'Открыть склад', key: 'E' },
    DriveSchool: { text: 'Открыть меню автошколы', key: 'E' },
    RentCar: { text: 'Открыть меню аренды', key: 'E' },
    Festive: { text: 'Открыть обмен Coin', key: 'E' },
    SaluteShop: { text: 'Открыть магазин фейерверков', key: 'E' },
    BlackMarket: { text: 'Открыть чёрный рынок', key: 'E' },
    TankRoyale: { text: 'Регистрация Tank Royale', key: 'E' },
    HealkitHouse: { text: 'Использовать аптечку', key: 'E' },
    EnterHotel: { text: 'Войти в отель', key: 'E' },
    Electrician: { text: 'Переодеться', key: 'E' },
    Repair: { text: 'Ремонтировать транспорт', key: 'E' },
    SeatingArrangements: { text: 'Взаимодействовать', key: 'E' },
    BuyPoints: { text: 'Купить алкоголь', key: 'E' },
    EnterHouse: [
      { text: 'Войти в дом', key: 'Tab' },
      { text: 'Открыть/закрыть дом', key: 'ArrowRight' }
    ],
    ExitHouse: { text: 'Выйти из дома', key: 'E' },
    EnterGarage: { text: 'Войти в гараж', key: 'E' },
    ExitGarage: { text: 'Выйти из гаража', key: 'E' },
    Casino: { text: 'двери в казино', key: 'E' },
    Atm: { text: 'Открыть меню банкомата', key: 'E' },
    Seating: { text: 'Чтобы сесть', key: 'E' },
    ExitHotel: { text: 'Выйти из отеля', key: 'E' },
    CarRentHotel: { text: 'Арендовать скутер', key: 'E' },
    FractionCityhallBeginWorkDay: { text: 'Раздевалка CityHall', key: 'E' },
    FractionCityhallGunMenu: { text: 'Оружейный склад CityHall', key: 'E' },
    JobGoPosta: { text: 'Начать работу почтальона', key: 'E' },
    JobGoPostaCar: { text: 'Взять рабочий транспорт', key: 'E' },
    TakeMoney: { text: 'Взять мешок с деньгами', key: 'E' },
    ChangeAutoNumber: { text: 'Открыть меню смены номеров', key: 'E' },
    Bear: { text: 'Нажмите чтобы поднять мишку', key: 'E' },
    BoatFix: { text: 'Нажмите чтобы починить лодку', key: 'E' },
    buyMetro: { text: 'Купить билет', key: 'E' },
    exitMetro: { text: 'Для выхода', key: 'E' },
    SeatingUp: { text: 'Встать', key: 'E' },
    ElectionPoint: { text: 'Открыть меню выборов', key: 'E' },

    // добавь остальные...
  };
  
  function getKeyCode(keyId) {
    return keyCodeMap[keyId] || keyId;
  }
  
  // АДАПТЕР для старого API
  if (typeof window !== 'undefined') {
    window.hudEnter = (text) => {
      if (text === -1) {
        visible = false;
        hintText = '';
        hintKeys = [];
      } else {
        const data = InfoData[text];
        
        if (!data) {
          // Если нет в словаре, показываем как есть
          visible = true;
          hintText = 'Нажмите для взаимодействия';
          hintKeys = ['E'];
        } else if (Array.isArray(data)) {
          // Несколько подсказок (как EnterHouse)
          visible = true;
          hintText = data.map(d => d.text).join(' / ');
          hintKeys = data.map(d => d.key);
        } else {
          // Одна подсказка
          visible = true;
          hintText = data.text;
          hintKeys = [data.key];
        }
      }
    };
  }
</script>

{#if visible}
  <div class="hint-container" transition:fade>
    <div class="hint row-block align-center">
      <span class="hint__text">Нажмите</span>
      
      {#each hintKeys as key, index}
        <div class="hint-icon">
          {#if iconsForKeys.includes(key)}
            <img 
              src="{cdn}icons/main/keys/{getKeyCode(key)}.svg" 
              alt="{key}"
              class="hint-icon__picture"
            />
          {:else}
            <span class="hint-icon__text">{key}</span>
          {/if}
        </div>
        
        {#if index < hintKeys.length - 1}
          <span class="hint__text">+</span>
        {/if}
      {/each}
      
      <span class="hint__text">{hintText}</span>
    </div>
  </div>
{/if}

<style>
  /* Твои стили */
</style>