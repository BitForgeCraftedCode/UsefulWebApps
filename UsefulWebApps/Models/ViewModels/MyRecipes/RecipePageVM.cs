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
        //for link memory
        public int ReturnPage { get; set; }
        public string ReturnSearchString { get; set; }
        public string ReturnCategories { get; set; }
        public bool ReturnAscending { get; set; }
        //add recipe ingredient to grocery list
        public AddRecipeIngredientToGroceryVM AddIngredientToGrocery { get; set; }

        public IEnumerable<SelectListItem> GroceryCategoriesList { get; set; }

        public IEnumerable<SelectListItem> AllMyGroceryLists { get; set; }
    }
}
