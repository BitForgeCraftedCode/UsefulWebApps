using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("note_shares")]
    public class NoteShares
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("NoteId")]
        [Required]
        public long NoteId { get; set; }

        [Column("SharedWithUserId")]
        [Required]
        public string SharedWithUserId { get; set; } = string.Empty;
    }
}
