<script>
    import { fly } from 'svelte/transition';
    import './phone.css'
 import { onDestroy } from 'svelte';

    import { onMount } from 'svelte';

  import { fade, slide } from 'svelte/transition';
  import Phone from './call.svelte';  // Импорт компонента CarService
  import Messages from './messages.svelte'
  import CarService from './CarService.svelte';  // Импорт компонента CarService
    import Music from './Music.svelte';  // Импорт компонента Music
    import WeazelNews from './apps/WeazelNews.svelte';  // Импорт компонента Music
    import Calendar from './apps/Calendar.svelte';  // Импорт компонента Music
    import Operator from './apps/Operator.svelte';  // Импорт компонента Music
    import Party from './apps/Party.svelte';  // Импорт компонента Music
    import Gallery from './apps/gallery.svelte'
    import Taxi from './apps/taxi.svelte'
import Camera from './components/camera/index.svelte'
import Photos from './apps/photos.svelte'
import Settings from './settings.svelte'
  // Props
  export let show = true;
  export let cdn = 'https://cdn.majestic-files.com/public/master/static';
  
  // State management
  let phoneState = {
    disabled: false,  // Экран включен по умолчанию
    timer: null,
    enableToContact: true,
    y: 0,
    isUnblocking: false
  };
  
  let lockWindow = {
    removed: false,
    unlocked: false
  };
  
  let activeCall = null;
  let openedApp = null;
  let cameraOpened = false;
  let darkness = 0;
  let wallpapers = 0;
  let network = 4;
  let balance = 1000;
  let limitedApplications = false;
  let callDuration = 0;
  
  // Time and date
  let time = '19:05';
  let date = 'Понедельник, 15 сентября';
  let weekday = 'Понедельник';
  let shortWeekDay = 'Пн';
  let monthDay = '15';
  
  // Apps configuration
  let apps = [
    { app: 'carService', title: 'Auto Plus', icon: { type: 'image' }, dark: true, isDisabled: false },
    { app: 'music', title: 'Радио', icon: { type: 'image' }, dark: true, isDisabled: false },
    { app: 'weazelNews', title: 'Weazel News', icon: { type: 'image' }, dark: true, isDisabled: false },
    { app: 'taxi', title: 'UberDrive', icon: { type: 'image' }, dark: false, isDisabled: false },
    { app: 'camera', title: 'Камера', icon: { type: 'image' }, dark: false, isDisabled: false },
    { app: 'calendar', title: 'Календарь', icon: { type: 'component' }, dark: true, isDisabled: false },
    { app: 'party', title: 'Группа', icon: { type: 'image' }, dark: true, isDisabled: false },
    { app: 'photos', title: 'Фото', icon: { type: 'image' }, dark: true, isDisabled: false },
    { app: 'advocate', title: 'Адвокаты', icon: { type: 'image' }, dark: true, isDisabled: false },
    { app: 'operator', title: 'Оператор', icon: { type: 'image' }, dark: true, isDisabled: false }
  ];
  
  let fastApps = [
    { app: 'phone', title: 'Телефон', dark: true, icon: { type: 'image' } },
    { app: 'messages', title: 'Сообщения', dark: true, icon: { type: 'image' } },
    { app: 'contacts', title: 'Контакты', dark: true, icon: { type: 'image' } },
    { app: 'settings', title: 'Настройки', dark: true, icon: { type: 'image' } }
  ];
  
  let barColors = {
    appBar: false,
    mainBar: false
  };
  
  let contacts = {};
  let notifications = [];
  let unreadMessages = 0;
  let missedCalls = 0;
  
  // Camera state
  let cameraState = {
    zoomLevel: 1,
    selectedCameraType: 'photo',
    cameraTypeOffset: 0
  };
  
  const CAMERA_TYPES = ['photo', 'video'];
  
  // Computed values
  $: statusbarStyle = (barColors.appBar && openedApp && lockWindow.removed) || 
                     (barColors.mainBar && !openedApp);
  
  $: connectionLevel = (() => {
    switch (network) {
      case 1: return 'GSM';
      case 2: return '3G';
      case 3: return '4G';
      case 4: return '5G';
      default: return 'GSM';
    }
  })();
  
  $: getDurationSeconds = (() => {
    let seconds = Math.floor(callDuration / 1000 % 60);
    return seconds < 10 ? `0${seconds}` : seconds;
  })();
  
  $: getDurationMinutes = (() => {
    let minutes = Math.floor(callDuration / 1000 / 60 % 60);
    return minutes < 10 ? `0${minutes}` : minutes;
  })();
  
  $: getDurationHours = (() => {
    let hours = Math.floor(callDuration / 1000 / 60 / 60 % 60);
    return hours < 10 ? `0${hours}` : hours;
  })();
  
  // Functions
  function unescapeString(str) {
    return str ? str.replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>') : '';
  }
  
  function getName(phoneNumber) {
    let foundName = null;
    Object.keys(contacts).forEach(group => {
      if (!foundName) {
        const contact = contacts[group].find(c => c.phoneNumber === phoneNumber);
        if (contact) {
          foundName = contact.name;
        }
      }
    });
    return foundName || phoneNumber;
  }
  
  function notificationsAmount(app) {
    switch (app.app) {
      case 'messages': return unreadMessages;
      case 'phone': return missedCalls;
      default: return 0;
    }
  }
  
  function setPhoneState(disabled) {
    if (!phoneState.enableToContact) return;
    
    phoneState.enableToContact = false;
    phoneState.disabled = disabled;
    
    if (disabled) {
      // Выключаем экран
      console.log('Screen turning off');
      setTimeout(() => {
        resetLock();
        closeApp();
      }, 250);
    } else {
      // Включаем экран
      console.log('Screen turning on');
    }
    
    setTimeout(() => {
      phoneState.enableToContact = true;
    }, 500);
  }
  
  function resetLock() {
    lockWindow.removed = false;
    lockWindow.unlocked = false;
  }
  
