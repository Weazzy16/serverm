<script>
  import SVGComponent from './SVGComponent.svelte';
  import './ava.css';

  // По умолчанию — пустые значения, чтобы не было ошибок «не определено»
  export let avatar = '';
  export let botName = '';
  export let questType = '';
  export let title = '';

  export let descriptionLines = [];
  export let goals = [];           // обязательно экспортируем
 export let onActivate = () => {};

  // Защитная функция на случай, если кто-то забудет передать onActivate
  function activate() {
    try {
      onActivate();
    } catch (e) {
      console.error('AvailableAndDetail: onActivate error', e);
    }
  }
  export let level = 0;
  // Всегда три звезды
  const maxStars = 3;
</script>

<div class="menuWrapper-main1">
  <div class="available full-width full-height row-block align-center justify-between">

    <!-- Левая колонка: список заданий -->
    <div class="list full-height column-block align-center justify-start">
      <div class="list-scroll full-width full-height column-block align-center justify-start disabled">
        <div class="vue-recycle-scroller ready direction-vertical table-view full-width full-height column-block">
          <slot name="list"></slot>
        </div>
      </div>
    </div>

    <!-- Правая колонка: детали выбранного задания -->
    <div class="selectedQuest full-height column-block align-start justify-start">
      <div class="selectedQuest--header full-width row-block align-center justify-start">
        <div class="botAvatar">
          {#if avatar}
            <img class="botAvatar" src={avatar} alt="Bot Avatar" />
          {/if}
        </div>

        <div class="column-block align-start justify-center">
          <div class="title">{title}</div>
          <div class="bot row-block align-center justify-start">
            <span class="botName">{botName}
            <span>—</span>
            <span class="questType">{questType}</span>
          </div>
        </div>

        <div class="difficulty row-block align-center justify-start">
          <span class="text">Сложность:</span>
  {#each Array(maxStars) as _, i}
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 13 13"  class="star {i < level ? 'filled' : 'empty'}">
	<g id="Группа_масок_353" data-name="Группа масок 353" transform="translate(-321 -50)" clip-path="url(#clip-path)">
		<g id="star" transform="translate(320.183 49.172)">
			<path fill="currentColor" id="Контур_16276" data-name="Контур 16276" d="M13.679,6.288,10.83,9.065l.673,3.922a.457.457,0,0,1-.664.482L7.317,11.617,3.8,13.469a.457.457,0,0,1-.664-.482L3.8,9.065.955,6.288a.457.457,0,0,1,.254-.78l3.937-.572L6.907,1.368a.476.476,0,0,1,.82,0L9.488,4.936l3.937.572a.457.457,0,0,1,.254.78Z"/>
		</g>
	</g>
</svg>
          {/each}
        </div>
      </div>

      <div class="contentWrapper column-block align-start justify-start full-width">
        <div class="content column-block align-start justify-start full-width">
          <div class="description column-block align-start justify-start">
            <div class="description--title">Описание:</div>
            <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 67 67">
	<g id="Группа_масок_354" data-name="Группа масок 354" transform="translate(-1118 -274)" clip-path="url(#clip-path)">
		<g id="document_1_" data-name="document (1)" transform="translate(1118 274)">
			<path id="Контур_16277" data-name="Контур 16277" d="M43.184,19.629a4.585,4.585,0,0,1-4.58-4.58V0H15.18a7.205,7.205,0,0,0-7.2,7.2V59.8a7.205,7.205,0,0,0,7.2,7.2H51.82a7.205,7.205,0,0,0,7.2-7.2V19.629ZM18.713,47.109h9.516a1.963,1.963,0,1,1,0,3.926H18.713a1.963,1.963,0,0,1,0-3.926ZM16.75,38.6a1.963,1.963,0,0,1,1.963-1.963H47.5a1.963,1.963,0,0,1,0,3.926H18.713A1.963,1.963,0,0,1,16.75,38.6ZM47.5,26.172a1.963,1.963,0,0,1,0,3.926H18.713a1.963,1.963,0,0,1,0-3.926Z"/>
			<path id="Контур_16278" data-name="Контур 16278" d="M42.529,15.049a.655.655,0,0,0,.654.654H58.143a7.18,7.18,0,0,0-1.376-1.8L44.149,1.969A7.215,7.215,0,0,0,42.529.814V15.049Z"/>
		</g>
	</g>
</svg>
            {#each descriptionLines as line}
              <div class="description--text">{line}</div>
            {/each}
          </div>

          <div class="goals column-block align-start justify-start" type="requirements">
            <svg class="icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 67 67">
	<g id="Группа_масок_357" data-name="Группа масок 357" transform="translate(-1138 -272)" clip-path="url(#clip-path)">
		<path id="hand" d="M29.83,22.119a.827.827,0,0,0,.266.044H48.327a3.513,3.513,0,1,1,0,7.026H35.317a.824.824,0,0,0,0,1.648h13.01a5.158,5.158,0,0,0,4.95-6.615l7.743-7.254a3.514,3.514,0,1,1,4.964,4.974L50.761,38.5a2.449,2.449,0,0,1-1.895.832h-21.6a2.79,2.79,0,0,0-1.851.68l-.393.333L11.812,25.369a18.882,18.882,0,0,1,18.017-3.25ZM0,32.822a1.206,1.206,0,0,0,.3.88L15.251,50.648a1.216,1.216,0,0,0,1.714.108L24.514,44.1a1.218,1.218,0,0,0,.107-1.714L9.675,25.436a1.217,1.217,0,0,0-1.714-.107L.411,31.987A1.207,1.207,0,0,0,0,32.822ZM16.628,47.03a2.209,2.209,0,1,1,2.209-2.209,2.209,2.209,0,0,1-2.209,2.209Zm0-2.771a.562.562,0,1,1-.561.562A.562.562,0,0,1,16.628,44.26Z" transform="translate(1137.999 271.999)" fill-rule="evenodd"/>
	</g>
</svg>
            <div class="goals--title innerElem">Цели:</div>

            {#if goals && goals.length > 0}
              {#each goals as { text, current, total }, idx}
                <div class="goal row-block align-center justify-start innerElem">
                  <div class="checkbox">
                    {#if current >= (total ?? Infinity)}
                      <!-- отмечаем галочкой, если выполнено -->
                      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 12 10">
                        <path d="M1814.956,295.8l3.229,3.338,6.771-7"
                              transform="translate(-1814.058 -291.268)"
                              fill="none" stroke="#f8f8f8" stroke-width="2.5" />
                      </svg>
                    {/if}
                  </div>
                  <span class="goal--title" style="color: rgb(147, 147, 147);">{text}</span>
                  {#if total !== undefined}
                    <span class="goal--progress--current"> {current}</span><span class="goal--progress">/</span><span class="goal--progress--current goal--progress--max" >{total}</span>
                  {/if}
                </div>
              {/each}
            {:else}
              <div class="innerElem">Целей нет</div>
            {/if}

          </div>
        </div>
      </div>

      
    </div>
  </div>
</div>

<style>
  /* Если у вас весь CSS уже лежит в ava.css, здесь можно не дублировать ничего */
</style>
