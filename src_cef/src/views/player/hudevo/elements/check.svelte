<script>
  import { fly } from 'svelte/transition';
  import SVGComponent from './SVGComponent.svelte';
  import { translateText } from 'lang';
  import { TimeFormat, GetTime } from 'api/moment';
  import { serverDateTime } from 'store/server';
  import { charFractionID } from 'store/chars';
  import { format } from 'api/formatter';
  import { mapWidth } from 'store/map';

  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';

  let mapNotifications = [];
  
  const svgIcons = ['bank', 'info', 'warn', 'error', 'success'];
  
  const vehicleKeywords = {
    'двигатель': 'engine',
    'двери': 'door',
    'дверь': 'door',
    'ремень': 'seatbelt',
  };
  
  const actionWords = [
    'заведён', 'заведен', 'завел', 'завела',
    'заглушен', 'заглушён', 'заглушил', 'заглушила',
    'открыты', 'открыта', 'открыт', 'открыл', 'открыла',
    'закрыты', 'закрыта', 'закрыт', 'закрыл', 'закрыла',
    'пристёгнут', 'пристегнут', 'пристегнул', 'пристегнула',
    'отстёгнут', 'отстегнут', 'отстегнул', 'отстегнула'
  ];
  
  const notifyTypeToIcon = {
    0: 'info',
    1: 'error',
    2: 'success',
    3: 'info',
    4: 'error'
  };
  
  const notifyTypeToColor = {
    0: '#FFFFFF',
    1: '#F53A3A',
    2: '#58DE80',
    3: '#FFC130',
    4: '#F53A3A',
  };
  
  const FractionTypes = {
    0: -1,
    1: 1, 2: 1, 3: 1, 4: 1, 5: 1,
    6: 2, 7: 2, 8: 2, 9: 2, 14: 2, 15: 2, 18: 2,
    10: 0, 11: 0, 12: 0, 13: 0,
    16: 4, 17: 3
  };
  
  $: bottomIndent = 340;
  $: leftIndent = 45;
  $: notificationWidth = 350;
  
  const wait = (ms) => new Promise(resolve => setTimeout(resolve, ms));
  
  function getVehicleIcon(message) {
    const msgLower = (message || '').toLowerCase();
    
    for (const [keyword, icon] of Object.entries(vehicleKeywords)) {
      if (msgLower.includes(keyword.toLowerCase())) {
        return icon;
      }
    }
    
    return null;
  }
  
  // ✅ Генерация CSS filter для цвета
  function getColorFilter(color) {
    // Зелёный #58DE80
    if (color === '#58DE80') {
      return 'invert(77%) sepia(26%) saturate(566%) hue-rotate(81deg) brightness(95%) contrast(87%)';
    }
    // Красный #F53A3A
    if (color === '#F53A3A') {
      return 'invert(32%) sepia(89%) saturate(2281%) hue-rotate(343deg) brightness(96%) contrast(94%)';
    }
    // Оранжевый #FFC130
    if (color === '#FFC130') {
      return 'invert(77%) sepia(71%) saturate(543%) hue-rotate(360deg) brightness(103%) contrast(101%)';
    }
    // По умолчанию белый
    return '';
  }
  
  function highlightActions(message, color) {
    if (!message) return [{ text: message, color: '#ffffff' }];
    
    const msgLower = message.toLowerCase();
    
    for (const action of actionWords) {
      const index = msgLower.indexOf(action.toLowerCase());
      
      if (index !== -1) {
        const result = [];
        
        if (index > 0) {
          result.push({
            text: message.substring(0, index),
            color: '#ffffff'
          });
        }
        
        result.push({
          text: message.substring(index, index + action.length),
          color: color
        });
        
        if (index + action.length < message.length) {
          result.push({
            text: message.substring(index + action.length),
            color: '#ffffff'
          });
        }
        
        return result;
      }
    }
    
    return [{ text: message, color: '#ffffff' }];
  }
  
  if (typeof window !== 'undefined') {
    if (!window.wait) {
      window.wait = wait;
    }
    
    window.newNotify = (data) => {
      const notification = typeof data === 'string' 
        ? { message: data, type: 'info', small: true, id: Date.now() }
        : { ...data, id: Date.now() };
      
      console.log('[newNotify] Adding notification:', notification);
      mapNotifications = [...mapNotifications, notification];
      
      const timeout = notification.timeout || 5000;
      setTimeout(() => {
        mapNotifications = mapNotifications.filter(n => n.id !== notification.id);
      }, timeout);
    };
    
    window.listernEvent = window.listernEvent || {};
    window.listernEvent.vehicleNotify = (type, msg, time) => {
      console.log('[vehicleNotify]', { type, msg, time });
      
      const iconType = notifyTypeToIcon[type] || 'info';
      const vehicleIcon = getVehicleIcon(msg);
      const iconColor = notifyTypeToColor[type] || '#FFFFFF';
      
      window.newNotify({
        type: iconType,
        small: true,
        message: msg,
        timeout: time || 3000,
        iconColor: iconColor,
        vehicleIcon: vehicleIcon,
        highlightColor: iconColor
      });
    };
    
    window.PayDay = async (cash) => {
      try {
        if (cash > 0) {
          const fractionType = FractionTypes[$charFractionID] || -1;
          const isGovernment = fractionType === 2 || fractionType === 3;
          const reason = isGovernment 
            ? translateText('player2', 'Зарплата')
            : translateText('player2', 'Пособие по безработице');
          
          if (window.hudStore && typeof window.hudStore.HideHelp === 'function') {
            window.hudStore.HideHelp(true);
          }
          
          window.newNotify({
            type: 'bank',
            small: false,
            localIcon: 'CHAR_BANK_FLEECA',
            title: `Новое сообщение`,
            subtitle: "Банк",
            message: `Начисление средств $${format("money", cash)}.\nПричина: ${reason}`,
            fromPlayer: false,
            timeout: 5000
          });
          
          await wait(5000);
          
          if (window.hudStore && typeof window.hudStore.HideHelp === 'function') {
            window.hudStore.HideHelp(false);
          }
        } else {
          if (window.hudStore && typeof window.hudStore.HideHelp === 'function') {
            window.hudStore.HideHelp(false);
          }
        }
      } catch (error) {
        console.error('PayDay error:', error);
      }
    };
    
    window.testNewNotify = {
      vehicleEngineOn: () => {
        window.listernEvent.vehicleNotify(2, 'Двигатель заведён', 3000);
      },
      vehicleEngineOff: () => {
        window.listernEvent.vehicleNotify(1, 'Двигатель заглушен', 3000);
      },
      vehicleDoorsOpen: () => {
        window.listernEvent.vehicleNotify(2, 'Двери открыты', 3000);
      },
      vehicleDoorsClosed: () => {
        window.listernEvent.vehicleNotify(1, 'Двери закрыты', 3000);
      }
    };
    
    console.log('[check.svelte] Ready');
  }
  
  function getConvertedMessage(message, highlightColor = null) {
    if (!message) return [];
    
    if (highlightColor) {
      return highlightActions(message, highlightColor);
    }
    
    return message.split('\n').map(line => ({ text: line, color: '#ffffff' }));
  }
