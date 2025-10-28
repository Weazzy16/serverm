<script>
  import SettingItem from "./SettingItem.svelte";
  export let settingKeys = [];
  
</script>

  {#each settingKeys as key (key)}
    <SettingItem settingKey={key} />
  {/each}
