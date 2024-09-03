using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyRecipes
{
    [Table("recipe_difficulties")]
    public class RecipeDifficulties
    {
        [Key]
        [Column("DifficultyId")]
        public int DifficultyId { get; set; }

        [Column("Difficulty")]
        [Required(ErrorMessage = "Recipe difficulty is required.")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Difficulty { get; set; }

        //fields for table JOINS
        //ONE TO MANY -- one recipe can have one course cuisine or difficulty --- one course cuisine or difficulty can have many recipes
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        public bool IsChecked { get; set; } = false;
    }
}
