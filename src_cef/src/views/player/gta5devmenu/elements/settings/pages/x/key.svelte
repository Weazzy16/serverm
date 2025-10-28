<script>
  import { bindList } from './state.js'

  export let title;
  export let style = "";
  export let disabled = false;
  export let code = -1;

  let active = false; 
  let use = false;
  let bindingNames = [];

  $: {
    if ($bindList) {
      let success = false;
      if ($bindList[code]) {                
        active = true;
        success = true;
        bindingNames = Array.isArray($bindList[code]) 
          ? $bindList[code] 
          : [$bindList[code]];
      }
      if (active && !success) {
        active = false;
        bindingNames = [];
      }
    }
  }

  const handleKeyDown = (event) => {
    if (!active && !disabled) {
      const { keyCode } = event;
      if (keyCode !== code) return;
      use = true; 
    }
  }

  const handleKeyUp = (event) => {
    if (use) {
      use = false; 
    }
  }
</script>

<svelte:window on:keydown={handleKeyDown} on:keyup={handleKeyUp} />

{#if (active && $bindList[code])}    
  <div title={bindingNames.join(', ')} class="key DCS {style}" class:disabled={disabled} class:active={active} class:down={use}>
    <div class="keycap">
      {title}
    </div>
  </div>
{:else}
  <div class="key DCS {style}" class:disabled={disabled} class:active={active} class:down={use}>
    <div class="keycap">
      {title}
    </div>
  </div>
{/if}