<script>
  import { translateText } from 'lang';
  import { executeClient, executeClientAsync } from 'api/rage';
  import { selectCase } from './state.js';
  import { format } from 'api/formatter';
  import { accountUnique } from 'store/account';
  import { ItemId } from 'json/itemsInfo.js';
  import { onMount } from 'svelte';

  // 1) Переменные
  let shopList    = [];
  let casesData   = [];
  let selectIndex = 0;
  let caseData    = [];
  let isLoad      = false;

  // 2) Загрузка с сервера
  executeClient("client.donate.load");
  executeClientAsync("donate.roulette.getList").then(result => {
    if (result && typeof result === "string") {
      const parsed    = JSON.parse(result);
      shopList        = parsed.shopList;
      casesData       = parsed.caseData;
      onSelectCases(0);
      isLoad          = true;
    }
  });

  // 3) Открытие кейса
  const onOpenCase = (index) => {
    selectCase.set(index);
    window.gameMenuView("Roulette", index);
  };

  // 4) При выборе категории подготавливаем caseData
  function onSelectCases(idx) {
    selectIndex = idx;
    const now   = Date.now();

    caseData = shopList[idx].cases
      .map(i => casesData.find(c => c.index === i))
      .filter(Boolean)
      .map(c => {
        // парсим даты или ставим дефолт
        const start = c.startCase ? new Date(c.startCase).getTime() : 0;
        const end   = c.endCase   ? new Date(c.endCase).getTime()   : 0;
        const hasSchedule = Boolean(c.startCase && c.endCase);

        // для тех, у кого есть расписание, считаем доступность и дни
        let available = true;
        let daysLeft  = 0;
        if (hasSchedule) {
          available = now >= start && now < end;
          daysLeft  = available
            ? Math.ceil((end - now) / (24*60*60*1000))
            : 0;
        }

        return {
          ...c,
          hasSchedule,
          available,
          daysLeft
        };
      });
  }

  // 5) Сортируем: доступные в начало
  $: sortedCaseData = [...caseData].sort((a, b) =>
    (b.available ? 1 : 0) - (a.available ? 1 : 0)
  );

  // 6) Если нужно авто-обновление дней — каждую минуту
  onMount(() => {
    const iv = setInterval(() => onSelectCases(selectIndex), 60000);
    return () => clearInterval(iv);
  });

  // 7) Цена (ваша логика)
  const getPrice = (price, index, unique) => {
    if (unique && unique.split("_")) {
      const [t, i, z] = unique.split("_").map((v,idx)=> idx>0?Number(v):v);
      if (t==="cases" && i===index && z===0) {
        return Math.round(price*0.7);
      }
    }
    return price;
  };
</script>

