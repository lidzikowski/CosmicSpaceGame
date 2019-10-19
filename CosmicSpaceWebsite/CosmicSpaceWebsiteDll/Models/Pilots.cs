using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Pilots
    {
        public Pilots()
        {
            Pilotresources = new HashSet<Pilotresources>();
            Pilotsitems = new HashSet<Pilotsitems>();
            Pilottask = new HashSet<Pilottask>();
        }

        public long Pilotid { get; set; }
        public long Userid { get; set; }
        public string Nickname { get; set; }
        public int Mapid { get; set; }
        public float Positionx { get; set; }
        public float Positiony { get; set; }
        public int Shipid { get; set; }
        public long Experience { get; set; }
        public int Level { get; set; }
        public double Scrap { get; set; }
        public double Metal { get; set; }
        public long Hitpoints { get; set; }
        public long Shields { get; set; }
        public int Ammunitionid { get; set; }
        public int Rocketid { get; set; }
        public byte Isdead { get; set; }
        public string Killerby { get; set; }
        public string Achievement { get; set; }

        public virtual Ammunitions Ammunition { get; set; }
        public virtual Maps Map { get; set; }
        public virtual Ammunitions Rocket { get; set; }
        public virtual Ships Ship { get; set; }
        public virtual Users User { get; set; }
        public virtual ICollection<Pilotresources> Pilotresources { get; set; }
        public virtual ICollection<Pilotsitems> Pilotsitems { get; set; }
        public virtual ICollection<Pilottask> Pilottask { get; set; }
    }
}
