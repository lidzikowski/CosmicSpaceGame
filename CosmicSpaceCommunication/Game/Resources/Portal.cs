using CosmicSpaceCommunication.Game.Interfaces;
using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Portal : IPrefab
    {
        public int Id { get; set; }
        public int PrefabId { get; set; }
        public string PrefabName { get; set; }
        public string PrefabTypeName { get; set; }

        public int MapId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public Map Map { get; set; }

        public int TargetMapId { get; set; }
        public float TargetPositionX { get; set; }
        public float TargetPositionY { get; set; }
        public Map TargetMap { get; set; }


        public int RequiredLevel { get; set; }
        
        public static Portal GetPortal(DataRow row)
        {
            float portaldistance = ConvertRow.Row<float>(row["portaldistance"]);
            float portalborder = ConvertRow.Row<float>(row["portalborder"]);
            return new Portal()
            {
                Id = ConvertRow.Row<int>(row["portalid"]),
                PrefabId = ConvertRow.Row<int>(row["prefabid"]),
                PrefabName = ConvertRow.Row<string>(row["prefabname"]),
                PrefabTypeName = ConvertRow.Row<string>(row["prefabtypename"]),

                MapId = ConvertRow.Row<int>(row["mapid"]),
                PositionX = ConvertRow.Row<float>(row["positionx"]) * portaldistance + portalborder,
                PositionY = -(ConvertRow.Row<float>(row["positiony"]) * portaldistance + portalborder),

                TargetMapId = ConvertRow.Row<int>(row["target_mapid"]),
                TargetPositionX = ConvertRow.Row<float>(row["target_positionx"]) * portaldistance + portalborder,
                TargetPositionY = -(ConvertRow.Row<float>(row["target_positiony"]) * portaldistance + portalborder),

                RequiredLevel = ConvertRow.Row<int>(row["requiredlevel"]),
            };
        }
    }
}