<script>
  import { storeSettings } from 'store/settings';
  import { storeSettings as menuStore } from 'store/settings';

  export let settingKey;
  export let settingValue = [];
  export let settingItem = null;
  export let disabled = false;

  let isActive = false;

  $: displayValue = settingValue && settingValue.length > 0 
    ? settingValue.join(" + ") 
    : (settingItem?.default ? settingItem.default.join(" + ") : "Нажмите клавишу");

  function startListen() {
    if (disabled || isActive) return;
    isActive = true;
    console.log("Listening for key bind:", settingKey);
    // Отправить сигнал на backend
  }
</script>

<div class="default-bind">
  <div class="bind" class:active={isActive} class:disabled>
    <p class="tooltip2">Нажмите клавишу</p>
    <button 
      class="bind-value"
      class:active={isActive}
      disabled={disabled}
      on:click={startListen}
      
    >{displayValue}
      {#if isActive}
        ↑
      {/if}
    </button>
  </div>
</div>