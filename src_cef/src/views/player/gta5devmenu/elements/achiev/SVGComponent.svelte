<script>
  /**
   * SVGComponent — универсальный компонент для отображения SVG-иконок
   * Подставляет базовый путь, если передан относительный путь.
   * Props:
   *  - path: string — относительный путь внутри CDN или полный URL до SVG
   *  - alt: string — альтернативный текст (по умолчанию пустой)
   *  - width, height: number|string — размеры (px, em, % и т.п.)
   */
  export let path;
  export let alt = '';
  export let width;
  export let height;

  const BASE_URL = 'https://cdn.majestic-files.com/public/master/static';

  // вычисляем src: если path начинается с http(s), используем его напрямую, иначе приставляем BASE_URL
  $: src = /^(https?:)?\/\//.test(path) ? path : `${BASE_URL}/${path}`;
</script>

<img
  src={src}
  {alt}
  {width}
  {height}
  {...$$restProps}
/>