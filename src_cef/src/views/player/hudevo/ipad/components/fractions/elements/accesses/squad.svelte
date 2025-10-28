<script>
    import { translateText } from 'lang'
    import {executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import Access from '../access/index.svelte'
    import {setPopup} from "../../data";

    let departments = []
    executeClientAsyncToGroup("getDepartments").then((result) => {
        if (result && typeof result === "string") {
            departments = JSON.parse(result);
        }
    });


    let selectDepartmentId = -1;
    const onSelectDepartmentId = (item) => {
        executeClientToGroup('departmentRankLoad', item.id)
        selectDepartmentId = item.id;
        selectId = -1;
    }

    let ranks = []
    const departmentRankInit = (_ranks) => {
        if (_ranks && typeof _ranks === "string")
            ranks = JSON.parse(_ranks);

        onSelectId (ranks[0])
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.departmentRankInit", departmentRankInit)


    let selectId = -1;
    const onSelectId = (item) => {
        executeClientToGroup('departmentRankAccessLoad', selectDepartmentId, item.id)
        selectId = item.id;
    }
</script>
<div class="appmenufrac access">
    <div class="leftacce">
        <h1>{translateText('player1', 'Отряды')}:</h1>
        <div class="listacce">
            {#each departments as item}
                <div class="blockacce" class:active={selectDepartmentId === item.id} on:keypress={() => {}} on:click={() => onSelectDepartmentId(item)}>
                    <p>{item.id}</p>
                    <b>{item.name}</b>
                </div>
            {/each}
        </div>
    </div>
    <div class="centeracce">
        <h1>Ранги:</h1>
        <div class="listacce">
            {#if selectDepartmentId >= 0}
                {#each ranks as item}
                    <div class="blockacce" class:active={selectId === item.id}  on:keypress={() => {}} on:click={() => onSelectId(item)}>
                        <p>{item.id}</p>
                        <b>{item.name}</b>
                    </div>
                {/each}
            {/if}
        </div>
    </div>
    <div class="rightacce">
            {#if selectId >= 0}
                <Access
                        itemId={`${selectDepartmentId}|${selectId}`}
                        executeName="updateDepartmentRankAccess"
                        isSelector={false}
                        clsScroll="w-399 h-480"
                        isSkip={true} />
                {:else}
                    <div class="fractions__main_scroll w-399 h-480" />
            {/if}
    </div>
</div>