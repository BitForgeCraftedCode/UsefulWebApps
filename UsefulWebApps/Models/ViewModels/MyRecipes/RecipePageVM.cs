using Microsoft.AspNetCore.Mvc.Rendering;
using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.Models.ViewModels.MyRecipes
{
    public class RecipePageVM
    {
        public Recipe Recipe { get; set; }

        public List<RecipeComment> RecipeComments { get; set; }

        public RecipeComment RecipeComment { get; set; }

        public RecipeUserSaved RecipeUserSaved { get; set; }
        //add recipe ingredient to grocery list
        public AddRecipeIngredientToGroceryVM AddIngredientToGrocery { get; set; }

        public IEnumerable<SelectListItem> GroceryCategoriesList { get; set; }
    }
}
