using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Pilottask
    {
        public Pilottask()
        {
            Pilottaskquest = new HashSet<Pilottaskquest>();
        }

        public long Pilottaskid { get; set; }
        public long Pilotid { get; set; }
        public int Taskid { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime? Enddate { get; set; }

        public virtual Pilots Pilot { get; set; }
        public virtual Tasks Task { get; set; }
        public virtual ICollection<Pilottaskquest> Pilottaskquest { get; set; }
    }
}
