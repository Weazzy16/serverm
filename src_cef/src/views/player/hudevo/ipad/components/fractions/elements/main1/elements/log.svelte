<script>
    import { translateText } from 'lang'
    import { setView, getLog } from "../../../data";
    import {executeClientAsyncToGroup} from "api/rage";
    import moment from 'moment';

    import { charUUID } from 'store/chars';

    let isLog = false;
    executeClientAsyncToGroup("isLog").then((result) => {
        isLog = result;
    });

    let logType = 0;

    const onGetLogs = (index) => {
        if (index !== 0 && !isLog)
            return;

        if (logType === index)
            return;

        logType = index;

        if (index === 1) //Общий
            getLog (-1, false, "", 0)
        else if (index === 0) //Личный
            getLog ($charUUID, false, "", 0)
        else if (index === 2) //Склад
            getLog (-1, true, "", 0)
    }

    const onOpenLogs = () => {
        if (!isLog)
            return;

        setView ("Logs")
    }

    let logs = []
    const getLogs = () => {
        executeClientAsyncToGroup("getLogs").then((result) => {
            if (result && typeof result === "string")
                logs = JSON.parse(result);
        });
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.logs", getLogs)
</script>

<div class="logs">
    <h1>Логи:</h1>
    <div class="btns">
        <div class="btnbro" class:active={logType === 0} on:keypress={() => {}} on:click={() => onGetLogs (0)}>
            <p>Личный</p>
        </div>
        <div class="btnbro" class:active={logType === 1} on:keypress={() => {}} on:click={() => onGetLogs (1)}>
            <p>Общий</p>
        </div>
        <div class="btnbro" class:active={logType === 2} on:keypress={() => {}} on:click={() => onGetLogs (2)}>
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