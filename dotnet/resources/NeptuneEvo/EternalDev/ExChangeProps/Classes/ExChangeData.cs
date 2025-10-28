using NeptuneEvo.EternalDev.ExChangeProps.Enums;

namespace NeptuneEvo.EternalDev.ExChangeProps.Classes
{
    public class ExChangeData
    {
        public int Owner { get; set; }
        public int Target { get; set; }

        public PropertyType OwnerPropType { get; set; } = PropertyType.None;
        public PropertyType TargetPropType { get; set; } = PropertyType.None;

        public int OwnerPropIndex { get; set; } = 0;
        public int TargetPropIndex { get; set; } = 0;

        public int OwnerSurcharge { get; set; } = 0;
        public int TargetSurcharge { get; set; } = 0;

        public bool OwnerReady { get; set; } = false;
        public bool TargetReady { get; set; } = false;
    }
}
