using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("notes")]
    public class Notes
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("Note")]
        [StringLength(50000, MinimumLength = 20, ErrorMessage = "Please Enter At Least 20 Characters.")]
        [Required(ErrorMessage = "Note Is Required.")]
        public string Note { get; set; } = string.Empty;

        [Column("UserId")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Column("NoteTitle")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please Enter At Least 3 Characters.")]
        [Required(ErrorMessage = "Note Title Is Required.")]
        public string NoteTitle {  get; set; } = string.Empty;

        [Column("Version")]
        public int Version { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}

