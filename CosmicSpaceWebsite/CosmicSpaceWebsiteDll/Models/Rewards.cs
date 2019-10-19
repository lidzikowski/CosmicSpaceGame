using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Rewards
    {
        public Rewards()
        {
            Enemies = new HashSet<Enemies>();
            Itemreward = new HashSet<Itemreward>();
            Ships = new HashSet<Ships>();
            Tasks = new HashSet<Tasks>();
        }

        public int Rewardid { get; set; }
        public long? Experience { get; set; }
        public double? Metal { get; set; }
        public double? Scrap { get; set; }
        public int? Ammunitionid { get; set; }
        public long? AmmunitionQuantity { get; set; }

        public virtual Ammunitions Ammunition { get; set; }
        public virtual ICollection<Enemies> Enemies { get; set; }
        public virtual ICollection<Itemreward> Itemreward { get; set; }
        public virtual ICollection<Ships> Ships { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
    }
}
