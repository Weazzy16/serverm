<script>
    import { translateText } from 'lang'
    import moment from 'moment';
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let members;
    export let onSelectMember;
    export let onlineData;
    export let onPlayerDelete;
    export let playerUuid;

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }

    const onDeletePlayer = (event, item) => {
        event.stopPropagation();
        onPlayerDelete (item)
    }

    export let settings;

    const getProgress = (value, max) => {
        let perc = Math.round(value / max * 100);

        if (!perc || perc < 0)
            perc = 0;
        else if (perc > 100)
            perc = 100;

        return perc;
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
<div class="nametable">
    <p>Имя Фамилия</p>
    <p>Ранг</p>
    <p>Отряд</p>
    <p>Очки</p>
    <p>Дата входа</p>
</div>
<div class="tablelist">
    {#each members.filter(el => filterCheck(el.name, searchText)) as item}
            {#if playerUuid !== item.uuid}
                <div class="tableuser" on:keypress={() => {}} on:click={() => onSelectMember (item)}>
                    <p>
                        <svg class:online={item.isOnline} width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <g filter="url(#filter0_d_995_2198)">
                            <rect x="6" y="6" width="7" height="7" rx="3.5" fill="#8FB545"/>
                            </g>
                            <defs>
                            <filter id="filter0_d_995_2198" x="0" y="0" width="19" height="19" filterUnits="userSpaceOnUse" color-interpolation-filters="sRGB">
                            <feFlood flood-opacity="0" result="BackgroundImageFix"/>
                            <feColorMatrix in="SourceAlpha" type="matrix" values="0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 127 0" result="hardAlpha"/>
                            <feMorphology radius="2" operator="dilate" in="SourceAlpha" result="effect1_dropShadow_995_2198"/>
                            <feOffset/>
                            <feGaussianBlur stdDeviation="2"/>
                            <feComposite in2="hardAlpha" operator="out"/>
                            <feColorMatrix type="matrix" values="0 0 0 0 0.560784 0 0 0 0 0.709804 0 0 0 0 0.270588 0 0 0 0.25 0"/>
                            <feBlend mode="normal" in2="BackgroundImageFix" result="effect1_dropShadow_995_2198"/>
                            <feBlend mode="normal" in="SourceGraphic" in2="effect1_dropShadow_995_2198" result="shape"/>
                            </filter>
                            </defs>
                        </svg>
                        <svg class:online={!item.isOnline} width="19" height="19" viewBox="0 0 19 19" fill="none" xmlns="http://www.w3.org/2000/svg">
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
                        {item.name}
                    </p>
                    <b>{item.rankName}</b>
                    <b>{item.departmentName}</b>
                    <b>{getProgress(item.score, item.maxScore)}</b>
                    {#if item.isOnline}
                            <p>В сети ID {item.playerId}</p>
                        {:else}
                            <p>{moment(item.date).format('DD.MM.YYYY')} {moment(item.date).format('HH:mm')}</p>
                    {/if}
                </div>
            {/if}
    {/each}
</div>