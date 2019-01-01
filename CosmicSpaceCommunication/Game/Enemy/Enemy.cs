using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Enemy
{
    [Serializable]
    public class Enemy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong Hitpoints { get; set; }
        public ulong Shields { get; set; }
        public int Speed { get; set; }
        public ulong Damage { get; set; }

        public Reward Reward { get; set; }

        public static Enemy GetEnemy(DataRow row)
        {
            return new Enemy()
            {
                Id = ConvertRow.Row<int>(row["enemyid"]),
                Name = ConvertRow.Row<string>(row["enemyname"]),
                Hitpoints = ConvertRow.Row<ulong>(row["hitpoints"]),
                Shields = ConvertRow.Row<ulong>(row["shields"]),
                Speed = ConvertRow.Row<int>(row["speed"]),
                Damage = ConvertRow.Row<ulong>(row["damage"]),
                Reward = Reward.GetReward(row)
            };
        }
    }
}