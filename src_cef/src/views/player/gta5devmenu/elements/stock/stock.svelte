  <script>
    import { onMount } from 'svelte';
    import { executeClient } from 'api/rage';
    import { itemsInfo } from 'json/itemsInfo.js';
    import { getPng } from './getPng.js';
    import './stock.css';

    let itemliststock = [{ ItemId: 3, Index: 0, SqlId: 101, Count: 5 },
        { ItemId: 4,  Index: 1, SqlId: 102, Count: 2 },{ ItemId: 3, Index: 0, SqlId: 101, Count: 5 },
        { ItemId: 4,  Index: 1, SqlId: 102, Count: 2 },{ ItemId: 3, Index: 0, SqlId: 101, Count: 5 },
        { ItemId: 4,  Index: 1, SqlId: 102, Count: 2 },{ ItemId: 3, Index: 0, SqlId: 101, Count: 5 },
        { ItemId: 4,  Index: 1, SqlId: 102, Count: 2 },
        ];

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
      itemliststock = arr.filter(i =>
        i
        && typeof i.ItemId === 'number'
        && typeof i.Index === 'number'
        && (i.ItemId in itemsInfo)
      );
    };

    onMount(() => {
      executeClient("client.stockitems");
    });

    $: sortedItems = [...itemliststock].sort((a,b) => a.SqlId - b.SqlId);

    function takeFromStock(item) {
      executeClient("client.gamemenu.inventory.stack", "other", item.Index, 2, 1);
      setTimeout(() => executeClient("client.stockitems"), 200);
    }
  </script>

  {#if sortedItems.length > 0}
    <div class="donate full-width full-height">

      <!-- единая обёртка -->
      <div class="items full-width full-height row-block">
        <div class="table-content">
          <!-- ваша таблица, если нужна -->
        </div>

        <!-- flex-контейнер для всех элементов -->
        <div class="items-viewer items-list row-block full-width full-height" style="--item-width: 20%;">
          
          {#each sortedItems as item (item.SqlId)}  
            <div class="item-element column-block {itemsInfo[item.ItemId].Color}">
              <div class="item-type">
              
  </div>

              <div class="metadata-container column-block align-end">
                <!-- пример вывода цены -->
                <div class="item-price row-block align-center">
                  <span class="item-price__title">
                    {itemsInfo[item.ItemId].Price || 0}
                  </span>
                  
                  <svg class="item-price__picture" xmlns="http://www.w3.org/2000/svg" id="Group_1326" data-name="Group 1326" width="22" height="22" viewBox="0 0 22 22">
    <circle id="Ellipse_21" data-name="Ellipse 21" cx="11" cy="11" r="11" fill="#e81c5a"/>
    <path id="Path_6385" data-name="Path 6385" d="M6.193-3.412a2,2,0,0,0-.522,1.031c0,.378.183,1.318.692,1.318.326,0,.953-.744,1.148-.992A27.023,27.023,0,0,1,12.5-7.119l.026.026c-.914,1.188-2.571,3.145-2.571,4.711,0,.757.287,1.579,1.148,1.579,1.122,0,3.928-2.219,4.241-3.119l-.052-.5c-1.762,1.71-2.78,2.3-3.145,2.3-.313,0-.457-.378-.457-.639a5.748,5.748,0,0,1,1.5-2.962l2.023-2.467c.157-.2.561-.653.561-.914s-.548-1.122-.9-1.122c-.235,0-.5.274-.666.418C12.5-8.45,11-7.027,9.391-5.644L9.364-5.67c.535-.822,1.945-2.754,1.945-3.719a.936.936,0,0,0-.953-.9c-.535.013-1.644.992-2.127,1.449L6.154-6.832l-.026-.026a16.464,16.464,0,0,0,.809-2.271.967.967,0,0,0-.157-.5l-.313-.548c-.131-.222-.183-.4-.5-.418A11.83,11.83,0,0,0,3.5-9.677L2.03-9,2-9.024c.509-.444.692-.679.692-.757s-.091-.131-.157-.131a.81.81,0,0,0-.326.183c-1.161.822-1.083.8-1.2.953a7.615,7.615,0,0,0-.339,1.54c-.065.313.1.483.313.483.666,0,.039-.026,4.059-1.905a1.3,1.3,0,0,1,.4-.157c.222,0,.457.261-.914,2.714L1.808-2.655c.144.4.4,1.436.953,1.436.418,0,.744-.5.966-.8L5.684-4.652,8.816-8.045c.091-.1.248-.274.4-.274s.2.144-.026.561A19.4,19.4,0,0,1,7.081-4.574Z" transform="translate(3.129 16.731)" fill="#fff"/>
  </svg>
                </div>
              </div>
              <div class="gradient"></div>
              <div class="item-element-image full-width full-height">
                <img class="item-element-image"
                  src="{getPng(item, itemsInfo[item.ItemId])}"
                  alt="{itemsInfo[item.ItemId].Name}"
                />
              </div>
              <div class="item-element__title">
                {itemsInfo[item.ItemId].Name}
              </div>
              <div class="control-buttons row-block full-width">
                <button
                  class="control-buttons__button full-width"
                  on:click={() => takeFromStock(item)}
                >
                  Забрать
                </button>
                <button class="control-buttons__button sell full-width">
                  Продать
                </button>
              </div>
            </div>
            
          {/each}
        </div>
      </div>

    </div>
  {:else}
    <p class="empty">Склад пуст</p>
  {/if}
