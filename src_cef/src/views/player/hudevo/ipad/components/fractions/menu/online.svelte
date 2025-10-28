<script>

    import { translateText } from 'lang'
    import { addListernEvent } from "api/functions";

    let onlineData = {
        online: 0,
        offline: 0,
        all: 0
    };
    const onOnline = (json) => {
        onlineData = JSON.parse(json);
    }
    addListernEvent ("table.online", onOnline)
</script>

<div class="onlineapp"><p>Колектив онлайн <b>{onlineData.online}</b>/{onlineData.all}</p></div>