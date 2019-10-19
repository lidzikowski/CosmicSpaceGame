using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class PrefabsTypes
    {
        public PrefabsTypes()
        {
            Prefabs = new HashSet<Prefabs>();
        }

        public int Prefabtypeid { get; set; }
        public string Prefabtypename { get; set; }

        public virtual ICollection<Prefabs> Prefabs { get; set; }
    }
}
