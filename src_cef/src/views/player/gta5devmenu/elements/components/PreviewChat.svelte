<script>
  import { onMount, onDestroy } from "svelte";
  import { storeSettings } from 'store/settings';
  
  const CDN_BG = "https://cdn.majestic-files.com/public/master/static/img/F4/Settings/Chat/ChatSettings/background.jpg";

  let messages = [
    { id: 0, time: "08:45:06", date: "17.09.2024", text: "Ваши документы.", author: "Anti Deviant" },
    { id: 1, time: "08:46:06", date: "17.09.2024", text: "А собственно, в чём проблема?", author: "Ducky Trappa" },
    { id: 2, time: "08:47:06", date: "17.09.2024", text: "Вы превысили скорость", author: "Anti Deviant" },
    { id: 3, time: "08:48:06", date: "17.09.2024", text: "Сколько штраф?", author: "Ducky Trappa" },
    { id: 4, time: "08:49:06", date: "17.09.2024", text: "1000$ или конфискация транспорта.", author: "Anti Deviant" },
    { id: 5, time: "08:50:06", date: "17.09.2024", text: "Вот это прокатился...", author: "Ducky Trappa" }
  ];

  let previewWidth = $storeSettings?.chatWidth ?? 500;
  let previewHeight = $storeSettings?.chatHeight ?? 300;
  let chatTimestamp = $storeSettings?.chatTimestamp ?? "DateTime";
  let chatTheme = $storeSettings?.chatTheme;
  let chatFont = $storeSettings?.chatFont;
  let chatFontStyle = $storeSettings?.chatFontStyle;
  let chatBorderType = $storeSettings?.chatBorderType;
  let chatFontSize = Number($storeSettings?.chatFontSize ?? 12);
  let chatFontShadow = Number($storeSettings?.chatFontShadow ?? 0);
  let chatFontShadowAlpha = Number($storeSettings?.chatFontShadowAlpha ?? 0);
  let chatLinesMargin = Number($storeSettings?.chatLinesMargin ?? 0);
  let chatLineHeight = Number($storeSettings?.chatLineHeight ?? 0);

  // Реактивно следим за всеми значениями
  $: previewWidth = $storeSettings?.chatWidth ?? 500;
  $: previewHeight = $storeSettings?.chatHeight ?? 300;
  $: chatTimestamp = $storeSettings?.chatTimestamp ?? "DateTime";
  $: chatTheme = $storeSettings?.chatTheme;
  $: chatFont = $storeSettings?.chatFont;
  $: chatFontStyle = $storeSettings?.chatFontStyle;
  $: chatBorderType = $storeSettings?.chatBorderType;
  $: chatFontSize = Number($storeSettings?.chatFontSize ?? 12);
  $: chatFontShadow = Number($storeSettings?.chatFontShadow ?? 0);
  $: chatFontShadowAlpha = Number($storeSettings?.chatFontShadowAlpha ?? 0);
  $: chatLinesMargin = Number($storeSettings?.chatLinesMargin ?? 0);
  $: chatLineHeight = Number($storeSettings?.chatLineHeight ?? 0);

  $: cssVars = {
    "--1ae35138": `${chatLinesMargin}px`,
    "--4b7a0df8": "rgb(255, 255, 255)",
    "--5b5ba8f6": chatFont || "ProximaNova",
    "--79b74462": `${chatFontSize}px`,
    "--20e9dace": getFontWeight(chatFontStyle),
    "--06da15a6": (100 + 10 * chatLineHeight) + "%",
    "--edb821ba": "rgba(30, 33, 36, 0.7)",
    "--4308679a": `${chatFontShadow}px`,
    "--1ffd6928": `rgba(26, 26, 26, ${chatFontShadowAlpha})`
  };

  function getFontWeight(style) {
    switch (style) {
      case "Light": return "300";
      case "Regular": return "400";
      case "Semibold": return "500";
      case "Bold": return "700";
      default: return "400";
    }
  }

  $: cssVarsString = Object.entries(cssVars)
    .map(([key, val]) => `${key}: ${val}`)
    .join("; ");
</script>

<div data-v-47425aa0 class="preview" style="{cssVarsString}">
  <img data-v-47425aa0 src={CDN_BG} alt="chat background" />

  <div data-v-47425aa0 class="messages-wrapper">
    <div data-v-47425aa0 class="messages">
      {#each messages as m}
        <div data-v-47425aa0 class="message" class:Block={chatTheme === "Block"} class:Clean={chatTheme === "Clean"} class:Stroke={chatBorderType === "Stroke"} class:Shadow={chatBorderType === "Shadow"}>
          {#if chatTimestamp !== "No"}
            <span data-v-47425aa0>[{chatTimestamp === "DateTime" ? `${m.date} ${m.time}` : m.time}]</span>
          {/if}
          <span data-v-47425aa0>{m.author} говорит:</span>
          <span data-v-47425aa0 class="text">{m.text}</span>
        </div>
      {/each}
    </div>
  </div>
</div>