using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Quest
{
    [Serializable]
    public class PilotTaskQuest
    {
        public ulong Id { get; set; }
        public decimal Progress { get; set; }
        public bool IsDone { get; set; }

        public Quest Quest { get; set; }



        public static PilotTaskQuest GetPilotTaskQuest(DataRow row)
        {
            return new PilotTaskQuest()
            {
                Id = ConvertRow.Row<ulong>(row["pilottaskquestid"]),
                Progress = ConvertRow.Row<decimal>(row["progress"]),
                IsDone = ConvertRow.Row<int>(row["isdone"]) == 1,
            };
        }
    }
}