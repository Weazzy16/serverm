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

  function goBacks() {
    selectedWeapon = null;

  }
</script>
<!-- svelte-ignore a11y-click-events-have-key-events -->
{#if selectedWeapon}
<div class="backs__icon" on:click={goBacks}  >
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 9 15" fill="none">
	<path opacity="0.5" d="M8 1.5L2 7.5L8 13.5" stroke-width="2"/>
</svg>
</div>
  <!-- Детальный просмотр -->
  <div class="itemPageW">
    
    <div class="contentW">
      <div class="header row-block align-center justify-between">
       <svg class="installTrackStatIsAvailable" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 18 18" fill="none">
	<path d="M9 1.5C4.8645 1.5 1.5 4.8645 1.5 9C1.5 13.1355 4.8645 16.5 9 16.5C13.1355 16.5 16.5 13.1355 16.5 9C16.5 4.8645 13.1355 1.5 9 1.5ZM9.75 14.9482V12.75H8.25V14.9482C6.93013 14.78 5.70349 14.1782 4.76265 13.2374C3.8218 12.2965 3.22001 11.0699 3.05175 9.75H5.25V8.25H3.05175C3.22001 6.93013 3.8218 5.70349 4.76265 4.76265C5.70349 3.8218 6.93013 3.22001 8.25 3.05175V5.25H9.75V3.05175C11.0699 3.21989 12.2966 3.82164 13.2375 4.76251C14.1784 5.70338 14.7801 6.93008 14.9482 8.25H12.75V9.75H14.9482C14.78 11.0699 14.1782 12.2965 13.2374 13.2374C12.2965 14.1782 11.0699 14.78 9.75 14.9482Z"/>
</svg>
        <div class="title">{(selectedSkin ?? selectedWeapon).title}</div>
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
          <div class="item__rarity"><svg
    fill="none"
    viewBox="0 0 16 4"
    xmlns="http://www.w3.org/2000/svg"
>
  <path
    d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z"
    fill="#8958FF"
  />
</svg>
</div>
          <div class="item__rarity__radial-gradient" />
          <img class="item__image" src={(selectedSkin ?? selectedWeapon).img} alt={(selectedSkin ?? selectedWeapon).title} />
          <div class="item__skin-info">
            <div class="item__skin-info-text">
              {(selectedSkin ?? selectedWeapon).title}
            </div>
          </div>
        </div>

        

        <div class="other-skins row-block">
          {#each relatedSkins  as skin}
          
         
            <div
              class="skin column-block class:selected={skin === selectedSkin}"
              on:click={() => pickSkin(skin)}
            >
            <div class="skin__icon">
            <svg class="success" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 18 18" fill="none">
	<circle cx="9" cy="9" r="5.5"/>
	<path d="M9 1.5C4.8645 1.5 1.5 4.8645 1.5 9C1.5 13.1355 4.8645 16.5 9 16.5C13.1355 16.5 16.5 13.1355 16.5 9C16.5 4.8645 13.1355 1.5 9 1.5ZM7.50075 12.3098L4.716 9.531L5.775 8.469L7.49925 10.1902L11.4698 6.21975L12.5303 7.28025L7.50075 12.3098Z"/>
</svg> 

<svg class="lock" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="none">
	<path d="M9.99967 1.66602C7.70217 1.66602 5.83301 3.53518 5.83301 5.83268V8.33268H4.99967C4.55765 8.33268 4.13372 8.50828 3.82116 8.82084C3.5086 9.1334 3.33301 9.55732 3.33301 9.99935V16.666C3.33301 17.108 3.5086 17.532 3.82116 17.8445C4.13372 18.1571 4.55765 18.3327 4.99967 18.3327H14.9997C15.4417 18.3327 15.8656 18.1571 16.1782 17.8445C16.4907 17.532 16.6663 17.108 16.6663 16.666V9.99935C16.6663 9.55732 16.4907 9.1334 16.1782 8.82084C15.8656 8.50828 15.4417 8.33268 14.9997 8.33268H14.1663V5.83268C14.1663 3.53518 12.2972 1.66602 9.99967 1.66602ZM7.49967 5.83268C7.49967 4.45435 8.62134 3.33268 9.99967 3.33268C11.378 3.33268 12.4997 4.45435 12.4997 5.83268V8.33268H7.49967V5.83268ZM10.833 14.7685V16.666H9.16634V14.7685C8.875 14.6017 8.64106 14.3505 8.49537 14.048C8.34968 13.7455 8.29908 13.406 8.35024 13.0742C8.40141 12.7423 8.55194 12.4338 8.78198 12.1893C9.01202 11.9448 9.31077 11.7757 9.63884 11.7043C9.88252 11.6505 10.1352 11.652 10.3782 11.7088C10.6212 11.7655 10.8484 11.8761 11.043 12.0324C11.2376 12.1887 11.3946 12.3866 11.5025 12.6117C11.6104 12.8367 11.6664 13.0831 11.6663 13.3327C11.6659 13.6241 11.5886 13.9103 11.4423 14.1624C11.296 14.4145 11.0858 14.6235 10.833 14.7685Z"/>
</svg>
</div>
<div class="award-card__trackStats">
  <div class="award-card__trackStats-value">1</div>
  <svg class="award-card__trackStats-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 18 18" fill="none">
	<path d="M9 1.5C4.8645 1.5 1.5 4.8645 1.5 9C1.5 13.1355 4.8645 16.5 9 16.5C13.1355 16.5 16.5 13.1355 16.5 9C16.5 4.8645 13.1355 1.5 9 1.5ZM9.75 14.9482V12.75H8.25V14.9482C6.93013 14.78 5.70349 14.1782 4.76265 13.2374C3.8218 12.2965 3.22001 11.0699 3.05175 9.75H5.25V8.25H3.05175C3.22001 6.93013 3.8218 5.70349 4.76265 4.76265C5.70349 3.8218 6.93013 3.22001 8.25 3.05175V5.25H9.75V3.05175C11.0699 3.21989 12.2966 3.82164 13.2375 4.76251C14.1784 5.70338 14.7801 6.93008 14.9482 8.25H12.75V9.75H14.9482C14.78 11.0699 14.1782 12.2965 13.2374 13.2374C12.2965 14.1782 11.0699 14.78 9.75 14.9482Z"/>
</svg>
</div>
              <div class="award-card award-card__rarity-{skin.rarity}">
                <div class="award-card__rarity"><svg
    fill="none"
    viewBox="0 0 16 4"
    xmlns="http://www.w3.org/2000/svg"
>
  <path
    d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z"
    fill="#8958FF"
  />
</svg>
</div>
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
              <div class="award-card__rarity">  <svg
    fill="none"
    viewBox="0 0 16 4"
    xmlns="http://www.w3.org/2000/svg"
>
  <path
    d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z"
    fill="#8958FF"
  />
</svg>
</div>
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