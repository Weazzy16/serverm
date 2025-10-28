<script>
    import { currentPage, selectNumber } from '../stores'

    const onMain = () => {
        currentPage.set("mainmenu")
        selectNumber.set(null)
    }
</script>
<div class="newtablet__homebuttonf" on:click={onMain}>
    <div class="newtablet__homebutton_buttonf"></div>
</div>