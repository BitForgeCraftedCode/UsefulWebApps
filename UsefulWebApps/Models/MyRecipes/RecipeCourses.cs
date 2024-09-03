using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsefulWebApps.Models.MyRecipes
{
    [Table("recipe_courses")]
    public class RecipeCourses
    {
        [Key]
        [Column("CourseId")]
        public int CourseId { get; set; }

        [Column("Course")]
        [Required(ErrorMessage = "Recipe course is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Course { get; set; }

        //fields for table JOINS
        //ONE TO MANY -- one recipe can have one course cuisine or difficulty --- one course cuisine or difficulty can have many recipes
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        public bool IsChecked { get; set; } = false;
    }
}
