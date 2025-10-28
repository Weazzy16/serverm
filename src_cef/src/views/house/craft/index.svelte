<script>
    import { translateText } from 'lang'
    import './main.css'
    import { executeClient } from 'api/rage'
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { onMount, onDestroy } from 'svelte';

    export let viewData;

    let value = 1;

    let nameselect = 0;

    let selectitemblock = 0;

    const selectitem = (index) => {
        executeClient ('clien.gta5devcraft.getinfo', index);
        selectitemblock = index;
        nameselect = viewData[index].Item;
    };

    let ingredient1;

    let shans1 = 0;

    let time1 = 0;

    window.gta5devcraft = function (resultitem, ingredient, shans, time) {
        nameselect = resultitem;
        ingredient1 = ingredient;
        shans1 = shans;
        time1 = time;
        checkitem();
    };

    onMount(() => {
        window.addEventListener('mousemove', (event) => trackMousePosition(event));
    });

    onDestroy(() => {
        window.removeEventListener('mousemove', (event) => trackMousePosition(event));
    });

    function handleNumberChange(event) {
        checkitem();
    }

    let checkblock = true;

    function checkitem() {
        checkblock = true;
        ingredient1.forEach(element => {
            if (window.getItemToCount(element.Item) < element.Count * value) checkblock = false;
        });
    }

    const startcraft = () => {
        executeClient ('clien.gta5devcraft.startcraft', nameselect, value);
        youitemcheck();
    }

    let hoveritemid = 0;

    let mouseX = 0;
    let mouseY = 0;

    function handleFocus(event) {
        // Обработка фокуса
    }

    function handleBlur() {
        // Обработка потери фокуса
    }

    let hoveritem = false;

    function trackMousePosition(event) {
        mouseX = event.clientX;
        mouseY = event.clientY;
        if (hoveritem) {
            requestAnimationFrame(trackMousePosition(event));
        }
    }

    function handleMouseOver(event, itemid) {
        mouseX = event.clientX;
        mouseY = event.clientY;
        hoveritem = true;
        hoveritemid = itemid;
        trackMousePosition(event);
    }

    function handleMouseOut() {
        mouseX = 0;
        mouseY = 0;
        hoveritem = false;
    }

    let searchText = "";

