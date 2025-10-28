<script>
  import { createEventDispatcher } from 'svelte';
  import SVGComponent from "../components/SVGComponent.svelte";

  const dispatch = createEventDispatcher();

  let password = '';
  let email = '';
  let passwordError = 'Поле не может быть пустым!';
  let emailError = 'Поле не может быть пустым!';

  $: isValid = password.trim() && email.trim() && !passwordError && !emailError;

  const handleChange = () => {
    passwordError = '';
    emailError = '';

    if (!password.trim()) {
      passwordError = 'Поле не может быть пустым!';
    }
    if (!email.trim()) {
      emailError = 'Поле не может быть пустым!';
    }

    if (password.trim() && email.trim()) {
      if (window.CEF && window.CEF.call) {
        window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:ChangeEmail", {
          password,
          email
        });
      }
      closeModal();
    }
  };

  const closeModal = () => {
    dispatch('close');
  };

  const validatePassword = () => {
    if (!password.trim()) {
      passwordError = 'Поле не может быть пустым!';
    } else {
      passwordError = '';
    }
  };

  const validateEmail = () => {
    if (!email.trim()) {
      emailError = 'Поле не может быть пустым!';
    } else {
      emailError = '';
    }
  };
</script>

<div data-v-322e10d6 data-v-c3f491b2 data-v-5929cd6e-s class="modal-wrapper">
  <div data-v-322e10d6 class="modal">
    <div data-v-322e10d6 class="title">СМЕНА EMAIL</div>
    <div data-v-322e10d6 class="close-btn" on:click={closeModal}>
      <SVGComponent path="icons/F4/Settings/cross.svg" />
    </div>
    
    <div data-v-5403346e data-v-322e10d6 class="changeEmail">
      <p data-v-5403346e>
        Для смены Email необходимо ввести пароль и новый Email.
      </p>

      <div data-v-5403346e class="inputs">
        <div data-v-5403346e class:error-state={!!passwordError}>
          <input
            data-v-5403346e
            placeholder="Введите пароль"
            type="password"
            bind:value={password}
            on:input={validatePassword}
          />
          {#if passwordError}
            <div data-v-5403346e class="error-tooltip">
              {passwordError}
            </div>
          {/if}
        </div>

        <div data-v-5403346e class:error-state={!!emailError}>
          <input
            data-v-5403346e
            placeholder="Новый Email"
            type="text"
            bind:value={email}
            on:input={validateEmail}
          />
          {#if emailError}
            <div data-v-5403346e class="error-tooltip">
              {emailError}
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