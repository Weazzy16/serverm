<script>
    import './main.sass';
    import './main.css';
    import './inventory.css';
    import './fonts/inv/style.css';
    import './fonts/items/style.css';
    import './fonts/gamemenu/style.css';
    import './fonts/Gilroy/stylesheet.css';
    import './fonts/SFPro/stylesheet.css';
    import { translateText } from 'lang'
    export let visible;
    import { otherStatsData } from 'store/account'
    import { charData } from 'store/chars';

    import { charGender, charMoney, charGolod, charWater, charHealth, charArmor } from 'store/chars'
    import { executeClient } from 'api/rage'
    import { ItemType, ItemId, itemsInfo } from 'json/itemsInfo.js'
    
    import { clothesData, ItemToWeaponHash, WeaponHashToItem, stageItem, clearSlot, defaulSelectItem, defaulHoverItem, maxSlots, otherName, otherType, clothes, clothesId, clothesName, itemIdCaseToId } from './functions.js';
    import { format } from 'api/formatter'
    import wComponents from './wComponents.js';
    import wMaxHP from './wMaxHP.js';
    import rangeslider from 'components/rangeslider/index'
    import Slot from "./slot.svelte";
    import Slot2 from "./slot2.svelte";
    import Slot3 from "./slot3.svelte";
    import Slot4 from "./slot4.svelte";
    import Slot5 from './slot5.svelte';
    import SlotAccs from "./slotaccs.svelte";
    import { onMount } from 'svelte';
    import { spring } from 'svelte/motion';
    import './fonts/newinv/style.css'
    import { getPng } from './getPng.js'

    
    let activeItem = null; 

    import inventoryWeapons from 'json/inventoryweapons.js'
    

    let useVisible = -1;

export let selectCharData;

$: {
    if (useVisible != visible) {
        if (visible && $otherStatsData.Name/* && $otherStatsData.UUID !== selectCharData.UUID*/) {
            selectCharData = $otherStatsData;
        } else if (visible && !$otherStatsData.Name && selectCharData !== $charData) {
            selectCharData = $charData;
        } else if (!visible && $otherStatsData.Name) {
            selectCharData = $charData;
            window.accountStore.otherStatsData ('{}');
        }
        useVisible = visible;
    }
}



let selcetinv = false;

const onSelectedInv = (type) => {
    selcetinv = type;
}

let searchText = "";

let itemsWithDataCount = 0;

$: itemsWithDataCount = ItemsData["other"].filter(item => item.ItemId && window.getItem(item.ItemId)).length;


let
    fastSlots = [1, 2, 3, 4, 5],
    clickTime = 0,
    invOpacity = 1,
    invOldOpacity = -1,
    ItemStack = -1,
    StackValue = 1,
    tradeMoney = "",
    useInventoryArea = false,
    mouseLeaveSelectedItem = false,
    mainInventoryArea = false,
    selectItem = defaulSelectItem,
    hoverItem = defaulHoverItem,
    infoItem = defaulHoverItem,
    isMoveBlock = false,
    moveBlock = {
        accessories: [null, null],
        inventory: [null, null],
        backpack: [null, null],
        other: [null, null],
        fastSlots: [null, null],
        trade: [null, null],
        with_trade: [null, null],
    },
    ItemsData = {
        accessories: Array(maxSlots.accessories).fill(clearSlot),
        inventory: Array(maxSlots.inventory).fill(clearSlot),
        backpack: Array(maxSlots.backpack).fill({ ...clearSlot, use: false }),
        other: Array(maxSlots.other).fill(clearSlot),
        fastSlots: Array(maxSlots.fastSlots).fill(clearSlot),
        trade: Array(maxSlots.trade).fill(clearSlot),
        with_trade: Array(maxSlots.with_trade).fill(clearSlot),
    },
    SlotToPrice = [],
    tradeInfo = {
        Active: false,
        
        YourStatus: false,		 	// Статус готовности обмена
        YourStatusChange: false, 	// Нажата кнопка "Обмен"
        YourMoney: 0,

        WithName: "Deluxe",			// Имя игрока, с которым вы обмениваетесь
        WithStatus: false,			// Статус готовности обмена игрока, с которым вы обмениваетесь
        WithStatusChange: false, 	// Нажата кнопка "Обмен"
        WithMoney: 0
    },
    PlayerInfo = {
        Sex: 0,
        Name: "",
        Backpack: false
    },
    OtherInfo = {
        Id: 0,
        Name: "",
    },
    OtherItemId = 0,
    OtherSqlId = 0,
    isArmyCar = false,
    isInVehicle = false;

/* Functions */
let coords = spring({ x: 0, y: 0 }, {
    stiffness: 1.0,
    damping: 1.0
});

const Close = () => {
    tradeInfo = {
        Active: false,
        
        YourStatus: false,		 	// Статус готовности обмена
        YourStatusChange: false, 	// Нажата кнопка "Обмен"
        YourMoney: "",

        WithName: "Deluxe",			// Имя игрока, с которым вы обмениваетесь
        WithStatus: false,			// Статус готовности обмена игрока, с которым вы обмениваетесь
        WithStatusChange: false, 	// Нажата кнопка "Обмен"
        WithMoney: ""
    }
    OtherInfo.Id = otherType.None;
    OtherInfo.Name = "";
    ItemsData.other = Array(maxSlots.other).fill(clearSlot);
    ItemsData.trade = Array(maxSlots.trade).fill(clearSlot);
    ItemsData.with_trade = Array(maxSlots.with_trade).fill(clearSlot);

    if (invOldOpacity != -1) {
        invOpacity = invOldOpacity;
        invOldOpacity = -1;
    }
    itemNoUse (1);
}
window.getItemToCount = (_ItemId) => {
    let count = 0;
    for (let arrayName in ItemsData) {
        if (arrayName !== "other" && arrayName !== "trade" && arrayName !== "with_trade" && arrayName !== "backpack") 
        {
            ItemsData[arrayName].forEach((i) => {
                if (i.ItemId == _ItemId) {
                    count += Math.round (i.Count);
                }
            })
        }
    }
    return count;
}
window.isItem = (_ItemsId) => {
    _ItemsId = JSON.parse (_ItemsId);
    let rItemId = [];
    for (let arrayName in ItemsData) {
        if (arrayName !== "other" && arrayName !== "trade" && arrayName !== "with_trade" && arrayName !== "backpack") 
        {
            ItemsData[arrayName].forEach((i) => {
                if (_ItemsId.includes(i.ItemId)) {
                    rItemId.push(i.ItemId);
                }
            })
        }
    }
    if (rItemId.length > 0) {
        executeClient ("client.inventory.GetItem", JSON.stringify(rItemId), true);
        return true;
    } else {
        executeClient ("client.inventory.GetItem", _ItemsId, false);
        return false;
    }
}
const Bool = (text) => {
    return String(text).toLowerCase() === "true";
}

//Выгрука

const InitData = (json, use = true) => {
    let itemsArray = JSON.parse(json);
    for (let arrayName in itemsArray) {
        ItemsData[arrayName] = LoadData (maxSlots [arrayName], itemsArray [arrayName], use);
    }
}

let maxSlotBackpack = 30;
const InitMyData = (maxSlot, json, use = true) => {
    maxSlotBackpack = maxSlot;
    ItemsData["backpack"] = LoadData (maxSlot, JSON.parse(json), use);
}

const UpdateSpecialVars = (isInVehicle_info = false) => {
    isInVehicle = isInVehicle_info;
}

const LoadData = (maxSlot, json, use = true) => {
    let returnArray = [];
    let indexItem;
    let itemsIndex = 0;
    let itemsArray = json;
    
    Array(maxSlot).fill(0).forEach((item, index) => {
        if (itemsArray && itemsArray.length && itemsArray[itemsIndex]) {
            item = itemsArray[itemsIndex];
            indexItem = item.Index;
        } else {
            indexItem = -1;
        }

        if (indexItem === index) {
            itemsIndex++;
            returnArray = [ ...returnArray, {
                ...clearSlot,
                ...item,
                use: use,
            }];
        } else {
            returnArray = [ ...returnArray, {
                ...clearSlot,
                use: use,
            }];
        }
    });
    
    return returnArray;
}

const InitSlotToPrice = (json = "[]") => {
    SlotToPrice = JSON.parse(json);
}

const InitOtherData = (otherId, otherName, json, maxSlot = 20, selectItemId = 0, isArmyCar_info = false, _isMyTent = false, _SlotToPrice = "[]") => {
    if (otherId == otherType.None) {
        if (OtherInfo.Id == otherType.None) return;
        closeOther ();
        return;
    }
    OtherInfo.Id = otherId;
    OtherInfo.Name = otherName;
    OtherInfo.IsMyTent = _isMyTent;
    if (selectItemId != 0 && selectItemId.split("_").length) {
        OtherItemId = selectItemId.split("_")[0];
        OtherSqlId = selectItemId.split("_")[1];
    }

    let returnArray = [];
    let indexItem;
    let itemsIndex = 0;
    let itemsArray = JSON.parse(json);

    SlotToPrice = JSON.parse(_SlotToPrice);

    /*if (OtherInfo.Id === otherType.Storage) {
        itemsArray.forEach(item => {
            returnArray = [ ...returnArray, {
                ...clearSlot,
                ...item,
            }];
        });
    } else {*/
        Array(maxSlot).fill(0).forEach((item, index) => {            
            if (itemsArray && itemsArray.length && itemsArray[itemsIndex]) {
                item = itemsArray[itemsIndex];
                indexItem = item.Index;
            } else {
                indexItem = -1;
            }

            if (indexItem === index) {
                itemsIndex++;
                returnArray = [ ...returnArray, {
                    ...clearSlot,
                    ...item,
                }];
            } else {
                returnArray = [ ...returnArray, {
                    ...clearSlot
                }];
            }
        });
    //}

    ItemsData.other = returnArray;
    isArmyCar = isArmyCar_info;
}

const InitTradeData = (Name) => {
    tradeInfo.Active = true;

    tradeInfo.YourStatus = false;
    tradeInfo.YourStatusChange = false;
    ItemsData.trade = Array(maxSlots ["trade"]).fill(clearSlot);
    tradeInfo.YourMoney = "";

    tradeInfo.WithStatus = false;
    tradeInfo.WithStatusChange = false;
    tradeInfo.WithName = Name;
    tradeInfo.WithMoney = "";
    
    ItemsData.with_trade = Array(maxSlots ["with_trade"]).fill(clearSlot);
}

const UpdateSlot = (inventoryType, inventoryIndex, json, isInfo) => {
    const item = JSON.parse(json);
    if (isInfo && (inventoryType === "inventory" || inventoryType === "backpack")) {
        window.hudItem.drop (item.ItemId, item.Count, item.Data, true)
    }

    window.events.callEvent ("cef.events.UpdateSlot", json);
    
    const oldItem = ItemsData[inventoryType][inventoryIndex];

    ItemsData[inventoryType][inventoryIndex] = {
        ...oldItem,
        ...item
    }
    let hoverIndex = -1,
        hoverArrayName = -1;
        
    if (hoverItem !== defaulHoverItem) {
        hoverIndex = hoverItem.index;
        hoverArrayName = hoverItem.arrayName;
    }
    //hoverItem = defaulHoverItem;
    if (hoverIndex === -1 && hoverArrayName === -1) infoItem = defaulHoverItem;
    else {            
        const _Item = getItemToIndex (hoverIndex, hoverArrayName);
        if (_Item.ItemId != 0) {
            infoItem = {
                ..._Item,
                index: hoverIndex,
                arrayName: hoverArrayName
            };
        } else infoItem = defaulHoverItem;
    }

    
    /*if (res.name === "weapons" && temsArray["fastSlots"][res.index].active) {
        dataUser.updateCharName ("weapon", {
            icon: window.getItem (temsArray["fastSlots"][res.index].ItemId).icon,
            ammo: ItemsData.basic["fastSlots"][res.index].item_amount
        });
    }*/
}

window.getItem = (item) => {
    if (itemsInfo [item]) {
        return itemsInfo [item];
    }
    return {
        Name: "",
        icon: "",
        Type: "",
        Text: "",
        functionType: 0,
    }
}

const FastSlots = (json) => {
    fastSlots = JSON.parse(json);
}
onMount(() => {

    // Инициализация инвентаря игрока
    window.events.addEvent("cef.inventory.InitData", InitData);

    window.events.addEvent("cef.inventory.InitMyData", InitMyData);

    window.events.addEvent("cef.inventory.UpdateSpecialVars", UpdateSpecialVars);
    
    // Инициализация инвентаря при взаимодействии с чем то
    window.events.addEvent("cef.inventory.InitOtherData", InitOtherData);
    
    // Инициализация инвентаря при трейде
    window.events.addEvent("cef.inventory.InitTradeData", InitTradeData);

    // Обновление слота в любом ивентаре
    window.events.addEvent("cef.inventory.UpdateSlot", UpdateSlot);

    // Инициализация инвентаря игрока
    window.events.addEvent("cef.inventory.TradeUpdate", TradeUpdate);
    
    // Инициализация инвентаря игрока
    window.events.addEvent("cef.inventory.tradeMoney", handleInputChange);
    
    // Обновление информации о бучтрых слотах
    window.events.addEvent("cef.inventory.fastSlots", FastSlots);

    // Закрытие инвентаря
    window.events.addEvent("cef.inventory.Close", Close);

    window.events.addEvent("cef.inventory.SlotToPrice", InitSlotToPrice);
});

import { onDestroy } from 'svelte'
onDestroy(() => {
    // Инициализация инвентаря игрока
    window.events.removeEvent("cef.inventory.InitData", InitData);
    
    window.events.removeEvent("cef.inventory.InitMyData", InitMyData);

    window.events.removeEvent("cef.inventory.UpdateSpecialVars", UpdateSpecialVars);
    
    // Инициализация инвентаря при взаимодействии с чем то
    window.events.removeEvent("cef.inventory.InitOtherData", InitOtherData);
    
    // Инициализация инвентаря при трейде
    window.events.removeEvent("cef.inventory.InitTradeData", InitTradeData);

    // Обновление слота в любом ивентаре
    window.events.removeEvent("cef.inventory.UpdateSlot", UpdateSlot);

    // Обновление информации о items
    window.events.removeEvent("cef.inventory.Init", Init);

    // Инициализация инвентаря игрока
    window.events.removeEvent("cef.inventory.TradeUpdate", TradeUpdate);
    
    // Инициализация инвентаря игрока
    window.events.removeEvent("cef.inventory.tradeMoney", handleInputChange);
    
    // Обновление информации о бучтрых слотах
    window.events.removeEvent("cef.inventory.fastSlots", FastSlots);

    // Закрытие инвентаря
    window.events.removeEvent("cef.inventory.Close", Close);

    window.events.removeEvent("cef.inventory.SlotToPrice", InitSlotToPrice);
});

