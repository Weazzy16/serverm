<script>
    import { translateText } from 'lang'
    import Players from './players.svelte'
    import Ranks from './ranks.svelte'
    import Squad from './squad.svelte'
    import { setView, isFraction } from '../../data'
    import { charName } from 'store/chars'
    export let onSelectedView;

    let page = {
        name: "Настройки",
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

    const Views = {
        Players,
        Ranks,
        Squad
    }
    let selectView = "Ranks"

    const onSelectView = (view) => {
        selectView = view
    }
    import Logo from '../other/logo.svelte'
</script>
<div class="apphead">
    <div class="left">
        <span on:keypress={() => {}} on:click={() => setView ("Main")}>&lt;</span>
        <p>{page.name}</p>
        <div class="btnappfр" class:active={selectView === "Ranks"} on:keypress={() => {}} on:click={() => onSelectView ("Ranks")}>
            <p>{translateText('player1', 'Ранга')}</p>
        </div>
        <div class="btnappfр" class:active={selectView === "Squad"} on:keypress={() => {}} on:click={() => onSelectView ("Squad")}>
            <p>{translateText('player1', 'Отряда')}</p>
        </div>
        <div class="btnappfр" class:active={selectView === "Players"} on:keypress={() => {}} on:click={() => onSelectView ("Players")}>
            <p>{translateText('player1', 'Персональный')}</p>
        </div>
    </div>
    <div class="right">
        <p>{userData.Name}</p>
        <img src="http://cdn.magicfive.ru/haxzer/ipad/apporg.png" alt=""/>
        <span on:keypress={() => {}} on:click={() => onSelectedView("mainmenu")}>X</span>
    </div>
</div>
<svelte:component this={Views[selectView]}  />