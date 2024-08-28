using UsefulWebApps.Models.MyRecipes;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        //any Recipe model specific database methods here
    }
}
