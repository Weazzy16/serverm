<script>
    import { translateText } from 'lang'
    import { executeClientToGroup } from 'api/rage'
    import Access from '../access/index.svelte'
    import { charUUID } from 'store/chars';

    executeClientToGroup('membersLoad')

    let members = []

    const getMembers = (_member, _members, _onlineData) => {

        if (_members && typeof _members === "string")
            members = JSON.parse(_members);

        if (members.length > 0)
            onSelectUUId (members[0])
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.members", getMembers)

    const UpdateMember = (_member) => {
        _member = JSON.parse(_member);

        let index = members.findIndex((m) => m.uuid === _member.uuid);

        if (members [index]) {
            members [index] = _member;
        }
    }

    addListernEvent ("table.updateMember", UpdateMember)

    let selectUUId = -1;
    const onSelectUUId = (item) => {
        executeClientToGroup('rankAccessInit', JSON.stringify(item.access), JSON.stringify(item.lock))
        selectUUId = item.uuid;
    }
</script>
<div class="appmenufrac access">
    <div class="leftacce">
        <h1>Отряды:</h1>
        <div class="listacce">
            {#each members as item}
                {#if $charUUID !== item.uuid}
                    <div class="blockacce" class:active={selectUUId === item.uuid} on:keypress={() => {}} on:click={() => onSelectUUId(item)}>
                        <p>{item.uuid}</p>
                        <b>{item.name}</b>
                    </div>
                {/if}
            {/each}
        </div>
    </div>
    <div class="rightacce">
            {#if selectUUId >= 0}
                <Access
                    itemId={selectUUId}
                    executeName="updateMemberRankAccess"
                    isSelector={false}
                    clsScroll="w-399 h-480"
                    isSkip={true} />
                {:else}
                    <div class="fractions__main_scroll w-399 h-480" />
                {/if}
    </div>
</div>