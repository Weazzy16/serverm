using GTANetworkAPI;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Events.Christmas.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeptuneEvo.Events.Christmas.BonusItems.Classes
{
    public class BonusItem
    {
        public string ModelName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public bool IsAlive { get { return _object != null;  } }

        public BonusItem(string modelName, Vector3 position, Vector3 rotation)
        {
            ModelName = modelName;
            Position = position;
            Rotation = rotation;

            BonusItemsManager.Pool.Add(this);
        }

        public DateTime Cooldown { get; set; } = DateTime.MinValue;

        private GTANetworkAPI.Object _object { get; set; }
        private ExtColShape _colShape { get; set; }
        public void Spawn()
        {
            _object = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(ModelName), Position, Rotation, 255, 0);
            _colShape = CustomColShape.CreateCylinderColShape(Position, 2, 2, 0, ColShapeEnums.ChristmasEvent_BonusItem, BonusItemsManager.Pool.IndexOf(this));
        }

        public void Destroy()
        {
            Cooldown = DateTime.Now.AddMinutes(Config.COOLDOWN_FOR_REPSAWN_BONUS_ITEMS);

            _object.Delete();
            _object = null;

            CustomColShape.DeleteColShape(_colShape);
            _colShape = null;
        }
    }
}
