using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("Maps")]
    public partial class Map
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MapId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public bool IsPvp { get; set; }

        [Required]
        public int RequiredLevel { get; set; }

        public virtual List<EnemyMap> EnemiesMaps { get; set; }
    }
}