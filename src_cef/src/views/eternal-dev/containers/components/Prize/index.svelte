<script>
    // --------------------------- //
    import { onDestroy } from "svelte";
    import { fade, fly } from "svelte/transition";
    // --------------------------- //
    import { format } from "api/formatter";
    import { executeClient } from "api/rage";
    // --------------------------- //
    import { secondsToTime } from "../../modules/timer";
    import { getPrizePicture } from "../../modules/prize";
    // --------------------------- //
    import "./style.scss";

    export let prizeData;
    export let close;

    const TRANSITION = { duration: 200 };

    const onClick = (type) => {
        executeClient("client.containers.menu.prize_action", type);
    }

    let interval = null,
        time = "0 мин. 0 сек.";

    $: time = secondsToTime(prizeData == null ? 0 : prizeData.Time);

    const destroyInterval = () => {
        if (interval != null)
            clearInterval(interval);

        interval = null;
    }

    const createInterval = () => {
        destroyInterval();

        interval = setInterval(() => {
            if (prizeData != null && prizeData.Time > 0)
                prizeData.Time--;
        }, 1000);
    }

    $: {
        createInterval();
    }

    onDestroy(() => {
        destroyInterval();
    });
</script>

<div in:fade={TRANSITION} out:fade={TRANSITION} class="containers-modal">
    <div in:fly={{ y: 500, duration: 300 }} class="modal-container column-block">
        <div class="modal-header row-block justify-between align-center">
            <span class="modal-header__title">Содержимое контейнера</span>
            <button on:click={() => close()} class="close-modal-block">
                <svg xmlns="http://www.w3.org/2000/svg" width="1.5853703703703703vh" height="1.5852777777777776vh" viewBox="0 0 17.122 17.121"> <g transform="translate(1.061 1.061)" stroke="#fff"> <path d="M0,0,15,15" fill="none" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path> <path d="M6.929,0l-15,15" transform="translate(8.071)" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path> </g> </svg>
            </button>
        </div>
        <div class="modal-body column-block full-width">
            <div class="container-content column-block full-width">
                <div class="color pink"></div>
                <canvas class="confetti full-width full-height" width="410" height="199"></canvas>
                <img src={ getPrizePicture(prizeData.Model) } class="container-content__picture" alt="" />
                <span class="container-content__title">{ prizeData.Name }</span>
            </div>
            <div class="take-decision row-block full-width align-center justify-between">
                <span class="take-decision__title">Время для принятия решения</span>
                <div class="time-left align-center justify-center">
                    <img src="https://cdn.majestic-files.com/public/master/static/img/containers/timer.svg" alt="" class="time-left__picture" />
                    <span class="time-left__title">{ time }</span>
                </div>
            </div>
            <div class="control-buttons row-block full-width">
                <button on:click={() => onClick('take')} class="control-button take align-center full-width">
                    <img class="control-button__picture" src="https://cdn.majestic-files.com/public/master/static/img/containers/car.svg" alt="" />
                    <span class="control-button__title">Забрать себе</span>
                </button>
                <button on:click={() => onClick("sell")} class="control-button sell align-center full-width">
                    <span class="control-button__title">Продать 
                        <span class="highlighted">{ format("money", prizeData.Price) }</span>
                    </span>
                </button>
            </div>
        </div>
    </div>

    <button on:click={() => close()} class="background_close"></button>
</div>
