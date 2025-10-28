<script>
    import { translateText } from 'lang'
    import moment from 'moment'
    import { TimeFormat } from 'api/moment'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('membersLoad')
    import Online from '../../menu/online.svelte'
    import { charName } from 'store/chars'
    import { serverDateTime } from 'store/server'
    import { setView, isFraction, setPopup, onUpdateRank } from '../../data'
    export let onSelectedView;

    let page = {
        name: "Состав",
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

    let member = {};
    let members = [];
    let selectMember = false;

    let onlineData = {
        online: 0,
        offline: 0,
        all: 0
    };

    let ranks = []

    const getRanks = () => {
        executeClientAsyncToGroup("getRanks").then((result) => {
            if (result && typeof result === "string")
                ranks = JSON.parse(result);
        });
    }
    getRanks();

    const getMembers = (_member, _members, _onlineData) => {
        if (_member && typeof _member === "string")
            member = JSON.parse(_member);

        selectMember = member;

        if (_members && typeof _members === "string")
            members = JSON.parse(_members);

        onlineData = JSON.parse(_onlineData);
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.members", getMembers)

    const UpdateMember = (_member) => {
        _member = JSON.parse(_member);

        let index = members.findIndex((m) => m.uuid === _member.uuid);

        if (members [index]) {
            members [index] = _member;
            if (selectMember.uuid === _member.uuid)
                onSelectMember (_member)
        }
    }

    addListernEvent ("table.updateMember", UpdateMember)

    import Members from './list.svelte'
    import Member from './member.svelte'


    const onSelectMember = (item) => {
        selectMember = item;
    }

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

    let deleteUuid;
    const onPlayerDeleteCallback = () => {
        executeClientToGroup('deletePlayer', deleteUuid);
    }

    const onPlayerDelete = (item) => {
        deleteUuid = item.uuid;
        setPopup ("Confirm", {
            headerTitle: "Роспуск",
            headerIcon: "fractionsicon-members",
            text: `Ты действительно хочешь уволить '${item.name}'?`,
            button: 'Уволить',
            callback: onPlayerDeleteCallback
        })
    }

    const onPlayerAddScoreCallback = (value) => {
        if (selectMember !== member) {
            executeClientToGroup('addPlayerScore', selectMember.uuid, value);
        }
    }
    const onPlayerAddScore = () => {
        setPopup ("Input", {
            headerTitle: "Дайте баллы",
            headerIcon: "fractionsicon-members",
            input:
            {
                title: "Количество балов:",
                placeholder: "Введите номер",
                maxLength: 36,
                type: 'number'
            },
            button: 'Дать',
            callback: onPlayerAddScoreCallback
        })
    }

    const getProgress = (value, max) => {
        let perc = Math.round(value / max * 100);

        if (!perc || perc < 0)
            perc = 0;
        else if (perc > 100)
            perc = 100;

        return perc;
    }

    const getProgressBackground = (value) => {
        if (value < 35)
            return '#FB4F69';
        else if (value < 70)
            return '#fc0';

        return '#05BB63';
    }

    import Avatar from '../avatar/index.svelte'

    const setAvatar = (png) => {
        member.avatar = png;

        if (selectMember.uuid === member.uuid)
            onSelectMember (member)
    }
    import Logo from '../other/logo.svelte'

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();
</script>

<div class="apphead">
    <div class="left">
        {#if selectMember === member}
                <span on:keypress={() => {}} on:click={() => setView ("Main")}>&lt;</span>
                <p>{page.name}</p>
            {:else}
                <span on:keypress={() => {}} on:click={() => onSelectMember (member)}>&lt;</span>
                <p>{page.name}</p>
                {#if settings.setRank}
                <div class="btnappfр" on:keypress={() => {}} on:click={() => onUpdateRank ("Список рангов", "fractionsicon-members", "Выберите ранг", 'updatePlayerRank', selectMember.uuid)}>
                    <p>{translateText('player1', 'Сменить ранг')}</p>
                </div>
                {/if}
                {#if settings.isLeader || (member.departmentRank === 3)}
                <div class="btnappfр" on:keypress={() => {}} on:click={onUpdateDepartment}>
                    <p>{translateText('player1', 'Сменить отряд')}</p>
                </div>
                {/if}
                {#if settings.reprimand}
                <div class="btnappfр" on:keypress={() => {}} on:click={onReprimand}>
                    <p>{translateText('player1', 'Выговор')}</p>
                </div>
                {/if}
                {#if settings.unInvite}
                <div class="btnappfр" on:keypress={() => {}} on:click={() => onPlayerDelete (selectMember)}>
                    <p>{translateText('player1', 'Уволить')}</p>
                </div>
                {/if}
        {/if}
    </div>
    <div class="right">
        <p>{userData.Name}</p>
        <img src="http://cdn.magicfive.ru/haxzer/ipad/apporg.png" alt=""/>
        <span on:keypress={() => {}} on:click={() => onSelectedView("mainmenu")}>X</span>
    </div>
</div>
<div class="appmenufrac table">
    <div class="statuser">
        <div class="statileft">
            <p>Имя:</p>
            <span>{selectMember.name}</span>
        </div>
        <div class="stati">
            <p>Ранг:</p>
            <span>{selectMember.rankName}</span>
        </div>
        <div class="stati">
            <p>UID:</p>
            <span>#{selectMember.uuid}</span>
        </div>
        <div class="stati">
            <p>Отряд:</p>
            <span>{selectMember.departmentName}</span>
        </div>
        <div class="stati">
            <p>Сегодня:</p>
            <span>{moment.duration(selectMember.todayTime, "minutes").format("h[ч.] m[мин.]")}</span>
        </div>
        <div class="stati">
            <p>Месяц:</p>
            <span>{moment.duration(selectMember.monthTime, "minutes").format("h[ч.] m[мин.]")}</span>
        </div>
        <div class="statiright">
            <p>Дата вступления:</p>
            <span>{TimeFormat (selectMember.date, "HH:mm DD.MM.YY")}</span>
        </div>
    </div>
    {#if selectMember === member}
            <Members playerUuid={member.uuid} {onlineData} {members} {onSelectMember} {onPlayerDelete} {settings} />
        {:else}
            <Member {selectMember} {member} />
    {/if}
</div>