<script>
    import { createEventDispatcher } from "svelte";
    import { executeClient, executeClientAsync } from 'api/rage';
    import { addListernEvent } from "api/functions";
    import "./css/info.scss";

    export let x = 10;
    export let y = 10;
    export let isOwner = false;
    export let attackGangName = "Marabunta";
    export let defendGangName = "Bloods";
    export let countMembers = 1;
    export let calibreIndex = "-";
    export let startDate = "11.08, 13:55";
    export let idSquare;

    let calibres = [
        "PistolAmmo",
        "RiflesAmmo",
        "ShotgunsAmmo",
        "SMGAmmo",
        "SniperAmmo",
    ];

    const dispatch = createEventDispatcher();

    addListernEvent("cef.capture.getInfoCapture", (squareInfo) => {
        dispatch('open-beat-capture', { square: JSON.parse(squareInfo), extraData: true });
    });

    function close() {
        dispatch("close");
    }
    function cancel(){
        executeClient("client.toserver.capture.cancel", idSquare);
        dispatch("close");
    }

    function edit() {
        executeClient("client.toserver.capture.getInfoCapture", idSquare);
    }
</script>

<div class="capt-info-container" on:click={close}>
    <div on:click|stopPropagation class="container" style="left: {x / 3.5}px; top: {y / 3.5}px">
        <span>Атака:</span>
        {attackGangName}
        <span>Защита:</span>
        {defendGangName}
        <span>Начало:</span>
        {startDate}
        <span>Участников:</span>
        {countMembers}
        <span>Калибр:</span>
        {calibres[calibreIndex]}

        {#if isOwner}
            <div class="divider"></div>
            <button class="edit" on:click={edit}>Редактировать</button>
            <button class="cancel" on:click={cancel}>Отмена</button>
        {/if}
    </div>
</div>
