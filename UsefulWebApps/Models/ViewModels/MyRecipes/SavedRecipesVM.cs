using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.Models.ViewModels.MyRecipes
{
    public class SavedRecipesVM
    {
        public List<RecipeUserSaved> RecipeUserSaved { get; set; }
        //for link memory
        public int ReturnPage { get; set; }
        public string ReturnSearchString { get; set; }
        public string ReturnCategories { get; set; }
        public bool ReturnAscending { get; set; }
    }
}
