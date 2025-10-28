<script>
  // Page that mirrors the original Vue structure:
  // div.page -> div.title -> div.tabs-wrapper.single -> div.tab -> div.column-wrapper
  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";

  export let title = "Интерфейс";

  // keys for this page (use the same keys as in your original config)
  const settingKeys = pageSettingKeys["basic:interface"] ?? [];
</script>


        <ColumnWrapper {settingKeys} />
