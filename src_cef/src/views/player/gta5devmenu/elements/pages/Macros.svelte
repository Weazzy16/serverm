<script>
  import { onBeforeMount, onBeforeUnmount } from 'svelte';
  import { writable } from 'svelte/store';

  let macrosData = writable({
    profiles: [],
    macros: []
  });

  let activeProfileId = null;
  let activeMacroId = null;

  function handleCreateProfile() {
    console.log('Create profile');
  }

  function handleDeleteProfile() {
    console.log('Delete profile');
  }

  function handleSelectProfile(profileId) {
    activeProfileId = profileId;
  }

  function handleCreateMacro() {
    console.log('Create macro');
  }

  function handleSelectMacro(macroId) {
    activeMacroId = macroId;
  }

  onBeforeMount(() => {
    // Инициализация
  });

  onBeforeUnmount(() => {
    // Очистка
  });
</script>

<div class="page">
  <div class="header">
    <div class="title">Макросы</div>
    <div class="header-actions">
      <div class="action" on:click={handleCreateProfile}>
        <span>Добавить профиль</span>
      </div>
      <div class="action disabled" on:click={handleDeleteProfile}>
        <span>Удалить профиль</span>
      </div>
    </div>
  </div>

  {#if $macrosData.profiles && $macrosData.profiles.length > 0}
    <div class="content">
      <div class="profiles">
        <div class="profile-name">
          <input 
            type="text" 
            placeholder="Введите имя профиля"
            value={$macrosData.profiles[0]?.name || ''}
          />
        </div>

        <div class="macros-section">
          <div class="macros-header">
            <p>Создать новый макрос</p>
            <button on:click={handleCreateMacro}>+</button>
          </div>

          {#if $macrosData.macros && $macrosData.macros.length > 0}
            <div class="macros-list">
              {#each $macrosData.macros as macro (macro.id)}
                <div 
                  class="macro-item"
                  class:active={macro.id === activeMacroId}
                  on:click={() => handleSelectMacro(macro.id)}
                >
                  <p>{macro.name}</p>
                </div>
              {/each}
            </div>
          {:else}
            <div class="empty-state">
              <p>Добавьте макрос и настройте его</p>
            </div>
          {/if}
        </div>

        <div class="apply-settings">
          <p>Применить настройки</p>
          <input type="text" placeholder="Введите код" />
          <button>Загрузить</button>
          <button>Поделиться</button>
        </div>
      </div>
    </div>
  {:else}
    <div class="empty-profiles">
      <p>Добавьте профиль и настройте его</p>
    </div>
  {/if}
</div>