using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Quests
    {
        public Quests()
        {
            Pilottaskquest = new HashSet<Pilottaskquest>();
            Questmap = new HashSet<Questmap>();
            Taskquest = new HashSet<Taskquest>();
        }

        public int Questid { get; set; }
        public byte Questtypeid { get; set; }
        public long Targetid { get; set; }
        public long Quantity { get; set; }

        public virtual Questtypes Questtype { get; set; }
        public virtual ICollection<Pilottaskquest> Pilottaskquest { get; set; }
        public virtual ICollection<Questmap> Questmap { get; set; }
        public virtual ICollection<Taskquest> Taskquest { get; set; }
    }
}
