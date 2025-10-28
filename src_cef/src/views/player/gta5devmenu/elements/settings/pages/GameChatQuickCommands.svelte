<script>
  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import PreviewChatCmds from "../components/PreviewChatCmds.svelte";
  import ApplySettingCode from "../components/ApplySettingCode.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";

  const settingKeys = pageSettingKeys["game-chat:quick-commands"] ?? [];
</script>

<div class="tab glued-columns">
  <div class="column-wrapper single glued-columns">
    <ColumnWrapper settingKeys={settingKeys} />
  </div>
  <ApplySettingCode pageName="game-chat:quick-commands" />
</div>

<div class="preview-tab">
  <PreviewChatCmds />
</div>