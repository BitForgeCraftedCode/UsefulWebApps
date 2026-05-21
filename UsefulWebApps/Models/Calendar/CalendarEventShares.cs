using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.Calendar
{
    [Table("calendar_event_shares")]
    public class CalendarEventShares
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("CalendarEventId")]
        [Required]
        public long CalendarEventId { get; set; }

        [Column("SharedWithUserId")]
        [Required]
        public string SharedWithUserId { get; set; } = string.Empty;
    }
}
