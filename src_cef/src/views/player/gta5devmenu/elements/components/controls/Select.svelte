<script>
  import { createEventDispatcher, onMount } from "svelte";

  // Совместимый Select:
  // - принимает либо `list`, либо `options` (оба работают)
  // - bind:value (value) — канонический внешний prop
  // - эмитит "change" (value) и "changeValue" (index)
  const dispatch = createEventDispatcher();

  export let list = undefined;      // preferred prop name
  export let options = undefined;   // alias for compatibility
  export let value = undefined;
  export let isSmall = false;

  // use the provided array (options or list)
  $: rawList = Array.isArray(list) ? list : (Array.isArray(options) ? options : []);

  // normalize items into { value, label, icon? }
  $: normalized = rawList.map((it) => {
    if (it && typeof it === "object") {
      return {
        value: it.value ?? it.label ?? it,
        label: it.label ?? String(it.value ?? it),
        icon: it.icon
      };
    }
    return { value: it, label: String(it) };
  });

  // internal index (single source of truth inside)
  let index = 0;

  onMount(() => {
    if (!normalized.length) {
      // nothing to show
      index = 0;
      return;
    }
    if (value === undefined || value === null) {
      index = 0;
      value = normalized[0].value;
      // emit initial state so parent knows
      dispatch("changeValue", index);
      dispatch("change", value);
    } else {
      const idx = normalized.findIndex((i) => i.value === value);
      index = idx >= 0 ? idx : 0;
      // keep value consistent if it didn't match
      if (normalized[index] && normalized[index].value !== value) {
        value = normalized[index].value;
        dispatch("change", value);
      }
    }
  });

  // if external `value` changes — update index (one-way)
  $: if (value !== undefined && value !== null && normalized.length) {
    const idx = normalized.findIndex((i) => i.value === value);
    if (idx >= 0 && idx !== index) index = idx;
  }

  function setIndex(idx) {
    if (!normalized.length) return;
    index = idx;
    const newVal = normalized[idx].value;
    if (newVal !== value) {
      value = newVal;
      dispatch("changeValue", idx);
      dispatch("change", value);
    } else {
      // still notify index change
      dispatch("changeValue", idx);
    }
  }

  function prev() {
    if (!normalized.length) return;
    const idx = index > 0 ? index - 1 : normalized.length - 1;
    setIndex(idx);
  }

  function next() {
    if (!normalized.length) return;
    const idx = index < normalized.length - 1 ? index + 1 : 0;
    setIndex(idx);
  }

  function onNativeChange(e) {
    const idx = e.target.selectedIndex;
    setIndex(idx);
  }
</script>

<div class="select {isSmall ? 'isSmall' : ''}" role="group" aria-label="select control">
  <div class="btn left {normalized.length <= 1 ? 'disabled' : ''}" on:click={prev} aria-disabled={normalized.length <= 1} role="button">
    <slot name="prevBtnImg">
           <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14" fill="none">
	<path d="M12 4.5C11.595 5.20125 7 9.5 7 9.5L2 4.5"/>
</svg>
    </slot>
  </div>

  <div class="item-wrapper" aria-hidden="false">
    <slot name="img">
      {#if normalized[index]?.icon}
        <img src={normalized[index].icon} alt="" />
      {/if}
    </slot>

    <slot name="listValue">
      <p>{normalized[index]?.label ?? "N/A"}</p>
    </slot>
  </div>

  <div class="btn right {normalized.length <= 1 ? 'disabled' : ''}" on:click={next} aria-disabled={normalized.length <= 1} role="button">
    <slot name="nextBtnImg">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14" fill="none">
	<path d="M12 4.5C11.595 5.20125 7 9.5 7 9.5L2 4.5"/>
</svg>
    </slot>
  </div>

  <!-- native select hidden for keyboard/navigability -->
  <select on:change={onNativeChange} aria-hidden="true" tabindex="-1" style="position:absolute;opacity:0;pointer-events:none;left:-9999px;">
    {#each normalized as item}
      <option>{item.label}</option>
    {/each}
  </select>
</div>