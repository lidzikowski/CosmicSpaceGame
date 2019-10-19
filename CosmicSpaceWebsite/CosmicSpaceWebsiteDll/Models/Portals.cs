using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Portals
    {
        public int Portalid { get; set; }
        public int Prefabid { get; set; }
        public int Mapid { get; set; }
        public int? Portalpositionid { get; set; }
        public int TargetMapid { get; set; }
        public int TargetPortalpositionid { get; set; }

        public virtual Maps Map { get; set; }
        public virtual Portalpositions Portalposition { get; set; }
        public virtual Prefabs Prefab { get; set; }
        public virtual Maps TargetMap { get; set; }
        public virtual Portalpositions TargetPortalposition { get; set; }
    }
}
