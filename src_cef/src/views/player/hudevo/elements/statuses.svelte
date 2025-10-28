<script>
  import { onMount, onDestroy } from 'svelte';
  
  let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  
  let buffs = [];
  let cursorVisible = false;
  let saveItemId = null;
  let descriptionModal = null;
  let timerInterval = null;
  
  // Переводы
  const translations = {
    Hunger: { name: "Голод", desc: "Вам необходимо поесть." },
    Dehydration: { name: "Жажда", desc: "Вам необходимо попить воды." },
    Drunk: { name: "Опьянение", desc: "Мир вокруг кажется расплывчатым." },
    Covid: { name: "Covid", desc: "Лёгкие горят, кости ноют." },
    Cold: { name: "Простуда", desc: "Туман в голове, ломота в теле." },
    Poisoning: { name: "Отравление", desc: "Токсичная смесь в желудке." },
    Diarrhea: { name: "Диарея", desc: "Живот скручивает от боли." },
    Agaric: { name: "Мухоморная эйфория", desc: "" },
    Green: { name: "Зелёная эйфория", desc: "" },
    Blue: { name: "Голубая эйфория", desc: "" },
    White: { name: "Белая эйфория", desc: "" },
    HealCapsule: { name: "Лечение", desc: "" },
    Styling: { name: "Клейкие волосы", desc: "Нельзя побрить." },
    GovernorAppreciation: { name: "Повышенная ставка", desc: "Увеличение зарплаты." },
    Biolink: { name: "Анабиоз", desc: "Избавление от жажды и голода." },
    Bioadditive: { name: "Нейронные связи", desc: "Увеличение скорости улучшения навыков." },
    MagicCandyBlack: { name: "Эльфийская злоба", desc: "Превращение в злого эльфа." },
    MagicCandyWhite: { name: "Эльфийская Радость", desc: "Превращение в доброго эльфа." },
    Mutesound: { name: "Без звука", desc: "Вы не слышите звуки вокруг." },
    Mutemicrophone: { name: "Не слышу", desc: "" },
    Ooc: { name: "Не игровой режим", desc: "" },
    Gunban: { name: "Бессилие", desc: "Вы не можете держать оружие." },
    Jammergreen: { name: "Союзные помехи", desc: "Вы в одной организации с владельцем глушилки." },
    Jammerred: { name: "Чужие помехи", desc: "Нельзя использовать средства связи." },
    EnergyDrink: { name: "Ускорение", desc: "Ускорение передвижения." },
    Overload: { name: "Перевес", desc: "Вы не можете быстро передвигаться." },
    Cuffed: { name: "Скованность", desc: "Запястья окованы." },
    Tied: { name: "Скованность", desc: "Запястья окованы." },
    Jail: { name: "Тюрьма", desc: "Вы находитесь в тюрьме." },
    Demorgan: { name: "Деморган", desc: "Вы находитесь в деморгане!" },
    Fire: { name: "Горение", desc: "" },
    Godmode: { name: "Бог", desc: "Вы неуязвимы." },
    Invisibility: { name: "Невидимость", desc: "Вы невидимы." },
    Fracture: { name: "Перелом", desc: "Вы не можете быстро двигаться." },
    Shocker: { name: "Электричество", desc: "" },
    DispellDebuffs: { name: "Противоядие", desc: "Избавление от негативных эффектов." },
    ResetStamina: { name: "Второе дыхание", desc: "Полное восстановление выносливости." },
    Taxes: { name: "Льготы", desc: "Уменьшение налогов." },
    ProteinBar: { name: "Прилив сил", desc: "Увеличение скорости улучшения навыков." },
    Overdose: { name: "Передозировка", desc: "Полная перегрузка тела." },
    Vacine: { name: "Вакцина", desc: "Защита от всех болезней." },
    TapedMouth: { name: "Немота", desc: "Ограничение способности говорить." },
    TiedLegs: { name: "Неподвижность", desc: "Ограничение передвижения." },
    Contusion: { name: "Контузия", desc: "Временная утрата зрения." },
    Deafness: { name: "Глухота", desc: "Снижение способности слышать." },
    DeafnessStrong: { name: "Сильная глухота", desc: "Временная неспособность передвигаться." },
    BlockWeapon: { name: "Запрещённое оружие", desc: "Данным оружием нельзя пользоваться!" },
    Instinct: { name: "Чутьё", desc: "" },
  };
    import SVGComponent from './SVGComponent.svelte';

  // API
  if (typeof window !== 'undefined') {
    window.hudStore = window.hudStore || {};
    window.hudStore.updateStatuses = (data) => {
      if (data.buffs) {
        buffs = data.buffs.map(buff => ({
          ...buff,
          title: translations[buff.id]?.name || buff.id,
          desc: translations[buff.id]?.desc || ''
        }));
      }
      if (data.cursorVisible !== undefined) cursorVisible = data.cursorVisible;
    };
  }
  
  $: if (saveItemId) {
    descriptionModal = buffs.find(item => item.id === saveItemId);
  } else {
    descriptionModal = null;
  }
  
  $: isItemExists = buffs.some(item => item.id === saveItemId);
  
  function handleMouseover(id) {
    if (!saveItemId && cursorVisible) saveItemId = id;
  }
  
  function handleMouseout() {
    saveItemId = null;
  }
  
  function getLeftPercent(item) {
    if (!item || -1 === Number(item.endTime)) return 100;
    const now = Date.now();
    const timeLeft = new Date(item.endTime) - now;
    if (!item.endTime || timeLeft <= 0) return 0;
    return (timeLeft / item.duration) * 100;
  }
  
  function formatTime(endTime) {
    if (!endTime || -1 === Number(endTime)) return null;
    const now = Date.now();
    const diff = new Date(endTime) - now;
    if (diff <= 0) return "0с.";
    
    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);
    
    let result = '';
    if (days > 0) result += `${days}д. `;
    if (hours > 0) result += `${hours}ч. `;
    if (minutes > 0) result += `${minutes}м. `;
    result += `${seconds}с.`;
    return result.trim();
  }
  
  $: timeDisplay = descriptionModal ? formatTime(descriptionModal.endTime) : null;
  
  function rgbaToString(color) {
    if (!color || !Array.isArray(color)) return 'rgba(255, 255, 255, 0.5)';
    return `rgba(${color[0]}, ${color[1]}, ${color[2]}, ${color[3]})`;
  }
  
  onMount(() => {
    timerInterval = setInterval(() => {
      buffs = [...buffs];
    }, 1000);
  });
  
  onDestroy(() => {
    if (timerInterval) clearInterval(timerInterval);
  });
