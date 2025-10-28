using EternalCore;
using GTANetworkAPI;
using NeptuneEvo.Character;
using NeptuneEvo.EternalDev.Sounds.Classes;
using NeptuneEvo.EternalDev.Sounds.Enums;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeptuneEvo.EternalDev.Sounds
{
    public class Controller : Script
    {
        private static int _lastId = 0;
        private static Dictionary<string, SoundEntity> _pool = new Dictionary<string, SoundEntity>();

        [ServerEvent(Event.PlayerConnected)]
        public static void OnConnected(ExtPlayer player)
        {
            _pool.Values.ToList().ForEach(sound 
                => sound.Init(player));
        }

        [ServerEvent(Event.EntityDeleted)]
        public static void OnEntityDelete(Entity entity)
        {
            try
            {
                if (!entity.HasData("sound.id")) 
                    return;

                var soundId = entity.GetData<string>("sound.id");
                Destroy(soundId);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(OnEntityDelete), ex, nameof(Controller)); }
        }

        public static bool GetSound(string id, out SoundEntity sound)
            => _pool.TryGetValue(id, out sound);

        public static bool GetSoundByEntity(Entity entity, out SoundEntity sound)
        {
            sound = _pool.Values.ToList().Find(x => x.Entity == entity);
            return sound != null;
        }

        public static void CreateSound(Entity entity, SoundType type, string url, int volume, bool isLooped = false, int maxDistance = 15)
        {
            try
            {
                var sound = new SoundEntity()
                {
                    Id = $"{type.ToString().ToLower()}_{_lastId}",
                    Entity = entity,
                    Type = type,
                    Volume = volume,
                    IsLooped = isLooped,
                    Url = url,
                    MaxDistance = maxDistance,
                    Start = DateTime.Now,
                };

                _lastId++;

                entity.SetData("sound.id", sound.Id);
                _pool.TryAdd(sound.Id, sound);

                Trigger.ClientEventForAll("e-dev.sound_manager.create3d", 
                    entity, 
                    JsonConvert.SerializeObject(sound.GetData())
                );
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(CreateSound), ex, nameof(Controller)); }
        }

        [RemoteEvent("server.soundManager.tryStop")]
        public static void Stop(ExtPlayer player, string soundId)
        {
            try
            {
                if (!player.IsCharacterData())
                    return;

                Destroy(soundId);
            }
            catch(Exception ex) { ELib.Logger.Error(nameof(Stop), ex, nameof(Controller)); }
        }

        public static void Destroy(string soundId)
        {
            try
            {
                if (!GetSound(soundId, out var sound)) 
                    return;

                if (sound.Entity != null)
                    sound.Entity.ResetData("sound.id");

                Trigger.ClientEventForAll("e-dev.sound_manager.stop3d", soundId);
                _pool.Remove(soundId);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(Destroy), ex, nameof(Controller)); }
        }

        public static void SetPaused(string soundId, bool toggle)
        {
            try
            {
                if (!GetSound(soundId, out var sound))
                    return;

                sound.IsPausing = toggle;
                Trigger.ClientEventForAll("e-dev.sound_manager.pause3d", soundId, toggle);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(SetPaused), ex, nameof(Controller)); }
        }

        public static void SetVolume(string soundId, int value)
        {
            try
            {
                if (value < 0 || value > 100 || !GetSound(soundId, out var sound))
                    return;

                sound.Volume = value;
                Trigger.ClientEventForAll("e-dev.sound_manager.change_volume", soundId, value);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(SetVolume), ex, nameof(Controller)); }
        }

        public static void SetLooped(string soundId, bool toggle)
        {
            try
            {
                if (!GetSound(soundId, out var sound)) 
                    return;

                sound.IsLooped = toggle;
                Trigger.ClientEventForAll("e-dev.sound_manager.loop3d", soundId, toggle);
            }
            catch (Exception ex) { ELib.Logger.Error(nameof(SetLooped), ex, nameof(Controller)); }
        }
    }
}
