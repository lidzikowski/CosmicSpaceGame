using System;
using System.Collections.Generic;

namespace CosmicSpaceWebsiteDll
{
    public partial class Ammunitions
    {
        public Ammunitions()
        {
            EnemiesAmmunition = new HashSet<Enemies>();
            EnemiesRocket = new HashSet<Enemies>();
            PilotsAmmunition = new HashSet<Pilots>();
            PilotsRocket = new HashSet<Pilots>();
            Rewards = new HashSet<Rewards>();
        }

        public int Ammunitionid { get; set; }
        public string Ammunitionname { get; set; }
        public float Multiplierplayer { get; set; }
        public float Multiplierenemy { get; set; }
        public double? Scrapprice { get; set; }
        public double? Metalprice { get; set; }
        public byte? Skillid { get; set; }
        public byte Isammunition { get; set; }
        public int? Basedamage { get; set; }

        public virtual ICollection<Enemies> EnemiesAmmunition { get; set; }
        public virtual ICollection<Enemies> EnemiesRocket { get; set; }
        public virtual ICollection<Pilots> PilotsAmmunition { get; set; }
        public virtual ICollection<Pilots> PilotsRocket { get; set; }
        public virtual ICollection<Rewards> Rewards { get; set; }
    }
}
