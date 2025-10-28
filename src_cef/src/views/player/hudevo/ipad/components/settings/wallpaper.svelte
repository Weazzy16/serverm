<script>
    import { translateText } from 'lang'
    import { fade } from 'svelte/transition'
    import { executeClientAsyncToGroup, executeClientToGroup } from 'api/rage';

    export let onSelectedView;

    const wallpapers = [
        document.cloud + "img/iphone/wallpapers/19.png",
        document.cloud + "img/iphone/wallpapers/23.png",
        document.cloud + "img/iphone/wallpapers/34.png",
        document.cloud + "img/iphone/wallpapers/35.png"
        
    ]

    let selectWallpaper = wallpapers[0]
    let defaultWallpaper = wallpapers[0]

    executeClientAsyncToGroup("settings.wallpaper").then((result) => {
        selectWallpaper = result;
        defaultWallpaper = selectWallpaper;
    });

    const onSelectWallpaper = (url) => {
        selectWallpaper = url
    }

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        if (defaultWallpaper !== selectWallpaper)
            executeClientToGroup("settings.wallpaper", selectWallpaper)
    });

</script>
<div class="newphone__settings_flex newphone__project_padding16" in:fade>
    <div class="box-flex" on:keypress={() => {}} on:click={()=> onSelectedView(null)}>
        <div class="phoneicons-Vector-Stroke"></div>
        <div>{translateText('player2', 'Назад')}</div>
    </div>
    <div>{translateText('player2', 'Обои')}</div>
    <div class="box-flex"></div>
</div>
<div class="newphone__settings_imagesselect n-p">
    {#each wallpapers as url}
    <div class="newphone__settings_imageselector" style="background-image: url({url});" class:active={selectWallpaper === url} on:keypress={() => {}} on:click={()=>onSelectWallpaper(url)}>
    </div>
    {/each}
</div>