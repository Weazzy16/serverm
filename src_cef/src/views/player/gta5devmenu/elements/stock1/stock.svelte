<script>
  import { onMount } from 'svelte';
  import { executeClient } from 'api/rage';
  import { itemsInfo } from 'json/itemsInfo.js';
  import { getPng } from './getPng.js';

  // сюда придут предметы со склада
  let itemliststock = [];

  // основной хендлер для InitOtherDataStock
  window['client.inventory.InitOtherDataStock'] = (
    otherId, otherName, jsonString,
    maxSlots, selectItemId,
    isArmyCar, isMyTent
  ) => {
    let arr;
    try {
      arr = JSON.parse(jsonString);
      if (!Array.isArray(arr)) throw new Error("Not an array");
    } catch (e) {
      console.error("Не смог распарсить JSON склада:", e, jsonString);
      arr = [];
    }
    // оставляем только записи с валидным ItemId и Index
    itemliststock = arr.filter(i =>
      i
      && typeof i.ItemId === 'number'
      && typeof i.Index === 'number'
      && (i.ItemId in itemsInfo)
    );
    console.log(">>> stock after filtering:", itemliststock);
  };

  // при монтировании кидаем запрос
  onMount(() => {
    executeClient("client.stockitems");
  });

  // реактивно сортируем по SqlId
  $: sortedItems = [...itemliststock].sort((a,b) => a.SqlId - b.SqlId);

  // забираем предмет из склада
  function takeFromStock(item) {
    executeClient("client.gamemenu.inventory.stack", "other", item.Index, 2, 1);
    // через короткую задержку перезапрашиваем список
    setTimeout(() => executeClient("client.stockitems"), 200);
  }
</script>

<style>
  .empty { text-align:center; color:#888; margin-top:2em }
  .magazlist { display:flex; flex-wrap:wrap; gap:12px }
  .blockrewrad1 {
    width:120px; background:#222;
    border-radius:8px; padding:8px;
    text-align:center;
  }
  .headrewrad { position:relative; height:24px }
  .headrewrad .line {
    position:absolute; left:0; right:0; top:50%;
    height:1px; background:#444;
  }
  .headrewrad span {
    position:relative; background:#222; padding:0 4px;
  }
  .blockrewrad1 img {
    width:64px; height:64px;
    object-fit:contain; margin:8px 0;
  }
  .blockrewrad1 p { font-size:14px; margin:4px 0 }
  .btnuse {
    display:inline-block; margin-top:6px;
    padding:4px 8px; background:#e91e63;
    color:#fff; font-size:13px;
    border-radius:4px; cursor:pointer;
  }
</style>

{#if sortedItems.length > 0}
  <div class="magazlist">
    {#each sortedItems as item (item.SqlId)}
      <div class="blockrewrad1">
        <div class="headrewrad">
          <div class="line"></div>
          <span>{item.Count}</span>
        </div>
        <img
          src="{getPng(item, itemsInfo[item.ItemId])}"
          alt="{itemsInfo[item.ItemId].Name}"
        />
        <p>{itemsInfo[item.ItemId].Name}</p>
        <div class="btnuse" on:click={() => takeFromStock(item)}>
          Забрать
        </div>
      </div>
    {/each}
  </div>
{:else}
  <p class="empty">Склад пуст</p>
{/if}
