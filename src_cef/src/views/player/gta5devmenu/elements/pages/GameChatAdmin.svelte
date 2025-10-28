<script>
  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";
  export let title = "Администрирование чата";

  const settingKeys = pageSettingKeys["game-chat:admin"] ?? [];
</script>


        <ColumnWrapper settingKeys={settingKeys} />
