<script>
    // @ts-ignore
    import RangeSlider from "svelte-range-slider-pips";
    import { fade, fly } from "svelte/transition";

    import { format } from "api/formatter";
    import { executeClient } from "api/rage";

    import configJson from "./json/edev-chipTuning.json";
    import categories from "./config/categories";
    import notices from "./config/notices";

    import "./style.scss";

    export let popupData;

    const NEGATIVE_VALUES_PARAMS = ["wheelTrackWidth", "wheelHeight"];

    let 
        modal = !true,
        vehicleHandlings = {"fDriveBiasFront":{"value":1,"values":[1],"default":1},"fBrakeBiasFront":{"value":1.2000000476837158,"values":[1.2000000476837158],"default":1.2000000476837158},"fHandBrakeForce":{"value":0.3499999940395355,"values":[0.3499999940395355],"default":0.3499999940395355},"fTractionCurveMax":{"value":2.049999952316284,"values":[2.049999952316284],"default":2.049999952316284},"fTractionCurveMin":{"value":1.850000023841858,"values":[1.850000023841858],"default":1.850000023841858},"fLowSpeedTractionLossMult":{"value":1.100000023841858,"values":[1.100000023841858],"default":1.100000023841858},"fTractionCurveLateral":{"value":0.39269909262657166,"values":[0.39269909262657166],"default":0.39269909262657166},"wheelRadius":{"value":1,"values":[1],"default":1},"wheelWidth":{"value":1,"values":[1],"default":1},"wheelCamber":{"value":9.376011576023302e-7,"values":[9.376011576023302e-7],"default":9.376011576023302e-7},"wheelTrackWidth":{"value":-0.8807132,"values":[-0.8807132],"default":-0.685356616973877},"wheelHeight":{"value":-0.41892001032829285,"values":[-0.41892001032829285],"default":-0.41892001032829285},"fSteeringLock":{"value":0.6981316804885864,"values":[0.6981316804885864],"default":0.6981316804885864}},
        vehicleControllers = {},
        vehiclePrice = 15000,
        currentCategory = "chip",

        currentHandlings = [],
        currentControllers = [];

    let refContainer;

    const getAttributes = (type) => {
        const data = configJson["handling-settings"].find(x => x.handlingParam == type);
        if (data == null)
            return;

        let attrs = [`${data.min}`, `${data.max}`];
        switch(type) {
            case "fDriveBiasFront":
                attrs = ["RWD", "FWD"];
                break;
            case "fBrakeBiasFront":
                attrs = ["R", "L"]
                break;
            case "fSteeringLock":
                attrs = [`${(data.min * 100).toFixed()}`, `${(data.max * 100).toFixed()}`]
                break;
        }

        return attrs;
    }

    $: currentHandlings = configJson["handling-settings"].filter(x => categories.find(x => x.type == currentCategory).handlings.includes(x.handlingParam));
    $: currentControllers = configJson["controller-settings"].filter(x => categories.find(x => x.type == currentCategory).controllers.includes(x.type));

    $: if (popupData && typeof popupData === "string") {
        popupData = JSON.parse (popupData)

        // @ts-ignore
        vehicleHandlings = Object.fromEntries(Object.entries(popupData.vehicleHandlings)
            .map(item => {
                const isNegative = NEGATIVE_VALUES_PARAMS.includes(item[0]);
                const value = Number((isNegative ? -item[1].value : item[1].value).toFixed(2));

                return [item[0], {
                    ...item[1],
                    value,
                    values: [value]
                }];
            })
        );

        vehiclePrice = popupData.vehiclePrice;
        vehicleControllers = popupData.vehicleControllers;
        
        setTimeout(() => modal = true, 250);
    }

    const setCategory = (type) => {
        currentCategory = type;
        if (refContainer)
            refContainer.scrollTo({ top: 0, behavior: "smooth" });
    };

    const onClick_buy = (type, subType, isRemove) => {
        if (type == "handling") {
            let value = vehicleHandlings[subType].values[0];
            
            const isNegative = NEGATIVE_VALUES_PARAMS.includes(subType);
            if (isNegative)
                value = -value;

            executeClient("e-dev.chipTuning.buy", type, subType, isRemove ? vehicleHandlings[subType].default : value, isRemove);
            vehicleHandlings[subType].values[0] = isRemove ? vehicleHandlings[subType].default : (isNegative ? -value : value);
        } else 
            executeClient("e-dev.chipTuning.buy", type, subType, isRemove);
    };

    const onUpdate = (type, value) => {
        const data = configJson["handling-settings"].find(x => x.handlingParam == type);
        if (data == null)
            return;

        const isNegative = NEGATIVE_VALUES_PARAMS.includes(type);
        if (isNegative)
            value = -value;

        executeClient("e-dev.chipTuning.update", type, value);
    }

    const getFakeValue = (type, value) => {
        const data = configJson["handling-settings"].find(x => x.handlingParam == type);
        const current = vehicleHandlings[type];
        if (data == null || current == null || !data.depends_default)
            return value;

        const isNegative = NEGATIVE_VALUES_PARAMS.includes(type);

        return value + data.default - (isNegative ? -current.default : current.default);
    };

    const formatMinMax = (type, valueType) => {
        const data = configJson["handling-settings"].find(x => x.handlingParam == type);
        const current = vehicleHandlings[type];
        if (current == null || !data.depends_default)
            return valueType == "min" ? data.min : data.max;

        const isNegative = NEGATIVE_VALUES_PARAMS.includes(type);

        let min = current.default - (data.default - data.min);
        let max = current.default - (data.default - data.max);

        if (isNegative) {
            min = -(current.default + (data.default - data.min)); 
            max = -(current.default + (data.default - data.max));
        }

        return valueType == "min" ? Number(min.toFixed(2)) : Number(max.toFixed(2));
    }

    const formatValue = (type, value) => {
        const current = vehicleHandlings[type];
        if (current == null)
            return "???";

        if (type == "fSteeringLock")
            value = Math.trunc(value * 100);

        return value.toFixed(2);
    }

    const TRANSITION = { duration: 250 };
