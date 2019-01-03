using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [System.Serializable]
    public class Ship : ReqLevel
    {
        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public int Lasers { get; set; }
        public int Generators { get; set; }
        public int Extras { get; set; }
        public int Speed { get; set; }
        public int Cargo { get; set; }
        public ulong Hitpoints { get; set; }
        public Reward Reward { get; set; }

        public static Ship GetShip(DataRow row)
        {
            return new Ship()
            {
                Id = ConvertRow.Row<int>(row["shipid"]),
                Name = ConvertRow.Row<string>(row["shipname"]),
                RequiredLevel = ConvertRow.Row<int>(row["requiredlevel"]),

                ScrapPrice = ConvertRow.Row<double?>(row["scrapprice"]),
                MetalPrice = ConvertRow.Row<double?>(row["metalprice"]),
                Lasers = ConvertRow.Row<int>(row["lasers"]),
                Generators = ConvertRow.Row<int>(row["generators"]),
                Extras = ConvertRow.Row<int>(row["extras"]),
                Speed = ConvertRow.Row<int>(row["speed"]),
                Cargo = ConvertRow.Row<int>(row["cargo"]),
                Hitpoints = ConvertRow.Row<ulong>(row["hitpoints"]),
                Reward = Reward.GetReward(row),
            };
        }
    }
}