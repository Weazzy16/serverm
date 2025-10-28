<script>
    import { translateText } from 'lang'
    import { executeClientToGroup } from "api/rage";
    import { format } from 'api/formatter'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    export let settings;

    import Access from '../access/index.svelte'

    executeClientToGroup('rankAccessLoad', settings.id)

    export let onSettings;

    let rankName = "";
    const onUpdateName = () => {
        let check = format("rank", rankName);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('updateRankName', settings.id, rankName)
        rankName = "";
    }

    let score = settings.maxScore;
    const onUpdateScore = () => {
        score = Math.round(score.replace(/\D+/g, ""));
        if (score < 0) score = 0;
        else if (score > 9999999) score = 9999999;

        executeClientToGroup('updateRankScore', settings.id, score)
    }
</script>
<div class="appmenufrac access">
    <div class="leftacce">
        <h1>{translateText('player1', 'Сменить название')}</h1>
        <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={rankName} type="text" placeholder={translateText('player1', 'Введите название..')}>
        <div class="btnrank" on:keypress={() => {}} on:click={onUpdateName}>{translateText('player1', 'Изменить')}</div>
        <h1>{translateText('player1', 'XP до получения')}</h1>
        <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={score} type="number" placeholder={translateText('player1', 'Введите кол-во..')}>
        <div class="btnrank" on:keypress={() => {}} on:click={onUpdateScore}>{translateText('player1', 'Изменить')}</div>
    </div>
    <div class="rightacce">
        <div class="listacce">
            <Access title={{
                icon: 'fractionsicon-rank',
                name: settings.name
            }}
            itemId={settings.id}
            executeName="updateRankAccess"
            isSelector={true}
            mainScroll="h-568"
            clsScroll="big" />
        </div>
    </div>
</div>