<script>
  import { createEventDispatcher, onMount, onDestroy } from 'svelte';
  
  export let cdn = 'https://cdn.majestic-files.com/public/master/static';
      import './css/radio.css'

  const dispatch = createEventDispatcher();
  
  // Component state
  let playing = false;
  let volume = 0; // Начинаем с 0 как в оригинале
  let musicTime = 100;
  let title = 'SunBloom Radio';
  let possibleToChange = true;
  let isPlayingInterval = null;
  let failedAttempts = 0;
  let player = null;
  let sliderContainer = null;
  let isDragging = false;
  
  // Mock waves data with real radio streams
  let waves = [
    { 
      name: 'SunBloom Radio', 
      id: 1, 
      url: 'https://a6.radioheart.ru:9037/RH27711',
      logo: 'SunBloom'
    },
    { 
      name: 'RadioDanz Live', 
      id: 2, 
      url: 'https://streamssl.radiodanz.com/live',
      logo: 'majestic'
    }
  ];
  
  let currentWaveIndex = 0;
  
  // Computed values
  $: volumePercent = (volume / 50) * 100;
  
  $: radioPicture = (() => {
    const currentWave = waves[currentWaveIndex];
    return currentWave?.logo || 'majestic';
  })();
  
  $: currentStreamUrl = waves[currentWaveIndex]?.url;
  
  // Functions
  function switchState() {
    if (!possibleToChange) return;
    
    playing = !playing;
    console.log('Radio', playing ? 'Start' : 'Stop');
    
    if (playing) {
      startPlaying();
    } else {
      stopPlaying();
    }
  }
  
  function startPlaying() {
    console.log('Starting radio playback for:', currentStreamUrl);
    possibleToChange = false;
    
    if (player) {
      player.src = currentStreamUrl;
      player.volume = volume / 50; // Convert to 0-1 range
      
      const playPromise = player.play();
      
      if (playPromise !== undefined) {
        playPromise
          .then(() => {
            console.log('Radio started successfully');
            possibleToChange = true;
            failedAttempts = 0;
          })
          .catch((error) => {
            console.error('Failed to start radio:', error);
            possibleToChange = true;
            playing = false;
            showError('Не удалось подключиться к радиостанции');
          });
      }
    } else {
      // Fallback for no audio support
      setTimeout(() => {
        possibleToChange = true;
        console.log('Radio started successfully (fallback)');
      }, 1000);
    }
  }
  
  function stopPlaying() {
    console.log('Stopping radio playback');
    failedAttempts = 0;
    
    if (player) {
      player.pause();
      player.currentTime = 0;
    }
    
    clearInterval(isPlayingInterval);
  }
  
  function selectWave(direction) {
    if (!possibleToChange) return;
    
    // Stop current playback
    if (playing) {
      stopPlaying();
      playing = false;
    }
    
    if (direction === 'next') {
      currentWaveIndex = (currentWaveIndex + 1) % waves.length;
    } else if (direction === 'prev') {
      currentWaveIndex = currentWaveIndex === 0 ? waves.length - 1 : currentWaveIndex - 1;
    }
    
    title = waves[currentWaveIndex].name;
    console.log('Selected wave:', title, 'URL:', currentStreamUrl);
  }
  
  // Vue-slider functions
  function handleSliderClick(event) {
    if (isDragging) return;
    
    const rect = sliderContainer.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const percentage = (x / rect.width) * 100;
    const newVolume = Math.round((percentage / 100) * 50);
    
    volume = Math.max(0, Math.min(50, newVolume));
    console.log('Volume changed via click:', volume);
  }
  
  function handleDotMouseDown(event) {
    event.preventDefault();
    isDragging = true;
    
    const handleMouseMove = (e) => {
      if (!isDragging || !sliderContainer) return;
      
      const rect = sliderContainer.getBoundingClientRect();
      const x = e.clientX - rect.left;
      const percentage = Math.max(0, Math.min(100, (x / rect.width) * 100));
      const newVolume = Math.round((percentage / 100) * 50);
      
      volume = Math.max(0, Math.min(50, newVolume));
      
      // Force reactive update for smooth dragging
      volumePercent = (volume / 50) * 100;
    };
    
    const handleMouseUp = () => {
      isDragging = false;
      document.removeEventListener('mousemove', handleMouseMove);
      document.removeEventListener('mouseup', handleMouseUp);
      console.log('Volume changed via drag:', volume);
    };
    
    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
  }
  
  function handleKeyDown(event) {
    switch(event.key) {
      case 'ArrowLeft':
        event.preventDefault();
        volume = Math.max(0, volume - 1);
        break;
      case 'ArrowRight':
        event.preventDefault();
        volume = Math.min(50, volume + 1);
        break;
      case 'Home':
        event.preventDefault();
        volume = 0;
        break;
      case 'End':
        event.preventDefault();
        volume = 50;
        break;
    }
  }
  
  function showError(message) {
    console.error(message);
  }
  
  function closeApp() {
    dispatch('close');
  }
  
  function closingStart() {
    console.log('Closing gesture started');
  }
  
  // Lifecycle
  onMount(() => {
    console.log('Music app mounted');
    
    // Initialize HTML5 Audio for radio streaming
    try {
      player = new Audio();
      player.crossOrigin = 'anonymous';
      player.preload = 'none';
      
      // Set initial stream
      if (currentStreamUrl) {
        player.src = currentStreamUrl;
      }
      
      // Audio event listeners
      player.addEventListener('loadstart', () => {
        console.log('Loading radio stream...');
      });
      
      player.addEventListener('canplay', () => {
        console.log('Radio stream ready to play');
        possibleToChange = true;
      });
      
      player.addEventListener('error', (e) => {
        console.error('Audio error:', e);
        possibleToChange = true;
        playing = false;
        showError('Ошибка воспроизведения радио');
      });
      
      player.addEventListener('ended', () => {
        // Radio streams shouldn't end, but handle it
        console.log('Radio stream ended unexpectedly');
        playing = false;
      });
      
    } catch (error) {
      console.error('Failed to initialize audio:', error);
      // Fallback to mock player for environments without audio support
      player = {
        play: () => Promise.resolve(),
        pause: () => {},
        currentTime: 0,
        volume: 1,
        src: ''
      };
    }
  });
  
  // Watch volume changes and apply to player
  $: if (player && player.volume !== undefined) {
    player.volume = volume / 50; // Convert 0-50 to 0-1
  }
  
  onDestroy(() => {
    if (isPlayingInterval) {
      clearInterval(isPlayingInterval);
    }
    console.log('Music app destroyed');
  });
