<script>
	import { createEventDispatcher } from 'svelte';
	import './css/settings.css';
	
	export let cdn = '';
	
	const dispatch = createEventDispatcher();
	
	// Состояние приложения
	let page = 'main'; // 'main', 'display', 'sounds', 'wallpapers'
	let exit = true;
	
	// Данные пользователя
	let login = 'Игрок';
	let phoneNumber = '12345';
	
	// Настройки
	let settings = [
		{
			path: 'DND',
			title: 'Не беспокоить',
			checkbox: false
		},
		{
			path: 'display',
			title: 'Экран и яркость',
			checkbox: null
		},
		{
			path: 'sounds',
			title: 'Звуки',
			checkbox: null
		},
		{
			path: 'wallpapers',
			title: 'Обои',
			checkbox: null
		}
	];
	
	// Настройки дисплея
	let darkness = 70;
	let savedDarkness = 70;
	let darknessDebounce = null;
	
	// Настройки звука
	let volume = 50; // 0-100 для удобства
	let savedVolume = 50;
	let volumeDebounce = null;
	let dragging = false;
	let ringtone = 0; // Возвращаем к 0 - это будет "По умолчанию"
	let notification = 0; // Возвращаем к 0 - это будет "По умолчанию"
	
	// Аудио данные
	let audios = {
		ringtones: {
			title: 'Мелодии звонка',
			sounds: [
				{ title: 'По умолчанию', sound: null },
				{ title: 'Michael Ringtone', sound: null },
				{ title: 'Travis Scott - goosebumps', sound: null },
				{ title: 'Izantachi - Zero Two', sound: null },
				{ title: '$uicideboy$ - Champion of Death', sound: null },
				{ title: 'Gorillaz - Feel Good Inc', sound: null },
				{ title: 'XXXTENTACION - Revenge', sound: null },
				{ title: 'XXXTENTACION - Jocelyn Flores', sound: null },
				{ title: 'Lil Nas X - HOLIDAY', sound: null },
				{ title: '4 Thangs', sound: null },
				{ title: 'lxst - ODIUM', sound: null },
				{ title: 'Mishlawi - All Night', sound: null },
				{ title: 'XXXTENTACION - Look At Me!', sound: null },
				{ title: 'Big K.R.I.T. - Kickoff', sound: null },
				{ title: 'ELEMENT', sound: null },
				{ title: 'All Night Long', sound: null },
				{ title: 'Tony Igy - Astronomia', sound: null },
				{ title: 'Mellen Gi Remix', sound: null },
				{ title: 'Travis Scott - STOP TRYING TO BE GOD', sound: null },
				{ title: 'Kanye West - Stronger', sound: null },
				{ title: 'iPhone Remix', sound: null },
				{ title: 'El Sonidito', sound: null },
				{ title: 'Walk Man', sound: null },
				{ title: 'Kulikitaka', sound: null },
				{ title: 'Telephone Awoo', sound: null },
				{ title: 'Galaxy S8', sound: null },
				{ title: 'Galaxy S10', sound: null },
				{ title: 'Lil Nas X - INDUSTRY BABY', sound: null },
				{ title: 'Air Max Violin', sound: null },
				{ title: 'Amaarae - Sad Girlz Luv', sound: null },
				{ title: 'Doja Cat - Woman', sound: null },
				{ title: 'LOONA - Why Not', sound: null },
				{ title: 'TGC - Dreamers', sound: null },
				{ title: 'Zone Ankha', sound: null },
				{ title: 'JNR - Choi Gunna To The Moon Gunna', sound: null },
				{ title: 'Central Cee - Obsessed With You', sound: null },
				{ title: 'Dave - Starlight', sound: null },
				{ title: 'Digga D - Pump 101', sound: null },
				{ title: 'Pashanim - Sommergewitter', sound: null },
				{ title: 'Russ Millions - Reggae & Calypso', sound: null }
			]
		},
		notifications: {
			title: 'Звуки уведомлений',
			sounds: [
				{ title: 'По умолчанию', sound: null },
				{ title: 'Уведомление 2', sound: null },
				{ title: 'Уведомление 3', sound: null },
				{ title: 'Уведомление 4', sound: null },
				{ title: 'Уведомление 5', sound: null },
				{ title: 'Уведомление 6', sound: null },
				{ title: 'Уведомление 7', sound: null },
				{ title: 'Уведомление 8', sound: null },
				{ title: 'Уведомление 9', sound: null },
				{ title: 'Уведомление 10', sound: null },
				{ title: 'Уведомление 11', sound: null },
				{ title: 'Уведомление 12', sound: null },
				{ title: 'Уведомление 13', sound: null },
				{ title: 'Уведомление 14', sound: null }
			]
		}
	};
	
	// Создаем аудио объекты при инициализации
	function initializeAudio() {
		// Инициализация мелодий звонков
		audios.ringtones.sounds.forEach((sound, index) => {
			if (index === 0) {
				// Первый элемент "По умолчанию" использует файл 1.ogg
				const audio = new Audio(`${cdn}/sounds/iphone/ringtones/1.ogg`);
				audio.volume = savedVolume / 100;
				sound.sound = {
					play: () => {
						audio.volume = savedVolume / 100;
						audio.currentTime = 0;
						audio.play().catch(e => console.log('Audio play failed:', e));
					},
					stop: () => {
						audio.pause();
						audio.currentTime = 0;
					},
					setVolume: (vol) => {
						audio.volume = vol / 100;
					}
				};
			} else {
				// Остальные элементы: index 1 -> файл 2.ogg, index 2 -> файл 3.ogg и т.д.
				const audio = new Audio(`${cdn}/sounds/iphone/ringtones/${index + 1}.ogg`);
				audio.volume = savedVolume / 100;
				sound.sound = {
					play: () => {
						audio.volume = savedVolume / 100;
						audio.currentTime = 0;
						audio.play().catch(e => console.log('Audio play failed:', e));
					},
					stop: () => {
						audio.pause();
						audio.currentTime = 0;
					},
					setVolume: (vol) => {
						audio.volume = vol / 100;
					}
				};
			}
		});
		
		// Инициализация звуков уведомлений
		audios.notifications.sounds.forEach((sound, index) => {
			if (index === 0) {
				// Первый элемент "По умолчанию" использует файл 1.ogg
				const audio = new Audio(`${cdn}/sounds/iphone/notifications/1.ogg`);
				audio.volume = savedVolume / 100;
				sound.sound = {
					play: () => {
						audio.volume = savedVolume / 100;
						audio.currentTime = 0;
						audio.play().catch(e => console.log('Audio play failed:', e));
					},
					stop: () => {
						audio.pause();
						audio.currentTime = 0;
					},
					setVolume: (vol) => {
						audio.volume = vol / 100;
					}
				};
			} else {
				// Остальные элементы: index 1 -> файл 2.ogg, index 2 -> файл 3.ogg и т.д.
				const audio = new Audio(`${cdn}/sounds/iphone/notifications/${index + 1}.ogg`);
				audio.volume = savedVolume / 100;
				sound.sound = {
					play: () => {
						audio.volume = savedVolume / 100;
						audio.currentTime = 0;
						audio.play().catch(e => console.log('Audio play failed:', e));
					},
					stop: () => {
						audio.pause();
						audio.currentTime = 0;
					},
					setVolume: (vol) => {
						audio.volume = vol / 100;
					}
				};
			}
		});
	}
	
	// Обои
	let wallpapers = 0;
	let wallpaperDebounce = null;
	
	// Слайдеры настройки (как в оригинальном JS)
	const darknessSlider = {
		min: 30,
		max: 100,
		interval: 1,
		width: "18vh"
	};
	
	const volumeSlider = {
		min: 0,
		max: 100, // изменено с 0-1 на 0-100
		interval: 1,
		width: "18vh"
	};
	
	// Основные функции
	function openPage(pageType) {
		page = pageType;
	}
	
	function closeApp() {
		dispatch('close');
	}
	
	function closingStart() {
		console.log('Closing start');
	}
	
	// Обработка настроек
	function interaction(setting) {
		if (setting.checkbox !== null) {
			// Это чекбокс
			if (darknessDebounce) return false;
			
			switch (setting.path) {
				case 'DND':
					setting.checkbox = !setting.checkbox;
					console.log('Toggle DND:', setting.checkbox);
					break;
			}
			
			darknessDebounce = setTimeout(() => {
				clearTimeout(darknessDebounce);
				darknessDebounce = null;
			}, 1000);
		} else {
			// Это страница
			openPage(setting.path);
		}
	}
	
	// Настройки дисплея
	function back() {
		openPage('main');
	}
	
	function changeDarknessValue(e) {
		let value = parseInt(e.target.value);
		value = Math.min(Math.max(value, darknessSlider.min), darknessSlider.max);
		darkness = value;
		savedDarkness = value;
		
		// Отправляем событие изменения яркости в родительский компонент
		dispatch('darknessChange', value);
		
		clearTimeout(darknessDebounce);
		darknessDebounce = null;
		darknessDebounce = setTimeout(() => {
			console.log('Change darkness:', value / 100);
		}, 1000);
	}
	
	// Обработка кликов на слайдере
	let isDragging = false;
	let currentSliderType = null;
	
	function handleSliderClick(event, sliderType) {
		const rect = event.currentTarget.getBoundingClientRect();
		const clickX = event.clientX - rect.left;
		const percentage = Math.max(0, Math.min(100, (clickX / rect.width) * 100));
		
		if (sliderType === 'darkness') {
			const newValue = Math.round((percentage / 100) * (darknessSlider.max - darknessSlider.min) + darknessSlider.min);
			savedDarkness = Math.min(Math.max(newValue, darknessSlider.min), darknessSlider.max);
			changeDarknessValue({ target: { value: savedDarkness } });
		} else if (sliderType === 'volume') {
			const newValue = Math.round((percentage / 100) * volumeSlider.max);
			savedVolume = Math.min(Math.max(newValue, volumeSlider.min), volumeSlider.max);
			changeVolumeValue({ target: { value: savedVolume } });
		}
	}
	
	function handleSliderMouseDown(event, sliderType) {
		isDragging = true;
		currentSliderType = sliderType;
		handleSliderClick(event, sliderType);
		
		if (sliderType === 'volume') {
			dragging = true;
		}
		
		// Добавляем глобальные обработчики
		document.addEventListener('mousemove', handleGlobalMouseMove);
		document.addEventListener('mouseup', handleGlobalMouseUp);
		
		event.preventDefault();
	}
	
	function handleGlobalMouseMove(event) {
		if (!isDragging || !currentSliderType) return;
		
		// Находим элемент слайдера по типу
		let sliderElement = null;
		if (currentSliderType === 'darkness') {
			sliderElement = document.querySelector('.display .vue-slider');
		} else if (currentSliderType === 'volume') {
			sliderElement = document.querySelector('.sounds .vue-slider');
		}
		
		if (sliderElement) {
			// Создаем псевдо-событие для handleSliderClick
			const fakeEvent = {
				currentTarget: sliderElement,
				clientX: event.clientX
			};
			handleSliderClick(fakeEvent, currentSliderType);
		}
	}
	
	function handleGlobalMouseUp() {
		if (isDragging && currentSliderType === 'volume') {
			dragging = false;
		}
		
		isDragging = false;
		currentSliderType = null;
		
		// Убираем глобальные обработчики
		document.removeEventListener('mousemove', handleGlobalMouseMove);
		document.removeEventListener('mouseup', handleGlobalMouseUp);
	}
	
	// Настройки звука
	function changeVolumeValue(e) {
		let value = parseInt(e.target.value);
		volume = value;
		savedVolume = value;
		
		// Обновляем громкость всех аудио объектов
		updateAllAudioVolumes(value);
		
		if (!volumeDebounce && !dragging) {
			console.log('Change volume:', value / 100); // Конвертируем в 0-1 для API
			volumeDebounce = setTimeout(() => {
				clearTimeout(volumeDebounce);
				volumeDebounce = null;
			}, 500);
		}
	}
	
	// Обновляет громкость всех аудио объектов
	function updateAllAudioVolumes(volume) {
		Object.keys(audios).forEach(category => {
			audios[category].sounds.forEach(sound => {
				if (sound.sound && sound.sound.setVolume) {
					sound.sound.setVolume(volume);
				}
			});
		});
	}
	
	function stopAllAudios() {
		Object.keys(audios).forEach(category => {
			audios[category].sounds.forEach(sound => sound.sound.stop());
		});
	}
	
	function getCurrentCategory(category) {
		switch (category) {
			case 'ringtones':
				return ringtone;
			case 'notifications':
				return notification;
			default:
				return 0;
		}
	}
	
	function selectSound(sound, category, index) {
		if (volumeDebounce) return;
		
		if ((category === 'ringtones' && ringtone === index) || 
			(category === 'notifications' && notification === index)) {
			return;
		}
		
		stopAllAudios();
		sound.sound.play();
		
		switch (category) {
			case 'ringtones':
				ringtone = index;
				console.log('Select ringtone:', index);
				break;
			case 'notifications':
				notification = index;
				console.log('Select notification:', index);
				break;
		}
		
		// Принудительно обновляем компонент
		audios = audios;
		
		volumeDebounce = setTimeout(() => {
			clearTimeout(volumeDebounce);
			volumeDebounce = null;
		}, 1000);
	}
	
	// Настройки обоев
	function selectWallpaper(index) {
		if (wallpaperDebounce) return;
		
		wallpapers = index;
		
		// Отправляем событие изменения обоев в родительский компонент
		dispatch('wallpaperChange', index);
		
		console.log('Select wallpaper:', index);
		
		wallpaperDebounce = setTimeout(() => {
			clearTimeout(wallpaperDebounce);
			wallpaperDebounce = null;
		}, 1000);
	}
	
	// Утилиты
	function sliceString(str, maxLength) {
		return str && str.length > maxLength ? str.slice(0, maxLength) + '...' : str;
	}
	
	// Watchers
	$: if (darkness !== savedDarkness) {
		savedDarkness = darkness;
	}
	
	$: if (volume !== savedVolume) {
		savedVolume = volume;
	}
	
	// Lifecycle
	$: if (page === 'wallpapers') {
		exit = false;
	} else {
		exit = true;
	}
	
	// Инициализируем аудио при монтировании компонента
	import { onMount } from 'svelte';
	
	onMount(() => {
		initializeAudio();
	});
