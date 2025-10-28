<script>
  import { createEventDispatcher } from 'svelte';
  import { onMount } from 'svelte';

  export let colorKey = null;
  export let configColor = null;
  export let settingKey = null;
  export let settingValue = {};
  export let defaultColor = '#FF0000FF';

  const dispatch = createEventDispatcher();

  let hue = 0;
  let saturation = 100;
  let lightness = 50;
  let alpha = 0;
  let colorCode = 'FF0000';
  let colorError = false;
  let rootElement;

  function hexToHsla(hex) {
    const hexRegex = /^#?([a-f0-9]{6}|[a-f0-9]{8})$/i;
    if (!hexRegex.test(hex)) return null;
    
    hex = hex.replace('#', '');
    const r = parseInt(hex.substring(0, 2), 16) / 255;
    const g = parseInt(hex.substring(2, 4), 16) / 255;
    const b = parseInt(hex.substring(4, 6), 16) / 255;
    const a = hex.length === 8 ? parseInt(hex.substring(6, 8), 16) / 255 : 1;

    const max = Math.max(r, g, b);
    const min = Math.min(r, g, b);
    let h = 0, s = 0;
    const l = (max + min) / 2;

    if (max !== min) {
      const d = max - min;
      s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
      
      switch (max) {
        case r: h = ((g - b) / d + (g < b ? 6 : 0)) / 6; break;
        case g: h = ((b - r) / d + 2) / 6; break;
        case b: h = ((r - g) / d + 4) / 6; break;
      }
    }

    return {
      h: Math.round(h * 360),
      s: Math.round(s * 100),
      l: Math.round(l * 100),
      a: Math.round(a * 100) / 100
    };
  }

  function hslaToHex(h, s, l, a) {
    s /= 100;
    l /= 100;

    const k = n => (n + h / 30) % 12;
    const c = l < 0.5 ? 2 * s * l : s * (2 - 2 * l);
    const x = c * (1 - Math.abs(k(0) % 2 - 1));
    
    let r = 0, g = 0, b = 0;
    if (h < 60) { r = c; g = x; }
    else if (h < 120) { r = x; g = c; }
    else if (h < 180) { g = c; b = x; }
    else if (h < 240) { g = x; b = c; }
    else if (h < 300) { r = x; b = c; }
    else { r = c; b = x; }

    const m = l - c / 2;
    const toHex = (n) => {
      const hex = Math.round((n + m) * 255).toString(16);
      return hex.length === 1 ? '0' + hex : hex;
    };

    const hex = toHex(r) + toHex(g) + toHex(b);
    const alphaHex = Math.round(a * 255).toString(16).padStart(2, '0');
    
    return hex.toUpperCase() + alphaHex.toUpperCase();
  }

  function generateHueGradient() {
    const colors = [];
    for (let i = 0; i <= 360; i += 30) {
      colors.push(`hsl(${i}, ${saturation}%, ${lightness}%)`);
    }
    return `linear-gradient(90deg, ${colors.join(', ')})`;
  }

  $: hueGradient = generateHueGradient();
  $: saturationGradient = `linear-gradient(90deg, hsl(${hue}, 0%, ${lightness}%), hsl(${hue}, 100%, ${lightness}%))`;
  $: lightnessGradient = `linear-gradient(90deg, hsl(${hue}, ${saturation}%, 0%), hsl(${hue}, ${saturation}%, 50%), hsl(${hue}, ${saturation}%, 100%))`;
  $: alphaGradient = `linear-gradient(90deg, hsla(${hue}, ${saturation}%, ${lightness}%, 0), hsla(${hue}, ${saturation}%, ${lightness}%, 1))`;

  $: {
    if (configColor) {
      const hsla = hexToHsla(configColor);
      if (hsla) {
        hue = hsla.h;
        saturation = hsla.s;
        lightness = hsla.l;
        alpha = hsla.a;
      }
    }
  }

  $: colorCode = hslaToHex(hue, saturation, lightness, alpha);
  $: colorPreview = '#' + colorCode;

  function handleColorCodeChange(e) {
    const code = e.target.value;
    colorCode = code.toUpperCase();
    const hexRegex = /^([A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$/;
    colorError = !hexRegex.test(code);
    
    if (!colorError) {
      const hsla = hexToHsla('#' + code);
      if (hsla) {
        hue = hsla.h;
        saturation = hsla.s;
        lightness = hsla.l;
        alpha = hsla.a;
      }
    }
  }

  function handleApply() {
    if (!colorError && colorKey && settingKey) {
      dispatch('apply', '#' + colorCode);
    }
  }

  function handleCancel() {
    dispatch('cancel');
  }

  function handleReset() {
    const hsla = hexToHsla(defaultColor);
    if (hsla) {
      hue = hsla.h;
      saturation = hsla.s;
      lightness = hsla.l;
      alpha = hsla.a;
    }
  }

  onMount(() => {
    if (!configColor) {
      const hsla = hexToHsla(defaultColor);
      if (hsla) {
        hue = hsla.h;
        saturation = hsla.s;
        lightness = hsla.l;
        alpha = hsla.a;
      }
    }
  });
</script>

<style>
  input[type="range"] {
    position: absolute;
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;
    background: transparent;
    border: none;
    cursor: pointer;
    z-index: 5;
    appearance: none;
    -webkit-appearance: none;
    -moz-appearance: none;
  }

  input[type="range"]::-webkit-slider-thumb {
    appearance: none;
    -webkit-appearance: none;
    width: 1.4814814815vh;
    height: 1.4814814815vh;
    border-radius: 50%;
    background: #fff;
    border: 0.0925925926vh solid #fff;
    box-shadow: 0 0 2.962962963vh rgba(0, 0, 0, 0.5);
    cursor: pointer;
    transform: translateY(-50%);
  }

  input[type="range"]::-webkit-slider-runnable-track {
    background: transparent;
    border: none;
    height: 100%;
  }

  input[type="range"]::-moz-range-thumb {
    width: 1.4814814815vh;
    height: 1.4814814815vh;
    border-radius: 50%;
    background: #fff;
    border: 0.0925925926vh solid #fff;
    box-shadow: 0 0 2.962962963vh rgba(0, 0, 0, 0.5);
    cursor: pointer;
    transform: translateY(-50%);
  }

  input[type="range"]::-moz-range-track {
    background: transparent;
    border: none;
  }
</style>

<div bind:this={rootElement} data-v-d04350d8 class="color-picker">
  <div data-v-d04350d8 class="color-control">
    <div data-v-d04350d8 class="sliders">
      <div data-v-483fc7f7 class="slider hue">
        <span data-v-483fc7f7>Оттенок</span>
        <div data-v-483fc7f7 class="vue-slider">
          <div 
            data-v-483fc7f7 
            class="vue-slider-rail"
            style="background: {hueGradient};"
          >
            <input 
              data-v-483fc7f7
              type="range"
              min="0"
              max="360"
              bind:value={hue}
            />
          </div>
        </div>
      </div>

      <div data-v-483fc7f7 class="slider saturation">
        <span data-v-483fc7f7>Насыщенность</span>
        <div data-v-483fc7f7 class="vue-slider">
          <div 
            data-v-483fc7f7 
            class="vue-slider-rail"
            style="background: {saturationGradient};"
          >
            <input 
              data-v-483fc7f7
              type="range"
              min="0"
              max="100"
              bind:value={saturation}
            />
          </div>
        </div>
      </div>

      <div data-v-483fc7f7 class="slider lightness">
        <span data-v-483fc7f7>Яркость</span>
        <div data-v-483fc7f7 class="vue-slider">
          <div 
            data-v-483fc7f7 
            class="vue-slider-rail"
            style="background: {lightnessGradient};"
          >
            <input 
              data-v-483fc7f7
              type="range"
              min="0"
              max="100"
              bind:value={lightness}
            />
          </div>
        </div>
      </div>

      <div data-v-483fc7f7 class="slider alpha">
        <span data-v-483fc7f7>Прозрачность</span>
        <div data-v-483fc7f7 class="vue-slider">
          <div 
            data-v-483fc7f7 
            class="vue-slider-rail"
            style="background: {alphaGradient};"
          >
            <input 
              data-v-483fc7f7
              type="range"
              min="0"
              max="1"
              step="0.01"
              bind:value={alpha}
            />
          </div>
        </div>
      </div>
    </div>

    <div data-v-d04350d8 class="color">
      <div data-v-d04350d8 class="color-preview">
        <div 
          data-v-d04350d8
          class="color-preview-value"
          style="background-color: {colorPreview}"
        ></div>
      </div>

      <div data-v-d04350d8 class="color-code" class:error={colorError}>
        <p data-v-d04350d8>#</p>
        <input 
          data-v-d04350d8
          type="text"
          value={colorCode}
          on:change={handleColorCodeChange}
          on:input={handleColorCodeChange}
          maxlength="8"
        />
      </div>

      <div data-v-d04350d8 class="color-reset" on:click={handleReset}>
        <span data-v-d04350d8>Сбросить</span>
      </div>
    </div>
  </div>

  <div data-v-d04350d8 class="btns">
    <div data-v-d04350d8 class="btn" class:disable={colorError} on:click={handleApply}>
      <span data-v-d04350d8>Применить</span>
    </div>
    <div data-v-d04350d8 class="btn" on:click={handleCancel}>
      <span data-v-d04350d8>Отмена</span>
    </div>
  </div>
</div>