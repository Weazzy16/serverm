<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    export let selectedId;
    export let selectCharData;


    import { executeClient, executeClientAsyncToGroup, executeClientToGroup } from "api/rage";
    import { addListernEvent, hasJsonStructure } from "api/functions";

    let stats = {};
    let isOrder = false;
    let isProductList = false;
    let isLoad = false;

    let data = [];
    let data1 = [];

    const initTime = (jsonStats, jsonProducts, jsonUsers) => {

        data = JSON.parse(jsonUsers);
        data1 = JSON.parse(jsonProducts);

        isLoad = true;
    }

    executeClientToGroup ("business.loadStats");
    addListernEvent ("phoneBusinessStatsInit", initTime);


    const updateData = () => {
        executeClientAsyncToGroup("business.getStats").then((result) => {
            if (hasJsonStructure(result))
                stats = JSON.parse(result);
        });
        executeClientAsyncToGroup("business.getOrders").then((result) => {
            if (hasJsonStructure(result)) {
                result = JSON.parse(result);
                isOrder = result.length > 0;
            }
        });

        executeClientAsyncToGroup("business.getProducts").then((result) => {
            if (hasJsonStructure(result)) {
                result = JSON.parse(result);
                isProductList = result.length > 1;
            }
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    export let onSelectedViewBusiness;
    export let onSelectedView;

    const onSell = () => {
        if (!window.loaderData.delay ("business.sell", 1.5))
            return;

        executeClientToGroup ("business.sell")
    }
    import { fade } from 'svelte/transition'
</script>
{#if !isLoad}
    <div></div>
{:else}
    <div class="mybizappmenu">
        <div class="mybizappmenu__head">
            <div class="blockinfo">
                <p>Баланс счёта</p>
                <span><b>$</b>{format("money", stats.cash)}</span>
            </div>
            <div class="blockinfo">
                <p>Прибыль за сегодня</p>
                <span><b>$</b>{format("money", Math.round(stats.pribil - stats.zatratq))}</span>
            </div>
            <div class="blockinfo">
                <p>Оплачен на</p>
                <span>{selectCharData.BizPaid} д.</span>
            </div>
        </div>
        <div class="mybizappmenu__dop">
            <div class="dop_left">
                <h1>Рейтинг покупателей</h1>
                <div class="listhead">
                    <span>№</span>
                    <p>Ид покупателей</p>
                    <b>Общая сумма</b>
                </div>
                {#each data as item, index}
                    <div class="listblock">
                        <span>{index + 1}</span>
                        <p>{item.uuid}</p>
                        <b>${format("money", item.price)}</b>
                    </div>
                {/each}
            </div>
            <div class="dop_right">
                <h1>Самые популярные товары</h1>
                <div class="listhead">
                    <span>№</span>
                    <p>Название товара</p>
                    <b>Общая сумма</b>
                </div>
                {#each data1 as item, index}
                    <div class="listblock">
                        <span>{index + 1}</span>
                        <p>{item.name}</p>
                        <b>${format("money", item.price)}</b>
                    </div>
                {/each}
            </div>
        </div>
    </div>
{/if}