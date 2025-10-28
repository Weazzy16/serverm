<script>
  import { createEventDispatcher } from 'svelte';
  import './carServic.css'
  export let cdn2 = 'https://cdn.majestic-files.com/public/master/static/icons/global/vehicles';
    export let cdn = 'https://cdn.majestic-files.com/public/master/static/icons/global/vehicles';

  const dispatch = createEventDispatcher();
  
  // Categories configuration
  const categories = ['all', 'private', 'rented', 'duplicate', 'family', 'commercial'];
  const LENGTH_WIDTH = 25;
  
  // Component state
  let search = '';
  let page = 'all';
  let active = null;
  
  // Mock data for demonstration
  let carService = [
    {
      id: 1,
      model: 'adder',
      title: 'Adder',
      number: 'ABC 123',
      isArrested: false,
      endTime: null,
      familyId: null,
      duplicate: false,
      commercial: false,
      isRent: false
    },
    {
      id: 2,
      model: 'zentorno',
      title: 'Zentorno',
      number: 'XYZ 456',
      isArrested: true,
      endTime: null,
      familyId: null,
      duplicate: false,
      commercial: false,
      isRent: false
    },
    {
      id: 3,
      model: 'elegy',
      title: 'Elegy',
      number: 'DEF 789',
      isArrested: false,
      endTime: '12-31-2024 23:59:59',
      familyId: null,
      duplicate: false,
      commercial: false,
      isRent: true
    }
  ];
  
  let totalSlots = 1;
  
  // Computed values
  $: activeVehicles = (() => {
    switch (page) {
      case 'all':
        return carService;
      case 'private':
        return carService.filter(v => !v.familyId && !v.duplicate && !v.endTime);
      case 'duplicate':
        return carService.filter(v => v.duplicate);
      case 'family':
        return carService.filter(v => v.familyId);
      case 'rented':
        return carService.filter(v => v.endTime);
      case 'commercial':
        return carService.filter(v => v.commercial);
      default:
        return carService;
    }
  })();
  
  $: activeVehiclesIndexed = (() => {
    const vehicles = activeVehicles.map(v => ({ ...v }));
    if (search.length) {
      return vehicles.filter(v => {
        const model = v.model.toLowerCase();
        const title = v.title.toLowerCase();
        const searchTerm = search.toLowerCase();
        return model.includes(searchTerm) || title.includes(searchTerm);
      });
    }
    return vehicles;
  })();
  
  // Functions
  function setPage(newPage) {
    page = newPage;
  }
  
  function setActive(vehicle) {
    active = vehicle;
  }
  
  function getValidVehicleName(vehicle) {
    return vehicle.title || vehicle.model || 'Unknown Vehicle';
  }
  
  function formatTime(time) {
    return time.toString().length === 1 ? `0${time}` : time;
  }
  
  function spawnVehicle() {
    if (active && active.id) {
      console.log('Spawning vehicle:', active.id);
      // Call client function
      // this.$_callClient("iphone.carService.find", active.id)
    }
  }
  
  function cancelRent() {
    if (active && active.id) {
      console.log('Cancelling rent:', active.id);
      // Call client function
      // this.$_callClient("iphone.carService.cancelRent", active.id)
    }
  }
  
  function closeApp() {
    dispatch('close');
  }
  
  function closingStart() {
    // Handle closing gesture start
    console.log('Closing gesture started');
  }
  
  // Category icons mapping
  function getCategoryIcon(category) {
    const icons = {
      all: 'all',
      private: 'private',
      rented: 'rented', 
      duplicate: 'duplicate',
      family: 'family',
      commercial: 'commercial'
    };
    return `${cdn2}/${icons[category]}.svg`;
  }
  
  // Translations (simplified)
  const translations = {
    search: 'Поиск',
    available: 'Доступно слотов',
    arrested: 'Арестован',
    deliver: 'Доставить',
    'route-parking-fine': 'Маршрут к штрафстоянке',
    'cancel-rent': 'Отменить аренду',
    days: 'д.',
    hours: 'ч.',
    minutes: 'мин.'
  };
  
  function t(key) {
    return translations[key] || key;
  }
</script>

