<script>
    // --------------------------- //
    import { onDestroy } from "svelte";
    import { fly } from "svelte/transition";
    // --------------------------- //
    import { charUUID } from "store/chars";
    // --------------------------- //
    import { format } from "api/formatter";
    import { executeClient } from "api/rage";
    // --------------------------- //
    import { secondsToTime } from "./modules/timer";
    import { getPrizePicture } from "./modules/prize";
    // --------------------------- //
    import Modal from "./components/Modal/index.svelte";
    import Prize from "./components/Prize/index.svelte";
    // --------------------------- //
    import "./style.scss";

    // @ts-ignore
    import CoinIcon from "./pictures/coin.svg";

    export let viewData;

    let page = "main",
        containerData = {
            Id: 1,
            Type: "Low",
            Name: "Низкий контейнер",
            Cost: 1000000,
            NonRefDep: 10000,
            Step: 10000,
            MaxBet: 10000000,
            IsDonate: !false,

            Prizes: [
                { Model: "comet2", Name: "Pfister Comet II", IsDonate: false, Price: 10000, Rarity: "red" },
                { Model: "comet2", Name: "Pfister Comet II", IsDonate: false, Price: 10000, Rarity: "unique" },
            ]
        },

        waitSeconds = 150,
        currentBet = 1500000,

        bets = [
            { Login: "merumond", Uuid: 2, Name: "Eternal_Dev", Price: 15000, UpTime: 0 },
            { Login: "merumond", Uuid: 1, Name: "Ilya_Merumond", Price: 15000, UpTime: 30 },
        ],

        interval = null,
        winPrize = null;
        // winPrize = { Time: 3000, Model: "comet2", Name: "Pfhister Comet II", IsDonate: false, Price: 10000 };

    let time = "0 мин. 0 сек.";
    $: time = secondsToTime(waitSeconds);

    const setPage = (pageName) => {
        page = pageName;
    }

    $: if (viewData && typeof viewData === "string") {
        viewData = JSON.parse (viewData)

        containerData = viewData.containerData;
        currentBet = viewData.currentBet;
        waitSeconds = viewData.waitSeconds;
        bets = viewData.bets;
        winPrize = viewData.winPrize;

        if (winPrize != null)
            setPage("prize");
    }

    const destroyInterval = () => {
        if (interval != null)
            clearInterval(interval);

        interval = null;
    }

    const createInterval = () => {
        destroyInterval();

        interval = setInterval(() => {
            if (waitSeconds > 0)
                waitSeconds--;
        }, 1000);
    }

    $: {
        createInterval();
    }

    onDestroy(() => {
        destroyInterval();
    });

    // @ts-ignore
    window.containers = {
        update(type, value) {
            switch(type) {
                case "waitSeconds":
                    return waitSeconds = value;
                case "currentBet":
                    return currentBet = value;
                case "bets":
                    return bets = value;
            }
        }
    }

    const close = () => {
        executeClient("client.containers.menu.try_close");
    }

    const TRANSITION = { y: 500, duration: 300 }
</script>

