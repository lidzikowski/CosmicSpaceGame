using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Prefabs
    {
        public Prefabs()
        {
            Enemies = new HashSet<Enemies>();
            Portals = new HashSet<Portals>();
            Ships = new HashSet<Ships>();
        }

        public int Prefabid { get; set; }
        public string Prefabname { get; set; }
        public int Prefabtypeid { get; set; }
        public string Description { get; set; }

        public virtual PrefabsTypes Prefabtype { get; set; }
        public virtual ICollection<Enemies> Enemies { get; set; }
        public virtual ICollection<Portals> Portals { get; set; }
        public virtual ICollection<Ships> Ships { get; set; }
    }
}
