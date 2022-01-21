using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Quest
{
    [Serializable]
    public class QuestTask
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint Level { get; set; }
        public Reward Reward { get; set; }

        public List<Quest> Quests { get; set; }



        public static QuestTask GetTask(DataRow row)
        {
            return new QuestTask()
            {
                Id = ConvertRow.Row<uint>(row["taskid"]),
                Name = ConvertRow.Row<string>(row["taskname"]),
                Level = ConvertRow.Row<uint>(row["level"]),
                Reward = Reward.GetReward(row),
            };
        }
    }
}