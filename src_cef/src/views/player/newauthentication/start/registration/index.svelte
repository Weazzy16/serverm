<script>
    import { accountLogin } from "store/account";
    import InputCustom from "components/input/index.svelte";
    import { executeClient } from "api/rage";

    import user from "../../img/user.svg";
    import password from "../../img/password.svg";
    import email from "../../img/email.svg";
    import logo from "../../img/logo.svg";
    import upi from "../../img/upi.svg";
    import promo from "../../img/promo.svg";

    let regusername = "",
        regemail = "",
        regpass1 = "",
        regpass2 = "",
        regpromo = "";

    const onRegister = () => {
        if (
            regusername &&
            regemail &&
            regpass1 &&
            regpass2 &&
            $accountLogin !== -99
        ) {
            executeClient(
                "client:OnSignUpv2",
                regusername,
                regemail,
                regpromo,
                regpass1,
                regpass2
            );
        }
    };

    const onKeyUp = (event) => {
        const { keyCode } = event;

        if (keyCode === 13) onRegister();
    };
</script>

<svelte:window on:keyup={onKeyUp} />
<!-- svelte-ignore a11y-click-events-have-key-events -->
<!-- svelte-ignore missing-declaration -->

<div class="background"></div>

<div class="reg">
    <div class="logosvg__reg">
        <!-- <img src="https://i.ibb.co/SRqZdSV/logo.png" alt="logo" /> -->
    </div>
    <div class="global">
        <div class="athmode__reg">
            <a>РЕГИСТРАЦИЯ</a><br />
            <b>Создание нового аккаунта!</b>
        </div>
    </div>
    <div class="reg_details">
        <div class="inputs__reg">
            <div class="login__reg">
                <img class="user" src={user} alt="user" />
                <InputCustom
                    setValue={(value) => (regusername = value)}
                    value={regusername}
                    placeholder="Логин"
                    type="text"
                />
            </div>
            <div class="pass__reg">
                <img class="password" src={password} alt="password" />
                <InputCustom
                    setValue={(value) => (regpass1 = value)}
                    value={regpass1}
                    placeholder="Пароль"
                    type="password"
                />
            </div>
            <div class="repet-pass__reg">
                <img class="password" src={password} alt="password" />
                <InputCustom
                    setValue={(value) => (regpass2 = value)}
                    value={regpass2}
                    placeholder="Пароль"
                    type="password"
                />
            </div>
            <div class="email__reg">
                <img class="email" src={email} alt="email" />
                <InputCustom
                    setValue={(value) => (regemail = value)}
                    value={regemail}
                    placeholder="Почта"
                    type="email"
                />
            </div>
            <div class="promocode__reg">
                <img class="promo" src={promo} alt="email" />
                <InputCustom
                    setValue={(value) => (regpromo = value)}
                    value={regpromo}
                    placeholder="Промокод"
                    type="text"
                />
            </div>
        </div>
        <div class="button_reg" on:click={onRegister}>
            <p>Зарегистрироваться</p>
        </div>
        <div class="button_reg" 
        on:click={() => OnSelectViews("Authentication")}>
            Вернутся
        </div>
    </div>
</div>
