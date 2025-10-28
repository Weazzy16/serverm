<script>
  import { executeClient } from 'api/rage';
  import SVGComponent from './SVGComponent.svelte';

  import { shopList as items } from './weaponpag.js';
$: parents = items.filter(it => Array.isArray(it.list) && it.list.length > 0);

  // 2) группировка по категориям (легкий бронежилет, винтовки, дробовики…)
  $: grouped = Object.entries(
    parents.reduce((map, it) => {
      const cat = it.category || 'Прочее';
      (map[cat] = map[cat] || []).push(it);
      return map;
    }, {})
  );

  let selectedWeapon = null;    // выбранная «родительская» карточка
  let selectedSkin   = null;    // индекс или объект выбранного скина внутри list
  let modalOpened    = false;
  let trackStatCount = 1;

  // дочерний список для текущего оружия
  $: relatedSkins = selectedWeapon?.list ?? [];

  // Клик по большому «родительскому» элементу:
  function openWeapon(weapon) {
    selectedWeapon = weapon;
    selectedSkin = null;          // вначале показываем дефолтный
  }

  // Клик по маленькому «дочернему» скину:
  function pickSkin(skin) {
    selectedSkin = skin;
  }

  function applySkin(id) {
    // …
  }
  function openModal() { modalOpened = true }
  function installTrackStat() { /* … */ }

  function goBack() {
    
    modalOpened = false;
  }
</script>
<!-- svelte-ignore a11y-click-events-have-key-events -->
{#if selectedWeapon}
  <!-- Детальный просмотр -->
  <div class="itemPageW">
    <div class="contentW">
      <div class="header row-block align-center justify-between">
       
        <div class="title">{selectedWeapon.title}</div>
       <div class="availableSkins">
          {"Доступно скинов: "}
          <span class="value">{selectedWeapon.length} из {selectedWeapon.list?.length ?? 0}</span>
        </div>
        <div class="btns-wrapperW">
          {#if selectedWeapon.weaponId !== 353}
            <div class="btn setTrackStat" on:click={openModal}>
            УСТАНОВИТЬ STATТRAK
          </div>
          {/if}
        <div class="btn applySkin" on:click={() => applySkin(selectedWeapon.id)}>
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
          <img class="item__image" src={selectedWeapon.img} alt={selectedWeapon.title} />
          <div class="item__skin-info">
            <div class="item__skin-info-text">
              {selectedWeapon.title}
            </div>
          </div>
        </div>

        

        <div class="other-skins row-block">
          {#each relatedSkins  as skin}
            <div
              class="skin column-block {skin.id === selectedWeapon.id ? 'selected' : ''}"
              on:click={() => pickSkin(skin)}
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
              <div class="btn cancel" on:click={goBack}>ОТМЕНА</div>
            </div>
          </div>
        </div>
      {/if}
    </div>
  </div>
{:else}
  <!-- Сетка всех скинов -->

    
    <div class="category">
        {#each grouped as [category, weapons]}
      <div class="category__title">{category}</div>
      <div class="category__items">
        
        {#each weapons as w}
          <div class="category__items-item" on:click={() => openWeapon(w)}>
            <div class={`award-card award-card__rarity-${w.rarity}`}>
              <div class="award-card__rarity"><svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
  <path d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z" fill="#8958FF"/>
</svg></div>
              <div class="award-card__rarity__radial-gradient"></div>
              <img class="award-card__media weaponSkin" src={w.img} alt={w.title} />
              
              <div class="award-card__info">
                {#if w.discount}
                  <div class="award-card__info-discount">-{w.discount}%</div>
                {/if}
                <div class="award-card__info-title">{w.title}</div>
              </div>
            </div>
 </div>
     
   
        {/each}
      
   
</div>
 {/each}
</div>
{/if}