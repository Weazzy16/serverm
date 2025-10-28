<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { serverDonatMultiplier } from 'store/server'
    import { validate } from 'api/validation';
    let selectIndex = 0;

    export let SetPopup;
    const getDonate = (text) => {
        if (text < 0) text = 0;
        else if (text > 999999) text = 999999;
        let tallage = 0;
        if ($serverDonatMultiplier > 1) {
            text = text * $serverDonatMultiplier;
        } else {
            if (text >= 20000) {
                tallage = 50;
            } else if (text >= 10000) {
                tallage = 30;
            } else if (text >= 3000) {
                tallage = 20;
            } else if (text >= 1000) {
                tallage = 10;
            }
        }

        return `${Math.round(text) + Math.round(text / 100 * tallage)}`;
    }

    import ImgMenuTop_up from './img/top_up.png';
    import ImgMenuChar from './img/char.png';

    import ImgBuy from './img/buy.svg';
    import ImgConversion from './img/conversion.svg';
    import ImgSapper from './img/sapper.png';

    import ImgP_1 from './img/p_1.svg';
    import ImgP_2 from './img/p_2.svg';
    import ImgP_3 from './img/p_3.svg';
    import SimPhoto from './img/sim.png';
    import NumberPhoto from './img/number.png';
    
    /*import ImgClM_1 from './img/clothes/male/1.png';
    import ImgClM_2 from './img/clothes/male/2.png';
    import ImgClM_3 from './img/clothes/male/3.png';
    import ImgClM_4 from './img/clothes/male/4.png';
    import ImgClM_5 from './img/clothes/male/5.png';
    import ImgClM_6 from './img/clothes/male/6.png';
    import ImgClM_7 from './img/clothes/male/7.png';
    import ImgClM_8 from './img/clothes/male/8.png';
    import ImgClM_9 from './img/clothes/male/9.png';
    import ImgClM_10 from './img/clothes/male/10.png';
    import ImgClM_11 from './img/clothes/male/11.png';
    import ImgClM_12 from './img/clothes/male/12.png';
    import ImgClM_13 from './img/clothes/male/13.png';
    import ImgClM_14 from './img/clothes/male/14.png';
    import ImgClM_15 from './img/clothes/male/15.png';
    import ImgClM_16 from './img/clothes/male/16.png';
    import ImgClM_17 from './img/clothes/male/17.png';
    import ImgClM_18 from './img/clothes/male/18.png';
    import ImgClM_19 from './img/clothes/male/19.png';
    import ImgClM_20 from './img/clothes/male/20.png';
    import ImgClM_21 from './img/clothes/male/21.png';
    import ImgClM_22 from './img/clothes/male/22.png';
    import ImgClM_23 from './img/clothes/male/23.png';

    import ImgClF_1 from './img/clothes/female/1.png';
    import ImgClF_2 from './img/clothes/female/2.png';
    import ImgClF_3 from './img/clothes/female/3.png';
    import ImgClF_4 from './img/clothes/female/4.png';
    import ImgClF_5 from './img/clothes/female/5.png';
    import ImgClF_6 from './img/clothes/female/6.png';
    import ImgClF_7 from './img/clothes/female/7.png';
    import ImgClF_8 from './img/clothes/female/8.png';
    import ImgClF_9 from './img/clothes/female/9.png';
    import ImgClF_10 from './img/clothes/female/10.png';
    import ImgClF_17 from './img/clothes/female/17.png';
    import ImgClF_18 from './img/clothes/female/18.png';
    import ImgClF_19 from './img/clothes/female/19.png';
    import ImgClF_20 from './img/clothes/female/20.png';
    import ImgClF_21 from './img/clothes/female/21.png';
    import ImgClF_22 from './img/clothes/female/22.png';
    import ImgClF_23 from './img/clothes/female/23.png';*/

    const shopList = [
        {
            title: "Главное",
            desc: "",
            function: "onSelectPrice",
            img: ImgBuy,
            list: [
                /*{
                    name: "Пополнить",
                    desc: "",
                    btnName: "Купить",
                    img: ImgBuy,
                },*/
                {
                    name: "Конвектировать",
                    btnName: "Convert",
                    desc: "",
                    img: ImgConversion,
                },
                {
                    name: "Сапер",
                    btnName: "Play",
                    desc: "",
                    img: ImgSapper,
                },
            ]
        },
        {
            title: "Персонаж",
            desc: "",
            function: "onSelectP",
            img: ImgMenuChar,
            list: [
                {
                    id: 0,
                    isName: true,
                    name: "Измените имя",
                    desc: "",
                    text: `Позволяет вам изменить имя одного человека один раз
Вашего персонажа.
После смены имени персонаж забудет о сделанных ранее рукопожатиях,
при этом инвентарь и статистика персонажа никак не изменятся. Только псевдоним.`,
                    img: ImgP_1,
                    price: 800,
                },
                {
                    id: 1,
                    name: "Изменить внешний вид",
                    desc: "",
                    text: `После оплаты этой функции,
Ваш персонаж будет отправлен в редактор внешнего вида (как при создании персонажа),
где вы сможете заново настроить его внешность.
Содержимое инвентаря и татуировки останутся.`,
                    img: ImgP_2,
                    price: 1000,
                },
                {
                    id: 2,
                    name: "Удаление варна",
                    desc: "",
                    text: `Warn - предупреждение от администрации. Эта функция пожертвования удаляет только 1 предупреждение.
Если у вас есть 2 предупреждения, то для полного вывода средств вам нужно будет дважды заплатить за "Снятие ПРЕДУПРЕЖДЕНИЯ".
Если вы наберете 3 предупреждения одновременно, персонаж будет автоматически заблокирован на 30 дней.`,
                    img: ImgP_3,
                    price: 1000,
                },
                {
                    id: 3,
                    isNumber: true,
                    name: "Приобрести номер",
                    desc: "",
                    text: `Долгожданная система покупки номерных знаков для автомобиля!
Соберите свой уникальный номер из букв и/или цифр и пусть все вокруг завидуют...
Номер на автомобиле выдается как элемент инвентаря.`,
                    img: NumberPhoto,
                    btnName: "Buy"
                },
                {
                    id: 4,
                    isSim: true,
                    name: "Приобрести SIM-карту",
                    desc: "",
                    text: `Долгожданная система покупки телефонных номеров!
Соберите свой уникальный номер из любых номеров и пусть все вокруг завидуют...
Номер на автомобиле выдается как элемент инвентаря.`,
                    img: SimPhoto,
                    btnName: "Buy"
                },
            ]
        },
        /*{
            title: "Мужская одежда",
            desc: "",
            function: "onSelectC",
            img: ImgClM_11,
            list: [
                {
                    id: 0,
                    name: "Борода",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_2,
                    price: 30000,
                },
                {
                    id: 1,
                    name: "Маска крысы",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_17,
                    price: 1000,
                },
                {
                    id: 2,
                    name: "Неоновая маска",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_18,
                    price: 2000,
                },
                {
                    id: 3,
                    name: "Маска Marshmallow",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_5,
                    price: 35000,
                },
                {
                    id: 4,
                    name: "Неоновые очки",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_19,
                    price: 10000,
                },
                {
                    id: 5,
                    name: "Неоновый шлем",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_20,
                    price: 30000,
                },
                {
                    id: 6,
                    name: "Фуражка адмирала",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_21,
                    price: 1500,
                },
                {
                    id: 7,
                    name: "Модная повязка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_6,
                    price: 4200,
                },
                {
                    id: 8,
                    name: "Неоновые штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_23,
                    price: 25000,
                },
                {
                    id: 9,
                    name: "Яркие штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_13,
                    price: 20000,
                },
                {
                    id: 10,
                    name: "Штаны с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_16,
                    price: 20000,
                },
                {
                    id: 11,
                    name: "Шорты Supreme",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_1,
                    price: 5000,
                },
                {
                    id: 12,
                    name: "Штаны GUCCI",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_12,
                    price: 30000,
                },
                {
                    id: 13,
                    name: "Неоновая обувь",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_22,
                    price: 25000,
                },
                {
                    id: 14,
                    name: "Ласты",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_4,
                    price: 3000,
                },
                {
                    id: 15,
                    name: "Неоновый верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_10,
                    price: 15000,
                },
                {
                    id: 16,
                    name: "Яркий верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_14,
                    price: 10000,
                },
                {
                    id: 17,
                    name: "Верх с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_15,
                    price: 10000,
                },
                {
                    id: 18,
                    name: "Толстовка GUCCI",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_11,
                    price: 30000,
                },
                {
                    id: 19,
                    name: "Модная футболка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_7,
                    price: 5000,
                },
            ]
        },
        {
            title: "Женская одежда",
            desc: "",
            function: "onSelectC",
            img: ImgClF_1,
            list: [
                {
                    id: 20,
                    name: "Маска крысы",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_17,
                    price: 1000,
                },
                {
                    id: 21,
                    name: "Неоновая маска",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_18,
                    price: 2000,
                },
                {
                    id: 22,
                    name: "Маска Marshmallow",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_7,
                    price: 35000,
                },
                {
                    id: 23,
                    name: "Неоновые очки",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_19,
                    price: 10000,
                },
                {
                    id: 24,
                    name: "Неоновый шлем",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_20,
                    price: 30000,
                },
                {
                    id: 25,
                    name: "Фуражка адмирала",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_21,
                    price: 1500,
                },
                {
                    id: 26,
                    name: "Неоновые штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_2,
                    price: 25000,
                },
                {
                    id: 27,
                    name: "Яркие штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_5,
                    price: 20000,
                },
                {
                    id: 28,
                    name: "Штаны с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_10,
                    price: 20000,
                },
                {
                    id: 29,
                    name: "Неоновая обувь",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_22,
                    price: 10000,
                },
                {
                    id: 30,
                    name: "Ласты",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_8,
                    price: 3000,
                },
                {
                    id: 31,
                    name: "Неоновый верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_3,
                    price: 15000,
                },
                {
                    id: 32,
                    name: "Яркий верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_6,
                    price: 10000,
                },
                {
                    id: 33,
                    name: "Верх с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_9,
                    price: 10000,
                },
                {
                    id: 34,
                    name: "Модная футболка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_1,
                    price: 2000,
                },
                {
                    id: 35,
                    name: "Пошлая футболка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_4,
                    price: 4000,
                },
            ]
        }*/
    ];

    let
        FirstName = "",
        LastName = "";
    const onToServer = (item) => {
        switch (shopList[selectIndex].function) {
            case "onSelectPrice":
                if (item.btnName === "Buy") SetPopup ("PopupPayment", 0);
                else if (item.btnName === "Convert") SetPopup ("PopupChange");
                else window.router.setView('DonateSapper');
                break;
            case "onSelectP":
                if (item.isNumber) {
                    SetPopup ("PopupNomer");
                    return;
                }
                if (item.isSim) {
                    SetPopup ("PopupSim");
                    return;
                }

                if (item.isName) {
                    let check;

                    check = validate("name", FirstName);
                    if(!check.valid) {
                        window.notificationAdd(4, 9, check.text, 3000);
                        return;
                    }

                    check = validate("surname", LastName);
                    if(!check.valid) {
                        window.notificationAdd(4, 9, check.text, 3000);
                        return;
                    }
                    item.isName = `${FirstName}_${LastName}`
                    item.text = `Вы действительно хотите сменить ник на ${item.isName}`
                }
                SetPopup ("PopupPPopup", item);
                break;
            case "onSelectC":
                SetPopup ("PopupCPopup", item);
                break;
        }
    }
    const onSelectPrice = (item) => {
        if (!item.price) SetPopup ("PopupPayment", 0);
        else SetPopup ("PopupPayment", item.priceReal);
    }
