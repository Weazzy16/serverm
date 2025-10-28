<script>
    export let onSelectedViewHouse;
    import { translateText } from 'lang'

    import { format } from 'api/formatter'
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import {executeClientAsyncToGroup, executeClient, executeClientToGroup} from "api/rage";

    let houseData = {}

    const updateData = () => {
        executeClientAsyncToGroup("house.houseData").then((result) => {
            if (hasJsonStructure(result))
                houseData = JSON.parse(result);
        });
    }

    const onOpenPark = () => {
        executeClientToGroup ("house.openPark");
    }

    updateData ();
    addListernEvent ("phoneHouseUpdate", updateData);

    const furnituresData = [
        /*{
            name: "Оружейный сейф",
            desc: "Сейф, в котором можно хранить оружие. Может быть взломан другими игроками.",
            icon: "houseicon-safe",
            price: 1500
        },
        {
            name: "Шкаф с одеждой",
            desc: "Шкаф, в котором можно хранить одежду. Может быть взломан другими игроками.",
            icon: "houseicon-safe",
            price: 1500
        },
        {
            name: "Шкаф с предметами",
            desc: "Шкаф, в котором можно хранить предметы. Может быть взломан другими игроками.",
            icon: "houseicon-safe",
            price: 1500
        },
        {
            name: "Взломостойкий сейф",
            desc: "Сейф, в котором можно хранить оружие. Нельзя взломать.",
            icon: "houseicon-safe",
            price: 10000
        },*/
        {
            name: translateText('player2', 'Домашняя аптечка'),
            desc: translateText('player2', 'Пополняет количество аптечек в доме.'),
            icon: "heal",
            price: 2500,
            isOne: "Healkit"
        },
        {
            name: translateText('player2', 'Сигнализация'),
            desc: translateText('player2', 'При взломе вашего дома всем ближайшим полицейским будет отправлено уведомление.'),
            icon: "signal",
            price: 3000,
            isOne: "Alarm"
        }
    ]

    const onBuy = (furniture) => {
        if (furniture.isOne && houseData [furniture.isOne])
            return;

        executeClientToGroup ("house.fBuy", furniture.name);
    }
</script>

<div class="menuhyhomeupgrade">
    {#each furnituresData as furniture, index}
        <div class="upgradeblock">
            <div class="headupblock">
                <h1>{furniture.name}</h1>
                <span>{format("money", furniture.price)}$</span>
            </div>
            <div class="infoupblock">
                <p>Описание:</p>
                <span>{furniture.desc}</span>
            </div>
            {#if furniture.isOne && houseData [furniture.isOne]}
                <div class="btnupblock">
                    <p>{translateText('player2', 'Куплено')}</p>
                </div>
                {:else}
                <div class="btnupblock" on:keypress={() => {}} on:click={() => onBuy (furniture)}>
                    <p>Приобрести</p>
                </div>
            {/if}
        </div>
    {/each}
        <div class="upgradeblock">
            <div class="headupblock">
                <h1>Гараж</h1>
                <span>{houseData.garageType} уровень</span>
            </div>
            <div class="infoupblock">
                <p>Описание:</p>
                <span>Помещение для стоянки, а иногда и ремонта автомобилей, мотоциклов и других транспортных средств. Может быть как частью жилого дома, так и отдельным строением. Гараж обычно расчитывается на один или два автомобиля, хотя в автопарках организаций используются гаражи на большее количество автомобилей.</span>
            </div>
            <div class="btnupblock" on:keypress={() => {}} on:click={onOpenPark}>
                <p>Улучшить</p>
            </div>
        </div>
</div>