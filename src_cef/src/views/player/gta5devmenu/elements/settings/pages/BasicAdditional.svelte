<script>
  // Страница "Дополнительно" — структура: div.page -> div.title -> div.tabs-wrapper.single -> div.tab -> div.column-wrapper
  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";

  export let title = "Дополнительно";

  // Берём ключи для этой страницы из общего конфига
  const settingKeys = pageSettingKeys["basic:additional"] ?? [];
</script>


        <ColumnWrapper {settingKeys} />
