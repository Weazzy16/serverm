namespace NeptuneEvo.EternalDev.ExChangeProps.Classes
{
    public class VehicleProperty
    {
        public string Model { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public int Mile { get; set; }
        public TuningData Tun { get; set; }

        public class TuningData
        {
            public int Engine { get; set; }
            public int Turbo { get; set; }
            public int Suspension { get; set; }
            public int Transmission { get; set; }
        }
    }
}
