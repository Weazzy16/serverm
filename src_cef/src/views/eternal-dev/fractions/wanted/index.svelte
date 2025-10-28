<script>
    import { fade, fly } from "svelte/transition";
    import { executeClient } from "api/rage";
    
    import "./style.scss";
    
    export let popupData;

    let starsValue = 0,
        reason = "";

    function doWanted() {
        executeClient("e-dev.wantedMenu.submit", Number(starsValue), reason);
    }

    function cancel() {
        executeClient("e-dev.wantedMenu.cancel");
    }
</script>

<div transition:fade={{ duration: 250 }} class="wanted-wrapper">
    <div transition:fly={{ y: 500, duration: 300, delay: 200 }} class="wanted-container">
        <div class="wanted-container__title">Объявить в розыск</div>
        <div class="wanted setwanted custom">
            <pre class="wanted-container__message">Укажите кол-во звезд (0-5)</pre>
            <input
                bind:value={starsValue}
                placeholder="Введите значение"
                class="wanted__input"
                type="number"
                max="5"
            />

            <pre class="wanted-container__message">Укажите статью</pre>
            <input
                bind:value={reason}
                placeholder="Введите значение"
                class="wanted__input"
                type="text"
            />
            
            <div class="buttons-container">
                <button on:click={() => doWanted()} class="buttons-container__button-accept">Объявить</button>
                <button on:click={() => cancel()} class="buttons-container__button-cancel">Отменить</button>
            </div>
        </div>
        <button on:click={() => cancel()} class="close-block">
            <svg xmlns="http://www.w3.org/2000/svg" width="17.122" height="17.121" viewBox="0 0 17.122 17.121">
                <g transform="translate(1.061 1.061)" stroke="#fff">
                  <path d="M0,0,15,15" fill="none" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path>
                  <path d="M6.929,0l-15,15" transform="translate(8.071)" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path>
                </g>
            </svg>
        </button>
    </div>
</div>
