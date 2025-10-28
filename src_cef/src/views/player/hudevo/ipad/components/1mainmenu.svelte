<script>
    import { charWorkID, charFractionID, charOrganizationID } from 'store/chars';
    import Header from './header.svelte';
    import { currentPage } from '../stores';
    import { executeClientAsyncToGroup, executeClientToGroup } from 'api/rage';
    import { onMessage } from '@/views/player/hudevo/tabletnew/data';
    import { fade } from 'svelte/transition';

    // Импорт компонента Logo
    import Logo2 from '../components/fractions/elements/other/logo2.svelte';

    // Импорт иконок
    //import MapsIcon from '../assets/images/maps.png';
  //  import GalleryIcon from '../assets/images/gallery.png';
    import OrganizationIcon from '../assets/images/no-family.svg';
    //import CarsIcon from '../assets/images/rent.png';
    //import NewsIcon from '../assets/images/news.png';
  //  import MechIcon from '../assets/images/mech.png';
    import AvitoIcon from '../assets/images/avito.png';
    import PropertyIcon from '../assets/images/Home.svg';
    import PropertybIcon from '../assets/images/business.svg';
    //import TinderIcon from '../assets/images/tinder.png';
    //import RadioIcon from '../assets/images/radio.png';
   // import ForbesIcon from '../assets/images/forbes.png';
    //import TruckerIcon from '../assets/images/trucker.png';
    import AucIcon from '../assets/images/auction.png';
    //import CallIcon from '../assets/images/call.png';
    //import MessagesIcon from '../assets/images/messages.png';
    //import CameraIcon from '../assets/images/camera.png';
    import SettingsIcon from '../assets/images/settings.png';
   // import Avito from './avito.svelte';
   // import GiftIcon from '../assets/images/gift.png';

    // Меню
    $: menuArray = [
     /*   {
            name: "GPS",
            icon: MapsIcon,
            link: "maps"
        },
        /*{
            name: "Галерея",
            icon: GalleryIcon,
            link: "gallery"
        },*/
        {
            name: "Avito",
            icon: AvitoIcon,
            link: "avito"
        },
        {
            name: "Дом",
            icon: PropertyIcon,
            link: "property"
        },
        {
            name: "Бизнес",
            icon: PropertybIcon,
            link: "propertyb"
        },
      /*  {
            name: "Транспорт",
            icon: CarsIcon,
            link: "cars"
        },
        {
            name: "W.News",
            icon: NewsIcon,
            link: "news"
        },
        {
            name: "Развозчик",
            icon: TruckerIcon,
            link: "trucker",
            jobId: 6
        },
        {
            name: "Механик",
            icon: MechIcon,
            link: "mech"
        },
        {
            name: "Радио",
            icon: RadioIcon,
            link: "radio"
        },*/
        
       /* {
            name: "Forbes",
            icon: ForbesIcon,
            link: "forbes"
        },*/
        {
            name: "Аукцион",
            icon: AucIcon,
            link: "auction"
        },
      /*  {
            name: "Подарок",
            icon: GiftIcon,
            link: 101
        },*/
        {
            name: "Фракция",
            link: "Fractions",
            requiresFraction: true,
            useLogo2: true // Используем компонент Logo для иконки
        },
        {
            name: "Организация",
            icon: OrganizationIcon,
            link: "Organization",
            requiresOrganization: true
        },
      /*  {
            name: "Tinder",
            icon: TinderIcon,
            link: "tinder"
        },*/
    ];

       // Функция для обработки перехода на страницу
       const onSelectPage = (pageName) => {
        if (typeof pageName === "string") {
            currentPage.set(pageName);
        } else if (typeof pageName === "number") {
            onMessage(pageName);
            executeClientToGroup("messageDefault", pageName);
        }
    };

    // Функция для определения, недоступен ли пункт меню
    function isDisabled(item) {
        if (item.requiresFraction && $charFractionID <= 0) {
            return true;
        }
        if (item.requiresOrganization && $charOrganizationID <= 0) {
            return true;
        }
        return false;
    }

    // Фоновое изображение
    let background = "";
    executeClientAsyncToGroup("settings.wallpaper").then((result) => {
        background = result;
    });
</script>

<div class="newtablet__background" in:fade="{{duration: 200}}" style="background-image: url({background})">
    <Header />
    <div class="newtablet__mainmenu_grid">
        {#each menuArray as item}
        {#if item.jobId === undefined || item.jobId === $charWorkID}
 <div class="newtablet__mainmenu_element {isDisabled(item) ? 'disabled' : ''}"
                        on:click={() => {
                            if (!isDisabled(item)) {
                                onSelectPage(item.link);
                            }
                        }}
                    >
                        {#if item.useLogo2}
                            <div class="newtablet__mainmenu_icon">
                                <Logo2 />
                            </div>
                        {:else}
                            <div class="newtablet__mainmenu_icon" style="background-image: url({item.icon})"></div>
                        {/if}
                        <div class="newtablet__mainmenu_name">{item.name}</div>
                    </div>
                {/if}
            {/each}
        </div>
    <div class="newtablet__mainmenu_bottom">
        <div class="newtablet__mainmenu_element" on:click={() => onSelectPage("settings")}>
            <div class="newtablet__mainmenu_icon" style="background-image: url({SettingsIcon})"></div>
        </div>
    </div>
</div>

<style>
    .newtablet__mainmenu_element {
        /* Ваши стили */
    }

    .newtablet__mainmenu_element.disabled {
        opacity: 0.5;
        pointer-events: none;
    }

    .newtablet__mainmenu_element.disabled .newtablet__mainmenu_icon {
        filter: grayscale(100%);
    }

    .newtablet__mainmenu_icon {
        width: 64px;
        height: 64px;
        background-size: cover;
        background-position: center;
    }
</style>

