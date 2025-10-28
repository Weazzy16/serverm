<script>
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng }      from './getPng.js'
  
    export let item
    export let defaultIcon
    export let defaultName
    export let defaultStyle
  
    // единственная реактивная декларация:
    $: iconInfo = item
      ? itemsInfo[item.ItemId] 
      : {}
  
    const Bool = text => String(text).toLowerCase() === "true"
  
    const GetClothesClass = (_iconInfo, _item) => {
      if (
        _iconInfo.functionType === ItemType.Clothes &&
        ![-5, -9, -1].includes(_item.ItemId) &&
        _item.Data.split("_").length >= 2
      ) {
        return Bool(_item.Data.split("_")[2]) ? "clothesM" : "clothesF"
      }
      return ""
    }
  </script>
  
  {#if item && item.ItemId !== 0}
    <div
      class="item"
      class:active={item.active}
      class:anim={item.anim}
      class:noAnim={!item.anim}
      class:noUse={!item.use}
      on:mousedown
      on:mouseup
      on:mouseenter
      on:mouseleave
    >
      <img src={getPng(item, iconInfo)} alt="" />
  
      {#if defaultIcon === undefined && item.Count > 1 && item.ItemId !== 19 && iconInfo.functionType !== ItemType.Clothes && iconInfo.functionType !== ItemType.Weapons}
        <span>{item.Count}</span>
      {/if}
  
      {#if item.ItemId === 19 && item.Data.split("_").length >= 1}
        <span>{item.Data.split("_")[0]}</span>
      {/if}
  
      {#if defaultIcon === undefined && iconInfo.functionType === ItemType.Clothes && ![-5, -9, -1].includes(item.ItemId) && item.Data.split("_").length >= 3}
        <span>{Bool(item.Data.split("_")[2]) ? "М" : "Ж"}</span>
      {/if}
  
      {#if defaultIcon === undefined && item.ItemId === -9}
        <span>{item.Data}</span>
      {/if}
  
      {#if defaultIcon === undefined && item.ItemId === ItemId.SimCard}
        <span>{item.Data}</span>
      {/if}
  
      {#if defaultIcon === undefined && item.ItemId === ItemId.VehicleNumber}
        <span>{item.Data}</span>
      {/if}
    </div>
  {:else}
    <div
      class="item"
      class:active={item?.active}
      class:anim={item?.anim}
      class:noAnim={!item?.anim}
      class:noUse={!item?.use}
      on:mousedown
      on:mouseup
      on:mouseenter
      on:mouseleave
    ></div>
  {/if}
  