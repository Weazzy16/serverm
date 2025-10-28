<script>
    import { createEventDispatcher } from 'svelte';
    import './css/photos.css';
    
    export let cdn = '';
    export let selectingForMessage = false;
    
    const dispatch = createEventDispatcher();
    
    // State variables
    let choosingPhotos = false;
    let selectedPhotos = [];
    let selectedIndex = null;
    let scrollWidth = 0;
    let changSize = 3; // Количество фото в ряду
    
    // Mock data - замените на реальные данные из store
    let pictures = []; // Массив фотографий
    
    // Constants
    const IMAGE_VERTICAL_WIDTH = 51 / 1080 * window.innerHeight;
    
    // Computed properties
    $: indexedPictures = pictures.map((pic, index) => ({
        path: `${cdn}${pic.url}`,
        id: index
    }));
    
    $: chunkedImages = chunkArray(pictures, changSize).map((images, id) => ({
        images,
        id
    }));
    
    // Functions
    function closeApp() {
        dispatch('close');
    }
    
    function closingStart() {
        console.log('Closing start');
    }
    
    function chunkArray(array, chunkSize) {
        const result = [];
        for (let i = 0; i < array.length; i += chunkSize) {
            result.push(array.slice(i, i + chunkSize));
        }
        return result;
    }
    
    function setChoosingPhotosState(state) {
        choosingPhotos = state;
        if (selectingForMessage && !state) {
            dispatch('close');
        }
        if (!choosingPhotos) {
            selectedPhotos = [];
        }
    }
    
    function addSelectedPhoto(index) {
        if (choosingPhotos) {
            if (selectedPhotos.includes(index)) {
                const photoIndex = selectedPhotos.indexOf(index);
                selectedPhotos.splice(photoIndex, 1);
                selectedPhotos = selectedPhotos; // Trigger reactivity
            } else {
                selectedPhotos = [...selectedPhotos, index];
            }
        } else {
            selectedIndex = index;
        }
    }
    
    function deletePhoto(single = false) {
        if (single) {
            // Удаление одной фотографии
            if (selectedIndex !== null) {
                pictures.splice(selectedIndex, 1);
                pictures = pictures; // Trigger reactivity
                selectedIndex = null;
            }
        } else {
            // Удаление выбранных фотографий
            selectedPhotos.sort((a, b) => b - a); // Сортируем по убыванию
            selectedPhotos.forEach(index => {
                pictures.splice(index, 1);
            });
            pictures = pictures; // Trigger reactivity
        }
        setChoosingPhotosState(false);
    }
    
    function scrollImages(event) {
        // Функция для прокрутки изображений
        event.preventDefault();
        // Реализация прокрутки при необходимости
    }
    
    function backToList() {
        selectedIndex = null;
    }
    
    function sendSelectedPhotos() {
        dispatch('images-selected', selectedPhotos);
    }
    
    // Lifecycle
    import { onMount } from 'svelte';
    
    onMount(() => {
        if (selectingForMessage) {
            setChoosingPhotosState(true);
        }
        
        // Scroll to bottom of pictures list
        setTimeout(() => {
            const picturesContainer = document.querySelector('.pictures');
            if (picturesContainer) {
                picturesContainer.scrollTo({
                    top: picturesContainer.scrollHeight,
                    behavior: 'smooth'
                });
            }
        }, 100);
    });
</script>

