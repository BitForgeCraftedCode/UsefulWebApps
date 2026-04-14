using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Models.ViewModels.MyRecipes;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        //any Recipe model specific database methods here
        //Recipe is very specific no generic repo methods used
        Task<(int count, List<Recipe> recipes)> Pagination(int limit, int offset, string searchString, bool ascending);
        Task<(int count, List<Recipe> recipes)> PaginationWithTagFilter(int limit, int offset, List<long> selectedCategoryIds, bool ascending);
        Task<Recipe> GetRecipeById(long? id);
        Task<RecipePageVM> GetRecipeAndCommentsById(long? id);
        Task<List<RecipeUserSaved>> GetUserSavedRecipes(string userId);
        Task<(
            List<Recipe> recipe, 
            List<RecipeCategories> recipeCategories, 
            List<RecipeCourses> recipeCourses, 
            List<RecipeCuisines> recipeCuisines, 
            List<RecipeDifficulties> recipeDifficulties
            )> GetRecipeAndCategoriesForEditDisplay(long? id);
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
        Task<bool> DeleteRecipe(long? id);
        Task<bool> DeleteUserSavedRecipe(long? id);

    }
}