const onKeyDown = (event) => {
    if (!visible) return;
    if (event.which === 17 && invOldOpacity === -1) {
        invOldOpacity = invOpacity;
        invOpacity = 0;
    }
}

const onKeyUp = (event) => {  
    if (!visible) return;
    if (event.which === 17 && invOldOpacity != -1) {
        invOpacity = invOldOpacity;
        invOldOpacity = -1;
    }
}
// Слот
const handleSlotMouseEnter = (event, index, arrayName) => {
    if (selectItem.use === stageItem.move && hoverItem === defaulHoverItem) {            
        hoverItem = {
            index: index,
            arrayName: arrayName
        }
    }
    
    if (selectItem.use !== stageItem.useItem && selectItem.use !== stageItem.move && getItemToIndex (index, arrayName).ItemId) {
        const target = event.target.getBoundingClientRect();
        coords.set({ x: (target.x + target.width/2), y: target.y });
        infoItem = {
            ...getItemToIndex(index, arrayName),
            index: index,
            arrayName: arrayName
        };
    }
}

// Когда выходим из зоны ячейки
const handleSlotMouseLeave = (e) => {
    if (hoverItem !== defaulHoverItem) hoverItem = defaulHoverItem;
    if (infoItem !== defaulHoverItem) infoItem = defaulHoverItem;
    
    if (mouseLeaveSelectedItem === false) mouseLeaveSelectedItem = true;
    
}
//

const closeOther = () => {
    OtherInfo.Id = otherType.None;
    OtherItemId = 0;
    OtherSqlId = 0;
    OtherInfo.Name = "";
    ItemsData.other = Array(maxSlots.other).fill(clearSlot);
    executeClient ("client.inventory.OtherClose");
}

const handleSlotMouseUp = () => {
    if (selectItem.use === stageItem.info && clickTime >= new Date().getTime()) {
        const index = selectItem.index;
        const arrayName = selectItem.arrayName;
        const _sItem = getItemToIndex (index, arrayName);
        const _sInfoItem = window.getItem (_sItem.ItemId);

        if (selectItem.arrayName === "other" || selectItem.arrayName === "backpack") {
            let MaxStakcItems = 0;
            if ((MaxStakcItems = getMaxStakcItems (_sItem, _sInfoItem)) == -1) {
                itemNoUse (2);
                return;
            }
            if (MaxStakcItems > 0) executeClient ("client.gamemenu.inventory.stack", arrayName, index, 2, MaxStakcItems);
            else executeClient ("client.gamemenu.inventory.stack", arrayName, index, 2, _sItem.Count);
        } else if (_sInfoItem.functionType === ItemType.Cases && itemIdCaseToId [Number (_sItem.ItemId)] !== undefined) {
            window.router.setPopUp("PopupRoulette", itemIdCaseToId [Number (_sItem.ItemId)]);
        } else if (OtherSqlId && Number (OtherSqlId) === Number (_sItem.SqlId)) {
            closeOther ();
            executeClient ("client.gamemenu.inventory.use", arrayName, index);                
        } else executeClient ("client.gamemenu.inventory.use", arrayName, index);
        itemNoUse (3);
    }
    else if (selectItem.use === stageItem.get || selectItem.use === stageItem.info) {
        //if (getItemsUse (selectItem) !== false && (arrayName === "accessories" || arrayName === "inventory" || arrayName === "backpack" || arrayName === "other" || arrayName === "other")) {
            const index = selectItem.index;

            const arrayName = selectItem.arrayName;

            if (updateItem(index, arrayName, "hover")) {
                selectItem = {
                    ...getItemToIndex(index, arrayName),
                    use: stageItem.info,
                    index: index,
                    arrayName: arrayName
                }
            } else selectItem = defaulSelectItem;
        //} else itemNoUse (4);
    }
}

const setAccessories = () => {
    if (selectItem === defaulSelectItem) return;
    else if (hoverItem === defaulHoverItem && selectItem.use === stageItem.move && selectItem.arrayName !== "accessories") {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;
        const _sItem = getItemToIndex (selectIndex, selectArrayName);
        const _sInfoItem = window.getItem (_sItem.ItemId);
        if (selectArrayName !== "inventory") {
            itemNoUse (5);
            window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
            return;
        }
        
        let hoverIndex = setClothes (_sItem.ItemId);

        if (hoverIndex == -2) return itemNoUse (6);

        hoverIndex = hoverIndex.slotId;
        const hoverArrayName = "accessories";

        const _hItem = getItemToIndex (hoverIndex, hoverArrayName);

        const _hInfoItem = window.getItem (_hItem.ItemId);

        if (isMove (hoverIndex, hoverArrayName, _sItem, _sInfoItem) == -2) {
            itemNoUse (7);
            return;
        }

        executeClient ("client.gamemenu.inventory.move", selectArrayName, selectIndex, hoverArrayName, hoverIndex);

        //{"Name":"Маска","Description":"","Icon":"item-pizza","Type":"Одежда","Model":3887136870,"Stack":1,"functionType":1}
        if (_hItem.ItemId === _sItem.ItemId && Number (_hInfoItem.Stack) > 1) {
            const amount = (_hItem.Count === undefined || _hItem.Count < 2 || !isNumber(_hItem.Count)) ? 1 : _hItem.Count;

            if (Number (_hInfoItem.Stack) >= (amount + _sItem.Count)) {
                _sItem.Count += amount;
                setItem (hoverIndex, hoverArrayName, _sItem);
                setItem (selectIndex, selectArrayName, clearSlot);

            } else {
                _hItem.Count = (amount + _sItem.Count) - _hInfoItem.Stack;
                _sItem.Count = _hInfoItem.Stack;
                setItem (hoverIndex, hoverArrayName, _sItem);
                setItem (selectIndex, selectArrayName, _hItem);
            }
        } else {
            setItem (hoverIndex, hoverArrayName, _sItem);
            setItem (selectIndex, selectArrayName, _hItem);
        }
        itemNoUse (8);
    }
}

const UpdateClothes = (event, componentId, drawableId, textureId) => {
    
    executeClient (event, componentId, drawableId, textureId);
}

const handleMouseDown = (event, index, arrayName) => {
    if (event.which == 1) {
        executeClient ("sounds.playInterface", "inventory/keys", 0.005);
        
        const item = getItemToIndex(index, arrayName);

        if (((selectItem.use === stageItem.info && (selectItem.index !== index || selectItem.arrayName !== arrayName)) ||
            selectItem.use !== stageItem.info) && item.ItemId != 0 && item.use) {

            if (arrayName === "other" && OtherInfo.Id === otherType.Nearby) {
                if (OtherInfo.Id === otherType.Nearby && item.remoteId) 
                    return executeClient ("client.gamemenu.inventory.nearby", item.remoteId);
            } else if (arrayName === "other" && OtherInfo.Id === otherType.Tent) {

                let _infoItem = window.getItem (item.ItemId);

                const _selectItem = {
                    ...item,
                    use: stageItem.useItem,
                    index: index,
                    arrayName: arrayName,
                    tent: true,
                    info: _infoItem
                }

                unHoverAll ();
                updateItem(index, arrayName, "hover", true);
                infoItem = defaulHoverItem;
                
                coords.set({ x: event.clientX, y: event.clientY });

                clickTime = new Date().getTime() + 200;
                
                StackValue = 1;

                selectItem = _selectItem;
                if (item.Count > 1) {
                    rangeslidercreate (item.Count);
                }

                return;
            }
            if (OtherSqlId && Number (OtherSqlId) === Number (getItemToIndex(index, arrayName).SqlId)) {
                closeOther ();
            }

            itemNoUse (9);

            const target = event.target.getBoundingClientRect();

            const offsetInElementX = (target.width - (target.right - event.clientX));
            const offsetInElementY = (target.height - (target.bottom - event.clientY));

            coords.set({ x: event.clientX, y: event.clientY });

            clickTime = new Date().getTime() + 1000;
            selectItem = {
                ...getItemToIndex(index, arrayName),
                use: stageItem.get,
                width: target.width,
                height: target.height,
                offsetInElementX: offsetInElementX,
                offsetInElementY: offsetInElementY,
                clientX: event.clientX,
                clientY: event.clientY,
                index: index,
                arrayName: arrayName
            }

            mouseLeaveSelectedItem = false;

        }
    } else if (event.which == 3 && (arrayName !== "other" || (arrayName === "other" && OtherInfo.Id !== otherType.Nearby && OtherInfo.Id !== otherType.Tent)) && ItemsData[arrayName][index].ItemId != 0 && getItemToIndex(index, arrayName).use) {
        const item = getItemToIndex(index, arrayName);

        const _selectItem = {
            ...item,
            use: stageItem.useItem,
            index: index,
            arrayName: arrayName
        }
        if (getItemsUse (selectItem) === false && OtherInfo.Id <= otherType.None && item.Count <= 0 && getDropItem (arrayName, item.ItemId) === false) return;

        unHoverAll ();
        updateItem(index, arrayName, "hover", true);
        infoItem = defaulHoverItem;
        
        coords.set({ x: event.clientX, y: event.clientY });

        selectItem = _selectItem;
    }
}

const handleInputChange = (name, value) => {
    value = Math.round(value.replace(/\D+/g, ""));
    if (value < 0) value = 0;
    else if (value > 9999999) value = 9999999;
    tradeInfo[name] = value;
    
    if (name === "YourMoney") executeClient ("client.gamemenu.inventory.tradeMoney", value);
}

const onBlur = () => {
    if (tradeInfo.YourMoney < 0) {
        window.notificationAdd(4, 9, translateText('player1', 'Сумма не может быть меньше 0.'), 3000);
        tradeInfo.YourMoney = 0;
        //return;
    } else if (tradeInfo.YourMoney > 9999999) {
        window.notificationAdd(4, 9, translateText('player1', 'Сумма не может быть больше $9.999.999.'), 3000);
        tradeInfo.YourMoney = 9999999;
        //return;
    } else if (Number($charMoney) < tradeInfo.YourMoney) {
        tradeInfo.YourMoney = Number($charMoney);
        window.notificationAdd(4, 9, `${translateText('player1', 'Сумма не может быть больше')} ${format("money", tradeInfo.YourMoney)}.`, 3000);
        //return;
    }
    executeClient ("client.gamemenu.inventory.tradeMoney", Math.round(tradeInfo.YourMoney));
}

//Глобальные
const handleGlobalMouseMove = (event) => {
    if (!visible) return;
    else if (isMoveBlock) {
        moveBlock[isMoveBlock] = [
            event.clientY - selectItem.offsetInElementY,
            event.clientX - selectItem.offsetInElementX
        ]
        
        if (moveBlock[isMoveBlock][0] + selectItem.height > window.innerHeight) moveBlock[isMoveBlock][0] = window.innerHeight - selectItem.height;
        else if (moveBlock[isMoveBlock][0] < 0) moveBlock[isMoveBlock][0] = 0;

        if (moveBlock[isMoveBlock][1] + selectItem.width > window.innerWidth) moveBlock[isMoveBlock][1] = window.innerWidth - selectItem.width;
        else if (moveBlock[isMoveBlock][1] < 0) moveBlock[isMoveBlock][1] = 0;
    }
    else if (infoItem !== defaulHoverItem) {
        boxInfoLeft = fixOutToCenter ($coords.x, boxItemInfo);
        boxInfoTop = fixOutToTop ($coords.y, boxItemInfo);
    }
    else if ((selectItem.use === stageItem.move && infoItem === defaulHoverItem) || (selectItem.use !== stageItem.useItem && selectItem.use !== stageItem.get && infoItem === defaulHoverItem)) {
        let clientX = event.clientX;
        let clientY = event.clientY;
        
        if (clientY + selectItem.height > window.innerHeight) clientY = window.innerHeight - selectItem.height;
        else if (clientY < 0) clientY = 0;

        if (clientX + selectItem.width > window.innerWidth) clientX = window.innerWidth - selectItem.width;
        else if (clientX < 0) clientX = 0;
        
        coords.set({ x: clientX, y: clientY });

    } else if (selectItem.use === stageItem.get && (selectItem.clientX !== event.clientX || selectItem.clientY !== event.clientY)) {
        unHoverAll ();
        selectItem = {
            ...selectItem,
            use: stageItem.move,
        }
        let clientX = event.clientX;
        let clientY = event.clientY;
        
        if (clientY + selectItem.height > window.innerHeight) clientY = window.innerHeight - selectItem.height;
        else if (clientY < 0) clientY = 0;

        if (clientX + selectItem.width > window.innerWidth) clientX = window.innerWidth - selectItem.width;
        else if (clientX < 0) clientX = 0;

        coords.set({ x: clientX, y: clientY });            
    }
}

const onUseItem = () => {
    if (selectItem.use === stageItem.useItem) {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;

        const Item = getItemToIndex(selectIndex, selectArrayName);
        const InfoItem = window.getItem (Item.ItemId);

        if (InfoItem.functionType === ItemType.Cases && itemIdCaseToId [Number (Item.ItemId)] !== undefined) {
            window.router.setPopUp("PopupRoulette", itemIdCaseToId [Number (Item.ItemId)] );
        } else if (OtherSqlId && Number (OtherSqlId) === Number (Item.SqlId)) {
            closeOther ();
        } else {
            executeClient ("client.gamemenu.inventory.use", selectArrayName, selectIndex);
        }
        itemNoUse (10);
    }
}

const onDropItem = () => {
    if (selectItem.use === stageItem.useItem) {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;

        if (selectItem.Count > 1) {
            ItemStack = 1;
            rangeslidercreate (selectItem.Count);
        } else {
            itemNoUse (11);
            executeClient ("client.gamemenu.inventory.drop", selectArrayName, selectIndex);
        }
    }
}

