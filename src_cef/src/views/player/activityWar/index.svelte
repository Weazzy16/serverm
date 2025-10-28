<script>
    import { executeClient } from 'api/rage'
    import './assets/css/style.css';

    var CountFamilyActivities = 1;

    var PriceActivity = 300;

    var points = [
        {
            ID: 0,
            Name: "Завод",
            Owner: "Youmans",
            IsMy: true,
            Date: "Время",
            TimeLastCaptured: "00:40",
            PlayersInZone: 2,
        },
        {
            ID: 1,
            Name: "Завод",
            Owner: "Youmans",
            IsMy: false,
            Date: "Время",
            TimeLastCaptured: "00:40",
            PlayersInZone: 2,
        },
        {
            ID: 2,
            Name: "Завод",
            Owner: "Youmans",
            IsMy: false,
            Date: "Время",
            TimeLastCaptured: "00:40",
            PlayersInZone: 2,
        },
        {
            ID: 3,
            Name: "Завод",
            Owner: "Youmans",
            IsMy: false,
            Date: "Время",
            TimeLastCaptured: "00:40",
            PlayersInZone: 2,
        },
        {
            ID: 4,
            Name: "Завод",
            Owner: "Youmans",
            IsMy: true,
            Date: "Время",
            TimeLastCaptured: "00:40",
            PlayersInZone: 2,
        },
        {
            ID: 5,
            Name: "Завод",
            Owner: "Youmans",
            IsMy: false,
            Date: "Время",
            TimeLastCaptured: "00:40",
            PlayersInZone: 2,
        },
    ];

    window.setActivityWarData = function(count, price, data) {
        CountFamilyActivities = count;
        PriceActivity = price;
        points = data;
    };

    function keyUp(event) {
        if (event.keyCode == 27) {
            executeClient("client.activityWar.close")
        }
    }
</script>

<svelte:window on:keyup={keyUp} />

<section id="activityWar">
    <nav>
        <header>
            <p>Предприятий захвачено вами: <b>{ CountFamilyActivities }</b></p>
            <p>Денег за точки: <b>{ CountFamilyActivities * PriceActivity } $</b></p>
        </header>
        {#each points as item, index}
            <div class="aw_block">
                <h1>{ item.Name }</h1>
                <ul>
                    <h2>Захвачено:</h2>
                    <h3>{ item.Owner }</h3>
                    {#if item.IsMy == true }
                        <p>Союзников на територии: <b>{ item.PlayersInZone }</b></p>
                        <p>С последнего захвата: <b>{ item.TimeLastCaptured }</b></p>
                    {/if}
                </ul>
                <footer>{ item.Date }</footer>

                <div class="aw_back" class:my="{ item.IsMy === true }" class:no="{ item.Owner === '' }" ></div>
                <img src="{ document.cloud + 'img/activityWar/' + item.ID + '.png' }" alt="">
            </div>
        {/each}
    </nav>
</section>