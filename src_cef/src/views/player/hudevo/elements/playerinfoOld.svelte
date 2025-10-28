<script>
    import { onMount } from 'svelte';
    import { itemsInfo } from 'json/itemsInfo.js'
    import { otherStatsData } from 'store/account'
    import { charData } from 'store/chars';
    import moment from 'moment'
    import fraction from 'json/fraction.js'
    import jobs from 'json/jobs.js'
    import vipinfo from 'json/vipinfo.js'
    export let visible;
    import { serverDonatMultiplier } from 'store/server'
    import { charWanted, charMoney, charBankMoney, charCoinsTime } from 'store/chars'
    import { isWaterMark, isPlayer } from 'store/hud'
    import { fly } from 'svelte/transition';
    import { format } from 'api/formatter'
    import { charIsPet } from 'store/chars'
    import CountUp from 'api/countup';
    export let SafeSone;
    import { coins, time, received, opened, open, close, startCountdown, stopCountdown } from './assets/coinsStore';

    import { translateText } from 'lang'
    import { getQuest } from 'json/quests/quests.js';
    import { storeQuests, selectQuest } from 'store/quest'
    import keys from 'store/keys'
    import keysName from 'json/keys.js'
    import { isInputToggled } from 'store/hud'
    import router from 'router';
    import { executeClient } from 'api/rage'
    import { isHelp } from 'store/hud'

    onMount(() => {
    window.events.addEvent("cef.coins.start", (newCoins, newTime, newReceived) => {
        open();
        coins.set(newCoins);
        time.set(newTime);
        received.set(parseInt(newReceived));
        
        if (parseInt(newReceived) === 0) {
            startCountdown(newTime); // ✅ Запускаем таймер, только если еще не получено
        }
    });

    window.events.addEvent("cef.coins.stop", (newTime, newReceived) => {
        time.set(newTime);
        received.set(parseInt(newReceived));
        stopCountdown(); // ✅ Останавливаем таймер
    });
});

$: {
    const uuid = Number(selectCharData?.UUID ?? characterData?.UUID ?? 0);
    if (uuid) {
      window.__hud = window.__hud || {};
      window.__hud.charUUID = uuid;          // ← вот тут всегда лежит статик
    }
  }

    let selectCharData = $charData;   
    let useVisible = -1;
    $: {
        if (useVisible != visible) {
            if (visible && $otherStatsData.Name/* && $otherStatsData.UUID !== selectCharData.UUID*/) {
                selectCharData = $otherStatsData;
            } else if (visible && !$otherStatsData.Name && selectCharData !== $charData) {
                selectCharData = $charData;
            } else if (!visible && $otherStatsData.Name) {
                selectCharData = $charData;
                window.accountStore.otherStatsData ('{}');
            }
            useVisible = visible;
        }
    }

    let isHint = false;
    window.hudStore.isHint = (value) => isHint = value;

    let HideHelp = false;
    window.hudStore.HideHelp = (value) => HideHelp = value;
    let QuestsList = [];
    let OldQuest = [];

    let quest = false;
    const onSelectQuest = (actorName) => {
        const listIndex = QuestsList.findIndex(q => q.ActorName == actorName);
        if (QuestsList[listIndex]) {
            quest = QuestsList[listIndex];
            return;
        }
        quest = false;
    }

    storeQuests.subscribe((value) => {
        if (value && value.length && OldQuest != value) {
            executeClient("client.quest.selectQuest.Clear");

            OldQuest = value;
            QuestsList = [];

            value.forEach(questData => {
                const questInfo = getQuest(questData.ActorName, questData.Line);

                if (questInfo && questInfo.Title && questInfo.Tasks && questInfo.Tasks[questData.Stage]) {
                    executeClient("client.quest.selectQuest.Add", questData.ActorName, (questInfo.Tasks.length - 1) === questData.Stage);

                    QuestsList.push({
                        ActorName: questData.ActorName,
                        Title: questInfo.Title,
                        Text: questInfo.Tasks[questData.Stage]
                    });
                }
            });
            QuestsList = QuestsList;

            if (!quest && QuestsList.length && typeof QuestsList[0] === "object" && typeof $selectQuest !== "string") {
                quest = QuestsList[0];
                selectQuest.set(quest.ActorName);
                return;
            }

            onSelectQuest($selectQuest);
        }
    });

    selectQuest.subscribe((value) => {
        onSelectQuest(value);
    });

    let userData = {
        targetMoney: 0,
        changeMoney: 0,
        timerIdMoney: 0,
        Money: 0,
        targetBank: 0,
        changeBank: 0,
        timerIdBank: 0,
        Bank: 0,
       
    };

    onMount(async () => {
        // Обновление счётчиков для денег, банка и т.д.
        charMoney.subscribe(value => {
            if (userData.Money !== value) {
                CounterUpdate("Money", value);
            }
        });
        charBankMoney.subscribe(value => {
            if (userData.Bank !== value) {
                CounterUpdate("Bank", value);
            }
        });
    });
    
    const CounterUpdate = (args, value) => {
        if (userData["timerId" + args])
            clearTimeout(userData["timerId" + args]);
        userData["change" + args] = userData[args] > value ? (0 - (userData[args] - value)) : (value - userData[args]);
        userData[args] = value;
        userData["timerId" + args] = setTimeout(() => {
            userData["timerId" + args] = 0;
            userData["change" + args] = 0;
            if (!userData["target" + args]) {
                userData["target" + args] = new CountUp("target" + args, value);
            }
            else
                userData["target" + args].update(value);
        }, !userData["target" + args] ? 0 : 5000)
    }

    let serverName = "";
    window.setServerName = (name) => serverName = name;

    let isRotate = false;
    const secretFunction = () => {
        isRotate = !isRotate;
    }
    let serverOnline = 837;
    window.serverStore.serverOnline = (value) => serverOnline = value;

    let serverPlayerId = 45;
    window.serverStore.serverPlayerId = (value) => serverPlayerId = value;

    let serverPlayerUUID = 11027;
    window.charStore.charUUID = (value) => serverPlayerUUID = value;

    let weaponItemId = 0;
    window.hudStore.weaponItemId = (value) => weaponItemId = value;

    let clipSize = 0;
    window.hudStore.clipSize = (value) => clipSize = value;

    let ammo = 0;
    window.hudStore.ammo = (value) => ammo = value;

    let isShow = false;
    serverDonatMultiplier.subscribe(value => {
        if (value > 1) {
            isShow = true;
            setTimeout(() => {
                isShow = false;
            }, 30 * 1000);
        }
    });
    window.app = {
    // Метод, который будет вызываться из mp.gui.emmit
    setTime: (val) => {
      time.set(val);
    }
  };

    // Обработчики событий для управления таймером и отображением награды
    
    const CreateProgressBar = () => {
        let ProgressBarId = new ProgressBar.Circle(".auth__characters_circle.UUID-" + charData.UUID, {
            color: '#FF9F1C',
            trailColor: '#eee',
            trailWidth: 1,
            duration: 1400,
            easing: 'bounce',
            strokeWidth: 6,
            from: {color: '#FF9F1C', a:0},
            to: {color: '#E71D36', a:1},
            // Set default step function for all animate calls
            step: function(state, circle) {
                circle.path.setAttribute('stroke', state.color);
            }
        });
        ProgressBarId.animate(GetProgress(charData.EXP));
    }
