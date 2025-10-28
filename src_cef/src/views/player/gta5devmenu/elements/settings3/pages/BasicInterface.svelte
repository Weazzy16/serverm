<script>
  // Пример простой страницы "Интерфейс"
  export let title = "Интерфейс";
  let theme = "dark";
  let showHints = true;

  function save() {
    // Тут можно отправить на API или сохранить в localStorage
    console.log('save interface', { theme, showHints });
    alert('Настройки интерфейса сохранены (пример)');
  }
</script>

<div class="settings-page">
  <h2>{title}</h2>
  <div>
    <label>
      Тема:
      <select bind:value={theme}>
        <option value="dark">Тёмная</option>
        <option value="light">Светлая</option>
      </select>
    </label>
  </div>
  <div>
    <label>
      <input type="checkbox" bind:checked={showHints} /> Показывать подсказки
    </label>
  </div>
  <div>
    <button on:click={save}>Сохранить</button>
  </div>
</div>