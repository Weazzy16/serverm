<script>
    export let selectedId;
    export let onSelectedView;


    let isLoad = false;
    const updateLoad = () => isLoad = true;

    import { executeClientToGroup, executeClient } from "api/rage";
    executeClientToGroup ("house.load");

    import { addListernEvent } from 'api/functions';
    addListernEvent ("tabletHouseInit", updateLoad);

    import Menu from './menu.svelte';
    import Upgrade from './upgrade.svelte';
    import Furniture from './furniture.svelte';
    import Residents from './resident.svelte';


    const viewsHouse = {
        Menu,
        Upgrade,
        Furniture,
        Residents
    }

    let selectedViewHouse = "Menu"

    const onSelectedViewHouse = (view) => selectedViewHouse = view;

    import { onDestroy } from 'svelte';
    import Loadersmall from '../../loadersmall.svelte';
    onDestroy(() => {
        executeClient ("client.tablet.house.close");
    });
</script>
{#if !isLoad}
    <Loadersmall />
{:else}
    <svelte:component this={viewsHouse[selectedViewHouse]} {onSelectedView} {selectedId} {onSelectedViewHouse} />
{/if}