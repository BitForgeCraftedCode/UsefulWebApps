using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UsefulWebApps.CustomValidation;

namespace UsefulWebApps.Models.MyRecipes
{
    [Table("recipes")]
    public class Recipe
    {
        [Key]
        [Column("RecipeId")]
        public int RecipeId { get; set; }

        [Column("RecipeTitle")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        [Required(ErrorMessage = "Recipe title is required.")]
        public string RecipeTitle { get; set; } = string.Empty;

        [Column("RecipeDescription")]
        //allow null empty RecipeDescription
        [ValidateNever]
        [StringLength(200, MinimumLength = 0)]
        public string RecipeDescription { get; set; } = string.Empty;

        [ForeignKey("CourseId")]
        [Column("CourseId")]
        public int CourseId { get; set; }

        [ForeignKey("CuisineId")]
        [Column("CuisineId")]
        public int CuisineId { get; set; }

        [ForeignKey("DifficultyId")]
        [Column("DifficultyId")]
        public int DifficultyId { get; set; }

        [Column("PrepTime")]
        [Required(ErrorMessage = "Enter a prep time")]
        [Range(0, ushort.MaxValue)]
        public ushort PrepTime { get; set; }

        [Column("CookTime")]
        [Required(ErrorMessage = "Enter a cook time")]
        [Range(0, ushort.MaxValue)]
        public ushort CookTime { get; set; }

        [Column("Rating")]
        [Required(ErrorMessage = "Enter a number between 1 and 10")]
        [Range(1, 10)]
        public byte Rating { get; set; }

        [Column("Servings")]
        [Required(ErrorMessage = "Enter a number between 1 and 20")]
        [Range(1, 20)]
        public byte Servings { get; set; }

        [Column("Nutrition")]
        //allow null empty Nutrition
        [ValidateNever]
        [StringLength(2000, MinimumLength = 0)]
        public string Nutrition { get; set; } = string.Empty;

        [Column("Ingredients")]
        [Required(ErrorMessage = "Recipe ingredients is required.")]
        [StringLength(3000, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Ingredients { get; set; } = string.Empty;

        [Column("Instructions")]
        [Required(ErrorMessage = "Recipe instructions is required.")]
        [StringLength(3000, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
        public string Instructions { get; set; } = string.Empty;

        [Column("Notes")]
        //allow null empty Notes
        [ValidateNever]
        [StringLength(2000, MinimumLength = 0)]
        public string Notes { get; set; } = string.Empty;

        [Column("UserId")]
        [Required(ErrorMessage = "User Id is required.")]
        public string UserId { get; set; } = string.Empty;

        [Column("UserName")]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; } = string.Empty;

        [Column("ImagePath")]
        //allow null empty ImagePath
        [ValidateNever]
        [StringLength(500, MinimumLength = 0)]
        public string ImagePath { get; set; } = string.Empty;

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
