using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Reward
    {
        public ulong Id { get; set; }

        public ulong? Experience { get; set; }
        public double? Metal { get; set; }
        public double? Scrap { get; set; }
        public int? AmmunitionId { get; set; }
        public long? AmmunitionQuantity { get; set; }

        public List<ItemReward> Items { get; set; }



        public static Reward GetReward(DataRow row)
        {
            return new Reward()
            {
                Id = ConvertRow.Row<ulong>(row["rewardid"]),
                Experience = ConvertRow.Row<ulong?>(row["experience"]),
                Metal = ConvertRow.Row<double?>(row["metal"]),
                Scrap = ConvertRow.Row<double?>(row["scrap"]),

                AmmunitionId = ConvertRow.Row<int?>(row["ammunitionid"]),
                AmmunitionQuantity = ConvertRow.Row<long?>(row["ammunition_quantity"]),

                Items = new List<ItemReward>()
            };
        }
    }
}