using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Quest
{
    [Serializable]
    public class Quest
    {
        public uint Id { get; set; }
        public QuestTypes QuestType { get; set; }
        public ulong TargetId { get; set; }
        public ulong Quantity { get; set; }

        public List<ulong> Maps { get; set; }



        public static Quest GetQuest(DataRow row)
        {
            return new Quest()
            {
                Id = ConvertRow.Row<uint>(row["questid"]),
                QuestType = (QuestTypes)ConvertRow.Row<uint>(row["questtypeid"]),
                TargetId = ConvertRow.Row<ulong>(row["targetid"]),
                Quantity = ConvertRow.Row<ulong>(row["quantity"]),
            };
        }
    }
}