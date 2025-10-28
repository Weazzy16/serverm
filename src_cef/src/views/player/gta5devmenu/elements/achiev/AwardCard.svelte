<!-- src/lib/AwardCard.svelte -->
<script>
  // Сопоставление ключа редкости к цвету.
  const COLOR_MAP = {
    common: '#3B82F6',  // синий
    rare:   '#8958FF',  // фиолетовый
    epic:   '#10B981',  // зелёный
    // добавьте нужные вам...
  };

  /** 
   * Props:
   *  - award: { color: string, value: number | string, type: string }
   *  - imagePath: URL картинки предмета
   *  - title:    Название предмета
   */
  export let award;
  export let imagePath;
  export let title;
</script>

<style>
  /* Берём ваши существующие классы */
  @import './achiv.css'; /* или путь к вашему CSS */

  /* Если нужно, небольшие донастройки: */
  .award-card__rarity svg path {
    transition: fill 0.3s;
  }
</style>

<div class="award-card award-card__rarity-{award.color}">
  <!-- цветная полоса редкости -->
  <div class="award-card__rarity">
    <svg fill="none" viewBox="0 0 16 4" xmlns="http://www.w3.org/2000/svg">
      <path
        d="M14 0H2C0.89543 0 0 0.89543 0 2C0 3.10457 0.89543 4 2 4H14C15.1046 4 16 3.10457 16 2C16 0.89543 15.1046 0 14 0Z"
        fill="{COLOR_MAP[award.color] || '#777'}"
      />
    </svg>
  </div>

  <!-- ваш градиент -->
  <div class="award-card__rarity__radial-gradient"></div>

  <!-- картинка предмета -->
  <img
    class="award-card__media {award.type}"
    src="{imagePath}"
    alt="{title}"
  />
</div>
  <!-- заголовок -->
  <div class="award-card__info">
    <div class="award-card__info-title">{title}</div>
  </div>

