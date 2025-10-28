<script>
  import { storeSettings } from 'store/settings';

  const CDN_BG = "https://cdn.majestic-files.com/public/master/static/img/F4/Settings/Chat/ChatSettings/background.jpg";

  const allCommands = [
    "/report", "/b", "/me", "/do", "/try", "/s", "/w", "/m", "/f", "/fb",
    "/c", "/cb", "/dep", "/mark", "/mark2", "/gnews", "/wnews", "/adv", "/vmute", "/pay"
  ];

  const QUICK_COMMANDS_LIMIT = 5;

  let selectedSlot = 0;
  let draggedCmd = null;

  $: chatCmdsSettings = $storeSettings?.chatCmdsSettings;
  $: chatCmdsFont = $storeSettings?.chatCmdsFont;
  $: chatCmdsFontStyle = $storeSettings?.chatCmdsFontStyle;
  $: chatCmdsFontSize = Number($storeSettings?.chatCmdsFontSize ?? 12);
  $: fullChatCmdsOrder = $storeSettings?.chatCmdsOrder ?? [...allCommands];
  
  $: chatCmdsOrder = fullChatCmdsOrder.slice(0, QUICK_COMMANDS_LIMIT);

  $: fontWeight = getFontWeight(chatCmdsFontStyle);
  $: fontSize = (chatCmdsFontSize * 1.48).toFixed(4) + "px";
  
  // ← INLINE СТИЛИ ДЛЯ ТЕКСТА (только если не standard)
  $: textStyle = chatCmdsSettings === "standard" 
    ? "" 
    : `font-family: var(--5b5ba8f6); font-size: ${fontSize}; font-weight: ${fontWeight};`;
  
  $: cssVars = {
    "--b8dab5a0": "#7F63BCB3",
    "--797b065e": "#FFFFFF",
    "--cbfcda16": fontSize,
    "--267c086c": fontWeight,
    "--3488e9c0": QUICK_COMMANDS_LIMIT,
    "--5b5ba8f6": chatCmdsFont === "ProximaNova" ? "'Proxima Nova', sans-serif" : "Arial, sans-serif"
  };

  $: cssVarsString = Object.entries(cssVars)
    .map(([key, val]) => `${key}: ${val}`)
    .join("; ");

  // ← ПРОВЕРКА НА STANDARD
  $: isStandard = chatCmdsSettings === "standard";

  function getFontWeight(style) {
    switch (style) {
      case "Light": return "300";
      case "Regular": return "400";
      case "Semibold": return "500";
      case "Bold": return "700";
      default: return "400";
    }
  }

  function selectSlot(index) {
    // ← БЛОКИРОВКА ДЛЯ STANDARD
    if (isStandard) return;
    if (index < chatCmdsOrder.length) {
      selectedSlot = index;
    }
  }

  function selectCommand(cmd) {
    // ← БЛОКИРОВКА ДЛЯ STANDARD
    if (isStandard || !chatCmdsOrder) return;
    
    const newOrder = [...fullChatCmdsOrder];
    const currentCmd = newOrder[selectedSlot];

    if (newOrder.includes(cmd)) {
      const idx = newOrder.indexOf(cmd);
      newOrder[idx] = currentCmd;
      newOrder[selectedSlot] = cmd;
    } else {
      newOrder[selectedSlot] = cmd;
    }

    storeSettings.update(v => ({
      ...v,
      chatCmdsOrder: newOrder
    }));
  }

  function dragStart(e, cmd) {
    if (isStandard) {
      e.preventDefault();
      return;
    }
    draggedCmd = cmd;
    e.dataTransfer.effectAllowed = "move";
  }

  function dragOver(e) {
    if (isStandard) {
      e.preventDefault();
      return;
    }
    e.preventDefault();
    e.dataTransfer.dropEffect = "move";
  }

  function drop(e, targetCmd) {
    if (isStandard) {
      e.preventDefault();
      return;
    }
    e.preventDefault();
    
    if (!draggedCmd || draggedCmd === targetCmd) return;

    const newOrder = [...fullChatCmdsOrder];
    const dragIndex = newOrder.indexOf(draggedCmd);
    const targetIndex = newOrder.indexOf(targetCmd);

    const temp = newOrder[dragIndex];
    newOrder[dragIndex] = newOrder[targetIndex];
    newOrder[targetIndex] = temp;

    storeSettings.update(v => ({
      ...v,
      chatCmdsOrder: newOrder
    }));

    draggedCmd = null;
  }
</script>

<div class="chat-cmds" style={cssVarsString} class:disabled={isStandard}>
  <!-- PREVIEW SECTION -->
  <div class="preview">
    <img src={CDN_BG} alt="chat background" />
    
    <div class="preview-wrapper">
      <div class="input-wrapper">
        <div class="template">
          <div class="selected-cmd" style={textStyle}>
            {#if chatCmdsOrder && chatCmdsOrder[selectedSlot]}
              {chatCmdsOrder[selectedSlot]}
            {:else}
              /report
            {/if}
          </div>
          <div class="cursor"></div>
        </div>
      </div>

      <div class="favorites-cmds">
        {#each chatCmdsOrder as cmd, idx (idx)}
          <div
            class="favorites-cmds-item"
            class:selected={selectedSlot === idx}
            class:disabled={isStandard}
            on:click={() => selectSlot(idx)}
            role="button"
            tabindex={isStandard ? -1 : 0}
            style={textStyle}
          >
            <span>{cmd}</span>
          </div>
        {/each}
      </div>
    </div>
  </div>

  <!-- SLOTS SECTION -->
  <div class="slots-wrapper" class:disabled={isStandard}>
    <div class="slots">
      {#each chatCmdsOrder as cmd, idx (idx)}
        <div
          class="slot"
          class:opened={selectedSlot === idx}
          class:disabled={isStandard}
          on:click={() => selectSlot(idx)}
          role="button"
          tabindex={isStandard ? -1 : 0}
          style={textStyle}
        >
          <span>{cmd}</span>
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 14 14" fill="none">
            <path d="M12 4.5C11.595 5.20125 7 9.5 7 9.5L2 4.5"/>
          </svg>
        </div>
      {/each}
    </div>

    <div class="opened-slot" class:left={selectedSlot === 0} class:right={selectedSlot === (chatCmdsOrder?.length - 1)}>
      {#each allCommands as cmd (cmd)}
        <div
          class="cmd"
          class:active={chatCmdsOrder?.includes(cmd)}
          class:disabled={isStandard}
          on:click={() => selectCommand(cmd)}
          role="button"
          tabindex={isStandard ? -1 : 0}
          draggable={!isStandard}
          on:dragstart={(e) => dragStart(e, cmd)}
          on:dragover={dragOver}
          on:drop={(e) => drop(e, cmd)}
          style={textStyle}
        >
          {#if chatCmdsOrder?.includes(cmd)}
            <div class="selected-slot-index">
              <span>{chatCmdsOrder.indexOf(cmd) + 1}</span>
            </div>
          {/if}
          <span>{cmd}</span>
        </div>
      {/each}
    </div>
  </div>
</div>