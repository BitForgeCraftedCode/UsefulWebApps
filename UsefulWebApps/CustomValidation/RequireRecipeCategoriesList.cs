using System.ComponentModel.DataAnnotations;
using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.CustomValidation
{
    public class RequireRecipeCategoriesList : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            Recipe recipe = (Recipe)validationContext.ObjectInstance;
            foreach (RecipeCategories category in recipe.Categories)
            {
                if(category.IsChecked == true)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Please check at least one recipe tag.");
        }
    }
}
