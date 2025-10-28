<script>
	import { createEventDispatcher } from 'svelte';
			import './css/operator.css'

	export let cdn = '';
	
	const dispatch = createEventDispatcher();
	
	// Состояние приложения
	let balance = 15000;
	let phoneNumber = '32323';
	let operator = 'tov-tov';
	let replenishment = false;
	let replenishValue = 100;
	
	// Настройки слайдера
	let sliderOptions = {
		min: 100,
		max: 10000,
		interval: 1
	};
	
	// Услуги оператора
	const pros = ['call', 'chat', 'connection'];
	
	// Функции
	function formatNumber(num) {
		return new Intl.NumberFormat('ru-RU').format(num);
	}
	
	function setReplenishment(value) {
		replenishment = value;
	}
	
	function submitReplenishment() {
		// Логика пополнения баланса
		console.log('Replenishing balance:', replenishValue);
		balance += replenishValue;
		replenishValue = 100;
		setReplenishment(false);
	}
	
	function changeSliderValue(value) {
		if (value < sliderOptions.min) {
			replenishValue = sliderOptions.min;
		} else if (value > sliderOptions.max) {
			replenishValue = sliderOptions.max;
		} else {
			replenishValue = value;
		}
	}
	
	function handleSliderInput(event) {
		const value = parseInt(event.target.value);
		changeSliderValue(value);
	}
	
	let isDragging = false;
	let sliderElement = null;
	
	function startDrag(event) {
		isDragging = true;
		sliderElement = event.target.closest('.vue-slider');
		updateSliderValue(event);
		
		// Добавляем обработчики для перетаскивания
		document.addEventListener('mousemove', handleDrag);
		document.addEventListener('mouseup', stopDrag);
		event.preventDefault();
	}
	
	function handleDrag(event) {
		if (!isDragging || !sliderElement) return;
		updateSliderValue(event);
	}
	
	function stopDrag() {
		isDragging = false;
		sliderElement = null;
		document.removeEventListener('mousemove', handleDrag);
		document.removeEventListener('mouseup', stopDrag);
	}
	
	function updateSliderValue(event) {
		if (!sliderElement) return;
		
		const rect = sliderElement.getBoundingClientRect();
		const offsetX = event.clientX - rect.left;
		const percentage = Math.max(0, Math.min(100, (offsetX / rect.width) * 100));
		
		const newValue = Math.round(sliderOptions.min + (percentage / 100) * (sliderOptions.max - sliderOptions.min));
		changeSliderValue(newValue);
	}
	
	function closeApp() {
		dispatch('close');
	}
	
	function closingStart() {
		console.log('Closing start');
	}
	
	// Переводы
	const translations = {
		'your-phone': 'Твой номер',
		'balance': 'Баланс',
		'no-limits': 'Безлимит на все',
		'call': 'Минуты',
		'chat': 'SMS', 
		'connection': 'Интернет',
		'replenishment': 'Пополнить баланс',
		'replenish-balance': 'Пополнение баланса',
		'replenish-number': 'номер пополнения',
		'my-number': 'Мой номер',
		'paying-method': 'СПОСОБ ОПЛАТЫ',
		'bank-card': 'Банковская карта',
		'sum': 'СУММА ОПЛАТЫ',
		'today': 'Сегодня'
	};
	
	function t(key) {
		return translations[key] || key;
	}
</script>

