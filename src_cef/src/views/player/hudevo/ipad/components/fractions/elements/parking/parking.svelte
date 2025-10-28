<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    executeClientToGroup('vehiclesLoad')
    import { setView, isFraction } from '../../data'
    import { charName } from 'store/chars'

    let page = {
        name: "Парковка",
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

    export let onSelectedView;

    let isVehicleUpdateRank = false;
    let isSellVehicle = false;
    let vehicles = [];

    const getVehicles = (_isVehicleUpdateRank, _vehicles, _isSellVehicle) => {
        isVehicleUpdateRank = _isVehicleUpdateRank;
        isSellVehicle = _isSellVehicle;

        if (_vehicles && typeof _vehicles === "string")
            vehicles = JSON.parse(_vehicles);

    }

    import { addListernEvent } from "api/functions";
    import { onUpdateRank } from "../../data";

    addListernEvent ("table.vehicles", getVehicles)


    const onEvacuation = (number) => {
        executeClientToGroup('evacuation', number);
    }

    const onGps = (number) => {
        executeClientToGroup('gps', number);
    }
    import Logo from '../other/logo.svelte'

    const onSellCar = (number) => {
        executeClientToGroup('sellCar', number);
    }

</script>

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
<div class="appmenufrac cars">
    <div class="carslist">
        {#each vehicles as item, index}
           <div class="carsblock" style="background: url(http://cdn.magicfive.ru/cloud/inventoryItems/vehicle/{item.model.toLowerCase()}.png);">
            <div class="topcar">
                {#if isVehicleUpdateRank}
                <div class="leftcar"  on:keypress={() => {}} on:click={() => onUpdateRank("Список рангов", "fractionsicon-parking", "Select the rank from which this transport will be available.", 'updateVehicleRank', item.number)}>Настройка доступа</div>
                {/if}
                <div class="rightcar">
                    <div class="namecar">{item.model}</div>
                    <div class="numbercar">{item.number}</div>
                </div>
            </div>
            <div class="bottomcar">
                <div class="leftcar">{item.rankName}</div>
                <div class="rightcar">
                    {#if isSellVehicle}
                    <div class="tran" on:keypress={() => {}} on:click={() => onSellCar(item.number)}>Продать</div>
                    {/if}
                    {#if item.isEvacuation}
                    <div class="gps" on:keypress={() => {}} on:click={() => onEvacuation(item.number)}>Эвакуировать</div>
                    {/if}
                    {#if item.isGps}
                    <div class="gps" on:keypress={() => {}} on:click={() => onGps(item.number)}>Отметить на GPS</div>
                    {/if}
                </div>
            </div>
        </div>
        {/each}
    </div>
</div>