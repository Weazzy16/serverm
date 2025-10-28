<script>
    import { executeClient } from 'api/rage'

    import './assets/css/petrol.css'

    let input = "";

    window.petrol = {
        reset: () => {
            input = ""
        }        
    }  

    const gov = () => {
        executeClient ('petrol.gov');
    }

    const full = () => {
        executeClient ('petrol.full');
    }

    const yes = () => {
        executeClient ('petrol', parseInt(input));
    }

    const no = () => {
        executeClient ('closePetrol');
    }

</script>

<div class="petrol-menu">
    <input maxLength="3" bind:value={input} />
    <div class="btn-gov">
        <div class="btn gov" on:click={gov}></div>
    </div>
    <div class="btn-full">
        <div class="btn full" on:click={full}></div>
    </div>
    <div class="btn-yes">
        <div class="btn2 yes" on:click={yes}></div>
    </div>
    <div class="btn-no">
        <div class="btn2 no" on:click={no}></div>
    </div>
</div>