<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { serverDonatMultiplier } from 'store/server'
    import { validate } from 'api/validation';
    import Anim from './Anim/Anim.svelte';
  
    export let pageload;
    export let pagenameitem;
    export let SetPopup;
    export let searchText;
        export let pagefilter;


    
    

</script>

{#if pageload === 0 || pageload === 1 }
    <Anim {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}