// Замените эти функции в вашем index.svelte

function unblockPhone() {
  console.log('Attempting to unlock phone');
  openedApp = null;
  lockWindow.removed = true;
  lockWindow.unlocked = true;
  phoneState.isUnblocking = false; // Сбрасываем состояние
  console.log('Phone unlocked successfully');
}

function unlockingStart(event) {
  event.stopPropagation(); // Предотвращаем всплытие события
  phoneState.isUnblocking = true;
  phoneState.y = event.clientY;
  console.log('Unlock gesture started at:', phoneState.y);
}

function onMouseMove(event) {
  if (phoneState.isUnblocking) {
    const deltaY = phoneState.y - event.clientY;
    console.log('Mouse move delta:', deltaY);
    
    // Уменьшаем порог для более чувствительной разблокировки
    if (deltaY > 15) {
      console.log('Unlock threshold reached');
      unblockPhone();
    }
  }
}

function onMouseUp() {
  if (phoneState.isUnblocking) {
    console.log('Unlock gesture ended');
    phoneState.isUnblocking = false;
  }
}

// Альтернативный метод - простая разблокировка по клику
function simpleUnlock() {
  if (!lockWindow.removed && !phoneState.disabled) {
    console.log('Simple unlock triggered');
    unblockPhone();
  }
}
  

  function openApp(app) {
    console.log('Opening app:', app.app);
    
    // Check balance for car service
    if (app.app === 'carService' && balance <= 0) {
      console.log('No money on card');
      return;
    }
    
    // Check if app is disabled
    if (app.isDisabled) {
      console.log('App is disabled');
      return;
    }
    
    // Check limited applications
    const limitedApps = ['carService', 'settings', 'weazelNews', 'music', 'calendar', 'taxi', 'party', 'photos', 'camera'];
    if (limitedApplications && limitedApps.includes(app.app)) {
      console.log('App is limited');
      return;
    }
    
    switch (app.app) {
      case 'camera':
        cameraOpened = true;
        break;
         case 'contacts':
      // Открываем приложение phone на странице контактов
      openedApp = 'phone';
      phoneInitialPage = 'contacts'; // Передаем начальную страницу
      break;
    case 'phone':
      openedApp = 'phone';
      phoneInitialPage = 'recent'; // По умолчанию страница "Недавние"
      break;
      default:
        openedApp = app.app;
        break;
    }
    
    // Set bar colors
    barColors.appBar = app.dark || false;
  }
  
  function closeApp() {
    openedApp = null;
    cameraOpened = false;
    barColors.appBar = false;
  }
  
  function stopCall() {
    activeCall = null;
    callDuration = 0;
  }
  
  function acceptCall() {
    if (activeCall) {
      activeCall.active = true;
      activeCall.status = 'active';
    }
  }
  // Добавить в начало скрипта
