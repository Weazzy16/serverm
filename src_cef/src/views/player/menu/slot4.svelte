<script>
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng } from './getPng.js'

    export let item;
    export let defaultIcon;
    export let defaultName;
    export let defaultStyle;
    export let key;

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
    <div class="blockitem" class:active={item.active} class:anim={item.anim} class:noAnim={!item.anim} class:noUse={!item.use} on:mousedown on:mouseup on:mouseenter on:mouseleave>
        <span>{key+1}</span>
        <img src="{getPng(item, iconInfo)}" alt="">
        <div class="disab">
            <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                <g clip-path="url(#clip0_2_313)">
                <path d="M10.593 4.742C7.74297 4.89385 5.0589 6.12985 3.09072 8.19675C1.12255 10.2636 0.0192979 13.0049 0.00700378 15.859V20L1.488 16.551C2.34221 14.8424 3.63479 13.3912 5.23363 12.3458C6.83246 11.3004 8.68021 10.6983 10.588 10.601V15.34L19.988 7.656L10.593 0V4.742Z"></path>
                </g>
                <defs>
                <clipPath id="clip0_2_313">
                <rect width="20" height="20" fill="white"></rect>
                </clipPath>
                </defs>
            </svg>
        </div>
    </div>
{:else}
    <div class="blockitem" class:active={item.active} class:anim={item.anim} class:noAnim={!item.anim} class:noUse={!item.use} on:mousedown on:mouseup on:mouseenter on:mouseleave>
        <span>{key+1}</span>
    </div>
{/if}
