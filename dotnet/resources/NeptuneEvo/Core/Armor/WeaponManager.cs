// WeaponManager.cs
using GTANetworkAPI;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using Redage.SDK;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public static class WeaponManager
{
    const string DB = "player_weapon_skins";
    // uuid -> (skinId -> isApplied)
    static Dictionary<int, Dictionary<int, bool>> PlayerSkins = new Dictionary<int, Dictionary<int, bool>>();


    public static void OnStart()
    {
        var dt = MySQL.QueryRead($"SELECT * FROM `{DB}`");
        if (dt != null)
        {
            foreach (DataRow r in dt.Rows)
            {
                int uuid = (int)r["uuid"];
                var list = JsonConvert.DeserializeObject<Dictionary<int, bool>>(r["list"].ToString());
                PlayerSkins[uuid] = list!;
            }
        }
    }

    static bool EnsurePlayer(int uuid, out Dictionary<int, bool> list)
    {
        if (!PlayerSkins.TryGetValue(uuid, out list))
        {
            list = new Dictionary<int, bool>();
            PlayerSkins[uuid] = list;
            MySQL.Query($"INSERT INTO `{DB}` (`uuid`,`list`) VALUES({uuid},'{JsonConvert.SerializeObject(list)}')");
            return false;
        }
        return true;
    }

    public static void OpenManager(ExtPlayer player)
    {
        var uuid = player.GetUUID();
        EnsurePlayer(uuid, out var list);
        // отправляем клиенту и конфиг
        Trigger.ClientEvent(player,
          "client.weaponManager.open",
          JsonConvert.SerializeObject(list),
          JsonConvert.SerializeObject(WeaponConfig.WEAPON_SKIN_TYPES));
    }

    public static void ApplySkin(ExtPlayer player, int skinId)
    {
        var uuid = player.GetUUID();
        if (!EnsurePlayer(uuid, out var list) || !list.ContainsKey(skinId)) return;

        // снимаем все остальные для того же WeaponId
        var wd = WeaponConfig.GetSkin(skinId);
        foreach (var k in list.Keys.Where(k => WeaponConfig.GetSkin(k)?.WeaponId == wd.WeaponId).ToArray())
            list[k] = false;

        list[skinId] = true;
        Save(uuid);
        // здесь логика, чтобы в игре «надеть» ваш WeaponData.ModelHash
        // например: Trigger.ClientEvent(player,"client.weaponManager.onApplied",skinId);
        OpenManager(player);
    }

    public static void InstallTrackStat(ExtPlayer player, int skinId)
    {
        var uuid = player.GetUUID();
        if (!EnsurePlayer(uuid, out var list) || !list.ContainsKey(skinId)) return;

        // ваше условие наличия счётчика
        bool hasCounter = true;
        if (!hasCounter)
        {
            Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Нет доступных StatTrak", 3000);
            return;
        }

        // тут можно, например, выставить дополнительный флаг или удалять предмет
        Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "StatTrak установлен", 3000);
    }

    static void Save(int uuid)
    {
        if (EnsurePlayer(uuid, out var list))
        {
            MySQL.Query($"UPDATE `{DB}` SET `list`='{JsonConvert.SerializeObject(list)}' WHERE `uuid`={uuid}");
        }
    }
}
