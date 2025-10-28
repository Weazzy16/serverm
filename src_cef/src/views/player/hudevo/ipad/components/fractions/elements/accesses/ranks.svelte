<script>
    import { translateText } from 'lang'
    import {executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import Access from '../access/index.svelte'

    let ranks = []
    executeClientAsyncToGroup("getRanks").then((result) => {
        if (result && typeof result === "string") {
            ranks = JSON.parse(result);
            onSelectId (ranks[0])
        }
    });


    let selectId = -1;
    const onSelectId = (item) => {
        executeClientToGroup('rankAccessLoad', item.id)
        selectId = item.id;
    }
</script>
<div class="appmenufrac access">
    <div class="leftacce">
        <h1>Отряды:</h1>
        <div class="listacce">
            {#each ranks as item, index}
                {#if (ranks.length - 1) !== index}
                    <div class="blockacce" class:active={selectId === item.id} on:keypress={() => {}} on:click={() => onSelectId(item)}>
                        <p>{item.id}</p>
                        <b>{item.name}</b>
                    </div>
                {/if}
            {/each}
        </div>
    </div>
    <div class="rightacce">
            {#if selectId >= 0}
                <Access
                    itemId={selectId}
                    executeName="updateRankAccess"
                    isSelector={false}
                    clsScroll="w-399 h-480" />
            {:else}
                <div class="fractions__main_scroll w-399 h-480" />
            {/if}
    </div>
</div>
