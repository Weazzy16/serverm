<script>
    import { translateText } from 'lang'
    import {executeClientAsyncToGroup} from "api/rage";
    import { format } from "api/formatter";
    import copy from 'copy-to-clipboard';

    let stats = {}

    const getStats = () => {
        executeClientAsyncToGroup("getStats").then((result) => {
            if (result && typeof result === "string")
                stats = JSON.parse(result);
        });
    }
    getStats ();
    import { isFraction } from "../../../data";

    export let settings;
    
    const onCopyDiscord = () => {
        copy(`https://discord.gg/${settings.discord}`)
        window.notificationAdd(4, 9, `You copied the link to Discord`, 3000);
    }
</script>
<div class="info">
    <h1>Склад организации:</h1>
    <div class="dop">
        <p>Деньги:</p>
        <span>{format("money", stats.money)}$</span>
    </div>
    <div class="dop">
        <p>Аптечки:</p>
        <span>{stats.medKits} шт.</span>
    </div>
    <div class="dop">
        <p>Наркотики:</p>
        <span>{stats.drugs} шт.</span>
    </div>
    <div class="dop">
        <p>Материалы:</p>
        <span>{stats.mats} / {stats.maxMats}</span>
    </div>
</div>
{#if stats.warZonesCount >= 0}
    <div class="info">
        <h1>Владения:</h1>
        <div class="dop">
            <p>Территории:</p>
            <span>{stats.warZonesCount} шт.</span>
        </div>
    </div>
{/if}
{#if stats.bizCount >= 0}
    <div class="info">
        <h1>Владения:</h1>
        <div class="dop">
            <p>Предприятия:</p>
            <span>{stats.bizCount} шт.</span>
        </div>
    </div>
{/if}
{#if stats.gangZonesCount >= 0}
    <div class="info">
        <h1>Владения:</h1>
        <div class="dop">
            <p>Территории:</p>
            <span>{stats.gangZonesCount} шт.</span>
        </div>
    </div>
{/if}