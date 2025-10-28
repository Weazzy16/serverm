<script>
  import { translateText } from 'lang';
  import './mains.css';

  import KeyAnimation from '@/components/keyAnimation/index.svelte';
  import { getQuest, getActors } from 'json/quests/quests.js';
  import { format } from 'api/formatter';
  import { storeQuests, selectQuest } from 'store/quest';
  import { executeClient } from 'api/rage';

  export let visible;

  let quests = [];
  let oldData = [];
  let selected = null;
  const numberOfVisibleCards = 3;
  let cardPosition = 0;

  // Подписка на storeQuests
  storeQuests.subscribe(data => {
    if (!data || data === oldData) return;
    oldData = data;
    executeClient("client.quest.selectQuest.Clear");

    quests = data
      .map(qd => {
        const info = getQuest(qd.ActorName, qd.Line);
        if (
          info &&
          info.Title &&
          info.Desc &&
          info.Tasks &&
          info.Tasks[qd.Stage]
        ) {
          return {
            ActorName: qd.ActorName,
            Title: info.Title,
            Desc: info.Desc,
            Tasks: info.Tasks,
            Reward: info.Reward,
            Stage: qd.Stage,
            currentLevel: info.Level || 0,
            current: info.Current || 0,
            nextLevel: info.Next || 0
          };
        }
        return null;
      })
      .filter(Boolean);

    if (!selected && typeof $selectQuest === "string") {
      const idx = quests.findIndex(q => q.ActorName === $selectQuest);
      if (idx !== -1) selected = quests[idx];
    }
  });

  function onSelect(q) {
    selected = q;
    selectQuest.set(q.ActorName);
    executeClient("client.quest.selectQuest", q.ActorName);
  }

  function onRoute(q) {
    if (q) executeClient("client.quest.router", q.ActorName);
  }

  function slidePrev() {
    if (cardPosition > 0) cardPosition--;
  }
  function slideNext() {
    if (cardPosition < quests.length - numberOfVisibleCards) cardPosition++;
  }

  function calcWidth(q) {
    if (q.currentLevel >= 5) return 100;
    return (q.currentLevel / 5) * 100 + (q.current / q.nextLevel) * 20;
  }
  function calcTasksProgress(q) {
    if (!q.Tasks || q.Tasks.length === 0) return 0;
    // если Stage — это индекс текущего (еще не выполненного) шага,
    // то выполнено q.Stage штук
    const done = q.Stage;
    return Math.round((done / q.Tasks.length) * 100);
  }
</script>

<svelte:window on:keyup={(e) => {
  if (!visible || !selected) return;
  if (e.keyCode === 37) slidePrev();
  if (e.keyCode === 39) slideNext();
  if (e.keyCode === 13) onRoute(selected);
  if (e.keyCode === 90) onSelect(selected);
}} />

