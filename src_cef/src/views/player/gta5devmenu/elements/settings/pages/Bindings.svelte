<script>
  import { storeSettings } from 'store/settings';
  import { onMount, onDestroy } from 'svelte';

  let allBinds = [];
  let indexId = -1;
  let loadedCategories = 0;
  const totalCategories = 6;
  let isResetting = false;

  const categories = ["all", "vehicle", "fast", "fraction", "other", "admin"];

  const indexToMenu = {
    0: "admin", 1: "admin", 2: "admin", 33: "admin",
    3: "fraction", 4: "fraction", 10: "fraction", 11: "fraction", 64: "fraction",
    6: "all", 7: "all", 8: "all", 9: "all", 12: "all", 14: "all", 19: "all", 24: "all", 
    25: "all", 27: "all", 30: "all", 31: "all", 32: "all", 36: "all", 43: "all", 
    47: "all", 49: "all", 50: "all", 52: "all", 55: "all", 56: "all", 59: "all", 
    60: "all", 63: "all", 65: "all", 66: "all", 67: "all",
    15: "fast", 16: "fast", 17: "fast", 18: "fast", 34: "fast", 48: "fast", 61: "fast",
    28: "vehicle", 38: "vehicle", 39: "vehicle", 40: "vehicle", 41: "vehicle", 
    42: "vehicle", 53: "vehicle", 54: "vehicle", 57: "vehicle", 58: "vehicle", 62: "vehicle",
  };

  // Дефолтные значения биндов из bind.js (keyCodeDefault)
  const defaultBinds = {
    0: "F8", 1: "F12", 2: "F4", 3: "F7", 4: "U", 6: "Y", 7: "N", 8: "U", 9: "E",
    10: "X", 11: "Z", 12: "I", 14: "R", 15: "1", 16: "2", 17: "3", 18: "4", 19: "T",
    24: "5", 25: "F10", 27: "F5", 28: "6", 30: "↑", 31: "G", 32: "H", 33: "F6",
    34: "Нет", 36: "N", 38: "B", 39: "L", 40: "←", 41: "→", 42: "↓", 43: "Tab",
    45: "Нет", 47: "Нет", 48: "Нет", 49: "Нет", 50: "M", 52: "Нет", 53: "J",
    54: "Нет", 55: "Нет", 56: "Нет", 57: "Нет", 58: "Нет", 59: "Нет", 60: "Нет",
    61: "Нет", 62: "Нет", 63: "F2", 64: "Нет", 65: "K", 66: "H", 67: "Нет"
  };

  if (typeof window !== 'undefined') {
    window.binder = {
      setData: (value) => {
        try {
          const newBinds = JSON.parse(value);
          console.log("✅ Загружено биндов:", newBinds.length);
          
          newBinds.forEach(newBind => {
            newBind.menu = indexToMenu[newBind.index] || "other";
            const exists = allBinds.some(b => b.index === newBind.index);
            if (!exists) {
              allBinds = [...allBinds, newBind];
            }
          });
          
          loadedCategories++;
        } catch (e) {
          console.error("❌ Ошибка загрузки биндов:", e);
        }
      },
      setBindData: (value) => {
        try {
          const bindData = JSON.parse(value);
          storeSettings.update(s => ({ ...s, ...bindData }));
        } catch (e) {
          console.error("❌ Ошибка setBindData:", e);
        }
      },
      updateData: (index, name) => {
        const bindIndex = allBinds.findIndex(b => b.index === index);
        if (bindIndex !== -1) {
          allBinds[bindIndex].name = name;
          allBinds = [...allBinds];
          console.log(`✅ Обновлен бинд index=${index}, name=${name}`);
        }
      },
      index: () => {
        indexId = -1;
      }
    };
  }

  let searchQuery = "";

  $: filteredBinds = allBinds.filter(item => {
    if (!searchQuery) return true;
    const lowerQuery = searchQuery.toLowerCase();
    return item.title?.toLowerCase().includes(lowerQuery) || 
           item.name?.toLowerCase().includes(lowerQuery);
  });

  $: groupedBinds = {
    all: filteredBinds.filter(b => b.menu === "all"),
    vehicle: filteredBinds.filter(b => b.menu === "vehicle"),
    fast: filteredBinds.filter(b => b.menu === "fast"),
    fraction: filteredBinds.filter(b => b.menu === "fraction"),
    other: filteredBinds.filter(b => b.menu === "other"),
    admin: filteredBinds.filter(b => b.menu === "admin"),
  };

  $: leftColumnCategories = [
    { key: 'all', title: 'Главное', items: groupedBinds.all },
    { key: 'vehicle', title: 'Транспорт', items: groupedBinds.vehicle },
    { key: 'fast', title: 'Прочее', items: groupedBinds.fast },
  ];

  $: rightColumnCategories = [
    { key: 'fraction', title: 'Государственные структуры', items: groupedBinds.fraction },
    { key: 'other', title: 'Общение', items: groupedBinds.other },
    { key: 'admin', title: 'Для администрации', items: groupedBinds.admin },
  ];

  // ← УМНЫЙ СБРОС: только измененные биндинги
  function onResetClick() {
    if (isResetting) return;
    
    // Находим измененные биндинги
    const changedBinds = allBinds.filter(bind => {
      const defaultValue = defaultBinds[bind.index];
      return defaultValue && bind.name !== defaultValue && bind.name !== "Нет";
    });

    if (changedBinds.length === 0) {
      alert("Нет измененных биндингов для сброса");
      return;
    }

    if (!confirm(`Вы уверены что хотите сбросить ${changedBinds.length} измененных биндингов?`)) {
      return;
    }
    
    console.log(`🔄 Сброс ${changedBinds.length} измененных биндингов...`);
    isResetting = true;
    
    // Сбрасываем каждый измененный бинд по отдельности
    let resetCount = 0;
    
    changedBinds.forEach((bind, index) => {
      setTimeout(() => {
        console.log(`Сброс бинда ${bind.index}: ${bind.name} → ${defaultBinds[bind.index]}`);
        
        // Обновляем локально
        const bindIndex = allBinds.findIndex(b => b.index === bind.index);
        if (bindIndex !== -1) {
          allBinds[bindIndex].name = defaultBinds[bind.index];
          allBinds = [...allBinds];
        }
        
        // Отправляем на сервер (вызываем refresh для конкретного индекса)
        if (typeof mp !== 'undefined' && mp.events && mp.events.callRemote) {
          mp.events.callRemote('bindConfigSave', bind.index, 0); // 0 = сброс
        }
        
        resetCount++;
        
        // Если все сброшены - завершаем
        if (resetCount === changedBinds.length) {
          console.log("✅ Все биндинги сброшены");
          setTimeout(() => {
            isResetting = false;
            
            // Перезагружаем данные для подтверждения
            allBinds = [];
            loadedCategories = 0;
            loadAllCategories();
          }, 300);
        }
      }, index * 50); // Задержка между сбросами
    });
  }

  function onBindClick(item) {
    if (isResetting) return;
    
    indexId = item.index;
    if (typeof mp !== 'undefined' && mp.trigger) {
      mp.trigger("client:binder", "update", item.index);
    } else if (window.executeClient) {
      window.executeClient("client:binder", "update", item.index);
    }
  }

  function loadAllCategories() {
    categories.forEach((category, index) => {
      setTimeout(() => {
        if (typeof mp !== 'undefined' && mp.trigger) {
          mp.trigger("client:binder", "get", category);
        } else if (window.executeClient) {
          window.executeClient("client:binder", "get", category);
        }
      }, index * 80);
    });
  }

  onMount(() => {
    console.log("🔵 Bindings mounted");
    setTimeout(() => loadAllCategories(), 100);
  });

  onDestroy(() => {
    if (window.binder) {
      delete window.binder;
    }
  });
