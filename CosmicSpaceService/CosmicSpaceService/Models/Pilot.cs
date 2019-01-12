using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("Pilots")]
    public partial class Pilot
    {
        [Required]
        [MaxLength(30)]
        public string Nickname { get; set; }
        
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public long Experience { get; set; }
        public int Level { get; set; }
        public double Scrap { get; set; }
        public double Metal { get; set; }
        public long Hitpoints { get; set; }
        public long Shields { get; set; }
        public bool IsDead { get; set; }

        [MaxLength(30)]
        public string KillBy { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Map")]
        public int MapId { get; set; }
        public virtual Map Map { get; set; }

        [ForeignKey("Ship")]
        public int ShipId { get; set; }
        public virtual Ship Ship { get; set; }
        
        public virtual List<AmmunitionPilot> Ammunitions { get; set; }
    }
}