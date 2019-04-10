using CosmicSpaceCommunication.Game.Interfaces;
using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Prefab : IPrefab
    {
        public int PrefabId { get; set; }
        public string PrefabName { get; set; }
        public string PrefabTypeName { get; set; }

        public Prefab(DataRow row)
        {
            PrefabId = ConvertRow.Row<int>(row["prefabid"]);
            PrefabName = ConvertRow.Row<string>(row["prefabname"]);
            PrefabTypeName = ConvertRow.Row<string>(row["prefabtypename"]);
        }
    }
}
