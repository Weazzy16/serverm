<script>

    import { executeClientToGroup } from "api/rage";
    export let count;
    export let selectPage;

    const maxPage = 5;


    $: pageCount = Array.from(
        Array(count).keys()
    )

    const slice = (arr, page) => {
        if (page < maxPage) {
            return arr.slice(0, maxPage)
        } else if (page > arr.length - (maxPage - 1)) {
            return arr.slice(arr.length - maxPage, arr.length)
        }
        return arr.slice(page - 2, page + 1)
    }

    $: buttons = slice(pageCount, selectPage)

    let pageType = "up"
    const setPage = (number) => {
        if (number <= 0)
            return;
        if (number > count)
            return;
        pageType = selectPage <= number ? "up" : "down";

        if (number === 1)
            pageType = "up";
        else if (number === count)
            pageType = "down";

        executeClientToGroup("getBoard", number);
    }
</script>

<div class="leftn" on:keypress={() => {}} on:click={() => setPage(selectPage - 1)}>
    <svg width="8" height="13" viewBox="0 0 8 13" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M6.38748 0.816406L1.88538 5.69368C1.53179 6.07674 1.53179 6.66718 1.88538 7.05024L6.38748 11.9275" stroke="white" stroke-opacity="0.2" stroke-width="1.3" stroke-linecap="round"></path>
    </svg>                                        
</div>
{#if pageCount.length > 0}
        <div class="numbernw" class:active={selectPage === 1} on:keypress={() => {}} on:click={() => setPage(1)}>
            1
        </div>
    {/if}
    {#if /*pageType === "down" && */pageCount.length > (maxPage + 1) && selectPage >= maxPage}
        <div class="numbernw">
            ..
        </div>
    {/if}

    {#each buttons as n}
        {#if n > 0 && n < pageCount.length - 1}
            <div class="numbernw" class:active={selectPage === n + 1} on:keypress={() => {}} on:click={() => setPage(n + 1)}>{n + 1}</div>
        {/if}
    {/each}

    {#if /*pageType === "up" && */pageCount.length > (maxPage + 1) && selectPage <= pageCount.length - 3}
        <div class="numbernw">
            ..
        </div>
    {/if}
    
    {#if pageCount.length > 1}
        <div class="numbernw" class:active={selectPage === pageCount.length} on:keypress={() => {}} on:click={() => setPage(pageCount.length)}>
            {pageCount.length}
        </div>
    {/if}

<div class="right" on:keypress={() => {}} on:click={() => setPage(selectPage + 1)}>
    <svg width="7" height="13" viewBox="0 0 7 13" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M1.25933 0.816406L5.75958 5.69384C6.11297 6.07686 6.11297 6.66706 5.75958 7.05008L1.25933 11.9275" stroke="white" stroke-opacity="0.2" stroke-width="1.3" stroke-linecap="round"></path>
    </svg>                                        
</div>
