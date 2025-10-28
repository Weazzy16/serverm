<script>
    import { translateText } from 'lang'
    import { onDestroy } from 'svelte';
    import {executeClient, executeClientAsync} from 'api/rage'
    import { selectCase } from './state.js'
    import { format } from 'api/formatter'
    import { accountRedbucks, accountUnique } from 'store/account'
    
    let isLoad = false;
    let caseData = {}
    executeClientAsync("donate.roulette.getCase", $selectCase).then((result) => {
        if (result && typeof result === "string") {
            caseData = JSON.parse(result);
            isLoad = true;
        }
    });
    
    let antiFlud = 0;

    let currentCount = 1;
    let toggledFast = false;

    const onOpen = (_toggledFast = false) => {
        if (antiFlud > new Date().getTime())
            return;
        else if ($accountRedbucks < getPrice (caseData.price * currentCount, caseData.index, $accountUnique))
            return window.notificationAdd(4, 9, `Not enough Redbucks!`, 3000);
        antiFlud = new Date().getTime() + 2500;
        toggledFast = _toggledFast;
        executeClient ("client.roullete.buy", caseData.index, currentCount);
    }

    const getPrice = (price, index, unique) => {
        if (unique && unique.split("_")) {
            let getData = unique.split("_");
            if (getData[0] === "cases" && Number (getData[1]) === index && Number (getData[2]) === 0) {
                price = Math.round (price * 0.7);
            }
        }
        return price;
    }

    const onCurrentCount = (count) => {
        if (antiFlud > new Date().getTime())
            return;
            
        currentCount  = count;
    }

</script>
{#if isLoad}
<div class="donathmenu">      
    <h1>Кейсы</h1>
    <div class="caseinfo">
        <div class="cinfoico">
            <img class="infoico" src="https://imgur.com/MvM92tG.png" alt="">
        </div>
        <div class="cinfo">
            <h1>Информация о кейсе</h1>
            <p>Здесь вы можете увидеть, что выпадает в этом случае, и редкость выпадающей капли</p>
        </div>
        <div class="buycase" on:keypress={() => {}} on:click={() => onOpen()}>
            <p>За {format("money", getPrice (caseData.price * currentCount, caseData.index, $accountUnique))} OC</p>
        </div>
    </div>
    <div class="caseitemlist">
        {#each caseData.items as value, index}
            <div class="items {value.color}">
                <p>{value.title}</p>
                <img class="icoi" src="{document.cloud + `img/roulette/${value.image}.png`}" alt="">
            </div>
        {/each}
    </div>
</div>

{/if}