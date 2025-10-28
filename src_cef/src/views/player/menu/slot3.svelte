<script>
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng } from './getPng.js'
    import { executeClient } from 'api/rage'

    export let item;
    export let defaultIcon;
    export let defaultName;
    export let defaultStyle;
    export let selcetinv;

    let value = 1;

    let iconInfo = itemsInfo [item.ItemId];

    $: if (item) 
        iconInfo = itemsInfo [item.ItemId];
        
    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }

    const GetClothesClass = (_iconInfo, _item) => {
        if (_iconInfo.functionType === ItemType.Clothes && _item.ItemId != -5 && _item.ItemId != -9 && _item.ItemId != -1 && _item.Data.split("_").length >= 2) {
            return Bool(item.Data.split("_")[2]) ? "clothesM" : "clothesF"
        }
        return "";
    }

    const useitem = (item) => {
        executeClient ("client.gamemenu.inventory.stack", "other", item.Index, 2, value);
    }

    const dropitem = (item) => {
        executeClient ("client.gamemenu.inventory.stack", "inventory", item.Index, 2, value);
    }

</script>
{#if item && item.ItemId != 0}
    <div class="itemblock">
        <div class="leftib">
            <img src="{getPng(item, iconInfo)}" alt="">
        </div>
        <div class="rightib">
            <h1>{itemsInfo [item.ItemId].Name} {#if defaultIcon === undefined && iconInfo.functionType === 1 && item.ItemId != -5 && item.ItemId != -9 && item.ItemId != -1 && item.Data.split("_").length >= 2}{Bool(item.Data.split("_")[2]) ? "М" : "Ж"}{/if}</h1>
            <p>{itemsInfo [item.ItemId].Description}</p>
            {#if item.Count > 1 }
                <span>{item.Count}<b>шт..</b></span>
                {:else}
                <span></span>
            {/if}
            {#if item.Count > 1 }
                <div class="kolvo">
                    <input bind:value={value}>
                    <div class="btnskolv">
                        <p on:keypress={() => {}} on:click={(e) => { e.stopPropagation(); value = value > 0 ? value - 1 : value;}}>-</p>
                        <p on:keypress={() => {}} on:click={(e) => { e.stopPropagation(); value = value < item.Count ? value + 1 : value;}}>+</p>
                    </div>
                    <div class="btnskolv">
                        <p on:keypress={() => {}} on:click={(e) => { e.stopPropagation(); value = 1;}}>Min</p>
                        <p on:keypress={() => {}} on:click={(e) => { e.stopPropagation(); value = item.Count; }}>Max</p>
                    </div>
                </div>
            {/if}
            {#if selcetinv}
                <div class="btnuse" on:keypress={() => {}} on:click={(e) => { e.stopPropagation(); useitem(item); }}>
                    <p>Взять</p>
                </div>
                {:else}
                <div class="btnuse" on:keypress={() => {}} on:click={(e) => { e.stopPropagation(); dropitem(item); }}>
                    <p>Положить</p>
                </div>
            {/if}
        </div>
    </div>
{/if}