</script>

<div in:fade={TRANSITION} out:fade={TRANSITION} class="chipTuning">
    {#if modal}
        <div in:fade={TRANSITION} out:fade={TRANSITION} class="modal">
            <div in:fly={{ duration: 250, y: 100, delay: 150 }} class="container">
                <div class="text">
                    Установите слайдер на интересующее Вас значение, а затем нажмите Ё(~), чтобы проверить изменения до покупки. После проверки нажмите Ё(~) чтобы вернутся в меню
                </div>
                <button on:click={() => modal = false} class="button">
                    Понятно
                </button>
            </div>
        </div>
    {/if}

    <div class="main">
        <div bind:this={refContainer} class="container">
            {#each notices[currentCategory] as item}
                <div class="notice">
                    {item}
                </div>
            {/each}

            {#each currentHandlings as item, index}
                <div class="controller">
                    <div class="header">{ item.name }</div>
    
                    <div class="slider-block">
                        <div class="attribute">{ getAttributes(item.handlingParam)[0] }</div>
                        <div class="slider">
                            <RangeSlider on:change={(e) => onUpdate(item.handlingParam, e.detail.value)} range="min" step={item.step} min={formatMinMax(item.handlingParam, "min")} max={formatMinMax(item.handlingParam, "max")} bind:values={vehicleHandlings[item.handlingParam].values} /> 
                        </div>
                        <div class="attribute">{ getAttributes(item.handlingParam)[1] }</div>
                    </div>
    
                    <div class="values"> 
                        <div class="value">Текущее: <b>{formatValue(item.handlingParam, getFakeValue(item.handlingParam, vehicleHandlings[item.handlingParam].values[0]))}</b></div>
                        <div class="value">Стандартное: <b>{formatValue(item.handlingParam, item.depends_default ? item.default : vehicleHandlings[item.handlingParam].default)}</b></div>
                    </div>
    
                    <div class="buttons">
                        <button on:click={() => onClick_buy("handling", item.handlingParam, false)} class="button buy">Купить { format("money", Math.trunc(item.price.cost + (item.price.depends_vehicle ? vehiclePrice * configJson["vehicle-price-mult"] : 0))) }{item.price.isDonate ? "RB" : "$"}</button>
                        <button on:click={() => onClick_buy("handling", item.handlingParam, true)} class="button">Убрать { format("money", Math.trunc((item.price.cost + (item.price.depends_vehicle ? vehiclePrice * configJson["vehicle-price-mult"] : 0)) * configJson["remove-handling-price-mult"])) }{item.price.isDonate ? "RB" : "$"}</button>
                    </div>
                </div>
            {/each}
            {#each currentControllers as item, index}
                <div class="controller">
                    <div class="header">{ item.name }</div>
        
                    <div class="description">
                        { item.description }
                    </div>
        
                    <div class="buttons">
                        <button on:click={() => onClick_buy("controller", item.type, false)} class="button buy">Купить { format("money", Math.trunc(item.price.cost + (vehiclePrice * configJson["vehicle-price-mult"]))) }$</button>
                        <button on:click={() => onClick_buy("controller", item.type, true)} class="button">Убрать { format("money", Math.trunc(item.price.cost + (vehiclePrice * configJson["vehicle-price-mult"]) * configJson["remove-handling-price-mult"])) }$</button>
                    </div>
                </div>
            {/each}
        </div>

        <div class="categories">
            {#each categories as item}
                <button on:click={() => setCategory(item.type)} class="block" class:active={currentCategory == item.type}>
                    { item.name }
                </button>
            {/each}
        </div>
    </div>
</div>