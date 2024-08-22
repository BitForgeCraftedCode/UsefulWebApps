using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.Models.ListBuddy
{
    public class ToDoList
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "To do item is required.")]
        public string ToDoItem { get; set; }
        [Required]
        public bool Complete { get; set; }
    }
}
