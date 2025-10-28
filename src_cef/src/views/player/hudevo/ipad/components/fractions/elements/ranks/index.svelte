<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import RanksList from './rankslist.svelte'
    import RankInfo from './rankinfo.svelte'
    import { charName } from 'store/chars'
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
    import { setView } from '../../data'
    export let onSelectedView;

    let page = {
        name: "Ранги",
    };

    let userData = {
        targetName: 0,
        changeName: 0,
        timerIdName: 0,
        Name: 0,
    };
    
    const onInit = () => {
        getSettings();
    }
    addListernEvent ("table.mainInit", onInit)

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

    let settings = null;
    let ranks = []

    const getRanks = () => {
        executeClientAsyncToGroup("getRanks").then((result) => {
            if (result && typeof result === "string") {
                ranks = JSON.parse(result);

                if (settings) {
                    const index = ranks.findIndex(r => r.id === settings.id);

                    if (ranks [index]) {
                        settings = ranks [index];
                    }
                    else
                        settings = null;
                }
            }
        });
    }
    getRanks();

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.ranks", getRanks)


    const onSettings = (item = null) => {
        settings = item;
    }

    let isLeader = false;
    executeClientAsyncToGroup("getSettings").then((result) => {
        if (result && typeof result === "string") {
            result = JSON.parse(result);
            isLeader = result.isLeader;
        }
    });

    const defaultRanksCallback = () => {
        executeClientToGroup('defaultRanks');

    }

    import { setPopup } from "../../data";
    const defaultRanks = () => {
        setPopup ("Confirm", {
            headerTitle: "Покиньте ряды",
            headerIcon: "fractionsicon-rank",
            text: `Ты действительно хочешь понизиться в звании?`,
            button: 'Да',
            callback: defaultRanksCallback
        })
    }
</script>
<div class="apphead">
    <div class="left">
        <span on:keypress={() => {}} on:click={() => setView ("Main")}>&lt;</span>
        <p>{page.name}</p>
        {#if settings}
            <div class="btnappfр" on:keypress={() => {}} on:click={() => onSettings()}>
                <p>Назад</p>
            </div>
        {/if}

    </div>
    <div class="right">
        <p>{userData.Name}</p>
        <img src="http://cdn.magicfive.ru/haxzer/ipad/apporg.png" alt=""/>
        <span on:keypress={() => {}} on:click={() => onSelectedView("mainmenu")}>X</span>
    </div>
</div>
<div class="appmenufrac table">
    {#if !settings}
        <RanksList {onSettings} {ranks} />
    {:else}
        <RankInfo {settings} {onSettings} />
    {/if}
</div>
