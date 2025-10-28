<script>
    import { itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng } from './getPng.js'

    export let activeItem;

    $: iconInfo = activeItem ? itemsInfo[activeItem.ItemId] : null;
</script>

<div class="blockitems" class:active={activeItem}>
    {#if activeItem && activeItem.ItemId !== 0}
        <img src="{getPng(activeItem, iconInfo)}" alt="{iconInfo.Name}" />
        <div class="active-overlay">Активен</div>
    {:else}
        <div class="empty-slot">Нет активного предмета</div>
    {/if}
</div>

<style>

</style>
