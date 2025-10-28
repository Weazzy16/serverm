<script>
	import { createEventDispatcher } from 'svelte';
	import './css/messages.css';
	
	export let cdn = '';
	
	const dispatch = createEventDispatcher();
	
	// Состояние приложения
	let page = 'dialogs'; // 'dialogs' или 'messages'
	let searchText = '';
	let searchFocused = false;
	let selectedDialog = null;
	let newMessageText = '';
	let choosingImage = false;
	let openedGeolocations = [];
	let popupPage = null; // null, 'new-message'
	
	// Данные пользователя
	let accountId = 12345;
	let phoneNumber = '12345';
	let balance = 1000;
	let timezone = 'Europe/Moscow';
	let lang = 'ru';
	
	// Контакты (для получения имен)
	let contacts = {
		A: [
			{ id: 1, name: 'Алексей Иванов', phoneNumber: '1234567', special: false }
		],
		services: [
			{ id: 'police', phoneNumber: '911', special: true, name: 'Полиция' },
			{ id: 'medical', phoneNumber: '112', special: true, name: 'Скорая помощь' },
			{ id: 'taxi', phoneNumber: '555', special: true, name: 'Такси' },
			{ id: 'bank', phoneNumber: 'bank', special: true, name: 'Банк' }
		]
	};
	
	// Диалоги
	let dialogs = [
		{
			id: 1,
			numberFrom: '666',
			numberTo: '12345',
			accountTo: 12345,
			accountFrom: 666,
			text: 'Привет, как дела?',
			date: new Date(),
			unreadMessagesCount: 2,
			photos: null
		},
		{
			id: 2,
			numberFrom: 'bank',
			numberTo: '12345',
			accountTo: 12345,
			accountFrom: 'bank',
			text: 'Ваш баланс составляет 1000$',
			date: new Date(Date.now() - 3600000),
			unreadMessagesCount: 0,
			photos: null
		}
	];
	
	// Сообщения для выбранного диалога
	let messages = [
		{
			id: 1,
			accountFrom: 666,
			accountTo: 12345,
			text: 'Привет!',
			date: new Date(Date.now() - 7200000),
			status: 1,
			statusDate: new Date(Date.now() - 7100000),
			photos: null
		},
		{
			id: 2,
			accountFrom: 12345,
			accountTo: 666,
			text: 'Привет, как дела?',
			date: new Date(Date.now() - 3600000),
			status: 1,
			statusDate: new Date(Date.now() - 3500000),
			photos: null
		},
		{
			id: 3,
			accountFrom: 666,
			accountTo: 12345,
			text: '*markerCoord*[-100,200]*markerCoord*',
			date: new Date(Date.now() - 1800000),
			status: 0,
			statusDate: null,
			photos: null
		}
	];
	
	// Служебные номера
	let serviceNumbers = ['bank', 'anonymous', 'tax', 'auction', 'marketplace', 'signaling', '555', '911', '112', '4242', 'dmv'];
	
	// Computed properties
	$: isDialogsActive = !selectedDialog && (page === 'dialogs' || page === null);
	$: filteredDialogs = dialogs.filter(dialog => 
		!searchText.length || dialogMatches(dialog, searchText)
	);
	$: groupedMessages = groupMessagesByDate(messages);
	$: allContacts = Object.values(contacts).flat();
	
	// Функции для работы с диалогами
	function setPage(pageType, dialog = null) {
		page = pageType;
		selectedDialog = dialog;
		searchText = '';
		
		if (dialog) {
			// Загружаем сообщения для диалога (здесь можно добавить API вызов)
			console.log('Loading messages for dialog:', dialog);
		}
	}
	
	function getTargetPhoneNumber(dialog) {
		return String(accountId) === String(dialog.accountTo) ? String(dialog.numberFrom) : String(dialog.numberTo);
	}
	
	function getContact(phoneNumber) {
		return allContacts.find(contact => String(contact.phoneNumber) === phoneNumber);
	}
	
	function getTitle(dialog) {
		const targetNumber = getTargetPhoneNumber(dialog);
		const contact = getContact(targetNumber);
		
		if (serviceNumbers.includes(targetNumber)) {
			switch (targetNumber) {
				case 'bank': return 'Банк';
				case 'anonymous': return 'Анонимный';
				case 'tax': return 'Налоговая';
				case 'auction': return 'Аукцион';
				case 'marketplace': return 'Торговая площадка';
				case 'signaling': return 'Сигнализация';
				case '555': return 'Такси';
				case '911': return 'Полиция';
				case '112': return 'Скорая помощь';
				case '4242': return 'Новости';
				case 'dmv': return 'Автошкола';
				default: return targetNumber;
			}
		}
		
		if (contact && contact.name) {
			return unescapeString(contact.name);
		}
		
		return targetNumber;
	}
	
	function getAvatar(dialog) {
		const targetNumber = getTargetPhoneNumber(dialog);
		
		if (serviceNumbers.includes(targetNumber)) {
			switch (targetNumber) {
				case 'bank': return `${cdn}/img/iPhone/v2/contactIcon/fleeca.svg`;
				case 'anonymous': return `${cdn}/img/iPhone/v2/contactIcon/anonym.svg`;
				case 'tax': return `${cdn}/img/iPhone/v2/contactIcon/tax.svg`;
				case 'auction':
				case 'marketplace': return `${cdn}/img/iPhone/v2/contactIcon/auction.png`;
				case 'signaling': return `${cdn}/img/iPhone/v2/contactIcon/signaling.png`;
				case '555': return `${cdn}/img/iPhone/v2/contactIcon/taxi.svg`;
				case '911': return `${cdn}/img/iPhone/v2/contactIcon/lspd.svg`;
				case '112': return `${cdn}/img/iPhone/v2/contactIcon/ems.svg`;
				case '4242': return `${cdn}/img/iPhone/v2/contactIcon/weazel.svg`;
				case 'dmv': return `${cdn}/img/iPhone/v2/contactIcon/dmv.png`;
				default: return null;
			}
		}
		
		return null;
	}
	
	function contactSlogan(dialog) {
		const title = getTitle(dialog);
		const words = title.split(' ');
		
		if (words.length >= 2) {
			return `${words[0][0]}${words[1][0]}`;
		}
		return title[0] || '?';
	}
	
	function getDialogText(text) {
		if (messageIsMarker(text)) {
			return 'Геолокация';
		}
		return text;
	}
	
	function dialogMatches(dialog, searchText) {
		if (!searchText) return true;
		
		const lowerSearch = searchText.toLowerCase();
		const title = getTitle(dialog).toLowerCase();
		const text = dialog.text.toLowerCase();
		const number = getTargetPhoneNumber(dialog);
		
		return title.includes(lowerSearch) || 
			   text.includes(lowerSearch) || 
			   number.includes(searchText);
	}
	
	// Функции для работы с сообщениями
	function groupMessagesByDate(messages) {
		const groups = {};
		
		messages.forEach(message => {
			const dateKey = new Date(message.date).toDateString();
			if (!groups[dateKey]) {
				groups[dateKey] = [];
			}
			groups[dateKey].push(message);
		});
		
		return Object.entries(groups).map(([date, msgs]) => ({
			date: new Date(date),
			messages: msgs
		}));
	}
	
	function messageIsMarker(text) {
		const regex = /\*markerCoord\*\[-?\d+,-?\d+\]\*markerCoord\*/;
		if (regex.test(text) && text.includes('*markerCoord*') && text.split('*markerCoord*').length === 3) {
			const coords = text.split('*markerCoord*').filter(part => part !== '');
			if (coords.length === 1) {
				try {
					const parsed = JSON.parse(coords[0]);
					if (Array.isArray(parsed) && parsed.length === 2 && 
						typeof parsed[0] === 'number' && typeof parsed[1] === 'number') {
						return [parsed[0] - 4, parsed[1] - 36];
					}
				} catch (e) {
					return false;
				}
			}
		}
		return false;
	}
	
	function geolocationIsOpened(messageId) {
		return openedGeolocations.includes(messageId);
	}
	
	function openGeolocation(message) {
		if (!openedGeolocations.includes(message.id)) {
			openedGeolocations = [...openedGeolocations, message.id];
		}
	}
	
	function sendMessage() {
		if (!newMessageText.trim() || balance <= 0) return;
		
		if (balance <= 0) {
			console.log('Insufficient balance');
			return;
		}
		
		// Здесь отправляем сообщение
		const newMessage = {
			id: Date.now(),
			accountFrom: accountId,
			accountTo: selectedDialog.accountFrom === accountId ? selectedDialog.accountTo : selectedDialog.accountFrom,
			text: newMessageText.replace(/\n/g, '[br]'),
			date: new Date(),
			status: 0,
			statusDate: null,
			photos: null
		};
		
		messages = [...messages, newMessage];
		newMessageText = '';
		
		// Обновляем последнее сообщение в диалоге
		const dialogIndex = dialogs.findIndex(d => d.id === selectedDialog.id);
		if (dialogIndex !== -1) {
			dialogs[dialogIndex].text = newMessage.text;
			dialogs[dialogIndex].date = newMessage.date;
			dialogs = [...dialogs];
		}
	}
	
	function call(dialog) {
		const targetNumber = getTargetPhoneNumber(dialog);
		
		if (balance <= 0 && !['112', '911', '4242', 'GOV'].includes(targetNumber)) {
			console.log('Insufficient balance for call');
			return;
		}
		
		console.log('Calling:', targetNumber);
		dispatch('call', { number: targetNumber });
	}
	
	function shareGeo() {
		console.log('Sharing geolocation');
		// Здесь логика отправки геолокации
	}
	
	function getContactsList(letter) {
		if (letter === 'search') {
			if (!searchText.length) return [];
			const filtered = [];
			for (const key of Object.keys(contacts)) {
				const results = contacts[key].filter(contact => getContactsMatch(contact, searchText));
				filtered.push(...results);
			}
			return filtered;
		}
		return contacts[letter] ? contacts[letter].filter(contact => getContactsMatch(contact, searchText)) : [];
	}
	
	function getContactsMatch(contact, searchText) {
		if (!searchText) return true;
		const lowerSearch = searchText.toLowerCase();
		
		if (contact.special) {
			return contact.name?.toLowerCase().includes(lowerSearch) || 
				   String(contact.phoneNumber).includes(searchText);
		}
		
		return String(contact.name || '').toLowerCase().includes(lowerSearch) || 
			   String(contact.phoneNumber).includes(searchText);
	}
	
	function getCaption(letter) {
		switch (letter) {
			case 'services': return 'Службы';
			case 'search': return 'Поиск';
			default: return letter;
		}
	}
	
	// Утилиты
	function unescapeString(str) {
		if (!str) return '';
		return str.replace(/&amp;/g, '&')
				  .replace(/&lt;/g, '<')
				  .replace(/&gt;/g, '>')
				  .replace(/&quot;/g, '"')
				  .replace(/&#x27;/g, "'");
	}
	
	function sliceString(str, maxLength) {
		if (!str) return '';
		return str.length > maxLength ? str.slice(0, maxLength) + '...' : str;
	}
	
	function formatDate(date, type = 'auto') {
		const now = new Date();
		const messageDate = new Date(date);
		
		if (type === 'auto') {
			if (now.toDateString() === messageDate.toDateString()) {
				return messageDate.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
			} else {
				return messageDate.toLocaleDateString('ru-RU');
			}
		} else if (type === 'time') {
			return messageDate.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
		} else if (type === 'date') {
			return messageDate.toLocaleDateString('ru-RU', { day: '2-digit', month: 'short', year: 'numeric' });
		}
		
		return messageDate.toString();
	}
	
	function closeApp() {
		dispatch('close');
	}
	
	function closingStart() {
		console.log('Closing start');
	}
	
	function setPopupPage(pageType) {
		popupPage = pageType;
	}
	
	function handleNewMessage() {
		setPopupPage('new-message');
	}
	
	function handleMessageTo(contact) {
		// Ищем существующий диалог с этим контактом
		const existingDialog = dialogs.find(dialog => {
			const targetNumber = getTargetPhoneNumber(dialog);
			return targetNumber === contact.phoneNumber;
		});
		
		if (existingDialog) {
			// Открываем существующий диалог
			setPage('messages', existingDialog);
		} else {
			// Создаем новый диалог
			const newDialog = {
				id: Date.now(),
				numberFrom: contact.phoneNumber,
				numberTo: phoneNumber,
				accountTo: accountId,
				accountFrom: contact.phoneNumber,
				text: '',
				date: new Date(),
				unreadMessagesCount: 0,
				photos: null
			};
			dialogs = [newDialog, ...dialogs];
			setPage('messages', newDialog);
		}
		
		setPopupPage(null);
	}
</script>

<!-- svelte-ignore a11y-click-events-have-key-events -->
<div class="messages-container column-block full-width full-height" class:scaled={popupPage}>
	
	{#if isDialogsActive}
	<!-- Список диалогов -->
	<div class="dialogs full-height full-width column-block align-center">
		<div class="dialogs-header full-width">
			<div class="send-message" on:click={handleNewMessage}>
				<svg viewBox="0 0 24 24" class="send-message__image">
					<path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"/>
				</svg>
			</div>
			
			<div class="dialogs-header__title">Сообщения</div>
			
			<div class="search-container align-center row-block" class:search={searchText.length}>
				<div class="search-panel row-block full-width align-center">
					<svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 13 13" class="search-panel__icon">
						<path d="M12.585,11.841,9.491,8.623A5.247,5.247,0,1,0,5.473,10.5a5.193,5.193,0,0,0,3.007-.95L11.6,12.79a.685.685,0,1,0,.987-.949ZM5.473,1.369a3.88,3.88,0,1,1-3.88,3.88A3.884,3.884,0,0,1,5.473,1.369Z" transform="translate(9 9)"/>
					</svg>
					
					<input 
						type="text"
						bind:value={searchText}
						on:focus={() => searchFocused = true}
						placeholder="Поиск"
						class="search-panel__input full-width full-height"
					/>
					
					<div class="clear-icon" on:click={() => searchText = ''}>
						<svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15" class="clear-icon__image">
							<path d="M7.5,15A7.5,7.5,0,1,0,0,7.5,7.5,7.5,0,0,0,7.5,15ZM4.705,5.295,6.911,7.5,4.705,9.705l.589.589L7.5,8.089l2.205,2.205.589-.589L8.089,7.5l2.205-2.205-.589-.589L7.5,6.911,5.295,4.705Z" fill-rule="evenodd"/>
						</svg>
					</div>
				</div>
				
				<div class="discard-button" on:click={() => searchText = ''}>
					Отменить
				</div>
			</div>
			
			<div class="divider"></div>
		</div>
		
		{#if filteredDialogs.length > 0}
		<div class="dialogs-content full-width column-block">
			{#each filteredDialogs as dialog (dialog.id)}
			<div class="dialog-container row-block full-width" 
				 class:unread={dialog.unreadMessagesCount > 0}
				 on:click={() => setPage('messages', dialog)}>
				
				{#if getAvatar(dialog)}
				<img src={getAvatar(dialog)} alt="" class="dialog-container__picture" />
				{:else}
				<div class="dialog-container__picture slogan">
					{contactSlogan(dialog)}
				</div>
				{/if}
				
				<div class="dialog-content column-block">
					<div class="dialog-container__text-nickname">
						{unescapeString(getTitle(dialog))}
					</div>
					
					{#if dialog.photos && dialog.photos.length >= 1}
					<div class="dialog-container__text-message">
						{dialog.photos.length === 1 ? 'Фото' : 'Фотографии'}
					</div>
					{:else}
					<div class="dialog-container__text-message">
						{sliceString(unescapeString(getDialogText(dialog.text.replace(/\[br\]/g, ' ').replace(/&/g, '&'))), 50)}
					</div>
					{/if}
				</div>
				
				<div class="date-block row-block align-center">
					{formatDate(dialog.date, 'auto')}
				</div>
				
				<div class="divider"></div>
			</div>
			{/each}
		</div>
		{/if}
	</div>
	
	{:else if selectedDialog}
	<!-- Просмотр сообщений -->
	<div class="messages full-width full-height column-block">
		<div class="messages-header align-center column-block">
			<div class="back-button-container row-block align-center">
				<div class="back-arrow" on:click={() => setPage('dialogs')}>
					<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" class="back-arrow__picture">
						<path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z"/>
					</svg>
				</div>
			</div>
			
			<div class="avatar column-block align-center">
				{#if getAvatar(selectedDialog)}
				<img src={getAvatar(selectedDialog)} alt="" class="avatar__picture" />
				{:else}
				<div class="avatar__picture slogan">
					{contactSlogan(selectedDialog)}
				</div>
				{/if}
				
				<div class="avatar__title">
					{unescapeString(getTitle(selectedDialog))}
				</div>
			</div>
			
			<div class="call-button" on:click={() => call(selectedDialog)}>
				<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 20 20" class="call-button__picture">
					<path d="M2.943.045C2.531.134,2.451.2,1.374,1.271.434,2.208.361,2.289.251,2.522A2.369,2.369,0,0,0,0,3.659,4.634,4.634,0,0,0,.2,5.2C1.2,9,5.686,14,9.982,16.1a7.194,7.194,0,0,0,3.185.9,2.476,2.476,0,0,0,1.277-.255c.259-.127.327-.188,1.263-1.12,1.047-1.043,1.11-1.122,1.242-1.564a1.984,1.984,0,0,0,0-.9c-.119-.4-.168-.459-1.493-1.788-.7-.7-1.361-1.339-1.471-1.417a1.572,1.572,0,0,0-2.065.158,1.43,1.43,0,0,0-.348.538,3.1,3.1,0,0,1-.232.45,1.708,1.708,0,0,1-1.457.649A5.885,5.885,0,0,1,6.952,9.981c-1.194-1.191-1.9-2.562-1.726-3.36a1.66,1.66,0,0,1,.651-.968,2.631,2.631,0,0,1,.354-.184,1.769,1.769,0,0,0,.592-.335A1.62,1.62,0,0,0,7.1,3.1C6.977,2.915,4.677.6,4.4.382a1.922,1.922,0,0,0-.781-.35A1.515,1.515,0,0,0,2.943.045Z" transform="translate(2 2)" fill-rule="evenodd"/>
				</svg>
			</div>
		</div>
		
		<div class="messages-content column-block full-height full-width">
			<div class="messages-viewbox full-width column-block">
				{#each groupedMessages as group (group.date.getTime())}
				<div class="messages-group column-block align-center justify-start full-width">
					<div class="messages-group-title">
						{formatDate(group.date, 'date')} {formatDate(group.messages[0].date, 'time')}
					</div>
					
					{#each group.messages as message (message.id)}
					<div class="messages-wrapper full-width {accountId === message.accountFrom ? 'local' : 'interlocutor'}"
						 class:only-image={messageIsMarker(message.text) && !message.text.replace(/\*markerCoord\*\[-?\d+,-?\d+\]\*markerCoord\*/, '').length}>
						
						{#if messageIsMarker(message.text)}
						<div class="map-wrapper">
							{#if geolocationIsOpened(message.id)}
							<div class="map">
								<!-- Здесь должна быть карта -->
								<div class="map-placeholder">Карта: {messageIsMarker(message.text)}</div>
							</div>
							{:else}
							<div class="hiddenGeolocation row-block align-center justify-center"
								 on:click={() => openGeolocation(message)}
								 style="background-image: url({cdn}/img/iPhone/geolocation.jpg)">
								Показать геолокацию
							</div>
							{/if}
						</div>
						{:else}
						<div class="message-block column-block withPhoto align-end">
							{#if message.photos && message.photos.length >= 1}
							{#each message.photos as photo, index (index)}
							<div class="message-block__picture full-width" 
								 class:several={message.photos.length >= 2}
								 style="background-image: url({photo.url})">
							</div>
							{/each}
							{/if}
							
							{#if message.text && message.text !== 'null'}
							<span class="message-text">
								{@html unescapeString(message.text.replace(/(?:\r\n|\r|\n)/g, '<br />').replace(/\[br\]/g, '<br>'))}
							</span>
							{/if}
						</div>
						{/if}
						
						<div class="readTime">
							{#if accountId === message.accountFrom && message.status === 1 && message.statusDate}
							<p class="read">Прочитано {formatDate(message.statusDate, 'time')}</p>
							{/if}
							<p>{formatDate(message.date, 'time')}</p>
						</div>
					</div>
					{/each}
				</div>
				{/each}
				
				<div class="time-splitter"></div>
			</div>
			
			<div class="input-block row-block align-center full-width justify-end">
				<div class="control-button full-height" on:click={() => choosingImage = true}>
					<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" class="control-button__picture">
						<path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm3.5 6L12 10.5 8.5 8 12 5.5 15.5 8zM12 19c-3.87 0-7-3.13-7-7s3.13-7 7-7 7 3.13 7 7-3.13 7-7 7z"/>
					</svg>
				</div>
				
				<div class="control-button full-height" on:click={shareGeo}>
					<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" class="control-button__picture">
						<path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z"/>
					</svg>
				</div>
				
				<div class="input-field full-width row-block align-center">
					<textarea 
						bind:value={newMessageText}
						placeholder="iMessage"
						class="input-field__input full-width full-height"
						rows="1"
					></textarea>
					
					<div class="send-message" on:click={sendMessage}>
						<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" class="send-message__picture">
							<path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z"/>
						</svg>
					</div>
				</div>
			</div>
		</div>
	</div>
	{/if}
	
	<div class="close-block full-width" 
		 on:mousedown={closingStart}
		 on:click={closeApp}>
		<div class="close-block-handler"></div>
	</div>
	
	<!-- Popup для создания нового сообщения -->
	{#if popupPage === 'new-message'}
	<div class="add-favourite full-width full-height align-end">
		<div class="container full-width column-block align-center">
			<div class="container-header column-block full-width align-center">
				<div class="row-block full-width">
					<div class="container-header__button full-height justify-start align-center" 
						 on:click={() => setPopupPage(null)}>
						Отменить
					</div>
					<span class="container-header__title align-center">Новое сообщение</span>
				</div>
				
				<div class="search-block row-block align-center full-width" class:clearable={searchText.length}>
					<div class="search-container row-block align-center full-width">
						<svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 13 13" class="search-container__icon">
							<path d="M12.585,11.841,9.491,8.623A5.247,5.247,0,1,0,5.473,10.5a5.193,5.193,0,0,0,3.007-.95L11.6,12.79a.685.685,0,1,0,.987-.949ZM5.473,1.369a3.88,3.88,0,1,1-3.88,3.88A3.884,3.884,0,0,1,5.473,1.369Z" transform="translate(9 9)"/>
						</svg>
						<input 
							type="text"
							bind:value={searchText}
							placeholder="Поиск"
							class="search-container__input full-width"
							maxlength="30"
						/>
						<svg class="search-container__icon-clear" 
							 on:click={() => searchText = ''} 
							 xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15">
							<path d="M7.5,15A7.5,7.5,0,1,0,0,7.5,7.5,7.5,0,0,0,7.5,15ZM4.705,5.295,6.911,7.5,4.705,9.705l.589.589L7.5,8.089l2.205,2.205.589-.589L8.089,7.5l2.205-2.205-.589-.589L7.5,6.911,5.295,4.705Z" fill-rule="evenodd"/>
						</svg>
					</div>
					<div class="discard-button" on:click={() => { searchText = ''; }}>
						Отменить
					</div>
				</div>
				<div class="divider full-width"></div>
			</div>
			
			<div class="container-body column-block full-width full-height">
				{#each Object.keys(contacts) as letter}
				{#if contacts[letter] && getContactsList(letter).length > 0}
				<div class="contact-container column-block">
					<div class="contact-container__text full-width align-center">
						{getCaption(letter)}
					</div>
					<div class="contacts-content full-width">
						{#each getContactsList(letter) as contact}
						<div class="contact-block align-center full-width" 
							 on:click={() => handleMessageTo(contact)}>
							{#if contact.special}
							<div class="contact-block__text align-center full-width">
								{contact.name || contact.phoneNumber}
							</div>
							{:else}
							<div class="contact-block__text align-center full-width">
								{unescapeString(contact.name) || contact.phoneNumber}
							</div>
							{/if}
							<div class="border"></div>
						</div>
						{/each}
					</div>
				</div>
				{/if}
				{/each}
				
				{#if searchText.length && !getContactsList('search').length}
				<div class="not-found full-width full-height column-block">
					<svg class="not-found__picture" xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 13 13">
						<path d="M12.585,11.841,9.491,8.623A5.247,5.247,0,1,0,5.473,10.5a5.193,5.193,0,0,0,3.007-.95L11.6,12.79a.685.685,0,1,0,.987-.949ZM5.473,1.369a3.88,3.88,0,1,1-3.88,3.88A3.884,3.884,0,0,1,5.473,1.369Z" transform="translate(9 9)"/>
					</svg>
					<span class="not-found__title">
						Нет результатов <br/> «{searchText}»
					</span>
					<span class="not-found__text">Проверьте запрос</span>
				</div>
				{/if}
			</div>
		</div>
	</div>
	{/if}
</div>