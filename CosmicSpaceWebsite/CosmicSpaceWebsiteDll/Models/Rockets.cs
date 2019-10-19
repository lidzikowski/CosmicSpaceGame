using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Rockets
    {
        public int Rocketid { get; set; }
        public string Rocketname { get; set; }
        public double? Scrapprice { get; set; }
        public double? Metalprice { get; set; }
        public byte? Skillid { get; set; }
        public int Damage { get; set; }
    }
}
