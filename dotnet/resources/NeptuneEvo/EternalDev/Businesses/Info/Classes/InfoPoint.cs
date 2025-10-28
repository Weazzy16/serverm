using GTANetworkAPI;     
using NeptuneEvo.Core;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using Newtonsoft.Json;

namespace NeptuneEvo.EternalDev.Businesses.Info.Classes
{
    public class InfoPoint
    {
        public int BusinessId { get; set; }
        public Vector3 Position { get; set; }

        public InfoPoint(int businessId, Vector3 position)
        {
            BusinessId = businessId;
            Position = position;
        }

        private ExtColShape _colShape { get; set; }
        private Marker _marker { get; set; }

        public void GTAElements()
        {
            _colShape = CustomColShape.CreateCylinderColShape(Position, 2, 2, 0, ColShapeEnums.BusinessInfo, BusinessId);
            _marker = NAPI.Marker.CreateMarker(Controller.Config.MARKER_TYPE, Position, new Vector3(), new Vector3(), Controller.Config.MARKER_SCALE, Controller.Config.MARKER_COLOR, false, 0);
        }

        public void Destroy()
        {
            if (_colShape != null)
                CustomColShape.DeleteColShape(_colShape);

            if (_marker != null)
                _marker.Delete();
        }

        public void Interaction(ExtPlayer player)
        {
            if (!BusinessManager.BizList.TryGetValue(BusinessId, out var business)) 
                return;

            var businessInfoData = new
            {
                Id = business.ID,
                Name = BusinessManager.BusinessTypeNames[business.Type],
                Owner = business.Owner,
                Price = business.SellPrice,
                Mafia = business.Mafia == -1 ? "Нет" : Fractions.Manager.GetName(business.Mafia),
            };

            Trigger.ClientEvent(player, "client.businessInfo.open", JsonConvert.SerializeObject(businessInfoData));
        }
    }
}
