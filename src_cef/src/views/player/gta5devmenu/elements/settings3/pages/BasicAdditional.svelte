<script>
  export let title = "Дополнительно";
  let enableBeta = false;
  let logsEnabled = false;

  function save() {
    console.log('save additional', { enableBeta, logsEnabled });
    alert('Дополнительные настройки сохранены (пример)');
  }
</script>

<div class="settings-page">
  <h2>{title}</h2>
  <div>
    <label><input type="checkbox" bind:checked={enableBeta} /> Включить бета‑функции</label>
  </div>
  <div>
    <label><input type="checkbox" bind:checked={logsEnabled} /> Включить логирование</label>
  </div>
  <div>
    <button on:click={save}>Сохранить</button>
  </div>
</div>