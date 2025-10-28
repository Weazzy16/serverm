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
    <div class="headcasemenu">
        <div class="cases_content_data1">
            <div class="lists">
            {#each popupData as item, index}
                {#if !item.Done && item.winBlock && caseData.items[item.winBlock.ItemIndex]}
                <div class="bg">
                    <svg class="{caseData.items[item.winBlock.ItemIndex].color}" width="355" height="212" viewBox="0 0 355 212" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M354.082 201.213L6.20624 0L0 11.4326L347.223 211.666L354.082 201.213Z"/>
                        <path d="M0.331909 201.548L348.208 0.334961L354.414 11.7675L7.19144 212L0.331909 201.548Z"/>
                    </svg>                                                                
                    <div class="block">
                        <div class="linecase {caseData.items[item.winBlock.ItemIndex].color}"></div>
                        <img src="{document.cloud + `img/roulette/${caseData.items[item.winBlock.ItemIndex].image}.png`}" alt="">
                        <span>{caseData.items[item.winBlock.ItemIndex].title}</span>
                    </div>
                </div>
                {/if}
            {/each}
            </div>
        </div>
    </div>
    <div class="opencasebtns1">                  
        <div class="usebtncase" on:keypress={() => {}} on:click={() => Take(0, -1)}>
            <p>ЗАБРАТЬ СЕБЕ</p>
        </div>
        {#if onAllWinBlockPrice(popupData)}
        <div class="sellbtncase" on:keypress={() => {}} on:click={() => Sell(0, -1)}>
            <p>ПРОДАТЬ ЗА</p>
            <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="">      
            <p>{format("money", onAllWinBlockPrice(popupData))}</p>
        </div>
        {/if}
    </div>
{/if}