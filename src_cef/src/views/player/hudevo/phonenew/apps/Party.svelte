<script>
    import copy from "copy-to-clipboard";
    import { fade } from "svelte/transition";
    import { crewData } from 'store/eternal-dev/crew';
    import { charUUID, charName } from 'store/chars';
  import { createEventDispatcher } from 'svelte';
  const dispatch = createEventDispatcher();

    import { executeClient } from "api/rage";
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";


    import "./style.scss";
    import { onDestroy } from "svelte";

    let currentPage = "players",
        players = [],
        isLeader = false,
        search = "",
        myMemberData = null,
        modalData = null;

    function openModal(type, additionalData = {}) {
        if (modalData != null && modalData.type == type)
            return modalData = null;

        modalData = {
            type,
            ...additionalData
        }
    }
  function closeApp() {
    dispatch('close');
  }
    function getMember(uuid) {
        if ($crewData == null)
            return null;

        return $crewData.Members[uuid] || null;
    }

    function callServer(eventName, ...args) {
        executeClient(`e-dev.crew-system.callServer`, eventName, ...args);
    }

    function createCrew() {
        callServer("create");
    }

    function copyInviteCode() {
        copy($crewData.InviteCode);
        executeClient("notify", 2, 9, "Вы успешно скопировали код приглашения!");
        modalData = null;
    }

    function changeInviteCode() {
        callServer("change-invited-code");
        modalData = null;
    }
  function closingStart() {
    // Handle closing gesture start
    console.log('Closing gesture started');
  }
    function onClick_modalButton() {
        if (modalData == null) return;

        switch(modalData.type) {
            case "enter-code":
                callServer("enter-code", modalData.input ?? "");
                break;
            case "disbandment":
                callServer("disbandment");
                break;
            case "leave":
                callServer("leave");
                break;
            case "delete":
                callServer("kick", modalData.targetPlayer.UUID);
                break;
            case "set-commander":
                callServer("set-commander", modalData.targetPlayer.UUID);
                break;
            case "set-leader":
                callServer("set-leader", modalData.targetPlayer.UUID);
                break;
            case "invite":
                callServer("invite", modalData.input ?? "");
                break;
        }

        modalData = null;
    }

    $: currentPage = $crewData ? "players" : "main";
    $: isLeader = $crewData ? getMember($charUUID).Access == 2 : false;
    $: myMemberData = $crewData ? getMember($charUUID) : null;
    $: players = $crewData ? Object.values($crewData.Members).filter(x => x.Name.replace("_", " ").toLowerCase().includes(search.toLowerCase())) : [];

    onDestroy(() => {
        onInputBlur();
    })
</script>

