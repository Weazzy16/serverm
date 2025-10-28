<script>
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng } from './getPng.js'

    export let item;
    export let defaultIcon;
    export let defaultName;
    export let defaultStyle;

    

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

</script>
    {#if item && item.ItemId != 0}
        <div class="itemblock" on:mouseenter on:mouseleave>
            <h1>{itemsInfo [item.ItemId].Name} {#if defaultIcon === undefined && iconInfo.functionType === 1 && item.ItemId != -5 && item.ItemId != -9 && item.ItemId != -1 && item.Data.split("_").length >= 2}{Bool(item.Data.split("_")[2]) ? "М" : "Ж"}{/if}</h1>
            <img src="{getPng(item, iconInfo)}" alt="">
            <span>{item.Count}<b>шт.</b></span>
        </div>
    {/if}
