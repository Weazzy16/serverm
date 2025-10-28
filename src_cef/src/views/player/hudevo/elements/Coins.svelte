<script>
    import { coins, time, received, opened, open, close } from './assets/coinsStore';

    // Функции открытия и закрытия окна
    export function openCoins() {
        open();
    }

    export function closeCoins() {
        close();
    }
</script>

<style>
    [v-cloak] { display: none; }
</style>

{#if $opened}
    <div id="app">
        <div class="main-cover full-width full-height">
            <div class="chat-container"></div>
            <div class="online column-block align-end">
                <div class="online__logo row-block"></div>
                <div class="missions-container align-end column-block">
                    <div class="mission column-block align-end">
                        <div class="coins column-block justify-center">
                            <div class="coins-amount row-block align-center">
                                <span class="coins-amount__title">БЕСПЛАТНО</span>
                                <img class="coins-amount__picture" src="img/logo.png" alt="">
                                <span class="coins-amount__text">{$coins}</span>
                            </div>
                            <div class="time-left full-height full-width align-center">
                                <span class="time-left__title">Осталось:</span>
                                <span class="time-left__value">
                                    {#if $received === 0}
                                        {$time}
                                    {:else}
                                        Получено!
                                    {/if}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
{/if}
