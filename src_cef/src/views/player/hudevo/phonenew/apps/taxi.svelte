<script>
	import { createEventDispatcher } from 'svelte';
	import './css/uber.css';
	
	export let cdn = '';
	
	const dispatch = createEventDispatcher();
	
	// Состояние приложения
	let orders = [
		{
			id: 1,
			accountId: 12345,
			socialClubName: 'john_doe',
			gender: 'male',
			dist: 1250.5
		},
		{
			id: 2,
			accountId: 67890,
			socialClubName: 'jane_smith',
			gender: 'female',
			dist: 2100.0
		},
		{
			id: 3,
			accountId: 11111,
			socialClubName: null,
			gender: 'male',
			dist: 750.25
		}
	];
	
	let taxiNotifies = true;
	let isStreamer = false;
	
	// Функции
	function takeOrder(order) {
		console.log('Taking order:', order.id);
		// Удаляем заказ из списка после взятия
		orders = orders.filter(o => o.id !== order.id);
		// Здесь должен быть вызов к клиенту
		// this.$_callClient("iphone.taxiTakeOrder", order.id)
	}
	
	function getAvatar(socialClubName) {
		if (socialClubName) {
			if (isStreamer) {
				return streamerPicture(socialClubName);
			} else {
				return `https://a.rsg.sc/n/${socialClubName.toLowerCase()}`;
			}
		} else {
			return `${cdn}/img/avatars/male.svg`;
		}
	}
	
	function streamerPicture(socialClubName) {
		// Логика для стримеров
		return `${cdn}/img/avatars/male.svg`;
	}
	
	function enableNotificationsTaxi() {
		taxiNotifies = !taxiNotifies;
		console.log('Toggle taxi notifications:', taxiNotifies);
		// Здесь должен быть вызов к клиенту
		// this.$_callClient("iphone.settings.toggleTaxiNotifies", taxiNotifies)
	}
	
	function closeApp() {
		dispatch('close');
	}
	
	function closingStart() {
		console.log('Closing start');
	}
	
	// Переводы
	const translations = {
		'orders-queue': 'Очередь заказов',
		'client': 'Клиент',
		'meters': 'м',
		'notify': 'Включить уведомления',
		'offNotify': 'Выключить уведомления'
	};
	
	function t(key) {
		return translations[key] || key;
	}
</script>

<div class="taxi-main align-center column-block full-width full-height">
	<div class="header row-block align-center justify-start full-width">
		<svg class="icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="49" height="49" viewBox="0 0 49 49">
  <defs>
    <clipPath id="clip-path">
      <rect id="Rectangle_5991" data-name="Rectangle 5991" width="49" height="49" rx="10" transform="translate(-4680 869)" fill="#fff"/>
    </clipPath>
    <clipPath id="clip-path-2">
      <rect id="Rectangle_5990" data-name="Rectangle 5990" width="33" height="33" transform="translate(-0.448 -0.448)" fill="#27292e"/>
    </clipPath>
  </defs>
  <g id="Taxi" transform="translate(4680 -869)" clip-path="url(#clip-path)">
    <rect id="Rectangle_5984" data-name="Rectangle 5984" width="49" height="49" transform="translate(-4680 869)" fill="#ffd245"/>
    <g id="Mask_Group_435" data-name="Mask Group 435" transform="translate(-4671.552 877.448)" clip-path="url(#clip-path-2)">
      <g id="squares" transform="translate(0 9.631)">
        <rect id="Rectangle_5985" data-name="Rectangle 5985" width="6.421" height="6.421" transform="translate(25.683 0)"/>
        <rect id="Rectangle_5986" data-name="Rectangle 5986" width="6.421" height="6.421" transform="translate(0 0)"/>
        <rect id="Rectangle_5987" data-name="Rectangle 5987" width="6.421" height="6.421" transform="translate(12.841 0)"/>
        <rect id="Rectangle_5988" data-name="Rectangle 5988" width="6.421" height="6.421" transform="translate(6.421 6.421)"/>
        <rect id="Rectangle_5989" data-name="Rectangle 5989" width="6.421" height="6.421" transform="translate(19.262 6.421)"/>
      </g>
    </g>
  </g>
