using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Portalpositions
    {
        public Portalpositions()
        {
            PortalsPortalposition = new HashSet<Portals>();
            PortalsTargetPortalposition = new HashSet<Portals>();
        }

        public int Portalpositionid { get; set; }
        public string Name { get; set; }
        public float Positionx { get; set; }
        public float Positiony { get; set; }

        public virtual ICollection<Portals> PortalsPortalposition { get; set; }
        public virtual ICollection<Portals> PortalsTargetPortalposition { get; set; }
    }
}
