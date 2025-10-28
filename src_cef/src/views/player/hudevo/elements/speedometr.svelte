<script>
  import { onMount, onDestroy } from 'svelte';
  import SVGComponent from './SVGComponent.svelte';
  import { fly } from 'svelte/transition';
  import { inVehicle, isInputToggled } from 'store/hud';
  import keys from 'store/keys';
  import keysName from 'json/keys.js';
  import './assets/css/index2.css';
  import { storeSettings } from 'store/settings'; // ← ДОБАВЬ ИМПОРТ

  // Props для настройки
  export let cdn = 'https://cdn.majestic-files.com/public/master/static/';
  
  // Состояние спидометра
  let isActive = false;
  let speed = 0;
  let rpm = 0;
  let targetRpm = 0; // ← ДОБАВЬ: целевое значение RPM
  let smoothRpm = 0; // ← ДОБАВЬ: плавное значение RPM
  let gear = 'N';
  let maxSpeed = 200;
  let currentFuel = 50;
  let maxFuel = 100;
  let fuelType = 'Petrol';
  let oilTemp = 90;
  let mileage = 0;
  let indicatorLeft = false;
  let indicatorRight = false;
  let handbrake = false;
  let seatbelt = true;
  let lock = false;
  let doorOpened = false;
  let dippedBeam = false;
  let highBeam = false;
   let speedoType = 'Minimalistic_2';
  $: speedoType = $storeSettings?.speedoType ?? 'Minimalistic_2';
  let vehicleRpm = 0;
  let isFlying = false;
  let isBoat = false;
  
  // Дополнительные переменные
  let autoPilot = false;
  let cruiseControl = false;
  let engine = false;
  let isToggledVehicleHud = true;
  
  // Constants
  const SPEEDO_TYPES = {
    Advanced_1: 'Advanced_1',
    Advanced_2: 'Advanced_2',
    Minimalistic_1: 'Minimalistic_1',
    Minimalistic_2: 'Minimalistic_2',
    Minimalistic_3: 'Minimalistic_3',
    Disabled: 'Disabled'
  };
  
  // State для Canvas
  let speedometerContainer;
  let speedometerCanvas;
  let ctx = null;
  let canvasInited = false;
  let currentProgress = 0;
  let raf = null;
  let oldGear = 'N';
  
  // Canvas settings
  let speedLineWidth = 0.015 * window.innerHeight;
  let speedRadius = -1;
  let centerX = -1;
  let centerY = -1;
  let speedStartAngle = toRadians(-220);
  let speedEndAngle = toRadians(40);
  let speedBarOffset = toRadians(90);
  let innerLineWidth = 0.0031 * window.innerHeight;
  let speedBarWidth = -1;
  let speedBarHeight = -1;
  let innerRadius = -1;
  let speedFrames = []; // ← ДОБАВЬ
let delay = 0; // ← ДОБАВЬ
let lastUpdateTime = Date.now(); // ← ДОБАВЬ
  // Left/Right indicators
  let leftStartAngle = toRadians(-220);
  let leftEndAngle = toRadians(-150);
  let rightStartAngle = toRadians(40);
  let rightEndAngle = toRadians(-30);
  
  // Second speedo settings
  const SECOND_SPEEDO = {
    transitionName: 'slideUp',
    outerRadius: -1,
    speedRadius: -1,
    speedBarRadius: -1,
    speedBarHeight: -1,
    speedBarWidth: -1,
    speedLineWidth: -1,
    speedLinesHeightBig: -1,
    speedLinesHeightSmall: -1,
    speedLinesWidth: -1,
    bigLine: {
      startAngle: toRadians(-210),
      endAngle: toRadians(-3)
    },
    smallLine: {
      startAngle: toRadians(0),
      endAngle: toRadians(30)
    },
    fuel: {
      startAngle: toRadians(-3),
      endAngle: toRadians(-33)
    },
    oilTemp: {
      startAngle: toRadians(30),
      endAngle: toRadians(0)
    }
  };
  
  // Computed
  $: speedRounded = Math.floor(speed);
  // ✅ ИСПРАВЛЕНО: Прямой reactive statement
$: gearComputed = (() => {
  console.log('🔧 [gearComputed] gear:', gear, 'speed:', speed);
  
  // ✅ Если стоим (скорость = 0) → показываем N
  if (Math.floor(speed) === 0) return 'N';
  
  // ✅ Если передача = 0 → это задняя (R)
  if (gear === 0 || gear === '0') return 'R';
  
  // ✅ Если передача пустая → нейтралка (N)
  if (!gear || gear === 'N') return 'N';
  
  // ✅ Иначе показываем передачу (1, 2, 3, ...)
  return String(gear);
})();
  $: isElectro = fuelType.toLowerCase() === 'electro';
  $: isFuelLow = currentFuel / maxFuel <= 0.1;
  $: isFuelRenderAllowed = speedoType === SPEEDO_TYPES.Advanced_1 || fuelType.toLowerCase() !== 'mechanical';
  $: isOilRenderAllowed = speedoType === SPEEDO_TYPES.Advanced_1 || fuelType.toLowerCase() !== 'mechanical';
  $: isGearRenderAllowed = checkGearRenderAllowed();
 let mileageComputed = [];  
  $: getMaximumFuelChars = isFuelRenderAllowed ? (isElectro ? 4 : `${maxFuel}/${maxFuel}l`.length) : 3;
  $: isActive = $inVehicle && isToggledVehicleHud;
  
  // Functions
  function toRadians(degrees) {
    return degrees * (Math.PI / 180);
  }
  
  function lerp(start, end, t) {
    return start * (1 - t) + end * t;
  }
  
  function clamp(value, min, max) {
    return Math.min(Math.max(value, min), max);
  }
  
  function rbToVh(value) {
    return (value / 1080) * window.innerHeight;
  }
  
