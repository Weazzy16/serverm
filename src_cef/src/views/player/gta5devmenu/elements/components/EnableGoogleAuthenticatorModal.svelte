<script>
  import { createEventDispatcher } from 'svelte';
  import SVGComponent from "../components/SVGComponent.svelte";

  const dispatch = createEventDispatcher();

  let pin = '';
  let pinError = '';
  let qrCode = '';
  let manualCode = '';

  $: isValid = pin.length === 6 && !pinError;

  function onMount() {
    if (window.CEF && window.CEF.call) {
      window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:GetGoogleAuthenticatorQR", (data) => {
        qrCode = data?.qrCode || '';
        manualCode = data?.manualCode || '';
      });
    }
  }

  if (typeof window !== 'undefined') {
    onMount();
  }

  const handlePinInput = (event) => {
    let value = event.target.value.replace(/[^0-9]/g, '');
    if (value.length > 6) value = value.slice(0, 6);
    pin = value;
    pinError = '';
  };

  const handleConfirm = () => {
    if (pin.length !== 6) {
      pinError = 'Код должен содержать 6 цифр';
      return;
    }

    if (window.CEF && window.CEF.call) {
      window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:ConfirmGoogleAuthenticator", {
        code: pin
      });
    }
    closeModal();
  };

  const closeModal = () => {
    dispatch('close');
  };
</script>

<div data-v-322e10d6 data-v-c3f491b2 data-v-5929cd6e-s class="modal-wrapper">
  <div data-v-322e10d6 class="modal">
    <div data-v-322e10d6 class="title">Google Authenticator</div>
    <div data-v-322e10d6 class="close-btn" on:click={closeModal}>
      <SVGComponent path="icons/F4/Settings/cross.svg" />
    </div>

    <div data-v-1180ab84 data-v-322e10d6 class="enableGoogle">
      <!-- Block 1: App Info -->
      <div data-v-1180ab84 class="block">
        <div data-v-1180ab84 class="info">
          <div data-v-1180ab84 class="title">
            Приложение аутентификатор
          </div>
          <div data-v-1180ab84 class="description">
            Скачайте и установите Google Authenticator на смартфон
            или планшет.
          </div>
        </div>
      </div>

      <!-- Block 2: QR Code -->
      <div data-v-1180ab84 class="block">
        <div data-v-1180ab84 class="qrcode">
          {#if qrCode}
            {@html qrCode}
          {/if}
        </div>
        <div data-v-1180ab84 class="info">
          <div data-v-1180ab84 class="title">
            Сканировать QR-код
          </div>
          <div data-v-1180ab84 class="description">
            Откройте приложение для аутентификации и просканируйте
            изображение слева с помощью камеры вашего смартфона.
          </div>
          <div data-v-1180ab84 class="manual">
            <div data-v-1180ab84 class="code">
              {manualCode}
            </div>
            <span data-v-1180ab84>
              Код 2FA <br /> (Ручной ввод)
            </span>
          </div>
        </div>
      </div>

      <!-- Block 3: PIN Input -->
      <div data-v-1180ab84 class="block">
        <img
          data-v-1180ab84
          src="https://cdn.majestic-files.com/public/master/static/img/F4/Settings/Modals/EnableGoogle/2faApp.png"
          alt="2faApp"
        />
        <div data-v-1180ab84 class="info">
          <div data-v-1180ab84 class="title">
            Войти с вашим кодом
          </div>
          <div data-v-1180ab84 class="description">
            Введите сгенерированный 6-значный код подтверждения.
          </div>
          <div data-v-1180ab84 class="enter-pin">
            <input
              type="text"
              inputmode="numeric"
              maxlength="6"
              value={pin}
              on:input={handlePinInput}
              placeholder="000000"
              class="pin-input-field"
            />
            <div data-v-55184d83 data-v-1180ab84 class="pin-input" style="--656ece1a: 6;">
              {#each Array(6) as _, i}
                <div data-v-55184d83 class="pin-symbol" class:active={i < pin.length}>
                  <span data-v-1180ab84 data-v-55184d83-s>
                    {pin[i] || ''}
                  </span>
                </div>
              {/each}
            </div>
            {#if pinError}
              <div data-v-1180ab84 class="error">
                {pinError}
              </div>
            {/if}
          </div>
          <button
            data-v-1180ab84
            class="btn {isValid ? 'active' : 'disabled'}"
            on:click={handleConfirm}
            disabled={!isValid}
          >
            <span data-v-1180ab84>
              {isValid ? 'Подтвердить' : 'Заполните код'}
            </span>
          </button>
        </div>
      </div>
    </div>
  </div>
</div>

<style>
  .pin-input-field {
    position: absolute;
    opacity: 0;
    width: 0;
    height: 0;
    pointer-events: none;
  }
</style>