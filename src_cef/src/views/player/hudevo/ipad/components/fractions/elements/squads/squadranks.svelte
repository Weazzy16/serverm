<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let settings;
    export let onSettings;

    //search

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }

    //
    import { setPopup } from "../../data";

    import SquadInfo from './squadinfo.svelte'

    let selectRank = null

    const onSelectRank = (item = null) => {
        selectRank = item
    }

    $: if (settings) {
        if (selectRank) {
            const index = settings.ranks.findIndex(a => a.id === selectRank.id)

            if (settings.ranks [index])
                onSelectRank(settings.ranks [index])
        }
    }
</script>

{#if selectRank}
    <SquadInfo departmentId={settings.id} {selectRank} {onSelectRank} />
{:else}
    <div class="appmenufrac squad">
        <div class="squadnav">
            <p>Ранг</p>
            <p>Название</p>
            <p>Участники</p>
        </div>
        <div class="squadlist">
            {#each settings.ranks.filter(el => filterCheck(el.name, searchText)) as item, index}
                <div class="squaduser">
                    <p>{item.id}</p>
                    <p>{item.name}</p>
                    <p>{item.playerCount} чел.</p>
                    <div class="btninfo" on:keypress={() => {}} on:click={() => onSelectRank (item)}>{translateText('player1', 'Редактировать')}</div>
                </div>
            {/each}
        </div>
    </div>
{/if}