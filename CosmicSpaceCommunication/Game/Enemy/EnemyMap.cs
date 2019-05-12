using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Enemy
{
    [Serializable]
    public class EnemyMap
    {
        public int Id { get; set; }
        public int EnemyId { get; set; }
        public EnemyTypes EnemyType{ get; set; }
        public int MapId { get; set; }
        public int Count { get; set; }

        public static EnemyMap GetEnemyMap(DataRow row)
        {
            return new EnemyMap()
            {
                Id = ConvertRow.Row<int>(row["id"]),
                EnemyId = ConvertRow.Row<int>(row["enemyid"]),
                EnemyType = (EnemyTypes)ConvertRow.Row<int>(row["enemytypeid"]),
                MapId = ConvertRow.Row<int>(row["mapid"]),
                Count = ConvertRow.Row<int>(row["count"]),
            };
        }
    }
}