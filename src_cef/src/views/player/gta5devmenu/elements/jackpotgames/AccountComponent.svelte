<script>
  import './accountdisp.css';

  // Props
  export let isAlternativeAvatar = true;
  export let displayedComponents = ["avatar"];
  export let login = null;
  export let gender = null;
  export let accountId = null;
  export let serverName = null;
  export let serverId = null;
  export let src = null;
  export let currentUserId = null;

  // State
  let imageError = false;

  // Computed values
  $: isYou = currentUserId !== null && accountId !== null && +accountId === +currentUserId;
  $: displayLogin = login?.toLowerCase() ?? null;
  $: displayGender = gender ? 'female' : 'male';
  $: displayAccountId = accountId !== null ? +accountId : null;
  $: displayServerName = serverName?.toLowerCase() ?? null;
  
  $: useAlternativeAvatar = isAlternativeAvatar || !src || imageError;
  $: avatarPath = `icons/ApplicationTS/components/ingame/Avatar/${displayGender}.svg`;
  
  // Display flags
  $: showAvatar = displayedComponents.includes("avatar");
  $: showYouBadge = isYou && displayedComponents.includes("you");
  $: showLogin = displayLogin && displayedComponents.includes("login");
  $: showAccountId = displayAccountId && displayedComponents.includes("accountId");
  $: showServerName = displayServerName && displayedComponents.includes("serverName");
  
  function handleImageError() {
    imageError = true;
  }
</script>

<div class="jack-avatar-wrapper">
  {#if showAvatar}
    <div class="jack-avatar">
      {#if useAlternativeAvatar}
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 80 80" fill="none">
          {#if displayGender === 'male'}
            <path xmlns="http://www.w3.org/2000/svg" d="M70.0967 80.1365C69.4744 78.7894 64.9783 75.3186 62.5459 74.5299C60.2252 73.7772 57.9313 72.921 56.0168 71.9523C56.0358 71.9362 56.0464 71.9266 56.0464 71.9266L55.7531 71.4555C55.8475 71.3586 55.8677 71.2958 55.7978 71.2773C55.0811 71.0854 53.909 64.0026 53.909 64.0026C53.9034 64.0462 53.8989 64.0899 53.8989 64.1341V64.0026C52.7196 63.7078 51.6118 63.4483 50.4738 63.2247C50.1297 60.7816 49.8515 58.1036 49.9235 56.9437C51.0978 55.4674 52.147 53.8973 52.5084 52.6519C53.3705 49.6833 53.6716 47.9858 53.6716 47.9858C53.6716 47.9858 54.4476 47.9858 54.9654 47.2994C55.4833 46.613 55.6124 42.2214 56.0006 40.9861C56.3889 39.7509 56.6476 37.6923 55.3531 37.8298C55.1096 37.8555 54.8883 37.9595 54.6906 38.1108C54.8196 37.199 56.2436 34.8905 56.2436 34.8905C56.2436 34.8905 57.4397 27.1356 57.2006 26.6184C56.9615 26.1012 58.3967 25.4549 56.2436 21.19C54.0906 16.9246 44.5213 10.7207 40.5738 10.2035C36.6263 9.68629 31.124 10.2035 28.8514 16.9246C28.8514 16.9246 22.8704 18.3464 22.0335 25.0675C21.1961 31.7885 25.8615 36.5712 25.8615 38.6387C25.8615 38.6471 25.8615 38.6555 25.8615 38.6639C25.6073 37.9488 25.3268 37.3742 25.0615 37.2809C24.2849 37.0065 22.8609 37.0065 23.1196 40.3003C23.3782 43.5942 23.6927 41.0441 24.8022 47.1625C25.4497 50.7308 26.6146 47.9858 26.6146 47.9858C26.6146 47.9858 27.6995 53.6671 30.2911 57.7377C30.2989 59.1565 30.3218 61.4053 30.3805 63.2133C29.2777 63.4375 28.1743 63.6994 27.0687 64.002V64.1526C27.0687 64.1245 27.057 64.0958 27.0531 64.0677C27.0531 64.0677 25.924 71.1637 25.2011 71.364C25.1017 71.3915 25.1827 71.5033 25.4039 71.6761L24.5698 72.353C24.1961 72.6561 23.7933 72.9174 23.3693 73.132L21.1637 74.2459L19.3564 74.747C17.8223 75.1721 16.4162 76.0127 15.2732 77.1876L11.5659 80.9981H70.4336C70.4336 80.9981 70.4548 80.909 70.0979 80.1359L70.0967 80.1365Z"/>
          {:else}
            <path xmlns="http://www.w3.org/2000/svg" d="M10 81H72C71.2147 78.2835 69.3634 75.0148 65.0546 73.2237C62.9438 72.3463 60.7142 71.664 58.5482 71.0012C55.2958 70.0058 52.1869 69.0544 49.8392 67.5524C47.6897 66.1771 47.4323 62.7343 49.3195 60.9174C51.1487 59.1567 52.5124 56.8101 53.5268 54.1961C57.1082 54.2433 60.0835 53.4178 62.2351 52.5239C64.0547 51.7678 64.6701 49.7693 63.4903 48.3447C61.613 46.077 59.7512 41.5152 60.365 32.5444C61.2339 19.844 52.5237 13.6293 47.5315 13.2723C47.5315 13.2723 45.844 10.4199 39.384 10.0225C36.3744 9.83714 33.3523 10.8131 30.681 12.4095C26.262 15.0503 23.0318 20.3127 22.3148 26.117C22.012 28.5685 22.1327 31.1191 22.2529 33.6606C22.5013 38.9117 22.748 44.1241 19.2537 48.3447C18.0741 49.7693 18.6896 51.7678 20.5091 52.5239C22.5086 53.3546 25.2196 54.1261 28.4668 54.1933C29.4816 56.8088 30.8462 59.157 32.6767 60.9186C34.564 62.7352 34.307 66.1783 32.1578 67.5539C29.8105 69.0561 26.7018 70.008 23.4497 71.0038C21.2836 71.6671 19.0539 72.3499 16.943 73.2277C12.6371 75.0183 10.7858 78.2844 10 81Z"/>
          {/if}
        </svg>
      {:else}
        <img src={src} alt="avatar" on:error={handleImageError} />
      {/if}
      
      {#if showYouBadge}
        <div class="jack-you">
          <p>ВЫ</p>
        </div>
      {/if}
    </div>
  {/if}

  {#if showLogin || showAccountId || showServerName}
    <div class="jack-content">
      <div class="jack-main-info">
        {#if showLogin}
          <p class="jack-login">{displayLogin}</p>
        {/if}
        
        <div class="jack-more-info">
          {#if showAccountId || (!showAvatar && showYouBadge)}
            <div class="jack-static">
              {#if showAccountId}
                <p class="jack-account-id">#{displayAccountId}</p>
              {/if}
              
              {#if !showAvatar && showYouBadge}
                <div class="jack-you">
                  <p>ВЫ</p>
                </div>
              {/if}
            </div>
          {/if}
          
          {#if showServerName}
            <div class="jack-serverName">
              <p>{displayServerName}</p>
            </div>
          {/if}
        </div>
      </div>
      
      <slot />
    </div>
  {/if}
</div>