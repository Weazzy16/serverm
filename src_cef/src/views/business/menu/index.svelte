<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import './css/main.sass'
    import './css/main.css'
    import './fonts/Gilroy/stylesheet.css';
    import './fonts/SFPro/stylesheet.css';
    //import './fonts/style.css'
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng } from './getPng.js' 



    let
        title = '',
        titleIcon = '',
        btn = '',
        elements = [],
        type = 0;

    window.smOpen = (_title, _titleIcon, _btn, _json, _type = 0) => {
        title = _title;
        titleIcon = _titleIcon;
        btn = _btn;
        elements = JSON.parse (_json);
        type = _type;
    }

    const configImages = [
        { name: 'Сим-карта', url: 'inventoryItems/items/sm-icon-sim.png' },
        { name: 'Рабочий топор', url: 'inventoryItems/items/244.png' },
        { name: 'Обычная кирка', url: 'inventoryItems/items/234.png' },
        { name: 'Усиленная кирка', url: 'inventoryItems/items/235.png' },
        { name: 'Профессиональная кирка', url: 'inventoryItems/items/236.png' },
        { name: 'Сумка', url: 'inventoryItems/clothes/male/bags/40_0.png' },
        { name: 'Сумка с дрелью', url: 'inventoryItems/items/15.png' },
        { name: 'Стяжки', url: 'inventoryItems/items/18.png' },
        { name: 'Мешок', url: 'inventoryItems/items/17.png' },
        { name: 'Бронежилет', url: 'inventoryItems/items/-9.png' },
        { name: 'Отмычка для замков', url: 'inventoryItems/items/11.png' },
        { name: 'Военная отмычка', url: 'inventoryItems/items/16-war.png' },
        { name: 'Радиоперехватчик', url: 'inventoryItems/items/279.png' },
        { name: 'QR-код', url: 'inventoryItems/items/270.png' },
        { name: 'Услуга по отмыву денег', url: 'inventoryItems/items/otmyv.png' },
        { name: 'Понизить розыск', url: 'inventoryItems/items/rozysk.png' },
        { name: 'Взломать наручники', url: 'inventoryItems/items/naruchniki.png' },
        { name: 'Мед. карта', url: 'inventoryItems/items/med.png' },
        { name: 'Лотерейный билет', url: 'inventoryItems/items/lottery.png' },
        { name: 'Лицензия на оружие', url: 'inventoryItems/items/litsenzia.png' },
        { name: 'Угон автотранспорта', url: 'inventoryItems/items/ugon.png' },
        { name: 'Перевозка автотранспорта', url: 'inventoryItems/items/perevozkaa.png' },
        { name: 'Перевозка оружия', url: 'inventoryItems/items/perevozkaw.png' },
        { name: 'Перевозка денег', url: 'inventoryItems/items/perevozkam.png' },
        { name: 'Перевозка трупов', url: 'inventoryItems/items/perevozkat.png' },
        { name: 'Сдать бронежилет', url: 'inventoryItems/items/-9.png' },
        { name: 'Дубинка', url: 'inventoryItems/items/181.png' },
        { name: 'Stun Gun', url: 'inventoryItems/items/109.png' },
        { name: 'Combat Pistol', url: 'inventoryItems/items/101.png' },
        { name: 'Heavy Pistol', url: 'inventoryItems/items/104.png' },
        { name: 'Pistol Mk2', url: 'inventoryItems/items/112.png' },
        { name: 'Pistol 50', url: 'inventoryItems/items/102.png' },
        { name: 'Ceramic Pistol', url: 'inventoryItems/items/151.png' },
        { name: 'Pump Shotgun Mk2', url: 'inventoryItems/items/149.png' },
        { name: 'Carbine Rifle Mk2', url: 'inventoryItems/items/133.png' },
        { name: 'Special Carbine', url: 'inventoryItems/items/129.png' },
        { name: 'Special Carbine Mk2', url: 'inventoryItems/items/134.png' },
        { name: 'SMG', url: 'inventoryItems/items/117.png' },
        { name: 'Bullpup Shotgun', url: 'inventoryItems/items/143.png' },
        { name: 'SawnOff Shotgun', url: 'inventoryItems/items/142.png' },
        { name: 'Heavy Shotgun', url: 'inventoryItems/items/146.png' },
        { name: 'Carbine Rifle', url: 'inventoryItems/items/127.png' },
        { name: 'Sniper Rifle', url: 'inventoryItems/items/136.png' },
        { name: 'Combat PDW', url: 'inventoryItems/items/119.png' },
        { name: 'Combat MG', url: 'inventoryItems/items/121.png' },
        { name: 'Combat MG Mk2', url: 'inventoryItems/items/125.png' },
        { name: 'Bullpup Rifle', url: 'inventoryItems/items/130.png' },
        { name: 'Аптечка', url: 'inventoryItems/items/1.png' },
        { name: 'Дробь', url: 'inventoryItems/items/204.png' },
        { name: 'Малый калибр', url: 'inventoryItems/items/201.png' },
        { name: 'Автоматный калибр', url: 'inventoryItems/items/202.png' },
        { name: 'Снайперский калибр', url: 'inventoryItems/items/203.png' },
        { name: 'Пистолетный калибр', url: 'inventoryItems/items/200.png' },
        { name: 'AP Pistol', url: 'inventoryItems/items/108.png' },
        { name: 'Assault SMG', url: 'inventoryItems/items/118.png' },
        { name: 'Sweeper Shotgun', url: 'inventoryItems/items/148.png' },
        { name: 'Assault Shotgun', url: 'inventoryItems/items/144.png' },
        { name: 'Advanced Rifle', url: 'inventoryItems/items/128.png' },
        { name: 'Bullpup Rifle Mk2', url: 'inventoryItems/items/135.png' },
        { name: 'Heavy Sniper', url: 'inventoryItems/items/137.png' },
        { name: 'Marksman Rifle Mk2', url: 'inventoryItems/items/140.png' },
        { name: 'Бейдж', url: 'inventoryItems/items/-7.png' },
        { name: 'Фейерверк обычный', url: 'inventoryItems/items/216.png' },
        { name: 'Фейерверк звезда', url: 'inventoryItems/items/217.png' },
        { name: 'Фейерверк взрывной', url: 'inventoryItems/items/218.png' },
        { name: 'Фейерверк фонтан', url: 'inventoryItems/items/219.png' },
        { name: 'Материалы', url: 'inventoryItems/items/13.png' },
        { name: 'Наркотики', url: 'inventoryItems/items/14.png' },
        { name: 'Эксклюзивный кейс', url: 'inventoryItems/items/281.png' },
        { name: 'Стул', url: 'inventoryItems/items/313.png' },
        { name: 'Конус', url: 'inventoryItems/items/371.png' },
        { name: 'Светящийся конус', url: 'inventoryItems/items/372.png' },
        { name: 'Отбойник', url: 'inventoryItems/items/373.png' },
        { name: 'Отбойник', url: 'inventoryItems/items/374.png' },
        { name: 'Перекрытие', url: 'inventoryItems/items/375.png' },
        { name: 'Знак STOP', url: 'inventoryItems/items/376.png' },
        { name: 'Знак НЕТ ПРОЕЗДА', url: 'inventoryItems/items/377.png' },
        { name: 'КПП', url: 'inventoryItems/items/378.png' },
        { name: 'Большой забор', url: 'inventoryItems/items/379.png' },
        { name: 'Маленький забор', url: 'inventoryItems/items/380.png' },
        { name: 'Ночной свет', url: 'inventoryItems/items/381.png' },
        { name: 'Камера видеонаблюдения', url: 'inventoryItems/items/382.png' },
        { name: 'Камера видеонаблюдения', url: 'inventoryItems/items/383.png' },


    ];

    const getOtherImageUrl = (name) => {
        let ind = configImages.findIndex(x => x.name === name);
        let url = document.cloud + configImages[ind].url;
        return url;
    }

    const getTypeName = (type) => {
        if(!type) return `Buy`;
        else if(type == 1) return `Take`;
        else return `Pass`;
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;
        executeClient ('client.sm.exit')
    }
