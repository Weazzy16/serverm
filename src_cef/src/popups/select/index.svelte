<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import './main.sass'
    import { fly } from 'svelte/transition';

    export let popupData;

    let title = popupData.title,
        elements = [];

    $: if (popupData.elements && typeof popupData.elements === "string")
        elements = JSON.parse(popupData.elements);

    let listId = 0;

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        switch (keyCode) {
            case 13:
                onSelected (elements[listId][1]);
                break;

            case 38: // up
                if(--listId < 0)
                    listId = elements.length - 1;
                break;
            case 40: // down
                if(++listId >= elements.length)
                    listId = 0;
                break;

            case 27:
                onSelected("null");
                break;
        }
    }

    const onSelected = (listItem) => {
        executeClient ('popup.list.selected', listItem)
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />


{#if title == "Рабочая одежда"}
    <div class="gta5devshop tattoo">
        <div class="left">
            <img src="http://cdn.magicfive.ru/haxzer/shop/cstore.png" alt="">
            <p>Выйти</p>
            <div class="listitems">
                {#each elements as item, index}
                    <div class="blockitem" on:keypress={() => {}} on:click={() => onSelected (item[1])}>
                        <p>{item [0]}</p>
                    </div>
                {/each}
            </div>
            <div class="btnback" on:keypress={() => {}} on:click={() => onSelected("null")}>
                <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <g clip-path="url(#clip0_30_47)">
                    <path d="M14.9333 0H1.06667C0.783856 0.000302558 0.512709 0.120808 0.312731 0.335069C0.112754 0.549331 0.000282388 0.839845 0 1.14286V14.8571C0.000282388 15.1602 0.112754 15.4507 0.312731 15.6649C0.512709 15.8792 0.783856 15.9997 1.06667 16H14.9333C15.2161 15.9997 15.4873 15.8792 15.6873 15.6649C15.8872 15.4507 15.9997 15.1602 16 14.8571V1.14286C15.9997 0.839845 15.8872 0.549331 15.6873 0.335069C15.4873 0.120808 15.2161 0.000302558 14.9333 0ZM11.7333 8.57143H5.55413L7.84373 11.0246C7.89467 11.0773 7.9353 11.1403 7.96325 11.2101C7.99121 11.2798 8.00592 11.3548 8.00653 11.4306C8.00715 11.5065 7.99365 11.5817 7.96684 11.652C7.94002 11.7222 7.90042 11.786 7.85034 11.8397C7.80027 11.8933 7.74072 11.9357 7.67518 11.9645C7.60963 11.9932 7.5394 12.0077 7.46859 12.007C7.39777 12.0063 7.32779 11.9906 7.26272 11.9606C7.19765 11.9307 7.1388 11.8871 7.0896 11.8326L3.8896 8.404C3.78984 8.2966 3.73384 8.15124 3.73384 7.99971C3.73384 7.84819 3.78984 7.70283 3.8896 7.59543L7.0896 4.16743C7.19019 4.06334 7.32491 4.00574 7.46475 4.00704C7.60459 4.00835 7.73835 4.06844 7.83724 4.17439C7.93612 4.28034 7.99221 4.42366 7.99343 4.57349C7.99464 4.72331 7.94088 4.86766 7.84373 4.97543L5.55413 7.42857H11.7333C11.8748 7.42857 12.0104 7.48878 12.1105 7.59594C12.2105 7.7031 12.2667 7.84845 12.2667 8C12.2667 8.15155 12.2105 8.2969 12.1105 8.40406C12.0104 8.51123 11.8748 8.57143 11.7333 8.57143Z"></path>
                    </g>
                    <defs>
                    <clipPath id="clip0_30_47">
                    <rect width="16" height="16"></rect>
                    </clipPath>
                    </defs>
                </svg>                    
                <p>Выйти</p>
            </div>
        </div>
    </div>
    {:else}
    <div class="popup__newhud_boxinput">
        <div class="popup__newhud_esc">
            <div class="popup__newhud_escbutton box-center">ESC</div>
            <div>{translateText('popups', 'Закрыть')}</div>
        </div>
        <div class="popup__newhud_box">
            <div class="popup__newhud_title">
                <span class="hud__icon-info popup__newhud_icon"/> {title}
            </div>  
            <div class="popup__select_elements">
                {#each elements as item, index}
                    <div class="popup__select_element" class:active={listId === index} on:keypress={() => {}} on:click={() => onSelected (item[1])}>
                        {item [0]}
                    </div>
                {/each}
            </div>
            <div class="popup__newhud__buttons">
                <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:keypress={() => {}} on:click={() => onSelected("null")}>{translateText('popups', 'Отмена')}</div>
            </div>
        </div>
    </div>
{/if}