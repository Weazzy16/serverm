<script>
  export let anim;
  import { createEventDispatcher } from 'svelte';
  const dispatch = createEventDispatcher();

  function close() { dispatch('close'); }
  function buy()   { dispatch('buy');   }
</script>

<div class="modal-backdrop" on:click={close}>
  <div class="modal" on:click|stopPropagation>
    <button class="close" on:click={close}>✕</button>
    <h2>{anim.title}</h2>
    <div class="preview">
      <img src={anim.img} alt={anim.title} />
    </div>
    <button class="btn-buy" on:click={buy}>
      Купить за {anim.price.toLocaleString('ru-RU')} 
      <svg class="icon"><use xlink:href="icons/panelMenu/donate/cases/mcoin.svg#icon"/></svg>
    </button>
  </div>
</div>
