using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("Rewards")]
    public partial class Reward
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RewardId { get; set; }
        
        public long Experience { get; set; }
        
        public double Metal { get; set; }
        
        public double Scrap { get; set; }
    }
}