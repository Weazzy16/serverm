<script>
    import { translateText } from 'lang'
    import List from './list.svelte'
    import moment from 'moment';

    export let department;
    export let members;
    export let onlineData;


    import { executeClientToGroup } from 'api/rage'


    import { setPopup } from "../../data";
    import { format } from 'api/formatter'

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

    import { executeClientAsyncToGroup } from "api/rage";

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();
</script>
<div class="squadinfo">
    <div class="leftinfo">
        <p>Гланвый:</p>
        <span>{department.chief}</span>
    </div>
    <div class="blinfo">
        <p>Отряд:</p>
        <span>{department.tag}</span>
    </div>
    <div class="blinfo">
        <p>Участников:</p>
        <span>{department.playerCount} чел.</span>
    </div>
    <div class="blinfo">
        <p>Зам. Отряда 1</p>
        <span>{department.zam1}</span>
    </div>
    <div class="blinfo">
        <p>Зам. Отряда 2</p>
        <span>{department.zam2}</span>
    </div>
    <div class="blinfo">
        <p>Тег</p>
        <span>#{department.tag}</span>
    </div>
    <div class="rightinfo">
        <p>Дата создания:</p>
        <span>{moment(department.date).format('DD.MM.YYYY')}</span>
    </div>
</div>

<List departmentId={department.id} {members} {onlineData} ranks={department.ranks} />