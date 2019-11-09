using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Quest
{
    [Serializable]
    public class PilotProgressTask
    {
        public uint Id { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }



        public static PilotProgressTask GetPilotProgressTask(DataRow row)
        {
            return new PilotProgressTask()
            {
                Id = ConvertRow.Row<uint>(row["taskid"]),
                Start = ConvertRow.Row<DateTime?>(row["startdate"]),
                End = ConvertRow.Row<DateTime?>(row["enddate"]),
            };
        }
    }
}