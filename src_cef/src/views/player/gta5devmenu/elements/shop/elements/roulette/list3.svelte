<script>
  import { translateText } from 'lang';
  import { executeClient, executeClientAsync } from 'api/rage';
  import { selectCase } from './state.js';
  import { format } from 'api/formatter';
  import { accountUnique } from 'store/account';
  import { ItemId } from 'json/itemsInfo.js';
  import { onMount } from 'svelte';

  let shopList = [];
  let casesData = [];
  let selectIndex = 0;
  let caseData = [];
  let isLoad = false;

  executeClient("client.donate.load");
  executeClientAsync("donate.roulette.getList").then(result => {
    if (result && typeof result === "string") {
      const parsed = JSON.parse(result);
      shopList = parsed.shopList;
      casesData = parsed.caseData;
      onSelectCases(0);
      isLoad = true;
    }
  });

  const onOpenCase = (index) => {
    selectCase.set(index);
    window.gameMenuView("Roulette", index);
  };

  function onSelectCases(idx) {
    selectIndex = idx;
    const now = Date.now();

    caseData = shopList[idx].cases
      .map(i => casesData.find(c => c.index === i))
      .filter(Boolean)
      .map(c => {
        const start = c.startCase ? new Date(c.startCase).getTime() : 0;
        const end = c.endCase ? new Date(c.endCase).getTime() : 0;
        const hasSchedule = Boolean(c.startCase && c.endCase);

        let available = true;
        let daysLeft = 0;
        if (hasSchedule) {
          available = now >= start && now < end;
          daysLeft = available ? Math.ceil((end - now) / (24*60*60*1000)) : 0;
        }

        return {
          ...c,
          hasSchedule,
          available,
          daysLeft
        };
      });
  }

  $: sortedCaseData = [...caseData].sort((a, b) =>
    (b.available ? 1 : 0) - (a.available ? 1 : 0)
  );

  onMount(() => {
    const iv = setInterval(() => onSelectCases(selectIndex), 60000);
    return () => clearInterval(iv);
  });

  const getPrice = (price, index, unique) => {
    if (unique && unique.split("_")) {
      const [t, i, z] = unique.split("_").map((v,idx)=> idx>0?Number(v):v);
      if (t==="cases" && i===index && z===0) {
        return Math.round(price*0.7);
      }
    }
    return price;
  };

  function formatMoney(num) {
    return new Intl.NumberFormat('ru-RU', {
      maximumFractionDigits: 0
    }).format(num);
  }
</script>

