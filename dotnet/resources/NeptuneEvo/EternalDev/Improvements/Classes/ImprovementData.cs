namespace NeptuneEvo.EternalDev.Improvements.Classes
{
    public class ImprovementsData
    {
        /// <summary>
        /// На сколько процентов будет снижен урон
        /// </summary>
        public int DamageMultiplayer { get; set; }

        /// <summary>
        /// Время действия эффекта
        /// </summary>
        public int TimeEffect { get; set; }

        /// <summary>
        /// Сколько хп добавить сразу же при использовании
        /// </summary>
        public int ExtraHealing { get; set; }

        /// <summary>
        /// Количество единиц хп, которое будет добавляться каждый HealingCooldown
        /// </summary>
        public int Healing { get; set; }

        /// <summary>
        /// Каждые N секунд будет добавляться здоровье в размере Healing
        /// </summary>
        public int HealingCooldown { get; set; }

        public AnimationData Animation { get; set; }

        public ImprovementsData(int damageMultiplayer, int timeEffect, int extraHealing = 0, 
            int healing = 0, int healingCooldown = 0, AnimationData animationData = null)
        {
            DamageMultiplayer = damageMultiplayer;
            TimeEffect = timeEffect;
            ExtraHealing = extraHealing;

            Healing = healing;
            HealingCooldown = healingCooldown;

            if (animationData == null)
                animationData = new AnimationData("amb@world_human_drinking@beer@male@idle_a", "idle_c", 49, 5000);

            Animation = animationData;
        }

        public class AnimationData
        {
            public string Dictionary { get; set; }
            public string Name { get; set; }
            public int Flag { get; set; }
            public int Duration { get; set; }

            public AnimationData(string dictionary, string name, int flag, int duration)
            {
                Dictionary = dictionary;
                Name = name;
                Flag = flag;
                Duration = duration;
            }
        }
    }
}
