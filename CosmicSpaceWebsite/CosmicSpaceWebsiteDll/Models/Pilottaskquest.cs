using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Pilottaskquest
    {
        public long Pilottaskquestid { get; set; }
        public long Pilottaskid { get; set; }
        public int Questid { get; set; }
        public long Progress { get; set; }
        public byte Isdone { get; set; }

        public virtual Pilottask Pilottask { get; set; }
        public virtual Quests Quest { get; set; }
    }
}
