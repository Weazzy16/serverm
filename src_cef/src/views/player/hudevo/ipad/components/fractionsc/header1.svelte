<script>
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
	import { onMount, onDestroy } from 'svelte';
    import { charSim } from "store/chars";

    let currentLink  = 0;

    let intervalId = -1;
	onMount(() => {
		intervalId = setInterval(() => {

            currentLink++;

            if (currentLink >= 3)
                currentLink = 0;

        }, 10 * 1000);
	});

	onDestroy(() => {
		if (intervalId !== -1)
            clearInterval (intervalId);
	});
</script>
<div class="newtablet__header1"> <!-- style={$currentPage == "maps" ? "color:white" : ""} -->
    <div class="box-flex">
        <div class="time">{TimeFormat ($serverDateTime, "H:mm")}</div>
        <span class="tableticons-location"></span>
    </div>
    <div class="newtablet__header_image1"></div>
    <div class="box-between">
        {#if $charSim !== -1}
            <div class="newtablet__header_text ml-auto">5G</div>
        {:else}
            <div class="newtablet__header_text">Нет SIM </div>
        {/if}
        <span class="tableticons-battery newtablet__header_battery"></span>
    </div>
</div>