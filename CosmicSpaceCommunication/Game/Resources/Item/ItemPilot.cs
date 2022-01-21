using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class ItemPilot
    {
        public ulong RelationId { get; set; }
        public ulong PilotId { get; set; }
        public long ItemId { get; set; }
        public Item Item { get; set; }
        public int UpgradeLevel { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsSold { get; set; }



        public static ItemPilot GetItemPilot(DataRow row)
        {
            return new ItemPilot()
            {
                RelationId = ConvertRow.Row<ulong>(row["relationid"]),
                PilotId = ConvertRow.Row<ulong>(row["userid"]),
                ItemId = ConvertRow.Row<long>(row["itemid"]),
                UpgradeLevel = ConvertRow.Row<int>(row["upgradelevel"]),
                IsEquipped = ConvertRow.Row<bool>(row["isequipped"]),
                IsSold = ConvertRow.Row<bool>(row["issold"]),
            };
        }
    }
}