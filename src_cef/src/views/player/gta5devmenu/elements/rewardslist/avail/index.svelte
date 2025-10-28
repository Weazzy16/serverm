<!-- src/routes/CalendarTasks/task/index.svelte -->
<script>
  import { onMount, onDestroy } from 'svelte';
  import AvailableAndDetail from './AvailableAndDetail.svelte';

  // сюда придёт массив задач от сервера
  // формат каждого элемента:
  // {
  //   taskId,
  //   botId, botName,
  //   title, description, questType,
  //   requirement: { title, count },
  //   progress, remaining, canTake
  // }
  let tasks = [];

  // ID выбранного задания
  let selectedId = null;

  // props для детали
  let detailProps = {
    avatar: '',
    botName: '',
    questType: '',
    title: '',
    difficulty: 0,
    descriptionLines: [],
    goals: [],
    onActivate: () => {}
  };

  function handleUpdate(e) {
    try {
      const arr = JSON.parse(e.detail);
      tasks = arr.map(x => ({
        taskId      : Number(x.taskId),
        botId       : Number(x.botId),
        botName     : x.botName,
        title       : x.title,
        description : x.description,
        questType   : x.questType,
        level       : Number(x.level),
        requirement : { title: x.requirement.title, count: Number(x.requirement.count) },
        progress    : Number(x.progress),
        remaining   : Number(x.remaining),
        canTake     : Boolean(x.canTake)
      }));
      console.log('[CEF] tasks payload:', arr);

      // если ничего не выбрано — сразу выделим первую
      if (selectedId === null && tasks.length > 0) {
        selectTask(tasks[0].taskId);
      } else {
        // иначе обновим деталь текущего
        selectTask(selectedId);
      }
    } catch (err) {
      console.error('calendar_tasks_update parse error', err);
    }
  }

  // выбор задания
  function selectTask(id) {
    selectedId = id;
    const def = tasks.find(t => t.taskId === id);
    if (!def) return;
    detailProps = {
      avatar          : `https://cdn.majestic-files.com/public/master/static/img/ipad/families/bots/${def.botId}.png`,
      botName         : def.botName,
      questType       : def.questType,
      title           : def.title,
      difficulty      : def.requirement.count,
      level      : def.level,
      descriptionLines: [def.description],
      goals           : [{
        text   : def.requirement.title,
        current: def.progress,
        total  : def.requirement.count
      }],
      onActivate: () => {
        if (def.canTake) {
          mp.events.call('client.calendar.take', def.taskId);
        }
      }
    };
  }

  onMount(() => {
    window.addEventListener('calendar_tasks_update', handleUpdate);
    mp.events.call('client.calendar.request');
  });

  onDestroy(() => {
    window.removeEventListener('calendar_tasks_update', handleUpdate);
  });
</script>


<div class="calendar-tasks">
  <AvailableAndDetail
    avatar={detailProps.avatar}
    botName={detailProps.botName}
    questType={detailProps.questType}
    title={detailProps.title}
    level={detailProps.level}
    difficulty={detailProps.difficulty}
    descriptionLines={detailProps.descriptionLines}
    goals={detailProps.goals}
    onActivate={detailProps.onActivate}
  >
    <div slot="list">
      {#each tasks as def (def.taskId)}
        <div
          class="availableQuest full-width row-block align-center justify-start
                 {selectedId === def.taskId ? 'selected' : ''}
                 {def.canTake ? '' : 'taken'}"
          on:click={() => selectTask(def.taskId)}
        >
          <img
            src={`https://cdn.majestic-files.com/public/master/static/img/ipad/families/bots/${def.botId}.png`}
            alt={def.botName}
            class="botAvatar"
          />
          <div class="column-block align-start justify-center">
            <div class="title">{def.title}</div>
            <div class="bot row-block align-center justify-start">
            <span class="botName">  {def.botName}
            </span>— Ежедневное задание
            </div>
          </div>
        </div>
      {/each}
    </div>
  </AvailableAndDetail>
</div>

<style>
  .calendar-tasks { display: flex; height: 100%; width: 100%; }
  .task-item {
    display: flex;
    align-items: center;
    padding: 0.5rem;
    cursor: pointer;
    border-bottom: 1px solid #222;
    transition: background .2s;
  }
  .task-item:hover { background: #2a2a2a; }
  .task-item.selected { background: #333; }
  .task-item.taken { opacity: 0.5; }
  .bot-thumb {
    width: 32px;
    height: 32px;
    margin-right: 0.75rem;
    border-radius: 4px;
  }
  .info .title { font-weight: bold; color: #fff; }
  .info .status { font-size: 0.85rem; color: #aaa; }
</style>
