<script>
    import { otherStatsData } from 'store/account';
    import { charData } from 'store/chars';
    import { format } from 'api/formatter';
    import { onMount } from 'svelte';
    import moment from 'moment';
    import fraction from 'json/fraction.js';
    import jobs from 'json/jobs.js';
    import vipinfo from 'json/vipinfo.js';
    export let visible;
    export let selectView;
    export let carsList;
    import { authInfo } from './authInfo.js';

    let onSelectedView2 = "Page1";
    let onSelectedView3 = "Page1";
    let onSelectedView4 = "Page1";
    let onSelectedView5 = "Page1";
    let onSelectedView6 = "Page1";
    let selectCharData = $charData;

    let useVisible = -1;

    $: {
        if (useVisible != visible) {
            if (visible && $otherStatsData.Name) {
                selectCharData = $otherStatsData;
            } else if (visible && !$otherStatsData.Name && selectCharData !== $charData) {
                selectCharData = $charData;
            } else if (!visible && $otherStatsData.Name) {
                selectCharData = $charData;
                window.accountStore.otherStatsData('{}');
            }
            useVisible = visible;
        }
    }

    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }

    let targetDate = new Date(selectCharData.CreateDate);
    let now = new Date();
    let diffInMilliseconds = now - targetDate;

    let diff = {
        years: 0,
        months: 0,
        days: 0,
        hours: 0,
        minutes: 0,
    };

    onMount(() => {
        diff.years = Math.floor(diffInMilliseconds / 31536000000);
        diffInMilliseconds = diffInMilliseconds % 31536000000;

        diff.months = Math.floor(diffInMilliseconds / 2628000000);
        diffInMilliseconds = diffInMilliseconds % 2628000000;

        diff.days = Math.floor(diffInMilliseconds / 86400000);
        diffInMilliseconds = diffInMilliseconds % 86400000;

        diff.hours = Math.floor(diffInMilliseconds / 3600000);
        diffInMilliseconds = diffInMilliseconds % 3600000;

        diff.minutes = Math.floor(diffInMilliseconds / 60000);
    });
    let installedItems = [
    { name: 'ур.', icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/engine.svg' },
    { name: 'ур.', icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/brakes.svg' },
    { name: 'ур.', icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/transmission.svg' },
    { name: 'ур.', icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/turbo.svg' },
    { name: 'км/ч', icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/maxSpeed.svg' },
    { icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/gpsTracker.svg' },
    { icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/doorTazer.svg' },
    { icon: 'http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/signaling.svg' }
];


</script>

{#if selectView === "Osnovnoe"}
    <div class="list1">
        <div class="block">
            <h1>Денежные средства</h1>
            <!-- 
                 Предположим, что вы хотите в <p> отобразить 
                 общую сумму или любую другую нужную информацию.
            -->
            <p>${(selectCharData.BankMoney + selectCharData.Money|| 0).toLocaleString('ru-RU')}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/money.svg" alt=""/>
            <!-- Блок, в котором выводим иконки и суммы -->
            <span class="money-infos">
                <b>
                    <svg class="black-image" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 12.981">
                        <g transform="translate(-1 -1.019)">
                          <path d="M13.5,5H3A1,1,0,0,1,3,3H5a.472.472,0,0,0,.5-.5A.472.472,0,0,0,5,2H3A2.006,2.006,0,0,0,1,4v8a2.006,2.006,0,0,0,2,2H13.5A1.473,1.473,0,0,0,15,12.5v-6A1.473,1.473,0,0,0,13.5,5ZM14,7.9v3.2a1.479,1.479,0,0,0-.5-.1h-2a1.5,1.5,0,0,1,0-3h2A1.479,1.479,0,0,0,14,7.9Z" fill="currentColor"/>
                          <path d="M3.5,3.5A.472.472,0,0,0,3,4a.472.472,0,0,0,.5.5H13a.617.617,0,0,0,.4-.2.486.486,0,0,0,.05-.45l-1-2.5a.52.52,0,0,0-.65-.3L5.4,3.5Z" fill="currentColor"/>
                          <path d="M12.5,9h-1a.5.5,0,0,0,0,1h1a.5.5,0,0,0,0-1Z" fill="currentColor"/>
                        </g>
                      </svg>
                  ${(selectCharData.Money || 0).toLocaleString('ru-RU')}
                </b>
                <b>
                  <!-- Вставляем инлайн SVG для банка -->
                  <svg class="black-image" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 12.6">
                    <path d="M15.417,7.025v7a.7.7,0,0,1-.7.7H2.117a.7.7,0,0,1-.7-.7v-7Zm0-1.4h-14v-2.8a.7.7,0,0,1,.7-.7h12.6a.7.7,0,0,1,.7.7Zm-4.9,5.6v1.4h2.8v-1.4Z" transform="translate(-1.417 -2.125)" fill="currentColor"/>
                  </svg>
                  ${(selectCharData.BankMoney || 0).toLocaleString('ru-RU')}
                </b>
              </span>
              
          </div>
          
        <div class="block">
            <h1>Возраст</h1>
            <p>21 год</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/age.svg" alt=""/>
            <span>Национальность:<b>Нет</b></span>
        </div>
        <div class="block">
            <h1>Место жительства</h1>
            {#if selectCharData.HouseId}
        <p>Есть прописка</p>
    {:else}
        <p>Нет прописки</p>
    {/if}
    <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/property.svg" alt=""/>
    <span class="address-info">
        Дом: <b>{selectCharData.HouseId ? '#' + selectCharData.HouseId : 'Нет'}</b>
        <div>Квартира: <b>Нет</b></div>
      </span>
        </div>
        <div class="block">
            <h1>Работа</h1>
            <p>{jobs[selectCharData.WorkID]}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/job.svg" alt=""/>
            <span></span>
        </div>
        
        
        <div class="block">
            <h1>Семейное положение</h1>
            <p>{selectCharData.WeddingName}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/married.svg" alt=""/>
            <span></span>
        </div>
       
        <div class="block">
            <h1>Организация</h1>
            <p>{fraction[selectCharData.FractionID]}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/fraction.svg" alt=""/>
            <span>Должность<b>{selectCharData.FractionLVL}</b></span>
        </div>
        
        
        <div class="block">
            <h1>Семья</h1>
            <p>{selectCharData.OrganizationID}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/family.svg" alt=""/>
            <span>Должность<b>{selectCharData.OrganizationLVL}</b></span>
        </div>
        
        <div class="block">
            <h1>Премиум статус</h1>
            <p>{selectCharData.VipLvl > 0 ? `${vipinfo[selectCharData.VipLvl]}` : vipinfo[selectCharData.VipLvl]}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/main/premSubscription.svg" alt=""/>
            <span>Истикает:<b>{selectCharData.VipLvl > 0 ? `${moment(selectCharData.VipDate).format('DD.MM.YYYY')}` : vipinfo[selectCharData.VipLvl]}</b></span>
        </div>
        
        <div class="block">
            <h1>Телефон</h1>
            <p>{selectCharData.Sim == -1 ? "Нет Сим-Карты" : selectCharData.Sim}</p>
            <span></span>
        </div>
        <div class="block">
            <h1>Номер счета</h1>
            <p>XXXXXXXXXX{selectCharData.Bank}</p>
            <span></span>
        </div>
    </div>
{/if}

{#if selectView === "Statiskic"}
    <div class="list1">
        <div class="block">
            <h1>Дата регистрации</h1>
            <p>
                {#if diff.years > 0}
                    {diff.years} года назад
                    {:else if diff.months > 0}
                        {diff.months} месяц(а) назад
                    {:else if diff.days > 0}
                        {diff.days} д. назад
                    {:else if diff.hours > 0}
                        {diff.hours} ч. назад
                    {:else}
                        {diff.minutes} мин. назад
                {/if}
            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/registration.svg" alt=""/>
            <span>Дата регистрации:<b>{moment(selectCharData.CreateDate).format('DD.MM.YYYY HH:mm')}</b></span>
        </div>
        <div class="block">
            <h1>Уровень</h1>
            <p>{selectCharData.LVL}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/level.svg" alt=""/>
            <span>Респекты<b>{selectCharData.EXP} / {(3 + selectCharData.LVL * 3)}</b></span>
        </div>
        <div class="block">
            <h1>Время в игре</h1>
            <p class="timeb2">
                {#if onSelectedView2 === "Page1"}
                    {moment.duration(selectCharData.TodayTime, "minutes").format("h[ч.] m[м.]")}
                {/if}
                {#if onSelectedView2 === "Page2"}
                    {moment.duration(selectCharData.MonthTime, "minutes").format("w[нед.] d[д.] h[ч.] m[м.]")}
                {/if}
                {#if onSelectedView2 === "Page3"}
                    {moment.duration(selectCharData.TotalTime, "minutes").format("y[г.] M[мес.] w[nнед.] d[д.] h[ч.] m[м.]")}
                {/if}
            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/in_game_time.svg" alt=""/>
            <div class="timeblocks1">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView2 = "Page1"} class:active={onSelectedView2 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView2 = "Page2"} class:active={onSelectedView2 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView2 = "Page3"} class:active={onSelectedView2 == "Page3"}>Всего</div>
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Аресты</h1>
            <p>{selectCharData.arrest}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/arrests.svg" alt=""/>
            <span>Количество<b>{selectCharData.arrest > 0 ? `${selectCharData.arrest}` : 0}</b></span>
        </div>
        
        <div class="block">
            <h1>Судимости</h1>
            <p class="timeb2">0</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/criminal_records.svg" alt=""/>
            <div class="timeblocks1">
            <span class="timeblocks">Активные судимости</span>
            </div>
        </div>
        <div class="block">
            <h1>Убийства</h1>
            <p>{selectCharData.Kills > 0 ? `${selectCharData.Kills}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/kills.svg" alt=""/>
            <span>Смерти:<b>{(selectCharData.Deaths)} </b></span>
        </div>
        <div class="block">
            <h1>Блокировки</h1>
            <p class="timeb2">{selectCharData.isBan > 0 ? `${selectCharData.isBan}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/bans.svg" alt=""/>
            <span class="timeblocks">История блокировок</span>
           
        </div>
        <div class="block">
            <h1>Заработано</h1>
            <p class="timeb2">
                {#if onSelectedView3 == "Page1"}
                    ${(selectCharData.EarnedMoneyDay || 0).toLocaleString('ru-RU')}
                {/if}
                {#if onSelectedView3 == "Page2"}
                    ${(selectCharData.EarnedMoneyMonth || 0).toLocaleString('ru-RU')}
                {/if}
                {#if onSelectedView3 == "Page3"}
                    ${(selectCharData.EarnedMoney || 0).toLocaleString('ru-RU')}
                {/if}
            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/money_gained.svg" alt=""/>
            <div class="timeblocks1">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView3 = "Page1"} class:active={onSelectedView3 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView3 = "Page2"} class:active={onSelectedView3 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView3 = "Page3"} class:active={onSelectedView3 == "Page3"}>Всего</div>
            </span>
        </div>
        </div>
        
        <div class="block">
            <h1>Потрачено</h1>
            <p class="timeb2">
                {#if onSelectedView4 == "Page1"}
    ${(selectCharData.SpentMoneyDay|| 0).toLocaleString('ru-RU')}
{/if}
{#if onSelectedView4 == "Page2"}
    ${(selectCharData.SpentMoneyMonth|| 0).toLocaleString('ru-RU')}
{/if}
{#if onSelectedView4 == "Page3"}
    ${(selectCharData.SpentMoney|| 0).toLocaleString('ru-RU')}
{/if}

            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/money_spent.svg" alt=""/>
            <div class="timeblocks1">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView4 = "Page1"} class:active={onSelectedView4 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView4 = "Page2"} class:active={onSelectedView4 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView4 = "Page3"} class:active={onSelectedView4 == "Page3"}>Всего</div>
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Выигрыш в казино</h1>
            <p class="timeb2">
                {#if onSelectedView5 == "Page1"}
    ${"0"}
{/if}
{#if onSelectedView5 == "Page2"}
${"В разработке"}
{/if}
{#if onSelectedView5 == "Page3"}
${"Амер гей"}
{/if}

            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/money_casino_win.svg" alt=""/>
            <div class="timeblocks2">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView5 = "Page1"} class:active={onSelectedView5 == "Page1"}>День</div>
                <div class="timeblock2" on:keypress={() => {}} on:click={() => onSelectedView5 = "Page2"} class:active={onSelectedView5 == "Page2"}>Месяц</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView5 = "Page3"} class:active={onSelectedView5 == "Page3"}>Всего</div>
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Контракты</h1>
            <p class="timeb2">
                {#if onSelectedView6 == "Page1"}
        {"0"}
{/if}
{#if onSelectedView6 == "Page2"}
{"В разработке"}
{/if}


            </p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/family_contracts.svg" alt=""/>
            <div class="timeblocks2">
            <span>
                <div class="timeblock1" on:keypress={() => {}} on:click={() => onSelectedView6 = "Page1"} class:active={onSelectedView6 == "Page1"}>Общие</div>
                <div class="timeblock3" on:keypress={() => {}} on:click={() => onSelectedView6 = "Page2"} class:active={onSelectedView6 == "Page2"}>Личные</div>
                
            </span>
            </div>
        </div>
        <div class="block">
            <h1>Мировых заданий</h1>
            <p class="timeb2">{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/world_quests.svg" alt=""/>
            <span>Получено:<b>{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0} / 3</b></span>
        </div>
        <div class="block">
            <h1>Достижений</h1>
            <p class="timeb2">{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/achievements.svg" alt=""/>
            <span></span>
        </div>
        <div class="block">
            <h1>Введённый промокод</h1>
            <p class="timeb2">{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0}</p>
            <img class="imgd" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/chart/promo.svg" alt=""/>
            <span>Личный промокод:<b>{selectCharData.Warns > 0 ? `${selectCharData.Warns}` : 0} / 3</b></span>
        </div>
    </div>
{/if}

{#if selectView === "Skills"}
    {#if selectCharData.jobSkillsInfo}
        <div class="list1">
            {#each selectCharData.jobSkillsInfo as job, index}
                <div class="blockskill">
                    <h1>{job.name}</h1>
                    <p>{job.currentLevel} ранг</p>
                    <div class="progskill">
                        <div class="headskill">
                            <p>Прогресс:</p>
                            <b>{job.current}<p>/ {job.nextLevel}</p></b>
                        </div>
                        <div class="bgprog">
                            <div class="progbar" style="width: {job.currentLevel >= 5 ? 100 : (((job.currentLevel / 5) * 100) + ((job.current / job.nextLevel) * 20))}%"></div>
                        </div>
                    </div>
                </div>
            {/each}
        </div>
    {/if}
{/if}

{#if selectView === "Carsuser"}
    <div class="listcars">
        {#each carsList as item}
            <div class="blockcars">
                <div class="headblock">
                    <div class="km-block">
                        <img src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/road.svg" alt="road" class="icon" />
                        <span class="price-text">33 161 км</span>
                    </div>
                    
                    <div class="price-wrapper">
                        <div class="price-row">
                            <img src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/bank.svg" class="icon" />
                            <span class="price-text">$7 500 000</span>
                        </div>
                        <div class="price-row">
                            <img src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/recycle-bin.svg" class="icon" />
                            <span class="price-text">$5 625 000</span>
                        </div>
                    </div>
                <!--    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/vehicle/bdivo.png" alt=""/>-->
                  <img src="{document.cloud}inventoryItems/vehicle/{item[1].toLowerCase()}.png" alt=""/> 
                </div>
                <h1>{item[1]}</h1>
                <div class="infocar">
                    <p>Рег. номер: <b>{item[2]}</b></p>
                    <div class="fuel">
                        <img class="fuel" src="http://cdn.piecerp.ru/cloud/img/panelmenu/main/cars/gas-pump.svg" alt="">
                        <span>Regular</span>                                        
                    </div>
                    
                </div>
                <div class="lines">
                </div>
                
                <div class="infodop">
                    <p>Установленные предметы:</p>
                    <div class="dop">
                        
                        <span>{#each installedItems.slice(5, 8) as item}
                            {#if item.icon.includes('http')}
                                <img src={item.icon} alt="icon" style="width: 20px; height: 20px; margin-right: 25px;" />
                            {:else}
                                <span style="margin-right: 0px;">{item.icon}</span>
                            {/if}
                        {/each}</span>                                        
                    </div>
                </div>
           <div class="installed-items">
            <div class="row">
                {#each installedItems.slice(0, 3) as item}
                    <div class="item2">
                        <span class="icons">
                            {#if item.icon.includes('http')}
                                <img class="icons" src={item.icon} alt="icons" />
                            {:else}
                                {item.icon} 
                            {/if}
                        </span>
                        <span class="name">0 {item.name}</span>
                    </div>
                {/each}
            </div>
            
            <div class="row">
                {#each installedItems.slice(3, 5) as item}
                    <div class="item2">
                        <span class="icons">
                            {#if item.icon.includes('http')}
                            <img class="icons" src={item.icon} alt="icons" />
                            {:else}
                                {item.icon}
                            {/if}
                        </span>
                        <span class="name">{item.name}</span>
                    </div>
                {/each}
            </div>
            
</div>

            
            
            </div>
            
        {/each}
    </div>
{/if}

{#if selectView === "Housebiz"}
    <div class="list5">
        {#if selectCharData.houseId}
            <div class="blockhouse" style="background: url(http://cdn.piecerp.ru/cloud/img/panelmenu/main/house/1.jpg);">
                <div class="bgblockhouse">
                    <h1>Дом #{selectCharData.houseId}</h1>
                    <div class="infohouse">
                        <div class="lefthouse">
                            <p>Списывается в час:</p>
                            <b>{selectCharData.houseCopiesHour}</b> удалить
                        </div>
                        <div class="righthouse">
                            <p>Гаражных мест:<b>{selectCharData.maxcars}</b></p>
                            <p>Класс дома:<b>{selectCharData.houseType}</b></p>
                            <p>На счету дома:<b>{selectCharData.houseCash}</b></p> удалить
                        </div>
                    </div>
                </div>
            </div>
        {/if}
        {#if selectCharData.BizId}
            <div class="blockhouse" style="background: url(http://6577f74f-piece.s3.twcstorage.ru/cloud/img/panelmenu/main/biz/ammo_shops/1.jpg);">
                <div class="bgblockhouse">
                    <h1>Бизнес #{selectCharData.BizId}</h1>
                    <div class="infohouse">
                        <div class="lefthouse">
                            <p>Списывается в час:</p> удалить 
                            <b>{selectCharData.BizCopiesHour}</b> удалить
                        </div>
                        <div class="righthouse">
                            <p>На счету бизнеса:<b>{selectCharData.BizCash}</b></p>удалить 
                        </div>
                    </div>
                </div>
            </div>
        {/if}
    </div>
{/if}