function getGearComputed() {
  console.log('🔧 [getGearComputed] gear:', gear, 'type:', typeof gear, 'speed:', speed);
  
  if (gear === 0 || gear === '0') {
    console.log('🔧 [getGearComputed] RESULT: R');
    return 'R';
  }
  
  if (!gear || gear === 'N') {
    console.log('🔧 [getGearComputed] RESULT: N (empty gear)');
    return 'N';
  }
  
  const result = String(gear);
  console.log('🔧 [getGearComputed] RESULT:', result);
  return result;
}
  
function checkGearRenderAllowed() {
  if (speedoType === SPEEDO_TYPES.Advanced_1) {
    return true;
  }
  
  const result = !isFlying && fuelType.toLowerCase() !== 'mechanical';
  
  return result;
}
  
  function getMileageComputed() {
  const result = [];
  const mileageStr = Math.floor(mileage).toString().padStart(5, '0').slice(-5); // ← FLOOR вместо ROUND
  let isGrey = true;
  
  
  for (const char of mileageStr) {
    if (isGrey && char !== '0') isGrey = false;
    result.push({ isGrey, char });
  }
  
  return result;
}
  function getSpeedColor(speed) {
    if (speed < 120) return 'white';
    if (speed >= 100 && speed <= 160) return '#FBC900';
    return '#FF3131';
  }
  
  function initCanvas() {
    if (!speedometerContainer || !speedometerCanvas) return;
    
    const rect = speedometerContainer.getBoundingClientRect();
    if (rect.width <= 0 || rect.height <= 0) return;
    
    let width = rect.width;
    let height = rect.height;
    
    if (speedoType === SPEEDO_TYPES.Advanced_2) {
      width /= 0.7;
      height /= 0.7;
    }
    
    speedometerCanvas.width = width;
    speedometerCanvas.height = height;
    
    centerX = width * 0.5;
    centerY = height * 0.5;
    speedRadius = width * 0.5 - speedLineWidth * 0.5;
    innerRadius = width * 0.4 - innerLineWidth * 0.5;
    speedBarWidth = 0.0037 * window.innerHeight;
    speedBarHeight = 0.02 * window.innerHeight;
    
    // Second speedo settings
    SECOND_SPEEDO.outerRadius = rbToVh(216.5);
    SECOND_SPEEDO.speedRadius = rbToVh(165);
    SECOND_SPEEDO.speedBarRadius = SECOND_SPEEDO.speedRadius - rbToVh(58);
    SECOND_SPEEDO.speedBarWidth = rbToVh(5);
    SECOND_SPEEDO.speedBarHeight = rbToVh(56);
    SECOND_SPEEDO.speedLineWidth = rbToVh(5);
    SECOND_SPEEDO.speedLinesWidth = rbToVh(1);
    SECOND_SPEEDO.speedLinesHeightBig = rbToVh(15);
    SECOND_SPEEDO.speedLinesHeightSmall = rbToVh(8);
    
    ctx = speedometerCanvas.getContext('2d');
    canvasInited = true;
  }
  
  function drawSpeedometer() {
    if (!ctx || !canvasInited) return;
    
    ctx.clearRect(0, 0, speedometerCanvas.width, speedometerCanvas.height);
    
    if (speedoType === SPEEDO_TYPES.Advanced_1) {
      drawAdvanced1();
    } else if (speedoType === SPEEDO_TYPES.Advanced_2) {
      drawAdvanced2();
    }
  }
  
  function drawAdvanced1() {
    // Main speed arc
    ctx.beginPath();
    const endAngle = lerp(speedStartAngle, speedEndAngle, currentProgress);
    ctx.strokeStyle = 'white';
    ctx.lineWidth = speedLineWidth;
    ctx.arc(centerX, centerY, speedRadius, speedStartAngle, endAngle, false);
    ctx.stroke();
    
    // Oil temp indicator
    if (isOilRenderAllowed) {
      const oilProgress = oilTemp / 180;
      ctx.beginPath();
      ctx.strokeStyle = '#E51A22';
      ctx.lineWidth = innerLineWidth;
      ctx.arc(centerX, centerY, innerRadius, leftStartAngle, lerp(leftStartAngle, leftEndAngle, oilProgress), false);
      ctx.stroke();
    }
    
    // Fuel indicator
    if (isFuelRenderAllowed) {
      const fuelProgress = currentFuel / maxFuel;
      ctx.beginPath();
      ctx.strokeStyle = '#FFB300';
      ctx.lineWidth = innerLineWidth;
      ctx.arc(centerX, centerY, innerRadius, rightStartAngle, lerp(rightStartAngle, rightEndAngle, fuelProgress), true);
      ctx.stroke();
    }
    
    // Speed bar
    ctx.save();
    ctx.translate(centerX, centerY);
    ctx.rotate(endAngle - speedBarOffset);
    ctx.translate(0, speedRadius);
    ctx.fillStyle = 'red';
    ctx.fillRect(-speedBarWidth * 0.5, speedLineWidth * 0.5, speedBarWidth, -speedBarHeight);
    ctx.restore();
  }
  
  function drawAdvanced2() {
  const oilProgress = oilTemp / 180;
  const fuelProgress = currentFuel / maxFuel;
  
  
  // Background arcs
  ctx.beginPath();
  ctx.strokeStyle = 'rgba(255, 255, 255, 0.5)';
  ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
  ctx.arc(centerX, centerY, SECOND_SPEEDO.speedRadius, SECOND_SPEEDO.bigLine.startAngle, SECOND_SPEEDO.bigLine.endAngle, false);
  ctx.stroke();
  
  ctx.beginPath();
  ctx.strokeStyle = 'rgba(223, 0, 91, 0.5)';
  ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
  ctx.arc(centerX, centerY, SECOND_SPEEDO.speedRadius, SECOND_SPEEDO.smallLine.startAngle, SECOND_SPEEDO.smallLine.endAngle, false);
  ctx.stroke();
  
  // Progress arcs
  const progressAngle = lerp(SECOND_SPEEDO.bigLine.startAngle, SECOND_SPEEDO.smallLine.endAngle, currentProgress);
  const whiteProgress = clamp(progressAngle, SECOND_SPEEDO.bigLine.startAngle, SECOND_SPEEDO.bigLine.endAngle);
  const redProgress = clamp(progressAngle, SECOND_SPEEDO.smallLine.startAngle, SECOND_SPEEDO.smallLine.endAngle);
  

  
  ctx.beginPath();
  ctx.strokeStyle = 'rgba(255, 255, 255, 1)';
  ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
  ctx.arc(centerX, centerY, SECOND_SPEEDO.speedRadius, SECOND_SPEEDO.bigLine.startAngle, whiteProgress, false);
  ctx.stroke();
  
  ctx.beginPath();
  ctx.strokeStyle = 'rgba(223, 0, 91, 1)';
  ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
  ctx.arc(centerX, centerY, SECOND_SPEEDO.speedRadius, SECOND_SPEEDO.smallLine.startAngle, redProgress, false);
  ctx.stroke();
  
  // Oil temp outer arc
  if (isOilRenderAllowed) {
    ctx.beginPath();
    ctx.strokeStyle = 'rgba(255, 34, 34, 0.2)';
    ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
    ctx.arc(centerX, centerY, SECOND_SPEEDO.outerRadius, SECOND_SPEEDO.oilTemp.startAngle, SECOND_SPEEDO.oilTemp.endAngle, true);
    ctx.stroke();
    
    ctx.beginPath();
    ctx.strokeStyle = 'rgba(255, 34, 34, 1)';
    ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
    ctx.arc(centerX, centerY, SECOND_SPEEDO.outerRadius, SECOND_SPEEDO.oilTemp.startAngle, lerp(SECOND_SPEEDO.oilTemp.startAngle, SECOND_SPEEDO.oilTemp.endAngle, oilProgress), true);
    ctx.stroke();
  }
  
  // Fuel outer arc
  if (isFuelRenderAllowed) {
    const fuelColor = isElectro ? '20, 166, 249' : '255, 181, 62';
    
    ctx.beginPath();
    ctx.strokeStyle = `rgba(${fuelColor}, 0.2)`;
    ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
    ctx.arc(centerX, centerY, SECOND_SPEEDO.outerRadius, SECOND_SPEEDO.fuel.startAngle, SECOND_SPEEDO.fuel.endAngle, true);
    ctx.stroke();
    
    ctx.beginPath();
    ctx.strokeStyle = `rgba(${fuelColor}, 1)`;
    ctx.lineWidth = SECOND_SPEEDO.speedLineWidth;
    ctx.arc(centerX, centerY, SECOND_SPEEDO.outerRadius, SECOND_SPEEDO.fuel.startAngle, lerp(SECOND_SPEEDO.fuel.startAngle, SECOND_SPEEDO.fuel.endAngle, fuelProgress), true);
    ctx.stroke();
  }
  
  // Draw speed lines
  drawSpeedLines(oilProgress, fuelProgress); // ← ИЗМЕНЕНО: передаём параметры
  
  // Draw speed bar
  const barAngle = clamp(progressAngle - toRadians(0.8), SECOND_SPEEDO.bigLine.startAngle + toRadians(0.75), SECOND_SPEEDO.smallLine.endAngle - toRadians(0.9));
  drawLine({
    height: SECOND_SPEEDO.speedBarHeight,
    color: barAngle >= toRadians(-3) ? 'speedBarGradientRed' : 'speedBarGradient',
    radius: SECOND_SPEEDO.speedBarRadius,
    angle: barAngle,
    width: SECOND_SPEEDO.speedBarWidth
  });
}
  
 function drawSpeedLines(oilProgress, fuelProgress) { // ← ДОБАВЬ ПАРАМЕТРЫ
  const angleOffsets = { 0: toRadians(0.15), 7: toRadians(0.2), 8: toRadians(-0.3) };
  
  for (let i = 0; i < 9; i++) {
    const angle = SECOND_SPEEDO.bigLine.startAngle + toRadians(30 * i) + (angleOffsets[i] || 0);
    let color = 'rgba(255, 255, 255, 0.5)';
    if (i >= 7) color = 'rgba(223, 0, 91, 0.5)';
    
    let lineColor = color;
    const normalizedProgress = (smoothRpm * 8); // 0-8 диапазон
    if (i <= normalizedProgress) {
      const opacity = Math.min(1, (normalizedProgress - i) + 0.5); // Плавная прозрачность
      lineColor = color.replace('0.5', opacity.toFixed(2));
    }
    
    drawLine({
      radius: SECOND_SPEEDO.speedRadius + SECOND_SPEEDO.speedLineWidth * 0.5,
      angle,
      height: SECOND_SPEEDO.speedLinesHeightBig,
      color: lineColor
    });
    
    if (i < 8) {
      for (let j = 0; j < 6; j++) {
        let offset = 0;
        if (i === 6) offset = toRadians(0.37 * j);
        
        const smallAngle = angle + toRadians(5 * j) + offset;
        
        drawLine({
          radius: SECOND_SPEEDO.speedRadius + SECOND_SPEEDO.speedLineWidth * 0.5,
          angle: smallAngle,
          height: SECOND_SPEEDO.speedLinesHeightSmall,
          color
        });
      }
    }
  }
  
  if (isOilRenderAllowed) {
    drawLine({
      radius: SECOND_SPEEDO.outerRadius + SECOND_SPEEDO.speedLineWidth * 0.5,
      angle: SECOND_SPEEDO.oilTemp.startAngle - toRadians(0.175),
      height: SECOND_SPEEDO.speedLinesHeightBig,
      color: `rgba(255, 34, 34, ${oilProgress < 0.01 ? '0.2' : '1'})` // ← ИСПОЛЬЗУЕМ ПАРАМЕТР
    });
  }
  
  if (isFuelRenderAllowed) {
    const fuelColor = isElectro ? '20, 166, 249' : '255, 181, 62';
    drawLine({
      radius: SECOND_SPEEDO.outerRadius + SECOND_SPEEDO.speedLineWidth * 0.5,
      angle: SECOND_SPEEDO.fuel.startAngle - toRadians(0.125),
      height: SECOND_SPEEDO.speedLinesHeightBig,
      color: `rgba(${fuelColor}, ${fuelProgress < 0.01 ? '0.2' : '1'})` // ← ИСПОЛЬЗУЕМ ПАРАМЕТР (ИСПРАВЛЕНО)
    });
  }
}
  
  function drawLine({ centerX: cx = centerX, centerY: cy = centerY, radius = 25, angle = 0, height = 15, width = 1, color = 'white', isRad = true } = {}) {
    const rad = isRad ? angle : toRadians(angle);
    const cos = Math.cos(rad);
    const sin = Math.sin(rad);
    const outerRadius = radius + height;
    
    const startX = cx + radius * cos;
    const startY = cy + radius * sin;
    const endX = cx + outerRadius * cos;
    const endY = cy + outerRadius * sin;
    
    let strokeStyle = color;
    
    if ((color === 'speedBarGradient' || color === 'speedBarGradientRed') && !isNaN(startX) && !isNaN(startY) && !isNaN(endX) && !isNaN(endY)) {
      const gradient = ctx.createLinearGradient(startX, startY, endX, endY);
      gradient.addColorStop(0, 'transparent');
      gradient.addColorStop(1, color === 'speedBarGradientRed' ? '#DF005B' : '#ffffff');
      strokeStyle = gradient;
    }
    
    ctx.beginPath();
    ctx.lineWidth = width;
    ctx.strokeStyle = strokeStyle;
    ctx.moveTo(startX, startY);
    ctx.lineTo(endX, endY);
    ctx.stroke();
  }
  
  function animationFrame() {
  if (!isActive) {
    if (raf) {
      cancelAnimationFrame(raf);
      raf = null;
    }
    canvasInited = false;
    return;
  }
  
  if ([SPEEDO_TYPES.Advanced_1, SPEEDO_TYPES.Advanced_2].includes(speedoType)) {
    if (!canvasInited) {
      initCanvas();
    }
    
    if (speedoType === SPEEDO_TYPES.Advanced_1 || isFlying) {
      // ✅ Плавная интерполяция для Advanced_1
      const targetProgress = Math.min(Math.max(speed / maxSpeed, 0), 1);
      const lerpSpeed = 0.15; // Скорость сглаживания
      currentProgress = lerp(currentProgress, targetProgress, lerpSpeed);
      
    } else if (speedoType === SPEEDO_TYPES.Advanced_2) {
      // ✅ Плавная интерполяция для Advanced_2
      const lerpSpeed = 0.2; // Скорость сглаживания (можно менять: 0.1-0.3)
      smoothRpm = lerp(smoothRpm, targetRpm, lerpSpeed);
      
      currentProgress = clamp(smoothRpm, 0, 1);
      
    }
    
    if (canvasInited) {
      drawSpeedometer();
    }
  }
  
  raf = requestAnimationFrame(animationFrame);
}
  
  // Window API - подключение данных из старого файла
  window.vehicleState = window.vehicleState || {};
  
  window.vehicleState.gear = (value) => {
    gear = value;
  };
  window.vehicleState.speedFrames = (frames) => {

  speedFrames = frames;
};

