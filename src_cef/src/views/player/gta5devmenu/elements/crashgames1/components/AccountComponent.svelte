<script>
  // публичные пропсы — как в твоём AccountComponent.js
  export let isAlternativeAvatar = true;
  export let displayedComponents = ['avatar'];   // ['avatar','you','login','accountId','serverName']
  export let login = null;
  export let gender = null;                      // number | boolean | null
  export let accountId = null;
  export let serverName = null;
  export let serverId = null;
  export let src = null;                         // реальный URL аватара (если есть)
  export let cdn = 'https://cdn.majestic-files.com/public/master/static';                           // базовый путь до icons/...

  // опционально: «кто я сейчас» (если не передашь — попробуем взять из глобалов)
  export let viewerLogin = null;
  export let viewerGender = null;
  export let viewerAccountId = null;
  export let viewerServerName = null;
  export let viewerServerId = null;

  // ——— helpers ———
  const show = (k) => Array.isArray(displayedComponents) && displayedComponents.includes(k);

  // берем «текущего пользователя» из пропсов или из глобалов проекта
  $: vLogin      = viewerLogin      ?? window?.cefState?.main?.login      ?? null;
  $: vGender     = viewerGender     ?? window?.cefState?.main?.gender     ?? null;
  $: vAccountId  = viewerAccountId  ?? window?.cefState?.main?.accountId  ?? window?.selectCharData?.UUID ?? null;
  $: vServerName = viewerServerName ?? window?.cefState?.main?.serverName ?? null;
  $: vServerId   = viewerServerId   ?? window?.cefState?.main?.server     ?? null;

  // нормализуем как в твоём js (строки → lower, id → number, gender → 0/1)
  $: normProps = {
    login: (login ?? '').toLowerCase() || null,
    gender: +!!gender,
    accountId: accountId != null ? Number(accountId) : null,
    serverName: (serverName ?? '').toLowerCase() || null,
    serverId: (serverId ?? '').toLowerCase() || null,
  };
  $: normViewer = {
    login: (vLogin ?? '').toLowerCase() || null,
    gender: +!!vGender,
    accountId: vAccountId != null ? Number(vAccountId) : null,
    serverName: (vServerName ?? '').toLowerCase() || null,
    serverId: (vServerId ?? '').toLowerCase() || null,
  };

  // «you» как в твоей логике: сравниваем все ключи КРОМЕ serverName
  $: isYou = ['login','gender','accountId','serverId']
    .every(k => normProps[k] == null || normProps[k] === normViewer[k]);

  // отображаемые значения
  $: displayLogin     = isYou ? (normViewer.login || login) : login;
  $: displayAccountId = isYou ? (normViewer.accountId ?? accountId) : accountId;
  $: displayServer    = isYou ? (normViewer.serverName || serverName) : serverName;

  // выбор иконки-аватара (если нет src или isAlternativeAvatar или ошибка загрузки)
  let loadError = false;
  $: useSvg = isAlternativeAvatar || !src || loadError;
  $: genderKey = (['male','female'][isYou ? normViewer.gender : normProps.gender]) || 'male';
  $: iconPath = `${cdn ? cdn + '/' : ''}icons/ApplicationTS/components/ingame/Avatar/${genderKey}.svg`;
  function onError(){ loadError = true; }

  // простая «локализация» надписи you — можешь заменить на свой i18n
  const youText = 'Вы';
</script>

<div class="avatar-wrapper">
  {#if show('avatar')}
    <div class="avatar">
      {#if useSvg}
        <img alt="avatar" src={iconPath} />
      {:else}
        <img alt="avatar" src={src} on:error={onError} />
      {/if}
      {#if isYou && show('you')}
        <div class="you"><p>{youText}</p></div>
      {/if}
    </div>
  {/if}

  {#if show('login') || show('accountId') || show('serverName')}
    <div class="content">
      <div class="main-info">
        {#if show('login')}
          <p>{displayLogin}</p>
        {/if}

        <div class="more-info">
          {#if show('accountId') || (!show('avatar') && isYou && show('you'))}
            <div class="static">
              {#if show('accountId')}
                <p>#{displayAccountId}</p>
              {/if}
              {#if !show('avatar') && isYou && show('you')}
                <div class="you"><p>{youText}</p></div>
              {/if}
            </div>
          {/if}

          {#if show('serverName') && displayServer}
            <div class="serverName"><p>{displayServer}</p></div>
          {/if}
        </div>
      </div>

      <slot />
    </div>
  {/if}
</div>
