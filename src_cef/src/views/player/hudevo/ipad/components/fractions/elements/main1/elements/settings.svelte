<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import copy from 'copy-to-clipboard';
    import { isFraction } from "@/views/player/menu/elements/fractions/data.js";
    import {setPopup, isOrganization} from "../../../data";
    import { TimeFormat } from 'api/moment'
    import { format } from 'api/formatter'
    import Stock from './stock.svelte'

    export let settings;

    const getUsersName = (count) => {
        if (count >= 2 && 4 <= count)
            return `${count} ${translateText('player1', 'человека')}`;

        return `${count} ${translateText('player1', 'человек')}`;
    }

    const onSaveSetting = (slogan, value, discord, color) => {
        let check;
        if (settings.discord != discord) {
            check = format("discord", discord);
            if (!check.valid) {
                window.notificationAdd(4, 9, check.text, 3000);
                return;
            }

            discord = discord.replace("https://discord.gg/", "")
        }

        if (settings.salary != value) {
            if (0 > value || value > 3) {
                window.notificationAdd(4, 9, "Сбор не может быть меньше 0% и не должен превышать 3%", 5000);
                return;
            }
        }

        if (color && color.length)
            executeClientToGroup ("saveSetting", slogan, value, discord, color[0], color[1], color[2]);
        else
            executeClientToGroup ("saveSetting", slogan, value, discord, -1, -1, -1);
    }

    const onOpenPopupSetting = () => {
        setPopup ("Input", {
            headerTitle: "Информационные настройки",
            headerIcon: "fractionsicon-stats",
            inputs: [
                {
                    title: "Лозунг",
                    placeholder: "Лозунг организации",
                    maxLength: 36,
                    value: settings.slogan
                },
                {
                    title: translateText('player1', 'Сборы в казну'),
                    placeholder: "Налог с игроков в казну (0-3%)",
                    maxLength: 1,
                    value: settings.salary,
                    type: "number",
                    min: 0,
                    max: 3
                },
                {
                    title: "Discord",
                    placeholder: "Invit-boots (пример: JbcMC7Zb)",
                    maxLength: 26,
                    value: settings.discord
                },
                {
                    //title: "Discord",
                    //placeholder: "Ссылка на Discord:",
                    type: "color",
                    value: settings.color
                },
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onSaveSetting
        })
    }

    const onCopyDiscord = () => {
        copy(`https://discord.gg/${settings.discord}`)
        window.notificationAdd(4, 9, `Вы скопировали ссылку на Discord. Вставьте ее в свой обычный браузер :)`, 7000);
    }
</script>

{#if isOrganization()}
<div class="info">
    <h1>Доп. информация:</h1>
    <div class="dop">
        <p>Путь фракции:</p>
        <span>{settings.crimeOptions ? translateText('player1', 'Криминал') : translateText('player1', 'Закон')}</span>
    </div>
    <div class="dop">
        <p>Слоган:</p>
        <span>"{settings.slogan}"</span>
    </div>
    <div class="dop">
        <p>Дата создания:</p>
        <span>{TimeFormat (settings.date, "HH:mm DD.MM.YY")}</span>
    </div>
    <div class="dop">
        <p>Сборы в казну:</p>
        <span>{settings.salary}%</span>
    </div>
</div>
{/if}

<Stock {settings} />
{#if isOrganization()}
{#if settings.isLeader}
    <div class="btnappf" on:keypress={() => {}} on:click={onOpenPopupSetting}>
        <p>Настроить</p>
    </div>
{/if}
{/if}