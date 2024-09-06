using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Models.ViewModels.MyRecipes;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        //any Recipe model specific database methods here
        //Recipe is very specific no generic repo methods used
        Task<(int count, List<Recipe> recipes)> Pagination(int limit, int offset, string searchString);
        Task<Recipe> GetRecipeById(int? id);
        Task<RecipePageVM> GetRecipeAndCommentsById(int? id);
        Task<(
            List<Recipe> recipe, 
            List<RecipeCategories> recipeCategories, 
            List<RecipeCourses> recipeCourses, 
            List<RecipeCuisines> recipeCuisines, 
            List<RecipeDifficulties> recipeDifficulties
            )> GetRecipeAndCategoriesForEditDisplay(int? id);
        Task<bool> UpdateRecipe(RecipeVM recipeVM);
        Task<(
            List<RecipeCategories> recipeCategories,
            List<RecipeCourses> recipeCourses,
            List<RecipeCuisines> recipeCuisines,
            List<RecipeDifficulties> recipeDifficulties
            )> GetCategoriesForCreateDisplay();

        Task<bool> AddRecipe(RecipeVM recipeVM);
        Task<bool> AddRecipeComment(RecipeComment recipeComment);
        Task<bool> AddUserSavedRecipe(RecipeUserSaved recipeUserSaved);
        Task<bool> DeleteRecipe(int? id);

    }
}
