using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;

namespace UsefulWebApps.Repository
{
    public class RecipeRepository : Repository<Recipe>, IRecipeRepository
    {
        private readonly MySqlConnection _connection;
        public RecipeRepository(MySqlConnection db) : base(db)
        {
            _connection = db;
        }
        //any Recipe model specific database methods here
    }
}
