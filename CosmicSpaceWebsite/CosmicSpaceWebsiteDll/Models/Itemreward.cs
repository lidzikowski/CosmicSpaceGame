using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Itemreward
    {
        public long Itemrewardid { get; set; }
        public int Rewardid { get; set; }
        public long Itemid { get; set; }
        public int Upgradelevel { get; set; }
        public int Chance { get; set; }

        public virtual Items Item { get; set; }
        public virtual Rewards Reward { get; set; }
    }
}
