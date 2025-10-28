<script>
    import { executeClient, invokeMethod } from "api/rage";
    import { format } from "api/formatter";
    import "./style.scss";

    export let viewData;

    let data = {
        Address: "Рокфорд",
        Owner: "Государство",
        Name: "АЗС",
        Price: 3000000,
        Mafia: "Нет",
        Id: 10,
    }

    $: if (viewData && typeof viewData === "string") {
        viewData = JSON.parse (viewData)

        data = viewData
    }

    const getPicture = (id) => {
        // return `https://cdn.majestic-files.com/img/playermenu/property/clothes_shops/2.jpg`;
        return document.cloud + `property/business/${id}.jpg`;
        // return `https:cdn.astrarp.fun/cloud/property/business/${id}.jpg`;
    }

    const close = () => {
        executeClient("client.businessInfo.close");
    }

    const buy = () => {
        invokeMethod("command", "buybiz");
    }
</script>

<div>
    <div class="businessInfo column-block">
        <div class="object-picture" 
            style="background-image: url({getPicture(data.Id)});">
        </div>

        <div class="buy-body">
            <div class="container-box">
                <div class="row-block">
                    <div class="big-container column-block">
                        <span class="big-container__subtitle">{ data.Address }</span>
                        <span class="big-container__title">{ data.Name } #{ data.Id }</span>
                    </div>
                    <div class="big-container column-block align-end">
                        <span class="big-container__subtitle">Владелец:</span>
                        <span class="big-container__title">{ data.Owner }</span>
                    </div>
                </div>

                <div class="business-info row-block">
                    <div class="big-container column-block">
                        <span class="big-container__subtitle">Цена:</span>
                        <span class="big-container__title">${ format("money", data.Price) }</span>
                    </div>
                    <div class="big-container column-block align-end">
                        <span class="big-container__subtitle">Контролирует:</span>
                        <span class="big-container__title">{ data.Mafia }</span>
                    </div>
                </div>

                {#if data.Owner == 'Государство' }
                    <div class="buy-body__buttons">
                        <div class="line">
                            <button on:click={() => buy()} class="property-button">
                                Купить
                            </button>

                            <button on:click={() => close()} class="property-button">
                                Отмена
                            </button>
                        </div>
                    </div>
                {/if}
            </div>
        </div>

        <button on:click={() => close()} class="close-block">
            <svg
               
                xmlns="http://www.w3.org/2000/svg"
                width="1.0568518518518517vh"
                height="1.0569444444444442vh"
                viewBox="0 0 11.414 11.415"
            >
                <path
                    id="Объединение_10"
                    data-name="Объединение 10"
                    d="M5785-1262l-5,5,5-5-5-5,5,5,5-5-5,5,5,5Z"
                    transform="translate(-5779.293 1267.708)"
                    fill="none"
                    stroke="#111"
                    stroke-width="2"
                ></path>
            </svg>
        </button>
    </div>
</div>
