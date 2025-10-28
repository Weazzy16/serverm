<script>
    import { onMount } from 'svelte';
    import { tweened } from 'svelte/motion';
    import { cubicOut } from 'svelte/easing';
    import { TimeFormat } from 'api/moment';
    import { serverDateTime } from 'store/server';
    import { isInputToggled, isWaterMark, isPlayer, isHelp, isPhone } from 'store/hud';
    import keys from 'store/keys';
    import keysName from 'json/keys.js';
    import bankimg from './assets/img/Subtract.svg';
    import walletimg from './assets/img/Union.svg';
    import { charFractionID, charOrganizationID } from 'store/chars';
    import CustomKey from './Key.svelte';
    import { itemsInfo } from 'json/itemsInfo.js';
    import { serverDonatMultiplier } from 'store/server';
    import { charWanted, charMoney, charBankMoney } from 'store/chars';
    import { fly } from 'svelte/transition';
    import { format } from 'api/formatter';
    import { translateText } from 'lang';
    import { getQuest } from 'json/quests/quests.js';
    import router from 'router';
    import { executeClient } from 'api/rage';
    import { executeClientAsyncToGroup } from "api/rage";
    import { weatherName, getWeatherIdToName } from './data.js';
    import { get } from 'svelte/store';


    let weatherIcon = 'sunny';
    let weatherDescription = 'Солнечно';
    let temperature = '20';

    const weatherMap = {
        0: 'sunny',
        1: 'sunny',
        2: 'cloud',
        3: 'cloud',
        4: 'fog',
        5: 'overcast',
        6: 'rain',
        7: 'thunder',
        8: 'cloud',
        10: 'snowlight',
        11: 'snow',
        12: 'blizzard',
        13: 'xmas'
    };

    window.hudStore.weatherId = (weatherId) => {
        const hour = new Date(get(serverDateTime)).getHours();
        const baseIcon = weatherMap[weatherId] || 'sunny';
        const isNight = hour >= 0 && hour < 6;

        if (isNight && ['sunny', 'cloud', 'overcast'].includes(baseIcon)) {
            weatherIcon = 'night';
        } else {
            weatherIcon = baseIcon;
        }

        weatherDescription = weatherName[weatherIcon] || 'Погода';

        console.log(`Weather ID changed: ${weatherId}, hour: ${hour}, icon: ${weatherIcon}, desc: ${weatherDescription}`);
    };

    window.hudStore.weatherTemp = (temp) => {
        temperature = temp;
        console.log(`Temperature changed: ${temp}`);
    };

    window.hudStore.currentWeather = (weatherId, temp) => {
    // Поддержка передачи объекта вместо отдельных параметров
    if (typeof weatherId === 'object') {
        const data = weatherId;
        weatherId = data.weatherId;
        temp = data.temp;
    }

    const hour = new Date(serverDateTime).getHours();
    weatherIcon = getWeatherIdToName(weatherId, hour);
    weatherDescription = weatherName[weatherIcon];
    temperature = temp;

    console.log(`Weather ID changed: ${weatherId}, hour: ${hour}, icon: ${weatherIcon}, desc: ${weatherDescription}`);
    console.log(`Temperature changed: ${temperature}`);
};


    export let SafeSone;

    const money = tweened(0, {
        duration: 700,
        easing: cubicOut
    });

    const bank = tweened(0, {
        duration: 700,
        easing: cubicOut
    });

    let moneyChangeDirection = null;
    let bankChangeDirection = null;

    let previousMoney = 0;
    let previousBank = 0;

    let isWorld = true;
    window.hudStore.isWorld = (value) => isWorld = value;

    let greenZone = false;
    window.hudStore.greenZone = (value) => greenZone = value;

    let street = "";
    window.hudStore.street = (value) => street = value;

    let area = "";
    window.hudStore.area = (value) => area = value;

    let micIcon = "hud__icon-micoff";

    let microphone = 0;
    window.hudStore.microphone = (value) => {
        microphone = value;
        UpdateMicrophoneIcon();
    }

    let isMute = false;
    window.hudStore.isMute = (value) => {
        isMute = value;
        UpdateMicrophoneIcon();
    }

    const UpdateMicrophoneIcon = () => {
    if (isMute) micIcon = "hud__icon-micmute";
    else if (microphone) micIcon = "hud__icon-custommic";  // используем кастомный значок
    else micIcon = "hud__icon-micoff";
}



    let polygon = 0;
    window.hudStore.polygon = (value) => polygon = value;

    let radio = 0;
    window.hudStore.radio = (value) => radio = value;

    let serverPlayerId = 0;
    window.serverStore.serverPlayerId = (value) => serverPlayerId = value;

    let serverOnline = 0;
    window.serverStore.serverOnline = (value) => serverOnline = value;

    const getOnlineName = (online) => {
        if (online <= 99)
            return "Низкий";
        else if (online <= 499)
            return "Средний";

        return "Высокий";
    }

    const formatNumberWithSpaces = (number) => {
        if (typeof number !== 'number') return '0';
        return number.toLocaleString('ru-RU').replace(/,/g, ' ');
    }

    onMount(() => {
        charMoney.subscribe(value => {
            const numericValue = Number(value);
            if (isNaN(numericValue)) return;

            if (numericValue > previousMoney) moneyChangeDirection = 'increase';
            else if (numericValue < previousMoney) moneyChangeDirection = 'decrease';
            else moneyChangeDirection = null;

            money.set(numericValue);
            previousMoney = numericValue;

            if (moneyChangeDirection) {
                setTimeout(() => {
                    moneyChangeDirection = null;
                }, 700);
            }
        });

        charBankMoney.subscribe(value => {
            const numericValue = Number(value);
            if (isNaN(numericValue)) return;

            if (numericValue > previousBank) bankChangeDirection = 'increase';
            else if (numericValue < previousBank) bankChangeDirection = 'decrease';
            else bankChangeDirection = null;

            bank.set(numericValue);
            previousBank = numericValue;

            if (bankChangeDirection) {
                setTimeout(() => {
                    bankChangeDirection = null;
                }, 700);
            }
        });
    });
