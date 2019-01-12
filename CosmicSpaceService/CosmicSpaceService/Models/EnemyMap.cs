using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("EnemiesMaps")]
    public partial class EnemyMap
    {
        [Required]
        public int Count { get; set; }

        [Required]
        [ForeignKey("Map")]
        public int MapId { get; set; }
        public virtual Map Map { get; set; }

        [Required]
        [ForeignKey("Enemy")]
        public int EnemyId { get; set; }
        public virtual Enemy Enemy { get; set; }
    }
}