</svg>
		<div class="title">{t('orders-queue')}</div>
		<div class="orders row-block align-center justify-end">
			<span class="orders-value">{orders.length}</span>
		</div>
	</div>
	
	<div class="wrapper-list">
		<div class="list align-center full-width column-block">
			{#each orders as order (order.id)}
			<div class="list-item column-block justify-center full-width" on:click={() => takeOrder(order)}>
				<div class="row-block full-width align-center justify-start">
					<img 
						class="list-item__picture" 
						src="{getAvatar(order.socialClubName)}" 
						alt="avatar"
					/>
					<span class="list-item__text-nickname row-block align-center justify-start">
						<span class="list-item__text-nickname-text">{t('client')}</span>
						<span class="list-item__text-nickname-value">#{order.accountId}</span>
					</span>
					<span class="list-item__text-distance row-block align-center justify-start">
						<span class="list-item__text-distance-value">
							{order.dist ? Number(order.dist).toFixed(2) : '0.00'}
						</span>
						<span class="list-item__text-distance-text">{t('meters')}</span>
					</span>
				</div>
			</div>
			{/each}
		</div>
		
		<div class="btn" on:click={enableNotificationsTaxi}>
			{#if !taxiNotifies}
			
            <svg class="notify" xmlns="http://www.w3.org/2000/svg" width="15" height="14" viewBox="0 0 15 14" fill="none">
	<path d="M3.82784 2.16547L3.00534 1.33714C1.08325 3.24639 1.08325 4.63005 1.08325 6.41797H1.66659L2.24992 6.38122C2.24992 4.67322 2.24992 3.7323 3.82784 2.16547ZM11.9939 1.33714L11.1726 2.16547C12.7499 3.7323 12.7499 4.67322 12.7499 6.41797L13.9166 6.38122C13.9166 4.63005 13.9166 3.24639 11.9939 1.33714ZM7.49992 12.8346C7.86118 12.8351 8.2136 12.723 8.5082 12.5139C8.80281 12.3048 9.02495 12.0091 9.14375 11.668H5.85609C5.97489 12.0091 6.19703 12.3048 6.49163 12.5139C6.78624 12.723 7.13866 12.8351 7.49992 12.8346ZM11.5833 8.5098V5.83464C11.5833 3.95805 10.3087 2.37722 8.582 1.9018C8.41109 1.4713 7.99342 1.16797 7.49992 1.16797C7.00642 1.16797 6.58875 1.4713 6.41784 1.9018C4.69059 2.37722 3.41659 3.95805 3.41659 5.83464V8.5098L2.42084 9.50555C2.36656 9.55963 2.32352 9.62391 2.29418 9.69469C2.26485 9.76547 2.24981 9.84135 2.24992 9.91797V10.5013C2.24992 10.656 2.31138 10.8044 2.42077 10.9138C2.53017 11.0232 2.67854 11.0846 2.83325 11.0846H12.1666C12.3213 11.0846 12.4697 11.0232 12.5791 10.9138C12.6885 10.8044 12.7499 10.656 12.7499 10.5013V9.91797C12.75 9.84135 12.735 9.76547 12.7057 9.69469C12.6763 9.62391 12.6333 9.55963 12.579 9.50555L11.5833 8.5098Z"/>
</svg>
			{:else}
			<svg class="notify" xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 14 14" fill="none">
<path d="M11.868 11.0444C11.9797 11.004 12.0762 10.9302 12.1446 10.833C12.2129 10.7359 12.2497 10.6201 12.2501 10.5013V9.91797C12.2502 9.84135 12.2351 9.76547 12.2058 9.69469C12.1765 9.62391 12.1334 9.55963 12.0791 9.50555L11.0834 8.5098V5.83464C11.0834 3.95805 9.80881 2.37722 8.08215 1.9018C7.91123 1.4713 7.49356 1.16797 7.00006 1.16797C6.50656 1.16797 6.0889 1.4713 5.91798 1.9018C5.14623 2.11414 4.48123 2.5598 3.96615 3.14255L2.16248 1.33889L1.33765 2.16372L11.8376 12.6637L12.6625 11.8389L11.868 11.0444ZM7.00006 12.8346C7.36132 12.8351 7.71375 12.723 8.00835 12.5139C8.30295 12.3048 8.52509 12.0091 8.6439 11.668H5.35623C5.47503 12.0091 5.69717 12.3048 5.99178 12.5139C6.28638 12.723 6.6388 12.8351 7.00006 12.8346ZM2.91673 5.83464V8.5098L1.92098 9.50555C1.8667 9.55963 1.82366 9.62391 1.79433 9.69469C1.765 9.76547 1.74995 9.84135 1.75006 9.91797V10.5013C1.75006 10.656 1.81152 10.8044 1.92092 10.9138C2.03031 11.0232 2.17869 11.0846 2.3334 11.0846H8.67948L2.9564 5.36155C2.93831 5.51789 2.91673 5.67364 2.91673 5.83464Z"/>
</svg>
			{/if}
			<p>{taxiNotifies ? t('offNotify') : t('notify')}</p>
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