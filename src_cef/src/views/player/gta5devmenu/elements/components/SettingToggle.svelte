<script>
  // Toggle row: label слева, переключатель справа.
  // Поддерживает bind:checked
  import { createEventDispatcher } from 'svelte';
  export let label = '';
  export let checked = false;
  export let hint = '';
  const dispatch = createEventDispatcher();

  function toggle() {
    checked = !checked;
    dispatch('change', { checked });
  }
</script>

<div class="setting-row toggle-row" role="group" aria-label={label}>
  <div class="setting-row__label">
    <div class="setting-title">{label}</div>
    {#if hint}
      <div class="setting-hint">{hint}</div>
    {/if}
  </div>

  <div class="setting-row__control">
    <!-- semantic accessible switch -->
    <button
      class="setting-switch"
      role="switch"
      aria-checked={checked}
      aria-label={label}
      on:click={toggle}
      type="button"
    >
      <span class="switch-knob" />
    </button>
  </div>
</div>