{#if page == "bet" }
    <Modal currentBet={currentBet} nonRefDep={containerData.NonRefDep} maxBet={containerData.MaxBet} isDonate={containerData.IsDonate} step={containerData.Step}
        close={() => setPage("main")} />
{/if}

{#if page == "prize" }
    <Prize prizeData={winPrize}
        close={() => setPage("main")} />
{/if}

<div in:fly={TRANSITION} out:fly={TRANSITION} class="containers">
    <div class="container-info column-block">
        <div class="container-image">
            <img src="https://cdn.majestic-files.com/public/master/static/img/containers/containers/1.png" alt="" class="container-image__picture" />
        </div>
        <div class="container-metadata column-block full-width">
            <div class="title-data row-block full-width justify-between align-center">
                <span class="title-data__title">{containerData.Name} #{containerData.Id}</span>
                <div class="title-data-info row-block align-center">
                    <img src="https://cdn.majestic-files.com/public/master/static/img/containers/timer.svg" alt="" class="title-data-info__image"/>
                    <span class="title-data-info__title">{ time }</span>
                </div>
            </div>

            <div class="bet-fields row-block full-width justify-between">
                <div class="row-block">
                    <span class="bet-fields__title">Шаг ставки:</span>
                    <span class="bet-fields__value">
                        { #if containerData.IsDonate}<img src={CoinIcon} alt="" />{:else}${/if}
                        { format("money", containerData.Step) }
                    </span>
                </div>
                <div class="row-block">
                    <span class="bet-fields__title">Начальная цена:</span>
                    <span class="bet-fields__value">
                        { #if containerData.IsDonate}<img src={CoinIcon} alt="" />{:else}${/if}
                        { format("money", containerData.Cost) }
                    </span>
                </div>
            </div>
        </div>
        <div class="container-content">
            <span class="container-content__title">Возможное содержимое</span>
            <div class="prizes-list row-block">
                {#each containerData.Prizes as item, index}
                    <div class={ `prize-container column-block ${item.Rarity}` }>
                        <div class="color-indicator"></div>
                        <img class="prize-container__picture" src={ getPrizePicture(item.Model) } alt="" />
                        <span class="prize-container__title">{ item.Name }</span>
                    </div>
                {/each}
            </div>
        </div>
    </div>

    <div class="trades column-block full-width full-height">
        <div class="trades-header full-width">
            <div class="header-title row-block full-width justify-between">
                <div class="row-block align-center">
                    <span class="header-title__title">Ход торгов</span>
                </div>
                <button on:click={() => close()} class="close-block row-block align-center">
                    <span class="close-block__title">Закрыть</span>
                    <div class="close-block__key">ESC</div>
                </button>
            </div>
            <div class="bet-data row-block full-width justify-between">
                <span class="bet-data__title">Текущая ставка</span>
                <span class="bet-data__value">
                    { #if containerData.IsDonate}<img src={CoinIcon} alt="" />{:else}${/if}
                    { format("money", currentBet) }
                </span>
            </div>
        </div>
        <div class="trades-body full-width column-block">
            {#each bets as item, index}
                <div class={ `chat-element row-block align-center bet-cell ${item.Uuid == $charUUID ? 'right-side' : ''}` }>
                    <div class="chat-cell column-block">
                        <div class="cell-header row-block justify-between full-width align-center">
                            <div class="row-block">
                                <span class="cell-header__title-name">{ item.Uuid == $charUUID ? "Вы" : item.Name.replace("_", " ") }</span>
                                <span class="cell-header__title-id">#{ item.Uuid }</span>
                            </div>
                            <!-- <span class="cell-header__title-date">09:15:23</span> -->
                        </div>

                        <div class="cell-body row-block">
                            Повысил (-а) ставку на 
                            <span class="highlighted">
                                { #if containerData.IsDonate}<img src={CoinIcon} alt="" />{:else}${/if}
                                { format("money", item.Price) }
                            </span>
                        </div>
                    </div>
                </div>

                {#if item.UpTime > 0} 
                    <div class="chat-element row-block align-center">
                        <div class="line full-width"></div>
                        <div class="time-container row-block align-center">
                            <img src="https://cdn.majestic-files.com/public/master/static/img/containers/clock-gray.svg" alt="" class="time-container__icon" />
                            <span class="time-container__title">
                                Аукцион продлён на 
                                <span class="highlighted">{ secondsToTime(item.UpTime) }</span>
                            </span>
                        </div>
                        <div class="line full-width"></div>
                    </div>
                {/if}
            {/each}
        </div>
        <div class="trades-footer full-width row-block">
            <button on:click={() => setPage("bet")} class="footer-button place-bet justify-center row-block align-center full-width full-height">
                <img src={ containerData.IsDonate ? CoinIcon : `https://cdn.majestic-files.com/public/master/static/img/containers/bet.svg` } alt="" class="footer-button__picture" />
                <span class="footer-button__title">Сделать ставку</span>
            </button>
            <button on:click={() => close()} class="footer-button leave row-block justify-center align-center full-height">
                <img class="footer-button__picture" src="https://cdn.majestic-files.com/public/master/static/img/containers/exit.svg" alt="" />
                <span class="footer-button__title">Выйти</span>
            </button>
        </div>
    </div>
</div>
