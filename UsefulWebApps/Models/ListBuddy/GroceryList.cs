using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("grocery_list")]
    public class GroceryList
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("GroceryItem")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Grocery item is required.")]
        public string GroceryItem { get; set; } = string.Empty;

        [Column("Category")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Grocery category is required.")]
        public string Category { get; set; } = string.Empty;

        [Column("Complete")]
        [Required]
        public bool Complete { get; set; }

        [Column("UserId")]
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
