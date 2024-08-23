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
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "To do item is required.")]
        public string ToDoItem { get; set; }

        [Column("Complete")]
        [Required]
        public bool Complete { get; set; }
    }
}