const onTransfer = () => {
    if (selectItem.use === stageItem.useItem) {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;

        if (selectItem.Count > 1) {
            ItemStack = 2;
            rangeslidercreate (selectItem.Count);
        } else {            
            const _sItem = getItemToIndex (selectIndex, selectArrayName);
            const _sInfoItem = window.getItem (_sItem.ItemId);
            if (selectItem.arrayName !== "other" && isMove (selectIndex, "other", _sItem, _sInfoItem) == -2) {
                itemNoUse (12);
                return;
            } else if ((selectItem.arrayName === "other" || selectItem.arrayName === "backpack") && getMaxStakcItems (_sItem, _sInfoItem) != 0) {
                itemNoUse (13);
                return;
            }
            executeClient ("client.gamemenu.inventory.stack", selectArrayName, selectIndex, 2, 1);
            itemNoUse (14);
        }
    }
}

const handleInputStackChange = (value) => {
    value = Math.round(value.replace(/\D+/g, ""));
    if (value < 0) value = 0;
    else if (ItemStack === 0 && value > selectItem.Count - 1) value = selectItem.Count - 1;
    else if (ItemStack !== 0 && value > selectItem.Count) value = selectItem.Count;
    StackValue = value;
}

const onBlurStack = () => {
    if (StackValue < 1) StackValue = 1;
    else if (StackValue > selectItem.Count - 1) StackValue = selectItem.Count - 1;
}

const onStack = () => {
    if (selectItem.use === stageItem.useItem) {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;
        if (ItemStack == 2) {
            const _sItem = getItemToIndex (selectIndex, selectArrayName);
            const _sInfoItem = window.getItem (_sItem.ItemId);
            if (selectItem.arrayName !== "other" && isMove (selectIndex, "other", _sItem, _sInfoItem) == -2) {
                itemNoUse (15);
                return;
            }
            let MaxStakcItems = 0;
            if ((selectItem.arrayName === "other" || selectItem.arrayName === "backpack") && (MaxStakcItems = getMaxStakcItems (_sItem, _sInfoItem)) == -1) {
                itemNoUse (16);
                return;
            }
            if (MaxStakcItems > 0) StackValue = MaxStakcItems;
        }
        executeClient ("client.gamemenu.inventory.stack", selectArrayName, selectIndex, ItemStack, StackValue);
        itemNoUse (17);
    }
}

const onBuy = () => {
    if (selectItem.use === stageItem.useItem) {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;
        executeClient ("client.gamemenu.inventory.buy", selectArrayName, selectIndex, StackValue);
        itemNoUse (17);
    }
}

const setClothes = (ItemId) => {
    if (ItemId === 12 || ItemId === 15) {
        return clothes["Bags"];
    } else if (ItemId === -15) {
        return clothes["Watches"];
    }
    let returnSlotId = -2;
    clothesId.forEach((item) => {
        if (clothes[item].itemId === ItemId) returnSlotId = clothes[item];
    });
    return returnSlotId;
}

const isMove = (index, arrayName, item, itemInfo) => {
    const OtherInfoId = OtherInfo.Id;
    let dataParse;
    if (item.ItemId != 0 && item.Data.split("_").length) dataParse = item.Data.split("_");

    if (arrayName === "accessories" && index === 8 && (getItemToIndex (8, "accessories").ItemId === 12 || getItemToIndex (8, "accessories").ItemId === 15))
    {
        window.notificationAdd(4, 9, translateText('player1', 'Это действие недоступно'), 3000);
        return -2;
    }
    else if (arrayName === "accessories" && item.ItemId != 0 && item.ItemId != -9 && item.ItemId != -5 && item.ItemId != -1 && itemInfo.functionType === ItemType.Clothes && dataParse && dataParse.length >= 2 && Bool(dataParse[2]) !== Bool($charGender)) {
        window.notificationAdd(4, 9, `Это ${Bool(dataParse[2]) ? translateText('player1', 'мужская') : translateText('player1', 'женская')} ${translateText('player1', 'одежда')}`, 3000);
        return -2;
    } else if (arrayName === "accessories" && item.ItemId != 0 && (clothes[clothesId[index]].itemId !== item.ItemId || (index === 8 && item.ItemId !== 12 && item.ItemId !== 15) || (index === 11 && item.ItemId !== -15))) {
        const _id = setClothes (item.ItemId);
        if (_id == -2) {
            window.notificationAdd(4, 9, translateText('player1', 'Данный слот не доступен!'), 3000);
            return -2;
        }
        //else if (_id.itemId == item.ItemId) return -2;
        return _id.slotId;
    } else if (arrayName === "accessories" && item.ItemId != 0 && index === 1 && !$charGender && (item.Data === "127_0_True" || item.Data === "127_2_True")) {
        window.notificationAdd(4, 9, translateText('player1', 'Вы не можете надеть этот уникальный аксессуар'), 3000);
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Safe && item.ItemId != 0 && itemInfo.functionType !== ItemType.Weapons &&  itemInfo.functionType !== ItemType.MeleeWeapons) {//Оружейный сейф
        window.notificationAdd(4, 9, translateText('player1', 'В данный сейф можно положить только оружие'), 3000);
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Chiffonier && item.ItemId != 0 && itemInfo.functionType !== ItemType.Clothes) {//Cейф под одежду
        window.notificationAdd(4, 9, translateText('player1', 'В данный шкаф можно положить только одежду'), 3000);
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Wardrobe && item.ItemId != 0 && (item.ItemId == 109 || item.ItemId == 12 || item.ItemId == 15 || item.ItemId == 19 || item.ItemId == 40 || item.ItemId == 41 || itemInfo.functionType === ItemType.Clothes || itemInfo.functionType === ItemType.Weapons)) {
        window.notificationAdd(4, 9, translateText('player1', 'Эта вещь не предназначена для этого шкафа'), 3000);
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Fraction && item.ItemId != 0 && itemInfo.functionType !== ItemType.Weapons && itemInfo.functionType !== ItemType.Ammo && item.ItemId != -9) {
        window.notificationAdd(4, 9, translateText('player1', 'На склад можно положить только оружие или патроны'), 3000);
        return -2;
    } else if ((arrayName === "inventory" || arrayName === "backpack") && OtherInfoId === otherType.Fraction && item.ItemId != 0) {
        let checkItem = getItemToIndex (index, arrayName);
        if (checkItem.ItemId != 0) {
            window.notificationAdd(4, 9, translateText('player1', 'Переложить можно только в пустой слот'), 3000);
            return -2;
        }

        /*let success = true;
        let count = item.Count;
        ItemsData[arrayName].forEach((i) => {
            if (success && item.ItemId == i.ItemId && item.SqlId != i.SqlId) {
                count += i.Count;
                if (count > itemInfo.Stack) {
                    success = false;
                    window.notificationAdd(4, 9, translateText('player1', 'Недостаточно места для такого количества'), 3000);
                }
            }
        })*/
        if (getMaxStakcItems (item, itemInfo) === -1) return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Organization && item.ItemId != 0 && itemInfo.functionType !== ItemType.Weapons &&  itemInfo.functionType !== ItemType.Ammo && item.ItemId != -9) {
        window.notificationAdd(4, 9, translateText('player1', 'На склад можно положить только оружие или патроны'), 3000);
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Key && item.ItemId != 0 && item.ItemId != 19) {
        window.notificationAdd(4, 9, translateText('player1', 'Только ключи от т/c'), 3000);
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Nearby) {
        window.notificationAdd(4, 9, translateText('player1', 'Данное действие недоступно!'), 3000);
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.Tent && !OtherInfo.IsMyTent) {
        window.notificationAdd(4, 9, translateText('player1', 'Данное действие недоступно!'), 3000);
        return -2; 
    } else if (arrayName === "other" && OtherInfoId === otherType.Tent && OtherInfo.IsMyTent && item.ItemId != 0 && (item.ItemId == 19)) {
        window.notificationAdd(4, 9, translateText('player1', 'Нельзя продавать этот предмет.'), 3000);
        return -2;
    } else if (OtherInfoId === otherType.Vehicle && isArmyCar === true && item.ItemId != 0 && (item.ItemId == 237 || item.ItemId == 238 || item.ItemId == 239 || item.ItemId == 240 || item.ItemId == 241 || item.ItemId == 242)) {
        window.notificationAdd(4, 9, translateText('player1', 'Нельзя перекладывать этот предмет.'), 3000);
        return -2;
    } else if ((arrayName === "inventory" || arrayName === "accessories") && item.ItemId == -9 && isInVehicle === true) {
        window.notificationAdd(4, 9, translateText('player1', 'Невозможно снять бронежилет находясь в транспорте.'), 3000);
        executeClient ("checkClientSpecialVars");
        return -2;
    } else if (arrayName === "other" && OtherInfoId === otherType.wComponents && item.ItemId != 0) {            
        const weaponHash = ItemToWeaponHash [OtherItemId];
        let success = 0;
        if (!(item.ItemId >= 206 && 217 >= item.ItemId)) {
            window.notificationAdd(4, 9, translateText('player1', 'Этот слот предназначен только для модификаций'), 3000);
            return -2;
        }
        else if (item.ItemId >= 206 && 217 >= item.ItemId) {
            if (Number (dataParse[0]) == Number (weaponHash)) {
                success = 1;
            }
            if (wComponents [weaponHash] && wComponents [weaponHash].Components) {
                let componentHash;
                let typeId = -1;
                //Сначала проверяем есть ли он в списке компонентов
                for (componentHash in wComponents [weaponHash].Components) {
                    if (Number (dataParse[1]) == Number (componentHash)) {
                        success = 1;
                        typeId = wComponents [weaponHash].Components [dataParse[1]].Type;
                    }
                }
                //Затем проверяемм есть ли уже такой тип 
                if (success == 1) {
                    ItemsData["other"].forEach((i) => {
                        if (success != -1 && i.ItemId != 0 && i.Data.split("_").length) {
                            let dParse = i.Data.split("_");                    
                            if (wComponents [weaponHash].Components [dParse[1]] && wComponents [weaponHash].Components [dParse[1]].Type == typeId) {
                                success = -1;
                            }
                        }
                    })
                }
            }
        }
        if (success == 1) return -1;
        else if (success == -1) window.notificationAdd(4, 9, translateText('player1', 'Модификация такого типа уже установлена на данномм оружие!'), 3000);
        else window.notificationAdd(4, 9, translateText('player1', 'Данная модификация не подходит к этому оружию!'), 3000);
        return -2;
    } else if (arrayName === "fastSlots" && index === 4 &&
            item.ItemId != 0 &&
            item.ItemId != -1) {
        window.notificationAdd(4, 9, translateText('player1', 'Этот слот предназначен только для масок'), 3000);
        return -2;
    } else if (arrayName === "fastSlots" && index !== 4 &&
                item.ItemId != 0 &&
                itemInfo.functionType !== ItemType.Weapons &&
                itemInfo.functionType !== ItemType.MeleeWeapons &&
                item.ItemId != 6 &&
                item.ItemId != 7 &&
                item.ItemId != 8 &&
                item.ItemId != 3 &&
                item.ItemId != 5 &&
                item.ItemId != 9 &&
                item.ItemId != 10 &&
                item.ItemId != 1 &&
                item.ItemId != 280 &&
                item.ItemId != 225 &&
                item.ItemId != 226 &&
                item.ItemId != 227 &&
                item.ItemId != 228 &&
                item.ItemId != 229 &&
                item.ItemId != 230 &&
                item.ItemId != 231 &&
                item.ItemId != 232 &&
                item.ItemId != 233 &&
                item.ItemId != 388 &&
                item.ItemId != 389 &&
                item.ItemId != ItemId.VehicleNumber) {
        window.notificationAdd(4, 9, `${translateText('player1', 'Вы не можете положить сюда')} ${itemInfo.Name}`, 3000);
        return -2;
    }/* else if (arrayName === "other" && !getMaxStakcItems (item, itemInfo)) {
        window.notificationAdd(4, 9, `Вы не можете больше вместить ${itemInfo.Name}`, 3000);
        return -2;
    }*/
    else if ((arrayName === "backpack" && (item.ItemId == -5 || item.ItemId == 12)) || (arrayName === "other" && ((item.ItemId === -5 && Number(item.Data.split("_")[0]) == 40) || item.ItemId == 12))) 
    {
        window.notificationAdd(4, 9, `${translateText('player1', 'Вы не можете положить сюда')} ${itemInfo.Name}`, 3000);
        return -2;
    }
    else if ((arrayName !== "backpack" && arrayName !== "other") && (item.ItemId == 13 || item.ItemId == 1)) 
    {
        let success = true;
        let count = 0;
        ItemsData[arrayName].forEach((i) => {
            if (success && item.ItemId == i.ItemId && item.SqlId != i.SqlId) {
                count += i.Count;
                if (count >= itemInfo.Stack) {
                    success = false;
                    window.notificationAdd(4, 9, `${translateText('player1', 'Вы не можете положить')} ${itemInfo.Name}`, 3000);
                }
            }
        })
        if (success) return -1;
        else return -2;
    } else if (arrayName === "other" && (OtherInfoId === otherType.Storage || OtherInfoId === otherType.Case) && item.ItemId != 0) {
        window.notificationAdd(4, 9, translateText('player1', 'Данный склад предназначен лишь для изъятия предметов из него.'), 3000);
        return -2;
    }
    return -1;
}