<div class="cases full-width full-height">
  <div class="background"></div>
  
  <div class="cases full-width full-height row-block">
    <div class="gradient full-width full-height"></div>
    
    {#if isLoad}
      {#each sortedCaseData as item}
        <div 
          class="case column-block align-center {item.available ? '' : 'case-unavailable'}"
          on:click={() => onOpenCase(item.index)}
        >
          <img 
            src="http://cdn.piecerp.ru/cloud/img/cases/{item.image}.png" 
            alt="" 
            class="case__picture"
          />
          
          <div class="case__title">{item.name}</div>
          
          {#if item.price > 0}
            <div class="price-wrapper {item.discount ? 'discount-type' : ''}">
              <div class="price row-block align-center">
                {#if item.discount}
                  <div class="old-price">{formatMoney(item.price)}</div>
                  <div class="current-price">
                    {formatMoney(Math.round(item.price - item.price / 100 * item.discount))}
                  </div>
                {:else}
                  <div class="current-price">{formatMoney(item.price)}</div>
                {/if}
                
                <svg class="icon-price" width="22" height="22" viewBox="0 0 22 22" xmlns="http://www.w3.org/2000/svg">
                  <circle cx="11" cy="11" r="11" fill="#e81c5a"/>
                  <path d="M6.193-3.412a2,2,0,0,0-.522,1.031c0,.378.183,1.318.692,1.318.326,0,.953-.744,1.148-.992A27.023,27.023,0,0,1,12.5-7.119l.026.026c-.914,1.188-2.571,3.145-2.571,4.711,0,.757.287,1.579,1.148,1.579,1.122,0,3.928-2.219,4.241-3.119l-.052-.5c-1.762,1.71-2.78,2.3-3.145,2.3-.313,0-.457-.378-.457-.639a5.748,5.748,0,0,1,1.5-2.962l2.023-2.467c.157-.2.561-.653.561-.914s-.548-1.122-.9-1.122c-.235,0-.5.274-.666.418C12.5-8.45,11-7.027,9.391-5.644L9.364-5.67c.535-.822,1.945-2.754,1.945-3.719a.936.936,0,0,0-.953-.9c-.535.013-1.644.992-2.127,1.449L6.154-6.832l-.026-.026a16.464,16.464,0,0,0,.809-2.271.967.967,0,0,0-.157-.5l-.313-.548c-.131-.222-.183-.4-.5-.418A11.83,11.83,0,0,0,3.5-9.677L2.03-9,2-9.024c.509-.444.692-.679.692-.757s-.091-.131-.157-.131a.81.81,0,0,0-.326.183c-1.161.822-1.083.8-1.2.953a7.615,7.615,0,0,0-.339,1.54c-.065.313.1.483.313.483.666,0,.039-.026,4.059-1.905a1.3,1.3,0,0,1,.4-.157c.222,0,.457.261-.914,2.714L1.808-2.655c.144.4.4,1.436.953,1.436.418,0,.744-.5.966-.8L5.684-4.652,8.816-8.045c.091-.1.248-.274.4-.274s.2.144-.026.561A19.4,19.4,0,0,1,7.081-4.574Z" transform="translate(3.129 16.731)" fill="#fff"/>
                </svg>
                
                {#if item.discount}
                  <div class="discount">-{item.discount}%</div>
                {/if}
              </div>
            </div>
          {/if}

          <div class="cases-overlay-data column-block">
            {#if item.hasSchedule && item.available}
              <div class="days-left row-block align-center">
                <svg class="days-left__picture" width="22" height="23" viewBox="0 0 22 23" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <path d="M21 11.5652C21 6.28249 16.5228 2 11 2C5.47715 2 1 6.28249 1 11.5652V12.4348C1 17.7175 5.47715 22 11 22C16.5228 22 21 17.7175 21 12.4348V11.5652Z" fill="#3DE7A5"/>
                  <path d="M11 7V13L14 15" stroke="black" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <span class="days-left__text">
                  Осталось: <b>{item.daysLeft}</b> {item.daysLeft === 1 ? 'день' : 'дней'}
                </span>
              </div>
            {/if}

            <div class="player-cases row-block align-center">
              <div class="player-cases__icon">
                <svg width="35" height="35" viewBox="0 0 35 35" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <g filter="url(#filter0_d_712_2779)">
                  <rect x="10" y="10" width="15" height="15" rx="2" fill="#E41958"/>
                  </g>
                  <path d="M16.3175 21C16.1118 20.9999 15.9145 20.9182 15.7691 20.7727L14.2177 19.2214C14.0764 19.0751 13.9983 18.8792 14 18.6758C14.0018 18.4724 14.0834 18.2779 14.2272 18.134C14.371 17.9902 14.5655 17.9086 14.7689 17.9069C14.9723 17.9051 15.1682 17.9833 15.3145 18.1246L16.2736 19.0837L20.3823 14.2905C20.4474 14.2094 20.528 14.1421 20.6196 14.0928C20.7111 14.0435 20.8116 14.0131 20.9151 14.0034C21.0186 13.9937 21.123 14.0049 21.2221 14.0363C21.3212 14.0678 21.413 14.1188 21.492 14.1864C21.5709 14.254 21.6355 14.3368 21.6819 14.4299C21.7282 14.523 21.7554 14.6244 21.7617 14.7282C21.7681 14.832 21.7535 14.936 21.7189 15.034C21.6843 15.132 21.6303 15.2221 21.5602 15.2988L16.9062 20.7285C16.834 20.8142 16.7439 20.883 16.6421 20.9299C16.5404 20.9769 16.4295 21.0008 16.3175 21Z" fill="white"/>
                  <defs>
                  <filter id="filter0_d_712_2779" x="0" y="0" width="35" height="35" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                  <feFlood flood-opacity="0" result="BackgroundImageFix"/>
                  <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"/>
                  <feOffset/>
                  <feGaussianBlur stdDeviation="5"/>
                  <feComposite in2="hardAlpha" operator="out"/>
                  <feColorMatrix type="matrix" values="0 0 0 0 0.894118 0 0 0 0 0.0980392 0 0 0 0 0.345098 0 0 0 0.5 0"/>
                  <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_712_2779"/>
                  <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_712_2779" result="shape"/>
                  </filter>
                  </defs>
                </svg>
              </div>
              <span class="player-cases__title">
                У вас <b>{window.getItemToCount(ItemId["Case"+item.index])}</b> кейсов
              </span>
            </div>
          </div>
        </div>
      {/each}
    {/if}
  </div>
</div>