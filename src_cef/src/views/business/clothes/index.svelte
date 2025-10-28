<script>
    import './main.sass' 
    import './main.css' 
    import './fonts/Gilroy/stylesheet.css';
    import './fonts/SFPro/stylesheet.css';
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
    import { format } from 'api/formatter'
    import { charGender } from 'store/chars'
    import { menu, getClothesDictionary, getBarberDictionary, clothesEmpty, getTattooDictionary } from 'json/clothes.js'
    import { clothesName } from '@/views/player/menu/functions.js';
    import InputBlock from './input_item.svelte';

    export let viewData;

    if (!viewData) {
        viewData = {
            type: 'clothes',
            menuList: '',
            priceType: 0,
            priceList: '{"Masks":[{"DrawableId":36,"Textures":[36]},{"DrawableId":46,"Textures":[46]},{"DrawableId":175,"Textures":[175]}],"Undershirts":[{"DrawableId":31,"Textures":[31]},{"DrawableId":32,"Textures":[32]},{"DrawableId":33,"Textures":[33,1]},{"DrawableId":34,"Textures":[34,1]},{"DrawableId":69,"Textures":[69,1]}],"Shoes":[{"DrawableId":1,"Textures":[1,1,2,13,14,15]},{"DrawableId":7,"Textures":[7,1,2]},{"DrawableId":9,"Textures":[9,1,2]},{"DrawableId":21,"Textures":[21,1,2,3,4,5,6,7,8,9,10,11]},{"DrawableId":24,"Textures":[24]},{"DrawableId":25,"Textures":[25]},{"DrawableId":36,"Textures":[36,1,2,3]},{"DrawableId":71,"Textures":[71,3,4]},{"DrawableId":73,"Textures":[73]}],"Legs":[{"DrawableId":10,"Textures":[10,1,2]},{"DrawableId":20,"Textures":[20]},{"DrawableId":24,"Textures":[24,1,5]},{"DrawableId":25,"Textures":[25,1,5]},{"DrawableId":28,"Textures":[28,1,3,6,8,10,14,15]},{"DrawableId":52,"Textures":[52]},{"DrawableId":129,"Textures":[129]}],"Accessories":[{"DrawableId":10,"Textures":[10,1,2]},{"DrawableId":11,"Textures":[11]},{"DrawableId":12,"Textures":[12,1,2]},{"DrawableId":36,"Textures":[36]},{"DrawableId":115,"Textures":[115,1]},{"DrawableId":127,"Textures":[127]},{"DrawableId":126,"Textures":[126]}],"Tops":[{"DrawableId":29,"Textures":[29,5,7]},{"DrawableId":31,"Textures":[31,5,7]},{"DrawableId":234,"Textures":[234]},{"DrawableId":337,"Textures":[337,5]},{"DrawableId":348,"Textures":[348,2,5,8,10,12]},{"DrawableId":349,"Textures":[349,2,5,8,10,12]}],"Decals":[{"DrawableId":57,"Textures":[57]},{"DrawableId":58,"Textures":[58,1]}],"Hat":[{"DrawableId":122,"Textures":[122,1]}],"Glasses":[{"DrawableId":18,"Textures":[18,2,3,5,6,7,9,10]},{"DrawableId":5,"Textures":[5,1,2,3,4,5,6,7,8,9,10]},{"DrawableId":25,"Textures":[25]},{"DrawableId":26,"Textures":[26]}]}',
            gender: false
        };
    }

    const gender = viewData.gender ? "Male" : "Female";
 
    let menuData = [];


    let selectMenu = {};

    let isFraction = viewData.priceType === 2;

    let isLoad = false;

    const onSelectMenu = data => {
        if (selectMenu == data)
            return;
        
        selectDictionary = false;
        isLoad = true;
        selectMenu = data;
        selectSort = 0;

        switch (viewData.type) {
            case "clothes":
                selectTexture = 0;
                executeClient ('client.clothes.getDictionary', getDictionary (selectMenu.dictionary, getClothesDictionary (gender, selectMenu.dictionary)));
                executeClient ('client.clothes.updateCameraToBone', selectMenu.camera);
                
                break;
            case "barber":
                selectColor = 0;
                selectColorHighlight = 0;
                selectOpacity = 100;
                executeClient ('client.clothes.getDictionary', getDictionary (selectMenu.dictionary, getBarberDictionary (gender, selectMenu.dictionary)));
                executeClient ('client.clothes.updateCameraToBone', selectMenu.camera);
                break;
            case "tattoo":
                executeClient ('client.clothes.getDictionary', getDictionary (selectMenu.dictionary, getTattooDictionary (selectMenu.dictionary)));
                executeClient ('client.clothes.updateCameraToBone', selectMenu.camera);
                break;
        }
    }

    const OnOpen = (type, menuList) => {
        if (menuList)
            menuList = JSON.parse (menuList);
        else
            menuList = false;

        let newMenu = [];
        menu.forEach(data => {
            if (data.type == type && (!menuList || menuList.includes (data.dictionary)) && (!data.gender || data.gender === gender)) {
                newMenu.push (data)
            }
        });

        menuData = newMenu;
    }


    const getDictionary = (dictionary, clothesData) => {

        //

        clothesData = JSON.parse (clothesData);

        const priceList = JSON.parse (viewData.priceList);
        const priceType = viewData.priceType;


        let returnData = {};

        if (priceType === 2) {
            if (!["Tops", "Legs", "Shoes"].includes(dictionary))
                returnData [-1] = {"Id": -1, "Variation": 0, "Name": "Empty", "Textures": [0]}

            if (dictionary === "Undershort")
                returnData [-1] = {"Id": -1, "Variation": 0, "Name": "Empty", "Textures": [0]}

            if (dictionary === "Tops" && priceList && priceList && priceList ["Undershort"])
                returnData [-1] = {"Id": -1, "Variation": 0, "Name": "Empty", "Textures": [0]}
        }

        if (priceList && clothesData && priceList [dictionary]) {
            priceList [dictionary].forEach((data) => {
                if (clothesData [data[0]]) {
                    returnData [data[0]] = clothesData [data[0]];

                    if (priceType === 0)
                        returnData [data[0]].Price = Number (data[1]);
                    else if (priceType === 1)
                        returnData [data[0]].Donate = Number (data[1]);
                    else if (priceType === 2)
                        returnData [data[0]].Textures = data[1].sort((a, b) => a - b);
                }
            })
        }

        return JSON.stringify (returnData);
    }

    import { onMount, onDestroy } from 'svelte'
    
    onDestroy(() => {
        window.events.removeEvent("cef.clothes.updateDictionary", UpdateDictionary);
        window.events.removeEvent("cef.clothes.setName", setName);
        window.events.removeEvent("cef.clothes.getTorso", GetTorso);
        window.events.removeEvent("cef.clothes.getColor", GetColor);
        window.events.removeEvent("cef.clothes.getTop", GetTop);
    });

    onMount(() => {
        OnOpen (viewData.type, viewData.menuList);
    });

    //

    let selectDictionary = false;
    let selectTexture = 0;
    let dictionaryData = [];  
    let textureSort = [];
    let selectSort = 0;
    
    let torso = clothesEmpty[gender][3];  
    let torsos = {};
    let torsosTexture = 0;

    const UpdateDictionary = (json) => {
        if (refCategory)
            refCategory.scrollTop = 0;
            
        dictionaryData = JSON.parse (json);
        OnSelectDictionary (dictionaryData[0]);
        OnSettingConditions ();

        window.loaderData.delay ("clothes.OnBuy", 1.5, false)
        isLoad = false;
        window.loaderData.delay ("clothes.OnBuy", 1.5, false)
    }

    window.events.addEvent("cef.clothes.updateDictionary", UpdateDictionary);

    const setName = (name) => {
        for(let i = 0; i < dictionaryData.length; i++) {
            if (dictionaryData [i] === selectDictionary) {
                dictionaryData [i].descName = name;
                break;
            }
        }
    }

    window.events.addEvent("cef.clothes.setName", setName);


    let refCategory;

    const OnSelectDictionary = data => {
        if (selectDictionary == data)
            return;

        selectDictionary = data;

        switch (viewData.type) {
            case "clothes":
                selectSort = 0;
                if (selectDictionary.Textures) {
                    
                    textureSort = selectDictionary.Textures.slice (0, length);
                    OnSelectClothes (selectDictionary.Textures [0]);
                }
                break;
            case "barber":
                selectSort = 1;
                if (selectMenu.dictionary == "Hair") {
                    OnSelectHair ();
                    OnSelectColor (selectColor);
                }
                else if (selectMenu.dictionary == "Eyes")
                    OnSelectEyes ();
                else {
                    OnSelectOverlay ();
                    OnSelectColor (selectColor);
                }
                break;
            case "tattoo":
                OnSetDecoration ();
                break;
        }        
    }

    const OnSelectClothes = (index) => {
        selectSort = 0;
        selectTexture = index;
        executeClient ('client.shop.getIndexToTextureName', selectDictionary.Name, selectDictionary.TName, selectTexture, selectDictionary.Id);

        const func = selectMenu.function;
        if (func && func[0] && func[0].event) {
            if (selectMenu.dictionary === "Torsos") {
                if (selectDictionary.Torsos [torso]) {
                    executeClient ('client.clothes.setComponentVariation', 
                                                    3, 
                                                    selectDictionary.Torsos [torso], 
                                                    selectTexture);
                }
            } else {
                let variation = selectDictionary.Variation;

                if (selectDictionary.Id == -1) {
                    if (func[0].event === "setComponentVariation") 
                        variation = clothesEmpty[gender][func[0].componentId];
                    else 
                        variation = -1;
                }

                executeClient ('client.clothes.' + func[0].event, 
                                                    func[0].componentId, 
                                                    variation, 
                                                    selectTexture);
            }
        }

        OnInitConditions ();
    }


    const OnSelectHair = () => {
        const func = selectMenu.function;
        if (func && func[0] && func[0].event)
            executeClient ('client.clothes.setComponentVariation', 
                                                func[0].componentId, 
                                                selectDictionary.Variation, 
                                                0);
        OnInitConditions ();
    }

    const OnSelectEyes = () => {

        const func = selectMenu.function;
        if (func && func[0] && func[0].event)
            executeClient ('client.clothes.setEyeColor', selectDictionary.Variation);

        OnInitConditions ();
    }

    const OnSelectOverlay = () => {
        const func = selectMenu.function;
        if (func && func[0] && func[0].event) 
            executeClient ('client.clothes.setHeadOverlay', 
                                                func[0].overlayID, 
                                                selectDictionary.Variation, 
                                                selectOpacity);

        OnInitConditions ();
    }


    const OnSetDecoration = () => {

        if (gender == "Male") 
            executeClient ('client.clothes.setDecoration', selectMenu.tattooId, JSON.stringify (selectDictionary.Slots), selectDictionary.Dictionary, selectDictionary.MaleHash);
        else 
            executeClient ('client.clothes.setDecoration', selectMenu.tattooId, JSON.stringify (selectDictionary.Slots), selectDictionary.Dictionary, selectDictionary.FemaleHash);

        OnInitConditions ();
    }

    let colorsData = [];
    let colorsDataSort = [];
    let selectColor = 0;
    let colorsHighlightData = [];
    let colorsHighlightDataSort = [];
    let selectColorHighlight = 0;  
    let selectOpacity = 100;  

    const OnSelectColor = (colorId) => {
        selectSort = 1;

        selectColor = colorId;
        const func = selectMenu.function;
        if (func && func[1] && func[1].event) {
            
            if (selectMenu.dictionary == "Hair")
                executeClient ('client.clothes.setHairColor', 
                                                    selectColor, 
                                                    selectColorHighlight);
            else if (func[1].overlayID)
                executeClient ('client.clothes.setHeadOverlayColor', 
                                                    func[1].overlayID, 
                                                    func[1].colorType,
                                                    selectColor);
        }
        OnInitConditions ();
    }

    const OnSelectColorHighlight = (colorId) => {
        
        selectSort = 2;
        selectColorHighlight = colorId;
        const func = selectMenu.function;
        if (func && func[1] && func[1].event) {
            
            if (selectMenu.dictionary == "Hair")
                executeClient ('client.clothes.setHairColor', 
                                                    selectColor, 
                                                    selectColorHighlight);
        }
        OnInitConditions ();
    }


    const OnSelectOpacity = (opacity) => {
        selectOpacity = opacity;  
        const func = selectMenu.function;
        if (func && func[0] && func[0].event) {
            executeClient ('client.clothes.setHeadOverlay', 
                                                func[0].overlayID, 
                                                selectDictionary.Variation, 
                                                selectOpacity);
                                            
            executeClient ('client.clothes.setHeadOverlayColor', 
                                                func[1].overlayID, 
                                                func[1].colorType,
                                                selectColor);
        }
        OnInitConditions ();
    }

    const getName = (name) => {
        if(typeof name == "number") 
        {
            if (clothesName [`${selectMenu.dictionary}_${name}_${gender}`]) name = clothesName [`${selectMenu.dictionary}_${name}_${gender}`];
            else if (clothesName [`${selectMenu.dictionary}_${name}`]) name = clothesName [`${selectMenu.dictionary}_${name}`];
            else name = `#${name}`;
        }
        return name;
    }

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }

    //

    const OnSettingConditions = () => {
        if (["Body", "Torso", "LeftArm", "RightArm"].includes (selectMenu.dictionary) || (selectDictionary && selectDictionary.Torso != undefined)) {//Если передеваем верх            
            executeClient ('client.clothes.getTorso');
        }

        if (selectDictionary && selectDictionary.Torsos != undefined) {//Если передеваем верх            
            executeClient ('client.clothes.getTop');
        }
        
        if (selectMenu.color != undefined) {
            executeClient ('client.clothes.getColor', selectMenu.isHair);
        }        
    }

    const GetTorso = (drawable, texture) => {
        torsos = {};
        torsosTexture = texture;
        const defaultTorsos = [0, 1, 2, 4, 5, 6, 8, 11, 12, 14, 15, 112, 113, 114];
        if (!defaultTorsos.includes (drawable)) {//15 дефолтный пустой торс
            const torsosData = JSON.parse (getClothesDictionary (gender, "Torsos"));
            Object.values (torsosData).forEach((data) => {
                if (data && data.Torsos && Object.values (data.Torsos)) {
                    Object.values (data.Torsos).forEach((torso) => {
                        if (torso === drawable) {
                            torsos = data.Torsos;
                            //torsosTexture = texture;
                        }
                    })
                }
            })
        }
        OnInitConditions ();
    }

    window.events.addEvent("cef.clothes.getTorso", GetTorso);

    const GetTop = (drawable) => {
        torso = clothesEmpty[gender][3];
        if (torso != drawable) {
            let isSuccess = false;
            const topsData = JSON.parse (getClothesDictionary (gender, "Tops"));
            Object.values (topsData).forEach((data) => {
                if (data && data.Variation == drawable) {
                    torso = data.Torso;
                    isSuccess = true;
                }
            });
            //
            if (!isSuccess) {
                const undershortData = JSON.parse (getClothesDictionary (gender, "Undershort"));
                Object.values (undershortData).forEach((data) => {
                    if (data && data.Variation == drawable) {
                        torso = data.Torso;
                    }
                })
            }
        }
        

        OnInitConditions ();
    }

    window.events.addEvent("cef.clothes.getTop", GetTop);

    const OnInitConditions = () => {
        if (selectDictionary && selectDictionary.Torso != undefined) {//Если передеваем верх

            executeClient ('client.clothes.setComponentVariation', 8, clothesEmpty[gender][8], 0, false);
            
            if (torsos [selectDictionary.Torso])
                executeClient ('client.clothes.setComponentVariation', 3, torsos [selectDictionary.Torso], torsosTexture, false);
            else
                executeClient ('client.clothes.setComponentVariation', 3, selectDictionary.Torso, torsosTexture, false);
        }

        if (selectDictionary.IsHair != undefined) {
            executeClient ('client.clothes.setComponentVariation', 2, 0, 0, false);
        }

        if (selectDictionary.IsHat != undefined) {
            executeClient ('client.clothes.clearProp', 0);
        }

        if (selectDictionary.IsGlasses != undefined) {
            executeClient ('client.clothes.clearProp', 1);
            executeClient ('client.clothes.clearProp', 2);
        }
        
        if (["Masks"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.clearMask');
        }

        if (["Hat", "Glasses", "Ears"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 1, clothesEmpty[gender][1], 0, false);
        }
        
        //Барбер
        if (["Hair", "Beard", "Eyebrows", "Eyes", "Lips", "Palette", "Makeup"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 1, clothesEmpty[gender][1], 0, false);
            executeClient ('client.clothes.clearProp', 0);
        }
        
        if (["Eyebrows", "Eyes", "Makeup"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.clearProp', 1);
        }

        if (["Body"].includes (selectMenu.dictionary)) {            
            if (torsos [clothesEmpty[gender][3]])
                executeClient ('client.clothes.setComponentVariation', 3, torsos [clothesEmpty[gender][3]], torsosTexture, false);
            else
                executeClient ('client.clothes.setComponentVariation', 3, clothesEmpty[gender][3], torsosTexture, false);

            executeClient ('client.clothes.setComponentVariation', 8, clothesEmpty[gender][8], 0, false);
            executeClient ('client.clothes.setComponentVariation', 11, clothesEmpty[gender][11], 0, false);
        }
        //Тату
        if (["Head"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 1, clothesEmpty[gender][1], 0, false);
            executeClient ('client.clothes.clearProp', 0);
        }

        if (["Torso", "LeftArm", "RightArm"].includes (selectMenu.dictionary)) {            
            if (torsos [clothesEmpty[gender][3]])
                executeClient ('client.clothes.setComponentVariation', 3, torsos [clothesEmpty[gender][3]], torsosTexture, false);
            else
                executeClient ('client.clothes.setComponentVariation', 3, clothesEmpty[gender][3], torsosTexture, false);

            executeClient ('client.clothes.setComponentVariation', 8, clothesEmpty[gender][8], 0, false);
            executeClient ('client.clothes.setComponentVariation', 11, clothesEmpty[gender][11], 0, false);
        }

        if (["LeftLeg", "RightLeg"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 4, clothesEmpty[gender][4], 0, false);
        }
    }

    const length = 8;

    //let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)
    const SplitColorsArray = (select, array,  sortArray) => {
        //.findIndex(a => a == anim);

        let index = array.findIndex(a => a == sortArray [0]);
        if (index != -1 && (index - 1) === select) {
            return array.slice ((index - 1), (index - 1) + length);
        } 
        
        index = array.findIndex(a => a == sortArray [length - 1]);
        if (index != -1 && (index + 1) === select) {
            index = array.findIndex(a => a == sortArray [0]);
            if (index != -1)
                return array.slice ((index + 1), (index + 1) + length);
        }
        return -1;
    }


    /*const SplitColorsArray = (select) => {
        //.findIndex(a => a == anim);

        let gtaid = colorsDataSort[0].gtaid - 1;
        if (gtaid === select) {
            colorsDataSort = colorsData.slice (gtaid, gtaid + length);
            return;
        } 
        
        gtaid = colorsDataSort[length - 1].gtaid + 1;
        if (gtaid === select) {
            gtaid = colorsDataSort[0].gtaid + 1;
            colorsDataSort = colorsData.slice (gtaid, gtaid + length);
        }
    }*/

    const GetColor = (json) => {
        colorsData = JSON.parse (json);

        colorsDataSort = colorsData.slice (0, length);

        if (selectMenu.colorHighlight) {
            colorsHighlightData = colorsData;
            colorsHighlightDataSort = colorsDataSort;
        }
    }

    window.events.addEvent("cef.clothes.getColor", GetColor);

    const handleKeyDown = (event) => {
        const { keyCode } = event;

        if (keyCode != 13) 
            window.loaderData.delay ("clothes.OnBuy", 1.5, false)

        switch (keyCode) {
            case 69: {
                if (!menuData)
                    return;

                let index = menuData.findIndex(a => a == selectMenu);
                if(menuData [index + 1] === undefined) 
                    return;
                
                index++;
                onSelectMenu (menuData [index])
                break;
            }
            case 81: {
                if (!menuData)
                    return;

                let index = menuData.findIndex(a => a == selectMenu);
                if(menuData [index - 1] === undefined) 
                    return;
                
                index--;
                onSelectMenu (menuData [index])
                break;
            }
            //
            case 38: {
                if (!dictionaryData)
                    return;

                let index = dictionaryData.findIndex(a => a == selectDictionary);
                if(dictionaryData [index - 1] === undefined) 
                    return;
                
                index--;
                OnSelectDictionary (dictionaryData [index]);

                const el = document.querySelector(`#menu_${index}`);
                const bounds = el.getBoundingClientRect();
                refCategory.scrollTop = (bounds.height * index) + ((bounds.height / 10) * index);
                break;
            }
            //
            case 40: {
                if (!dictionaryData)
                    return;

                let index = dictionaryData.findIndex(a => a == selectDictionary);
                if(dictionaryData [index + 1] === undefined) 
                    return;
                
                index++;
                OnSelectDictionary (dictionaryData [index]);

                const el = document.querySelector(`#menu_${index}`);
                const bounds = el.getBoundingClientRect();
                refCategory.scrollTop = (bounds.height * index) + ((bounds.height / 10) * index);
                break;
            }
            //
            case 37:
                switch (selectSort) {
                    case 0:
                        if (!selectDictionary.Textures)
                            return;

                        let index = selectDictionary.Textures.findIndex(a => a == selectTexture);
                        if(selectDictionary.Textures [index - 1] === undefined) 
                            return;
                        
                        OnSelectClothes (selectDictionary.Textures [index - 1]);

                        let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)

                        if (returnSort != -1)
                            textureSort = returnSort;
                        break;
                    case 1:
                        if(--selectColor < 0) 
                            selectColor = 0;
                        else {
                            OnSelectColor (selectColor);
                            let returnSort = SplitColorsArray (selectColor, colorsDataSort, colorsData);
                            if (returnSort != -1)
                                colorsDataSort = returnSort;
                        }
                        break;
                    case 2:
                        if(--selectColorHighlight < 0) 
                            selectColorHighlight = 0;
                        else {
                            OnSelectColorHighlight (selectColorHighlight);
                            let returnSort = SplitColorsArray (selectColorHighlight, colorsHighlightDataSort, colorsHighlightData);
                            if (returnSort != -1)
                                colorsHighlightDataSort = returnSort;
                        }
                        break;
                }
                break;
            case 39:
                switch (selectSort) {
                    case 0:
                        if (selectDictionary.Textures === undefined)
                            return;

                        let index = selectDictionary.Textures.findIndex(a => a == selectTexture);
                        if(selectDictionary.Textures [index + 1] === undefined) 
                            return;
                        
                        OnSelectClothes (selectDictionary.Textures [index + 1]);

                        let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)

                        if (returnSort != -1)
                            textureSort = returnSort;
                        break;
                    case 1:
                        if(++selectColor > colorsData.length - 1) 
                            selectColor = colorsData.length - 1;
                        else {
                            OnSelectColor (selectColor);
                            let returnSort = SplitColorsArray (selectColor, colorsDataSort, colorsData);
                            if (returnSort != -1)
                                colorsDataSort = returnSort;
                        }
                        break;
                    case 2:
                        if(++selectColorHighlight > colorsHighlightData.length - 1) 
                            selectColorHighlight = colorsHighlightData.length - 1;
                        else {
                            OnSelectColorHighlight (selectColorHighlight);
                            let returnSort = SplitColorsArray (selectColorHighlight, colorsHighlightDataSort, colorsHighlightData);
                            if (returnSort != -1)
                                colorsHighlightDataSort = returnSort;
                        }
                        break;
                }
                break;
            case 13:
                OnBuy ()
                break;

        }
    }

    const handleKeyUp = (event) => {
        const { keyCode } = event;
        switch (keyCode) {
            case 27:
                OnExit ()
                break;

        }
    }
    const OnExit = () => {
        executeClient ('client.shop.close');
    }

    const OnBack = () => {
        onSelectMenu (menuData);
    }

    const Onleft = () => {
        switch (selectSort) {
            case 0:
                if (!selectDictionary.Textures)
                    return;

                let index = selectDictionary.Textures.findIndex(a => a == selectTexture);
                if(selectDictionary.Textures [index - 1] === undefined) 
                    return;
                        
                OnSelectClothes (selectDictionary.Textures [index - 1]);

                let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)

                if (returnSort != -1)
                    textureSort = returnSort;
                break;
        }
    }

    const Onright = () => {
        switch (selectSort) {
            case 0:
                if (selectDictionary.Textures === undefined)
                    return;

                let index = selectDictionary.Textures.findIndex(a => a == selectTexture);
                    if(selectDictionary.Textures [index + 1] === undefined) 
                    return;
                        
                OnSelectClothes (selectDictionary.Textures [index + 1]);

                let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)

                if (returnSort != -1)
                    textureSort = returnSort;
                break;
        }
    }

    const Onleft1 = () => {
        switch (selectSort) {
            case 1:
                if(--selectColor < 0) 
                    selectColor = 0;
                else {
                    OnSelectColor (selectColor);
                    let returnSort = SplitColorsArray (selectColor, colorsDataSort, colorsData);
                    if (returnSort != -1)
                        colorsDataSort = returnSort;
                }
                break;
        }
    }

    const Onright1 = () => {
        switch (selectSort) {
            case 1:
                if(++selectColor > colorsData.length - 1) 
                    selectColor = colorsData.length - 1;
                else {
                    OnSelectColor (selectColor);
                    let returnSort = SplitColorsArray (selectColor, colorsDataSort, colorsData);
                    if (returnSort != -1)
                        colorsDataSort = returnSort;
                }
                break;
        }
    }

    const Onleft2 = () => {
        switch (selectSort) {
            case 2:
                if(--selectColorHighlight < 0) 
                    selectColorHighlight = 0;
                else {
                    OnSelectColorHighlight (selectColorHighlight);
                    let returnSort = SplitColorsArray (selectColorHighlight, colorsHighlightDataSort, colorsHighlightData);
                    if (returnSort != -1)
                        colorsHighlightDataSort = returnSort;
                }
                break;
        }
    }

    const Onright2 = () => {
        switch (selectSort) {
            case 2:
                if(++selectColorHighlight > colorsHighlightData.length - 1) 
                    selectColorHighlight = colorsHighlightData.length - 1;
                else {
                    OnSelectColorHighlight (selectColorHighlight);
                    let returnSort = SplitColorsArray (selectColorHighlight, colorsHighlightDataSort, colorsHighlightData);
                    if (returnSort != -1)
                        colorsHighlightDataSort = returnSort;
                }
                break;
        }
    }



    const OnBuy = () => {
        if (!selectDictionary)
            return;

        if (isLoad)
            return;

        if (!window.loaderData.delay ("clothes.OnBuy", 1.5))
            return;

        switch (viewData.type) {
            case "clothes":
                if (!isFraction)
                    executeClient (`client.clothes.buy`, selectMenu.dictionary, selectDictionary.Id, selectTexture);
                else
                    executeClient (`client.table.editClothingSet`, selectMenu.dictionary, selectDictionary.Id, selectTexture);
                break;
            case "barber":
                executeClient (`client.barber.buy`, selectMenu.dictionary, selectDictionary.Id, selectColor, selectColorHighlight, selectOpacity);
                break;                
            case "tattoo":
                executeClient (`client.tattoo.buy`, selectMenu.dictionary, selectDictionary.Id);                
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} on:keydown={handleKeyDown} />

<div class="gta5devshop {viewData.type}">
    <div class="left">
        <img src="http://gta5dev.online/heone/shop/{viewData.type}.png" alt=""/>
        {#if selectMenu.dictionary === undefined}
            <div class="listitems" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                {#each menuData as data, index}
                    <div class="blockitem" class:active={selectMenu == data} on:keypress={() => {}} on:click={() => onSelectMenu (data)}>
                        <p>{data.title}</p>
                    </div>
                {/each}
            </div>
            <div class="btnback" on:keypress={() => {}} on:click={OnExit}>
                <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <g clip-path="url(#clip0_30_47)">
                    <path d="M14.9333 0H1.06667C0.783856 0.000302558 0.512709 0.120808 0.312731 0.335069C0.112754 0.549331 0.000282388 0.839845 0 1.14286V14.8571C0.000282388 15.1602 0.112754 15.4507 0.312731 15.6649C0.512709 15.8792 0.783856 15.9997 1.06667 16H14.9333C15.2161 15.9997 15.4873 15.8792 15.6873 15.6649C15.8872 15.4507 15.9997 15.1602 16 14.8571V1.14286C15.9997 0.839845 15.8872 0.549331 15.6873 0.335069C15.4873 0.120808 15.2161 0.000302558 14.9333 0ZM11.7333 8.57143H5.55413L7.84373 11.0246C7.89467 11.0773 7.9353 11.1403 7.96325 11.2101C7.99121 11.2798 8.00592 11.3548 8.00653 11.4306C8.00715 11.5065 7.99365 11.5817 7.96684 11.652C7.94002 11.7222 7.90042 11.786 7.85034 11.8397C7.80027 11.8933 7.74072 11.9357 7.67518 11.9645C7.60963 11.9932 7.5394 12.0077 7.46859 12.007C7.39777 12.0063 7.32779 11.9906 7.26272 11.9606C7.19765 11.9307 7.1388 11.8871 7.0896 11.8326L3.8896 8.404C3.78984 8.2966 3.73384 8.15124 3.73384 7.99971C3.73384 7.84819 3.78984 7.70283 3.8896 7.59543L7.0896 4.16743C7.19019 4.06334 7.32491 4.00574 7.46475 4.00704C7.60459 4.00835 7.73835 4.06844 7.83724 4.17439C7.93612 4.28034 7.99221 4.42366 7.99343 4.57349C7.99464 4.72331 7.94088 4.86766 7.84373 4.97543L5.55413 7.42857H11.7333C11.8748 7.42857 12.0104 7.48878 12.1105 7.59594C12.2105 7.7031 12.2667 7.84845 12.2667 8C12.2667 8.15155 12.2105 8.2969 12.1105 8.40406C12.0104 8.51123 11.8748 8.57143 11.7333 8.57143Z"/>
                    </g>
                    <defs>
                    <clipPath id="clip0_30_47">
                    <rect width="16" height="16"/>
                    </clipPath>
                    </defs>
                </svg>                    
                <p>Выйти</p>
            </div>
            {:else}
            <div class="listitems" bind:this={refCategory} on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                {#each dictionaryData as data, index}
                    <div class="blockitem" id="menu_{index}" class:active={data === selectDictionary} on:keypress={() => {}} on:click={() => OnSelectDictionary (data)}>
                        <p>{@html getName(data.descName)}</p>
                        {#if data.Donate > 0 && !isFraction}
                            <span>RB {format("money", data.Donate)}</span>
                            {:else if data.Price > 0 && !isFraction}
                            <span>$ {format("money", data.Price)}</span>
                        {/if}
                    </div>
                {/each}
            </div>
            <div class="btnback" on:keypress={() => {}} on:click={OnBack}>
                <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <g clip-path="url(#clip0_30_47)">
                    <path d="M14.9333 0H1.06667C0.783856 0.000302558 0.512709 0.120808 0.312731 0.335069C0.112754 0.549331 0.000282388 0.839845 0 1.14286V14.8571C0.000282388 15.1602 0.112754 15.4507 0.312731 15.6649C0.512709 15.8792 0.783856 15.9997 1.06667 16H14.9333C15.2161 15.9997 15.4873 15.8792 15.6873 15.6649C15.8872 15.4507 15.9997 15.1602 16 14.8571V1.14286C15.9997 0.839845 15.8872 0.549331 15.6873 0.335069C15.4873 0.120808 15.2161 0.000302558 14.9333 0ZM11.7333 8.57143H5.55413L7.84373 11.0246C7.89467 11.0773 7.9353 11.1403 7.96325 11.2101C7.99121 11.2798 8.00592 11.3548 8.00653 11.4306C8.00715 11.5065 7.99365 11.5817 7.96684 11.652C7.94002 11.7222 7.90042 11.786 7.85034 11.8397C7.80027 11.8933 7.74072 11.9357 7.67518 11.9645C7.60963 11.9932 7.5394 12.0077 7.46859 12.007C7.39777 12.0063 7.32779 11.9906 7.26272 11.9606C7.19765 11.9307 7.1388 11.8871 7.0896 11.8326L3.8896 8.404C3.78984 8.2966 3.73384 8.15124 3.73384 7.99971C3.73384 7.84819 3.78984 7.70283 3.8896 7.59543L7.0896 4.16743C7.19019 4.06334 7.32491 4.00574 7.46475 4.00704C7.60459 4.00835 7.73835 4.06844 7.83724 4.17439C7.93612 4.28034 7.99221 4.42366 7.99343 4.57349C7.99464 4.72331 7.94088 4.86766 7.84373 4.97543L5.55413 7.42857H11.7333C11.8748 7.42857 12.0104 7.48878 12.1105 7.59594C12.2105 7.7031 12.2667 7.84845 12.2667 8C12.2667 8.15155 12.2105 8.2969 12.1105 8.40406C12.0104 8.51123 11.8748 8.57143 11.7333 8.57143Z"/>
                    </g>
                    <defs>
                    <clipPath id="clip0_30_47">
                    <rect width="16" height="16"/>
                    </clipPath>
                    </defs>
                </svg>                    
                <p>Назад</p>
            </div>
        {/if}
    </div>
    <div class="right">
        <h1>Настройки</h1>
        {#if (selectDictionary.Textures && selectDictionary.Textures.length > 0)}
            <div class="inr">
                <p>Варианты</p>
                <div class="vibs">
                    <div class="btnleft" on:keypress={() => {}} on:click={Onleft}>
                        <svg width="7" height="12" viewBox="0 0 7 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M5.61538 1L1.62611 5.32172C1.27251 5.70478 1.27251 6.29522 1.6261 6.67828L5.61538 11" stroke-linecap="round"/>
                        </svg>                            
                    </div>
                    <p>{#each textureSort as index, _}<p class:active={selectTexture === index} id="texture_{index}" on:keypress={() => {}} on:click={() => OnSelectClothes (index)}>{index +1}</p>{/each}<b>/{selectDictionary.Textures.length}</b></p>
                    <div class="btnright" on:keypress={() => {}} on:click={Onright}>
                        <svg width="7" height="12" viewBox="0 0 7 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M1.00473 11L4.99401 6.67828C5.34761 6.29522 5.34761 5.70478 4.99401 5.32172L1.00473 1"stroke-linecap="round"/>
                        </svg>                            
                    </div>
                </div>
            </div>
        {/if}
        {#if selectMenu.color && colorsData.length > 0}
            <div class="inr">
                <p>Цвет</p>
                <div class="vibs">
                    <div class="btnleft" on:keypress={() => {}} on:click={Onleft1}>
                        <svg width="7" height="12" viewBox="0 0 7 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M5.61538 1L1.62611 5.32172C1.27251 5.70478 1.27251 6.29522 1.6261 6.67828L5.61538 11" stroke-linecap="round"/>
                        </svg>                            
                    </div>
                    <p>{#each colorsDataSort as data, index}<p class:active={selectColor === data.gtaid} id="colors_{index}" on:keypress={() => {}} on:click={() => OnSelectColor (data.gtaid)}>{data.gtaid +1}</p>{/each}<b>/{colorsData.length}</b></p>
                    <div class="btnright" on:keypress={() => {}} on:click={Onright1}>
                        <svg width="7" height="12" viewBox="0 0 7 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M1.00473 11L4.99401 6.67828C5.34761 6.29522 5.34761 5.70478 4.99401 5.32172L1.00473 1"stroke-linecap="round"/>
                        </svg>                            
                    </div>
                </div>
            </div>
        {/if}
        {#if selectMenu.colorHighlight && colorsHighlightData.length > 0}
            <div class="inr">
                <p>Доп. Цвет</p>
                <div class="vibs">
                    <div class="btnleft" on:keypress={() => {}} on:click={Onleft2}>
                        <svg width="7" height="12" viewBox="0 0 7 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M5.61538 1L1.62611 5.32172C1.27251 5.70478 1.27251 6.29522 1.6261 6.67828L5.61538 11" stroke-linecap="round"/>
                        </svg>                            
                    </div>
                    <p>{#each colorsHighlightDataSort as data, index}<p class:active={selectColorHighlight === data.gtaid} id="colorsHighlight_{index}" on:keypress={() => {}} on:click={() => OnSelectColorHighlight (data.gtaid)}>{data.gtaid +1}</p>{/each}<b>/{colorsHighlightData.length}</b></p>
                    <div class="btnright" on:keypress={() => {}} on:click={Onright2}>
                        <svg width="7" height="12" viewBox="0 0 7 12" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M1.00473 11L4.99401 6.67828C5.34761 6.29522 5.34761 5.70478 4.99401 5.32172L1.00473 1"stroke-linecap="round"/>
                        </svg>                            
                    </div>
                </div>
            </div>
        {/if}
        <div class="price">
            <p>Цена:</p>
            {#each dictionaryData as data, index}
                <div class="priceitem" class:active={data === selectDictionary}>
                    {#if data.Donate > 0 && !isFraction}
                        <span>RB{format("money", data.Donate)}</span>
                        {:else if data.Price > 0 && !isFraction}
                        <span>${format("money", data.Price)}</span>
                    {/if}
                </div>
            {/each}
        </div>
        <div class="btnbuys">
            <div class="btnshopbuy">
                <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <g clip-path="url(#clip0_30_24)">
                    <path d="M0 3.59999C0 3.28173 0.126428 2.97651 0.351472 2.75147C0.576515 2.52642 0.88174 2.39999 1.2 2.39999H14.8C15.1183 2.39999 15.4235 2.52642 15.6485 2.75147C15.8736 2.97651 16 3.28173 16 3.59999V4.79999H0V3.59999Z"/>
                    <path fill-rule="evenodd" clip-rule="evenodd" d="M0 6.39999V12.4C0 12.7183 0.126428 13.0235 0.351472 13.2485C0.576515 13.4736 0.88174 13.6 1.2 13.6H14.8C15.1183 13.6 15.4235 13.4736 15.6485 13.2485C15.8736 13.0235 16 12.7183 16 12.4V6.39999H0ZM5.6 9.59999H1.6V7.99999H5.6V9.59999Z"/>
                    </g>
                    <defs>
                    <clipPath id="clip0_30_24">
                    <rect width="16" height="16"/>
                    </clipPath>
                    </defs>
                </svg>                            
                <p>Картой</p>
            </div>
            <div class="btnshopbuy" on:keypress={() => {}} on:click={OnBuy}>
                <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path fill-rule="evenodd" clip-rule="evenodd" d="M16 6.46742C16 6.24793 15.9128 6.03743 15.7576 5.88223C15.6024 5.72702 15.3919 5.63983 15.1724 5.63983H0.827586C0.608097 5.63983 0.397597 5.72702 0.242394 5.88223C0.0871919 6.03743 0 6.24793 0 6.46742V13.6398C0 13.8593 0.0871919 14.0698 0.242394 14.225C0.397597 14.3802 0.608097 14.4674 0.827586 14.4674H15.1724C15.3919 14.4674 15.6024 14.3802 15.7576 14.225C15.9128 14.0698 16 13.8593 16 13.6398V6.46742ZM1.10345 11.9847C1.10345 12.0578 1.13251 12.128 1.18425 12.1797C1.23598 12.2315 1.30615 12.2605 1.37931 12.2605C1.5988 12.2605 1.8093 12.3477 1.9645 12.5029C2.1197 12.6581 2.2069 12.8686 2.2069 13.0881C2.2069 13.1613 2.23596 13.2314 2.28769 13.2832C2.33943 13.3349 2.4096 13.364 2.48276 13.364H13.5172C13.5904 13.364 13.6606 13.3349 13.7123 13.2832C13.764 13.2314 13.7931 13.1613 13.7931 13.0881C13.7931 12.8686 13.8803 12.6581 14.0355 12.5029C14.1907 12.3477 14.4012 12.2605 14.6207 12.2605C14.6939 12.2605 14.764 12.2315 14.8158 12.1797C14.8675 12.128 14.8966 12.0578 14.8966 11.9847V8.12259C14.8966 8.04943 14.8675 7.97926 14.8158 7.92753C14.764 7.87579 14.6939 7.84673 14.6207 7.84673C14.4012 7.84673 14.1907 7.75954 14.0355 7.60433C13.8803 7.44913 13.7931 7.23863 13.7931 7.01914C13.7931 6.94598 13.764 6.87581 13.7123 6.82408C13.6606 6.77234 13.5904 6.74328 13.5172 6.74328H2.48276C2.4096 6.74328 2.33943 6.77234 2.28769 6.82408C2.23596 6.87581 2.2069 6.94598 2.2069 7.01914C2.2069 7.23863 2.1197 7.44913 1.9645 7.60433C1.8093 7.75954 1.5988 7.84673 1.37931 7.84673C1.30615 7.84673 1.23598 7.87579 1.18425 7.92753C1.13251 7.97926 1.10345 8.04943 1.10345 8.12259V11.9847Z"/>
                    <path fill-rule="evenodd" clip-rule="evenodd" d="M1.65527 11.7364V8.37088C1.92024 8.31658 2.16344 8.18569 2.35469 7.99443C2.54595 7.80318 2.67684 7.55998 2.73114 7.29501H13.2691C13.3234 7.55998 13.4543 7.80318 13.6455 7.99443C13.8368 8.18569 14.08 8.31658 14.3449 8.37088V11.7364C14.08 11.7907 13.8368 11.9216 13.6455 12.1128C13.4543 12.3041 13.3234 12.5473 13.2691 12.8123H2.73114C2.67684 12.5473 2.54595 12.3041 2.35469 12.1128C2.16344 11.9216 1.92024 11.7907 1.65527 11.7364ZM8.00065 7.84536C7.41526 7.84572 6.85396 8.07845 6.44006 8.49241C6.02615 8.90636 5.7935 9.4677 5.7932 10.0531C5.7935 10.6384 6.02616 11.1997 6.44007 11.6137C6.85399 12.0276 7.41529 12.2602 8.00065 12.2605C8.58604 12.2602 9.14737 12.0276 9.56133 11.6137C9.97529 11.1998 10.208 10.6385 10.2084 10.0531C10.2081 9.46765 9.97539 8.90627 9.56143 8.49231C9.14746 8.07834 8.58609 7.84565 8.00065 7.84536Z"/>
                    <path d="M8.00045 11.7089C8.91488 11.7089 9.65618 10.9676 9.65618 10.0532C9.65618 9.13878 8.91488 8.39749 8.00045 8.39749C7.08602 8.39749 6.34473 9.13878 6.34473 10.0532C6.34473 10.9676 7.08602 11.7089 8.00045 11.7089Z"/>
                    <path fill-rule="evenodd" clip-rule="evenodd" d="M5.1099 5.08796L12.1259 3.20796C12.1609 3.19846 12.1974 3.19597 12.2334 3.20064C12.2693 3.20532 12.304 3.21705 12.3354 3.23518C12.3668 3.25331 12.3944 3.27746 12.4164 3.30627C12.4384 3.33507 12.4545 3.36794 12.4638 3.40299C12.4919 3.50798 12.5404 3.6064 12.6066 3.69264C12.6727 3.77888 12.7552 3.85124 12.8493 3.9056C12.9435 3.95996 13.0474 3.99525 13.1551 4.00945C13.2629 4.02365 13.3724 4.01649 13.4773 3.98837C13.548 3.96957 13.6233 3.97953 13.6866 4.01609C13.7499 4.05264 13.7962 4.11281 13.8153 4.18341L14.0578 5.08796H15.122L14.3996 2.39113C14.3714 2.28614 14.3229 2.18772 14.2567 2.10149C14.1905 2.01526 14.108 1.94292 14.0139 1.88858C13.9197 1.83425 13.8158 1.79899 13.708 1.78483C13.6002 1.77066 13.4907 1.77787 13.3858 1.80603L1.13721 5.08796H5.1099Z"/>
                    <path fill-rule="evenodd" clip-rule="evenodd" d="M7.24121 5.0883L12.0288 3.80554C12.1497 4.04746 12.3391 4.24852 12.5733 4.38374C12.8075 4.51895 13.0763 4.58236 13.3463 4.56609L13.4862 5.0883H7.24121Z"/>
                </svg>                            
                <p>Наличкой</p>
            </div>
        </div>
        <img src="http://gta5dev.online/heone/shop/{viewData.type}.png" alt=""/>
        <div class="mouseinfo">
            <p>Для поворота камерой используйте мышку</p>
        </div>
    </div>
</div>