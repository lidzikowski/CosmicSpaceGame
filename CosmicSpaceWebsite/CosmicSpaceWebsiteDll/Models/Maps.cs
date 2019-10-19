using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Maps
    {
        public Maps()
        {
            Enemymap = new HashSet<Enemymap>();
            Pilots = new HashSet<Pilots>();
            PortalsMap = new HashSet<Portals>();
            PortalsTargetMap = new HashSet<Portals>();
            Questmap = new HashSet<Questmap>();
        }

        public int Mapid { get; set; }
        public string Mapname { get; set; }
        public byte Ispvp { get; set; }
        public int Requiredlevel { get; set; }

        public virtual ICollection<Enemymap> Enemymap { get; set; }
        public virtual ICollection<Pilots> Pilots { get; set; }
        public virtual ICollection<Portals> PortalsMap { get; set; }
        public virtual ICollection<Portals> PortalsTargetMap { get; set; }
        public virtual ICollection<Questmap> Questmap { get; set; }
    }
}
