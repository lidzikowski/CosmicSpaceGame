using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("Ammunitions")]
    public partial class Ammunition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AmmunitionId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public double MultiplierPilot { get; set; }

        [Required]
        public double MultiplierEnemy { get; set; }
        
        public double ScrapPrice { get; set; }
        
        public double MetalPrice { get; set; }

        //public int SkillId { get; set; }

        [ForeignKey("AmmunitionId")]
        public virtual AmmunitionPilot AmmunitionsPilots { get; set; }
    }
}