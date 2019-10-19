using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Items
    {
        public Items()
        {
            Itemreward = new HashSet<Itemreward>();
            Pilotsitems = new HashSet<Pilotsitems>();
        }

        public long Itemid { get; set; }
        public string Name { get; set; }
        public string Prefabname { get; set; }
        public int Itemtypeid { get; set; }
        public int Requiredlevel { get; set; }
        public long? LaserDamagePvp { get; set; }
        public long? LaserDamagePve { get; set; }
        public int? LaserShotrange { get; set; }
        public float? LaserShotdispersion { get; set; }
        public float? GeneratorSpeed { get; set; }
        public long? GeneratorShield { get; set; }
        public float? GeneratorShieldDivision { get; set; }
        public int? GeneratorShieldRepair { get; set; }
        public double? Scrapprice { get; set; }
        public double? Metalprice { get; set; }

        public virtual Itemtypes Itemtype { get; set; }
        public virtual ICollection<Itemreward> Itemreward { get; set; }
        public virtual ICollection<Pilotsitems> Pilotsitems { get; set; }
    }
}
