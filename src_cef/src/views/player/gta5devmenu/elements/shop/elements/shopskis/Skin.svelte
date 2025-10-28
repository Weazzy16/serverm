<script>
  import { executeClient } from 'api/rage';
  import SVGComponent from './SVGComponent.svelte';

  import { shopList as items } from './weaponpag.js';

  // Для поиска
  export let searchText = '';
$: mainItems = items.filter(it => Array.isArray(it.list));
  // Отфильтрованный список для сетки всех скинов
 $: filteredMain = mainItems.filter(
    it => !searchText || it.title.toLowerCase().includes(searchText.toLowerCase())
  );

  // Выбранный для детального просмотра
  let selected = null;
  let modalOpened = false;
  let trackStatCount = 1;

  // Скины только для текущего оружия
   $: related = selected ? selected.list : [];
 $: grouped = Object.entries(
    filteredMain.reduce((acc, it) => {
      const cat = it.category || 'Прочее';
      (acc[cat] = acc[cat] || []).push(it);
      return acc;
    }, {})
  );
  function openDetail(skin) {
    selected = skin;
    window.pageloadf2(0);
    window.pagenameitemf2(skin.title);
  }

  function closeDetail() {
    selected = null;
    modalOpened = false;
  }

  function applySkin(id) {
    executeClient('client.skin.apply', id);
  }

  function openModal() {
    modalOpened = true;
  }

  function installTrackStat() {
    executeClient('client.skin.installTrackStat', selected.id);
    modalOpened = false;
  }
</script>
<!-- svelte-ignore a11y-click-events-have-key-events -->
{#if selected}
  <!-- Детальный просмотр -->
  <div class="itemPageW">
    <div class="contentW">
      <div class="header row-block align-center justify-between">
       
        <div class="title">{selected.title}</div>
       <div class="availableSkins">
          {"Доступно скинов: "}
          <span class="value">{related.length} из {related.length}</span>
        </div>
        <div class="btns-wrapperW">
            <div class="btn setTrackStat" on:click={openModal}>
            УСТАНОВИТЬ STATТRAK
          </div>
        <div class="btn applySkin" on:click={() => applySkin(selected.id)}>
          УСТАНОВИТЬ
        </div>
      </div>
      </div>

      <div class="contentW">
        <div class="item">
          <div class="item__rarity"><svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
  <path d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z" fill="#8958FF"/>
</svg></div>
          <div class="item__rarity__radial-gradient" />
          <img class="item__image" src={selected.img} alt={selected.title} />
          <div class="item__skin-info">
            <div class="item__skin-info-text">
              {selected.title}
            </div>
          </div>
        </div>

        

        <div class="other-skins row-block">
          {#each related as skin}
            <div
              class="skin column-block {skin.id === selected.id ? 'selected' : ''}"
              on:click={() => openDetail(skin)}
            >
              <div class="award-card award-card__rarity-{skin.rarity}">
                <div class="award-card__rarity"><svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
  <path d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z" fill="#8958FF"/>
</svg></div>
                <div class="award-card__rarity__radial-gradient" />
                <img class="award-card__media" src={skin.img} alt={skin.title} />
                <div class="award-card__info">
                  <div class="award-card__info-title">{skin.title}</div>
                </div>
              </div>
            </div>
          {/each}
        </div>
      </div>
 
  {#if modalOpened}
        <!-- Модалка установки TrackStat -->
        <div class="modal">
          <div class="wrapperW">
            <div class="title">
              УСТАНОВИТЬ СЧЁТЧИК
            </div>
            <div class="description">
              StatTrak подсчитывает количество убийств, совершённых выбранным скином. Счётчик закрепляется навсегда.</div>
               <div class="counters">
              Счётчиков в инвентаре: <span class="value">{trackStatCount}</span>
            </div>
            <div class="btns">
              <div class="btn set" on:click={installTrackStat}>УСТАНОВИТЬ</div>
              <div class="btn cancel" on:click={openModal}>ОТМЕНА</div>
            </div>
          </div>
        </div>
      {/if}
    </div>
  </div>
{:else}
  <!-- Сетка всех скинов -->

    
    <div class="category">
        {#each grouped as [category, items]}
      <div class="category__title">{category}</div>
      <div class="category__items">
        
        {#each items as item}
          <div class="category__items-item" on:click={() => openDetail(item)}>
            <div class={`award-card award-card__rarity-${item.rarity}`}>
              <div class="award-card__rarity"><svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
  <path d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z" fill="#8958FF"/>
</svg></div>
              <div class="award-card__rarity__radial-gradient"></div>
              <img class="award-card__media weaponSkin" src={item.img} alt={item.title} />
              
              <div class="award-card__info">
                {#if item.discount}
                  <div class="award-card__info-discount">-{item.discount}%</div>
                {/if}
                <div class="award-card__info-title">{item.title}</div>
              </div>
            </div>
 </div>
     
   
        {/each}
      
   
</div>
 {/each}
</div>
{/if}