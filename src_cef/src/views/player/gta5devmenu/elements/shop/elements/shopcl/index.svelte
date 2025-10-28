<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { serverDonatMultiplier } from 'store/server'
    import { validate } from 'api/validation';
    import Accessories from './clothes/accessories.svelte';
    import Bags from './clothes/armore.svelte';
    import Glasses from './clothes/heat.svelte';
    import Decals from './clothes/outerwear.svelte';
    import Legs from './clothes/pants.svelte';
    import Shoes from './clothes/shoes.svelte';
    import Tops from './clothes/undergarments.svelte';
    import Mask from './clothes/mask.svelte';
    import Hats from './clothes/hats.svelte';
    import Watches from './clothes/watches.svelte';
    export let pageload;
    export let pagenameitem;
    export let SetPopup;
    export let pagefilter;
    export let searchText;

</script>
<div class="propertys">
    <div class="container">
{#if pageload === 0 || pageload === 15 }
    <Accessories {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 5 }
    <Tops {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 8 }
    <Legs {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 14 }
    <Mask {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 9 }
    <Shoes {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 11 }
    <Watches {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 17 }
    <Decals {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 10 }
    <Glasses {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 18 }
    <Bags {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
{#if pageload === 0 || pageload === 4 }
    <Hats {searchText} {pagefilter} {pagenameitem} {pageload} {SetPopup}/>
{/if}
</div>
</div>