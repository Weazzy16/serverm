<script>
    import { translateText } from 'lang'
    export let popupData;
    import { ItemId } from 'json/itemsInfo.js'
    import { onMount, onDestroy, createEventDispatcher } from 'svelte';
    import { selectCase } from '../shop/elements/roulette/state.js'
    import { executeClient, executeClientAsync } from 'api/rage'
    import { accountRedbucks, accountUnique } from 'store/account'
    import { format } from 'api/formatter'
    const dispatch = createEventDispatcher();
    import './main.css';

    let width;
    $: endTs = caseData.endCase ? new Date(caseData.endCase).getTime() : null;
    let daysLeft = 0;

    function updateTimer() {
        if (!endTs) {
            daysLeft = 0;
            return;
        }
        const diff = endTs - Date.now();
        daysLeft = diff > 0 ? Math.ceil(diff / (24 * 60 * 60 * 1000)) : 0;
    }

    $: if (endTs) {
        updateTimer();
    }

    let timerInterval;
    onMount(() => {
        timerInterval = setInterval(updateTimer, 60 * 60 * 1000);
    });
    onDestroy(() => {
        clearInterval(timerInterval);
    });

    $: hideBuy = caseData.price <= 0 || (endTs !== null && daysLeft <= 0);

    window.addEventListener('resize', () => {
        width = window.innerWidth;
    });

    width = window.innerWidth;

    let caseData = {}
    const getData = () => {
        executeClientAsync("donate.roulette.getCaseOne", $selectCase).then((result) => {
            if (result && typeof result === "string") {
                caseData = JSON.parse(result);
                dispatch('casetitle', caseData.name);
                selectCaseToItems = caseData.items;
                casesData = GetRouletteData()
                isLoad = true;
                checkitem()
            }
        });
    }

    function updateCaseuses() {
        setTimeout(() => {
            caseuses = window.getItemToCount(ItemId["Case" + caseData.index]);
        }, 1000);
    }

    updateCaseuses();

    let value = 1;
    let roulletcase = false;
    let faststart = false
    
    executeClient("client.donate.roulette.loadCase", $selectCase);

    let isLoad = false;

    import { addListernEvent } from 'api/functions';
    addListernEvent("donate.roulette.initCase", getData)
    
    import PoputWin from './popupprise.svelte'
    let dataPopup;
    let isPopup;

    const maxCount = 8;

    const SetPopup = (toggled = false, data = null) => {
        dataPopup = data;
        isPopup = toggled;
        roulletcase = false;
    } 

    let antiFlud = 0;

    const getRndInteger = (min, max) => {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    const GetRouletteData = () => {
        let _casesData = [];
        for(let i = 0; i < maxCount; i++) {
            let newItems = [];
            const sCase = caseData.items;
            let randToIndex;

            for (let i = 0; i < 50; i++) {
                randToIndex = getRndInteger(0, sCase.length - 1);
                newItems = [
                    ...newItems,
                    sCase[randToIndex]
                ]
            }

            _casesData.push({
                randomBlocks: newItems,
                startRandomBlocks: newItems.slice(0, 9),
                winBlock: {},
                carousel: 0,
                carouselStart: 0,
                fixСarousel: true,
                IntervalId: null,
                IntervalTick: null,
                lastCenterIndex: -1
            });
        }
        return _casesData;
    }

    let currentCount = 1;
    let casesData = {},
        toggledFast = false,
        selectCaseToItems = [];

    let isConfirm = false;

    const tickSound = new Audio("http://cdn.piecerp.ru/cloud/inventoryItems/donate/cases/sound/tick.ogg");
    
    function startTickInterval(caseindex) {
        if (toggledFast) return;
        
        const containerSelector = `#popuponate__roulette1 .newdonate__roulette-main:nth-child(${caseindex + 1})`;
        const containerEl = document.querySelector(containerSelector);
        if (!containerEl) return;

        const centerLine = containerEl.querySelector(".linecenter");
        if (!centerLine) return;

        const items = containerEl.querySelectorAll(".newdonate__roulette-element");
        if (!items.length) return;

        casesData[caseindex].IntervalTick = setInterval(() => {
            const centerRect = centerLine.getBoundingClientRect();
            const centerX = centerRect.left + centerRect.width / 2;

            let minDistance = Infinity;
            let centerItemIndex = 0;

            items.forEach((elem, i) => {
                const rect = elem.getBoundingClientRect();
                const elemCenter = rect.left + rect.width / 2;
                const distance = Math.abs(elemCenter - centerX);
                if (distance < minDistance) {
                    minDistance = distance;
                    centerItemIndex = i;
                }
            });

            if (centerItemIndex !== casesData[caseindex].lastCenterIndex) {
                casesData[caseindex].lastCenterIndex = centerItemIndex;
                tickSound.currentTime = 0;
                tickSound.play().catch(e => {});
            }
        }, 50);
    }

    function stopTickInterval(caseindex) {
        if (casesData[caseindex].IntervalTick) {
            clearInterval(casesData[caseindex].IntervalTick);
            casesData[caseindex].IntervalTick = null;
        }
    }

    const Confirm = (data) => {
        if (isConfirm) return;

        isConfirm = true;
        data.forEach((caseItem, caseindex) => {
            const elemWidth = document.querySelector(`#popuponate__roulette1 .newdonate__roulette-main:nth-child(${caseindex + 1}) .newdonate__roulette-element:first-child`);
            let newItems = casesData[caseindex].startRandomBlocks;
            let randToIndex;
            for (let index = newItems.length; index < 50; index++) {
                if (index === caseItem.Index) {
                    newItems = [
                        ...newItems,
                        selectCaseToItems[caseItem.ItemIndex]
                    ];
                } else {
                    randToIndex = getRndInteger(0, selectCaseToItems.length - 1);
                    newItems = [
                        ...newItems,
                        selectCaseToItems[randToIndex]
                    ];
                }                    
            }

            const randomCarousel = Math.round(getRndInteger(0 - (elemWidth.clientWidth / 2) + 180, elemWidth.clientWidth / 2) - 180);

            casesData[caseindex] = {
                fixСarousel: false,
                winBlock: caseItem,
                randomBlocks: newItems,
                startRandomBlocks: newItems.slice(caseItem.Index - 3, caseItem.Index + 6),
                carouselStart: randomCarousel
            }

            setTimeout(() => {
                const first = document.querySelector(
                    `#popuponate__roulette1 .newdonate__roulette-main:nth-child(${caseindex + 1}) .newdonate__roulette-element:nth-child(4)`
                );
                const realCarousel = document.querySelector(
                    `#popuponate__roulette1 .newdonate__roulette-main:nth-child(${caseindex + 1}) .newdonate__roulette-element:nth-child(${caseItem.Index + 1})`
                );
                casesData[caseindex].carousel =
                    realCarousel.getBoundingClientRect().x - first.getBoundingClientRect().x + randomCarousel;

                startTickInterval(caseindex);

                if (!toggledFast) {
                    let stopToCord = -1;
                    casesData[caseindex].IntervalId = setInterval(() => {
                        if (stopToCord === elemWidth.getBoundingClientRect().left) {
                            clearInterval(casesData[caseindex].IntervalId);
                            casesData[caseindex].IntervalId = null;
                            casesData[caseindex].fixСarousel = true;

                            stopTickInterval(caseindex);
                            playSoundForColor(selectCaseToItems[caseItem.ItemIndex]);
                            openPopup();
                        } else {
                            stopToCord = elemWidth.getBoundingClientRect().left;
                        }
                    }, 500);
                } else {
                    casesData[caseindex].fixСarousel = true;
                    playSoundForColor(selectCaseToItems[caseItem.ItemIndex]);
                    openPopup();
                }
            }, 0)
        });
        return;
    }

    const openPopup = () => {
        let toggled = false;
        
        casesData.forEach((caseItem) => {
            if (!caseItem.fixСarousel && caseItem.winBlock && caseItem.winBlock.Item) {
                toggled = true;
            }
        })
        
        if (!toggled)
            SetPopup(true, casesData);
    }
    
    window.events.addEvent("cef.roullete.confirm", Confirm);

    onDestroy(() => {
        for(let i = 0; i < maxCount; i++) {
            if (casesData[i].IntervalId !== null) {
                clearInterval(casesData[i].IntervalId);
                casesData[i].IntervalId = null;
            }
            if (casesData[i].IntervalTick) {
                clearInterval(casesData[i].IntervalTick);
                casesData[i].IntervalTick = null;
            }
        }
        window.events.removeEvent("cef.roullete.confirm", Confirm);
        executeClient("client.roullete.confirm", false, -1);      
    });

    let caseuses = window.getItemToCount(ItemId["Case" + caseData.index])

    const onOpen = (_toggledFast = false) => {
        if (isConfirm) return;
        else if (antiFlud > new Date().getTime()) return;
        else if (window.getItemToCount(ItemId["Case" + caseData.index]) < currentCount)
            return window.notificationAdd(4, 9, `Недостаточно Кейсов!`, 3000);
        antiFlud = new Date().getTime() + 2500;
        toggledFast = _toggledFast;
        roulletcase = true;
        executeClient("client.roullete.open", caseData.index, currentCount);
        updateCaseuses();
    }

    const onOpen1 = (_toggledFast = false) => {
        let value1 = value
        if (antiFlud > new Date().getTime()) return;
        else if ($accountRedbucks < getPrice(caseData.price * value1, caseData.index, $accountUnique))
            return window.notificationAdd(4, 9, `Недостаточно AstraCoins!`, 3000);
        antiFlud = new Date().getTime() + 2500;
        toggledFast = _toggledFast;
        executeClient("client.roullete.buy", caseData.index, value1);
        updateCaseuses();
    }

    function checkitem() {
        caseuses = window.getItemToCount(ItemId["Case" + caseData.index]);
    }

    const onCurrentCount = (count) => {
        if (isConfirm) return;
        else if (antiFlud > new Date().getTime()) return;
        currentCount = count;
    }

    let isEndPopup = isPopup;
    $: {
        if (isPopup !== isEndPopup) {
            isEndPopup = isPopup;
            if (!isPopup && isConfirm) {                
                for(let i = 0; i < maxCount; i++) {
                    casesData[i].winBlock = {};
                }
                isConfirm = false;
            }
        }
    }

    const getPrice = (price, index, unique) => {
        if (unique && unique.split("_")) {
            let getData = unique.split("_");
            if (getData[0] === "cases" && Number(getData[1]) === index && Number(getData[2]) === 0) {
                price = Math.round(price * 0.7);
            }
        }
        return price;
    }

    const onfast = () => {
        faststart = true;
    }

    const offfast = () => {
        faststart = false;
    }

    $: totalPrice = getPrice(caseData.price * value, caseData.index, $accountUnique);

    const colorSounds = {
        "blue": new Audio("http://cdn.piecerp.ru/cloud/inventoryItems/donate/cases/sound/gray.ogg"),
        "yellow": new Audio("http://cdn.piecerp.ru/cloud/inventoryItems/donate/cases/sound/blue.ogg"),
        "pink": new Audio("http://cdn.piecerp.ru/cloud/inventoryItems/donate/cases/sound/red.ogg"),
        "red": new Audio("http://cdn.piecerp.ru/cloud/inventoryItems/donate/cases/sound/gold.ogg")
    };

    function playSoundForColor(item) {
        const color = item.color;
        if (colorSounds[color]) {
            colorSounds[color].play().catch((e) => console.error("Ошибка воспроизведения для цвета", color, e));
        }
    }

    let min = 1;
    let max = 20;
    let rangeEl;

    $: if (value > max) value = max;
    $: if (value < min) value = min;

    $: if (rangeEl) {
        const percent = ((value - min) / (max - min)) * 92;
        rangeEl.style.background = `
            linear-gradient(to right,
                #E81C5A 0%,
                #E81C5A ${percent}%,
                transparent ${percent}%,
                transparent 100%)
        `;
    }

    onMount(() => {
        rangeEl && rangeEl.dispatchEvent(new Event('input'));
    });

    function formatMoney(num) {
        return new Intl.NumberFormat('ru-RU', {
            maximumFractionDigits: 0
        }).format(num);
    }
</script>

{#if isLoad}
<div id="app" class="roulette-container">
    {#if isPopup}
        <PoputWin {SetPopup} popupData={dataPopup} {roulletcase} /> 
    {:else if roulletcase}
        <!-- Область с крутящейся рулеткой -->
        <div class="main-container">
            <div class="band-wrapper full-width {caseData.altCurrency || ''}">
                <div class="band-cover {caseData.altCurrency || ''}">
                    <div class="band-container align-center full-width full-height">
                        <div class="cursor row-block full-width full-height">
                            <div class="line"></div>
                        </div>
                        
                        <div id="popuponate__roulette1" class="roulette-band full-width row-block">
                            {#each casesData as caseData, indexCase}
                                {#if currentCount > indexCase}
                                    <div class="newdonate__roulette-main">
                                        <div class="linecenter"></div>
                                        <div 
                                            class="newdonate__roulette-elements" 
                                            style={`transition: ${caseData.fixСarousel ? "none" : "all 10000ms cubic-bezier(0.32, 0.64, 0.45, 1) 0s"};transform: translate3d(${caseData.fixСarousel ? (0 - (caseData.carouselStart + 350) / width * 100) : (0 - (caseData.carousel + 350) / width * 100)}vw, 0px, 0px)`}
                                        >
                                            {#each (!caseData.fixСarousel ? caseData.randomBlocks : caseData.startRandomBlocks) as item, index}
                                                <div class="vue-recycle-scroller__item-view">
                                                    <div class="case-element margin-22">
                                                        <div class="cases_content_prizes_list-item-line {item.color}"></div>
                                                        {#if item.price > 0}
                                                            <div class="price-badge">
                                                                {formatMoney(item.price)}
                                                                <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                                                            </div>
                                                        {/if}
                                                        <img src="http://cdn.piecerp.ru/cloud/img/roulette/{item.image}.png" alt="">
                                                        <span>{item.title}</span>
                                                    </div>
                                                </div>
                                            {/each}
                                        </div>
                                    </div>
                                {/if}
                            {/each}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    {:else}
        <!-- Главная страница кейса -->
        <div class="main-container">
            <div class="case-data full-width full-height row-block">
                <div class="case-data-container row-block align-center">
                    <!-- Описание кейса -->
                    <div class="description column-block">
                        <span class="description__title">{caseData.name}</span>
                        <span class="description__text">{caseData.desc}</span>
                    </div>

                    <!-- Изображение кейса -->
                    <div class="image-container align-center column-block">
                        <img 
                            class="image-container__picture" 
                            src="http://cdn.piecerp.ru/cloud/img/cases/{caseData.image}.png" 
                            alt=""
                        />
                        
                        <!-- Показываем сколько кейсов у игрока -->
                        <div class="player-cases row-block align-center">
                            <div class="player-cases__icon">
                                <svg width="35" height="35" viewBox="0 0 35 35" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <g filter="url(#filter0_d_712_2779)">
                                    <rect x="10" y="10" width="15" height="15" rx="2" fill="#E41958"/>
                                    </g>
                                    <path d="M16.3175 21C16.1118 20.9999 15.9145 20.9182 15.7691 20.7727L14.2177 19.2214C14.0764 19.0751 13.9983 18.8792 14 18.6758C14.0018 18.4724 14.0834 18.2779 14.2272 18.134C14.371 17.9902 14.5655 17.9086 14.7689 17.9069C14.9723 17.9051 15.1682 17.9833 15.3145 18.1246L16.2736 19.0837L20.3823 14.2905C20.4474 14.2094 20.528 14.1421 20.6196 14.0928C20.7111 14.0435 20.8116 14.0131 20.9151 14.0034C21.0186 13.9937 21.123 14.0049 21.2221 14.0363C21.3212 14.0678 21.413 14.1188 21.492 14.1864C21.5709 14.254 21.6355 14.3368 21.6819 14.4299C21.7282 14.523 21.7554 14.6244 21.7617 14.7282C21.7681 14.832 21.7535 14.936 21.7189 15.034C21.6843 15.132 21.6303 15.2221 21.5602 15.2988L16.9062 20.7285C16.834 20.8142 16.7439 20.883 16.6421 20.9299C16.5404 20.9769 16.4295 21.0008 16.3175 21Z" fill="white"/>
                                    <defs>
                                    <filter id="filter0_d_712_2779" x="0" y="0" width="35" height="35" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                                    <feFlood flood-opacity="0" result="BackgroundImageFix"/>
                                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"/>
                                    <feOffset/>
                                    <feGaussianBlur stdDeviation="5"/>
                                    <feComposite in2="hardAlpha" operator="out"/>
                                    <feColorMatrix type="matrix" values="0 0 0 0 0.894118 0 0 0 0 0.0980392 0 0 0 0 0.345098 0 0 0 0.5 0"/>
                                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_712_2779"/>
                                    <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_712_2779" result="shape"/>
                                    </filter>
                                    </defs>
                                </svg>
                            </div>
                            <span class="player-cases__title">У вас <b>{caseuses}</b> кейсов</span>
                        </div>
                    </div>

                    <!-- Блок покупки кейсов -->
                    {#if !hideBuy}
                        <div class="cases-control column-block">
                            <span class="cases-control__title">КУПИТЬ КЕЙСЫ</span>
                            
                            {#if endTs}
                                <div class="case-limits row-block">
                                    <div class="cases-left row-block align-center">
                                        <svg width="22" height="23" viewBox="0 0 22 23" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M21 11.5652C21 6.28249 16.5228 2 11 2C5.47715 2 1 6.28249 1 11.5652V12.4348C1 17.7175 5.47715 22 11 22C16.5228 22 21 17.7175 21 12.4348V11.5652Z" fill="#3DE7A5"/>
                                            <path d="M11 7V13L14 15" stroke="black" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                                        </svg>
                                        <div class="cases-left__value">
                                            Осталось: <span class="cases-left__value-highlighted">{daysLeft}</span> {daysLeft === 1 ? 'день' : 'дней'}
                                        </div>
                                    </div>
                                </div>
                            {/if}

                            <div class="buy-amount">
                                <span class="buy-amount__cases-amount">Количество кейсов</span>
                                <div class="input-field row-block justify-between">
                                    <input
                                        type="number"
                                        min={min}
                                        max={max}
                                        bind:value
                                        class="input-field__input full-height"
                                    />
                                    <div class="coins row-block align-center">
                                        <span class="coins__value">{formatMoney(totalPrice)}</span>
                                        <svg width="22" height="22" viewBox="0 0 22 22" xmlns="http://www.w3.org/2000/svg">
                                            <circle cx="11" cy="11" r="11" fill="#e81c5a"/>
                                            <path d="M6.193-3.412a2,2,0,0,0-.522,1.031c0,.378.183,1.318.692,1.318.326,0,.953-.744,1.148-.992A27.023,27.023,0,0,1,12.5-7.119l.026.026c-.914,1.188-2.571,3.145-2.571,4.711,0,.757.287,1.579,1.148,1.579,1.122,0,3.928-2.219,4.241-3.119l-.052-.5c-1.762,1.71-2.78,2.3-3.145,2.3-.313,0-.457-.378-.457-.639a5.748,5.748,0,0,1,1.5-2.962l2.023-2.467c.157-.2.561-.653.561-.914s-.548-1.122-.9-1.122c-.235,0-.5.274-.666.418C12.5-8.45,11-7.027,9.391-5.644L9.364-5.67c.535-.822,1.945-2.754,1.945-3.719a.936.936,0,0,0-.953-.9c-.535.013-1.644.992-2.127,1.449L6.154-6.832l-.026-.026a16.464,16.464,0,0,0,.809-2.271.967.967,0,0,0-.157-.5l-.313-.548c-.131-.222-.183-.4-.5-.418A11.83,11.83,0,0,0,3.5-9.677L2.03-9,2-9.024c.509-.444.692-.679.692-.757s-.091-.131-.157-.131a.81.81,0,0,0-.326.183c-1.161.822-1.083.8-1.2.953a7.615,7.615,0,0,0-.339,1.54c-.065.313.1.483.313.483.666,0,.039-.026,4.059-1.905a1.3,1.3,0,0,1,.4-.157c.222,0,.457.261-.914,2.714L1.808-2.655c.144.4.4,1.436.953,1.436.418,0,.744-.5.966-.8L5.684-4.652,8.816-8.045c.091-.1.248-.274.4-.274s.2.144-.026.561A19.4,19.4,0,0,1,7.081-4.574Z" transform="translate(3.129 16.731)" fill="#fff"/>
                                        </svg>
                                    </div>
                                    <input
                                        type="range"
                                        bind:this={rangeEl}
                                        min={min}
                                        max={max}
                                        bind:value
                                        class="slider-component"
                                    />
                                    <div class="slider-background"></div>
                                </div>
                            </div>

                            <div class="btn-wrapper">
                                <div class="buy-button row-block" on:click={() => onOpen1()}>
                                    <span class="buy-for__title">Купить за</span>
                                    <svg width="22" height="22" viewBox="0 0 22 22" xmlns="http://www.w3.org/2000/svg">
                                        <circle cx="11" cy="11" r="11" fill="#e81c5a"/>
                                        <path d="M6.193-3.412a2,2,0,0,0-.522,1.031c0,.378.183,1.318.692,1.318.326,0,.953-.744,1.148-.992A27.023,27.023,0,0,1,12.5-7.119l.026.026c-.914,1.188-2.571,3.145-2.571,4.711,0,.757.287,1.579,1.148,1.579,1.122,0,3.928-2.219,4.241-3.119l-.052-.5c-1.762,1.71-2.78,2.3-3.145,2.3-.313,0-.457-.378-.457-.639a5.748,5.748,0,0,1,1.5-2.962l2.023-2.467c.157-.2.561-.653.561-.914s-.548-1.122-.9-1.122c-.235,0-.5.274-.666.418C12.5-8.45,11-7.027,9.391-5.644L9.364-5.67c.535-.822,1.945-2.754,1.945-3.719a.936.936,0,0,0-.953-.9c-.535.013-1.644.992-2.127,1.449L6.154-6.832l-.026-.026a16.464,16.464,0,0,0,.809-2.271.967.967,0,0,0-.157-.5l-.313-.548c-.131-.222-.183-.4-.5-.418A11.83,11.83,0,0,0,3.5-9.677L2.03-9,2-9.024c.509-.444.692-.679.692-.757s-.091-.131-.157-.131a.81.81,0,0,0-.326.183c-1.161.822-1.083.8-1.2.953a7.615,7.615,0,0,0-.339,1.54c-.065.313.1.483.313.483.666,0,.039-.026,4.059-1.905a1.3,1.3,0,0,1,.4-.157c.222,0,.457.261-.914,2.714L1.808-2.655c.144.4.4,1.436.953,1.436.418,0,.744-.5.966-.8L5.684-4.652,8.816-8.045c.091-.1.248-.274.4-.274s.2.144-.026.561A19.4,19.4,0,0,1,7.081-4.574Z" transform="translate(3.129 16.731)" fill="#fff"/>
                                    </svg>
                                    <span class="buy-for__value">{formatMoney(totalPrice)}</span>
                                </div>
                            </div>
                        </div>
                    {/if}
                </div>
            </div>
        </div>
    {/if}

    <!-- Контрольная панель -->
    <div class="control-bar row-block justify-center full-width">
        <div class="bar-container row-block">
            {#if roulletcase}
                <div class="open is-opening">ОТКРЫВАЕМ...</div>
            {:else}
                <div class="open" on:click={() => faststart ? onOpen(true) : onOpen()}>
                    ОТКРЫТЬ КЕЙС
                </div>
            {/if}

            <div class="fast-open row-block align-center" class:hidden={roulletcase}>
                <label for="fastOpen" class="fast-open__title">БЫСТРОЕ ОТКРЫТИЕ</label>
                <input
                    id="fastOpen"
                    class="fast-open__checkbox"
                    type="checkbox"
                    bind:checked={faststart}
                />
            </div>
        </div>
    </div>

    <!-- Содержимое кейса -->
    <div class="case-content-title">СОДЕРЖИМОЕ КЕЙСА</div>
    <div class="case-elements">
        {#each caseData.items as item}
            <div class="case-element">
                <div class="cases_content_prizes_list-item-line {item.color}"></div>
                {#if item.price > 0}
                    <div class="price-badge">
                        {formatMoney(item.price)}
                        <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                    </div>
                {/if}
                <img src="http://cdn.piecerp.ru/cloud/img/roulette/{item.image}.png" alt="">
                <span>{item.title}</span>
            </div>
        {/each}
    </div>
</div>
{/if}