const maxItemCount = 3;
const getMaxStakcItems = (item, itemInfo) => {
    if (item.ItemId == 0) return true;
    //if ([237, 238, 239, 240, 241, 242, 245, 246, 247].includes (item.ItemId)) 
    //    return item.Count;
    let countItems = 0;
    let maxStack = itemInfo.Stack;
    if (itemInfo.Stack <= 1) {
        if (itemInfo.functionType === ItemType.Weapons && item.ItemId != 109 && item.ItemId != 150) {
            const WeaponsAmmoTypes = {"100":200,"101":200,"102":200,"103":200,"104":200,"105":200,"106":200,"107":200,"108":200,"110":200,"111":200,"112":200,"113":200,"114":200,"151":200,"152":200,"115":201,"116":201,"117":201,"118":201,"119":201,"120":201,"121":201,"122":201,"123":201,"124":201,"125":201,"153":201,"126":202,"127":202,"128":202,"129":202,"130":202,"131":202,"132":202,"133":202,"134":202,"135":202,"136":203,"137":203,"138":203,"139":203,"140":203,"154":200,"155":200,"156":200,"157":200,"158":200,"159":200,"160":200,"161":200,"162":200,"141":204,"142":204,"143":204,"144":204,"145":204,"146":204,"147":204,"148":204,"149":204};
            const ammoType = WeaponsAmmoTypes[item.ItemId];
            let success = 0;
            for (let arrayName in ItemsData) {
                if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
                {
                    ItemsData[arrayName].forEach((i) => {
                        if (!success && ((ammoType && ammoType == WeaponsAmmoTypes[i.ItemId]) || item.ItemId === i.ItemId)) {
                            if (++countItems >= maxItemCount) {
                                success = -1;
                                window.notificationAdd(4, 9, `Невозможно взять ${itemInfo.Name}, потому что в инвентаре уже есть оружие такого типа.`, 3000);
                            }
                        }                   
                    })
                }
            }
            return success;
        } else if (itemInfo.functionType === ItemType.MeleeWeapons || item.ItemId == -5 || item.ItemId == 41 || item.ItemId == 109 || item.ItemId == 150) {
            let success = 0;
            for (let arrayName in ItemsData) {
                if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
                {
                    ItemsData[arrayName].forEach((i) => {
                        if (!success && item.ItemId == i.ItemId) {
                            success = -1;
                            window.notificationAdd(4, 9, `${translateText('player1', 'У Вас уже есть')} ${itemInfo.Name}`, 3000);
                        }
                    })
                }
            }
            return success;
        } else if (item.ItemId == 12 || item.ItemId == 15) {
            let success = 0;
            for (let arrayName in ItemsData) {
                if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
                {
                    ItemsData[arrayName].forEach((i) => {
                        if (!success && (i.ItemId == 12 || i.ItemId == 15)) {
                            success = -1;
                            window.notificationAdd(4, 9, `${translateText('player1', 'У Вас уже есть')} ${itemInfo.Name}`, 3000);
                        }
                    })
                }
            }
            return success;
        } else if (item.ItemId == -9) {
            let success = 0;
            for (let arrayName in ItemsData) {
                if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
                {
                    ItemsData[arrayName].forEach((i) => {
                        if (!success && i.ItemId == -9) {                                
                            if (++countItems >= maxItemCount) {
                                success = -1;
                                window.notificationAdd(4, 9, `У Вас уже есть ${itemInfo.Name}`, 3000);
                            }
                        }
                    })
                }
            }
            return success;
        }
    } else {
        let count = 0;
        for (let arrayName in ItemsData) {
            if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
            {
                ItemsData[arrayName].forEach((i) => {
                    if (i.ItemId == item.ItemId) {
                        count += Math.round (i.Count);
                    }
                })
            }
        }

        if (itemInfo.functionType == ItemType.Ammo)
            maxStack *= maxItemCount;

        if (Math.round (maxStack) === Math.round (count)) {
            window.notificationAdd(4, 9, `Нет места для ${itemInfo.Name}, максимум можно иметь при себе - ${maxStack} шт. | У вас ${count} шт.`, 3000);
            return -1;
        }
        else if (Math.round (maxStack) >= Math.round (count + item.Count)) return 0;
        else {
            return Math.round (maxStack) - count;
        }
    }
    return 0;
}

const handleGlobalMouseUp = (event) => {
    if (!visible) return;
    else if (event.which !== 1) return;
    if (isMoveBlock) {
        selectItem = defaulSelectItem;
        isMoveBlock = false;
        return;
    }
    else if (selectItem.use === stageItem.move) {
        if (hoverItem === defaulHoverItem && tradeInfo.Active === false && selectItem !== defaulSelectItem && getDropItem (selectItem.arrayName, selectItem.ItemId) !== false) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;
            //const _Item = window.getItem (id, arrayName);
            
            // Если мышка покинула первоначальную ячейку, и перенос был в несуществубщую ячейку, то дропаем
            if (mouseLeaveSelectedItem === true && mainInventoryArea === false/* && (selectArrayName === "inventory" || selectArrayName === "backpack")*/) {
                
                executeClient ("client.gamemenu.inventory.drop", selectArrayName, selectIndex);                    
            }
            itemNoUse(18);
        } else if (hoverItem !== defaulHoverItem && (hoverItem.index !== selectItem.index || hoverItem.arrayName !== selectItem.arrayName)) {
            //Item на который наводимся
            let hoverIndex = hoverItem.index;
            const hoverArrayName = hoverItem.arrayName;
            let _hItem = getItemToIndex (hoverIndex, hoverArrayName);
            let _hInfoItem = window.getItem (_hItem.ItemId);
            //Выбранный
            let selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;
            let _sItem = getItemToIndex (selectIndex, selectArrayName);
            let _sInfoItem = window.getItem (_sItem.ItemId);

            let returnMove = -1;
            if (!_hItem.use || hoverArrayName === "with_trade") {
                itemNoUse (19);
                window.notificationAdd(4, 9, translateText('player1', 'Данный слот не доступен!'), 3000);
                return;
            } else if ((hoverArrayName === "accessories" || hoverArrayName === "fastSlots") && selectArrayName !== "inventory") {
                itemNoUse (20);
                window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
                return;
            } else if ((selectArrayName === "accessories" || selectArrayName === "fastSlots") && hoverArrayName !== "inventory") {
                itemNoUse (21);
                window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
                return;
            } else if (hoverArrayName === "other" && OtherInfo.Id === otherType.Nearby) {
                executeClient ("client.gamemenu.inventory.drop", selectArrayName, selectIndex);   
                itemNoUse(18);
                return;
            }          
            
            if (hoverArrayName !== selectArrayName && (returnMove = isMove (hoverIndex, hoverArrayName, _sItem, _sInfoItem)) == -2) {
                itemNoUse (22);
                return;
            }
            
            if (hoverArrayName === "other" && OtherInfo.Id === otherType.Tent && OtherInfo.IsMyTent) {
                executeClient ("client.gamemenu.inventory.stack", selectArrayName, selectIndex, 2, _sItem.Count);
                itemNoUse(18);
                return;
            }  

            if (returnMove !== -1) {
                hoverIndex = returnMove;
                _hItem = getItemToIndex (hoverIndex, hoverArrayName);
                _hInfoItem = window.getItem (_hItem.ItemId);
                if (isMove (hoverIndex, hoverArrayName, _sItem, _sInfoItem) == -2) {
                    itemNoUse (23);
                    return;
                }
            }
            returnMove = -1;
            
            if (hoverArrayName !== selectArrayName && (returnMove = isMove (selectIndex, selectArrayName, _hItem, _hInfoItem)) == -2) {
                itemNoUse (24);
                return;
            }

            if (returnMove !== -1) {
                selectIndex = returnMove;
                _sItem = getItemToIndex (selectIndex, selectArrayName);
                _sInfoItem = window.getItem (_sItem.ItemId);
            
                if (isMove (selectIndex, selectArrayName, _hItem, _hInfoItem) == -2) {
                    itemNoUse (25);
                    return;
                }
            }

            let MaxStakcItems = 0;
            if ((hoverArrayName !== "other" && hoverArrayName !== "backpack") && (selectArrayName === "other" || selectArrayName === "backpack") && ![0, 237, 238, 239, 240, 241, 242, 245, 246, 247].includes (_sItem.ItemId) && (MaxStakcItems = getMaxStakcItems (_sItem, _sInfoItem)) == -1) {
                //window.notificationAdd(4, 9, `Нет места для ${itemInfo.Name}, максимум можно иметь при себе - ${itemInfo.Stack} шт. | У вас ${count + item.Count} шт.`, 3000);
                itemNoUse (26);
                return;
            }

            if (MaxStakcItems > 0) {
                if (_hItem.ItemId === _sItem.ItemId || _hItem.ItemId === 0) {
                    executeClient ("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);
                    /*if (_hItem.ItemId === _sItem.ItemId && _hItem.Count === _sInfoItem.Stack) {                            
                        setItem (hoverIndex, hoverArrayName, _sItem);
                        setItem (selectIndex, selectArrayName, _hItem);
                    }
                    else */if (_hItem.ItemId === _sItem.ItemId) {
                        _hItem.Count += MaxStakcItems;
                        _sItem.Count -= MaxStakcItems;
                        setItem (hoverIndex, hoverArrayName, _hItem);
                        setItem (selectIndex, selectArrayName, _sItem);
                        executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
                    } else {
                        _sItem.Count -= MaxStakcItems;
                        setItem (selectIndex, selectArrayName, _sItem);
                        _hItem = _sItem;
                        _hItem.Count = MaxStakcItems;
                        setItem (hoverIndex, hoverArrayName, _hItem);
                        executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
                    }
                }
                else window.notificationAdd(4, 9, `${translateText('player1', 'Нет места для')} ${_sInfoItem.Name}, ${translateText('player1', 'максимум можно иметь при себе')} - ${_sInfoItem.Stack} ${translateText('player1', 'шт.')}`, 3000);
                itemNoUse (27);
                return;
            }

            MaxStakcItems = 0;
            if ((selectArrayName !== "other" && selectArrayName !== "backpack") && (hoverArrayName === "other" || hoverArrayName === "backpack") && ![0, 237, 238, 239, 240, 241, 242, 245, 246, 247].includes (_hItem.ItemId) && _hItem.ItemId != _sItem.ItemId && (MaxStakcItems = getMaxStakcItems (_hItem, _hInfoItem)) == -1) {
                itemNoUse (28);
                return;
            }

            if (MaxStakcItems > 0) {
                /*if (_hItem.Count === _hInfoItem.Stack) {      
                    executeClient ("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);                      
                    setItem (hoverIndex, hoverArrayName, _sItem);
                    setItem (selectIndex, selectArrayName, _hItem);
                } else */if (_hItem.ItemId === _sItem.ItemId) {
                    executeClient ("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);
                    _sItem.Count += MaxStakcItems;
                    _hItem.Count -= MaxStakcItems;
                    setItem (hoverIndex, hoverArrayName, _hItem);
                    setItem (selectIndex, selectArrayName, _sItem);
                    executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
                }
                else window.notificationAdd(4, 9, `${translateText('player1', 'Нет места для')} ${_sInfoItem.Name}, ${translateText('player1', 'максимум можно иметь при себе')} - ${_sInfoItem.Stack} ${translateText('player1', 'шт.')}`, 3000);
                itemNoUse (29);
                return;
            }
            executeClient ("client.gamemenu.inventory.move", selectArrayName, selectIndex, hoverArrayName, hoverIndex);

            //{"Name":"Маска","Description":"","Icon":"item-pizza","Type":"Одежда","Model":3887136870,"Stack":1,"functionType":1}                
            if (_hItem.ItemId === _sItem.ItemId && Number (_hInfoItem.Stack) > 1 && Number (_hInfoItem.Stack) > _sItem.Count && Number (_hInfoItem.Stack) > _hItem.Count) {
                const amount = (_hItem.Count === undefined || _hItem.Count < 2 || !isNumber(_hItem.Count)) ? 1 : _hItem.Count;

                if (Number (_hInfoItem.Stack) >= (amount + _sItem.Count)) {
                    _sItem.Count += amount;
                    setItem (hoverIndex, hoverArrayName, _sItem);
                    setItem (selectIndex, selectArrayName, clearSlot);
                } else {
                    _hItem.Count = (amount + _sItem.Count) - _hInfoItem.Stack;
                    _sItem.Count = _hInfoItem.Stack;
                    setItem (hoverIndex, hoverArrayName, _sItem);
                    setItem (selectIndex, selectArrayName, _hItem);
                }
            } else {
                setItem (hoverIndex, hoverArrayName, _sItem);
                setItem (selectIndex, selectArrayName, _hItem);
            }
            executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
        }
        itemNoUse (30, true);
    }
}

const handleGlobalMouseDown = (event) => {
    if (!visible) return;
    else if (event.which !== 1) return;
    else if (selectItem.tent && clickTime >= new Date().getTime()) return;
    else if (selectItem.use === stageItem.useItem && !useInventoryArea) {
        itemNoUse (31);
    }
}

const getItemToIndex = (index, arrayName) => {
    return ItemsData[arrayName][index];
}

const unHoverAll = () => {
    for (let arrayName in ItemsData) {
        ItemsData[arrayName].forEach((item, index) => {
            if (item.hover) updateItem(index, arrayName, "hover");                 
        })
    }
}

const isNumber = (value) => {
    return /^[-]?\d+$/.test(value);
}

const updateItem = (index, arrayName, name, value = null) => {
    if (!arrayName && !index) return;
    if (ItemsData[arrayName][index].ItemId != 0) {
        value = (value === null) ? !ItemsData[arrayName][index][name] : value;
        ItemsData[arrayName][index][name] = value;
        return value;
    }
    return -9;
}

// Обновление полного массива item`а
const setItem = (index, arrayName, item) => {
    if (item.active) item.active = false;
    ItemsData[arrayName][index] = {
        ...item,
        index: index
    };
    
    //window.CallServer("UpdateSlotData", getInfo[arrayName], index, item.item_id, item.ItemId, item.item_amount, item.item_trade, item.item_money);
}

const itemNoUse = (hash, toggled = false) => {
    let hoverIndex = -1,
        hoverArrayName = -1;
    if (selectItem !== defaulSelectItem) updateItem(selectItem.index, selectItem.arrayName, "hover", false);

    clickTime = 0;
    
    selectItem = defaulSelectItem;

    if (hoverItem !== defaulHoverItem) {
        hoverIndex = hoverItem.index;
        hoverArrayName = hoverItem.arrayName;
    }
    //hoverItem = defaulHoverItem;
    if (hoverIndex === -1 && hoverArrayName === -1) infoItem = defaulHoverItem;
    else {            
        const _Item = getItemToIndex (hoverIndex, hoverArrayName);
        if (_Item.ItemId != 0) {
            infoItem = {
                ..._Item,
                index: hoverIndex,
                arrayName: hoverArrayName
            };
        } else infoItem = defaulHoverItem;
    }
    //toggledSplitter = false;

    ItemStack = -1;
    StackValue = 1;
    
}

const fixOutToX = (coordsX, element) => {
    if (!element) return coordsX;
    else if (document.querySelector('.box-inventory')) {
        let mainWidth = document.querySelector('.box-inventory').getBoundingClientRect().width;
        let elementWidth = element.getBoundingClientRect().width;
        if ((elementWidth + coordsX) >= mainWidth) return coordsX - elementWidth;
        return coordsX;
    }
    return coordsX;
}

const fixOutToY = (coordsY, element) => {
    if (!element) return coordsY;
    else if (document.querySelector('.box-inventory')) {
        let mainHeight = document.querySelector('.box-inventory').getBoundingClientRect().height;
        let elementHeight = element.getBoundingClientRect().height;
        if ((coordsY - elementHeight) < 0) return coordsY;
        return coordsY - elementHeight;
    }
    return coordsY;
}

const fixOutToCenter = (coordsX, element) => {
    if (!element) return coordsX;
    let elementWidth = element.getBoundingClientRect().width / 2;
    return coordsX - elementWidth;
}

const fixOutToTop = (coordsY, element) => {
    if (!element) return coordsY;
    let elementHeight = element.getBoundingClientRect().height;
    return coordsY - (elementHeight + (elementHeight * 0.2));
}

const animItems = (index, arrayName) => {
    ItemsData[arrayName][index] = {
        ...ItemsData[arrayName][index],
        anim: false
    };
    
    setTimeout(() => {
        ItemsData[arrayName][index] = {
            ...ItemsData[arrayName][index],
            anim: true
        };
    }, 0);
}

/* Трейд */
const TradeCancel = () => {
    if (tradeInfo.YourStatus) {
        tradeInfo.YourStatus = false;
        tradeInfo.YourStatusChange = false;
        ItemsData.trade.forEach((item, index) => {
            ItemsData.trade[index] = {
                ...ItemsData.trade[index],
                use: true
            };
        });
        executeClient ("client.gamemenu.inventory.trade", 0);
    }
}

const TradeSelect = () => {   
    if (!tradeInfo.YourStatus) {
        let seccuss = false;
        ItemsData.trade.forEach((item, index) => {
            if (selectItem.ItemId !== 0) {
                seccuss = true;
            }
        });

        if (tradeInfo.WithStatus && !seccuss) {// Проверяем, если у нас пустые слоты и второй игрок подтвердил трейд то
            
            ItemsData.with_trade.forEach((item, index) => {
                if (selectItem.ItemId !== 0) {
                    seccuss = true;
                }
            });
            if (!seccuss) { //Если у второго тоже пустые слото то выдаем ошибку
                window.notificationAdd(4, 9, translateText('player1', 'Для начала выберите предмет!'), 3000);
                return;
            }
        }         
        executeClient ("client.gamemenu.inventory.trade", 1);
    } else if (tradeInfo.YourStatus && tradeInfo.WithStatus && !tradeInfo.YourStatusChange) {
        executeClient ("client.gamemenu.inventory.trade", 2);
    }
}

const TradeUpdate = (status) => {
    if (status == -1) {
        tradeInfo.YourStatus = true;
        ItemsData.trade.forEach((item, index) => {
            ItemsData.trade[index] = {
                ...ItemsData.trade[index],
                use: false
            };
        });   
        return;
    }
    if (status == -2) {
        tradeInfo.YourStatusChange = true;
        window.notificationAdd(4, 9, translateText('player1', 'Вы подтвердили свою готовность!'), 3000);
        return;
    }
    let lastWithStatus = tradeInfo.WithStatus;
    if (!status) {
        tradeInfo.WithStatus = false;
        tradeInfo.WithStatusChange = false;
        tradeInfo.YourStatusChange = false;
    }
    else if (status === 1) tradeInfo.WithStatus = true;
    else if (status === 2) tradeInfo.WithStatusChange = true;

    ItemsData.with_trade.forEach((item, index) => {
        ItemsData.with_trade[index] = {
            ...ItemsData.with_trade[index],
            use: tradeInfo.WithStatus ? false : true
        };
    });

    if (lastWithStatus !== tradeInfo.WithStatus) {            
        selectItem = defaulSelectItem;
        isMoveBlock = false;
        if (!tradeInfo.YourStatus) {
            ItemsData.trade.forEach((item, index) => {
                ItemsData.trade[index] = {
                    ...ItemsData.trade[index],
                    use: tradeInfo.WithStatus ? false : true
                };
            });
        }
    }
    if (status === 1 && !tradeInfo.YourStatus) {
        window.notificationAdd(4, 9, `${tradeInfo.WithName} ${translateText('player1', 'выбрал предметы для трейда')}!`, 3000);
    } else if (status === 1 && tradeInfo.YourStatus) {
        window.notificationAdd(4, 9, `${translateText('player1', 'Вы и')} ${tradeInfo.WithName} ${translateText('player1', 'выбрали предметы для трейда')}`, 3000);
    } else if (status === 0) {
        window.notificationAdd(4, 9, `${tradeInfo.WithName} ${translateText('player1', 'выбирает предметы')}`, 3000);
    } else if (status === 2) {
        window.notificationAdd(4, 9, `${tradeInfo.WithName} ${translateText('player1', 'готов к обмену')}`, 3000);
    }
}

const rangeslidercreate = (max) => {
    StackValue = Math.round(max / 2);
    setTimeout(() => {
        rangeslider.create(document.getElementById("stack"), {min: 1, max: max, value: Math.round(max / 2), step: 1, onSlide: (value, percent, position) => {
            StackValue = Number(value);
        }});
    }, 0);

}

const createRangeSlider = () => {
    rangeslider.create(document.getElementById("invOpacity"), {min: 0, max: 1, value: invOpacity, step: 0.1, onSlide: (value, percent, position) => {
        invOpacity = value;
    }});
}
const entityMap = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#39;',
    '/': '&#x2F;',
    '`': '&#x60;',
    '=': '&#x3D;'
};

