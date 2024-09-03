using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyRecipes
{
    [Table("recipe_categores")]
    public class RecipeCategories
    {
        [Key]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Column("Category")]
        [Required(ErrorMessage = "Recipe category is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Category { get; set; }

        //fields for table JOINS
        //MANY TO MANY -- one recipe can have many categories -- one category can have many recipes
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();
        public bool IsChecked { get; set; } = false;
    }
}
