<script>
  export let path = '';
  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  
  let svgContent = '';
  
  $: fullPath = path.startsWith('http') ? path : `${cdn}${path}`;
  
  $: loadSvg(fullPath);
  
  async function loadSvg(url) {
    try {
      const response = await fetch(url);
      svgContent = await response.text();
    } catch (error) {
      console.error('Failed to load SVG:', url, error);
      svgContent = '';
    }
  }
</script>

{#if svgContent}
  {@html svgContent}
{:else}
  <span>...</span>
{/if}

<style>
  :global(svg) {
    display: block;
  }
</style>