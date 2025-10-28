<script>
	import { onMount } from 'svelte';
import './css/calendar.css'
	import { createEventDispatcher } from 'svelte';
    const dispatch = createEventDispatcher();
let selectedYear = null;
	let years = [];
	let currentDate = new Date();

	const months = [
		"January", "February", "March", "April", "May", "June",
		"July", "August", "September", "October", "November", "December"
	];

	const shortMonths = [
		"Jan", "Feb", "Mar", "Apr", "May", "Jun",
		"Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
	];

	const weekdays = ["Mon", "Tues", "Wed", "Thurs", "Fri", "Sat", "Sun"];

	onMount(() => {
		const currentYear = currentDate.getFullYear();
		years = [currentYear, currentYear + 1, currentYear + 2, currentYear + 3];
	});

	function daysInMonth(year, month) {
		return new Date(year, month, 0).getDate();
	}

	function isCurrentYear(year) {
		return currentDate.getFullYear() === year;
	}

	function isCurrentMonth(year, month) {
		return isCurrentYear(year) && month === currentDate.getMonth();
	}

	function isCurrentDay(year, month, day) {
		return isCurrentMonth(year, month) && day === currentDate.getDate();
	}

	function getWeekdayIndent(month) {
		const indent = new Date(selectedYear, month - 1, 1).getDay();
		return indent === 0 ? 6 : indent - 1;
	}

	function scrollToCurrentDay() {
		const selector = selectedYear ? '.day__title.active' : '.day__number.active';
		const activeElement = document.querySelector(selector);
		if (activeElement) {
			activeElement.scrollIntoView({ block: 'center' });
		}
	}

	function selectYear(year, month = null) {
		selectedYear = year;
		
		// Если передан месяц, скроллим к нему после небольшой задержки
		if (month !== null) {
			setTimeout(() => {
				scrollToMonth(month);
			}, 100);
		}
	}

	function scrollToMonth(monthIndex) {
		// Находим элемент месяца и скроллим к нему
		const monthElements = document.querySelectorAll('.months.column-block .month');
		if (monthElements[monthIndex]) {
			monthElements[monthIndex].scrollIntoView({ 
				behavior: 'smooth',
				block: 'start'
			});
		}
	}

	function closeApp() {
		dispatch('close');
		console.log('Close app');
	}

	function closingStart() {
		// Логика начала закрытия
		console.log('Closing start');
	}
</script>

<div class="calendar full-width full-height">
	<main class="flex-block full-width full-height" class:year-selected={selectedYear !== null}>
		
		{#if selectedYear === null}
		<div class="years">
			{#each years as year}
			<div>
				<div class="year-tile column-block">
					<div class="year-tile__title" class:active={isCurrentYear(year)}>
						{year}
					</div>
					<div>
						<div class="months justify-between row-block">
							{#each Array(12) as _, monthIndex}
							<div 
								class="month column-block" 
								on:click={() => selectYear(year, monthIndex)}
							>
								<div class="month__title" class:active={isCurrentMonth(year, monthIndex)}>
									{months[monthIndex]}
								</div>
								<div class="days row-block">
									{#each Array(daysInMonth(year, monthIndex + 1)) as _, dayIndex}
									<div class="day">
										<div class="day__number" class:active={isCurrentDay(year, monthIndex, dayIndex + 1)}>
											{dayIndex + 1}
										</div>
										{#if isCurrentDay(year, monthIndex, dayIndex + 1)}
										<div class="is-current-day"></div>
										{/if}
									</div>
									{/each}
								</div>
							</div>
							{/each}
						</div>
					</div>
				</div>
			</div>
			{/each}
		</div>
		{/if}

		{#if selectedYear !== null}
		<div class="selected-year full-width">
			<header>
				<div class="back align-center row-block" on:click={() => selectYear(null)}>
					<svg xmlns="http://www.w3.org/2000/svg" width="8.5" height="15" viewBox="0 0 8.5 15">
  <path id="Arrow" d="M6.756,14.695.207,8.009a.73.73,0,0,1,0-1.018L6.756.305A1.008,1.008,0,0,1,8.2.305a1.06,1.06,0,0,1,0,1.475L2.6,7.5l5.6,5.718a1.061,1.061,0,0,1,0,1.476,1.008,1.008,0,0,1-1.445,0" fill="#ff3b30"/>
</svg>
					<span class="back__text">{selectedYear}</span>
				</div>
				<div class="weekdays justify-between row-block full-width">
					{#each weekdays as weekday}
					<div class="weekdays__title">{weekday}</div>
					{/each}
				</div>
			</header>
			<div class="months column-block full-width">
				{#each Array(12) as _, monthIndex}
				<div>
					<div class="column-block month indent-for-day-{getWeekdayIndent(monthIndex + 1)}">
						<div class="month__title" class:active={isCurrentMonth(selectedYear, monthIndex)}>
							{getWeekdayIndent(monthIndex + 1) === 0 ? shortMonths[monthIndex] : months[monthIndex]}
						</div>
						<div class="days row-block">
							{#each Array(daysInMonth(selectedYear, monthIndex + 1)) as _, dayIndex}
							<div class="day">
								<div class="day__title" class:active={isCurrentDay(selectedYear, monthIndex, dayIndex + 1)}>
									{dayIndex + 1}
								</div>
								{#if isCurrentDay(selectedYear, monthIndex, dayIndex + 1)}
								<div class="is-current-day"></div>
								{/if}
							</div>
							{/each}
						</div>
					</div>
				</div>
				{/each}
			</div>
		</div>
		{/if}

	</main>

	<footer class="full-width justify-center">
		<div class="today-button" on:click={scrollToCurrentDay}>
			Today
		</div>
		<div 
			class="close-block full-width" style="z-index: 2;"
			on:mousedown={closingStart}
			on:click={closeApp}
		>
			<div class="close-block-handler"></div>
		</div>
	</footer>
</div>
