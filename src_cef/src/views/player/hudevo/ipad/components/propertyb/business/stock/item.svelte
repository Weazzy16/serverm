<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { itemsInfo } from '../../../../../../../../json/itemsInfo.js'
    import { getPng } from './../data'

    export let selectedProductName;
    export let onSelectedViewProduct;
    export let onSelectedProductName;

    let product = {};

    const updateData = () => {
        executeClientAsyncToGroup("business.getProduct", selectedProductName).then((result) => {
            if (hasJsonStructure(result))
                product = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    const getExtraCharge = (bType, price, defaultPrice) => {
        if (bType == 7 || bType == 9 || bType == 10 || bType == 11 || bType == 12){
            return `${price}%`;
        }
        return `${Math.round(price * 100 / defaultPrice)}% ($${format("money", price)})`;
    }

    const onChangePrice = () => {
        onSelectedViewProduct ("ChangePrice");
    }

    const onOrderProduct = () => {
        onSelectedViewProduct ("OrderProduct");
    }

    const onSelectBack = () => {
        onSelectedViewProduct ("List");
        onSelectedProductName ("");
    }

    let bizType = 0;
    executeClientAsyncToGroup("business.getType").then((result) => {
        bizType = result;
    });

    const cancelOrder = () => {
        if (!window.loaderData.delay ("business.cancelOrder", 1.5))
            return;

        executeClientToGroup("business.cancelOrder", product.uidOrder);
    }
    import { fade } from 'svelte/transition'
    import Activecapt from '../../../../../elements/activecapt.svelte';
</script>

<div class="mybizappmenuitem" in:fade>
    <div class="mybizappmenuitem__item">
        <div class="headitem">
            <div class="leftitem">
                <img src="{getPng(product.productType, product.name, product.itemId)}" alt=""/>
            </div>
            <div class="rightitem">
                <h1>{product.name}</h1>
                <div class="iteminfo">
                    <p>Цена: {getExtraCharge (bizType, product.price, product.defaultPrice)}</p>
                    <span>Количество: {product.count}/{product.maxCount}<b>шт.</b></span>
                </div>
            </div>
        </div>
        <div class="btnsitem">
            {#if !["Lottoschein", "Verbrauchsmaterial"].includes(product.name)}
                <div class="btnsitem__item" on:keypress={() => {}} on:click={onChangePrice}>
                    <p>Изменить цену</p>
                </div>
            {/if}
            {#if !product.isOrder && product.maxCount > product.count}
                <div class="btnsitem__item" on:keypress={() => {}} on:click={onOrderProduct}>
                    <p>Заказать товар</p>
                </div>
            {/if}
        </div>
    </div>
</div>