</script>

<style>
    .money, .bank {
        transition: color .5s ease;
    }
    .increase {
        transform: scale(1.02);
        color: #6fec74;
    }
    .decrease {
        transform: scale(1.02);
        color: #f44336;
    }
    




</style>

<div class="hudevo__placetime">
    <div class="placetime__buttonsw">
        <div class="playerinfo__box">
            <CustomKey bottom={true} keyCode={$keys[36]} nonactive={isMute || $isInputToggled}>
                <span class="placetime__icon {micIcon}" class:active={microphone} class:polygon={polygon} class:mute={isMute}></span>
                
            </CustomKey>
            
        </div>
    </div>

    <div class="hud_left_bottom">
        <div class="hud_weather_block">
            <span class="weather_temp">+ {temperature}</span>
            <img class="weather_icon" src={`http://cdn.piecerp.ru/cloud/weather/${weatherIcon}.svg`} alt={weatherDescription}>
            <span class="weather_desc">{weatherDescription}</span>
        </div>

<div class="time-container">
        <div class="leftbottom_timeblock">
            <div class="leftbottom_timeblock_top">{TimeFormat($serverDateTime, "H:mm")}</div>
             <div class="leftbottom_timeblock_day"> {TimeFormat($serverDateTime, "DD.MM.YYYY")}</div>
            </div>
            <div class="timeblock_top_day"> {TimeFormat($serverDateTime, 'dddd')[0].toUpperCase() + TimeFormat($serverDateTime, 'dddd').slice(1)}
            </div>
        </div>
        <div class="streetblock">
        <div class="leftbottom_streetblock">{area}</div>
        <div class="leftbottom_streetblocks">{street}</div>
    </div>
        <div class="leftbottom_moneyblock">
            <div class="leftbottom_moneyblock_money">
                <span>
               <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16">
    <g id="money" transform="translate(0 0)">
        <path id="Контур_15474" data-name="Контур 15474" d="M.218,8.021a.333.333,0,0,0-.2.427l.209.576L1.823,7.429Z" fill="#fff"/>
        <path id="Контур_15475" data-name="Контур 15475" d="M1.727,13.141l.96,2.64a.33.33,0,0,0,.173.188A.335.335,0,0,0,3,16a.326.326,0,0,0,.115-.021l1.059-.391Z" fill="#fff"/>
        <path id="Контур_15476" data-name="Контур 15476" d="M15.98,10.887l-1.1-3.021L12.244,10.5l1.307-.481a.333.333,0,0,1,.231.625l-1.66.612a.333.333,0,0,1-.428-.2s0,0,0-.006L8.9,13.849l6.882-2.536A.332.332,0,0,0,15.98,10.887Z" fill="#fff"/>
        <path id="Контур_15477" data-name="Контур 15477" d="M15.9,5.431,10.569.1A.334.334,0,0,0,10.1.1l-10,10a.334.334,0,0,0,0,.471L5.431,15.9a.33.33,0,0,0,.235.1.335.335,0,0,0,.236-.1l10-10A.334.334,0,0,0,15.9,5.431ZM3.9,9.236,2.569,10.569A.334.334,0,0,1,2.1,10.1L3.431,8.765a.334.334,0,0,1,.472.471Zm5.756.423a1.258,1.258,0,0,1-.913.349,2.754,2.754,0,0,1-1.854-.885A3.054,3.054,0,0,1,6.059,7.7a1.393,1.393,0,0,1,.3-1.349A1.389,1.389,0,0,1,7.7,6.06a3.048,3.048,0,0,1,1.419.833C10.049,7.817,10.285,9.033,9.659,9.659ZM13.9,5.9,12.569,7.236a.334.334,0,0,1-.472-.471L13.43,5.431A.334.334,0,0,1,13.9,5.9Z" fill="#fff"/>
    </g>
</svg>
                </span>
                <span class="money {moneyChangeDirection}">
                    ${formatNumberWithSpaces(Math.round($money))}
                </span>
            </div>
            <div class="leftbottom_moneyblock_bank">
                <span>
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="10.88" viewBox="0 0 16 10.88">
    <g id="credit-card_1_" data-name="credit-card (1)" transform="translate(0 -2.56)">
        <path id="Контур_15478" data-name="Контур 15478" d="M15.04,2.56H.96A.961.961,0,0,0,0,3.52v1.6H16V3.52A.961.961,0,0,0,15.04,2.56Z" fill="#fff"/>
        <path id="Контур_15479" data-name="Контур 15479" d="M0,12.48a.961.961,0,0,0,.96.96H15.04a.961.961,0,0,0,.96-.96V7.04H0ZM2.88,8.96H8.64a.32.32,0,0,1,0,.64H2.88a.32.32,0,1,1,0-.64Zm0,1.28h3.2a.32.32,0,1,1,0,.64H2.88a.32.32,0,1,1,0-.64Z" fill="#fff"/>
    </g>
</svg>
          </span>      
                <span class="bank {bankChangeDirection}">
                    ${formatNumberWithSpaces(Math.round($bank))}
                </span>
            </div>
        </div>
    </div>
</div>
