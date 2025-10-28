<script>
  import { createEventDispatcher, onDestroy } from "svelte";

  // Props
  export let modelValue = 0;
  export let min = 0;
  export let max = 100;
  export let interval = 1;
  export let disabled = false;
  export let tooltip = "active"; // none|always|focus|hover|active
  export let marks = false; // array|object|boolean
  export let process = true;
  export let direction = "ltr"; // ltr|rtl|ttb|btt
  export let lazy = false;
  export let width = undefined;
  export let height = undefined;

  const dispatch = createEventDispatcher();

  let container;
  let dragging = false;
  let pointerId = null;
  let focus = false;
  let internal = clamp(Number(modelValue ?? min), min, max);

  $: if (!isNaN(Number(modelValue)) && Number(modelValue) !== internal && !dragging) internal = clamp(Number(modelValue), min, max);
  $: percent = (internal - min) / (max - min) * 100 || 0;
  $: isHorizontal = direction === "ltr" || direction === "rtl";
  $: normalizedMarks = normalizeMarks(marks);

  function normalizeMarks(m) {
    if (!m) return [];
    if (Array.isArray(m)) return m.map(v => ({ value: v, label: String(v), pos: valueToPos(v) }));
    if (typeof m === "object") return Object.keys(m).sort((a,b)=>+a-+b).map(k => ({ value: Number(k), label: m[k], pos: valueToPos(Number(k)) }));
    return [];
  }

  function valueToPos(v) {
    return ((clamp(v, min, max) - min) / (max - min)) * 100;
  }

  function posToValue(p) {
    const raw = min + (p / 100) * (max - min);
    const snapped = Math.round((raw - min) / interval) * interval + min;
    return clamp(roundTo(snapped, interval), min, max);
  }

  function roundTo(v, step) {
    const prec = Math.max(0, (String(step).split(".")[1] || "").length);
    return Number(v.toFixed(prec));
  }

  function clamp(v, a, b) {
    if (isNaN(v)) return a;
    return Math.max(a, Math.min(b, v));
  }

  function getPosFromPointerEvent(evt) {
    if (!container) return 0;
    const rect = container.getBoundingClientRect();
    const clientX = evt.clientX;
    const clientY = evt.clientY;
    if (isHorizontal) {
      const x = clientX - rect.left;
      const p = (x / rect.width) * 100;
      return direction === "rtl" ? 100 - p : p;
    } else {
      const y = clientY - rect.top;
      const p = (y / rect.height) * 100;
      return direction === "btt" ? 100 - p : p;
    }
  }

  function setValueFromPos(p, emitImmediately = true) {
    const v = posToValue(clamp(p, 0, 100));
    internal = v;
    dispatch("update:modelValue", internal);
    if (emitImmediately && !lazy) {
      dispatch("change", internal);
    }
  }

  // Pointer events handlers (pointerdown/pointermove/pointerup)
  function onPointerDownHandle(evt) {
    if (disabled) return;
    // Only handle left button or touch
    if (evt.pointerType === 'mouse' && evt.button !== 0) return;
    evt.preventDefault();
    pointerId = evt.pointerId;
    const target = evt.target;
    dragging = true;
    try { evt.target.setPointerCapture && evt.target.setPointerCapture(pointerId); } catch(e) {}
    const p = getPosFromPointerEvent(evt);
    setValueFromPos(p, true);
    dispatch("drag-start", internal);
    console.debug("[slider] pointerdown", { pointerId, clientX: evt.clientX, clientY: evt.clientY, value: internal });
  }

  function onPointerMoveGlobal(evt) {
    if (!dragging || evt.pointerId !== pointerId) return;
    evt.preventDefault();
    const p = getPosFromPointerEvent(evt);
    setValueFromPos(p, true);
    if (!lazy) {
      dispatch("input", internal);
      dispatch("update:modelValue", internal);
    }
    dispatch("dragging", internal);
    console.debug("[slider] pointermove", { clientX: evt.clientX, clientY: evt.clientY, value: internal });
  }

  function onPointerUpGlobal(evt) {
    if (!dragging || evt.pointerId !== pointerId) return;
    dragging = false;
    try { evt.target.releasePointerCapture && evt.target.releasePointerCapture(pointerId); } catch(e) {}
    pointerId = null;
    dispatch("input", internal);
    dispatch("update:modelValue", internal);
    dispatch("drag-end", internal);
    dispatch("change", internal);
    console.debug("[slider] pointerup", { clientX: evt.clientX, clientY: evt.clientY, value: internal });
  }

  // rail pointerdown: jump+start drag
  function onRailDown(evt) {
    if (disabled) return;
    if (evt.pointerType === 'mouse' && evt.button !== 0) return;
    evt.preventDefault();
    // we want container to capture pointer moves
    pointerId = evt.pointerId;
    try { container.setPointerCapture && container.setPointerCapture(pointerId); } catch(e) {}
    dragging = true;
    const p = getPosFromPointerEvent(evt);
    setValueFromPos(p, true);
    dispatch("input", internal);
    console.debug("[slider] rail pointerdown", { pointerId, clientX: evt.clientX, clientY: evt.clientY, value: internal });
  }

  function onRailClick(evt) {
    if (disabled) return;
    const p = getPosFromPointerEvent(evt);
    setValueFromPos(p, true);
    dispatch("input", internal);
    dispatch("change", internal);
    console.debug("[slider] rail click", { clientX: evt.clientX, clientY: evt.clientY, value: internal });
  }

  function onKeyDown(e) {
    if (disabled) return;
    const k = e.key;
    let handled = false;
    const step = interval || 1;
    if (k === "ArrowLeft" || k === "ArrowDown") { internal = clamp(roundTo(internal - step, interval), min, max); handled = true; }
    else if (k === "ArrowRight" || k === "ArrowUp") { internal = clamp(roundTo(internal + step, interval), min, max); handled = true; }
    else if (k === "Home") { internal = min; handled = true; }
    else if (k === "End") { internal = max; handled = true; }
    else if (k === "PageUp") { internal = clamp(roundTo(internal + step * 10, interval), min, max); handled = true; }
    else if (k === "PageDown") { internal = clamp(roundTo(internal - step * 10, interval), min, max); handled = true; }
    if (handled) {
      e.preventDefault();
      dispatch("input", internal);
      dispatch("update:modelValue", internal);
      dispatch("change", internal);
      console.debug("[slider] keydown", { key: k, value: internal });
    }
  }

  function handleMarkClick(mark) {
    if (disabled) return;
    setValueFromPos(mark.pos, true);
    dispatch("input", internal);
    dispatch("change", internal);
    console.debug("[slider] mark click", { mark, value: internal });
  }

  // global pointer listeners to capture moves/releases outside component
  window.addEventListener('pointermove', onPointerMoveGlobal, { passive: false });
  window.addEventListener('pointerup', onPointerUpGlobal);

  onDestroy(() => {
    window.removeEventListener('pointermove', onPointerMoveGlobal);
    window.removeEventListener('pointerup', onPointerUpGlobal);
  });

  $: if (Number(modelValue) !== internal && !dragging) {
    internal = clamp(Number(modelValue ?? internal), min, max);
  }

  function railBackground() {
    const fill = "#e81c5a";
    const rail = "#292929";
    const p = clamp(percent, 0, 100);
    return isHorizontal
      ? `linear-gradient(90deg, ${fill} 0%, ${fill} ${p}%, ${rail} ${p}%, ${rail} 100%)`
      : `linear-gradient(180deg, ${fill} 0%, ${fill} ${p}%, ${rail} ${p}%, ${rail} 100%)`;
  }

  function valueToPosStyle(v) {
    if (isHorizontal) {
      const pos = direction === "rtl" ? 100 - percent : percent;
      return `left:${pos}%; transform: translateX(-50%) translateY(-50%);`;
    } else {
      const pos = direction === "btt" ? 100 - percent : percent;
      return `bottom:${pos}%; transform: translateX(-50%) translateY(50%);`;
    }
  }
