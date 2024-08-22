using System.ComponentModel.DataAnnotations;

namespace UsefulWebApps.Models.MyRecipes
{
    public class RecipeCourses
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Recipe course is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Course { get; set; }

        //fields for table JOINS
        //ONE TO MANY -- one recipe can have one course cuisine or difficulty --- one course cuisine or difficulty can have many recipes
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        public bool IsChecked { get; set; } = false;
    }
}
