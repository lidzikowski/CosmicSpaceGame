using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Player
{
    [Serializable]
    public class PilotResources
    {
        public List<ulong> Ammunitions { get; set; }
        public List<ulong> Rockets { get; set; }

        public static PilotResources GetPilotResources(DataRow row)
        {
            return new PilotResources()
            {
                Ammunitions = new List<ulong>(){
                    ConvertRow.Row<ulong>(row["ammunition0"]),
                    ConvertRow.Row<ulong>(row["ammunition1"]),
                    ConvertRow.Row<ulong>(row["ammunition2"]),
                    ConvertRow.Row<ulong>(row["ammunition3"]),
                },
                Rockets = new List<ulong>(){
                    ConvertRow.Row<ulong>(row["rocket0"]),
                    ConvertRow.Row<ulong>(row["rocket1"]),
                    ConvertRow.Row<ulong>(row["rocket2"]),
                }
            };
        }
    }
}