using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.Models.ViewModels.MyRecipes
{
    public class AddRecipeIngredientToGroceryVM
    {
        public long RecipeId { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Grocery item is required.")]
        public string GroceryItem { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 1, ErrorMessage = "Please enter at least 1 characters.")]
        [Required(ErrorMessage = "Grocery category is required.")]
        public string Category { get; set; } = string.Empty;
    }
}
