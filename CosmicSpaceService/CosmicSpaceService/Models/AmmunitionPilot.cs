using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("AmmunitionsPilots")]
    public partial class AmmunitionPilot
    {
        [Required]
        public long Count { get; set; }

        [Required]
        [ForeignKey("Ammunition")]
        public int AmmunitionId { get; set; }
        public virtual Ammunition Ammunition { get; set; }

        [Required]
        [ForeignKey("Pilot")]
        public long PilotId { get; set; }
        public virtual Pilot Pilot { get; set; }
    }
}