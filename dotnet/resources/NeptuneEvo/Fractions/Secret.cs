using System;
using System.Collections.Generic;
using GTANetworkAPI;
using NeptuneEvo.Accounts;
using NeptuneEvo.Character;
using NeptuneEvo.Functions;
using NeptuneEvo.Handles;
using NeptuneEvo.Players;
using Redage.SDK;

namespace NeptuneEVO
{
    public class Secret : Script
    {
        private static readonly nLog Log = new nLog("Secret");

        private static readonly int _minPayment = 600;                                  // Минимальная цена за точку
        private static readonly int _maxPayment = 1000;                                 // Максимальная цена за точку
        private static readonly int _countPoints = 5;                                   // Количество точек за рак
        private static readonly List<Vector3> Positions = new List<Vector3>() // Список точек
        {
            new Vector3(-74.350235, -2414.6733, 4.880091),
            new Vector3(-164.76125, -2419.8171, 4.8799953),
            new Vector3(-206.35855, -2458.647, 5.008034),
            new Vector3(-158.40466, -2524.845, 5.0249224),
            new Vector3(-90.19457, -2529.8967, 4.9722652),
            new Vector3(-804.3763, -2292.9424, 11.192127),
            new Vector3(-1014.1225, -2195.6501, 7.860202),
            new Vector3(-991.3299, -2281.3093, 7.885994),
            new Vector3(-988.4046, -2290.6973, 7.8787565),
            new Vector3(-1309.5118, -1520.4803, 3.2977104),
            new Vector3(-1328.993, -1544.9445, 3.616291),
            new Vector3(-1389.8871, -1321.0023, 3.0301647),
            new Vector3(-1616.9327, -1061.721, 4.2364345),
            new Vector3(-1666.1345, -1012.9481, 6.1757793),
            new Vector3(-1669.4768, -1004.32404, 6.2550697),
            new Vector3(-1630.8428, -969.6561, 6.652419),
            new Vector3(-333.90677, 5.7620893, 46.72124),
            new Vector3(-376.42813, -31.418861, 46.051933),
            new Vector3(-414.18198, -28.556696, 45.549873),
            new Vector3(-1004.7816, 244.83617, 65.75054),
            new Vector3(-1008.2447, 307.5169, 67.450806),
            new Vector3(-1017.881, 326.57935, 67.87123),
            new Vector3(-1360.9349, 635.4453, 134.85071),
            new Vector3(-1349.665, 600.9518, 132.67896),
            new Vector3(-911.9905, 805.75946, 184.0354),
            new Vector3(-925.5782, 795.54724, 182.76866),
            new Vector3(-947.98566, 798.82263, 180.2836),
            new Vector3(-700.77295, 567.95105, 132.9672),
            new Vector3(-767.391, 503.61945, 105.1582),
            new Vector3(-700.85986, 525.9362, 109.84746),
            new Vector3(-457.9045, 463.5242, 108.63056),
            new Vector3(-447.4513, 541.5049, 120.63694),
            new Vector3(-195.44716, 681.3422, 205.07118),
            new Vector3(487.95807, -982.0832, 26.454723),
            new Vector3(510.57254, -978.70087, 26.394884),
            new Vector3(515.48505, -1073.6273, 27.499348),
            new Vector3(486.03818, -1118.8263, 28.171516),
            new Vector3(456.99808, -1118.3108, 28.20546),
            new Vector3(454.7314, -930.82825, 27.363762),
            new Vector3(452.0432, -895.2894, 27.443926),
            new Vector3(453.5694, -877.8312, 26.442745),
            new Vector3(450.40115, -860.0113, 27.013954),
            new Vector3(458.4887, -848.423, 26.873472),
            new Vector3(450.45648, -837.2913, 26.904236),
            new Vector3(477.60098, -845.56287, 25.144583),
            new Vector3(450.60852, -791.49115, 26.237772),
            new Vector3(465.15836, -754.1706, 26.241793),
            new Vector3(496.38196, -631.3433, 23.738203),
            new Vector3(472.64932, -632.5027, 23.838085),
            new Vector3(523.9028, -593.97906, 23.651136),
            new Vector3(547.16925, -633.85693, 24.782032),
            new Vector3(577.47125, -527.2308, 23.630491),
            new Vector3(579.82874, -482.08032, 23.62882),
            new Vector3(637.20605, -454.61145, 23.637018),
            new Vector3(660.0177, -435.25327, 23.60586),
            new Vector3(666.4461, -433.69214, 23.58251),
            new Vector3(2176.738, -756.12744, 72.73231),
            new Vector3(31.25273, 108.141266, 77.89228),
            new Vector3(-110.65406, -1422.9791, 28.86258),
            new Vector3(-76.22114, -1436.4255, 30.834064),
            new Vector3(162.25105, -1907.9156, 20.614882),
            new Vector3(269.62936, -1968.7952, 21.05496),
            new Vector3(323.2136, -1999.6229, 22.158348),
            new Vector3(384.99316, -2046.3684, 21.55265),
            new Vector3(1202.1935, -1615.7024, 47.018826),
            new Vector3(1183.8593, -1662.0817, 37.878166),
            new Vector3(1398.522, -1484.2155, 58.567196),
            new Vector3(1331.797, -1701.9407, 57.090416),
            new Vector3(1272.6758, -1699.9312, 53.525497),
            new Vector3(1055.038, -2025.159, 30.508347),
            new Vector3(1060.3864, -1991.0853, 29.894644),
            new Vector3(1083.3369, -1962.6835, 29.894478),
            new Vector3(1104.8081, -1990.3899, 29.887924),
            new Vector3(1106.6388, -1956.0509, 29.853794),
            new Vector3(1144.5099, -1949.5204, 37.369534),
            new Vector3(1085.1943, -2279.7766, 29.091087),
            new Vector3(1122.5044, -2259.6638, 29.010843),
            new Vector3(1131.3822, -2276.581, 30.805628),
            new Vector3(1184.9733, -2266.547, 29.174574),
            new Vector3(806.9649, -2371.6165, 27.977179),
            new Vector3(706.2763, -2192.0151, 28.064987),
            new Vector3(801.0263, -1686.6874, 29.744385),
            new Vector3(813.6539, -1625.2068, 30.17393),
            new Vector3(788.9206, -1629.7228, 30.044754),
            new Vector3(774.6599, -1595.2018, 30.183983),
            new Vector3(484.10477, -1963.891, 23.706507),
            new Vector3(488.60577, -2006.552, 23.646334),
            new Vector3(-50.658585, -2440.3186, 4.881611),
        };  

