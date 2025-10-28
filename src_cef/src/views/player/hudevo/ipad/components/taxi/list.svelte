<script>

    import HomeButton from '../homebutton.svelte'

    import { fade } from 'svelte/transition'

    export let setOtherElement;
    export let onSelectView;
    export let closeMenu;

</script>

<div class="newphone__maps_categories" in:fade use:setOtherElement>
    <div class="newphone__maps_subcategories">
        <div class="box-between newphone__project_padding20">
            <div class="newphone__maps_header"></div>
            <div class="phoneicons-add1" on:keypress={() => {}} on:click={closeMenu}></div>
        </div>
        <div class="newphone__maps_list">
            <div class="newphone__maps_title">Wählen Sie eine Option aus</div>
            <div class="newphone__maps_subtitle">Hier können Sie ein Taxi rufen oder als Taxifahrer arbeiten</div>
            <div class="newphone__maps_selected" on:keypress={() => {}} on:click={()=> onSelectView ("Client")}>
                <div class="newphone__maps_square">
                    <div class="phoneicons-npc newphone__maps_elementicon"></div>
                </div>
                <div class="box-column">
                    <div class="newphone__maps_headertitle">Kunde</div>
                    <div class="newphone__maps_headersubtitle">Möchten Sie ein Taxi bestellen?</div>
                </div>
                <div class="phoneicons-Button newphone__chevron-back"></div>
            </div>
            <div class="newphone__maps_selected" on:keypress={() => {}} on:click={()=> onSelectView ("Driver")}>
                <div class="newphone__maps_square">
                    <div class="phoneicons-cars newphone__maps_elementicon"></div>
                </div>
                <div class="box-column">
                    <div class="newphone__maps_headertitle">Fahrer</div>
                    <div class="newphone__maps_headersubtitle">Möchten Sie als Taxifahrer arbeiten?</div>
                </div>
                <div class="phoneicons-Button newphone__chevron-back"></div>
            </div>
        </div>
        <HomeButton />
    </div>
</div>