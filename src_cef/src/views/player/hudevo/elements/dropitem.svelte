<script>
  import { fade } from 'svelte/transition';
  import SVGComponent from './SVGComponent.svelte';
  
  export let show = false;
  export let id = null;
  export let actions = [];
  export let type = null;
  export let crosshair = false;
  export let entityType = null;
  export let item = null;
  export let keyName = 'E';
  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  
  let cooldown = false;
  
  // Маппинг для иконок (если нужно)
  const keyCodeMap = {
    'E': 69,
    'F': 70,
    'G': 71,
    // добавь другие если нужно
  };
  
  function getKeyCode(key) {
    return keyCodeMap[key] || key;
  }
  
  // Функции для работы с предметами
  function getItemTitle(item) {
    return item?.Name || 'Неизвестный предмет';
  }
  
  function getItemDescription(item) {
    return item?.Description || '';
  }
  
  function isItemClothes(itemId) {
    // Логика определения одежды
    return false;
  }
  
  function getTitle(item) {
    if (isItemClothes(item.itemId)) {
      return `${getItemTitle(item)} - ${getItemDescription(item)}`;
    }
    return getItemTitle(item);
  }
  
  // Вычисление состояния оружия
  $: weaponCondition = item?.condition != null && item?.limit != null
    ? 100 - Math.round((item.condition * 100) / (item.limit || 1000))
    : item?.condition != null 
    ? Math.round(item.condition)
    : 0;
    
  $: percentColor = weaponCondition < 20 
    ? 'red' 
    : weaponCondition < 50 
    ? 'orange' 
    : weaponCondition < 70 
    ? 'yellow' 
    : 'green';
  
  // Обработчики событий
  function select(action) {
    if (cooldown) return;
    // Вызов клиента (замени на свой метод)
    console.log('interaction.select', action, id, type);
    // mp.trigger('interaction.select', JSON.stringify(action), id, type);
  }
  
  function close() {
    // Вызов клиента (замени на свой метод)
    console.log('interaction.exit');
    // mp.trigger('interaction.exit');
  }
  
  function onKeyUp(e) {
    if (cooldown) {
      cooldown = false;
      return;
    }
    
    if (e.keyCode === 27) { // ESC
      e.preventDefault();
      close();
    }
  }
  
  function onKeyDown(e) {
    if (cooldown) return;
    
    cooldown = true;
    
    const numberKeys = [49, 50, 51, 52, 53, 54, 55, 56, 57]; // 1-9
    const keyIndex = numberKeys.findIndex(k => k === e.keyCode);
    
    if (keyIndex !== -1 && keyIndex < actions.length) {
      select(actions[keyIndex]);
    }
  }
  
  // Подключение/отключение слушателей
  $: if (show) {
    window.addEventListener('keyup', onKeyUp);
    window.addEventListener('keydown', onKeyDown);
  } else {
    window.removeEventListener('keyup', onKeyUp);
    window.removeEventListener('keydown', onKeyDown);
  }
</script>

<div class="interaction-main full-width full-height">
  <!-- Кроссхэйр с предметом -->
  {#if !actions?.length && crosshair && item}
    <div class="interaction-button row-block" transition:fade>
      <div class="letter-container">
        {keyName}
      </div>
      
      <span class="interaction-button__title full-height align-center">
        Поднять "{getTitle(item)}"
      </span>
      
      {#if item.count}
        <span class="interaction-button__text-amount full-height align-center">
          {item.count} шт
        </span>
      {/if}
      
      {#if item.condition != null}
        <span class="interaction-button__text-percent full-height align-center {percentColor}">
          {weaponCondition}%
        </span>
      {/if}
    </div>
  {/if}
  

</div>
