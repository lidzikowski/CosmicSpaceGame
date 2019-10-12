using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Quest
{
    [Serializable]
    public class PilotTask
    {
        public uint Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public QuestTask Task { get; set; }
        public List<PilotTaskQuest> TaskQuest { get; set; }



        public static PilotTask GetPilotTask(DataRow row)
        {
            return new PilotTask()
            {
                Id = ConvertRow.Row<uint>(row["pilottaskid"]),
                Start = ConvertRow.Row<DateTime>(row["startdate"]),
                End = ConvertRow.Row<DateTime?>(row["enddate"]),
            };
        }
    }
}