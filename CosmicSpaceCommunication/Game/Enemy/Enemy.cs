using CosmicSpaceCommunication.Game.Interfaces;
using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Enemy
{
    [Serializable]
    public class Enemy : IShip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long Hitpoints { get; set; }
        public long Shields { get; set; }
        public int Speed { get; set; }
        public long Damage { get; set; }
        public int ShotDistance { get; set; }
        public bool IsAggressive { get; set; }

        public Reward Reward { get; set; }
        public Prefab Prefab { get; set; }

        public static Enemy GetEnemy(DataRow row)
        {
            return new Enemy()
            {
                Id = ConvertRow.Row<int>(row["enemyid"]),
                Name = ConvertRow.Row<string>(row["enemyname"]),
                Hitpoints = ConvertRow.Row<long>(row["hitpoints"]),
                Shields = ConvertRow.Row<long>(row["shields"]),
                Speed = ConvertRow.Row<int>(row["speed"]),
                Damage = ConvertRow.Row<long>(row["damage"]),
                ShotDistance = ConvertRow.Row<int>(row["shotdistance"]),
                IsAggressive = ConvertRow.Row<bool>(row["isaggressive"]),
                Reward = Reward.GetReward(row),
                Prefab = new Prefab(row),
            };
        }
    }
}