window.vehicleState.delay = (value) => {

  delay = value;
};
  window.vehicleState.speed = (value) => {
    speed = value;
  };
  
  window.vehicleState.maxSpeed = (value) => {
    maxSpeed = value * 1.15;
  };
  
  window.vehicleState.fuel = (value) => {
    currentFuel = value;
  };
  
  window.vehicleState.rpm = (value) => {
  targetRpm = value; // ← ИЗМЕНЕНО: сохраняем целевое значение
  rpm = value; // оставляем для совместимости
};
    window.vehicleState = window.vehicleState || {};

  window.vehicleState.autoPilot = (value) => autoPilot = value;
  window.vehicleState.belt = (value) => seatbelt = value;
  window.vehicleState.cruiseControl = (value) => cruiseControl = value;
  window.vehicleState.rightIL = (value) => indicatorRight = value;
  window.vehicleState.leftIL = (value) => indicatorLeft = value;
  window.vehicleState.doors = (value) => doorOpened = value;
  window.vehicleState.engine = (value) => lock = !value;
  window.vehicleState.isToggledVehicleHud = (value) => isToggledVehicleHud = value;
  window.vehicleState.handbrake = (value) => handbrake = value;
  window.vehicleState.lights = (value) => {
    dippedBeam = value === 1 || value === 2;
    highBeam = value === 2;
  };
  window.vehicleState.mileage = (value) => {
    mileage = Number(value) / 100;

      mileageComputed = getMileageComputed();

  };
  window.vehicleState.oilTemp = (value) => oilTemp = value;
  window.vehicleState.fuelType = (value) => fuelType = value;
  window.vehicleState.maxFuel = (value) => maxFuel = value;

  window.vehicleState.isFlying = (value) => isFlying = value;
  window.vehicleState.isBoat = (value) => isBoat = value;
    window.vehicleState.isToggledVehicleHud = (value) => isToggledVehicleHud = value;
