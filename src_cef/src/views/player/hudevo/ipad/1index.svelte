<script>
    import { fly } from 'svelte/transition';

    import { onDestroy } from 'svelte';

    import { onMount } from 'svelte';
    import { setGroup, executeClientAsyncToGroup, executeClientToGroup } from 'api/rage'
    import {currentPage, selectedImage, selectedImageFunc, selectNumber} from './stores'
    

    setGroup (".tablet.");

    const isTabletCall = () => {
        executeClientAsyncToGroup("isCall").then((result) => {
            if (result)
                currentPage.set("callView");
            else if ($currentPage === "callView")
                currentPage.set("mainmenu");
        });
    }

    isTabletCall ();
    addListernEvent ("isTabletCall", isTabletCall)

    import './main.sass';

    import mainmenu from './components/mainmenu.svelte'
   // import maps from './components/gps/index.svelte'
 //   import gallery from './components/gallery/index.svelte'
    import Popup from './components/gallery/popup.svelte'
  //  import camera from './components/camera/index.svelte'
   // import news from './components/news/index.svelte'
    //import trucker from './components/trucker/index.svelte'


    import property from './components/property/index.svelte'
    import propertyb from './components/propertyb/index.svelte'


    
    import Fractions from './components/fractions/index.svelte';
    import Organization from './components/fractions/index.svelte';
    //import cars from './components/cars/index.svelte'
    //import mech from './components/mech/index.svelte'
     import avito from './components/avito.svelte'
    //import radio from './components/radio/radio.svelte'
   // import forbes from './components/forbes/index.svelte'
   // import darknet from './components/darknet.svelte'
    import auction from './components/auction/index.svelte'
    //import call from './components/calls/call.svelte'
    //import callView from './components/calls/callView.svelte'
    import settings from './components/settings/settings.svelte'
   /// import messages from './components/messages/index.svelte'
  //  import tinder from './components/tinder/index.svelte'

    import Loader from './components/loader.svelte'


    import './assets/fonts/fonts/style.css'
    import './assets/fonts/smiles/style.css'


    import router from "router";
    import {addListernEvent} from "api/functions";

    const views = {
        mainmenu,
      //  call,
      //  callView,
        settings,
       // messages,
   //     maps,
       // gallery,
        Fractions,
        Organization,
       // cars,
        //news,
        mech,
        avito,
        property,
        propertyb,
        //radio,
      //  forbes,
        darknet,
        auction,
        //camera,
        //trucker,
        //tinder,
        Popup
    }

    let isLoad = true;

    onMount(async () => {
        isLoad = false;
    });

    const onKeyUp = (event) => {
        if (!$router.opacity)
            return;

        const { keyCode } = event;

        const isCamera = $router.popup === "PopupCamera";

        if(keyCode == 27 && !isCamera) {
            executeClientToGroup ("close")
        }
    }

    onDestroy(() => {
        currentPage.set("mainmenu");
        selectNumber.set(null);
        selectedImage.set(false);
        selectedImageFunc.set(false);
    });

    import Notification from './notification.svelte'
    export let tabletNotification;
    
</script>

<svelte:window on:keyup={onKeyUp} />

<div id="newtablet" in:fly={{ y: tabletNotification ? 100 : 200, duration: tabletNotification ? 250 : 500 }} out:fly={{ y: 200, duration: tabletNotification ? 0 : 250 }}>
    <div class="newtablet__image">
        {#if tabletNotification}
            <Notification {tabletNotification} />
        {/if}

        {#if $selectedImage === true}
            <Popup />
        {/if}

        {#if isLoad}
            <Loader />
        {/if}

        <svelte:component this={views[$currentPage]} selectView={$currentPage} />
    
    </div>
</div>