</script>

<!-- Пример разметки для отображения бонуса и оставшегося времени -->
<div class="hud_hachestic">
    <div class="hud_right">
        <div class="hud_rightup">
            <div class="hud_rightup_logoblock">
                <div class="logo__img">
                    <img src="https://cdn.majestic-files.com/public/master/static/img/main/hud/majestic.svg" class="logo__img">
                </div>
            </div>
            <div class="hud_rightup_info">
                <div class="hud_rightup_info_top">
                    <span>Online:</span><p>123{serverOnline}</p>
                    <a>Atlanta</a>
                </div>
                <span class="hud_rightup_info_bottom"><a>ID: {serverPlayerId} | </a><span>#{serverPlayerUUID}</span></span>
            </div>
        </div>

        <div class="hud_bonus">
            <div class="hud_bonus_top">
                <span>{$coins}</span>
                <div class="hud_bonus_img">
                    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" class="coins__picture" alt="Coins">
                </div>
                <a>БЕСПЛАТНО</a>
            </div>
            <div class="hud_bonus_bottom">
                <span class="time-left__title"></span>
                <span class="time-left__value">
                    <span class="time-left__text">Осталось отыграть:</span>
                    {#if $received === 0}
                    <span class="timer">{$time}</span>
                    {:else}
    <span class="timers">Завершено!</span>
{/if}
                </span>
                
                
            </div>
            

        </div>
        {#if quest && quest.Title && $isHelp}
        <div class="hud_quest">
            <div class="hud_quest_top">
                <span>{@html quest.Title}</span>
            </div>
            <div class="hud_quest_bottom">
                <span>{@html quest.Text}</span>
            </div>
        </div>
        {/if}
        {#if ammo > 0}
            <div class="box-flex mb-5" class:newhud__hide={!$isPlayer}>
                {#if ammo > 0}
                    <div class="hudevo__playerinfo_ammo">{ammo}{#if clipSize > 0 && clipSize < 1000}/{clipSize}{/if}</div>
                {/if}
                <div class="hudevo__playerinfo_red">{itemsInfo[weaponItemId].Name}</div>
            </div>
        {/if}
        {#if !$charIsPet && $isHelp}
        <div class="hud_binds">
          <div class="hud_binds_bind">
            <span>{keysName[$keys[20]]}</span>
            <a>Меню игрока</a>
          </div>
          <div class="hud_binds_bind">
            <span>{keysName[$keys[8]]}</span>
            <a>Навигатор</a>
          </div>
          <div class="hud_binds_bind">
            <span>{keysName[$keys[33]]}</span>
            <a>Голосовой чат</a>
          </div>
          <div class="hud_binds_bind">
            <span>{keysName[$keys[12]]}</span>
            <a>Инвентарь</a>
          </div>
          <div class="hud_binds_bind">
            <span>{keysName[$keys[25]]}</span>
            <a>Взаимодействие</a>
          </div>
          <div class="hud_binds_bind">
            <span>{keysName[$keys[22]]}</span>
            <a>Телефон</a>
          </div>
          <div class="hud_binds_bind">
            <span>{keysName[$keys[26]]}</span>
            <a>Скрыть подсказки</a>
          </div>
          <div class="hud_binds_bind">
            <span>{keysName[$keys[35]]}</span>
            <a>Зажигание ТС</a>
          </div>
        </div>
      {/if}
      
    </div>
</div>