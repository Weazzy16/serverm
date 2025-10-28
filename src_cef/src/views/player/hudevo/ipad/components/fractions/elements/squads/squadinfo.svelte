<script>
    import { translateText } from 'lang'
    import Access from '../access/index.svelte'
    import { executeClientToGroup } from "api/rage";
    import { format } from 'api/formatter'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let departmentId;
    export let selectRank;
    export let onSelectRank;

    executeClientToGroup('rankAccessInit', JSON.stringify(selectRank.access), JSON.stringify(selectRank.lock))

    let rankName = "";
    const onUpdateName = () => {
        let check = format("rank", rankName);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('updateDepartmentRankName', departmentId, selectRank.id, rankName)
        rankName = "";
    }
</script>
<div class="appmenufrac access">
    <div class="leftacce">
        <h1>{translateText('player1', 'Сменить название')}</h1>
        <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={rankName} type="text" placeholder={translateText('player1', 'Введите название..')}>
        <div class="btnrank" on:keypress={() => {}} on:click={onUpdateName}>{translateText('player1', 'Изменить')}</div>
    </div>
    <div class="rightacce">
        <div class="listacce">
            <Access title={{
                icon: 'fractionsicon-rank',
                name: selectRank.name
            }}
            itemId={`${departmentId}|${selectRank.id}`}
            executeName="updateDepartmentRankAccess"
            isSelector={true}
            isSkip={true}
            mainScroll="h-568"
            clsScroll="big" />
        </div>
    </div>
</div>