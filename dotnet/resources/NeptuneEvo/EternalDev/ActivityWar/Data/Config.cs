namespace NeptuneEvo.EternalDev.ActivityWar.Data
{
    public class Config
    {
        /// <summary>
        /// Цена в час за каждый актив
        /// </summary>
        public static readonly int PRICE_FOR_ANY_ACTIVIY = 10000;

        /// <summary>
        /// Минимальное количество игроков с одной организации
        /// </summary>
        public static readonly int MINIMAL_PLAYERS_FROM_ORGANIZATION_TO_START = 1;

        /// <summary>
        /// Время, за которое игрок может захватить точку
        /// </summary>
        public static readonly int TIME_TO_WAR = 240;
    }
}
