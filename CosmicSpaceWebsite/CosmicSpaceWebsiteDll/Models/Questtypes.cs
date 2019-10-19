using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Questtypes
    {
        public Questtypes()
        {
            Quests = new HashSet<Quests>();
        }

        public byte Questtypeid { get; set; }
        public string Questtypename { get; set; }

        public virtual ICollection<Quests> Quests { get; set; }
    }
}