let phoneInitialPage = 'recent'; // По умолчанию "Недавние"
  function declineCall() {
    activeCall = null;
    callDuration = 0;
  }
  
  function selectZoomLevel(level) {
    cameraState.zoomLevel = level;
  }
  
  function setCameraType(type) {
    cameraState.selectedCameraType = type;
  }
  
  // Lifecycle
  onMount(() => {
    // Initialize component
    console.log('iPhone component mounted');
    
    // Update time every second
    const timeInterval = setInterval(() => {
      const now = new Date();
      time = now.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
    }, 1000);
    
    return () => {
      clearInterval(timeInterval);
    };
  });
  
  onDestroy(() => {
    console.log('iPhone component destroyed');
  });
</script>

{#if show}
  <div class="iphone-main align-end justify-end" 
       on:mousemove={onMouseMove} 
       on:mouseup={onMouseUp}
       transition:slide>
    
    <!-- iPhone всегда показан, но с темным экраном когда disabled -->
    <div class="iphone"  transition:slide class:disabled={phoneState.disabled} >
      <div class="iphone-content">
        <!-- Shadow overlay -->
        <div class="iphone-shadow" 
     class:disabled={phoneState.disabled}
     style="background-color: rgba(0, 0, 0, {(100 - darkness) / 100 * 0.0});"
     on:click={() => setPhoneState(false)}>
</div>
        
        <!-- Status bar -->
        <div class="iphone-statusbar row-block" class:dark={statusbarStyle} class:default={!statusbarStyle}>
          <div class="time">{time}</div>
          <div class="phone-status align-center row-block">
            <!-- Network indicator -->
            <svg xmlns="http://www.w3.org/2000/svg" width="12.5" height="8" viewBox="0 0 12.5 8" class="net">
              <rect class:active={network >= 1} opacity="0.3" y="5" width="2" height="3" rx="1"/>
              <path opacity="0.3" class:active={network >= 2} 
                    d="M4.49,3.5h0a1,1,0,0,1,1,1V7a1,1,0,0,1-1,1h0a1,1,0,0,1-1-1V4.49a1,1,0,0,1,1-1Z"/>
              <rect opacity="0.3" class:active={network >= 3} x="7" y="1.8" width="2" height="6.2" rx="1"/>
              <path opacity="0.3" class:active={network >= 4}
                    d="M11.48,0h0a1,1,0,0,1,1,1V7a1,1,0,0,1-1,1h0a1,1,0,0,1-1-1V1a1,1,0,0,1,1-1Z"/>
            </svg>
            <!-- WiFi and battery icons would go here -->
          </div>
        </div>
        
        <!-- Camera notch -->
        <div class="camera full-width">
          <img class="camera__picture" src="{cdn}/img/iPhone/v2/DI.svg?a" alt="" />
        </div>
        
        <!-- Active call interface -->
        {#if activeCall}
          <div class="iphone-voice-conversation full-width full-height" transition:fade>
            <div class="main-content full-height full-width column-block align-center">
              <div class="main-content__text-username">{unescapeString(getName(activeCall.number))}</div>
              <div class="status-block">
                {#if activeCall.active}
                  <div class="status-block__text-isTalking justify-center">
                    {getDurationHours}:{getDurationMinutes}:{getDurationSeconds}
                  </div>
                {:else if activeCall.status === 'outcoming'}
                  <div class="status-block__text-isCalling">Вызов</div>
                {:else}
                  <div class="status-block__text-isCalling">Входящий вызов</div>
                {/if}
              </div>
              
              <div class="control-container full-width justify-center">
                {#if activeCall.status === 'outcoming' && !activeCall.active || activeCall.active}
                  <div class="control-button decline" on:click={stopCall}>
                    <!-- Decline icon -->
                  </div>
                {:else if activeCall.status === 'incoming' && !activeCall.active}
                  <div class="incoming-call justify-between full-width">
                    <div class="column-block align-center" on:click={declineCall}>
                      <div class="control-button decline">
                        <!-- Decline icon -->
                      </div>
                      <div class="incoming-call__text">Отклонить</div>
                    </div>
                    <div class="column-block align-center" on:click={acceptCall}>
                      <div class="control-button accept">
                        <!-- Accept icon -->
                      </div>
                      <div class="incoming-call__text">Принять</div>
                    </div>
                  </div>
                {/if}
              </div>
            </div>
          </div>
        {/if}
        
        <!-- Lock screen -->
        <div class="iphone-locked align-center column-block" 
             class:removed={lockWindow.removed}
             class:transition={!phoneState.disabled}
             style="background-image: url('{cdn}/img/iPhone/wallpapers/v2/{wallpapers + 1}.png?a')"
             on:click={simpleUnlock}
     on:mousedown={unlockingStart}
     on:touchstart={unlockingStart}>
       
          
          <div class="time-week align-center column-block">
            <span>{date}</span>
          </div>
          <div class="time-main">{time}</div>
          <div class="bottom-content align-center column-block">
            <div class="unlock-hint justify-between row-block">
              Для разблокировки нажмите на экран
            </div>
            <div class="handler full-width align-end justify-center">
              <div class="handler-content"></div>
            </div>
          </div>
        </div>
        
        <!-- Main content -->
        <div class="iphone-main-content">
          <!-- Opened app -->
          {#if openedApp}
            <div class="iphone-app" transition:fade>
              {#if openedApp === 'carService'}
                <CarService {cdn} on:close={closeApp} />
             {:else if openedApp === 'music'}
                <Music {cdn} on:close={closeApp} />
              {:else if openedApp === 'weazelNews'}
                <WeazelNews {cdn} on:close={closeApp} />
                {:else if openedApp === 'taxi'}
                <Taxi {cdn} on:close={closeApp} />
                {:else if openedApp === 'gallery'}
                <Gallery {cdn} on:close={closeApp} />
                {:else if openedApp === 'camera'}
                <Camera {cdn} on:close={closeApp} />
                {:else if openedApp === 'calendar'}
                <Calendar {cdn} on:close={closeApp} />
                {:else if openedApp === 'party'}
                <Party {cdn} on:close={closeApp} />
                {:else if openedApp === 'photos'}
                <Photos {cdn} on:close={closeApp} />
                {:else if openedApp === 'operator'}
                <Operator {cdn} on:close={closeApp} />
              {:else if openedApp === 'phone'}
  <Phone {cdn} initialPage={phoneInitialPage} on:close={closeApp} />
              {:else if openedApp === 'messages'}
                <Messages {cdn} on:close={closeApp} />
                {:else if openedApp === 'settings'}
<Settings {cdn} 
          on:close={closeApp} 
          on:darknessChange={(event) => darkness = event.detail}
          on:wallpaperChange={(event) => wallpapers = event.detail} />
              {/if}
              
            </div>
          {/if}
          
          <!-- Home screen -->
          <div class="iphone-home" class:hidden={!lockWindow.removed}>
            <div class="apps-container">
              {#each apps as app (app.app)}
                <div class="app-container align-center column-block">
                  <div class="app-container-icon flex-block" on:click={() => openApp(app)}>
                    {#if app.icon.type === 'image'}
                      <img class="app-container-icon__picture" 
                           src="{cdn}/img/global/ios/apps/v2/{app.app}.svg?aa" 
                           alt="" />
                    {:else if app.app === 'calendar'}
                      <mainw class="full-height column-block align-center full-width">
                        <span class="weekday">{shortWeekDay}</span>
                        <span class="monthday">{parseInt(monthDay)}</span>
                      </mainw>
                    {/if}
                  </div>
                  <div class="app-container__title">{app.title}</div>
                </div>
              {/each}
            </div>
            
            <div class="fast-apps-container justify-center">
              <div class="fast-apps align-center justify-center">
                {#each fastApps as app (app.app)}
                  <div class="app-container">
                    <div class="app-container-icon flex-block" on:click={() => openApp(app)}>
                      <img class="app-container-icon__picture" 
                           src="{cdn}/img/global/ios/apps/v2/{app.app}.svg" 
                           alt="" />
                      {#if notificationsAmount(app) > 0}
                        <div class="notifications-amount digits-{notificationsAmount(app).toString().length}">
                          {notificationsAmount(app)}
                        </div>
                      {/if}
                    </div>
                  </div>
                {/each}
              </div>
            </div>
          </div>
        </div>
        
        <!-- Background wallpaper -->
        <div class="iphone-background" 
             style="background-image: url('{cdn}/img/iPhone/wallpapers/v2/{wallpapers + 1}.png?a')">
        </div>
      </div>
      
      <!-- iPhone frame -->
      <img src="{cdn}/img/iPhone/v2/background.svg?aa" alt="" class="iphone-frame" />
      
      <!-- Unlock button -->
      <div class="unlock-button" on:click={() => setPhoneState(!phoneState.disabled)}></div>
    </div>
    
    <!-- Camera component -->
    {#if cameraOpened}
      <div class="camera full-width full-height" transition:fade>
        <div class="control-field row-block align-center">
          <div class="zoom-levels column-block justify-end align-center">
            <div class="zoom-level zoom-3" on:click={() => selectZoomLevel(3)}>
              <div class:selected={cameraState.zoomLevel === 3}>3x</div>
            </div>
            <div class="zoom-level zoom-1" on:click={() => selectZoomLevel(1)}>
              <div class:selected={cameraState.zoomLevel === 1}>1x</div>
            </div>
            <div class="zoom-level zoom-05" on:click={() => selectZoomLevel(0.5)}>
              <div class:selected={cameraState.zoomLevel === 0.5}>0.5x</div>
            </div>
          </div>
          
          <div class="control-panel full-height column-block align-center justify-center">
            <div class="action-button">
              <!-- Action button content -->
            </div>
            
            <div class="camera-types row-block">
              <div class="moving-container row-block" style="transform: translateX({cameraState.cameraTypeOffset}px)">
                {#each CAMERA_TYPES as type}
                  <div class="camera-type" 
                       class:selected={cameraState.selectedCameraType === type}
                       on:click={() => setCameraType(type)}>
                    {type === 'photo' ? 'Фото' : 'Видео'}
                  </div>
                {/each}
              </div>
            </div>
          </div>
        </div>
        
        <div class="frames full-height full-width">
          {#each Array(4) as _, i}
            <div class="frame">
              <div class="frame-element"></div>
              <div class="frame-element"></div>
            </div>
          {/each}
        </div>
      </div>
    {/if}
  </div>
{/if}

<style>
  /* Add your existing CSS styles here */
  .hidden {
    display: none;
  }
  
  .active {
    opacity: 1 !important;
  }

  
  /* Additional styles would go here based on your existing CSS */
</style>