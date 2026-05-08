using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.ListBuddy
{
    [Table("grocery_list_items")]
    public class GroceryListItems
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("ListId")]
        [Required]
        public long ListId { get; set; }

        [Column("GroceryItem")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Item must have at least 3 character.")]
        [Required(ErrorMessage = "Grocery item is required.")]
        public string GroceryItem { get; set; } = string.Empty;

        [Column("Category")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Category must have at least 3 character.")]
        [Required(ErrorMessage = "Grocery category is required.")]
        public string Category { get; set; } = string.Empty;

        [Column("Complete")]
        public bool Complete { get; set; } = false;

        [Column("SortOrder")]
        public int SortOrder { get; set; } = 0;

        [NotMapped]
        public int ListVersion { get; set; } = 0;
    }
}