</script>
{#if hoveritem}
<div class="iteminfoblock" style={`top:${mouseY}px;left:${mouseX + 10}px;`}>
    <h1>{itemsInfo[hoveritemid].Name}</h1>
    <p>{itemsInfo[hoveritemid].Description}</p>
</div>
{/if}

<div class="gta5devcraftmenu">
    <div class="craftmenu">
        <div class="cmleft">
            <h1>Крафт</h1>
            <div class="bginput">
                <svg width="17" height="17" viewBox="0 0 17 17" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M14.5637 13.0613L11.8665 10.3642C12.4434 9.49716 12.7508 8.47886 12.75 7.4375C12.75 4.50819 10.3668 2.125 7.4375 2.125C4.50819 2.125 2.125 4.50819 2.125 7.4375C2.125 10.3668 4.50819 12.75 7.4375 12.75C8.47886 12.7508 9.49716 12.4434 10.3642 11.8665L13.0613 14.5637C13.1593 14.6652 13.2766 14.7461 13.4062 14.8018C13.5358 14.8575 13.6752 14.8868 13.8163 14.888C13.9574 14.8892 14.0973 14.8624 14.2279 14.8089C14.3585 14.7555 14.4771 14.6766 14.5769 14.5769C14.6766 14.4771 14.7555 14.3585 14.8089 14.2279C14.8624 14.0973 14.8892 13.9574 14.888 13.8163C14.8868 13.6752 14.8575 13.5358 14.8018 13.4062C14.7461 13.2766 14.6652 13.1593 14.5637 13.0613ZM3.71875 7.4375C3.71875 5.38688 5.38688 3.71875 7.4375 3.71875C9.48812 3.71875 11.1562 5.38688 11.1562 7.4375C11.1562 9.48812 9.48812 11.1562 7.4375 11.1562C5.38688 11.1562 3.71875 9.48812 3.71875 7.4375Z" fill="white" fill-opacity="0.4"/>
                </svg>                        
                <input bind:value={searchText} placeholder="Введите значение для поиска"> 
            </div>
            <div class="cmllist">
                {#each viewData as item,index}
                    {#if (!searchText || !searchText.length) || (searchText && itemsInfo[item.Item].Name.toLowerCase().trim().includes(searchText.toLowerCase().trim()))}
                    <div class="blockitem" class:active={index === selectitemblock} on:keypress={() => {}} on:click={() => selectitem(index)}>
                        <img src="{document.cloud}/inventoryItems/items/{item.Item}.png" alt=""/>
                        <div class="info">
                            <p>{itemsInfo[item.Item].Name}</p>
                            <span>{item.Count} шт.</span>
                        </div>
                    </div>
                    {/if}
                {/each}
            </div>
        </div>
        {#if nameselect}
        <div class="cmright">
            <div class="cmrhead">
                <h1>{itemsInfo[nameselect].Name}</h1>
                <span>{itemsInfo[nameselect].Description}</span>
            </div>
            <h1>Требования к крафту:</h1>
            <div class="itemlist">
                {#if ingredient1}
                {#each ingredient1 as item,index}
                    <div class="itemcraft">
                        <div class="bgitems" on:mouseover={(event) => handleMouseOver(event, item.Item)} 
                            on:mouseout={handleMouseOut} 
                            on:focus={handleFocus} 
                            on:blur={handleBlur}>
                            <img src="{document.cloud}/inventoryItems/items/{item.Item}.png" alt=""/>
                        </div>
                        <p><b>{window.getItemToCount(item.Item)}</b>/ {item.Count * value}</p>
                        {#if window.getItemToCount(item.Item) >= item.Count * value}
                            <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M11 19C15.4183 19 19 15.4183 19 11C19 6.58172 15.4183 3 11 3C6.58172 3 3 6.58172 3 11C3 15.4183 6.58172 19 11 19Z" fill="#4AB866"/>
                                <path d="M8.9852 15C8.72022 14.9999 8.46613 14.9065 8.27879 14.7402L6.28048 12.9673C6.09847 12.8001 5.99776 12.5762 6.00004 12.3437C6.00231 12.1113 6.1074 11.889 6.29265 11.7246C6.4779 11.5603 6.7285 11.467 6.99048 11.465C7.25245 11.463 7.50484 11.5523 7.69329 11.7138L8.92874 12.8099L14.2213 7.33198C14.3051 7.23927 14.409 7.16245 14.5269 7.10609C14.6448 7.04972 14.7742 7.01496 14.9076 7.00387C15.0409 6.99278 15.1754 7.00558 15.303 7.04152C15.4307 7.07746 15.5489 7.1358 15.6506 7.21307C15.7524 7.29034 15.8356 7.38497 15.8952 7.49133C15.9549 7.5977 15.9899 7.71362 15.9981 7.83222C16.0063 7.95081 15.9876 8.06966 15.943 8.1817C15.8984 8.29373 15.8288 8.39667 15.7385 8.48439L9.74356 14.6897C9.65061 14.7877 9.53447 14.8663 9.4034 14.9199C9.27232 14.9735 9.12953 15.0009 8.9852 15Z" fill="white"/>
                            </svg>
                        {:else}
                            <svg width="22" height="22" viewBox="0 0 22 22" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M11 19C15.4183 19 19 15.4183 19 11C19 6.58172 15.4183 3 11 3C6.58172 3 3 6.58172 3 11C3 15.4183 6.58172 19 11 19Z" fill="#BF3B3B"/>
                                <path d="M8 8L11 11M14 14L11 11M11 11L14 8L8 14" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                            </svg>    
                        {/if}                                                                               
                    </div>
                {/each}
                {/if}
            </div>
            <div class="inputhead">
                <p>Количество предметов</p>
                <p>Время к крафту</p>
            </div>
            <div class="inputbg">
                <input placeholder="1" maxlength="3" bind:value={value} on:input={handleNumberChange}>
                <p>{Math.floor((time1 * value) / 60)} м. {(time1 * value) % 60} с.</p>
            </div>
            {#if checkblock}
                <div class="btncraft"  on:keypress on:click={startcraft}>
                    <p>Запустить крафт</p>
                </div>
            {:else}
                <div class="btncraftnone">
                    <p>Не хватает ресурсов</p>
                </div>
            {/if}
            <div class="kpd">
                <p>Шанс успеха крафта</p>
                <span>{Math.round(shans1 * 100)}%</span>
            </div>
        </div>
        {/if}
        <span class="closed" on:keypress={() => {}} on:click={() => executeClient ('clien.gta5devcraft.exit')}>
            <svg width="13" height="13" viewBox="0 0 13 13" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M1.5 1.5L6.5 6.5M11.5 11.5L6.5 6.5M6.5 6.5L11.5 1.5L1.5 11.5" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path>
            </svg>                                        
        </span>
    </div>
</div>
