using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyRecipes
{
    [Table("recipe_cuisines")]
    public class RecipeCuisines
    {
        [Key]
        [Column("CuisineId")]
        public int CuisineId { get; set; }

        [Column("Cuisine")]
        [Required(ErrorMessage = "Recipe cuisine is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Cuisine { get; set; }

        //fields for table JOINS
        //ONE TO MANY -- one recipe can have one course cuisine or difficulty --- one course cuisine or difficulty can have many recipes
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        public bool IsChecked { get; set; } = false;
    }
}