</script>

<div class="statuses">
  {#if isItemExists && cursorVisible && descriptionModal}
    <div class="description-block">
      <div class="header">
        <div class="title">
          
          <SVGComponent 
  path="icons/main/hud/statuses/{descriptionModal.icon}.svg" 
   class="status-icon"
/>
          <p>{descriptionModal.title}</p>
        </div>
        {#if descriptionModal.desc}
          <div class="desc">
            <p>{descriptionModal.desc}</p>
          </div>
        {/if}
      </div>
      
      {#if -1 !== Number(descriptionModal.endTime) && timeDisplay}
        <div class="timer">
          <div class="timer__title">Действует</div>
          <div class="timer__value">{timeDisplay}</div>
        </div>
      {/if}
    </div>
  {/if}
  
  <div class="list">
    {#each buffs as item (item.id)}
      <div 
        class="itembuf"
        style="opacity: {!saveItemId || saveItemId === item.id ? '1' : '0.5'}"
        on:mouseenter={() => handleMouseover(item.id)}
        on:mouseleave={handleMouseout}
      >
        <div class="main">
          <SVGComponent 
  path="icons/main/hud/statuses/{item.icon}.svg" 
  class="buff-icon"
/>
          <div 
            class="status-background"
            style="height: {getLeftPercent(item)}%; background-color: {rgbaToString(item.bgColor)}"
          ></div>
        </div>
        
        {#if item.stack > 1}
          <div class="stack">
            <p>{item.stack}</p>
          </div>
        {/if}
      </div>
    {/each}
  </div>
</div>