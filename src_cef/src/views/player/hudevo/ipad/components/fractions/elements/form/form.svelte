<script>
    import { translateText } from 'lang'
    import { executeClient, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('clothesLoad')
    import { setView, isFraction } from '../../data'
    import { charName } from 'store/chars'
    export let onSelectedView;

    let page = {
        name: "Форма",
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


    let clothes = []
    const getClothes = (_clothes) => {
        clothes = JSON.parse(_clothes);
    }

    import { addListernEvent } from "api/functions";

    addListernEvent ("table.clothes", getClothes)

    import { setPopup, onUpdateRank } from "../../data";


    let selectForm = {}
    let newFormName = ""

    const updateRank = (rank) => {
        executeClientToGroup("clothesUpdate", selectForm.name, newFormName, rank, selectForm.gender);
        executeClient ('client.ipad.close');
    }

    const onEditFormCallback = (value) => {
        newFormName = value
        onUpdateRank("List of ranks", "fractionsicon-form", "Select the rank from which the form will be available.", null, null, updateRank);
    }

    const onEditForm = (item) => {
        selectForm = item
        setPopup ("Input", {
            headerTitle: "1",
            headerIcon: "fractionsicon-form",
            input: {
                title: "Название формы",
                placeholder: "Введите название формы",
                maxLength: 36,
                value: selectForm.name
            },
            button: translateText('popups', 'Confirm'),
            callback: onEditFormCallback
        })
    }
    import Logo from '../other/logo.svelte'
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
<div class="appmenufrac clothes">
    <div class="clotheslist">
        {#each clothes as item, index}
            <div class="blockclo">
                <div class="leftclo">
                    <div class="name">
                        <p>{item.name} ({item.gender ? "М" : "Ж"})</p>
                        <span>{item.rankName}</span>
                    </div>
                    <div class="setting" on:keypress={() => {}} on:click={() => onEditForm(item)}>Настройка доступа</div>
                </div>
                <div class="rightclo">
                    <img src="http://cdn.magicfive.ru/haxzer/ipad/form.png" alt="">
                </div>
            </div>
        {/each}
    </div>
</div>