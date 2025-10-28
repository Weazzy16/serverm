<script>
    import { translateText } from 'lang'
    import { getPngUrl, getTimeFromMins } from './data'
    import { getPngToItemId } from '@/views/player/menu/getPng.js'
    export let item;
    
    let intervalId = null;
    if (!item.status) {
        intervalId = setInterval(() => {
            if (--item.time <= 0)
                onDestroyInterval ();
        }, 1000);
    }

    const onDestroyInterval = () => {
        if (intervalId !== null)
            clearInterval(intervalId);

        intervalId = null;
    }

    import { onDestroy } from 'svelte';
    onDestroy(onDestroyInterval)

    const getProgress = (value, max) => {
        let progress = value / max * 100;

        if (progress > 100)
            progress = 100;

        return progress;
    }

</script>

<div class="taskblock">
    <p>{item.desc}</p>
    {#each item.awards as award}
        <img src="{getPngToItemId ({
            Name: "",
            Png: "",
            Type: award.type,
            ItemId: award.itemId,
            Data: award.data,

        }).png}" alt="">
        {/each}
    <p>{item.count}/{item.maxCount}</p>
    {#if item.status}
        <b>{translateText('player1', 'Выполнено')}</b>
        {:else}
        <b> &#128338; {getTimeFromMins (item.time)}</b>
    {/if}
</div>