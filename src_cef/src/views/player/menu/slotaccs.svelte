<script>
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng } from './getPng.js'

    export let item;
    export let defaultIcon;
    export let defaultName;
    export let defaultNamei = ""; // например, "head"
    export let defaultStyle;

    let iconInfo = itemsInfo [item.ItemId];

    $: if (item) 
        iconInfo = itemsInfo [item.ItemId];
        
    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }
    $: defaultIconUrl = `http://cdn.piecerp.ru/cloud/inventoryItems/icons/${defaultNamei}.svg`;
    const GetClothesClass = (_iconInfo, _item) => {
        if (_iconInfo.functionType === ItemType.Clothes && _item.ItemId != -5 && _item.ItemId != -9 && _item.ItemId != -1 && _item.Data.split("_").length >= 2) {
            return Bool(item.Data.split("_")[2]) ? "clothesM" : "clothesF"
        }
        return "";
    }

</script>
{#if item && item.ItemId != 0}
    <div class="blockitem" class:active={item.active} class:anim={item.anim} class:noAnim={!item.anim} class:noUse={!item.use} on:mousedown on:mouseup on:mouseenter on:mouseleave>
        <img src="{getPng(item, iconInfo)}" alt="">                                                  
    </div>
{:else}
    <div class="blockitem" class:active={item.active} class:anim={item.anim} class:noAnim={!item.anim} class:noUse={!item.use} on:mousedown on:mouseup on:mouseenter on:mouseleave>
        <img src={defaultIconUrl} alt={defaultNamei} />
        <p>{defaultName}</p>                                     
    </div>
{/if}