{#if quests.length}
  <div class="menuWrapper-mainq row-block full-height">
    <div
      class="swiper row-block full-height"
      style="margin-left: -{cardPosition * (100 / numberOfVisibleCards)}%;"
    >
      {#each quests as q}
        <div class="swiper-slide full-height">
          <div class="quest column-block align-start justify-start full-height">
            <div
              class="botAvatar"
              style="background-image: url('http://gta5dev.online/heone/f2menu/{q.ActorName}.png')"
            ></div>

            <div class="title">{q.Title}</div>
            <div class="bot row-block align-center justify-start">
              <span class="botName">{getActors(q.ActorName).name}</span>
              <span class="questType">— Квестовое задание</span>
            </div>

            <div class="progressBar row-block align-center justify-start">
              <div class="progressBar--back full-width">
                <div
                  class="progressBar--value"
                  style="width: {calcTasksProgress(q)}%;"
                ></div>
              </div>
              <span class="progressBar--percent">
                {calcTasksProgress(q)}%
              </span>
            </div>

            <div class="info row-block align-center justify-start">
              <div class="difficulty row-block align-center justify-start">
                <span class="text">Сложность:</span>
                {#each Array(3) as _, idx}
                  <svg
                    class="icon"
                    class:active={idx < q.currentLevel}
                    viewBox="0 0 13 13"
                  >
                    <path
                      d="M13.679,6.288,10.83,9.065l.673,3.922a.457.457,0,0,1-.664.482L7.317,11.617,3.8,13.469a.457.457,0,0,1-.664-.482L3.8,9.065.955,6.288a.457.457,0,0,1,.254-.78l3.937-.572L6.907,1.368a.476.476,0,0,1,.82,0L9.488,4.936l3.937.572a.457.457,0,0,1,.254.78Z"
                    />
                  </svg>
                {/each}
              </div>
            </div>

            <div class="description column-block align-start justify-start">
              <div class="description--title">Цели:</div>
              <div class="description column-block align-start justify-start">
                {#if !q.Tasks || q.Tasks.length === 0}
                  <p>— нет задач на этом этапе</p>
                {:else}
                  {#each q.Tasks as task, i}
                    <div
                      class="description--title row-block align-center justify-start innerElem"
                    >
                      <div class="button" style="margin-top: 0.2vh;">
            {#if i < q.Stage}
              <!-- выполнено -->
              <svg class="done" width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect width="21" height="21" rx="2" fill="#E81C5B"/>
                <path d="M8.28372 15C7.99225 14.9999 7.71274 14.8948 7.50667 14.7078L5.30852 12.7132C5.10832 12.5251 4.99754 12.2732 5.00004 12.0117C5.00255 11.7502 5.11813 11.5001 5.32191 11.3152C5.52569 11.1303 5.80135 11.0254 6.08952 11.0231C6.3777 11.0209 6.65533 11.1214 6.86261 11.303L8.22162 12.5362L14.0434 6.37347C14.1356 6.26918 14.2499 6.18276 14.3796 6.11935C14.5092 6.05594 14.6517 6.01683 14.7983 6.00436C14.945 5.99188 15.0929 6.00628 15.2333 6.04671C15.3738 6.08714 15.5038 6.15277 15.6157 6.2397C15.7276 6.32663 15.8191 6.43309 15.8848 6.55275C15.9504 6.67241 15.9889 6.80282 15.9979 6.93624C16.007 7.06966 15.9863 7.20337 15.9373 7.32941C15.8882 7.45545 15.8117 7.57125 15.7124 7.66994L9.11791 14.6509C9.01567 14.7612 8.88792 14.8496 8.74374 14.9099C8.59955 14.9702 8.44249 15.001 8.28372 15Z" fill="white"/>
              </svg>
            {:else}
              <!-- не выполнено -->
              <svg class="undone" width="21" height="21" viewBox="0 0 21 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect width="21" height="21" rx="2" fill="white" fill-opacity="0.19"/>
              </svg>
            {/if}
          </div>
                      <span class="description--text">{task}</span>
                    </div>
                  {/each}
                {/if}
              </div>
            </div>

            <div
              class="control-buttons row-block align-center justify-between full-width"
              style="margin-top: auto;"
            >
              <div
                class="control-button cancel row-block align-center justify-center full-width"
                on:click={() => onSelect(q)}
              >
                <span>Взять квест</span>
              </div>
              <div
                class="control-button track row-block align-center justify-center full-width"
                on:click={() => onRoute(q)}
              >
                <span>Построить маршрут</span>
              </div>
            </div>
          </div>
        </div>
      {/each}
    </div>

    <div
      class="background
        {cardPosition > 0 && cardPosition < quests.length - numberOfVisibleCards
          ? 'all'
          : cardPosition > 0
          ? 'left'
          : cardPosition < quests.length - numberOfVisibleCards
          ? 'right'
          : ''}"
    ></div>

    {#if quests.length > numberOfVisibleCards}
      <div class="arrow-buttons">
        <div
          class="button"
          on:click={slidePrev}
          style="opacity: {cardPosition > 0 ? 1 : 0}"
        >
          ←
        </div>
        <div
          class="button revert"
          on:click={slideNext}
          style="opacity:
            {cardPosition < quests.length - numberOfVisibleCards ? 1 : 0}"
        >
          ←
        </div>
      </div>
    {/if}
  </div>
{:else}
  <div class="questmenu">
    <div class="questleft"></div>
    <div class="questright"></div>
  </div>
{/if}
