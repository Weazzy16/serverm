<script>
  import { onMount } from "svelte";
  import './css/1489.css'
    import './css/7209.css'
  import './css/545.css'
    import './css/7209.bff4928f.css'
    import './css/8332.69baf864.css'
    import './css/6332.0a8e50db.css'
  import { storeSettings } from 'store/settings' // ← ДОБАВЬ ИМПОРТ

  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";

  const pageLoaders = {
    "basic:interface": () => import("../pages/BasicInterface.svelte"),
    "basic:additional": () => import("../pages/BasicAdditional.svelte"),
      "sound": () => import("../pages/BasicSound.svelte"),
      "game-chat:admin": () => import("../pages/GameChatAdmin.svelte"),
  "game-chat:chat-settings": () => import("../pages/GameChatSettings.svelte"),
  "game-chat:quick-commands": () => import("../pages/GameChatQuickCommands.svelte"),
      "macros": () => import("../pages/Macros.svelte"),


    // add other pages here
  };

  let categories = [
    {
      id: "basic",
      title: "Основное",
      children: [
        { id: "interface", title: "Интерфейс" },
        { id: "additional", title: "Дополнительно" },
      ],
      open: true,
    },
    { id: "sound", title: "Настройка звука", single: true, children: [], open: false },
    {
      id: "game-chat",
      title: "Игровой чат",
      children: [
        { id: "admin", title: "Администратор" },
        { id: "chat-settings", title: "Настройки чата" },
        { id: "quick-commands", title: "Быстрые команды" },
      ],
      open: false,
    },
    { id: "keybinds", title: "Назначение клавиш", single: true, children: [], open: false },
    { id: "macros", title: "Макросы", single: true, children: [], open: false },
    { id: "crosshair", title: "Настройка прицела", single: true, children: [], open: false },
    {
      id: "security",
      title: "Безопасность",
      children: [
        { id: "security-main", title: "Безопасность" },
        { id: "tg-notifs", title: "Уведомления Telegram" },
        { id: "login-history", title: "История входов" },
      ],
      open: false,
    },
  ];

  // active state
  let activeCategoryId = categories[0]?.id ?? null;

  // initialize activeSub synchronously (so loader key exists immediately)
  let activeSub = {};
  categories.forEach((cat) => {
    if (cat.children && cat.children.length) activeSub[cat.id] = cat.children[0].id;
  });

  let Page = null;
  let fallbackSettingKeys = [];

  // helper reactive key — Svelte будет отслеживать currentKey и перезапускать реактивные выражения, в которых он используется
  $: currentSub = activeSub[activeCategoryId];
  $: currentKey = currentSub ? `${activeCategoryId}:${currentSub}` : activeCategoryId;

  // load page when currentKey changes — Svelte tracks currentKey here
  $: if (currentKey) {
    loadPageForCurrent(currentKey);
  }

  // Load page for given key (synchronous signature)
  async function loadPageForCurrent(key) {
    Page = null;
    fallbackSettingKeys = [];

    console.debug("DEBUG: loadPageForCurrent called", { key, activeCategoryId, activeSub });

    const loader = pageLoaders[key];
    if (!loader) {
      console.warn("DEBUG: no page loader for key", key, "available keys:", Object.keys(pageLoaders));
      // fallback: use pageSettingKeys if available
      fallbackSettingKeys = pageSettingKeys[key] ?? [];
      return;
    }

    try {
      const module = await loader();
      Page = module.default;
      console.debug("DEBUG: page loaded for key", key);
    } catch (err) {
      console.error("DEBUG: failed to import page for", key, err);
      fallbackSettingKeys = pageSettingKeys[key] ?? [];
    }
  }

  // Handlers: avoid in-place mutation of activeSub — reassign the object so Svelte detects changes
   function onClickCategory(cat) {
    // Переприсваивание categories, чтобы Svelte увидел изменение (и закрыл/открыл нужную категорию)
    categories = categories.map((c) => ({ ...c, open: c.id === cat.id ? !c.open : false }));
    activeCategoryId = cat.id;
    // Обязательно переприсваиваем activeSub (если нужно)
    activeSub = { ...activeSub, [cat.id]: activeSub[cat.id] ?? cat.children?.[0]?.id };
    console.debug('onClickCategory', { activeCategoryId, activeSub, categories });
  }

   function onClickSubItem(cat, child, ev) {
    ev.stopPropagation();

    // 1) переприсваиваем activeSub — Svelte отслеживает замену ссылки
    activeSub = { ...activeSub, [cat.id]: child.id };

    // 2) помечаем родительскую категорию как открытой и переприсваиваем массив categories,
    //    чтобы компонент левой колонки перерендерил DOM и применил нужные классы
    categories = categories.map((c) => c.id === cat.id ? { ...c, open: true } : c);

    // 3) и делаем категорию активной
    activeCategoryId = cat.id;

    console.debug('onClickSubItem', { catId: cat.id, childId: child.id, activeCategoryId, activeSub, categories });
  }

  const isActiveCategory = (cat) => activeCategoryId === cat.id;
  const isSubActive = (cat, child) => activeSub[cat.id] === child.id;
    $: isFullPage = currentKey === "bindings" || currentKey === "macros";

</script>

<div class="settings">
  <div class="categories">
    {#each categories as cat (cat.id)}
      <div class="category {cat.single ? 'single' : ''} {isActiveCategory(cat) ? 'isActive' : ''}">
        <div class="category-main" on:click={() => onClickCategory(cat)} role="button" aria-expanded={cat.open}>
          <div class="category-main__data">
            <p>{cat.title}</p>
          </div>
        </div>

        {#if cat.children && cat.children.length}
          <div class="category-list" style="height: {cat.open ? 'auto' : '0rem'}; overflow: hidden;">
            {#each cat.children as child}
              <div
                class="category-list__item {isSubActive(cat, child) ? 'active' : ''}"
                on:click={(e) => onClickSubItem(cat, child, e)}
                role="button"
                tabindex="0"
              >
                <div class="category-dot"></div>
                <p>{child.title}</p>
              </div>
            {/each}
          </div>
        {/if}
      </div>
    {/each}
  </div>

  <div class="page-wrapper">
    <div class="page">
      <div class="title">
        {#if categories.find((c) => c.id === activeCategoryId)}
          {#if activeSub[activeCategoryId]}
            {categories.find((c) => c.id === activeCategoryId).children.find((ch) => ch.id === activeSub[activeCategoryId])?.title}
          {:else}
            {categories.find((c) => c.id === activeCategoryId).title}
          {/if}
        {:else}
          Настройки
        {/if}
      </div>

      <div class="tabs-wrapper single">
        <div class="tab">
          <div class="column-wrapper">
            {#if Page}
              <svelte:component this={Page} />
            {:else if fallbackSettingKeys && fallbackSettingKeys.length}
              <ColumnWrapper settingKeys={fallbackSettingKeys} />
            {:else}
              <p>activeCategoryId: {activeCategoryId}</p>
              {#if activeSub[activeCategoryId]}
                <p>activeSub: {activeSub[activeCategoryId]}</p>
              {/if}
              <p>Содержимое для этой вкладки ещё не реализовано.</p>
            {/if}
          </div>
        </div>
      </div>
    </div>
  </div>
</div>