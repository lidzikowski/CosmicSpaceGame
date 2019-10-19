using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Tasks
    {
        public Tasks()
        {
            Pilottask = new HashSet<Pilottask>();
            Taskquest = new HashSet<Taskquest>();
        }

        public int Taskid { get; set; }
        public string Taskname { get; set; }
        public int Level { get; set; }
        public int Rewardid { get; set; }

        public virtual Rewards Reward { get; set; }
        public virtual ICollection<Pilottask> Pilottask { get; set; }
        public virtual ICollection<Taskquest> Taskquest { get; set; }
    }
}
