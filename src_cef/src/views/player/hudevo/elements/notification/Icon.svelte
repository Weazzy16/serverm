<script>
    export let type;
</script>

<div class="icon">
    {#if type === 'success'}
        <img src="http://cdn.piecerp.ru/cloud/notification/success.svg" alt="Успех" />
    {:else if type === 'error'}
        <img src="http://cdn.piecerp.ru/cloud/notification/error.svg" alt="Ошибка" />
    {:else if type === 'information'}
        <img src="http://cdn.piecerp.ru/cloud/notification/information.svg" alt="Информация" />
    {/if}
</div>

<style>
    .icon {
        width: 24px;
        height: 24px;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-right: 15px; 
    
    }
</style>
