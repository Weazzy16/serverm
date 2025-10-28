<script>
  export let value = 0;
  export let min = 0;
  export let max = 100;
  export let label = '';

  let inputRef;

  function handleChange(e) {
    value = parseFloat(e.target.value);
  }
</script>

<div data-v-483fc7f7 class="sliderC">
  <span data-v-483fc7f7>{label}</span>
  <input 
    bind:this={inputRef}
    data-v-483fc7f7
    type="range"
    {min}
    {max}
    bind:value
    on:change={handleChange}
    on:input={handleChange}
    class="vue-sliderC"
  />
</div>

<style>
  input[type="range"] {
    width: 100%;
    height: 4px;
    cursor: pointer;
  }

  input[type="range"]::-webkit-slider-thumb {
    appearance: none;
    width: 14px;
    height: 14px;
    border-radius: 50%;
    background: #ff6b9d;
    border: 2px solid white;
    cursor: pointer;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.5);
  }

  input[type="range"]::-moz-range-thumb {
    width: 14px;
    height: 14px;
    border-radius: 50%;
    background: #ff6b9d;
    border: 2px solid white;
    cursor: pointer;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.5);
  }
</style>