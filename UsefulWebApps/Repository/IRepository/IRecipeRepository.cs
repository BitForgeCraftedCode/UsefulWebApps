using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Models.ViewModels.MyRecipes;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        //any Recipe model specific database methods here
        Task<(int count, List<Recipe> recipes)> Pagination(int limit, int offset, string searchString);
        Task<List<Recipe>> GetRecipeById(int? id);
        Task<(
            List<Recipe> recipe, 
            List<RecipeCategories> recipeCategories, 
            List<RecipeCourses> recipeCourses, 
            List<RecipeCuisines> recipeCuisines, 
            List<RecipeDifficulties> recipeDifficulties
            )> GetRecipeAndCategoriesForEditDisplay(int? id);
        Task<bool> UpdateRecipe(RecipeVM recipeVM, List<Object> checkedCategoriesParams);

    }
}
