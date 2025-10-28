<script>
  // Страница "Настройка звука" — использует ColumnWrapper и pageSettingKeys["sound"]
  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";

  export let title = "Настройка звука";

  const settingKeys = pageSettingKeys["sound"] ?? [];
</script>


        <ColumnWrapper {settingKeys} />
