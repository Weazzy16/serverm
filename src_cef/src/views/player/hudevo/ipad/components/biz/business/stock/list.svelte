<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { getPng } from './../data'

    export let onSelectedViewProduct;
    export let onSelectedProductName;
    export let onSelectedViewBusiness;


    let stats = {};
    let productsList = [];

    const updateData = () => {
        executeClientAsyncToGroup("business.getStats").then((result) => {
            if (hasJsonStructure(result))
                stats = JSON.parse(result);
        });

        executeClientAsyncToGroup("business.getProducts").then((result) => {
            if (hasJsonStructure(result))
                productsList = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);


    const onSelectItem = (name) => {
        onSelectedViewProduct ("Item");
        onSelectedProductName (name);
    }

    let bizType = 0;
    executeClientAsyncToGroup("business.getType").then((result) => {
        bizType = result;
    });

    const onMaxProducts = () => {
        if (!window.loaderData.delay ("business.maxProducts", 2.5))
            return;

        executeClientToGroup("business.maxProducts");
    }
    import { fade } from 'svelte/transition'
</script>
<div class="mybizappmenuitems" in:fade>
    {#each productsList as product}
        <div class="blockitem" on:keypress={() => {}} on:click={() => onSelectItem(product.name)}>
            <img src="{getPng(product.productType, product.name, product.itemId)}" alt=""/>
            <p>{product.name}</p>
            <div class="blockbottom">
                <b>${format("money", product.price)}</b>
                <span>{product.count} шт.</span>
            </div>
        </div>
    {/each}
</div>