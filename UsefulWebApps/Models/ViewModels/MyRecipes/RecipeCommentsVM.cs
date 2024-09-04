using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.Models.ViewModels.MyRecipes
{
    public class RecipeCommentsVM
    {
        public Recipe Recipe { get; set; }

        public List<RecipeComment> RecipeComments { get; set; }
    }
}
