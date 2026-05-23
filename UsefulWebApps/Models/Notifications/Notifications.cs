using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.Notifications
{
    [Table("notifications")]
    public class Notifications
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("UserId")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Column("SenderUserId")]
        public string? SenderUserId { get; set; }

        [Column("Message")]
        [Required]
        public string Message { get; set; } = string.Empty;

        [Column("NotificationType")]
        [StringLength(100)]
        public string? NotificationType { get; set; }

        [Column("RelatedEntityId")]
        public long? RelatedEntityId { get; set; }

        [Column("IsRead")]
        public bool IsRead { get; set; } = false;

        [Column("CreatedAt")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}