const escapeHtml = (str) => {
    return String(str).replace(/[&<>"'`=\/]/g, function (s) {
        return entityMap[s];
    });
}
const getName = (Item, arrayName) => {
    const _infoItem = window.getItem (Item.ItemId);
    let name = escapeHtml (_infoItem.Name);
    if (Item.ItemId == -9 && arrayName !== "accessories") {
        name += `<span>Состояние: ${Item.Data}</span>`;
    }
    else if (Item.ItemId == 19 && Item.Data.split("_")) {
        name += `<span>${Item.Data.split("_")[0]} | ${Item.Data.split("_")[1]}</span>`;
    } 
    else if (_infoItem.Stack > 1 && Item.Count > 1) {
        name += ` | ${Item.Count} шт.`;
    } else if (_infoItem.functionType === ItemType.Modification && Item.Data.split("_").length) {
        let dataParse = Item.Data.split("_");
        if (WeaponHashToItem [dataParse[0]]) name += `<span>Weapon: ${window.getItem (WeaponHashToItem [dataParse[0]]).Name}</span>`;
    } else if (Item.ItemId == 220) {
        name += `<span>${Item.Data}</span>`;
    }
    else if (Item.ItemId == 234 || Item.ItemId == 235 || Item.ItemId == 236 || Item.ItemId == 244
        || Item.ItemId == 225 || Item.ItemId == 229 || Item.ItemId == 249) {
        let ItemValue = 300;

        switch (Item.ItemId) {
            case 234:
                ItemValue = 300;
                break;
            case 235:
                ItemValue = 1248;
                break;
            case 236:
                ItemValue = 2250;
                break;
            case 244:
                ItemValue = 1250;
                break;
            case 225:
                ItemValue = 100;
                break;
            case 229:
                ItemValue = 420;
                break;
            case 249:
                ItemValue = 1000;
                break;
        }
        
        name += `<span>${translateText('player1', 'Состояние')}: ${weaponCondition (Item.Data, wMaxHP [Item.ItemId])}%</span>`;
    }
    else if (Item.ItemId == ItemId.SimCard || Item.ItemId == ItemId.VehicleNumber)
        name += `<span>${Item.Data}</span>`;
    return name;
}

const getNameToData = (Item, arrayName) => {
    const _infoItem = window.getItem (Item.ItemId);
    let name = "";
    if (_infoItem.functionType === ItemType.Weapons) {
        return Item.Data;
    }
    return name;
}

const getDropItem  = (arrayName, ItemId) => {
    if (arrayName === "fastSlots") return false;
    switch (ItemId) {
        case 2: return false;
        //case 243: return false;
    }
    return true;
}


const getItemsUse  = (Item) => {
    if (Item.arrayName !== "inventory" && Item.arrayName !== "accessories" && Item.arrayName !== "fastSlots") return false;
    else if (Item.arrayName === "accessories" && (Item.ItemId === 12 || Item.ItemId === 15)) return false;
    const _infoItem = window.getItem (Item.ItemId);
    if (_infoItem.functionType === ItemType.Clothes || _infoItem.functionType === ItemType.Weapons || _infoItem.functionType === ItemType.MeleeWeapons || _infoItem.functionType === ItemType.Alco) return true;
    else if (Item.ItemId == 41 && ((OtherInfo.Id !== otherType.None && OtherInfo.Id !== otherType.Key) || tradeInfo.Active)) return false;
    switch (Item.ItemId) {
        case 220:
        case 4:
        case 41:
        case 19:
        case 13:
        case 6:
        case 14:
        case 9:
        case 2:
        case 1:
        case 280:
        case 7:
        case 11:
        case 16:
        case 5:
        case 8:
        case 10:
        case 3:
        case 40:
        case 12:
        case 225:
        case 15:
        case 248:
        case 229:
        case ItemId.SimCard:return true;
        //case 249: return true;
    }
    return false;
}

const getItemsClickInfo = (info) => {
    let _infoItem = window.getItem (info.ItemId);
    if (info.arrayName === "accessories") return `${translateText('player1', 'снять')}`;
    else if (_infoItem.functionType === ItemType.Clothes) return `${translateText('player1', 'надеть')}`;
    else if (info.arrayName !== "fastSlots" && (_infoItem.functionType === ItemType.Weapons || _infoItem.functionType === ItemType.MeleeWeapons) && ItemToWeaponHash [info.ItemId] && wComponents [ItemToWeaponHash [info.ItemId]] && wComponents [ItemToWeaponHash [info.ItemId]].Components) return `${translateText('player1', 'модификации')}`;
    else if (info.arrayName !== "fastSlots" && (_infoItem.functionType === ItemType.Weapons || _infoItem.functionType === ItemType.MeleeWeapons)) return `${translateText('player1', 'взять')}`;
    else if (_infoItem.functionType === ItemType.Alco || _infoItem.functionType === ItemType.Water) return `${translateText('player1', 'выпить')}`;
    else if (_infoItem.functionType === ItemType.Eat) return `${translateText('player1', 'съесть')}`;
    else if (info.ItemId == 41 && OtherInfo.Id === otherType.None) return `${translateText('player1', 'открыть')}`;
    else if ((OtherInfo.Id === otherType.wComponents || OtherInfo.Id === otherType.Key) && OtherSqlId && Number (OtherSqlId) === Number (info.SqlId)) return `${translateText('player1', 'закрыть')}`;
    else if ((info.ItemId == 225 || info.ItemId == 229) && OtherInfo.Id === otherType.None) return `${translateText('player1', 'курить')}`;
    else if (_infoItem.functionType === ItemType.Cases) return `${translateText('player1', 'открыть')}`;
    else if (info.ItemId == ItemId.SimCard) return `${translateText('player1', 'Вставить')}`;
    return `${translateText('player1', 'использовать')}`;
}

const onItemsHands = () => {
    if (selectItem.use === stageItem.useItem) {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;

        executeClient ("client.gamemenu.inventory.hands", selectArrayName, selectIndex);
        itemNoUse (32);
    }
}

const getToPut = (ItemId) => {
    switch (ItemId) {
        case 234:
        case 235:
        case 236:
        case 237:
        case 238:
        case 239:
        case 240:
        case 241:
        case 242:
        case 248:
        case 229: return false;
        //case 249: return false;
    }
    return true;
}

const onToPut = () => {
    if (selectItem.use === stageItem.useItem) {
        const selectIndex = selectItem.index;
        const selectArrayName = selectItem.arrayName;

        executeClient ("client.gamemenu.inventory.toput", selectArrayName, selectIndex);
        itemNoUse (33);
    }
}

const handleMouseDownBlock = (event, eClass, arrayName) => {
    /*if (event.which == 1) {
        if (selectItem.use === stageItem.none) {
            itemNoUse (34);

            const target = document.querySelector('.box-inventory .' + eClass).getBoundingClientRect();

            const offsetInElementX = (target.width - (target.right - event.clientX));
            const offsetInElementY = (target.height - (target.bottom - event.clientY));

            selectItem = {
                use: stageItem.moveBlock,
                width: target.width,
                height: target.height,
                offsetInElementX: offsetInElementX,
                offsetInElementY: offsetInElementY,
            } 
            moveBlock[arrayName] = [
                event.clientY - offsetInElementY,
                event.clientX - offsetInElementX,
                //event.clientX - offsetInElementX
            ]

            isMoveBlock = arrayName;

        }
    }*/
}

let boxItemInfo;
let boxInfoLeft = 0;
let boxInfoTop = 0;

$: if (boxItemInfo) {
    boxInfoLeft = fixOutToCenter ($coords.x, boxItemInfo);
    boxInfoTop = fixOutToTop ($coords.y, boxItemInfo);
}

let boxPopup;

const weaponCondition = (hp, maxHp) => {
    const condition = Math.floor((hp / maxHp) * 100);
    if (!condition || isNaN(condition)) {
        return 0;
    }
    return condition;
}

let cursorX = 0;
let cursorY = 0;

function handleMouseMove(event) {
    cursorX = event.clientX;
    cursorY = event.clientY;
}

const onExit = () => {
    executeClient ("client.inventory.Close");
}

const onOpenBattlePass = () => {
    executeClient ("client.battlepass.open");
}





    

const CounterUpdate = (args, value) => {
    if (userData["timerId" + args])
        clearTimeout (userData["timerId" + args]);
    userData["change" + args] = userData[args] > value ? (0 - (userData[args] - value)) : (value - userData[args]);
    userData[args] = value;
    userData["timerId" + args] = setTimeout (() => {
        userData["timerId" + args] = 0;
        userData["change" + args] = 0;
        if (!userData["target" + args]) {
            userData["target" + args] = new CountUp("target" + args, value);
            //userData["target" + args].start();
            //userData["target" + args].update(value);
        }
        else
            userData["target" + args].update(value);
    }, !userData["target" + args] ? 0 : 5000)
}
let opened = true;
let hp = 100;
let eat = 100;
let water = 100;

const open = () => opened = true;
const close = () => opened = false;
const updateHealth = (val) => hp = val;
const updateEat = (val) => eat = val;
const updateWater = (val) => water = val;

