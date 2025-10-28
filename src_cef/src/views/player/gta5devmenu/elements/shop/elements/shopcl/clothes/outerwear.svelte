<script>
  import { accountRedbucks } from 'store/account';
  import { executeClient } from 'api/rage';
  import './cars.css';

  export let SetPopup;
  export let searchText = '';
  export let pagefilter;

  import { shopList } from './decalsList.js';

  // Отсортированный список для главного каталога
  const sortFunctions = {
    20: (a, b) => a.price - b.price,
    21: (a, b) => b.price - a.price,
    22: (a, b) => a.line.localeCompare(b.line),
    23: (a, b) => b.line.localeCompare(a.line),
  };
  $: sortedShopList = [...shopList].sort(sortFunctions[pagefilter] || (() => 0));

  // Модель, выбранная в "Другие модели" или из каталога
  let selectedModel = null;
  let currentIndex = 0;
  let maxIndex = 0;
let otherModels = [];

  // Функция-утилита: перемешивает массив «на месте»
   function shuffle(arr) {
    for (let i = arr.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [arr[i], arr[j]] = [arr[j], arr[i]];
    }
    return arr;
  }

  const maxPrice = Math.max(...shopList.map(m => m.price));

  $: if (selectedModel) {
    const base = selectedModel.price;
    let eligible;

    if (base === maxPrice) {
      // для самой дорогой: -2…-5к
      eligible = shopList.filter(m =>
        m.price <= base - 2000 &&
        m.price >= base - 5000
      );
    } else {
      // для остальных: +2…+5к
      eligible = shopList.filter(m =>
        m.price >= base + 0 &&
        m.price <= base + 5000
      );
    }

    // -- вот тут добавляем fallback: если мало ⇒ докидываем просто любые другие машины
    if (eligible.length < 4) {
      // исключаем из докидывашек уже попавшие и ту же модель
      const pool = shopList.filter(m =>
        m.title !== selectedModel.title &&
        !eligible.includes(m)
      );
      // берем из pool столько, сколько не хватает
      eligible = eligible.concat(shuffle(pool).slice(0, 4 - eligible.length));
    }

    otherModels = shuffle(eligible).slice(0, 4);
  } else {
    // до выбора — первые 4 любых
    otherModels = shopList.slice(0, 4);
  }

  function selectModel(model) {
    selectedModel = model;
    currentIndex = 0;
    maxIndex = model.list.length - 1;
    switchBlock();
    window.pageloadf2(17);
    window.pagenameitemf2(model.title);
    
  }

  function switchBlock() {
    const blocks = document.querySelectorAll('.blgf');
    blocks.forEach((block, idx) => {
      block.style.display = idx === currentIndex ? 'flex' : 'none';
    });
  }

  function nextBlock() {
    if (currentIndex < maxIndex) {
      currentIndex += 1;
      switchBlock();
    }
  }

  function prevBlock() {
    if (currentIndex > 0) {
      currentIndex -= 1;
      switchBlock();
    }
  }

  function onBuy(item) {
    if ($accountRedbucks < item.price) {
      return window.notificationAdd(4, 9, 'Недостаточно Redbucks!', 3000);
    }
    SetPopup();
    executeClient('client.donate.buy.clothes', item.id);
    window.notificationAdd(3, 9, 'Поздравляем с покупкой', 3000);
  }
   function buildRoute() {
    console.log('Построить маршрут для', selectedModel.title);
  }
  
</script>

