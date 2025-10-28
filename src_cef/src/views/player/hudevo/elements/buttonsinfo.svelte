<script>
    import { translateText } from 'lang'
    import { charIsPet } from 'store/chars'
    import { fly } from 'svelte/transition';
    import { isInputToggled, isHelp } from 'store/hud'
    import keys from 'store/keys'
    import keysName from 'json/keys.js'
	import router from 'router';

    let isHint = false;
    window.hudStore.isHint = (value) => isHint = value;

    let HideHelp = false;
    window.hudStore.HideHelp = (value) => HideHelp = value;
</script>
{#if !$charIsPet && $isHelp}
    <!-- <div class="buttonsinfo" in:fly={{ x: -50, duration: 500 }} out:fly={{ x: -50, duration: 250 }} class:newhud__hide={HideHelp || !$isHelp}>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[8]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Анимации')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[43]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Биндер')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[45]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Указать пальцем')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[27]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Скрыть HUD')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[49]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Скрыть уведомление')}</div>
        </div>
    </div> -->
    <!-- <div class="center-left-block">
        <div class="bg-text">
            <span class="text-after-gb">↑</span> Телефон
        </div>
        <div class="bg-text">
            <span class="text-after-gb">I</span> Инвентарь
        </div>
        <div class="bg-text">
            <span class="text-after-gb">X</span> Микрофон
        </div>
        <div class="bg-text">
            <span class="text-after-gb">T</span> Чат
        </div>
        <div class="bg-text">
            <span class="text-after-gb">U</span> Меню
        </div>
        <div class="bg-text">
            <span class="text-after-gb">Ё</span> Курсор
        </div>
        <div class="bg-text">
            <span class="text-after-gb">F10</span> Помощь
        </div>
        <div class="bg-text">
            <span class="text-after-gb">F2</span> Скрыть худ
        </div>
        <div v-if="" class="bg-text">
            <span class="text-after-gb">L</span> Открыть / Закрыть
        </div>
        <div v-if="" class="bg-text">
            <span class="text-after-gb">B</span> Завести машину
        </div>
        <div v-if="" class="bg-text">
            <span class="text-after-gb">↓</span> Аварийка
        </div>
        <div v-if="" class="bg-text">
            <span class="text-after-gb">←</span>
            <span class="text-after-gb">→</span> Поворотники
        </div>
        <div v-if="" class="bg-text">
            <span class="text-after-gb">X</span> Круиз-контроль
        </div>
        <div v-if="" class="bg-text">
            <span class="text-after-gb">Z</span> Автопилот
        </div>
    </div> -->
{/if}