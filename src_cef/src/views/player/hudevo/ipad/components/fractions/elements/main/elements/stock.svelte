<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";

    let stock = {}

    const getStock = () => {
        executeClientAsyncToGroup("getStock").then((result) => {
            if (result && typeof result === "string")
                stock = JSON.parse(result);
        });
    }
    getStock();

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.isStock", getStock)

    const updateGunStock = () => {
        executeClientToGroup("updateGunStock")
    }

    const updateStock = () => {
        executeClientToGroup("updateStock")
    }
</script>

{#if stock.isGunAccessStock}
        <div class="btns">
            {#if stock.isGunAccessStock}
            <div class="btnappf">
                <p on:keypress={() => {}} on:click={updateGunStock}>{stock.isGunStock ? translateText('player1', 'Закрыть склад') : translateText('player1', 'Открыть склад')}</p>
            </div>
            {/if}
            {#if stock.isAccessStock}
            <div class="btnappf">
                <p on:keypress={() => {}} on:click={updateStock}>{stock.isStock ? translateText('player1', 'Запретить крафт') : translateText('player1', 'Разрешить крафт')}</p>
            </div>
            {/if}
        </div>
    {:else}
        <div class="info">
            <h1>Склад:</h1>
            <div class="dop">
                <p>Крафт:</p>
                <span>{stock.isStock ? translateText('player1', 'Разрешён') : translateText('player1', 'Запрещён')}</span>
            </div>
        </div>

        {#if stock.isAccessStock}
            <div class="btnappf">
                <p on:keypress={() => {}} on:click={updateStock}>{stock.isStock ? translateText('player1', 'Запретить крафт') : translateText('player1', 'Разрешить крафт')}</p>
            </div>
        {/if}
    {/if}
