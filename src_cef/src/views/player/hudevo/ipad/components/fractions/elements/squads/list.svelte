<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    import { executeClientToGroup } from "api/rage";
    import moment from 'moment';
    import {setPopup} from "../../data";

    export let departmentId;
    export let members;
    export let onlineData;
    export let ranks;

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }

    let deletePlayerUUid = 0;
    const onPlayerDeleteCallback = () => {
        executeClientToGroup("deletePlayerDepartment", departmentId, deletePlayerUUid)
    }

    const onDepartmentDelete = (item) => {
        deletePlayerUUid = item.uuid;
        setPopup ("Confirm", {
            headerTitle: "Удалить из отряда",
            headerIcon: "fractionsicon-members",
            text: `Вы действительно хотите удалить '${item.name}' Из отряда?`,
            button: 'Убрать',
            callback: onPlayerDeleteCallback
        })
    }

</script>
<div class="onliuser">
    <p>В сети {onlineData.online}
            <svg width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
            <g filter="url(#filter0_d_995_2088)">
            <rect x="6" y="6" width="7" height="7" rx="3.5" fill="#8FB545"></rect>
            </g>
            <defs>
            <filter id="filter0_d_995_2088" x="0" y="0" width="19" height="19" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
            <feMorphology radius="2" operator="dilate" in="SourceAlpha" result="effect1_dropShadow_995_2088"></feMorphology>
            <feOffset></feOffset>
            <feGaussianBlur stdDeviation="2"></feGaussianBlur>
            <feComposite in2="hardAlpha" operator="out"></feComposite>
            <feColorMatrix type="matrix" values="0 0 0 0 0.560784 0 0 0 0 0.709804 0 0 0 0 0.270588 0 0 0 0.25 0"></feColorMatrix>
            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_995_2088"></feBlend>
            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_995_2088" result="shape"></feBlend>
            </filter>
            </defs>
        </svg>
    </p>
    <p>В офлайне {onlineData.offline}
        <svg width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
            <g filter="url(#filter0_d_995_2092)">
            <rect x="6" y="6" width="7" height="7" rx="3.5" fill="#FF4545"></rect>
            </g>
            <defs>
            <filter id="filter0_d_995_2092" x="0" y="0" width="19" height="19" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
            <feMorphology radius="2" operator="dilate" in="SourceAlpha" result="effect1_dropShadow_995_2092"></feMorphology>
            <feOffset></feOffset>
            <feGaussianBlur stdDeviation="2"></feGaussianBlur>
            <feComposite in2="hardAlpha" operator="out"></feComposite>
            <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 0.270588 0 0 0 0 0.270588 0 0 0 0.25 0"></feColorMatrix>
            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_995_2092"></feBlend>
            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_995_2092" result="shape"></feBlend>
            </filter>
            </defs>
        </svg>                                
    </p>
    <p>Всего {onlineData.all}
        <svg width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
            <g filter="url(#filter0_d_995_2096)">
            <rect x="6" y="6" width="7" height="7" rx="3.5" fill="white"></rect>
            </g>
            <defs>
            <filter id="filter0_d_995_2096" x="0" y="0" width="19" height="19" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
            <feFlood flood-opacity="0" result="BackgroundImageFix"></feFlood>
            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"></feColorMatrix>
            <feMorphology radius="2" operator="dilate" in="SourceAlpha" result="effect1_dropShadow_995_2096"></feMorphology>
            <feOffset></feOffset>
            <feGaussianBlur stdDeviation="2"></feGaussianBlur>
            <feComposite in2="hardAlpha" operator="out"></feComposite>
            <feColorMatrix type="matrix" values="0 0 0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0.25 0"></feColorMatrix>
            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_995_2096"></feBlend>
            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_995_2096" result="shape"></feBlend>
            </filter>
            </defs>
        </svg>                                
    </p>
</div>

<div class="squadnav">
    <p>Имя Фамилия</p>
    <p>Ранг</p>
    <p>Отряд</p>
    <p>Очки</p>
</div>

<div class="squadlist">
    {#each members.filter(el => filterCheck(el.name, searchText)) as item}
        <div class="squaduser">
            <p>{item.name}</p>
            <b>{item.rankName}</b>
            <b>{ranks[item.departmentRank].name}</b>
            <b>{item.score}</b>
        </div>
    {/each}
</div>
