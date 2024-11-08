using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("notes")]
    public class Notes
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Note")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Please Enter At Least 10 Characters.")]
        [Required(ErrorMessage = "Note Is Required.")]
        public string Note { get; set; } = string.Empty;

        [Column("UserId")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Column("NoteTitle")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please Enter At Least 3 Characters.")]
        [Required(ErrorMessage = "Note Title Is Required.")]
        public string NoteTitle {  get; set; } = string.Empty;
    }
}

