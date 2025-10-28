<script>
  import { createEventDispatcher } from 'svelte';
  import SVGComponent from "../components/SVGComponent.svelte";

  const dispatch = createEventDispatcher();

  let verificationCode = '';
  let newPassword = '';
  let confirmPassword = '';
  let verificationCodeError = 'Поле не может быть пустым!';
  let newPasswordError = 'Поле не может быть пустым!';
  let confirmPasswordError = 'Поле не может быть пустым!';

  $: isValid = verificationCode.trim() && newPassword.trim() && confirmPassword.trim() && newPassword === confirmPassword && !verificationCodeError && !newPasswordError && !confirmPasswordError;

  const handleChange = () => {
    verificationCodeError = '';
    newPasswordError = '';
    confirmPasswordError = '';

    if (!verificationCode.trim()) {
      verificationCodeError = 'Поле не может быть пустым!';
    }
    if (!newPassword.trim()) {
      newPasswordError = 'Поле не может быть пустым!';
    }
    if (!confirmPassword.trim()) {
      confirmPasswordError = 'Поле не может быть пустым!';
    }
    if (newPassword && confirmPassword && newPassword !== confirmPassword) {
      confirmPasswordError = 'Пароли не совпадают!';
    }

    if (isValid) {
      if (window.CEF && window.CEF.call) {
        window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:ChangePassword", {
          verificationCode,
          newPassword
        });
      }
      closeModal();
    }
  };

  const closeModal = () => {
    dispatch('close');
  };

  const validateCode = () => {
    if (!verificationCode.trim()) {
      verificationCodeError = 'Поле не может быть пустым!';
    } else {
      verificationCodeError = '';
    }
  };

  const validatePassword = () => {
    if (!newPassword.trim()) {
      newPasswordError = 'Поле не может быть пустым!';
    } else {
      newPasswordError = '';
    }
  };

  const validateConfirmPassword = () => {
    if (!confirmPassword.trim()) {
      confirmPasswordError = 'Поле не может быть пустым!';
    } else if (newPassword && confirmPassword !== newPassword) {
      confirmPasswordError = 'Пароли не совпадают!';
    } else {
      confirmPasswordError = '';
    }
  };
</script>

<div data-v-322e10d6 data-v-c3f491b2 data-v-5929cd6e-s class="modal-wrapper">
  <div data-v-322e10d6 class="modal">
    <div data-v-322e10d6 class="title">СМЕНА ПАРОЛЯ</div>
    <div data-v-322e10d6 class="close-btn" on:click={closeModal}>
      <SVGComponent path="icons/F4/Settings/cross.svg" />
    </div>
    
    <div data-v-5403346e data-v-322e10d6 class="changePassword">
      <p data-v-5403346e>
        На ваш Email адрес было отправлено письмо с проверочным кодом.
      </p>

      <div data-v-5403346e class="inputs">
        <div data-v-5403346e class:error-state={!!verificationCodeError}>
          <input
            data-v-5403346e
            placeholder="Введите код"
            type="text"
            bind:value={verificationCode}
            on:input={validateCode}
          />
          {#if verificationCodeError}
            <div data-v-5403346e class="error-tooltip">
              {verificationCodeError}
            </div>
          {/if}
        </div>

        <div data-v-5403346e class:error-state={!!newPasswordError}>
          <input
            data-v-5403346e
            placeholder="Новый пароль"
            type="password"
            bind:value={newPassword}
            on:input={validatePassword}
          />
          {#if newPasswordError}
            <div data-v-5403346e class="error-tooltip">
              {newPasswordError}
            </div>
          {/if}
        </div>

        <div data-v-5403346e class:error-state={!!confirmPasswordError}>
          <input
            data-v-5403346e
            placeholder="Повторите пароль"
            type="password"
            bind:value={confirmPassword}
            on:input={validateConfirmPassword}
          />
          {#if confirmPasswordError}
            <div data-v-5403346e class="error-tooltip">
              {confirmPasswordError}
            </div>
          {/if}
        </div>
      </div>

      <div data-v-5403346e class="buttons">
        <button 
          data-v-5403346e 
          class="change {isValid ? 'active' : ''}"
          on:click={handleChange}
          disabled={!isValid}
        >
          <span data-v-5403346e>
            {isValid ? 'Изменить' : 'Заполните поля'}
          </span>
        </button>
        <button 
          data-v-5403346e 
          class="cancel"
          on:click={closeModal}
        >
          <span data-v-5403346e>Отменить</span>
        </button>
      </div>
    </div>
  </div>
</div>