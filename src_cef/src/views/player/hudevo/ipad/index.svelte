<script>
    import { fly } from 'svelte/transition';
    import { onDestroy, onMount } from 'svelte';
    import { setGroup, executeClientAsyncToGroup, executeClient, executeClientToGroup } from 'api/rage';
    import { currentPage, selectedImage, selectedImageFunc, selectNumber } from './stores';
    import { addListernEvent } from 'api/functions';
    import router from "router";

    // Ваши импорты стилей
    import './main.sass';
    import './main.css';

    // Компоненты
    import mainmenu from './components/mainmenu.svelte';
    import Table from "./components/fractions/index.svelte";
    import market from "./components/market/index.svelte";
    import auction from './components/auction/index.svelte';
    import home from "./components/home/index.svelte";
    import biz from './components/biz/index.svelte';

    setGroup(".phone.");

    // Слушатель, который проверяет, есть ли звонок
    const isipadCall = () => {
        executeClientAsyncToGroup("isCall").then((result) => {
            if (result) currentPage.set("callView");
            else if ($currentPage === "callView") currentPage.set("mainmenu");
        });
    };
    isipadCall();
    addListernEvent("isPhoneCall", isipadCall);

    // Список доступных "view"
    const views = {
        mainmenu,
        Fractions: Table,
        Organization: Table,
        auction,
        market,
        home,
        biz
    };

    // Покажем анимацию "доставания"
    let isLoad = true; // Начинаем как "true" (ещё не показано)

    let selectedView = "mainmenu";

    onMount(async () => {
        isLoad = false; // когда компонент смонтировался, переключаем в false -> планшет появится
    });

    const onSelectedView = (view) => (selectedView = view);

    const onKeyUp = (event) => {
        if (!$router.opacity) return;
        const { keyCode } = event;
        const isCamera = $router.popup === "PopupCamera";
        if (keyCode == 27 && !isCamera) {
            executeClientToGroup("close");
        }
    };

    onDestroy(() => {
        // При уничтожении сбрасываем сторы
        currentPage.set("mainmenu");
        selectNumber.set(null);
        selectedImage.set(false);
        selectedImageFunc.set(false);
    });

    const onMain = () => {
        currentPage.set("mainmenu");
        selectNumber.set(null);
    };

    // Выполним некоторые клиентские функции
    executeClient("client.phone.cars.load");
    executeClient("client.gta5devmenucars");
    addListernEvent("phoneCarsLoad", true);
</script>

<svelte:window on:keyup={onKeyUp} />

<!-- ВАЖНО: оборачиваем в условие, чтобы сработали анимации in:fly/out:fly -->
{#if !isLoad}
<div
  class="gta5devpad"
  in:fly={{ y: 400, duration: 500 }}
  out:fly={{ y: 400, duration: 300 }}
>
    <div class="menupad">
        <div class="btnback" on:click={() => onSelectedView("mainmenu")}></div>
        <svelte:component this={views[selectedView]} {onSelectedView} {selectedView} />
    </div>
</div>
{/if}
