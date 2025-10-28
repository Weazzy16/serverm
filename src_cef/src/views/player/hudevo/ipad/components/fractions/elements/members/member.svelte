<script>
    import { translateText } from 'lang'
    import {executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    export let selectMember;
    export let member;

    //Логи

    import moment from 'moment';
    let isLog = false;
    executeClientAsyncToGroup("isLog").then((result) => {
        isLog = result;
        onGetLogs (0);
    });
    let logType = -1;

    let searchText = "";
    const onGetLogs = (index) => {
        if (!isLog)
            return;

        if (logType === index)
            return;

        logType = index;

        if (!searchText)
            searchText = "";

        if (index === 0) //Общий
            getLog (selectMember.uuid, false, searchText, 0)
        else if (index === 1) //Склад
            getLog (selectMember.uuid, true, searchText, 0)
    }

    let logs = []
    const getLogs = () => {
        executeClientAsyncToGroup("getLogs").then((result) => {
            if (result && typeof result === "string")
                logs = JSON.parse(result);
        });
    }

    import { addListernEvent } from "api/functions";
    import {getLog, setPopup, onUpdateRank} from "../../data";
    addListernEvent ("table.logs", getLogs)

    import Access from '../access/index.svelte'

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 13 && searchText && searchText.length >= 1) {
            onGetLogs ()
            searchText = ""
        }
    }

    //

    const onUpdateDepartmentCallback = (rank) => {
        executeClientToGroup('invitePlayerDepartment', rank, selectMember.uuid);

    }
    const onUpdateDepartment = () => {
        executeClientAsyncToGroup("getDepartments").then((result) => {
            if (result && typeof result === "string") {
                result = JSON.parse(result);

                if (!result || !result.length)
                    return;

                let departments = [];

                if (selectMember.departmentId > 0 && (settings.isLeader || result.findIndex(r => Number(i.id) === Number(member.departmentId) &&  Number(r.id) === Number(selectMember.departmentId)) !== -1))
                    departments.push({
                        id: 0,
                        name: "Без отряда"
                    })

                result.forEach(item => {
                    if (settings.isLeader || Number(item.id) === Number(member.departmentId)) {
                        departments.push(item)
                    }
                });

                setPopup("List", {
                    headerTitle: "Give glasses",
                    headerIcon: "fractionsicon-members",
                    listTitle: 'Choose a detachment',
                    list: departments,
                    button: 'Choose',
                    callback: onUpdateDepartmentCallback
                })
            }
        });
    }

    const onReprimandCallback = (title, text) => {
        executeClientToGroup("reprimand", selectMember.uuid, title, text);
    }

    const onReprimand = () => {
        setPopup ("Input", {
            headerTitle: "Упрек",
            headerIcon: "fractionsicon-news",
            inputs: [
                {
                    title: "Основание для выговора",
                    placeholder: "Введите заголовок",
                    maxLength: 20
                },
                {
                    title: "Текст выговора",
                    placeholder: "Введите текст",
                    maxLength: 100,
                    textarea: true
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onReprimandCallback
        })
    }

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string") {
                settings = JSON.parse(result);

                if (settings.isLeader)
                    executeClientToGroup('rankAccessInit', JSON.stringify(selectMember.access), JSON.stringify(selectMember.lock))
            }
        });
    }
    getSettings();

</script>

<svelte:window on:keyup={HandleKeyDown} />
<div class="appmenufrac access" style="height: 62.5vh;margin-top: 1vh;border: none;">
    {#if settings.isLeader}
    <div class="rightacce">
        <div class="listacce" style="height: 61vh;">
            <Access title={{
                icon: 'fractionsicon-fractionsicon-stats',
                name: translateText('player1', 'Управление доступом')
            }}
            itemId={selectMember.uuid}
            executeName="updateMemberRankAccess"
            isSelector={true}
            mainScroll="h-430"
            clsScroll=""
            isSkip={true} />
        </div>
    </div>
    {/if}
    <div class="right">
        <div class="logs">
            <h1>Логи:</h1>
            <div class="btns">
                <div class="btnbro" class:active={logType === 0} on:keypress={() => {}} on:click={() => onGetLogs (0)}>
                    <p>Общий</p>
                </div>
                <div class="btnbro" class:active={logType === 1} on:keypress={() => {}} on:click={() => onGetLogs (1)}>
                    <p>Склад</p>
                </div>
            </div>
            <div class="logss">
                {#if logs && logs.length}
                    {#each logs as log}
                        <div class="blockl">
                            {#if log.uuid}
                            <div class="left">
                                <p>{log.name} <b>{log.rankName}</b></p>
                            </div>
                            {/if}
                            <div class="center">
                                <p>{log.text}</p>
                            </div>
                            <div class="right">
                                <p>{moment(log.time).format('DD.MM.YYYY')} <b>{moment(log.time).format('HH:mm')}</b></p>
                            </div>
                        </div>
                    {/each}
                    {:else}
                    <p>{translateText('player1', 'Никаких действий не замечено!')}</p>
                {/if}
            </div>
        </div>
    </div>
</div>