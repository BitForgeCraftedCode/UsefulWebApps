using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.Models.MyRecipes
{
    public class RecipeDifficulties
    {
        public int DifficultyId { get; set; }

        [Required(ErrorMessage = "Recipe difficulty is required.")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Difficulty { get; set; }

        //fields for table JOINS
        //ONE TO MANY -- one recipe can have one course cuisine or difficulty --- one course cuisine or difficulty can have many recipes
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        public bool IsChecked { get; set; } = false;
    }
}
