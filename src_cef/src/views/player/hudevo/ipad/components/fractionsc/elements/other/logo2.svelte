<script>
    import { charFractionID } from 'store/chars';

    // Определяем базовый URL для изображений
    const cloudUrl = 'http://cdn.piecerp.ru/cloud/'; // Замените на ваш реальный URL

    // Вычисляем URL иконки фракции
    $: logoUrl = cloudUrl + ($charFractionID > 0 ? `img/fractionLogo/Fraction_${$charFractionID}.png` : `img/fractionLogo/no-fraction.svg`);
</script>

<!-- Убедитесь, что стили соответствуют вашему дизайну -->
<div class="newtablet__mainmenu_icon" style="background-image: url({logoUrl});"></div>

<style>
    .logo-icon {
        width: 64px; /* Настройте размер под ваш дизайн */
        height: 64px;
        background-size: cover;
        background-position: center;
    }
</style>
