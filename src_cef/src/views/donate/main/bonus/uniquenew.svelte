<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage';
    
    let selectListId = {
        id: 0,
        name: "Зимний Пропуск",
        class: "reborn", //ХЗ ЧЕ ЭТО)))
        price: 7777,
        img: document.cloud + 'inventoryItems/items/bp.png',
        list: [
            "Более 300 уникальных заданий",
            "Огромное количество уникальной одежды",
            "Два уникальных автомобиля, которые уже ждут вас",
        ]
    };

    const onSelectMenu = () => {
        executeClient ("client.donate.close");
        executeClient ('client.battlepass.open');
    }


</script>


<div class="lefttop" on:keypress={() => {}} on:click={onSelectMenu}>
    <div class="left">
        <div class="bgiconseas">
            <svg width="271" height="291" viewBox="0 0 271 291" fill="none" xmlns="http://www.w3.org/2000/svg">
                <g filter="url(#filter0_d_1_5911)">
                    <path d="M133.115 68.3772C134.591 67.525 136.409 67.525 137.885 68.3772L201.098 104.873C202.574 105.725 203.483 107.3 203.483 109.004V181.996C203.483 183.7 202.574 185.275 201.098 186.127L137.885 222.623C136.409 223.475 134.591 223.475 133.115 222.623L69.9024 186.127C68.4263 185.275 67.517 183.7 67.517 181.996V109.004C67.517 107.3 68.4263 105.725 69.9024 104.873L133.115 68.3772Z" fill="url(#paint0_radial_1_5911)" fill-opacity="0.05"></path>
                    <path d="M133.365 68.8102C134.686 68.0473 136.314 68.0473 137.635 68.8102L200.848 105.306C202.169 106.069 202.983 107.479 202.983 109.004V181.996C202.983 183.521 202.169 184.931 200.848 185.694L137.635 222.19C136.314 222.953 134.686 222.953 133.365 222.19L70.1524 185.694C68.831 184.931 68.017 183.521 68.017 181.996V109.004C68.017 107.479 68.831 106.069 70.1524 105.306L133.365 68.8102Z" stroke="#0A4BFF"></path>
                </g>
                <defs>
                    <filter id="filter0_d_1_5911" x="0.72728" y="0.948227" width="269.545" height="289.104" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                        <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                        <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                        <feOffset></feOffset>
                        <feGaussianBlur stdDeviation="33.3949"></feGaussianBlur>
                        <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 0.576471 0 0 0 0 0.270588 0 0 0 0.25 0"></feColorMatrix>
                        <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1_5911"></feBlend>
                        <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_1_5911" result="shape"></feBlend>
                    </filter>
                    <radialGradient id="paint0_radial_1_5911" cx="0" cy="0" r="1" gradientUnits="userSpaceOnUse" gradientTransform="translate(135.5 145.5) rotate(90) scale(78.5)">
                        <stop stop-color="#0A4BFF" stop-opacity="0"></stop>
                        <stop offset="1" stop-color="#0A4BFF"></stop>
                    </radialGradient>
                </defs>
            </svg>
            <img class="iconseas" src="https://imgur.com/Qjrs2OY.png" alt="">
        </div>
    </div>
    <div class="right">
        <div class="headseas">
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="head">
                <p>{selectListId.name}</p>
            </div>
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="lineh"></div>
        </div>
        <div class="infoseas">
            {#if selectListId.list}
                {#each selectListId.list as text, index}
                    <p>{text}</p>
                {/each}
            {:else if selectListId.desc}
                <p>{selectListId.desc}</p>
            {/if}
        </div>
    </div>
</div>