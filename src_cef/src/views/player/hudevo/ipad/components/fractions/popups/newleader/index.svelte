<script>
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    import { setPopup } from "../../data";
    import { charUUID } from 'store/chars';

    import { executeClientToGroup } from 'api/rage'
    executeClientToGroup('membersLoad')

    let members = [];
    let selectItem = null;

    const onConfirmCallback = () => {
        executeClientToGroup("setLeader", selectItem.uuid)
        setPopup ()
    }

    const onSelectItem = (item = null) => {

        selectItem = item;
    }

    const getMembers = (_member, _members, _onlineData) => {
        if (_members && typeof _members === "string") {
            members = JSON.parse(_members);
        }
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.members", getMembers)

</script>

{#if selectItem}
    <div class="popup__newhud">
        <div class="popup__newhud_box">
            <div class="popup__newhud_title">
                <span class="popup__newhud_icon hud__icon-inventory"/> Transfer of power
            </div>
            <div class="popup__newhud_text">You really want to make a leader {selectItem.name}?</div>

            <div class="popup__newhud__buttons">
                <div class="popup__newhud_button" on:keypress={() => {}} on:click={onConfirmCallback}>Да</div>
                <div class="popup__newhud_button" on:keypress={() => {}} on:click={() => onSelectItem()}>{translateText('popups', 'Отмена')}</div>
            </div>
        </div>
    </div>
{:else}

<div class="popup__newhud_boxinput">
    <div class="popup__newhud_box">
        <div class="popup__newhud_title">
            <span class="popup__newhud_icon fractionsicon-members"/> Transfer of power
        </div>

        <div class="popup__newhud_subtitle">
            Choose a participant
        </div>
        <div class="popup__select_elements">
            {#each members as member, index}
                {#if member.uuid !== $charUUID}
                <div class="popup__select_element" on:keypress={() => {}} on:click={() => onSelectItem (member)}>
                    {member.name}
                </div>
                {/if}
            {/each}
        </div>

        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 750 }} on:keypress={() => {}} on:click={() => setPopup ()}>{translateText('popups', 'Отмена')}</div>
        </div>
    </div>
</div>
{/if}