onMount(() => {
    window.events.addEvent('cef.hp.Open', open);
    window.events.addEvent('cef.hp.Close', close);
    window.events.addEvent('cef.hp.UpdateHealth', updateHealth);
    window.events.addEvent('cef.eatwater.Open', open);
    window.events.addEvent('cef.eatwater.Close', close);
    window.events.addEvent('cef.eatwater.UpdateEat', updateEat);
    window.events.addEvent('cef.eatwater.UpdateWater', updateWater);

    window.mp.trigger("cefEatWaterReady");

    window.mp.trigger("cefReady");
});

</script>
<style>
.hud {
    display: flex;
    flex-direction: column;
    gap: 10px;
    color: white;
    padding-top: 50px;
}

.hud .indicator {
    display: flex;
    align-items: center;
    gap: 18px;
}
.hud .indicatorimg {
    display: flex;
    align-items: center;
    color: #417ff3;
    gap: 18px;
}

.hud .indicator img {
    width: 24px;
    height: 24px;
}

.hud .indicator .bar {
background-color: rgb(116, 116, 116);
    webkit-appearance: none;
-moz-appearance: none;
appearance: none;
border: none;
width: 25rem;
height: .25rem
}

.hud .indicator .bar .fill {
    background: linear-gradient(to right, #FF4040, #FF4040);
    height: 100%;
    transition: width 0.3s ease;
}

.hud .indicator .bar .fill2 {
    background: linear-gradient(to right, #40B6FF, #40B6FF);
    height: 100%;
    transition: width 0.3s ease;
}
.hud .indicator .bar .fill3 {
    background: linear-gradient(to right, #FFC640, #FFC640);
    height: 100%;
    transition: width 0.3s ease;
}


.hud .indicator span {
    min-width: 30px;
    text-align: right;
    font-size: 1.3889vh;
    font-weight: 600;
}


/* Растягиваем фон на весь экран */

</style>

<!-- Ваш контент здесь -->


<svelte:window on:mousemove={handleGlobalMouseMove} on:mousemove={handleMouseMove} on:mouseup={handleGlobalMouseUp} on:mousedown={handleGlobalMouseDown} on:keyup={onKeyUp} on:keydown={onKeyDown} />

{#if visible}

    <div class="invhax_menu">
        <div id="box-menu">
            
<div class="box-inventory">

{#if (selectItem.use === stageItem.move)}

    <div class="dragonDrop" style="width: {selectItem.width}px;height: {selectItem.height}px;top: {$coords.y - selectItem.offsetInElementY}px;left: {$coords.x - selectItem.offsetInElementX}px;">
        <Slot
            item={selectItem}
            iconInfo={window.getItem (selectItem.ItemId)} />
    </div>
{/if}

{#if (infoItem !== defaulHoverItem && selectItem.use !== stageItem.move)} 
    <div class="magiciteminfo" style="top: {fixOutToY (cursorY)}px;left: {fixOutToX (cursorX + 20)}px;" bind:this={boxPopup}>
        <h1>{@html getName (infoItem, infoItem.arrayName)}</h1>
        <p>{itemsInfo [infoItem.ItemId].Description}</p>
        {#if itemsInfo [infoItem.ItemId].functionType === ItemType.Weapons && inventoryWeapons [infoItem.ItemId]}
            <span style="color: #EE6269"> | Урон: {inventoryWeapons [infoItem.ItemId].damage}% </span>
            <span style="color: #EE6269"> | Скорость: {inventoryWeapons [infoItem.ItemId].firerate}% </span>
            {#if inventoryWeapons [infoItem.ItemId].accuracy}
                <span style="color: #EE6269"> | Точность: {inventoryWeapons [infoItem.ItemId].accuracy}% </span>
            {/if}
            <span style="color: #EE6269"> | Дальность: {inventoryWeapons [infoItem.ItemId].range}% </span>
            {#if inventoryWeapons [infoItem.ItemId].ammo}
                <span style="color: #EE6269"> | Патронов в магазине: {inventoryWeapons [infoItem.ItemId].ammo}% </span>
            {/if}
        {/if}
        {#if wMaxHP [infoItem.ItemId] && infoItem.Data && infoItem.Data.split("_") && infoItem.Data.split("_").length > 1 && infoItem.Data.split("_")[1] != undefined}
            <span style="color: #EE6269"> | Состояние: {weaponCondition (infoItem.Data.split("_")[1], wMaxHP [infoItem.ItemId])}% </span>
        {/if}
        {#if OtherInfo.Id == otherType.Tent && infoItem.Price}
            <span style="color: #EE6269">Цена за 1 ед. $ {format("money", infoItem.Price)}</span>
        {/if}
    </div>
{/if}

{#if selectItem.use === stageItem.useItem && selectItem.tent && !OtherInfo.IsMyTent}
    <div bind:this={boxPopup} class="box-stack" 
        style="top: {fixOutToY ($coords.y, boxPopup)}px;left: {fixOutToX ($coords.x, boxPopup)}px;"
        on:mouseenter={e => useInventoryArea = true} on:mouseleave={e => useInventoryArea = false}>
        <div class="box-text">
            <span class='icon {selectItem.info.Icon}' />{@html getName (selectItem, selectItem.arrayName)}
        </div>
        <div class="box-number">
            {translateText('player1', 'Кол-во')}: <input type="number" bind:value={StackValue} class="box-number-input" on:input={(event) => handleInputStackChange (event.target.value)} onBlur={onBlurStack} />
        </div>
        {#if selectItem.Count > 1}
        <div class="slider box-slider">
            <input type="range" id="stack" />
        </div>
        {/if}
        <div class="btn-slap" on:click={onBuy}>Купить</div>
        <div class="box-cancel"><span class='icon inv-close' on:click={e => selectItem = defaulSelectItem} /></div>
    </div>
{:else if selectItem.use === stageItem.useItem && ItemStack === -1}
<div bind:this={boxPopup} class="magicitemuse" 
    style="top: {fixOutToY ($coords.y)}px;left: {fixOutToX ($coords.x + 10)}px;"
    on:mouseenter={e => useInventoryArea = true} on:mouseleave={e => useInventoryArea = false}>
    
    {#if OtherInfo.Id == otherType.Tent && OtherInfo.IsMyTent && selectItem.arrayName === "other"}
        <p on:keypress={() => {}}  on:click={onTransfer}>{translateText('player1', 'забрать')}</p>
    {:else}
        {#if getItemsUse (selectItem) !== false}
            <p on:keypress={() => {}}  on:click={onUseItem}>{@html getItemsClickInfo (selectItem)}</p>
        {/if}
        {#if getToPut (selectItem.ItemId) !== false && getDropItem (selectItem.arrayName, selectItem.ItemId) !== false && selectItem.arrayName === "inventory"}
            <p on:keypress={() => {}}  on:click={onToPut}>{translateText('player1', 'поставить')}</p>
        {/if}
        {#if OtherInfo.Id != otherType.Tent && (OtherInfo.Id > otherType.None || (maxSlotBackpack > 0 && ItemsData["backpack"].length) || tradeInfo.Active === true) && selectItem.arrayName !== "fastSlots" && selectItem.arrayName !== "accessories"}
            <p on:keypress={() => {}}  on:click={onTransfer}>{(selectItem.arrayName === "other" || selectItem.arrayName === "backpack" || selectItem.arrayName === "trade") ? translateText('player1', 'взять') : translateText('player1', 'передать')}</p>
        {:else if OtherInfo.Id == otherType.Tent && OtherInfo.IsMyTent && (selectItem.arrayName === "inventory" || selectItem.arrayName === "backpack" )}
            <p on:keypress={() => {}}  on:click={onTransfer}>{translateText('player1', 'продать')}</p>
        {/if}
        {#if selectItem.Count > 1 && selectItem.arrayName !== "fastSlots"}
            <p on:keypress={() => {}}  on:click={e => {ItemStack = 0; rangeslidercreate (selectItem.Count - 1);}}>{translateText('player1', 'разделить')}</p>
        {/if}
        {#if getDropItem (selectItem.arrayName, selectItem.ItemId) !== false}
            <p on:keypress={() => {}}  on:click={onDropItem}>{translateText('player1', 'выбросить')}</p>
        {/if}
    {/if}
</div>
{:else if selectItem.use === stageItem.useItem && ItemStack !== -1}
<div bind:this={boxPopup} class="magiciteminput" 
    style="top: {fixOutToY ($coords.y, boxPopup)}px;left: {fixOutToX ($coords.x, boxPopup)}px;"
    on:mouseenter={e => useInventoryArea = true} on:mouseleave={e => useInventoryArea = false}>
    <h1>{!ItemStack ? translateText('player1', 'Разделить') : ItemStack == 1 ? translateText('player1', 'Выбросить') : translateText('player1', 'Передать')}</h1>
    <div class="closedinp" on:keypress={() => {}} on:click={e => ItemStack = -1}>
        <svg width="8" height="8" viewBox="0 0 8 8" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M1 1L4.00033 4M7.00065 7L4.00033 4M4.00033 4L7.00065 1L1 7" stroke="#949494" stroke-linecap="round" stroke-linejoin="round"></path>
        </svg>
    </div>
    <div class="dopinput">
        <input pattern="\d*" bind:value={StackValue} on:input={(event) => handleInputStackChange (event.target.value)} onBlur={onBlurStack}>
        {#if selectItem.Count > 2}
            <input bind:value={StackValue} on:input={(event) => handleInputStackChange (event.target.value)} onBlur={onBlurStack} type="range" id="dopi" min="0" max={selectItem.Count - 1} />
        {/if}
    </div>
    <div class="btninput" on:keypress={() => {}} on:click={onStack}>
        <p>Принять</p>
    </div>
</div>
{/if}
<div class="magicinv">
    {#if OtherInfo.Id == "1" || OtherInfo.Id == "2" || OtherInfo.Id == "3" || OtherInfo.Id == "4" || OtherInfo.Id == "5" || OtherInfo.Id == "6" || OtherInfo.Id == "10" || OtherInfo.Id == "12" || OtherInfo.Id == "13"}
    <div class="minvdop">
        <div class="mainleft">
            <div class="mhead">
                <svg width="20" height="20" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <g clip-path="url(#clip0_2_25)">
                    <path d="M19.0827 0.0390625H9.84602C9.36176 0.0390625 8.96773 0.433086 8.96773 0.917344V8.75125C8.96773 9.23555 9.36176 9.62957 9.84602 9.62957H19.0827C19.5669 9.62957 19.9609 9.23555 19.9609 8.75125V0.917383C19.9609 0.433086 19.5669 0.0390625 19.0827 0.0390625ZM19.3359 8.75129C19.3359 8.81845 19.3092 8.88283 19.2617 8.93032C19.2142 8.97781 19.1498 9.00453 19.0827 9.00461H9.84602C9.77886 9.00453 9.71448 8.97781 9.667 8.93032C9.61951 8.88283 9.59281 8.81845 9.59273 8.75129V0.917383C9.59282 0.850234 9.61953 0.785859 9.66701 0.738377C9.71449 0.690896 9.77887 0.664184 9.84602 0.664102H12.4754V2.57195C12.4754 2.7918 12.5988 2.98672 12.7972 3.08063C12.8727 3.11658 12.9553 3.1353 13.0389 3.13543C13.1659 3.13543 13.2912 3.0918 13.3946 3.00695L14.4643 2.13L15.5339 3.00695C15.704 3.14637 15.933 3.17453 16.1314 3.08063C16.3298 2.98672 16.4532 2.79176 16.4532 2.57195V0.664062H19.0826C19.1498 0.664145 19.2141 0.690857 19.2616 0.738338C19.3091 0.78582 19.3358 0.850195 19.3359 0.917344V8.75129H19.3359ZM4.85133 16.5422V17.9509C4.85133 18.0338 4.8184 18.1133 4.7598 18.1719C4.70119 18.2305 4.62171 18.2634 4.53883 18.2634H2.04902C1.96614 18.2634 1.88666 18.2305 1.82805 18.1719C1.76945 18.1133 1.73652 18.0338 1.73652 17.9509V16.5422C1.73652 16.4593 1.76945 16.3798 1.82805 16.3212C1.88666 16.2626 1.96614 16.2297 2.04902 16.2297H4.53883C4.62171 16.2297 4.70119 16.2626 4.7598 16.3212C4.8184 16.3798 4.85133 16.4593 4.85133 16.5422ZM13.78 6.2109V7.61961C13.78 7.70249 13.747 7.78198 13.6884 7.84058C13.6298 7.89919 13.5503 7.93211 13.4675 7.93211H10.9777C10.8948 7.93211 10.8153 7.89919 10.7567 7.84058C10.6981 7.78198 10.6652 7.70249 10.6652 7.61961V6.2109C10.6652 6.12802 10.6981 6.04853 10.7567 5.98993C10.8153 5.93132 10.8948 5.8984 10.9777 5.8984H13.4675C13.5503 5.8984 13.6298 5.93132 13.6884 5.98993C13.747 6.04853 13.78 6.12802 13.78 6.2109ZM10.154 10.3704H0.917344C0.433086 10.3704 0.0390625 10.7644 0.0390625 11.2487V19.0826C0.0390625 19.5669 0.433086 19.9609 0.917344 19.9609H10.154C10.6383 19.9609 11.0323 19.5669 11.0323 19.0827V11.2487C11.0323 10.7644 10.6383 10.3704 10.154 10.3704ZM10.4073 19.0826C10.4072 19.1498 10.3805 19.2141 10.3331 19.2616C10.2856 19.3091 10.2212 19.3358 10.1541 19.3359H0.917344C0.850192 19.3358 0.785811 19.3091 0.738327 19.2616C0.690843 19.2142 0.664135 19.1498 0.664062 19.0826V11.2487C0.664145 11.1815 0.690857 11.1171 0.738338 11.0697C0.78582 11.0222 0.850195 10.9955 0.917344 10.9954H3.5468V12.9033C3.5468 13.1231 3.67008 13.318 3.86855 13.4119C3.94406 13.4479 4.02663 13.4666 4.11027 13.4667C4.23727 13.4667 4.36258 13.4231 4.46602 13.3382L5.53566 12.4613L6.60527 13.3382C6.77531 13.4776 7.00414 13.5059 7.20273 13.4119C7.40121 13.318 7.52449 13.123 7.52449 12.9033V10.9954H10.154C10.2211 10.9955 10.2855 11.0222 10.333 11.0697C10.3805 11.1171 10.4072 11.1815 10.4072 11.2487V19.0826H10.4073ZM2.2043 9.52434C1.71258 8.06402 1.77781 6.55063 2.38805 5.26293C2.95273 4.07133 3.94559 3.145 5.20203 2.62988L5.07523 2.14762C5.02488 1.95621 5.08676 1.75652 5.2368 1.6266C5.3857 1.49762 5.5916 1.46555 5.77383 1.54277L8.04102 2.50348C8.11361 2.53394 8.17818 2.58075 8.22971 2.64027C8.28124 2.69978 8.31833 2.7704 8.33809 2.8466C8.35834 2.92266 8.36075 3.00236 8.34514 3.0795C8.32953 3.15664 8.29631 3.22913 8.24809 3.29133L6.74664 5.24281C6.6259 5.39984 6.43094 5.47301 6.23781 5.43402C6.04324 5.39469 5.89109 5.25121 5.84082 5.05965L5.74453 4.6934C3.02082 6.25461 2.81035 8.69109 2.81293 9.42355C2.81307 9.4647 2.80509 9.50547 2.78944 9.54352C2.77379 9.58157 2.75078 9.61616 2.72174 9.6453C2.69269 9.67444 2.65817 9.69756 2.62017 9.71333C2.58216 9.72909 2.54142 9.7372 2.50027 9.73719C2.43487 9.73713 2.37114 9.71656 2.31805 9.67837C2.26495 9.64019 2.22517 9.58632 2.2043 9.52434ZM17.7957 10.4757C18.2875 11.936 18.2222 13.4494 17.612 14.7371C17.0473 15.9287 16.0545 16.855 14.798 17.3701L14.9248 17.8524C14.9751 18.0439 14.9132 18.2436 14.7632 18.3734C14.6704 18.4542 14.5517 18.4986 14.4288 18.4987C14.3591 18.4986 14.2902 18.4845 14.2261 18.4572L11.9589 17.4965C11.8864 17.4661 11.8218 17.4192 11.7702 17.3597C11.7187 17.3002 11.6816 17.2296 11.6619 17.1534C11.6416 17.0773 11.6392 16.9976 11.6548 16.9205C11.6704 16.8434 11.7036 16.7709 11.7519 16.7087L13.2534 14.7572C13.3741 14.6003 13.5691 14.527 13.7621 14.566C13.9567 14.6053 14.1089 14.7487 14.1591 14.9404L14.2554 15.3066C16.9791 13.7454 17.1896 11.3089 17.187 10.5764C17.1868 10.5022 17.2129 10.4304 17.2608 10.3737C17.3087 10.317 17.3752 10.2792 17.4484 10.267C17.5217 10.2549 17.5968 10.2693 17.6604 10.3075C17.724 10.3457 17.772 10.4053 17.7957 10.4757Z" fill="#FBFBFB"></path>
                    </g>
                    <defs>
                    <clipPath id="clip0_2_25">
                    <rect width="20" height="20" fill="white"></rect>
                    </clipPath>
                    </defs>
                </svg> 
                <h1>{otherName[OtherInfo.Id].name}</h1>
                <div class="maxslot">
                    <p>{itemsWithDataCount}<b>/{ItemsData["other"].length}</b></p>
                    <svg width="20.000000" height="20.000000" viewBox="0 0 20 20" fill="none" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
                        <defs>
                            <clipPath id="clip2_138">
                                <rect id="Frame" rx="0.000000" width="19.000000" height="19.000000" transform="translate(0.500000 0.500000)" fill="white" fill-opacity="0"></rect>
                            </clipPath>
                        </defs>
                        <rect id="Frame" rx="0.000000" width="19.000000" height="19.000000" transform="translate(0.500000 0.500000)" fill="#FFFFFF" fill-opacity="0"></rect>
                        <g clip-path="url(#clip2_138)">
                            <path id="Vector" d="M12.29 7.11L14.05 7.11L14.05 9.82L12.29 9.82L12.29 7.11ZM6.54 17.34L8.89 17.34L8.89 17.8L6.54 17.8L6.54 17.34Z" fill="#FFFFFF" fill-opacity="0.250000" fill-rule="nonzero"></path>
                            <path id="Vector" d="M1.52 10.31C1.5 10.31 1.48 10.32 1.46 10.33C1.44 10.35 1.43 10.37 1.43 10.4L1.43 18.66C1.43 18.7 1.48 18.74 1.52 18.74L9.78 18.74C9.83 18.74 9.87 18.7 9.87 18.66L9.87 10.4C9.87 10.37 9.86 10.35 9.84 10.33C9.83 10.32 9.81 10.31 9.78 10.31L7.16 10.31L7.16 13.32C7.16 13.41 7.12 13.49 7.07 13.55C7.01 13.6 6.93 13.64 6.85 13.64L4.46 13.64C4.38 13.64 4.3 13.6 4.24 13.55C4.18 13.49 4.15 13.41 4.15 13.32L4.15 10.31L1.52 10.31ZM5.91 17.03C5.91 16.94 5.95 16.86 6.01 16.81C6.06 16.75 6.14 16.71 6.23 16.71L9.2 16.71C9.29 16.71 9.37 16.75 9.43 16.81C9.48 16.86 9.52 16.94 9.52 17.03L9.52 18.12C9.52 18.2 9.48 18.28 9.43 18.34C9.37 18.4 9.29 18.43 9.2 18.43L6.23 18.43C6.14 18.43 6.06 18.4 6.01 18.34C5.95 18.28 5.91 18.2 5.91 18.12L5.91 17.03ZM4.28 17.16C4.28 17.08 4.32 17 4.38 16.94C4.43 16.88 4.51 16.85 4.6 16.85C4.68 16.85 4.76 16.88 4.82 16.94C4.88 17 4.91 17.08 4.91 17.16L4.91 17.98C4.91 18.07 4.88 18.15 4.82 18.2C4.76 18.26 4.68 18.3 4.6 18.3C4.51 18.3 4.43 18.26 4.38 18.2C4.32 18.15 4.28 18.07 4.28 17.98L4.28 17.16ZM2.5 17.98C2.5 18.07 2.47 18.15 2.41 18.2C2.35 18.26 2.27 18.3 2.19 18.3C2.11 18.3 2.03 18.26 1.97 18.2C1.91 18.15 1.88 18.07 1.88 17.98L1.88 17.16C1.88 17.08 1.91 17 1.97 16.94C2.03 16.88 2.11 16.85 2.19 16.85C2.27 16.85 2.35 16.88 2.41 16.94C2.47 17 2.5 17.08 2.5 17.16L2.5 17.98ZM3.48 17.16C3.48 17.08 3.52 17 3.57 16.94C3.63 16.88 3.71 16.85 3.79 16.85C3.88 16.85 3.96 16.88 4.02 16.94C4.07 17 4.11 17.08 4.11 17.16L4.11 17.98C4.11 18.07 4.07 18.15 4.02 18.2C3.96 18.26 3.88 18.3 3.79 18.3C3.71 18.3 3.63 18.26 3.57 18.2C3.52 18.15 3.48 18.07 3.48 17.98L3.48 17.16ZM3.31 17.16L3.31 17.98C3.31 18.07 3.27 18.15 3.21 18.2C3.15 18.26 3.08 18.3 2.99 18.3C2.91 18.3 2.83 18.26 2.77 18.2C2.71 18.15 2.68 18.07 2.68 17.98L2.68 17.16C2.68 17.08 2.71 17 2.77 16.94C2.83 16.88 2.91 16.85 2.99 16.85C3.08 16.85 3.15 16.88 3.21 16.94C3.27 17 3.31 17.08 3.31 17.16ZM9.3 1.25L11.06 1.25L11.06 4.13L9.3 4.13L9.3 1.25ZM14.67 17.06L17.44 17.06L17.44 17.67L14.67 17.67L14.67 17.06Z" fill="#FFFFFF" fill-opacity="0.250000" fill-rule="nonzero"></path>
                            <path id="Vector" d="M10.49 18.75L18.56 18.75L18.56 7.11L14.67 7.11L14.67 10.13C14.67 10.21 14.64 10.29 14.58 10.35C14.52 10.41 14.44 10.44 14.36 10.44L11.97 10.44C11.89 10.44 11.81 10.41 11.75 10.35C11.69 10.29 11.66 10.21 11.66 10.13L11.66 7.11L7.77 7.11L7.77 9.68L9.78 9.68C9.97 9.68 10.15 9.76 10.29 9.89C10.42 10.03 10.5 10.21 10.5 10.4L10.5 18.66C10.5 18.69 10.49 18.72 10.49 18.74L10.49 18.75ZM14.05 16.74C14.05 16.66 14.08 16.58 14.14 16.52C14.2 16.46 14.27 16.43 14.36 16.43L17.75 16.43C17.84 16.43 17.92 16.46 17.97 16.52C18.03 16.58 18.07 16.66 18.07 16.74L18.07 17.98C18.07 18.07 18.03 18.15 17.97 18.2C17.92 18.26 17.84 18.3 17.75 18.3L14.36 18.3C14.27 18.3 14.2 18.26 14.14 18.2C14.08 18.15 14.05 18.07 14.05 17.98L14.05 16.74ZM4.77 10.31L6.53 10.31L6.53 13.01L4.77 13.01L4.77 10.31Z" fill="#FFFFFF" fill-opacity="0.250000" fill-rule="nonzero"></path>
                            <path id="Vector" d="M3.44 1.25L3.44 9.68L7.15 9.68L7.15 6.8C7.15 6.72 7.18 6.64 7.24 6.58C7.3 6.52 7.38 6.49 7.46 6.49L16.93 6.49L16.93 1.25L11.69 1.25L11.69 4.44C11.69 4.53 11.66 4.61 11.6 4.66C11.54 4.72 11.46 4.76 11.38 4.76L8.99 4.76C8.91 4.76 8.83 4.72 8.77 4.66C8.71 4.61 8.68 4.53 8.68 4.44L8.68 1.25L3.44 1.25Z" fill="#FFFFFF" fill-opacity="0.250000" fill-rule="nonzero"></path>
                        </g>
                    </svg>
                </div>
                <div class="inputbg">
                    <svg width="14" height="14" viewBox="0 0 14 14" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <g clip-path="url(#clip0_2_21)">
                        <path d="M13.8289 13.0042L9.84773 9.02303C10.6189 8.07047 11.0832 6.86005 11.0832 5.54174C11.0832 2.48627 8.59706 0.00012207 5.54159 0.00012207C2.48612 0.00012207 0 2.48625 0 5.54172C0 8.59719 2.48615 11.0833 5.54162 11.0833C6.85993 11.0833 8.07035 10.619 9.02291 9.84785L13.0041 13.8291C13.1179 13.9428 13.2672 14 13.4165 14C13.5659 14 13.7152 13.9428 13.829 13.8291C14.057 13.601 14.057 13.2323 13.8289 13.0042ZM5.54162 9.91667C3.12897 9.91667 1.16666 7.95437 1.16666 5.54172C1.16666 3.12906 3.12897 1.16676 5.54162 1.16676C7.95427 1.16676 9.91658 3.12906 9.91658 5.54172C9.91658 7.95437 7.95425 9.91667 5.54162 9.91667Z" fill="white" fill-opacity="0.15"></path>
                        </g>
                        <defs>
                        <clipPath id="clip0_2_21">
                        <rect width="14" height="14" fill="white"></rect>
                        </clipPath>
                        </defs>
                    </svg>
                    <input bind:value={searchText} placeholder="Поиск...">                            
                </div>                    
            </div>
            <div class="itemslist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                {#each ItemsData["other"] as item, index}
                    {#if (!searchText || !searchText.length) || (searchText && itemsInfo[item.ItemId].Name.toLowerCase().trim().includes(searchText.toLowerCase().trim()))}
                        <Slot2
                            key={index}
                            item={item}
                            iconInfo={item ? window.getItem(item.ItemId) : null}
                            on:mousedown={(event) => handleMouseDown(event, index, "other")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "other")}
                            on:mouseleave={handleSlotMouseLeave} />
                    {/if}
                {/each}
            </div>
        </div>
        <div class="mainright">
            <div class="mhead">
                <p class:active={selcetinv} on:keypress={() => {}} on:click={() => onSelectedInv (true)}>Предметы</p>
                <p class:active={!selcetinv} on:keypress={() => {}} on:click={() => onSelectedInv (false)}>Мой инвентарь</p>
                <span on:keypress={() => {}} on:click={() => onExit ()}>
                    <svg width="10" height="10" viewBox="0 0 10 10" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M0.999207 0.999756L4.99921 4.99976M8.99921 8.99976L4.99921 4.99976M4.99921 4.99976L8.99921 0.999756L0.999207 8.99976" stroke="#949494" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"></path>
                    </svg>
                </span>
            </div>
            <div class="itemslist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                {#if selcetinv}
                    {#each ItemsData["other"] as item, index}
                        {#if (!searchText || !searchText.length) || (searchText && itemsInfo[item.ItemId].Name.toLowerCase().trim().includes(searchText.toLowerCase().trim()))}
                            <Slot3
                                key={index}
                                item={item}
                                iconInfo={item ? window.getItem(item.ItemId) : null}
                                {onTransfer}
                                {selcetinv}
                                on:mousedown={(event) => handleMouseDown(event, index, "other")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, index, "other")}
                                on:mouseleave={handleSlotMouseLeave} />
                        {/if}
                    {/each}
                    {:else}
                        {#each ItemsData["inventory"] as item, index}
                                <Slot3
                                    key={index}
                                    item={item}
                                    {onTransfer}
                                    {selcetinv}
                                    iconInfo={item ? window.getItem(item.ItemId) : null}
                                    on:mousedown={(event) => handleMouseDown(event, index, "inventory")}
                                    on:mouseup={handleSlotMouseUp}
                                    on:mouseenter={(event) => handleSlotMouseEnter(event, index, "inventory")}
                                    on:mouseleave={handleSlotMouseLeave} />
                        {/each}
                {/if}
            </div>
        </div>
    </div>
    {:else}
      
            {#if tradeInfo.Active === false}
            <div class="minvosn">
                <div class="closedinv" on:keypress={() => {}} on:click={() => onExit ()}>
                    <svg width="10" height="10" viewBox="0 0 10 10" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M0.999268 0.999756L4.99927 4.99976M8.99927 8.99976L4.99927 4.99976M4.99927 4.99976L8.99927 0.999756L0.999268 8.99976" stroke="#949494" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                    </svg>                    
                </div>
                <div class="leftinv">
                    <h1>Окружение</h1>
                    <div class="itemlist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                        {#each ItemsData["other"] as item, index}
                            <Slot
                                key={index}
                                item={item}
                                iconInfo={item ? window.getItem(item.ItemId) : null}
                                on:mousedown={(event) => handleMouseDown(event, index, "other")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, index, "other")}
                                on:mouseleave={handleSlotMouseLeave} />
                        {/each}
                    </div>
                    {#if maxSlotBackpack > 0 && ItemsData["backpack"].length && ItemsData["backpack"][0] !== undefined && ItemsData["backpack"][0].use !== undefined}
                        <h1>Рюкзак</h1>
                        <div class="itemlist max" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                            {#each ItemsData["backpack"] as item, index}
                                <Slot
                                    key={index}
                                    item={item}
                                    iconInfo={item ? window.getItem(item.ItemId) : null}
                                    on:mousedown={(event) => handleMouseDown(event, index, "backpack")}
                                    on:mouseup={handleSlotMouseUp}
                                    on:mouseenter={(event) => handleSlotMouseEnter(event, index, "backpack")}
                                    on:mouseleave={handleSlotMouseLeave} />
                            {/each}
                        </div>
                    {/if}
                </div>
                <div class="centerinv">
                    <h1>{selectCharData.Name}</h1>
                    <div class="invpers">
                        <div class="lpers" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Hats.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Hats.slotId].ItemId)}
                                defaultIcon={clothes.Hats.icon}
                                defaultName={translateText('player1', 'шапка')}
                                defaultNamei="head"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Hats.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Hats.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Tops.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Tops.slotId].ItemId)}
                                defaultIcon={clothes.Tops.icon}
                                defaultName={translateText('player1', 'верх')}
                                defaultNamei="tops"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Tops.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Tops.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Accessories.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Accessories.slotId].ItemId)}
                                defaultIcon={clothes.Accessories.icon}
                                defaultName={translateText('player1', 'шея')}
                                defaultNamei="decals"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Accessories.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Accessories.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Legs.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Legs.slotId].ItemId)}
                                defaultIcon={clothes.Legs.icon}
                                defaultName={translateText('player1', 'штаны')}
                                defaultNamei="legs"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Legs.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Legs.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Watches.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Watches.slotId].ItemId)}
                                defaultIcon={clothes.Watches.icon}
                                defaultName={translateText('player1', 'часы')}
                                defaultNamei="watches"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Watches.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Watches.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                        </div>
                        <div class="cpers">
                            <img src="https://cdn.majestic-files.com/public/master/static/img/inventory/male.png" alt="">
                            <div class="listitem" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                                <SlotAccs
                                    item={ItemsData["accessories"][clothes.Masks.slotId]}
                                    iconInfo={window.getItem (ItemsData["accessories"][clothes.Masks.slotId].ItemId)}
                                    defaultIcon={clothes.Masks.icon}
                                    defaultName={translateText('player1', 'маска')}
                                    defaultNamei="masks"
                                    on:mousedown={(event) => handleMouseDown(event, clothes.Masks.slotId, "accessories")}
                                    on:mouseup={handleSlotMouseUp}
                                    on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Masks.slotId, "accessories")}
                                    on:mouseleave={handleSlotMouseLeave} />
                                <SlotAccs
                                    item={ItemsData["accessories"][clothes.Ears.slotId]}
                                    iconInfo={window.getItem (ItemsData["accessories"][clothes.Ears.slotId].ItemId)}
                                    defaultIcon={clothes.Ears.icon}
                                    defaultName={translateText('player1', 'уши')}
                                    defaultNamei="ears"
                                    on:mousedown={(event) => handleMouseDown(event, clothes.Ears.slotId, "accessories")}
                                    on:mouseup={handleSlotMouseUp}
                                    on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Ears.slotId, "accessories")}
                                    on:mouseleave={handleSlotMouseLeave} />
                                <SlotAccs
                                    item={ItemsData["accessories"][clothes.Bracelets.slotId]}
                                    iconInfo={window.getItem (ItemsData["accessories"][clothes.Bracelets.slotId].ItemId)}
                                    defaultIcon={clothes.Bracelets.icon}
                                    defaultName={translateText('player1', 'браслет')}
                                    defaultNamei="bracelets"
                                    on:mousedown={(event) => handleMouseDown(event, clothes.Bracelets.slotId, "accessories")}
                                    on:mouseup={handleSlotMouseUp}
                                    on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Bracelets.slotId, "accessories")}
                                    on:mouseleave={handleSlotMouseLeave} /> 
                            </div>
                        </div>
                        <div class="rpers" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Glasses.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Glasses.slotId].ItemId)}
                                defaultIcon={clothes.Glasses.icon}
                                defaultName={translateText('player1', 'очки')}
                                defaultNamei="glasses"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Glasses.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Glasses.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Undershirts.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Undershirts.slotId].ItemId)}                               
                                defaultIcon={clothes.Undershirts.icon}
                                defaultName={translateText('player1', 'футболка')}
                                defaultNamei="undershirts"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Undershirts.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Undershirts.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Shoes.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Shoes.slotId].ItemId)}
                                defaultIcon={clothes.Shoes.icon}
                                defaultName={translateText('player1', 'обувь')}
                                defaultNamei="shoes"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Shoes.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Shoes.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Torsos.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Torsos.slotId].ItemId)}
                                defaultIcon={"inv-item-hand-right"}
                                defaultName={translateText('player1', 'перчатки')}
                                defaultNamei="gloves"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Torsos.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Torsos.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                            <SlotAccs
                                item={ItemsData["accessories"][clothes.Suit.slotId]}
                                iconInfo={window.getItem (ItemsData["accessories"][clothes.Suit.slotId].ItemId)}
                                defaultIcon={clothes.Suit.icon}
                                defaultName={translateText('player1', 'украшения')}
                                defaultNamei="accessories"
                                on:mousedown={(event) => handleMouseDown(event, clothes.Suit.slotId, "accessories")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Suit.slotId, "accessories")}
                                on:mouseleave={handleSlotMouseLeave} />
                        </div>
                    </div>
                    <div class="hud" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>

                        {#if opened}
                        <div class="indicator">
                            <img src="http://cdn.piecerp.ru/cloud/icons/water.svg" alt="water" />
                            <div class="bar">
                                <div class="fill2" style="width: {water}%"></div>
                            </div>
                            <span>{water}</span>
                        </div>
                        <div class="indicator">
                            <img src="http://cdn.piecerp.ru/cloud/icons/hunger.svg" alt="hunger" />
                            <div class="bar">
                                <div class="fill3" style="width: {eat}%"></div>
                            </div>
                            <span>{eat}</span>
                        </div>
                        <div class="indicator">
                            <img src="http://cdn.piecerp.ru/cloud/icons/health.svg" alt="HP" />
                            <div class="bar">
                                <div class="fill" style="width: {hp}%"></div>
                            </div>
                            <span>{hp}</span>
                        </div>
                        
                       
                        {/if}
                  
                    </div>
                    
                    <div class="dopslots" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                        
                        <SlotAccs
                            item={ItemsData["accessories"][clothes.Armors.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Armors.slotId].ItemId)}
                            defaultIcon={clothes.Armors.icon}
                            defaultName={translateText('player1', 'бронежилет')}
                            defaultNamei="armour"
                            on:mousedown={(event) => handleMouseDown(event, clothes.Armors.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Armors.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <SlotAccs
                            item={ItemsData["accessories"][clothes.Bags.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Bags.slotId].ItemId)}
                            defaultIcon={clothes.Bags.icon}
                            defaultName={translateText('player1', 'рюкзак')}
                            defaultNamei="bags"
                            on:mousedown={(event) => handleMouseDown(event, clothes.Bags.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Bags.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                    </div>
                    <div class="fastslot" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                        {#each ItemsData["fastSlots"] as item, index}
                            <Slot4 
                                key={index}
                                index={fastSlots [index]}
                                item={item}
                                iconInfo={item ? window.getItem(item.ItemId) : null}
                                defaultStyle="smoll"
                                on:mousedown={(event) => handleMouseDown(event, index, "fastSlots")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, index, "fastSlots")}
                                on:mouseleave={handleSlotMouseLeave} />
                        {/each}
                    </div>
                </div>
                <div class="rightinv">
                    <h1>Инвентарь</h1>
                    <div class="itemlist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                        {#each ItemsData["inventory"] as item, index}
                            <Slot
                                key={index}
                                item={item}
                                iconInfo={item ? window.getItem(item.ItemId) : null}
                                on:mousedown={(event) => handleMouseDown(event, index, "inventory")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, index, "inventory")}
                                on:mouseleave={handleSlotMouseLeave} />
                        {/each}
                    </div>
                </div>
            </div>
        {:else}
        <div class="minvtrade">
            <div class="closedinv" on:keypress={() => {}} on:click={() => onExit ()}>
                <svg width="10" height="10" viewBox="0 0 10 10" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M0.999268 0.999756L4.99927 4.99976M8.99927 8.99976L4.99927 4.99976M4.99927 4.99976L8.99927 0.999756L0.999268 8.99976" stroke="#949494" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>                    
            </div>
            <div class="leftinv">
                <div class="lihead">
                    <h1>{tradeInfo.WithName}</h1>
                    {#if tradeInfo.WithMoney > 0}
                        <p>Доплата вам:<b>{tradeInfo.WithMoney}$</b></p>
                    {/if}
                </div>
                <div class="itemlist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    {#if tradeInfo.WithStatus}
                        <div class="activetrade"></div>
                    {/if}
                    {#each ItemsData["with_trade"] as item, index}
                        <Slot
                            key={index}
                            item={item}
                            iconInfo={item ? window.getItem(item.ItemId) : null}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "with_trade")}
                            on:mouseleave={handleSlotMouseLeave} />
                    {/each}
                </div>
                <div class="btntrade" class:active={tradeInfo.WithStatus}>
                    {#if tradeInfo.WithStatus}
                        <p>Готов к обмену</p>
                        {:else}
                        <p>Ожидание</p>
                    {/if}
                </div>
            </div>
            <div class="centerinv">
                <h1>Предметы для обмена</h1>
                <div class="itemlist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    {#if tradeInfo.YourStatus}
                        <div class="activetrade"></div>
                    {/if}
                    {#each ItemsData["trade"] as item, index}
                        <Slot
                            key={index}
                            item={item}
                            iconInfo={item ? window.getItem(item.ItemId) : null}
                            on:mousedown={(event) => handleMouseDown(event, index, "trade")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "trade")}
                            on:mouseleave={handleSlotMouseLeave} />
                    {/each}
                </div>
                <input type="number" bind:value={tradeInfo.YourMoney} class="input" on:input={(event) => handleInputChange ("YourMoney", event.target.value)} onBlur={onBlur} placeholder="Введите сумму доплаты" disabled={!(!tradeInfo.YourStatusChange && !tradeInfo.YourStatus)}>
                <div class="checktrade">
                    <div class="yestrade" class:active={tradeInfo.YourStatus} on:keypress={() => {}} on:click={TradeSelect}>
                        <svg width="38" height="33" viewBox="0 0 38 33" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <g filter="url(#filter0_d_1_2123)">
                            <path d="M27.5818 10.4181C27.0248 9.86045 26.1205 9.8608 25.5628 10.4181L16.4762 19.5051L12.4375 15.4665C11.8799 14.9089 10.9759 14.9089 10.4182 15.4665C9.86058 16.0242 9.86058 16.9282 10.4182 17.4858L15.4663 22.5339C15.745 22.8125 16.1104 22.9522 16.4758 22.9522C16.8412 22.9522 17.2069 22.8129 17.4856 22.5339L27.5818 12.4374C28.1394 11.8801 28.1394 10.9757 27.5818 10.4181Z" fill="white"></path>
                            </g>
                            <defs>
                            <filter id="filter0_d_1_2123" x="0" y="0" width="38" height="32.9521" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
                            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
                            <feOffset></feOffset>
                            <feGaussianBlur stdDeviation="5"></feGaussianBlur>
                            <feComposite in2="hardAlpha" operator="out"></feComposite>
                            <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.25 0"></feColorMatrix>
                            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_1_2123"></feBlend>
                            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_1_2123" result="shape"></feBlend>
                            </filter>
                            </defs>
                        </svg>                                                       
                    </div>
                    <p>Подтвердите ваши действия</p>
                </div>
                {#if tradeInfo.YourStatus}
                    {#if tradeInfo.YourStatusChange === true && tradeInfo.WithStatus === true}
                        <div class="btntrade" class:active={tradeInfo.YourStatusChange}>
                            <p>Готов к обмену</p>
                        </div>
                        {:else}
                        <div class="btntrade" on:keypress={() => {}} on:click={TradeSelect}>
                            <p>Потдвердить обмен</p>
                        </div>
                    {/if}
                    {:else}
                    <div class="btntrade" on:keypress={() => {}} on:click={TradeCancel}>
                        <p>Отменить</p>
                    </div>
                {/if}
            </div>
            <div class="rightinv">
                <h1>Инвентарь</h1>
                <div class="itemlist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    {#each ItemsData["inventory"] as item, index}
                        <Slot
                            key={index}
                            item={item}
                            iconInfo={item ? window.getItem(item.ItemId) : null}
                            on:mousedown={(event) => handleMouseDown(event, index, "inventory")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "inventory")}
                            on:mouseleave={handleSlotMouseLeave} />
                    {/each}
                </div>
                {#if maxSlotBackpack > 0 && ItemsData["backpack"].length && ItemsData["backpack"][0] !== undefined && ItemsData["backpack"][0].use !== undefined}
                    <h1>Рюкзак</h1>
                    <div class="itemlist" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                        {#each ItemsData["backpack"] as item, index}
                            <Slot
                                key={index}
                                item={item}
                                iconInfo={item ? window.getItem(item.ItemId) : null}
                                on:mousedown={(event) => handleMouseDown(event, index, "backpack")}
                                on:mouseup={handleSlotMouseUp}
                                on:mouseenter={(event) => handleSlotMouseEnter(event, index, "backpack")}
                                on:mouseleave={handleSlotMouseLeave} />
                                {/each}
                            </div>
                        {/if}
                    </div>
                </div>
                {/if}
            {/if}
        </div>
    </div>
</div>
</div>

    {/if}

