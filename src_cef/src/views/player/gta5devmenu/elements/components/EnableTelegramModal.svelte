<script>
  import { createEventDispatcher } from 'svelte';
  import SVGComponent from "../components/SVGComponent.svelte";

  const dispatch = createEventDispatcher();

  let qrCode = '';

  function onMount() {
    if (window.CEF && window.CEF.call) {
      window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:GetTelegramQR", (data) => {
        qrCode = data?.qrCode || '';
      });
    }
  }

  if (typeof window !== 'undefined') {
    onMount();
  }

  const handleOpenBot = () => {
    if (window.CEF && window.CEF.call) {
      window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:OpenTelegramBot");
    }
  };

  const closeModal = () => {
    dispatch('close');
  };
</script>

<div data-v-322e10d6 data-v-c3f491b2 data-v-5929cd6e-s class="modal-wrapper">
  <div data-v-322e10d6 class="modal">
    <div data-v-322e10d6 class="title">Подключение Telegram</div>
    <div data-v-322e10d6 class="close-btn" on:click={closeModal}>
      <SVGComponent path="icons/F4/Settings/cross.svg" />
    </div>

    <div data-v-26a559e4 data-v-322e10d6 class="enableTelegram">
      <!-- Block 1: App Info -->
      <div data-v-26a559e4 class="block">
        <div data-v-26a559e4 class="info">
          <div data-v-26a559e4 class="title">
            Приложение Telegram
          </div>
          <div data-v-26a559e4 class="description">
            Скачайте и установите Telegram на смартфон или планшет.
          </div>
        </div>
      </div>

      <!-- Block 2: QR Code -->
      <div data-v-26a559e4 class="block">
        <div data-v-26a559e4 class="qrcode">
          {#if qrCode}
            {@html qrCode}
          {/if}
        </div>
        <div data-v-26a559e4 class="info">
          <div data-v-26a559e4 class="title">
            Сканировать QR-код
          </div>
          <div data-v-26a559e4 class="description">
            Отсканируйте изображение слева с помощью камеры вашего
            смартфона. Нажмите кнопку «Запустить» и следуйте
            инструкциям бота.
          </div>
          <button 
            data-v-26a559e4
            class="btn active"
            on:click={handleOpenBot}
          >
                  </button>
        </div>
      </div>
    </div>
  </div>
</div>