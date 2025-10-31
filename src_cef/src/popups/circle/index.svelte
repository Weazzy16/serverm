<script>
    import './assets/css/iconscircle.css';
    import './assets/css/circle.sass';
    import { executeClient } from 'api/rage';
    import keys from 'store/keys';
    import { onMount, onDestroy } from 'svelte';

    export let popupData = [];

    let cooldown = null;
    let currentPage = 0;
    const itemsPerPage = 7; // Максимум 7 элементов на странице

    // Аудио
    let popinSound = null;
    let popclickSound = null;

    $: if (popupData && typeof popupData === "string") {
        popupData = JSON.parse(popupData);
        currentPage = 0;
    }

    // Вычисляем элементы для текущей страницы
    $: startIndex = currentPage * itemsPerPage;
    $: endIndex = startIndex + itemsPerPage;
    $: currentItems = popupData.slice(startIndex, endIndex);
    $: hasMore = endIndex < popupData.length;

    const updateCategory = (json) => {
        popupData = JSON.parse(json);
        currentPage = 0;
    };
    
    window.events.addEvent("cef.circle.updateCategory", updateCategory);

    const playSound = (sound) => {
        if (sound) {
            sound.currentTime = 0;
            sound.play().catch(err => console.error('Error playing sound:', err));
        }
    };

    // Выбор элемента - ИСПРАВЛЕНО: передаем правильный item из оригинального массива
    const select = (item) => {
        if (!item) return;
        playSound(popclickSound);
        // Передаем item.func и item.index (которые уже правильные из popupData)
        executeClient("client.circle.select", item.func, item.index);
    };

    // Следующая страница
    const nextPage = () => {
        if (hasMore) {
            playSound(popclickSound);
            currentPage++;
        }
    };

    // Закрытие
    const close = () => {
        playSound(popclickSound);
        executeClient("client.circle.select", "back");
    };

    // Обработчики клавиатуры
    const onKeyUp = (event) => {
        if (cooldown) {
            cooldown = false;
        }

        switch (event.keyCode) {
            case 27: // ESC
                event.preventDefault();
                close();
                break;
        }
    };

    const onKeyDown = (event) => {
        if (cooldown) return;
        
        cooldown = true;

        const numberKeys = [49, 50, 51, 52, 53, 54, 55, 56, 57]; // 1-9
        const keyIndex = numberKeys.findIndex(key => key === event.keyCode);

        if (keyIndex !== -1) {
            // Если нажали 8 и есть кнопка "Ещё..."
            if (keyIndex === 7 && hasMore && currentItems.length === 7) {
                nextPage();
            }
            // Иначе выбираем элемент
            else if (keyIndex < currentItems.length) {
                select(currentItems[keyIndex]);
            }
        }
    };

    // Lifecycle
    onMount(() => {
        popinSound = new Audio('https://cdn.majestic-files.com/public/master/static/sounds/popin.ogg');
        popclickSound = new Audio('https://cdn.majestic-files.com/public/master/static/sounds/popclick.ogg');
        
        popinSound.volume = 0.3;
        popclickSound.volume = 0.3;
        
        popinSound.load();
        popclickSound.load();
        
        playSound(popinSound);
        
        window.addEventListener("keyup", onKeyUp);
        window.addEventListener("keydown", onKeyDown);
    });

    onDestroy(() => {
        window.removeEventListener("keydown", onKeyDown);
        window.removeEventListener("keyup", onKeyUp);
        window.events.removeEvent("cef.circle.updateCategory", updateCategory);
        
        if (popinSound) {
            popinSound.pause();
            popinSound = null;
        }
        if (popclickSound) {
            popclickSound.pause();
            popclickSound = null;
        }
    });
</script>

<svelte:window on:keydown={onKeyDown} on:keyup={onKeyUp} />

<div class="interaction-main full-width full-height" data-v-b6f590ae>
    {#if currentItems && currentItems.length}
        <div data-v-b6f590ae>
            <!-- Close button -->
            <div class="close-block" on:click={close} data-v-b6f590ae>
                <img 
                    src="https://cdn.majestic-files.com/public/master/static/img/main/interaction/close.svg" 
                    alt="" 
                    data-v-b6f590ae 
                />
            </div>

            <!-- Actions block -->
            <div class="actions-block row-block actions-{currentItems.length + (hasMore ? 1 : 0)}" data-v-b6f590ae>
                <!-- Отображаем текущие элементы (максимум 7) -->
                {#each currentItems as item, localIndex (`${item.func}_${item.index}_page${currentPage}`)}
                    <div class="action-block-wrapper" data-v-b6f590ae>
                        <div 
                            class="action-block row-block {item.func}"
                            on:click={() => select(item)}
                            data-v-b6f590ae
                        >
                            <!-- Emoji или иконка -->
                            {#if item.func !== 'nextpage'}
                                <div class="action-block__text-emoji" data-v-b6f590ae>
                                    {item.name.match(/[\p{Emoji}]/u)?.[0] || '📦'}
                                </div>
                            {:else}
                                <img 
                                    class="action-block__picture-nextpage" 
                                    src="https://cdn.majestic-files.com/public/master/static/img/main/interaction/nextpage.svg" 
                                    alt="" 
                                    data-v-b6f590ae
                                />
                            {/if}

                            <!-- Action name -->
                            <div class="action-block__text-actionName" data-v-b6f590ae>
                                {item.name.replace(/[\p{Emoji}\s]+/u, '').trim()}
                            </div>
                        </div>
                    </div>
                {/each}

                <!-- Кнопка "Ещё..." (8-я кнопка) -->
                {#if hasMore}
                    <div class="action-block-wrapper" data-v-b6f590ae>
                        <div 
                            class="action-block row-block nextpage"
                            on:click={nextPage}
                            data-v-b6f590ae
                        >
                            <img 
                                class="action-block__picture-nextpage" 
                                src="https://cdn.majestic-files.com/public/master/static/img/main/interaction/nextpage.svg" 
                                alt="" 
                                data-v-b6f590ae
                            />
                            <div class="action-block__text-actionName" data-v-b6f590ae>
                                Ещё...
                            </div>
                        </div>
                    </div>
                {/if}
            </div>
        </div>
    {/if}
</div>
