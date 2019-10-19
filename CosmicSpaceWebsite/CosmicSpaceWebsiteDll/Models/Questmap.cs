using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Questmap
    {
        public long Questmapid { get; set; }
        public int Questid { get; set; }
        public int Mapid { get; set; }

        public virtual Maps Map { get; set; }
        public virtual Quests Quest { get; set; }
    }
}