<!-- svelte-ignore a11y-click-events-have-key-events -->
<div class="photos full-width full-height">
    {#if selectedIndex === null}
        <!-- Photos List View -->
        <div class="photos-list full-width full-height">
            
            {#if selectingForMessage || (!selectingForMessage && pictures.length)}
                <!-- Header -->
                <header class="row-block align-center full-width justify-between">
                    <span class="header__title">Фото</span>
                    <div 
                        class="choose-pictures-button" 
                        class:is-choosing={choosingPhotos}
                        on:click={() => setChoosingPhotosState(!choosingPhotos)}
                    >
                        {choosingPhotos ? 'Отменить' : 'Выбрать'}
                    </div>
                </header>
            {/if}
            
            {#if pictures.length}
                <div class="shadow full-width"></div>
            {/if}
            
            {#if pictures.length}
                <!-- Pictures Grid -->
                <div class="pictures full-width full-height column-block" class:items-filled={selectedPhotos.length}>
                    {#each chunkedImages as row, rowIndex}
                        <div class="pictures-line row-block full-width">
                            {#each row.images as image, colIndex}
                                {@const photoIndex = rowIndex * changSize + colIndex}
                                <div 
                                    class="picture-block full-height"
                                    style="background-image: url({cdn}{image.url})"
                                    on:click={() => addSelectedPhoto(photoIndex)}
                                >
                                    {#if selectedPhotos.includes(photoIndex)}
                                        <svg class="tick" viewBox="0 0 24 24">
                                            <circle cx="12" cy="12" r="10" fill="#007aff"/>
                                            <path d="m9 12 2 2 4-4" stroke="white" stroke-width="2" fill="none"/>
                                        </svg>
                                    {/if}
                                </div>
                            {/each}
                        </div>
                    {/each}
                </div>
            {:else}
                <!-- Empty State -->
                <div class="pictures-empty">
                    <div class="pictures-empty__main">
                       <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 46 46">
  <defs>
    <clipPath id="clip-path">
      <rect id="Прямоугольник_5997" data-name="Прямоугольник 5997" width="46" height="46" transform="translate(125 257)" fill="#27292e"/>
    </clipPath>
  </defs>
  <g id="Группа_масок_436" data-name="Группа масок 436" transform="translate(-125 -257)" opacity="0.5" clip-path="url(#clip-path)">
    <g id="image" transform="translate(125 258.872)">
      <path id="Контур_15631" data-name="Контур 15631" d="M11.978,33.587A7.177,7.177,0,0,1,5.189,28.7l-.067-.22A7,7,0,0,1,4.791,26.4V13.331L.141,28.852a4.353,4.353,0,0,0,3.051,5.281L32.83,42.07a4.4,4.4,0,0,0,1.1.142,4.274,4.274,0,0,0,4.142-3.134L39.8,33.587Zm0,0" fill="#27292e"/>
      <path id="Контур_15632" data-name="Контур 15632" d="M17.249,13.461a3.834,3.834,0,1,0-3.834-3.834A3.837,3.837,0,0,0,17.249,13.461Zm0,0" fill="#27292e"/>
      <path id="Контур_15633" data-name="Контур 15633" d="M41.208.044H12.457A4.8,4.8,0,0,0,7.666,4.836V25.92a4.8,4.8,0,0,0,4.792,4.792H41.208A4.8,4.8,0,0,0,46,25.92V4.836A4.8,4.8,0,0,0,41.208.044ZM12.457,3.878H41.208a.959.959,0,0,1,.958.958V18.443l-6.055-7.065a3.433,3.433,0,0,0-2.57-1.179,3.35,3.35,0,0,0-2.561,1.209l-7.119,8.544L21.543,17.64a3.364,3.364,0,0,0-4.753,0L11.5,22.928V4.836A.959.959,0,0,1,12.457,3.878Zm0,0" fill="#27292e"/>
    </g>
  </g>
</svg>
                        <p>У вас нет фотографий</p>
                    </div>
                </div>
            {/if}
            
            <!-- Selected Items Control -->
            {#if selectedPhotos.length}
                <div class="selected-items-control row-block full-width">
                    <div class="row-block full-width justify-between align-center">
                        <div class="control-button delete-button" on:click={() => deletePhoto()}>
                            Удалить
                        </div>
                        <span class="selected-items-control__title">
                            Выбрано объектов: {selectedPhotos.length}
                        </span>
                        {#if selectingForMessage}
                            <div class="control-button send-button" on:click={sendSelectedPhotos}>
                                Отправить
                            </div>
                        {/if}
                    </div>
                </div>
            {/if}
        </div>
    {:else}
        <!-- Single Image View -->
        <div class="certain-image column-block full-width full-height align-center">
            <div class="certain-image__title">
                Изображение {selectedIndex + 1}
            </div>
            
            <div class="image-header full-width row-block">
                <div class="back align-center row-block" on:click={backToList}>
                    <svg class="back__picture" viewBox="0 0 24 24">
                        <path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z" fill="currentColor"/>
                    </svg>
                    <span class="back__title">Назад</span>
                </div>
            </div>
            
            <div class="image-container full-width full-height">
                <div 
                    class="image-container__picture full-width"
                    style="background-image: url({cdn}{pictures[selectedIndex].url})"
                ></div>
            </div>
            
            <!-- Image thumbnails scroll -->
            <div class="scroller-container full-width" on:wheel={scrollImages}>
                <div class="image-list full-width">
                    {#each indexedPictures as pic, index}
                        <div 
                            class="image-container full-height"
                            class:selected={selectedIndex === index}
                            on:click={() => selectedIndex = index}
                        >
                            <img 
                                src="{pic.path}" 
                                class="image-container__image" 
                                alt="" 
                            />
                        </div>
                    {/each}
                </div>
            </div>
            
            <div class="delete-button" on:click={() => deletePhoto(true)}>
                Удалить
            </div>
        </div>
    {/if}
    
    <!-- Close Button -->
    <div 
        class="close-block full-width" 
        class:is-black={selectedPhotos.length}
        on:mousedown={closingStart}
        on:click={closeApp}
    >
        <div class="close-block-handler"></div>
    </div>
</div>