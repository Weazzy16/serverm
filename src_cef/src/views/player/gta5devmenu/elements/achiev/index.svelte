<script>
  import { onMount, onDestroy, tick } from 'svelte';
    import './achiv.css';
  import './achiv2.css';
  import Award from './Award.svelte';
  import { achievementsList } from './achievementsList.js';

  // Входные данные (можно передать извне)
  export let data = {};

  // Локальный демонстрационный набор, если data пуст
  const dataDemo = {
    questsList: [
      { id: 'allTypes', title: 'Весь список', difficulty: 1 },
      { id: 'general',  title: 'Общие',     difficulty: 2 },
      { id: 'fun',      title: 'Развлечения',difficulty: 1 },
      { id: 'fin',      title: 'Финансы',   difficulty: 2 },
      { id: 'org',      title: 'Организация',difficulty: 3 },
      { id: 'fish',     title: 'Рыбалка',   difficulty: 4 }
    ],
    goalsList: achievementsList.map(a => ({
      id:              a.id,
      byQuest:         a.byQuest,
      title:           a.title,
      additionalTitle: a.additionalTitle,
      type:            a.type,
      completedvalue:  a.completedValue,
      targetValue:     a.targetValue,
      award:           a.award
    }))
  };

  // Выбираем реальные или демонстрационные данные
  $: localData = Object.keys(data).length ? data : dataDemo;

  // Состояние вкладок и прокрутки
  let selectedQuest   = 'allTypes';
  let questsContainer = null;

  $: questsList       = localData.questsList;
  $: goalsList        = localData.goalsList;
  $: currentGoalsList = selectedQuest === 'allTypes'
    ? goalsList
    : goalsList.filter(g => g.byQuest === selectedQuest);

  $: selectedQuestIndex = questsList.findIndex(q => q.id === selectedQuest);
  $: leftArrowVisible   = selectedQuestIndex > 0;
  $: rightArrowVisible  = selectedQuestIndex < questsList.length - 1;

  function selectQuest(id) {
    selectedQuest = id;
    scrollToCurrentQuest();
  }

  function prevQuest() {
    selectedQuest = leftArrowVisible
      ? questsList[selectedQuestIndex - 1].id
      : questsList[questsList.length - 1].id;
    scrollToCurrentQuest();
  }

  function nextQuest() {
    selectedQuest = rightArrowVisible
      ? questsList[selectedQuestIndex + 1].id
      : questsList[0].id;
    scrollToCurrentQuest();
  }

  function scrollToCurrentQuest() {
    tick().then(() => {
      const items  = Array.from(questsContainer.querySelectorAll('.quests-list__item'));
      const current = items[selectedQuestIndex];
      if (current) {
        questsContainer.scrollTo({ left: current.offsetLeft, behavior: 'smooth' });
      }
    });
  }

  // Получение награды
function getAward(id) {
  const numeric = typeof id === 'string' && id.startsWith('a')
    ? +id.slice(1)
    : id;
  console.log('[Achiv] → dispatch requestAchievementReward', numeric);
  window.dispatchEvent(new CustomEvent('requestAchievementReward', { detail: numeric }));
}




  // Обработка прихода данных от сервера
  function handleShow(e) {
    const serverData = e.detail;  // [{id, current, target, completed, rewarded}, ...]
    console.log('[Achiv] ← showAchievements', serverData);

    for (const srv of serverData) {
      const goal = localData.goalsList.find(g => g.id === srv.id);
      if (!goal) continue;

      goal.completedvalue = srv.current;

      // типы:
      // < target    → inprogress (скрываем кнопку)
      // >= target && !rewarded → received (показываем «Получить»)
      // >= target && rewarded  → completed (показываем «Выполнено»)
      if (srv.current < srv.target) {
        goal.type = 'inprogress';
      } else if (!srv.rewarded) {
        goal.type = 'received';
      } else {
        goal.type = 'completed';
      }
    }

    // форсим реактивность
    localData = { ...localData };
  }

  onMount(() => {
    // 1) Подписка на приход результатов
    window.addEventListener('showAchievements', handleShow);

    // 2) Мост CEF→JS: запрос ачивок
    window.addEventListener('requestAchievements', () => {
  console.log('[CEF-Bridge] requestAchievements → client.achievements.request');
      mp.events.call('client.achievements.request');
    });

    // 3) Мост CEF→JS: запрос выдачи награды
    window.addEventListener('requestAchievementReward', e => {
  console.log('[CEF-Bridge] requestAchievementReward →', e.detail);
  mp.events.call('client.achievements.getAward', e.detail);
});


    // Сразу шлём запрос за актуальными данными
    console.log('[Achiv] → requestAchievements');
    window.dispatchEvent(new CustomEvent('requestAchievements'));
  });

  onDestroy(() => {
    window.removeEventListener('showAchievements', handleShow);
    window.removeEventListener('requestAchievements', () => {});
  });
