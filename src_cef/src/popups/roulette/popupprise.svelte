<script>
    import { translateText } from 'lang'
    export let SetPopup;
    export let popupData;
    import { format } from 'api/formatter'
    import {executeClient, executeClientAsync} from 'api/rage'

    let caseData = {};
    let isLoad = false;
    executeClientAsync("donate.roulette.getCaseOne").then((result) => {
        if (result && typeof result === "string") {
            caseData = JSON.parse(result);
            isLoad = true;
        }
    });

    let selectWin = 0;
    
    const Take = (index, IndexList) => {
        if (IndexList !== -1) {
            popupData[index].Done = true;
            ClosePopup ();
        } else 
            SetPopup ()
        executeClient ("client.roullete.confirm", false, IndexList);
    }

    const Sell = (index, IndexList) => {
        if (IndexList !== -1) {
            popupData[index].Done = true;
            ClosePopup ();
        } else 
            SetPopup ()
        executeClient ("client.roullete.confirm", true, IndexList);
    }

    const ClosePopup = () => {
        let isDone = true;
        
        popupData.forEach(item => {
            if (!item.Done && item.winBlock && caseData.items[item.winBlock.ItemIndex])
                isDone = false;
        });
        
        if (isDone)
            SetPopup ()
    }

    const onAllWinBlockPrice = (data) => {
        let price = 0;
        data.forEach(item => {
            if (!item.Done && item.winBlock && item.winBlock.Price)
                price += item.winBlock.Price;
        });
        return price;
    }
</script>

{#if isLoad}
<div class="caselut">
    <p>ВЫ</p>
    <p>ПОЛУЧИЛИ</p>
    <div class="itemlut">
        {#each popupData as item, index}
            {#if !item.Done && item.winBlock && caseData.items[item.winBlock.ItemIndex]}
                <img class="iclut" src="{document.cloud + `img/roulette/${caseData.items[item.winBlock.ItemIndex].image}.png`}" alt="">
            {/if}
        {/each}
        <p>{@html popupData[selectWin].winBlock.Text}</p>
    </div>
    <div class="lutbtn">
        <div class="btninv" on:keypress={() => {}} on:click={() => Take(0, -1)}>
            <p>Забрать в инвентарь</p>
        </div>
        {#if onAllWinBlockPrice(popupData)}
            <div class="btnsell" on:keypress={() => {}} on:click={() => Sell(0, -1)}>
                <p>Продать за {format("money", onAllWinBlockPrice(popupData))} OC</p>
            </div>
        {/if}
    </div>
</div>
{/if}