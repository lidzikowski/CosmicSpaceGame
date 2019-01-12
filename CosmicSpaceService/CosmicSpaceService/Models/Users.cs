using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmicSpaceService.Models
{
    [Serializable]
    [Table("Users")]
    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        [Required]
        [MaxLength(128)]
        public string Username { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; }

        [Required]
        [MaxLength(128)]
        public string Email { get; set; }

        public bool EmailNewsletter { get; set; }
        public bool AcceptRules { get; set; }
        public bool Ban { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
        
        public virtual Pilot Pilot { get; set; }
    }
}