<script>
	import { createEventDispatcher } from 'svelte';
	
	export let contacts = {};
	export let favourites = [];
	
	const dispatch = createEventDispatcher();
	
	let inputSearch = '';
	let searchFocused = false;
	let selectedContact = null;
	
	$: computedData = searchFocused ? ['search'] : Object.keys(contacts).sort();
	
	function getCaption(letter) {
		switch (letter) {
			case 'services': return 'Службы';
			case 'search': return 'Поиск';
			default: return letter;
		}
	}
	
	function getContacts(contact, searchText) {
		if (!searchText) return true;
		searchText = searchText.toLowerCase();
		
		if (contact.special) {
			// Для специальных контактов ищем по имени службы
			return contact.name?.toLowerCase().includes(searchText) || 
				   String(contact.phoneNumber).includes(searchText);
		}
		
		return String(contact.name || '').toLowerCase().includes(searchText) || 
			   String(contact.phoneNumber).includes(searchText);
	}
	
	function getContactsList(letter) {
		let contactList = [];
		
		if (letter === 'search') {
			if (!inputSearch.length) return [];
			
			for (const key of Object.keys(contacts)) {
				const filtered = contacts[key].filter(contact => getContacts(contact, inputSearch));
				contactList = [...contactList, ...filtered];
			}
		} else {
			contactList = contacts[letter] ? contacts[letter].filter(contact => getContacts(contact, inputSearch)) : [];
		}
		
		// Исключаем контакты, которые уже в избранном
		return contactList.filter(contact => 
			!favourites.some(fav => fav.phoneNumber === contact.phoneNumber)
		);
	}
	
	function setContactInfo(contact) {
		selectedContact = contact;
	}
	
	function commitContact() {
		if (selectedContact) {
			dispatch('selectContact', selectedContact);
		}
	}
	
	function discard() {
		// Сбрасываем состояние при отмене
		inputSearch = '';
		searchFocused = false;
		selectedContact = null;
		dispatch('discard');
	}
	
	function clearSearch() {
		inputSearch = '';
	}
	
	function unescapeString(str) {
		if (!str) return '';
		return str.replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
	}
</script>

<div class="add-favourite full-width full-height align-end">
	<div class="containercall full-width column-block align-center">
		<span class="containercall-header__title-hint">Нажмите чтобы добавить</span>
		<div class="containercall-header column-block justify-between full-width align-center">
			<div class="row-block full-width justify-between">
				<div class="containercall-header__button full-height justify-start align-center" on:click={discard}>
					Отменить
				</div>
				<span class="containercall-header__title align-center">Контакт</span>
				<div class="containercall-header__button align-center full-height justify-end"
					 class:disabled={!selectedContact}
					 on:click={commitContact}>
					Готово
				</div>
			</div>
			<div class="search-containercall row-block align-center full-width" class:clearable={inputSearch.length}>
				<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="13" height="13" viewBox="0 0 13 13">
					<g transform="translate(-9 -9)">
						<path d="M12.585,11.841,9.491,8.623A5.247,5.247,0,1,0,5.473,10.5a5.193,5.193,0,0,0,3.007-.95L11.6,12.79a.685.685,0,1,0,.987-.949ZM5.473,1.369a3.88,3.88,0,1,1-3.88,3.88A3.884,3.884,0,0,1,5.473,1.369Z" transform="translate(9 9)"/>
					</g>
				</svg>
				<input 
					maxlength="30"
					type="text"
					bind:value={inputSearch}
					on:focus={() => searchFocused = true}
					class="search-containercall__input full-width"
					placeholder="Поиск"
				/>
				
				{#if inputSearch.length}
				<svg class="search-containercall__icon-clear" on:click={clearSearch} xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 15 15">
					<path d="M7.5,15A7.5,7.5,0,1,0,0,7.5,7.5,7.5,0,0,0,7.5,15ZM4.705,5.295,6.911,7.5,4.705,9.705l.589.589L7.5,8.089l2.205,2.205.589-.589L8.089,7.5l2.205-2.205-.589-.589L7.5,6.911,5.295,4.705Z" fill-rule="evenodd"/>
				</svg>
				{/if}
			</div>
			<div class="divider full-width"></div>
		</div>
		
		<div class="containercall-body column-block full-width full-height">
			{#each computedData as letter}
				{#if contacts[letter] && getContactsList(letter).length}
				<div class="contact-containercall column-block">
					<div class="contact-containercall__text full-width align-center">
						{getCaption(letter)}
					</div>
					<div class="contacts-content full-width">
						{#each getContactsList(letter) as contact}
						<div class="contact-block align-center full-width" 
							 class:selected={selectedContact === contact}
							 on:click={() => setContactInfo(contact)}>
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
			
			{#if searchFocused && inputSearch.length && !getContactsList('search').length}
			<div class="not-found full-width full-height column-block">
				<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="13" height="13" viewBox="0 0 13 13">
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
	</div>
</div>