{#if selectedModel}
  <div class="itemPage">
    <div class="content">
      <!-- HEADER -->
      <div class="header">
        <div class="title">{selectedModel.list[currentIndex].name}</div>
        <div class="price">
          {#if selectedModel.oldPrice}
            <div class="old-price">
              {selectedModel.oldPrice.toLocaleString('ru-RU')}
            </div>
          {/if}
          <div class="current-price">
            {selectedModel.price.toLocaleString('ru-RU')}
          </div>
          <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 20 20" fill="none">
<path class="back" d="M9.99931 19.1668C15.0619 19.1668 19.166 15.0628 19.166 10.0002C19.166 4.93755 15.0619 0.833496 9.99931 0.833496C4.9367 0.833496 0.832642 4.93755 0.832642 10.0002C0.832642 15.0628 4.9367 19.1668 9.99931 19.1668Z"/>
<path class="m" d="M8.2834 11.8721C8.05388 12.1028 7.89926 12.3974 7.83973 12.7173C7.83973 13.0271 7.99465 13.7971 8.42732 13.7971C8.70232 13.7971 9.23582 13.1875 9.40265 12.984C10.625 11.4209 12.0484 10.0259 13.6358 8.8352L13.6578 8.85628C12.8805 9.83162 11.4752 11.4339 11.4752 12.7173C11.4752 13.3379 11.7191 14.0116 12.4506 14.0116C13.4039 14.0116 15.7863 12.1939 16.0521 11.4559L16.0081 11.0499C14.5112 12.4496 13.6468 12.9309 13.3388 12.9309C13.073 12.9309 12.9511 12.621 12.9511 12.4065C13.1223 11.4878 13.5663 10.642 14.2252 9.9792L15.9431 7.96253C16.076 7.80212 16.4197 7.42812 16.4197 7.21362C16.4197 6.99912 15.9541 6.29695 15.6552 6.29695C15.4554 6.29695 15.2345 6.52153 15.0897 6.63887C13.6376 7.75078 12.3635 8.91678 11.0004 10.0498L10.9784 10.0287C11.4367 9.35495 12.6284 7.77187 12.6284 6.98078C12.615 6.7776 12.5231 6.58757 12.3723 6.45077C12.2215 6.31396 12.0234 6.24106 11.8199 6.24745C11.3616 6.25845 10.4238 7.06053 10.0141 7.43453L8.2504 9.07078L8.2284 9.0497C8.50398 8.44757 8.7338 7.82553 8.9159 7.18887C8.91064 7.0437 8.86457 6.90296 8.78298 6.78278L8.51715 6.33362C8.40623 6.15028 8.36223 6.00178 8.0964 5.9917C7.37449 6.16448 6.67343 6.41497 6.00548 6.73878L4.75332 7.2952L4.72215 7.27412C5.1539 6.90745 5.30973 6.7177 5.30973 6.65353C5.30973 6.58937 5.23182 6.54628 5.17682 6.54628C5.07498 6.57453 4.98093 6.62563 4.90182 6.6957C3.91548 7.36945 3.98515 7.34837 3.88248 7.4767C3.74234 7.88603 3.64646 8.30919 3.59648 8.73895C3.54148 8.99562 3.68815 9.13495 3.86232 9.13495C4.4279 9.13495 3.89532 9.11387 7.30898 7.57662C7.41662 7.51503 7.5337 7.47167 7.65548 7.44828C7.84432 7.44828 8.04323 7.66187 6.87998 9.67303L4.56082 12.4899C4.68273 12.8218 4.90457 13.666 5.36932 13.666C5.72407 13.666 6.0009 13.2599 6.18973 13.0134L7.85165 10.8564L10.51 8.0762C10.5879 7.99095 10.7208 7.85162 10.8537 7.85162C10.9866 7.85162 11.0196 7.96895 10.8317 8.30995C10.3221 9.2379 9.72082 10.1125 9.0369 10.9206L8.2834 11.8721Z"/>
</svg>
          {#if selectedModel.discount}
            <div class="discount">{selectedModel.discount}</div>
          {/if}
        </div>
        <button class="buildRoute" on:click={buildRoute}>
          ПОСТРОИТЬ МАРШРУТ
        </button>
      </div>

      <!-- MAIN ITEM CARD (слайдер превью) -->
      <div class="item item__rarity-{selectedModel.line}">
        <div class="item__rarity"><svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
  <path d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z" fill="#8958FF"/>
</svg></div>
        <div class="item__rarity__radial-gradient"></div>
        <div class="swiper">
      {#each selectedModel.list as imgItem, idx}
        <div
          class="swiper-slide__image"
          style="background-image: url({imgItem.img}); display: {idx === currentIndex ? 'block' : 'none'}"
        ></div>
      {/each}
    </div>

    {#if selectedModel.list.length >= 2}
      <div class="arrow-buttons">
        <!-- Кнопка «назад» -->
        <div
          class="button prev"
          class:disabled={currentIndex === 0}
          on:click={prevBlock}
        >
          <!-- сюда вставить SVG back.svg -->
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none">
	<path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"/>
</svg>
        </div>

        <!-- Кнопка «вперёд» -->
        <div
          class="button revert next"
          class:disabled={currentIndex === maxIndex}
          on:click={nextBlock}
        >
          <!-- ту же иконку, но с классом .revert она повернётся -->
          <svg class="revert" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none">
	<path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"/>
</svg>
        </div>
      </div>
      <div class="pagination">
        {#each Array(selectedModel.list.length) as _, i}
          <div
            class="pagination-item"
            class:active={i === currentIndex}
            on:click={() => {
              currentIndex = i;
              switchBlock();
            }}
          ></div>
        {/each}
        </div>
    {/if}
  </div>
    

     


      <!-- БЛОК «ДРУГИЕ МОДЕЛИ» -->
      <div class="similar">
        <div class="title">Другие модели</div>
        <div class="items">
          {#each otherModels as model}
            <div class="similar-item" on:click={() => selectModel(model)}>
              <div class="award-card award-card__rarity-{model.line}">
                <div class="award-card__rarity"><svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
  <path d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z" fill="#8958FF"/>
</svg></div>
                <div class="award-card__rarity__radial-gradient"></div>
                <img
                  class="award-card__media vehicle"
                  src={model.img}
                  alt={model.title}
                />
                <div class="award-card__price">
                  <div class="current-price">
                    {model.price.toLocaleString('ru-RU')}
                  </div>
                  <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 20 20" fill="none">
<path class="back" d="M9.99931 19.1668C15.0619 19.1668 19.166 15.0628 19.166 10.0002C19.166 4.93755 15.0619 0.833496 9.99931 0.833496C4.9367 0.833496 0.832642 4.93755 0.832642 10.0002C0.832642 15.0628 4.9367 19.1668 9.99931 19.1668Z"/>
<path class="m" d="M8.2834 11.8721C8.05388 12.1028 7.89926 12.3974 7.83973 12.7173C7.83973 13.0271 7.99465 13.7971 8.42732 13.7971C8.70232 13.7971 9.23582 13.1875 9.40265 12.984C10.625 11.4209 12.0484 10.0259 13.6358 8.8352L13.6578 8.85628C12.8805 9.83162 11.4752 11.4339 11.4752 12.7173C11.4752 13.3379 11.7191 14.0116 12.4506 14.0116C13.4039 14.0116 15.7863 12.1939 16.0521 11.4559L16.0081 11.0499C14.5112 12.4496 13.6468 12.9309 13.3388 12.9309C13.073 12.9309 12.9511 12.621 12.9511 12.4065C13.1223 11.4878 13.5663 10.642 14.2252 9.9792L15.9431 7.96253C16.076 7.80212 16.4197 7.42812 16.4197 7.21362C16.4197 6.99912 15.9541 6.29695 15.6552 6.29695C15.4554 6.29695 15.2345 6.52153 15.0897 6.63887C13.6376 7.75078 12.3635 8.91678 11.0004 10.0498L10.9784 10.0287C11.4367 9.35495 12.6284 7.77187 12.6284 6.98078C12.615 6.7776 12.5231 6.58757 12.3723 6.45077C12.2215 6.31396 12.0234 6.24106 11.8199 6.24745C11.3616 6.25845 10.4238 7.06053 10.0141 7.43453L8.2504 9.07078L8.2284 9.0497C8.50398 8.44757 8.7338 7.82553 8.9159 7.18887C8.91064 7.0437 8.86457 6.90296 8.78298 6.78278L8.51715 6.33362C8.40623 6.15028 8.36223 6.00178 8.0964 5.9917C7.37449 6.16448 6.67343 6.41497 6.00548 6.73878L4.75332 7.2952L4.72215 7.27412C5.1539 6.90745 5.30973 6.7177 5.30973 6.65353C5.30973 6.58937 5.23182 6.54628 5.17682 6.54628C5.07498 6.57453 4.98093 6.62563 4.90182 6.6957C3.91548 7.36945 3.98515 7.34837 3.88248 7.4767C3.74234 7.88603 3.64646 8.30919 3.59648 8.73895C3.54148 8.99562 3.68815 9.13495 3.86232 9.13495C4.4279 9.13495 3.89532 9.11387 7.30898 7.57662C7.41662 7.51503 7.5337 7.47167 7.65548 7.44828C7.84432 7.44828 8.04323 7.66187 6.87998 9.67303L4.56082 12.4899C4.68273 12.8218 4.90457 13.666 5.36932 13.666C5.72407 13.666 6.0009 13.2599 6.18973 13.0134L7.85165 10.8564L10.51 8.0762C10.5879 7.99095 10.7208 7.85162 10.8537 7.85162C10.9866 7.85162 11.0196 7.96895 10.8317 8.30995C10.3221 9.2379 9.72082 10.1125 9.0369 10.9206L8.2834 11.8721Z"/>
</svg>
                </div>
                <div class="award-card__info">
                  <div class="award-card__info-title">
                    {model.title}
                  </div>
                </div>
              </div>
            </div>
          {/each}
        </div>
      </div>
    </div>
  </div>


{:else}
  <!-- Главный каталог -->
 
  
      <div class="category">
        <div class="category__title">Нашивки</div>
        <div class="category__items">
          {#each sortedShopList as model}
            {#if !searchText || model.title.toLowerCase().includes(searchText.toLowerCase())}
              <div class="category__items-item" on:click={() => selectModel(model)}>
                <div class={`award-card award-card__rarity-${model.line}`}>
                  <div class="award-card__rarity">
                    <svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
  <path d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z" fill="#8958FF"/>
</svg>
                  </div>
                  <div class="award-card__rarity__radial-gradient"></div>
            <img data-v-30e95d59 class="award-card__media clothes" src="{model.img}" alt="{model.title}" />
                  <div class="award-card__price">
                    {#if model.oldPrice}
                      <div class="old-price">
                        {model.oldPrice.toLocaleString('ru-RU')}
                      </div>
                    {/if}
                    <div class="current-price">
                      {model.price.toLocaleString('ru-RU')}
                    </div>
                    <svg class="icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 20 20" fill="none">
<path class="back" d="M9.99931 19.1668C15.0619 19.1668 19.166 15.0628 19.166 10.0002C19.166 4.93755 15.0619 0.833496 9.99931 0.833496C4.9367 0.833496 0.832642 4.93755 0.832642 10.0002C0.832642 15.0628 4.9367 19.1668 9.99931 19.1668Z"/>
<path class="m" d="M8.2834 11.8721C8.05388 12.1028 7.89926 12.3974 7.83973 12.7173C7.83973 13.0271 7.99465 13.7971 8.42732 13.7971C8.70232 13.7971 9.23582 13.1875 9.40265 12.984C10.625 11.4209 12.0484 10.0259 13.6358 8.8352L13.6578 8.85628C12.8805 9.83162 11.4752 11.4339 11.4752 12.7173C11.4752 13.3379 11.7191 14.0116 12.4506 14.0116C13.4039 14.0116 15.7863 12.1939 16.0521 11.4559L16.0081 11.0499C14.5112 12.4496 13.6468 12.9309 13.3388 12.9309C13.073 12.9309 12.9511 12.621 12.9511 12.4065C13.1223 11.4878 13.5663 10.642 14.2252 9.9792L15.9431 7.96253C16.076 7.80212 16.4197 7.42812 16.4197 7.21362C16.4197 6.99912 15.9541 6.29695 15.6552 6.29695C15.4554 6.29695 15.2345 6.52153 15.0897 6.63887C13.6376 7.75078 12.3635 8.91678 11.0004 10.0498L10.9784 10.0287C11.4367 9.35495 12.6284 7.77187 12.6284 6.98078C12.615 6.7776 12.5231 6.58757 12.3723 6.45077C12.2215 6.31396 12.0234 6.24106 11.8199 6.24745C11.3616 6.25845 10.4238 7.06053 10.0141 7.43453L8.2504 9.07078L8.2284 9.0497C8.50398 8.44757 8.7338 7.82553 8.9159 7.18887C8.91064 7.0437 8.86457 6.90296 8.78298 6.78278L8.51715 6.33362C8.40623 6.15028 8.36223 6.00178 8.0964 5.9917C7.37449 6.16448 6.67343 6.41497 6.00548 6.73878L4.75332 7.2952L4.72215 7.27412C5.1539 6.90745 5.30973 6.7177 5.30973 6.65353C5.30973 6.58937 5.23182 6.54628 5.17682 6.54628C5.07498 6.57453 4.98093 6.62563 4.90182 6.6957C3.91548 7.36945 3.98515 7.34837 3.88248 7.4767C3.74234 7.88603 3.64646 8.30919 3.59648 8.73895C3.54148 8.99562 3.68815 9.13495 3.86232 9.13495C4.4279 9.13495 3.89532 9.11387 7.30898 7.57662C7.41662 7.51503 7.5337 7.47167 7.65548 7.44828C7.84432 7.44828 8.04323 7.66187 6.87998 9.67303L4.56082 12.4899C4.68273 12.8218 4.90457 13.666 5.36932 13.666C5.72407 13.666 6.0009 13.2599 6.18973 13.0134L7.85165 10.8564L10.51 8.0762C10.5879 7.99095 10.7208 7.85162 10.8537 7.85162C10.9866 7.85162 11.0196 7.96895 10.8317 8.30995C10.3221 9.2379 9.72082 10.1125 9.0369 10.9206L8.2834 11.8721Z"/>
</svg>
                  </div>
                  <div class="award-card__info">
                    {#if model.discount}
                      <div class="award-card__info-discount">
                        {model.discount}
                      </div>
                    {/if}
                    <div class="award-card__info-title">
                      {model.title}
                    </div>
                  </div>
                </div>
              </div>
            {/if}
          {/each}
        </div>
     
  </div>

{/if}
