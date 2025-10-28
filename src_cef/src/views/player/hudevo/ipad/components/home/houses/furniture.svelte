<script>
    import { translateText } from 'lang'
    import {currentPage} from '../../../stores'

    export let onSelectedViewHouse;

    import {addListernEvent, hasJsonStructure} from "api/functions";
    import {executeClient, executeClientAsyncToGroup, executeClientToGroup} from "api/rage";

    let houseFurnitures = []
    const updateData = () => {
        executeClientAsyncToGroup("house.houseFurnitures").then((result) => {
            if (hasJsonStructure(result))
                houseFurnitures = JSON.parse(result);
                console.log(result)
        });
    }

    updateData ();
    addListernEvent ("phoneHouseUpdate", updateData);
    let selectedFurniture = null;
    const onSelectedFurniture = (item) => selectedFurniture = item;

    const onFurnitureBuy = (type, item) => {
        if (!window.loaderData.delay ("onFurnitureBuy", 1))
            return;

        const index = selectedFurniture;
        selectedFurniture = null;

        executeClientToGroup ("house.fUse", houseFurnitures[item].Id, type);

        if (type == 0) {
            houseFurnitures.splice(item, 1);
            houseFurnitures = houseFurnitures;
        }
    }

    const setPoint = () => {
        executeClient ("gps.name", translateText('player2', 'Мебельный магазин'));
        executeClientToGroup ("close");
    }

</script>
<div class="menuhyhomefur">
    {#if houseFurnitures}
        {#each houseFurnitures as furniture, index}
            <div class="blockitem">
                <div class="btnsell" on:keypress={() => {}} on:click={() => onFurnitureBuy (0 , index)}>
                    <p>$</p>
                </div>
                <img src="{document.cloud + `inventoryItems/furniture/${furniture.Model}.png`}" alt=""/>
                <span>{translateText('player2', 'Мебель')} {furniture.Name}</span>
                <div class="btnuse" on:keypress={() => {}} on:click={() => onFurnitureBuy (1, index)}>
                    <p>{!houseFurnitures [index].IsSet ? translateText('player2', 'Установить') : translateText('player2', 'Убрать')}</p>
                </div>
            </div>
        {/each}          
    {/if}
</div>
