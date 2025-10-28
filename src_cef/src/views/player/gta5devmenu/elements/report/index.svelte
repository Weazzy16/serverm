<script>
    import { executeClient } from 'api/rage'
    import { selected, reportsData, text } from './index.js'
    import { onDestroy, onMount  } from 'svelte'
        import { charName } from 'store/chars'

    
    let
        reports = [],
        answer = $text;

    onDestroy(() => {
        text.set(answer) 
    });

    onMount(() => {
        window.reportsStore.clear();
        executeClient('loadreports');
    });

    reportsData.subscribe(value => {
		reports = value;
    });

    let antiSpam = new Date().getTime();
    const onSelectReport = (report, index) => {
        if (report.blocked && report.blockedBy !== $charName) return;
        else if ($selected) return;
        else if (new Date().getTime() - antiSpam < 500) return;
        antiSpam = new Date().getTime();

        selected.set (report);
        //if (index != undefined && reports[index]) {
        //    reports[index].blocked = true;
        //    reports[index].blockedBy = $charName;
        //}
        if (!report.closed)
            executeClient ("takereport", report.id);
    }
const closeTicket = (report) => {
        if (!report) return;
        executeClient ("closereport", report.id);
        selected.set(false);
        answer = "";
    }

    const otherAdmin = (report) => {
        if (!report) return;
        executeClient ("otheradminreport", report.id);
        selected.set(false);
        answer = "";
    }
    const onSendAnswer = () => {
        if (!answer) return;
        const report = $selected;
        if (report && !report.closed) {
            if (window.global && window.global.isAdmin)
                executeClient("sendreport", report.id, answer);
            else
                executeClient("reportmsg", answer);
        } else {
            executeClient("sendReportFromClient", answer);
        }
        answer = "";
    }

    const onReturnReport = (report) => {
        if (!report) return;

        //const index = reports.findIndex(r => r.id == report.id);
        //if (index != undefined && reports[index]) {
        //    reports[index].blocked = false;
        //    reports[index].blockedBy = "";
        //}
        executeClient ("takereport", report.id);
        selected.set(false);
        answer = "";
    }

    const func = (funcName) => {
        const report = $selected;
        if (!report) return;
        executeClient ("funcreport", report.id, funcName);
    }

    const onKeyBan = (e) => {
        if (event.key === 'Enter') {
            event.preventDefault();
        }
    }

    const sendsms = (event) => {
        const { keyCode } = event;
        if (keyCode !== 13) return;

        setTimeout(() => {
            onSendAnswer();
        }, 50);

    }
</script>

<svelte:window on:keyup={sendsms} />

<div class="reportmenu">
    <div class="reportleft">
        <div class="reporthead">ИСТОРИЯ ОБРАЩЕНИЙ</div>
        <div class="reportlist">
            {#each reports as report, index}
                <div class="reportblock" on:keypress={() => {}} on:click={e => onSelectReport(report, index)}>
                    <div class="headblock">
                        <p>Обращение:<b>#{report.id}</b></p>
                        {#if report.blocked}
                        <span>{report.blockedBy}</span>
                        {/if}
                        <div class="timedata">{report.author}</div>
                        {#if report.closed}
                        <span class="closed">(закрыто)</span>
                        {/if}
                    </div>
                    <div class="textreport">
                        <p>{report.text}</p>
                    </div>
                </div>
            {/each}
        </div>
    </div>
    <div class="reportright">
        <div class="reporthead">ОБРАЩЕНИЕ: {#if $selected}#{$selected.id}{:else}не выбрано{/if}</div>
        <div class="bgreportmessage">
            {#if $selected}
            {#each $selected.messages as msg}
                <div class="messageuser">
                    <div class="messleft">
                        <div class="headmess">
                            <p>#{$selected.id}<b>{msg.author}</b></p>
                        </div>
                        <div class="messus">
                           <p>{msg.text}</p>
                        </div>
                    </div>
                    <div class="messright">
                        <img src="https://imgur.com/yGsjlYC.png" alt=""/>
                    </div>
                </div>
                {/each}
                {#if !$selected.closed}
                <div class="btns">
                    <div class="btnreport" on:click={e => closeTicket($selected)}>
                        <p>Моя проблема решена</p>
                    </div>
                    <div class="btnreport" on:click={e => selected.set(false)}>
                        <p>Проблема актуальна</p>
                    </div>
                    <div class="btnreport" on:click={e => otherAdmin($selected)}>
                        <p>Помощь другого администратора</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => onReturnReport($selected)}>
                        <p>Закрыть обращение</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("tp")}>
                        <p>Телепортироваться</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("sp")}>
                        <p>Следить</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("metp")}>
                        <p>Телепортировать к себе</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("stats")}>
                        <p>Статистика</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("kill")}>
                        <p>Убить</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("ptime")}>
                        <p>Личное время</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("checkdim")}>
                        <p>Проверить мир</p>
                    </div>
                    <div class="btnreport" on:keypress={() => {}} on:click={e => func("nhistory")}>
                        <p>История</p>
                    </div>
                </div>
                {/if}
            {/if}
        </div>
        {#if !$selected || !$selected.closed}
        <div class="inputbg">
            <input onKeyPress={e => onKeyBan(e)} ref="answerBox" bind:value={answer} placeholder="Напишите сообщение...">
        </div>
        {/if}
    </div>
</div>