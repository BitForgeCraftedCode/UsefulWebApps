using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;
using static Dapper.SqlMapper;

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
        public async Task<(int count, List<Recipe> recipes)> Pagination(int limit, int offset, string searchString)
        {
            /* Basic limit offset pagination
            * 
            * The way that the OFFSET keyword works is that it discards the first n rows from the result set. 
            * It doesn't simply skip over them. Instead, it reads the rows and then discards them. 
            * This means that as you work into deeper and deeper pages of your result set, 
            * the performance of your query will degrade
            * 
            * use deferred join in mysql for more efficient pagination of large data
            * 
            * limit = page size
            * offset = page size * page - 1
            * 
            * https://planetscale.com/learn/courses/mysql-for-developers/indexes/fulltext-indexes?autoplay=1
            */

            string sqlMult = @"
                SELECT COUNT(*) FROM recipes;
                SELECT * FROM recipes ORDER BY RecipeId, RecipeTitle LIMIT @Limit OFFSET @Offset;
            ";
            string sqlMultFilter = @"
                SELECT COUNT(*) FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST(@SearchString) ORDER BY RecipeId, RecipeTitle;
                SELECT * FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST(@SearchString) ORDER BY RecipeId, RecipeTitle LIMIT @Limit OFFSET @Offset;
            ";
            GridReader gridReader = null;
            if (String.IsNullOrEmpty(searchString))
            {
                gridReader = await _connection.QueryMultipleAsync(sqlMult, new { Limit = limit, Offset = offset });
            }
            else
            {
                gridReader = await _connection.QueryMultipleAsync(sqlMultFilter, new { SearchString = searchString, Limit = limit, Offset = offset });
            }

            //count is the total number of recipes in database
            int count = await gridReader.ReadFirstAsync<int>();
            List<Recipe> recipes = (List<Recipe>)await gridReader.ReadAsync<Recipe>();
            return (count, recipes);
        }
    }
}
