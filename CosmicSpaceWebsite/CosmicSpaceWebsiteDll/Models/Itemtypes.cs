using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Itemtypes
    {
        public Itemtypes()
        {
            Items = new HashSet<Items>();
        }

        public int Itemtypeid { get; set; }
        public string Itemtypename { get; set; }

        public virtual ICollection<Items> Items { get; set; }
    }
}
