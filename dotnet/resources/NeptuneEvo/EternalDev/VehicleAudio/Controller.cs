using EternalCore;
using GTANetworkAPI;
using LinqToDB.Common;
using NeptuneEvo.EternalDev.Sounds.Enums;
using NeptuneEvo.EternalDev.VehicleAudio.Configs;
using NeptuneEvo.EternalDev.VehicleAudio.Extensions;
using NeptuneEvo.Handles;
using Redage.SDK;
using System;

namespace NeptuneEvo.EternalDev.VehicleAudio
{
    public class Controller : Script
    {
        public static VehicleAudioConfig Config = ELib.JsonReader.Read<VehicleAudioConfig>(VehicleAudioConfig.Path);

        [RemoteEvent("server.vehicleAudio.open")]
        public static void Open(ExtPlayer player)
        {
            try
            {
                if (!GetPlayerVehicle(player, true, out var vehicle))
                    return;

                if (Config.BlockedVehicleClasses.Contains(vehicle.Class))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "У данного вида транспорта нет проигрывателя!", 3000);
                    return;
                }

                string soundId = vehicle.HasData("sound.id") ? vehicle.GetData<string>("sound.id") : "";
                Trigger.ClientEvent(player, "client.vehicleAudio.open", soundId);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(Open), ex, nameof(Controller)); }
        }

        public static bool GetPlayerVehicle(ExtPlayer player, bool checkDriver, out ExtVehicle vehicle)
        {
            vehicle = player.Vehicle as ExtVehicle;
            if (vehicle == null)
                return false;

            if (checkDriver && NAPI.Vehicle.GetVehicleDriver(vehicle) != player)
                return false;

            return true;
        }

        [RemoteEvent("server.vehicleAudio.playOrPause")]
        public static void PlayOrPause(ExtPlayer player, string url, bool looped)
        {
            try
            {
                if (!player.IsTimeouted("vehicleAudio.playOrPause", 1) || url.IsNullOrEmpty()) 
                    return;

                if (!GetPlayerVehicle(player, true, out var vehicle))
                    return;

                Sounds.Controller.GetSoundByEntity(player.Vehicle, out var currentSound);
                if (currentSound is null || currentSound.Url != url)
                {
                    if (!player.IsTimeouted("vehicleAudio.play", 5))
                    {
                        Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, "Немного подождите...", 3000);
                        return;
                    }

                    if (currentSound != null && currentSound.Url != url)
                        Sounds.Controller.Destroy(currentSound.Id);

                    Sounds.Controller.CreateSound(player.Vehicle, SoundType.Vehicle, url, 100, looped);
                }
                else
                {
                    Sounds.Controller.SetPaused(currentSound.Id, !currentSound.IsPausing);
                }
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(PlayOrPause), ex, nameof(Controller)); }
        }

        [RemoteEvent("server.vehicleAudio.setLooped")]
        public static void SetLooped(ExtPlayer player, string soundId, bool state)
        {
            try
            {
                if (!player.IsTimeouted("vehicleAudio.setLooped", 1) || 
                    !GetPlayerVehicle(player, true, out var vehicle)) 
                    return;

                Sounds.Controller.SetLooped(soundId, state);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(SetLooped), ex, nameof(Controller)); }
        }

        [RemoteEvent("server.vehicleAudio.volume.change")]
        public static void ChangeVolume(ExtPlayer player, string soundId, int volume)
        {
            try
            {
                if (!player.IsTimeouted("vehicleAudio.volume", 1) 
                    || !GetPlayerVehicle(player, true, out var vehicle)) 
                    return;

                Sounds.Controller.SetVolume(soundId, volume);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(ChangeVolume), ex, nameof(Controller)); }
        }

        [ServerEvent(Event.VehicleDeath)]
        public static void OnDeathVehicle(Vehicle vehicle)
        {
            try
            {
                string soundId = string.Empty;
                if (vehicle.HasData("sound.id"))
                    soundId = vehicle.GetData<string>("sound.id");

                if (soundId.IsNullOrEmpty()) 
                    return;
                
                Sounds.Controller.Destroy(soundId);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnDeathVehicle), ex, nameof(Controller)); }
        }
    }
}
