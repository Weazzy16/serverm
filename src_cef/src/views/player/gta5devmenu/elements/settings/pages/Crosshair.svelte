<script>
  import { onMount, onDestroy } from 'svelte';
  import { pageSettingKeys } from "../lib/settingsConfig.js";
  import SettingItem from "../components/SettingItem.svelte";
  import PreviewCrosshair from "../components/PreviewCrosshair.svelte";

  const settingKeys = pageSettingKeys["crosshair"] || [];

  onMount(() => {
    console.log('Crosshair page mounted');
  });

  onDestroy(() => {
    console.log('Crosshair page destroyed');
  });
</script>

<div class="tabs-wrapper">
  <div class="tab glued-columns">
    <div data-v-25785e6e class="column-wrapper single">
      {#each settingKeys as key (key)}
        <SettingItem settingKey={key} />
      {/each}
    </div>

    <div data-v-42bf2d36 class="apply-setting-code">
      <div data-v-42bf2d36 class="header">
        <div data-v-42bf2d36 class="name">Применить настройки</div>
      </div>
      <div data-v-42bf2d36 class="input-wrapper">
        <input data-v-42bf2d36
          type="text" 
          maxlength="512" 
          placeholder="Введите код" 
        />
      </div>
      <div data-v-42bf2d36 class="btn-wrapper">
        <div data-v-42bf2d36 class="btn disabled">
          <span data-v-42bf2d36>Загрузить</span>
        </div>
        <div data-v-42bf2d36 class="btn">
          <span data-v-42bf2d36>Поделиться</span>
        </div>
      </div>
    </div>
  </div>

  <div class="previewC-tab">
    <PreviewCrosshair />
  </div>
</div>