        private static readonly int _checkpointConstant = 888;  
        private static readonly string _dataPoints = "Secret_points";
        private static readonly string _dataPoint = "Secret_point";
        private static List<Point> Points = new List<Point>();
        private static int _payment = 0;
        public static void Initialize()
        {
            try
            {
                int type = 0;
                foreach (Vector3 position in Positions)
                {
                    Point point = new Point()
                    {
                        ID = type,
                        Position = position
                    };
                    CustomColShape.CreateCylinderColShape(position, 2, 2, 0, ColShapeEnums.SecretCheck, type, type);
                    type++;

                    Points.Add(point);
                }
                ChangePayment();
            }
            catch(Exception ex) { Log.Write("Initialize: " + ex.ToString()); }
        }

        public static void ChangePayment()
        {
            _payment = new Random().Next(_minPayment, _maxPayment);
            Log.Write($"Новая цена контробанды: {_payment}$");
        }

        public static void Start(ExtPlayer player)
        {
            try
            {
                var sessionData = player.GetSessionData();
                if (sessionData == null) return;
                var accountData = player.GetAccountData();
                if (accountData == null) return;
                var characterData = player.GetCharacterData();
                if (characterData == null) return; 

                if (player.HasData(_dataPoints))
                {
                    Notify.Send(player, NotifyType.Error, NotifyPosition.BottomCenter, "Вы уже взяли контракт", 3000);
                    return;
                }

                var data = GetPoints();
                if (data is null) return;

                foreach(Point point in data)
                {
                    Trigger.ClientEvent(player, "createCheckpoint", _checkpointConstant + point.ID, 1, point.Position, 1, 0, 77, 205, 255);
                    Trigger.ClientEvent(player, "createBlip", _checkpointConstant + point.ID, "Контрабанда", 0, point.Position, 1);
                }

                player.SetData(_dataPoints, data);
                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы взяли контракт на развозку контрабанды", 3000);
            }
            catch(Exception ex) { Log.Write("Start: " + ex.ToString()); }
        }

        [Interaction(ColShapeEnums.SecretCheck, In: true)]
        public static void Interaction(ExtPlayer player, int Index, int ListId)
        {
            try
            {
                if (!player.HasData(_dataPoints)) return;
                var point = Points[Index];
                var data = player.GetData<List<Point>>(_dataPoints);
                if (!data.Contains(point)) return;
                    point.Interaction(player);
            }
            catch(Exception ex) { Log.Write("Interaction: " + ex.ToString()); }
        }

        public static List<Point> GetPoints()
        {
            try
            {
                if (Points.Count < _countPoints) return null;

                var list = new List<Point>();
                var random = new Random();

                var item = Points[random.Next(0, Points.Count)];
                while (list.Count < _countPoints)
                {
                    item = Points[random.Next(0, Points.Count)];
                    if (!list.Contains(item))
                        list.Add(item);
                }

                return list;
            }
            catch(Exception ex) { Log.Write("GetPoints: " + ex.ToString()); return null; }
        }

        public class Point
        {
            public int ID;
            public Vector3 Position;

            public void Interaction(ExtPlayer player)
            {
                try
                {
                    if (!player.HasData(_dataPoints)) return;

                    var data = player.GetData<List<Point>>(_dataPoints);
                    if (!data.Contains(this)) return; 
                    
                    player.PlayAnimation("anim@heists@money_grab@briefcase", "put_down_case", 1);
                    NAPI.Task.Run(() =>
                    {
                        try
                        {
                            if (player is null) return;

                            player.StopAnimation();
                            data.Remove(this);

                            player.SetData(_dataPoints, data);
                            Trigger.ClientEvent(player, "deleteCheckpoint", _checkpointConstant + ID);
                            Trigger.ClientEvent(player, "deleteBlip", _checkpointConstant + ID);

                            NeptuneEvo.MoneySystem.Wallet.Change(player, _payment);

                            if (data.Count <= 0)
                            {
                                player.ResetData(_dataPoints);
                                Notify.Send(player, NotifyType.Success, NotifyPosition.BottomCenter, "Вы развезли контрабанду по всем точкам", 3000);
                                return;
                            }
                            Notify.Send(player, NotifyType.Info, NotifyPosition.BottomCenter, $"Осталось {data.Count} точек", 3000);
                        }
                        catch(Exception ex) { Log.Write("Task.Run - Interaction: " + ex.ToString()); }
                    }, 1700);
                }
                catch(Exception ex) { Log.Write("Interaction: " + ex.ToString()); }
            }
        }
    }
}