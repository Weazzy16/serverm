<script>
    import { translateText } from 'lang'
    import { setGroup, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import Online from '../../menu/online.svelte'
    executeClientToGroup('mainLoad')
    import { charName } from 'store/chars'
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
    import { setView, isFraction } from '../../data'
    export let onSelectedView;

    let page = {
        name: "Информация",
    };

    let userData = {
        targetName: 0,
        changeName: 0,
        timerIdName: 0,
        Name: 0,
    };

    import { onMount } from 'svelte';
    onMount(async () => {
        //bar.animate(1.0);

        charName.subscribe(value => {
            if (userData.Name !== value) {
                CounterUpdate ("Name", value);
            }
        });
    });

    const CounterUpdate = (args, value) => {
        if (userData["timerId" + args])
            clearTimeout (userData["timerId" + args]);
        userData["change" + args] = userData[args] > value ? (0 - (userData[args] - value)) : (value - userData[args]);
        userData[args] = value;
        userData["timerId" + args] = setTimeout (() => {
            userData["timerId" + args] = 0;
            userData["change" + args] = 0;
            if (!userData["target" + args]) {
                userData["target" + args] = new CountUp("target" + args, value);
                //userData["target" + args].start();
                //userData["target" + args].update(value);
            }
            else
                userData["target" + args].update(value);
        }, !userData["target" + args] ? 0 : 5000)
    }



    import Leader from './elements/leaders.svelte'
    import Stats from './elements/stats.svelte'
    import Settings from './elements/settings.svelte'
    import News from './elements/news.svelte'
    import Log from './elements/log.svelte'
    import Stock from './elements/stock.svelte'


    import { addListernEvent } from "api/functions";
    import {setPopup, isOrganization} from "../../data";
    import Upgrade from "../../popups/upgrade/index.svelte";
    let isLoad = false;
    const onInit = () => {
        getOrgUpgrate();
        getSettings();
        isLoad = true;
    }
    addListernEvent ("table.mainInit", onInit)

    //

    let isOrgUpgrate = false;

    const getOrgUpgrate = () => {
        executeClientAsyncToGroup("isOrgUpgrate").then((result) => {
            isOrgUpgrate = result;
        });
    }
    getOrgUpgrate();

    addListernEvent ("table.isOrgUpgrate", getOrgUpgrate)

    const onUpgrade = () => {
        setPopup ("Upgrade");
    }

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();

    const onInvate = (value) => {
        executeClientToGroup('invitePlayer', value);
    }

    const onOpenPopupInvate = () => {
        setPopup ("Input", {
            headerTitle: "Приглашение",
            headerIcon: "fractionsicon-members",
            input: {
                title: "ID или имя фамилия",
                placeholder: "Введите данные игрока",
                maxLength: 36,
            },
            button: translateText('popups', 'Подтвердить'),
            callback: onInvate
        })
    }

    let tableInfo = {};

    const onTableInfo = (json) => {
        tableInfo = JSON.parse(json);
        getSettings();
    }
    addListernEvent ("table.tableInfo", onTableInfo)
    
</script>

{#if isLoad}
<div class="apphead">
    <div class="left">
        <span on:keypress={() => {}} on:click={() => setView ("Main")}>&lt;</span>
        <p>{page.name}</p>
    </div>
    <div class="right">
        <p>{userData.Name}</p>
        <img src="http://cdn.magicfive.ru/haxzer/ipad/apporg.png" alt=""/>
        <span on:keypress={() => {}} on:click={() => onSelectedView("mainmenu")}>X</span>
    </div>
</div>
<div class="appmenufrac">
    <div class="left">
        {#if isOrganization()}
            <img src="http://cdn.magicfive.ru/haxzer/ipad/apporg.png" alt="">
            {:else}
            <img src="http://cdn.magicfive.ru/haxzer/ipad/{tableInfo.name}.png" alt="">
        {/if}
        <h1>{tableInfo.name}</h1>
        <p>Discord: <b>{settings.discord}</b></p>
        {#if isOrgUpgrate}
            <div class="btns">
                <div class="btnappf" on:keypress={() => {}} on:click={onUpgrade}>
                    <p>Улучшения</p>
                </div>
                <div class="btnappf" on:keypress={() => {}} on:click={onOpenPopupInvate}>
                    <p>Пригласить</p>
                </div>
            </div>
            {:else}
            <div class="btnappf" on:keypress={() => {}} on:click={onOpenPopupInvate}>
                <p>Пригласить</p>
            </div>
        {/if}
        <Settings {settings} />
        <Stats {settings} />
    </div>
    <div class="right">
        <News {settings} />
        <Log />
    </div>
</div>
{/if}