<div in:fade class="phone_crew">
  
    <div class="phone_crew-content">
        <div class="crew__header">
            <div class="player-data">
              
                <div class="user-login">
                    <div class="name">
                        <span class="name__title">{ $charName.split(" ")[0] }</span>
                        {#if isLeader}
                       
                        {/if}
                    </div>
                    <span class="user-login__text-surname">{ $charName.split(" ")[1] }</span>
                </div>
            </div>

            {#if currentPage == "main"}
                <div class="group-management">
                    <button on:click={() => openModal("enter-code")} class="group-management__button">Вступить в группу</button>
                    <button on:click={() => createCrew()} class="group-management__button">Создать группу</button>
                </div>
            {/if}

            {#if currentPage == "players"}
                <div class="search-bar">
                    <div class="members">
                   
                        <span class="members__text">{ Object.keys($crewData.Members).length }</span>
                    </div>
                    <div class="search-block">
                        <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={search} type="text" class="search-block__input" placeholder="Поиск" maxlength="20">
                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="13" height="13" viewBox="0 0 13 13">
                            <path d="M12.692,11.119,9.7,8.13a5.255,5.255,0,1,0-1.45,1.521l2.953,2.953a1.051,1.051,0,1,0,1.486-1.486ZM5.253,8.892A3.551,3.551,0,1,1,8.8,5.341,3.551,3.551,0,0,1,5.253,8.892Z"/>
                        </svg>
                    </div>
                </div>
            {/if}
        </div>

        <div class="crew__main">
            {#if currentPage == "main"}
                <div class="default-page">
                  
                    <span class="default-page__title">Приложение Группа</span>
                    <span class="default-page__text-1">
                        Вы сможете совместно работать на кооперативных работах или просто проводить время с друзьями
                    </span>
                    <span class="default-page__text-2">Участники группы будут отмечены на мини карте</span>
                </div>
            {/if}

            {#if currentPage == "players"}
                <div class="players-list">
                    {#each players as item, index }
                        <div class="player-container" class:leader-hover={isLeader && item.UUID != $charUUID}>
                            <div class="row-block">
                                <div class="player-container__picture">
                         
                                    <!-- <div class="player-container__picture-overlay"></div> -->
                                <!---->
                                </div>
                                <div class="player-login">
                                    <span class="player-login__text-name">{ item.Name.split("_")[0] }</span>
                                    <span class="player-login__text-surname">{ item.Name.split("_")[1] }</span>
                                    <div class="gradient"></div>
                                </div>
                            </div>
                            <div class="control-buttons">
                                <div class="control-buttons">
                                    <button on:click={() => openModal("set-leader", { targetPlayer: item })} class="control-button">
                                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="16" height="16" viewBox="0 0 16 16"> <g transform="translate(-10 -10)"> <g transform="translate(8.172 8.175)"> <path d="M17.811,8.5,16.3,15.176a.614.614,0,0,1-.614.485H3.918a.614.614,0,0,1-.614-.485L1.843,8.5a.614.614,0,0,1,.872-.682L6.3,9.556,9.274,4.3a.614.614,0,0,1,1.069,0l2.973,5.263,3.611-1.75a.614.614,0,0,1,.884.688Z"/> </g> </g> </svg>
                                    </button>
                                    <button on:click={() => openModal("set-commander", { targetPlayer: item })} class="control-button">
                                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="16" height="16" viewBox="0 0 16 16"> <g transform="translate(-10 -10)" clip-path="url(#clip-path)"> <path d="M15.652,7.994a1.148,1.148,0,0,0-.636-1.957l-3.874-.563A.506.506,0,0,1,10.76,5.2L9.028,1.686a1.147,1.147,0,0,0-2.058,0L5.238,5.2a.507.507,0,0,1-.382.277L.983,6.037A1.148,1.148,0,0,0,.346,7.994l2.8,2.732a.507.507,0,0,1,.146.449l-.661,3.858a1.123,1.123,0,0,0,.25.93,1.159,1.159,0,0,0,1.414.279l3.465-1.822a.519.519,0,0,1,.472,0L11.7,16.242a1.135,1.135,0,0,0,.534.134,1.151,1.151,0,0,0,.88-.413,1.123,1.123,0,0,0,.25-.93L12.7,11.175a.507.507,0,0,1,.146-.449Z" transform="translate(10.001 9.289)"/> </g> </svg>
                                    </button>
                                    <button on:click={() => openModal("delete", { targetPlayer: item })} class="control-button delete">
                                        <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="16" height="16" viewBox="0 0 16 16"> <g transform="translate(-10 -10)"> <path d="M7.9.93H9.956a2.763,2.763,0,0,1,1.015.116,1.675,1.675,0,0,1,.672.484,2.762,2.762,0,0,1,.431.926l.026.077.21.63h4.063a.558.558,0,1,1,0,1.116H15.41l-.45,7.643v.024h0c-.045.769-.081,1.379-.15,1.87a3.618,3.618,0,0,1-.4,1.312,3.535,3.535,0,0,1-1.532,1.444,3.619,3.619,0,0,1-1.333.318c-.495.039-1.106.039-1.876.039H8.188c-.77,0-1.381,0-1.876-.039a3.618,3.618,0,0,1-1.333-.318,3.535,3.535,0,0,1-1.532-1.444,3.618,3.618,0,0,1-.4-1.312c-.068-.491-.1-1.1-.149-1.87v-.024L2.45,4.279H1.488a.558.558,0,1,1,0-1.116H5.551l.21-.63.026-.077a2.763,2.763,0,0,1,.431-.926,1.674,1.674,0,0,1,.672-.484A2.763,2.763,0,0,1,7.9.93ZM11.04,2.886l.092.277h-4.4l.092-.277a2.293,2.293,0,0,1,.254-.64A.558.558,0,0,1,7.3,2.085a2.293,2.293,0,0,1,.687-.039H9.875a2.293,2.293,0,0,1,.687.039.558.558,0,0,1,.224.161A2.292,2.292,0,0,1,11.04,2.886Zm-3.6,4A.558.558,0,0,1,8,7.442v5.209a.558.558,0,1,1-1.116,0V7.442A.558.558,0,0,1,7.442,6.884Zm2.977,0a.558.558,0,0,1,.558.558v5.209a.558.558,0,0,1-1.116,0V7.442A.558.558,0,0,1,10.419,6.884Z" transform="translate(9.07 9.07)" fill-rule="evenodd"/> </g> </svg>
                                    </button>
                                </div>
                            </div>
                            {#if item.Access == 2}
                            
                            {/if}

                            <div class="is-active" class:active={item.IsOnline}></div>
                        </div>
                    {/each}
                </div>
            {/if}
        </div>

        {#if currentPage == "players"}
            <button on:click={() => openModal(isLeader ? "disbandment" : "leave")} class="crew__footer">
                <div class="footer-action">
                    { isLeader ? "Расформировать группу" : "Покинуть группу" }
                </div>
            </button>
        {/if}

        {#if modalData}
            {#if modalData.type == "invite"}
                <div role="button" tabindex="0" on:keydown={() => {}} on:click={(e) => {
                    if (e.target.className.includes("modal-content"))
                        modalData = null;
                }} class="modal-content">
                    <div class="invitation-wrapper">
                        <div class="texture"></div>
                        <div class="invitation">
                            <span class="invitation__title">Пригласить в группу</span>
                            <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={modalData.input} type="text" class="invitation__input" placeholder="Введите #ID игрока" maxlength="10">
                            <button on:click={() => onClick_modalButton()} class="send-invitation">Отправить</button>
                        </div>
                        <div class="massive-invitation">
                            <span class="massive-invitation__title">Массовое приглашение</span>
                            <span class="massive-invitation__text">Отправьте код друзьям, чтобы они могли присоединиться к вам</span>
                            <span class="massive-invitation__text-code">{ $crewData.InviteCode }</span>
                            <div class="control-buttons">
                                <button on:click={() => copyInviteCode()} class="control-button">Скопировать код</button>
                                <button on:click={() => changeInviteCode()} class="control-button">Изменить код</button>
                            </div>
                        </div>
                    </div>
                </div>
            {/if}

            {#if ["disbandment", "leave", "delete", "set-leader", "set-commander", "enter-code"].includes(modalData.type)}
                <div role="button" tabindex="0" on:keydown={() => {}} on:click={(e) => {
                    if (e.target.className.includes("modal-content"))
                        modalData = null;
                }} class="not-invitation modal-content">
                    {#if modalData.type == "enter-code"}
                        <div class="enter-code">
                            <span class="enter-code__title">Код для вступления</span>
                            <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={modalData.input} class="enter-code__input" placeholder="Введите код" type="text">
                            <button on:click={() => onClick_modalButton()} class="control-button">Вступить</button>
                        </div>
                    {:else}
                        <div class="question-window leader">
                            <span class="question-window__title">
                                {   
                                    modalData.type == "disbandment" ? "Расформировать группу" :
                                    modalData.type == "set-leader" ? "Передать лидерство" :
                                    modalData.type == "set-commander" ? "Назначить помощником" :
                                    modalData.type == "delete" ? "Выгнать участника" :
                                    modalData.type == "leave" ? "Покинуть группу" : "" 
                                }
                            </span>
                            <div class="question-window__text">
                                {   
                                    modalData.type == "disbandment" ? "Вы уверены что хотите расформировать группу?" :
                                    modalData.type == "set-leader" ? " Лидерство группы будет передано " :
                                    modalData.type == "set-commander" ? "Вы назначите вашего помощника " :
                                    modalData.type == "delete" ? "Вы уверены что хотите выгнать участника " :
                                    modalData.type == "leave" ? "Вы уверены что хотите покинуть группу?" : "" 
                                }

                                {#if ["set-leader", "set-commander", "delete"].includes(modalData.type)}
                                    <span>{ modalData.targetPlayer.Name.replace("_", " ") }</span>
                                {/if}
                            </div>
                            <div class="accept-actions">Подтвердите свои действия.</div>
                            <div class="control-buttons">
                                <button on:click={() => onClick_modalButton()} class="control-button">
                                    {   
                                        modalData.type == "disbandment" ? "Подтвердить" :
                                        modalData.type == "set-leader" ? "Применить" :
                                        modalData.type == "set-commander" ? "Применить" :
                                        modalData.type == "delete" ? "Выгнать" :
                                        modalData.type == "leave" ? "Подтвердить" : "" 
                                    }
                                </button>
                                <button on:click={() => modalData = null} class="control-button">Отменить</button>
                            </div>
                        </div>
                    {/if}
                </div>
            {/if}
        {/if}

        {#if currentPage == "players" && myMemberData != null && myMemberData.Access >= 1}
            <button class="add-player" class:active={modalData && modalData.type == "invite"}
                on:click={() => openModal("invite")}>
                <svg xmlns="http://www.w3.org/2000/svg" width="10" height="10" viewBox="0 0 10 10">
                    <g transform="translate(532 199)">
                    <path d="M631.074,313.114v10" transform="translate(-1158.074 -512.114)" stroke-width="1"/>
                    <path d="M618.384,316.345h10" transform="translate(-1150.384 -510.345)" stroke-width="1"/>
                    </g>
                </svg>
            </button>
        {/if}

</div>
   
</div>
  <div 
    class="close-block full-width"
    on:mousedown={closingStart}
    on:click={closeApp}
  >
    <div class="close-block-handler"></div>
  </div>   
