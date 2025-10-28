<script>
  import { onMount, onDestroy, tick } from 'svelte';
  import { translateText } from 'lang';
  import moment from 'moment';
  import { executeClient, invokeMethod } from 'api/rage';
  import Commands from 'json/commands.js';
  import { format } from 'api/formatter';
  import CustomKey from './Key.svelte';
  import { isInputToggled, isHelp } from 'store/hud';
  import { storeSettings } from 'store/settings'; // ← ДОБАВЬ ИМПОРТ
  import "./css/chat.css";

  export let SafeSone;

  // Actions / message types (kept from original)
  const ACTIONS = {
    default: { switchInTab: true },
    report: { command: "/report", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    b: { command: "/b", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    me: { command: "/me", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    do: { command: "/do", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    try: { command: "/try", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    s: { command: "/s", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    w: { command: "/w", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    m: { command: "/m", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    f: { command: "/f", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    fb: { command: "/fb", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    c: { command: "/c", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    cb: { command: "/cb", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    dep: { command: "/dep", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    mark: { command: "/mark", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    mark2: { command: "/mark2", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    gnews: { command: "/gnews", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    wnews: { command: "/wnews", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    adv: { command: "/adv", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    vmute: { command: "/vmute", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    pay: { command: "/pay", asPrefix: false, color: "#FFFFFF", backgroundColor: "#7F63BCB3" },
    global: { command: "", color: "#000", backgroundColor: "#FFC130" },
    gov: { command: "", backgroundColor: "#00BAFF" },
    org: { command: "", backgroundColor: "#4271F2" },
    fam: { command: "", backgroundColor: "#42A0F2" },
    adm: { command: "", backgroundColor: "#E81C5A" }
  };

  const MESSAGE_TYPES = {
    default: "default",
    me: "me",
    do: "do",
    try: "try",
    global: "global",
    gov: "gov",
    adv: "adv",
    dep: "dep",
    org: "org",
    fam: "fam",
    adm: "adm",
    nonRp: "b",
    confirmation: "confirmation"
  };
const defaultSettings = {
    chatTheme: "Clean",
    chatFont: "ProximaNova",
    chatFontStyle: "Bold",
    chatBorderType: "Stroke",
    chatTimestamp: "DateTime",
    chatFontSize: 14,
    chatFontShadow: 0,
    chatFontShadowAlpha: 0,
    chatLinesMargin: 0,
    chatLineHeight: 0,
  };
  // Chat tags
  let selectChat = 0;
  const ChatTegs = [
    { name: "IC", chat: false, visible: true, type: "default", color: "#FFFFFF", backgroundColor: "#4A5568" },
    { name: "OOC", chat: "b", visible: true, type: "b", color: ACTIONS.b.color, backgroundColor: ACTIONS.b.backgroundColor },
    { name: "ME", chat: "me", visible: true, type: "me", color: ACTIONS.me.color, backgroundColor: ACTIONS.me.backgroundColor },
    { name: "DO", chat: "do", visible: true, type: "do", color: ACTIONS.do.color, backgroundColor: ACTIONS.do.backgroundColor },
    { name: "TRY", chat: "try", visible: true, type: "try", color: ACTIONS.try.color, backgroundColor: ACTIONS.try.backgroundColor },
    { name: "REP", chat: "report", visible: true, type: "report", color: ACTIONS.report.color, backgroundColor: ACTIONS.report.backgroundColor }
  ];

  // ← ОБНОВЛЕННЫЕ НАСТРОЙКИ ИЗ STORE
   $: chatSettings = $storeSettings?.chatSettings || "standard";
  $: chatTheme = chatSettings === "standard" ? defaultSettings.chatTheme : ($storeSettings?.chatTheme || defaultSettings.chatTheme);
  $: fontFamily = getFontFamily(chatSettings === "standard" ? defaultSettings.chatFont : ($storeSettings?.chatFont || defaultSettings.chatFont));
  $: fontWeight = getFontWeight(chatSettings === "standard" ? defaultSettings.chatFontStyle : ($storeSettings?.chatFontStyle || defaultSettings.chatFontStyle));
  $: fontSize = chatSettings === "standard" ? defaultSettings.chatFontSize : Number($storeSettings?.chatFontSize || defaultSettings.chatFontSize);
  $: lineHeight = 1 + (chatSettings === "standard" ? defaultSettings.chatLineHeight : Number($storeSettings?.chatLineHeight || defaultSettings.chatLineHeight)) * 0.1;
  $: shadowType = (chatSettings === "standard" ? defaultSettings.chatBorderType : ($storeSettings?.chatBorderType || defaultSettings.chatBorderType)) === "Shadow" ? "shadow" : "stroke";
  $: shadowSize = chatSettings === "standard" ? defaultSettings.chatFontShadow : Number($storeSettings?.chatFontShadow || defaultSettings.chatFontShadow);
  $: shadowAlpha = (chatSettings === "standard" ? defaultSettings.chatFontShadowAlpha : Number($storeSettings?.chatFontShadowAlpha || defaultSettings.chatFontShadowAlpha)) / 100;
  $: messagesGap = chatSettings === "standard" ? defaultSettings.chatLinesMargin : Number($storeSettings?.chatLinesMargin || defaultSettings.chatLinesMargin);
  $: chatWidth = chatSettings === "standard" ? defaultSettings.chatWidth : Number($storeSettings?.chatWidth || defaultSettings.chatWidth); // ← ДОБАВИЛ
  $: chatHeight = chatSettings === "standard" ? defaultSettings.chatHeight : Number($storeSettings?.chatHeight || defaultSettings.chatHeight); // ← ДОБАВИЛ
  $: chatOpacity = $storeSettings?.ChatOpacity || 100;
  $: transition = $storeSettings?.Transition || 0.3;
  $: TimeStamp = (chatSettings === "standard" ? defaultSettings.chatTimestamp : ($storeSettings?.chatTimestamp || defaultSettings.chatTimestamp)) !== "No";
  $: isChatVisible = $storeSettings?.schat ?? true;
  $: isHudVisible = $storeSettings?.hud ?? true;


  // Helper functions for mapping values
  function getFontFamily(font) {
    const fontMap = {
      "ProximaNova": "'Proxima Nova', sans-serif",
      "ProximaNova-Condensed": "'Proxima Nova Condensed', sans-serif",
      "ProximaNova-ExtraCondensed": "'Proxima Nova ExtraCondensed', sans-serif",
      "San-Francisco": "-apple-system, BlinkMacSystemFont, 'San Francisco', sans-serif"
    };
    return fontMap[font] || "'Proxima Nova', sans-serif";
  }

  function getFontWeight(style) {
    const weightMap = {
      "Light": 300,
      "Regular": 400,
      "Semibold": 500,
      "Bold": 700
    };
    return weightMap[style] || 400;
  }

let TextInput;
  let Messages = [];
  let InputValue = "";
  let savedInput = "";
  let bufferState = -1;
  let bufferCurrent = "";
  let buffer = [];
  let pagesize = $storeSettings?.Pagesize || 10;
  let noActive = false;
  let timerId = 0;

  // derived lists
  $: actionsInTab = ChatTegs.map(t => (t.chat ? t.chat : 'default'));
  $: favoritesCmds = actionsInTab.slice();

  // DOM refs
  let chatElement;

  // derived
  $: currentChatConfig = ChatTegs[selectChat];
  $: messageType = selectChat === 0 ? "default" : ChatTegs[selectChat].chat;

  // safe HTML parsing (uses project's format("parse"))
  function safeParseHtml(str) {
    try {
      return format("parse", String(str ?? ""));
    } catch (e) {
      return String(str ?? "");
    }
  }

  // Color tag parser: !{#fff}...
  const parseMessage = (text) => {
    const re = new RegExp('!{((#[0-9a-f]{3})|(#[0-9a-f]{6})|([0-9a-f]{3})|([0-9a-f]{6})|(\\w+))}', 'i');
    const match = re.exec(text);
    if (!match) return null;
    let color = match[1];
    color = color ? (color.startsWith('#') ? color : (/(^[0-9A-Fa-f]{6}$)|(^[0-9A-Fa-f]{3}$)/i.test(color) ? '#' + color : color)) : null;
    return { color, index: match.index, remainingText: text.slice(match.index + match[0].length) };
  };

  const getColorizedMessagePart = (text) => {
    const parts = [];
    let prevColor = null;
    let currentText = String(text ?? "");
    let message;
    let safety = 0;
    while ((message = parseMessage(currentText)) && safety < 100) {
      const prevText = currentText.slice(0, message.index);
      if (prevText.length) {
        parts.push({ color: prevColor, text: safeParseHtml(prevText.replace(/\\!{/g, '!{').replace(/\\}/g, '}')) });
      }
      prevColor = message.color;
      currentText = message.remainingText;
      safety++;
    }
    if (currentText.length) {
      parts.push({ color: prevColor, text: safeParseHtml(currentText.replace(/\\!{/g, '!{').replace(/\\}/g, '}')) });
    }
    return parts.length ? parts : [{ color: null, text: safeParseHtml(text) }];
  };

  // autoscroll helper
  async function scrollToBottom() {
    await tick();
    if (chatElement) {
      try { chatElement.scrollTop = chatElement.scrollHeight; } catch (e) {}
    }
  }

  // Add message (stores messageType)
  function AddChatMessage(text, msgType = "default") {
    const generated = getColorizedMessagePart(text);
    const entry = { time: moment().format('HH:mm:ss'), parts: generated, messageType: msgType || "default" };
    Messages = [entry, ...Messages];
    if (Messages.length > 200) Messages.pop();
    onActive();
    scrollToBottom();
  }

  function onActive() {
    // use store value for input toggled
    if ($isInputToggled) return;
    noActive = false;
    if (timerId) clearTimeout(timerId);
    timerId = setTimeout(() => {
      timerId = 0;
      noActive = true;
    }, 15000);
  }

  // Toggle input — use store if possible, fallback to window.hudStore
  function ToggleChatInput(state, clearchat = false, updatelastbuffer = false, event = false) {
    if (state) {
      noActive = false;
      if (timerId) { clearTimeout(timerId); timerId = 0; }
      if (isInputToggled && typeof isInputToggled.set === 'function') isInputToggled.set(true);
      else if (window.hudStore && typeof window.hudStore.isInputToggled === 'function') window.hudStore.isInputToggled(true);
      setTimeout(() => {
        if (TextInput) {
          TextInput.focus();
          if (bufferState === -1) InputValue = savedInput;
          else InputValue = buffer[bufferState] !== undefined ? buffer[bufferState] : "";
          SetInputFocused();
          if (event) executeClient("client:OnChatInputChanged", true);
        }
      }, 0);
    } else {
      bufferCurrent = clearchat ? "" : (updatelastbuffer ? InputValue : bufferCurrent);
      bufferState = clearchat ? -1 : bufferState;
      savedInput = clearchat ? "" : InputValue;
      if (isInputToggled && typeof isInputToggled.set === 'function') isInputToggled.set(false);
      else if (window.hudStore && typeof window.hudStore.isInputToggled === 'function') window.hudStore.isInputToggled(false);
      onActive();
      InputValue = "";
      if (event) executeClient("client:OnChatInputChanged", false);
    }
  }

  function SetInputFocused() {
    if (TextInput) {
      TextInput.focus();
      try { TextInput.selectionStart = InputValue.length; } catch (e) {}
    }
  }

  function OnSubmitMessage() {
    bufferState = -1;
    bufferCurrent = "";
    if (InputValue && InputValue.trim().length) {
      if (buffer.length > 50) buffer.shift();
      if (buffer[buffer.length - 1] !== InputValue) buffer.push(InputValue);
      if (InputValue[0] === '/') {
        let commandText = InputValue.trim().substr(1);
        if (commandText.length > 0) {
          let params = commandText.split(' ');
          let command = params[0];
          params.shift();
          switch (command) {
            case "widthsize":
            case "pagesize":
            case "fontsize":
            case "timestamp":
            case "chatalpha":
              if (window.notificationAdd) window.notificationAdd(4, 9, translateText('player2', 'Данная функция была перенесена в настройки'), 3000);
              break;
            default:
              commandText = format("parseDell", commandText);
              if (commandText.length > 0) invokeMethod("command", commandText);
              break;
          }
        }
      } else {
        let message = format("stringify", InputValue);
        if (message.length > 0) {
          if (selectChat === 0) invokeMethod("chatMessage", message);
          else {
            const commandText = `${ChatTegs[selectChat].chat} ${message}`;
            if (commandText.length > 0) invokeMethod("command", commandText);
          }
        }
      }
    }
    ToggleChatInput(false, true, true, true);
  }

  function handleKeyUp(e) {
    if ($isInputToggled) {
      if (e.keyCode === 13) OnSubmitMessage();
    }
  }

  function handleKeyDown(event) {
    if ($isInputToggled) {
      switch (event.keyCode) {
        case 9: // TAB
          event.preventDefault();
          selectChat = (selectChat + 1) % ChatTegs.length;
          break;
        case 38: // UP
          event.preventDefault();
          if (bufferState === -1) {
            bufferState = buffer.length - 1;
            bufferCurrent = InputValue;
          } else if (bufferState > 0) {
            bufferState = bufferState - 1;
          }
          if (buffer[bufferState]) { InputValue = buffer[bufferState]; SetInputFocused(); }
          break;
        case 40: // DOWN
          event.preventDefault();
          if (bufferState === -1) break;
          if (bufferState < buffer.length - 1) {
            bufferState = bufferState + 1;
            InputValue = buffer[bufferState] || "";
            SetInputFocused();
          } else {
            InputValue = bufferCurrent;
            bufferState = -1;
            bufferCurrent = "";
            SetInputFocused();
          }
          break;
      }
    }
  }

  // actions on message parts (phone/geo/delete)
  function advEvent(type, id, number) {
    switch (type) {
      case "phone":
        invokeMethod("W2C:WnNotifyHud:Call", { number });
        break;
      case "message":
        invokeMethod("W2C:WnNotifyHud:Message", { number });
        break;
      case "geo":
        invokeMethod("W2C:WnNotifyHud:Geo", { id });
        break;
      case "delete":
        invokeMethod("W2C:WnNotifyHud:AdminDelete", { id });
        break;
    }
    ToggleChatInput(false, true, true, true);
  }

  // window.chat API (compat)
  window.chat = {
    updateConfig: (config) => {
            console.log("🔵 CHAT: updateConfig called with:", config); // ← ДОБАВЬ

      try {
        config = JSON.parse(config);
        // Поддержка старых названий
        TimeStamp = !!config["Timestamp"];
        pagesize = config["Pagesize"] ?? pagesize;
        chatOpacity = config["ChatOpacity"] ?? chatOpacity;
        transition = config["Transition"] ?? transition;
        fontSize = config["Fontsize"] ?? fontSize;
        chatWidth = config["Widthsize"] ?? chatWidth;

        if (config["ChatWidth"] !== undefined) chatWidth = config["ChatWidth"];
        if (config["ChatHeight"] !== undefined) chatHeight = config["ChatHeight"];
        if (config["ChatFontSize"] !== undefined) fontSize = config["ChatFontSize"];
                console.log("✅ Chat config updated:", { chatWidth, chatHeight, fontSize });

      } catch (e) {        console.error("❌ CHAT: updateConfig error:", e); // ← ДОБАВЬ
}
    },
    toggleInput: (toggled, clearchat = false, updatelastbuffer = false, event = false) => {
      ToggleChatInput(toggled, clearchat, updatelastbuffer, event);
    },
    addMessage: (message, _clearhtml = true) => {
      let msgType = "default";
      let messageText = String(message ?? "");
      for (const [actionType, actionConfig] of Object.entries(ACTIONS)) {
        if (actionConfig.command && messageText.startsWith(actionConfig.command + " ")) {
          msgType = actionType;
          messageText = messageText.substring(actionConfig.command.length + 1);
          break;
        }
      }
      AddChatMessage(messageText, msgType);
    }
  };
$: if (!isChatVisible) {
    // Скрыть чат
  }
  // mp integration
  function onChatPush(message) { AddChatMessage(message); }
  onMount(() => {
    if (window.mp && window.mp.events && typeof window.mp.events.add === 'function') window.mp.events.add("chat:push", onChatPush);
    scrollToBottom();
  });
  onDestroy(() => {
    if (window.mp && window.mp.events && typeof window.mp.events.remove === 'function') window.mp.events.remove("chat:push", onChatPush);
  });
</script>

<svelte:window on:keyup={handleKeyUp} on:keydown={handleKeyDown} />

<div class="chat-container" class:hidden={!isChatVisible}>
  <div class="chat-window column-block" class:disable-pointer-events={!$isInputToggled}>
    <div class="chat-main-container">
      <div class="chat-hide-container" class:hint-showed={false}>
        <div
          class="chat {chatTheme.toLowerCase()}"
          class:input-active={$isInputToggled}
          class:afk={noActive && !$isInputToggled}
          bind:this={chatElement}
          style="
            font-family: {fontFamily}; 
            font-size: {fontSize}px; 
            font-weight: {fontWeight};
            line-height: {lineHeight};
            filter: drop-shadow(0 0 {shadowSize}px rgba(0,0,0,{shadowAlpha}));
            opacity: {chatOpacity / 100};
            width: {chatWidth}px;
            height: {chatHeight}px;
          "
        >
          {#each Messages.slice().reverse() as message, index}
            <div class="chat__message {message.messageType || 'default'}" style="margin-bottom: {messagesGap}px;">
              {#if TimeStamp}
                <span class="chat__message-timestamp">[{message.time}] </span>
              {/if}

              {#each message.parts as part, i}
                {#if message.messageType === "try" && i === message.parts.length - 1}
                  <span class="try-result" style="background-color: {part.color || 'transparent'}; color: {part.color || 'inherit'};">
                    {@html part.text}
                  </span>
                {:else}
                  <span class="chat__message-part" style="color: {part.color || 'inherit'};">
                    {@html part.text}
                    {#if part.number || part.geo || part.isAdmin}
                      <span class="chat__part-buttons">
                        {#if part.number}
                          <button class="chat__btn" on:click={() => advEvent('phone', message.id, part.number)} title="Call">📞</button>
                          <button class="chat__btn" on:click={() => advEvent('message', message.id, part.number)} title="Message">💬</button>
                        {/if}
                        {#if part.geo}
                          <button class="chat__btn" on:click={() => advEvent('geo', message.id)} title="Geo">📍</button>
                        {/if}
                        {#if part.isAdmin}
                          <button class="chat__btn" on:click={() => advEvent('delete', message.id)} title="Delete">🗑️</button>
                        {/if}
                      </span>
                    {/if}
                  </span>
                {/if}
              {/each}
            </div>
          {/each}
        </div>
      </div>

      {#if $isInputToggled}
        <div class="shadow"></div>
        <div class="input-wrapper">
          <div class="input-container row-block align-center">
            {#if selectChat !== 0}
              <span
                class="input-container__label {messageType}"
                style="background-color: {ACTIONS[messageType]?.backgroundColor || ''}; color: {ACTIONS[messageType]?.color || ''}"
              >
                {currentChatConfig.name}
              </span>
            {/if}

            <input
              bind:this={TextInput}
              bind:value={InputValue}
              type="text"
              autocomplete="off"
              spellcheck={false}
              maxlength={144}
              placeholder=""
              class="input-container__input full-width"
            />
          </div>
        </div>
      {/if}
    </div>
  </div>
</div>