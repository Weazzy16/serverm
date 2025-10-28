<script>
  import { createEventDispatcher } from 'svelte';
  import './accountdisp.css'
  
  const dispatch = createEventDispatcher();

  // Props
  export let displayedComponents = ["accountId"];
  export let login = null;
  export let gender = null;
  export let accountId = null;
  export let serverName = null;
  export let serverId = null;
  export let currentUserId = null;

  // Упрощенная логика определения текущего пользователя
  $: isCurrentUser = currentUserId !== null && 
                     accountId !== null && 
                     +currentUserId === +accountId;

  // Прямое отображение данных без сложной логики
  $: displayAccountId = accountId;
  $: displayServerName = serverName;

  // Component visibility checks
  $: showYou = isCurrentUser && displayedComponents.includes("you");
  $: showLogin = false; // Всегда скрываем логин
  $: showAccountId = displayAccountId && displayedComponents.includes("accountId");
  $: showServerName = displayServerName && displayedComponents.includes("serverName");
</script>

<div class="wheel-avatar-wrapper">
  <div class="wheel-content">
    <div class="wheel-main-info">
      {#if showLogin}
        <p>{displayLogin}</p>
      {/if}
      
      <div class="wheel-more-info">
        {#if showAccountId || showYou}
          <div class="wheel-static">
            {#if showAccountId}
              <p>#{displayAccountId}</p>
            {/if}
            {#if showYou}
              <div class="wheel-you">
                <p>You</p>
              </div>
            {/if}
          </div>
        {/if}
         </div>
        {#if showServerName}
          <div class="wheel-server-name">
            <p>{displayServerName}</p>
          </div>
        {/if}
      </div>
   
    
    <slot />
  </div>
</div>
