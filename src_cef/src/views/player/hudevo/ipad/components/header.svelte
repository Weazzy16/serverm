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
<div class="newphone__header"> <!-- style={$currentPage == "maps" ? "color:white" : ""} -->
    <div class="box-flex">
        <div class="time">{TimeFormat ($serverDateTime, "H:mm")}</div>
        <span class="phoneicons-location"></span>
    </div>
    <div class="newphone__header_image"></div>
    <div class="box-between">
        {#if $charSim !== -1}
            <div class="newphone__header_text ml-auto">5G</div>
        {:else}
            <div class="newphone__header_text">SIM fehlt</div>
        {/if}
        <svg width="26" height="37" viewBox="0 0 26 37" fill="none" xmlns="http://www.w3.org/2000/svg" style="width: 0.7407vh;height: 1.1111vh;">
            <g opacity="0.5">
            <path d="M7.86602 4.2665V2.4665C7.86602 1.3865 8.58602 0.666504 9.66602 0.666504H16.866C17.946 0.666504 18.666 1.3865 18.666 2.4665V4.2665H24.066C25.146 4.2665 25.866 4.9865 25.866 6.0665V34.8665C25.866 35.9465 25.146 36.6665 24.066 36.6665H2.46602C1.38602 36.6665 0.666016 35.9465 0.666016 34.8665V6.0665C0.666016 4.9865 1.38602 4.2665 2.46602 4.2665H7.86602ZM15.066 18.6665V9.6665L6.06602 22.2665H11.466V31.2665L20.466 18.6665H15.066Z" fill="white"></path>
            <rect x="3.64648" y="7.95068" width="17.6344" height="23.9625" fill="white"></rect>
            </g>
            <path d="M0.693359 13.1001H25.8371V34.8907C25.8371 35.8572 25.0536 36.6407 24.0871 36.6407H2.44336C1.47686 36.6407 0.693359 35.8572 0.693359 34.8907V13.1001Z" fill="#D9D9D9"></path>
        </svg>
    </div>
</div>