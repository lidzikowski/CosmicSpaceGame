using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Ships
    {
        public Ships()
        {
            Pilots = new HashSet<Pilots>();
        }

        public int Shipid { get; set; }
        public string Shipname { get; set; }
        public int Prefabid { get; set; }
        public int Requiredlevel { get; set; }
        public double? Scrapprice { get; set; }
        public double? Metalprice { get; set; }
        public byte Lasers { get; set; }
        public byte Generators { get; set; }
        public byte Extras { get; set; }
        public short Speed { get; set; }
        public short Cargo { get; set; }
        public long Hitpoints { get; set; }
        public int Rewardid { get; set; }

        public virtual Prefabs Prefab { get; set; }
        public virtual Rewards Reward { get; set; }
        public virtual ICollection<Pilots> Pilots { get; set; }
    }
}
