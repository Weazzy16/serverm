<script>
    import { executeClient, executeClientToGroup } from 'api/rage'
    import { format } from "api/formatter";
    executeClientToGroup('missionsLoad')

    import pngMission0 from './images/0.png'
    import pngMission1 from './images/1.png'
    import pngMission2 from './images/2.png'
    import pngMission3 from './images/3.png'
    import pngMission4 from './images/4.png'
    import pngMission5 from './images/5.png'
    import pngMission6 from './images/6.png'

    let missions = [
        {
            id: 0,
            title: "Наземный патруль",
            desc: "Наземный патруль - это задача для небольшого вооруженного отряда, задача заключается в осмотре местности, поддержании порядка и государственной охране. Патрулирование осуществляется на наземном транспорте.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission0
        },
        {
            id: 1,
            title: "Воздушное патрулирование",
            desc: "Воздушное патрулирование ведет наблюдение за ранее назначенным районом с высоты птичьего полета. Следуйте по назначенному маршруту и следите за порядком внизу! Выполняется вертолетом.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission1
        },
        {
            id: 2,
            title: "Перевозка запрещенного груза",
            desc: "Перевозка запрещенного груза - это задание для мафии, в котором сотрудник должен будет перевезти запрещенный груз со склада мафии на другой конец острова. Полиция будет сидеть на хвосте, поэтому вам нужно быть начеку.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission2
        },
        {
            id: 3,
            title: "Перевозка лекарственных средств",
            desc: "Транспортировка лекарств - это задание для врачей, в котором сотрудники должны будут перевезти лекарства на свой склад. Бандиты будут сидеть на хвосте, поэтому вам нужно быть начеку.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission3
        },
        {
            id: 4,
            title: "Транспортировка материалов",
            desc: "Транспортировка материалов - это задание для армейского штаба, в котором сотрудники должны будут перевозить материалы. Бандиты будут сидеть на хвосте, поэтому вам нужно быть начеку.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission4
        },
        {
            id: 5,
            title: "AIRDROP",
            desc: "Airdrop - это отличная возможность не только заработать, но и посоревноваться с другими игроками и определить, кто достоин его забрать.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission5
        },
        {
            id: 6,
            title: "Крушение вертолета",
            desc: "Крушение вертолета — это отличная возможность посоревноваться с другими игроками по ящикам с ценным грузом, которые выпадают при крушении вертолета.",
            bonus: "",
            gps: [],
            colorId: 0,
            isTake: false,
            isShow: false,
            img: pngMission6
        },
    ]
    let selectMission = false;

    const onSelectMission = (item) => {
        selectMission = item;
    }

    const colors = [
        "green",
        "blue",
        "orange",
        "red",
    ]

    export let onSetLoad;

    onSetLoad (true);
    const updateLoad = (json) => {
        onSetLoad (false);

        json = JSON.parse(json)

        json.forEach((item) => {
            const index = missions.findIndex(m => m.id === item.id);

            if (missions [index]) {
                missions [index] = {
                    ...missions [index],
                    ...item
                }

                if (!selectMission && missions [index].isShow) {
                    selectMission = missions [index];
                }
                else if (selectMission && selectMission.id === missions [index].id) {
                    selectMission = missions [index];
                }
            }
        })
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("table.missions", updateLoad);

    const onUse = () => {
        
        executeClientToGroup('missionUse', selectMission.id)
    }

    const onGps = () => {
        executeClient('createWaypoint', selectMission.gps.x, selectMission.gps.y)
        executeClient('client.inventory.Close')
        window.notificationAdd(4, 9, `Это место было отмечено на карте`, 3000);
    }
</script>

<div class="appmenufrac task">
    {#if selectMission}
    <div class="lefttask">
        <h1>{selectMission.title}</h1>
        <img src="{selectMission.img}" alt=""/>
        <div class="info">
            <h1>Награда:</h1>
            <div class="dop">
                <p>Деньги:</p>
                <span>{format("money", selectMission.bonus)}$</span>
            </div>
        </div>
        <div class="info">
            <h1>Описание:</h1>
            <div class="dop1">
                <p>{selectMission.desc}</p>
            </div>
        </div>
        {#if selectMission.gps}
        <div class="btnappf" on:keypress={() => {}} on:click={onGps}>
            <p>Отметка в GPS</p>
        </div>
        {/if}
        {#if selectMission.isTake}
        <div class="btnappf" on:keypress={() => {}} on:click={onUse}>
            <p>Принять участие</p>
        </div>
        {/if}
    </div>
    {/if}
    <div class="righttask">
        <h1>Доступно:</h1>
        <div class="listtask">
            {#each missions as mission}
                {#if mission.isShow}
                    <div class="blocktask" on:keypress={() => {}} on:click={() => onSelectMission (mission)}>
                        <p>{mission.title}</p>
                        <img src="{mission.img}" alt="">
                    </div>
                {/if}
            {/each}
        </div>
    </div>
</div>