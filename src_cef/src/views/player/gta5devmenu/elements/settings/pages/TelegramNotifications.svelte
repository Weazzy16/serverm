<script>
  import ColumnWrapper from '../components/ColumnWrapper.svelte';

  // Ключи для каждой секции Telegram уведомлений
  const accountKeys = [
    'telegramActivity',
    'telegramRules',
  ];

  const attackNotificationKeys = [
    'telegramAttack',
    'territoryAttack',
  ];

  const propertyTaxKeys = [
    'telegramPersonalHouse',
    'telegramFamilyHouse',
    'telegramFamilyOffice',
    'telegramFamilyStorage',
    'telegramApartment',
    'telegramBusiness',
  ];

  const marketplaceKeys = [
    'telegramVictory',
    'telegramSale',
    'telegramBid',
    'telegramStock',
    'rentalTransport',
    'rentalReturn',
  ];
</script>

<div class="tabs-wrapper">
  <!-- Аккаунт -->
  <div class="tab darkTitle">
    <p>Аккаунт</p>
    <div class="column-wrapper single glued-columns">
      <ColumnWrapper settingKeys={accountKeys} />
    </div>
  </div>

  <!-- Уведомление о нападении -->
  <div class="tab darkTitle">
    <p>Уведомление о нападении</p>
    <div class="column-wrapper single glued-columns">
      <ColumnWrapper settingKeys={attackNotificationKeys} />
    </div>
  </div>

  <!-- Налог на имущество -->
  <div class="tab darkTitle">
    <p>Налог на имущество</p>
    <div class="column-wrapper single glued-columns">
      <ColumnWrapper settingKeys={propertyTaxKeys} />
    </div>
  </div>

  <!-- Маркетплейс -->
  <div class="tab darkTitle">
    <p>Маркетплейс</p>
    <div class="column-wrapper single glued-columns">
      <ColumnWrapper settingKeys={marketplaceKeys} />
    </div>
  </div>
</div>

