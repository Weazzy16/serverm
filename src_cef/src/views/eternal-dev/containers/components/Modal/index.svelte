<script>
    // @ts-ignore
    import RangeSlider from "svelte-range-slider-pips";
    import { fade, fly } from "svelte/transition";
    // --------------------------- //
    import { format } from "api/formatter";
    import { executeClient } from "api/rage";
    // --------------------------- //

    import "./style.scss";
    // @ts-ignore
    import CoinIcon from "../../pictures/coin.svg";

    export let maxBet;
    export let currentBet;
    export let nonRefDep;
    export let step;
    export let isDonate;

    export let close;

    let bet = [currentBet + step];

    const TRANSITION = { duration: 200 }

    const onClick = (paymentType) => {
        executeClient("client.containers.menu.place_bet", Number(bet[0]), paymentType);
        close();
    }
</script>

<div in:fade={TRANSITION} out:fade={TRANSITION} class="containers-modal">
    <div in:fly={{ y: 500, duration: 300 }} class="modal-container column-block">
        <div class="modal-header row-block justify-between align-center">
            <span class="modal-header__title">Сделать ставку</span>
            <button on:click={() => close()} class="close-modal-block">
                <svg xmlns="http://www.w3.org/2000/svg" width="1.5853703703703703vh" height="1.5852777777777776vh" viewBox="0 0 17.122 17.121"> <g transform="translate(1.061 1.061)" stroke="#fff"> <path d="M0,0,15,15" fill="none" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path> <path d="M6.929,0l-15,15" transform="translate(8.071)" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path> </g> </svg>
            </button>
        </div>
        <div class="modal-body column-block full-width">
            <div class="data-blocks column-block full-width">
                <div class="container-data row-block full-width justify-between">
                    <span class="container-data__title">Текущая ставка</span>
                    <span class="container-data__value">
                        { #if isDonate} <img src={CoinIcon} alt="" /> {:else} $ {/if}
                        { format("money", currentBet) }
                    </span>
                </div>
                <div class="container-data row-block full-width justify-between">
                    <span class="container-data__title">Невозвратный залог</span>
                    <span class="container-data__value red">
                        { #if isDonate} <img src={CoinIcon} alt="" /> {:else} $ {/if}
                        { format("money", nonRefDep) }
                    </span>
                </div>
            </div>
            <div class="bet-size column-block">
                <span class="bet-size__title">Размер ставки</span>
                <div class="bet-input-container row-block justify-between full-width align-center">
                    <div class="bet-block">
                        <span class="bet-block__sign">
                            {#if isDonate} <img src={CoinIcon} alt="" /> {:else} $ {/if}
                        </span>
                        <span class="bet-block__value">{ format("money", bet[0]) }</span>
                    </div>
                    <div class="bet-step row-block">
                        Шаг ставки: 
                        {#if isDonate} <img src={CoinIcon} alt="" /> {:else} $ {/if}
                        { format("money", step) }
                    </div>

                    <RangeSlider range="min" min={ currentBet + step } max={ maxBet } step={ step } bind:values={bet} />
                </div>
                <div class="pay-sum row-block justify-between">
                    <span class="pay-sum__title">Сумма к оплате</span>
                    <span class="pay-sum__value">
                        {#if isDonate} <img src={CoinIcon} alt="" /> {:else} $ {/if}
                        { format("money", bet[0] + nonRefDep) }
                    </span>
                </div>
                {#if isDonate}
                    <div class="payment-buttons row-block full-width justify-between">
                        <button on:click={() => onClick("Donate")} class="payment-button full-width row-block">
                            <img class="payment-button__image" src={CoinIcon} alt="" />
                            <span class="payment-button__title">Оплатить коинами</span>
                        </button>
                    </div>
                {:else}
                    <div class="payment-buttons row-block full-width justify-between">
                        <button on:click={() => onClick("Card")} class="payment-button full-width row-block">
                            <img class="payment-button__image" src="https://cdn.majestic-files.com/public/master/static/img/containers/card.svg" alt="" />
                            <span class="payment-button__title">Оплатить картой</span>
                        </button>
                        <button on:click={() => onClick("Wallet")} class="payment-button full-width row-block">
                            <img class="payment-button__image" src="https://cdn.majestic-files.com/public/master/static/img/containers/cash.svg" alt="" />
                            <span class="payment-button__title">Оплатить наличными</span>
                        </button>
                    </div>
                {/if}
                <div class="warning column-block">
                    <div class="warning-header row-block align-center">
                        <img class="warning-header__picture" src="https://cdn.majestic-files.com/public/master/static/img/containers/warning.svg" alt="" />
                        <span class="warning-header__title">ВНИМАНИЕ</span>
                    </div>
                    <span class="warning__text">В случае если вы не сможете сразу забрать приз, то у вас будет еще 60 минут для того чтобы забрать содержимое контейнера. Иначе оно будет автоматически продано и деньги придут вам на счет.</span>
                </div>
            </div>
        </div>
    </div>

    <button on:click={() => close()} class="background_close"></button>
</div>
