using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Pilotsitems
    {
        public long Relationid { get; set; }
        public long Userid { get; set; }
        public long Itemid { get; set; }
        public int Upgradelevel { get; set; }
        public byte Isequipped { get; set; }
        public byte Issold { get; set; }

        public virtual Items Item { get; set; }
        public virtual Pilots User { get; set; }
    }
}
