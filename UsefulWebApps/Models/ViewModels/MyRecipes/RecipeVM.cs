using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.Models.ViewModels.MyRecipes
{
    public class RecipeVM
    {
        public Recipe Recipe { get; set; }
        public List<RecipeCategories> RecipeCategories { get; set; }
        public List<RecipeCourses> RecipeCourses { get; set; }
        public List<RecipeCuisines> RecipeCuisines { get; set; }
        public List<RecipeDifficulties> RecipeDifficulties { get; set; }


    }
}
