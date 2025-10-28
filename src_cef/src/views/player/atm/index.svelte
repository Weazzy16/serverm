<script>
    import "./style.scss";
    import { svgStorage } from "./images/svg.js";
    import { fade, fly } from "svelte/transition";
    import { executeClient } from "api/rage";
    import { charBankMoney, charName } from "store/chars";

    export let viewData;

    const ACTION_BUTTONS = [
        {
            isServerRequired: false,
            isExit: false,
            path: "replenishment",
            img: "card",
            textKey: "Пополнить счет",
        },
        {
            isServerRequired: false,
            isExit: false,
            path: "transfer",
            img: "transfer",
            textKey: "Перевод средств",
        },
        {
            isServerRequired: false,
            isExit: false,
            path: "withdraw",
            img: "cash",
            textKey: "Снять наличные",
        },
        {
            isServerRequired: true,
            isExit: false,
            path: "house",
            img: "house",
            textKey: "Дом",
        },
        {
            isServerRequired: true,
            isExit: false,
            path: "business",
            img: "business",
            textKey: "Бизнес",
        }
    ];

    const ADDITIONAL_INPUTS = {
        transfer: [{
            isButtoned: true,
            id: "account",
            img: "character",
            value: "",
            maxlength: 8,
            placeholder: "Введите счёт"
        }],
        
        mobile: [{
            isButtoned: true,
            id: "mobile",
            img: "mobile",
            value: "",
            maxlength: 8,
            placeholder: "Введите номер"
        }]
    };

    const ATM_ACTION_TYPES = {
        NONE: "none",
        TRANSFER: "transfer",
        REPLACEMENT: "replenishment",
        WITHDRAW: "withdraw",
        HOUSE: "house",
        BUSINESS: "business",
        MOBILE: "mobile"
    }

    const TRANSITION_SETTINGS_IN = { duration: 150, delay: 350 };
    const TRANSITION_SETTINGS_OUT = { duration: 150 };

    let page = "main",
        theme = "light",

        atmId = 1,
        type = ATM_ACTION_TYPES.TRANSFER,
        subData = {};

    $: if (viewData && typeof viewData === "string") {
        viewData = JSON.parse (viewData)

        theme = viewData.theme
        atmId = viewData.id;
    }

    const closeAtm = () => {
        executeClient("client.atm.close");
    }

    const onClick_changePage = (pageName, subType = ATM_ACTION_TYPES.NONE) => {
        var actionButton = ACTION_BUTTONS.find(x => x.path == subType);
        if (actionButton != null) {
            if (actionButton.isServerRequired)
                return executeClient("client.atm.change_page", subType);

            if (actionButton.isExit)
                return closeAtm();
        }

        page = pageName;
        type = subType;
    };

    // @ts-ignore
    window.majestic_atm = {
        setTransaction(subType, data) {
            page = "transaction"
            
            subData = data;
            type = subType;
        },

        setPage(pageName) {
            page = pageName;
        }
    }

    const onClick_buyAtm = () => {
        window.notificationAdd(4, 9, "В данный момент недоступно для покупки", 3000);
    }

    const onClick_action = () => {
        var values = inputs.map(x => x.value);
        console.log(type, ...values);
        executeClient("client.atm.action", type, ...values);
    }

    let inputs = [],
        sumText = "",
        logoPath = "logo.svg";

    const genereateInputs = (currentType) => {
        let list = [];

        if (currentType === ATM_ACTION_TYPES.TRANSFER)
            list.push(...ADDITIONAL_INPUTS.transfer.map(x => {
                return { ...x} 
            }));

            
        if (currentType === ATM_ACTION_TYPES.MOBILE)
            list.push(...ADDITIONAL_INPUTS.mobile.map(x => {
                return { ...x} 
            }));

        list.push({
            isButtoned: true,
            id: "amount",
            img: "dollar",
            value: "",
            maxlength: 9,
            placeholder: "Введите сумму",
            // maxSum: 5e5 500.000
            maxSum: 5e7 // Эквивалент 50,000,000
        })

        return list;
    }

    $: logoPath = theme == "dark" ? "logo.svg" : "logo-light.svg";
    $: inputs = genereateInputs(type);
    $: sumText = (() => {
        switch(type) {
            case ATM_ACTION_TYPES.BUSINESS: 
            case ATM_ACTION_TYPES.HOUSE: 
            case ATM_ACTION_TYPES.MOBILE: 
            case ATM_ACTION_TYPES.REPLACEMENT: 
                return "Сумма пополнения";
            case ATM_ACTION_TYPES.WITHDRAW:
                return "Сумма снятия";
            case ATM_ACTION_TYPES.TRANSFER:
                return "Сумма перевода";
        }

        return "Итоговая сумма";
    })()
    $: headerText = (() => {
        switch(type) {
            case ATM_ACTION_TYPES.BUSINESS: 
            case ATM_ACTION_TYPES.HOUSE:  
            case ATM_ACTION_TYPES.REPLACEMENT: 
                return "Пополнить счет";
            case ATM_ACTION_TYPES.MOBILE:
                return "Пополнить счет мобильного"
            case ATM_ACTION_TYPES.WITHDRAW:
                return "Снять наличные";
            case ATM_ACTION_TYPES.TRANSFER:
                return "Перевод средств";
        }

        return "none";
    })()

    const getMaxSum = (item) => {
        if (item.maxSum == null)
            return null;

        if ([ATM_ACTION_TYPES.BUSINESS, ATM_ACTION_TYPES.HOUSE].includes(type)) {
            return (subData.MaxBalance - subData.Balance).toString();
        }
        else {
            return (type == ATM_ACTION_TYPES.WITHDRAW && $charBankMoney < Number(item.maxSum)) || (type != ATM_ACTION_TYPES.REPLACEMENT && $charBankMoney < Number(item.maxSum)) ? $charBankMoney : item.maxSum;
        }
    }
