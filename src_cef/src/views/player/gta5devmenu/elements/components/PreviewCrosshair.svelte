<script>
  import { onMount, onDestroy } from 'svelte';
  import { storeSettings } from 'store/settings';
  import CrosshairDraw from "./CrosshairDraw.svelte";

  let backgroundIndex = 0;
  const backgrounds = ["background"];

  let settings = {};
  let unsubscribe;

  onMount(() => {
    unsubscribe = storeSettings.subscribe(v => {
      settings = v;
    });
  });

  onDestroy(() => {
    if (unsubscribe) unsubscribe();
  });

  $: currentBackground = backgrounds[backgroundIndex % backgrounds.length];
  $: totalBackgrounds = backgrounds.length;
  $: currentBackgroundIndex = backgroundIndex % totalBackgrounds;

  $: crosshairSettings = settings.crosshairSettings ?? 'standard';
  $: crosshairColor = settings.crosshairColor || { "Основной": "#ffffff", "Наведение": "#ff0000" };
  $: crosshairCenter = settings.crosshairCenter ?? 'no';
  $: crosshairCentralDistance = settings.crosshairCentralDistance ?? 0;
  $: crosshairLineLength = settings.crosshairLineLength ?? 10;
  $: crosshairLineThickness = settings.crosshairLineThickness ?? 2;
  $: crosshairIncline = settings.crosshairIncline ?? 0;
  $: crosshairStroke = settings.crosshairStroke ?? 0;
  $: crosshairOpacity = settings.crosshairOpacity ?? 100;

  $: centerValue = (() => {
    switch (crosshairCenter) {
      case "small":
        return 1;
      case "large":
        return 2;
      default:
        return 0;
    }
  })();

  $: crosshairData = {
    crosshairCenter: centerValue,
    crosshairCentralDistance,
    crosshairLineLength,
    crosshairLineThickness,
    crosshairIncline,
    crosshairStroke,
    crosshairOpacity,
    crosshairColor: crosshairColor?.["Основной"] || "#ffffff",
    crosshairEffectColor: crosshairColor?.["Наведение"] || "#ff0000"
  };

  function prevBackground() {
    backgroundIndex = backgroundIndex > 0 ? backgroundIndex - 1 : totalBackgrounds - 1;
  }

  function nextBackground() {
    backgroundIndex = backgroundIndex < totalBackgrounds - 1 ? backgroundIndex + 1 : 0;
  }

  function resetSettings() {
    if (window.CEF && window.CEF.call) {
      window.CEF.call('W2C:Menu_F4_Settings:Reset', {
        pageName: 'Crosshair'
      });
    }
  }
</script>

<div data-v-3767a116 class="crosshair">
  <div data-v-3767a116 class="preview">
    <img data-v-3767a116
      src={`https://cdn.majestic-files.com/public/master/static/img/F4/Settings/Crosshair/${currentBackground}.jpg`}
      alt="crosshair-preview"
    />

    {#if totalBackgrounds > 1}
      <div data-v-3767a116 class="themeCounter">
        <span data-v-3767a116>{currentBackgroundIndex + 1}</span>
        <p data-v-3767a116>/</p>
        <p data-v-3767a116>{totalBackgrounds}</p>
      </div>
    {/if}

    {#if totalBackgrounds > 1}
      <div data-v-3767a116 class="arrows">
        <div data-v-3767a116 class="arrow" on:click={prevBackground}>←</div>
        <div data-v-3767a116 class="arrow right" on:click={nextBackground}>→</div>
      </div>
    {/if}

    <CrosshairDraw {crosshairData} isPreview={true} />

    <div data-v-3767a116 class="reset" on:click={resetSettings}>
      <span data-v-3767a116>Сбросить настройки</span>
    </div>
  </div>
</div>