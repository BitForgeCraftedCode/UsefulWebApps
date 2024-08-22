using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UsefulWebApps.CustomValidation;

namespace UsefulWebApps.Models.MyRecipes
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Recipe title is required.")]
        public string RecipeTitle { get; set; }

        //allow null empty RecipeDescription
        [ValidateNever]
        [StringLength(200, MinimumLength = 0)]
        public string RecipeDescription { get; set; }

        [ForeignKey("CourseId")]
        public int CourseId { get; set; }

        [ForeignKey("CuisineId")]
        public int CuisineId { get; set; }

        [ForeignKey("DifficultyId")]
        public int DifficultyId { get; set; }

        [Required(ErrorMessage = "Enter a prep time")]
        [Range(0, ushort.MaxValue)]
        public ushort PrepTime { get; set; }

        [Required(ErrorMessage = "Enter a cook time")]
        [Range(0, ushort.MaxValue)]
        public ushort CookTime { get; set; }

        [Required(ErrorMessage = "Enter a number between 1 and 10")]
        [Range(1, 10)]
        public byte Rating { get; set; }

        [Required(ErrorMessage = "Enter a number between 1 and 20")]
        [Range(1, 20)]
        public byte Servings { get; set; }

        //allow null empty Nutrition
        [ValidateNever]
        [StringLength(2000, MinimumLength = 0)]
        public string Nutrition { get; set; }

        [Required(ErrorMessage = "Recipe ingredients is required.")]
        [StringLength(3000, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Recipe instructions is required.")]
        [StringLength(3000, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Instructions { get; set; }

        //allow null empty Notes
        [ValidateNever]
        [StringLength(2000, MinimumLength = 0)]
        public string Notes { get; set; }

        //fields for table JOINS
        //MANY TO MANY -- one recipe can have many categories -- one category can have many recipes
        [Required]
        [RequireRecipeCategoriesList]
        public List<RecipeCategories> Categories { get; set; } = new List<RecipeCategories>();

        //ONE TO MANY -- one recipe can have one course cuisine or difficulty --- one course cuisine or difficulty can have many recipes
        [Required]
        public RecipeCourses Course { get; set; }
        [Required]
        public RecipeCuisines Cuisine { get; set; }
        [Required]
        public RecipeDifficulties Difficulty { get; set; }

    }
}