<div class="operator full-height full-width justify-center {operator}">
	<div class="figure-1"></div>
	<div class="figure-2"></div>
	<div class="figure-3"></div>
	<div class="figure-4"></div>
	
	<img class="logo" src="{cdn}/img/iPhone/apps/operator/logo.svg" alt="" />
	
	<img 
		src="{cdn}/img/avatars/male.svg" 
		alt="avatar" 
		class="light avatar" 
	/>
	
	<div class="content-container full-width full-height justify-between column-block">
		<div class="column-block">
			<div class="content-container__title column-block align-center">
				{t('your-phone')}
			</div>
			<div class="content-container__value column-block align-center">
				{phoneNumber}
			</div>
			
			<div class="balance-container full-width column-block justify-between">
				<div class="balance-figure-1"></div>
				<div class="balance-figure-2"></div>
				<div class="column-block">
					<span class="balance-container__title">{t('balance')}</span>
					<span class="balance-container__value">${formatNumber(balance)}</span>
				</div>
				<div class="balance-container__text row-block full-width justify-end">
					{t('no-limits')}
				</div>
			</div>
			
			<div class="pros row-block full-width justify-between">
				{#each pros as pro}
				<div class="pros-container column-block align-center">
					<div class="pros-figure"></div>
					<img 
						class="pros-container__icon" 
						src="{cdn}/img/iPhone/apps/operator/{pro}.svg" 
						alt="" 
					/>
					<span class="pros-container__value">500</span>
					<span class="pros-container__title">{t(pro)}</span>
				</div>
				{/each}
			</div>
		</div>
		
		<div class="replenishment full-width" on:click={() => setReplenishment(true)}>
			{t('replenishment')}
		</div>
	</div>
	
	<div class="replenishment-container full-width column-block justify-between" class:showed={replenishment}>
		<div class="column-block full-width align-center">
			<div class="hide-button" on:click={() => setReplenishment(false)}></div>
			<span class="replenishment-container__title">{t('replenish-balance')}</span>
			
			<div class="data-container column-block full-width">
				<span class="data-container__title">{t('replenish-number')}</span>
				<div class="data-block row-block align-center">
					<img 
						src="{cdn}/img/avatars/male.svg" 
						alt="avatar" 
						class="light replenish-avatar" 
					/>
					<div class="my-number column-block full-height justify-center">
						<span class="my-number__title">{t('my-number')}</span>
						<span class="my-number__value">{phoneNumber}</span>
					</div>
				</div>
			</div>
			
			<div class="data-container column-block full-width">
				<span class="data-container__title">{t('paying-method')}</span>
				<div class="data-block row-block align-center">
					<img 
						class="card-image" 
						src="{cdn}/img/iPhone/apps/operator/card.svg" 
						alt="" 
					/>
					<span class="data-block__title">{t('bank-card')}</span>
				</div>
			</div>
			
			<div class="data-container column-block full-width">
				<span class="data-container__title">{t('sum')}</span>
				<div class="input-blockOp full-width column-block justify-start align-start">
					<div class="input-blockOp__wrapper column-block align-start justify-center">
						<div class="row-block align-center justify-start">
							<span>$</span>
							<input 
								type="text" 
								bind:value={replenishValue}
								on:input={(e) => changeSliderValue(parseInt(e.target.value) || sliderOptions.min)}
								class="input-blockOp__input" 
								placeholder="Enter value" 
							/>
							<span class="additionalValue"></span>
						</div>
						<div class="trail-back">
							<div 
								class="trail-value" 
								style="width: {((replenishValue - sliderOptions.min) / (sliderOptions.max - sliderOptions.min)) * 100}%; transition-duration: 0.5s;"
							></div>
						</div>
					</div>
					
					<div class="vue-slider vue-slider-ltr" style="padding: 7px 0px; width: auto; height: 4px;">
						<div class="vue-slider-rail">
							<div 
								class="vue-slider-process" 
								style="height: 100%; top: 0px; left: 0%; width: {((replenishValue - sliderOptions.min) / (sliderOptions.max - sliderOptions.min)) * 100}%; transition-property: width, left; transition-duration: 0.5s;"
							></div>
							<div 
								class="vue-slider-dot"
								aria-valuetext="{replenishValue}"
								role="slider"
								aria-valuenow="{replenishValue}"
								aria-valuemin="{sliderOptions.min}"
								aria-valuemax="{sliderOptions.max}"
								aria-orientation="horizontal"
								tabindex="0"
								style="width: 14px; height: 14px; transform: translate(-50%, -50%); top: 50%; left: {((replenishValue - sliderOptions.min) / (sliderOptions.max - sliderOptions.min)) * 100}%; transition: left 0.5s;"
								on:mousedown={startDrag}
							>
								<div class="vue-slider-dot-handle"></div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		
		<div class="replenishment full-width" on:click={submitReplenishment}>
			{t('replenishment')}
		</div>
	</div>
	
	<div 
		class="close-block full-width" 
		on:mousedown={closingStart}
		on:click={closeApp}
	>
		<div class="close-block-handler"></div>
	</div>
</div>