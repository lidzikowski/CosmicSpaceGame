using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Player
{
    [Serializable]
    public class PilotResource
    {
        public int Index { get; set; }
        public string ColumnName { get; set; }
        public ulong Count { get; set; }

        public static List<PilotResource> GetPilotResource(DataRow row)
        {
            List<PilotResource> pilotResources = new List<PilotResource>();
            for (int i = 1; i < row.Table.Columns.Count; i++)
            {
                pilotResources.Add(new PilotResource()
                {
                    Index = i,
                    ColumnName = row.Table.Columns[i].ColumnName,
                    Count = ConvertRow.Row<ulong>(row[row.Table.Columns[i].ColumnName]),
                });
            }
            return pilotResources;
        }
    }
}