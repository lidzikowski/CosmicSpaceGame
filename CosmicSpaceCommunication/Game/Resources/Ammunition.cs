using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Ammunition : Base
    {
        public float? MultiplierPlayer { get; set; }
        public float? MultiplierEnemy { get; set; }
        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public int? SkillId { get; set; }

        public static Ammunition GetAmmunition(DataRow row)
        {
            return new Ammunition()
            {
                Id = ConvertRow.Row<int>(row["ammunitionid"]),
                Name = ConvertRow.Row<string>(row["ammunitionname"]),

                MultiplierPlayer = ConvertRow.Row<float?>(row["multiplierplayer"]),
                MultiplierEnemy = ConvertRow.Row<float?>(row["multiplierenemy"]),
                ScrapPrice = ConvertRow.Row<double?>(row["scrapprice"]),
                MetalPrice = ConvertRow.Row<double?>(row["metalprice"]),
                SkillId = ConvertRow.Row<int?>(row["skillid"]),
            };
        }
    }
}