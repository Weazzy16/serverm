<script>
    import { translateText } from 'lang'
    import SquadsList from './squadslist.svelte'
    import SquadSettings, {id} from './squadsettings.svelte'
    import SquadRanks from './squadranks.svelte'
    import SquadInfo from './squadinfo.svelte'
    import { charName } from 'store/chars'
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
    import { setView, isFraction } from '../../data'
    export let onSelectedView;

    let page = {
        name: "Отряды",
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


    import { setGroup, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('departmentsLoad')

    let isLoad = false;
    let departments = []

    const getDepartments = (_departments) => {
        departments = JSON.parse(_departments);

        if (selectDepartment) {
            const index = departments.findIndex(r => r.id === selectDepartment.id);

            if (departments [index]) {
                selectDepartment = departments [index];
            }
            else
                selectDepartment = null;
        }

        isLoad = true
    }
    import { addListernEvent } from "api/functions";
    addListernEvent ("table.departments", getDepartments)


    //
    let department = {}
    let members = [];

    let onlineData = {
        online: 0,
        offline: 0,
        all: 0
    };

    const getDepartment = (_department, _members, _onlineData) => {
        department = JSON.parse(_department);
        if (settings)
            onSettings (department)
        members = JSON.parse(_members);
        onlineData = JSON.parse(_onlineData);

        isLoad = true
    }
    addListernEvent ("table.department", getDepartment)

    //

    let selectDepartment = null;

    const onSelectDepartment = (item = null) => {
        if (item) {
            isLoad = false;
            executeClientToGroup('departmentLoad', item.id)
        }
        selectDepartment = item;
    }

    import { setPopup } from "../../data";

    let deleteId = 0;
    const onDepartmentDeleteCallback = () => {
        executeClientToGroup('removeDepartment', deleteId);
    }

    const onDepartmentDelete = (item) => {
        deleteId = item.id;
        setPopup ("Confirm", {
            headerTitle: "Распустить отряд",
            headerIcon: "fractionsicon-squads",
            text: `Вы действительно хотите распустить отряд '${item.id}. ${item.name}'?`,
            button: 'Распустить',
            callback: onDepartmentDeleteCallback
        })
    }

    const onUpdateCallback = (name, tag) => {
        let check = format("rank", name);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        check = format("tag", tag);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('updateDepartment', department.id, name, tag)
    }

    const onUpdate = () => {
        setPopup ("Input", {
            headerTitle: "Измените данные об отряде",
            headerIcon: "fractionsicon-squads",
            inputs: [
                {
                    title: "Название отряда",
                    placeholder: "Название отряда",
                    maxLength: 36,
                    value: department.name
                },
                {
                    title: "Метка отряда",
                    placeholder: "Метка отряда",
                    maxLength: 5,
                    value: department.tag
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onUpdateCallback
        })
    }

    const onSquadRanksCallback = (json) => {
        executeClientToGroup('setLeadersDepartment', department.id, json)
    }

    const onSquadRanks = () => {
        setPopup ("SquadRanks", {
            id: department.id,
            ranks: department.ranks,
            myRank: department.myRank,
            callback: onSquadRanksCallback
        })
    }

    //

    let settings = null;
    const onSettings = (item = null) => {
        settings = item;
    }

    let settings1 = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings1 = JSON.parse(result);
        });
    }
    getSettings();

    //

    import Logo from '../other/logo.svelte'

</script>
<div class="apphead">
    <div class="left">
        {#if selectDepartment !== null && settings === null}
        <span on:keypress={() => {}} on:click={() => onSelectDepartment()}>&lt;</span>
        <p>{page.name}</p>
            {#if settings1.isLeader}
            <div class="btnappfр" on:keypress={() => {}} on:click={() => onSettings (department)}>
                <p>{translateText('player1', 'Настройки')}</p>
            </div>
            {/if}
            {#if settings1.deleteDepartment}
            <div class="btnappfр" on:keypress={() => {}} on:click={() => onDepartmentDelete(department)}>
                <p>{translateText('player1', 'Удалить отряд')}</p>
            </div>
            {/if}
            {#if department.isSettings || settings1.isLeader || department.myRank === 3}
                {#if department.isSettings}
                    <div class="btnappfр" on:keypress={() => {}} on:click={onUpdate}>
                        <p>{translateText('player1', 'Изменить данные отряда')}</p>
                    </div>
                {/if}
                {#if settings1.isLeader || department.myRank === 3}
                    <div class="btnappfр" on:keypress={() => {}} on:click={onSquadRanks}>
                        <p>{translateText('player1', 'Назначить руководство')}</p>
                    </div>
                {/if}
            {/if}
        {:else if selectDepartment !== null && settings !== null}
        <span on:keypress={() => {}} on:click={() => setView ("Main")}>&lt;</span>
        <p>{page.name}</p>
        {:else}
        <span on:keypress={() => {}} on:click={() => setView ("Main")}>&lt;</span>
        <p>{page.name}</p>
        {/if}
    </div>
    <div class="right">
        <p>{userData.Name}</p>
        <img src="http://cdn.magicfive.ru/haxzer/ipad/apporg.png" alt=""/>
        <span on:keypress={() => {}} on:click={() => onSelectedView("mainmenu")}>X</span>
    </div>
</div>
    {#if isLoad}
        {#if selectDepartment === null}
        <div class="appmenufrac table">
            <SquadsList {departments} {onSelectDepartment} {onDepartmentDelete} />
        </div>
        {:else if selectDepartment !== null && settings === null}
        <div class="appmenufrac squad">
            <SquadSettings {onSelectDepartment} {onDepartmentDelete} {department} {members} {onlineData} {onSettings} />
        </div>
        {:else if selectDepartment !== null && settings !== null}
            <SquadRanks {settings} {onSettings} />
        {/if}
    {/if}