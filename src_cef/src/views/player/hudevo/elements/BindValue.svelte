<script>
  import SVGComponent from './SVGComponent.svelte';
  
  export let bind = [];
  export let isActive = false;
  
  const iconsForKeys = ['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Space', 'Shift', 'Ctrl', 'Alt', 'Tab', 'Enter', 'Escape'];
  
  const keyCodeMap = {
    'ArrowUp': 38,
    'ArrowDown': 40,
    'ArrowLeft': 37,
    'ArrowRight': 39,
    'Space': 32,
    'Shift': 16,
    'Ctrl': 17,
    'Alt': 18,
    'Tab': 9,
    'Enter': 13,
    'Escape': 27
  };
  
  function getKeyCode(keyId) {
    return keyCodeMap[keyId] || keyId;
  }
  
  function getKeyName(key) {
    return key;
  }
</script>

<div class="bind-value">
  <div class="keys">
    {#if bind && bind.length > 0}
      {#each bind as key, index}
        {#if iconsForKeys.includes(key)}
          <SVGComponent class="key-icon"
            path="icons/main/keys/{getKeyCode(key)}.svg" 
            style="    filter: invert(100%) sepia(0) saturate(0) hue-rotate(249deg) brightness(107%) contrast(105%);"
          />
        {:else}
          <span>{getKeyName(key)}</span>
        {/if}
        
        {#if index < bind.length - 1}
          <span>+</span>
        {/if}
      {/each}
      
      {#if isActive}
        <span>....</span>
      {/if}
    {:else}
      <span>....</span>
    {/if}
  </div>
  
  <slot name="title" />
</div>