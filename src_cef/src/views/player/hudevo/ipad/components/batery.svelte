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
<div class="newtablet__headerb"> <!-- style={$currentPage == "maps" ? "color:white" : ""} -->
    <div class="box-flex">
    
    </div>
    <div class="newtablet__header_imageb"></div>
    <div class="box-between">

            <div class="newtablet__header_text ml-auto">5G</div>
      
        
    </div>
</div>