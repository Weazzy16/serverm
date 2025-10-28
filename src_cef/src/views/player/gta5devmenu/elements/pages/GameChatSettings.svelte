<script>
  import ColumnWrapper from "../components/ColumnWrapper.svelte";
  import PreviewChat from "../components/PreviewChat.svelte";
  import { pageSettingKeys } from "../lib/settingsConfig.js";

  const settingKeys = pageSettingKeys["game-chat:chat-settings"] ?? [];
  const previewKeys = pageSettingKeys["game-chat:chat-settings-preview"] ?? ["chatWidth", "chatHeight"];
</script>

  <div class="tab">
    <div class="column-wrapper single glued-columns">
      <ColumnWrapper settingKeys={settingKeys} />
    </div>
  </div>

  <div class="preview-tab">
    <PreviewChat />
    
    <div class="column-wrapper single glued-columns">
      <ColumnWrapper settingKeys={previewKeys} />
    </div>
  </div>
