using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.Models.ListBuddy
{
    public class GroceryList
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Grocery item is required.")]
        public string GroceryItem { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Grocery category is required.")]
        public string Category { get; set; }
        [Required]
        public bool Complete { get; set; }
    }
}
