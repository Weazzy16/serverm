<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { getPrefix } from './../data'

    export let selectedProductName;
    export let onSelectedViewProduct;

    let product = {};

    const updateData = () => {
        executeClientAsyncToGroup("business.getProduct", selectedProductName).then((result) => {
            if (hasJsonStructure(result))
                product = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    let bizType = 0;
    executeClientAsyncToGroup("business.getType").then((result) => {
        bizType = result;
    });

    let price;

    const onSelect = () => {
        if (!window.loaderData.delay ("business.extraCharge", 1.5))
            return;

        const value = Number (price);

        if (product.minPrice > value || value > product.maxPrice ) {
            window.notificationAdd(4, 9, `${translateText('player2', 'Наценка не может быть меньше')} ${product.minPrice}${getPrefix (selectedProductName, bizType)} und größer als ${product.maxPrice}${getPrefix (selectedProductName, bizType)}`, 3000);
            return;
        }

        executeClientToGroup("business.extraCharge", selectedProductName, value);
    }

    const onSelectBack = () => {
        onSelectedViewProduct ("Item");
    }
    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>
<div class="dialogmenu" in:fade>
    <div class="dmenu">
        <h1>Изменить цену продажи</h1>
        <div class="infoitem">
            <p>Государственная цена:</p>
            <b>{product.minPrice}{getPrefix (selectedProductName, bizType)}</b>
        </div>
        <div class="infoitem">
            <p>Максимальная цена:</p>
            <b>{product.maxPrice}{getPrefix (selectedProductName, bizType)}</b>
        </div>
        <div class="input">
            <p>Введите новое значение</p>
            <input type="text" bind:value={price} on:focus={onInputFocus} on:blur={onInputBlur} id="price"/>
        </div>
        <div class="btnsitem">
            <div class="btni" on:keypress={() => {}} on:click={onSelect}>
                <p>Применить</p>
            </div>
            <div class="btni" on:keypress={() => {}} on:click={onSelectBack}>
                <p>Отмена</p>
            </div>
        </div>
    </div>
</div>