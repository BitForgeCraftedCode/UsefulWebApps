using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.Models.ViewModels.MyRecipes
{
    public class RecipeIndexVM
    {
        public List<Recipe> Recipes { get; set; }
        public List<RecipeCategories> RecipeCategories { get; set; }
        public List<RecipeCourses> RecipeCourses { get; set; }
        public List<RecipeCuisines> RecipeCuisines { get; set; }
        public List<RecipeDifficulties> RecipeDifficulties { get; set; }
        public string CategoriesQuery { get; set; }
        public bool Ascending { get; set; } = true;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecipes { get; set; }
        public string SearchString { get; set; }
    }
}
