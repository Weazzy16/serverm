<script>
    import { translateText } from 'lang'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'

    import Loader from './../loader.svelte'
    import { otherStatsData } from 'store/account'
    import { charData } from 'store/chars';
    export let visible = false;
    let selectCharData = $charData;   

    let useVisible = -1;

    $: {
        if (useVisible != visible) {
            if (visible && $otherStatsData.Name/* && $otherStatsData.UUID !== selectCharData.UUID*/) {
                selectCharData = $otherStatsData;
            } else if (visible && !$otherStatsData.Name && selectCharData !== $charData) {
                selectCharData = $charData;
            } else if (!visible && $otherStatsData.Name) {
                selectCharData = $charData;
                window.accountStore.otherStatsData ('{}');
            }
            useVisible = visible;
        }
    }

    let isLoad = false;//false
    const updateLoad = () => isLoad = true;

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("loadProperty");

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phoneMainPropertyLoad", updateLoad);

    import Business from './business/index.svelte';
    import {onDestroy} from "svelte";

    const views = {
        Business
    }

    let selectedView = "Business"

    const onSelectedView = (view = false) => selectedView = !view ? "Business" : view;

    let selectedId = selectCharData.BizId;
    const onSelectedId = (id) => selectedId = id;
    import { fade } from 'svelte/transition'
</script>

{#if !isLoad}
    <div></div>
{:else}
    <svelte:component this={views[selectedView]} {onSelectedView} {onSelectedId} {selectedId} {selectCharData} />
{/if}

