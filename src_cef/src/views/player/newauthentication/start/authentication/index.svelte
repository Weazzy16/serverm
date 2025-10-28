<script>
    import { translateText } from "lang";
    import { accountIsSession } from "store/account";
    import { executeClient } from "api/rage";
    import InputCustom from "components/input/index.svelte";
    import { accountLogin } from "store/account";
    import { onMount } from "svelte";
    export let OnSelectViews;
    let loginInput;
    let loginIsFocus = false;
    let passwordInput;
    let passwordIsFocus = false;

    import logo from "../../img/logo.svg";
    import upi from "../../img/upi.svg";
    import user from "../../img/user.svg";
    import password from "../../img/password.svg";

    onMount(() => {
        accountLogin.subscribe((value) => {
            if (value == -99) return;
            if (value == -1 || value == "-1") loginIsFocus = true;
            else {
                passwordIsFocus = true;
                loginInput = value;
            }
        });
    });

    const onLogin = () => {
        if (
            loginInput &&
            passwordInput &&
            !$accountIsSession &&
            loginInput.length &&
            passwordInput.length &&
            $accountLogin !== -99
        )
            executeClient("client:OnSignInv2", loginInput, passwordInput);
    };

    const onKeyUp = (event) => {
        const { keyCode } = event;

        if (keyCode === 13) onLogin();
    };



    document.addEventListener('DOMContentLoaded', function() {
            const audio = document.getElementById('backgroundAudio');
            audio.volume = 0.5;

            document.addEventListener('keydown', function(event) {
                if (event.key === 'Enter') {
                    document.getElementById('welcomeScreen').style.display = 'none';
                    audio.pause();
                }
            });
        });
</script>

<svelte:window on:keyup={onKeyUp} />
<!-- svelte-ignore a11y-click-events-have-key-events -->

<!-- <div class="welcome-screen" id="welcomeScreen">
    <div class="text__block__welcome">
        <div>Добро пожаловать</div>
        <div>ASTRA RolePlay</div>
        <div class="click__enter">Пожалуйста, нажмите <span style="font-weight:bold; color:red;">ENTER</span>, чтобы продолжить</div>
    </div>

    <div class="background-overlay"></div>
 
    <video class="background-video" autoplay muted loop>
        <source src="http://cdn.piecerp.ru/src/views/player/newauthentication/images/back22.mp4" type="video/mp4">
    </video>

    <audio id="backgroundAudio" autoplay loop>
        <source src="http://cdn.piecerp.ru/src/views/player/newauthentication/images/music_back.mp3" type="audio/mpeg">
    </audio>
</div> -->



<!-- <div class="background"></div> -->
<div class="man-container">
    <img class="man" src="http://cdn.piecerp.ru/src/views/player/newauthentication/images/man.png" alt="Персонаж">
</div>

<div class="auth">
    <div class="logosvg__auth">
        <!-- <img src="https://i.ibb.co/SRqZdSV/logo.png" alt="logo" /> -->
    </div>
    <div class="auth__content">
        <div class="global">
             <!-- <div class="first">
                <div class="upi1">
                    <img src={upi} alt="upi1" />
                </div>
                <div class="unicmode">
                    <a class="title">Глубокий Ролевой Опыт</a>
                    <span class="text"
                        >Наш сервер для GTA 5 RP предлагает игрокам погрузиться
                        в настоящий мир ролевой игры. Мы поддерживаем
                        разнообразные характеры и сценарии, позволяя игрокам
                        создавать уникальные личности и развивать их в
                        уникальных историях
                    </span>
                </div>
            </div>
            <div class="first">
                <div class="upi2">
                    <img src={upi} alt="upi2" />
                </div>
                <div class="designmode">
                    <a class="title">Инновационные Механики Геймплея</a>
                    <span class="text">
                        Мы предлагаем новые и улучшенные механики геймплея,
                        которые обогащают взаимодействие игроков. Системы
                        экономики, работы, взаимодействия с объектами и другие
                        элементы были переработаны для более глубокого и
                        реалистичного опыта
                    </span>
                </div>
            </div>
            <div class="first">
                <div class="upi3">
                    <img src={upi} alt="upi3" />
                </div>
                <div class="systemmode">
                    <a class="title">Активное Развитие и Сообщество</a>
                    <span class="text">
                        Мы постоянно работаем над улучшениями сервера, внедряя
                        новые функции и реагируя на обратную связь от игроков.
                        Наше дружелюбное и отзывчивое сообщество способствует
                        активному обмену идеями и помощи новичкам, создавая
                        ливую атмосферу
                    </span>
                </div>
            </div> -->
        </div>
        <div class="auth_details">
            <div class="athmode1">
                <a>s</a><b>Авторизация</b>
            </div>
            <div class="inputs__auth">
                <div class="us">
                    <img class="user" src={user} alt="user" />
                    <InputCustom
                        setValue={(value) => (loginInput = value)}
                        value={loginInput}
                        isFocus={loginIsFocus}
                        placeholder="Введите логин"
                        type="text"
                    />
                </div>
                <div class="pass">
                    <img class="password" src={password} alt="passowrd" />
                    <InputCustom
                        setValue={(value) => (passwordInput = value)}
                        value={passwordInput}
                        isFocus={passwordIsFocus}
                        placeholder="Введите пароль"
                        type="password"
                    />
                </div>
                <div class="button_login" on:click={onLogin}>Войти</div>
                <!-- <div class="button_login" on:click={() => OnSelectViews("Registration")}>РЕГИСТРАЦИЯ</div> -->
            </div>

            <!-- <div class="restore_btn" on:click={() => OnSelectViews("Restore")}>Забыли пароль?</div> -->
        </div>
    </div>
</div>
