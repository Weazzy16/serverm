<script>
    import { translateText } from 'lang'
    import { accountUnique } from 'store/account'
    import { format } from 'api/formatter'
    export let SetView;
    export let SetPopup;


    let ItemList = {
        packages: false,
        cases: false
    };


    executeClientAsync("donate.getPack").then((result) => {
        if (result && typeof result === "string") {
            ItemList.packages = JSON.parse(result);
            getUnique ();
        }
    });

    executeClientAsync("donate.roulette.getList").then((result) => {
        if (result && typeof result === "string") {
            ItemList.cases = JSON.parse(result).caseData;
            getUnique ();
        }
    });

    let selectListId = {};
    let isBuy = false;

    const onSelectMenu = () => {
        getUnique ();
        if (isBuy)
            return window.notificationAdd(4, 9, `You have already used this proposal`, 3000);
        let getList = $accountUnique.split("_")[0];
        if (getList === "cases") SetView("Cases")
        else SetPopup ("PopupDpPopup", selectListId);
    }

    import {executeClientAsync} from "api/rage";

    const getUnique = () => {
        if ($accountUnique) {
            let getData = $accountUnique.split("_");
            if (ItemList[getData[0]][getData[1]]) {
                selectListId = ItemList[getData[0]][getData[1]];
                isBuy = Number (getData[2]);
            } else
                isBuy = true;
        } else
            isBuy = true; 
    }

</script>

{#if ItemList.cases && ItemList.packages}
<div class="righttop">
    <div class="left">
        <div class="bgiconday">
            <svg xmlns="http://www.w3.org/2000/svg" width="271" height="291" viewBox="0 0 271 291" fill="none">
                <g filter="url(#filter0_d_1_5980)">
                    <path d="M133.115 68.3772C134.591 67.525 136.409 67.525 137.885 68.3772L201.098 104.873C202.574 105.725 203.483 107.3 203.483 109.004V181.996C203.483 183.7 202.574 185.275 201.098 186.127L137.885 222.623C136.409 223.475 134.591 223.475 133.115 222.623L69.9024 186.127C68.4263 185.275 67.517 183.7 67.517 181.996V109.004C67.517 107.3 68.4263 105.725 69.9024 104.873L133.115 68.3772Z" fill="url(#paint0_radial_1_5980)" fill-opacity="0.05"></path>
                    <path d="M133.365 68.8102C134.686 68.0473 136.314 68.0473 137.635 68.8102L200.848 105.306C202.169 106.069 202.983 107.479 202.983 109.004V181.996C202.983 183.521 202.169 184.931 200.848 185.694L137.635 222.19C136.314 222.953 134.686 222.953 133.365 222.19L70.1524 185.694C68.831 184.931 68.017 183.521 68.017 181.996V109.004C68.017 107.479 68.831 106.069 70.1524 105.306L133.365 68.8102Z" stroke="#C92A2A"></path>
                </g>
                <defs>
                    <filter id="filter0_d_1_5980" x="0.72728" y="0.948227" width="269.545" height="289.104" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                        <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                        <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                        <feOffset></feOffset>
                        <feGaussianBlur stdDeviation="33.3949"></feGaussianBlur>
                        <feColorMatrix type="matrix" values="0 0 0 0 0.788235 0 0 0 0 0.164706 0 0 0 0 0.164706 0 0 0 0.25 0"></feColorMatrix>
                        <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1_5980"></feBlend>
                        <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_1_5980" result="shape"></feBlend>
                    </filter>
                    <radialGradient id="paint0_radial_1_5980" cx="0" cy="0" r="1" gradientUnits="userSpaceOnUse" gradientTransform="translate(135.5 145.5) rotate(90) scale(78.5)">
                        <stop stop-color="#C92A2A" stop-opacity="0"></stop>
                        <stop offset="1" stop-color="#C92A2A"></stop>
                    </radialGradient>
                </defs>
            </svg>
            <img class="iconday" src="https://imgur.com/A9oX3AU.png" alt="">
        </div>
    </div>
    <div class="right">
        <div class="headday">
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="head">
                <p>Предложения дня (-30%)</p>
            </div>
            <div class="lineh"></div>
            <div class="lineh"></div>
            <div class="lineh"></div>
        </div>
        <div class="infoday">
            <div class="container" on:keypress={() => {}} on:click={onSelectMenu}>
                <div class="bgitem">
                    {#if $accountUnique.split("_")[0] === "cases"}
                        <img class="item" src="{document.cloud + `img/roulette/${selectListId.image}.png`}" alt="">
                    {:else}
                        <img class="item" src="{document.cloud + `donate/personal/${selectListId.id + 1}.png`}" alt="">
                    {/if}
                </div>
                <p>{selectListId.name}</p>
            </div>
        </div>
    </div>
</div>
{/if}