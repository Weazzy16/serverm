
<script>
    import { format } from "api/formatter";
    import {accountRedbucks} from "store/account";
    import { charWanted, charMoney, charBankMoney } from 'store/chars'
    import {executeClient} from "api/rage";
    import {validate} from "api/validation";
    export let SetPopup;

    let number = "";

    const getPrice = (text) => {
        text = text.toLowerCase()
        if (text.match(/^([A-Za-z]{1,1})([0-9]{1,1})([0-9]{1,1})([0-9]{1,1})([A-Za-z]{1,1})$/)) {
            let coincidence = 0

            for (var i = 1; i < 4; i++) {
                for (var x = i + 1; x < 4; x++) {
                    if (text[i] == text[x]) coincidence++;
                }
            }

            if (text[0] != text[4]) {
                if (coincidence == 0)
                    return [500000, "$", "Обычный"];//вирты
                else if (coincidence == 1 && text[1] != text[3])
                    return [3000, "", "Редкий"];
                else if (coincidence == 1 && text[1] == text[3])
                    return [5000, "", "Уникальный"];
                else if (coincidence == 3)
                    return [7500, "", "Уникальный"];
            } else if (text[0] == text[4]) {
                if (coincidence == 0)
                    return [3000, "", "Редкий"];
                else if (coincidence == 1 && text[1] != text[3])
                    return [5000, "", "Уникальный"];
                else if (coincidence == 1 && text[1] == text[3])
                    return [7500, "", "Уникальный"];
                else if (coincidence == 3)
                    return [10000, "", "Люкс"];
            }
        }
        return [30000, "", "Люкс"];
    }

    const onBuy = () => {
        let check = validate("vehicleNumber", number);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }
        const numberData = getPrice (number);

        if (numberData[1] === "RB" && $accountRedbucks < numberData[0])
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        if (numberData[1] === "$" && $charMoney < numberData[0])
            return window.notificationAdd(4, 9, `Недостаточно денег!`, 3000);

        else {

            if (!window.loaderData.delay ("donate.onBuy", 1.5))
                return;

            executeClient ("client.donate.buyVehNumber", number);
        }
    }

    const exit = () => {
        SetPopup ()
    }
    function formatMoney(num) {
    return new Intl.NumberFormat('ru-RU', {
      maximumFractionDigits: 0
    }).format(num);
  }
</script>

<div class="opendialogpersm"  on:mouseenter on:mouseleave>
    <div class="dialogpm">
        <div class="headdial">
            <h1>Номер автомобиля</h1>
            <span on:keypress on:click={exit}>
                <svg width="13" height="13" viewBox="0 0 13 13" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M1.5 1.5L6.5 6.5M11.5 11.5L6.5 6.5M6.5 6.5L11.5 1.5L1.5 11.5" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>                                        
            </span>                                    
        </div>
        <div class="infodial">
            <p>Введите желаемый номер. Номер не должен начинаться с 0. {getPrice (number)[2]} номер транспортного средства позволит притянуть завистливые взгляды окружающих к вашему авто! Цена формируется динамически в зависимости от количества символов и формата номера.</p>
        </div>
        <input type="text" placeholder="GTA5DEV" bind:value={number} maxLength={8}>
        <div class="infodialdop">
            <div class="lineidp"></div>
            <div class="idilp">
                <b>Подробнее:</b>
                <p>Введите желаемый номер. Номер не должен начинаться с 0. {getPrice (number)[2]} номер транспортного средства позволит притянуть завистливые взгляды окружающих к вашему авто! Цена формируется динамически в зависимости от количества символов и формата номера.</p>
            </div>
        </div>
        <div class="btnsdial">
            <div class="btndial" on:keypress on:click={onBuy}>
                <p>Купить за</p>
                <svg class="icon"id="Group_1326" data-name="Group 1326" xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 22 22">
                    <circle id="Ellipse_21" data-name="Ellipse 21" cx="11" cy="11" r="11" fill="#e81c5a"/>
                    <path id="Path_6385" data-name="Path 6385" d="M6.193-3.412a2,2,0,0,0-.522,1.031c0,.378.183,1.318.692,1.318.326,0,.953-.744,1.148-.992A27.023,27.023,0,0,1,12.5-7.119l.026.026c-.914,1.188-2.571,3.145-2.571,4.711,0,.757.287,1.579,1.148,1.579,1.122,0,3.928-2.219,4.241-3.119l-.052-.5c-1.762,1.71-2.78,2.3-3.145,2.3-.313,0-.457-.378-.457-.639a5.748,5.748,0,0,1,1.5-2.962l2.023-2.467c.157-.2.561-.653.561-.914s-.548-1.122-.9-1.122c-.235,0-.5.274-.666.418C12.5-8.45,11-7.027,9.391-5.644L9.364-5.67c.535-.822,1.945-2.754,1.945-3.719a.936.936,0,0,0-.953-.9c-.535.013-1.644.992-2.127,1.449L6.154-6.832l-.026-.026a16.464,16.464,0,0,0,.809-2.271.967.967,0,0,0-.157-.5l-.313-.548c-.131-.222-.183-.4-.5-.418A11.83,11.83,0,0,0,3.5-9.677L2.03-9,2-9.024c.509-.444.692-.679.692-.757s-.091-.131-.157-.131a.81.81,0,0,0-.326.183c-1.161.822-1.083.8-1.2.953a7.615,7.615,0,0,0-.339,1.54c-.065.313.1.483.313.483.666,0,.039-.026,4.059-1.905a1.3,1.3,0,0,1,.4-.157c.222,0,.457.261-.914,2.714L1.808-2.655c.144.4.4,1.436.953,1.436.418,0,.744-.5.966-.8L5.684-4.652,8.816-8.045c.091-.1.248-.274.4-.274s.2.144-.026.561A19.4,19.4,0,0,1,7.081-4.574Z" transform="translate(3.129 16.731)" fill="#fff"/>
                  </svg>
                <b>{formatMoney( getPrice (number)[0])}{getPrice (number)[1]}</b>                                        
            </div>
            <div class="btndial" on:keypress on:click={exit}>
                <p>Отмена</p>                                       
            </div>
        </div>
    </div>
</div>