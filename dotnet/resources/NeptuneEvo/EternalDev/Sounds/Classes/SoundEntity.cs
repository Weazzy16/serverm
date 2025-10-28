using GTANetworkAPI;
using NeptuneEvo.EternalDev.Sounds.Enums;
using NeptuneEvo.Handles;
using Newtonsoft.Json;
using System;

namespace NeptuneEvo.EternalDev.Sounds.Classes
{
    public class SoundEntity
    {
        public string Id { get; set; }
        public SoundType Type { get; set; }

        public string Url { get; set; }
        public bool IsLooped { get; set; }
        public int Volume { get; set; }
        public int MaxDistance { get; set; }
        public bool IsPausing { get; set; }

        public int StartOffset
        {
            get
            {
                return (int)Math.Floor((DateTime.Now - Start).TotalSeconds);
            }
        }

        public DateTime Start { get; set; } = DateTime.Now;
        public Entity Entity { get; set; }

        public object GetData()
        {
            return new
            {
                id = Id,
                url = Url,
                type = (int)Type,

                looped = IsLooped,
                startOffset = StartOffset,
                volume = Volume,
                maxDistance = MaxDistance,

                isPausing = IsPausing,
            };
        }

        public void Init(ExtPlayer player)
        {
            Trigger.ClientEvent(player, "e-dev.sound_manager.create3d", Entity, JsonConvert.SerializeObject(GetData()));
        }
    }
}
