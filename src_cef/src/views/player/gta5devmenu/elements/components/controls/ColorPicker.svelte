<script>
  export let settingKey = null;
  export let settingValue = {};
  export let settingItem = null;
  export let disabled = false;

  import { createEventDispatcher } from 'svelte';
  const dispatch = createEventDispatcher();

  function toggleColor(colorKey) {
    if (disabled) return;
    dispatch('toggleColor', colorKey);
  }
</script>

<div data-v-dedcb410 class="color-picker">
  {#each Object.entries(settingValue || {}) as [colorKey, colorValue] (colorKey)}
    <div 
      data-v-0d503cc7
      class="color-picker"
    >
      <span data-v-0d503cc7>{colorKey}</span>
      <div 
        data-v-0d503cc7
        class="btn"
        style="background-color: {colorValue || '#ffffff'}"
        on:click={() => toggleColor(colorKey)}
      >
        <svg class="icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="none">
  <path d="M13.3347 9.33396C13.2733 9.37663 12.0013 10.7226 12.0013 11.6673C12.0013 12.6633 12.634 13.2993 13.3347 13.334C13.9387 13.3633 14.668 12.74 14.668 11.6673C14.668 10.6673 13.396 9.37663 13.3347 9.33396ZM6.39201 13.334C6.64401 13.586 6.97868 13.7246 7.33468 13.7246C7.69068 13.7246 8.02534 13.586 8.27734 13.334L12.944 8.6673L12.4727 8.19596L7.33468 3.05796L5.80601 1.5293L4.86334 2.47196L6.39201 4.00063L2.66801 7.72463C2.41601 7.97663 2.27734 8.3113 2.27734 8.6673C2.27734 9.0233 2.41601 9.35796 2.66801 9.60996L6.39201 13.334ZM7.33468 4.9433L11.0587 8.6673H3.61068L7.33468 4.9433Z"/>
</svg>
      </div>
    </div>
  {/each}
</div>