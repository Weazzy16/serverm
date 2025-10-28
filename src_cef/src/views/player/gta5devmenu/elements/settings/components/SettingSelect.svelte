<script>
  // Select row: label слева, control справа.
  // bind:value поддерживается.
  import { createEventDispatcher } from 'svelte';
  export let label = '';
  export let value = '';
  export let options = []; // [{ value, label }]
  export let onPrev = null;
  export let onNext = null;
  const dispatch = createEventDispatcher();

  // Найти индекс текущего значения
  $: currentIndex = options.findIndex(o => o.value === value);
  if (currentIndex === -1 && options.length) {
    // если value пустое/не найдено — выставляем первый
    value = options[0].value;
  }

  function prev() {
    if (!options.length) return;
    const idx = (options.findIndex(o => o.value === value) - 1 + options.length) % options.length;
    value = options[idx].value;
    dispatch('change', { value });
    if (onPrev) onPrev();
  }
  function next() {
    if (!options.length) return;
    const idx = (options.findIndex(o => o.value === value) + 1) % options.length;
    value = options[idx].value;
    dispatch('change', { value });
    if (onNext) onNext();
  }
</script>

<div class="setting-row select-row" role="group" aria-label={label}>
  <div class="setting-row__label">
    <div class="setting-title">{label}</div>
  </div>

  <div class="setting-row__control select-control">
    <button class="select-arrow left" on:click={prev} type="button" aria-label="previous">‹</button>

    <!-- нативный select для фокусировки и доступности -->
    <select bind:value aria-label={label}>
      {#each options as opt}
        <option value={opt.value}>{opt.label}</option>
      {/each}
    </select>

    <button class="select-arrow right" on:click={next} type="button" aria-label="next">›</button>
  </div>
</div>