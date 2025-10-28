<script>
  import { createEventDispatcher } from "svelte";

  // Compatible API with original:
  // - prop: state (boolean) or checked (bindable)
  // - emits: changeValue (boolean), change (boolean)
  // Supports bind:checked or bind:state
  export let checked = false; // bind:checked support
  export let state = undefined; // alias; if provided, prefer it internally
  const dispatch = createEventDispatcher();

  // internal current state (ties to either state or checked)
  $: current = (state === undefined || state === null) ? checked : state;

  function toggle() {
    const next = !current;
    // update both exported values to support binding
    checked = next;
    state = next;
    dispatch("changeValue", next);
    dispatch("change", next);
  }

  function onKeydown(e) {
    if (e.key === "Enter" || e.key === " ") {
      e.preventDefault();
      toggle();
    }
  }
</script>

<button
  class="switch {current ? 'active' : ''}"
  role="switch"
  aria-checked={current}
  on:click={toggle}
  on:keydown={onKeydown}
  type="button"
>
  <span class="point" />
</button>