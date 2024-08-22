using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.Models.ViewModels.MyRecipes
{
    public class RecipeIndexVM
    {
        public List<Recipe> Recipes { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecipes { get; set; }
        public string SearchString { get; set; }
    }
}
