
<script>
    import { format } from "api/formatter";
    import {accountRedbucks} from "store/account";
    import { charWanted, charMoney, charBankMoney } from 'store/chars'
    import {executeClient} from "api/rage";
    import {validate} from "api/validation";
    import {numberValue} from "@/views/player/hudevo/phonenew/components/calls/addContact.svelte";
    export let SetPopup;

    let number = "";

    const getPrice = (text) => {
        text = Number(text);

        const length = text.toString().length;
        if (length == 1)
            return [50000, " OC", "Luxury"];
        else if (length == 2)
            return [40000, " OC", "Luxury"];
        else if (length == 3)
            return [30000, " OC", "Luxury"];
        else if (length == 4)
            return [20000, " OC", "Rare"];
        else if (length == 5)
            return [15000, " OC", "Rare"];
        else if (length == 6)
            return [10000, " OC", "Unique"];
        else if (length == 7)
            return [7500, " OC", "Unique"];
        else if (length == 8)
            return [5000, " OC", "Unique"];
        else if (length == 9)
            return [2500, " OC", "Ordinary"];

        return [0, " OC", "Ordinary"];
    }

    let confirm = false;
    const onBuy = () => {
        const sim = Number (number);
        let check = validate("phonenumber", sim);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }
        
        const numberData = getPrice (sim);

        if (numberData[1] === " OC" && $accountRedbucks < numberData[0])
            return window.notificationAdd(4, 9, `Not enough Redbucks!`, 3000);

        if (!confirm)
            confirm = true;
        else {

            if (!window.loaderData.delay ("donate.onBuy", 1.5))
                return;


            executeClient ("client.donate.buySim", sim);
        }
    }
</script>


<div class="donathmenuiteminfo">
    <div class="blockinfo">
        <div class="closed"  on:keypress={() => {}} on:click={() => SetPopup (-1)}>
            <p>X</p>
        </div>
        <div class="headblock">
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="head">
                <p>Номер телефона</p>
            </div>
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="lineh"></div>
        </div>
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
            <img class="item" src="https://imgur.com/FtVZkw9.png" alt="">
        </div>
        <div class="iteminfo">
            <p>Число не должно начинаться с 0.</p>
            <p>Это <b>{getPrice (number)[2]}</b></p>
        </div>
        <div class="inputblocks">
            <input type="text" placeholder="420" bind:value={number} maxLength={9}>
        </div>
        <div class="buybtni" on:keypress={() => {}} on:click={onBuy}>
            <p>За {format("money", getPrice (number)[0])}{getPrice (number)[1]}</p>
        </div>
    </div>
</div>