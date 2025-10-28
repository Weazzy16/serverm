<script>
    import { translateText } from 'lang'
    import './assets/main.sass';
    import './assets/main.css';
	import { fade } from 'svelte/transition';
    import { accountRedbucks } from 'store/account'
    import { serverDateTime, serverDonatMultiplier } from 'store/server'
    import { format } from 'api/formatter'
    import { executeClient } from 'api/rage'
    import { TimeFormat } from 'api/moment'
    import MainMenu from './mainmenu.svelte';
    import Shop from './elements/shop/index.svelte';
    import Personal from './elements/personal/index.svelte';

    import Roulette from './elements/roulette/index.svelte';
    import Cases from './elements/roulette/list.svelte';

    import Premium from './elements/premium/index.svelte';

    let Views = {
        MainMenu,
        Shop,
        Personal,
        Roulette,
        Cases,
        Premium,
    }
    let SelectView = "MainMenu";

    const SetView = (view) => {
        SelectView = view;
    }

    import PopupChange from './popup/popupchange.svelte';
    import PopupPremium from './elements/premium/popup.svelte';
    import PopupPayment from './popup/popuppayment.svelte';
    import PopupDpPopup from './elements/personal/dpPopup.svelte';
    import PopupPPopup from './elements/shop/pPopup.svelte';
    import PopupCPopup from './elements/shop/cPopup.svelte';
    import PopupNomer from './popup/popupnomer.svelte';
    import PopupSim from './popup/popupsim.svelte';
    import PopupInfo from './popup/popupinfo.svelte';

    let Popups = {
        PopupChange,
        PopupPremium,
        PopupDpPopup,
        PopupPPopup,
        PopupInfo,
        PopupSim,
        PopupPayment,
        PopupCPopup,
        PopupNomer
    }

    let SelectPopup = "";
    let SelectPopupData = null;
    let AreaPopup = false;

    const SetPopup = (popup = null, data = null) => {
        if (popup === -1 && AreaPopup)
            return;
        SelectPopupData = data;
        SelectPopup = popup;
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;
        if (Popups[SelectPopup]) {
            SelectPopup = "";
            SelectPopupData = null;
        } else {
            if (SelectView === "MainMenu") {
                executeClient ("client.donate.close");
            }
            else SelectView = "MainMenu";
        }
    }

    let isLoad = false;
    const getData = () => {
        isLoad = true;
    }

    executeClient ("client.donate.load");
    import { addListernEvent } from 'api/functions';
    addListernEvent ("donate.init", getData)
</script>

<svelte:window on:keyup={HandleKeyDown} />

{#if isLoad}
    <div class="donathax">
        <div class="donathead">
            <div class="left">
                <img class="loadhead" src="https://imgur.com/39MJ9NJ.png" alt=""/>
            </div>
            <div class="right">
                <h1>Меню</h1>
                <span>Пожертвование</span>
            </div>
        </div>
        {#if SelectView !== "MainMenu"}
            <div class="doantback" on:keypress={() => {}} on:click={() => SetView("MainMenu")}>
                <p>НАЗАД</p>
            </div>
        {/if}
        <div class="donatclosed">
            <p>Выйти из меню</p>
            <div class="btncl">
                <p>ESC</p>
            </div>
        </div>
        <div class="donatblance">
            <p>{format("money", $accountRedbucks)}<b>OC</b></p>
            <div class="btnp">
                <p>пополнить</p>
            </div>
        </div>
        {#if Popups[SelectPopup]}
            <svelte:component this={Popups[SelectPopup]} {SetPopup} popupData={SelectPopupData} on:mouseenter={e => AreaPopup = true} on:mouseleave={e => AreaPopup = false} />
        {/if}

        <svelte:component this={Views[SelectView]} {SetView} {SetPopup} isPopup={Popups[SelectPopup]} />

    </div>
{/if}