<style>
  /* ваш CSS */
  .pricecased { opacity: 0; }
  .expired  { color: #f55; }
</style>

<img class="imgd2" src="https://cdn.majestic-files.com/public/master/static/img/panelMenu/donate/cases/cases2.png" />

<div class="magazcase">
  {#if isLoad}
    {#each sortedCaseData as item}
      <div
        class="blockcase {item.available ? '' : 'usecaseb'}"
        on:click={() => onOpenCase(item.index)}
      >
        <div class="blockcase-info">
          <!-- Показываем таймер только если есть расписание и кейс ещё активен -->
          {#if item.hasSchedule && item.available}
            <div class="usecaseO">
              <svg width="22" height="23" viewBox="0 0 22 23" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M21 11.5652C21 6.28249 16.5228 2 11 2C5.47715 2 1 6.28249 1 11.5652V12.4348C1 17.7175 5.47715 22 11 22C16.5228 22 21 17.7175 21 12.4348V11.5652Z" fill="#3DE7A5"/>
                <path d="M11 7V13L14 15" stroke="black" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <p> Осталось: <b>{item.daysLeft}</b>
              {item.daysLeft === 1 ? ' день' : 'дней'}</p>
            </div>
          {/if}
          {#if item.hasSchedule && !item.available}
            <div class="usecaseb"></div>
          {/if}


          <!-- Всегда показываем, сколько у игрока этих кейсов -->
          <div class="usecase">
            <svg width="47" height="46" viewBox="0 0 47 46" fill="none" xmlns="http://www.w3.org/2000/svg">
              <rect x="13" y="13" width="20" height="20" rx="5" fill="#E81C5A"/>
              <path d="M21.1121 27.2C20.8359 27.1998 20.5709 27.0925 20.3757 26.9012L18.2924 24.8624C18.1026 24.6701 17.9978 24.4126 18 24.1453C18.0025 23.878 18.112 23.6224 18.3051 23.4332C18.4982 23.2442 18.7594 23.137 19.0326 23.1348C19.3057 23.1324 19.5688 23.2352 19.7652 23.4209L21.0532 24.6814L26.5706 18.3818C26.658 18.2752 26.7662 18.1867 26.8892 18.1219C27.0121 18.0572 27.147 18.0172 27.286 18.0044C27.425 17.9917 27.5652 18.0064 27.6983 18.0477C27.8314 18.0891 27.9546 18.1561 28.0607 18.245C28.1667 18.3338 28.2534 18.4426 28.3157 18.565C28.3779 18.6874 28.4144 18.8206 28.4229 18.957C28.4315 19.0935 28.4119 19.2302 28.3654 19.359C28.319 19.4878 28.2464 19.6062 28.1523 19.707L21.9026 26.8432C21.8057 26.9558 21.6847 27.0462 21.548 27.1078C21.4114 27.1696 21.2625 27.201 21.1121 27.2Z" fill="white"/>
              </svg>
           <p> У вас: <b>{window.getItemToCount(ItemId["Case"+item.index])}</b></p>
          </div>
        </div>

        <img
          src="http://cdn.piecerp.ru/cloud/img/cases/{item.image}.png"
          alt={item.name}
        />
        <div class="namecase">{item.name}</div>

        {#if item.price > 0}
        <div class="pricecase {item.available ? '' : 'pricecased'}">
            <p>{format("money", getPrice(item.price, item.index, $accountUnique))}</p>
            <svg id="Group_1326" data-name="Group 1326" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 22 22">
              <circle id="Ellipse_21" data-name="Ellipse 21" cx="11" cy="11" r="11" fill="#e81c5a"/>
              <path id="Path_6385" data-name="Path 6385" d="M6.193-3.412a2,2,0,0,0-.522,1.031c0,.378.183,1.318.692,1.318.326,0,.953-.744,1.148-.992A27.023,27.023,0,0,1,12.5-7.119l.026.026c-.914,1.188-2.571,3.145-2.571,4.711,0,.757.287,1.579,1.148,1.579,1.122,0,3.928-2.219,4.241-3.119l-.052-.5c-1.762,1.71-2.78,2.3-3.145,2.3-.313,0-.457-.378-.457-.639a5.748,5.748,0,0,1,1.5-2.962l2.023-2.467c.157-.2.561-.653.561-.914s-.548-1.122-.9-1.122c-.235,0-.5.274-.666.418C12.5-8.45,11-7.027,9.391-5.644L9.364-5.67c.535-.822,1.945-2.754,1.945-3.719a.936.936,0,0,0-.953-.9c-.535.013-1.644.992-2.127,1.449L6.154-6.832l-.026-.026a16.464,16.464,0,0,0,.809-2.271.967.967,0,0,0-.157-.5l-.313-.548c-.131-.222-.183-.4-.5-.418A11.83,11.83,0,0,0,3.5-9.677L2.03-9,2-9.024c.509-.444.692-.679.692-.757s-.091-.131-.157-.131a.81.81,0,0,0-.326.183c-1.161.822-1.083.8-1.2.953a7.615,7.615,0,0,0-.339,1.54c-.065.313.1.483.313.483.666,0,.039-.026,4.059-1.905a1.3,1.3,0,0,1,.4-.157c.222,0,.457.261-.914,2.714L1.808-2.655c.144.4.4,1.436.953,1.436.418,0,.744-.5.966-.8L5.684-4.652,8.816-8.045c.091-.1.248-.274.4-.274s.2.144-.026.561A19.4,19.4,0,0,1,7.081-4.574Z" transform="translate(3.129 16.731)" fill="#fff"/>
            </svg>  
          </div>
        {/if}
      </div>
    {/each}
  {/if}
</div>
