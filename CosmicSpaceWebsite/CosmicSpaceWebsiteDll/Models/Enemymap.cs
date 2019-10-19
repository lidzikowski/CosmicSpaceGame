using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Enemymap
    {
        public int Id { get; set; }
        public int Enemyid { get; set; }
        public int Mapid { get; set; }
        public short Count { get; set; }

        public virtual Enemies Enemy { get; set; }
        public virtual Maps Map { get; set; }
    }
}
