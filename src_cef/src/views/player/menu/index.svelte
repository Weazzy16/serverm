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
// ✅ ПОДПИСКА НА ВЕС ИЗ charData
$: {
    if ($charData) {
        inventoryWeight = $charData.InventoryWeight || 0;
        maxInventoryWeight = $charData.MaxInventoryWeight || 50;
        backpackWeight = $charData.BackpackWeight || 0;
        maxBackpackWeight = $charData.MaxBackpackWeight || 30;
    }
}
    
    let activeItem = null; 
    let cdn = "https://cdn.majestic-files.com/public/master/static";

    import inventoryWeapons from 'json/inventoryweapons.js'
    

    let useVisible = -1;

    export let selectCharData;

    $: {
        if (useVisible != visible) {
            if (visible && $otherStatsData.Name) {
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

    // ========================
    // ✅ ФУНКЦИЯ РАЗМЕРА ПРЕДМЕТА
    // ========================
    function getItemSize(item, turnRotation = false) {
        const baseSize = 5.02778; // vh
        
        if (!item || !item.ItemId) {
            return `width: ${baseSize}vh; height: ${baseSize}vh;`;
        }
        
        const itemConfig = itemsInfo[item.ItemId] || {};
        const width = itemConfig.Width || 1;
        const height = itemConfig.Height || 1;
        const isTurned = item.isTurn || false;
        
        let resultWidth = width;
        let resultHeight = height;
        let transform = '';
        
        if (isTurned) {
            resultWidth = height;
            resultHeight = width;
            if (turnRotation) {
                transform = 'transform: rotate(90deg);';
            }
        }
        
        return `width: ${baseSize * resultWidth}vh; height: ${baseSize * resultHeight}vh; ${transform}`;
    }

    // ========================
    // ✅ ПРОВЕРКА МОЖНО ЛИ ПОЛОЖИТЬ
    // ========================
    function checkSlot(matrix, item, startX, startY) {
        if (!item || !matrix || !item.ItemId) return false;
        
        const itemConfig = itemsInfo[item.ItemId] || {};
        
        // ✅ УЧИТЫВАЕМ ПОВОРОТ
        const width = item.isTurn ? (itemConfig.Height || 1) : (itemConfig.Width || 1);
        const height = item.isTurn ? (itemConfig.Width || 1) : (itemConfig.Height || 1);
        
        // Проверяем границы
        if (startY + height > matrix.length || startX + width > (matrix[0]?.length || 0)) {
            return false;
        }
        
        // Проверяем занятость слотов
        for (let y = startY; y < startY + height; y++) {
            for (let x = startX; x < startX + width; x++) {
                const slotItem = matrix[y]?.[x];
                // Слот занят другим предметом (проверяем по SqlId чтобы игнорировать сам предмет)
                if (slotItem && slotItem.SqlId && slotItem.SqlId !== item.SqlId) {
                    return false;
                }
            }
        }
        
        return true;
    }
    // ========================
    // ✅ СОЗДАНИЕ МАТРИЦЫ
    // ========================
    function createMatrix(arrayName) {
        let rows = 17, cols = 5;
        
        switch(arrayName) {
            case "other":
                rows = 19;
                break;
            case "backpack":
                rows = 6;
                break;
            case "inventory":
                rows = 17;
                break;
        }
        
        const matrix = Array(rows).fill(null).map(() => Array(cols).fill(null));
        
        const items = ItemsData[arrayName] || [];
        items.forEach((item, index) => {
            if (!item || !item.ItemId || item.ItemId === 0) return;
            
            const x = index % cols;
            const y = Math.floor(index / cols);
            
            const itemConfig = itemsInfo[item.ItemId] || {};
            const width = item.isTurn ? (itemConfig.Height || 1) : (itemConfig.Width || 1);
            const height = item.isTurn ? (itemConfig.Width || 1) : (itemConfig.Height || 1);
            
            for (let dy = 0; dy < height; dy++) {
                for (let dx = 0; dx < width; dx++) {
                    if (y + dy < rows && x + dx < cols) {
                        matrix[y + dy][x + dx] = item;
                    }
                }
            }
        });
        
        return matrix;
    }

    let slotSize = 0;
    let handler = {
        width: 0,
        height: 0,
        offsetX: 0,
        offsetY: 0
    };

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
        isDragging = false,
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
            
            YourStatus: false,
            YourStatusChange: false,
            YourMoney: 0,

            WithName: "Deluxe",
            WithStatus: false,
            WithStatusChange: false,
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
        const result = LoadData(maxSlots[arrayName], itemsArray[arrayName], use);
        ItemsData[arrayName] = result.items;
        
        // ✅ ОБНОВЛЯЕМ ВЕС ИНВЕНТАРЯ
        if (arrayName === "inventory") {
            inventoryWeight = result.weight;
        }
    }
}

let maxSlotBackpack = 30;
const InitMyData = (maxSlot, json, use = true) => {
    maxSlotBackpack = maxSlot;
    const result = LoadData(maxSlot, JSON.parse(json), use);
    ItemsData["backpack"] = result.items;
    
    // ✅ ОБНОВЛЯЕМ ВЕС РЮКЗАКА
    backpackWeight = result.weight;
}

const UpdateSpecialVars = (isInVehicle_info = false) => {
    isInVehicle = isInVehicle_info;
}

const LoadData = (maxSlot, json, use = true) => {
    let returnArray = [];
    let indexItem;
    let itemsIndex = 0;
    let itemsArray = json;
    
    // ✅ ПЕРЕМЕННАЯ ДЛЯ ПОДСЧЁТА ВЕСА
    let totalWeight = 0;
    
    Array(maxSlot).fill(0).forEach((item, index) => {
        if (itemsArray && itemsArray.length && itemsArray[itemsIndex]) {
            item = itemsArray[itemsIndex];
            indexItem = item.Index;
        } else {
            indexItem = -1;
        }

        if (indexItem === index) {
            itemsIndex++;
            
            // ✅ РАССЧИТЫВАЕМ ВЕС ПРЕДМЕТА
            if (item.ItemId && item.ItemId !== 0) {
                const itemConfig = itemsInfo[item.ItemId] || {};
                const itemWeight = itemConfig.Weight || 0;
                const count = item.Count || 1;
                totalWeight += itemWeight * count;
            }
            
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
    
    return { items: returnArray, weight: totalWeight }; // ✅ ВОЗВРАЩАЕМ МАССИВ + ВЕС
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
        
        // CTRL (прозрачность)
        if (event.which === 17 && invOldOpacity === -1) {
            invOldOpacity = invOpacity;
            invOpacity = 0;
        }
        
        // ✅ ПРОБЕЛ (ПОВОРОТ ПРЕДМЕТА)
        if (event.which === 32 && isDragging && selectItem.ItemId) {
            event.preventDefault();
            event.stopPropagation();
            
            const itemConfig = itemsInfo[selectItem.ItemId] || {};
            const width = itemConfig.Width || 1;
            const height = itemConfig.Height || 1;
            
            // Если предмет квадратный (1x1, 2x2 и т.д.) - не поворачиваем
            if (width === height) return;
            
            // ✅ ПЕРЕКЛЮЧАЕМ ПОВОРОТ (РЕАКТИВНОСТЬ!)
            selectItem = {
                ...selectItem,
                isTurn: !selectItem.isTurn
            };
            
            // ✅ МЕНЯЕМ МЕСТАМИ РАЗМЕРЫ HANDLER
            const oldWidth = handler.width;
            const oldHeight = handler.height;
            handler = {
                ...handler,
                width: oldHeight,
                height: oldWidth,
                offsetX: oldHeight / 2,
                offsetY: oldWidth / 2
            };
            
            // ✅ ОБНОВЛЯЕМ КООРДИНАТЫ ЧТОБЫ ПРЕДМЕТ НЕ "ПРЫГАЛ"
            coords.update(c => ({
                x: c.x,
                y: c.y
            }));
        }
    }

    const onKeyUp = (event) => {  
        if (!visible) return;
        
        // CTRL (возврат прозрачности)
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
            };
        }
        
        // ✅ HIGHLIGHT С ПРАВИЛЬНОЙ ПОЗИЦИЕЙ
        if (isDragging && selectItem.ItemId) {
            // Очищаем все предыдущие highlights
            document.querySelectorAll('.highlight').forEach(el => {
                el.style.backgroundColor = "";
                el.style.width = "0";
                el.style.height = "0";
            });
            
            const slot = event.currentTarget;
            if (!slot) return;
            
            // ✅ ПОЛУЧАЕМ КООРДИНАТЫ ТЕКУЩЕГО СЛОТА
            const slotX = parseInt(slot.dataset.x || 0);
            const slotY = parseInt(slot.dataset.y || 0);
            
            // ✅ ПОЛУЧАЕМ РАЗМЕРЫ ПРЕДМЕТА
            const itemConfig = itemsInfo[selectItem.ItemId] || {};
            const itemWidth = selectItem.isTurn ? (itemConfig.Height || 1) : (itemConfig.Width || 1);
            const itemHeight = selectItem.isTurn ? (itemConfig.Width || 1) : (itemConfig.Height || 1);
            
            // ✅ ВЫЧИСЛЯЕМ OFFSET ОТ МЕСТА ЗАХВАТА (в слотах)
            const offsetSlotX = Math.floor(handler.offsetX / slotSize);
            const offsetSlotY = Math.floor(handler.offsetY / slotSize);
            
            // ✅ СТАРТОВАЯ ПОЗИЦИЯ ПРЕДМЕТА = ТЕКУЩИЙ СЛОТ - OFFSET
            const startX = slotX - offsetSlotX;
            const startY = slotY - offsetSlotY;
            
            // ✅ ОПРЕДЕЛЯЕМ РАЗМЕРЫ ИНВЕНТАРЯ
            let maxCols = 5;
            let maxRows = 17;
            
            switch(arrayName) {
                case "other":
                    maxRows = 19;
                    break;
                case "backpack":
                    maxRows = 6;
                    break;
                case "inventory":
                    maxRows = 17;
                    break;
                case "accessories":
                    return; // В одежду нельзя так класть
                case "fastSlots":
                    maxCols = 5;
                    maxRows = 1;
                    break;
            }
            
            // ✅ ПРОВЕРКА ВЫХОДА ЗА ГРАНИЦЫ СЛЕВА/СВЕРХУ
            if (startX < 0 || startY < 0) {
                // Показываем красный на текущем слоте
                const highlight = slot.querySelector('.highlight');
                if (highlight) {
                    highlight.style.backgroundColor = "rgba(247, 20, 43, 0.3)";
                    highlight.style.width = `${handler.width}px`;
                    highlight.style.height = `${handler.height}px`;
                }
                return;
            }
            
            // ✅ ПРОВЕРКА ВЫХОДА ЗА ГРАНИЦЫ СПРАВА/СНИЗУ
            const exceedsRight = (startX + itemWidth) > maxCols;
            const exceedsBottom = (startY + itemHeight) > maxRows;
            
            if (exceedsRight || exceedsBottom) {
                const highlight = slot.querySelector('.highlight');
                if (highlight) {
                    highlight.style.backgroundColor = "rgba(247, 20, 43, 0.3)";
                    highlight.style.width = `${handler.width}px`;
                    highlight.style.height = `${handler.height}px`;
                }
                return;
            }
            
            // ✅ НАХОДИМ СТАРТОВЫЙ СЛОТ (ГДЕ НАЧИНАЕТСЯ ПРЕДМЕТ)
            const startSlot = document.querySelector(
                `.slot[data-position="${getPositionId(arrayName)}"][data-x="${startX}"][data-y="${startY}"]`
            );
            
            if (!startSlot) {
                const highlight = slot.querySelector('.highlight');
                if (highlight) {
                    highlight.style.backgroundColor = "rgba(247, 20, 43, 0.3)";
                    highlight.style.width = `${handler.width}px`;
                    highlight.style.height = `${handler.height}px`;
                }
                return;
            }
            
            const startHighlight = startSlot.querySelector('.highlight');
            if (!startHighlight) return;
            
            // ✅ ПРОВЕРЯЕМ МОЖНО ЛИ ПОЛОЖИТЬ (ЗАНЯТОСТЬ СЛОТОВ)
            const matrix = createMatrix(arrayName);
            const canPlace = checkSlot(matrix, selectItem, startX, startY);
            
            // 🟢 Зелёный / 🔴 Красный
            if (canPlace) {
                startHighlight.style.backgroundColor = "rgba(105, 240, 108, 0.3)";
            } else {
                startHighlight.style.backgroundColor = "rgba(247, 20, 43, 0.3)";
            }
            
            startHighlight.style.width = `${handler.width}px`;
            startHighlight.style.height = `${handler.height}px`;
        }
        
        // Инфо о предмете
        if (selectItem.use !== stageItem.useItem && 
            selectItem.use !== stageItem.move && 
            getItemToIndex(index, arrayName).ItemId) {
            
            const target = event.target.getBoundingClientRect();
            coords.set({ x: (target.x + target.width/2), y: target.y });
            infoItem = {
                ...getItemToIndex(index, arrayName),
                index: index,
                arrayName: arrayName
            };
        }
    }
    
    // ✅ ВСПОМОГАТЕЛЬНАЯ ФУНКЦИЯ (если её ещё нет)
    

// Когда выходим из зоны ячейки
const handleSlotMouseLeave = (event) => {
        // Сброс highlight
        const slot = event.currentTarget;
        const highlight = slot.querySelector('.highlight');
        if (highlight) {
            highlight.style.backgroundColor = "";
            highlight.style.width = "0";
            highlight.style.height = "0";
        }
        
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
            executeClient("sounds.playInterface", "inventory/keys", 0.005);
            
            const item = getItemToIndex(index, arrayName);

            if (((selectItem.use === stageItem.info && 
                 (selectItem.index !== index || selectItem.arrayName !== arrayName)) ||
                 selectItem.use !== stageItem.info) && 
                 item.ItemId != 0 && item.use) {

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

                itemNoUse(9);

                const target = event.target.getBoundingClientRect();
                const offsetInElementX = (target.width - (target.right - event.clientX));
                const offsetInElementY = (target.height - (target.bottom - event.clientY));

                coords.set({ x: event.clientX, y: event.clientY });
                clickTime = new Date().getTime() + 1000;
                
                isDragging = true;
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
                    arrayName: arrayName,
                    isTurn: item.isTurn || false
                }

                const itemConfig = itemsInfo[item.ItemId] || {};
                const width = selectItem.isTurn ? (itemConfig.Height || 1) : (itemConfig.Width || 1);
                const height = selectItem.isTurn ? (itemConfig.Width || 1) : (itemConfig.Height || 1);
                
                handler.width = width * slotSize;
                handler.height = height * slotSize;
                handler.offsetX = offsetInElementX;
                handler.offsetY = offsetInElementY;

                mouseLeaveSelectedItem = false;
                
                // ✅ СКРЫВАЕМ ИСХОДНЫЙ ПРЕДМЕТ
                setTimeout(() => {
                    const fillElement = document.querySelector(
                        `.slot[data-position="${getPositionId(arrayName)}"][data-x="${index % 5}"][data-y="${Math.floor(index / 5)}"] .fill`
                    );
                    if (fillElement) {
                        fillElement.style.opacity = '0.3'; // Полупрозрачный
                    }
                }, 0);
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
function getPositionId(arrayName) {
        const positions = {
            'inventory': '1',
            'other': '7',
            'backpack': '18',
            'accessories': '9',
            'fastSlots': '20'
        };
        return positions[arrayName] || '1';
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
        if (hoverItem === defaulHoverItem && tradeInfo.Active === false && selectItem !== defaulSelectItem && getDropItem(selectItem.arrayName, selectItem.ItemId) !== false) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;
            
            if (mouseLeaveSelectedItem === true && mainInventoryArea === false) {
                // ✅ ПЕРЕДАЁМ ПОВОРОТ ПРИ ДРОПЕ
                executeClient("client.gamemenu.inventory.drop", selectArrayName, selectIndex, selectItem.isTurn ? 1 : 0);
            }
            itemNoUse(18);
        } 
        else if (hoverItem !== defaulHoverItem && (hoverItem.index !== selectItem.index || hoverItem.arrayName !== selectItem.arrayName)) {
            // ✅ ВЫЧИСЛЯЕМ ПРАВИЛЬНУЮ ПОЗИЦИЮ С УЧЁТОМ OFFSET
            let hoverIndex = hoverItem.index;
            const hoverArrayName = hoverItem.arrayName;
            
            const itemConfig = itemsInfo[selectItem.ItemId] || {};
            const itemWidth = selectItem.isTurn ? (itemConfig.Height || 1) : (itemConfig.Width || 1);
            const itemHeight = selectItem.isTurn ? (itemConfig.Width || 1) : (itemConfig.Height || 1);
            
            let maxCols = 5;
            let maxRows = 17;
            
            switch(hoverArrayName) {
                case "other":
                    maxRows = 19;
                    break;
                case "backpack":
                    maxRows = 6;
                    break;
                case "inventory":
                    maxRows = 17;
                    break;
            }
            
            const hoverSlotX = hoverIndex % maxCols;
            const hoverSlotY = Math.floor(hoverIndex / maxCols);
            
            // ✅ ВЫЧИСЛЯЕМ OFFSET
            const offsetSlotX = Math.floor(handler.offsetX / slotSize);
            const offsetSlotY = Math.floor(handler.offsetY / slotSize);
            
            // ✅ РЕАЛЬНАЯ ПОЗИЦИЯ ПРЕДМЕТА
            const realX = hoverSlotX - offsetSlotX;
            const realY = hoverSlotY - offsetSlotY;
            
            // ✅ ПРЕОБРАЗУЕМ ОБРАТНО В INDEX
            const realIndex = realY * maxCols + realX;
            
            // ✅ ПРОВЕРЯЕМ ГРАНИЦЫ
            if (realX < 0 || realY < 0 || (realX + itemWidth) > maxCols || (realY + itemHeight) > maxRows) {
                itemNoUse(35);
                window.notificationAdd(4, 9, translateText('player1', 'Предмет не помещается в это место!'), 3000);
                return;
            }
            
            // ✅ ТЕПЕРЬ ИСПОЛЬЗУЕМ realIndex ВМЕСТО hoverIndex
            hoverIndex = realIndex;
            
            let _hItem = getItemToIndex(hoverIndex, hoverArrayName);
            let _hInfoItem = window.getItem(_hItem.ItemId);
            
            let selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;
            let _sItem = getItemToIndex(selectIndex, selectArrayName);
            let _sInfoItem = window.getItem(_sItem.ItemId);

            let returnMove = -1;
            if (!_hItem.use || hoverArrayName === "with_trade") {
                itemNoUse(19);
                window.notificationAdd(4, 9, translateText('player1', 'Данный слот не доступен!'), 3000);
                return;
            } else if ((hoverArrayName === "accessories" || hoverArrayName === "fastSlots") && selectArrayName !== "inventory") {
                itemNoUse(20);
                window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
                return;
            } else if ((selectArrayName === "accessories" || selectArrayName === "fastSlots") && hoverArrayName !== "inventory") {
                itemNoUse(21);
                window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
                return;
            } else if (hoverArrayName === "other" && OtherInfo.Id === otherType.Nearby) {
                executeClient("client.gamemenu.inventory.drop", selectArrayName, selectIndex);   
                itemNoUse(18);
                return;
            }          
            
            if (hoverArrayName !== selectArrayName && (returnMove = isMove(hoverIndex, hoverArrayName, _sItem, _sInfoItem)) == -2) {
                itemNoUse(22);
                return;
            }
            
            if (hoverArrayName === "other" && OtherInfo.Id === otherType.Tent && OtherInfo.IsMyTent) {
                executeClient("client.gamemenu.inventory.stack", selectArrayName, selectIndex, 2, _sItem.Count);
                itemNoUse(18);
                return;
            }  

            if (returnMove !== -1) {
                hoverIndex = returnMove;
                _hItem = getItemToIndex(hoverIndex, hoverArrayName);
                _hInfoItem = window.getItem(_hItem.ItemId);
                if (isMove(hoverIndex, hoverArrayName, _sItem, _sInfoItem) == -2) {
                    itemNoUse(23);
                    return;
                }
            }
            returnMove = -1;
            
            if (hoverArrayName !== selectArrayName && (returnMove = isMove(selectIndex, selectArrayName, _hItem, _hInfoItem)) == -2) {
                itemNoUse(24);
                return;
            }

            if (returnMove !== -1) {
                selectIndex = returnMove;
                _sItem = getItemToIndex(selectIndex, selectArrayName);
                _sInfoItem = window.getItem(_sItem.ItemId);
            
                if (isMove(selectIndex, selectArrayName, _hItem, _hInfoItem) == -2) {
                    itemNoUse(25);
                    return;
                }
            }

            let MaxStakcItems = 0;
            if ((hoverArrayName !== "other" && hoverArrayName !== "backpack") && (selectArrayName === "other" || selectArrayName === "backpack") && ![0, 237, 238, 239, 240, 241, 242, 245, 246, 247].includes(_sItem.ItemId) && (MaxStakcItems = getMaxStakcItems(_sItem, _sInfoItem)) == -1) {
                itemNoUse(26);
                return;
            }

            if (MaxStakcItems > 0) {
                if (_hItem.ItemId === _sItem.ItemId || _hItem.ItemId === 0) {
                    executeClient("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);
                    if (_hItem.ItemId === _sItem.ItemId) {
                        _hItem.Count += MaxStakcItems;
                        _sItem.Count -= MaxStakcItems;
                        setItem(hoverIndex, hoverArrayName, _hItem);
                        setItem(selectIndex, selectArrayName, _sItem);
                        executeClient("sounds.playInterface", "inventory/drag_drop", 0.05);
                    } else {
                        _sItem.Count -= MaxStakcItems;
                        setItem(selectIndex, selectArrayName, _sItem);
                        _hItem = {..._sItem};
                        _hItem.Count = MaxStakcItems;
                        setItem(hoverIndex, hoverArrayName, _hItem);
                        executeClient("sounds.playInterface", "inventory/drag_drop", 0.05);
                    }
                } else {
                    window.notificationAdd(4, 9, `${translateText('player1', 'Нет места для')} ${_sInfoItem.Name}, ${translateText('player1', 'максимум можно иметь при себе')} - ${_sInfoItem.Stack} ${translateText('player1', 'шт.')}`, 3000);
                }
                itemNoUse(27);
                return;
            }

            MaxStakcItems = 0;
            if ((selectArrayName !== "other" && selectArrayName !== "backpack") && (hoverArrayName === "other" || hoverArrayName === "backpack") && ![0, 237, 238, 239, 240, 241, 242, 245, 246, 247].includes(_hItem.ItemId) && _hItem.ItemId != _sItem.ItemId && (MaxStakcItems = getMaxStakcItems(_hItem, _hInfoItem)) == -1) {
                itemNoUse(28);
                return;
            }

            if (MaxStakcItems > 0) {
                if (_hItem.ItemId === _sItem.ItemId) {
                    executeClient("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);
                    _sItem.Count += MaxStakcItems;
                    _hItem.Count -= MaxStakcItems;
                    setItem(hoverIndex, hoverArrayName, _hItem);
                    setItem(selectIndex, selectArrayName, _sItem);
                    executeClient("sounds.playInterface", "inventory/drag_drop", 0.05);
                } else {
                    window.notificationAdd(4, 9, `${translateText('player1', 'Нет места для')} ${_sInfoItem.Name}, ${translateText('player1', 'максимум можно иметь при себе')} - ${_sInfoItem.Stack} ${translateText('player1', 'шт.')}`, 3000);
                }
                itemNoUse(29);
                return;
            }
            
            // ✅ ПЕРЕДАЁМ isTurn ПРИ ПЕРЕМЕЩЕНИИ
            executeClient("client.gamemenu.inventory.move", 
                selectArrayName, selectIndex, 
                hoverArrayName, hoverIndex, 
                selectItem.isTurn ? 1 : 0
            );

            if (_hItem.ItemId === _sItem.ItemId && Number(_hInfoItem.Stack) > 1 && Number(_hInfoItem.Stack) > _sItem.Count && Number(_hInfoItem.Stack) > _hItem.Count) {
                const amount = (_hItem.Count === undefined || _hItem.Count < 2 || !isNumber(_hItem.Count)) ? 1 : _hItem.Count;

                if (Number(_hInfoItem.Stack) >= (amount + _sItem.Count)) {
                    _sItem.Count += amount;
                    _sItem.isTurn = selectItem.isTurn;
                    setItem(hoverIndex, hoverArrayName, _sItem);
                    setItem(selectIndex, selectArrayName, clearSlot);
                } else {
                    _hItem.Count = (amount + _sItem.Count) - _hInfoItem.Stack;
                    _sItem.Count = _hInfoItem.Stack;
                    _sItem.isTurn = selectItem.isTurn;
                    setItem(hoverIndex, hoverArrayName, _sItem);
                    setItem(selectIndex, selectArrayName, _hItem);
                }
            } else {
                _sItem.isTurn = selectItem.isTurn;
                setItem(hoverIndex, hoverArrayName, _sItem);
                setItem(selectIndex, selectArrayName, _hItem);
            }
            
            executeClient("sounds.playInterface", "inventory/drag_drop", 0.05);
        }
        itemNoUse(30, true);
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
        
        // ✅ СОХРАНЯЕМ isTurn ЕСЛИ ОН БЫЛ
        ItemsData[arrayName][index] = {
            ...item,
            index: index,
            isTurn: item.isTurn || false // ← ВАЖНО!
        };
    }

const itemNoUse = (hash, toggled = false) => {
        let hoverIndex = -1,
            hoverArrayName = -1;
            
        if (selectItem !== defaulSelectItem) {
            updateItem(selectItem.index, selectItem.arrayName, "hover", false);
            
            // ✅ ВОЗВРАЩАЕМ ВИДИМОСТЬ
            const fillElement = document.querySelector(
                `.slot[data-position="${getPositionId(selectItem.arrayName)}"][data-x="${selectItem.index % 5}"][data-y="${Math.floor(selectItem.index / 5)}"] .fill`
            );
            if (fillElement) {
                fillElement.style.opacity = '1';
            }
        }

        clickTime = 0;
        selectItem = defaulSelectItem;
        isDragging = false;

        if (hoverItem !== defaulHoverItem) {
            hoverIndex = hoverItem.index;
            hoverArrayName = hoverItem.arrayName;
        }
        
        if (hoverIndex === -1 && hoverArrayName === -1) {
            infoItem = defaulHoverItem;
        } else {            
            const _Item = getItemToIndex(hoverIndex, hoverArrayName);
            if (_Item.ItemId != 0) {
                infoItem = {
                    ..._Item,
                    index: hoverIndex,
                    arrayName: hoverArrayName
                };
            } else {
                infoItem = defaulHoverItem;
            }
        }

        ItemStack = -1;
        StackValue = 1;
        
        document.querySelectorAll('.highlight').forEach(el => {
            el.style.backgroundColor = "";
            el.style.width = "0";
            el.style.height = "0";
        });
    }
 onMount(() => {
        // ... (твой существующий onMount код) ...
        
        // ✅ ПОЛУЧАЕМ РАЗМЕР СЛОТА ДЛЯ HIGHLIGHT
        setTimeout(() => {
            const slot = document.querySelector('.slot');
            if (slot) {
                slotSize = slot.getBoundingClientRect().width;
            }
        }, 100);
    });
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
let inventoryWeight = 0;
let maxInventoryWeight = 50;
let backpackWeight = 0;
let maxBackpackWeight = 30;

// ✅ РЕАКТИВНЫЕ ВЫЧИСЛЕНИЯ ПРОЦЕНТОВ
$: inventoryPercent = (inventoryWeight / maxInventoryWeight) * 100;
$: backpackPercent = maxBackpackWeight > 0 ? (backpackWeight / maxBackpackWeight) * 100 : 0;

// ✅ ЦВЕТА ИНДИКАТОРОВ
$: inventoryColor = inventoryPercent > 90 ? '#ff0000' : inventoryPercent > 70 ? '#ffaa00' : '#00ff00';
$: backpackColor = backpackPercent > 90 ? '#ff0000' : backpackPercent > 70 ? '#ffaa00' : '#00ff00';

const open = () => opened = true;
const close = () => opened = false;
const updateHealth = (val) => hp = val;
const updateEat = (val) => eat = val;
const updateWater = (val) => water = val;

let clickMenu = {
    visible: false,
    x: 0,
    y: 0,
    buttons: {
        wear: false,
        putInBackpack: false,
        drop: false,
        use: false,
        split: false
    }
};

function handleAction(action) {
    console.log('Action:', action);
    // Здесь будет логика действий
    clickMenu.visible = false;
}
function handleSlotClick(event, index, arrayName) {
    event.preventDefault();
    event.stopPropagation();
    
    const item = ItemsData[arrayName][index];
    
    // Если слот пустой, не показываем меню
    if (!item || item.ItemId === 0) {
        clickMenu.visible = false;
        return;
    }
    
    // Определяем доступные действия в зависимости от типа предмета и местоположения
    const itemInfo = itemsInfo[item.ItemId];
    
    clickMenu.visible = true;
    clickMenu.x = event.clientX;
    clickMenu.y = event.clientY;
    
    // Сбрасываем все кнопки
    clickMenu.buttons = {
        wear: false,
        putInBackpack: false,
        drop: false,
        use: false,
        split: false
    };
    
    // Логика для определения доступных действий
    if (itemInfo.Category === 'cloth') {
        clickMenu.buttons.wear = true;
    }
    
    if (arrayName === 'inventory' && maxSlotBackpack > 0) {
        clickMenu.buttons.putInBackpack = true;
    }
    
    clickMenu.buttons.drop = true;
    
    if (itemInfo.Category === 'food' || itemInfo.Category === 'medical') {
        clickMenu.buttons.use = true;
    }
    
    if (item.Count > 1) {
        clickMenu.buttons.split = true;
    }
    
    // Сохраняем выбранный предмет для действий
    clickMenu.selectedItem = {
        item: item,
        index: index,
        arrayName: arrayName
    };
}

// Закрытие меню при клике вне его
function handleGlobalClick(event) {
    if (clickMenu.visible && !event.target.closest('.click-block')) {
        clickMenu.visible = false;
    }
}
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
$: {
    if ($charData) {
        console.log("=== CHAR DATA ===");
        console.log("Full charData:", $charData);
        console.log("InventoryWeight:", $charData.InventoryWeight);
        console.log("MaxInventoryWeight:", $charData.MaxInventoryWeight);
        console.log("BackpackWeight:", $charData.BackpackWeight);
        console.log("MaxBackpackWeight:", $charData.MaxBackpackWeight);
        console.log("==================");
        
        inventoryWeight = $charData.InventoryWeight || 0;
        maxInventoryWeight = $charData.MaxInventoryWeight || 50;
        backpackWeight = $charData.BackpackWeight || 0;
        maxBackpackWeight = $charData.MaxBackpackWeight || 30;
    }
}
</script>

<svelte:window on:mousemove={handleGlobalMouseMove} on:mousemove={handleMouseMove} on:mouseup={handleGlobalMouseUp} on:mousedown={handleGlobalMouseDown} on:keyup={onKeyUp} on:keydown={onKeyDown} />

{#if visible}
    <div class="inventory-interface full-width full-height" data-v-29f6b6db>
        
        <!-- Click Menu (Context Menu) -->
        {#if clickMenu.visible}
            <div class="click-block" data-v-29f6b6db 
                style="left: {clickMenu.x}px; top: {clickMenu.y}px; opacity: 1;">
                
                {#if clickMenu.buttons.wear}
                    <div class="button" data-v-29f6b6db on:click={() => handleAction('wear')}>Надеть</div>
                {/if}
                
                {#if clickMenu.buttons.putInBackpack}
                    <div class="button" data-v-29f6b6db on:click={() => handleAction('putInBackpack')}>Положить в рюкзак</div>
                {/if}
                
                {#if clickMenu.buttons.drop}
                    <div class="button" data-v-29f6b6db on:click={() => handleAction('drop')}>Выбросить</div>
                {/if}
                
                {#if clickMenu.buttons.use}
                    <div class="button" data-v-29f6b6db on:click={() => handleAction('use')}>Использовать</div>
                {/if}
                
                {#if clickMenu.buttons.split}
                    <div class="button" data-v-29f6b6db on:click={() => handleAction('split')}>Разделить</div>
                {/if}
            </div>
        {/if}

        <!-- Hover Block -->
        {#if (infoItem !== defaulHoverItem && selectItem.use !== stageItem.move)}
            <div class="hover-block" data-v-29f6b6db 
                style="opacity: 1; left: {fixOutToX(cursorX + 20)}px; top: {fixOutToY(cursorY)}px;">
                <div class="hover-block-body column-block" data-v-29f6b6db>
                    <div class="title" data-v-29f6b6db>
                        <div data-v-29f6b6db>{@html getName(infoItem, infoItem.arrayName)}</div>
                        <div class="hover-block-body__text-description" data-v-29f6b6db>
                            {itemsInfo[infoItem.ItemId].Description}
                        </div>
                    </div>
                    <div class="row-block full-width align-center justify-between" data-v-29f6b6db>
                        <div class="weight row-block align-center" data-v-29f6b6db>
                            <span class="weight__value" data-v-29f6b6db>{infoItem.Count || 1}</span>
                            <span class="weight__icon" data-v-29f6b6db>шт</span>
                        </div>
                    </div>
                </div>
            </div>
        {/if}

        <!-- Drag & Drop Handler -->
        {#if (selectItem.use === stageItem.move)}
    <div class="handler" data-v-29f6b6db 
        style="width: {handler.width}px; 
               height: {handler.height}px; 
               left: {$coords.x - handler.offsetX}px; 
               top: {$coords.y - handler.offsetY}px;">
        <div class="handler_static" data-v-29f6b6db>
            <div class="picture-handler" data-v-29f6b6db 
                style="width: {handler.width}px; 
                       height: {handler.height}px;">
                
                <!-- ✅ КАРТИНКА С ПОВОРОТОМ -->
                <div class="picture-handler__picture" data-v-29f6b6db
                    style="background-image: url({getPng(selectItem, window.getItem(selectItem.ItemId))});
                           {selectItem.isTurn ? 'transform: rotate(90deg);' : ''}">
                </div>
            </div>
                </div>
            </div>
        {/if}

        <!-- Main Inventory -->
       
    <div class="main-inventory full-width full-height" data-v-29f6b6db>
        <div class="inventory row-block align-center" data-v-29f6b6db>
            <div style="position: fixed; top: 10px; right: 10px; background: rgba(0,0,0,0.8); color: white; padding: 15px; z-index: 9999; font-size: 12px;">
        <h3>DEBUG INFO</h3>
        <p>InventoryWeight: {inventoryWeight}</p>
        <p>MaxInventoryWeight: {maxInventoryWeight}</p>
        <p>BackpackWeight: {backpackWeight}</p>
        <p>MaxBackpackWeight: {maxBackpackWeight}</p>
        <hr>
        <p>charData.InventoryWeight: {$charData?.InventoryWeight}</p>
        <p>charData.MaxInventoryWeight: {$charData?.MaxInventoryWeight}</p>
    </div>
            <!-- ========================================= -->
            <!-- LEFT COLUMN: ОКРУЖЕНИЕ + РЮКЗАК -->
            <!-- ========================================= -->
            <div class="column-block" data-v-29f6b6db>
                
                <!-- ОКРУЖЕНИЕ (19 линий x 5 слотов = 95 слотов) -->
                <div class="inventory-col environment column-block {maxSlotBackpack > 0 ? 'environment-with-backpack' : ''}" data-v-29f6b6db>
                    <div class="border-block full-width full-height" data-v-29f6b6db></div>
                    <div class="general-title align-center justify-between" data-v-29f6b6db>
                        <span data-v-29f6b6db>Окружение</span>
                        <div class="row-block align-center" data-v-29f6b6db>
                            <img data-v-29f6b6db src="https://cdn.majestic-files.com/public/master/static/img/inventory/accepted.png" alt="">
                        </div>
                    </div>
                    <div class="scrollable-wrapper" data-v-29f6b6db id="vs-7-0" 
                         on:mouseenter={e => mainInventoryArea = true} 
                         on:mouseleave={e => mainInventoryArea = false}>
                        <div class="scroll-up" data-v-29f6b6db></div>
                        <div class="scroll-down" data-v-29f6b6db></div>
                        <div class="container full-width" data-v-29f6b6db>
                            <div class="inv-block" data-v-29f6b6db>
                                {#each Array(19) as _, lineIndex}
                                    <div class="line" data-v-29f6b6db>
                                        {#each Array(5) as _, slotIndex}
                                            {@const index = lineIndex * 5 + slotIndex}
                                            {@const item = ItemsData["other"][index]}
                                            
                                            <div class="slot" data-v-29f6b6db
                                                track-by="$index"
                                                data-position="7" 
                                                data-id="0" 
                                                data-x={slotIndex} 
                                                data-y={lineIndex}
                                                on:mousedown={(event) => handleMouseDown(event, index, "other")}
                                                on:mouseup={handleSlotMouseUp}
                                                on:mouseenter={(event) => handleSlotMouseEnter(event, index, "other")}
                                                on:mouseleave={handleSlotMouseLeave}>
                                                
                                                <!-- ✅ FILL - ПРЕДМЕТ -->
                                                {#if item && item.ItemId != 0}
                                                    <div class="fill active" data-v-29f6b6db
                                                         style="{getItemSize(item)}">
                                                         
                                                        <div class="item-properties row-block" data-v-29f6b6db>
                                                            {#if item.fraction}
                                                                <img src="{cdn}/img/inventory/job.png" alt="" class="fraction-icon" />
                                                            {:else if item.gender != null}
                                                                <img src="{cdn}/img/inventory/{item.gender ? 'female' : 'male'}.svg" alt="" class="fraction-icon" />
                                                            {/if}
                                                        </div>

                                                        <div class="picture-handler" data-v-29f6b6db
                                                             style="{getItemSize(item, true)}">
                                                            <div class="picture-handler__picture" data-v-29f6b6db
                                                                data-picture="true"
                                                                style="background-image: url({getPng(item, itemsInfo[item.ItemId])})">
                                                            </div>
                                                        </div>
                                                        
                                                        {#if item.Count > 1}
                                                            <div class="amount" data-v-29f6b6db>{item.Count}</div>
                                                        {/if}
                                                        
                                                        <div class="fill-border" data-v-29f6b6db></div>
                                                    </div>
                                                {/if}
                                                
                                                <!-- ✅ HIGHLIGHT И BORDER - ВСЕГДА -->
                                                <div class="highlight" data-v-29f6b6db></div>
                                                <div class="border" data-v-29f6b6db></div>
                                            </div>
                                        {/each}
                                    </div>
                                {/each}
                            </div>
                        </div>
                    </div>
                </div>

                <!-- РЮКЗАК (6 линий x 5 слотов = 30 слотов) -->
                {#if maxSlotBackpack > 0 && ItemsData["backpack"].length}
                    <div class="inventory-col backpack" data-v-29f6b6db>
                        <div class="general-title row-block align-center justify-between" data-v-29f6b6db style="padding-top: 0px;">
                            <div class="row-block full-width align-center" data-v-29f6b6db>
                                <span data-v-29f6b6db>Рюкзак</span>
                            
                            <div class="weight row-block align-center" data-v-29f6b6db>
        <span data-v-29f6b6db class="weight__text-current" style="color: {backpackColor}">
            {backpackWeight.toFixed(1)}
        </span>
        <span data-v-29f6b6db class="weight__text-max align-center">
            &nbsp;/ {maxBackpackWeight} <span data-v-29f6b6db class="kg">kg</span>
        </span>
    </div></div>
                            <div data-v-29f6b6db=""><div data-v-29f6b6db="" class="take-off align-center"><!----><span data-v-29f6b6db="" class="take-off__title">Снять</span></div></div>
                        </div>
                        <div class="scrollable-wrapper" data-v-29f6b6db id="vs-18" 
                             on:mouseenter={e => mainInventoryArea = true} 
                             on:mouseleave={e => mainInventoryArea = false}>
                            <div class="scroll-up" data-v-29f6b6db></div>
                            <div class="scroll-down" data-v-29f6b6db></div>
                            <div class="container full-width" data-v-29f6b6db>
                                <div class="inv-block" data-v-29f6b6db>
                                    {#each Array(6) as _, lineIndex}
                                        <div class="line" data-v-29f6b6db>
                                            {#each Array(5) as _, slotIndex}
                                                {@const index = lineIndex * 5 + slotIndex}
                                                {@const item = ItemsData["backpack"][index]}
                                                
                                                <div class="slot" data-v-29f6b6db
                                                    track-by="$index"
                                                    data-position="18" 
                                                    data-id="132" 
                                                    data-x={slotIndex} 
                                                    data-y={lineIndex}
                                                    on:mousedown={(event) => handleMouseDown(event, index, "backpack")}
                                                    on:mouseup={handleSlotMouseUp}
                                                    on:mouseenter={(event) => handleSlotMouseEnter(event, index, "backpack")}
                                                    on:mouseleave={handleSlotMouseLeave}>
                                                    
                                                    <!-- ✅ FILL - ПРЕДМЕТ -->
                                                    {#if item && item.ItemId != 0}
                                                        <div class="fill active" data-v-29f6b6db
                                                             style="{getItemSize(item)}">
                                                             
                                                            <div class="item-properties row-block" data-v-29f6b6db>
                                                                {#if item.fraction}
                                                                    <img src="{cdn}/img/inventory/job.png" alt="" class="fraction-icon" />
                                                                {:else if item.gender != null}
                                                                    <img src="{cdn}/img/inventory/{item.gender ? 'female' : 'male'}.svg" alt="" class="fraction-icon" />
                                                                {/if}
                                                            </div>

                                                            <div class="picture-handler" data-v-29f6b6db
                                                                 style="{getItemSize(item, true)}">
                                                                <div class="picture-handler__picture" data-v-29f6b6db
                                                                    data-picture="true"
                                                                    style="background-image: url({getPng(item, itemsInfo[item.ItemId])})">
                                                                </div>
                                                            </div>
                                                            
                                                            {#if item.Count > 1}
                                                                <div class="amount" data-v-29f6b6db>{item.Count}</div>
                                                            {/if}
                                                            
                                                            <div class="fill-border" data-v-29f6b6db></div>
                                                        </div>
                                                    {/if}
                                                    
                                                    <div class="highlight" data-v-29f6b6db></div>
                                                    <div class="border" data-v-29f6b6db></div>
                                                </div>
                                            {/each}
                                        </div>
                                    {/each}
                                </div>
                            </div>
                        </div>
                    </div>
                {/if}
            </div>

            <!-- ========================================= -->
            <!-- CENTER COLUMN: ПЕРСОНАЖ -->
            <!-- ========================================= -->
            <div class="inventory-col character full-height" data-v-29f6b6db>
                <div class="name full-width" data-v-29f6b6db>{selectCharData.Name}</div>
                
                <div class="player-data column-block" data-v-29f6b6db>
                    <div class="row-block full-width justify-between" data-v-29f6b6db>
                        <!-- Left Clothes -->
                        <!-- Left Clothes -->
<div class="objects column-block" data-v-29f6b6db>
    {#each [
        {type: "head", slotId: clothes.Hats.slotId},
        {type: "tops", slotId: clothes.Tops.slotId},
        {type: "decals", slotId: clothes.Accessories.slotId},
        {type: "legs", slotId: clothes.Legs.slotId}
    ] as cloth}
        {@const hasItem = ItemsData["accessories"][cloth.slotId] && ItemsData["accessories"][cloth.slotId].ItemId != 0}
        
        <div class="cloth" data-v-29f6b6db
            track-by="$index"
            data-position="9" 
            data-tag={cloth.type}
            style="{hasItem ? '' : `background-image: url('https://cdn.majestic-files.com/public/master/static/img/inventory/clothes/v2/${cloth.type}.svg');`}"
            on:mousedown={(event) => handleMouseDown(event, cloth.slotId, "accessories")}
            on:mouseup={handleSlotMouseUp}
            on:mouseenter={(event) => handleSlotMouseEnter(event, cloth.slotId, "accessories")}
            on:mouseleave={handleSlotMouseLeave}>
            
            {#if hasItem}
                <div class="picture-handler" data-v-29f6b6db>
                    <div class="picture-handler__picture" data-v-29f6b6db
                        style="background-image: url({getPng(ItemsData['accessories'][cloth.slotId], itemsInfo[ItemsData['accessories'][cloth.slotId].ItemId])})">
                    </div>
                </div>
            {/if}
            
            <div class="highlight" data-v-29f6b6db></div>
            <div class="border" data-v-29f6b6db></div>
        </div>
    {/each}
</div>

<!-- Gender Image -->
<div class="gender" data-v-29f6b6db>
    <img data-v-29f6b6db
        class="full-width full-height" 
        src="https://cdn.majestic-files.com/public/master/static/img/inventory/{$charGender ? 'female' : 'male'}.png" 
        alt="">
</div>

<!-- Right Clothes -->
<div class="objects column-block" data-v-29f6b6db>
    {#each [
        {type: "glasses", slotId: clothes.Glasses.slotId},
        {type: "undershirts", slotId: clothes.Undershirts.slotId},
        {type: "shoes", slotId: clothes.Shoes.slotId},
        {type: "gloves", slotId: clothes.Torsos.slotId}
    ] as cloth}
        {@const hasItem = ItemsData["accessories"][cloth.slotId] && ItemsData["accessories"][cloth.slotId].ItemId != 0}
        
        <div class="cloth" data-v-29f6b6db
            track-by="$index"
            data-position="9" 
            data-tag={cloth.type}
            style="{hasItem ? '' : `background-image: url('https://cdn.majestic-files.com/public/master/static/img/inventory/clothes/v2/${cloth.type}.svg');`}"
            on:mousedown={(event) => handleMouseDown(event, cloth.slotId, "accessories")}
            on:mouseup={handleSlotMouseUp}
            on:mouseenter={(event) => handleSlotMouseEnter(event, cloth.slotId, "accessories")}
            on:mouseleave={handleSlotMouseLeave}>
            
            {#if hasItem}
                <div class="picture-handler" data-v-29f6b6db>
                    <div class="picture-handler__picture" data-v-29f6b6db
                        style="background-image: url({getPng(ItemsData['accessories'][cloth.slotId], itemsInfo[ItemsData['accessories'][cloth.slotId].ItemId])})">
                    </div>
                </div>
            {/if}
            
            <div class="highlight" data-v-29f6b6db></div>
            <div class="border" data-v-29f6b6db></div>
        </div>
    {/each}
</div>
                    </div>

                    <!-- Bottom Accessories -->
                    <div class="objects bottom row-block" data-v-29f6b6db>
    {#each [
        {type: "watches", slotId: clothes.Watches.slotId},
        {type: "masks", slotId: clothes.Masks.slotId},
        {type: "ears", slotId: clothes.Ears.slotId},
        {type: "bracelets", slotId: clothes.Bracelets.slotId},
        {type: "accessories", slotId: clothes.Suit.slotId}
    ] as cloth}
        {@const hasItem = ItemsData["accessories"][cloth.slotId] && ItemsData["accessories"][cloth.slotId].ItemId != 0}
        
        <div class="slot cloth" data-v-29f6b6db
            track-by="$index"
            data-position="9" 
            data-tag={cloth.type}
            style="{hasItem ? '' : `background-image: url('https://cdn.majestic-files.com/public/master/static/img/inventory/clothes/v2/${cloth.type}.svg');`}"
            on:mousedown={(event) => handleMouseDown(event, cloth.slotId, "accessories")}
            on:mouseup={handleSlotMouseUp}
            on:mouseenter={(event) => handleSlotMouseEnter(event, cloth.slotId, "accessories")}
            on:mouseleave={handleSlotMouseLeave}>
            
            {#if hasItem}
                <div class="picture-handler" data-v-29f6b6db>
                    <div class="picture-handler__picture" data-v-29f6b6db
                        style="background-image: url({getPng(ItemsData['accessories'][cloth.slotId], itemsInfo[ItemsData['accessories'][cloth.slotId].ItemId])})">
                    </div>
                </div>
            {/if}
            
            <div class="highlight" data-v-29f6b6db></div>
            <div class="border" data-v-29f6b6db></div>
        </div>
    {/each}
</div>

                    <!-- Indicators -->
                    <div class="indicators column-block" data-v-29f6b6db>
                        <div class="state-block align-center hunger" data-v-29f6b6db>
                            <img data-v-29f6b6db class="state-block__picture" src="https://cdn.majestic-files.com/public/master/static/img/inventory/indicators/v2/hunger.svg" alt="">
                            <progress data-v-29f6b6db class="state-block__progress" max="100" min="0" value={eat}></progress>
                            <div class="state-block__value" data-v-29f6b6db>{eat}</div>
                        </div>
                        <div class="state-block align-center water" data-v-29f6b6db>
                            <img data-v-29f6b6db class="state-block__picture" src="https://cdn.majestic-files.com/public/master/static/img/inventory/indicators/v2/water.svg" alt="">
                            <progress data-v-29f6b6db class="state-block__progress" max="100" min="0" value={water}></progress>
                            <div class="state-block__value" data-v-29f6b6db>{water}</div>
                        </div>
                        <div class="state-block align-center health" data-v-29f6b6db>
                            <img data-v-29f6b6db class="state-block__picture" src="https://cdn.majestic-files.com/public/master/static/img/inventory/indicators/v2/health.svg" alt="">
                            <progress data-v-29f6b6db class="state-block__progress" max="100" min="0" value={hp}></progress>
                            <div class="state-block__value" data-v-29f6b6db>{hp}</div>
                        </div>
                    </div>
                </div>

                <!-- Weapon Slot -->
                <div class="weapon" data-v-29f6b6db>
                    <div class="slot cloth" data-v-29f6b6db data-position="19">
                        <div class="empty-item full-width full-height column-block" data-v-29f6b6db>
                            <img data-v-29f6b6db class="empty-item__picture" src="https://cdn.majestic-files.com/public/master/static/img/inventory/empty/weapon.svg" alt="">
                            <span data-v-29f6b6db class="empty-item__title">Активные предметы</span>
                        </div>
                        <div class="highlight" data-v-29f6b6db></div>
                        <div class="border" data-v-29f6b6db></div>
                    </div>
                </div>

                <!-- Armor & Bag -->
                <div class="wear-items row-block full-width justify-between" data-v-29f6b6db>
                    <div class="wear-item" data-v-29f6b6db>
                        <div class="slot cloth" data-v-29f6b6db data-position="9" data-tag="armor">
                            <div class="empty-item full-width full-height column-block" data-v-29f6b6db>
                                <img data-v-29f6b6db class="empty-item__picture" src="https://cdn.majestic-files.com/public/master/static/img/inventory/empty/armour.svg" alt="">
                                <span data-v-29f6b6db class="empty-item__title">Бронежилет</span>
                            </div>
                            <div class="highlight" data-v-29f6b6db></div>
                            <div class="border" data-v-29f6b6db></div>
                        </div>
                    </div>
                    <div class="wear-item" data-v-29f6b6db>
                        <div class="slot cloth" data-v-29f6b6db data-position="9" data-tag="bags">
                            <div class="empty-item full-width full-height column-block" data-v-29f6b6db>
                                <img data-v-29f6b6db class="empty-item__picture" src="https://cdn.majestic-files.com/public/master/static/img/inventory/empty/backpack.svg" alt="">
                                <span data-v-29f6b6db class="empty-item__title">Рюкзак</span>
                            </div>
                            <div class="highlight" data-v-29f6b6db></div>
                            <div class="border" data-v-29f6b6db></div>
                        </div>
                    </div>
                </div>

                <!-- Fast Slots -->
                <div class="fast-slots row-block justify-between" data-v-29f6b6db>
                    {#each ItemsData["fastSlots"] as item, index}
                        <div class="fast-slot" data-v-29f6b6db
                            track-by="$index"
                            data-position="20"
                            on:mousedown={(event) => handleMouseDown(event, index, "fastSlots")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "fastSlots")}
                            on:mouseleave={handleSlotMouseLeave}>
                            
                            {#if item && item.ItemId != 0}
                                <div class="picture-handler" data-v-29f6b6db
                                     >
                                    <div class="picture-handler__picture" data-v-29f6b6db
                                        style="background-image: url({getPng(item, itemsInfo[item.ItemId])})">
                                    </div>
                                </div>
                            {/if}
                            
                            <div class="fast-slot__text-index" data-v-29f6b6db>{index + 1}</div>
                            <div class="border" data-v-29f6b6db></div>
                        </div>
                    {/each}
                </div>
            </div>

            <!-- ========================================= -->
            <!-- RIGHT COLUMN: ИНВЕНТАРЬ -->
            <!-- ========================================= -->
            <div class="inventory-col" data-v-29f6b6db>
                <div class="general-title row-block align-center justify-between" data-v-29f6b6db>
                    <span data-v-29f6b6db>Инвентарь</span>
                    <div class="row-block align-center" data-v-29f6b6db>
                        <div class="weight row-block align-center" data-v-29f6b6db>
            <span data-v-29f6b6db class="weight__text-current" style="color: {inventoryColor}">
                {inventoryWeight.toFixed(1)}
            </span>
            <span data-v-29f6b6db class="weight__text-max align-center">
                &nbsp;/ {maxInventoryWeight} <span data-v-29f6b6db class="kg">kg</span>
            </span>
        </div>
        <div class="close-block flex-block" data-v-29f6b6db on:click={onExit}></div>
    </div>
                </div>
                
                <div class="scrollable-wrapper" data-v-29f6b6db id="vs-1" 
                     on:mouseenter={e => mainInventoryArea = true} 
                     on:mouseleave={e => mainInventoryArea = false}>
                    <div class="scroll-up" data-v-29f6b6db></div>
                    <div class="scroll-down" data-v-29f6b6db></div>
                    <div class="container full-width" data-v-29f6b6db>
                        <div class="inv-block" data-v-29f6b6db>
                            {#each Array(17) as _, lineIndex}
                                <div class="line" data-v-29f6b6db>
                                    {#each Array(5) as _, slotIndex}
                                        {@const index = lineIndex * 5 + slotIndex}
                                        {@const item = ItemsData["inventory"][index]}
                                        
                                        <div class="slot" data-v-29f6b6db
                                            track-by="$index"
                                            data-position="1" 
                                            data-x={slotIndex} 
                                            data-y={lineIndex}
                                            on:mousedown={(event) => handleMouseDown(event, index, "inventory")}
                                            on:mouseup={handleSlotMouseUp}
                                            on:click={(event) => handleSlotClick(event, index, "inventory")}
                                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "inventory")}
                                            on:mouseleave={handleSlotMouseLeave}>
                                            
                                            <!-- ✅ FILL - ПРЕДМЕТ -->
                                            {#if item && item.ItemId != 0}
                                                <div class="fill active" data-v-29f6b6db
                                                     style="{getItemSize(item)}">
                                                     
                                                    <div class="item-properties row-block" data-v-29f6b6db>
                                                        {#if item.fraction}
                                                            <img src="{cdn}/img/inventory/job.png" alt="" class="fraction-icon" />
                                                        {:else if item.gender != null}
                                                            <img src="{cdn}/img/inventory/{item.gender ? 'female' : 'male'}.svg" alt="" class="fraction-icon" />
                                                        {/if}
                                                    </div>

                                                    <div class="picture-handler" data-v-29f6b6db
                                                         style="{getItemSize(item, true)}">
                                                        <div class="picture-handler__picture" data-v-29f6b6db
                                                            data-picture="true"
                                                            style="background-image: url({getPng(item, itemsInfo[item.ItemId])})">
                                                        </div>
                                                    </div>
                                                    
                                                    {#if item.Count > 1}
                                                        <div class="amount" data-v-29f6b6db>{item.Count}</div>
                                                    {/if}
                                                    
                                                    <div class="fill-border" data-v-29f6b6db></div>
                                                </div>
                                            {/if}
                                            
                                            <!-- ✅ HIGHLIGHT И BORDER - ВСЕГДА -->
                                            <div class="highlight" data-v-29f6b6db></div>
                                            <div class="border" data-v-29f6b6db></div>
                                        </div>
                                    {/each}
                                </div>
                            {/each}
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

    </div>
    {/if}