window.vehicleState.speedoType = (value) => {
    console.log('⚠️ speedoType called from old system:', value, '(ignored, using storeSettings)');
    // Не обновляем, т.к. теперь читаем из storeSettings
  };
  // Lifecycle
  $: {
  console.log('🔧 [Advanced_2 Check] speedoType:', speedoType);
  console.log('🔧 [Advanced_2 Check] isGearRenderAllowed:', isGearRenderAllowed);
  console.log('🔧 [Advanced_2 Check] gear:', gear);
  console.log('🔧 [Advanced_2 Check] gearComputed:', gearComputed);
  console.log('🔧 [Advanced_2 Check] speed:', speed);
  console.log('🔧 [Advanced_2 Check] isFlying:', isFlying);
  console.log('🔧 [Advanced_2 Check] fuelType:', fuelType);
}
  onMount(() => {
    if (isActive) {
      raf = requestAnimationFrame(animationFrame);
    }
  });
  
  onDestroy(() => {
    if (raf) {
      cancelAnimationFrame(raf);
      raf = null;
    }
  });
  
  // Watchers
  $: if (isActive && !raf) {
    raf = requestAnimationFrame(animationFrame);
  } else if (!isActive && raf) {
    cancelAnimationFrame(raf);
    raf = null;
    canvasInited = false;
  }
  
  $: if (speedoType) {
    canvasInited = false;
  }
  
  $: if (gearComputed !== oldGear) {
    const toNum = (g) => g === 'N' ? 0 : g === 'R' ? -1 : parseInt(g);
    SECOND_SPEEDO.transitionName = toNum(gearComputed) > toNum(oldGear) ? 'slideUp' : 'slideDown';
    oldGear = gearComputed;
  }
