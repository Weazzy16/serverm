<script>
    import { executeClient } from 'api/rage'
    import { accountRedbucks } from 'store/account'
    import { serverDonateDoubleConvert } from 'store/server'

    const onChange1 = () => {
        let moneytransfer1 = 100;
        if ($accountRedbucks < moneytransfer1)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        executeClient ("client.donate.change", moneytransfer1);
        return window.notificationAdd(2, 9, `Вы приобрели 10 000$`, 3000);
    }

    const onChange2 = () => {
        let moneytransfer2 = 500;
        if ($accountRedbucks < moneytransfer2)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        executeClient ("client.donate.change", moneytransfer2);
        return window.notificationAdd(2, 9, `Вы приобрели 50 000$`, 3000);
    }

    const onChange3 = () => {
        let moneytransfer3 = 2000;
        if ($accountRedbucks < moneytransfer3)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        executeClient ("client.donate.change", moneytransfer3);
        return window.notificationAdd(2, 9, `Вы приобрели 200 000$`, 3000);
    }

    const onChange4 = () => {
        let moneytransfer4 = 5000;
        if ($accountRedbucks < moneytransfer4)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        executeClient ("client.donate.change", moneytransfer4);
        return window.notificationAdd(2, 9, `Вы приобрели 500 000$`, 3000);
    }

    const onChange5 = () => {
        let moneytransfer5 = 20000;
        if ($accountRedbucks < moneytransfer5)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        executeClient ("client.donate.change", moneytransfer5);
        return window.notificationAdd(2, 9, `Вы приобрели 2 000 000$`, 3000);
    }

    const onChange6 = () => {
        let moneytransfer6 = 100000;
        if ($accountRedbucks < moneytransfer6)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        executeClient ("client.donate.change", moneytransfer6);
        return window.notificationAdd(2, 9, `Вы приобрели 10 000 000$`, 3000);
    }
</script>

<div class="list3">
    <div class="blockmoney">
        <div class="iconitem">
            <img src="https://cdn.majestic-files.com/public/master/static/img/donate/products/10k.png" alt=""/>
        </div>
        <div class="infoitem">
            <h1>10 000$</h1>
            <span>10 000$ на ваш игровой счёт</span>
            <div class="buyinfoitem">
                <p>
                    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                    100
                </p>
                <div class="btnbuyitem" on:keypress on:click={onChange1}>Приобрести</div>
            </div>
        </div>
    </div>
    <div class="blockmoney">
        <div class="iconitem">
            <img src="https://cdn.majestic-files.com/public/master/static/img/donate/products/50k.png" alt=""/>
        </div>
        <div class="infoitem">
            <h1>50 000$</h1>
            <span>50 000$ на ваш игровой счёт</span>
            <div class="buyinfoitem">
                <p>
                    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                    500
                </p>
                <div class="btnbuyitem" on:keypress on:click={onChange2}>Приобрести</div>
            </div>
        </div>
    </div>
    <div class="blockmoney">
        <div class="iconitem">
            <img src="https://cdn.majestic-files.com/public/master/static/img/donate/products/200k.png" alt=""/>
        </div>
        <div class="infoitem">
            <h1>200 000$</h1>
            <span>200 000$ на ваш игровой счёт</span>
            <div class="buyinfoitem">
                <p>
                    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                    2 000
                </p>
                <div class="btnbuyitem" on:keypress on:click={onChange3}>Приобрести</div>
            </div>
        </div>
    </div>
    <div class="blockmoney">
        <div class="iconitem">
            <img src="https://cdn.majestic-files.com/public/master/static/img/donate/products/500k.png" alt=""/>
        </div>
        <div class="infoitem">
            <h1>500 000$</h1>
            <span>500 000$ на ваш игровой счёт</span>
            <div class="buyinfoitem">
                <p>
                    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                    5 000
                </p>
                <div class="btnbuyitem" on:keypress on:click={onChange4}>Приобрести</div>
            </div>
        </div>
    </div>
    <div class="blockmoney">
        <div class="iconitem">
            <img src="https://cdn.majestic-files.com/public/master/static/img/donate/products/2kk.png" alt=""/>
        </div>
        <div class="infoitem">
            <h1>2 000 000$</h1>
            <span>2 000 000$ на ваш игровой счёт</span>
            <div class="buyinfoitem">
                <p>
                    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                    20 000
                </p>
                <div class="btnbuyitem" on:keypress on:click={onChange5}>Приобрести</div>
            </div>
        </div>
    </div>
    <div class="blockmoney">
        <div class="iconitem">
            <img src="https://cdn.majestic-files.com/public/master/static/img/donate/products/10kk.png" alt=""/>
        </div>
        <div class="infoitem">
            <h1>10 000 000$</h1>
            <span>10 000 000$ на ваш игровой счёт</span>
            <div class="buyinfoitem">
                <p>
                    <img src="http://cdn.piecerp.ru/cloud/inventoryItems/donate/pconin.svg" alt="icon" />
                    100 000
                </p>
                <div class="btnbuyitem" on:keypress on:click={onChange6}>Приобрести</div>
            </div>
        </div>
    </div>
</div>