using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Pilotresources
    {
        public int Pilotresourceid { get; set; }
        public long Userid { get; set; }
        public long Ammunition0 { get; set; }
        public long Ammunition1 { get; set; }
        public long Ammunition2 { get; set; }
        public long Ammunition3 { get; set; }
        public long Rocket0 { get; set; }
        public long Rocket1 { get; set; }
        public long Rocket2 { get; set; }

        public virtual Pilots User { get; set; }
    }
}