<div class="carService full-width full-height">
  <div class="main-content align-center column-block full-width full-height">
    <img 
      class="backgrounds" 
      src="{cdn}/img/iPhone/apps/carService/v2/background.png" 
      alt="" 
    />
    
    <header class="column-block full-width">
      <!-- Categories -->
      <div class="categories row-block full-width">
        {#each categories as category}
          <div 
            class="category" 
            class:selected={page === category}
            on:click={() => setPage(category)}
          >
            <img src="{getCategoryIcon(category)}" alt="{category}" />
          </div>
        {/each}
      </div>
      
      <!-- Search -->
      <div class="search row-block full-width align-center justify-between">
        <input 
          bind:value={search}
          placeholder={t('search')}
          type="text" 
          maxlength="15" 
          class="search__input" 
        />
        <div class="search-icon">
          <svg width="16" height="16" viewBox="0 0 24 24">
            <path d="M15.5 14h-.79l-.28-.27C15.41 12.59 16 11.11 16 9.5 16 5.91 13.09 3 9.5 3S3 5.91 3 9.5 5.91 16 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
          </svg>
        </div>
        <div class="search__icon" class:filled={search}>
          <svg width="16" height="16" viewBox="0 0 24 24">
            <path d="M9.5,3A6.5,6.5 0 0,1 16,9.5C16,11.11 15.41,12.59 14.44,13.73L14.71,14H15.5L20.5,19L19,20.5L14,15.5V14.71L13.73,14.44C12.59,15.41 11.11,16 9.5,16A6.5,6.5 0 0,1 3,9.5A6.5,6.5 0 0,1 9.5,3M9.5,5C8.67,5 7.91,5.25 7.27,5.69C6.64,6.13 6.13,6.64 5.69,7.27C5.25,7.91 5,8.67 5,9.5C5,10.33 5.25,11.09 5.69,11.73C6.13,12.36 6.64,12.87 7.27,13.31C7.91,13.75 8.67,14 9.5,14C10.33,14 11.09,13.75 11.73,13.31C12.36,12.87 12.87,12.36 13.31,11.73C13.75,11.09 14,10.33 14,9.5C14,8.67 13.75,7.91 13.31,7.27C12.87,6.64 12.36,6.13 11.73,5.69C11.09,5.25 10.33,5 9.5,5Z" />
          </svg>
        </div>
      </div>
      
      <!-- Car count -->
      <div class="car-count">
        <p>
          {t('available')}: <span>{totalSlots}</span>
        </p>
      </div>
    </header>
    
    <!-- Car list -->
    <div 
      class="car-list full-width full-height column-block"
      class:double-btn={active && active.endTime}
    >
      {#each activeVehiclesIndexed as vehicle, index (vehicle.id)}
        <div 
          class="tile-block full-width align-center"
          class:active={active && active.id === vehicle.id}
          class:arrested={vehicle.isArrested}
          class:expanded={getValidVehicleName(vehicle).length >= LENGTH_WIDTH}
          on:click={() => setActive(vehicle)}
        >
          <img 
            src="{cdn}/img/vehicles/{vehicle.model}.png" 
            class="tile-block__picture full-height" 
            alt="" 
          />
          <div class="content full-height">
            <div class="content__title">
              {getValidVehicleName(vehicle)}
            </div>
            <div class="car-info row-block">
              <span class="car-info__text-number">{vehicle.number}</span>
              
              {#if vehicle.isArrested}
                <div class="arrested row-block align-center">
                  <svg class="arrested__picture" width="16" height="16" viewBox="0 0 24 24">
                    <path d="M12,1L3,5V11C3,16.55 6.84,21.74 12,23C17.16,21.74 21,16.55 21,11V5L12,1M12,7C13.4,7 14.8,8.6 14.8,10V11.5C15.4,11.5 16,12.1 16,12.7V16.2C16,16.8 15.4,17.3 14.8,17.3H9.2C8.6,17.3 8,16.8 8,16.2V12.7C8,12.1 8.6,11.5 9.2,11.5V10C9.2,8.6 10.6,7 12,7M12,8.2C11.2,8.2 10.5,8.7 10.5,9.5V11.5H13.5V9.5C13.5,8.7 12.8,8.2 12,8.2Z" />
                  </svg>
                  <span class="arrested__value">{t('arrested')}</span>
                </div>
              {:else if vehicle.endTime}
                <div class="rent-time align-center row-block">
                  <svg class="rent-time__picture" width="16" height="16" viewBox="0 0 24 24">
                    <path d="M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M16.2,16.2L11,13V7H12.5V12.2L17,14.7L16.2,16.2Z" />
                  </svg>
                  <span class="rent-time__value">
                    <!-- Simplified countdown display -->
                    2ч. 30мин.
                  </span>
                </div>
              {/if}
            </div>
          </div>
        </div>
      {/each}
    </div>
    
    <!-- Control buttons -->
    {#if active}
      <div class="control-button-container full-width">
        <div 
          class="control-button full-width"
          class:arrested={active.isArrested}
          class:disabled={false}
          on:click={spawnVehicle}
        >
          <span>
            {active.isArrested ? t('route-parking-fine') : t('deliver')}
          </span>
        </div>
        
        {#if active && active.endTime && active.isRent}
          <div 
            class="control-button cancelRent full-width"
            on:click={cancelRent}
          >
            <span>{t('cancel-rent')}</span>
          </div>
        {/if}
      </div>
    {/if}
  </div>
  
  <!-- Close block -->
  <div 
    class="close-block full-width"
    on:mousedown={closingStart}
    on:click={closeApp}
  >
    <div class="close-block-handler"></div>
  </div>
</div>
