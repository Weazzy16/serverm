<script>
	import { createEventDispatcher } from 'svelte';
	import { validate } from 'api/validation';
	
	export let popupData = null;
	
	const dispatch = createEventDispatcher();
	
	// Состояние формы
	let name = '';
	let surname = '';
	let phone = '';
	
	// Инициализация данных при наличии popupData
	$: if (popupData && popupData.phoneNumber) {
		phone = popupData.phoneNumber;
	}
	
	$: saveAvailable = (name.length || surname.length) && String(phone).length;
	
	function saveContact() {
		const fullName = `${name} ${surname}`.trim();
		if (!fullName.length || !phone.length) return;
		
		const cleanPhone = phone.replace(/[^\d]/g, '');
		
		// Валидация имени
		let check = validate("phonename", fullName);
		if (!check.valid) {
			window.notificationAdd(4, 9, check.text, 3000);
			return;
		}
		
		// Валидация номера
		const phoneNumber = Number(cleanPhone);
		check = validate("phonenumber", phoneNumber);
		if (!check.valid) {
			window.notificationAdd(4, 9, check.text, 3000);
			return;
		}
		
		// Отправляем данные родительскому компоненту
		dispatch('save', {
			name: fullName,
			phone: cleanPhone
		});
		
		// Сбрасываем форму
		name = '';
		surname = '';
		phone = '';
	}
	
	function handlePhoneInput(event) {
		const value = event.target.value.replace(/[^0-9]/g, '');
		// Ограничиваем длину номера
		if (value.length <= 8) {
			phone = value;
		}
	}
	
	function handleNameInput(event, field) {
		const value = event.target.value;
		// Ограничиваем длину имени/фамилии
		if (value.length <= 20) {
			if (field === 'name') {
				name = value;
			} else if (field === 'surname') {
				surname = value;
			}
		}
	}
	
	function discard() {
		// Сбрасываем форму при отмене
		name = '';
		surname = '';
		phone = '';
		dispatch('discard');
	}
</script>

<div class="create-contact full-width full-height align-end">
	<div class="containercall full-width">
		<div class="containercall-header row-block justify-between full-width align-center">
			<div class="containercall-header__button full-height justify-start align-center" on:click={discard}>
				Отменить
			</div>
			<span class="containercall-header__title">Контакт</span>
			<div class="containercall-header__button align-center justify-end full-height" 
				 class:disabled={!saveAvailable}
				 on:click={saveContact}>
				Готово
			</div>
		</div>
		<div class="containercall-main column-block align-center">
			<div class="inputs-containercall column-block full-width justify-center">
				<input 
					type="text"
					bind:value={name}
					maxlength="20"
					class="inputs-containercall__input"
					placeholder="Имя"
					on:input={(e) => handleNameInput(e, 'name')}
				/>
				<div class="line"></div>
				<input 
					type="text"
					bind:value={surname}
					maxlength="20"
					class="inputs-containercall__input"
					placeholder="Фамилия"
					on:input={(e) => handleNameInput(e, 'surname')}
				/>
			</div>
			<div class="inputs-containercall column-block full-width">
				<input 
					type="text"
					bind:value={phone}
					maxlength="8"
					class="inputs-containercall__input"
					placeholder="Телефон"
					on:input={handlePhoneInput}
				/>
			</div>
		</div>
	</div>
</div>