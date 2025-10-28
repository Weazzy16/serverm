using NeptuneEvo.EternalDev.Containers.Classes;
using System.Collections.Generic;

namespace NeptuneEvo.EternalDev.Containers.Data
{
    public class ContainerSettingsData
    {
        /// <summary>
        /// Название контейнера
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список призов
        /// </summary>
        public List<PrizeData> Prizes { get; set; } = new List<PrizeData>();

        /// <summary>
        /// Донатный контейнер или нет
        /// </summary>
        public bool IsDonate { get; set; }

        /// <summary>
        /// Модель контейнера
        /// </summary>
        public string ContainerModel { get; set; }

        /// <summary>
        /// Модель левой двери контейнера
        /// </summary>
        public string LeftDoorModel { get; set; }

        /// <summary>
        /// Модель правой двери контейнера
        /// </summary>
        public string RightDoorModel { get; set; }

        public ContainerSettingsData(string name, List<PrizeData> prizes, bool isDonate, string containerModel, string leftDoorModel, string rightDoorModel)
        {
            Name = name;
            Prizes = prizes;
            IsDonate = isDonate;
            ContainerModel = containerModel;
            LeftDoorModel = leftDoorModel;
            RightDoorModel = rightDoorModel;
        }
    }
}
