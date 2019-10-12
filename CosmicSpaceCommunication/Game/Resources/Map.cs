using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Map : ReqLevel
    {
        public bool IsPvp { get; set; }
        public List<Portal> Portals { get; set; }



        public static Map GetMap(DataRow row)
        {
            return new Map()
            {
                Id = ConvertRow.Row<int>(row["mapid"]),
                Name = ConvertRow.Row<string>(row["mapname"]),
                RequiredLevel = ConvertRow.Row<int>(row["requiredlevel"]),

                IsPvp = ConvertRow.Row<bool>(row["ispvp"])
            };
        }
    }
}