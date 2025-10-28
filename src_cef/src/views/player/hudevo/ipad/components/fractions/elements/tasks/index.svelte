<script>
    import { translateText } from 'lang'
    import Missions from './missions.svelte'
    import Personal from './myList.svelte'
    import Fraction from './fractionList.svelte'
    import Logo from '../other/logo.svelte'
    import { TimeFormat } from 'api/moment'

    import { charName } from 'store/chars'
    import { serverDateTime } from 'store/server'
    import { setView, isFraction } from '../../data'

    export let onSelectedView;

    let page = {
        name: "Задания",
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
        Missions,
        Personal,
        Fraction
    }

    let selectView = "Missions";

    let isLoad = false;

    const onSelectView = (view) => {
        if (isLoad)
            return;

        selectView = view;
    }

    const onSetLoad = (toggled) => {
        isLoad = toggled;
    }

</script>
<div class="apphead">
    <div class="left">
        <span on:keypress={() => {}} on:click={() => setView ("Main")}>&lt;</span>
        <p>{page.name}</p>
        <div class="btnappfр" class:active={selectView === "Missions"} on:keypress={() => {}} on:click={() => onSelectView ("Missions")}>
            <p>{translateText('player1', 'Задания')}</p>
        </div>
        <div class="btnappfр" class:active={selectView === "Personal"} on:keypress={() => {}} on:click={() => onSelectView ("Personal")}>
            <p>{translateText('player1', 'Персональные')}</p>
        </div>
        <div class="btnappfр" class:active={selectView === "Fraction"} on:keypress={() => {}} on:click={() => onSelectView ("Fraction")}>
            <p>{translateText('player1', 'Фракционные')}</p>
        </div>
    </div>
    <div class="right">
        <p>{userData.Name}</p>
        <img src="http://cdn.magicfive.ru/haxzer/ipad/apporg.png" alt=""/>
        <span on:keypress={() => {}} on:click={() => onSelectedView("mainmenu")}>X</span>
    </div>
</div>
<svelte:component this={Views[selectView]} {onSetLoad} />