</script>


<div class="donathmenu">
    <div class="hmenutop" style="justify-content: space-evenly;">
        {#each shopList as item, index}     
            <div class="btncat" class:active={selectIndex === index} on:keypress={() => {}} on:click={() => selectIndex = index}><p>{item.title}</p></div>   
        {/each}
    </div>
    <div class="hmenulist">
        {#each shopList[selectIndex].list as item, index}
            <div class="block" on:keypress={() => {}} on:click={() => onToServer (item)}>
                {#if item.btnName}
                        <div class="price" style="display: none;">
                            <p>{item.btnName}</p>
                        </div>
                    {:else}
                        <div class="price">
                            <p>{format("money", item.price)} OC</p>
                        </div>
                {/if}
                <div class="bgitem">
                    <svg xmlns="http://www.w3.org/2000/svg" width="271" height="291" viewBox="0 0 271 291" fill="none">
                        <g filter="url(#filter0_d_1_5786)">
                            <path d="M133.115 68.3772C134.591 67.525 136.409 67.525 137.885 68.3772L201.098 104.873C202.574 105.725 203.483 107.3 203.483 109.004V181.996C203.483 183.7 202.574 185.275 201.098 186.127L137.885 222.623C136.409 223.475 134.591 223.475 133.115 222.623L69.9024 186.127C68.4263 185.275 67.517 183.7 67.517 181.996V109.004C67.517 107.3 68.4263 105.725 69.9024 104.873L133.115 68.3772Z" fill="url(#paint0_radial_1_5786)" fill-opacity="0.05"></path>
                            <path d="M133.365 68.8102C134.686 68.0473 136.314 68.0473 137.635 68.8102L200.848 105.306C202.169 106.069 202.983 107.479 202.983 109.004V181.996C202.983 183.521 202.169 184.931 200.848 185.694L137.635 222.19C136.314 222.953 134.686 222.953 133.365 222.19L70.1524 185.694C68.831 184.931 68.017 183.521 68.017 181.996V109.004C68.017 107.479 68.831 106.069 70.1524 105.306L133.365 68.8102Z" stroke="white"></path>
                        </g>
                        <defs>
                            <filter id="filter0_d_1_5786" x="0.72728" y="0.948227" width="269.545" height="289.104" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                                <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                                <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                                <feOffset></feOffset>
                                <feGaussianBlur stdDeviation="33.3949"></feGaussianBlur>
                                <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.25 0"></feColorMatrix>
                                <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1_5786"></feBlend>
                                <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_1_5786" result="shape"></feBlend>
                            </filter>
                            <radialGradient id="paint0_radial_1_5786" cx="0" cy="0" r="1" gradientUnits="userSpaceOnUse" gradientTransform="translate(135.5 145.5) rotate(90) scale(78.5)">
                                <stop stop-color="white" stop-opacity="0"></stop>
                                <stop offset="0.973958" stop-color="white" stop-opacity="0.973958"></stop>
                                <stop offset="1" stop-color="#FF9345"></stop>
                            </radialGradient>
                        </defs>
                    </svg>
                    {#if item.icon}
                        <img class="item" src="{item.icon}" alt="">
                    {:else}
                        <img class="item" src="{item.img}" alt="">
                    {/if}
                </div>
                {#if item.isName}
                    <div class="inputblocks">
                        <input placeholder="Имя" type="text" bind:value={FirstName} >
                        <input placeholder="Фамилия" type="text" bind:value={LastName}>
                    </div>
                {:else}
                    <div class="inputblocks">
                    </div>
                {/if}
                <div class="headblock">
                    <div class="lineh"></div>
                    <div class="lineh"></div>
                    <div class="lineh"></div>
                    <div class="head">
                        <p>{item.name}</p>
                    </div>
                    <div class="lineh"></div>
                    <div class="lineh"></div>
                    <div class="lineh"></div>
                </div>
            </div>
        {/each}  
    </div>
</div>