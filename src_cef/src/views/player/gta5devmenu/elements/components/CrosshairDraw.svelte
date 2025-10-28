<script>
  import { onMount } from 'svelte';

  export let crosshairData = {};
  export let isPreview = false;

  let canvas;
  let ctx;

  function drawCrosshair() {
    if (!canvas || !ctx) return;

    const width = canvas.width;
    const height = canvas.height;
    const centerX = width / 2;
    const centerY = height / 2;

    ctx.clearRect(0, 0, width, height);
    ctx.globalAlpha = (crosshairData.crosshairOpacity || 100) / 100;
    ctx.strokeStyle = crosshairData.crosshairColor || '#FFFFFF';
    ctx.fillStyle = crosshairData.crosshairColor || '#FFFFFF';
    ctx.lineWidth = crosshairData.crosshairLineThickness || 1;

    const distance = crosshairData.crosshairCentralDistance || 0;
    const length = crosshairData.crosshairLineLength || 5;
    const incline = crosshairData.crosshairIncline || 0;

    if (crosshairData.crosshairCenter > 0) {
      const dotSize = crosshairData.crosshairCenter === 1 ? 2 : 4;
      ctx.fillRect(centerX - dotSize / 2, centerY - dotSize / 2, dotSize, dotSize);
    }

    ctx.save();
    ctx.translate(centerX, centerY);
    ctx.rotate((incline * Math.PI) / 180);

    ctx.beginPath();
    ctx.moveTo(0, -distance);
    ctx.lineTo(0, -(distance + length));
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(0, distance);
    ctx.lineTo(0, distance + length);
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(-distance, 0);
    ctx.lineTo(-(distance + length), 0);
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(distance, 0);
    ctx.lineTo(distance + length, 0);
    ctx.stroke();

    ctx.restore();
    ctx.globalAlpha = 1;
  }

  onMount(() => {
    if (canvas) {
      ctx = canvas.getContext('2d');
      canvas.width = canvas.parentElement.offsetWidth;
      canvas.height = canvas.parentElement.offsetHeight;
      drawCrosshair();
    }
  });

  $: if (ctx && crosshairData) {
    drawCrosshair();
  }
</script>

<canvas bind:this={canvas}></canvas>

<style>
  canvas {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: block;
  }
</style>