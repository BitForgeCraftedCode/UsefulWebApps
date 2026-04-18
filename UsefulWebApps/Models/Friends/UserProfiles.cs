using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.Friends
{
    [Table("user_profiles")]
    public class UserProfiles
    {
        [Key]
        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Column("DisplayName")]
        [StringLength(100)]
        public string? DisplayName { get; set; }

        [Column("AvatarPath")]
        [StringLength(500)]
        public string? AvatarPath { get; set; }

        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
