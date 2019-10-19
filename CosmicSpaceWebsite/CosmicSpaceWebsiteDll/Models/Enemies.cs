using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Enemies
    {
        public Enemies()
        {
            Enemymap = new HashSet<Enemymap>();
        }

        public int Enemyid { get; set; }
        public string Enemyname { get; set; }
        public int Prefabid { get; set; }
        public long Hitpoints { get; set; }
        public long Shields { get; set; }
        public short Speed { get; set; }
        public long Damage { get; set; }
        public int Shotdistance { get; set; }
        public byte Isaggressive { get; set; }
        public int Rewardid { get; set; }
        public int? Ammunitionid { get; set; }
        public int? Rocketid { get; set; }

        public virtual Ammunitions Ammunition { get; set; }
        public virtual Prefabs Prefab { get; set; }
        public virtual Rewards Reward { get; set; }
        public virtual Ammunitions Rocket { get; set; }
        public virtual ICollection<Enemymap> Enemymap { get; set; }
    }
}
