<script>
  import { onMount, onDestroy } from "svelte";
  import { slide } from "svelte/transition";
  import './css/1489.css'
  import './css/7209.css'
  import './css/1558.b0654744.css'
  import './css/1630.a827b670.css'
  import './css/2319.d6ec8b41.css'
  import './css/545.css'
  import './css/8332.69baf864.css'
  import './css/5429.4e328824.css'
  import './css/6332.0a8e50db.css'

  import { storeSettings } from 'store/settings'
  import { executeClient } from 'api/rage'

  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";

  export let visible = true;

  const pageLoaders = {
    "basic:interface": () => import("./pages/BasicInterface.svelte"),
    "basic:additional": () => import("./pages/BasicAdditional.svelte"),
    "sound": () => import("./pages/BasicSound.svelte"),
    "game-chat:admin": () => import("./pages/GameChatAdmin.svelte"),
    "game-chat:chat-settings": () => import("./pages/GameChatSettings.svelte"),
    "game-chat:quick-commands": () => import("./pages/GameChatQuickCommands.svelte"),
    "bindings": () => import("./pages/Bindings.svelte"),
    "macros": () => import("./pages/Macros.svelte"),
    "crosshair": () => import("./pages/Crosshair.svelte"),
    "security:security-main": () => import("./pages/Secure.svelte"),
    "security:tg-notifs": () => import("./pages/TelegramNotifications.svelte"),
    "security:login-history": () => import("./pages/LoginHistory.svelte")
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
    { id: "sound", title: "Настройка звука", single: true, children: [], open: false },
    {
      id: "game-chat",
      title: "Игровой чат",
      children: [
        { id: "admin", title: "Администратор" },
        { id: "chat-settings", title: "Настройки чата" },
        { id: "quick-commands", title: "Быстрые команды" },
      ],
      open: false,
    },
    {
      id: "bindings",
      title: "Назначение клавиш",
      children: [],
      single: true,
      open: false,
    },
    { id: "crosshair", title: "Настройка прицела", single: true, children: [], open: false },
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

  let activeCategoryId = categories[0]?.id ?? null;
  let activeSub = {};
  categories.forEach((cat) => {
    if (cat.children && cat.children.length) activeSub[cat.id] = cat.children[0].id;
  });

  let Page = null;
  let fallbackSettingKeys = [];

  $: currentSub = activeSub[activeCategoryId];
  $: currentKey = currentSub ? `${activeCategoryId}:${currentSub}` : activeCategoryId;
  $: isFullPage = currentKey === "bindings" || currentKey === "security:security-main" || currentKey === "security:tg-notifs" || currentKey === "security:login-history" || currentKey === "crosshair";
  
  $: if (currentKey) {
    loadPageForCurrent(currentKey);
  }

  async function loadPageForCurrent(key) {
    Page = null;
    fallbackSettingKeys = [];
    const loader = pageLoaders[key];
    if (!loader) {
      fallbackSettingKeys = pageSettingKeys[key] ?? [];
      return;
    }
    try {
      const module = await loader();
      Page = module.default;
    } catch (err) {
      console.error("Failed to import page for", key, err);
      fallbackSettingKeys = pageSettingKeys[key] ?? [];
    }
  }

  function onClickCategory(cat) {
    categories = categories.map((c) => ({ ...c, open: c.id === cat.id ? !c.open : false }));
    activeCategoryId = cat.id;
    activeSub = { ...activeSub, [cat.id]: activeSub[cat.id] ?? cat.children?.[0]?.id };
  }

  function onClickSubItem(cat, child, ev) {
    ev.stopPropagation();
    activeSub = { ...activeSub, [cat.id]: child.id };
    categories = categories.map((c) => c.id === cat.id ? { ...c, open: true } : c);
    activeCategoryId = cat.id;
  }

  const isActiveCategory = (cat) => activeCategoryId === cat.id;
  const isSubActive = (cat, child) => activeSub[cat.id] === child.id;

  // ========== ОПТИМИЗИРОВАННАЯ СИСТЕМА СОХРАНЕНИЯ ==========
  let saveTimeout;
  let unsubscribe;
  let hasChanges = false;
  let lastSavedState = null;
  let isInitialized = false;

  function saveSettingsToServer(settings) {
    if (!isInitialized) return; // Не сохраняем при первой загрузке
    
    console.log("💾 Saving settings to server...");
    
    try {
      executeClient("chatconfig", JSON.stringify(settings));
      lastSavedState = JSON.stringify(settings);
      hasChanges = false;
      console.log("✅ Settings saved");
    } catch (e) {
      console.error("❌ Save error:", e);
    }
  }

  // ← УВЕЛИЧЕННЫЙ ДЕБАУНС ДЛЯ СЛАЙДЕРОВ
  function scheduleSave(settings, delay = 2000) {
    clearTimeout(saveTimeout);
    hasChanges = true;
    
    saveTimeout = setTimeout(() => {
      saveSettingsToServer(settings);
    }, delay);
  }

  onMount(() => {
    console.log("🟢 Settings component mounted");
    
    // Ждем 500мс перед инициализацией (чтобы не сохранять дефолтные значения)
    setTimeout(() => {
      storeSettings.subscribe(v => {
        if (!lastSavedState) {
          lastSavedState = JSON.stringify(v);
          console.log("📌 Initial state captured");
        }
      })();
      
      isInitialized = true;
    }, 500);

    // Подписываемся на изменения
    unsubscribe = storeSettings.subscribe((value) => {
      if (!isInitialized) return;
      
      const currentState = JSON.stringify(value);
      
      if (lastSavedState && currentState !== lastSavedState) {
        console.log("🔄 Settings changed");
        scheduleSave(value, 2000); // 2 секунды дебаунс
      }
    });
  });

  // Сохранение при закрытии меню
  $: if (!visible && hasChanges && isInitialized) {
    console.log("🚪 Settings closed, saving immediately...");
    clearTimeout(saveTimeout);
    const currentSettings = {};
    storeSettings.subscribe(v => Object.assign(currentSettings, v))();
    saveSettingsToServer(currentSettings);
  }

  onDestroy(() => {
    console.log("🔴 Settings component destroyed");
    if (unsubscribe) unsubscribe();
    clearTimeout(saveTimeout);
    
    if (hasChanges && isInitialized) {
      const currentSettings = {};
      storeSettings.subscribe(v => Object.assign(currentSettings, v))();
      saveSettingsToServer(currentSettings);
    }
  });
  // ========================================
</script>

<div class="settings">
  <div class="categories">
    {#each categories as cat (cat.id)}
      <div class="category14 {cat.single ? 'single' : ''} {isActiveCategory(cat) ? 'isActive' : ''}">
        <div class="category14-main" on:click={() => onClickCategory(cat)} role="button" aria-expanded={cat.open}>
          <div class="category14-main__data">
            <p>{cat.title}</p>
          </div>
        </div>

        {#if cat.children && cat.children.length}
          {#if cat.open}
            <div class="category14-list" transition:slide={{ duration: 300 }}>
              {#each cat.children as child}
                <div
                  class="category14-list__item {isSubActive(cat, child) ? 'active' : ''}"
                  on:click={(e) => onClickSubItem(cat, child, e)}
                  role="button"
                  tabindex="0"
                >
                  <div class="category14-dot"></div>
                  <p>{child.title}</p>
                </div>
              {/each}
            </div>
          {/if}
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

      {#if isFullPage}
        {#if Page}
          <svelte:component this={Page} />
        {:else}
          <p>Загрузка...</p>
        {/if}
      {:else}
        <div class="tabs-wrapper single">
          <div class="tab">
            <div class="column-wrapper">
              {#if Page}
                <svelte:component this={Page} />
              {:else if fallbackSettingKeys && fallbackSettingKeys.length}
                <ColumnWrapper settingKeys={fallbackSettingKeys} />
              {:else}
                <p>Содержимое для этой вкладки ещё не реализовано.</p>
              {/if}
            </div>
          </div>
        </div>
      {/if}
    </div>
  </div>
</div>