using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("to_do_list")]
    public class ToDoList
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ToDoItem")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please Enter At Least 3 Characters.")]
        [Required(ErrorMessage = "To Do Item Is Required.")]
        public string ToDoItem { get; set; } = string.Empty;

        [Column("Complete")]
        [Required]
        public bool Complete { get; set; }

        [Column("UserId")]
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
