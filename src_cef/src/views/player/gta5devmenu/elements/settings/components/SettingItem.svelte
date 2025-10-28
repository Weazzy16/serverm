<script>
  import { getSettingItem, getSettingValue, updateSettingValue } from "../lib/settingsStore.js";
  import { storeSettings } from 'store/settings';
  import SwitchControl from "./controls/Switch.svelte";
  import SelectControl from "./controls/Select.svelte";
  import RangeControl from "./controls/Range.svelte";
  import BindControl from "./controls/Bind.svelte";
  import ColorPickerControl from "./controls/ColorPicker.svelte";
  import ColorPickerEditor from "./controls/ColorPickerEditor.svelte";

  export let settingKey;

  const item = getSettingItem(settingKey);

  let openedDropDown = null;

  function onChange(newVal) {
    updateSettingValue(settingKey, newVal);
    console.debug("SettingItem changed", { settingKey, newVal });
  }

  function openDropDown(data) {
    openedDropDown = {
      settingKey: settingKey,
      data: data
    };
  }

  function closeDropDown() {
    openedDropDown = null;
  }

  function handleApplyColor(newColor) {
    if (openedDropDown?.data && item.type === 'colorPicker') {
      const updated = { ...value, [openedDropDown.data]: newColor };
      onChange(updated);
      closeDropDown();
    }
  }

  $: value = $storeSettings?.[settingKey];

  $: depends = item?.depends ?? [];
  $: isDisabled = depends.some(dep => {
    const depValue = $storeSettings?.[dep.key];
    return depValue !== dep.value;
  });

  $: chatSettingsValue = $storeSettings?.chatSettings;
  $: isStandardChat = chatSettingsValue === "standard" || chatSettingsValue === 0;
  
  $: chatCmdsSettingsValue = $storeSettings?.chatCmdsSettings;
  $: isStandardCmds = chatCmdsSettingsValue === "standard" || chatCmdsSettingsValue === 0;

  $: chatBorderTypeValue = $storeSettings?.chatBorderType;
  $: isStroke = chatBorderTypeValue === "Stroke";

  // Новое: Для прицела
  $: crosshairSettingsValue = $storeSettings?.crosshairSettings;
  $: isStandardCrosshair = crosshairSettingsValue === "standard";

  $: isDisabledByChat = (
    settingKey !== "chatSettings" && 
    settingKey.startsWith("chat") && 
    !settingKey.startsWith("chatCmds") &&
    isStandardChat
  );

  $: isDisabledByCmds = (
    settingKey !== "chatCmdsSettings" && 
    settingKey.startsWith("chatCmds") && 
    isStandardCmds
  );

  $: isDisabledByStroke = (
    (settingKey === "chatFontShadow" || settingKey === "chatFontShadowAlpha") &&
    isStroke
  );

  // Новое: Отключаем всё когда прицел на стандартных настройках
  $: isDisabledByCrosshair = (
    settingKey !== "crosshairSettings" && 
    settingKey.startsWith("crosshair") && 
    isStandardCrosshair
  );

  $: finalDisabled = isDisabled || isDisabledByChat || isDisabledByCmds || isDisabledByStroke || isDisabledByCrosshair;
</script>

{#if item}
  <div class="setting-item" class:disabled={finalDisabled}>
    <div class="header">
      <div class="name">{item.title}</div>

      {#if item.type === 'checkbox'}
        <SwitchControl 
          on:changeValue={(e) => onChange(e.detail ?? e)} 
          checked={value}
          disabled={finalDisabled}
        />
      {:else if item.type === 'selector'}
        <SelectControl 
          value={value}
          list={item.list} 
          options={item.list} 
          disabled={finalDisabled}
          on:change={(e) => onChange(e.detail ?? e)} 
        />
      {:else if item.type === 'range'}
        <RangeControl 
          modelValue={value}
          min={item.sliderOptions?.min ?? 0}
          max={item.sliderOptions?.max ?? 100}
          interval={item.sliderOptions?.interval ?? 1}
          tooltip="active"
          marks={item.marks ?? false}
          disabled={finalDisabled}
          on:change={(e) => onChange(e.detail ?? e)}
          on:input={(e) => onChange(e.detail ?? e)} 
        />
      {:else if item.type === 'keyBind'}
        <BindControl 
          settingKey={settingKey}
          settingValue={value}
          settingItem={item}
          disabled={finalDisabled}
        />
      {:else if item.type === 'colorPicker'} 
        <ColorPickerControl 
          settingKey={settingKey}
          settingValue={value}        
          settingItem={item}
          disabled={finalDisabled}
          on:toggleColor={(e) => openDropDown(e.detail)}
          on:change={(e) => onChange(e.detail ?? e)}
        />
      {:else}
        <div class="control">Тип: {item.type} — значение: {JSON.stringify(value)}</div>
      {/if}
    </div>

    {#if openedDropDown?.settingKey === settingKey && item.type === 'colorPicker' && openedDropDown?.data}
      <div data-v-1725493a class="dropDown" class:disabled={finalDisabled}>
        <div data-v-1725493a class="overlay" on:click={closeDropDown}></div>
        <div data-v-1725493a class="dropDown-item">
          <ColorPickerEditor 
            colorKey={openedDropDown.data}
            configColor={value?.[openedDropDown.data]}
            {settingKey}
            settingValue={value}
            defaultColor={item.default?.[openedDropDown.data]}
            on:apply={(e) => handleApplyColor(e.detail)}
            on:cancel={closeDropDown}
          />
        </div>
      </div>
    {/if}
  </div>
{:else}
  <div class="setting-item">
    <div class="header">
      <div class="name">{settingKey}</div>
      <div class="control">Нет мета-информации для этой настройки</div>
    </div>
  </div>
{/if}