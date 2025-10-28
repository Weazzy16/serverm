<script>
  export let direction = 'vertical';
  export let itemSize = 100;
  export let items = [];
  export let keyField = 'id';
  
  let scrollContainer;
  let containerClass = '';
  export { containerClass as class };
</script>

<div bind:this={scrollContainer} class="recycle-scroller {containerClass}">
  {#each items as item, index (item[keyField])}
    <slot {item} {index} />
  {/each}
</div>

<style>
  .recycle-scroller {
    overflow: auto;
  }
</style>