</script>

<div class="range" style="width: {width ?? '100%'}; height: {height ?? 'auto'}; opacity: {disabled ? 0.5 : 1};">
  <div class="sliderInput">
    <div class="vue-slider" bind:this={container}
         style="position:relative;display:flex;align-items:center;height:100%; touch-action: none;"
         aria-disabled={disabled}
    >
      <!-- rail: use pointerdown on rail (container captures pointermove/up) -->
      <div class="vue-slider-rail"
           on:pointerdown|stopPropagation={onRailDown}
           on:click|stopPropagation={onRailClick}
           style="flex:1;height:100%;background: {railBackground()}; border-radius: .2777777778vh; position:relative; z-index:0;">
      </div>

      {#if process}
        <div class="vue-slider-process"
             style={isHorizontal ? `position:absolute; left:0; top:0; bottom:0; width:${percent}%; z-index:1; pointer-events:none; background:#e81c5a; border-radius:.2777777778vh;` : `position:absolute; left:0; right:0; bottom:0; height:${percent}%; z-index:1; pointer-events:none; background:#e81c5a; border-radius:.2777777778vh;`}>
        </div>
      {/if}

      <!-- dot: attach pointerdown here so user can start drag from handle -->
      <div class="vue-slider-dot"
           tabindex={disabled ? -1 : 0}
           role="slider"
           aria-valuemin={min}
           aria-valuemax={max}
           aria-valuenow={internal}
           on:keydown={onKeyDown}
           on:focus={() => (focus = true)}
           on:blur={() => (focus = false)}
           on:pointerdown|stopPropagation={onPointerDownHandle}
           style={`position:absolute; top:50%; ${valueToPosStyle(internal)}; z-index:10; pointer-events:auto;`}
      >
        <div class="vue-slider-dot-handle" style="width:1.4814814815vh;height:1.4814814815vh;border-radius:50%;background:#fff;"></div>

        {#if tooltip !== 'none' && (tooltip === 'always' || (tooltip === 'active' && (dragging || focus)) || (tooltip === 'focus' && focus))}
          <div class="vue-slider-dot-tooltip vue-slider-dot-tooltip-top" style="position:absolute;left:50%;transform:translate(-50%,-120%);top:-1.2vh;z-index:11;">
            <div class="vue-slider-dot-tooltip-inner" style="background:rgba(0,0,0,0.7);padding:.4vh .8vh;border-radius:.37vh;color:#fff;font-weight:700;font-size:1.1vh;">
              <span class="vue-slider-dot-tooltip-text">{internal}</span>
            </div>
          </div>
        {/if}
      </div>

      <!-- marks -->
      {#if normalizedMarks.length}
        <div class="vue-slider-marks" style="position:absolute;left:0;right:0;top:0;bottom:0;pointer-events:none;z-index:2;">
          {#each normalizedMarks as mark (mark.pos)}
            <div class="vue-slider-mark" style={isHorizontal ? `position:absolute;left:${mark.pos}%;top:100%;transform:translateX(-50%);pointer-events:auto;` : `position:absolute;bottom:${mark.pos}%;left:50%;transform:translateX(-50%);pointer-events:auto;`}>
              <div class="vue-slider-mark-step" on:click|stopPropagation={() => handleMarkClick(mark)} style="width:.4vh;height:.4vh;background:rgba(255,255,255,0.08);border-radius:50%;margin:0 auto .4vh;"></div>
              <div class="vue-slider-mark-label" on:click|stopPropagation={() => handleMarkClick(mark)} style="color:rgba(255,255,255,0.7);font-size:1.1vh;cursor:pointer;">{mark.label}</div>
            </div>
          {/each}
        </div>
      {/if}
    </div>
  </div>

  <div class="value">
    <input class="input" type="number" min={min} max={max} step={interval}
           bind:value={internal}
           {disabled}
           on:input={() => {
             internal = clamp(Number(internal), min, max);
             dispatch("input", internal);
             dispatch("update:modelValue", internal);
             dispatch("change", internal);
           }}
           aria-label="value" />
  </div>
</div>

<style>
  /* minimal local adjustments — main visuals come from your CSS */
  .vue-slider { position: relative; }
  .vue-slider-process { pointer-events: none; }
  .vue-slider-dot { pointer-events: auto; z-index: 10; }
</style>