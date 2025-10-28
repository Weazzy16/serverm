<script>
  export let settingKey;
  export let bindValue = [];
  export let disabled = false;
  
  import { storeSettings } from 'store/settings';

  let isListening = false;

  function startListen() {
    if (disabled || isListening) return;
    isListening = true;
    console.log("Listening for key bind:", settingKey);
    // Здесь нужно отправить на backend чтобы начать слушать клавиши
  }

  function stopListen() {
    isListening = false;
  }

  function clearBind() {
    storeSettings.update(v => ({
      ...v,
      [settingKey]: []
    }));
  }
</script>

<div class="keybind-control" class:disabled class:listening={isListening}>
  <button 
    class="keybind-btn"
    class:active={isListening}
    disabled={disabled}
    on:click={startListen}
  >
    {#if isListening}
      Ожидание...
    {:else if bindValue && bindValue.length > 0}
      {bindValue.join(" + ")}
    {:else}
      Нажмите клавишу
    {/if}
  </button>
  
  {#if bindValue && bindValue.length > 0}
    <button 
      class="keybind-clear"
      disabled={disabled}
      on:click={clearBind}
      title="Очистить"
    >
      ✕
    </button>
  {/if}
</div>
