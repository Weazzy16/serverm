<script>
    import { executeClient } from 'api/rage'
    import { translateText } from 'lang'
    import weaponsinfo from './assets/js/weaponsinfo'
    import { format } from 'api/formatter'
    import './assets/sass/weaponshop.sass'
    import './assets/sass/main.css'

    const wComponentsType = {
        1: "Раскраска",//-
        2: "Магазин",
        3: "Глушитель", //Supp,
        4: "Прицел", //Scope,
        5: "Дульный тормоз", //Invalid,
        6: "Ствол", //Clip2,
        7: "Фонарик", //FlashLaser,
        8: "Рукоять", //Scope2,
        9: "Раскраска", //Grip2,
    }

    let category = ['Пистолеты','Дробовики','Пистолеты пулеметы', 'Штурмовые винтовки'],
        sumAmmo = 0,
        activeWeaponId = 0,
        activeWeaponCategory = 0,
        cntAmmo = 0,
        weapons = [[{Name:"Pistol","Icon":"inv-item-Pistol","Mats":50},{Name:"SNS Pistol","Icon":"inv-item-SNS-Pistol","Mats":40}]],
        ammo = [],
        components = [],
        ctypes = [],
        activeComponentId = 0,
        activeComponentCategory = 0;

    let selectIWeapon = weapons[activeWeaponCategory][activeWeaponId];
    let activeWeaponInfo = weaponsinfo[selectIWeapon.Name];
    let activeComponentInfo = components[activeComponentId];
    
    const maxAmmo = [
        100,
        50,
        300,
        250,
        48,
    ]

    const onHandleInput = (value) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 1) value = 0;  
        const max = maxAmmo [activeWeaponCategory];
        if (value > max) {
            value = max
        }
        
        cntAmmo = value;
        sumAmmo = Math.round(value * ammo[activeWeaponCategory]);
    }

    window.weaponshop = (weaponJson, ammoJson) => {
        weapons = JSON.parse(weaponJson);
        ammo = JSON.parse(ammoJson);
    }
    
    window.weaponshopcomponents = (componentsJson, ctypesJson) => {
        components = JSON.parse(componentsJson);
        ctypes = JSON.parse(ctypesJson);
        onClickComponentCategory (ctypes[0])
    }

    const Specifications = (num) => {
        let step;
        let array = [];
            for (step = 0; step < 5; step++) {
                array += (`<svg class=${step >= num ? '' : 'active'} width="32" height="3" viewBox="0 0 32 3" fill="none" xmlns="http://www.w3.org/2000/svg"><rect x="0.5" width="32" height="3"></rect></svg>`)
            }
        return array
    }

    const onSelectComponent = () => {
        executeClient('client.weaponshop.components', selectIWeapon.Name.replace(/\s/g, ''));
    }

    const onClickWeaponCategory = (id) => {
        activeWeaponCategory = id;
        activeWeaponId = 0;
        cntAmmo = 0;
        sumAmmo = 0;
        selectIWeapon = weapons[activeWeaponCategory][activeWeaponId];
        activeWeaponInfo = weaponsinfo[selectIWeapon.Name];
    }

    const onClickWeapon = (id) => {
        activeWeaponId = id; 
        cntAmmo = 0;
        sumAmmo = 0;
        selectIWeapon = weapons[activeWeaponCategory][activeWeaponId];
        activeWeaponInfo = weaponsinfo[selectIWeapon.Name];
    }
    
    const onClickComponentCategory = (id) => {
        activeComponentCategory = id;
        let updateComponentId = false;
        components.forEach((item, index) => {
            if (item.type == id && !updateComponentId) {
                activeComponentId = index;
                updateComponentId = true;
            }
        });
        activeComponentInfo = components[activeComponentId];
    }

    const onClickComponent = (id) => {
        activeComponentId = id;
        activeComponentInfo = components[activeComponentId];
    }

    const onBuy = () => {
        executeClient('client.weaponshop.buy', activeWeaponCategory, activeWeaponId);
    }

    const onBuyAmmo = () => {
        executeClient('client.weaponshop.buyAmmo', activeWeaponCategory,cntAmmo);
    }

    const onBuyComponent = () => {
        executeClient('client.weaponshop.buyComponent', activeWeaponCategory, activeWeaponId, activeComponentInfo.hash);
    }

    const onExit = () => {
        executeClient('client.weaponshop.close');
    }

    const getLengthFix = (length) => {
        let rLength = 6;
        switch (length) {
            case 1:
            case 2:
            case 3:
                rLength = 3;
                break;
            case 4:
            case 5:
            case 6:
                rLength = 6;
                break;
            case 7:
            case 8:
            case 9:
                rLength = 9;
                break;
            case 10:
            case 11:
            case 12:
                rLength = 12;
                break;
            case 13:
            case 14:
            case 15:
                rLength = 15;
                break;
        }
        return rLength;
    }

    const getLengthFixToComponents = (type) => {
        let pushData = [];
        components.forEach((item, index) => {
            if (item.type == type) pushData.push({...item, index: index});
        });
        if (getLengthFix (pushData.length) != pushData.length) {
            for (let i = 0; i <= getLengthFix (pushData.length) - pushData.length; i++) {
                pushData.push(0);
            }
        }
        return pushData;
    }
    const onBack = () => {
        components = [];
        ctypes = [];
        activeComponentId = 0;
        activeComponentCategory = 0;
        activeComponentInfo = components[activeComponentId];
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;
        onExit()
    }