</script>

<div in:fly={{ y: 500, duration: 300 }} out:fly={{ y: 500, duration: 300 }} class={`atm ${theme}`}>
    <div class="header">
        <img src={ 'https://cdn.majestic-files.com/img/atm2/' + logoPath } alt="logo.svg" />

        <div class="header-data">
            <div class="header-data__user" style="display: flex;">
                <p>{ $charName }</p>
                <img src="https://cdn.majestic-files.com/img/avatars/male.svg" alt="avatar" />
            </div>
            <div class="header-data__close">
                <p>Выйти</p>
                <button on:click={() => closeAtm()} tabindex="-1" class="header-data__close-button">Esc</button>
            </div>
        </div>
    </div>

    <div class="atm-content">
        {#if page === "main"}
            <div in:fade={TRANSITION_SETTINGS_IN} out:fade={TRANSITION_SETTINGS_OUT} class="_wrapper main">
                <div class="content">
                    <div class="content-grid">
                        <div class="tile Standart">
                            <div class="background"></div>
                            <div class="tile-balance">
                                <p>Баланс счёта:</p>
                                <div class="tile-balance__main">
                                    {@html svgStorage["card"]}
                                    <p>${$charBankMoney}</p>
                                </div>
                            </div>
                            <p>Standart</p>
                        </div>

                        {#each ACTION_BUTTONS as item, index}
                            <button on:click={() => onClick_changePage(item.isExit ? 'close' : 'transaction', item.path)} tabindex="-1" class="content-grid__item">
                                {@html svgStorage[`button-${item.img}`]}
                                <p>{item.textKey}</p>
                            </button>
                        {/each}
                    </div>

                    <div class="data">
                        <div class="data-main">
                            <div class="data-main__content">
                                <p>Банкомат №{atmId}</p>
                            </div>
                            <div class="data-main__tax">
                                <div class="data-block data-main__tax-item">
                                    <p>Пополнение:</p>
                                    <p>0.00%</p>
                                </div>
                                <div class="data-block data-main__tax-item">
                                    <p>Снятие:</p>
                                    <p>0.00%</p>
                                </div>
                            </div>
                        </div>
                        <div class="data-manage">
                            <div class="data-block data-manage__price">
                                <p>Гос. цена банкомата:</p>
                                <p>$0</p>
                            </div>
                            <button on:click={() => onClick_buyAtm()} tabindex="-1" class="data-manage__button">Купить банкомат</button>
                        </div>
                    </div>
                </div>
            </div>
        {:else if page === "transaction"}
            <div in:fade={TRANSITION_SETTINGS_IN} out:fade={TRANSITION_SETTINGS_OUT} class="_wrapper transaction">
                <div class="content">
                    <div class="header">
                        <button on:click={() => onClick_changePage('main')} tabindex="-1" class="header-button">
                            {@html svgStorage["back"]}
                            <p>Назад</p></button
                        >
                        <div class="header-main">
                            <p>Баланс счёта:</p>
                            <div class="header-main__balance">
                                <p>${ ['house', 'business'].includes(type) ? `${subData.Balance} / ${subData.MaxBalance}` :  $charBankMoney }</p>
                                { @html svgStorage["card"] }
                            </div>
                        </div>
                    </div>
                    <div class="content-main">
                        <p>{ headerText }</p>
                        <div class="content-main__inputs">
                            {#each inputs as item, index }
                                <div class={ `wrapper input ${item.value.length > 0 ? 'active' : ''}` }>
                                    <div class="wrapper-main">
                                        { @html svgStorage[item.img] }
                                        <input placeholder={item.placeholder} bind:value={item.value} on:input={(event) => {
                                            // @ts-ignore
                                            const value = event.target.value;
                                            const maxSum = getMaxSum(item);

                                            if (value.length > item.maxlength)
                                                item.value = value.slice(0, item.maxlength);
                                            else if (maxSum != null && parseInt(value) > maxSum)
                                                item.value = maxSum.toString();
                                            else if (value < 0)
                                                item.value = '0';
                                            else {
                                                item.value = value.replace(/[^0-9]/g, '');;
                                            }
                                        }} type="text" maxlength={item.maxlength} />
                                    </div>
                                    
                                    {#if item.maxSum != null}
                                        <button on:click={() => {
                                            item.value = getMaxSum(item).toString()
                                        }} tabindex="-1">Max</button>
                                    {/if}
                                </div>
                            {/each}
                        </div>
                        <div class="content-main__tax">
                            <p>{ sumText }: <span>${ inputs[inputs.length - 1].value || 0 }</span></p>
                            <p>Комиссия: <span>$0</span></p>
                        </div>
                    </div>
                    
                    <button on:click={() => onClick_action()} tabindex="-1" class="content-button">Выполнить</button>
                </div>
            </div>
        {/if}
    </div>
</div>
