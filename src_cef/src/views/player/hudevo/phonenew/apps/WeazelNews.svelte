<script>
  import { createEventDispatcher, onMount, onDestroy } from 'svelte';
      import './css/wn.css'

  export let cdn = 'https://cdn.majestic-files.com/public/master/static';
  
  const dispatch = createEventDispatcher();
  
  // Component state
  let currentPage = 'lent';
  let repeateItem = {};
  let saveInfo = null;
  
  // Pages configuration
  const pages = [
    { name: 'lent', title: 'Лента объявлений' },
    { name: 'advertisement', title: 'Реклама' },
    { name: 'createAdv', title: 'Создать объявление' }
  ];
  
  // Mock advertisements data
  let advertisements = [
    {
      id: 1,
      text: "Продаю автомобиль в отличном состоянии. Торг уместен.",
      userName: "Иван Иванов",
      userId: 12345,
      phoneNumber: "555-0123",
      createAt: Math.floor(Date.now() / 1000) - 3600, // 1 час назад
      isLocation: true,
      editorName: null,
      editorId: null
    },
    {
      id: 2, 
      text: "Требуется помощь в переезде. Оплата договорная.",
      userName: "Мария Петрова", 
      userId: 54321,
      phoneNumber: "555-0456",
      createAt: Math.floor(Date.now() / 1000) - 7200, // 2 часа назад
      isLocation: false,
      editorName: null,
      editorId: null
    }
  ];
  
  let historyAdvertisements = [
    {
      id: 101,
      text: "Искал работу водителем. Уже нашел, спасибо всем!",
      type: "JOB",
      phoneNumber: "555-0789",
      locationType: "CITY",
      createAt: Math.floor(Date.now() / 1000) - 86400, // 1 день назад
      status: "DONE",
      reasonCancel: "",
      isUrgent: false
    },
    {
      id: 102,
      text: "Продавал мотоцикл, сделка сорвалась по вине покупателя",
      type: "SELL", 
      phoneNumber: "555-0321",
      locationType: "DISTRICT",
      createAt: Math.floor(Date.now() / 1000) - 172800, // 2 дня назад
      status: "CANCEL",
      reasonCancel: "Покупатель не явился",
      isUrgent: true
    }
  ];
  
  // CreateAdv state
  let createAdvState = {
    type: 'NOTIFICATION', // NOTIFICATION или CHAT
    isPhone: true,
    location: 'NONE', // NONE, STATIC, UPDATE
    isUrgent: false,
    text: '',
    priceSymbolChat: 150,
    priceSymbol: 150,
    advQueue: 5,
    advQueueChat: 3,
    hasUpdateLocation: true
  };
  
  let modalType = null; // 'type' или 'location'
  const MAX_TEXT_LENGTH = 300;
  let hasAdmin = false; // Права администратора
  
  // Computed values
  $: currentPageData = pages.find(p => p.name === currentPage);
  $: pageTitle = currentPageData?.title || 'Лента объявлений';
  
  // Functions
  function setPage(pageName) {
    currentPage = pageName;
    console.log('WeazelNews page changed to:', pageName);
    
    // Notify server about page changes
    if (pageName === 'lent') {
      // callClient("W2C:IphoneWeazelNews:MainReady")
    } else if (pageName === 'advertisement') {
      // callClient("W2C:IphoneWeazelNews:HistoryReady") 
    }
  }
  
  function repeat(item) {
    repeateItem = item;
    setPage('createAdv');
    // callClient("W2C:IphoneWeazelNews:Repeat", { id: item.id })
  }
  
  function clearRepeateItem() {
    repeateItem = {};
  }
  
  function setSaveInfo(info) {
    saveInfo = info;
  }
  
  function closeApp() {
    dispatch('close');
  }
  
  function closingStart() {
    console.log('WeazelNews closing gesture started');
  }
  
  function getPageIconPath(pageName) {
    return `${cdn}/icons/iPhone/apps/weazelNewsV2/${pageName}.svg`;
  }
  
  // Card actions
  function callPhone(phoneNumber) {
    console.log('Calling phone:', phoneNumber);
    // callClient("W2C:IphoneWeazelNews:Call", { number: Number(phoneNumber) })
  }
  
  function sendMessage(phoneNumber) {
    console.log('Sending message to:', phoneNumber);
    // callClient("W2C:IphoneWeazelNews:Message", { number: Number(phoneNumber) })
  }
  
  function showLocation(itemId) {
    console.log('Showing location for:', itemId);
    // callClient("W2C:IphoneWeazelNews:Geo", { id: itemId })
  }
  
  function deleteItem(itemId) {
    if (hasAdmin) {
      console.log('Admin deleting item:', itemId);
      // callClient("W2C:IphoneWeazelNews:AdminDelete", { id: itemId })
    }
  }
  
  // Date formatting
  function formatDateTime(timestamp) {
    return new Date(timestamp * 1000).toLocaleString('ru-RU', {
      hour: '2-digit',
      minute: '2-digit',
      day: '2-digit',
      month: '2-digit', 
      year: 'numeric'
    });
  }
  
  function formatTime(timestamp) {
    return new Date(timestamp * 1000).toLocaleTimeString('ru-RU', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }
  
  // CreateAdv functions
  function openModal(type) {
    modalType = type;
  }
  
  function closeModal() {
    modalType = null;
  }
  
  function selectType(type) {
    createAdvState.type = type;
    closeModal();
  }
  
  function selectLocation(location) {
    createAdvState.location = location;
    if (location === 'STATIC') {
      // В оригинале здесь открывается карта
      console.log('Opening map for location selection');
    }
    closeModal();
  }
  
  function togglePhone() {
    createAdvState.isPhone = !createAdvState.isPhone;
  }
  
  function toggleLocation() {
    if (createAdvState.location === 'NONE') {
      openModal('location');
    } else {
      createAdvState.location = 'NONE';
    }
  }
  
  function toggleUrgent() {
    createAdvState.isUrgent = !createAdvState.isUrgent;
    if (createAdvState.isUrgent) {
      console.log('Urgent advertisement enabled');
    }
  }
  
  function handleTextInput(event) {
    const target = event.target;
    
    // Предотвращаем Enter
    if (event.key === 'Enter') {
      event.preventDefault();
      return;
    }
    
    // Предотвращаем двойные пробелы
    if (event.key === ' ') {
      const value = target.value;
      const cursorPos = target.selectionStart;
      if (cursorPos > 0 && value[cursorPos - 1] === ' ') {
        event.preventDefault();
        return;
      }
    }
  }
  
  function handlePaste(event) {
    event.preventDefault();
    const pastedText = (event.clipboardData?.getData('text/plain') || '').replace(/\s+/g, ' ');
    
    const target = event.target;
    const start = target.selectionStart || 0;
    const end = target.selectionEnd || 0;
    const currentValue = target.value;
    
    const newValue = currentValue.substring(0, start) + pastedText + currentValue.substring(end);
    target.value = newValue;
    createAdvState.text = newValue;
    
    const newCursor = start + pastedText.length;
    target.setSelectionRange(newCursor, newCursor);
  }
  
  function calculatePrice() {
    const pricePerSymbol = createAdvState.type === 'CHAT' ? createAdvState.priceSymbolChat : createAdvState.priceSymbol;
    const basePrice = createAdvState.text.length * pricePerSymbol;
    return createAdvState.isUrgent ? basePrice * 2 : basePrice;
  }
  
  function submitAdvertisement() {
    if (createAdvState.text.trim().length < 20) {
      console.log('Error: Text too short (minimum 20 characters)');
      return;
    }
    
    console.log('Submitting advertisement:', {
      type: createAdvState.type,
      isPhone: createAdvState.isPhone,
      location: createAdvState.location,
      isUrgent: createAdvState.isUrgent,
      text: createAdvState.text
    });
    
    // В оригинале здесь вызов: callClient("W2C:IphoneWeazelNews:Create", data)
    
    resetForm();
  }
  
  function resetForm() {
    createAdvState.type = 'NOTIFICATION';
    createAdvState.isPhone = true;
    createAdvState.location = 'NONE';
    createAdvState.isUrgent = false;
    createAdvState.text = '';
  }
  
  // Computed values for CreateAdv
  $: isTextEmpty = createAdvState.text.trim().length === 0;
  $: textLength = createAdvState.text.length;
  $: pricePerSymbol = createAdvState.type === 'CHAT' ? createAdvState.priceSymbolChat : createAdvState.priceSymbol;
  $: queueCount = createAdvState.type === 'CHAT' ? createAdvState.advQueueChat : createAdvState.advQueue;
  $: totalPrice = calculatePrice();
  
  // Инициализация при повторе объявления
  function initializeFromRepeat() {
    if (repeateItem && repeateItem.text) {
      createAdvState.type = repeateItem.type || 'NOTIFICATION';
      createAdvState.text = repeateItem.text;
      createAdvState.isPhone = !!repeateItem.phoneNumber;
      createAdvState.location = repeateItem.locationType || 'NONE';
      createAdvState.isUrgent = repeateItem.isUrgent || false;
    }
  }
  
  // Lifecycle
  onMount(() => {
    console.log('WeazelNews app mounted');
    
    // Инициализация данных при повторе объявления
    initializeFromRepeat();
  });
  
  onDestroy(() => {
    console.log('WeazelNews app destroyed');
    // Очистка повторяемого элемента
    clearRepeateItem();
    // Notify server that app was closed
    // В оригинале: callClient("W2C:IphoneWeazelNews:Destroyed")
  });
</script>

<div class="weazel">
  <!-- Header -->
  <div class="weazel-header">
    <svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 30 30" fill="none">
<rect width="30" height="30" rx="8" fill="url(#paint0_linear_3705_59550)"/>
<path d="M26.3249 15.29H17.9414V16.0613H26.3249V15.29Z" fill="#FEFEFE"/>
<path d="M26.3249 16.6787H17.9414V17.45H26.3249V16.6787Z" fill="#FEFEFE"/>
<path d="M26.3249 18.0449H17.9414V18.8162H26.3249V18.0449Z" fill="#FEFEFE"/>
<path d="M26.3249 19.4326H17.9414V20.2039H26.3249V19.4326Z" fill="#FEFEFE"/>
<path fill-rule="evenodd" clip-rule="evenodd" d="M3.67188 20.2037H17.1658V15.29H3.67188V20.2037ZM14.2153 17.0899C14.2931 17.8922 15.4576 18.009 15.7215 18.1544C15.8035 18.2161 15.8626 18.3031 15.8897 18.4017C15.9168 18.5003 15.9102 18.605 15.8711 18.6996C15.8319 18.7941 15.7624 18.8731 15.6733 18.9242C15.5842 18.9754 15.4806 18.9958 15.3786 18.9823C14.7235 18.9823 14.6556 18.2523 14.6556 18.2523L14.0488 18.3448C14.2258 19.683 15.2941 19.6094 15.2941 19.6094C16.8804 19.5486 16.698 18.1538 16.3435 17.7493C15.989 17.3448 14.7694 17.2773 14.7658 16.8734C14.7621 16.4695 15.2158 16.4573 15.2158 16.4573C15.2938 16.4443 15.3739 16.4489 15.4499 16.4708C15.526 16.4928 15.596 16.5316 15.6549 16.5842C15.7138 16.6368 15.7599 16.702 15.7899 16.7748C15.8198 16.8476 15.8329 16.9262 15.828 17.0048L16.458 16.9585C16.4574 16.6616 16.3432 16.376 16.1386 16.1596C15.9339 15.9433 15.6542 15.8122 15.356 15.7931C15.1877 15.7786 15.0184 15.8037 14.8618 15.8664C14.7051 15.9291 14.5656 16.0276 14.4544 16.154C14.3433 16.2803 14.2638 16.4309 14.2222 16.5935C14.1807 16.7562 14.1783 16.9262 14.2153 17.0899ZM11.0427 18.138L10.5902 15.8679H9.89963L10.6276 19.548H11.4498L11.9684 16.9415L12.487 19.548H13.259L13.9466 16.099L13.9711 15.9695L13.9931 15.8679H13.3031L13.2847 15.967V15.9713L13.2786 15.9993L13.2664 16.0638L13.2633 16.082L13.1917 16.4366L12.8782 18.0181L12.4496 15.8679H11.4933L11.0427 18.138ZM9.68167 15.8679H7.45371V19.5467H9.72943V18.8837H8.06841V17.919H9.54943V17.2992H8.06841V16.5309H9.68167V15.8679ZM4.96739 15.8679H4.37351V19.5498H4.94779V17.1337L6.2133 19.5504H6.78759V15.8703H6.21514V18.2548L4.96739 15.8679Z" fill="#FEFEFE"/>
<path d="M24.2914 13.6404V9.7998H23.3045V13.6404H20.7331V12.4719H22.7045V11.5906H20.7331V10.6848H22.9065V9.80102H19.7535V13.6404H17.4135L19.5171 10.6855H19.5202V9.80102H16.3641V10.6848H18.2767L16.219 13.5784L14.8929 9.80285H13.8759L12.529 13.6404H10.1798V12.4719H12.1512V11.5906H10.1798V10.6848H12.3533V9.80102H9.19531V14.5243H13.2318L13.6047 13.4598H15.1616L15.5345 14.5243H26.3247V13.6404H24.2914ZM13.9176 12.576L14.3865 11.2432L14.8555 12.576H13.9176Z" fill="#FEFEFE"/>
<path d="M5.7431 14.524L6.43065 11.1785L7.12432 14.524H8.15534L9.0682 10.1L9.10188 9.93154L9.13004 9.7959H8.208L8.18351 9.9285L8.13085 10.1755L7.64106 12.5569L7.06861 9.7959H5.79575L5.19636 12.7156L4.59392 9.80076H3.67188L4.64657 14.524H5.7431Z" fill="#FEFEFE"/>
<defs>
<linearGradient id="paint0_linear_3705_59550" x1="7.54959e-07" y1="-0.360001" x2="30" y2="30" gradientUnits="userSpaceOnUse">
<stop stop-color="#F22E2E"/>
<stop offset="1" stop-color="#DD2A2A"/>
</linearGradient>
</defs>
</svg>
    <div class="Pagetitlewn">{pageTitle}</div>
  </div>
  
  <!-- Page content -->
  <div class="weazel-page">
    <div class="weazel-page__wrapper">
      
      {#if currentPage === 'lent'}
        <!-- Лента объявлений -->
        <div class="Lent">
          {#if advertisements.length === 0}
            <div class="no-adv">
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 76 89" fill="none">
  <path d="M38 23.2337C40.2444 23.2337 42.0212 21.4537 42.0212 19.2053V4.02842C42.0212 1.78 40.2444 0 38 0C35.7556 0 33.9788 1.78 33.9788 4.02842V19.1116C33.9788 21.36 35.7556 23.2337 38 23.2337Z"/>
  <path d="M16.4913 24.1705C17.1459 25.7632 18.6421 26.7 20.2319 26.7C20.793 26.7 21.2606 26.6063 21.8217 26.4189C23.8791 25.5758 24.9077 23.14 23.9726 21.0789L18.1746 7.12C17.3329 5.05895 14.995 4.02842 12.8441 4.96526C10.7868 5.80842 9.7581 8.24421 10.6933 10.3053L16.4913 24.1705Z"/>
  <path d="M54.1783 26.4189C54.6459 26.6063 55.207 26.7 55.7681 26.7C57.3579 26.7 58.8541 25.7632 59.5087 24.1705L65.3067 10.2116C66.1484 8.15053 65.2132 5.80842 63.1559 4.87158C61.005 4.12211 58.5736 5.15263 57.7319 7.21368L51.9339 21.1726C51.0923 23.2337 52.1209 25.5758 54.1783 26.4189Z"/>
  <path d="M75.1259 59.7705L64.091 36.6305C63.4364 35.2253 62.0337 34.2884 60.4439 34.2884H15.5561C13.9663 34.2884 12.5636 35.2253 11.909 36.6305L0.874065 59.7705C0.593517 60.3326 0.5 60.8947 0.5 61.5505V84.9716C0.5 87.22 2.27681 89 4.5212 89H71.4788C73.7232 89 75.5 87.22 75.5 84.9716V61.5505C75.5 60.9884 75.4065 60.3326 75.1259 59.7705ZM17.9016 43.004C18.0685 42.6585 18.4184 42.4389 18.8021 42.4389H57.2878C57.6734 42.4389 58.0246 42.6606 58.1905 43.0087L64.1576 55.5298C64.4738 56.1934 63.99 56.96 63.2548 56.96H12.7544C12.017 56.96 11.5332 56.1891 11.8539 55.5251L17.9016 43.004Z"/>
</svg>
              <div class="namewns">Сейчас нет активных объявлений</div>
            </div>
          {:else}
            <!-- Список объявлений -->
            <div class="vue-recycle-scroller">
              {#each advertisements as ad}
                <div class="adv">
                  <div class="card">
                    <!-- Редактор если есть -->
                    {#if ad.editorName}
                      <div class="card-author">
                        <svg xmlns="http://www.w3.org/2000/svg" width="19" height="19" viewBox="0 0 19 19" fill="none">
	<rect width="19" height="19" rx="9.5" fill="#E8E8E8"/>
	<path d="M10.5602 6.50293L5.60822 11.4577C5.58362 11.4827 5.56603 11.5138 5.55722 11.5478L5.00822 13.749C4.99943 13.7783 4.99766 13.8093 5.00305 13.8394C5.00843 13.8695 5.02083 13.8979 5.03922 13.9224C5.05762 13.9468 5.08151 13.9666 5.10895 13.9801C5.1364 13.9936 5.16664 14.0005 5.19722 14.0001C5.21307 14.0001 5.22886 13.998 5.24422 13.9941L7.44422 13.4448C7.47817 13.436 7.50921 13.4184 7.53422 13.3938L12.4902 8.43401L10.5602 6.50293Z" fill="#27292E" fill-opacity="0.5"/>
	<path d="M13.7163 5.82718L13.1643 5.27587C12.9784 5.09877 12.7315 5 12.4748 5C12.2181 5 11.9713 5.09877 11.7853 5.27587L11.1113 5.95125L13.0413 7.88233L13.7163 7.20695C13.8069 7.11636 13.8787 7.00881 13.9277 6.89043C13.9767 6.77206 14.002 6.64519 14.002 6.51706C14.002 6.38894 13.9767 6.26206 13.9277 6.14369C13.8787 6.02532 13.8069 5.91777 13.7163 5.82718Z" fill="#27292E" fill-opacity="0.5"/>
</svg>
                        <p>Редактор</p>
                        <div class="editAd">{ad.editorName}</div>
                        <p>#{ad.editorId}</p>
                      </div>
                    {/if}
                    
                    <!-- Автор если есть -->
                    {#if ad.userName}
                      <div class="card-title">
                        <svg xmlns="http://www.w3.org/2000/svg" width="19" height="19" viewBox="0 0 19 19" fill="none">
	<path d="M19 9.5C19 4.25329 14.7467 0 9.5 0C4.25329 0 0 4.25329 0 9.5C0 14.7467 4.25329 19 9.5 19C14.7467 19 19 14.7467 19 9.5Z" fill="#E8E8E8"/>
	<path d="M7.61093 6.895C7.61093 6.52016 7.72209 6.15374 7.93036 5.84209C8.13863 5.53044 8.43465 5.28755 8.78098 5.14415C9.1273 5.00075 9.50837 4.96329 9.87599 5.03649C10.2436 5.10969 10.5813 5.29027 10.8463 5.55539C11.1112 5.82051 11.2916 6.15826 11.3646 6.52592C11.4376 6.89358 11.4 7.27463 11.2564 7.62088C11.1128 7.96713 10.8698 8.26302 10.558 8.47112C10.2462 8.67923 9.87977 8.7902 9.50493 8.79C9.00252 8.78974 8.52077 8.58997 8.16561 8.23461C7.81044 7.87926 7.61093 7.39741 7.61093 6.895ZM10.4489 9.737H8.55393C8.2052 9.71438 7.85566 9.76657 7.52875 9.89006C7.20183 10.0135 6.90509 10.2055 6.65841 10.453C6.41173 10.7006 6.22081 10.998 6.09845 11.3253C5.97609 11.6526 5.92511 12.0024 5.94893 12.351C5.92091 12.575 5.94458 12.8025 6.01813 13.016C6.09169 13.2294 6.21316 13.4232 6.37324 13.5824C6.53332 13.7416 6.72775 13.862 6.94161 13.9344C7.15546 14.0068 7.38306 14.0292 7.60693 14H11.3959C11.6198 14.0294 11.8475 14.0072 12.0615 13.9348C12.2754 13.8625 12.4699 13.7421 12.6301 13.5828C12.7902 13.4236 12.9117 13.2297 12.9851 13.0162C13.0586 12.8026 13.0822 12.5751 13.0539 12.351C13.0778 12.0024 13.0268 11.6526 12.9044 11.3253C12.782 10.998 12.5911 10.7006 12.3444 10.453C12.0978 10.2055 11.801 10.0135 11.4741 9.89006C11.1472 9.76657 10.7977 9.71438 10.4489 9.737Z" fill="#27292E" fill-opacity="0.5"/>
</svg> 
                        <p>Автор:</p>
                        <div class="editcardAd">{ad.userName}</div>
                        {#if ad.userId}
                          <p>#{ad.userId}</p>
                        {/if}
                      </div>
                    {/if}
                    
                    <!-- Текст объявления -->
                    <div class="card-desc">{ad.text}</div>
                    
                    <!-- Номер телефона если есть -->
                    {#if ad.phoneNumber}
                      <div class="card-number">
                        <p>Номер телефона</p>
                        <div class="numberDiv">{ad.phoneNumber}</div>
                      </div>
                    {/if}
                    
                    <!-- Футер карточки -->
                    <div class="card-footer">
                      <div class="card-footer_left" class:show={!ad.userName}>
                        {#if ad.phoneNumber}
                          <div class="icon" on:click={() => callPhone(ad.phoneNumber)}>
<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 18 18" fill="none">
<path d="M15.3655 12.8547L12.3167 10.0827C12.1726 9.95171 11.9832 9.88185 11.7886 9.88786C11.5939 9.89388 11.4092 9.9753 11.2735 10.1149L9.47871 11.9607C9.04671 11.8782 8.17821 11.6074 7.28421 10.7157C6.39021 9.82094 6.11946 8.95019 6.03921 8.52119L7.88346 6.72569C8.02327 6.59004 8.10481 6.40531 8.11083 6.2106C8.11685 6.01589 8.04688 5.82647 7.91571 5.68244L5.14446 2.63444C5.01324 2.48996 4.83087 2.40232 4.63608 2.39014C4.44128 2.37795 4.24941 2.44218 4.10121 2.56919L2.47371 3.96494C2.34404 4.09508 2.26665 4.26828 2.25621 4.45169C2.24496 4.63919 2.03046 9.08069 5.47446 12.5262C8.47896 15.5299 12.2425 15.7497 13.279 15.7497C13.4305 15.7497 13.5235 15.7452 13.5482 15.7437C13.7316 15.7334 13.9047 15.6557 14.0342 15.5254L15.4292 13.8972C15.5567 13.7495 15.6214 13.5577 15.6095 13.363C15.5976 13.1682 15.51 12.9858 15.3655 12.8547Z"/>
</svg>                          </div>
                          
                          <div class="icon" on:click={() => sendMessage(ad.phoneNumber)}>
                           <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14" fill="none">
	<path fill-rule="evenodd" clip-rule="evenodd" d="M6.27734 0.714834C6.21716 0.719554 5.99112 0.747875 5.77558 0.777545C4.67892 0.904512 3.63097 1.28743 2.72268 1.89306C1.8144 2.49869 1.07321 3.30875 0.563437 4.25293C0.10037 5.09975 -0.0838656 6.06244 0.0352834 7.01271C0.154432 7.96298 0.571307 8.85569 1.23035 9.57188C1.52482 9.88728 1.85324 10.1716 2.21008 10.4202C2.46481 10.5928 2.45501 10.555 2.35564 10.9245C2.28026 11.17 2.16804 11.4035 2.02253 11.6177C1.8581 11.8098 1.68363 11.9937 1.49978 12.1686L1.0596 12.6002L1.83918 12.5887C2.19124 12.6118 2.54491 12.585 2.88889 12.5092C3.49199 12.3653 4.04848 12.0791 4.50824 11.6764L4.67829 11.5334L5.09817 11.6144C5.73715 11.7542 6.39235 11.8129 7.04713 11.789C7.45345 11.8011 7.86014 11.7851 8.26409 11.7411C9.64489 11.6017 10.9551 11.083 12.0388 10.2469C12.9682 9.51084 13.6143 8.49557 13.8737 7.36367C14.133 6.23177 13.9906 5.04875 13.4692 4.00411C12.8855 2.87467 11.9217 1.96925 10.7351 1.43567C9.88493 1.04885 8.96993 0.811367 8.03315 0.734389C7.4487 0.696684 6.86254 0.690156 6.27734 0.714834Z"/>
</svg>
                          </div>
                        {/if}
                        
                        {#if ad.isLocation}
                          <div class="icon" on:click={() => showLocation(ad.id)}>
                             <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="none">
  <path d="M7.9963 1.57386e-06C7.31354 0.000526523 6.63757 0.132515 6.00699 0.388431C5.37641 0.644347 4.80357 1.01918 4.32116 1.49153C3.83876 1.96387 3.45625 2.52448 3.19546 3.14135C2.93468 3.75821 2.80073 4.41926 2.80127 5.08673C2.80127 8.03479 6.59569 11.801 7.70178 12.836C7.78201 12.9118 7.88923 12.9541 8.0008 12.9541C8.11237 12.9541 8.21958 12.9118 8.29982 12.836C9.40591 11.801 13.2003 8.03719 13.2003 5.08673C13.2008 4.41854 13.0664 3.75683 12.8051 3.13944C12.5437 2.52205 12.1604 1.96109 11.677 1.48865C11.1937 1.01621 10.6198 0.641555 9.98824 0.386119C9.35667 0.130683 8.67979 -0.000524191 7.9963 1.57386e-06ZM7.9963 6.7983C7.64962 6.79751 7.31096 6.69626 7.02313 6.50734C6.7353 6.31842 6.51122 6.05031 6.37922 5.73692C6.24722 5.42352 6.21323 5.0789 6.28154 4.74662C6.34985 4.41434 6.51739 4.10932 6.76299 3.87011C7.00859 3.63091 7.32122 3.46825 7.66136 3.40271C8.0015 3.33718 8.35389 3.37169 8.67396 3.5019C8.99404 3.63211 9.26745 3.85217 9.45962 4.13426C9.65179 4.41635 9.7541 4.74781 9.75361 5.08673C9.75329 5.31195 9.70757 5.53489 9.61906 5.74283C9.53055 5.95076 9.40099 6.1396 9.23779 6.29855C9.07459 6.45751 8.88094 6.58346 8.66792 6.66921C8.4549 6.75496 8.22667 6.79883 7.9963 6.7983Z"/>
  <path d="M10.9301 11.3516C10.656 11.6715 10.386 11.9658 10.1259 12.2393C10.6625 12.4745 10.9955 12.7992 10.9955 13.1535C10.9955 13.8733 9.65462 14.4524 8.00039 14.4524C6.34616 14.4524 5.00527 13.8709 5.00527 13.1535C5.00527 12.7968 5.33743 12.4737 5.87493 12.2393C5.61477 11.965 5.34479 11.6683 5.07072 11.3516C3.47049 11.7939 2.40039 12.5841 2.40039 13.4886C2.40039 14.8755 4.9071 16 8.00039 16C11.0937 16 13.6004 14.8755 13.6004 13.4886C13.6004 12.5841 12.5303 11.7931 10.9301 11.3516Z"/>
</svg>
                          </div>
                        {/if}
                      </div>
                      
                      <div class="card-footer_right">
                        <p>{formatTime(ad.createAt)}</p>
                        {#if hasAdmin}
                          <div class="icon" on:click={() => deleteItem(ad.id)}>
 <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14" fill="none">
	<path fill-rule="evenodd" clip-rule="evenodd" d="M6.10791 0.00700829H7.8979C8.19666 -0.0146269 8.49683 0.0197916 8.78301 0.1085C9.00899 0.198883 9.20889 0.344442 9.36456 0.531964C9.53883 0.776086 9.66623 1.05064 9.74023 1.34156L9.76233 1.40922L9.94494 1.95984H13.4889C13.5548 1.95661 13.6207 1.96684 13.6825 1.98992C13.7444 2.01299 13.8009 2.04843 13.8487 2.09409C13.8965 2.13974 13.9346 2.19465 13.9606 2.2555C13.9866 2.31634 14 2.38185 14 2.44805C14 2.51425 13.9866 2.57976 13.9606 2.64061C13.9346 2.70145 13.8965 2.75637 13.8487 2.80202C13.8009 2.84767 13.7444 2.88311 13.6825 2.90619C13.6207 2.92926 13.5548 2.9395 13.4889 2.93626H12.6491L12.263 9.6207V9.6417C12.2234 10.3148 12.192 10.8479 12.1327 11.2749C12.0904 11.6763 11.972 12.066 11.7838 12.4228C11.4766 12.9703 11.0105 13.411 10.4474 13.6862C10.0809 13.8526 9.68617 13.9472 9.2843 13.965C8.8528 14 8.3201 14 7.64784 14H6.35565C5.68338 14 5.15069 14 4.71919 13.965C4.31732 13.9472 3.92254 13.8526 3.5561 13.6862C2.99303 13.411 2.52689 12.9703 2.21971 12.4228C2.03148 12.066 1.91304 11.6763 1.87078 11.2749C1.81147 10.8444 1.78006 10.3113 1.74052 9.6417V9.6207L1.35088 2.93393H0.511134C0.445212 2.93716 0.379321 2.92693 0.317459 2.90386C0.255597 2.88078 0.199053 2.84534 0.151257 2.79969C0.103461 2.75404 0.0654082 2.69912 0.0394075 2.63827C0.0134069 2.57743 0 2.51192 0 2.44572C0 2.37952 0.0134069 2.31401 0.0394075 2.25317C0.0654082 2.19232 0.103461 2.13741 0.151257 2.09175C0.199053 2.0461 0.255597 2.01066 0.317459 1.98759C0.379321 1.96451 0.445212 1.95428 0.511134 1.95751H4.05506L4.23883 1.40689L4.26093 1.33923C4.33494 1.0483 4.46233 0.773753 4.63661 0.529631C4.79227 0.342109 4.99217 0.19655 5.21815 0.106167C5.50596 0.017666 5.80775 -0.0159671 6.10791 0.00700829ZM8.84349 1.71603L8.92375 1.95868H5.08556L5.16581 1.71603C5.21097 1.51929 5.28547 1.33052 5.3868 1.15608C5.43913 1.09337 5.50632 1.04484 5.5822 1.01492C5.77903 0.973512 5.98096 0.962107 6.18119 0.981093H7.82812C8.02834 0.962107 8.23028 0.973512 8.42711 1.01492C8.50299 1.04484 8.57017 1.09337 8.62251 1.15608C8.72385 1.33051 8.79835 1.51929 8.84349 1.71603ZM5.70316 5.21574C5.83201 5.21543 5.95573 5.26631 6.04728 5.35725C6.13882 5.44819 6.19073 5.5718 6.19165 5.70103V10.2588C6.18554 10.3842 6.13159 10.5024 6.04098 10.5889C5.95036 10.6755 5.83003 10.7237 5.7049 10.7237C5.57977 10.7237 5.45944 10.6755 5.36882 10.5889C5.27821 10.5024 5.22426 10.3842 5.21815 10.2588V5.70103C5.21846 5.5718 5.26978 5.44795 5.36089 5.35657C5.45199 5.26519 5.57547 5.21371 5.70432 5.2134L5.70316 5.21574ZM8.30033 5.21574C8.42958 5.21574 8.55354 5.26723 8.64493 5.3589C8.73632 5.45057 8.78767 5.57489 8.78767 5.70453V10.2588C8.78155 10.3842 8.7276 10.5024 8.63699 10.5889C8.54638 10.6755 8.42604 10.7237 8.30091 10.7237C8.17578 10.7237 8.05545 10.6755 7.96484 10.5889C7.87422 10.5024 7.82027 10.3842 7.81416 10.2588V5.70103C7.81447 5.5716 7.86595 5.44757 7.95731 5.35616C8.04867 5.26474 8.17245 5.2134 8.30149 5.2134L8.30033 5.21574Z"/>
</svg>                          </div>
                        {/if}
                      </div>
                    </div>
                  </div>
                </div>
              {/each}
            </div>
          {/if}
          
          <!-- Map background -->
          <img 
            src="{cdn}/icons/iPhone/apps/weazelNewsV2/gtaVMap.webp" 
            alt=""
          />
        </div>
        
      {:else if currentPage === 'advertisement'}
        <!-- История объявлений -->
        <div class="advertisement">
          {#if historyAdvertisements.length === 0}
            <div class="no-adv">
              <svg viewBox="0 0 24 24">
                <path d="M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,17A1.5,1.5 0 0,1 10.5,15.5A1.5,1.5 0 0,1 12,14A1.5,1.5 0 0,1 13.5,15.5A1.5,1.5 0 0,1 12,17M12,10A1,1 0 0,1 13,11V13A1,1 0 0,1 12,14A1,1 0 0,1 11,13V11A1,1 0 0,1 12,10Z"/>
              </svg>
              <div>Нет записей в истории</div>
            </div>
          {:else}
            <!-- Список истории -->
            <div class="vue-recycle-scroller">
              {#each historyAdvertisements as item}
                <div class="adv">
                  <div class="itemwn">
                    <!-- Заголовок элемента -->
                    <div class="itemwn-header">
                      <div class="itemwn-header_icon">
                        <!-- Иконки по типу -->
                        {#if item.type}
                          <img src="{cdn}/icons/iPhone/apps/weazelNewsV2/{item.type.toLowerCase()}.svg" alt="" />
                        {/if}
                        
                        <!-- Иконка телефона -->
                        {#if item.phoneNumber}
                          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 18 18" fill="none">
<path d="M15.3655 12.8547L12.3167 10.0827C12.1726 9.95171 11.9832 9.88185 11.7886 9.88786C11.5939 9.89388 11.4092 9.9753 11.2735 10.1149L9.47871 11.9607C9.04671 11.8782 8.17821 11.6074 7.28421 10.7157C6.39021 9.82094 6.11946 8.95019 6.03921 8.52119L7.88346 6.72569C8.02327 6.59004 8.10481 6.40531 8.11083 6.2106C8.11685 6.01589 8.04688 5.82647 7.91571 5.68244L5.14446 2.63444C5.01324 2.48996 4.83087 2.40232 4.63608 2.39014C4.44128 2.37795 4.24941 2.44218 4.10121 2.56919L2.47371 3.96494C2.34404 4.09508 2.26665 4.26828 2.25621 4.45169C2.24496 4.63919 2.03046 9.08069 5.47446 12.5262C8.47896 15.5299 12.2425 15.7497 13.279 15.7497C13.4305 15.7497 13.5235 15.7452 13.5482 15.7437C13.7316 15.7334 13.9047 15.6557 14.0342 15.5254L15.4292 13.8972C15.5567 13.7495 15.6214 13.5577 15.6095 13.363C15.5976 13.1682 15.51 12.9858 15.3655 12.8547Z"/>
</svg>
                        {/if}
                        
                        <!-- Иконка локации -->
                        {#if item.locationType && item.locationType !== 'NONE'}
                          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 16 16" fill="none">
<path d="M7.9963 1.57386e-06C7.31354 0.000526523 6.63757 0.132515 6.00699 0.388431C5.37641 0.644347 4.80357 1.01918 4.32116 1.49153C3.83876 1.96387 3.45625 2.52448 3.19546 3.14135C2.93468 3.75821 2.80073 4.41926 2.80127 5.08673C2.80127 8.03479 6.59569 11.801 7.70178 12.836C7.78201 12.9118 7.88923 12.9541 8.0008 12.9541C8.11237 12.9541 8.21958 12.9118 8.29982 12.836C9.40591 11.801 13.2003 8.03719 13.2003 5.08673C13.2008 4.41854 13.0664 3.75683 12.8051 3.13944C12.5437 2.52205 12.1604 1.96109 11.677 1.48865C11.1937 1.01621 10.6198 0.641555 9.98824 0.386119C9.35667 0.130683 8.67979 -0.000524191 7.9963 1.57386e-06ZM7.9963 6.7983C7.64962 6.79751 7.31096 6.69626 7.02313 6.50734C6.7353 6.31842 6.51122 6.05031 6.37922 5.73692C6.24722 5.42352 6.21323 5.0789 6.28154 4.74662C6.34985 4.41434 6.51739 4.10932 6.76299 3.87011C7.00859 3.63091 7.32122 3.46825 7.66136 3.40271C8.0015 3.33718 8.35389 3.37169 8.67396 3.5019C8.99404 3.63211 9.26745 3.85217 9.45962 4.13426C9.65179 4.41635 9.7541 4.74781 9.75361 5.08673C9.75329 5.31195 9.70757 5.53489 9.61906 5.74283C9.53055 5.95076 9.40099 6.1396 9.23779 6.29855C9.07459 6.45751 8.88094 6.58346 8.66792 6.66921C8.4549 6.75496 8.22667 6.79883 7.9963 6.7983Z"/>
<path d="M10.9301 11.3516C10.656 11.6715 10.386 11.9658 10.1259 12.2393C10.6625 12.4745 10.9955 12.7992 10.9955 13.1535C10.9955 13.8733 9.65462 14.4524 8.00039 14.4524C6.34616 14.4524 5.00527 13.8709 5.00527 13.1535C5.00527 12.7968 5.33743 12.4737 5.87493 12.2393C5.61477 11.965 5.34479 11.6683 5.07072 11.3516C3.47049 11.7939 2.40039 12.5841 2.40039 13.4886C2.40039 14.8755 4.9071 16 8.00039 16C11.0937 16 13.6004 14.8755 13.6004 13.4886C13.6004 12.5841 12.5303 11.7931 10.9301 11.3516Z"/>
</svg>
                        {/if}
                        
                        <!-- Метка срочности -->
                        {#if item.isUrgent}
                          <div class="tag">
                            <p>FAST</p>
                          </div>
                        {/if}
                      </div>
                      
                      <div class="itemwn-header_date">
                        {formatDateTime(item.createAt)}
                      </div>
                    </div>
                    
                    <!-- Основной текст -->
                    <div class="itemwn-main">
                      {item.text}
                    </div>
                    
                    <!-- Футер элемента -->
                    <div class="itemwn-footer">
                      {#if item.status}
                        <div class="itemwn-footer_status itemwn-footer_status-{item.status.toLowerCase()}">
                           <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 19 20" fill="none">
  <rect y="0.5" rx="9.5"/>
  <path d="M10.5602 7.00293L5.60822 11.9577C5.58362 11.9827 5.56603 12.0138 5.55722 12.0478L5.00822 14.249C4.99943 14.2783 4.99766 14.3093 5.00305 14.3394C5.00843 14.3695 5.02083 14.3979 5.03922 14.4224C5.05762 14.4468 5.08151 14.4666 5.10895 14.4801C5.1364 14.4936 5.16664 14.5005 5.19722 14.5001C5.21307 14.5001 5.22886 14.498 5.24422 14.4941L7.44422 13.9448C7.47817 13.936 7.50921 13.9184 7.53422 13.8938L12.4902 8.93401L10.5602 7.00293Z"/>
  <path d="M13.7163 6.32718L13.1643 5.77587C12.9784 5.59877 12.7315 5.5 12.4748 5.5C12.2181 5.5 11.9713 5.59877 11.7853 5.77587L11.1113 6.45125L13.0413 8.38233L13.7163 7.70695C13.8069 7.61636 13.8787 7.50881 13.9277 7.39043C13.9767 7.27206 14.002 7.14519 14.002 7.01706C14.002 6.88894 13.9767 6.76206 13.9277 6.64369C13.8787 6.52532 13.8069 6.41777 13.7163 6.32718Z"/>
</svg>
                          <div>
                            {item.status === 'DONE' ? 'Выполнено' : item.status === 'CANCEL' ? 'Отменено' : item.status}
                            {item.status === 'CANCEL' && item.reasonCancel ? ' ' + item.reasonCancel : ''}
                          </div>
                        </div>
                      {/if}
                      
                      <div class="itemwn-footer_icon" on:click={() => repeat(item)}>
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 13 11" fill="none">
  <path d="M9.84232 9.7588C9.96526 9.66009 9.96526 9.47702 9.86958 9.36397L9.47297 8.92667C9.37729 8.82797 9.22708 8.81363 9.1175 8.89855C8.39322 9.51891 7.4771 9.8575 6.50637 9.8575C4.26456 9.8575 2.40549 8.05267 2.28261 5.78185L3.32173 5.78185C3.52698 5.78185 3.64992 5.55631 3.54035 5.37269L2.02337 2.80627C1.91379 2.63698 1.68181 2.63698 1.57223 2.80627L0.0408828 5.38695C-0.0686934 5.57003 0.0547798 5.79611 0.259508 5.79611L1.17514 5.79611L1.17514 5.82424C1.33924 8.72918 3.67667 11 6.50647 11C7.73694 11 8.91289 10.5627 9.84243 9.75871L9.84232 9.7588Z"/>
  <path d="M10.7307 5.20389L9.69155 5.20389C9.48629 5.20389 9.36336 5.42943 9.47292 5.61305L11.0039 8.19373C11.1134 8.36302 11.3454 8.36302 11.455 8.19373L12.9591 5.61305C13.0687 5.42997 12.9452 5.20389 12.7405 5.20389L11.8382 5.20389L11.8382 5.17576C11.6741 2.27082 9.33669 -1.60128e-07 6.50689 -2.83823e-07C5.26306 -3.38192e-07 4.07378 0.437294 3.14424 1.25506C3.0213 1.35377 3.0213 1.53685 3.11698 1.64989L3.51359 2.08719C3.60927 2.1859 3.75948 2.20023 3.86906 2.11531C4.60723 1.46682 5.53683 1.12824 6.50742 1.12824C8.73472 1.12824 10.5666 2.90498 10.7307 5.20389Z"/>
</svg>
                      </div>
                    </div>
                  </div>
                </div>
              {/each}
            </div>
          {/if}
          
          <!-- Map background -->
          <img class="lent"
            src="{cdn}/icons/iPhone/apps/weazelNewsV2/gtaVMap.webp" 
            alt=""
          />
        </div>
        
      {:else if currentPage === 'createAdv'}
        <!-- Создать объявление -->
        <div class="createAdv">
          
          <!-- Modal окна -->
          {#if modalType}
            <div class="modal-wrapper" on:click|self={closeModal}>
              <div class="modal">
                {#if modalType === 'type'}
                  <div class="modal-type">
                    <p>Выберите тип объявления</p>
                    <div class="modal-type_btns">
                      <button on:click={() => selectType('NOTIFICATION')}>Уведомление</button>
                      <button on:click={() => selectType('CHAT')}>Чат</button>
                    </div>
                  </div>
                {:else if modalType === 'location'}
                  <div class="modal-location">
                    <p>Выберите местоположение</p>
                    <div class="modal-location_btns">
                      <button on:click={() => selectLocation('UPDATE')}>Текущее</button>
                      <button on:click={() => selectLocation('STATIC')}>Выбрать на карте</button>
                    </div>
                  </div>
                {/if}
              </div>
            </div>
          {/if}
          
          <!-- Основная форма -->
          <div class="createAdv-list">
            
            <!-- Тип объявления -->
            <div class="createAdv-list_type">
              <div class="divcreatetype">Тип</div>
              <div class="openModalType" on:click={() => openModal('type')}>
                <p>{createAdvState.type === 'CHAT' ? 'Чат' : 'Уведомление'}</p>
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 15" fill="none">
  <path fill-rule="evenodd" clip-rule="evenodd" d="M2.18441 0.26099C1.91335 0.578613 1.94476 1.06138 2.25457 1.33928L9.12269 7.5L2.25457 13.6607C1.94476 13.9386 1.91335 14.4214 2.18441 14.739C2.45547 15.0566 2.92636 15.0888 3.23617 14.8109L10.7454 8.07511C10.9072 7.93 11 7.72037 11 7.5C11 7.27963 10.9072 7.07 10.7454 6.92489L3.23617 0.189063C2.92636 -0.0888369 2.45547 -0.0566342 2.18441 0.26099Z"/>
</svg>
              </div>
            </div>
            
            <!-- Номер телефона -->
            <div class="createAdv-list_phone">
              <div class="divphonelist">Номер телефона</div>
              <div class="input-switchWn" on:click={togglePhone}>
                <div class="switchWn" class:active={createAdvState.isPhone}>
                  <div class="point"></div>
                </div>
              </div>
            </div>
            
            <!-- Местоположение -->
            <div class="createAdv-list_location" class:disabled={!createAdvState.hasUpdateLocation}>
              <div class="divlocat">Местоположение</div>
              <div class="input-switchWn" on:click={toggleLocation}>
                <div class="switchWn" class:active={createAdvState.location !== 'NONE'}>
                  <div class="point"></div>
                </div>
              </div>
            </div>
            
            <!-- Срочность -->
            <div class="createAdv-list_supply">
              <div class="text">
                <div class="divtext">Приоритет за деньги</div>
                <p>+${pricePerSymbol * textLength}</p>
              </div>
              <div class="input-switchWn" on:click={toggleUrgent}>
                <div class="switchWn" class:active={createAdvState.isUrgent}>
                  <div class="point"></div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Текстовое поле -->
          <div class="createAdv-text">
            <div class="createAdv-text_title">
              {#if createAdvState.type}
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 70 70" fill="none">
<path d="M13.3321 45.6584V37.1323H22.0004V45.6584H13.3321ZM56.665 13.6611C56.6534 11.9647 55.9631 10.3419 54.7452 9.14802C53.5273 7.95414 51.881 7.28643 50.1667 7.29105H6.50125C5.64993 7.28839 4.80642 7.45174 4.01891 7.77177C3.2314 8.0918 2.51533 8.56223 1.91159 9.15619C1.30785 9.75015 0.82829 10.456 0.500293 11.2334C0.172296 12.0108 0.00229457 12.8446 0 13.687L0 52.0486C0.012342 54.8798 1.15753 57.5909 3.18495 59.5885C5.21236 61.5862 7.95689 62.7077 10.8179 62.7077H59.1821C62.0431 62.7077 64.7876 61.5862 66.8151 59.5885C68.8425 57.5909 69.9877 54.8798 70 52.0486V26.4733C69.9977 25.6308 69.8277 24.7971 69.4997 24.0197C69.1717 23.2422 68.6921 22.5364 68.0884 21.9424C67.4847 21.3485 66.7686 20.878 65.9811 20.558C65.1936 20.238 64.3501 20.0746 63.4988 20.0773H60.9992V50.8999C60.9992 51.4687 60.7709 52.0141 60.3644 52.4163C59.958 52.8185 59.4068 53.0444 58.8321 53.0444C58.2573 53.0444 57.7061 52.8185 57.2997 52.4163C56.8933 52.0141 56.665 51.4687 56.665 50.8999V13.6611ZM9.00083 22.2102C9.0016 21.9294 9.05826 21.6514 9.16758 21.3922C9.2769 21.133 9.43674 20.8977 9.63797 20.6996C9.8392 20.5016 10.0779 20.3447 10.3404 20.2379C10.6029 20.1311 10.8841 20.0765 11.1679 20.0773H45.8179C46.3755 20.0976 46.9033 20.331 47.2905 20.7286C47.6777 21.1261 47.8941 21.6567 47.8941 22.2088C47.8941 22.7609 47.6777 23.2915 47.2905 23.689C46.9033 24.0866 46.3755 24.32 45.8179 24.3403H11.165C10.5923 24.3411 10.0427 24.1169 9.63694 23.717C9.23116 23.317 9.00238 22.777 9.00083 22.2102ZM32.8329 32.8693H45.8179C46.3755 32.8896 46.9033 33.1231 47.2905 33.5206C47.6777 33.9181 47.8941 34.4487 47.8941 35.0008C47.8941 35.5529 47.6777 36.0835 47.2905 36.481C46.9033 36.8786 46.3755 37.112 45.8179 37.1323H32.8329C32.5434 37.1429 32.2547 37.0955 31.984 36.9932C31.7134 36.8908 31.4664 36.7355 31.2578 36.5365C31.0492 36.3376 30.8833 36.099 30.77 35.8351C30.6566 35.5713 30.5982 35.2875 30.5982 35.0008C30.5982 34.7141 30.6566 34.4303 30.77 34.1665C30.8833 33.9026 31.0492 33.6641 31.2578 33.4651C31.4664 33.2661 31.7134 33.1108 31.984 33.0084C32.2547 32.9061 32.5434 32.8587 32.8329 32.8693ZM30.6658 47.7885C30.6666 47.5076 30.7233 47.2297 30.8326 46.9705C30.9419 46.7113 31.1017 46.4759 31.303 46.2779C31.5042 46.0798 31.7429 45.9229 32.0054 45.8161C32.2679 45.7093 32.5491 45.6548 32.8329 45.6555H45.8179C46.3755 45.6758 46.9033 45.9093 47.2905 46.3068C47.6777 46.7043 47.8941 47.2349 47.8941 47.787C47.8941 48.3392 47.6777 48.8698 47.2905 49.2673C46.9033 49.6648 46.3755 49.8983 45.8179 49.9186H32.8329C32.2607 49.9201 31.7113 49.697 31.305 49.2983C30.8988 48.8995 30.6689 48.3547 30.6658 47.7885ZM11.165 32.8664H24.1821C24.4659 32.8656 24.7471 32.9202 25.0096 33.027C25.2721 33.1338 25.5108 33.2907 25.712 33.4887C25.9133 33.6868 26.0731 33.9221 26.1824 34.1813C26.2917 34.4405 26.3484 34.7185 26.3492 34.9994V47.7885C26.3484 48.0693 26.2917 48.3473 26.1824 48.6065C26.0731 48.8657 25.9133 49.101 25.712 49.2991C25.5108 49.4972 25.2721 49.6541 25.0096 49.7609C24.7471 49.8676 24.4659 49.9222 24.1821 49.9214H11.165C10.8812 49.9222 10.6 49.8676 10.3375 49.7609C10.075 49.6541 9.83629 49.4972 9.63505 49.2991C9.43382 49.101 9.27398 48.8657 9.16466 48.6065C9.05534 48.3473 8.99868 48.0693 8.99792 47.7885V34.9994C8.99868 34.7185 9.05534 34.4405 9.16466 34.1813C9.27398 33.9221 9.43382 33.6868 9.63505 33.4887C9.83629 33.2907 10.075 33.1338 10.3375 33.027C10.6 32.9202 10.8812 32.8656 11.165 32.8664Z"/>
</svg>              {/if}
              <div class="divCadvt">{queueCount} в очереди объявлений</div>
            </div>
            
            <div class="createAdv-text_input">
              <textarea
                bind:value={createAdvState.text}
                placeholder="Введите текст объявления"
                maxlength="{MAX_TEXT_LENGTH}"
                on:keydown={handleTextInput}
                on:paste={handlePaste}
              ></textarea>
              <div class="quantity">
                <div class="qinptdiv">{textLength}</div>
                <p>/ {MAX_TEXT_LENGTH}</p>
              </div>
            </div>
          </div>
          
          <!-- Цена -->
          <div class="createAdv-price">
            <div class="createAdv-price_amount">
              <div class="priceDiv">Сумма к оплате</div>
              <p>${pricePerSymbol * textLength}</p>
            </div>
            <div class="createAdv-price_symbol">
              <div class="SymbolDiv">Цена за символ</div>
              <p>${pricePerSymbol}</p>
            </div>
          </div>
          
          <!-- Кнопка отправки -->
          <div class="createAdv-btn">
            <button 
              class:disabled={isTextEmpty}
              disabled={isTextEmpty}
              on:click={submitAdvertisement}
            >
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 12" fill="none">
  <g clip-path="url(#clip0_5453_2568)">
    <path d="M14 -0.00292969L2 -0.00292969C1.17275 -0.00292969 0.5 0.66982 0.5 1.49707L0.5 2.99707L15.5 2.99707V1.49707C15.5 0.66982 14.8273 -0.00292969 14 -0.00292969ZM0.5 10.4971C0.5 11.3243 1.17275 11.9971 2 11.9971L14 11.9971C14.8273 11.9971 15.5 11.3243 15.5 10.4971L15.5 5.99707L0.5 5.99707L0.5 10.4971ZM2.75 8.24707H7.25V9.74707H2.75L2.75 8.24707Z"/>
  </g>
  <defs>
    <clipPath id="clip0_5453_2568">
      <rect width="16" height="12" transform="translate(0 -0.00292969)"/>
    </clipPath>
  </defs>
</svg>
              <p>Оплатить и отправить</p>
            </button>
          </div>
          
        </div>
      {/if}
      
    </div>
  </div>
  
  <!-- Navigation pages -->
  <div class="weazel-pages">
    {#each pages as page}
      <div class="pagelick" on:click={() => setPage(page.name)}>
        {#if page.name === 'lent'}
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" class:active={currentPage === page.name}>
            <path fill-rule="evenodd" clip-rule="evenodd" d="M11.084 0.0072151C8.31841 0.227227 5.71284 1.39247 3.70512 3.30713C1.6974 5.22179 0.40991 7.76918 0.0589958 10.5212C-0.0196653 11.506 -0.0196653 12.4955 0.0589958 13.4802C0.341421 15.6816 1.22201 17.7634 2.605 19.4992C3.16056 20.1663 3.77431 20.7828 4.439 21.3412C6.18765 22.7517 8.29212 23.6515 10.52 23.9412C11.5051 24.0198 12.4949 24.0198 13.48 23.9412C16.1361 23.6018 18.6044 22.3902 20.4977 20.4966C22.391 18.6029 23.6021 16.1344 23.941 13.4782C24.0196 12.4935 24.0196 11.504 23.941 10.5192C23.608 7.89947 22.4252 5.46118 20.5739 3.57801C18.7225 1.69485 16.3047 0.470714 13.691 0.0932151C12.8248 0.00834329 11.9539 -0.0203834 11.084 0.0072151ZM7.217 3.51222C7.234 4.58322 7.267 4.70322 7.586 4.86822C7.678 4.91522 8.606 5.13521 9.65 5.35721C10.694 5.57922 11.625 5.79922 11.723 5.84722C11.8916 5.94271 12.0387 6.0719 12.1552 6.22674C12.2717 6.38158 12.355 6.55876 12.4 6.74722C12.784 8.02221 12.836 8.49322 12.626 8.89122C12.568 9.00022 11.826 9.96222 10.979 11.0262C10.132 12.0902 9.41 13.0052 9.379 13.0562C9.348 13.1072 9.126 14.0382 8.879 15.1152C8.632 16.1922 8.392 17.1622 8.352 17.2632C8.28213 17.3993 8.18375 17.5188 8.06356 17.6135C7.94337 17.7082 7.80419 17.7759 7.65549 17.8119C7.50679 17.848 7.35206 17.8516 7.20185 17.8225C7.05163 17.7934 6.90946 17.7322 6.785 17.6432C6.567 17.4642 6.508 17.3572 5.419 14.9432L4.472 12.8312L4.567 12.5472C4.619 12.3912 4.93 11.4772 5.258 10.5162C5.9 8.63622 5.9 8.61922 5.7 8.29322C5.623 8.16522 5.389 8.03621 4.213 7.46122C3.449 7.08622 2.813 6.77322 2.8 6.76122C2.95916 6.43038 3.15183 6.11673 3.375 5.82521C4.12275 4.78909 5.04843 3.89388 6.109 3.18122C6.45148 2.95406 6.8071 2.74736 7.174 2.56222C7.20928 2.87759 7.22364 3.19495 7.217 3.51222ZM21.6 7.50322C22.0466 8.45115 22.3499 9.46016 22.5 10.4972C22.5706 11.0184 22.5974 11.5446 22.58 12.0702C22.6009 12.6445 22.5583 13.2193 22.453 13.7842C22.0539 16.0711 20.9224 18.1663 19.229 19.7542L18.838 20.1132L18.438 19.8542C18.22 19.7122 18.002 19.5702 17.974 19.5412C17.7485 18.1404 17.5953 16.7288 17.515 15.3122C17.562 14.9452 17.715 14.6962 18.788 13.1732C19.345 12.3852 19.842 11.6622 19.888 11.5672C19.972 11.4062 19.979 11.2842 19.979 9.96722V8.53522L20.643 7.75522C21.008 7.32622 21.316 6.97521 21.327 6.97521C21.4283 7.1457 21.5194 7.32201 21.6 7.50322ZM13.274 20.9742C13.3697 21.0078 13.459 21.0575 13.538 21.1212C13.8663 21.4937 14.1655 21.8909 14.433 22.3092C13.8929 22.4447 13.3417 22.5307 12.786 22.5662C12.2835 22.6243 11.7757 22.6209 11.274 22.5562C11.3851 22.2075 11.5461 21.8768 11.752 21.5742C11.9158 21.4289 12.0993 21.3074 12.297 21.2132C12.915 20.9082 13.006 20.8852 13.274 20.9742Z"/>
          </svg>
        {:else if page.name === 'advertisement'}
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 70 70" fill="none" class:active={currentPage === page.name}>
            <path d="M13.3321 45.6584V37.1323H22.0004V45.6584H13.3321ZM56.665 13.6611C56.6534 11.9647 55.9631 10.3419 54.7452 9.14802C53.5273 7.95414 51.881 7.28643 50.1667 7.29105H6.50125C5.64993 7.28839 4.80642 7.45174 4.01891 7.77177C3.2314 8.0918 2.51533 8.56223 1.91159 9.15619C1.30785 9.75015 0.82829 10.456 0.500293 11.2334C0.172296 12.0108 0.00229457 12.8446 0 13.687L0 52.0486C0.012342 54.8798 1.15753 57.5909 3.18495 59.5885C5.21236 61.5862 7.95689 62.7077 10.8179 62.7077H59.1821C62.0431 62.7077 64.7876 61.5862 66.8151 59.5885C68.8425 57.5909 69.9877 54.8798 70 52.0486V26.4733C69.9977 25.6308 69.8277 24.7971 69.4997 24.0197C69.1717 23.2422 68.6921 22.5364 68.0884 21.9424C67.4847 21.3485 66.7686 20.878 65.9811 20.558C65.1936 20.238 64.3501 20.0746 63.4988 20.0773H60.9992V50.8999C60.9992 51.4687 60.7709 52.0141 60.3644 52.4163C59.958 52.8185 59.4068 53.0444 58.8321 53.0444C58.2573 53.0444 57.7061 52.8185 57.2997 52.4163C56.8933 52.0141 56.665 51.4687 56.665 50.8999V13.6611ZM9.00083 22.2102C9.0016 21.9294 9.05826 21.6514 9.16758 21.3922C9.2769 21.133 9.43674 20.8977 9.63797 20.6996C9.8392 20.5016 10.0779 20.3447 10.3404 20.2379C10.6029 20.1311 10.8841 20.0765 11.1679 20.0773H45.8179C46.3755 20.0976 46.9033 20.331 47.2905 20.7286C47.6777 21.1261 47.8941 21.6567 47.8941 22.2088C47.8941 22.7609 47.6777 23.2915 47.2905 23.689C46.9033 24.0866 46.3755 24.32 45.8179 24.3403H11.165C10.5923 24.3411 10.0427 24.1169 9.63694 23.717C9.23116 23.317 9.00238 22.777 9.00083 22.2102ZM32.8329 32.8693H45.8179C46.3755 32.8896 46.9033 33.1231 47.2905 33.5206C47.6777 33.9181 47.8941 34.4487 47.8941 35.0008C47.8941 35.5529 47.6777 36.0835 47.2905 36.481C46.9033 36.8786 46.3755 37.112 45.8179 37.1323H32.8329C32.5434 37.1429 32.2547 37.0955 31.984 36.9932C31.7134 36.8908 31.4664 36.7355 31.2578 36.5365C31.0492 36.3376 30.8833 36.099 30.77 35.8351C30.6566 35.5713 30.5982 35.2875 30.5982 35.0008C30.5982 34.7141 30.6566 34.4303 30.77 34.1665C30.8833 33.9026 31.0492 33.6641 31.2578 33.4651C31.4664 33.2661 31.7134 33.1108 31.984 33.0084C32.2547 32.9061 32.5434 32.8587 32.8329 32.8693ZM30.6658 47.7885C30.6666 47.5076 30.7233 47.2297 30.8326 46.9705C30.9419 46.7113 31.1017 46.4759 31.303 46.2779C31.5042 46.0798 31.7429 45.9229 32.0054 45.8161C32.2679 45.7093 32.5491 45.6548 32.8329 45.6555H45.8179C46.3755 45.6758 46.9033 45.9093 47.2905 46.3068C47.6777 46.7043 47.8941 47.2349 47.8941 47.787C47.8941 48.3392 47.6777 48.8698 47.2905 49.2673C46.9033 49.6648 46.3755 49.8983 45.8179 49.9186H32.8329C32.2607 49.9201 31.7113 49.697 31.305 49.2983C30.8988 48.8995 30.6689 48.3547 30.6658 47.7885ZM11.165 32.8664H24.1821C24.4659 32.8656 24.7471 32.9202 25.0096 33.027C25.2721 33.1338 25.5108 33.2907 25.712 33.4887C25.9133 33.6868 26.0731 33.9221 26.1824 34.1813C26.2917 34.4405 26.3484 34.7185 26.3492 34.9994V47.7885C26.3484 48.0693 26.2917 48.3473 26.1824 48.6065C26.0731 48.8657 25.9133 49.101 25.712 49.2991C25.5108 49.4972 25.2721 49.6541 25.0096 49.7609C24.7471 49.8676 24.4659 49.9222 24.1821 49.9214H11.165C10.8812 49.9222 10.6 49.8676 10.3375 49.7609C10.075 49.6541 9.83629 49.4972 9.63505 49.2991C9.43382 49.101 9.27398 48.8657 9.16466 48.6065C9.05534 48.3473 8.99868 48.0693 8.99792 47.7885V34.9994C8.99868 34.7185 9.05534 34.4405 9.16466 34.1813C9.27398 33.9221 9.43382 33.6868 9.63505 33.4887C9.83629 33.2907 10.075 33.1338 10.3375 33.027C10.6 32.9202 10.8812 32.8656 11.165 32.8664Z"/>
          </svg>
        {:else if page.name === 'createAdv'}
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" class:active={currentPage === page.name}>
            <path d="M21.1702 2.07272C21.5023 1.98511 21.8504 1.97342 22.1878 2.03856C22.5252 2.10371 22.8429 2.24394 23.1163 2.44844C23.3897 2.65295 23.6115 2.91628 23.7647 3.21813C23.918 3.51998 23.9985 3.85229 24 4.18946V19.2441C23.9981 19.5925 23.9119 19.9356 23.7485 20.2451C23.5852 20.5545 23.3492 20.8216 23.0602 21.0243C22.7711 21.2271 22.4371 21.3597 22.0856 21.4113C21.7341 21.4629 21.3751 21.432 21.0382 21.3212L13.9058 19.0045C13.4827 20.0311 12.7096 20.8821 11.7182 21.4125C10.7268 21.9428 9.57855 22.1197 8.46908 21.9129C7.3596 21.7061 6.3576 21.1285 5.63385 20.2785C4.91009 19.4284 4.50938 18.3587 4.5 17.2514V15.9551L1.53818 14.9924C1.09179 14.8488 0.702918 14.5709 0.426793 14.1982C0.150668 13.8255 0.00133344 13.377 0 12.9163L0 9.2508C0.000533804 8.76633 0.1647 8.29567 0.46686 7.91234C0.769019 7.52901 1.19216 7.25458 1.67018 7.13193L21.1702 2.07272ZM6 16.4374V17.2493C6.00904 18.0074 6.28283 18.7397 6.77573 19.3239C7.26862 19.9081 7.95085 20.3091 8.7087 20.46C9.46656 20.6108 10.2543 20.5025 10.9405 20.153C11.6268 19.8034 12.1701 19.2339 12.48 18.5392L6 16.4374Z"/>
          </svg>
        {/if}
      </div>
    {/each}
  </div>
  
  <!-- Close button -->
  <div class="weazel-close">
    <button 
      tabindex="-1" 
      class="weazel-close__button"
      on:mousedown={closingStart}
      on:click={closeApp}
    ></button>
  </div>
</div>