</script>


<div class="menuWrapper-main">
  <div class="achievements full-width full-height column-block align-start justify-start">
    <!-- Quests selector -->
    <div class="quests full-width row-block align-center justify-between">
      <div class="slide-btn prev full-height" on:click={prevQuest} class:disabled={!leftArrowVisible}>
       <svg xmlns="http://www.w3.org/2000/svg" width="8.591" height="15.061" viewBox="0 0 8.591 15.061">
  <path d="M4387.549-1114.65l-7,7,7,7" transform="translate(-4379.488 1115.18)" fill="none" stroke-width="1.5"/>
</svg>
      </div>
      <div class="quests-listWrapper row-block align-center justify-start full-height">
        <div bind:this={questsContainer} class="quests-list row-block align-center justify-start full-height">
          {#each questsList as quest (quest.id)}
            <div
              class="quests-list__item column-block align-start justify-between full-height {quest.id === selectedQuest ? 'active' : ''}"
              on:click={() => selectQuest(quest.id)}
            >
              <div class="title">{quest.title}</div>
              <div class="difficulty row-block align-center justify-start">
                <span class="difficulty-text">Сложность:</span>
                {#each Array(3) as _, i}
                  <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 13 13">
	<g id="Группа_масок_353" data-name="Группа масок 353" transform="translate(-321 -50)" clip-path="url(#clip-path)">
		<g id="star" transform="translate(320.183 49.172)">
			<path id="Контур_16276" data-name="Контур 16276" d="M13.679,6.288,10.83,9.065l.673,3.922a.457.457,0,0,1-.664.482L7.317,11.617,3.8,13.469a.457.457,0,0,1-.664-.482L3.8,9.065.955,6.288a.457.457,0,0,1,.254-.78l3.937-.572L6.907,1.368a.476.476,0,0,1,.82,0L9.488,4.936l3.937.572a.457.457,0,0,1,.254.78Z"/>
		</g>
	</g>
</svg>
                {/each}
              </div>
            </div>
          {/each}
        </div>
      </div>
      <div class="slide-btn next full-height" on:click={nextQuest} class:disabled={!rightArrowVisible}>
       <svg xmlns="http://www.w3.org/2000/svg" width="8.591" height="15.061" viewBox="0 0 8.591 15.061">
  <path d="M4387.549-1114.65l-7,7,7,7" transform="translate(-4379.488 1115.18)" fill="none" stroke-width="1.5"/>
</svg>
      </div>
    </div>

    <!-- Goals and awards -->
    <div class="goals-list row-block align-start justify-start">
      {#each currentGoalsList as goal (goal.id)}
        <div class="goals-list__item column-block justify-between">

          <div class="goalHeader row-block justify-between align-center">
            <div class="goalHeader-main row-block align-center">
              <!-- Group input+svg in one label -->
              <label class="goalHeader-main__checkbox">
                <input
                  type="checkbox"
                  checked={goal.completedvalue >= goal.targetValue}
                  on:click|preventDefault
                />
                {#if goal.completedvalue >= goal.targetValue}
                 <svg
                    viewBox="0 0 14 11"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      d="M1 5.18513L4.8748 9L13 1"
                      stroke="white"
                      stroke-width="1.91176"
                    />
                  </svg>
                {/if}
              </label>
               <p>
                
                {goal.title}
                {#if goal.additionalTitle}
                  <span>{goal.additionalTitle}</span>
                {/if}
              </p>
            </div>
            

            {#if goal.type !== 'completed'}
              <p class="goalHeader-progress">{goal.completedvalue} / {goal.targetValue}</p>
            {/if}
          </div>

          <div class="goals-list__item__award">
            <Award award={goal.award} />
          

    
            {#if goal.type === 'received'}
  <button
    class="background row-block align-center justify-center full-width full-height"
    on:click|stopPropagation={() => getAward(goal.id)}
    style="pointer-events: auto;"
  >
    <div class="received row-block align-center justify-center">
      <div class="text">Получить</div>
    </div>
  </button>
            {:else if goal.type === 'completed'}
             <div class="background row-block align-center justify-center full-width full-height">
              <div class="completed column-block align-center justify-center">
                <div class="icon row-block align-center justify-center">
                <svg  viewBox="0 0 14 11" fill="none" xmlns="">
                    <path
                      d="M1 5.18513L4.8748 9L13 1"
                      stroke="white"
                      stroke-width="1.91176"
                    />
                  </svg>
                  </div>
                <div class="text">Выполнено</div>
              </div>
              </div>
            {/if}
          </div>
          </div>
 
      {/each}
    </div>
  </div>
</div>
