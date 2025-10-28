<script>
  let code = "";

  export let pageName = null;

  function onLoad() {
    if (code && pageName) {
      // Отправить команду загрузки
      console.log("Load settings:", { pageName, code });
    }
  }

  function onShare() {
    if (pageName) {
      // Отправить команду поделиться
      console.log("Share settings:", { pageName });
    }
  }
</script>

<div class="apply-setting-code">
  <div class="header">
    <div class="name">Применить настройки</div>
  </div>
  <div class="input-wrapper">
    <input
      type="text"
      maxlength="512"
      placeholder="Введите код"
      bind:value={code}
    />
  </div>
  <div class="btn-wrapper">
    <div class="btn" class:disabled={!code} on:click={onLoad}>
      <span>Загрузить</span>
    </div>
    <div class="btn" on:click={onShare}>
      <span>Поделиться</span>
    </div>
  </div>
</div>