</script>

<div class="page-wrapper">
  <div class="page">
    <div class="header">
      <div class="title">Назначение клавиш</div>
      <div class="header-actions">
      <!--  <div class="reset" class:disabled={isResetting} on:click={onResetClick}>
          <span>{isResetting ? 'Сброс...' : 'Сбросить настройки'}</span>
        </div>-->
        <div class="search">
          <div class="iconss">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 18 18" fill="none">
              <path d="M7.5 13.5C8.83123 13.4997 10.1241 13.0541 11.1728 12.234L14.4697 15.531L15.5303 14.4705L12.2333 11.1735C13.0538 10.1247 13.4997 8.83158 13.5 7.5C13.5 4.19175 10.8082 1.5 7.5 1.5C4.19175 1.5 1.5 4.19175 1.5 7.5C1.5 10.8082 4.19175 13.5 7.5 13.5ZM7.5 3C9.98175 3 12 5.01825 12 7.5C12 9.98175 9.98175 12 7.5 12C5.01825 12 3 9.98175 3 7.5C3 5.01825 5.01825 3 7.5 3Z"/>
            </svg>
          </div>
          <input 
            type="text" 
            placeholder="Поиск" 
            bind:value={searchQuery}
            maxlength="32"
            disabled={isResetting}
          />
          {#if searchQuery}
            <div class="cross" on:click={() => searchQuery = ""}>✕</div>
          {/if}
        </div>
      </div>
    </div>

    <div class="tabs-wrapper">
      {#if isResetting}
        <div class="empty-state">
          <p>Сброс настроек...</p>
        </div>
      {:else if filteredBinds.length > 0}
        <!-- Левая колонка -->
        {#if leftColumnCategories.some(cat => cat.items.length > 0)}
          <div class="tabs-wrapper single" style="row-gap: 4.0740740741vh!important;">
            {#each leftColumnCategories as category}
              {#if category.items.length > 0}
                <div class="tab darkTitle">
                  <p>{category.title}</p>
                  {#each category.items as item (item.index)}
                    <div class="bind-row" class:active={item.index === indexId} on:click={() => onBindClick(item)}>
                      <div class="setting-item">
                        <div class="header">
                          <div class="name">{item.title}</div>
                          <div class="default-bind">
                            <div class="bind" class:active={item.index === indexId}>
                              <button 
                                class="bind-value"
                                class:active={item.index === indexId}
                              >
                                {item.name || 'Нет'}
                                {#if item.index === indexId}
                                  ↑
                                {/if}
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  {/each}
                </div>
              {/if}
            {/each}
          </div>
        {/if}

        <!-- Правая колонка -->
        {#if rightColumnCategories.some(cat => cat.items.length > 0)}
          <div class="tabs-wrapper single" style="row-gap: 4.0740740741vh!important;">
            {#each rightColumnCategories as category}
              {#if category.items.length > 0}
                <div class="tab darkTitle">
                  <p>{category.title}</p>
                  {#each category.items as item (item.index)}
                    <div class="bind-row" class:active={item.index === indexId} on:click={() => onBindClick(item)}>
                      <div class="setting-item">
                        <div class="header">
                          <div class="name">{item.title}</div>
                          <div class="default-bind">
                            <div class="bind" class:active={item.index === indexId}>
                              <button 
                                class="bind-value"
                                class:active={item.index === indexId}
                              >
                                {item.name || 'Нет'}
                                {#if item.index === indexId}
                                  ↑
                                {/if}
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  {/each}
                </div>
              {/if}
            {/each}
          </div>
        {/if}
      {:else}
        <div class="empty-state">
          {#if searchQuery}
            <p>Ничего не найдено по запросу "{searchQuery}"</p>
          {:else if loadedCategories < totalCategories}
            <p>Загрузка биндов... ({loadedCategories}/{totalCategories})</p>
          {:else}
            <p>Нет доступных биндов</p>
          {/if}
        </div>
      {/if}
    </div>
  </div>
</div>

<style>


  .empty-state {
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 60px 20px;
    color: rgba(255, 255, 255, 0.5);
    font-size: 16px;
    text-align: center;
  }
</style>