</script>

<style>
	/* Скрываем стандартный range input но оставляем интерактивность */
	.hidden-range {
		position: absolute;
		top: 0;
		left: 0;
		width: 100%;
		height: 100%;
		opacity: 0;
		cursor: pointer;
		z-index: 100;
		margin: 0;
		padding: 0;
		-webkit-appearance: none;
		appearance: none;
		background: transparent;
	}
	
	.hidden-range::-webkit-slider-thumb {
		-webkit-appearance: none;
		appearance: none;
		width: 20px;
		height: 20px;
		cursor: pointer;
		background: transparent;
	}
	
	.hidden-range::-moz-range-thumb {
		width: 20px;
		height: 20px;
		cursor: pointer;
		border: none;
		background: transparent;
		-moz-appearance: none;
	}
	
	.hidden-range::-moz-range-track {
		background: transparent;
		border: none;
	}
	
	/* Убираем фокус outline */
	.hidden-range:focus {
		outline: none;
	}
	
	/* Позиционирование контейнера слайдера */
	.vue-slider {
		position: relative;
	}
</style>

<!-- svelte-ignore a11y-click-events-have-key-events -->
<div class="settings column-block full-width full-height">
	{#if page === 'main'}
	<!-- Главная страница настроек -->
	<div class="main2-page full-height column-block">
		<header class="full-width align-end">Настройки</header>
		<main2 class="column-block full-width full-height">
			<div class="personal-data row-block align-center full-width">
				<img 
					src="{cdn}/img/avatars/male.svg" 
					alt="avatar" 
					class="light personal-data__picture"
				/>
				<div class="user-info column-block">
					<span class="user-info__text">{login}</span>
					<div class="phone-number">
						<span class="phone-number__title">Мой номер:</span>
						<span class="phone-number__value">{phoneNumber}</span>
					</div>
				</div>
			</div>
			
			<div class="containerSe column-block">
				{#each settings as setting (setting.path)}
				<div class="setting-item align-center row-block" on:click={() => interaction(setting)}>
					<img 
						src="{cdn}/img/iPhone/apps/settings/icons/{setting.path}.svg"
						class="setting-picture"
						alt=""
					/>
					<div class="setting-content align-center full-width row-block justify-between">
						<div class="setting-content__title">{setting.title}</div>
						
						{#if setting.checkbox !== null}
						<div class="checkbox align-center">
							<input type="checkbox" bind:checked={setting.checkbox} />
						</div>
						{:else}
						<svg class="arrow" viewBox="0 0 24 24">
							<path d="M8.59 16.59L10 18l6-6-6-6-1.41 1.41L13.17 12l-4.58 4.59z"/>
						</svg>
						{/if}
					</div>
				</div>
				{/each}
			</div>
		</main2>
	</div>
	
	{:else if page === 'display'}
	<!-- Страница настроек дисплея -->
	<div class="display column-block">
		<header class="row-block align-end full-width">
			<div class="header-containerSe align-center row-block full-width">
				<div class="backwards align-center row-block" on:click={back}>
					
					<svg  class="backwards__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="17" height="17" viewBox="0 0 17 17">
  <g transform="translate(-2223 3309) rotate(180)">
    <path d="M7,1,1,6l6,5" transform="translate(-2240 3294.5)" fill="none" stroke="#000" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/>
  </g>
</svg>
					<span class="backwards__text">Назад</span>
				</div>
				<div class="header-containerSe__title full-width justify-center">
					Экран и яркость
				</div>
			</div>
		</header>
		
		<main2>
			<div class="title">Яркость экрана</div>
			<div class="controller row-block align-center">
				<img 
					src="{cdn}/img/iPhone/apps/settings/display/low-light.svg"
					class="controller__picture low-light"
					alt=""
				/>
				
				<!-- Интерактивный Vue Slider -->
				<div 
					class="vue-slider vue-slider-ltr controller__slider" 
					style="padding: 7px 0px; width: 18vh; height: 4px;"
					on:mousedown={(e) => handleSliderMouseDown(e, 'darkness')}
					on:click={(e) => handleSliderClick(e, 'darkness')}
				>
					<div class="vue-slider-rail">
						<div 
							class="vue-slider-process" 
							style="height: 100%; top: 0px; left: 0%; width: {((savedDarkness - darknessSlider.min) / (darknessSlider.max - darknessSlider.min)) * 100}%; transition-property: width, left; transition-duration: 0.5s;"
						></div>
						<div 
							class="vue-slider-dot" 
							aria-valuetext="{savedDarkness}"
							role="slider"
							aria-valuenow="{savedDarkness}"
							aria-valuemin="{darknessSlider.min}"
							aria-valuemax="{darknessSlider.max}"
							aria-orientation="horizontal"
							tabindex="0"
							style="width: 14px; height: 14px; transform: translate(-50%, -50%); top: 50%; left: {((savedDarkness - darknessSlider.min) / (darknessSlider.max - darknessSlider.min)) * 100}%; transition: left 0.5s;"
						>
							<div class="vue-slider-dot-handle"></div>
						</div>
					</div>
				</div>
				
				<img 
					src="{cdn}/img/iPhone/apps/settings/display/high-light.svg"
					class="controller__picture high-light"
					alt=""
				/>
			</div>
		</main2>
	</div>
	
	{:else if page === 'sounds'}
	<!-- Страница настроек звука -->
	<div class="sounds colummn-block full-width full-height">
		<header class="row-block align-end full-width">
			<div class="header-containerA align-center row-block full-width">
				<div class="backwards align-center row-block" on:click={back}>
					<svg  class="backwards__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="17" height="17" viewBox="0 0 17 17">
  <g transform="translate(-2223 3309) rotate(180)">
    <path d="M7,1,1,6l6,5" transform="translate(-2240 3294.5)" fill="none" stroke="#000" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/>
  </g>
</svg>
					<span class="backwards__text">Назад</span>
				</div>
				<div class="header-containerA__title full-width justify-center">
					Звуки
				</div>
			</div>
		</header>
		
		<main class="full-width full-height" style="overflow-y: auto; max-height: 51.8518518519vh;">
			<div class="containerA">
				<div class="containerA__title">Громкость звуков</div>
				<div>
					<div class="tile row-block align-center volume">
						<img src="{cdn}/img/iPhone/apps/settings/sounds/less.svg" alt="" />
						
						<!-- Интерактивный Vue Slider для громкости -->
						<div 
							class="vue-slider vue-slider-ltr tile__slider" 
							style="padding: 7px 0px; width: 18vh; height: 4px;"
							on:mousedown={(e) => handleSliderMouseDown(e, 'volume')}
							on:click={(e) => handleSliderClick(e, 'volume')}
						>
							<div class="vue-slider-rail">
								<div 
									class="vue-slider-process" 
									style="height: 100%; top: 0px; left: 0%; width: {(savedVolume / volumeSlider.max) * 100}%; transition-property: width, left; transition-duration: 0.5s;"
								></div>
								<div 
									class="vue-slider-dot" 
									aria-valuetext="{savedVolume / 100}"
									role="slider"
									aria-valuenow="{savedVolume / 100}"
									aria-valuemin="{volumeSlider.min}"
									aria-valuemax="1"
									aria-orientation="horizontal"
									tabindex="0"
									style="width: 14px; height: 14px; transform: translate(-50%, -50%); top: 50%; left: {(savedVolume / volumeSlider.max) * 100}%; transition: left 0.5s;"
								>
									<div class="vue-slider-dot-handle"></div>
								</div>
							</div>
						</div>
						
						<img src="{cdn}/img/iPhone/apps/settings/sounds/more.svg" alt="" />
					</div>
				</div>
			</div>
			
			{#each Object.keys(audios) as category}
			<div class="containerA column-block">
				<div class="containerA__title">{audios[category].title}</div>
				<div class="column-block full-width">
					{#each audios[category].sounds as sound, index}
					<div class="tile row-block align-center" on:click={() => selectSound(sound, category, index)}>
						<div class="tick full-height">
							{#if index === getCurrentCategory(category)}
							<img src="{cdn}/img/iPhone/apps/settings/sounds/tick.svg" alt="" />
							{/if}
						</div>
						<div class="tile__title full-width align-center">
							{sliceString(sound.title, 26)}
						</div>
					</div>
					{/each}
				</div>
			</div>
			{/each}
		</main>
	</div>
	
	{:else if page === 'wallpapers'}
	<!-- Страница настроек обоев -->
	<div class="wallpapers colummn-block full-width full-height">
		<header class="row-block align-end full-width">
			<div class="header-containerSe align-center row-block full-width">
				<div class="backwards align-center row-block" on:click={back}>
					<svg  class="backwards__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="17" height="17" viewBox="0 0 17 17">
  <g transform="translate(-2223 3309) rotate(180)">
    <path d="M7,1,1,6l6,5" transform="translate(-2240 3294.5)" fill="none" stroke="#000" stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"/>
  </g>
</svg>
					<span class="backwards__text">Назад</span>
				</div>
				<div class="header-containerSe__title full-width justify-center">
					Выбрать обои
				</div>
			</div>
		</header>
		
		<main2 class="full-width full-height row-block">
			{#each Array(21) as _, index}
			<div class="tile column-block align-center" on:click={() => selectWallpaper(index)}>
				<div 
					class="tile__picture"
					style="background-image: url({cdn}/img/iPhone/wallpapers/v2/{index + 1}.png?aa)"
				>
					<img 
						class="selected {index === wallpapers ? 'showed' : ''}"
						src="{cdn}/img/iPhone/apps/settings/icons/selected.svg"
						alt=""
					/>
				</div>
				<div class="tile__title">Обои {index + 1}</div>
			</div>
			{/each}
		</main2>
	</div>
	{/if}
	
	<!-- Кнопка закрытия -->
	<div class="close-block full-width" 
		 on:mousedown={closingStart}
		 on:click={closeApp}>
		<div class="close-block-handler"></div>
	</div>
</div>