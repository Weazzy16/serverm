<script>
    import Element from './elements/index.svelte';
    export let onSetLoad;

    let rewardList = [];

    onSetLoad (true);
    const updateLoad = (json) => {
        onSetLoad (false);

        rewardList = JSON.parse(json)
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("table.tasks", updateLoad);
</script>

<div class="appmenufrac task1">
    <div class="tasknav">
        <p>Задание</p>
        <p>Награда</p>
        <p>Прогресс</p>
        <p>Осталось времени</p>
    </div>
    <div class="tasklist">
        {#each rewardList as item}
            <Element {item} />
        {/each}
    </div>
</div>