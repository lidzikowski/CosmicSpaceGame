using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("Enemies")]
    public partial class Enemy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EnemyId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        
        [Required]
        public long BasicHitpoints { get; set; }

        [Required]
        public long BasicShields { get; set; }

        [Required]
        public int BasicSpeed { get; set; }

        [Required]
        public long BasicDamage { get; set; }

        [Required]
        public int BasicShotDistance { get; set; }

        [Required]
        public bool IsAggressive { get; set; }

        [Required]
        [ForeignKey("Reward")]
        public int RewardId { get; set; }
        public virtual Reward Reward { get; set; }

        public virtual List<EnemyMap> EnemiesMaps { get; set; }
    }
}