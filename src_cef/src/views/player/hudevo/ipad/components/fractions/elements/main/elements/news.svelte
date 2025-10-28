<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import { charUUID } from 'store/chars';
    import { TimeFormat } from 'api/moment'

    export let settings;
    let board = { };
    let selectPage = 1;

    const getBoard = () => {
        executeClientAsyncToGroup("getBoard").then((result) => {
            if (result && typeof result === "string") {
                board = JSON.parse(result);

                selectPage = board.page;
            }
        });
    }
    getBoard();

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.board", getBoard)

    import Pagination from './newPagination.svelte'
    import { setPopup } from "../../../data";

    const onAddBoard = (title, text) => {
        if (!title.length || title.length >= 20)
            return;
        else if (!text.length || text.length >= 100)
            return;

        executeClientToGroup("addBoard", title, text);
    }

    const addBoard = () => {
        setPopup ("Input", {
            headerTitle: "Пишите новости",
            headerIcon: "fractionsicon-news",
            inputs: [
                {
                    title: "Введите название новости",
                    placeholder: "Введите название новости",
                    maxLength: 20
                },
                {
                    title: "Введите текст новости",
                    placeholder: "Введите текст новости",
                    maxLength: 100,
                    textarea: true
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onAddBoard
        })
    }

    const onUpdateBoard = (title, text) => {
        if (!title.length || title.length >= 20)
            return;
        else if (!text.length || text.length >= 100)
            return;

        executeClientToGroup("updateBoard", board.page, title, text);
    }

    const updateBoard = () => {
        setPopup ("Input", {
            headerTitle: "Редактировать новости",
            headerIcon: "fractionsicon-news",
            inputs: [
                {
                    title: "Название новости",
                    placeholder: "Название новости",
                    maxLength: 20,
                    value: board.title
                },
                {
                    title: "Новый текст новости",
                    placeholder: "Новый текст новости",
                    maxLength: 100,
                    textarea: true,
                    value: board.text
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onUpdateBoard
        })
    }

    const deleteBoard = () => {
        executeClientToGroup("deleteBoard", board.page);

    }
</script>
<div class="news">
    <h1>Важные новости:</h1>
    {#if board.count > 0}
        <div class="blocknews">
            <div class="headnews">
                <p>{board.title}</p>
                <span>{TimeFormat (board.time, "HH:mm DD.MM.YY")}</span>
            </div>
            <span>{board.text}</span>
            <p>Автор: <b>{board.name}</b> {board.rankName}</p>
        </div>
    {:else}
        <div class="blocknews">
            <div class="headnews">
                <p>Новостей нету</p>
            </div>
            <span>Ждём когда будет что-то</span>
        </div>
    {/if}
    <div class="btnnews">
        <Pagination count={board.count} {selectPage} />
        {#if board.uuid === $charUUID || (typeof board.uuid !== "undefined" && settings.editAllTabletWall)}
            <div class="btnbro" on:keypress={() => {}} on:click={deleteBoard}><p>{translateText('player1', 'Удалить')}</p></div>
        {/if}
        {#if board.uuid === $charUUID || (typeof board.uuid !== "undefined" && settings.editAllTabletWall)}
            <div class="btnbro" on:keypress={() => {}} on:click={updateBoard}><p>{translateText('player1', 'Редактировать')}</p></div>
        {/if}
        {#if settings.tableWall}
            <div class="btnbro" on:keypress={() => {}} on:click={addBoard}><p>{translateText('player1', 'Написать')}</p></div>
        {/if}
    </div>
</div>