</script>
<svelte:window on:keyup={HandleKeyDown} />

{#if title == "Магазин" }
    <div class="shoprd">
        <div class="leftsh">
            <div class="headsh">
                <div class="leftsho">
                    <img src="http://cdn.piecerp.ru/src/views/business/menu/img/shop.png" alt="">
                </div>
                <div class="rightsho">
                    <h1>SUPERMARKET 24/7</h1>
                    <p>Добро пожаловать в супермаркет. Здесь есть все, что можно съесть и выпить.</p>
                </div>
            </div>
            <div class="imgshop">
                <img src="http://cdn.piecerp.ru/src/views/business/menu/img/Persone.png" alt="">
            </div>
        </div>
        <div class="rightsh">
            <div class="listsh">
                {#each elements as value, index}
                    <div class="blockitem" id={value.id} key={index}>
                        <span>{value.Price.replace(/[0-9]+/,'')}{value.Price.replace(/[^\d]+/g,'')}</span>
                        <svg width="44" height="41" viewBox="0 0 44 41" fill="none" xmlns="http://www.w3.org/2000/svg"  on:keypress={() => {}} on:click={() => executeClient ('client.sm.click', value.Id)}>
                            <g filter="url(#filter0_d_757_467)">
                                <path d="M9 32.5H35.5" stroke="white" stroke-linecap="round"></path>
                            </g>
                            <g clip-path="url(#clip0_757_467)">
                                <g filter="url(#filter1_d_757_467)">
                                    <path d="M30.125 11.25H26.2938L23.7938 6.28122C23.7124 6.146 23.5829 6.04657 23.4312 6.00288C23.2796 5.9592 23.117 5.97449 22.9762 6.0457C22.8354 6.11691 22.7267 6.23875 22.672 6.38678C22.6173 6.5348 22.6206 6.69804 22.6812 6.84372L24.8937 11.25H19.1063L21.3062 6.87497C21.3506 6.80133 21.3792 6.71934 21.3904 6.63413C21.4016 6.54891 21.3951 6.4623 21.3713 6.37972C21.3475 6.29713 21.3069 6.22034 21.2521 6.15414C21.1973 6.08794 21.1294 6.03376 21.0527 5.99498C20.976 5.9562 20.8922 5.93365 20.8064 5.92875C20.7206 5.92385 20.6347 5.9367 20.554 5.9665C20.4734 5.99629 20.3998 6.0424 20.3378 6.10193C20.2758 6.16146 20.2268 6.23313 20.1938 6.31247L17.6938 11.3125H13.875C13.3777 11.3125 12.9008 11.51 12.5492 11.8616C12.1975 12.2133 12 12.6902 12 13.1875V13.4687C12.0162 13.8805 12.1912 14.27 12.4883 14.5555C12.7855 14.841 13.1817 15.0003 13.5938 15L15.8187 22.0062C15.9793 22.512 16.2965 22.9535 16.7246 23.267C17.1527 23.5805 17.6694 23.7496 18.2 23.75H25.8C26.3306 23.7496 26.8473 23.5805 27.2754 23.267C27.7035 22.9535 28.0207 22.512 28.1812 22.0062L30.4062 15C30.8289 15 31.2343 14.8321 31.5332 14.5332C31.8321 14.2343 32 13.8289 32 13.4062V13.125C32 12.6277 31.8025 12.1508 31.4508 11.7991C31.0992 11.4475 30.6223 11.25 30.125 11.25ZM30.75 13.4062C30.7508 13.4516 30.7425 13.4967 30.7256 13.5388C30.7086 13.5809 30.6833 13.6191 30.6512 13.6512C30.6191 13.6833 30.5809 13.7086 30.5388 13.7255C30.4967 13.7425 30.4516 13.7508 30.4062 13.75C30.1419 13.7506 29.8845 13.835 29.6711 13.9911C29.4578 14.1472 29.2994 14.3669 29.2188 14.6187L27 21.6312C26.9187 21.885 26.7584 22.1062 26.5426 22.2625C26.3267 22.4188 26.0665 22.502 25.8 22.5H18.2C17.9335 22.502 17.6733 22.4188 17.4574 22.2625C17.2416 22.1062 17.0813 21.885 17 21.6312L14.7812 14.6187C14.7006 14.3669 14.5422 14.1472 14.3289 13.9911C14.1155 13.835 13.8581 13.7506 13.5938 13.75C13.5484 13.7508 13.5033 13.7425 13.4612 13.7255C13.4191 13.7086 13.3809 13.6833 13.3488 13.6512C13.3167 13.6191 13.2914 13.5809 13.2744 13.5388C13.2575 13.4967 13.2492 13.4516 13.25 13.4062V13.125C13.25 12.9592 13.3158 12.8002 13.4331 12.683C13.5503 12.5658 13.7092 12.5 13.875 12.5H17.0812L16.6062 13.45C16.4715 13.6052 16.3924 13.801 16.3813 14.0062C16.3836 14.0165 16.3836 14.0272 16.3813 14.0375C16.3815 14.1893 16.4186 14.3388 16.4893 14.4731C16.5601 14.6074 16.6625 14.7226 16.7876 14.8086C16.9127 14.8946 17.0568 14.949 17.2076 14.967C17.3583 14.985 17.5112 14.9661 17.6531 14.912C17.7949 14.8578 17.9215 14.77 18.0219 14.6562C18.1223 14.5423 18.1936 14.4057 18.2296 14.2582C18.2655 14.1107 18.2651 13.9567 18.2284 13.8094C18.1917 13.662 18.1197 13.5258 18.0187 13.4125L18.4813 12.5H25.5187L25.9875 13.4437C25.8865 13.5571 25.8146 13.6933 25.7778 13.8406C25.7411 13.9879 25.7407 14.142 25.7767 14.2895C25.8127 14.437 25.8839 14.5735 25.9844 14.6874C26.0848 14.8013 26.2114 14.8891 26.3532 14.9432C26.495 14.9974 26.6479 15.0162 26.7987 14.9982C26.9494 14.9802 27.0936 14.9259 27.2187 14.8398C27.3438 14.7538 27.4461 14.6387 27.5169 14.5044C27.5877 14.3701 27.6248 14.2206 27.625 14.0687C27.6226 14.0584 27.6226 14.0477 27.625 14.0375C27.6139 13.8322 27.5347 13.6365 27.4 13.4812L26.9188 12.5H30.125C30.2908 12.5 30.4497 12.5658 30.5669 12.683C30.6842 12.8002 30.75 12.9592 30.75 13.125V13.4062Z" fill="white"></path>
                                </g>
                                <g filter="url(#filter2_d_757_467)">
                                    <path d="M22 15.625C21.8342 15.625 21.6753 15.6908 21.5581 15.8081C21.4408 15.9253 21.375 16.0842 21.375 16.25V20C21.375 20.1658 21.4408 20.3247 21.5581 20.4419C21.6753 20.5592 21.8342 20.625 22 20.625C22.1658 20.625 22.3247 20.5592 22.4419 20.4419C22.5592 20.3247 22.625 20.1658 22.625 20V16.25C22.625 16.0842 22.5592 15.9253 22.4419 15.8081C22.3247 15.6908 22.1658 15.625 22 15.625Z" fill="white"></path>
                                </g>
                                <g filter="url(#filter3_d_757_467)">
                                    <path d="M18.8746 16.0496C18.8215 15.8921 18.7081 15.7622 18.5593 15.6883C18.4104 15.6145 18.2383 15.6028 18.0808 15.6558C17.9234 15.7089 17.7934 15.8223 17.7196 15.9712C17.6457 16.12 17.634 16.2921 17.6871 16.4496L18.9371 20.1996C18.9772 20.3188 19.0523 20.4231 19.1527 20.499C19.253 20.5748 19.3739 20.6186 19.4996 20.6246C19.566 20.6331 19.6332 20.6331 19.6996 20.6246C19.7841 20.6022 19.8631 20.5624 19.9313 20.5076C19.9995 20.4528 20.0554 20.3843 20.0955 20.3066C20.1355 20.2288 20.1588 20.1435 20.1638 20.0562C20.1688 19.9688 20.1555 19.8814 20.1246 19.7996L18.8746 16.0496Z" fill="white"></path>
                                </g>
                                <g filter="url(#filter4_d_757_467)">
                                    <path d="M25.9505 15.6562C25.8725 15.6298 25.7901 15.619 25.7079 15.6244C25.6258 15.6299 25.5455 15.6516 25.4717 15.6882C25.398 15.7247 25.3322 15.7755 25.2781 15.8376C25.2241 15.8997 25.1828 15.9719 25.1567 16.05L23.9067 19.8C23.8753 19.8798 23.8607 19.9652 23.8637 20.0509C23.8667 20.1365 23.8873 20.2207 23.9242 20.2981C23.9612 20.3755 24.0136 20.4444 24.0783 20.5007C24.1431 20.5569 24.2187 20.5992 24.3005 20.625C24.3668 20.6336 24.4341 20.6336 24.5005 20.625C24.6315 20.6253 24.7593 20.5845 24.8658 20.5082C24.9724 20.4319 25.0523 20.3241 25.0942 20.2L26.3442 16.45C26.3707 16.372 26.3815 16.2896 26.376 16.2074C26.3705 16.1253 26.3489 16.045 26.3123 15.9713C26.2757 15.8975 26.2249 15.8317 26.1628 15.7777C26.1007 15.7236 26.0285 15.6823 25.9505 15.6562Z" fill="white"></path>
                                </g>
                            </g>
                            <defs>
                                <filter id="filter0_d_757_467" x="0.5" y="24" width="43.5" height="17" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                                    <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                                    <feOffset></feOffset>
                                    <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                                    <feComposite in2="hardAlpha" operator="out"></feComposite>
                                    <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.8 0"></feColorMatrix>
                                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_757_467"></feBlend>
                                    <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_757_467" result="shape"></feBlend>
                                </filter>
                                <filter id="filter1_d_757_467" x="4" y="-2.07227" width="36" height="33.8223" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                                    <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                                    <feOffset></feOffset>
                                    <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                                    <feComposite in2="hardAlpha" operator="out"></feComposite>
                                    <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.5 0"></feColorMatrix>
                                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_757_467"></feBlend>
                                    <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_757_467" result="shape"></feBlend>
                                </filter>
                                <filter id="filter2_d_757_467" x="13.375" y="7.625" width="17.25" height="21" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                                    <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                                    <feOffset></feOffset>
                                    <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                                    <feComposite in2="hardAlpha" operator="out"></feComposite>
                                    <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.5 0"></feColorMatrix>
                                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_757_467"></feBlend>
                                    <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_757_467" result="shape"></feBlend>
                                </filter>
                                <filter id="filter3_d_757_467" x="9.6543" y="7.62305" width="18.5107" height="21.0078" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                                    <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                                    <feOffset></feOffset>
                                    <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                                    <feComposite in2="hardAlpha" operator="out"></feComposite>
                                    <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.5 0"></feColorMatrix>
                                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_757_467"></feBlend>
                                    <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_757_467" result="shape"></feBlend>
                                </filter>
                                <filter id="filter4_d_757_467" x="15.8633" y="7.62305" width="18.5137" height="21.0088" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                                    <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                                    <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                                    <feOffset></feOffset>
                                    <feGaussianBlur stdDeviation="4"></feGaussianBlur>
                                    <feComposite in2="hardAlpha" operator="out"></feComposite>
                                    <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.5 0"></feColorMatrix>
                                    <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_757_467"></feBlend>
                                    <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_757_467" result="shape"></feBlend>
                                </filter>
                                <clipPath id="clip0_757_467">
                                    <rect width="30" height="30" fill="white" transform="translate(7)"></rect>
                                </clipPath>
                            </defs>
                        </svg>
                        {#if value.ItemId == 0 || value.ItemId == -5}
                                    <img alt="" src="{getOtherImageUrl(value.Name)}">
                                {:else}
                                    <img alt="" src="{getPng(value, itemsInfo[value.ItemId])}">
                        {/if}
                        <div class="itemline"></div>
                        <p>{@html value.Name}</p>
                    </div>
                {/each}
            </div>
        </div>
    </div>    
    <div class="closedhax">
        <p>Выйдите из магазина</p>
        <span>X</span>
    </div>
{/if}
{#if title != "Магазин" }
    <div id='shop'>
        <div class="box-ch">
            <div class="box-info">
                <div class="l">
                    <div class="title"><span class="i-title {titleIcon}" />{title}</div>
                </div>
                <div class="button-box">
                    <div class="btn red" on:keypress={() => {}} on:click={() => executeClient ('client.sm.exit')}>{translateText('business', 'Выйти')}</div>
                </div>
            </div>
            <div class="item-info">
    
                <ul class="items">
                    {#each elements as value, index}
                    <li id={value.id} key={index} class="block" on:keypress={() => {}} on:click={() => executeClient ('client.sm.click', value.Id)}>
                        <div class="box">
                            <div class="name">{@html value.Name}</div>
                        <!--<div class="name">{@html getPng(value, itemsInfo[value.ItemId])}</div>
                            <span class="item-img {value.Icon}" />  
                            <img src="{getPng(value, itemsInfo[value.ItemId])}">
                            <span class="item-img" style="background-image: url({getPng(value, itemsInfo[value.ItemId])})" />-->
    
                            {#if value.ItemId == 0}
                                <div class="item-img"><img alt="" src="{getOtherImageUrl(value.Name)}"></div>
                                {:else}
                                <div class="item-img"><img alt="" src="{getPng(value, itemsInfo[value.ItemId])}"></div>
                             {/if}
    
                            {#if value.Price}
                                <div class="price">
                                    {value.Price.replace(/[^\d]+/g,'')}
                                    <span class="green"> {value.Price.replace(/[0-9]+/,'')}</span>
                                </div>
                            {/if}
                        </div>
                        <div class="btn {btn}">{getTypeName(value.Name.match(/Pass/g) ? 2 : type)}</div>
                     </li>
                     {/each}
                 </ul>
    
            </div>
            
            
        </div>
    </div>
{/if}
