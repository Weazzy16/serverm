<script>
    import { translateText } from 'lang'
    import './main.sass';
    import './fonts/style.css';

    import { getQuest, getActors } from 'json/quests/quests.js';
	import { fade, fly } from 'svelte/transition';
    import { executeClient } from 'api/rage';
    import Achivment from '@/views/quests/achivment/index.svelte';   
    
    export let viewData;

    const volumeSound = 0.25;

    let questData = {
        actorName: "npc_taxi",
        data: {
                buttons:[
                    {name: "Устроиться на работу.", type: "perform"},
                    {name: "Арендовать рабочее авто.", type: "action"},
                    {name: "Неинтересно."}
                ],
                text:"Привет!",
                sound: "cloud/npc/hunt/hunter1.ogg"
        },
        select: 0
    }

    let duration;

    let videoData = {
        data: 0,
        time: 0,
        show: false,
        timeoutId: 0,
        intervalId: 0
    }   
    
    let selectTitleData = 0;
    let titleData = [];
    
    let QuestCompilityData = false;

    const handleArrowKeys = (events) => {
        const { keyCode } = events;
        switch (keyCode) {
            case 37:
                if (questData && questData.data) {
                    if(--questData.select < 0) questData.select = questData.data.buttons.length - 1;
                }
                break;
            case 39:
                if (questData && questData.data) {
                    if(++questData.select > questData.data.buttons.length - 1) questData.select = 0;
                }
                break;
            case 13: 
                handleEnter ();
                break;
            case 27: 
                handleEsc ();
                break;
        }
    }

    const handleEsc = () => {
        if (QuestCompilityData) {
            clearData ();
            executeClient("client.quest.perform", true);
        } else if (questData && questData.data) {
            clearData ();
            executeClient("client.quest.close");
        }
    }

    const handleEnter = () => {
        if (questData.data.buttons && questData.data.buttons.length && questData.data.buttons[questData.select] !== undefined)
            clickButton (questData.data.buttons[questData.select]);
        else if (QuestCompilityData && QuestCompilityData.actorName) { 
            if (QuestCompilityData.selectId) {           
                questData.select = 0;
                questData.actorName = QuestCompilityData.actorName;
                questData.data = getQuest(QuestCompilityData.actorName, QuestCompilityData.selectId);

                if (questData.data.sound) 
                    executeClient ("sounds.playQuest", questData.data.sound, volumeSound);

                titleData = [
                    ...titleData,
                    questData.data.text
                ];
                selectTitleData++;
                QuestCompilityData = false;
                executeClient("client.quest.perform", false);
            } else {
                QuestCompilityData = false;
                executeClient("client.quest.perform", true);
            }
        }
    }

    const handleMouseMove = () => {
        // Делает интерфейс видимым, и скрывает его
        // через 2.5 секунды бездействия
		clearTimeout(videoData.timeoutId);
		videoData.timeoutId = setTimeout(() => videoData.show = false, 5000);
		videoData.show = true;
    }

    /////////////////////////////////
    const clickButton = (item, startCutScene = true) => {
        if (item.questId) {
            if (item.type === "perform") { //Сначала открываем диалог с наградой
                if (questData.data.Reward) {
                    QuestCompilityData = {
                        actorName: questData.actorName,
                        selectId: item.questId,
                        Desc: questData.data.Desc,
                        Reward: questData.data.Reward
                    };
                    clearData (false);
                } else {
                    executeClient("client.quest.perform", false);

                    questData.select = 0;
                    questData.data = getQuest(questData.actorName, item.questId);

                    if (questData.data.sound) 
                        executeClient ("sounds.playQuest", questData.data.sound, volumeSound);

                    titleData = [
                        ...titleData,
                        questData.data.text
                    ];
                    selectTitleData++;
                }
            } else {
                if (item.type === "action") 
                    executeClient("client.quest.action", false);
                    
                questData.select = 0;
                questData.data = getQuest(questData.actorName, item.questId);
                    
                if (questData.data.sound) 
                    executeClient ("sounds.playQuest", questData.data.sound, volumeSound);

                titleData = [
                    ...titleData,
                    questData.data.text
                ];
                selectTitleData++;
            }
        } else if (item.type === "take") {
            clearData ();
            if (item.index != undefined) 
                executeClient("client.quest.take", item.index);
            else
                executeClient("client.quest.take", 0); 
        } else if (item.type === "action") {
            clearData ();
            executeClient("client.quest.action", true);
        } else if (item.type === "perform") {
            if (questData.data.Reward) {
                QuestCompilityData = {
                    actorName: questData.actorName, 
                    Desc: questData.data.Desc,
                    Reward: questData.data.Reward
                };
                clearData (false);
            } else {
                executeClient("client.quest.perform", true);
                clearData ();
            }
        } else {
            clearData ();
            executeClient("client.quest.close");
        }
    }
    
    const clearData = (clearCompilityData = true) => {
        if (clearCompilityData)
            QuestCompilityData = false;
        questData = {
            actorName: 0,
            data: 0,
            select: 0
        };  

        executeClient ("sounds.stopQuest");

        titleData = []
        selectTitleData = 0;
        //window.router.setHud ();
    }

    /**
     * aName (string) - Библиотека бота
     * qID (int) - Номер квеста
     * status (bool) - Взят ли квестэ
     * compility (int) - Выполнен ли квест (0 - Если нет | 1... - Если выполнен)
     */ 

    const showDialogQuests = (aName, qId, status, compility) => {
        questData.select = 0;
        questData.actorName = aName;
        if (qId === -1) { //Если квестовая линия не доступна
            questData.data = getQuest(aName, -1);
        } else if (status === 0) {//Если квестовая линия взята но с актером еще не говорили
            questData.data = getQuest(aName, qId);
        } else if (status === 1 && compility == 0) { //Если уже поговорили с актером но квест не выполнен
            questData.data = getQuest(aName, qId);
            if (typeof questData.data.textError !== "undefined") {
                questData.data.text = questData.data.textError;
                questData.data.buttons = questData.data.buttonsError;
                questData.data.sound = questData.data.soundError;
            }
        } else if (status === 1 && compility != 0) { //Если уже поговорили с актером но квест выполнен
            questData.data = getQuest(aName, qId);
            if (typeof questData.data.textSuccess !== "undefined") {
                questData.data.text = questData.data.textSuccess;
                questData.data.buttons = questData.data.buttonsSuccess;
                questData.data.sound = questData.data.soundSuccess;
            }
        }

        if (questData.data.sound) 
            executeClient ("sounds.playQuest", questData.data.sound, volumeSound);

        titleData = [
            questData.data.text
        ];
        selectTitleData = 0;
    }

    if (viewData && viewData.aName !== undefined && viewData.qId !== undefined && viewData.status !== undefined && viewData.compility !== undefined) {
        showDialogQuests (viewData.aName, viewData.qId, viewData.status, viewData.compility);
    }
    
    const onLeft = () => {
        if (--selectTitleData < 0)
            selectTitleData = titleData.length - 1;
    }
    
    const onRight = () => {        
        if (++selectTitleData >= titleData.length)
            selectTitleData = 0;
    }
</script>
<svelte:window on:keyup={handleArrowKeys} />
{#if questData && questData.data}
<div class="dialog-main">
    <div class="shadow"></div>
    <div class="dialog-container">
        <div class="row-block">
            <div class="speaker-name">{getActors (questData.actorName).name}</div>
            <div class="speaker-info">{getActors (questData.actorName).type}</div>
        </div>
        <div class="speaker-speech">{titleData [titleData.length - 1]}</div>
        <div class="answers-container">
            {#each questData.data.buttons as item, index}
            <div class="answer-content" on:click={() => clickButton(item)}>{item.name}</div>
            {/each}
        </div>
    </div>
</div>
{/if}