</script>

{#if isActive}
  <div class="timebars-with-speedo" in:fly={{ x: 50, duration: 500 }} out:fly={{ x: 50, duration: 250 }}>
    <div class="timebars"></div>
    <div class="carhud justify-end carhud-{speedoType}">
      {#if ['Advanced_2', 'Minimalistic_2', 'Minimalistic_3'].includes(speedoType)}
        <div class="carhud-radial-shadow"></div>
      {/if}
      
      {#if speedoType === SPEEDO_TYPES.Advanced_1}
        <div class="carhud-container">
          <div class="carhud-container-content">
            <div class="carhud-container-content__picture-attention" class:active={handbrake} class:no-gear={!isGearRenderAllowed}>
              <SVGComponent path="icons/main/hud/speedometer_1/designations/handbrake.svg" />
            </div>
            
            <div class="velocities">
              <div class="velocities-arrow" class:active={indicatorLeft}>
                <div class="velocities__picture-arrowLeft velocities-arrow__picture">
                  <SVGComponent path="icons/main/hud/speedometer_1/designations/turn-arrow.svg" />
                </div>
              </div>
              
              <div class="velocities-content" class:no-gear={!isGearRenderAllowed}>
                <div class="velocities-content-animated velocity-{speed === 0 ? 0 : gear}">
                  <span class="velocity">N</span>
                  <span class="velocity">1</span>
                  <span class="velocity">2</span>
                  <span class="velocity">3</span>
                  <span class="velocity">4</span>
                  <span class="velocity">5</span>
                  <span class="velocity">6</span>
                  <span class="velocity">7</span>
                </div>
              </div>
              
              <div class="velocities-arrow" class:active={indicatorRight}>
                <div class="velocities__picture-arrowRight velocities-arrow__picture">
                  <SVGComponent path="icons/main/hud/speedometer_1/designations/turn-arrow.svg" />
                </div>
              </div>
            </div>
            
            <div class="car-status">
              <div class="oil-status" class:warning={oilTemp >= 120 && oilTemp <= 150} class:threat={oilTemp >= 150} class:hidden={!isOilRenderAllowed}>
                <SVGComponent path="icons/main/hud/speedometer_1/designations/engine-status.svg" />
                <div class="oil-status__text">{oilTemp != null ? Math.floor(oilTemp) : 0}</div>
              </div>
              
              <div class="speed-status">
                <div class="speed-status__text-speed">{Math.floor(speed)}</div>
                <div class="speed-status__text-speedType">km/h</div>
              </div>
              
              <div class="fuel-status" class:active={currentFuel <= 5} class:hidden={!isFuelRenderAllowed} class:electro={isElectro}>
                <SVGComponent path="icons/main/hud/speedometer_1/designations/{isElectro ? 'electroFuel' : 'fuel'}.svg" />
                <div class="fuel-status__text-fuel" class:active={currentFuel <= 5}>{Math.floor(currentFuel)}</div>
                <div class="fuel-status__text-maxFuel">{Math.floor(maxFuel)}</div>
              </div>
            </div>
            
            <div class="mileage">
              <div class="mileage__text-value">{mileage.toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</div>
              <div class="mileage__text-mileageType">km</div>
            </div>
            
            <div class="designations">
              <div class="highBeam" class:active={highBeam}>
                <SVGComponent path="icons/main/hud/speedometer_1/designations/high-beam.svg" />
              </div>
              <div class="dippedBeam" class:active={dippedBeam}>
                <SVGComponent path="icons/main/hud/speedometer_1/designations/dipped-beam.svg" />
              </div>
              <div class="belt" class:active={!seatbelt}>
                <SVGComponent path="icons/main/hud/speedometer_1/designations/seat-belt.svg" />
              </div>
              <div class="start" class:active={lock}>
                <SVGComponent path="icons/main/hud/speedometer_1/designations/lock.svg" />
              </div>
              <div class="doors" class:active={doorOpened}>
                <SVGComponent path="icons/main/hud/speedometer_1/designations/doors.svg" />
              </div>
            </div>
          </div>
          
          <div bind:this={speedometerContainer} class="carhud-container-background">
            <canvas bind:this={speedometerCanvas} class="speedometer-canvas"></canvas>
          </div>
        </div>
      {:else if speedoType === SPEEDO_TYPES.Advanced_2}
        <div class="carhud-container-second">
          <div bind:this={speedometerContainer} class="carhud-container-second__background">
            <canvas bind:this={speedometerCanvas} class="speedometer-canvas"></canvas>
          </div>
          
          {#if isFuelRenderAllowed}
            <div class="carhud-container-second__fuel" class:isElectro={isElectro} class:fuel-blinking={isFuelLow}>
              <div class="carhud-container-second__fuel__icon">
                <SVGComponent path="icons/main/hud/speedometer_2/{isElectro ? 'electroFuel' : 'fuel'}.svg" />
              </div>
              <div class="carhud-container-second__fuel__text fuel">
                {#if isElectro}
                  {Math.floor(currentFuel / maxFuel * 100)}%
                {:else}
                  <div>{Math.floor(currentFuel)}</div>
                  <div class="carhud-container-second__fuel__text__splitter"></div>
                  <div class="grey">{Math.round(maxFuel)}</div>
                {/if}
              </div>
            </div>
          {/if}
          
          {#if isOilRenderAllowed}
            <div class="carhud-container-second__oil">
              <div class="carhud-container-second__oil__icon">
                <SVGComponent path="icons/main/hud/speedometer_2/oilTemp.svg" />
              </div>
              <div class="carhud-container-second__oil__text">{Math.floor(oilTemp / 180 * 100)}%</div>
            </div>
          {/if}
          
          {#if !isFlying}
            <div class="carhud-container-second__numbers">
              {#each Array(9) as _, i}
                <div class="number-{i}" class:noOpacity={0.125 * i <= smoothRpm}         style="opacity: {i <= (smoothRpm * 8) ? Math.min(1, (smoothRpm * 8 - i) + 0.5) : 0.3}"
>{i}</div>
              {/each}
            </div>
          {/if}
          
          <div class="carhud-container-second__turnSignals">
            <div class="arrow" class:active={indicatorLeft}>
              <SVGComponent path="icons/main/hud/speedometer_2/turnArrow.svg" />
            </div>
            <div class="emergencyGang" class:active={handbrake}>
              <SVGComponent path="icons/main/hud/speedometer_2/emergencyGang.svg" />
            </div>
            <div class="arrow right" class:active={indicatorRight}>
              <SVGComponent path="icons/main/hud/speedometer_2/turnArrow.svg" />
            </div>
          </div>
          
          <div class="carhud-container-second__speed">
            <div class="carhud-container-second__speed__value">
              {#if isGearRenderAllowed}
                <div class="carhud-container-second__speed__value__gear">
                  {#key gearComputed}
                    <div>{gearComputed}</div>
                  {/key}
                </div>
              {/if}
              <div class="carhud-container-second__speed__value__speed">
                <div class="grey-color">{'0'.repeat(3 - speedRounded.toString().length)}</div>
                <div class:grey-color={Math.floor(speed) === 0}>{speedRounded}</div>
              </div>
            </div>
            <div class="carhud-container-second__speed__label">KM/H</div>
          </div>
          
          <div class="carhud-container-second__additional-icons">
            <div class="carhud-container-second__additional-icons__highBeam" class:active={highBeam}>
              <SVGComponent path="icons/main/hud/speedometer_2/highBeam.svg" />
            </div>
            <div class="carhud-container-second__additional-icons__dippedBeam" class:active={dippedBeam}>
              <SVGComponent path="icons/main/hud/speedometer_2/dippedBeam.svg" />
            </div>
            <div class="carhud-container-second__additional-icons__seatBelt" class:active={!seatbelt}>
              <SVGComponent path="icons/main/hud/speedometer_2/seatbelt.svg" />
            </div>
            <div class="carhud-container-second__additional-icons__doors" class:active={doorOpened}>
              <SVGComponent path="icons/main/hud/speedometer_2/doors.svg" />
            </div>
            <div class="carhud-container-second__additional-icons__lock" class:active={lock}>
              <SVGComponent path="icons/main/hud/speedometer_2/lock.svg" />
            </div>
          </div>
          
          <div class="carhud-container-second__mileage">
            <div class="carhud-container-second__mileage__value">
              {#each mileageComputed as { char, isGrey }}
                <div class="square" class:grey={isGrey}>{char}</div>
              {/each}
            </div>
            <div class="carhud-container-second__mileage__label">KM</div>
          </div>
        </div>
      {:else if speedoType === SPEEDO_TYPES.Minimalistic_1}
        <div class="carhud-container-simple">
          <div class="speedometer hud_container">
            <img src="{cdn}img/main/hud/speedometer_simple/speedometer.svg" alt="" />
            <div class="speedometer__text-speed" style="color: {getSpeedColor(speed)}">
              {Math.floor(speed)} km/h
            </div>
          </div>
          
          <div class="gas hud_container">
            {#if !isElectro}
              <img src="{cdn}img/main/hud/speedometer_simple/gas-station.svg" alt="" />
            {:else}
              <img src="{cdn}img/main/hud/speedometer_simple/energy.svg" alt="" />
            {/if}
            <div class="fuel">
              <span class="fuel__text-current">{Math.floor(currentFuel)}</span>
              <span class="fuel__text-max">/{Math.floor(maxFuel)} {!isElectro ? 'L' : ''}</span>
            </div>
          </div>
          
          <div class="additional-info hud_container">
            <div class="start-simple" class:active={lock}>
              <SVGComponent path="icons/main/hud/speedometer_3/lock.svg" />
            </div>
            <div class="belt-simple" class:active={!seatbelt}>
              <SVGComponent path="icons/main/hud/speedometer_3/seatbelt.svg" />
            </div>
            <div class="attention-simple" class:active={handbrake}>
              <SVGComponent path="icons/main/hud/speedometer_3/handbrake.svg" />
            </div>
          </div>
        </div>
      {:else if speedoType === SPEEDO_TYPES.Minimalistic_2}
        <div class="carhud-container-fourth">
          <div class="carhud-container-fourth__kmh">KM/H</div>
          <div class="carhud-container-fourth__speed">
            {#if isGearRenderAllowed}
              <div class="carhud-container-fourth__speed__gear">
                {#key gearComputed}
                  <div>{gearComputed}</div>
                {/key}
              </div>
            {/if}
            <div class="carhud-container-fourth__speed__value">
              <div class="grey-color">{'0'.repeat(Math.max(0, 3 - speedRounded.toString().length))}</div>
              <div class:grey-color={Math.floor(speed) === 0}>{speedRounded}</div>
            </div>
          </div>
          
          <div class="carhud-container-fourth__additional">
            <div class="carhud-container-fourth__additional__icons">
              <svg width="2.962962962962963vh" height="2.4074074074074074vh" viewBox="0 0 32 26" fill="none" xmlns="http://www.w3.org/2000/svg" class="carhud-container-fourth__additional__icons__emergencyGang" class:active={handbrake}>
                <path d="M17.1556 16.157C17.1556 16.6999 16.7947 17.1394 16.345 17.1394H15.6104C15.1608 17.1394 14.7998 16.6999 14.7998 16.157V5.83587C14.7998 5.29299 15.1608 4.85352 15.6104 4.85352H16.345C16.7947 4.85352 17.1556 5.29299 17.1556 5.83587V16.157Z" fill="white"/>
                <path d="M17.181 20.164C17.181 20.7069 16.82 21.1528 16.3704 21.1528H15.6104C15.1608 21.1528 14.7998 20.7133 14.7998 20.164V19.2398C14.7998 18.6969 15.1608 18.251 15.6104 18.251H16.3704C16.82 18.251 17.181 18.6905 17.181 19.2398V20.164Z" fill="white"/>
                <path d="M15.9966 1.05981C9.54342 1.05981 4.2998 6.41751 4.2998 12.9967C4.2998 19.5823 9.54976 24.9335 15.9966 24.9335C22.4498 24.9335 27.6934 19.5758 27.6934 12.9967C27.6998 6.41751 22.4498 1.05981 15.9966 1.05981ZM15.9966 22.6974C10.7593 22.6974 6.49731 18.3479 6.49731 13.0031C6.49731 7.65837 10.7593 3.30888 15.9966 3.30888C21.2339 3.30888 25.4959 7.65837 25.4959 13.0031C25.5023 18.3479 21.2402 22.6974 15.9966 22.6974Z" fill="white"/>
                <path d="M7.06115 23.9835C6.86483 23.8284 2.19751 20.1446 2.19751 13.0031C2.19751 5.86168 6.8585 2.1714 7.06115 2.01629C7.54245 1.64791 7.64378 0.943459 7.27647 0.452283C6.9155 -0.0453554 6.22521 -0.142298 5.73758 0.226084C5.50327 0.40058 0 4.70483 0 13.0031C0 21.3014 5.50327 25.5992 5.73758 25.7801C5.9339 25.9288 6.16188 26.0063 6.3962 26.0063C6.73184 26.0063 7.06115 25.8512 7.27647 25.5604C7.63744 25.0628 7.54245 24.3583 7.06115 23.9835Z" fill="white"/>
                <path d="M26.2561 0.226084C25.7684 -0.142298 25.0845 -0.0453554 24.7235 0.452283C24.3626 0.943459 24.4575 1.64791 24.9388 2.02275C25.1352 2.17786 29.8025 5.86168 29.8025 13.0096C29.8025 20.151 25.1415 23.8413 24.9388 23.9964C24.4575 24.3713 24.3562 25.0692 24.7235 25.5604C24.9388 25.8577 25.2682 26.0128 25.6038 26.0128C25.8318 26.0128 26.0661 25.9417 26.2624 25.7866C26.4967 25.6057 32 21.3079 32 13.0096C32 4.69837 26.4904 0.40058 26.2561 0.226084Z" fill="white"/>
              </svg>
              <div class="carhud-container-fourth__additional__icons__seatBelt" class:active={!seatbelt}>
                <SVGComponent path="icons/main/hud/speedometer_2/seatbelt.svg" />
              </div>
              <div class="carhud-container-fourth__additional__icons__lock" class:active={lock}>
                <SVGComponent path="icons/main/hud/speedometer_2/lock.svg" />
              </div>
            </div>
            <div class="carhud-container-fourth__additional__splitter"></div>
            <div class="carhud-container-fourth__additional__fuel" class:fuel-blinking={isFuelLow}>
              <div class="carhud-container-fourth__additional__fuel__icon">
                <SVGComponent path="icons/main/hud/speedometer_2/{isElectro ? 'electroFuel' : 'fuel'}.svg" />
              </div>
              <div class="carhud-container-fourth__additional__fuel__text" style="--chars: {getMaximumFuelChars}">
                {#if isFuelRenderAllowed}
                  {#if isElectro}
                    <span>{Math.round(currentFuel / maxFuel * 100)}%</span>
                  {:else}
                    <span>{Math.floor(currentFuel)}</span>/{Math.floor(maxFuel)}L
                  {/if}
                {:else}
                  N/A
                {/if}
              </div>
            </div>
          </div>
        </div>
      {:else if speedoType === SPEEDO_TYPES.Minimalistic_3}
        <div class="carhud-container-fifth">
          <div class="carhud-container-fifth__speed">
            <div class="carhud-container-fifth__speed__value">{speedRounded}</div>
            <div class="carhud-container-fifth__speed__label">KM/H</div>
          </div>
          <div class="carhud-container-fifth__splitter"></div>
          <div class="carhud-container-fifth__additional">
            <div class="carhud-container-fifth__additional__fuel" class:fuel-blinking={isFuelLow}>
              <div class="carhud-container-fifth__additional__fuel__icon">
                <SVGComponent path="icons/main/hud/speedometer_2/{isElectro ? 'electroFuel' : 'fuel'}.svg" />
              </div>
              <div class="carhud-container-fifth__additional__fuel__text">
                {#if isFuelRenderAllowed}
                  {#if isElectro}
                    <span>{Math.round(currentFuel / maxFuel * 100)}%</span>
                  {:else}
                    <span>{Math.floor(currentFuel)}</span>/{Math.floor(maxFuel)} L
                  {/if}
                {:else}
                  N/A
                {/if}
              </div>
            </div>
            <div class="carhud-container-fifth__additional__icons">
              <div class="carhud-container-fifth__additional__icons__emergencyGang" class:active={handbrake}>
                <SVGComponent path="icons/main/hud/speedometer_2/emergencyGang.svg" />
              </div>
              <div class="carhud-container-fifth__additional__icons__seatBelt" class:active={!seatbelt}>
                <SVGComponent path="icons/main/hud/speedometer_2/seatbelt.svg" />
              </div>
              <div class="carhud-container-fifth__additional__icons__lock" class:active={lock}>
                <SVGComponent path="icons/main/hud/speedometer_2/lock.svg" />
              </div>
            </div>
          </div>
        </div>
      {/if}
    </div>
  </div>
{/if}