<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let departments;
    export let onSelectDepartment;

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }


    import { setPopup } from "../../data";
    import { executeClientToGroup } from "api/rage";
    import { format } from 'api/formatter'

    const onCreateCallback = (name, tag) => {
        let check = format("rank", name);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        check = format("tag", tag);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('createDepartment', name, tag)
    }

    const onCreate = () => {
        setPopup ("Input", {
            headerTitle: "Создайте отряд",
            headerIcon: "fractionsicon-squads",
            inputs: [
                {
                    title: "Название отряда",
                    placeholder: "Название отряда",
                    maxLength: 36,
                },
                {
                    title: "Метка отряда",
                    placeholder: "Метка отряда",
                    maxLength: 5
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onCreateCallback
        })
    }

    import { executeClientAsyncToGroup } from "api/rage";
    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();
</script>

<div class="squadnav">
    <p>Название</p>
    <p>Начальник</p>
    <p>Тег отряда</p>
    <p>Числиность</p>
    <div class="btns">
        {#if settings.createDepartment}
            <div class="btn" on:keypress={() => {}} on:click={onCreate}>Создать отряд</div>
        {/if}
    </div>
</div>
<div class="squadlist">
    {#each departments.filter(el => filterCheck(el.name, searchText)) as item}
        <div class="squad">
            <p>{item.name}</p>
            <b>{item.chiefName}</b>
            <b>#{item.tag}</b>
            <b>{item.playerCount} чел.</b>
            <div class="btns">
                <div class="btn" on:keypress={() => {}} on:click={() => onSelectDepartment (item)}>Информация</div>
            </div>
        </div>
    {/each}
</div>