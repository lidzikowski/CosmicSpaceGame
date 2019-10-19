using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Taskquest
    {
        public long Taskquestid { get; set; }
        public int Taskid { get; set; }
        public int Questid { get; set; }

        public virtual Quests Quest { get; set; }
        public virtual Tasks Task { get; set; }
    }
}
