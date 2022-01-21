using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class ItemReward
    {
        public long ItemRewardId { get; set; }
        public uint RewardId { get; set; }
        public long ItemId { get; set; }
        public Item Item { get; set; }
        public int UpgradeLevel { get; set; }
        public float Chance { get; set; }



        public static ItemReward GetItemReward(DataRow row)
        {
            return new ItemReward()
            {
                ItemRewardId = ConvertRow.Row<long>(row["itemrewardid"]),
                RewardId = ConvertRow.Row<uint>(row["rewardid"]),
                ItemId = ConvertRow.Row<long>(row["itemid"]),
                UpgradeLevel = ConvertRow.Row<int>(row["upgradelevel"]),
                Chance = ConvertRow.Row<float>(row["chance"]),
            };
        }
    }
}