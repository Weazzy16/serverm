<script>
    import { translateText } from 'lang'
    import { accessType } from "../../data";
    import { executeClientToGroup } from "api/rage";

    export let isSkip = false;
    export let executeName;
    export let itemId;
    export let title;
    export let mainScroll = "";
    export let clsScroll = "";
    export let isSelector = true;

    let selectItemId = itemId;
    let isWeaponAccess = false;
    let access = []

    const getAccess = (_access) => {
        if (_access && typeof _access === "string")
            access = JSON.parse(_access);
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.rankAccess", getAccess)

    const onUpdateAccess = (id, type) => {
        const index = access.findIndex(a => a.id === id);

        const item = access[index];

        if (item.type === type)
            return;

        if (typeof item !== "undefined")
            item.oldType = item.type;

        item.type = type;

        access[index] = item;
    }

    const executeAccess = () => {
        const updateAccess = {};

        access.forEach((item) => {
            if (typeof item.oldType !== "undefined" && item.oldType !== item.type) {
                updateAccess [item.id] = item.type;
            }
        });

        if (updateAccess && Object.keys(updateAccess).length)
            executeClientToGroup(executeName, selectItemId, JSON.stringify(updateAccess))
    }

    import { onDestroy } from 'svelte';
    onDestroy(executeAccess)


    $: if (itemId !== undefined) {
        executeAccess ();
        selectItemId = itemId;
    }
</script>

{#if isSelector}
        <div class="btnsranks">
            <div class="btnappfр" class:active={!isWeaponAccess} on:keypress={() => {}} on:click={() => isWeaponAccess = false}>
                <p>{translateText('player1', 'Доступы')}</p>
            </div>
            {#if access.findIndex(a => a.isWeaponAccess) !== -1}
                <div class="btnappfр"  class:active={isWeaponAccess} on:keypress={() => {}} on:click={() => isWeaponAccess = true}>
                    <p>Craft</p>
                </div>
            {/if}
        </div>
        <div class="listacce">
            {#each access as item, index}
                <div class="blockacce">
                    <div class="btnseting" class:active={item.type === accessType.Add}>
                        <svg class="on" width="57" height="43" viewBox="0 0 57 43" fill="none" xmlns="http://www.w3.org/2000/svg" on:keypress={() => {}} on:click={() => onUpdateAccess (item.id, accessType.Remove)}>
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M15.857 3.09215C17.597 2.99215 19.335 3.00015 21.075 3.00015C21.087 3.00015 29.892 3.00015 29.892 3.00015C31.666 3.00015 33.404 2.99215 35.143 3.09215C36.724 3.18215 38.264 3.37415 39.797 3.80315C43.024 4.70515 45.842 6.58915 47.879 9.26015C49.904 11.9142 51 15.1632 51 18.4992C51 21.8392 49.904 25.0862 47.879 27.7402C45.842 30.4102 43.024 32.2952 39.797 33.1972C38.264 33.6262 36.724 33.8172 35.143 33.9082C33.404 34.0082 31.666 33.9992 29.926 33.9992C29.914 33.9992 21.107 34.0002 21.107 34.0002C19.335 33.9992 17.597 34.0082 15.857 33.9082C14.277 33.8172 12.737 33.6262 11.204 33.1972C7.977 32.2952 5.159 30.4102 3.122 27.7402C1.097 25.0862 0 21.8392 0 18.5002C0 15.1632 1.097 11.9142 3.122 9.26015C5.159 6.58915 7.977 4.70515 11.204 3.80315C12.737 3.37415 14.277 3.18215 15.857 3.09215Z" fill="#34C759"></path>
                            <g filter="url(#filter0_dd_1066_474)">
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M35.5 32C42.9558 32 49 25.9558 49 18.5C49 11.0442 42.9558 5 35.5 5C28.0442 5 22 11.0442 22 18.5C22 25.9558 28.0442 32 35.5 32Z" fill="white"></path>
                            </g>
                            <defs>
                            <filter id="filter0_dd_1066_474" x="14" y="0" width="43" height="43" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                            <feOffset dy="3"></feOffset>
                            <feGaussianBlur stdDeviation="0.5"></feGaussianBlur>
                            <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.06 0"></feColorMatrix>
                            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1066_474"></feBlend>
                            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                            <feOffset dy="3"></feOffset>
                            <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                            <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.15 0"></feColorMatrix>
                            <feBlend mode="normal" in2="effect1_dropShadow_1066_474" result="effect2_dropShadow_1066_474"></feBlend>
                            <feBlend mode="normal" in="SourceGraphic" in2="effect2_dropShadow_1066_474" result="shape"></feBlend>
                            </filter>
                            </defs>
                        </svg> 
                        <svg class="off" width="57" height="43" viewBox="0 0 57 43" fill="none" xmlns="http://www.w3.org/2000/svg" on:keypress={() => {}} on:click={() => onUpdateAccess (item.id, accessType.Add)}>
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M21.857 3.09215C23.597 2.99215 25.335 3.00015 27.075 3.00015C27.087 3.00015 35.892 3.00015 35.892 3.00015C37.666 3.00015 39.404 2.99215 41.143 3.09215C42.724 3.18215 44.264 3.37415 45.797 3.80315C49.024 4.70515 51.842 6.58915 53.879 9.26015C55.904 11.9142 57 15.1632 57 18.4992C57 21.8392 55.904 25.0862 53.879 27.7402C51.842 30.4102 49.024 32.2952 45.797 33.1972C44.264 33.6262 42.724 33.8172 41.143 33.9082C39.404 34.0082 37.666 33.9992 35.926 33.9992C35.914 33.9992 27.107 34.0002 27.107 34.0002C25.335 33.9992 23.597 34.0082 21.857 33.9082C20.277 33.8172 18.737 33.6262 17.204 33.1972C13.977 32.2952 11.159 30.4102 9.122 27.7402C7.097 25.0862 6 21.8392 6 18.5002C6 15.1632 7.097 11.9142 9.122 9.26015C11.159 6.58915 13.977 4.70515 17.204 3.80315C18.737 3.37415 20.277 3.18215 21.857 3.09215Z" fill="#787880" fill-opacity="0.16"></path>
                            <g filter="url(#filter0_dd_1066_477)">
                            <path fill-rule="evenodd" clip-rule="evenodd" d="M21.5 32C28.9558 32 35 25.9558 35 18.5C35 11.0442 28.9558 5 21.5 5C14.0442 5 8 11.0442 8 18.5C8 25.9558 14.0442 32 21.5 32Z" fill="white"></path>
                            </g>
                            <defs>
                            <filter id="filter0_dd_1066_477" x="0" y="0" width="43" height="43" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                            <feOffset dy="3"></feOffset>
                            <feGaussianBlur stdDeviation="0.5"></feGaussianBlur>
                            <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.06 0"></feColorMatrix>
                            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1066_477"></feBlend>
                            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                            <feOffset dy="3"></feOffset>
                            <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                            <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.15 0"></feColorMatrix>
                            <feBlend mode="normal" in2="effect1_dropShadow_1066_477" result="effect2_dropShadow_1066_477"></feBlend>
                            <feBlend mode="normal" in="SourceGraphic" in2="effect2_dropShadow_1066_477" result="shape"></feBlend>
                            </filter>
                            </defs>
                        </svg>                                                                               
                    </div>
                    <p>{item.name}</p>
                </div>
            {/each}
        </div>
        {:else}
        <div class="listacce">
        {#each access as item, index}
            <div class="blockacce">
                <div class="btnseting" class:active={item.type === accessType.Add}>
                    <svg class="on" width="57" height="43" viewBox="0 0 57 43" fill="none" xmlns="http://www.w3.org/2000/svg" on:keypress={() => {}} on:click={() => onUpdateAccess (item.id, accessType.Remove)}>
                        <path fill-rule="evenodd" clip-rule="evenodd" d="M15.857 3.09215C17.597 2.99215 19.335 3.00015 21.075 3.00015C21.087 3.00015 29.892 3.00015 29.892 3.00015C31.666 3.00015 33.404 2.99215 35.143 3.09215C36.724 3.18215 38.264 3.37415 39.797 3.80315C43.024 4.70515 45.842 6.58915 47.879 9.26015C49.904 11.9142 51 15.1632 51 18.4992C51 21.8392 49.904 25.0862 47.879 27.7402C45.842 30.4102 43.024 32.2952 39.797 33.1972C38.264 33.6262 36.724 33.8172 35.143 33.9082C33.404 34.0082 31.666 33.9992 29.926 33.9992C29.914 33.9992 21.107 34.0002 21.107 34.0002C19.335 33.9992 17.597 34.0082 15.857 33.9082C14.277 33.8172 12.737 33.6262 11.204 33.1972C7.977 32.2952 5.159 30.4102 3.122 27.7402C1.097 25.0862 0 21.8392 0 18.5002C0 15.1632 1.097 11.9142 3.122 9.26015C5.159 6.58915 7.977 4.70515 11.204 3.80315C12.737 3.37415 14.277 3.18215 15.857 3.09215Z" fill="#34C759"></path>
                        <g filter="url(#filter0_dd_1066_474)">
                        <path fill-rule="evenodd" clip-rule="evenodd" d="M35.5 32C42.9558 32 49 25.9558 49 18.5C49 11.0442 42.9558 5 35.5 5C28.0442 5 22 11.0442 22 18.5C22 25.9558 28.0442 32 35.5 32Z" fill="white"></path>
                        </g>
                        <defs>
                        <filter id="filter0_dd_1066_474" x="14" y="0" width="43" height="43" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                        <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                        <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                        <feOffset dy="3"></feOffset>
                        <feGaussianBlur stdDeviation="0.5"></feGaussianBlur>
                        <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.06 0"></feColorMatrix>
                        <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1066_474"></feBlend>
                        <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                        <feOffset dy="3"></feOffset>
                        <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                        <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.15 0"></feColorMatrix>
                        <feBlend mode="normal" in2="effect1_dropShadow_1066_474" result="effect2_dropShadow_1066_474"></feBlend>
                        <feBlend mode="normal" in="SourceGraphic" in2="effect2_dropShadow_1066_474" result="shape"></feBlend>
                        </filter>
                        </defs>
                    </svg> 
                    <svg class="off" width="57" height="43" viewBox="0 0 57 43" fill="none" xmlns="http://www.w3.org/2000/svg" on:keypress={() => {}} on:click={() => onUpdateAccess (item.id, accessType.Add)}>
                        <path fill-rule="evenodd" clip-rule="evenodd" d="M21.857 3.09215C23.597 2.99215 25.335 3.00015 27.075 3.00015C27.087 3.00015 35.892 3.00015 35.892 3.00015C37.666 3.00015 39.404 2.99215 41.143 3.09215C42.724 3.18215 44.264 3.37415 45.797 3.80315C49.024 4.70515 51.842 6.58915 53.879 9.26015C55.904 11.9142 57 15.1632 57 18.4992C57 21.8392 55.904 25.0862 53.879 27.7402C51.842 30.4102 49.024 32.2952 45.797 33.1972C44.264 33.6262 42.724 33.8172 41.143 33.9082C39.404 34.0082 37.666 33.9992 35.926 33.9992C35.914 33.9992 27.107 34.0002 27.107 34.0002C25.335 33.9992 23.597 34.0082 21.857 33.9082C20.277 33.8172 18.737 33.6262 17.204 33.1972C13.977 32.2952 11.159 30.4102 9.122 27.7402C7.097 25.0862 6 21.8392 6 18.5002C6 15.1632 7.097 11.9142 9.122 9.26015C11.159 6.58915 13.977 4.70515 17.204 3.80315C18.737 3.37415 20.277 3.18215 21.857 3.09215Z" fill="#787880" fill-opacity="0.16"></path>
                        <g filter="url(#filter0_dd_1066_477)">
                        <path fill-rule="evenodd" clip-rule="evenodd" d="M21.5 32C28.9558 32 35 25.9558 35 18.5C35 11.0442 28.9558 5 21.5 5C14.0442 5 8 11.0442 8 18.5C8 25.9558 14.0442 32 21.5 32Z" fill="white"></path>
                        </g>
                        <defs>
                        <filter id="filter0_dd_1066_477" x="0" y="0" width="43" height="43" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                        <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                        <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                        <feOffset dy="3"></feOffset>
                        <feGaussianBlur stdDeviation="0.5"></feGaussianBlur>
                        <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.06 0"></feColorMatrix>
                        <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1066_477"></feBlend>
                        <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                        <feOffset dy="3"></feOffset>
                        <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                        <feColorMatrix type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0.15 0"></feColorMatrix>
                        <feBlend mode="normal" in2="effect1_dropShadow_1066_477" result="effect2_dropShadow_1066_477"></feBlend>
                        <feBlend mode="normal" in="SourceGraphic" in2="effect2_dropShadow_1066_477" result="shape"></feBlend>
                        </filter>
                        </defs>
                    </svg>                                                                               
                </div>
                <p>{item.name}</p>
            </div>
        {/each}
        </div>
{/if}