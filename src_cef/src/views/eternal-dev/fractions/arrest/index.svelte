<script>
    import "./style.scss";
    
    import { charFractionID } from "store/chars";
    import { executeClient } from "api/rage";
    import { fly } from "svelte/transition";

    import RangeSlider from "svelte-range-slider-pips";

    export let popupData;

    let 
        timeSettings = [5, 100], 
        pledgeSettings = [15_000, 125_000],

        userData = {
            Avatar: "",
            Name: "Ilya Merumond",
            UUID: 233,
            Phone: 123,
            Gender: true,
            CriminalRecord: 1
        }, 

        userAvatar = "",
        reason = "",
        
        timeValues = [timeSettings[0]],
        timeValue = timeValues[0],
        
        canPledge = false,
        pledgeValues = [pledgeSettings[0]],
        pledgeValue = pledgeValues[0],
        
        fractionLogo = "LSPD";

    $: timeValue = timeValues[0];
    $: pledgeValue = pledgeValues[0];
    $: fractionLogo = $charFractionID == 9 ? "FIB" : "LSPD";
    $: userAvatar = "https://cdn.majestic-files.com/img/avatars/male.svg";

    const FRACTION_NAMES = {
        7: "Los Santos Police Department",
        9: "Federal Investigation Buerau"
    }

    $: if (popupData && typeof (popupData) == "string") {
        popupData = JSON.parse(popupData);

        timeSettings = popupData.TimeSettings;
        pledgeSettings = popupData.PledgeSettings;
        userData = popupData.UserData;

        pledgeValues[0] = pledgeSettings[0];
        timeValues[0] = timeSettings[0];
    }

    function doArrest() {
        executeClient("e-dev.arrestMenu.submit", timeValue, canPledge, pledgeValue, reason);
    }

    function cancel() {
        executeClient("e-dev.arrestMenu.cancel");
    }
</script>

<div transition:fly={{ y: 500, duration: 300 }} class="database-arrest full-width full-height">
    <div class="database-modal column-block">
        <header class="modal-header row-block full-width align-center">
            <button on:click={() => cancel()} class="close-block">
                <svg xmlns="http://www.w3.org/2000/svg" width="17.122" height="17.121" viewBox="0 0 17.122 17.121">
                    <g transform="translate(1.061 1.061)" stroke="#fff">
                      <path d="M0,0,15,15" fill="none" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path>
                      <path d="M6.929,0l-15,15" transform="translate(8.071)" stroke-linecap="square" stroke-miterlimit="10" stroke-width="2"></path>
                    </g>
                </svg>
            </button>
            <div class="modal-header__picture LSPD">
                <img class="modal-header__picture"
                    src="https://cdn.majestic-files.com/img/ipad/apps/organizations/logotypes/{fractionLogo}.svg"
                    alt="LSPD.svg"
                />
            </div>
            <div class="header-data column-block">
                <span class="header-data__title">Произвести арест</span>
                <span class="header-data__subtitle">{ FRACTION_NAMES[$charFractionID] }</span>
            </div>
        </header>
        <main class="modal-main column-block full-width">
            <div class="user-data row-block">
                <img src={userAvatar} alt="avatar"
                    class="user-data__image"/>
                <div class="user-info column-block full-width">
                    <span class="user-info__title">{ userData.Name }</span>
                    <div class="user-elements row-block full-width">
                        <div class="user-data-container column-block">
                            <div class="data-block row-block">
                                <span class="data-block__title">Static ID:</span>
                                <span class="data-block__value">#{ userData.UUID }</span>
                            </div>
                            <div class="data-block row-block">
                                <span class="data-block__title">Пол:</span>
                                <span class="data-block__value">{ userData.Gender ? "Мужской" : "Женский" }</span>
                            </div>
                        </div>
                        <div class="user-data-container column-block">
                            <div class="data-block row-block">
                                <span class="data-block__title">Телефон:</span>
                                <span class="data-block__value">{ userData.Phone }</span>
                            </div>
                            <div class="data-block row-block">
                                <span class="data-block__title">Судимости:</span>
                                <span class="data-block__value">{ userData.CriminalRecord }</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="input-container column-block full-width">
                <span class="input-container__title">Срок заключения</span>
                <div class="input-element row-block justify-between align-center">
                    <input class="input-element__value" bind:value={timeValues[0]} type="text" />
                    <span class="input-element__title">Минут</span>
                    <RangeSlider step={1} range="min" min={timeSettings[0]} max="{timeSettings[1]}" bind:values={timeValues} />
                </div>
            </div>
            <div class="checkboxes column-block full-width">
                <div class="checkbox-container full-width row-block align-center justify-between">
                    <span class="checkbox-container__title">Возможность выйти под залог</span>
                    <input on:click={() => canPledge = !canPledge} type="checkbox" class="checkbox-container__input" />
                </div>

                {#if canPledge}
                    <div class="input-container bail-possibility column-block full-width">
                        <span class="input-container__title">Сумма залога</span>
                        <div class="input-element row-block justify-between align-center">
                            <span class="input-element__value">
                                <span class="highlighted">$ </span>
                                <input class="input-element__value" type="text" bind:value={pledgeValues[0]} />
                            </span>
                            <RangeSlider step={1000} range="min" min={pledgeSettings[0]} max="{pledgeSettings[1]}" bind:values={pledgeValues} />
                        </div>
                    </div>
                {/if}
                <!-- <div class="checkbox-container full-width row-block align-center justify-between">
                    <span class="checkbox-container__title">Создать запись о судимости</span>
                    <input type="checkbox" class="checkbox-container__input" />
                </div> -->
            </div>
            <div class="orders column-block full-width">
                <span class="orders__title">Причина заключения</span>
                <input class="orders__input" bind:value={reason} placeholder="Перечислите номера статей через запятую" />
            </div>
            <div class="control-buttons row-block full-width justify-between">
                <button on:click={() => doArrest()} class="control-button full-width">
                    Арестовать
                </button>
                <button on:click={() => cancel()} class="control-button full-width">
                    Отмена
                </button>
            </div>
        </main>
    </div>
</div>
