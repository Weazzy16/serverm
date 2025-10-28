<script>
    export let OnSelectViews;

    import { executeClient } from "api/rage";
    import InputCustom from "components/input/index.svelte";

    import logo from "../../img/logo.svg";
    import email from "../../img/email.svg";

    const onKeyUp = (event) => {
        const { keyCode } = event;
        if (keyCode === 13) onSubmitRestore();
    };

    let restoretype = false,
        placeholder = "Логин",
        restorelength = 50,
        restoreInput = "";

    const SetStep = (step) => {
        if (step == 0) {
            restoretype = false;
            placeholder = "Логин";
            restorelength = 50;
            restoreInput = "";
        } else if (step == 1) {
            restoretype = true;
            restorelength = 6;
            placeholder = "Код из письма";
            restoreInput = "";
        } else if (step == 2) {
        }
    };
    window.events.addEvent("cef.authentication.restoreStep", SetStep);

    import { onDestroy } from "svelte";

    onDestroy(() => {
        window.events.removeEvent("cef.authentication.restoreStep", SetStep);
    });

    const onSubmitRestore = (event) => {
        if (restoreInput) {
            if (restoretype == false) {
                if (restoreInput.length != 0) {
                    executeClient("restorepass", 0, restoreInput);
                    restorelength = 0;
                    placeholder = "Отправка сообщения...";
                    restoreInput = "";
                }
            } else {
                if (restoreInput.length == 6) {
                    executeClient("restorepass", 1, restoreInput);
                    restorelength = 0;
                    placeholder = "Происходит авторизация";
                    restoreInput = "";
                } else restoreInput = "";
            }
            return false;
        }
    };
</script>

<svelte:window on:keyup={onKeyUp} />
<!-- svelte-ignore a11y-click-events-have-key-events -->
<div class="res">
    <div class="res_details">
        <!-- <img class="logo" src={logo} alt="logo" /> -->
        <div class="name_res">ВОССТАНОВЛЕНИЕ</div>
        <div class="inputs_res">
            <InputCustom
                setValue={(value) => (restoreInput = value)}
                value={restoreInput}
                type="text"
            />
            <div class="input_res">Ваш email</div>
            <img class="email" src={email} alt="email" />
            <div
                style="margin-top: -27vh;"
                class="buttons"
                on:click={onSubmitRestore}
            >
                <p>ВОССТАНОВИТЬ</p>
            </div>
            <div
                style="margin-top: -27.5vh;margin-left: 1vh;"
                class="restore_btn"
                on:click={() => OnSelectViews("Authentication")}
            >
                Есть аккаунт
            </div>
        </div>
    </div>
</div>