</script>

<div class="music-container column-block full-width full-height">
  <!-- Radio logo -->
  <img 
    class="music-container__picture" 
    src="{cdn}/img/iPhone/apps/music/logotypes/{radioPicture}.png" 
    alt="" 
  />
  
  <!-- Title block -->
  <div class="title-block column-block full-width">
    <div class="title-block__value-name">{title}</div>
    <div class="title-block__value-author">Radio</div>
  </div>
  
  <!-- Progress bar -->
  <div class="music-time column-block">
    <div class="progress-block flex-block full-width">
      <div class="progress-block__progress full-width">
        <div 
          class="value full-height" 
          style="width: {musicTime}%"
        ></div>
      </div>
    </div>
    <div class="time justify-between row-block full-width">
      <span>-∞</span>
      <span>∞</span>
    </div>
  </div>
  
  <!-- Control buttons -->
  <div class="control-buttons justify-center align-center row-block">
    <!-- Previous button -->
    <div 
      class="arrowr" 
      class:disable={!possibleToChange}
      on:click={() => selectWave('prev')}
    >
      <svg class="control-buttons__picture-forward" viewBox="0 0 24 24">
        <path d="M15.41,16.58L10.83,12L15.41,7.41L14,6L8,12L14,18L15.41,16.58Z" fill="#000"/>
      </svg>
    </div>
    
    <!-- Play/Pause button -->
    <div 
      class="play" 
      class:disable={!possibleToChange}
      on:click={switchState}
    >
      {#if playing}
        <svg class="control-buttons__picture-play" viewBox="0 0 24 24">
          <path d="M14,19H18V5H14M6,19H10V5H6V19Z" fill="#000"/>
        </svg>
      {:else}
        <svg class="control-buttons__picture-play" viewBox="0 0 24 24">
          <path d="M8,5.14V19.14L19,12.14L8,5.14Z" fill="#000"/>
        </svg>
      {/if}
    </div>
    
    <!-- Next button -->
    <div 
      class="arrowr" 
      class:disable={!possibleToChange}
      on:click={() => selectWave('next')}
    >
      <svg class="control-buttons__picture-forward" viewBox="0 0 24 24">
        <path d="M8.59,16.58L13.17,12L8.59,7.41L10,6L16,12L10,18L8.59,16.58Z" fill="#000"/>
      </svg>
    </div>
  </div>
  
  <!-- Volume control -->
  <div class="volume row-block align-center">
    <!-- Volume down icon -->
    <svg class="volume__picture" viewBox="0 0 24 24">
      <path d="M3,9V15H7L12,20V4L7,9H3M16.5,12A4.5,4.5 0 0,0 14.5,8.32V15.68A4.5,4.5 0 0,0 16.5,12Z"/>
    </svg>
    
    <!-- Vue-slider -->
    <div 
      class="vue-slider vue-slider-ltr" 
      style="padding: 7px 0px; width: auto; height: 4px;"
      bind:this={sliderContainer}
      on:click={handleSliderClick}
    >
      <div class="vue-slider-rail">
        <!-- Process bar -->
        <div 
          class="vue-slider-process" 
          style="height: 100%; top: 0px; left: 0%; width: {volumePercent}%; transition-property: width, left; transition-duration: 0.5s;"
        ></div>
        
        <!-- Dot handle -->
        <div 
          class="vue-slider-dot" 
          aria-valuetext={volume.toString()}
          role="slider" 
          aria-valuenow={volume}
          aria-valuemin="0" 
          aria-valuemax="50" 
          aria-orientation="horizontal" 
          tabindex="0"
          style="width: 14px; height: 14px; transform: translate(-50%, -50%); top: 50%; left: {volumePercent}%; transition: left 0.5s;"
          on:mousedown={handleDotMouseDown}
          on:keydown={handleKeyDown}
        >
          <div class="vue-slider-dot-handle"></div>
        </div>
      </div>
    </div>
    
    <!-- Volume up icon -->
    <svg class="volume__picture" viewBox="0 0 24 24">
      <path d="M14,3.23V5.29C16.89,6.15 19,8.83 19,12C19,15.17 16.89,17.85 14,18.71V20.77C18.01,19.86 21,16.28 21,12C21,7.72 18.01,4.14 14,3.23M16.5,12C16.5,10.23 15.5,8.71 14,7.97V16C15.5,15.29 16.5,13.76 16.5,12M3,9V15H7L12,20V4L7,9H3Z"/>
    </svg>
  </div>
  
  <!-- Close handle -->
  <div 
    class="close-block full-width"
    on:mousedown={closingStart}
    on:click={closeApp}
  >
    <div class="close-block-handler"></div>
  </div>
</div>