</script>

<style>
  .notifications-container {
    position: fixed;
    z-index: 100;
  }
  
  .map-notifications {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
</style>

<div 
  class="notifications-container column-block"
  style="bottom: {bottomIndent}px; left: {leftIndent}px; max-width: {notificationWidth}px; min-width: {notificationWidth}px;"
>
  <div class="map-notifications">
    {#each mapNotifications as notification, index (`${index}-${notification.type}`)}
      <div 
        class="map-notification list-complete-item align-center {notification.small ? 'small' : 'big'}"
        style="max-width: {notificationWidth}px; min-width: {notificationWidth}px;"
        transition:fly={{ x: -100, duration: 300 }}
      >
        {#if notification.small}
          <div class="row-block">
            {#if notification.vehicleIcon}
              <!-- ✅ ИСПРАВЛЕНО: Используем img с CSS filter -->
              <img 
                src="{cdn}icons/main/hud/map-notifications/{notification.vehicleIcon}.svg" 
                class="map-notification__icon"
                alt=""
                style="filter: brightness(0) saturate(100%) {getColorFilter(notification.iconColor)}"
              />
            {:else if svgIcons.includes(notification.type)}
              <img 
                src="{cdn}img/main/hud/notifications/bottom/{notification.type}.svg" 
                class="map-notification__icon"
                alt=""
              />
            {:else}
              <div class="icon-container" style="--svg-color: {notification.iconColor || '#fff'}">
                <SVGComponent 
                  path="icons/main/hud/map-notifications/{notification.type}.svg"
                  class="map-notification__icon"
                />
              </div>
            {/if}
            
            <div class="text-container">
              {#each getConvertedMessage(notification.message, notification.highlightColor) as part}
                <span class="map-notification__text" style="color: {part.color}">
                  {part.text}
                </span>
              {/each}
            </div>
          </div>
        {:else}
          <div class="column-block full-width full-height">
            <headernoti class="full-width row-block align-center">
              <div 
                class="picture" 
                style="background-image: url({notification.localIcon ? `${cdn}img/main/hud/notification_icons/${notification.localIcon}.png` : notification.icon})"
              ></div>
              
              <div class="headersno column-block">
                <span class="headers__title">{notification.title}</span>
                <span class="headers__subtitle">{notification.subtitle}</span>
              </div>
              
              <img 
                class="icon" 
                src="{cdn}img/main/hud/notifications/map/message.png" 
                alt=""
              />
            </headernoti>
            
            <mainnoti class="full-width full-height column-block">
              <div>
                {#each getConvertedMessage(notification.message) as part}
                  <span class="main-text" style="color: {part.color}">
                    {@html part.text.replace(/\n/g, '<br />')}
                  </span>
                {/each}
              </div>
              
              {#if notification.description}
                <span class="description {notification.fromPlayer ? 'from-player' : ''}">
                  {notification.description}
                </span>
              {/if}
            </mainnoti>
          </div>
        {/if}
      </div>
    {/each}
  </div>
</div>