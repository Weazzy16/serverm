<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { serverDonatMultiplier } from 'store/server'
    import { validate } from 'api/validation';
    import Skin from './Skin/Skin.svelte';
    import './skin/weapon.css';
    export let pageload;
    export let pagenameitem;
    export let SetPopup;
    export let searchText;
        export let pagefilter;


    
    

</script>
<div class="skins">
    <div class="container">
{#if pageload === 0 || pageload === 1 }
    <Skin {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
    </div>
    </div>
