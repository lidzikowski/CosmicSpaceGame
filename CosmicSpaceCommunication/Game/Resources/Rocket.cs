using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Rocket : Base
    {
        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public int? SkillId { get; set; }
        public int Damage { get; set; }

        public static Rocket GetRocket(DataRow row)
        {
            return new Rocket()
            {
                Id = ConvertRow.Row<int>(row["rocketid"]),
                Name = ConvertRow.Row<string>(row["rocketname"]),

                ScrapPrice = ConvertRow.Row<double?>(row["scrapprice"]),
                MetalPrice = ConvertRow.Row<double?>(row["metalprice"]),
                SkillId = ConvertRow.Row<int?>(row["skillid"]),
                Damage = ConvertRow.Row<int>(row["damage"]),
            };
        }
    }
}