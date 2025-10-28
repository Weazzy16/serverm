<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let ranks;

    import { format } from 'api/formatter'

    const onCreateCallback = (rankName, score) => {
        let check = format("rank", rankName);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        score = Math.round(score.toString().replace(/\D+/g, ""));
        if (score < 0)
            score = 0;
        else if (score > 9999999)
            score = 9999999;

        executeClientToGroup('createRank', rankName, score)
    }

    const onCreate = () => {
        setPopup ("Input", {
            headerTitle: "Создайте ранг",
            headerIcon: "fractionsicon-rank",
            inputs: [
                {
                    title: "Название ранга",
                    placeholder: "Название ранга",
                    maxLength: 25,
                },
                {
                    title: "Количество баллов для получения ранга",
                    placeholder: "Количество баллов для получения ранга",
                    maxLength: 23,
                    type: "number"
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onCreateCallback
        })
    }

    //

    export let onSettings;

    import { spring } from 'svelte/motion';

    let coords = spring({ x: 0, y: 0 }, {
        stiffness: 1.0,
        damping: 1.0
    });

    let dragonDropItem = false;
    let offsetInElementX = 0;
    let offsetInElementY = 0;
    let mainElements = {}
    let copyArray = []

    const onStartDragonDrop = (event, index, item) => {
        copyArray = ranks;

        const target = mainElements[index].getBoundingClientRect();

        offsetInElementX = (target.width - (target.right - event.clientX));
        offsetInElementY = (target.height - (target.bottom - event.clientY));

        coords.set({ x: event.clientX, y: event.clientY });
        dragonDropItem = item;
    }

    const handleGlobalMouseMove = (event) => {
        if (dragonDropItem !== false) {
            coords.set({ x: event.clientX, y: event.clientY });
        }
    }

    const handleGlobalMouseUp = () => {
        if (dragonDropItem !== false) {
            dragonDropItem = false;
        }
    }

    const onUpdateIndex = (item) => {
        if (dragonDropItem !== false) {
            const index1 = ranks.findIndex(i => i.id === dragonDropItem.id);
            const item1 = Object.assign({}, ranks [index1]);
            const id1 = item1.id;
            const maxScore1 = item1.maxScore;
            if (typeof item1.oldId === "undefined")
                item1.oldId = item1.id;
            item1.id = item.id;
            item1.maxScore = item.maxScore;

            const index2 = ranks.findIndex(i => i.id === item.id);
            const item2 = Object.assign({}, ranks [index2]);
            if (typeof item2.oldId === "undefined")
                item2.oldId = item2.id;
            item2.id = id1;
            item2.maxScore = maxScore1;

            ranks [index1] = item2;
            dragonDropItem = item1;
            ranks [index2] = item1;
        }

    }

    import { onDestroy } from 'svelte';
    import { setPopup, isOrganization } from "../../data";

    let isSave = false;

    const saveRanks = () => {
        if (isSave)
            return;
        isSave = true;

        const updateRankId = {};

        ranks.forEach((item) => {
            if (typeof item.oldId !== "undefined" && item.oldId !== item.id) {
                updateRankId [item.oldId] = item.id;
            }
        });

        if (updateRankId && Object.keys(updateRankId).length)
            executeClientToGroup('updateRanksId', JSON.stringify(updateRankId))
    }

    onDestroy(saveRanks)


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

    const onSelectItem = (item) => {
        saveRanks ();
        onSettings (item);
    }

    let deleteId = 0;
    const onRankDeleteCallback = () => {
        executeClientToGroup('removeRank', deleteId)
    }

    const onRankDelete = (item) => {
        deleteId = item.id;
        setPopup ("Confirm", {
            headerTitle: "Снятие ранга",
            headerIcon: "fractionsicon-members",
            text: `Вы действительно хотите удалить '${item.id}. ${item.name}'?`,
            button: 'Удалить',
            callback: onRankDeleteCallback
        })
    }

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings ();
</script>

<svelte:window on:mouseup={handleGlobalMouseUp} on:mousemove={handleGlobalMouseMove} />

<div class="squadnav">
    <p>Ранг</p>
    <p>Название ранга</p>
    <p>Требуемый опыт</p>
    <p>Числиность</p>
    <div class="btns">
        {#if settings.isLeader && isOrganization()}
            <div class="btn" on:keypress={() => {}} on:click={onCreate}>{translateText('player1', 'Создать ранг')}</div>
        {/if}
    </div>
</div>
<div class="squadlist">
    {#if dragonDropItem}
        <div class="fractions__dragondrop_element" style={`top:${$coords.y - offsetInElementY}px;left:${$coords.x - offsetInElementX}px;`}>
            <div class="squad">
                <p>{dragonDropItem.id}</p>
                <b>{dragonDropItem.name}</b>
                <b>{dragonDropItem.maxScore} XP</b>
                <b>{dragonDropItem.playerCount} чел.</b>
                <div class="btns">
                    <div class="btn">{translateText('player1', 'Редактировать')}</div>
                </div>
            </div>
        </div>
    {/if}
    {#each ranks.filter(el => filterCheck(el.name, searchText)) as item, index}
        <div class="squad" bind:this={mainElements[index]} on:mouseenter={() => onUpdateIndex (item)}>
            {#if isOrganization()}
                <p on:mousedown={(e) => onStartDragonDrop (e, index, item)}>{item.id}</p>
                {:else}
                <p>{item.id}</p>
            {/if}
            <b>{item.name}</b>
            <b>{item.maxScore} XP</b>
            <b>{item.playerCount} чел.</b>
            <div class="btns">
                <div class="btn" on:keypress={() => {}} on:click={() => onSelectItem (item)}>{translateText('player1', 'Редактировать')}</div>
            </div>
        </div>
    {/each}
</div>