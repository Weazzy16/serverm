<script>
	import { createEventDispatcher, onMount } from 'svelte';
	import './css/call.css';
	import CreateContact from './CreateContactV2.svelte';
	import AddFavourite from './AddFavouriteV2.svelte';
	import { executeClientToGroup, executeClientAsyncToGroup, setGroup } from 'api/rage';
	import { validate } from 'api/validation';
	import { charSim } from 'store/chars';
	import { TimeFormat } from 'api/moment';
	
	export let cdn = '';
	
	const dispatch = createEventDispatcher();
	
	// Устанавливаем группу для API
	setGroup(".phone.");
	
	// Состояние приложения
	let page = 'recent';
	let inputSearch = '';
	let searchFocused = false;
	let recentFilter = 'all';
	let popupPage = null;
	let recentHeaderScrolled = false;
	let popupData = null;
	let number = '';
	let contactInfo = {};
	let showContactInfo = false;
	let activeCall = null;
	export let initialPage = 'recent';
	
	// Данные пользователя - получаем из системы
	let balance = 1000;
	let phoneNumber = $charSim;
	let accountId = $charSim;
	let login = 'Игрок';
	
	// Недавние звонки - загружаем из API
	let recentCalls = [];
	
	// Контакты - загружаем из API (используем структуру из старой системы)
	let contacts = {};
	let contactsData = [];
	let contactsSystemData = [];
	
	// Избранные
	let favourites = [];
	
	// Избранные
	
	// Панель управления
	const controlPanel = [
		{ type: 'favourites', title: 'Избранные' },
		{ type: 'recent', title: 'Недавние' },
		{ type: 'contacts', title: 'Контакты' },
		{ type: 'keys', title: 'Клавиши' }
	];
	
	// Клавиатура
	const keys = [
		{ symbol: 1, letters: '' },
		{ symbol: 2, letters: 'ABC', eLetters: 'АБВ' },
		{ symbol: 3, letters: 'DEF', eLetters: 'ГДЕ' },
		{ symbol: 4, letters: 'GHI', eLetters: 'ЖЗИ' },
		{ symbol: 5, letters: 'JKL', eLetters: 'КЛМ' },
		{ symbol: 6, letters: 'MNO', eLetters: 'НОП' },
		{ symbol: 7, letters: 'PQRS', eLetters: 'РСТ' },
		{ symbol: 8, letters: 'TUV', eLetters: 'УФХ' },
		{ symbol: 9, letters: 'WXYZ', eLetters: 'ЦЧШ' },
		{ symbol: '*', letters: '', isComponent: true },
		{ symbol: 0, letters: '+', labelComponent: true },
		{ symbol: '#', letters: '', isComponent: true }
	];

	onMount(() => {
		if (initialPage) {
			setPage(initialPage);
		}
		// Небольшая задержка перед загрузкой данных, чтобы группа API успела установиться
		setTimeout(() => {
			loadRecentCalls();
			loadContacts();
		}, 100);
	});

	// Загрузка недавних звонков - используем точно такой же код как в старом recent.svelte
	async function loadRecentCalls() {
		try {
			const result = await executeClientAsyncToGroup("getRecents");
			if (result && typeof result === "string") {
				const recentsData = JSON.parse(result);
				// Используем данные напрямую как в старом коде
				recentCalls = recentsData.map((recent, index) => ({
					id: index + 1,
					numberFrom: recent.isCall ? phoneNumber : recent.Number,
					numberTo: recent.isCall ? recent.Number : phoneNumber,
					accountTo: recent.isCall ? recent.Number : accountId,
					accountFrom: recent.isCall ? accountId : recent.Number,
					status: recent.isCall ? 'outgoing' : 'incoming',
					date: new Date(recent.time),
					duration: 0,
					name: recent.Name
				}));
				console.log('Recent calls loaded:', recentCalls.length);
			} else {
				console.warn('No recent calls data received');
				recentCalls = [];
			}
		} catch (error) {
			console.warn('Recent calls not available:', error);
			recentCalls = [];
		}
	}

	// Загрузка контактов - используем точно такой же код как в старом contacts.svelte
	async function loadContacts() {
		try {
			const result = await executeClientAsyncToGroup("getContacts");
			if (result && typeof result === "string") {
				const contactsResult = JSON.parse(result);
				
				// Сортируем контакты точно как в старом коде
				const ruCollator = new Intl.Collator('ru-RU');
				const sortedContacts = contactsResult.sort((a, b) => ruCollator.compare(a.Name, b.Name));
				
				// Разделяем системные и обычные контакты
				contactsData = sortedContacts.filter(el => !filterCheckSystem(el.List));
				contactsSystemData = sortedContacts.filter(el => filterCheckSystem(el.List));
				
				// Преобразуем в формат для нового интерфейса
				contacts = {};
				
				// Добавляем системные контакты
				const systemContacts = [];
				contactsSystemData.forEach(letterData => {
					letterData.List.forEach(contact => {
						if (!contact.IsNotShow) {
							systemContacts.push({
								id: contact.Number,
								phoneNumber: contact.Number,
								special: true,
								name: contact.Name,
								blocked: contact.IsBlackList || false,
								favourite: false
							});
						}
					});
				});
				if (systemContacts.length > 0) {
					contacts.services = systemContacts;
				}
				
				// Добавляем обычные контакты по буквам
				contactsData.forEach(letterData => {
					const letter = letterData.Name;
					contacts[letter] = letterData.List.map(contact => ({
						id: contact.Number,
						name: contact.Name,
						phoneNumber: contact.Number,
						special: false,
						blocked: contact.IsBlackList || false,
						favourite: false,
						avatar: contact.Avatar
					}));
				});
				
				console.log('Contacts loaded:', Object.keys(contacts));
			} else {
				console.warn('No contacts data received');
				// Дефолтные контакты если нет данных
				contacts = {
					services: [
						{ id: '911', phoneNumber: '911', special: true, name: 'Полиция', blocked: false, favourite: false },
						{ id: '112', phoneNumber: '112', special: true, name: 'Скорая помощь', blocked: false, favourite: false }
					]
				};
			}
		} catch (error) {
			console.warn('Contacts not available:', error);
			// Дефолтные контакты при ошибке
			contacts = {
				services: [
					{ id: '911', phoneNumber: '911', special: true, name: 'Полиция', blocked: false, favourite: false },
					{ id: '112', phoneNumber: '112', special: true, name: 'Скорая помощь', blocked: false, favourite: false },
					{ id: '555', phoneNumber: '555', special: true, name: 'Такси', blocked: false, favourite: false }
				]
			};
		}
	}

	// Вспомогательная функция для проверки системных контактов
	const filterCheckSystem = (data) => {
		let success = false;
		data.forEach(el => {
			success = el.IsSystem;
		});
		return success;
	};
	
	// Computed properties
	$: filteredRecentCalls = recentFilter === 'all' ? recentCalls : 
		recentCalls.filter(call => call.status === 'missed' && Number(call.numberTo) === Number(phoneNumber));
	
	$: isHeaderActive = !['contactInfo', 'createContact', 'keys'].includes(page);
	
	$: getPageCaption = () => {
		switch (page) {
			case 'recent': return 'Недавние';
			case 'favourites': return 'Избранные';
			case 'contacts': return 'Контакты';
			default: return '';
		}
	};
	
	$: filterActive = page === 'recent';
	
	$: computedContactsData = searchFocused ? ['search'] : Object.keys(contacts).sort();
	
	// Основные функции
	function setPage(pageData) {
		recentHeaderScrolled = false;
		contactInfo = {};
		page = pageData.page || pageData;
	}
	
	function setPopup(popupType, data = null) {
		popupPage = popupType;
		if (data) {
			popupData = data;
		}
	}
	
	function closeApp() {
		dispatch('close');
		// Сбрасываем состояние точно как в старом коде
		openedApp = null;
		cameraOpened = false;
		phoneInitialPage = 'recent'; // Сбрасываем на дефолт
		barColors.appBar = false;
	}
	
	function closingStart() {
		console.log('Closing start');
	}
	
	// Функции для недавних звонков
	function getTitle(call) {
		if (call.name) {
			return call.name;
		}
		const contact = getContact(call);
		if (contact && contact.name) {
			return contact.name;
		}
		const targetNumber = accountId === call.accountTo ? call.numberFrom : call.numberTo;
		return targetNumber;
	}
	
	function getContact(call) {
		const targetNumber = accountId === call.accountTo ? call.numberFrom : call.numberTo;
		const allContacts = Object.values(contacts).flat();
		return allContacts.find(c => c.phoneNumber === targetNumber);
	}
	
	function getType(call) {
		if (accountId === call.accountTo && call.status === 'missed') {
			return 'missed';
		}
		return accountId === call.accountTo ? 'incoming' : 'outgoing';
	}
	
	function getDate(call) {
		return TimeFormat(call.date, "H:mm");
	}
	
	function call(callData) {
		const targetNumber = accountId === callData.accountTo ? callData.numberFrom : callData.numberTo;
		
		// Проверяем валидность номера
		let check = validate("phonenumber", Number(targetNumber));
		if (!check.valid) {
			window.notificationAdd(4, 9, check.text, 3000);
			return;
		}
		
		try {
			executeClientToGroup("call", Number(targetNumber));
			console.log('Calling:', targetNumber);
		} catch (error) {
			console.error('Error making call:', error);
			window.notificationAdd(4, 9, 'Ошибка при совершении звонка', 3000);
		}
	}
	
	function deleteRecent(call) {
		recentCalls = recentCalls.filter(c => c.id !== call.id);
	}
	
	function addToContact(call) {
		const targetNumber = accountId === call.accountTo ? call.numberFrom : call.numberTo;
		setPopup('new-contact', { phoneNumber: targetNumber });
	}
	
	// Функции для контактов
	function setContactInfo(contact) {
		contactInfo = contact;
		showContactInfo = true;
	}
	
	function getContactsList(letter) {
		if (letter === 'search') {
			if (!inputSearch.length) return [];
			const allContacts = Object.values(contacts).flat();
			return allContacts.filter(contact => 
				contact.name?.toLowerCase().includes(inputSearch.toLowerCase()) ||
				contact.phoneNumber.includes(inputSearch)
			);
		}
		return contacts[letter] || [];
	}
	
	function getCaption(letter) {
		switch (letter) {
			case 'services': return 'Службы';
			case 'search': return 'Поиск';
			default: return letter;
		}
	}
	
	// Функции для клавиатуры
	function pressKey(key) {
		if (number && number.length >= 9) return;
		
		let symbol = key.symbol;
		
		if (!number) {
			number = symbol.toString();
		} else {
			number = `${number}${number.length === 4 ? '-' : ''}${symbol}`;
		}
	}
	
	function clearNumber() {
		const numStr = number.toString();
		number = numStr.length - 2 === 4 ? numStr.slice(0, -2) : numStr.slice(0, -1);
	}
	
	function makeCall() {
		if (!number.length) return;
		
		const cleanNumber = number.replace(/[^\d]/g, '');
		
		if (cleanNumber === phoneNumber.toString()) {
			console.log('Cannot call yourself');
			return;
		}
		
		let check = validate("phonenumber", Number(cleanNumber));
		if (!check.valid) {
			window.notificationAdd(4, 9, check.text, 3000);
			return;
		}
		
		executeClientToGroup("call", Number(cleanNumber));
		console.log('Calling:', cleanNumber);
		number = '';
	}
	
	// Функции для звонков
	function endCall() {
		activeCall = null;
	}
	
	function answerCall() {
		if (activeCall) {
			activeCall.active = true;
			activeCall.status = 'active';
		}
	}
	
	function addNumber() {
		setPopup('new-contact', { phoneNumber: number.replace(/[^\d]/g, '') });
	}
	
	function onlyNumbers(event) {
		const input = event.target;
		const value = input.value;
		const cleaned = value.replace(/[^0-9#*]/g, '');
		const limited = cleaned.substring(0, 8);
		input.value = limited;
		number = limited;
	}
	
	// Функции для избранных
	function selectFavourite(favourite) {
		if (Object.keys(favourite).length) {
			const contact = Object.values(contacts).flat().find(c => c.phoneNumber === favourite.phoneNumber);
			setContactInfo(contact || {});
		} else {
			setContactInfo({});
		}
	}
	
	function deleteFavourite(favourite) {
		favourites = favourites.filter(f => f.phoneNumber !== favourite.phoneNumber);
	}
	
	// Обработчики событий от дочерних компонентов
	async function handleSaveContact(event) {
		const { name, phone } = event.detail;
		
		try {
			let check = validate("phonename", name);
			if (!check.valid) {
				window.notificationAdd(4, 9, check.text, 3000);
				return;
			}

			const number = Number(phone);
			check = validate("phonenumber", number);
			if (!check.valid) {
				window.notificationAdd(4, 9, check.text, 3000);
				return;
			}

			await executeClientToGroup("addContact", number, name);
			
			// Перезагружаем контакты
			await loadContacts();
			
			setPopup(null);
		} catch (error) {
			console.error('Error saving contact:', error);
		}
	}
	
	function handleSelectContact(event) {
		const contact = event.detail;
		favourites = [...favourites, {
			phoneNumber: contact.phoneNumber,
			nickname: contact.name,
			avatar: contact.avatar || cdn + '/img/avatars/male.svg'
		}];
		setPopup(null);
	}
	
	// Утилиты
	function unescapeString(str) {
		if (!str) return '';
		return str.replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
	}
	
	function sliceString(str, maxLength) {
		return str && str.length > maxLength ? str.slice(0, maxLength) + '...' : str;
	}
</script>

<!-- svelte-ignore a11y-click-events-have-key-events -->
<div class="recent-main full-width full-height column-block">
	<!-- Основной контейнер скрывается при показе информации о контакте или активном звонке -->
	{#if !showContactInfo && !activeCall}
	<div class="expandable-containercall full-width full-height column-block" class:scaled={popupPage}>
		
		<!-- Header -->
		{#if isHeaderActive}
		<div class="recent-header full-width column-block align-end {page}" 
			 class:search-focused={searchFocused}
			 class:search-entered={inputSearch.length}
			 class:recent-scrolled={recentHeaderScrolled}>
			
			<div class="add-contact" on:click={() => setPopup(page === 'contacts' ? 'new-contact' : 'add-favourite')}>
				<svg viewBox="0 0 24 24" class="add-contact__image">
					<path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"/>
				</svg>
			</div>
			
			<div class="recent-caption column-block full-width">
				{#if filterActive}
				<div class="filter-containercall full-width row-block justify-center">
					<div class="filters row-block align-center selected-{recentFilter}">
						<div class="filters__button" on:click={() => recentFilter = 'all'}>
							<span>Все</span>
						</div>
						<div class="filters__button" on:click={() => recentFilter = 'missed'}>
							<span>Проп.</span>
						</div>
					</div>
				</div>
				{/if}
				<div class="recent-caption__text-caption">{getPageCaption()}</div>
			</div>
			
			{#if page === 'contacts'}
			<div class="input-containercall align-center full-width">
				<div class="search-block align-center full-width row-block">
					<svg class="search-block__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="13" height="13" viewBox="0 0 13 13">
						<g transform="translate(-9 -9)">
							<path d="M12.585,11.841,9.491,8.623A5.247,5.247,0,1,0,5.473,10.5a5.193,5.193,0,0,0,3.007-.95L11.6,12.79a.685.685,0,1,0,.987-.949ZM5.473,1.369a3.88,3.88,0,1,1-3.88,3.88A3.884,3.884,0,0,1,5.473,1.369Z" transform="translate(9 9)"/>
						</g>
					</svg>
					<input 
						type="text"
						bind:value={inputSearch}
						on:focus={() => searchFocused = true}
						maxlength="30"
						placeholder="Поиск"
						class="search-block__input"
					/>
					<div class="reset-button" on:click={() => inputSearch = ''}>
						<svg class="reset-button" on:click={() => inputSearch = ''} xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15">
							<path d="M7.5,15A7.5,7.5,0,1,0,0,7.5,7.5,7.5,0,0,0,7.5,15ZM4.705,5.295,6.911,7.5,4.705,9.705l.589.589L7.5,8.089l2.205,2.205.589-.589L8.089,7.5l2.205-2.205-.589-.589L7.5,6.911,5.295,4.705Z" fill-rule="evenodd"/>
						</svg>
					</div>
				</div>
				<div class="discard-button" on:click={() => { searchFocused = false; inputSearch = ''; }}>
					Отменить
				</div>
			</div>
			{/if}
			
			<div class="line"></div>
		</div>
		{/if}
		
		<!-- Recent calls -->
		{#if page === 'recent'}
		<div class="recent-block full-width column-block column-block align-end" 
			 class:search-focused={searchFocused}>
			<div class="recent-block__title full-width">Недавние</div>
			<div class="line"></div>
			
			{#each filteredRecentCalls as call (call.id)}
			<div class="item-block full-width column-block" on:click={() => call(call)}>
				<div class="item-containercall row-block full-width">
					<div class="call-type full-height">
						{#if getType(call) === 'outgoing'}
						<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="10" height="10" viewBox="0 0 10 10">
							<defs>
								<clipPath id="clip-path">
									<rect id="Rectangle_238" data-name="Rectangle 238" width="10" height="10" transform="translate(-1955 220)"/>
								</clipPath>
								<clipPath id="clip-path-2">
									<rect id="Rectangle_237" data-name="Rectangle 237" width="10" height="10"/>
								</clipPath>
							</defs>
							<g id="Mask_Group_39" data-name="Mask Group 39" transform="translate(1955 -220)" opacity="0.25" clip-path="url(#clip-path)">
								<g id="Group_110" data-name="Group 110" transform="translate(-1955 220)" clip-path="url(#clip-path-2)">
									<path id="Path_94" data-name="Path 94" d="M1.787.031A6.54,6.54,0,0,0,1.04.769,6.748,6.748,0,0,0,.282,1.945c-.224.449-.3.649-.279.753A10.9,10.9,0,0,0,.634,3.94,16.192,16.192,0,0,0,6.847,9.8c.47.234.49.238.75.134a6.178,6.178,0,0,0,2.178-1.5c.237-.268.249-.3.2-.476A4.319,4.319,0,0,0,8.907,6.349a5.168,5.168,0,0,0-1.316-.96c-.278-.139-.389-.155-.5-.074a2.041,2.041,0,0,0-.247.336c-.2.313-.489.736-.592.859h0a.1.1,0,0,1-.139.012l-.156-.13A18.951,18.951,0,0,1,3.582,4.006a1.577,1.577,0,0,1-.158-.212c.081-.075.572-.416.92-.638A2.056,2.056,0,0,0,4.687,2.9c.073-.111.054-.242-.073-.491a5.1,5.1,0,0,0-.474-.767A4.672,4.672,0,0,0,2.16.068C1.922-.012,1.866-.018,1.787.031Z" transform="translate(0 0)" fill-rule="evenodd"/>
									<path id="Path_95" data-name="Path 95" d="M10.71,2.248a.489.489,0,0,0-.223.267.385.385,0,0,0,.17.423c.068.047.094.05.5.06l.409.01h0a.008.008,0,0,1,.006.014l-.66.661-.4.4a.394.394,0,0,0,0,.552h0a.391.391,0,0,0,.556.008l.253-.251.815-.814a.012.012,0,0,1,.021.009h0v.39c0,.4,0,.422.051.506a.394.394,0,0,0,.616.072c.11-.118.112-.137.11-1.107a7.7,7.7,0,0,0-.026-.97.455.455,0,0,0-.218-.229A15.843,15.843,0,0,0,10.71,2.248Z" transform="translate(-4.28 -0.913)"/>
								</g>
							</g>
						</svg>
						{/if}
					</div>
					<div class="text-data row-block full-width full-height align-center justify-between">
						<div class="column-block">
							<div class="item-containercall__text-contact" class:missed={getType(call) === 'missed'}>
								{sliceString(unescapeString(getTitle(call)), 13)}
							</div>
							<div class="item-containercall__text-device">iPhone</div>
						</div>
						<div class="row-block align-center">
							<div class="call-time">{getDate(call)}</div>
							{#if !getContact(call)}
							<div class="btn add" on:click|stopPropagation={() => addToContact(call)}>
								<svg class="btn__picture" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14" fill="none">
									<path d="M7 14C6.04251 14 5.14094 13.8165 4.2953 13.4494C3.44966 13.0868 2.7047 12.5832 2.0604 11.9386C1.41611 11.294 0.910515 10.5509 0.543624 9.70935C0.181208 8.86331 0 7.95907 0 6.99664C0 6.03869 0.181208 5.13893 0.543624 4.29736C0.910515 3.45132 1.41387 2.706 2.05369 2.06139C2.69799 1.41679 3.44295 0.913189 4.28859 0.5506C5.13423 0.183533 6.03579 0 6.99329 0C7.95078 0 8.85235 0.183533 9.69799 0.5506C10.5481 0.913189 11.2931 1.41679 11.9329 2.06139C12.5772 2.706 13.0828 3.45132 13.4497 4.29736C13.8166 5.13893 14 6.03869 14 6.99664C14 7.95907 13.8166 8.86331 13.4497 9.70935C13.0828 10.5509 12.5772 11.294 11.9329 11.9386C11.2931 12.5832 10.5503 13.0868 9.7047 13.4494C8.85906 13.8165 7.95749 14 7 14ZM3.71812 7.00336C3.71812 7.20927 3.78523 7.37938 3.91946 7.51367C4.05369 7.64349 4.22595 7.70839 4.43624 7.70839H6.28859V9.57506C6.28859 9.78098 6.3557 9.95108 6.48993 10.0854C6.62416 10.2197 6.79418 10.2868 7 10.2868C7.20582 10.2868 7.37584 10.2197 7.51007 10.0854C7.64877 9.95108 7.71812 9.78098 7.71812 9.57506V7.70839H9.57718C9.783 7.70839 9.95302 7.64349 10.0872 7.51367C10.226 7.37938 10.2953 7.20927 10.2953 7.00336C10.2953 6.79297 10.2282 6.62062 10.094 6.48633C9.95973 6.35204 9.78747 6.28489 9.57718 6.28489H7.71812V4.42494C7.71812 4.21455 7.64877 4.04221 7.51007 3.90791C7.37584 3.77362 7.20582 3.70647 7 3.70647C6.79418 3.70647 6.62416 3.77362 6.48993 3.90791C6.3557 4.04221 6.28859 4.21455 6.28859 4.42494V6.28489H4.43624C4.22595 6.28489 4.05369 6.35204 3.91946 6.48633C3.78523 6.62062 3.71812 6.79297 3.71812 7.00336Z"/>
								</svg>
							</div>
							{/if}
							<div class="btn delete" on:click|stopPropagation={() => deleteRecent(call)}>
								<svg class="btn__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="12" height="12" viewBox="0 0 12 12">
									<g transform="translate(-268 -279)">
										<path d="M5.928.7H7.467a2.072,2.072,0,0,1,.761.087,1.256,1.256,0,0,1,.5.363,2.072,2.072,0,0,1,.323.694l.019.058.157.472h3.047a.419.419,0,1,1,0,.837h-.722L11.22,8.941v.018h0c-.034.577-.061,1.034-.112,1.4a2.714,2.714,0,0,1-.3.984,2.651,2.651,0,0,1-1.149,1.083,2.714,2.714,0,0,1-1,.239c-.371.03-.829.03-1.407.03H6.141c-.578,0-1.036,0-1.407-.03a2.714,2.714,0,0,1-1-.239,2.651,2.651,0,0,1-1.149-1.083,2.713,2.713,0,0,1-.3-.984c-.051-.369-.078-.826-.112-1.4V8.941L1.838,3.209H1.116a.419.419,0,1,1,0-.837H4.163L4.321,1.9l.019-.058a2.072,2.072,0,0,1,.323-.694,1.256,1.256,0,0,1,.5-.363A2.072,2.072,0,0,1,5.928.7ZM8.28,2.165l.069.208h-3.3l.069-.208a1.72,1.72,0,0,1,.19-.48.419.419,0,0,1,.168-.121,1.72,1.72,0,0,1,.515-.029H7.407a1.72,1.72,0,0,1,.515.029.419.419,0,0,1,.168.121A1.719,1.719,0,0,1,8.28,2.165Zm-2.7,3A.419.419,0,0,1,6,5.581V9.488a.419.419,0,0,1-.837,0V5.581A.419.419,0,0,1,5.581,5.163Zm2.233,0a.419.419,0,0,1,.419.419V9.488a.419.419,0,0,1-.837,0V5.581A.419.419,0,0,1,7.814,5.163Z" transform="translate(267.302 278.302)" fill-rule="evenodd"/>
									</g>
								</svg>
							</div>
						</div>
					</div>
				</div>
				<div class="divider"></div>
			</div>
			{/each}
		</div>
		{/if}
		
		<!-- Favourites -->
		{#if page === 'favourites'}
		<div class="favourite-block full-width column-block">
			{#each favourites as favourite (favourite.phoneNumber)}
			<div class="item-block full-width column-block" on:click={() => selectFavourite(favourite)}>
				<div class="item-containercall row-block align-center full-width full-height">
					<div class="item-containercall__picture-avatar" style="background-image: url('{favourite.avatar}')"></div>
					<div class="row-block align-center justify-between full-width">
						<div class="column-block">
							<div class="item-containercall__text-nickname">{sliceString(favourite.nickname, 14)}</div>
							<div class="item-containercall__text-device">iPhone</div>
						</div>
					</div>
					<div class="delete" on:click|stopPropagation={() => deleteFavourite(favourite)}>
                        <svg class="delete__icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="12" height="12" viewBox="0 0 12 12">
							<g transform="translate(-268 -279)">
								<path d="M5.928.7H7.467a2.072,2.072,0,0,1,.761.087,1.256,1.256,0,0,1,.5.363,2.072,2.072,0,0,1,.323.694l.019.058.157.472h3.047a.419.419,0,1,1,0,.837h-.722L11.22,8.941v.018h0c-.034.577-.061,1.034-.112,1.4a2.714,2.714,0,0,1-.3.984,2.651,2.651,0,0,1-1.149,1.083,2.714,2.714,0,0,1-1,.239c-.371.03-.829.03-1.407.03H6.141c-.578,0-1.036,0-1.407-.03a2.714,2.714,0,0,1-1-.239,2.651,2.651,0,0,1-1.149-1.083,2.713,2.713,0,0,1-.3-.984c-.051-.369-.078-.826-.112-1.4V8.941L1.838,3.209H1.116a.419.419,0,1,1,0-.837H4.163L4.321,1.9l.019-.058a2.072,2.072,0,0,1,.323-.694,1.256,1.256,0,0,1,.5-.363A2.072,2.072,0,0,1,5.928.7ZM8.28,2.165l.069.208h-3.3l.069-.208a1.72,1.72,0,0,1,.19-.48.419.419,0,0,1,.168-.121,1.72,1.72,0,0,1,.515-.029H7.407a1.72,1.72,0,0,1,.515.029.419.419,0,0,1,.168.121A1.719,1.719,0,0,1,8.28,2.165Zm-2.7,3A.419.419,0,0,1,6,5.581V9.488a.419.419,0,0,1-.837,0V5.581A.419.419,0,0,1,5.581,5.163Zm2.233,0a.419.419,0,0,1,.419.419V9.488a.419.419,0,0,1-.837,0V5.581A.419.419,0,0,1,7.814,5.163Z" transform="translate(267.302 278.302)" fill-rule="evenodd"/>
							</g>
						</svg>
					</div>
				</div>
				<div class="divider"></div>
			</div>
			{/each}
		</div>
		{/if}
		
		<!-- Contacts -->
		{#if page === 'contacts'}
		<div class="contacts-block column-block full-width full-height">
			{#if !searchFocused}
			<div class="user-block row-block align-center full-width">
				<div class="user-block__picture-avatar"></div>
				<div class="column-block">
					<div class="user-block__title">{login}</div>
					<div class="phone-number">
						<span class="phone-number__title">Мой номер:</span>
						<span class="phone-number__value">{phoneNumber == -1 ? "Нет сим-карты" : phoneNumber}</span>
					</div>
				</div>
			</div>
			{/if}
			
			{#each computedContactsData as letter}
				{#if contacts[letter] && getContactsList(letter).length}
				<div class="contact-containercall column-block">
					<div class="contact-containercall__text-letter full-width align-center">
						{getCaption(letter)}
					</div>
					<div class="contacts-content full-width">
						{#each getContactsList(letter) as contact}
						<div class="contact-block full-width" on:click={() => setContactInfo(contact)}>
							<div>
								{#if contact.special}
								<div class="contact-block__text align-center full-width">
									{contact.name || contact.phoneNumber}
								</div>
								{:else}
								<div class="contact-block__text align-center full-width">
									{unescapeString(contact.name) || contact.phoneNumber}
								</div>
								{/if}
								<div class="divider"></div>
							</div>
						</div>
						{/each}
					</div>
				</div>
				{/if}
			{/each}
			
			{#if searchFocused && inputSearch.length && !getContactsList('search').length}
			<div class="not-found full-width full-height column-block">
                <svg class="not-found__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="13" height="13" viewBox="0 0 13 13">
					<g transform="translate(-9 -9)">
						<path d="M12.585,11.841,9.491,8.623A5.247,5.247,0,1,0,5.473,10.5a5.193,5.193,0,0,0,3.007-.95L11.6,12.79a.685.685,0,1,0,.987-.949ZM5.473,1.369a3.88,3.88,0,1,1-3.88,3.88A3.884,3.884,0,0,1,5.473,1.369Z" transform="translate(9 9)"/>
					</g>
				</svg>
				<span class="not-found__title">
					Нет результатов <br/> «{inputSearch}»
				</span>
				<span class="not-found__text">Проверьте запрос</span>
			</div>
			{/if}
		</div>
		{/if}
		
		<!-- Keys (Keypad) -->
		{#if page === 'keys'}
		<div class="main-keys full-height full-width column-block align-center">
			<div class="number-field column-block full-width">
				<input 
					class="number-field__text-number justify-center"
					type="text"
					bind:value={number}
					on:input={onlyNumbers}
					maxlength="8"
				/>
				{#if number}
				<div class="number-field__text-add full-width justify-center align-center" on:click={addNumber}>
					Добавить номер
				</div>
				{/if}
			</div>
			
			<div class="keys-row">
				{#each keys as key}
				<div class="key-block-containercall">
					<div class="key-block column-block align-center" 
						 class:centered={key.isComponent}
						 on:click={() => pressKey(key)}>
						{#if key.symbol === '*'}
						<svg class="key-block__icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="20" height="20" viewBox="0 0 20 20">
							<g transform="translate(-1551 1548)">
								<path d="M9.074,2l.2,8h1.521l.2-8ZM9.041,18h1.917l-.2-8H9.24ZM2,6.7,9.636,10.54,10.4,9.46,2.992,5.3Zm16,0-.959-1.4L9.636,9.46l.727,1.079Zm0,6.6L10.364,9.429,9.636,10.54l7.4,4.159ZM2,13.333l.992,1.4,7.4-4.159L9.636,9.46Z" transform="translate(1551 -1548)"/>
							</g>
						</svg>
						{:else if key.symbol === '#'}
						<svg class="key-block__icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="20" height="20" viewBox="0 0 20 20">
							<g transform="translate(-1495 1546)">
								<path d="M6.918,20l1.132-6.012H8l1.234-6.4h.051L10.353,2H8.5L5.061,20Zm6.132,0L16.485,2H14.628L13.547,7.588H13.6l-1.234,6.4h-.051L11.193,20Zm4.16-4.877L17.58,13.3H13.292v.05H7.147V13.3H3.369L3,15.123Zm1.17-6.6L18.75,6.7H4.527L4.158,8.524H8.114l.013-.05h6.107v.05Z" transform="translate(1494.125 -1547)"/>
							</g>
						</svg>
						{:else if key.symbol === 0 && key.labelComponent}
						<span class="key-block__title">{key.symbol}</span>
						<svg class="key-block__subicon" xmlns="http://www.w3.org/2000/svg" width="9" height="9" viewBox="0 0 9 9">
							<path d="M7.446,1.081H3.7V5H2.2V1.081h-3.75V-.208H2.2V-4H3.7V-.208h3.75Z" transform="translate(1.554 3.997)"/>
						</svg>
						{:else}
						<span class="key-block__title">{key.symbol}</span>
						{/if}
						
						{#if key.letters && !key.labelComponent && !key.isComponent}
						<div class="letters-block full-width justify-center">{key.letters}</div>
						{/if}
						
						{#if key.eLetters && !key.isComponent}
						<div class="letters-block full-width justify-center">{key.eLetters}</div>
						{/if}
					</div>
				</div>
				{/each}
			</div>
			
			<div class="control-buttons row-block full-width justify-center align-center">
				<div class="call-button">
					<div class="key-block" on:click={makeCall}>
						<svg class="key-block__icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="20" height="20" viewBox="0 0 20 20">
							<path d="M9.6,12.3A16.386,16.386,0,0,1,6.135,8.16l-.449-.847c.157-.168,1.354-1.451,1.873-2.146C8.211,4.294,7.266,3.5,7.266,3.5S4.606.844,4,.316A1.305,1.305,0,0,0,2.7.081C1.421.9.1,1.62.021,5.063c0,3.223,2.444,6.547,5.089,9.121C7.76,17.09,11.4,20,14.917,20c3.442-.079,4.158-1.4,4.981-2.674a1.306,1.306,0,0,0-.234-1.3c-.528-.607-3.189-3.267-3.189-3.267s-.788-.946-1.662-.293c-.651.487-1.82,1.569-2.1,1.832A23.323,23.323,0,0,1,9.6,12.3Z"/>
						</svg>
					</div>
				</div>
				<div class="clear-button" on:click={clearNumber}>
					<svg class="clear-button__picture" xmlns="http://www.w3.org/2000/svg" width="23.949" height="17.744" viewBox="0 0 23.949 17.744">
						<g transform="translate(311.63 -18.088)">
							<path class="clear-path" d="M8.034.3A2.6,2.6,0,0,0,7.5.685L.834,6.824a2.588,2.588,0,0,0-.039,3.769l6.692,6.43a2.587,2.587,0,0,0,1.793.722H21.361a2.588,2.588,0,0,0,2.588-2.588V2.588A2.588,2.588,0,0,0,21.361,0H9.249A2.587,2.587,0,0,0,8.034.3Zm2.635,4.115a.555.555,0,0,0-.793.775l3.6,3.679-3.6,3.679a.555.555,0,0,0,.793.775l3.577-3.661,3.577,3.661a.555.555,0,0,0,.793-.775l-3.6-3.679,3.6-3.679a.555.555,0,0,0-.793-.775L14.245,8.078Z" transform="translate(-311.63 18.088)" fill-rule="evenodd"/>
							<path d="M27.237,11.667a.555.555,0,0,0-.793.775l3.6,3.679-3.6,3.679a.555.555,0,0,0,.793.775l3.577-3.661,3.577,3.661a.555.555,0,0,0,.793-.775l-3.6-3.679,3.6-3.679a.555.555,0,0,0-.793-.775l-3.577,3.661Z" transform="translate(-328.199 10.839)"/>
						</g>
					</svg>
				</div>
			</div>
		</div>
		{/if}
		
		<!-- Control Panel -->
		<div class="recent-control full-width row-block {page}">
			{#each controlPanel as panel}
			<div class="control-block column-block align-center" 
				 class:selected={panel.type === page}
				 on:click={() => setPage(panel.type)}>
                <svg class="control-block__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="18" height="18" viewBox="0 0 18 18">
					{#if panel.type === 'favourites'}
					<path xmlns="http://www.w3.org/2000/svg" d="M8.269.556a.758.758,0,0,1,1.461,0l1.6,5.148a.772.772,0,0,0,.731.556H17.23a.811.811,0,0,1,.451,1.455L13.5,10.9a.826.826,0,0,0-.279.9l1.6,5.148a.778.778,0,0,1-1.182.9L9.452,14.661a.741.741,0,0,0-.9,0L4.365,17.843a.778.778,0,0,1-1.182-.9l1.6-5.148a.826.826,0,0,0-.279-.9L.318,7.714A.811.811,0,0,1,.77,6.26H5.941A.773.773,0,0,0,6.671,5.7Z"/>					
					{:else if panel.type === 'recent'}
					<path xmlns="http://www.w3.org/2000/svg" d="M9,18A9,9,0,1,0,0,9,9,9,0,0,0,9,18ZM9.63,3.87V9.09q0,.023,0,.045t0,.045A.63.63,0,0,1,9,9.81H5.22a.63.63,0,0,1,0-1.26H8.37V3.87a.63.63,0,0,1,1.26,0Z" fill-rule="evenodd"/>					
					{:else if panel.type === 'contacts'}
					<path xmlns="http://www.w3.org/2000/svg" d="M18,9A9,9,0,1,1,9,0,9,9,0,0,1,18,9ZM11.88,7.2A2.974,2.974,0,0,1,9,10.26,2.974,2.974,0,0,1,6.12,7.2,2.974,2.974,0,0,1,9,4.14,2.974,2.974,0,0,1,11.88,7.2ZM8.91,11.52a7.091,7.091,0,0,0-5.247,2.312,7.2,7.2,0,0,0,10.584.1A7.093,7.093,0,0,0,8.91,11.52Z" fill-rule="evenodd"/>
					{:else if panel.type === 'keys'}
					<path d="M3.33,5.76A2.43,2.43,0,1,0,.9,3.33,2.43,2.43,0,0,0,3.33,5.76Z"/>
					<path d="M9,5.76A2.43,2.43,0,1,0,6.57,3.33,2.43,2.43,0,0,0,9,5.76Z"/>
					<path d="M17.1,3.33A2.43,2.43,0,1,1,14.67.9,2.43,2.43,0,0,1,17.1,3.33Z"/>
					<path d="M11.43,9A2.43,2.43,0,1,1,9,6.57,2.43,2.43,0,0,1,11.43,9Z"/>
					<path d="M3.33,11.43A2.43,2.43,0,1,0,.9,9,2.43,2.43,0,0,0,3.33,11.43Z"/>
					<path d="M5.76,14.67a2.43,2.43,0,1,1-2.43-2.43A2.43,2.43,0,0,1,5.76,14.67Z"/>
					<path d="M9,17.1a2.43,2.43,0,1,0-2.43-2.43A2.43,2.43,0,0,0,9,17.1Z"/>
					<path d="M17.1,14.67a2.43,2.43,0,1,1-2.43-2.43A2.43,2.43,0,0,1,17.1,14.67Z"/>
					<path d="M14.67,11.43A2.43,2.43,0,1,0,12.24,9,2.43,2.43,0,0,0,14.67,11.43Z"/> 
					{/if}
				</svg>
				<div class="control-block__text">{panel.title}</div>
			</div>
			{/each}
		</div>
		
	</div>
	{/if}
	
	<!-- Contact Info -->
	{#if showContactInfo && Object.keys(contactInfo).length}
	<div class="contact full-width full-height column-block">
		<div class="contact-header row-block full-width justify-between align-center">
			<div class="header-button row-block align-center" on:click={() => { showContactInfo = false; contactInfo = {}; }}>
				<svg viewBox="0 0 24 24" class="header-button__picture">
					<path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z"/>
				</svg>
				<span class="header-button__title">Назад</span>
			</div>
			<div class="header-button align-center">
				<div class="header-button__title">Править</div>
			</div>
		</div>
		
		<div class="contact-info column-block align-center">
			<div class="contact-info-picture slogan">
				{contactInfo.name ? contactInfo.name.slice(0, 2).toUpperCase() : '??'}
			</div>
			<span class="contact-info__title">{contactInfo.name || contactInfo.phoneNumber}</span>
		</div>
		
		<div class="control-buttons row-block full-width justify-between">
			<div class="control-button column-block align-center full-width" on:click={() => {
				const targetNumber = Number(contactInfo.phoneNumber);
				let check = validate("phonenumber", targetNumber);
				if (!check.valid) {
					window.notificationAdd(4, 9, check.text, 3000);
					return;
				}
				executeClientToGroup("call", targetNumber);
			}}>
				<svg class="control-button__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="20" height="20" viewBox="0 0 20 20">
					<path d="M2.943.045C2.531.134,2.451.2,1.374,1.271.434,2.208.361,2.289.251,2.522A2.369,2.369,0,0,0,0,3.659,4.634,4.634,0,0,0,.2,5.2C1.2,9,5.686,14,9.982,16.1a7.194,7.194,0,0,0,3.185.9,2.476,2.476,0,0,0,1.277-.255c.259-.127.327-.188,1.263-1.12,1.047-1.043,1.11-1.122,1.242-1.564a1.984,1.984,0,0,0,0-.9c-.119-.4-.168-.459-1.493-1.788-.7-.7-1.361-1.339-1.471-1.417a1.572,1.572,0,0,0-2.065.158,1.43,1.43,0,0,0-.348.538,3.1,3.1,0,0,1-.232.45,1.708,1.708,0,0,1-1.457.649A5.885,5.885,0,0,1,6.952,9.981c-1.194-1.191-1.9-2.562-1.726-3.36a1.66,1.66,0,0,1,.651-.968,2.631,2.631,0,0,1,.354-.184,1.769,1.769,0,0,0,.592-.335A1.62,1.62,0,0,0,7.1,3.1C6.977,2.915,4.677.6,4.4.382a1.922,1.922,0,0,0-.781-.35A1.515,1.515,0,0,0,2.943.045Z" transform="translate(2 2)" fill-rule="evenodd"/>
				</svg>
				<span class="control-button__title">Вызов</span>
			</div>
			<div class="control-button column-block align-center full-width" on:click={() => executeClientToGroup("messageDefault", Number(contactInfo.phoneNumber))}>
				<svg class="control-button__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="20" height="20" viewBox="0 0 20 20">
					<path d="M8.965,1.022c-.086.007-.409.049-.717.093A9.7,9.7,0,0,0,.8,6.269a6.983,6.983,0,0,0,.953,7.888,8.881,8.881,0,0,0,1.4,1.258c.364.256.35.2.208.748a3.894,3.894,0,0,1-.476,1.028,10.869,10.869,0,0,1-.747.817l-.629.64,1.114-.017a5.155,5.155,0,0,0,1.5-.118,5.353,5.353,0,0,0,2.314-1.235l.243-.212.6.12a10.742,10.742,0,0,0,2.785.259,12.161,12.161,0,0,0,1.739-.071,10.357,10.357,0,0,0,5.394-2.216A7.677,7.677,0,0,0,19.242,5.9a8.273,8.273,0,0,0-3.907-3.809,11.289,11.289,0,0,0-3.861-1.04A22.7,22.7,0,0,0,8.965,1.022Z" transform="translate(0 0.176)" fill-rule="evenodd"/>
				</svg>
				<span class="control-button__title">Написать</span>
			</div>
		</div>
		
		<div class="number-containercall column-block full-width">
			<span class="number-containercall__title">сотовый</span>
			<span class="number-containercall__value">{contactInfo.phoneNumber}</span>
		</div>
		
		<!-- Contact Action Buttons -->
		<div class="contact-actions column-block full-width">
			{#if !contactInfo.favourite}
			<div class="button-containercall blue" on:click={() => {
				// Добавить в избранное
				favourites = [...favourites, {
					phoneNumber: contactInfo.phoneNumber,
					nickname: contactInfo.name,
					avatar: contactInfo.avatar || cdn + '/img/avatars/male.svg'
				}];
				contactInfo.favourite = true;
			}}>
				Добавить в избранное
			</div>
			{:else}
			<div class="button-containercall blue" on:click={() => {
				// Удалить из избранного
				favourites = favourites.filter(f => f.phoneNumber !== contactInfo.phoneNumber);
				contactInfo.favourite = false;
			}}>
				Удалить из избранного
			</div>
			{/if}
			
			<div class="button-containercall blue">
				Поделиться геопозицией
			</div>
			
			<div class="button-containercall blue" on:click={async () => {
				// Удалить контакт
				try {
					const result = await executeClientAsyncToGroup("dellContact", Number(contactInfo.phoneNumber));
					if (result) {
						showContactInfo = false;
						contactInfo = {};
						// Перезагружаем контакты
						loadContacts();
					}
				} catch (error) {
					console.error('Error deleting contact:', error);
				}
			}}>
				Удалить контакт
			</div>
			
			{#if !contactInfo.blocked}
			<div class="button-containercall red" on:click={async () => {
				// Заблокировать контакт
				try {
					const result = await executeClientAsyncToGroup("addBlackList", Number(contactInfo.phoneNumber));
					if (result) {
						contactInfo.blocked = true;
					}
				} catch (error) {
					console.error('Error blocking contact:', error);
				}
			}}>
				Заблокировать абонента
			</div>
			{:else}
			<div class="button-containercall blue" on:click={async () => {
				// Разблокировать контакт
				try {
					const result = await executeClientAsyncToGroup("dellBlackList", Number(contactInfo.phoneNumber));
					if (result) {
						contactInfo.blocked = false;
					}
				} catch (error) {
					console.error('Error unblocking contact:', error);
				}
			}}>
				Разблокировать абонента
			</div>
			{/if}
		</div>
		
		<!-- Bottom Navigation -->
		<div class="recent-control full-width row-block">
			{#each controlPanel as panel}
			<div class="control-block column-block align-center" 
				 class:selected={panel.type === page}
				 on:click={() => { setPage(panel.type); showContactInfo = false; contactInfo = {}; }}>
                <svg class="control-block__picture" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="18" height="18" viewBox="0 0 18 18">
					{#if panel.type === 'favourites'}
					<path xmlns="http://www.w3.org/2000/svg" d="M8.269.556a.758.758,0,0,1,1.461,0l1.6,5.148a.772.772,0,0,0,.731.556H17.23a.811.811,0,0,1,.451,1.455L13.5,10.9a.826.826,0,0,0-.279.9l1.6,5.148a.778.778,0,0,1-1.182.9L9.452,14.661a.741.741,0,0,0-.9,0L4.365,17.843a.778.778,0,0,1-1.182-.9l1.6-5.148a.826.826,0,0,0-.279-.9L.318,7.714A.811.811,0,0,1,.77,6.26H5.941A.773.773,0,0,0,6.671,5.7Z"/>
					{:else if panel.type === 'recent'}
					<path xmlns="http://www.w3.org/2000/svg" d="M9,18A9,9,0,1,0,0,9,9,9,0,0,0,9,18ZM9.63,3.87V9.09q0,.023,0,.045t0,.045A.63.63,0,0,1,9,9.81H5.22a.63.63,0,0,1,0-1.26H8.37V3.87a.63.63,0,0,1,1.26,0Z" fill-rule="evenodd"/>
					{:else if panel.type === 'contacts'}
					<path xmlns="http://www.w3.org/2000/svg" d="M18,9A9,9,0,1,1,9,0,9,9,0,0,1,18,9ZM11.88,7.2A2.974,2.974,0,0,1,9,10.26,2.974,2.974,0,0,1,6.12,7.2,2.974,2.974,0,0,1,9,4.14,2.974,2.974,0,0,1,11.88,7.2ZM8.91,11.52a7.091,7.091,0,0,0-5.247,2.312,7.2,7.2,0,0,0,10.584.1A7.093,7.093,0,0,0,8.91,11.52Z" fill-rule="evenodd"/>
					{:else if panel.type === 'keys'}
					<path d="M3.33,5.76A2.43,2.43,0,1,0,.9,3.33,2.43,2.43,0,0,0,3.33,5.76Z"/>
					<path d="M9,5.76A2.43,2.43,0,1,0,6.57,3.33,2.43,2.43,0,0,0,9,5.76Z"/>
					<path d="M17.1,3.33A2.43,2.43,0,1,1,14.67.9,2.43,2.43,0,0,1,17.1,3.33Z"/>
					<path d="M11.43,9A2.43,2.43,0,1,1,9,6.57,2.43,2.43,0,0,1,11.43,9Z"/>
					<path d="M3.33,11.43A2.43,2.43,0,1,0,.9,9,2.43,2.43,0,0,0,3.33,11.43Z"/>
					<path d="M5.76,14.67a2.43,2.43,0,1,1-2.43-2.43A2.43,2.43,0,0,1,5.76,14.67Z"/>
					<path d="M9,17.1a2.43,2.43,0,1,0-2.43-2.43A2.43,2.43,0,0,0,9,17.1Z"/>
					<path d="M17.1,14.67a2.43,2.43,0,1,1-2.43-2.43A2.43,2.43,0,0,1,17.1,14.67Z"/>
					<path d="M14.67,11.43A2.43,2.43,0,1,0,12.24,9,2.43,2.43,0,0,0,14.67,11.43Z"/>
					{/if}
				</svg>
				<div class="control-block__text">{panel.title}</div>
			</div>
			{/each}
		</div>
	</div>
	{/if}
	
	<!-- Active Call Interface -->
	{#if activeCall}
	<div class="iphone-voice-conversation full-width full-height">
		<div class="main-content full-height full-width column-block align-center">
			<div class="main-content__text-username">{activeCall.number}</div>
			<div class="status-block">
				{#if activeCall.active}
				<div class="status-block__text-isTalking justify-center">
					00:00:00
				</div>
				{:else if activeCall.status === 'outgoing'}
				<div class="status-block__text-isCalling">Вызов</div>
				{:else}
				<div class="status-block__text-isCalling">Входящий вызов</div>
				{/if}
			</div>
			
			<div class="control-containercall full-width justify-center">
				{#if activeCall.status === 'outgoing' || activeCall.active}
				<div class="control-button decline" on:click={endCall}>
					<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="10" height="10" viewBox="0 0 10 10">
						<defs>
							<clipPath id="clip-path">
								<rect id="Rectangle_238" data-name="Rectangle 238" width="10" height="10" transform="translate(-1955 220)"/>
							</clipPath>
							<clipPath id="clip-path-2">
								<rect id="Rectangle_237" data-name="Rectangle 237" width="10" height="10"/>
							</clipPath>
						</defs>
						<g id="Mask_Group_39" data-name="Mask Group 39" transform="translate(1955 -220)" opacity="0.25" clip-path="url(#clip-path)">
							<g id="Group_110" data-name="Group 110" transform="translate(-1955 220)" clip-path="url(#clip-path-2)">
								<path id="Path_94" data-name="Path 94" d="M1.787.031A6.54,6.54,0,0,0,1.04.769,6.748,6.748,0,0,0,.282,1.945c-.224.449-.3.649-.279.753A10.9,10.9,0,0,0,.634,3.94,16.192,16.192,0,0,0,6.847,9.8c.47.234.49.238.75.134a6.178,6.178,0,0,0,2.178-1.5c.237-.268.249-.3.2-.476A4.319,4.319,0,0,0,8.907,6.349a5.168,5.168,0,0,0-1.316-.96c-.278-.139-.389-.155-.5-.074a2.041,2.041,0,0,0-.247.336c-.2.313-.489.736-.592.859h0a.1.1,0,0,1-.139.012l-.156-.13A18.951,18.951,0,0,1,3.582,4.006a1.577,1.577,0,0,1-.158-.212c.081-.075.572-.416.92-.638A2.056,2.056,0,0,0,4.687,2.9c.073-.111.054-.242-.073-.491a5.1,5.1,0,0,0-.474-.767A4.672,4.672,0,0,0,2.16.068C1.922-.012,1.866-.018,1.787.031Z" transform="translate(0 0)" fill-rule="evenodd"/>
								<path id="Path_95" data-name="Path 95" d="M10.71,2.248a.489.489,0,0,0-.223.267.385.385,0,0,0,.17.423c.068.047.094.05.5.06l.409.01h0a.008.008,0,0,1,.006.014l-.66.661-.4.4a.394.394,0,0,0,0,.552h0a.391.391,0,0,0,.556.008l.253-.251.815-.814a.012.012,0,0,1,.021.009h0v.39c0,.4,0,.422.051.506a.394.394,0,0,0,.616.072c.11-.118.112-.137.11-1.107a7.7,7.7,0,0,0-.026-.97.455.455,0,0,0-.218-.229A15.843,15.843,0,0,0,10.71,2.248Z" transform="translate(-4.28 -0.913)"/>
							</g>
						</g>
					</svg>
				</div>
				{:else if activeCall.status === 'incoming'}
				<div class="incoming-call justify-between full-width">
					<div class="column-block align-center" on:click={endCall}>
						<div class="control-button decline">
							<svg viewBox="0 0 24 24">
								<path d="M12 9c-1.6 0-3.15.25-4.6.72v3.1c0 .39-.23.74-.56.9-.98.49-1.87 1.12-2.66 1.85-.18.18-.43.28-.7.28-.28 0-.53-.11-.71-.29L.29 13.08c-.18-.17-.29-.42-.29-.7 0-.28.11-.53.29-.71C3.34 8.78 7.46 7 12 7s8.66 1.78 11.71 4.67c.18.18.29.43.29.71 0 .28-.11.53-.29.7l-2.48 2.48c-.18.18-.43.29-.71.29-.27 0-.52-.1-.7-.28-.79-.73-1.68-1.36-2.66-1.85-.33-.16-.56-.5-.56-.9v-3.1C15.15 9.25 13.6 9 12 9z"/>
							</svg>
						</div>
						<div class="incoming-call__text">Отклонить</div>
					</div>
					<div class="column-block align-center" on:click={answerCall}>
						<div class="control-button accept">
							<svg viewBox="0 0 24 24">
								<path d="M6.62 10.79c1.44 2.83 3.76 5.14 6.59 6.59l2.2-2.2c.27-.27.67-.36 1.02-.24 1.12.37 2.33.57 3.57.57.55 0 1 .45 1 1V20c0 .55-.45 1-1 1-9.39 0-17-7.61-17-17 0-.55.45-1 1-1h3.5c.55 0 1 .45 1 1 0 1.25.2 2.45.57 3.57.11.35.03.74-.25 1.02l-2.2 2.2z"/>
							</svg>
						</div>
						<div class="incoming-call__text">Принять</div>
					</div>
				</div>
				{/if}
			</div>
		</div>
	</div>
	{/if}
	
	<!-- Create Contact Popup -->
	{#if popupPage === 'new-contact'}
	<CreateContact 
		{popupData} 
		on:save={handleSaveContact}
		on:discard={() => setPopup(null)}
	/>
	{/if}

	<!-- Add Favourite Popup -->
	{#if popupPage === 'add-favourite'}
	<AddFavourite 
		{contacts} 
		{favourites}
		on:selectContact={handleSelectContact}
		on:discard={() => setPopup(null)}
	/>
	{/if}
	
	<div 
		class="close-block full-width" 
		on:mousedown={closingStart}
		on:click={closeApp}
	>
		<div class="close-block-handler"></div>
	</div>
</div>