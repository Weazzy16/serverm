<script>
  import { onMount, onDestroy } from 'svelte';
  import { writable } from 'svelte/store';

  let macrosData = writable({
    profiles: [],
    macros: []
  });

  let activeProfileId = null;
  let activeMacroId = null;

  function handleCreateProfile() {
    macrosData.update(data => {
      const newProfile = { 
        id: Date.now(), 
        name: 'Новый профиль', 
        active: data.profiles.length === 0 
      };
      activeProfileId = newProfile.id;
      return { ...data, profiles: [...data.profiles, newProfile] };
    });
  }

  function handleDeleteProfile() {
    if (activeProfileId) {
      macrosData.update(data => ({
        ...data,
        profiles: data.profiles.filter(p => p.id !== activeProfileId),
        macros: data.macros.filter(m => m.profileId !== activeProfileId)
      }));
      activeProfileId = $macrosData.profiles[0]?.id || null;
    }
  }

  function handleCreateMacro() {
    if (activeProfileId) {
      macrosData.update(data => ({
        ...data,
        macros: [...data.macros, {
          id: Date.now(),
          profileId: activeProfileId,
          name: 'Пустой макрос',
          actions: []
        }]
      }));
    }
  }

  onMount(() => {
    console.log('Macros page mounted');
  });

  onDestroy(() => {
    console.log('Macros page destroyed');
  });
</script>

<div class="page">
  <div class="header">
    <div class="title">Макросы</div>
    <div class="actions">
      <div class="action" on:click={handleCreateProfile}>
        <span>Добавить профиль</span>
      </div>
      <div 
        class="action" 
        class:disabled={!activeProfileId}
        on:click={handleDeleteProfile}
      >
        <span>Удалить профиль</span>
      </div>
    </div>
  </div>

  {#if $macrosData.profiles.length > 0}
    <div class="wrapper">
      <div class="profiles">
        <div class="profiles-line-wrapper">
          <div class="profiles-line">
            {#each $macrosData.profiles as profile (profile.id)}
              <div 
                class="profile"
                class:selected={profile.id === activeProfileId}
                on:click={() => activeProfileId = profile.id}
              >
                <span>{profile.name}</span>
              </div>
            {/each}
          </div>
        </div>
      </div>

      <div class="content">
        <div class="config-macros-list">
          <div class="body">
            <div class="rename-delete-panel">
              <div class="input-wrapper">
                <input 
                  type="text"
                  value={$macrosData.profiles.find(p => p.id === activeProfileId)?.name || ''}
                  placeholder="Новый профиль"
                  maxlength="16"
                />
              </div>
              <div class="delete-btn">🗑️</div>
            </div>

            <div class="drag-wrapper">
              <div class="header">
                <p>Создать новый макрос</p>
                <div class="add-btn" on:click={handleCreateMacro}>+</div>
              </div>

              <div class="list">
                {#if $macrosData.macros.filter(m => m.profileId === activeProfileId).length > 0}
                  {#each $macrosData.macros.filter(m => m.profileId === activeProfileId) as macro (macro.id)}
                    <div 
                      class="macro-item"
                      class:isActive={macro.id === activeMacroId}
                      on:click={() => activeMacroId = macro.id}
                    >
                      <p>{macro.name}</p>
                      <div class="drag">
                        {#each Array(6) as _, i}
                          <div key={`drag_${i}`}></div>
                        {/each}
                      </div>
                    </div>
                  {/each}
                {:else}
                  <div class="empty-macros">
                    <p>Добавьте макрос и настройте его</p>
                  </div>
                {/if}
              </div>
            </div>

            <div class="apply-setting-code">
              <div class="header">
                <div class="name">Применить настройки</div>
              </div>
              <div class="input-wrapper">
                <input type="text" placeholder="Введите код" maxlength="512" />
              </div>
              <div class="btn-wrapper">
                <div class="btn">
                  <span>Загрузить</span>
                </div>
                <div class="btn">
                  <span>Поделиться</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        {#if activeMacroId}
          <div class="config-macro-item">
            <div class="rename-delete-panel">
              <div class="input-wrapper">
                <input 
                  type="text"
                  value={$macrosData.macros.find(m => m.id === activeMacroId)?.name || ''}
                  placeholder="Пустой макрос"
                  maxlength="16"
                />
              </div>
              <div class="delete-btn">🗑️</div>
            </div>

            <div class="body">
              <div class="column-wrapper">
                <!-- Параметры макроса здесь -->
              </div>

              <div class="drag-wrapper">
                <div class="header">
                  <p>Команды</p>
                  <div class="add-btn">+</div>
                </div>
                <div class="list">
                  <div class="empty-actions">
                    <p>Добавьте действие и настройте его</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        {/if}
      </div>
    </div>
  {:else}
    <div class="empty-profiles">
      <p>Добавьте профиль и настройте его</p>
    </div>
  {/if}
</div>