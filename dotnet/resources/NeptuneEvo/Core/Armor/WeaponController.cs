// WeaponController.cs
using GTANetworkAPI;
using NeptuneEvo.Handles;

public class WeaponController : Script
{
    [ServerEvent(Event.ResourceStart)]
    public static void OnStart() => WeaponManager.OnStart();

    [RemoteEvent("server.weaponManager.open")]
    public static void OnOpen(ExtPlayer player)
      => WeaponManager.OpenManager(player);

    [RemoteEvent("server.weaponManager.apply")]
    public static void OnApply(ExtPlayer player, int skinId)
      => WeaponManager.ApplySkin(player, skinId);

    [RemoteEvent("server.weaponManager.installTrackStat")]
    public static void OnInstallStattrak(ExtPlayer player, int skinId)
      => WeaponManager.InstallTrackStat(player, skinId);
}
