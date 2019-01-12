using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("Ships")]
    public partial class Ship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShipId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public int RequiredLevel { get; set; }

        [Required]
        public double ScrapPrice { get; set; }

        [Required]
        public double MetalPrice { get; set; }

        [Required]
        public int Lasers { get; set; }

        [Required]
        public int Generators { get; set; }

        [Required]
        public int Extras { get; set; }

        [Required]
        public int BasicSpeed { get; set; }

        [Required]
        public int BasicCargo { get; set; }

        [Required]
        public long BasicHitpoints { get; set; }

        [Required]
        [ForeignKey("Reward")]
        public int RewardId { get; set; }
        public virtual Reward Reward { get; set; }
    }
}