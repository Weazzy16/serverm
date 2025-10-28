using NeptuneEvo.Handles;

namespace NeptuneEvo.EternalDev.Containers.Classes
{
    public class BetData
    {
        public string Login { get; set; }
        public int Uuid { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public int UpTime { get; set; } = 0;

        public ExtPlayer GetPlayer()
            => Main.GetPlayerByUUID(Uuid);

        public BetData(int uuid, string login, string name, int price)
        {
            Uuid = uuid;
            Login = login;
            Name = name;
            Price = price;
        }
    }
}
