using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("to_do_items")]
    public class ToDoItems
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("ListId")]
        [Required]
        public long ListId { get; set; }

        [Column("ToDoItem")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please Enter At Least 3 Characters.")]
        [Required(ErrorMessage = "To Do Item Is Required.")]
        public string ToDoItem { get; set; } = string.Empty;

        [Column("Complete")]
        [Required]
        public bool Complete { get; set; } = false;

        [Column("SortOrder")]
        public int SortOrder { get; set; } = 0;

        [NotMapped]
        public int ListVersion { get; set; } = 0;
    }
}
