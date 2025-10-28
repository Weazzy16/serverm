    <script>
        import { charData } from 'store/chars';
        import "./report.css"; 
    </script>
    {#if $charData.isAdmin > 2}
    <div class="report-info align-center justify-end row-block">
        <div class="report-info-container row-block align-center justify-center">
            Репорты:
            <div class="report-info__title"></div>
            <!-- вместо «1» выводим переменную -->
            <div class="reports-amount">{$charData.ReportsCount}</div>
        </div>

        <div class="report-info-container row-block align-center justify-center">
            Модерация:
            <div class="report-info__title"></div>
            <!-- вместо «1» выводим переменную -->
            <div class="reports-amount">0</div>
        </div>
    </div>
    {/if}