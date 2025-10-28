<script>
  import { onMount } from 'svelte';

  let loginHistory = [];
  let isLoading = false;
  let error = null;

  onMount(() => {
    loadLoginHistory();
  });

  function loadLoginHistory() {
    isLoading = true;
    error = null;

    if (window.CEF && window.CEF.call) {
      window.CEF.call("W2C:Menu_F4_Settings_Secure_Secure:GetLoginHistory", (data) => {
        if (data?.success && Array.isArray(data.history)) {
          loginHistory = data.history;
        } else {
          // Тестовые данные если API не работает
          loginHistory = [
            {
              id: 1,
              timestamp: Math.floor(Date.now() / 1000) - 3600, // 1 час назад
              country: 'Romania',
              city: 'Brasov',
              socialClub: 'Mason4x',
              ip: '82.77.130.130',
              hwid: '7002***2304'
            },
            {
              id: 2,
              timestamp: Math.floor(Date.now() / 1000) - 86400, // 1 день назад
              country: 'Romania',
              city: 'Brasov',
              socialClub: 'Mason4x',
              ip: '82.77.130.130',
              hwid: '7002***2304'
            },
            {
              id: 3,
              timestamp: Math.floor(Date.now() / 1000) - 172800, // 2 дня назад
              country: 'Romania',
              city: 'Brasov',
              socialClub: 'Mason4x',
              ip: '188.77.142.142',
              hwid: '2430***4506'
            },
            {
              id: 4,
              timestamp: Math.floor(Date.now() / 1000) - 259200, // 3 дня назад
              country: 'Romania',
              city: 'Brasov',
              socialClub: 'Mason4x',
              ip: '188.77.142.142',
              hwid: '2430***4506'
            },
            {
              id: 5,
              timestamp: Math.floor(Date.now() / 1000) - 345600, // 4 дня назад
              country: 'Romania',
              city: 'Brasov',
              socialClub: 'Mason4x',
              ip: '188.77.142.142',
              hwid: '2430***4506'
            },
            {
              id: 6,
              timestamp: Math.floor(Date.now() / 1000) - 432000, // 5 дней назад
              country: 'Ukraine',
              city: 'Kyiv',
              socialClub: 'Mason4x',
              ip: '195.88.200.200',
              hwid: '5511***7890'
            },
          ];
        }
        isLoading = false;
      });
    } else {
      // Если CEF не доступен - используем тестовые данные
      loginHistory = [
        {
          id: 1,
          timestamp: Math.floor(Date.now() / 1000) - 3600,
          country: 'Romania',
          city: 'Brasov',
          socialClub: 'Mason4x',
          ip: '82.77.130.130',
          hwid: '7002***2304'
        },
        {
          id: 2,
          timestamp: Math.floor(Date.now() / 1000) - 86400,
          country: 'Romania',
          city: 'Brasov',
          socialClub: 'Mason4x',
          ip: '82.77.130.130',
          hwid: '7002***2304'
        },
        {
          id: 3,
          timestamp: Math.floor(Date.now() / 1000) - 172800,
          country: 'Romania',
          city: 'Brasov',
          socialClub: 'Mason4x',
          ip: '188.77.142.142',
          hwid: '2430***4506'
        },
      ];
      isLoading = false;
    }
  }

  function formatDate(timestamp) {
    if (!timestamp) return '-';
    const date = new Date(timestamp * 1000 || timestamp);
    return date.toLocaleDateString('ru-RU') + ' ' + date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' });
  }

  function maskIP(ip) {
    if (!ip) return '-';
    const parts = ip.split('.');
    if (parts.length === 4) {
      return `${parts[0]}.${parts[1]}.***.*${parts[3]}`;
    }
    return ip;
  }

  function maskHWID(hwid) {
    if (!hwid || hwid.length < 8) return hwid || '-';
    return hwid.substring(0, 4) + '***' + hwid.substring(hwid.length - 4);
  }
</script>

<div data-v-53ab6017 class="page">  
  <p data-v-53ab6017>
    Информацией о последних авторизациях: дата, время, устройство, IP-адрес
    и местоположение. Помогает отслеживать активность и повышает
    безопасность аккаунта.
  </p>

  <div data-v-53ab6017 class="table">
    <!-- Заголовок таблицы -->
    <div data-v-53ab6017 class="header">
      <p data-v-53ab6017 class="field date">Дата и время</p>
      <p data-v-53ab6017 class="field country">Страна и город</p>
      <p data-v-53ab6017 class="field socialClub">Social Club</p>
      <p data-v-53ab6017 class="field ip">IP</p>
      <p data-v-53ab6017 class="field hwid">HWID</p>
    </div>

    <!-- Контейнер с записями -->
    <div data-v-53ab6017 class="containerProps" style="overflow-y: auto;">
      {#if isLoading}
        <div data-v-53ab6017 class="loader">
          <p data-v-53ab6017>Загрузка списка</p>
        </div>
      {:else if error}
        <div data-v-53ab6017 class="error-message">
          <p data-v-53ab6017>{error}</p>
        </div>
      {:else if loginHistory && loginHistory.length > 0}
        {#each loginHistory as entry (entry.id || entry.timestamp)}
          <div data-v-53ab6017 class="table-item">
            <p data-v-53ab6017 class="field date">
              {formatDate(entry.timestamp)}
            </p>
            <p data-v-53ab6017 class="field country">
              {entry.country ? `${entry.country}, ${entry.city || ''}` : '-'}
            </p>
            <p data-v-53ab6017 class="field socialClub">
              {entry.socialClub || '-'}
            </p>
            <p data-v-53ab6017 class="field ip">
              {maskIP(entry.ip)}
            </p>
            <p data-v-53ab6017 class="field hwid">
              {maskHWID(entry.hwid)}
            </p>
          </div>
        {/each}
      {:else}
        <div data-v-53ab6017 class="empty-state">
          <p data-v-53ab6017>История входов пуста</p>
        </div>
      {/if}
    </div>
  </div>
</div>