</script>
<svelte:window on:keyup={HandleKeyDown} />

<div class="gunshophaxzer">
    <div class="gunshophaxzer_menu">
        <div class="gunshop_head">
            <div class="ico">
                <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/ammologo.png" alt=""/>
            </div>
            <div class="info">
                <h1>Магазин оружия</h1>
                <span>Какой-то текст будет здесь</span>
            </div>
        </div>
        <div class="gunshop_cat">
            {#if (ctypes.length)}
                {#each ctypes as value, index}
                    <div on:keypress on:click={() => onClickComponentCategory(value)} key={index} class={"block" + (activeComponentCategory === value ? " act" : "")}>
                        <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/{wComponentsType [value]}1.png" alt=""/>
                        <p>{wComponentsType [value]}</p>
                    </div>
                {/each}
            {:else}
                {#each category as value, index}
                    <div on:keypress on:click={() => onClickWeaponCategory(index)} key={index} class={"block" + (activeWeaponCategory === index ? " act" : "")}>
                        <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/{value}.png" alt=""/>
                        <p>{@html value}</p>
                    </div>
                {/each}
            {/if}
        </div>
        <div class="gunshop_close" on:keypress on:click={onExit}>
            <p>X</p>
        </div>
        <div class="gunshop_menu">
            {#if (ctypes.length)}
                <div class="name">
                    <h1>{@html activeComponentInfo.Name}</h1>
                    <span>{@html activeComponentInfo.Desc}</span>
                </div>
                <div class="iconweapon">
                    <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/{wComponentsType [activeComponentCategory]}.png" alt=""/>
                </div>
                <div class="info">
                    <div class="har">
                        <div class="block">
                            <div class="icon">
                                <img src="http://gta5dev.online/gta5dev/gunshop/Урон.png" alt=""/>
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Урон')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.damage : 0)}
                                </div>
                            </div>
                        </div>
                        <div class="block">
                            <div class="icon">
                                <img src="http://gta5dev.online/gta5dev/gunshop/Скорострельность.png" alt=""/>
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Скорострельность')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.ratefire : 0)}
                                </div>
                            </div>
                        </div>
                        <div class="block">
                            <div class="icon">
                                <img src="http://gta5dev.online/gta5dev/gunshop/Точность.png" alt=""/>
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Точность')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.accuracy : 0)}
                                </div>
                            </div>
                        </div>
                        <div class="block">
                            <div class="icon">
                                <img src="http://gta5dev.online/gta5dev/gunshop/Дальность.png" alt=""/>
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Дальность')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.range : 0)}
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="buy">
                        <div class="price">
                            <p>Общая цена</p>
                            <span>{format("money", activeComponentInfo.Mats)} $</span>
                        </div>
                        <div class="infobtn">
                            <div class="btnammo" on:keypress on:click={onBack}>Назад</div>
                            <div class="btn" on:keypress on:click={onBuyComponent}>Купить</div>
                        </div>
                    </div>
                </div>
                {:else}
                <div class="name">
                    <h1>{selectIWeapon.Name}</h1>
                    <span>{activeWeaponInfo ? activeWeaponInfo.desc : "Нет"} (1 {translateText('business', 'патрон')} = {ammo[activeWeaponCategory]}$)</span>
                </div>
                <div class="iconweapon">
                    <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/{selectIWeapon.Name}.png" alt=""/>
                </div>
                <div class="info">
                    <div class="har">
                        <div class="block">
                            <div class="icon">
                               
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Урон')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.damage : 0)}
                                </div>
                            </div>
                        </div>
                        <div class="block">
                            <div class="icon">
                                
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Скорострельность')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.ratefire : 0)}
                                </div>
                            </div>
                        </div>
                        <div class="block">
                            <div class="icon">
                                
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Точность')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.accuracy : 0)}
                                </div>
                            </div>
                        </div>
                        <div class="block">
                            <div class="icon">
                            
                            </div>
                            <div class="dop">
                                <h1>{translateText('business', 'Дальность')}</h1>
                                <div class="bar">
                                    {@html Specifications(activeWeaponInfo ? activeWeaponInfo.range : 0)}
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="buy">
                        <div class="price">
                            <p>Общая цена</p>
                            <span>{format("money", selectIWeapon.Mats)} $</span>
                        </div>
                        <div class="infobtn">
                            <div class="icona">
                                <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/ammo.png" alt=""/>
                            </div>
                            <input bind:value={cntAmmo} on:input={(event) => onHandleInput (event.target.value)} placeholder="0" maxLength="6">
                            <div class="btnammo" on:keypress on:click={onBuyAmmo}>П.Купить</div>
                            <div class="btn" on:keypress on:click={onSelectComponent}>Модификации</div>
                            <div class="btn" on:keypress on:click={onBuy}>Купить</div>
                        </div>
                    </div>
                </div>
            {/if}
        </div>
        <div class="item_list">
            {#if (ctypes.length)}
                    {#each getLengthFixToComponents(activeComponentCategory) as item, index}
                    <div class="block" class:act={item && activeComponentId === item.index} key={index} on:keypress on:click={item != 0 ? () => onClickComponent(item.index) : null}>
                        {#if item != 0}
                            <h1>{@html item.Name}</h1>
                            <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/{wComponentsType [activeComponentCategory]}.png" alt=""/>
                            <span style={true ? '' : 'opacity: 0;'}>$ {format("money", item.Mats)}</span>
                        {:else}
                            <p>Свободный Слот</p>
                        {/if}
                    </div>
                    {/each}
                {:else}
                    {#each new Array(getLengthFix (weapons.length)).fill(0) as _, index}
                    <div class="block" class:act={activeWeaponId === index} on:keypress on:click={weapons[activeWeaponCategory][index] ? () => onClickWeapon(index) : null}>
                        {#if weapons[activeWeaponCategory][index]}
                            <h1>{@html weapons[activeWeaponCategory][index].Name}</h1>
                            <img src="http://cdn.piecerp.ru/src/views/business/weaponshop/gunimg/{weapons[activeWeaponCategory][index].Name}.png" alt=""/>
                            <span style={true ? '' : 'opacity: 0;'}>$ {format("money", weapons[activeWeaponCategory][index].Mats)}</span>
                        {:else}
                            <p>Свободный Слот</p>
                        {/if}
                    </div>
                    {/each}
            {/if}
        </div>
    </div>
</div>
